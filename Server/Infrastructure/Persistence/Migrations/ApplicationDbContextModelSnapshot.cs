﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Persistence.Context;

#nullable disable

namespace Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.33")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Identity.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer")
                        .HasColumnName("access_failed_count");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<string>("DeletedBy")
                        .HasColumnType("text")
                        .HasColumnName("deleted_by");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("deleted_date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("email_confirmed");

                    b.Property<string>("FirstName")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("first_name");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<string>("LastName")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("last_name");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("lockout_enabled");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("lockout_end");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_email");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_user_name");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("phone_number_confirmed");

                    b.Property<DateTime>("RefreshTokenExpiryTime")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("refresh_token_expiry_time");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text")
                        .HasColumnName("security_stamp");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("two_factor_enabled");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("email_index");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("user_name_index");

                    b.ToTable("users", "Identity");
                });

            modelBuilder.Entity("Domain.Entities.Identity.UserRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<string>("Description")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_name");

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("updated_by");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.HasKey("Id")
                        .HasName("pk_roles");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("role_name_index");

                    b.ToTable("roles", "Identity");
                });

            modelBuilder.Entity("Domain.Entities.Stock.Stock", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("company_name");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<decimal>("CurrentPrice")
                        .HasPrecision(18, 6)
                        .HasColumnType("numeric(18,6)")
                        .HasColumnName("current_price");

                    b.Property<decimal>("DayHigh")
                        .HasPrecision(18, 6)
                        .HasColumnType("numeric(18,6)")
                        .HasColumnName("day_high");

                    b.Property<decimal>("DayLow")
                        .HasPrecision(18, 6)
                        .HasColumnType("numeric(18,6)")
                        .HasColumnName("day_low");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated");

                    b.Property<decimal>("OpenPrice")
                        .HasPrecision(18, 6)
                        .HasColumnType("numeric(18,6)")
                        .HasColumnName("open_price");

                    b.Property<int>("PopularityScore")
                        .HasColumnType("integer")
                        .HasColumnName("popularity_score");

                    b.Property<decimal>("PreviousClose")
                        .HasPrecision(18, 6)
                        .HasColumnType("numeric(18,6)")
                        .HasColumnName("previous_close");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .HasColumnName("symbol");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.Property<long>("Volume")
                        .HasColumnType("bigint")
                        .HasColumnName("volume");

                    b.HasKey("Id")
                        .HasName("pk_stocks");

                    b.HasIndex("Symbol")
                        .IsUnique()
                        .HasDatabaseName("ix_stocks_symbol");

                    b.ToTable("stocks", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Stock.StockAlert", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<bool>("IsTriggered")
                        .HasColumnType("boolean")
                        .HasColumnName("is_triggered");

                    b.Property<DateTime?>("LastTriggeredAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_triggered_at");

                    b.Property<string>("StockId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("stock_id");

                    b.Property<decimal>("Threshold")
                        .HasColumnType("numeric")
                        .HasColumnName("threshold");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_stock_alert");

                    b.HasIndex("StockId")
                        .HasDatabaseName("ix_stock_alert_stock_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_stock_alert_user_id");

                    b.ToTable("stock_alert");
                });

            modelBuilder.Entity("Domain.Entities.Stock.StockPriceHistory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<decimal>("Close")
                        .HasColumnType("numeric")
                        .HasColumnName("close");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<decimal>("High")
                        .HasColumnType("numeric")
                        .HasColumnName("high");

                    b.Property<decimal>("Low")
                        .HasColumnType("numeric")
                        .HasColumnName("low");

                    b.Property<decimal>("Open")
                        .HasColumnType("numeric")
                        .HasColumnName("open");

                    b.Property<string>("StockId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("stock_id");

                    b.Property<string>("StockId1")
                        .HasColumnType("text")
                        .HasColumnName("stock_id1");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("timestamp");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.Property<long>("Volume")
                        .HasColumnType("bigint")
                        .HasColumnName("volume");

                    b.HasKey("Id")
                        .HasName("pk_stock_price_histories");

                    b.HasIndex("StockId1")
                        .HasDatabaseName("ix_stock_price_histories_stock_id1");

                    b.HasIndex("StockId", "Timestamp")
                        .HasDatabaseName("ix_stock_price_histories_stock_id_timestamp");

                    b.ToTable("stock_price_histories", (string)null);
                });

            modelBuilder.Entity("Domain.Entities.Stock.UserWatchlist", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text")
                        .HasColumnName("created_by");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("StockId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("stock_id");

                    b.Property<string>("StockId1")
                        .HasColumnType("text")
                        .HasColumnName("stock_id1");

                    b.Property<string>("UpdatedBy")
                        .HasColumnType("text")
                        .HasColumnName("updated_by");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_date");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_watchlists");

                    b.HasIndex("StockId")
                        .HasDatabaseName("ix_user_watchlists_stock_id");

                    b.HasIndex("StockId1")
                        .HasDatabaseName("ix_user_watchlists_stock_id1");

                    b.HasIndex("UserId", "StockId")
                        .IsUnique()
                        .HasDatabaseName("ix_user_watchlists_user_id_stock_id");

                    b.ToTable("user_watchlists", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_role_claims");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_role_claims_role_id");

                    b.ToTable("role_claims", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_claims");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_claims_user_id");

                    b.ToTable("user_claims", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text")
                        .HasColumnName("login_provider");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text")
                        .HasColumnName("provider_key");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text")
                        .HasColumnName("provider_display_name");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_user_logins");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_logins_user_id");

                    b.ToTable("user_logins", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<string>("RoleId")
                        .HasColumnType("text")
                        .HasColumnName("role_id");

                    b.HasKey("UserId", "RoleId")
                        .HasName("pk_user_roles");

                    b.HasIndex("RoleId")
                        .HasDatabaseName("ix_user_roles_role_id");

                    b.ToTable("user_roles", "Identity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text")
                        .HasColumnName("login_provider");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Value")
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_user_tokens");

                    b.ToTable("user_tokens", "Identity");
                });

            modelBuilder.Entity("Domain.Entities.Stock.StockAlert", b =>
                {
                    b.HasOne("Domain.Entities.Stock.Stock", "Stock")
                        .WithMany("Alerts")
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_stock_alert_stocks_stock_id");

                    b.HasOne("Domain.Entities.Identity.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_stock_alert_users_user_id");

                    b.Navigation("Stock");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.Stock.StockPriceHistory", b =>
                {
                    b.HasOne("Domain.Entities.Stock.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_stock_price_histories_stocks_stock_id");

                    b.HasOne("Domain.Entities.Stock.Stock", null)
                        .WithMany("PriceHistory")
                        .HasForeignKey("StockId1")
                        .HasConstraintName("fk_stock_price_histories_stocks_stock_id1");

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("Domain.Entities.Stock.UserWatchlist", b =>
                {
                    b.HasOne("Domain.Entities.Stock.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_watchlists_stocks_stock_id");

                    b.HasOne("Domain.Entities.Stock.Stock", null)
                        .WithMany("UserWatchlists")
                        .HasForeignKey("StockId1")
                        .HasConstraintName("fk_user_watchlists_stocks_stock_id1");

                    b.HasOne("Domain.Entities.Identity.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_watchlists_users_user_id");

                    b.Navigation("Stock");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Domain.Entities.Identity.UserRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_role_claims_roles_role_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Domain.Entities.Identity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_claims_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Domain.Entities.Identity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_logins_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Domain.Entities.Identity.UserRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_roles_roles_role_id");

                    b.HasOne("Domain.Entities.Identity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_roles_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Domain.Entities.Identity.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("fk_user_tokens_users_user_id");
                });

            modelBuilder.Entity("Domain.Entities.Stock.Stock", b =>
                {
                    b.Navigation("Alerts");

                    b.Navigation("PriceHistory");

                    b.Navigation("UserWatchlists");
                });
#pragma warning restore 612, 618
        }
    }
}
