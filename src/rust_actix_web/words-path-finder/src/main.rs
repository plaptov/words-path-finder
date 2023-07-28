#![warn(clippy::all)]

use actix_web::HttpResponse;
use actix_web::{error, get, middleware, web, App, HttpServer, Responder};
use serde::Deserialize;

mod astar;
mod graph_cache;
mod path_finder_service;

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    dotenv::dotenv().ok();
    env_logger::init_from_env(env_logger::Env::new().default_filter_or("info"));

    // initialize DB pool outside of `HttpServer::new` so that it is shared across all workers
    //let pool = initialize_db_pool();

    log::info!("starting HTTP server at http://localhost:8080");

    HttpServer::new(move || {
        App::new()
            // add DB pool handle to app data; enables use of `web::Data<DbPool>` extractor
            //.app_data(web::Data::new(pool.clone()))
            // add request logger middleware
            .wrap(middleware::Logger::default())
            // add route handlers
            .service(get_words_path)
    })
    .bind(("127.0.0.1", 8080))?
    .run()
    .await
}

#[derive(Debug, Deserialize)]
pub struct PathRequest {
    from: String,
    to: String,
}

#[get("/api/wordsPath")]
async fn get_words_path(req: web::Query<PathRequest>) -> actix_web::Result<impl Responder> {
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

    let path = path_finder_service::find_shortest_path_from_cache(from, to)
        .await
        .map_err(|e| error::ErrorInternalServerError(format!("failed to get graph: {}", e)))?;
    Ok(HttpResponse::Ok().json(path))
}
