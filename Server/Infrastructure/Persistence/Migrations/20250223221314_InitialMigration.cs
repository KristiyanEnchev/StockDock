﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "Identity",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stocks",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    symbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    company_name = table.Column<string>(type: "text", nullable: false),
                    current_price = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    day_high = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    day_low = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    open_price = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    previous_close = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    volume = table.Column<long>(type: "bigint", nullable: false),
                    last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    popularity_score = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stocks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "Identity",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    refresh_token_expiry_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_by = table.Column<string>(type: "text", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                schema: "Identity",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_claims_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "Identity",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stock_price_histories",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    stock_id = table.Column<string>(type: "text", nullable: false),
                    open = table.Column<decimal>(type: "numeric", nullable: false),
                    high = table.Column<decimal>(type: "numeric", nullable: false),
                    low = table.Column<decimal>(type: "numeric", nullable: false),
                    close = table.Column<decimal>(type: "numeric", nullable: false),
                    volume = table.Column<long>(type: "bigint", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    stock_id1 = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stock_price_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_stock_price_histories_stocks_stock_id",
                        column: x => x.stock_id,
                        principalTable: "stocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_stock_price_histories_stocks_stock_id1",
                        column: x => x.stock_id1,
                        principalTable: "stocks",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "stock_alert",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    stock_id = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    threshold = table.Column<decimal>(type: "numeric", nullable: false),
                    is_triggered = table.Column<bool>(type: "boolean", nullable: false),
                    last_triggered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_stock_alert", x => x.id);
                    table.ForeignKey(
                        name: "fk_stock_alert_stocks_stock_id",
                        column: x => x.stock_id,
                        principalTable: "stocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_stock_alert_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "Identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                schema: "Identity",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_claims_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "Identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                schema: "Identity",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_user_logins_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "Identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                schema: "Identity",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "Identity",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "Identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                schema: "Identity",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_user_tokens_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "Identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_watchlists",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    stock_id = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    stock_id1 = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_watchlists", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_watchlists_stocks_stock_id",
                        column: x => x.stock_id,
                        principalTable: "stocks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_watchlists_stocks_stock_id1",
                        column: x => x.stock_id1,
                        principalTable: "stocks",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_user_watchlists_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "Identity",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_role_claims_role_id",
                schema: "Identity",
                table: "role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "role_name_index",
                schema: "Identity",
                table: "roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_stock_alert_stock_id",
                table: "stock_alert",
                column: "stock_id");

            migrationBuilder.CreateIndex(
                name: "ix_stock_alert_user_id",
                table: "stock_alert",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_stock_price_histories_stock_id_timestamp",
                table: "stock_price_histories",
                columns: new[] { "stock_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "ix_stock_price_histories_stock_id1",
                table: "stock_price_histories",
                column: "stock_id1");

            migrationBuilder.CreateIndex(
                name: "ix_stocks_symbol",
                table: "stocks",
                column: "symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_claims_user_id",
                schema: "Identity",
                table: "user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_logins_user_id",
                schema: "Identity",
                table: "user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                schema: "Identity",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_watchlists_stock_id",
                table: "user_watchlists",
                column: "stock_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_watchlists_stock_id1",
                table: "user_watchlists",
                column: "stock_id1");

            migrationBuilder.CreateIndex(
                name: "ix_user_watchlists_user_id_stock_id",
                table: "user_watchlists",
                columns: new[] { "user_id", "stock_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "email_index",
                schema: "Identity",
                table: "users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "user_name_index",
                schema: "Identity",
                table: "users",
                column: "normalized_user_name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_claims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "stock_alert");

            migrationBuilder.DropTable(
                name: "stock_price_histories");

            migrationBuilder.DropTable(
                name: "user_claims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "user_logins",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "user_roles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "user_tokens",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "user_watchlists");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "stocks");

            migrationBuilder.DropTable(
                name: "users",
                schema: "Identity");
        }
    }
}
