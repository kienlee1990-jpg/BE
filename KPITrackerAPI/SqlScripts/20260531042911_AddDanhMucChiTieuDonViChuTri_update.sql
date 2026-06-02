SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NULL
    BEGIN
        THROW 51000, 'Khong tim thay bang __EFMigrationsHistory. Script nay chi dung de update database da tao bang EF migration.', 1;
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[__EFMigrationsHistory]
        WHERE [MigrationId] = N'20260422041222_InitialSchema'
    )
    BEGIN
        THROW 51001, 'Database chua co migration InitialSchema. Khong chay script update nay tren database rong.', 1;
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM [dbo].[__EFMigrationsHistory]
        WHERE [MigrationId] = N'20260531042911_AddDanhMucChiTieuDonViChuTri'
    )
    BEGIN
        IF OBJECT_ID(N'[dbo].[DanhMucChiTieus]', N'U') IS NULL
        BEGIN
            THROW 51002, 'Khong tim thay bang DanhMucChiTieus.', 1;
        END;

        IF OBJECT_ID(N'[dbo].[DonVi]', N'U') IS NULL
        BEGIN
            THROW 51003, 'Khong tim thay bang DonVi.', 1;
        END;

        IF COL_LENGTH(N'dbo.DanhMucChiTieus', N'DonViChuTriId') IS NULL
        BEGIN
            ALTER TABLE [dbo].[DanhMucChiTieus]
            ADD [DonViChuTriId] bigint NULL;
        END;

        IF NOT EXISTS (
            SELECT 1
            FROM sys.indexes
            WHERE [name] = N'IX_DanhMucChiTieus_DonViChuTriId'
              AND [object_id] = OBJECT_ID(N'[dbo].[DanhMucChiTieus]')
        )
        BEGIN
            CREATE INDEX [IX_DanhMucChiTieus_DonViChuTriId]
            ON [dbo].[DanhMucChiTieus] ([DonViChuTriId]);
        END;

        IF NOT EXISTS (
            SELECT 1
            FROM sys.foreign_keys
            WHERE [name] = N'FK_DanhMucChiTieus_DonVi_DonViChuTriId'
              AND [parent_object_id] = OBJECT_ID(N'[dbo].[DanhMucChiTieus]')
        )
        BEGIN
            ALTER TABLE [dbo].[DanhMucChiTieus] WITH CHECK
            ADD CONSTRAINT [FK_DanhMucChiTieus_DonVi_DonViChuTriId]
            FOREIGN KEY ([DonViChuTriId])
            REFERENCES [dbo].[DonVi] ([Id])
            ON DELETE NO ACTION;
        END;

        INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES (N'20260531042911_AddDanhMucChiTieuDonViChuTri', N'8.0.12');
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    THROW;
END CATCH;
