// @generated automatically by Diesel CLI.

diesel::table! {
    words_paths (from, to) {
        from -> Text,
        to -> Text,
        steps -> Nullable<Array<Nullable<Text>>>,
    }
}
