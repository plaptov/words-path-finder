use std::collections::{HashMap, HashSet, VecDeque};

use super::{path::Path, word::Word, words_graph::WordsGraph};

impl WordsGraph {
    pub fn find_shortest_path<'a>(&'a self, start: &'a Word, finish: &'a Word) -> Option<Path> {
        if !self.contains(start) {
            return None;
        }

        let mut paths = HashMap::new();
        let cur_path = Path::new(None, start, finish);
        let mut min_priority = cur_path.priority;
        paths.insert(cur_path.priority, VecDeque::from(vec![cur_path]));
        let mut used_words = HashSet::new();
        used_words.insert(start);
        loop {
            if paths.is_empty() {
                return None;
            }
            let queue = paths.get_mut(&min_priority).unwrap();
            let cur_path = queue.pop_front().unwrap();
            if cur_path.last() == &finish {
                return Some(cur_path);
            }
            if queue.is_empty() {
                paths.remove(&min_priority);
            }
            if let Some(steps) = self.get(cur_path.last()) {
                for word in steps {
                    if used_words.insert(word) {
                        let new_path = Path::new(Some(&cur_path), word, finish);
                        paths
                            .entry(new_path.priority)
                            .or_default()
                            .push_back(new_path);
                    }
                }
            }
            min_priority = paths.keys().copied().min().unwrap_or_default();
        }
    }
}
