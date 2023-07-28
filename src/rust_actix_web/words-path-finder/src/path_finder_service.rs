use crate::{
    astar::word::Word,
    graph_cache::{self, Result},
};
use serde::Serialize;

#[derive(Serialize)]
pub struct PathResponse {
    pub length: usize,
    pub steps: Vec<String>,
}

pub async fn find_shortest_path_from_cache(
    from: String,
    to: String,
) -> Result<Option<PathResponse>> {
    let graph = graph_cache::get_graph(from.chars().count()).await?;
    let start = Word::new(from);
    let finish = Word::new(to);
    let path = graph
        .find_shortest_path(&start, &finish)
        .map(|p| PathResponse {
            length: p.len(),
            steps: p.words.iter().map(|w| w.text.clone()).collect(),
        });
    Ok(path)
}
