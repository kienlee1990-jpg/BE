const RAW_NHOM_THI_DUA = [
  {
    id: 'catp',
    tenNhom: 'Công an thành phố',
    moTa: 'Nhóm tổng hợp các chỉ tiêu do Công an thành phố trực tiếp thực hiện.',
    exactNames: ['Công an thành phố Đà Nẵng'],
    keywords: ['cong an thanh pho da nang']
  },
  {
    id: 'khoi-phong',
    tenNhom: 'Khối Phòng',
    moTa: 'Nhóm mặc định cho các phòng, ban và đơn vị nghiệp vụ cấp thành phố.',
    exactNames: [],
    keywords: [
      'phong ',
      'doi ',
      'trai tam giam',
      'van phong',
      'thanh tra',
      'an ninh',
      'canh sat',
      'tham muu',
      'hau can',
      'ky thuat',
      'xay dung phong trao'
    ]
  },
  {
    id: 'khoi-cadp-phuong-xa',
    tenNhom: 'Khối CADP phường/xã',
    moTa: 'Nhóm mặc định cho Công an phường, xã, thị trấn và đơn vị cấp cơ sở.',
    exactNames: [],
    keywords: ['cong an phuong', 'cong an xa', 'thi tran', 'phuong', 'xa']
  },
  {
    id: 'khoi-khac',
    tenNhom: 'Khối khác',
    moTa: 'Các đơn vị chưa khớp với cấu hình nhóm thi đua hiện tại.',
    exactNames: [],
    keywords: []
  }
]

const FALLBACK_GROUP = RAW_NHOM_THI_DUA[RAW_NHOM_THI_DUA.length - 1]

export const NHOM_THI_DUA_CONFIG = RAW_NHOM_THI_DUA.map((group) => ({
  ...group,
  exactNames: group.exactNames.map(normalizeGroupText),
  keywords: group.keywords.map(normalizeGroupText)
}))

export function normalizeGroupText(value) {
  return String(value || '')
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/đ/g, 'd')
    .replace(/Đ/g, 'D')
    .trim()
    .toLowerCase()
}

export function resolveNhomThiDua(tenDonVi) {
  const normalizedName = normalizeGroupText(tenDonVi)

  for (const group of NHOM_THI_DUA_CONFIG) {
    if (group.exactNames.includes(normalizedName)) {
      return group
    }

    if (group.keywords.some(keyword => normalizedName.includes(keyword))) {
      return group
    }
  }

  return FALLBACK_GROUP
}

export function getNhomThiDuaOptions(unitNames = []) {
  const availableIds = new Set(unitNames.map(name => resolveNhomThiDua(name).id))
  return NHOM_THI_DUA_CONFIG.filter(group => availableIds.has(group.id))
}
