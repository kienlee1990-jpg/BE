/*
    Seed dữ liệu demo KPI dựa trên đơn vị và kỳ báo cáo đã có.

    Mục tiêu:
    - Không tạo đơn vị hoặc kỳ báo cáo mới.
    - Tạo dữ liệu có quan hệ logic qua các bảng nghiệp vụ chính:
      DanhMucChiTieus, DotGiaoChiTieu, ChiTietGiaoChiTieu,
      TheoDoiThucHienKPI, DanhGiaKPI, NhomThiDua và các bảng liên kết.
    - Idempotent: có thể chạy lại, script sẽ cập nhật dữ liệu demo theo mã DEMO-*.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Now DATETIME2 = SYSUTCDATETIME();
    DECLARE @Actor NVARCHAR(100) = N'demo-script';
    DECLARE @Nam INT;
    DECLARE @CatpId BIGINT;
    DECLARE @NgayBatDau DATE;
    DECLARE @NgayKetThuc DATE;
    DECLARE @MaDotGiao NVARCHAR(50);
    DECLARE @DotGiaoId BIGINT;
    DECLARE @LoaiKy NVARCHAR(30);
    DECLARE @PeriodCount INT;
    DECLARE @LatestPeriodSeq INT;
    DECLARE @UnitCount INT;

    SELECT TOP (1)
        @Nam = Nam
    FROM KyBaoCaoKPI
    ORDER BY Nam DESC, NgayCuoiKy DESC, Id DESC;

    IF @Nam IS NULL
        THROW 51000, N'Chưa có dữ liệu kỳ báo cáo. Vui lòng tạo kỳ báo cáo trước khi chạy seed demo.', 1;

    SELECT
        @NgayBatDau = MIN(TuNgay),
        @NgayKetThuc = MAX(DenNgay)
    FROM KyBaoCaoKPI
    WHERE Nam = @Nam;

    SELECT TOP (1)
        @CatpId = Id
    FROM DonVi
    WHERE TrangThai = N'HOAT_DONG'
      AND (
            LoaiDonVi = N'THANH_PHO'
            OR UPPER(MaDonVi) = N'CATP'
            OR TenDonVi LIKE N'%Công an thành phố%'
      )
    ORDER BY
        CASE
            WHEN UPPER(MaDonVi) = N'CATP' THEN 0
            WHEN LoaiDonVi = N'THANH_PHO' THEN 1
            ELSE 2
        END,
        Id;

    IF @CatpId IS NULL
        THROW 51001, N'Không tìm thấy đơn vị Công an thành phố đang hoạt động để làm đơn vị giao.', 1;

    DROP TABLE IF EXISTS #DemoUnits;
    SELECT TOP (12)
        ROW_NUMBER() OVER (
            ORDER BY
                CASE
                    WHEN LoaiDonVi = N'XA' THEN 0
                    WHEN LoaiDonVi = N'PHONG' THEN 1
                    ELSE 2
                END,
                TenDonVi,
                Id
        ) AS Seq,
        Id,
        MaDonVi,
        TenDonVi,
        LoaiDonVi
    INTO #DemoUnits
    FROM DonVi
    WHERE TrangThai = N'HOAT_DONG'
      AND Id <> @CatpId
    ORDER BY
        CASE
            WHEN LoaiDonVi = N'XA' THEN 0
            WHEN LoaiDonVi = N'PHONG' THEN 1
            ELSE 2
        END,
        TenDonVi,
        Id;

    SELECT @UnitCount = COUNT(*) FROM #DemoUnits;

    IF @UnitCount = 0
        THROW 51002, N'Chưa có đơn vị cấp dưới để giao chỉ tiêu demo.', 1;

    DROP TABLE IF EXISTS #Catalog;
    CREATE TABLE #Catalog
    (
        MaChiTieu NVARCHAR(50) NOT NULL PRIMARY KEY,
        TenChiTieu NVARCHAR(500) NOT NULL,
        NguonChiTieu NVARCHAR(50) NOT NULL,
        LoaiChiTieu NVARCHAR(50) NOT NULL,
        CapApDung NVARCHAR(50) NOT NULL,
        LinhVucNghiepVu NVARCHAR(255) NULL,
        DonViTinh NVARCHAR(50) NULL,
        MoTa NVARCHAR(MAX) NULL,
        HuongDanTinhToan NVARCHAR(MAX) NULL,
        CoChoPhepPhanRa BIT NOT NULL,
        TrangThaiSuDung NVARCHAR(50) NOT NULL,
        DieuKienHoanThanh NVARCHAR(MAX) NULL,
        DieuKienKhongHoanThanh NVARCHAR(MAX) NULL,
        TyLePhanTramMucTieu DECIMAL(18,2) NULL,
        LoaiMocSoSanh NVARCHAR(50) NULL,
        ChieuSoSanh NVARCHAR(50) NULL,
        MaChiTieuCha NVARCHAR(50) NULL,
        ThuTuHienThi INT NULL,
        BatBuocDatTatCaTieuChiCon BIT NOT NULL
    );

    INSERT INTO #Catalog
    (
        MaChiTieu, TenChiTieu, NguonChiTieu, LoaiChiTieu, CapApDung,
        LinhVucNghiepVu, DonViTinh, MoTa, HuongDanTinhToan,
        CoChoPhepPhanRa, TrangThaiSuDung, DieuKienHoanThanh,
        DieuKienKhongHoanThanh, TyLePhanTramMucTieu, LoaiMocSoSanh,
        ChieuSoSanh, MaChiTieuCha, ThuTuHienThi, BatBuocDatTatCaTieuChiCon
    )
    VALUES
    (N'DEMO-ANTT-001', N'Không để xảy ra vụ việc phức tạp về an ninh trật tự',
        N'THANH_PHO', N'DINH_TINH', N'TOAN_LUC_LUONG', N'An ninh trật tự', NULL,
        N'Theo dõi chất lượng giữ ổn định địa bàn, không phát sinh điểm nóng kéo dài.',
        N'Đơn vị tự đánh giá DAM_BAO/KHONG_DAM_BAO theo tình hình thực tế trong kỳ.',
        0, N'DANG_AP_DUNG', N'DAM_BAO', N'KHONG_DAM_BAO', NULL, NULL, NULL, NULL, 10, 1),

    (N'DEMO-TTKS-001', N'Tổ chức tuần tra kiểm soát địa bàn trọng điểm',
        N'THANH_PHO', N'DINH_LUONG_TICH_LUY', N'TOAN_LUC_LUONG', N'Trật tự xã hội', N'lượt',
        N'Phản ánh cường độ tuần tra chủ động tại các tuyến, khu vực, địa bàn trọng điểm.',
        N'Lũy kế số lượt tuần tra đã thực hiện trong năm; đạt khi bằng hoặc vượt mục tiêu giao.',
        0, N'DANG_AP_DUNG', NULL, NULL, NULL, NULL, NULL, NULL, 20, 1),

    (N'DEMO-TTTP-001', N'Tỷ lệ giải quyết tin báo, tố giác tội phạm đúng hạn',
        N'THANH_PHO', N'DINH_LUONG_SO_SANH', N'TOAN_LUC_LUONG', N'Điều tra xử lý tin báo', N'%',
        N'Đo tỷ lệ tin báo, tố giác tội phạm được giải quyết đúng hạn trên tổng số tin báo tiếp nhận.',
        N'Tỷ lệ thực tế = số tin báo giải quyết đúng hạn / tổng số tin báo tiếp nhận * 100.',
        0, N'DANG_AP_DUNG', NULL, NULL, 90, NULL, N'TANG', NULL, 30, 1),

    (N'DEMO-PCCC-001', N'Kiểm tra an toàn phòng cháy chữa cháy tại cơ sở',
        N'THANH_PHO', N'DINH_LUONG_TICH_LUY', N'TOAN_LUC_LUONG', N'Phòng cháy chữa cháy', N'cơ sở',
        N'Ghi nhận số cơ sở được kiểm tra, hướng dẫn khắc phục tồn tại về PCCC.',
        N'Lũy kế số cơ sở đã kiểm tra trong năm; đạt khi bằng hoặc vượt mục tiêu giao.',
        0, N'DANG_AP_DUNG', NULL, NULL, NULL, NULL, NULL, NULL, 40, 1),

    (N'DEMO-GT-001', N'Kéo giảm tai nạn giao thông trên địa bàn',
        N'THANH_PHO', N'PHAN_RA', N'TOAN_LUC_LUONG', N'Trật tự an toàn giao thông', NULL,
        N'Chỉ tiêu cha tổng hợp từ các tiêu chí con về số vụ tai nạn và số người bị thương/vong.',
        N'Không nhập số liệu trực tiếp ở chỉ tiêu cha; hệ thống tổng hợp từ tiêu chí con.',
        1, N'DANG_AP_DUNG', NULL, NULL, NULL, NULL, NULL, NULL, 50, 1),

    (N'DEMO-GT-001-VU', N'Số vụ tai nạn giao thông',
        N'THANH_PHO', N'DINH_LUONG_TICH_LUY', N'TOAN_LUC_LUONG', N'Trật tự an toàn giao thông', N'vụ',
        N'Theo dõi số vụ tai nạn giao thông phát sinh trong kỳ.',
        N'Lũy kế số vụ tai nạn không được vượt mục tiêu tối đa đã giao.',
        0, N'DANG_AP_DUNG', NULL, NULL, NULL, NULL, N'GIAM', N'DEMO-GT-001', 1, 1),

    (N'DEMO-GT-001-THUONGVONG', N'Số người bị thương hoặc tử vong do tai nạn giao thông',
        N'THANH_PHO', N'DINH_LUONG_TICH_LUY', N'TOAN_LUC_LUONG', N'Trật tự an toàn giao thông', N'người',
        N'Theo dõi số người bị thương hoặc tử vong do tai nạn giao thông trong kỳ.',
        N'Lũy kế số người bị thương/tử vong không được vượt mục tiêu tối đa đã giao.',
        0, N'DANG_AP_DUNG', NULL, NULL, NULL, NULL, N'GIAM', N'DEMO-GT-001', 2, 1);

    MERGE DanhMucChiTieus AS target
    USING (
        SELECT *
        FROM #Catalog
        WHERE MaChiTieuCha IS NULL
    ) AS source
    ON target.MaChiTieu = source.MaChiTieu
    WHEN MATCHED THEN
        UPDATE SET
            TenChiTieu = source.TenChiTieu,
            NguonChiTieu = source.NguonChiTieu,
            LoaiChiTieu = source.LoaiChiTieu,
            CapApDung = source.CapApDung,
            LinhVucNghiepVu = source.LinhVucNghiepVu,
            DonViTinh = source.DonViTinh,
            MoTa = source.MoTa,
            HuongDanTinhToan = source.HuongDanTinhToan,
            CoChoPhepPhanRa = source.CoChoPhepPhanRa,
            TrangThaiSuDung = source.TrangThaiSuDung,
            DieuKienHoanThanh = source.DieuKienHoanThanh,
            DieuKienKhongHoanThanh = source.DieuKienKhongHoanThanh,
            TyLePhanTramMucTieu = source.TyLePhanTramMucTieu,
            LoaiMocSoSanh = source.LoaiMocSoSanh,
            ChieuSoSanh = source.ChieuSoSanh,
            ChiTieuChaId = NULL,
            ThuTuHienThi = source.ThuTuHienThi,
            BatBuocDatTatCaTieuChiCon = source.BatBuocDatTatCaTieuChiCon,
            UpdatedAt = @Now
    WHEN NOT MATCHED THEN
        INSERT
        (
            MaChiTieu, TenChiTieu, NguonChiTieu, LoaiChiTieu, CapApDung,
            LinhVucNghiepVu, DonViTinh, MoTa, HuongDanTinhToan, CoChoPhepPhanRa,
            TrangThaiSuDung, NgayHieuLuc, DieuKienHoanThanh, DieuKienKhongHoanThanh,
            TyLePhanTramMucTieu, LoaiMocSoSanh, ChieuSoSanh, ChiTieuChaId,
            ThuTuHienThi, BatBuocDatTatCaTieuChiCon, CreatedAt
        )
        VALUES
        (
            source.MaChiTieu, source.TenChiTieu, source.NguonChiTieu, source.LoaiChiTieu, source.CapApDung,
            source.LinhVucNghiepVu, source.DonViTinh, source.MoTa, source.HuongDanTinhToan, source.CoChoPhepPhanRa,
            source.TrangThaiSuDung, @NgayBatDau, source.DieuKienHoanThanh, source.DieuKienKhongHoanThanh,
            source.TyLePhanTramMucTieu, source.LoaiMocSoSanh, source.ChieuSoSanh, NULL,
            source.ThuTuHienThi, source.BatBuocDatTatCaTieuChiCon, @Now
        );

    MERGE DanhMucChiTieus AS target
    USING (
        SELECT
            c.*,
            p.Id AS ChiTieuChaId
        FROM #Catalog c
        INNER JOIN DanhMucChiTieus p ON p.MaChiTieu = c.MaChiTieuCha
        WHERE c.MaChiTieuCha IS NOT NULL
    ) AS source
    ON target.MaChiTieu = source.MaChiTieu
    WHEN MATCHED THEN
        UPDATE SET
            TenChiTieu = source.TenChiTieu,
            NguonChiTieu = source.NguonChiTieu,
            LoaiChiTieu = source.LoaiChiTieu,
            CapApDung = source.CapApDung,
            LinhVucNghiepVu = source.LinhVucNghiepVu,
            DonViTinh = source.DonViTinh,
            MoTa = source.MoTa,
            HuongDanTinhToan = source.HuongDanTinhToan,
            CoChoPhepPhanRa = source.CoChoPhepPhanRa,
            TrangThaiSuDung = source.TrangThaiSuDung,
            DieuKienHoanThanh = source.DieuKienHoanThanh,
            DieuKienKhongHoanThanh = source.DieuKienKhongHoanThanh,
            TyLePhanTramMucTieu = source.TyLePhanTramMucTieu,
            LoaiMocSoSanh = source.LoaiMocSoSanh,
            ChieuSoSanh = source.ChieuSoSanh,
            ChiTieuChaId = source.ChiTieuChaId,
            ThuTuHienThi = source.ThuTuHienThi,
            BatBuocDatTatCaTieuChiCon = source.BatBuocDatTatCaTieuChiCon,
            UpdatedAt = @Now
    WHEN NOT MATCHED THEN
        INSERT
        (
            MaChiTieu, TenChiTieu, NguonChiTieu, LoaiChiTieu, CapApDung,
            LinhVucNghiepVu, DonViTinh, MoTa, HuongDanTinhToan, CoChoPhepPhanRa,
            TrangThaiSuDung, NgayHieuLuc, DieuKienHoanThanh, DieuKienKhongHoanThanh,
            TyLePhanTramMucTieu, LoaiMocSoSanh, ChieuSoSanh, ChiTieuChaId,
            ThuTuHienThi, BatBuocDatTatCaTieuChiCon, CreatedAt
        )
        VALUES
        (
            source.MaChiTieu, source.TenChiTieu, source.NguonChiTieu, source.LoaiChiTieu, source.CapApDung,
            source.LinhVucNghiepVu, source.DonViTinh, source.MoTa, source.HuongDanTinhToan, source.CoChoPhepPhanRa,
            source.TrangThaiSuDung, @NgayBatDau, source.DieuKienHoanThanh, source.DieuKienKhongHoanThanh,
            source.TyLePhanTramMucTieu, source.LoaiMocSoSanh, source.ChieuSoSanh, source.ChiTieuChaId,
            source.ThuTuHienThi, source.BatBuocDatTatCaTieuChiCon, @Now
        );

    DROP TABLE IF EXISTS #CatalogMap;
    SELECT
        Id,
        MaChiTieu,
        LoaiChiTieu,
        ChiTieuChaId,
        BatBuocDatTatCaTieuChiCon
    INTO #CatalogMap
    FROM DanhMucChiTieus
    WHERE MaChiTieu LIKE N'DEMO-%';

    SET @MaDotGiao = CONCAT(N'DEMO-CATP-', @Nam);

    MERGE DotGiaoChiTieu AS target
    USING (
        SELECT
            @MaDotGiao AS MaDotGiao,
            CONCAT(N'Đợt giao demo kiểm thử hiển thị CATP năm ', @Nam) AS TenDotGiao
    ) AS source
    ON target.MaDotGiao = source.MaDotGiao
    WHEN MATCHED THEN
        UPDATE SET
            TenDotGiao = source.TenDotGiao,
            NamApDung = @Nam,
            NguonDotGiao = N'THANH_PHO_GIAO',
            CapGiao = N'THANH_PHO',
            DonViGiaoId = @CatpId,
            NgayBatDau = @NgayBatDau,
            NgayKetThuc = @NgayKetThuc,
            TrangThai = N'PUBLISHED',
            GhiChu = N'Dữ liệu demo có logic liên thông từ danh mục, giao chỉ tiêu, báo cáo đến đánh giá.',
            UpdatedAt = @Now
    WHEN NOT MATCHED THEN
        INSERT
        (
            MaDotGiao, TenDotGiao, NamApDung, NguonDotGiao, CapGiao,
            DonViGiaoId, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, CreatedAt
        )
        VALUES
        (
            source.MaDotGiao, source.TenDotGiao, @Nam, N'THANH_PHO_GIAO', N'THANH_PHO',
            @CatpId, @NgayBatDau, @NgayKetThuc, N'PUBLISHED',
            N'Dữ liệu demo có logic liên thông từ danh mục, giao chỉ tiêu, báo cáo đến đánh giá.',
            @Now
        );

    SELECT @DotGiaoId = Id
    FROM DotGiaoChiTieu
    WHERE MaDotGiao = @MaDotGiao;

    DROP TABLE IF EXISTS #AssignmentConfig;
    CREATE TABLE #AssignmentConfig
    (
        MaChiTieu NVARCHAR(50) NOT NULL PRIMARY KEY,
        TargetBase DECIMAL(18,2) NULL,
        TargetStep DECIMAL(18,2) NULL,
        DauKyBase DECIMAL(18,2) NULL,
        DauKyStep DECIMAL(18,2) NULL,
        GiaTriMucTieuText NVARCHAR(MAX) NULL,
        TieuChiDanhGia NVARCHAR(50) NULL,
        LoaiMocSoSanh NVARCHAR(50) NULL,
        KieuSoSanh NVARCHAR(50) NULL,
        ChieuSoSanh NVARCHAR(50) NULL,
        QuyTacDanhGia NVARCHAR(50) NULL,
        TanSuatBaoCao NVARCHAR(30) NULL,
        ThuTuHienThi INT NULL,
        GhiChu NVARCHAR(MAX) NULL
    );

    INSERT INTO #AssignmentConfig
    VALUES
    (N'DEMO-ANTT-001', NULL, NULL, 0, 0, N'DAM_BAO', N'DINH_TINH', NULL, NULL, NULL, N'MAC_DINH', N'THANG', 10, N'Định tính: đơn vị chọn kết quả DAM_BAO/KHONG_DAM_BAO khi báo cáo.'),
    (N'DEMO-TTKS-001', 80, 6, 0, 0, NULL, N'DINH_LUONG_TICH_LUY', NULL, NULL, N'TANG', N'DAT_TOI_THIEU', N'THANG', 20, N'Lũy kế lượt tuần tra, mục tiêu tăng theo quy mô đơn vị.'),
    (N'DEMO-TTTP-001', 90, 0, 0, 0, NULL, N'DINH_LUONG_SO_SANH', NULL, N'TY_LE', N'TANG', N'DAT_TOI_THIEU', N'THANG', 30, N'Tỷ lệ giải quyết đúng hạn = đúng hạn / tiếp nhận.'),
    (N'DEMO-PCCC-001', 24, 2, 0, 0, NULL, N'DINH_LUONG_TICH_LUY', NULL, NULL, N'TANG', N'DAT_TOI_THIEU', N'THANG', 40, N'Lũy kế số cơ sở PCCC được kiểm tra.'),
    (N'DEMO-GT-001', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'THANG', 50, N'Chỉ tiêu cha: tổng hợp từ các tiêu chí con, không nhập số liệu trực tiếp.');

    MERGE ChiTietGiaoChiTieu AS target
    USING (
        SELECT
            @DotGiaoId AS DotGiaoChiTieuId,
            c.Id AS DanhMucChiTieuId,
            u.Id AS DonViNhanId,
            u.Id AS DonViThucHienChinhId,
            CASE WHEN cfg.TargetBase IS NULL THEN NULL ELSE cfg.TargetBase + cfg.TargetStep * u.Seq END AS GiaTriMucTieu,
            cfg.GiaTriMucTieuText,
            CASE WHEN cfg.DauKyBase IS NULL THEN NULL ELSE cfg.DauKyBase + cfg.DauKyStep * u.Seq END AS GiaTriDauKyCoDinh,
            cfg.TieuChiDanhGia,
            cfg.LoaiMocSoSanh,
            cfg.KieuSoSanh,
            cfg.ChieuSoSanh,
            cfg.QuyTacDanhGia,
            cfg.TanSuatBaoCao,
            cfg.ThuTuHienThi,
            cfg.GhiChu
        FROM #DemoUnits u
        CROSS JOIN #AssignmentConfig cfg
        INNER JOIN #CatalogMap c ON c.MaChiTieu = cfg.MaChiTieu
        WHERE c.ChiTieuChaId IS NULL
    ) AS source
    ON target.DotGiaoChiTieuId = source.DotGiaoChiTieuId
       AND target.DanhMucChiTieuId = source.DanhMucChiTieuId
       AND target.DonViNhanId = source.DonViNhanId
    WHEN MATCHED THEN
        UPDATE SET
            DonViThucHienChinhId = source.DonViThucHienChinhId,
            GiaTriMucTieu = source.GiaTriMucTieu,
            GiaTriMucTieuText = source.GiaTriMucTieuText,
            GiaTriDauKyCoDinh = source.GiaTriDauKyCoDinh,
            TieuChiDanhGia = source.TieuChiDanhGia,
            LoaiMocSoSanh = source.LoaiMocSoSanh,
            KieuSoSanh = source.KieuSoSanh,
            ChieuSoSanh = source.ChieuSoSanh,
            QuyTacDanhGia = source.QuyTacDanhGia,
            GhiChu = source.GhiChu,
            ThuTuHienThi = source.ThuTuHienThi,
            TanSuatBaoCao = source.TanSuatBaoCao,
            TrangThai = N'DA_GIAO',
            UpdatedAt = @Now,
            UpdatedBy = @Actor
    WHEN NOT MATCHED THEN
        INSERT
        (
            DotGiaoChiTieuId, DanhMucChiTieuId, DonViNhanId, DonViThucHienChinhId,
            GiaTriMucTieu, GiaTriMucTieuText, GiaTriDauKyCoDinh, TieuChiDanhGia,
            LoaiMocSoSanh, KieuSoSanh, ChieuSoSanh, QuyTacDanhGia, GhiChu,
            ThuTuHienThi, TanSuatBaoCao, TrangThai, CreatedAt, CreatedBy
        )
        VALUES
        (
            source.DotGiaoChiTieuId, source.DanhMucChiTieuId, source.DonViNhanId, source.DonViThucHienChinhId,
            source.GiaTriMucTieu, source.GiaTriMucTieuText, source.GiaTriDauKyCoDinh, source.TieuChiDanhGia,
            source.LoaiMocSoSanh, source.KieuSoSanh, source.ChieuSoSanh, source.QuyTacDanhGia, source.GhiChu,
            source.ThuTuHienThi, source.TanSuatBaoCao, N'DA_GIAO', @Now, @Actor
        );

    DROP TABLE IF EXISTS #ChildAssignmentConfig;
    CREATE TABLE #ChildAssignmentConfig
    (
        MaChiTieu NVARCHAR(50) NOT NULL PRIMARY KEY,
        TargetBase DECIMAL(18,2) NOT NULL,
        TargetStep DECIMAL(18,2) NOT NULL,
        TieuChiDanhGia NVARCHAR(50) NOT NULL,
        QuyTacDanhGia NVARCHAR(50) NOT NULL,
        ThuTuHienThi INT NOT NULL,
        GhiChu NVARCHAR(MAX) NULL
    );

    INSERT INTO #ChildAssignmentConfig
    VALUES
    (N'DEMO-GT-001-VU', 5, 1, N'DINH_LUONG_TICH_LUY', N'KHONG_VUOT_NGUONG', 1, N'Không vượt số vụ tai nạn tối đa được giao.'),
    (N'DEMO-GT-001-THUONGVONG', 2, 1, N'DINH_LUONG_TICH_LUY', N'KHONG_VUOT_NGUONG', 2, N'Không vượt số người bị thương/vong tối đa được giao.');

    MERGE ChiTietGiaoChiTieu AS target
    USING (
        SELECT
            @DotGiaoId AS DotGiaoChiTieuId,
            childCatalog.Id AS DanhMucChiTieuId,
            parentAssign.DonViNhanId,
            parentAssign.DonViThucHienChinhId,
            cfg.TargetBase + cfg.TargetStep * u.Seq AS GiaTriMucTieu,
            NULL AS GiaTriMucTieuText,
            CAST(0 AS DECIMAL(18,2)) AS GiaTriDauKyCoDinh,
            cfg.TieuChiDanhGia,
            NULL AS LoaiMocSoSanh,
            NULL AS KieuSoSanh,
            N'GIAM' AS ChieuSoSanh,
            cfg.QuyTacDanhGia,
            parentAssign.Id AS ChiTietGiaoChaId,
            cfg.GhiChu,
            cfg.ThuTuHienThi,
            parentAssign.TanSuatBaoCao
        FROM #ChildAssignmentConfig cfg
        INNER JOIN #CatalogMap childCatalog ON childCatalog.MaChiTieu = cfg.MaChiTieu
        INNER JOIN #CatalogMap parentCatalog ON parentCatalog.MaChiTieu = N'DEMO-GT-001'
        INNER JOIN ChiTietGiaoChiTieu parentAssign
            ON parentAssign.DotGiaoChiTieuId = @DotGiaoId
           AND parentAssign.DanhMucChiTieuId = parentCatalog.Id
           AND parentAssign.ChiTietGiaoChaId IS NULL
        INNER JOIN #DemoUnits u ON u.Id = parentAssign.DonViNhanId
    ) AS source
    ON target.DotGiaoChiTieuId = source.DotGiaoChiTieuId
       AND target.DanhMucChiTieuId = source.DanhMucChiTieuId
       AND target.DonViNhanId = source.DonViNhanId
    WHEN MATCHED THEN
        UPDATE SET
            DonViThucHienChinhId = source.DonViThucHienChinhId,
            GiaTriMucTieu = source.GiaTriMucTieu,
            GiaTriMucTieuText = source.GiaTriMucTieuText,
            GiaTriDauKyCoDinh = source.GiaTriDauKyCoDinh,
            TieuChiDanhGia = source.TieuChiDanhGia,
            LoaiMocSoSanh = source.LoaiMocSoSanh,
            KieuSoSanh = source.KieuSoSanh,
            ChieuSoSanh = source.ChieuSoSanh,
            QuyTacDanhGia = source.QuyTacDanhGia,
            ChiTietGiaoChaId = source.ChiTietGiaoChaId,
            GhiChu = source.GhiChu,
            ThuTuHienThi = source.ThuTuHienThi,
            TanSuatBaoCao = source.TanSuatBaoCao,
            TrangThai = N'DA_GIAO',
            UpdatedAt = @Now,
            UpdatedBy = @Actor
    WHEN NOT MATCHED THEN
        INSERT
        (
            DotGiaoChiTieuId, DanhMucChiTieuId, DonViNhanId, DonViThucHienChinhId,
            GiaTriMucTieu, GiaTriMucTieuText, GiaTriDauKyCoDinh, TieuChiDanhGia,
            LoaiMocSoSanh, KieuSoSanh, ChieuSoSanh, QuyTacDanhGia, ChiTietGiaoChaId,
            GhiChu, ThuTuHienThi, TanSuatBaoCao, TrangThai, CreatedAt, CreatedBy
        )
        VALUES
        (
            source.DotGiaoChiTieuId, source.DanhMucChiTieuId, source.DonViNhanId, source.DonViThucHienChinhId,
            source.GiaTriMucTieu, source.GiaTriMucTieuText, source.GiaTriDauKyCoDinh, source.TieuChiDanhGia,
            source.LoaiMocSoSanh, source.KieuSoSanh, source.ChieuSoSanh, source.QuyTacDanhGia, source.ChiTietGiaoChaId,
            source.GhiChu, source.ThuTuHienThi, source.TanSuatBaoCao, N'DA_GIAO', @Now, @Actor
        );

    MERGE NhomThiDua AS target
    USING (
        SELECT N'DEMO-NHOM-PHUONG-XA' AS MaNhom, N'Nhóm thi đua demo Công an phường/xã' AS TenNhom, N'Nhóm dùng để kiểm thử xếp hạng, so sánh và tổng hợp đánh giá.' AS MoTa
        UNION ALL
        SELECT N'DEMO-NHOM-PHONG', N'Nhóm thi đua demo cấp phòng/đội', N'Nhóm dùng để kiểm thử xếp hạng, so sánh và tổng hợp đánh giá.'
    ) AS source
    ON target.MaNhom = source.MaNhom
    WHEN MATCHED THEN
        UPDATE SET TenNhom = source.TenNhom, MoTa = source.MoTa, TrangThai = N'HOAT_DONG', UpdatedAt = @Now
    WHEN NOT MATCHED THEN
        INSERT (MaNhom, TenNhom, MoTa, TrangThai, CreatedAt)
        VALUES (source.MaNhom, source.TenNhom, source.MoTa, N'HOAT_DONG', @Now);

    INSERT INTO NhomThiDuaDonVi (NhomThiDuaId, DonViId, CreatedAt)
    SELECT
        n.Id,
        u.Id,
        @Now
    FROM #DemoUnits u
    INNER JOIN NhomThiDua n
        ON n.MaNhom = CASE WHEN u.LoaiDonVi = N'XA' THEN N'DEMO-NHOM-PHUONG-XA' ELSE N'DEMO-NHOM-PHONG' END
    WHERE NOT EXISTS (
        SELECT 1
        FROM NhomThiDuaDonVi existing
        WHERE existing.NhomThiDuaId = n.Id
          AND existing.DonViId = u.Id
    );

    INSERT INTO NhomThiDuaChiTieu (NhomThiDuaId, DanhMucChiTieuId, CreatedAt)
    SELECT
        n.Id,
        c.Id,
        @Now
    FROM NhomThiDua n
    CROSS JOIN #CatalogMap c
    WHERE n.MaNhom IN (N'DEMO-NHOM-PHUONG-XA', N'DEMO-NHOM-PHONG')
      AND c.ChiTieuChaId IS NULL
      AND NOT EXISTS (
          SELECT 1
          FROM NhomThiDuaChiTieu existing
          WHERE existing.NhomThiDuaId = n.Id
            AND existing.DanhMucChiTieuId = c.Id
      );

    IF NOT EXISTS (SELECT 1 FROM CauHinhNguongDanhGiaKPI WHERE DanhMucChiTieuId IS NULL)
    BEGIN
        INSERT INTO CauHinhNguongDanhGiaKPI
        (
            DanhMucChiTieuId, TieuChiDanhGia, QuyTacDanhGia,
            TuTyLe, DenTyLe, XepLoai, DieuKienThoiHan, Diem, GhiChu, CreatedAt
        )
        VALUES
        (NULL, NULL, NULL, 0, 99.99, N'CHUA_HOAN_THANH', N'CHUA_DEN_HAN', 50, N'Demo: chưa đạt nhưng chưa đến hạn cuối.', @Now),
        (NULL, NULL, NULL, 0, 99.99, N'KHONG_HOAN_THANH', N'DA_DEN_HAN', 0, N'Demo: chưa đạt khi đã đến hạn.', @Now),
        (NULL, NULL, NULL, 100, 109.99, N'HOAN_THANH', N'MAC_DINH', 80, N'Demo: đạt mục tiêu.', @Now),
        (NULL, NULL, NULL, 110, 999999.99, N'HOAN_THANH_VUOT_MUC', N'MAC_DINH', 100, N'Demo: vượt mục tiêu.', @Now);
    END;

    SELECT TOP (1)
        @LoaiKy = LoaiKy
    FROM KyBaoCaoKPI
    WHERE Nam = @Nam
    GROUP BY LoaiKy
    ORDER BY
        CASE UPPER(LoaiKy)
            WHEN N'THANG' THEN 0
            WHEN N'QUY' THEN 1
            WHEN N'6THANG' THEN 2
            WHEN N'NAM' THEN 3
            ELSE 4
        END,
        COUNT(*) DESC;

    DROP TABLE IF EXISTS #PeriodPick;
    SELECT TOP (4)
        Id,
        MaKy,
        TenKy,
        LoaiKy,
        Nam,
        SoKy,
        TuNgay,
        DenNgay,
        NgayCuoiKy
    INTO #PeriodPick
    FROM KyBaoCaoKPI
    WHERE Nam = @Nam
      AND LoaiKy = @LoaiKy
    ORDER BY NgayCuoiKy DESC, Id DESC;

    DROP TABLE IF EXISTS #Periods;
    SELECT
        ROW_NUMBER() OVER (ORDER BY Nam, NgayCuoiKy, Id) AS PeriodSeq,
        *
    INTO #Periods
    FROM #PeriodPick;

    SELECT
        @PeriodCount = COUNT(*),
        @LatestPeriodSeq = MAX(PeriodSeq)
    FROM #Periods;

    IF @PeriodCount = 0
        THROW 51003, N'Không tìm thấy kỳ báo cáo phù hợp để tạo dữ liệu theo dõi demo.', 1;

    DROP TABLE IF EXISTS #LeafAssignments;
    SELECT
        a.Id AS ChiTietGiaoChiTieuId,
        a.DanhMucChiTieuId,
        c.MaChiTieu,
        c.LoaiChiTieu AS LoaiChiTieuDanhMuc,
        a.DonViNhanId,
        u.Seq AS UnitSeq,
        u.TenDonVi,
        a.GiaTriMucTieu,
        a.GiaTriDauKyCoDinh,
        a.TieuChiDanhGia,
        a.KieuSoSanh,
        a.QuyTacDanhGia
    INTO #LeafAssignments
    FROM ChiTietGiaoChiTieu a
    INNER JOIN DanhMucChiTieus c ON c.Id = a.DanhMucChiTieuId
    INNER JOIN #DemoUnits u ON u.Id = a.DonViNhanId
    WHERE a.DotGiaoChiTieuId = @DotGiaoId
      AND (
            c.MaChiTieu IN (SELECT MaChiTieu FROM #AssignmentConfig)
            OR c.MaChiTieu IN (SELECT MaChiTieu FROM #ChildAssignmentConfig)
      )
      AND NOT EXISTS (
          SELECT 1
          FROM ChiTietGiaoChiTieu child
          WHERE child.ChiTietGiaoChaId = a.Id
      );

    DROP TABLE IF EXISTS #TheoDoiPlan;
    SELECT
        leaf.ChiTietGiaoChiTieuId,
        p.Id AS KyBaoCaoKPIId,
        CAST(ISNULL(leaf.GiaTriDauKyCoDinh, 0) AS DECIMAL(18,2)) AS GiaTriDauKy,
        CAST(
            CASE
                WHEN leaf.TieuChiDanhGia = N'DINH_TINH'
                    THEN CASE WHEN (leaf.UnitSeq + p.PeriodSeq) % 7 = 0 THEN 0 ELSE 1 END
                WHEN leaf.KieuSoSanh = N'TY_LE'
                    THEN FLOOR(calc.Denominator * calc.RatioValue)
                WHEN leaf.QuyTacDanhGia = N'KHONG_VUOT_NGUONG'
                    THEN ROUND((leaf.GiaTriMucTieu / @PeriodCount) * calc.LowerIsBetterFactor, 0)
                ELSE ROUND((leaf.GiaTriMucTieu / @PeriodCount) * calc.ProgressFactor, 0)
            END
            AS DECIMAL(18,2)
        ) AS GiaTriThucHienTrongKy,
        CAST(
            CASE
                WHEN leaf.KieuSoSanh = N'TY_LE' THEN calc.Denominator
                ELSE NULL
            END
            AS DECIMAL(18,2)
        ) AS GiaTriPhatSinhTrongKy,
        CASE
            WHEN leaf.TieuChiDanhGia = N'DINH_TINH'
                THEN CASE WHEN (leaf.UnitSeq + p.PeriodSeq) % 7 = 0 THEN N'KHONG_DAM_BAO' ELSE N'DAM_BAO' END
            WHEN leaf.KieuSoSanh = N'TY_LE'
                THEN N'Tiếp nhận ' + CAST(calc.Denominator AS NVARCHAR(20)) + N' tin, giải quyết đúng hạn theo tỷ lệ thực tế.'
            WHEN leaf.QuyTacDanhGia = N'KHONG_VUOT_NGUONG'
                THEN N'Theo dõi phát sinh, mục tiêu là không vượt ngưỡng đã giao.'
            ELSE N'Đã cập nhật kết quả thực hiện theo tiến độ demo.'
        END AS NhanXet,
        CASE
            WHEN p.PeriodSeq = @LatestPeriodSeq
                 AND leaf.MaChiTieu = N'DEMO-ANTT-001'
                 AND leaf.UnitSeq = 1
                THEN N'TRA_LAI_NHAP_LAI'
            WHEN p.PeriodSeq = @LatestPeriodSeq
                 AND leaf.MaChiTieu = N'DEMO-TTTP-001'
                 AND leaf.UnitSeq = CASE WHEN @UnitCount >= 2 THEN 2 ELSE 1 END
                THEN N'CHO_XET_DUYET'
            ELSE N'DA_GHI_NHAN'
        END AS TrangThai
    INTO #TheoDoiPlan
    FROM #LeafAssignments leaf
    CROSS JOIN #Periods p
    CROSS APPLY (
        SELECT
            CAST(0.86 + (((leaf.UnitSeq + p.PeriodSeq) % 5) * 0.07) AS DECIMAL(18,4)) AS ProgressFactor,
            CAST(0.50 + (((leaf.UnitSeq + p.PeriodSeq) % 4) * 0.18) AS DECIMAL(18,4)) AS LowerIsBetterFactor,
            CAST(0.84 + (((leaf.UnitSeq + p.PeriodSeq) % 5) * 0.035) AS DECIMAL(18,4)) AS RatioValue,
            CAST(18 + leaf.UnitSeq * 3 + p.PeriodSeq * 2 AS DECIMAL(18,2)) AS Denominator
    ) calc
    WHERE NOT (
        p.PeriodSeq = @LatestPeriodSeq
        AND leaf.MaChiTieu = N'DEMO-PCCC-001'
        AND leaf.UnitSeq = @UnitCount
        AND @UnitCount > 1
    );

    MERGE TheoDoiThucHienKPI AS target
    USING #TheoDoiPlan AS source
    ON target.ChiTietGiaoChiTieuId = source.ChiTietGiaoChiTieuId
       AND target.KyBaoCaoKPIId = source.KyBaoCaoKPIId
    WHEN MATCHED THEN
        UPDATE SET
            GiaTriDauKy = source.GiaTriDauKy,
            GiaTriThucHienTrongKy = source.GiaTriThucHienTrongKy,
            GiaTriPhatSinhTrongKy = source.GiaTriPhatSinhTrongKy,
            GiaTriCuoiKy = CASE WHEN source.TrangThai = N'DA_GHI_NHAN' THEN source.GiaTriDauKy + source.GiaTriThucHienTrongKy ELSE 0 END,
            GiaTriLuyKe = CASE WHEN source.TrangThai = N'DA_GHI_NHAN' THEN source.GiaTriThucHienTrongKy ELSE 0 END,
            GiaTriPhatSinhLuyKe = CASE WHEN source.TrangThai = N'DA_GHI_NHAN' THEN ISNULL(source.GiaTriPhatSinhTrongKy, 0) ELSE 0 END,
            NhanXet = source.NhanXet,
            TrangThai = source.TrangThai,
            UpdatedAt = @Now,
            UpdatedBy = @Actor
    WHEN NOT MATCHED THEN
        INSERT
        (
            ChiTietGiaoChiTieuId, KyBaoCaoKPIId, GiaTriDauKy, GiaTriThucHienTrongKy,
            GiaTriPhatSinhTrongKy, GiaTriCuoiKy, GiaTriLuyKe, GiaTriPhatSinhLuyKe,
            NhanXet, TrangThai, CreatedAt, CreatedBy
        )
        VALUES
        (
            source.ChiTietGiaoChiTieuId, source.KyBaoCaoKPIId, source.GiaTriDauKy, source.GiaTriThucHienTrongKy,
            source.GiaTriPhatSinhTrongKy,
            CASE WHEN source.TrangThai = N'DA_GHI_NHAN' THEN source.GiaTriDauKy + source.GiaTriThucHienTrongKy ELSE 0 END,
            CASE WHEN source.TrangThai = N'DA_GHI_NHAN' THEN source.GiaTriThucHienTrongKy ELSE 0 END,
            CASE WHEN source.TrangThai = N'DA_GHI_NHAN' THEN ISNULL(source.GiaTriPhatSinhTrongKy, 0) ELSE 0 END,
            source.NhanXet, source.TrangThai, @Now, @Actor
        );

    ;WITH OrderedApproved AS
    (
        SELECT
            t.Id,
            leaf.GiaTriDauKyCoDinh,
            SUM(ISNULL(t.GiaTriThucHienTrongKy, 0)) OVER (
                PARTITION BY t.ChiTietGiaoChiTieuId
                ORDER BY p.Nam, p.NgayCuoiKy, p.Id
                ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
            ) AS GiaTriLuyKe,
            SUM(ISNULL(t.GiaTriPhatSinhTrongKy, 0)) OVER (
                PARTITION BY t.ChiTietGiaoChiTieuId
                ORDER BY p.Nam, p.NgayCuoiKy, p.Id
                ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW
            ) AS GiaTriPhatSinhLuyKe
        FROM TheoDoiThucHienKPI t
        INNER JOIN #LeafAssignments leaf ON leaf.ChiTietGiaoChiTieuId = t.ChiTietGiaoChiTieuId
        INNER JOIN KyBaoCaoKPI p ON p.Id = t.KyBaoCaoKPIId
        WHERE t.TrangThai = N'DA_GHI_NHAN'
    )
    UPDATE t
    SET
        GiaTriDauKy = ISNULL(o.GiaTriDauKyCoDinh, 0),
        GiaTriLuyKe = o.GiaTriLuyKe,
        GiaTriPhatSinhLuyKe = o.GiaTriPhatSinhLuyKe,
        GiaTriCuoiKy = ISNULL(o.GiaTriDauKyCoDinh, 0) + o.GiaTriLuyKe,
        UpdatedAt = @Now,
        UpdatedBy = @Actor
    FROM TheoDoiThucHienKPI t
    INNER JOIN OrderedApproved o ON o.Id = t.Id;

    UPDATE t
    SET
        GiaTriCuoiKy = 0,
        GiaTriLuyKe = 0,
        GiaTriPhatSinhLuyKe = 0,
        UpdatedAt = @Now,
        UpdatedBy = @Actor
    FROM TheoDoiThucHienKPI t
    INNER JOIN #LeafAssignments leaf ON leaf.ChiTietGiaoChiTieuId = t.ChiTietGiaoChiTieuId
    WHERE t.TrangThai IN (N'CHO_XET_DUYET', N'TRA_LAI_NHAP_LAI');

    DROP TABLE IF EXISTS #LeafEvalBase;
    SELECT
        t.ChiTietGiaoChiTieuId,
        t.KyBaoCaoKPIId,
        leaf.GiaTriMucTieu,
        t.GiaTriDauKy,
        t.GiaTriCuoiKy,
        CAST(NULL AS DECIMAL(18,2)) AS GiaTriCungKyNamTruoc,
        CASE
            WHEN leaf.TieuChiDanhGia = N'DINH_TINH'
                THEN CASE WHEN t.NhanXet IN (N'DAM_BAO', N'KHONG_XAY_RA', N'DAT_100') THEN 100 ELSE 0 END
            WHEN leaf.TieuChiDanhGia = N'DINH_LUONG_TICH_LUY' AND leaf.QuyTacDanhGia = N'KHONG_VUOT_NGUONG'
                THEN CASE
                    WHEN ISNULL(t.GiaTriLuyKe, 0) <= ISNULL(leaf.GiaTriMucTieu, 0) THEN 100
                    WHEN ISNULL(t.GiaTriLuyKe, 0) = 0 THEN NULL
                    ELSE (leaf.GiaTriMucTieu / NULLIF(t.GiaTriLuyKe, 0)) * 100
                END
            WHEN leaf.TieuChiDanhGia = N'DINH_LUONG_SO_SANH' AND leaf.KieuSoSanh = N'TY_LE'
                THEN CASE
                    WHEN ISNULL(t.GiaTriPhatSinhLuyKe, 0) = 0 OR ISNULL(leaf.GiaTriMucTieu, 0) = 0 THEN NULL
                    ELSE ((t.GiaTriLuyKe / NULLIF(t.GiaTriPhatSinhLuyKe, 0)) * 100 / leaf.GiaTriMucTieu) * 100
                END
            ELSE CASE
                    WHEN ISNULL(leaf.GiaTriMucTieu, 0) = 0 THEN NULL
                    ELSE (t.GiaTriLuyKe / leaf.GiaTriMucTieu) * 100
                END
        END AS TyLeHoanThanh,
        t.NhanXet,
        p.NgayCuoiKy
    INTO #LeafEvalBase
    FROM TheoDoiThucHienKPI t
    INNER JOIN #LeafAssignments leaf ON leaf.ChiTietGiaoChiTieuId = t.ChiTietGiaoChiTieuId
    INNER JOIN KyBaoCaoKPI p ON p.Id = t.KyBaoCaoKPIId
    WHERE t.TrangThai = N'DA_GHI_NHAN';

    DROP TABLE IF EXISTS #LeafEvalPlan;
    SELECT
        ChiTietGiaoChiTieuId,
        KyBaoCaoKPIId,
        GiaTriMucTieu,
        GiaTriDauKy,
        GiaTriCuoiKy,
        GiaTriCungKyNamTruoc,
        GiaTriCuoiKy - GiaTriDauKy AS ChenhLechSoVoiDauKy,
        CASE WHEN ISNULL(GiaTriDauKy, 0) = 0 THEN NULL ELSE ((GiaTriCuoiKy - GiaTriDauKy) / GiaTriDauKy) * 100 END AS TyLeTangTruongSoVoiDauKy,
        CAST(NULL AS DECIMAL(18,2)) AS ChenhLechSoVoiCungKyNamTruoc,
        CAST(NULL AS DECIMAL(18,2)) AS TyLeTangTruongSoVoiCungKyNamTruoc,
        CAST(ROUND(TyLeHoanThanh, 2) AS DECIMAL(18,2)) AS TyLeHoanThanh,
        CASE
            WHEN TyLeHoanThanh IS NULL THEN N'CHUA_DANH_GIA'
            WHEN TyLeHoanThanh >= 110 THEN N'HOAN_THANH_VUOT_MUC'
            WHEN TyLeHoanThanh >= 100 THEN N'HOAN_THANH'
            WHEN NgayCuoiKy < @NgayKetThuc THEN N'CHUA_HOAN_THANH'
            ELSE N'KHONG_HOAN_THANH'
        END AS XepLoai,
        NhanXet
    INTO #LeafEvalPlan
    FROM #LeafEvalBase;

    MERGE DanhGiaKPI AS target
    USING (
        SELECT
            *,
            CASE XepLoai
                WHEN N'HOAN_THANH_VUOT_MUC' THEN N'Hoàn thành vượt mức'
                WHEN N'HOAN_THANH' THEN N'Hoàn thành'
                WHEN N'CHUA_HOAN_THANH' THEN N'Chưa hoàn thành'
                WHEN N'KHONG_HOAN_THANH' THEN N'Không hoàn thành'
                ELSE N'Chưa đánh giá'
            END AS KetQua
        FROM #LeafEvalPlan
    ) AS source
    ON target.ChiTietGiaoChiTieuId = source.ChiTietGiaoChiTieuId
       AND target.KyBaoCaoKPIId = source.KyBaoCaoKPIId
    WHEN MATCHED THEN
        UPDATE SET
            GiaTriMucTieu = source.GiaTriMucTieu,
            GiaTriDauKy = source.GiaTriDauKy,
            GiaTriCuoiKy = source.GiaTriCuoiKy,
            GiaTriCungKyNamTruoc = source.GiaTriCungKyNamTruoc,
            ChenhLechSoVoiDauKy = source.ChenhLechSoVoiDauKy,
            TyLeTangTruongSoVoiDauKy = source.TyLeTangTruongSoVoiDauKy,
            ChenhLechSoVoiCungKyNamTruoc = source.ChenhLechSoVoiCungKyNamTruoc,
            TyLeTangTruongSoVoiCungKyNamTruoc = source.TyLeTangTruongSoVoiCungKyNamTruoc,
            TyLeHoanThanh = source.TyLeHoanThanh,
            XepLoai = source.XepLoai,
            KetQua = source.KetQua,
            NhanXetDanhGia = source.NhanXet,
            NguoiDanhGia = @Actor,
            NgayDanhGia = @Now,
            UpdatedAt = @Now
    WHEN NOT MATCHED THEN
        INSERT
        (
            ChiTietGiaoChiTieuId, KyBaoCaoKPIId, GiaTriMucTieu, GiaTriDauKy,
            GiaTriCuoiKy, GiaTriCungKyNamTruoc, ChenhLechSoVoiDauKy,
            TyLeTangTruongSoVoiDauKy, ChenhLechSoVoiCungKyNamTruoc,
            TyLeTangTruongSoVoiCungKyNamTruoc, TyLeHoanThanh, XepLoai, KetQua,
            NhanXetDanhGia, NguoiDanhGia, NgayDanhGia, CreatedAt
        )
        VALUES
        (
            source.ChiTietGiaoChiTieuId, source.KyBaoCaoKPIId, source.GiaTriMucTieu, source.GiaTriDauKy,
            source.GiaTriCuoiKy, source.GiaTriCungKyNamTruoc, source.ChenhLechSoVoiDauKy,
            source.TyLeTangTruongSoVoiDauKy, source.ChenhLechSoVoiCungKyNamTruoc,
            source.TyLeTangTruongSoVoiCungKyNamTruoc, source.TyLeHoanThanh, source.XepLoai, source.KetQua,
            source.NhanXet, @Actor, @Now, @Now
        );

    DROP TABLE IF EXISTS #ParentEvalBase;
    SELECT
        parent.Id AS ChiTietGiaoChiTieuId,
        p.Id AS KyBaoCaoKPIId,
        COUNT(child.Id) AS SoTieuChiCon,
        COUNT(dg.Id) AS SoTieuChiConDaDanhGia,
        SUM(CASE WHEN dg.XepLoai IN (N'HOAN_THANH', N'HOAN_THANH_VUOT_MUC') THEN 1 ELSE 0 END) AS SoTieuChiConHoanThanh,
        SUM(CASE WHEN dg.XepLoai = N'HOAN_THANH' THEN 1 ELSE 0 END) AS SoTieuChiConHoanThanhThuong,
        MIN(dg.TyLeHoanThanh) AS TyLeMin,
        AVG(dg.TyLeHoanThanh) AS TyLeAvg,
        SUM(ISNULL(dg.GiaTriMucTieu, 0)) AS GiaTriMucTieu,
        SUM(ISNULL(dg.GiaTriDauKy, 0)) AS GiaTriDauKy,
        SUM(ISNULL(dg.GiaTriCuoiKy, 0)) AS GiaTriCuoiKy,
        SUM(ISNULL(dg.GiaTriCungKyNamTruoc, 0)) AS GiaTriCungKyNamTruoc,
        p.NgayCuoiKy
    INTO #ParentEvalBase
    FROM ChiTietGiaoChiTieu parent
    INNER JOIN #CatalogMap parentCatalog ON parentCatalog.Id = parent.DanhMucChiTieuId
    INNER JOIN ChiTietGiaoChiTieu child ON child.ChiTietGiaoChaId = parent.Id
    INNER JOIN KyBaoCaoKPI p ON p.Id IN (SELECT Id FROM #Periods)
    LEFT JOIN DanhGiaKPI dg
        ON dg.ChiTietGiaoChiTieuId = child.Id
       AND dg.KyBaoCaoKPIId = p.Id
    WHERE parent.DotGiaoChiTieuId = @DotGiaoId
      AND parentCatalog.MaChiTieu = N'DEMO-GT-001'
    GROUP BY parent.Id, p.Id, p.NgayCuoiKy;

    DROP TABLE IF EXISTS #ParentEvalPlan;
    SELECT
        ChiTietGiaoChiTieuId,
        KyBaoCaoKPIId,
        NULLIF(GiaTriMucTieu, 0) AS GiaTriMucTieu,
        NULLIF(GiaTriDauKy, 0) AS GiaTriDauKy,
        NULLIF(GiaTriCuoiKy, 0) AS GiaTriCuoiKy,
        NULLIF(GiaTriCungKyNamTruoc, 0) AS GiaTriCungKyNamTruoc,
        NULLIF(GiaTriCuoiKy, 0) - NULLIF(GiaTriDauKy, 0) AS ChenhLechSoVoiDauKy,
        CASE WHEN GiaTriDauKy = 0 THEN NULL ELSE ((GiaTriCuoiKy - GiaTriDauKy) / GiaTriDauKy) * 100 END AS TyLeTangTruongSoVoiDauKy,
        CAST(NULL AS DECIMAL(18,2)) AS ChenhLechSoVoiCungKyNamTruoc,
        CAST(NULL AS DECIMAL(18,2)) AS TyLeTangTruongSoVoiCungKyNamTruoc,
        CAST(ROUND(TyLeMin, 2) AS DECIMAL(18,2)) AS TyLeHoanThanh,
        CASE
            WHEN SoTieuChiConDaDanhGia < SoTieuChiCon THEN N'CHUA_DANH_GIA'
            WHEN SoTieuChiConHoanThanh = SoTieuChiCon
                THEN CASE WHEN SoTieuChiConHoanThanhThuong > 0 THEN N'HOAN_THANH' ELSE N'HOAN_THANH_VUOT_MUC' END
            WHEN NgayCuoiKy < @NgayKetThuc THEN N'CHUA_HOAN_THANH'
            ELSE N'KHONG_HOAN_THANH'
        END AS XepLoai,
        CONCAT(N'Tổng hợp tự động từ ', SoTieuChiConDaDanhGia, N'/', SoTieuChiCon, N' tiêu chí con.') AS NhanXetDanhGia
    INTO #ParentEvalPlan
    FROM #ParentEvalBase;

    MERGE DanhGiaKPI AS target
    USING (
        SELECT
            *,
            CASE XepLoai
                WHEN N'HOAN_THANH_VUOT_MUC' THEN N'Hoàn thành vượt mức'
                WHEN N'HOAN_THANH' THEN N'Hoàn thành'
                WHEN N'CHUA_HOAN_THANH' THEN N'Chưa hoàn thành'
                WHEN N'KHONG_HOAN_THANH' THEN N'Không hoàn thành'
                ELSE N'Chưa đánh giá'
            END AS KetQua
        FROM #ParentEvalPlan
    ) AS source
    ON target.ChiTietGiaoChiTieuId = source.ChiTietGiaoChiTieuId
       AND target.KyBaoCaoKPIId = source.KyBaoCaoKPIId
    WHEN MATCHED THEN
        UPDATE SET
            GiaTriMucTieu = source.GiaTriMucTieu,
            GiaTriDauKy = source.GiaTriDauKy,
            GiaTriCuoiKy = source.GiaTriCuoiKy,
            GiaTriCungKyNamTruoc = source.GiaTriCungKyNamTruoc,
            ChenhLechSoVoiDauKy = source.ChenhLechSoVoiDauKy,
            TyLeTangTruongSoVoiDauKy = source.TyLeTangTruongSoVoiDauKy,
            ChenhLechSoVoiCungKyNamTruoc = source.ChenhLechSoVoiCungKyNamTruoc,
            TyLeTangTruongSoVoiCungKyNamTruoc = source.TyLeTangTruongSoVoiCungKyNamTruoc,
            TyLeHoanThanh = source.TyLeHoanThanh,
            XepLoai = source.XepLoai,
            KetQua = source.KetQua,
            NhanXetDanhGia = source.NhanXetDanhGia,
            NguoiDanhGia = @Actor,
            NgayDanhGia = @Now,
            UpdatedAt = @Now
    WHEN NOT MATCHED THEN
        INSERT
        (
            ChiTietGiaoChiTieuId, KyBaoCaoKPIId, GiaTriMucTieu, GiaTriDauKy,
            GiaTriCuoiKy, GiaTriCungKyNamTruoc, ChenhLechSoVoiDauKy,
            TyLeTangTruongSoVoiDauKy, ChenhLechSoVoiCungKyNamTruoc,
            TyLeTangTruongSoVoiCungKyNamTruoc, TyLeHoanThanh, XepLoai, KetQua,
            NhanXetDanhGia, NguoiDanhGia, NgayDanhGia, CreatedAt
        )
        VALUES
        (
            source.ChiTietGiaoChiTieuId, source.KyBaoCaoKPIId, source.GiaTriMucTieu, source.GiaTriDauKy,
            source.GiaTriCuoiKy, source.GiaTriCungKyNamTruoc, source.ChenhLechSoVoiDauKy,
            source.TyLeTangTruongSoVoiDauKy, source.ChenhLechSoVoiCungKyNamTruoc,
            source.TyLeTangTruongSoVoiCungKyNamTruoc, source.TyLeHoanThanh, source.XepLoai, source.KetQua,
            source.NhanXetDanhGia, @Actor, @Now, @Now
        );

    COMMIT TRANSACTION;

    SELECT
        @Nam AS NamDemo,
        @LoaiKy AS LoaiKyDemo,
        @UnitCount AS SoDonViDemo,
        (SELECT COUNT(*) FROM #CatalogMap) AS SoDanhMucChiTieuDemo,
        (SELECT COUNT(*) FROM ChiTietGiaoChiTieu WHERE DotGiaoChiTieuId = @DotGiaoId) AS SoChiTietGiaoChiTieuDemo,
        (SELECT COUNT(*) FROM #TheoDoiPlan) AS SoBaoCaoTheoDoiDemo,
        (SELECT COUNT(*) FROM DanhGiaKPI dg INNER JOIN ChiTietGiaoChiTieu ct ON ct.Id = dg.ChiTietGiaoChiTieuId WHERE ct.DotGiaoChiTieuId = @DotGiaoId) AS SoDanhGiaDemo,
        @MaDotGiao AS MaDotGiaoDemo;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
