#![warn(clippy::all)]

use actix_web::HttpResponse;
use actix_web::{error, get, middleware, web, App, HttpServer, Responder};
use diesel::{r2d2, PgConnection};
use serde::Deserialize;

use crate::path_finder_service::PathResponse;

mod astar;
mod dal;
mod graph_cache;
mod path_finder_service;

/// Short-hand for the database pool type to use throughout the app.
type DbPool = r2d2::Pool<r2d2::ConnectionManager<PgConnection>>;
type PathResponseResult = Result<PathResponse, Box<dyn std::error::Error + Send + Sync>>;

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    dotenv::dotenv().ok();
    env_logger::init_from_env(env_logger::Env::new().default_filter_or("info"));

    // initialize DB pool outside of `HttpServer::new` so that it is shared across all workers
    let pool = initialize_db_pool();

    log::info!("starting HTTP server at http://localhost:8080");

    HttpServer::new(move || {
        App::new()
            // add DB pool handle to app data; enables use of `web::Data<DbPool>` extractor
            .app_data(web::Data::new(pool.clone()))
            // add request logger middleware
            .wrap(middleware::Logger::default())
            // add route handlers
            .service(get_words_path)
    })
    .bind(("127.0.0.1", 8080))?
    .run()
    .await
}

/// Initialize database connection pool based on `DATABASE_URL` environment variable.
///
/// See more: <https://docs.rs/diesel/latest/diesel/r2d2/index.html>.
fn initialize_db_pool() -> DbPool {
    let conn_spec = std::env::var("DATABASE_URL").expect("DATABASE_URL should be set");
    let manager = r2d2::ConnectionManager::<PgConnection>::new(conn_spec);
    r2d2::Pool::builder()
        .build(manager)
        .expect("database URL should be valid path to PostgreSQL database")
}

#[derive(Debug, Deserialize)]
pub struct PathRequest {
    from: String,
    to: String,
}

#[get("/api/wordsPath")]
async fn get_words_path(
    req: web::Query<PathRequest>,
    pool: web::Data<DbPool>,
) -> actix_web::Result<impl Responder> {
    let from = req.from.clone();
    let to = req.to.clone();
    log::info!("Request from {} to {}", from, to);
    if from.is_empty() || to.is_empty() {
        return Err(error::ErrorBadRequest(
            "'from' and 'to' query parameters are required",
        ));
    }
    if from.chars().count() != to.chars().count() {
        return Err(error::ErrorBadRequest(
            "'from' and 'to' query parameters must be of same length",
        ));
    }

    let pool_clone = pool.clone();
    let db_path = web::block(move || {
        let mut conn = pool_clone.get()?;
        path_finder_service::get_path_from_db(&from, &to, &mut conn)
    })
    .await?
    .map_err(|e| error::ErrorInternalServerError(format!("failed to get graph from DB: {}", e)))?;

    if db_path.is_some() {
        return Ok(HttpResponse::Ok().json(db_path));
    }

    let path = path_finder_service::find_shortest_path_from_cache(req.from.clone(), req.to.clone())
        .await
        .map_err(|e| error::ErrorInternalServerError(format!("failed to find graph: {}", e)))?;

    if let Some(p) = path {
        let pp = web::block(move || {
            let mut conn = pool.get()?;
            path_finder_service::save_path_to_db(req.from.clone(), req.to.clone(), &p, &mut conn)?;
            PathResponseResult::Ok(p)
        })
        .await?
        .map_err(|e| {
            error::ErrorInternalServerError(format!("failed to save graph to DB: {}", e))
        })?;

        return Ok(HttpResponse::Ok().json(Some(pp)));
    }

    Ok(HttpResponse::NotFound().finish())
}
