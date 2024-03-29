﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace asg_form.Controllers
{
    class FormConfig : IEntityTypeConfiguration<form>
    {
        public void Configure(EntityTypeBuilder<form> builder)
        {
            builder.ToTable("F_form");
            builder.Property(e => e.team_name).IsRequired();
            builder.Property(e => e.team_tel).IsRequired();
            builder.Property(e => e.team_password).IsRequired();
            builder.Property(e => e.time).IsRequired();
            builder.Property(e => e.piaoshu).IsRequired();
            builder.HasOne(e=>e.events).WithMany(e=>e.forms);
            builder.HasOne<Events.T_events>(c => c.events).WithMany(a => a.forms).IsRequired();

        }
    }

    class RoleConfig : IEntityTypeConfiguration<role>
    {
        public void Configure(EntityTypeBuilder<role> builder)
        {
            builder.ToTable("F_role");
            builder.Property(e => e.role_id).IsRequired();
            builder.Property(e => e.role_lin).IsRequired();
            builder.Property(e => e.role_name).IsRequired();
            builder.HasOne<form>(c => c.form).WithMany(a => a.role).IsRequired();

        }
    }


    class newsConfig : IEntityTypeConfiguration<T_news>
    {
        public void Configure(EntityTypeBuilder<T_news> builder)
        {
            builder.ToTable("F_news");
            builder.Property(e => e.FormName).IsRequired();
            builder.Property(e => e.msg).IsRequired();
            builder.Property(e => e.Title).IsRequired();
            builder.Property(e => e.time).IsRequired();
        }
    }

    class blogConfig : IEntityTypeConfiguration<blog.blog_db>
    {
        public void Configure(EntityTypeBuilder<blog.blog_db> builder)
        {
            builder.ToTable("F_blog");
            builder.Property(e => e.title).IsRequired();
            builder.Property(e => e.msg).IsRequired();
            builder.Property(e => e.formuser).IsRequired();
            builder.Property(e => e.pushtime).IsRequired();
        }
    }


    class schgameConfig : IEntityTypeConfiguration<schedule.team_game>
    {
        public void Configure(EntityTypeBuilder<schedule.team_game> builder)
        {
            builder.ToTable("F_game");
            builder.Property(e => e.team1_name).IsRequired();
            builder.Property(e => e.team1_piaoshu).IsRequired();
            builder.Property(e => e.team2_name).IsRequired();
            builder.Property(e => e.team2_piaoshu).IsRequired();
            builder.Property(e => e.opentime).IsRequired();
         
        }
    }


    class schlogConfig : IEntityTypeConfiguration<schedule.schedule_log>
    {
        public void Configure(EntityTypeBuilder<schedule.schedule_log> builder)
        {
            builder.ToTable("F_achlog");
            builder.Property(e => e.userid).IsRequired();
            builder.Property(e => e.win);
            builder.HasOne<schedule.team_game>(e => e.team).WithMany(o=>o.logs).IsRequired();
        }
    }
    class EventsConfig : IEntityTypeConfiguration<Events.T_events>
    {
        public void Configure(EntityTypeBuilder<Events.T_events> builder)
        {
            builder.ToTable("F_events");
            builder.Property(e => e.Id).IsRequired();
            builder.Property(e => e.is_over);
            builder.Property(e => e.name);
            builder.Property(e => e.opentime);
         
        }
    }
    class ChampionConfig : IEntityTypeConfiguration<Champion.T_Champion>
    {
        public void Configure(EntityTypeBuilder<Champion.T_Champion> builder)
        {
            builder.ToTable("F_Champion");
            builder.Property(e => e.Id).IsRequired();
            builder.HasOne(a => a.events);
            builder.Property(e => e.msg);
            builder.HasOne(a=>a.events);

        }
    }



    class TestDbContext : DbContext
    {
        public DbSet<form> Forms { get; set; }
        public DbSet<role> Roles { get; set; }
        public DbSet<T_news> news { get; set; }
        public DbSet<blog.blog_db> blogs { get; set; }
        public DbSet<schedule.schedule_log> schlogs { get; set; }
        public DbSet<schedule.team_game> team_Games { get; set; }
        public DbSet<Events.T_events> events { get; set; }
        public DbSet<Champion.T_Champion> Champions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connStr = @"Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=true";
            optionsBuilder.UseSqlServer(connStr);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

    }

    public class IDBcontext : IdentityDbContext<User, Role, long>
    {
        public DbSet<User> users { get; set; }
        public DbSet<Role> roles { get; set; }
        public IDBcontext(DbContextOptions<IDBcontext> opt) : base(opt)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }


    }


}