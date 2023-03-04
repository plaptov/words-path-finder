﻿// <auto-generated />
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WordsPathFinder.DAL;

#nullable disable

namespace WordsPathFinder.DAL.Migrations
{
    [DbContext(typeof(WordsPathFinderContext))]
    [Migration("20230304204931_InitMigration")]
    partial class InitMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WordsPathFinder.DAL.WordsPathModel", b =>
                {
                    b.Property<string>("From")
                        .HasColumnType("text");

                    b.Property<string>("To")
                        .HasColumnType("text");

                    b.Property<List<string>>("Steps")
                        .HasColumnType("text[]");

                    b.HasKey("From", "To");

                    b.ToTable("WordsPaths");
                });
#pragma warning restore 612, 618
        }
    }
}