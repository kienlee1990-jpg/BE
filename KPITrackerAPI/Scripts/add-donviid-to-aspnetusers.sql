IF COL_LENGTH('dbo.AspNetUsers', 'DonViId') IS NULL
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD [DonViId] bigint NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_AspNetUsers_DonViId'
      AND object_id = OBJECT_ID('dbo.AspNetUsers')
)
BEGIN
    CREATE INDEX [IX_AspNetUsers_DonViId] ON [dbo].[AspNetUsers]([DonViId]);
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_AspNetUsers_DonVi_DonViId'
)
BEGIN
    ALTER TABLE [dbo].[AspNetUsers]
    ADD CONSTRAINT [FK_AspNetUsers_DonVi_DonViId]
    FOREIGN KEY ([DonViId]) REFERENCES [dbo].[DonVi]([Id]);
END;
GO
