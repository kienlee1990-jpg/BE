import { computed, onMounted, reactive, ref } from 'vue'
import httpClient from '../../services/httpClient'
import {
  countTrackedStatuses,
  DANH_GIA_TRACKED_STATUS_OPTIONS,
  getDanhGiaBadgeClass,
  getDanhGiaLabel
} from '../../utils/danhGiaStatusClean.js'

export function useBaoCaoTongHopPage() {
  const loading = ref(false)
  const errorMessage = ref('')

  const kyBaoCaoOptions = ref([])
  const theoDoiRows = ref([])
  const chiTietGiaoRows = ref([])
  const danhMucRows = ref([])
  const dotGiaoRows = ref([])
  const donViRows = ref([])
  const danhGiaRows = ref([])

  const trackedStatusOptions = DANH_GIA_TRACKED_STATUS_OPTIONS

  const filters = reactive({
    kyBaoCaoKPIId: '',
    donVi: '',
    xepLoai: '',
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

  const selectedKySort = computed(() => {
    return selectedKy.value ? extractKySortInfo(selectedKy.value) : null
  })

  const flattenedAssignments = computed(() => {
    const flattened = []
    chiTietGiaoRows.value.forEach(item => appendFlattenedAssignment(flattened, item, null, danhMucById.value, dotGiaoById.value, donViById.value))
    return flattened.filter(item => !item.coTieuChiCon)
  })

  const normalizedTheoDoiRows = computed(() => {
    return theoDoiRows.value
      .map(rawItem => {
        const chiTietGiaoChiTieuId = Number(pick(rawItem, 'chiTietGiaoChiTieuId', 'ChiTietGiaoChiTieuId') || 0)
        const kyBaoCaoKPIId = Number(pick(rawItem, 'kyBaoCaoKPIId', 'KyBaoCaoKPIId') || 0)
        const kyBaoCao = kyBaoCaoById.value.get(kyBaoCaoKPIId) || null

        return {
          chiTietGiaoChiTieuId,
          kyBaoCaoKPIId,
          kySort: extractKySortInfo({ ...kyBaoCao, ...rawItem }),
          maKy: pick(rawItem, 'maKy', 'MaKy') || pick(kyBaoCao, 'maKy', 'MaKy') || '',
          tenKy: pick(rawItem, 'tenKy', 'TenKy') || pick(kyBaoCao, 'tenKy', 'TenKy') || '',
          loaiKy: pick(rawItem, 'loaiKy', 'LoaiKy') || pick(kyBaoCao, 'loaiKy', 'LoaiKy') || '',
          nam: pick(rawItem, 'nam', 'Nam') ?? pick(kyBaoCao, 'nam', 'Nam') ?? '',
          soKy: pick(rawItem, 'soKy', 'SoKy') ?? pick(kyBaoCao, 'soKy', 'SoKy') ?? '',
          giaTriDauKy: getNumberOrNull(pick(rawItem, 'giaTriDauKy', 'GiaTriDauKy')),
          giaTriThucHienTrongKy: getNumberOrNull(pick(rawItem, 'giaTriThucHienTrongKy', 'GiaTriThucHienTrongKy')),
          giaTriCuoiKy: getNumberOrNull(pick(rawItem, 'giaTriCuoiKy', 'GiaTriCuoiKy')),
          giaTriLuyKe: getNumberOrNull(pick(rawItem, 'giaTriLuyKe', 'GiaTriLuyKe')),
          nhanXet: String(pick(rawItem, 'nhanXet', 'NhanXet') || '').trim()
        }
      })
      .filter(item => item.chiTietGiaoChiTieuId > 0)
      .filter(item => !selectedKySort.value || compareKySort(item.kySort, selectedKySort.value) <= 0)
  })

  const normalizedDanhGiaRows = computed(() => {
    return danhGiaRows.value
      .map(rawItem => {
        const chiTietGiaoChiTieuId = Number(pick(rawItem, 'chiTietGiaoChiTieuId', 'ChiTietGiaoChiTieuId') || 0)
        const kyBaoCaoKPIId = Number(pick(rawItem, 'kyBaoCaoKPIId', 'KyBaoCaoKPIId') || 0)
        const kyBaoCao = kyBaoCaoById.value.get(kyBaoCaoKPIId) || null

        return {
          chiTietGiaoChiTieuId,
          kyBaoCaoKPIId,
          kySort: extractKySortInfo({ ...kyBaoCao, ...rawItem }),
          maKy: pick(rawItem, 'maKy', 'MaKy') || pick(kyBaoCao, 'maKy', 'MaKy') || '',
          tenKy: pick(rawItem, 'tenKy', 'TenKy') || pick(kyBaoCao, 'tenKy', 'TenKy') || '',
          loaiKy: pick(rawItem, 'loaiKy', 'LoaiKy') || pick(kyBaoCao, 'loaiKy', 'LoaiKy') || '',
          nam: pick(rawItem, 'nam', 'Nam') ?? pick(kyBaoCao, 'nam', 'Nam') ?? '',
          soKy: pick(rawItem, 'soKy', 'SoKy') ?? pick(kyBaoCao, 'soKy', 'SoKy') ?? '',
          tyLeHoanThanh: getNumberOrNull(pick(rawItem, 'tyLeHoanThanh', 'TyLeHoanThanh')),
          xepLoai: pick(rawItem, 'xepLoai', 'XepLoai') || '',
          ketQua: pick(rawItem, 'ketQua', 'KetQua') || '',
          nhanXetDanhGia: String(pick(rawItem, 'nhanXetDanhGia', 'NhanXetDanhGia') || '').trim(),
          giaTriCuoiKy: getNumberOrNull(pick(rawItem, 'giaTriCuoiKy', 'GiaTriCuoiKy')),
          giaTriMucTieu: getNumberOrNull(pick(rawItem, 'giaTriMucTieu', 'GiaTriMucTieu'))
        }
      })
      .filter(item => item.chiTietGiaoChiTieuId > 0)
      .filter(item => !selectedKySort.value || compareKySort(item.kySort, selectedKySort.value) <= 0)
  })

  const theoDoiByAssignment = computed(() => groupRowsById(normalizedTheoDoiRows.value, item => item.chiTietGiaoChiTieuId))
  const danhGiaByAssignment = computed(() => groupRowsById(normalizedDanhGiaRows.value, item => item.chiTietGiaoChiTieuId))

  const groupedRows = computed(() => {
    return flattenedAssignments.value
      .map(assignment => buildSummaryRow(assignment, theoDoiByAssignment.value, danhGiaByAssignment.value))
      .sort((left, right) => {
        const leftValue = `${left.tenDotGiaoChiTieu || ''} ${left.tenDonViNhan || ''} ${left.maChiTieu || ''}`.toLowerCase()
        const rightValue = `${right.tenDotGiaoChiTieu || ''} ${right.tenDonViNhan || ''} ${right.maChiTieu || ''}`.toLowerCase()
        return leftValue.localeCompare(rightValue, 'vi')
      })
  })

  const donViOptions = computed(() => {
    return [...new Set(groupedRows.value.map(item => item.tenDonViNhan).filter(Boolean))].sort((a, b) =>
      a.localeCompare(b, 'vi')
    )
  })

  const filteredRows = computed(() => {
    let data = [...groupedRows.value]

    if (filters.donVi) {
      data = data.filter(item => item.tenDonViNhan === filters.donVi)
    }

    if (filters.xepLoai) {
      data = data.filter(item => normalizeCode(item.xepLoai) === filters.xepLoai)
    }

    if (filters.keyword) {
      const keyword = normalizeText(filters.keyword)
      data = data.filter(item =>
        [
          item.maChiTieu,
          item.tenChiTieu,
          item.tenChiTieuCha,
          item.tenDonViNhan,
          item.maDotGiaoChiTieu,
          item.tenDotGiaoChiTieu,
          item.maKyGanNhat,
          item.tenKyGanNhat,
          item.ketQuaDanhGia,
          item.nhanXetDanhGia
        ]
          .filter(Boolean)
          .some(value => normalizeText(value).includes(keyword))
      )
    }

    return data
  })

  const thongKe = computed(() => countTrackedStatuses(filteredRows.value, item => item.xepLoai))
  const tongGiaTriThucHienCongDon = computed(() => sumField(filteredRows.value, 'giaTriThucHienCongDon'))
  const averageCompletion = computed(() => averageOf(filteredRows.value, 'tyLeHoanThanh'))

  onMounted(async () => {
    await fetchBaoCaoTongHop()
  })

  async function fetchBaoCaoTongHop() {
    loading.value = true
    errorMessage.value = ''

    const requests = [
      { key: 'kyBaoCao', label: 'kỳ báo cáo', required: false, request: () => httpClient.get('/KyBaoCaoKPI') },
      { key: 'theoDoi', label: 'theo dõi thực hiện KPI', required: true, request: () => httpClient.get('/TheoDoiThucHienKPI') },
      { key: 'chiTiet', label: 'chi tiết giao chỉ tiêu', required: true, request: () => httpClient.get('/ChiTietGiaoChiTieu') },
      { key: 'danhMuc', label: 'danh mục chỉ tiêu', required: false, request: () => httpClient.get('/DanhMucChiTieu') },
      { key: 'dotGiao', label: 'đợt giao chỉ tiêu', required: false, request: () => httpClient.get('/dot-giao-chi-tieu') },
      { key: 'donVi', label: 'đơn vị', required: false, request: () => httpClient.get('/DonVi') },
      { key: 'danhGia', label: 'đánh giá KPI', required: false, request: () => httpClient.get('/DanhGiaKPI') }
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
        if (current.key === 'dotGiao') dotGiaoRows.value = normalized
        if (current.key === 'donVi') donViRows.value = normalized
        if (current.key === 'danhGia') danhGiaRows.value = normalized
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
    filters.donVi = ''
    filters.xepLoai = ''
    filters.keyword = ''
    fetchBaoCaoTongHop()
  }

  function buildSummaryRow(assignment, theoDoiMap, danhGiaMap) {
    const theoDoiItems = [...(theoDoiMap.get(assignment.id) || [])]
    const danhGiaItems = [...(danhGiaMap.get(assignment.id) || [])]

    const latestTheoDoi = findLatestRow(theoDoiItems)
    const latestDanhGia = findLatestRow(danhGiaItems)
    const latestKy = compareLatestRow(latestDanhGia, latestTheoDoi) >= 0 ? latestDanhGia : latestTheoDoi

    const giaTriThucHienCongDon = sumField(theoDoiItems, 'giaTriThucHienTrongKy')
    const giaTriDauKyGanNhat =
      latestTheoDoi?.giaTriDauKy ??
      latestDanhGia?.giaTriDauKy ??
      assignment.giaTriDauKyCoDinh ??
      null
    const giaTriLuyKeHienTai =
      latestTheoDoi?.giaTriLuyKe ??
      latestTheoDoi?.giaTriCuoiKy ??
      (giaTriThucHienCongDon !== 0 ? giaTriThucHienCongDon : null)

    const tyLeHoanThanh =
      latestDanhGia?.tyLeHoanThanh ??
      calculateCompletionRate(giaTriLuyKeHienTai, assignment.giaTriMucTieu, assignment.loaiChiTieu)

    const xepLoai = latestDanhGia?.xepLoai || 'CHUA_DANH_GIA'
    const ketQuaDanhGia = latestDanhGia?.ketQua || getDanhGiaLabel(xepLoai) || ''

    return {
      groupKey: String(assignment.id),
      chiTietGiaoChiTieuId: assignment.id,
      maChiTieu: assignment.maChiTieu || '-',
      tenDanhMucChiTieu: assignment.tenDanhMucChiTieu || assignment.tenChiTieu || '-',
      tenChiTieu: assignment.tenDanhMucChiTieu || assignment.tenChiTieu || '-',
      tenChiTieuCha: assignment.tenChiTieuCha || '',
      tenChiTieuGiao: assignment.tenChiTieuCha || '',
      tenDonViNhan: assignment.tenDonViNhan || '-',
      maDotGiaoChiTieu: assignment.maDotGiaoChiTieu || '',
      tenDotGiaoChiTieu: assignment.tenDotGiaoChiTieu || '-',
      namApDungDotGiao: assignment.namApDungDotGiao ?? '',
      nguonDotGiao: assignment.nguonDotGiao || '',
      capGiaoDotGiao: assignment.capGiaoDotGiao || '',
      ngayBatDauDotGiao: assignment.ngayBatDauDotGiao || '',
      ngayKetThucDotGiao: assignment.ngayKetThucDotGiao || '',
      trangThaiDotGiao: assignment.trangThaiDotGiao || '',
      tieuChiDanhGia: assignment.tieuChiDanhGia || '',
      giaTriMucTieu: assignment.giaTriMucTieu,
      giaTriDauKyGanNhat,
      giaTriThucHienCongDon,
      giaTriCuoiKyGanNhat: latestTheoDoi?.giaTriCuoiKy ?? latestDanhGia?.giaTriCuoiKy ?? null,
      giaTriCungKyNamTruocGanNhat: latestDanhGia?.giaTriCungKyNamTruoc ?? null,
      giaTriLuyKeHienTai,
      tyLeHoanThanh,
      soDuMucTieu:
        assignment.giaTriMucTieu === null || giaTriLuyKeHienTai === null
          ? null
          : assignment.giaTriMucTieu - giaTriLuyKeHienTai,
      maKyGanNhat: latestKy?.maKy || '-',
      tenKyGanNhat: latestKy?.tenKy || '-',
      loaiKyGanNhat: latestKy?.loaiKy || '',
      namGanNhat: latestKy?.nam ?? '',
      soKyGanNhat: latestKy?.soKy ?? '',
      xepLoai,
      ketQuaDanhGia,
      nhanXetDanhGia: latestDanhGia?.nhanXetDanhGia || latestTheoDoi?.nhanXet || '',
      soKyDaTongHop: theoDoiItems.length
    }
  }

  function findLatestRow(items) {
    if (!items.length) return null
    return [...items].sort((left, right) => compareKySort(right.kySort, left.kySort))[0]
  }

  function compareLatestRow(left, right) {
    if (!left && !right) return 0
    if (!left) return -1
    if (!right) return 1
    return compareKySort(left.kySort, right.kySort)
  }

  function groupRowsById(items, getKey) {
    return items.reduce((map, item) => {
      const key = Number(getKey(item) || 0)
      if (!map.has(key)) map.set(key, [])
      map.get(key).push(item)
      return map
    }, new Map())
  }

  function sumField(items, field) {
    return roundNumber(
      items.reduce((sum, item) => {
        const value = Number(item?.[field])
        return Number.isFinite(value) ? sum + value : sum
      }, 0)
    )
  }

  function averageOf(items, field) {
    const values = items
      .map(item => Number(item[field]))
      .filter(value => Number.isFinite(value))

    if (!values.length) return 0
    return roundNumber(values.reduce((sum, value) => sum + value, 0) / values.length)
  }

  function calculateCompletionRate(giaTriThucHien, giaTriMucTieu, loaiChiTieu) {
    if (normalizeCode(loaiChiTieu) === 'DINH_TINH') return null
    const mucTieu = getNumberOrNull(giaTriMucTieu)
    const thucHien = getNumberOrNull(giaTriThucHien)
    if (mucTieu === null || mucTieu === 0 || thucHien === null) return null
    return (thucHien / mucTieu) * 100
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

  function appendFlattenedAssignment(target, rawItem, parentAssignment, danhMucMap, dotGiaoMap, donViMap) {
    const current = normalizeAssignment(rawItem, parentAssignment, danhMucMap, dotGiaoMap, donViMap)
    target.push(current)
    const children = normalizeList(pick(rawItem, 'tieuChiCon', 'TieuChiCon'))
    children.forEach(child => appendFlattenedAssignment(target, child, current, danhMucMap, dotGiaoMap, donViMap))
  }

  function normalizeAssignment(rawItem, parentAssignment, danhMucMap, dotGiaoMap, donViMap) {
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

    const danhMuc = danhMucMap.get(danhMucChiTieuId) || null
    const dotGiao = dotGiaoMap.get(dotGiaoChiTieuId) || null
    const donVi = donViMap.get(donViNhanId) || null

    return {
      id,
      coTieuChiCon: children.length > 0,
      danhMucChiTieuId: danhMucChiTieuId || null,
      dotGiaoChiTieuId,
      donViNhanId,
      maChiTieu:
        pick(rawItem, 'maDanhMucChiTieu', 'MaDanhMucChiTieu', 'maChiTieu', 'MaChiTieu') ||
        pick(danhMuc, 'maChiTieu', 'MaChiTieu') ||
        '',
      tenDanhMucChiTieu:
        pick(rawItem, 'tenDanhMucChiTieu', 'TenDanhMucChiTieu', 'tenChiTieu', 'TenChiTieu') ||
        pick(danhMuc, 'tenChiTieu', 'TenChiTieu') ||
        '',
      tenChiTieu:
        pick(rawItem, 'tenDanhMucChiTieu', 'TenDanhMucChiTieu', 'tenChiTieu', 'TenChiTieu') ||
        pick(danhMuc, 'tenChiTieu', 'TenChiTieu') ||
        '',
      tenChiTieuCha: parentAssignment?.tenChiTieu || '',
      maDotGiaoChiTieu:
        pick(rawItem, 'maDotGiaoChiTieu', 'MaDotGiaoChiTieu') ||
        pick(dotGiao, 'maDotGiao', 'MaDotGiao') ||
        parentAssignment?.maDotGiaoChiTieu ||
        '',
      tenDotGiaoChiTieu:
        pick(rawItem, 'tenDotGiaoChiTieu', 'TenDotGiaoChiTieu') ||
        pick(dotGiao, 'tenDotGiao', 'TenDotGiao') ||
        parentAssignment?.tenDotGiaoChiTieu ||
        '-',
      namApDungDotGiao:
        pick(rawItem, 'namApDungDotGiao', 'NamApDungDotGiao') ??
        pick(dotGiao, 'namApDung', 'NamApDung') ??
        parentAssignment?.namApDungDotGiao ??
        '',
      nguonDotGiao:
        pick(rawItem, 'nguonDotGiao', 'NguonDotGiao') ||
        pick(dotGiao, 'nguonDotGiao', 'NguonDotGiao') ||
        parentAssignment?.nguonDotGiao ||
        '',
      capGiaoDotGiao:
        pick(rawItem, 'capGiaoDotGiao', 'CapGiaoDotGiao') ||
        pick(dotGiao, 'capGiao', 'CapGiao') ||
        parentAssignment?.capGiaoDotGiao ||
        '',
      ngayBatDauDotGiao:
        pick(rawItem, 'ngayBatDauDotGiao', 'NgayBatDauDotGiao') ||
        pick(dotGiao, 'ngayBatDau', 'NgayBatDau') ||
        parentAssignment?.ngayBatDauDotGiao ||
        '',
      ngayKetThucDotGiao:
        pick(rawItem, 'ngayKetThucDotGiao', 'NgayKetThucDotGiao') ||
        pick(dotGiao, 'ngayKetThuc', 'NgayKetThuc') ||
        parentAssignment?.ngayKetThucDotGiao ||
        '',
      trangThaiDotGiao:
        pick(rawItem, 'trangThaiDotGiao', 'TrangThaiDotGiao') ||
        pick(dotGiao, 'trangThai', 'TrangThai') ||
        parentAssignment?.trangThaiDotGiao ||
        '',
      tenDonViNhan:
        pick(rawItem, 'tenDonViNhan', 'TenDonViNhan') ||
        pick(donVi, 'tenDonVi', 'TenDonVi') ||
        parentAssignment?.tenDonViNhan ||
        '-',
      loaiChiTieu:
        normalizeCode(pick(rawItem, 'loaiChiTieu', 'LoaiChiTieu') || pick(danhMuc, 'loaiChiTieu', 'LoaiChiTieu')) || '',
      tieuChiDanhGia:
        normalizeCode(pick(rawItem, 'tieuChiDanhGia', 'TieuChiDanhGia') || pick(danhMuc, 'tieuChiDanhGia', 'TieuChiDanhGia')) || '',
      giaTriMucTieu: getNumberOrNull(pick(rawItem, 'giaTriMucTieu', 'GiaTriMucTieu')),
      giaTriDauKyCoDinh: getNumberOrNull(pick(rawItem, 'giaTriDauKyCoDinh', 'GiaTriDauKyCoDinh'))
    }
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

  function roundNumber(value) {
    return Number(Number(value || 0).toFixed(2))
  }

  function formatNumber(value) {
    if (value === null || value === undefined || value === '') return '-'
    const parsed = parseNumber(value)
    if (!Number.isFinite(parsed)) return '-'
    return parsed.toLocaleString('vi-VN', { minimumFractionDigits: 0, maximumFractionDigits: 2 })
  }

  function formatPercent(value) {
    if (value === null || value === undefined || value === '') return '-'
    const parsed = parseNumber(value)
    if (!Number.isFinite(parsed)) return '-'
    return `${parsed.toLocaleString('vi-VN', { minimumFractionDigits: 0, maximumFractionDigits: 2 })}%`
  }

  function badgeClass(xepLoai) {
    return getDanhGiaBadgeClass(xepLoai)
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

  function buildDotGiaoMeta(item) {
    const parts = []
    if (item.maDotGiaoChiTieu) parts.push(item.maDotGiaoChiTieu)
    if (item.capGiaoDotGiao) parts.push(item.capGiaoDotGiao)
    if (item.namApDungDotGiao !== '' && item.namApDungDotGiao !== null && item.namApDungDotGiao !== undefined) {
      parts.push(`Năm ${item.namApDungDotGiao}`)
    }
    if (item.nguonDotGiao) parts.push(item.nguonDotGiao)
    return parts.join(' | ') || '-'
  }

  function formatDate(value) {
    if (!value) return '-'
    const date = parseDate(value)
    if (!date) return '-'
    return date.toLocaleDateString('vi-VN')
  }

  function exportCsv() {
    const headers = [
      'Mã chỉ tiêu',
      'Tên danh mục chỉ tiêu',
      'Chỉ tiêu giao',
      'Đơn vị',
      'Mã đợt giao',
      'Đợt giao chỉ tiêu',
      'Năm áp dụng đợt giao',
      'Nguồn đợt giao',
      'Cấp giao',
      'Ngày bắt đầu đợt giao',
      'Ngày kết thúc đợt giao',
      'Trạng thái đợt giao',
      'Mã kỳ gần nhất',
      'Tên kỳ gần nhất',
      'Loại kỳ',
      'Năm',
      'Số kỳ',
      'Giá trị mục tiêu',
      'Giá trị thực hiện cộng dồn',
      'Giá trị cuối kỳ gần nhất',
      'Lũy kế hiện tại',
      'Số dư mục tiêu',
      '% hoàn thành',
      'Đánh giá',
      'Kết quả',
      'Nhận xét đánh giá',
      'Số kỳ đã tổng hợp'
    ]

    const csvRows = filteredRows.value.map(item => [
      item.maChiTieu || '',
      item.tenDanhMucChiTieu || item.tenChiTieu || '',
      item.tenChiTieuGiao || item.tenChiTieuCha || '',
      item.tenDonViNhan || '',
      item.maDotGiaoChiTieu || '',
      item.tenDotGiaoChiTieu || '',
      item.namApDungDotGiao ?? '',
      item.nguonDotGiao || '',
      item.capGiaoDotGiao || '',
      formatDate(item.ngayBatDauDotGiao),
      formatDate(item.ngayKetThucDotGiao),
      item.trangThaiDotGiao || '',
      item.maKyGanNhat || '',
      item.tenKyGanNhat || '',
      item.loaiKyGanNhat || '',
      item.namGanNhat ?? '',
      item.soKyGanNhat ?? '',
      item.giaTriMucTieu ?? '',
      item.giaTriThucHienCongDon ?? '',
      item.giaTriCuoiKyGanNhat ?? '',
      item.giaTriLuyKeHienTai ?? '',
      item.soDuMucTieu ?? '',
      item.tyLeHoanThanh ?? '',
      getDanhGiaLabel(item.xepLoai) || '',
      item.ketQuaDanhGia || '',
      item.nhanXetDanhGia || '',
      item.soKyDaTongHop ?? ''
    ])

    const csvContent = [headers, ...csvRows]
      .map(row => row.map(value => `"${String(value).replace(/"/g, '""')}"`).join(','))
      .join('\n')

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', 'bao-cao-tong-hop-kpi.csv')
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
  }

  return {
    loading,
    errorMessage,
    kyBaoCaoOptions,
    trackedStatusOptions,
    filters,
    groupedRows,
    donViOptions,
    filteredRows,
    thongKe,
    tongGiaTriThucHienCongDon,
    averageCompletion,
    fetchBaoCaoTongHop,
    resetFilters,
    getId,
    getKyLabel,
    buildDotGiaoMeta,
    buildLatestKyMeta,
    formatDate,
    formatNumber,
    formatPercent,
    badgeClass,
    getDanhGiaLabel,
    exportCsv
  }
}
