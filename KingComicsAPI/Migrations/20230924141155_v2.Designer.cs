﻿// <auto-generated />
using System;
using KingComicsAPI.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KingComicsAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230924141155_v2")]
    partial class v2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("KingComicsAPI.Models.Admin", b =>
                {
                    b.Property<Guid>("Admin_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Admin_id");

                    b.ToTable("admin", (string)null);
                });

            modelBuilder.Entity("KingComicsAPI.Models.Chapter", b =>
                {
                    b.Property<Guid>("Chapter_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Arrange")
                        .HasColumnType("integer");

                    b.Property<Guid>("Comic_id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Slug")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Views")
                        .HasColumnType("integer");

                    b.HasKey("Chapter_id");

                    b.HasIndex("Comic_id");

                    b.ToTable("chapter", (string)null);
                });

            modelBuilder.Entity("KingComicsAPI.Models.Comic", b =>
                {
                    b.Property<Guid>("Comic_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CoverImage")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Slug")
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Comic_id");

                    b.HasIndex("Title")
                        .IsUnique();

                    b.ToTable("comic", (string)null);
                });

            modelBuilder.Entity("KingComicsAPI.Models.Comic_Genre", b =>
                {
                    b.Property<Guid>("Comic_id")
                        .HasColumnType("uuid");

                    b.Property<int>("Genre_id")
                        .HasColumnType("integer");

                    b.HasKey("Comic_id", "Genre_id");

                    b.HasIndex("Genre_id");

                    b.ToTable("comic_genre", (string)null);
                });

            modelBuilder.Entity("KingComicsAPI.Models.FollowComic", b =>
                {
                    b.Property<Guid>("User_id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("Comic_id")
                        .HasColumnType("uuid");

                    b.HasKey("User_id", "Comic_id");

                    b.HasIndex("Comic_id");

                    b.ToTable("follow_comic", (string)null);
                });

            modelBuilder.Entity("KingComicsAPI.Models.Genre", b =>
                {
                    b.Property<int>("Genre_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Genre_id"));

                    b.Property<string>("Genre_Name")
                        .HasColumnType("text");

                    b.Property<string>("Slug")
                        .HasColumnType("text");

                    b.Property<int>("status")
                        .HasColumnType("integer");

                    b.HasKey("Genre_id");

                    b.ToTable("genre", (string)null);
                });

            modelBuilder.Entity("KingComicsAPI.Models.Image", b =>
                {
                    b.Property<Guid>("Image_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Arrange")
                        .HasColumnType("integer");

                    b.Property<Guid>("Chapter_id")
                        .HasColumnType("uuid");

                    b.Property<string>("UrlImage")
                        .HasColumnType("text");

                    b.HasKey("Image_id");

                    b.HasIndex("Chapter_id");

                    b.ToTable("image", (string)null);
                });

            modelBuilder.Entity("KingComicsAPI.Models.ReadingHistory", b =>
                {
                    b.Property<Guid>("User_id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("Comic_id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("Chapter_id")
                        .HasColumnType("uuid");

                    b.HasKey("User_id", "Comic_id", "Chapter_id");

                    b.HasIndex("Chapter_id");

                    b.HasIndex("Comic_id");

                    b.ToTable("reading_history", (string)null);
                });

            modelBuilder.Entity("KingComicsAPI.Models.User", b =>
                {
                    b.Property<Guid>("User_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Avatar")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("NickName")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("User_id");

                    b.ToTable("user", (string)null);
                });

            modelBuilder.Entity("KingComicsAPI.Models.Chapter", b =>
                {
                    b.HasOne("KingComicsAPI.Models.Comic", "Comic")
                        .WithMany("Chapters")
                        .HasForeignKey("Comic_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comic");
                });

            modelBuilder.Entity("KingComicsAPI.Models.Comic_Genre", b =>
                {
                    b.HasOne("KingComicsAPI.Models.Comic", "Comic")
                        .WithMany("ComicGenres")
                        .HasForeignKey("Comic_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KingComicsAPI.Models.Genre", "Genre")
                        .WithMany("ComicGenres")
                        .HasForeignKey("Genre_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comic");

                    b.Navigation("Genre");
                });

            modelBuilder.Entity("KingComicsAPI.Models.FollowComic", b =>
                {
                    b.HasOne("KingComicsAPI.Models.Comic", "Comic")
                        .WithMany("FollowComics")
                        .HasForeignKey("Comic_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KingComicsAPI.Models.User", "User")
                        .WithMany("FollowComics")
                        .HasForeignKey("User_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comic");

                    b.Navigation("User");
                });

            modelBuilder.Entity("KingComicsAPI.Models.Image", b =>
                {
                    b.HasOne("KingComicsAPI.Models.Chapter", "Chapter")
                        .WithMany("Images")
                        .HasForeignKey("Chapter_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chapter");
                });

            modelBuilder.Entity("KingComicsAPI.Models.ReadingHistory", b =>
                {
                    b.HasOne("KingComicsAPI.Models.Chapter", "Chapter")
                        .WithMany("ReadingHistories")
                        .HasForeignKey("Chapter_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KingComicsAPI.Models.Comic", "Comic")
                        .WithMany("ReadingHistories")
                        .HasForeignKey("Comic_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KingComicsAPI.Models.User", "User")
                        .WithMany("ReadingHistories")
                        .HasForeignKey("User_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chapter");

                    b.Navigation("Comic");

                    b.Navigation("User");
                });

            modelBuilder.Entity("KingComicsAPI.Models.Chapter", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("ReadingHistories");
                });

            modelBuilder.Entity("KingComicsAPI.Models.Comic", b =>
                {
                    b.Navigation("Chapters");

                    b.Navigation("ComicGenres");

                    b.Navigation("FollowComics");

                    b.Navigation("ReadingHistories");
                });

            modelBuilder.Entity("KingComicsAPI.Models.Genre", b =>
                {
                    b.Navigation("ComicGenres");
                });

            modelBuilder.Entity("KingComicsAPI.Models.User", b =>
                {
                    b.Navigation("FollowComics");

                    b.Navigation("ReadingHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
