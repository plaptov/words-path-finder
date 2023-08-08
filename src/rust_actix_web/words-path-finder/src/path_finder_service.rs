use crate::dal::models;
use crate::dal::schema::words_paths::dsl::*;
use crate::{astar::word::Word, dal::schema::words_paths, graph_cache};
use diesel::{prelude::*, PgConnection};
use serde::Serialize;

type DbError = Box<dyn std::error::Error + Send + Sync>;

#[derive(Serialize)]
pub struct PathResponse {
    pub length: usize,
    pub steps: Vec<String>,
}

pub async fn find_shortest_path_from_cache(
    from_str: String,
    to_str: String,
) -> graph_cache::Result<Option<PathResponse>> {
    let graph = graph_cache::get_graph(from_str.chars().count()).await?;
    let start = Word::new(from_str);
    let finish = Word::new(to_str);
    let path = graph
        .find_shortest_path(&start, &finish)
        .map(|p| PathResponse {
            length: p.len(),
            steps: p.words.iter().map(|w| w.text.clone()).collect(),
        });
    Ok(path)
}

pub fn get_path_from_db(
    from_str: &str,
    to_str: &str,
    connection: &mut PgConnection,
) -> Result<Option<PathResponse>, DbError> {
    let db_path = words_paths::dsl::words_paths
        .filter(from.eq(from_str))
        .filter(to.eq(to_str))
        .first::<models::WordsPath>(connection)
        .optional()?;

    Ok(match db_path {
        Some(path) => path.steps.map(|mut s| PathResponse {
            length: s.len(),
            steps: s.drain(..).map(|x| x.unwrap()).collect(),
        }),
        None => None,
    })
}

pub fn save_path_to_db(
    from_str: String,
    to_str: String,
    path: &PathResponse,
    connection: &mut PgConnection,
) -> Result<(), DbError> {
    let new_path = models::WordsPath {
        from: from_str,
        to: to_str,
        steps: Some(path.steps.iter().map(|s| Some(s.to_string())).collect()),
    };

    diesel::insert_into(words_paths::dsl::words_paths)
        .values(&new_path)
        .execute(connection)?;

    Ok(())
}
