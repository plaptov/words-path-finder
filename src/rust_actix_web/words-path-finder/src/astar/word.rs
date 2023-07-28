extern crate derive_more;
// use the derives that you want in the file
use derive_more::Display;

#[derive(Eq, PartialEq, Debug, Hash, Clone, Display)]
pub struct Word {
    pub text: String,
}

impl Word {
    pub fn new(text: String) -> Self {
        Self { text }
    }

    pub fn distance_to(&self, other: &Self) -> usize {
        self.text
            .chars()
            .zip(other.text.chars())
            .filter(|(c1, c2)| c1 != c2)
            .count()
    }

    pub fn is_only_one_letter_diff_with(&self, other: &Self) -> bool {
        self.text
            .chars()
            .zip(other.text.chars())
            .filter(|(c1, c2)| c1 != c2)
            .take(2)
            .count()
            == 1
    }
}
