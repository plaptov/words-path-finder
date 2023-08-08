use diesel::prelude::*;

#[derive(Queryable, Selectable, Insertable)]
#[diesel(table_name = crate::dal::schema::words_paths)]
#[diesel(check_for_backend(diesel::pg::Pg))]
pub struct WordsPath {
    pub from: String,
    pub to: String,
    pub steps: Option<Vec<Option<String>>>,
}
