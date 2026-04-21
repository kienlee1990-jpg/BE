using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KPITrackerAPI.Migrations
{
    public partial class AddDonViIdToApplicationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF COL_LENGTH('dbo.AspNetUsers', 'DonViId') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[AspNetUsers]
                    ADD [DonViId] bigint NULL;
                END
                """
            );

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_AspNetUsers_DonViId'
                      AND object_id = OBJECT_ID('dbo.AspNetUsers')
                )
                BEGIN
                    CREATE INDEX [IX_AspNetUsers_DonViId] ON [dbo].[AspNetUsers]([DonViId]);
                END
                """
            );

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_AspNetUsers_DonVi_DonViId'
                )
                BEGIN
                    ALTER TABLE [dbo].[AspNetUsers]
                    ADD CONSTRAINT [FK_AspNetUsers_DonVi_DonViId]
                    FOREIGN KEY ([DonViId]) REFERENCES [dbo].[DonVi]([Id]);
                END
                """
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_AspNetUsers_DonVi_DonViId'
                )
                BEGIN
                    ALTER TABLE [dbo].[AspNetUsers]
                    DROP CONSTRAINT [FK_AspNetUsers_DonVi_DonViId];
                END
                """
            );

            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_AspNetUsers_DonViId'
                      AND object_id = OBJECT_ID('dbo.AspNetUsers')
                )
                BEGIN
                    DROP INDEX [IX_AspNetUsers_DonViId] ON [dbo].[AspNetUsers];
                END
                """
            );

            migrationBuilder.Sql(
                """
                IF COL_LENGTH('dbo.AspNetUsers', 'DonViId') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[AspNetUsers]
                    DROP COLUMN [DonViId];
                END
                """
            );
        }
    }
}
