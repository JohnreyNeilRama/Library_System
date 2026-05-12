using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystem.Migrations
{
    /// <inheritdoc />
    public partial class ReservationApprovalFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('Reservations', 'ApprovedAt') IS NULL
                    ALTER TABLE [Reservations] ADD [ApprovedAt] datetime2 NULL;
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('Reservations', 'ClaimDeadline') IS NULL
                    ALTER TABLE [Reservations] ADD [ClaimDeadline] datetime2 NULL;
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('Reservations', 'ClaimedAt') IS NULL
                    ALTER TABLE [Reservations] ADD [ClaimedAt] datetime2 NULL;
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('Reservations', 'Status') IS NULL
                    ALTER TABLE [Reservations] ADD [Status] nvarchar(30) NOT NULL CONSTRAINT [DF_Reservations_Status] DEFAULT N'Pending';
                """);

            migrationBuilder.Sql("""
                UPDATE [Reservations]
                SET [Status] = N'Pending'
                WHERE [Status] IS NULL OR [Status] = N'';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('Reservations', 'ApprovedAt') IS NOT NULL
                    ALTER TABLE [Reservations] DROP COLUMN [ApprovedAt];
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('Reservations', 'ClaimDeadline') IS NOT NULL
                    ALTER TABLE [Reservations] DROP COLUMN [ClaimDeadline];
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('Reservations', 'ClaimedAt') IS NOT NULL
                    ALTER TABLE [Reservations] DROP COLUMN [ClaimedAt];
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH('Reservations', 'Status') IS NOT NULL
                    ALTER TABLE [Reservations] DROP COLUMN [Status];
                """);
        }
    }
}
