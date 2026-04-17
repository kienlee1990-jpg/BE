$path = 'D:\Lê Trung Kiên\Source code\KPI Tracker UI\src\components\views\GiaoChiTieuChoDonViPage.vue'
$content = Get-Content -Path $path -Raw

$content = $content.Replace(
@'
    import BaseLayout from '../BaseLayout.vue'
    import { apiRequest } from '../../services/api.js'
'@,
@'
    import BaseLayout from '../BaseLayout.vue'
    import { apiRequest } from '../../services/api.js'
    import {
        CHIEU_SO_SANH_OPTIONS,
        LOAI_MOC_SO_SANH_OPTIONS,
        TIEU_CHI_DANH_GIA_OPTIONS,
        getLoaiMocSoSanhLabel,
        getTieuChiDanhGiaLabel,
        isDinhLuongSoSanhCriterion,
        isDinhTinhCriterion
    } from '../../utils/danhGiaStatusClean.js'
'@
)

$content = $content.Replace(
@'
        tanSuatBaoCao: '',
        giaTriMucTieu: '',
'@,
@'
        tanSuatBaoCao: '',
        tieuChiDanhGia: '',
        loaiMocSoSanh: '',
        chieuSoSanh: '',
        giaTriMucTieu: '',
'@
)

$content = $content.Replace(
@'
        loaiChiTieu: String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || 'DINH_TINH'),
        donViTinh: String(pick(item, 'donViTinh', 'DonViTinh') || ''),
'@,
@'
        loaiChiTieu: String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || 'DINH_TINH'),
        tieuChiDanhGia: String(pick(item, 'tieuChiDanhGia', 'TieuChiDanhGia', 'loaiChiTieu', 'LoaiChiTieu') || 'DINH_TINH'),
        donViTinh: String(pick(item, 'donViTinh', 'DonViTinh') || ''),
'@
)

$content = $content.Replace(
@'
        loaiChiTieu: String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || ''),
        chiTieuChaId: pick(item, 'chiTieuChaId', 'ChiTieuChaId'),
'@,
@'
        loaiChiTieu: String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || ''),
        tieuChiDanhGia: String(pick(item, 'tieuChiDanhGia', 'TieuChiDanhGia', 'loaiChiTieu', 'LoaiChiTieu') || ''),
        chiTieuChaId: pick(item, 'chiTieuChaId', 'ChiTieuChaId'),
'@
)

$content = $content.Replace(
@'
        loaiChiTieu: String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || 'DINH_TINH'),
        donViTinh: String(pick(item, 'donViTinh', 'DonViTinh') || ''),
        giaTriMucTieu: pick(item, 'giaTriMucTieu', 'GiaTriMucTieu'),
'@,
@'
        loaiChiTieu: String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || 'DINH_TINH'),
        tieuChiDanhGia: String(pick(item, 'tieuChiDanhGia', 'TieuChiDanhGia', 'loaiChiTieu', 'LoaiChiTieu') || 'DINH_TINH'),
        loaiMocSoSanh: String(pick(item, 'loaiMocSoSanh', 'LoaiMocSoSanh') || ''),
        chieuSoSanh: String(pick(item, 'chieuSoSanh', 'ChieuSoSanh') || ''),
        donViTinh: String(pick(item, 'donViTinh', 'DonViTinh') || ''),
        giaTriMucTieu: pick(item, 'giaTriMucTieu', 'GiaTriMucTieu'),
'@
)

$content = $content.Replace(
@'
        loaiChiTieu: String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || ''),
        donViTinh: String(pick(item, 'donViTinh', 'DonViTinh') || ''),
        tanSuatBaoCao: String(pick(item, 'tanSuatBaoCao', 'TanSuatBaoCao') || ''),
'@,
@'
        loaiChiTieu: String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || ''),
        tieuChiDanhGia: String(pick(item, 'tieuChiDanhGia', 'TieuChiDanhGia', 'loaiChiTieu', 'LoaiChiTieu') || ''),
        loaiMocSoSanh: String(pick(item, 'loaiMocSoSanh', 'LoaiMocSoSanh') || ''),
        chieuSoSanh: String(pick(item, 'chieuSoSanh', 'ChieuSoSanh') || ''),
        donViTinh: String(pick(item, 'donViTinh', 'DonViTinh') || ''),
        tanSuatBaoCao: String(pick(item, 'tanSuatBaoCao', 'TanSuatBaoCao') || ''),
'@
)

$content = $content.Replace(
@'
    const mapLoai = (value) => {
        const map = {
            DINH_TINH: 'Định tính',
            DINH_LUONG_TICH_LUY: 'Định lượng tích lũy',
            DINH_LUONG_SO_SANH: 'Định lượng so sánh',
            PHAN_RA: 'Phân rã'
        }
        return map[value] || value || '-'
    }

    const isQualitative = (loaiChiTieu) => loaiChiTieu === 'DINH_TINH'
'@,
@'
    const normalizeCode = (value) => String(value || '').trim().toUpperCase()
    const getEffectiveCriterion = (item) => normalizeCode(item?.tieuChiDanhGia || item?.loaiChiTieu)
    const getEffectiveLoaiMocSoSanh = (item) => normalizeCode(item?.loaiMocSoSanh)
    const getEffectiveChieuSoSanh = (item) => normalizeCode(item?.chieuSoSanh)

    const mapLoai = (value) => {
        if (normalizeCode(value) === 'PHAN_RA') {
            return 'Phân rã'
        }

        return getTieuChiDanhGiaLabel(value)
    }

    const isQualitative = (value) => isDinhTinhCriterion(value)
    const isComparisonCriterion = (value) => isDinhLuongSoSanhCriterion(value)
'@
)

$content = $content.Replace(
@'
            KY_TRUOC: 'kỳ trước'
'@,
@'
            KY_TRUOC: 'kỳ trước',
            TONG_NAM_TRUOC: 'tổng năm trước'
'@
)

$content = $content.Replace(
@'
    const buildCatalogHint = (item) => {
        if (!item) return ''

        if (item.loaiChiTieu === 'DINH_TINH') {
            return `Điều kiện hoàn thành: ${item.dieuKienHoanThanh || 'chưa khai báo'}`
        }

        if (item.loaiChiTieu === 'DINH_LUONG_SO_SANH') {
            return `Mục tiêu ${item.chieuSoSanh === 'GIAM' ? 'giảm' : 'tăng'} ${item.tyLePhanTramMucTieu || 0}% so với ${getMocSoSanhLabel(item.loaiMocSoSanh)}`
        }

        return `Đánh giá theo giá trị định lượng${item.donViTinh ? ` (${item.donViTinh})` : ''}`
    }
'@,
@'
    const buildCatalogHint = (item) => {
        if (!item) return ''

        const criterion = getEffectiveCriterion(item)
        if (criterion === 'DINH_TINH') {
            return `Điều kiện hoàn thành: ${item.dieuKienHoanThanh || 'chưa khai báo'}`
        }

        if (criterion === 'DINH_LUONG_SO_SANH') {
            const direction = getEffectiveChieuSoSanh(item) === 'GIAM' ? 'giảm' : 'tăng'
            const target = item.tyLePhanTramMucTieu || item.giaTriMucTieu || 0
            return `Mục tiêu ${direction} ${target}% so với ${getLoaiMocSoSanhLabel(getEffectiveLoaiMocSoSanh(item))}`
        }

        return `Đánh giá theo giá trị định lượng${item.donViTinh ? ` (${item.donViTinh})` : ''}`
    }
'@
)

$hydratePattern = 'const hydrateTargetsFromDanhMuc = \(existingAssignment = null\) => \{[\s\S]*?\n    \}\n\n    watch\(\(\) => form\.danhMucChiTieuId'
$hydrateReplacement = @'
const hydrateTargetsFromDanhMuc = (existingAssignment = null) => {
        const danhMuc = selectedDanhMuc.value

        if (!danhMuc) {
            form.tieuChiDanhGia = ''
            form.loaiMocSoSanh = ''
            form.chieuSoSanh = ''
            form.giaTriMucTieu = ''
            form.giaTriDauKyCoDinh = ''
            form.giaTriMucTieuText = ''
            form.tieuChiCon = []
            return
        }

        form.tieuChiDanhGia = existingAssignment?.tieuChiDanhGia || danhMuc.tieuChiDanhGia || danhMuc.loaiChiTieu || 'DINH_TINH'
        form.loaiMocSoSanh = existingAssignment?.loaiMocSoSanh || danhMuc.loaiMocSoSanh || ''
        form.chieuSoSanh = existingAssignment?.chieuSoSanh || danhMuc.chieuSoSanh || ''

        if (danhMuc.tieuChiDanhGias.length > 0) {
            const assignmentByCatalogId = new Map((existingAssignment?.tieuChiCon || []).map(child => [child.danhMucChiTieuId, child]))
            form.giaTriMucTieu = ''
            form.giaTriMucTieuText = ''
            form.giaTriDauKyCoDinh = ''
            form.tieuChiCon = danhMuc.tieuChiDanhGias.map((criterion, index) => {
                const assigned = assignmentByCatalogId.get(criterion.id)
                return {
                    localKey: `${criterion.id}-${index}-${Math.random()}`,
                    danhMucChiTieuId: criterion.id,
                    maChiTieu: criterion.maChiTieu,
                    tenChiTieu: criterion.tenChiTieu,
                    loaiChiTieu: criterion.loaiChiTieu,
                    tieuChiDanhGia: assigned?.tieuChiDanhGia || criterion.tieuChiDanhGia || criterion.loaiChiTieu || 'DINH_TINH',
                    donViTinh: criterion.donViTinh || danhMuc.donViTinh,
                    giaTriMucTieu: assigned?.giaTriMucTieu ?? '',
                    giaTriDauKyCoDinh: assigned?.giaTriDauKyCoDinh ?? 0,
                    giaTriMucTieuText: assigned?.giaTriMucTieuText || '',
                    ghiChu: assigned?.ghiChu || '',
                    thuTuHienThi: assigned?.thuTuHienThi || criterion.thuTuHienThi || index + 1,
                    dieuKienHoanThanh: criterion.dieuKienHoanThanh,
                    dieuKienKhongHoanThanh: criterion.dieuKienKhongHoanThanh,
                    tyLePhanTramMucTieu: criterion.tyLePhanTramMucTieu,
                    loaiMocSoSanh: assigned?.loaiMocSoSanh || criterion.loaiMocSoSanh || '',
                    chieuSoSanh: assigned?.chieuSoSanh || criterion.chieuSoSanh || ''
                }
            })
            return
        }

        form.tieuChiCon = []
        form.giaTriMucTieu = existingAssignment?.giaTriMucTieu ?? ''
        form.giaTriDauKyCoDinh = existingAssignment?.giaTriDauKyCoDinh ?? 0
        form.giaTriMucTieuText = existingAssignment?.giaTriMucTieuText || ''
    }

    watch(() => form.danhMucChiTieuId
'@
$content = [regex]::Replace($content, $hydratePattern, $hydrateReplacement, [System.Text.RegularExpressions.RegexOptions]::Singleline)

$payloadPattern = 'const buildPayload = \(\) => \(\{[\s\S]*?\n    \}\)\n\n    const validateForm'
$payloadReplacement = @'
const buildPayload = () => ({
        dotGiaoChiTieuId: form.dotGiaoChiTieuId,
        danhMucChiTieuId: form.danhMucChiTieuId,
        donViNhanId: getResolvedDonViId(),
        donViThucHienChinhId: getResolvedDonViId(),
        tanSuatBaoCao: form.tanSuatBaoCao || null,
        tieuChiDanhGia: form.tieuChiDanhGia || null,
        loaiMocSoSanh: isComparisonCriterion(form.tieuChiDanhGia) ? form.loaiMocSoSanh || null : null,
        chieuSoSanh: isComparisonCriterion(form.tieuChiDanhGia) ? form.chieuSoSanh || null : null,
        giaTriMucTieu: selectedDanhMuc.value && !selectedDanhMuc.value.tieuChiDanhGias.length && !isQualitative(form.tieuChiDanhGia)
            ? Number(form.giaTriMucTieu)
            : null,
        giaTriDauKyCoDinh: selectedDanhMuc.value && !selectedDanhMuc.value.tieuChiDanhGias.length && !isQualitative(form.tieuChiDanhGia)
            ? Number(form.giaTriDauKyCoDinh)
            : null,
        giaTriMucTieuText: selectedDanhMuc.value && !selectedDanhMuc.value.tieuChiDanhGias.length && isQualitative(form.tieuChiDanhGia)
            ? form.giaTriMucTieuText?.trim() || null
            : null,
        ghiChu: form.ghiChu?.trim() || null,
        tieuChiCon: form.tieuChiCon.map((child) => ({
            danhMucChiTieuId: child.danhMucChiTieuId,
            tieuChiDanhGia: child.tieuChiDanhGia || null,
            loaiMocSoSanh: isComparisonCriterion(child.tieuChiDanhGia || child.loaiChiTieu) ? child.loaiMocSoSanh || null : null,
            chieuSoSanh: isComparisonCriterion(child.tieuChiDanhGia || child.loaiChiTieu) ? child.chieuSoSanh || null : null,
            giaTriMucTieu: isQualitative(child.tieuChiDanhGia || child.loaiChiTieu) ? null : Number(child.giaTriMucTieu),
            giaTriDauKyCoDinh: isQualitative(child.tieuChiDanhGia || child.loaiChiTieu) ? null : Number(child.giaTriDauKyCoDinh),
            giaTriMucTieuText: isQualitative(child.tieuChiDanhGia || child.loaiChiTieu) ? child.giaTriMucTieuText?.trim() || null : null,
            ghiChu: child.ghiChu?.trim() || null,
            thuTuHienThi: child.thuTuHienThi || null
        }))
    })

    const validateCriterionConfig = (criterion, source, direction, label) => {
        if (!criterion) {
            alert(`Vui lòng chọn tiêu chí đánh giá cho ${label}.`)
            return false
        }

        if (isComparisonCriterion(criterion)) {
            if (!source) {
                alert(`Vui lòng chọn mốc so sánh cho ${label}.`)
                return false
            }

            if (!direction) {
                alert(`Vui lòng chọn chiều so sánh cho ${label}.`)
                return false
            }
        }

        return true
    }

    const validateTargetValues = (target, label) => {
        const criterion = target.tieuChiDanhGia || target.loaiChiTieu
        if (!validateCriterionConfig(criterion, target.loaiMocSoSanh, target.chieuSoSanh, label)) {
            return false
        }

        if (isQualitative(criterion)) {
            if (!target.giaTriMucTieuText?.trim()) {
                alert(`Vui lòng nhập mô tả mục tiêu cho ${label}.`)
                return false
            }
            return true
        }

        if (target.giaTriMucTieu === '' || target.giaTriMucTieu === null || target.giaTriMucTieu === undefined) {
            alert(`Vui lòng nhập giá trị mục tiêu cho ${label}.`)
            return false
        }

        if (Number.isNaN(Number(target.giaTriMucTieu)) || Number(target.giaTriMucTieu) <= 0) {
            alert(`Giá trị mục tiêu của ${label} không hợp lệ.`)
            return false
        }

        if (target.giaTriDauKyCoDinh === '' || target.giaTriDauKyCoDinh === null || target.giaTriDauKyCoDinh === undefined) {
            alert(`Vui lòng nhập đầu kỳ cố định cho ${label}.`)
            return false
        }

        if (Number.isNaN(Number(target.giaTriDauKyCoDinh)) || Number(target.giaTriDauKyCoDinh) < 0) {
            alert(`Đầu kỳ cố định của ${label} không hợp lệ.`)
            return false
        }

        return true
    }

    const validateForm
'@
$content = [regex]::Replace($content, $payloadPattern, $payloadReplacement, [System.Text.RegularExpressions.RegexOptions]::Singleline)

$validatePattern = 'const validateForm = \(\) => \{[\s\S]*?\n    \}\n\n    const loadData = async'
$validateReplacement = @'
const validateForm = () => {
        if (!form.dotGiaoChiTieuId) {
            alert('Vui lòng chọn đợt giao chỉ tiêu.')
            return false
        }

        if (!form.danhMucChiTieuId) {
            alert('Vui lòng chọn danh mục chỉ tiêu.')
            return false
        }

        if (!getResolvedDonViId()) {
            alert(`Vui lòng chọn ${scopeMeta.value.receiverLabel.toLowerCase()}.`)
            return false
        }

        if (!form.tanSuatBaoCao) {
            alert('Vui lòng chọn kỳ báo cáo.')
            return false
        }

        if (!selectedDanhMuc.value) {
            alert('Không tìm thấy danh mục chỉ tiêu đã chọn.')
            return false
        }

        if (!validateCriterionConfig(form.tieuChiDanhGia, form.loaiMocSoSanh, form.chieuSoSanh, 'chỉ tiêu giao')) {
            return false
        }

        if (selectedDanhMuc.value.tieuChiDanhGias.length > 0) {
            for (let i = 0; i < form.tieuChiCon.length; i += 1) {
                if (!validateTargetValues(form.tieuChiCon[i], `tiêu chí con ${i + 1}`)) {
                    return false
                }
            }
            return true
        }

        return validateTargetValues(form, 'chỉ tiêu giao')
    }

    const loadData = async
'@
$content = [regex]::Replace($content, $validatePattern, $validateReplacement, [System.Text.RegularExpressions.RegexOptions]::Singleline)

$content = $content.Replace(
@'
            tanSuatBaoCao: item.tanSuatBaoCao,
            giaTriMucTieu: item.giaTriMucTieu ?? '',
'@,
@'
            tanSuatBaoCao: item.tanSuatBaoCao,
            tieuChiDanhGia: item.tieuChiDanhGia || item.loaiChiTieu || '',
            loaiMocSoSanh: item.loaiMocSoSanh || '',
            chieuSoSanh: item.chieuSoSanh || '',
            giaTriMucTieu: item.giaTriMucTieu ?? '',
'@
)

$content = $content.Replace("{{ mapLoai(item.loaiChiTieu) }}", "{{ mapLoai(item.tieuChiDanhGia || item.loaiChiTieu) }}")
$content = $content.Replace("{{ mapLoai(selectedDanhMuc.loaiChiTieu) }}", "{{ mapLoai(form.tieuChiDanhGia || selectedDanhMuc.tieuChiDanhGia || selectedDanhMuc.loaiChiTieu) }}")
$content = $content.Replace("{{ mapLoai(child.loaiChiTieu) }}", "{{ mapLoai(child.tieuChiDanhGia || child.loaiChiTieu) }}")
$content = $content.Replace("{{ buildCatalogHint(selectedDanhMuc) }}", "{{ buildCatalogHint({ ...selectedDanhMuc, tieuChiDanhGia: form.tieuChiDanhGia, loaiMocSoSanh: form.loaiMocSoSanh, chieuSoSanh: form.chieuSoSanh, giaTriMucTieu: form.giaTriMucTieu }) }}")
$content = $content.Replace("isQualitative(selectedDanhMuc.loaiChiTieu)", "isQualitative(form.tieuChiDanhGia)")
$content = $content.Replace("isQualitative(child.loaiChiTieu)", "isQualitative(child.tieuChiDanhGia || child.loaiChiTieu)")
$content = $content.Replace("!isQualitative(child.loaiChiTieu)", "!isQualitative(child.tieuChiDanhGia || child.loaiChiTieu)")
$content = $content.Replace("!isQualitative(selectedDanhMuc.value.loaiChiTieu)", "!isQualitative(form.tieuChiDanhGia)")
$content = $content.Replace("isQualitative(selectedDanhMuc.value.loaiChiTieu)", "isQualitative(form.tieuChiDanhGia)")

$content = $content.Replace(
@'
                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Loại đánh giá</label>
                                        <input :value="selectedDanhMuc ? mapLoai(selectedDanhMuc.loaiChiTieu) : '-'"
                                            type="text" class="form-control" readonly />
                                    </div>
'@,
@'
                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Tiêu chí đánh giá <span class="text-danger">*</span></label>
                                        <select v-model="form.tieuChiDanhGia" class="form-select">
                                            <option value="">Chọn tiêu chí đánh giá</option>
                                            <option v-for="item in TIEU_CHI_DANH_GIA_OPTIONS" :key="item.value" :value="item.value">
                                                {{ item.label }}
                                            </option>
                                        </select>
                                    </div>
'@
)

$content = $content.Replace(
@'
                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Phương thức giao</label>
                                        <input
                                            :value="selectedDanhMuc && selectedDanhMuc.tieuChiDanhGias.length ? 'Giao theo tiêu chí con' : 'Giao trực tiếp cho chỉ tiêu'"
                                            type="text" class="form-control" readonly />
                                    </div>
'@,
@'
                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Phương thức giao</label>
                                        <input
                                            :value="selectedDanhMuc && selectedDanhMuc.tieuChiDanhGias.length ? 'Giao theo tiêu chí con' : 'Giao trực tiếp cho chỉ tiêu'"
                                            type="text" class="form-control" readonly />
                                    </div>

                                    <div v-if="isComparisonCriterion(form.tieuChiDanhGia)" class="col-12 col-md-4">
                                        <label class="form-label">Mốc so sánh <span class="text-danger">*</span></label>
                                        <select v-model="form.loaiMocSoSanh" class="form-select">
                                            <option value="">Chọn mốc so sánh</option>
                                            <option v-for="item in LOAI_MOC_SO_SANH_OPTIONS" :key="item.value" :value="item.value">
                                                {{ item.label }}
                                            </option>
                                        </select>
                                    </div>

                                    <div v-if="isComparisonCriterion(form.tieuChiDanhGia)" class="col-12 col-md-4">
                                        <label class="form-label">Chiều so sánh <span class="text-danger">*</span></label>
                                        <select v-model="form.chieuSoSanh" class="form-select">
                                            <option value="">Chọn chiều so sánh</option>
                                            <option v-for="item in CHIEU_SO_SANH_OPTIONS" :key="item.value" :value="item.value">
                                                {{ item.label }}
                                            </option>
                                        </select>
                                    </div>
'@
)

$content = $content.Replace(
@'
                                                <div class="small text-muted mb-3">{{ buildCatalogHint(child) }}</div>

                                                <div class="row g-3">
'@,
@'
                                                <div class="small text-muted mb-3">{{ buildCatalogHint(child) }}</div>

                                                <div class="row g-3">
                                                    <div class="col-12 col-md-4">
                                                        <label class="form-label">Tiêu chí đánh giá <span class="text-danger">*</span></label>
                                                        <select v-model="child.tieuChiDanhGia" class="form-select">
                                                            <option value="">Chọn tiêu chí đánh giá</option>
                                                            <option v-for="item in TIEU_CHI_DANH_GIA_OPTIONS" :key="item.value" :value="item.value">
                                                                {{ item.label }}
                                                            </option>
                                                        </select>
                                                    </div>

                                                    <div v-if="isComparisonCriterion(child.tieuChiDanhGia || child.loaiChiTieu)" class="col-12 col-md-4">
                                                        <label class="form-label">Mốc so sánh <span class="text-danger">*</span></label>
                                                        <select v-model="child.loaiMocSoSanh" class="form-select">
                                                            <option value="">Chọn mốc so sánh</option>
                                                            <option v-for="item in LOAI_MOC_SO_SANH_OPTIONS" :key="item.value" :value="item.value">
                                                                {{ item.label }}
                                                            </option>
                                                        </select>
                                                    </div>

                                                    <div v-if="isComparisonCriterion(child.tieuChiDanhGia || child.loaiChiTieu)" class="col-12 col-md-4">
                                                        <label class="form-label">Chiều so sánh <span class="text-danger">*</span></label>
                                                        <select v-model="child.chieuSoSanh" class="form-select">
                                                            <option value="">Chọn chiều so sánh</option>
                                                            <option v-for="item in CHIEU_SO_SANH_OPTIONS" :key="item.value" :value="item.value">
                                                                {{ item.label }}
                                                            </option>
                                                        </select>
                                                    </div>
'@
)

Set-Content -Path $path -Value $content -Encoding UTF8

