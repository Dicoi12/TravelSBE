﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TravelSBE.Data;

#nullable disable

namespace TravelSBE.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TravelSBE.Entity.Answer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("IdQuestion")
                        .HasColumnType("integer");

                    b.Property<int>("IdUser")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("IdQuestion");

                    b.HasIndex("IdUser");

                    b.ToTable("Answers");
                });

            modelBuilder.Entity("TravelSBE.Entity.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("IdObjective")
                        .HasColumnType("integer");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("IdObjective");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("TravelSBE.Entity.Itinerary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("IdUser")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Itineraries");
                });

            modelBuilder.Entity("TravelSBE.Entity.ItineraryDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Descriere")
                        .HasColumnType("text");

                    b.Property<int?>("IdEvent")
                        .HasColumnType("integer");

                    b.Property<int>("IdItinerary")
                        .HasColumnType("integer");

                    b.Property<int?>("IdObjective")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("VisitOrder")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("IdEvent");

                    b.HasIndex("IdObjective");

                    b.ToTable("ItineraryDetails");
                });

            modelBuilder.Entity("TravelSBE.Entity.Objective", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Interval")
                        .HasColumnType("text");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Pret")
                        .HasColumnType("text");

                    b.Property<int?>("Type")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Website")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Type");

                    b.ToTable("Objectives");
                });

            modelBuilder.Entity("TravelSBE.Entity.ObjectiveImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("ExperienceId")
                        .HasColumnType("integer");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("IdEvent")
                        .HasColumnType("integer");

                    b.Property<int?>("IdExperienta")
                        .HasColumnType("integer");

                    b.Property<int?>("IdObjective")
                        .HasColumnType("integer");

                    b.Property<string>("ImageMimeType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ExperienceId");

                    b.HasIndex("IdEvent");

                    b.HasIndex("IdObjective");

                    b.ToTable("ObjectiveImages");
                });

            modelBuilder.Entity("TravelSBE.Entity.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DatePosted")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("IdEvent")
                        .HasColumnType("integer");

                    b.Property<int?>("IdObjective")
                        .HasColumnType("integer");

                    b.Property<int>("IdUser")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("IdEvent");

                    b.HasIndex("IdObjective");

                    b.HasIndex("IdUser");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("TravelSBE.Entity.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DatePosted")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("IdObjective")
                        .HasColumnType("integer");

                    b.Property<int>("IdUser")
                        .HasColumnType("integer");

                    b.Property<int>("Raiting")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("TravelSBE.Entity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Phone")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TravelsBE.Entity.Experience", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("City")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Country")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean");

                    b.Property<double>("Latitude")
                        .HasColumnType("double precision");

                    b.Property<string>("LocationName")
                        .HasColumnType("text");

                    b.Property<double>("Longitude")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int?>("Rating")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Experiences");
                });

            modelBuilder.Entity("TravelsBE.Entity.ObjectiveSchedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<TimeSpan>("CloseTime")
                        .HasColumnType("interval");

                    b.Property<string>("DayOfWeek")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("EntryPrice")
                        .HasColumnType("numeric");

                    b.Property<int>("ObjectiveId")
                        .HasColumnType("integer");

                    b.Property<TimeSpan>("OpenTime")
                        .HasColumnType("interval");

                    b.HasKey("Id");

                    b.HasIndex("ObjectiveId");

                    b.ToTable("ObjectiveSchedules");
                });

            modelBuilder.Entity("TravelsBE.Entity.ObjectiveType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("ObjectiveTypes");
                });

            modelBuilder.Entity("TravelSBE.Entity.Answer", b =>
                {
                    b.HasOne("TravelSBE.Entity.Question", "Question")
                        .WithMany()
                        .HasForeignKey("IdQuestion")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TravelSBE.Entity.User", "User")
                        .WithMany()
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TravelSBE.Entity.Event", b =>
                {
                    b.HasOne("TravelSBE.Entity.Objective", "Objective")
                        .WithMany()
                        .HasForeignKey("IdObjective");

                    b.Navigation("Objective");
                });

            modelBuilder.Entity("TravelSBE.Entity.Itinerary", b =>
                {
                    b.HasOne("TravelSBE.Entity.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TravelSBE.Entity.ItineraryDetail", b =>
                {
                    b.HasOne("TravelSBE.Entity.Itinerary", "Itinerary")
                        .WithMany("ItineraryDetails")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TravelSBE.Entity.Event", "Event")
                        .WithMany()
                        .HasForeignKey("IdEvent");

                    b.HasOne("TravelSBE.Entity.Objective", "Objective")
                        .WithMany()
                        .HasForeignKey("IdObjective");

                    b.Navigation("Event");

                    b.Navigation("Itinerary");

                    b.Navigation("Objective");
                });

            modelBuilder.Entity("TravelSBE.Entity.Objective", b =>
                {
                    b.HasOne("TravelsBE.Entity.ObjectiveType", "ObjectiveType")
                        .WithMany()
                        .HasForeignKey("Type");

                    b.Navigation("ObjectiveType");
                });

            modelBuilder.Entity("TravelSBE.Entity.ObjectiveImage", b =>
                {
                    b.HasOne("TravelsBE.Entity.Experience", "Experience")
                        .WithMany("Images")
                        .HasForeignKey("ExperienceId");

                    b.HasOne("TravelSBE.Entity.Event", "Event")
                        .WithMany("Images")
                        .HasForeignKey("IdEvent");

                    b.HasOne("TravelSBE.Entity.Objective", "Objective")
                        .WithMany("Images")
                        .HasForeignKey("IdObjective");

                    b.Navigation("Event");

                    b.Navigation("Experience");

                    b.Navigation("Objective");
                });

            modelBuilder.Entity("TravelSBE.Entity.Question", b =>
                {
                    b.HasOne("TravelSBE.Entity.Event", "Event")
                        .WithMany()
                        .HasForeignKey("IdEvent");

                    b.HasOne("TravelSBE.Entity.Objective", "Objective")
                        .WithMany()
                        .HasForeignKey("IdObjective");

                    b.HasOne("TravelSBE.Entity.User", "User")
                        .WithMany()
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Objective");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TravelSBE.Entity.Review", b =>
                {
                    b.HasOne("TravelSBE.Entity.Objective", "Objective")
                        .WithMany("Reviews")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TravelSBE.Entity.User", "User")
                        .WithMany()
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Objective");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TravelsBE.Entity.ObjectiveSchedule", b =>
                {
                    b.HasOne("TravelSBE.Entity.Objective", "Objective")
                        .WithMany()
                        .HasForeignKey("ObjectiveId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Objective");
                });

            modelBuilder.Entity("TravelSBE.Entity.Event", b =>
                {
                    b.Navigation("Images");
                });

            modelBuilder.Entity("TravelSBE.Entity.Itinerary", b =>
                {
                    b.Navigation("ItineraryDetails");
                });

            modelBuilder.Entity("TravelSBE.Entity.Objective", b =>
                {
                    b.Navigation("Images");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("TravelsBE.Entity.Experience", b =>
                {
                    b.Navigation("Images");
                });
#pragma warning restore 612, 618
        }
    }
}
