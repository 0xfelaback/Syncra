using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Syncra.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImplementApplicationmanagedConcurrencyTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventArchives_Accounts_account_id",
                table: "EventArchives");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Accounts_account_id",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdempotencyKeys",
                table: "IdempotencyKeys");

            migrationBuilder.DropIndex(
                name: "IX_Events_account_id",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_node_id",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "version",
                table: "AccountSnapshots");

            migrationBuilder.RenameColumn(
                name: "account_id",
                table: "EventArchives",
                newName: "aggregateId");

            migrationBuilder.RenameIndex(
                name: "IX_EventArchives_account_id",
                table: "EventArchives",
                newName: "IX_EventArchives_aggregateId");

            migrationBuilder.AddColumn<Guid>(
                name: "version_control",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "version_control",
                table: "NodeStates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "idempotency_key",
                table: "IdempotencyKeys",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "version_control",
                table: "IdempotencyKeys",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<long>(
                name: "server_sequence",
                table: "Events",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<Guid>(
                name: "version_control",
                table: "Events",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<long>(
                name: "server_sequence",
                table: "EventArchives",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<Guid>(
                name: "version_control",
                table: "EventArchives",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "version_control",
                table: "Conflicts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "version_control",
                table: "AccountStates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "version_control",
                table: "AccountSnapshots",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "version_control",
                table: "Accounts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdempotencyKeys",
                table: "IdempotencyKeys",
                column: "idempotency_key");

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyKeys_event_id",
                table: "IdempotencyKeys",
                column: "event_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_aggregateId",
                table: "Events",
                column: "aggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_node_id",
                table: "Events",
                column: "node_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventArchives_Accounts_aggregateId",
                table: "EventArchives",
                column: "aggregateId",
                principalTable: "Accounts",
                principalColumn: "account_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Accounts_aggregateId",
                table: "Events",
                column: "aggregateId",
                principalTable: "Accounts",
                principalColumn: "account_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventArchives_Accounts_aggregateId",
                table: "EventArchives");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Accounts_aggregateId",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdempotencyKeys",
                table: "IdempotencyKeys");

            migrationBuilder.DropIndex(
                name: "IX_IdempotencyKeys_event_id",
                table: "IdempotencyKeys");

            migrationBuilder.DropIndex(
                name: "IX_Events_aggregateId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_node_id",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "version_control",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "version_control",
                table: "NodeStates");

            migrationBuilder.DropColumn(
                name: "idempotency_key",
                table: "IdempotencyKeys");

            migrationBuilder.DropColumn(
                name: "version_control",
                table: "IdempotencyKeys");

            migrationBuilder.DropColumn(
                name: "version_control",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "version_control",
                table: "EventArchives");

            migrationBuilder.DropColumn(
                name: "version_control",
                table: "Conflicts");

            migrationBuilder.DropColumn(
                name: "version_control",
                table: "AccountStates");

            migrationBuilder.DropColumn(
                name: "version_control",
                table: "AccountSnapshots");

            migrationBuilder.DropColumn(
                name: "version_control",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "aggregateId",
                table: "EventArchives",
                newName: "account_id");

            migrationBuilder.RenameIndex(
                name: "IX_EventArchives_aggregateId",
                table: "EventArchives",
                newName: "IX_EventArchives_account_id");

            migrationBuilder.AlterColumn<long>(
                name: "server_sequence",
                table: "Events",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "account_id",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "server_sequence",
                table: "EventArchives",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "version",
                table: "AccountSnapshots",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdempotencyKeys",
                table: "IdempotencyKeys",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_account_id",
                table: "Events",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_node_id",
                table: "Events",
                column: "node_id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventArchives_Accounts_account_id",
                table: "EventArchives",
                column: "account_id",
                principalTable: "Accounts",
                principalColumn: "account_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Accounts_account_id",
                table: "Events",
                column: "account_id",
                principalTable: "Accounts",
                principalColumn: "account_id");
        }
    }
}
