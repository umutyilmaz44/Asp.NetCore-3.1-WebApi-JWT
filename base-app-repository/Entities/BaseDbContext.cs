using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace base_app_repository.Entities
{
    public partial class BaseDbContext : DbContext
    {
        public BaseDbContext()
        {
        }

        public BaseDbContext(DbContextOptions<BaseDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Grand> Grand { get; set; }
        public virtual DbSet<GrandRole> GrandRole { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<Page> Page { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserLogin> UserLogin { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<UserType> UserType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=192.168.0.112;Database=baseDb;Username=postgres;Password=Vhs1569*");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Grand>(entity =>
            {
                entity.ToTable("grand");

                entity.HasIndex(e => e.GrandName)
                    .HasName("grand_grand_name_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GrandName)
                    .IsRequired()
                    .HasColumnName("grand_name")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<GrandRole>(entity =>
            {
                entity.ToTable("grand_role");

                entity.HasIndex(e => e.GrandId)
                    .HasName("grand_role_grand_id_idx");

                entity.HasIndex(e => e.RoleId)
                    .HasName("grand_role_role_id_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.GrandId).HasColumnName("grand_id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.Grand)
                    .WithMany(p => p.GrandRole)
                    .HasForeignKey(d => d.GrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("grand_role_fk");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.GrandRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("grand_role_fk_1");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("organization");

                entity.HasIndex(e => e.ParentId)
                    .HasName("organization_parentid_idx");

                entity.HasIndex(e => e.Title)
                    .HasName("organization_title_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.RecordDate)
                    .HasColumnName("record_date")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.ToTable("page");

                entity.HasIndex(e => e.NaviagteUrl)
                    .HasName("pages_naviagte_url_idx");

                entity.HasIndex(e => e.PageName)
                    .HasName("pages_page_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.NaviagteUrl)
                    .IsRequired()
                    .HasColumnName("naviagte_url")
                    .HasColumnType("character varying");

                entity.Property(e => e.PageName)
                    .IsRequired()
                    .HasColumnName("page_name")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_token");

                entity.HasIndex(e => e.ExpiryDate)
                    .HasName("refreshtoken_expirydate_idx");

                entity.HasIndex(e => e.Token)
                    .HasName("refresh_token_token_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("refreshtoken_userid_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("timestamp(0) without time zone");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnName("token")
                    .HasColumnType("character varying");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RefreshToken)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("refreshtoken_fk");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.HasIndex(e => e.RoleName)
                    .HasName("role_rolename_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Desc)
                    .HasColumnName("desc")
                    .HasColumnType("character varying");

                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasColumnName("role_name")
                    .HasColumnType("character varying");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.EmailAddress)
                    .HasName("user_email_address_idx");

                entity.HasIndex(e => e.FirstName)
                    .HasName("user_firstname_idx");

                entity.HasIndex(e => e.OrganizationId)
                    .HasName("user_organizationid_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Deleted).HasColumnName("deleted");

                entity.Property(e => e.EmailAddress)
                    .HasColumnName("email_address")
                    .HasColumnType("character varying");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasColumnType("character varying");

                entity.Property(e => e.LastLoginTime)
                    .HasColumnName("last_login_time")
                    .HasColumnType("timestamp(0) without time zone");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("character varying");

                entity.Property(e => e.MiddleName)
                    .HasColumnName("middle_name")
                    .HasColumnType("character varying");

                entity.Property(e => e.OrganizationId)
                    .HasColumnName("organization_id")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("character varying");

                entity.Property(e => e.UserTypeId)
                    .HasColumnName("user_type_id")
                    .HasDefaultValueSql("1");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_fk");

                entity.HasOne(d => d.UserType)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.UserTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_fk_1");
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.ToTable("user_login");

                entity.HasIndex(e => e.LoginTime)
                    .HasName("user_login_login_time_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_login_user_id_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.LoginTime)
                    .HasColumnName("login_time")
                    .HasColumnType("timestamp(0) without time zone")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLogin)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_login_fk");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("user_role");

                entity.HasIndex(e => e.RoleId)
                    .HasName("user_role_role_id_idx");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_role_user_id_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_role_fk_1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_role_fk");
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.ToTable("user_type");

                entity.HasIndex(e => e.TypeName)
                    .HasName("user_type_type_name_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TokenLifeTime)
                    .HasColumnName("token_life_time")
                    .HasDefaultValueSql("60");

                entity.Property(e => e.TypeDescription)
                    .HasColumnName("type_description")
                    .HasColumnType("character varying");

                entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasColumnName("type_name")
                    .HasColumnType("character varying");
            });

            modelBuilder.HasSequence("grand_id_seq");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
