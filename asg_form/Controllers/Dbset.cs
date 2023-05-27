using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

        }
    }



    class TestDbContext : DbContext
    {
        public DbSet<form> Forms { get; set; }
        public DbSet<T_news> news { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connstr = @"Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=true";
            optionsBuilder.UseSqlServer(connstr);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }

    }

    public class IDBcontext : IdentityDbContext<User, Role, long>
    {
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
