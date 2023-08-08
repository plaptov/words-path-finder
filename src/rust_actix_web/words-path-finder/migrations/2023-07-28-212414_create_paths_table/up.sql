-- Your SQL goes here
CREATE TABLE "words_paths"(
	"from" TEXT NOT NULL,
	"to" TEXT NOT NULL,
	"steps" TEXT[],
	PRIMARY KEY("from", "to")
);

