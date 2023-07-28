use super::word::Word;

pub struct Path<'a> {
    pub priority: usize,
    pub words: Vec<&'a Word>,
}

impl<'a> Path<'a> {
    pub fn new(prev_path: Option<&Path<'a>>, new_word: &'a Word, finish: &'a Word) -> Path<'a> {
        let mut words = match prev_path {
            Some(prev) => Vec::from_iter(prev.words.iter().copied()),
            None => Vec::new(),
        };
        words.push(new_word);
        let priority = words.len() + new_word.distance_to(finish);
        Path { words, priority }
    }

    pub fn len(&self) -> usize {
        self.words.len()
    }

    pub fn last(&self) -> &&Word {
        self.words.last().unwrap()
    }
}
