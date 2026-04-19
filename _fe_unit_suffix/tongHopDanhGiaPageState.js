import { computed, onMounted, reactive, ref } from 'vue'
import httpClient from '../../services/httpClient'
import {
  countTrackedStatuses,
  getDanhGiaBadgeClass,
  getDanhGiaLabel,
  getThoiHanLabel
} from '../../utils/danhGiaStatusClean.js'

export function useTongHopDanhGiaPage() {
  const loading = ref(false)
  const errorMessage = ref('')

  const kyBaoCaoOptions = ref([])
  const theoDoiRows = ref([])
  const chiTietGiaoRows = ref([])
  const danhMucRows = ref([])
  const cauHinhDanhGiaRows = ref([])
  const dotGiaoRows = ref([])
  const donViRows = ref([])

  const filters = reactive({
    kyBaoCaoKPIId: '',
    keyword: ''
  })

  const kyBaoCaoById = computed(() => buildMapById(kyBaoCaoOptions.value))
  const danhMucById = computed(() => buildMapById(danhMucRows.value))
  const dotGiaoById = computed(() => buildMapById(dotGiaoRows.value))
  const donViById = computed(() => buildMapById(donViRows.value))

  const selectedKy = computed(() => {
    const selectedId = Number(filters.kyBaoCaoKPIId || 0)
    return selectedId > 0 ? kyBaoCaoById.value.get(selectedId) || null : null
  })

  const normalizedEvaluationConfigs = computed(() => {
    return cauHinhDanhGiaRows.value
      .map(item => ({
        danhMucChiTieuId: getNumberOrNull(pick(item, 'danhMucChiTieuId', 'DanhMucChiTieuId')),
        tuTyLe: getNumberOrNull(pick(item, 'tuTyLe', 'TuTyLe')),
        denTyLe: getNumberOrNull(pick(item, 'denTyLe', 'DenTyLe')),
        dieuKienThoiHan: normalizeCode(pick(item, 'dieuKienThoiHan', 'DieuKienThoiHan')) || 'MAC_DINH',
        xepLoai: pick(item, 'xepLoai', 'XepLoai') || ''
      }))
      .filter(item => item.xepLoai)
  })

  const flattenedAssignments = computed(() => {
    const flattened = []
    chiTietGiaoRows.value.forEach(item => appendFlattenedAssignment(flattened, item, null))
    return flattened
  })

  const assignmentById = computed(() => {
    const map = new Map()
    flattenedAssignments.value.forEach(item => {
      if (item.id > 0) map.set(item.id, item)
    })
    return map
  })

  onMounted(async () => {
    await fetchTongHopDanhGia()
  })

  async function fetchTongHopDanhGia() {
    loading.value = true
    errorMessage.value = ''

    const requests = [
      { key: 'kyBaoCao', label: 'kỳ báo cáo', required: false, request: () => httpClient.get('/KyBaoCaoKPI') },
      { key: 'theoDoi', label: 'theo dõi thực hiện KPI', required: true, request: () => httpClient.get('/TheoDoiThucHienKPI') },
      { key: 'chiTiet', label: 'chi tiết giao chỉ tiêu', required: true, request: () => httpClient.get('/ChiTietGiaoChiTieu') },
      { key: 'danhMuc', label: 'danh mục chỉ tiêu', required: false, request: () => httpClient.get('/DanhMucChiTieu') },
      { key: 'cauHinh', label: 'cấu hình đánh giá', required: false, request: () => httpClient.get('/cau-hinh-nguong-danh-gia-kpi') },
      { key: 'dotGiao', label: 'đợt giao chỉ tiêu', required: false, request: () => httpClient.get('/dot-giao-chi-tieu') },
      { key: 'donVi', label: 'đơn vị', required: false, request: () => httpClient.get('/DonVi') }
    ]

    try {
      const settled = await Promise.allSettled(requests.map(item => item.request()))
      const failedRequiredLabels = []

      settled.forEach((result, index) => {
        const current = requests[index]
        const normalized = result.status === 'fulfilled' ? normalizeList(result.value) : []

        if (result.status === 'rejected') {
          console.error(`Lỗi tải ${current.key}:`, result.reason)
          if (current.required) failedRequiredLabels.push(current.label)
        }

        if (current.key === 'kyBaoCao') kyBaoCaoOptions.value = normalized
        if (current.key === 'theoDoi') theoDoiRows.value = normalized
        if (current.key === 'chiTiet') chiTietGiaoRows.value = normalized
        if (current.key === 'danhMuc') danhMucRows.value = normalized
        if (current.key === 'cauHinh') cauHinhDanhGiaRows.value = normalized
        if (current.key === 'dotGiao') dotGiaoRows.value = normalized
        if (current.key === 'donVi') donViRows.value = normalized
      })

      if (failedRequiredLabels.length) {
        errorMessage.value = `Không thể tải dữ liệu từ ${failedRequiredLabels.join(', ')}.`
      }
    } finally {
      loading.value = false
    }
  }

  function resetFilters() {
    filters.kyBaoCaoKPIId = ''
    filters.keyword = ''
    fetchTongHopDanhGia()
  }

  const normalizedTheoDoiRows = computed(() => {
    const selectedKySort = selectedKy.value ? extractKySortInfo(selectedKy.value) : null

    return theoDoiRows.value
      .map(rawItem => {
        const chiTietGiaoChiTieuId = Number(pick(rawItem, 'chiTietGiaoChiTieuId', 'ChiTietGiaoChiTieuId') || 0)
        const kyBaoCaoKPIId = Number(pick(rawItem, 'kyBaoCaoKPIId', 'KyBaoCaoKPIId') || 0)
        const assignment = assignmentById.value.get(chiTietGiaoChiTieuId) || createFallbackAssignment(rawItem)
        const kyBaoCao = kyBaoCaoById.value.get(kyBaoCaoKPIId) || null
        const dotGiao = dotGiaoById.value.get(assignment.dotGiaoChiTieuId || 0) || null

        return {
          groupKey: String(chiTietGiaoChiTieuId),
          chiTietGiaoChiTieuId,
          kyBaoCaoKPIId,
          kySort: extractKySortInfo({ ...kyBaoCao, kyBaoCaoKPIId }),
          maChiTieu: assignment.maChiTieu || pick(rawItem, 'maChiTieu', 'MaChiTieu') || '',
          tenChiTieu: assignment.tenChiTieu || pick(rawItem, 'tenChiTieu', 'TenChiTieu') || '',
          tenChiTieuCha: assignment.tenChiTieuCha || '',
          tenDonViNhan: assignment.tenDonViNhan || '-',
          tenDotGiaoChiTieu: assignment.tenDotGiaoChiTieu || '-',
          donViTinh: assignment.donViTinh || '',
          danhMucChiTieuId: assignment.danhMucChiTieuId,
          loaiChiTieu: assignment.loaiChiTieu || '',
          giaTriMucTieu: assignment.giaTriMucTieu,
          giaTriDauKy: getNumberOrNull(pick(rawItem, 'giaTriDauKy', 'GiaTriDauKy')),
          giaTriCuoiKy: getNumberOrNull(pick(rawItem, 'giaTriCuoiKy', 'GiaTriCuoiKy')),
          giaTriLuyKe: getNumberOrNull(pick(rawItem, 'giaTriLuyKe', 'GiaTriLuyKe')),
          nhanXet: String(pick(rawItem, 'nhanXet', 'NhanXet') || '').trim(),
          maKy: pick(rawItem, 'maKy', 'MaKy') || pick(kyBaoCao, 'maKy', 'MaKy') || '',
          tenKy: pick(rawItem, 'tenKy', 'TenKy') || pick(kyBaoCao, 'tenKy', 'TenKy') || '',
          loaiKy: pick(kyBaoCao, 'loaiKy', 'LoaiKy', 'loaiKyBaoCao', 'LoaiKyBaoCao') || '',
          nam: pick(kyBaoCao, 'nam', 'Nam') ?? '',
          soKy: pick(kyBaoCao, 'soKy', 'SoKy') ?? '',
          dieuKienThoiHan: resolveDeadlineCondition(dotGiao, kyBaoCao),
          laChiTiet: !assignment.coTieuChiCon
        }
      })
      .filter(item => item.laChiTiet)
      .filter(item => !selectedKySort || compareKySort(item.kySort, selectedKySort) <= 0)
  })

  const groupedRows = computed(() => {
    const map = new Map()

    normalizedTheoDoiRows.value.forEach(item => {
      const current = map.get(item.groupKey)

      if (!current) {
        map.set(item.groupKey, {
          ...item,
          latest: item
        })
        return
      }

      if (compareKySort(item.kySort, current.latest.kySort) > 0) {
        current.latest = item
      }
    })

    return Array.from(map.values())
      .map(group => finalizeGroup(group))
      .sort((a, b) => {
        const aValue = `${a.tenDotGiaoChiTieu || ''} ${a.tenDonViNhan || ''} ${a.maChiTieu || ''}`.toLowerCase()
        const bValue = `${b.tenDotGiaoChiTieu || ''} ${b.tenDonViNhan || ''} ${b.maChiTieu || ''}`.toLowerCase()
        return aValue.localeCompare(bValue, 'vi')
      })
  })

  const filteredRows = computed(() => {
    if (!filters.keyword) return groupedRows.value

    const keyword = normalizeText(filters.keyword)

    return groupedRows.value.filter(item =>
      [
        item.maChiTieu,
        item.tenChiTieu,
        item.tenChiTieuCha,
        item.tenDonViNhan,
        item.tenDotGiaoChiTieu,
        item.maKyGanNhat,
        item.tenKyGanNhat,
        getDanhGiaLabel(item.xepLoai),
        getThoiHanLabel(item.dieuKienThoiHan)
      ]
        .filter(Boolean)
        .some(value => normalizeText(value).includes(keyword))
    )
  })

  const thongKe = computed(() => countTrackedStatuses(filteredRows.value, item => item.xepLoai))

  function finalizeGroup(group) {
    const latest = group.latest
    const giaTriLuyKeHienTai = latest.giaTriLuyKe ?? latest.giaTriCuoiKy
    const tyLeHoanThanh = calculateCompletionRate(giaTriLuyKeHienTai, group.giaTriMucTieu, group.loaiChiTieu)
    const evaluation = calculateComputedEvaluation({
      danhMucChiTieuId: group.danhMucChiTieuId,
      loaiChiTieu: group.loaiChiTieu,
      tyLeHoanThanh,
      nhanXet: latest.nhanXet,
      dieuKienThoiHan: latest.dieuKienThoiHan
    })

    return {
      groupKey: group.groupKey,
      maChiTieu: group.maChiTieu || '-',
      tenChiTieu: group.tenChiTieu || '-',
      tenChiTieuCha: group.tenChiTieuCha || '',
      tenDonViNhan: group.tenDonViNhan || '-',
      tenDotGiaoChiTieu: group.tenDotGiaoChiTieu || '-',
      donViTinh: latest.donViTinh || group.donViTinh || '',
      giaTriMucTieu: group.giaTriMucTieu,
      giaTriDauKyGanNhat: latest.giaTriDauKy,
      giaTriCuoiKyGanNhat: latest.giaTriCuoiKy,
      giaTriLuyKeHienTai,
      soDuMucTieu:
        group.giaTriMucTieu === null || giaTriLuyKeHienTai === null
          ? null
          : group.giaTriMucTieu - giaTriLuyKeHienTai,
      tyLeHoanThanh,
      maKyGanNhat: latest.maKy || '-',
      tenKyGanNhat: latest.tenKy || '-',
      loaiKyGanNhat: latest.loaiKy || '',
      namGanNhat: latest.nam ?? '',
      soKyGanNhat: latest.soKy ?? '',
      dieuKienThoiHan: latest.dieuKienThoiHan,
      xepLoai: evaluation.xepLoai,
      nguonDanhGiaTinhToan: evaluation.nguon
    }
  }

  function calculateComputedEvaluation({ danhMucChiTieuId, loaiChiTieu, tyLeHoanThanh, nhanXet, dieuKienThoiHan }) {
    const normalizedLoaiChiTieu = normalizeCode(loaiChiTieu)

    if (normalizedLoaiChiTieu === 'DINH_TINH') {
      const qualitativeStatus = getQualitativeStatus(nhanXet)
      if (qualitativeStatus) {
        return { xepLoai: qualitativeStatus, nguon: 'Định tính' }
      }
    }

    if (Number.isFinite(Number(tyLeHoanThanh))) {
      const matchedConfig = findMatchingEvaluationConfig(
        danhMucChiTieuId,
        Number(tyLeHoanThanh),
        dieuKienThoiHan
      )

      if (matchedConfig?.xepLoai) {
        return { xepLoai: matchedConfig.xepLoai, nguon: 'Cấu hình ngưỡng' }
      }

      return {
        xepLoai: getDefaultStatusByCompletion(Number(tyLeHoanThanh)),
        nguon: 'Mặc định theo lũy kế'
      }
    }

    return {
      xepLoai: 'CHUA_DANH_GIA',
      nguon: 'Chưa có dữ liệu'
    }
  }

  function getQualitativeStatus(value) {
    const code = normalizeCode(value)
    if (['KHONG_XAY_RA', 'DAM_BAO', 'DAT_100'].includes(code)) return 'HOAN_THANH'
    if (['XAY_RA', 'KHONG_DAM_BAO', 'KHONG_DAT'].includes(code)) return 'KHONG_HOAN_THANH'
    return ''
  }

  function getDefaultStatusByCompletion(value) {
    const rate = Number(value)
    if (!Number.isFinite(rate)) return 'CHUA_DANH_GIA'
    if (rate > 100) return 'HOAN_THANH_VUOT_MUC'
    if (rate >= 100) return 'HOAN_THANH'
    if (rate > 0) return 'CHUA_HOAN_THANH'
    return 'KHONG_HOAN_THANH'
  }

  function findMatchingEvaluationConfig(danhMucChiTieuId, tyLeHoanThanh, dieuKienThoiHan) {
    const targetDanhMucId = Number(danhMucChiTieuId || 0)
    const targetCondition = normalizeCode(dieuKienThoiHan) || 'MAC_DINH'

    const candidates = normalizedEvaluationConfigs.value.filter(item => {
      const configDanhMucId = Number(item.danhMucChiTieuId || 0)
      const sameDanhMuc = !configDanhMucId || configDanhMucId === targetDanhMucId
      if (!sameDanhMuc) return false

      const configCondition = normalizeCode(item.dieuKienThoiHan) || 'MAC_DINH'
      if (configCondition !== targetCondition && configCondition !== 'MAC_DINH') return false

      const tuTyLe = item.tuTyLe ?? -Infinity
      const denTyLe = item.denTyLe ?? Infinity
      return tyLeHoanThanh >= tuTyLe && tyLeHoanThanh <= denTyLe
    })

    if (!candidates.length) return null

    return [...candidates].sort((a, b) => {
      const priorityDiff =
        getConfigPriority(b, targetDanhMucId, targetCondition) -
        getConfigPriority(a, targetDanhMucId, targetCondition)
      if (priorityDiff !== 0) return priorityDiff

      const fromDiff = (b.tuTyLe ?? -Infinity) - (a.tuTyLe ?? -Infinity)
      if (fromDiff !== 0) return fromDiff

      return (a.denTyLe ?? Infinity) - (b.denTyLe ?? Infinity)
    })[0]
  }

  function getConfigPriority(item, targetDanhMucId, targetCondition) {
    let priority = 0
    const configDanhMucId = Number(item.danhMucChiTieuId || 0)
    const configCondition = normalizeCode(item.dieuKienThoiHan) || 'MAC_DINH'

    if (configDanhMucId && configDanhMucId === targetDanhMucId) priority += 4
    else if (!configDanhMucId) priority += 1

    if (configCondition === targetCondition) priority += 2
    else if (configCondition === 'MAC_DINH') priority += 1

    return priority
  }

  function normalizeList(response) {
    if (Array.isArray(response)) return response
    if (Array.isArray(response?.data)) return response.data
    if (Array.isArray(response?.data?.data)) return response.data.data
    if (Array.isArray(response?.data?.items)) return response.data.items
    if (Array.isArray(response?.items)) return response.items
    return []
  }

  function pick(source, ...keys) {
    for (const key of keys) {
      if (source?.[key] !== undefined && source?.[key] !== null) return source[key]
    }
    return undefined
  }

  function getId(item) {
    const value = Number(pick(item, 'id', 'Id') || 0)
    return Number.isFinite(value) ? value : 0
  }

  function buildMapById(items) {
    const map = new Map()
    items.forEach(item => {
      const id = getId(item)
      if (id > 0) map.set(id, item)
    })
    return map
  }

  function normalizeText(value) {
    return String(value || '')
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/đ/g, 'd')
      .replace(/Đ/g, 'D')
      .trim()
      .toLowerCase()
  }

  function normalizeCode(value) {
    return String(value || '')
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/đ/g, 'd')
      .replace(/Đ/g, 'D')
      .trim()
      .toUpperCase()
      .replace(/\s+/g, '_')
  }

  function parseNumber(value) {
    if (value === null || value === undefined || value === '') return Number.NaN
    if (typeof value === 'number') return value

    const text = String(value).trim()
    if (!text) return Number.NaN

    const hasDot = text.includes('.')
    const hasComma = text.includes(',')
    let normalized = text

    if (hasDot && hasComma) normalized = text.replace(/\./g, '').replace(',', '.')
    else if (!hasDot && hasComma) normalized = text.replace(',', '.')

    const parsed = Number(normalized)
    return Number.isFinite(parsed) ? parsed : Number.NaN
  }

  function getNumberOrNull(value) {
    const parsed = parseNumber(value)
    return Number.isFinite(parsed) ? parsed : null
  }

  function parseDate(value) {
    if (!value) return null
    const date = new Date(value)
    return Number.isNaN(date.getTime()) ? null : date
  }

  function toDateOnly(value) {
    const date = parseDate(value)
    return date ? new Date(date.getFullYear(), date.getMonth(), date.getDate()) : null
  }

  function getLoaiKyOrder(loaiKy) {
    switch (normalizeCode(loaiKy)) {
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

  function extractKySortInfo(source) {
    return {
      kyBaoCaoKPIId: Number(pick(source, 'kyBaoCaoKPIId', 'KyBaoCaoKPIId', 'id', 'Id') || 0),
      nam: Number(pick(source, 'nam', 'Nam') || 0),
      loaiKy: pick(source, 'loaiKy', 'LoaiKy', 'loaiKyBaoCao', 'LoaiKyBaoCao') || '',
      soKy: Number(pick(source, 'soKy', 'SoKy') || 0),
      ngayCuoiKy: parseDate(pick(source, 'ngayCuoiKy', 'NgayCuoiKy'))
    }
  }

  function compareKySort(left, right) {
    const leftDate = left?.ngayCuoiKy
    const rightDate = right?.ngayCuoiKy

    if (leftDate && rightDate && leftDate.getTime() !== rightDate.getTime()) {
      return leftDate.getTime() - rightDate.getTime()
    }

    const namDiff = Number(left?.nam || 0) - Number(right?.nam || 0)
    if (namDiff !== 0) return namDiff

    const loaiKyDiff = getLoaiKyOrder(left?.loaiKy) - getLoaiKyOrder(right?.loaiKy)
    if (loaiKyDiff !== 0) return loaiKyDiff

    const soKyDiff = Number(left?.soKy || 0) - Number(right?.soKy || 0)
    if (soKyDiff !== 0) return soKyDiff

    return Number(left?.kyBaoCaoKPIId || 0) - Number(right?.kyBaoCaoKPIId || 0)
  }

  function appendFlattenedAssignment(target, rawItem, parentAssignment) {
    const current = normalizeAssignment(rawItem, parentAssignment)
    target.push(current)
    const children = normalizeList(pick(rawItem, 'tieuChiCon', 'TieuChiCon'))
    children.forEach(child => appendFlattenedAssignment(target, child, current))
  }

  function normalizeAssignment(rawItem, parentAssignment) {
    const children = normalizeList(pick(rawItem, 'tieuChiCon', 'TieuChiCon'))
    const id = getId(rawItem)
    const danhMucChiTieuId = Number(
      pick(rawItem, 'danhMucChiTieuId', 'DanhMucChiTieuId') || parentAssignment?.danhMucChiTieuId || 0
    )
    const dotGiaoChiTieuId = Number(
      pick(rawItem, 'dotGiaoChiTieuId', 'DotGiaoChiTieuId') || parentAssignment?.dotGiaoChiTieuId || 0
    )
    const donViNhanId = Number(
      pick(rawItem, 'donViNhanId', 'DonViNhanId') || parentAssignment?.donViNhanId || 0
    )

    const danhMuc = danhMucById.value.get(danhMucChiTieuId) || null
    const dotGiao = dotGiaoById.value.get(dotGiaoChiTieuId) || null
    const donVi = donViById.value.get(donViNhanId) || null

    return {
      id,
      coTieuChiCon: children.length > 0,
      danhMucChiTieuId: danhMucChiTieuId || null,
      dotGiaoChiTieuId,
      donViNhanId,
      maChiTieu: pick(rawItem, 'maChiTieu', 'MaChiTieu') || pick(danhMuc, 'maChiTieu', 'MaChiTieu') || '',
      tenChiTieu: pick(rawItem, 'tenChiTieu', 'TenChiTieu') || pick(danhMuc, 'tenChiTieu', 'TenChiTieu') || '',
      donViTinh:
        pick(rawItem, 'donViTinh', 'DonViTinh') ||
        pick(danhMuc, 'donViTinh', 'DonViTinh') ||
        parentAssignment?.donViTinh ||
        '',
      tenChiTieuCha: parentAssignment?.tenChiTieu || '',
      tenDotGiaoChiTieu:
        pick(rawItem, 'tenDotGiaoChiTieu', 'TenDotGiaoChiTieu') ||
        pick(dotGiao, 'tenDotGiao', 'TenDotGiao') ||
        parentAssignment?.tenDotGiaoChiTieu ||
        '-',
      tenDonViNhan:
        pick(rawItem, 'tenDonViNhan', 'TenDonViNhan') ||
        pick(donVi, 'tenDonVi', 'TenDonVi') ||
        parentAssignment?.tenDonViNhan ||
        '-',
      loaiChiTieu:
        normalizeCode(pick(rawItem, 'loaiChiTieu', 'LoaiChiTieu') || pick(danhMuc, 'loaiChiTieu', 'LoaiChiTieu')) || '',
      giaTriMucTieu: getNumberOrNull(pick(rawItem, 'giaTriMucTieu', 'GiaTriMucTieu'))
    }
  }

  function createFallbackAssignment(rawItem) {
    return {
      id: Number(pick(rawItem, 'chiTietGiaoChiTieuId', 'ChiTietGiaoChiTieuId') || 0),
      coTieuChiCon: false,
      danhMucChiTieuId: Number(pick(rawItem, 'danhMucChiTieuId', 'DanhMucChiTieuId') || 0) || null,
      dotGiaoChiTieuId: Number(pick(rawItem, 'dotGiaoChiTieuId', 'DotGiaoChiTieuId') || 0),
      donViNhanId: Number(pick(rawItem, 'donViNhanId', 'DonViNhanId') || 0),
      maChiTieu: pick(rawItem, 'maChiTieu', 'MaChiTieu') || '',
      tenChiTieu: pick(rawItem, 'tenChiTieu', 'TenChiTieu') || '',
      donViTinh: pick(rawItem, 'donViTinh', 'DonViTinh') || '',
      tenChiTieuCha: '',
      tenDotGiaoChiTieu:
        pick(rawItem, 'tenDotGiaoChiTieu', 'TenDotGiaoChiTieu', 'dotGiaoChiTieu', 'DotGiaoChiTieu') || '-',
      tenDonViNhan: pick(rawItem, 'tenDonViNhan', 'TenDonViNhan') || '-',
      loaiChiTieu: normalizeCode(pick(rawItem, 'loaiChiTieu', 'LoaiChiTieu')) || '',
      giaTriMucTieu: getNumberOrNull(pick(rawItem, 'giaTriMucTieu', 'GiaTriMucTieu'))
    }
  }

  function resolveDeadlineCondition(dotGiao, kyBaoCao) {
    const ngayKetThucDotGiao = toDateOnly(pick(dotGiao, 'ngayKetThuc', 'NgayKetThuc'))
    const ngayCuoiKy = toDateOnly(pick(kyBaoCao, 'ngayCuoiKy', 'NgayCuoiKy'))

    if (!ngayKetThucDotGiao || !ngayCuoiKy) return 'MAC_DINH'
    return ngayCuoiKy < ngayKetThucDotGiao ? 'CHUA_DEN_HAN' : 'DA_DEN_HAN'
  }

  function calculateCompletionRate(giaTriLuyKe, giaTriMucTieu, loaiChiTieu) {
    if (normalizeCode(loaiChiTieu) === 'DINH_TINH') return null
    const mucTieu = getNumberOrNull(giaTriMucTieu)
    if (mucTieu === null || mucTieu === 0) return null
    return ((giaTriLuyKe ?? 0) / mucTieu) * 100
  }

  function getKyLabel(item) {
    const tenKy = pick(item, 'tenKy', 'TenKy')
    const maKy = pick(item, 'maKy', 'MaKy')
    return [maKy, tenKy].filter(Boolean).join(' - ') || tenKy || maKy || '-'
  }

  function buildLatestKyMeta(item) {
    const parts = []
    if (item.loaiKyGanNhat) parts.push(item.loaiKyGanNhat)
    if (item.namGanNhat !== '' && item.namGanNhat !== null && item.namGanNhat !== undefined) parts.push(`Năm ${item.namGanNhat}`)
    if (item.soKyGanNhat !== '' && item.soKyGanNhat !== null && item.soKyGanNhat !== undefined) parts.push(`Số kỳ ${item.soKyGanNhat}`)
    return parts.join(' | ') || '-'
  }

  function formatNumber(value, donViTinh = '') {
    if (value === null || value === undefined || value === '') return '-'
    const parsed = parseNumber(value)
    if (!Number.isFinite(parsed)) return '-'
    const formatted = parsed.toLocaleString('vi-VN', { minimumFractionDigits: 0, maximumFractionDigits: 2 })
    return appendUnit(formatted, donViTinh)
  }

  function formatPercent(value) {
    if (value === null || value === undefined || value === '') return '-'
    const parsed = parseNumber(value)
    if (!Number.isFinite(parsed)) return '-'
    return `${parsed.toLocaleString('vi-VN', { minimumFractionDigits: 0, maximumFractionDigits: 2 })}%`
  }

  function appendUnit(formattedValue, donViTinh) {
    const unit = String(donViTinh || '').trim()
    return unit ? `${formattedValue} ${unit}` : formattedValue
  }

  function badgeClass(xepLoai) {
    return getDanhGiaBadgeClass(xepLoai)
  }

  function exportCsv() {
    const headers = [
      'Mã chỉ tiêu',
      'Tên chỉ tiêu',
      'Chỉ tiêu cha',
      'Đơn vị',
      'Đợt giao chỉ tiêu',
      'Mã kỳ gần nhất',
      'Tên kỳ gần nhất',
      'Loại kỳ',
      'Năm',
      'Số kỳ',
      'Giá trị mục tiêu',
      'Giá trị đầu kỳ gần nhất',
      'Giá trị cuối kỳ gần nhất',
      'Giá trị lũy kế hiện tại',
      'Số dư mục tiêu',
      '% hoàn thành',
      'Đánh giá',
      'Điều kiện thời hạn',
      'Nguồn đánh giá'
    ]

    const csvRows = filteredRows.value.map(item => [
      item.maChiTieu || '',
      item.tenChiTieu || '',
      item.tenChiTieuCha || '',
      item.tenDonViNhan || '',
      item.tenDotGiaoChiTieu || '',
      item.maKyGanNhat || '',
      item.tenKyGanNhat || '',
      item.loaiKyGanNhat || '',
      item.namGanNhat ?? '',
      item.soKyGanNhat ?? '',
      item.giaTriMucTieu ?? '',
      item.giaTriDauKyGanNhat ?? '',
      item.giaTriCuoiKyGanNhat ?? '',
      item.giaTriLuyKeHienTai ?? '',
      item.soDuMucTieu ?? '',
      item.tyLeHoanThanh ?? '',
      getDanhGiaLabel(item.xepLoai) || '',
      getThoiHanLabel(item.dieuKienThoiHan) || '',
      item.nguonDanhGiaTinhToan || ''
    ])

    const csvContent = [headers, ...csvRows]
      .map(row => row.map(value => `"${String(value).replace(/"/g, '""')}"`).join(','))
      .join('\n')

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', 'tong-hop-danh-gia-luy-ke.csv')
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
  }

  return {
    loading,
    errorMessage,
    kyBaoCaoOptions,
    filters,
    filteredRows,
    thongKe,
    fetchTongHopDanhGia,
    resetFilters,
    getId,
    getKyLabel,
    buildLatestKyMeta,
    formatNumber,
    formatPercent,
    badgeClass,
    getDanhGiaLabel,
    getThoiHanLabel,
    exportCsv
  }
}
