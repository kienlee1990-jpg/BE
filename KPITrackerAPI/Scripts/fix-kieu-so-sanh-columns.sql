DECLARE @ChiTietTable sysname;
DECLARE @TheoDoiTable sysname;
DECLARE @Sql nvarchar(max);

SELECT TOP (1) @ChiTietTable = t.name
FROM sys.tables t
WHERE t.name IN (N'ChiTietGiaoChiTieu', N'ChiTietGiaoChiTieus')
ORDER BY CASE WHEN t.name = N'ChiTietGiaoChiTieu' THEN 0 ELSE 1 END;

SELECT TOP (1) @TheoDoiTable = t.name
FROM sys.tables t
WHERE t.name IN (N'TheoDoiThucHienKPI', N'TheoDoiThucHienKPIs')
ORDER BY CASE WHEN t.name = N'TheoDoiThucHienKPI' THEN 0 ELSE 1 END;

IF @ChiTietTable IS NULL
BEGIN
    RAISERROR(N'Không tìm thấy bảng ChiTietGiaoChiTieu/ChiTietGiaoChiTieus trong database hiện tại.', 16, 1);
    RETURN;
END

IF @TheoDoiTable IS NULL
BEGIN
    RAISERROR(N'Không tìm thấy bảng TheoDoiThucHienKPI/TheoDoiThucHienKPIs trong database hiện tại.', 16, 1);
    RETURN;
END

SET @Sql = N'
IF COL_LENGTH(''dbo.' + @ChiTietTable + ''', ''KieuSoSanh'') IS NULL
BEGIN
    ALTER TABLE [dbo].' + QUOTENAME(@ChiTietTable) + '
    ADD [KieuSoSanh] nvarchar(50) NULL;
END

IF COL_LENGTH(''dbo.' + @TheoDoiTable + ''', ''GiaTriPhatSinhTrongKy'') IS NULL
BEGIN
    ALTER TABLE [dbo].' + QUOTENAME(@TheoDoiTable) + '
    ADD [GiaTriPhatSinhTrongKy] decimal(18,2) NULL;
END

IF COL_LENGTH(''dbo.' + @TheoDoiTable + ''', ''GiaTriPhatSinhLuyKe'') IS NULL
BEGIN
    ALTER TABLE [dbo].' + QUOTENAME(@TheoDoiTable) + '
    ADD [GiaTriPhatSinhLuyKe] decimal(18,2) NULL;
END
';

EXEC sp_executesql @Sql;
GO
