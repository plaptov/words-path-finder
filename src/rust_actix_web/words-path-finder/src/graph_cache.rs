use super::astar::words_graph::WordsGraph;
use dashmap::DashMap;
use error_chain::error_chain;
use once_cell::sync::Lazy;

error_chain! {
    foreign_links {
        Io(std::io::Error);
        HttpRequest(reqwest::Error);
    }
}

static mut SHARED_MAP: Lazy<DashMap<usize, &'static WordsGraph>> = Lazy::new(DashMap::new);

async fn download_lines(id: usize) -> Result<Vec<String>> {
    let url = format! {"https://raw.githubusercontent.com/plaptov/words-path-finder/main/dict/{}.txt", id};
    let res = reqwest::get(url).await?;
    let body = res.text().await?;
    let lines = body
        .split(&"\r\n")
        .map(|s| s.to_owned())
        .collect::<Vec<_>>();
    Ok(lines)
}

pub async fn get_graph(id: usize) -> Result<&'static WordsGraph> {
    unsafe {
        if let Some(graph_ref) = SHARED_MAP.get(&id) {
            return Ok(graph_ref.value());
        }
    }
    let lines = download_lines(id).await?;
    unsafe {
        Ok(SHARED_MAP
            .entry(id)
            .or_insert_with(|| {
                let graph = WordsGraph::generate(&lines);
                Box::leak(Box::new(graph))
            })
            .value())
    }
}
