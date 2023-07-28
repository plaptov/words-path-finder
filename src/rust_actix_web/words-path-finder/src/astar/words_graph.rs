use std::collections::HashMap;

use super::word::Word;

pub struct WordsGraph {
    dict: HashMap<Word, Vec<Word>>,
}

impl WordsGraph {
    pub fn generate(strings: &[String]) -> WordsGraph {
        let mut dict = HashMap::new();
        let words = strings.iter().cloned().map(Word::new).collect::<Vec<_>>();
        words.iter().enumerate().for_each(|(i, word)| {
            let mut steps = words[i + 1..]
                .iter()
                .filter(|w| word.is_only_one_letter_diff_with(w))
                .cloned()
                .collect::<Vec<_>>();
            let vec = dict.entry(word.clone()).or_insert_with(Vec::new);
            steps.iter().for_each(|w| vec.push(w.clone()));
            steps
                .drain(..)
                .for_each(|w| dict.entry(w).or_insert_with(Vec::new).push(word.clone()));
        });
        Self { dict }
    }

    pub fn get(&self, word: &Word) -> Option<&Vec<Word>> {
        self.dict.get(word)
    }

    pub fn contains(&self, word: &Word) -> bool {
        self.dict.contains_key(word)
    }
}
