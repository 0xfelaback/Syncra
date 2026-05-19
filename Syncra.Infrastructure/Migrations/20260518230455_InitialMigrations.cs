using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Syncra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NodeStates",
                columns: table => new
                {
                    node_id = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    local_sequence = table.Column<int>(type: "integer", nullable: false),
                    last_synced_server_sequence = table.Column<long>(type: "bigint", nullable: false),
                    pending_events_count = table.Column<int>(type: "integer", nullable: false),
                    last_sync_attempt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_successful_sync = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_seen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeStates", x => x.node_id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userName = table.Column<string>(type: "text", nullable: false),
                    passwordhash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    account_id = table.Column<string>(type: "text", nullable: false),
                    userId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.account_id);
                    table.ForeignKey(
                        name: "FK_Accounts_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountSnapshots",
                columns: table => new
                {
                    snapshot_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<string>(type: "text", nullable: false),
                    snapshot_sequence = table.Column<long>(type: "bigint", nullable: false),
                    balance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    event_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSnapshots", x => x.snapshot_id);
                    table.ForeignKey(
                        name: "FK_AccountSnapshots_Accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "Accounts",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountStates",
                columns: table => new
                {
                    account_id = table.Column<string>(type: "text", nullable: false),
                    balance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    provisional_balance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    last_event_id = table.Column<string>(type: "text", nullable: false),
                    last_server_sequence = table.Column<long>(type: "bigint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStates", x => x.account_id);
                    table.ForeignKey(
                        name: "FK_AccountStates_Accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "Accounts",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conflicts",
                columns: table => new
                {
                    conflict_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    detected_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    account_id = table.Column<string>(type: "text", nullable: false),
                    original_event_id = table.Column<string>(type: "text", nullable: true),
                    compensation_event_id = table.Column<string>(type: "text", nullable: true),
                    original_event_archive_id = table.Column<string>(type: "text", nullable: true),
                    compensation_event_archive_id = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    atempted_balance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    actual_balance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    resolution = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conflicts", x => x.conflict_id);
                    table.ForeignKey(
                        name: "FK_Conflicts_Accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "Accounts",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventArchives",
                columns: table => new
                {
                    event_id = table.Column<string>(type: "text", nullable: false),
                    parent_event_id = table.Column<string>(type: "text", nullable: true),
                    compensates_event_id = table.Column<string>(type: "text", nullable: true),
                    node_id = table.Column<string>(type: "text", nullable: false),
                    node_sequence = table.Column<int>(type: "integer", nullable: false),
                    server_sequence = table.Column<long>(type: "bigint", nullable: false),
                    node_timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    server_timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    payload_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    payload_reason = table.Column<string>(type: "text", nullable: false),
                    payload_from_account_id = table.Column<string>(type: "text", nullable: false),
                    payload_to_account_id = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    caused_conflict_id = table.Column<int>(type: "integer", nullable: true),
                    compensates_conflict_id = table.Column<int>(type: "integer", nullable: true),
                    account_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventArchives", x => x.event_id);
                    table.ForeignKey(
                        name: "FK_EventArchives_Accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "Accounts",
                        principalColumn: "account_id");
                    table.ForeignKey(
                        name: "FK_EventArchives_Accounts_payload_from_account_id",
                        column: x => x.payload_from_account_id,
                        principalTable: "Accounts",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventArchives_Accounts_payload_to_account_id",
                        column: x => x.payload_to_account_id,
                        principalTable: "Accounts",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventArchives_Conflicts_caused_conflict_id",
                        column: x => x.caused_conflict_id,
                        principalTable: "Conflicts",
                        principalColumn: "conflict_id");
                    table.ForeignKey(
                        name: "FK_EventArchives_Conflicts_compensates_conflict_id",
                        column: x => x.compensates_conflict_id,
                        principalTable: "Conflicts",
                        principalColumn: "conflict_id");
                    table.ForeignKey(
                        name: "FK_EventArchives_EventArchives_compensates_event_id",
                        column: x => x.compensates_event_id,
                        principalTable: "EventArchives",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK_EventArchives_EventArchives_parent_event_id",
                        column: x => x.parent_event_id,
                        principalTable: "EventArchives",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK_EventArchives_NodeStates_node_id",
                        column: x => x.node_id,
                        principalTable: "NodeStates",
                        principalColumn: "node_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    event_id = table.Column<string>(type: "text", nullable: false),
                    parent_event_id = table.Column<string>(type: "text", nullable: true),
                    compensates_event_id = table.Column<string>(type: "text", nullable: true),
                    caused_conflict_id = table.Column<int>(type: "integer", nullable: true),
                    compensates_conflict_id = table.Column<int>(type: "integer", nullable: true),
                    node_id = table.Column<string>(type: "text", nullable: false),
                    node_sequence = table.Column<int>(type: "integer", nullable: false),
                    server_sequence = table.Column<long>(type: "bigint", nullable: false),
                    node_timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    server_timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    payload_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    payload_reason = table.Column<string>(type: "text", nullable: false),
                    payload_from_account_id = table.Column<string>(type: "text", nullable: false),
                    payload_to_account_id = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    account_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.event_id);
                    table.ForeignKey(
                        name: "FK_Events_Accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "Accounts",
                        principalColumn: "account_id");
                    table.ForeignKey(
                        name: "FK_Events_Accounts_payload_from_account_id",
                        column: x => x.payload_from_account_id,
                        principalTable: "Accounts",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_Accounts_payload_to_account_id",
                        column: x => x.payload_to_account_id,
                        principalTable: "Accounts",
                        principalColumn: "account_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_Conflicts_caused_conflict_id",
                        column: x => x.caused_conflict_id,
                        principalTable: "Conflicts",
                        principalColumn: "conflict_id");
                    table.ForeignKey(
                        name: "FK_Events_Conflicts_compensates_conflict_id",
                        column: x => x.compensates_conflict_id,
                        principalTable: "Conflicts",
                        principalColumn: "conflict_id");
                    table.ForeignKey(
                        name: "FK_Events_Events_compensates_event_id",
                        column: x => x.compensates_event_id,
                        principalTable: "Events",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK_Events_Events_parent_event_id",
                        column: x => x.parent_event_id,
                        principalTable: "Events",
                        principalColumn: "event_id");
                    table.ForeignKey(
                        name: "FK_Events_NodeStates_node_id",
                        column: x => x.node_id,
                        principalTable: "NodeStates",
                        principalColumn: "node_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_userId",
                table: "Accounts",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSnapshots_account_id",
                table: "AccountSnapshots",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStates_last_event_id",
                table: "AccountStates",
                column: "last_event_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_account_id",
                table: "Conflicts",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_compensation_event_archive_id",
                table: "Conflicts",
                column: "compensation_event_archive_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_compensation_event_id",
                table: "Conflicts",
                column: "compensation_event_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_original_event_archive_id",
                table: "Conflicts",
                column: "original_event_archive_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conflicts_original_event_id",
                table: "Conflicts",
                column: "original_event_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventArchives_account_id",
                table: "EventArchives",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_EventArchives_caused_conflict_id",
                table: "EventArchives",
                column: "caused_conflict_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventArchives_compensates_conflict_id",
                table: "EventArchives",
                column: "compensates_conflict_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventArchives_compensates_event_id",
                table: "EventArchives",
                column: "compensates_event_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventArchives_node_id",
                table: "EventArchives",
                column: "node_id");

            migrationBuilder.CreateIndex(
                name: "IX_EventArchives_parent_event_id",
                table: "EventArchives",
                column: "parent_event_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventArchives_payload_from_account_id",
                table: "EventArchives",
                column: "payload_from_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_EventArchives_payload_to_account_id",
                table: "EventArchives",
                column: "payload_to_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_EventArchives_server_sequence",
                table: "EventArchives",
                column: "server_sequence",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventArchives_server_timestamp",
                table: "EventArchives",
                column: "server_timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Events_account_id",
                table: "Events",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_caused_conflict_id",
                table: "Events",
                column: "caused_conflict_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_compensates_conflict_id",
                table: "Events",
                column: "compensates_conflict_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_compensates_event_id",
                table: "Events",
                column: "compensates_event_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_node_id",
                table: "Events",
                column: "node_id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_parent_event_id",
                table: "Events",
                column: "parent_event_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_payload_from_account_id",
                table: "Events",
                column: "payload_from_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_payload_to_account_id",
                table: "Events",
                column: "payload_to_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_server_sequence",
                table: "Events",
                column: "server_sequence",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_server_timestamp",
                table: "Events",
                column: "server_timestamp");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountStates_Events_last_event_id",
                table: "AccountStates",
                column: "last_event_id",
                principalTable: "Events",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conflicts_EventArchives_compensation_event_archive_id",
                table: "Conflicts",
                column: "compensation_event_archive_id",
                principalTable: "EventArchives",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conflicts_EventArchives_original_event_archive_id",
                table: "Conflicts",
                column: "original_event_archive_id",
                principalTable: "EventArchives",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conflicts_Events_compensation_event_id",
                table: "Conflicts",
                column: "compensation_event_id",
                principalTable: "Events",
                principalColumn: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conflicts_Events_original_event_id",
                table: "Conflicts",
                column: "original_event_id",
                principalTable: "Events",
                principalColumn: "event_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Users_userId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Conflicts_Accounts_account_id",
                table: "Conflicts");

            migrationBuilder.DropForeignKey(
                name: "FK_EventArchives_Accounts_account_id",
                table: "EventArchives");

            migrationBuilder.DropForeignKey(
                name: "FK_EventArchives_Accounts_payload_from_account_id",
                table: "EventArchives");

            migrationBuilder.DropForeignKey(
                name: "FK_EventArchives_Accounts_payload_to_account_id",
                table: "EventArchives");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Accounts_account_id",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Accounts_payload_from_account_id",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Accounts_payload_to_account_id",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Conflicts_Events_compensation_event_id",
                table: "Conflicts");

            migrationBuilder.DropForeignKey(
                name: "FK_Conflicts_Events_original_event_id",
                table: "Conflicts");

            migrationBuilder.DropForeignKey(
                name: "FK_Conflicts_EventArchives_compensation_event_archive_id",
                table: "Conflicts");

            migrationBuilder.DropForeignKey(
                name: "FK_Conflicts_EventArchives_original_event_archive_id",
                table: "Conflicts");

            migrationBuilder.DropTable(
                name: "AccountSnapshots");

            migrationBuilder.DropTable(
                name: "AccountStates");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "EventArchives");

            migrationBuilder.DropTable(
                name: "Conflicts");

            migrationBuilder.DropTable(
                name: "NodeStates");
        }
    }
}
