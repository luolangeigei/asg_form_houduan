﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using asg_form.Controllers;

#nullable disable

namespace asg_form.Migrations
{
    [DbContext(typeof(IDBcontext))]
    [Migration("20230917101052_pg")]
    partial class pg
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<long>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<long>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<long>", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<long>", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("asg_form.Controllers.Champion+T_Champion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("eventsId")
                        .HasColumnType("integer");

                    b.Property<long>("formId")
                        .HasColumnType("bigint");

                    b.Property<string>("msg")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("eventsId");

                    b.HasIndex("formId");

                    b.ToTable("F_Champion", (string)null);
                });

            modelBuilder.Entity("asg_form.Controllers.Events+T_events", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("events_rule_uri")
                        .HasColumnType("text");

                    b.Property<bool?>("is_over")
                        .HasColumnType("boolean");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<DateTime?>("opentime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("F_events", (string)null);
                });

            modelBuilder.Entity("asg_form.Controllers.T_news", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("FormName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("msg")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime?>("time")
                        .IsRequired()
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("F_news", (string)null);
                });

            modelBuilder.Entity("asg_form.Controllers.form", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("eventsId")
                        .HasColumnType("integer");

                    b.Property<string>("logo_uri")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("piaoshu")
                        .HasColumnType("integer");

                    b.Property<string>("team_name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("team_password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("team_tel")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("time")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("eventsId");

                    b.ToTable("F_form", (string)null);
                });

            modelBuilder.Entity("asg_form.Controllers.role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("formId")
                        .HasColumnType("bigint");

                    b.Property<string>("role_id")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("role_lin")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("role_name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("formId");

                    b.ToTable("F_role", (string)null);
                });

            modelBuilder.Entity("asg_form.Controllers.schedule+schedule_log", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("chickteam")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("teamid")
                        .HasColumnType("bigint");

                    b.Property<string>("userid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool?>("win")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("teamid");

                    b.ToTable("F_achlog", (string)null);
                });

            modelBuilder.Entity("asg_form.Controllers.schedule+team_game", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("id"));

                    b.Property<string>("belong")
                        .HasColumnType("text");

                    b.Property<string>("bilibiliuri")
                        .HasColumnType("text");

                    b.Property<string>("commentary")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("opentime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("referee")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("team1_name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("team1_piaoshu")
                        .HasColumnType("integer");

                    b.Property<string>("team2_name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("team2_piaoshu")
                        .HasColumnType("integer");

                    b.Property<string>("winteam")
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("F_game", (string)null);
                });

            modelBuilder.Entity("asg_form.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("msg")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("asg_form.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserBase64")
                        .HasColumnType("text");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("chinaname")
                        .HasColumnType("text");

                    b.Property<long?>("haveformId")
                        .HasColumnType("bigint");

                    b.Property<bool?>("isbooking")
                        .HasColumnType("boolean");

                    b.Property<string>("officium")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.HasIndex("haveformId");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("asg_form.blog+blog_db", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("ID"));

                    b.Property<string>("formuser")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("msg")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("pushtime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("F_blog", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>", b =>
                {
                    b.HasOne("asg_form.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<long>", b =>
                {
                    b.HasOne("asg_form.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<long>", b =>
                {
                    b.HasOne("asg_form.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<long>", b =>
                {
                    b.HasOne("asg_form.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("asg_form.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<long>", b =>
                {
                    b.HasOne("asg_form.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("asg_form.Controllers.Champion+T_Champion", b =>
                {
                    b.HasOne("asg_form.Controllers.Events+T_events", "events")
                        .WithMany()
                        .HasForeignKey("eventsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("asg_form.Controllers.form", "form")
                        .WithMany()
                        .HasForeignKey("formId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("events");

                    b.Navigation("form");
                });

            modelBuilder.Entity("asg_form.Controllers.form", b =>
                {
                    b.HasOne("asg_form.Controllers.Events+T_events", "events")
                        .WithMany("forms")
                        .HasForeignKey("eventsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("events");
                });

            modelBuilder.Entity("asg_form.Controllers.role", b =>
                {
                    b.HasOne("asg_form.Controllers.form", "form")
                        .WithMany("role")
                        .HasForeignKey("formId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("form");
                });

            modelBuilder.Entity("asg_form.Controllers.schedule+schedule_log", b =>
                {
                    b.HasOne("asg_form.Controllers.schedule+team_game", "team")
                        .WithMany("logs")
                        .HasForeignKey("teamid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("team");
                });

            modelBuilder.Entity("asg_form.User", b =>
                {
                    b.HasOne("asg_form.Controllers.form", "haveform")
                        .WithMany()
                        .HasForeignKey("haveformId");

                    b.Navigation("haveform");
                });

            modelBuilder.Entity("asg_form.Controllers.Events+T_events", b =>
                {
                    b.Navigation("forms");
                });

            modelBuilder.Entity("asg_form.Controllers.form", b =>
                {
                    b.Navigation("role");
                });

            modelBuilder.Entity("asg_form.Controllers.schedule+team_game", b =>
                {
                    b.Navigation("logs");
                });
#pragma warning restore 612, 618
        }
    }
}
