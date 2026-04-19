import { computed, reactive, ref } from 'vue'
import { apiRequest } from '../services/api.js'

export const TARGET_EXECUTION_UNIT = 'Công an thành phố Đà Nẵng'

export function useCatpKpiData() {
  const loading = ref(false)
  const errorMessage = ref('')
  const chiTietRows = ref([])
  const danhGiaRows = ref([])
  const kyBaoCaoOptions = ref([])

  const filters = reactive({
    kyBaoCaoKPIId: '',
    donVi: '',
    xepLoai: '',
    keyword: ''
  })

  const allAssignments = computed(() => flattenAssignments(chiTietRows.value))

  const targetAssignments = computed(() => {
    return allAssignments.value.filter(item =>
      normalizeText(item.tenDonViThucHienChinh) === normalizeText(TARGET_EXECUTION_UNIT)
    )
  })

  const evaluationMap = computed(() => {
    const map = new Map()

    for (const item of danhGiaRows.value) {
      const key = Number(item.chiTietGiaoChiTieuId || item.ChiTietGiaoChiTieuId || 0)
      if (!key) continue

      if (!map.has(key)) {
        map.set(key, [])
      }

      map.get(key).push(item)
    }

    return map
  })

  const reportRows = computed(() => {
    return targetAssignments.value.map(item => {
      const evaluations = evaluationMap.value.get(item.id) || []
      const selectedEvaluation = resolveEvaluationForAssignment(evaluations, filters.kyBaoCaoKPIId)

      return {
        id: item.id,
        chiTietGiaoChiTieuId: item.id,
        dotGiaoChiTieuId: item.dotGiaoChiTieuId,
        maDotGiao: item.maDotGiao,
        tenDotGiaoChiTieu: item.tenDotGiaoChiTieu,
        maChiTieu: item.maChiTieu,
        tenDanhMucChiTieu: item.tenDanhMucChiTieu || item.tenChiTieu,
        tenChiTieu: item.tenChiTieu,
        tenChiTieuCha: item.tenChiTieuCha,
        tenChiTieuGiao: item.giaTriMucTieuText || item.tenChiTieuCha || '',
        donViTinh: item.donViTinh || '',
        tenDonViNhan: item.tenDonViNhan,
        tenDonViThucHienChinh: item.tenDonViThucHienChinh,
        tanSuatBaoCao: item.tanSuatBaoCao,
        giaTriMucTieu: item.giaTriMucTieu,
        giaTriMucTieuText: item.giaTriMucTieuText,
        kyBaoCaoKPIId: Number(selectedEvaluation?.kyBaoCaoKPIId || selectedEvaluation?.KyBaoCaoKPIId || 0),
        maKy: selectedEvaluation?.maKy || selectedEvaluation?.MaKy || '',
        tenKy: selectedEvaluation?.tenKy || selectedEvaluation?.TenKy || '',
        loaiKy: selectedEvaluation?.loaiKy || selectedEvaluation?.LoaiKy || '',
        xepLoai: selectedEvaluation?.xepLoai || selectedEvaluation?.XepLoai || 'CHUA_DANH_GIA',
        ketQua: selectedEvaluation?.ketQua || selectedEvaluation?.KetQua || '',
        nguoiDanhGia: selectedEvaluation?.nguoiDanhGia || selectedEvaluation?.NguoiDanhGia || '',
        nhanXetDanhGia: selectedEvaluation?.nhanXetDanhGia || selectedEvaluation?.NhanXetDanhGia || '',
        tyLeHoanThanh: getNumberOrNull(selectedEvaluation?.tyLeHoanThanh || selectedEvaluation?.TyLeHoanThanh),
        chenhLechSoVoiDauKy: getNumberOrNull(selectedEvaluation?.chenhLechSoVoiDauKy || selectedEvaluation?.ChenhLechSoVoiDauKy),
        tyLeTangTruongSoVoiDauKy: getNumberOrNull(selectedEvaluation?.tyLeTangTruongSoVoiDauKy || selectedEvaluation?.TyLeTangTruongSoVoiDauKy),
        chenhLechSoVoiCungKyNamTruoc: getNumberOrNull(selectedEvaluation?.chenhLechSoVoiCungKyNamTruoc || selectedEvaluation?.ChenhLechSoVoiCungKyNamTruoc),
        tyLeTangTruongSoVoiCungKyNamTruoc: getNumberOrNull(selectedEvaluation?.tyLeTangTruongSoVoiCungKyNamTruoc || selectedEvaluation?.TyLeTangTruongSoVoiCungKyNamTruoc)
      }
    })
  })

  const donViOptions = computed(() => {
    return [...new Set(reportRows.value.map(item => item.tenDonViNhan).filter(Boolean))]
      .sort((left, right) => left.localeCompare(right, 'vi'))
  })

  async function fetchData() {
    loading.value = true
    errorMessage.value = ''

    try {
      const [chiTietData, danhGiaData, kyData] = await Promise.all([
        apiRequest('/ChiTietGiaoChiTieu'),
        apiRequest('/DanhGiaKPI'),
        apiRequest('/KyBaoCaoKPI')
      ])

      chiTietRows.value = Array.isArray(chiTietData) ? chiTietData : []
      danhGiaRows.value = Array.isArray(danhGiaData) ? danhGiaData : []
      kyBaoCaoOptions.value = Array.isArray(kyData) ? sortKyBaoCaoOptions(kyData) : []
    } catch (error) {
      console.error(error)
      errorMessage.value = error.message || 'Không thể tải dữ liệu chỉ tiêu Công an thành phố.'
      chiTietRows.value = []
      danhGiaRows.value = []
      kyBaoCaoOptions.value = []
    } finally {
      loading.value = false
    }
  }

  function resetFilters() {
    filters.kyBaoCaoKPIId = ''
    filters.donVi = ''
    filters.xepLoai = ''
    filters.keyword = ''
    return fetchData()
  }

  return {
    donViOptions,
    errorMessage,
    fetchData,
    filters,
    kyBaoCaoOptions,
    loading,
    reportRows,
    resetFilters
  }
}

export function buildKyLabel(item) {
  const maKy = item?.maKy || item?.MaKy || ''
  const tenKy = item?.tenKy || item?.TenKy || ''
  return [maKy, tenKy].filter(Boolean).join(' - ') || tenKy || maKy || '-'
}

export function formatTarget(row) {
  if (row?.giaTriMucTieu !== null && row?.giaTriMucTieu !== undefined && row?.giaTriMucTieu !== '') {
    const formatted = Number(row.giaTriMucTieu).toLocaleString('vi-VN', {
      minimumFractionDigits: 0,
      maximumFractionDigits: 2
    })
    const unit = String(row?.donViTinh || '').trim()
    return unit ? `${formatted} ${unit}` : formatted
  }

  return row?.giaTriMucTieuText || '-'
}

export function normalizeText(value) {
  return String(value || '')
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/đ/g, 'd')
    .replace(/Đ/g, 'D')
    .toLowerCase()
    .trim()
}

function flattenAssignments(items, parent = null) {
  const flattened = []

  for (const item of items) {
    const current = {
      id: Number(item.id || item.Id || 0),
      dotGiaoChiTieuId: Number(item.dotGiaoChiTieuId || item.DotGiaoChiTieuId || 0),
      maDotGiao: item.maDotGiaoChiTieu || item.MaDotGiaoChiTieu || item.maDotGiao || item.MaDotGiao || '',
      maChiTieu: item.maChiTieu || item.MaChiTieu || item.maDanhMucChiTieu || item.MaDanhMucChiTieu || '',
      tenDanhMucChiTieu: item.tenDanhMucChiTieu || item.TenDanhMucChiTieu || item.tenChiTieu || item.TenChiTieu || '',
      tenChiTieu: item.tenChiTieu || item.TenChiTieu || item.tenDanhMucChiTieu || item.TenDanhMucChiTieu || '',
      donViTinh: item.donViTinh || item.DonViTinh || parent?.donViTinh || '',
      tenChiTieuCha: parent?.tenChiTieu || '',
      tenDotGiaoChiTieu: item.tenDotGiaoChiTieu || item.TenDotGiaoChiTieu || item.tenDotGiao || item.TenDotGiao || '',
      tenDonViNhan: item.tenDonViNhan || item.TenDonViNhan || '',
      tenDonViThucHienChinh: item.tenDonViThucHienChinh || item.TenDonViThucHienChinh || '',
      tanSuatBaoCao: item.tanSuatBaoCao || item.TanSuatBaoCao || '',
      giaTriMucTieu: getNumberOrNull(item.giaTriMucTieu ?? item.GiaTriMucTieu),
      giaTriMucTieuText: item.giaTriMucTieuText || item.GiaTriMucTieuText || ''
    }

    flattened.push(current)

    const children = Array.isArray(item.tieuChiCon || item.TieuChiCon)
      ? (item.tieuChiCon || item.TieuChiCon)
      : []

    if (children.length) {
      flattened.push(...flattenAssignments(children, current))
    }
  }

  return flattened
}

function resolveEvaluationForAssignment(evaluations, selectedKyBaoCaoKPIId) {
  if (!Array.isArray(evaluations) || evaluations.length === 0) return null

  if (selectedKyBaoCaoKPIId) {
    return evaluations.find(item =>
      String(item.kyBaoCaoKPIId || item.KyBaoCaoKPIId || '') === String(selectedKyBaoCaoKPIId)
    ) || null
  }

  return [...evaluations].sort(compareEvaluationOrder).at(-1) || null
}

function compareEvaluationOrder(left, right) {
  const leftKy = extractKySortInfo(left)
  const rightKy = extractKySortInfo(right)

  if (leftKy.nam !== rightKy.nam) return leftKy.nam - rightKy.nam
  if (leftKy.loaiKyOrder !== rightKy.loaiKyOrder) return leftKy.loaiKyOrder - rightKy.loaiKyOrder
  if (leftKy.soKy !== rightKy.soKy) return leftKy.soKy - rightKy.soKy

  const leftDate = parseDate(left.ngayDanhGia || left.NgayDanhGia || left.updatedAt || left.UpdatedAt || left.createdAt || left.CreatedAt)
  const rightDate = parseDate(right.ngayDanhGia || right.NgayDanhGia || right.updatedAt || right.UpdatedAt || right.createdAt || right.CreatedAt)

  return (leftDate?.getTime() || 0) - (rightDate?.getTime() || 0)
}

function extractKySortInfo(item) {
  return {
    nam: Number(item?.nam || item?.Nam || 0),
    loaiKyOrder: getLoaiKyOrder(item?.loaiKy || item?.LoaiKy || ''),
    soKy: Number(item?.soKy || item?.SoKy || 0)
  }
}

function getLoaiKyOrder(loaiKy) {
  switch (normalizeText(loaiKy).toUpperCase()) {
    case 'THANG':
      return 1
    case 'QUY':
      return 2
    case '6THANG':
      return 3
    case 'NAM':
      return 4
    default:
      return 99
  }
}

function sortKyBaoCaoOptions(items) {
  return [...items].sort((left, right) => compareEvaluationOrder(left, right))
}

function parseDate(value) {
  if (!value) return null
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? null : date
}

function getNumberOrNull(value) {
  if (value === null || value === undefined || value === '') return null
  const number = Number(value)
  return Number.isFinite(number) ? number : null
}
