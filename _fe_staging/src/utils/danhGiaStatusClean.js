export const DANH_GIA_STATUS_OPTIONS = [
  { value: 'HOAN_THANH_VUOT_MUC', label: 'Hoàn thành vượt mức', shortLabel: 'Vượt mức', className: 'badge-excellent', rank: 4 },
  { value: 'HOAN_THANH', label: 'Hoàn thành', shortLabel: 'Hoàn thành', className: 'badge-good', rank: 3 },
  { value: 'CHUA_HOAN_THANH', label: 'Chưa hoàn thành', shortLabel: 'Chưa hoàn thành', className: 'badge-pass', rank: 2 },
  { value: 'KHONG_HOAN_THANH', label: 'Không hoàn thành', shortLabel: 'Không hoàn thành', className: 'badge-fail', rank: 1 },
  { value: 'CHUA_DANH_GIA', label: 'Chưa đánh giá', shortLabel: 'Chưa đánh giá', className: 'badge-pending', rank: 0 },
  { value: 'CHUA_CAU_HINH', label: 'Chưa cấu hình', shortLabel: 'Chưa cấu hình', className: 'badge-muted', rank: -1 }
]

export const DANH_GIA_TRACKED_STATUS_OPTIONS = DANH_GIA_STATUS_OPTIONS.filter(item =>
  item.value === 'HOAN_THANH_VUOT_MUC' ||
  item.value === 'HOAN_THANH' ||
  item.value === 'CHUA_HOAN_THANH' ||
  item.value === 'KHONG_HOAN_THANH'
)

export const DANH_GIA_TRACKED_STATUS_ORDER = DANH_GIA_TRACKED_STATUS_OPTIONS.map(item => item.value)

export const DIEU_KIEN_THOI_HAN_OPTIONS = [
  { value: 'MAC_DINH', label: 'Mặc định' },
  { value: 'CHUA_DEN_HAN', label: 'Chưa đến hạn' },
  { value: 'DA_DEN_HAN', label: 'Đã đến hạn' }
]

export const TIEU_CHI_DANH_GIA_OPTIONS = [
  { value: 'DINH_TINH', label: 'Định tính' },
  { value: 'DINH_LUONG_TICH_LUY', label: 'Định lượng tích lũy' },
  { value: 'DINH_LUONG_SO_SANH', label: 'Định lượng so sánh' }
]

export const LOAI_MOC_SO_SANH_OPTIONS = [
  { value: 'DAU_KY', label: 'Đầu kỳ' },
  { value: 'CUNG_KY', label: 'Cùng kỳ năm trước' },
  { value: 'KY_TRUOC', label: 'Kỳ trước' },
  { value: 'TONG_NAM_TRUOC', label: 'Tổng năm trước' }
]

export const KIEU_SO_SANH_OPTIONS = [
  { value: 'CHENH_LECH', label: 'Chênh lệch theo mốc' },
  { value: 'TY_LE', label: 'Tỷ lệ thực hiện / phát sinh' }
]

export const CHIEU_SO_SANH_OPTIONS = [
  { value: 'TANG', label: 'Tăng' },
  { value: 'GIAM', label: 'Giảm' }
]

export const QUY_TAC_DANH_GIA_OPTIONS = [
  { value: 'MAC_DINH', label: 'Mặc định' },
  { value: 'DAT_TOI_THIEU', label: 'Đạt tối thiểu' },
  { value: 'KHONG_VUOT_NGUONG', label: 'Không vượt ngưỡng' }
]

export const DINH_TINH_OPTION_GROUPS = {
  hoanThanh: [
    { value: 'KHONG_XAY_RA', label: 'Không xảy ra' },
    { value: 'DAM_BAO', label: 'Đảm bảo' },
    { value: 'DAT_100', label: 'Đạt 100%' }
  ],
  khongHoanThanh: [
    { value: 'XAY_RA', label: 'Xảy ra' },
    { value: 'KHONG_DAM_BAO', label: 'Không đảm bảo' },
    { value: 'KHONG_DAT', label: 'Không đạt' }
  ]
}

export const DINH_TINH_OPTIONS = [
  ...DINH_TINH_OPTION_GROUPS.hoanThanh,
  ...DINH_TINH_OPTION_GROUPS.khongHoanThanh
]

const STATUS_ALIASES = {
  HOAN_THANH_VUOT_MUC: 'HOAN_THANH_VUOT_MUC',
  VUOT_MUC: 'HOAN_THANH_VUOT_MUC',
  XUAT_SAC: 'HOAN_THANH_VUOT_MUC',
  HOAN_THANH: 'HOAN_THANH',
  TOT: 'HOAN_THANH',
  DAT: 'HOAN_THANH',
  CHUA_HOAN_THANH: 'CHUA_HOAN_THANH',
  KHONG_HOAN_THANH: 'KHONG_HOAN_THANH',
  KHONG_DAT: 'KHONG_HOAN_THANH',
  CHUA_DANH_GIA: 'CHUA_DANH_GIA',
  CHUA_CAU_HINH: 'CHUA_CAU_HINH'
}

const statusMetaByCode = Object.fromEntries(DANH_GIA_STATUS_OPTIONS.map(item => [item.value, item]))

export function normalizeDanhGiaText(value) {
  return String(value || '')
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/đ/g, 'd')
    .replace(/Đ/g, 'D')
    .trim()
    .toUpperCase()
    .replace(/\s+/g, '_')
}

export function getDanhGiaStatusCode(value) {
  const normalized = normalizeDanhGiaText(value)
  return STATUS_ALIASES[normalized] || normalized || ''
}

export function getDanhGiaStatusMeta(value) {
  const code = getDanhGiaStatusCode(value)
  return statusMetaByCode[code] || {
    value: code || '',
    label: value || 'Chưa xác định',
    shortLabel: value || 'Chưa xác định',
    className: 'badge-default',
    rank: -99
  }
}

export function getDanhGiaLabel(value) {
  return getDanhGiaStatusMeta(value).label
}

export function getDanhGiaShortLabel(value) {
  return getDanhGiaStatusMeta(value).shortLabel
}

export function getDanhGiaBadgeClass(value) {
  return getDanhGiaStatusMeta(value).className
}

export function getDanhGiaRank(value) {
  return getDanhGiaStatusMeta(value).rank
}

export function isHoanThanhStatus(value) {
  const code = getDanhGiaStatusCode(value)
  return code === 'HOAN_THANH' || code === 'HOAN_THANH_VUOT_MUC'
}

export function isKhongHoanThanhStatus(value) {
  const code = getDanhGiaStatusCode(value)
  return code === 'CHUA_HOAN_THANH' || code === 'KHONG_HOAN_THANH'
}

export function createTrackedStatusCounter() {
  return {
    HOAN_THANH_VUOT_MUC: 0,
    HOAN_THANH: 0,
    CHUA_HOAN_THANH: 0,
    KHONG_HOAN_THANH: 0
  }
}

export function countTrackedStatuses(items, selector = item => item?.xepLoai) {
  const result = createTrackedStatusCounter()

  for (const item of items) {
    const code = getDanhGiaStatusCode(selector(item))
    if (Object.prototype.hasOwnProperty.call(result, code)) {
      result[code] += 1
    }
  }

  return result
}

export function getThoiHanLabel(value) {
  const normalized = normalizeDanhGiaText(value)
  return DIEU_KIEN_THOI_HAN_OPTIONS.find(item => item.value === normalized)?.label || value || 'Mặc định'
}

export function getTieuChiDanhGiaLabel(value) {
  const normalized = normalizeDanhGiaText(value)
  return TIEU_CHI_DANH_GIA_OPTIONS.find(item => item.value === normalized)?.label || value || '-'
}

export function getLoaiMocSoSanhLabel(value) {
  const normalized = normalizeDanhGiaText(value)
  return LOAI_MOC_SO_SANH_OPTIONS.find(item => item.value === normalized)?.label || value || '-'
}

export function getKieuSoSanhLabel(value) {
  const normalized = normalizeDanhGiaText(value)
  return KIEU_SO_SANH_OPTIONS.find(item => item.value === normalized)?.label || value || '-'
}

export function getChieuSoSanhLabel(value) {
  const normalized = normalizeDanhGiaText(value)
  return CHIEU_SO_SANH_OPTIONS.find(item => item.value === normalized)?.label || value || '-'
}

export function getQuyTacDanhGiaLabel(value) {
  const normalized = normalizeDanhGiaText(value)
  return QUY_TAC_DANH_GIA_OPTIONS.find(item => item.value === normalized)?.label || value || 'Mặc định'
}

export function isDinhTinhCriterion(value) {
  return normalizeDanhGiaText(value) === 'DINH_TINH'
}

export function isDinhLuongSoSanhCriterion(value) {
  return normalizeDanhGiaText(value) === 'DINH_LUONG_SO_SANH'
}

export function isDinhLuongTichLuyCriterion(value) {
  return normalizeDanhGiaText(value) === 'DINH_LUONG_TICH_LUY'
}

export function isTyLeComparisonKind(value) {
  return normalizeDanhGiaText(value) === 'TY_LE'
}

export function isChenhLechComparisonKind(value) {
  return normalizeDanhGiaText(value) === 'CHENH_LECH'
}

export function isQuantitativeCriterion(value) {
  return isDinhLuongTichLuyCriterion(value) || isDinhLuongSoSanhCriterion(value)
}

export function isKhongVuotNguongRule(value) {
  return normalizeDanhGiaText(value) === 'KHONG_VUOT_NGUONG'
}

export function getQuyTacOptionsByCriterion(value, comparisonKind = '') {
  if (isDinhTinhCriterion(value)) {
    return QUY_TAC_DANH_GIA_OPTIONS.filter(item => item.value === 'MAC_DINH')
  }

  return QUY_TAC_DANH_GIA_OPTIONS.filter(item =>
    item.value === 'DAT_TOI_THIEU' || item.value === 'KHONG_VUOT_NGUONG'
  )
}
