<template>
  <BaseLayout>
    <div class="page-shell">
      <div class="page-header">
        <div class="gov-banner">
          <div class="gov-emblem">
            <i class="bi bi-shield-lock-fill"></i>
          </div>
          <div>
            <div class="gov-title">PHÂN QUYỀN CHI TIẾT</div>
            <div class="gov-sub">Cấu hình quyền theo chức năng cho user hoặc role</div>
          </div>
        </div>
        <RouterLink to="/users" class="btn btn-outline-primary">Quay về màn người dùng</RouterLink>
      </div>

      <div class="alert alert-info border-0 shadow-sm">
        <strong>Vai trò được gán ở màn Người dùng.</strong>
        Màn này dùng để bật hoặc tắt các quyền chức năng theo từng user hoặc từng role dựa trên các màn hiện có ở frontend.
      </div>

      <div class="mode-switch">
        <button class="btn" :class="activeTab === 'USER' ? 'btn-primary' : 'btn-outline-secondary'" @click="switchTab('USER')">
          Quyền theo user
        </button>
        <button class="btn" :class="activeTab === 'ROLE' ? 'btn-primary' : 'btn-outline-secondary'" @click="switchTab('ROLE')">
          Quyền theo role
        </button>
      </div>

      <div v-if="error" class="alert alert-danger">{{ error }}</div>
      <div v-if="successMessage" class="alert alert-success">{{ successMessage }}</div>

      <div class="summary-grid">
        <div class="summary-card">
          <span class="summary-label">Tổng user</span>
          <strong class="summary-value">{{ users.length }}</strong>
        </div>
        <div class="summary-card">
          <span class="summary-label">Tổng role</span>
          <strong class="summary-value">{{ roles.length }}</strong>
        </div>
        <div class="summary-card">
          <span class="summary-label">Nhóm quyền</span>
          <strong class="summary-value">{{ permissionGroups.length }}</strong>
        </div>
        <div class="summary-card">
          <span class="summary-label">Tổng quyền khả dụng</span>
          <strong class="summary-value">{{ allPermissionOptions.length }}</strong>
        </div>
      </div>

      <div class="card shadow-sm border-0 mb-3">
        <div class="card-body">
          <div class="selector-grid">
            <div>
              <label class="form-label">Tìm nhanh {{ activeTab === "USER" ? "user" : "role" }}</label>
              <input
                v-model.trim="targetSearch"
                type="text"
                class="form-control"
                :placeholder="activeTab === 'USER' ? 'Nhập họ tên, username, email' : 'Nhập tên role'"
              />
            </div>
            <div>
              <label class="form-label">Lọc quyền theo nhóm chức năng</label>
              <select v-model="groupFilter" class="form-select">
                <option value="ALL">Tất cả nhóm quyền</option>
                <option v-for="group in permissionGroups" :key="group.key" :value="group.key">{{ group.label }}</option>
              </select>
            </div>
            <div v-if="activeTab === 'USER'">
              <label class="form-label">Chọn người dùng</label>
              <select class="form-select" v-model="selectedUserId" @change="handleUserChange">
                <option value="">-- Chọn user --</option>
                <option v-for="item in filteredUsers" :key="item.id" :value="item.id">
                  {{ item.fullName || item.userName || item.email }} - {{ item.email }}
                </option>
              </select>
              <div class="small text-muted mt-2">Quyền ở đây là quyền gán trực tiếp riêng cho user.</div>
            </div>
            <div v-else>
              <label class="form-label">Chọn role</label>
              <select class="form-select" v-model="selectedRoleId" @change="handleRoleChange">
                <option value="">-- Chọn role --</option>
                <option v-for="item in filteredRoles" :key="item.id" :value="item.id">
                  {{ item.roleName }}{{ item.userCount ? ` (${item.userCount} user)` : '' }}
                </option>
              </select>
              <div class="small text-muted mt-2">Quyền theo role sẽ áp dụng cho các user đang mang role tương ứng.</div>
            </div>
          </div>
        </div>
      </div>

      <div v-if="loadingUsers || loadingRoles || loadingDetail" class="loading-state">
        <div class="spinner-border text-primary"></div>
        <span>Đang tải dữ liệu phân quyền...</span>
      </div>

      <div v-else-if="activeTargetName" class="card shadow-sm border-0">
        <div class="card-body">
          <div class="permission-header">
            <div>
              <h5 class="mb-1">{{ activeTab === "USER" ? "Phân quyền cho user" : "Phân quyền cho role" }}</h5>
              <div class="text-muted">{{ activeTargetName }}</div>
            </div>
            <div class="permission-actions">
              <button class="btn btn-sm btn-outline-primary" @click="checkAllVisible">Chọn tất cả đang hiển thị</button>
              <button class="btn btn-sm btn-outline-secondary" @click="uncheckAllVisible">Bỏ chọn đang hiển thị</button>
              <button class="btn btn-sm btn-outline-dark" @click="expandAll = !expandAll">
                {{ expandAll ? "Thu gọn nhóm" : "Mở tất cả nhóm" }}
              </button>
            </div>
          </div>

          <div class="meta-grid mb-3">
            <div class="meta-card">
              <span class="meta-label">Loại cấu hình</span>
              <div class="meta-value">{{ activeTab === "USER" ? "Quyền riêng theo user" : "Quyền chung theo role" }}</div>
            </div>
            <div class="meta-card">
              <span class="meta-label">Vai trò liên quan</span>
              <div class="meta-value">
                <template v-if="activeTab === 'USER'">
                  <span v-for="role in selectedUserRoles" :key="role" class="badge role-chip me-1">{{ role }}</span>
                  <span v-if="!selectedUserRoles.length" class="text-muted">Chưa có vai trò</span>
                </template>
                <template v-else>
                  <span class="badge role-chip">{{ selectedRoleName || "Chưa chọn role" }}</span>
                </template>
              </div>
            </div>
            <div class="meta-card" v-if="activeTab === 'ROLE'">
              <span class="meta-label">Số user mang role</span>
              <div class="meta-value">{{ selectedRoleUserCount }}</div>
            </div>
            <div class="meta-card">
              <span class="meta-label">Quyền đang chọn</span>
              <div class="meta-value">{{ selectedPermissions.length }}/{{ allPermissionOptions.length }}</div>
            </div>
          </div>

          <div class="group-list">
            <section v-for="group in visiblePermissionGroups" :key="group.key" class="group-card">
              <button type="button" class="group-header" @click="toggleGroup(group.key)">
                <div>
                  <h6 class="mb-1">{{ group.label }}</h6>
                  <div class="text-muted small">{{ group.description }}</div>
                </div>
                <div class="group-header-right">
                  <span class="badge text-bg-light">
                    {{ countSelectedInGroup(group) }}/{{ group.permissions.length }}
                  </span>
                  <i class="bi" :class="isGroupOpen(group.key) ? 'bi-chevron-up' : 'bi-chevron-down'"></i>
                </div>
              </button>

              <div v-show="isGroupOpen(group.key)" class="group-body">
                <div class="group-actions">
                  <button class="btn btn-sm btn-outline-primary" @click="checkGroup(group)">Chọn cả nhóm</button>
                  <button class="btn btn-sm btn-outline-secondary" @click="uncheckGroup(group)">Bỏ nhóm</button>
                </div>

                <div class="permission-grid">
                  <label v-for="perm in group.permissions" :key="perm.value" class="permission-card">
                    <div class="permission-card-main">
                      <input class="form-check-input mt-1" type="checkbox" :value="perm.value" v-model="selectedPermissions" />
                      <div class="permission-copy">
                        <strong class="d-block permission-name">{{ perm.label }}</strong>
                        <div class="permission-description">{{ perm.description }}</div>
                        <span class="permission-code">{{ perm.value }}</span>
                      </div>
                    </div>
                    <span class="permission-state-badge" :class="selectedPermissions.includes(perm.value) ? 'is-selected' : 'is-unselected'">
                      {{ selectedPermissions.includes(perm.value) ? "Đã chọn" : "Chưa chọn" }}
                    </span>
                  </label>
                </div>
              </div>
            </section>
          </div>

          <div class="save-bar">
            <div class="small text-muted">
              {{ activeTab === "USER"
                ? "Lưu để cập nhật quyền trực tiếp cho user đang chọn."
                : "Lưu để cập nhật quyền áp dụng cho toàn bộ user đang mang role này." }}
            </div>
            <button class="btn btn-primary" :disabled="saving || loadingDetail" @click="savePermissions">
              {{ saving ? "Đang lưu..." : "Lưu phân quyền" }}
            </button>
          </div>
        </div>
      </div>

      <div v-else class="alert alert-light border shadow-sm">
        Chọn {{ activeTab === "USER" ? "một user" : "một role" }} để bắt đầu cấu hình quyền chi tiết.
      </div>
    </div>
  </BaseLayout>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import BaseLayout from '../BaseLayout.vue'
import httpClient from '../../services/httpClient'

const permissionGroups = [
  {
    key: 'overview',
    label: 'Tổng quan',
    description: 'Các màn hiển thị thông tin tổng quan và báo cáo chỉ tiêu CATP.',
    permissions: [
      { label: 'Xem bảng điều khiển', value: 'ViewDashboard', description: 'Cho phép mở màn Bảng điều khiển.' },
      { label: 'Xem báo cáo chỉ tiêu CATP', value: 'ViewCatpIndicatorReport', description: 'Cho phép xem báo cáo các chỉ tiêu Công an thành phố thực hiện.' }
    ]
  },
  {
    key: 'catalog',
    label: 'Thiết lập nền',
    description: 'Danh mục nền tảng và kỳ báo cáo dùng chung cho toàn hệ thống.',
    permissions: [
      { label: 'Quản lý danh mục chỉ tiêu', value: 'ManageIndicatorCatalog', description: 'Cho phép xem, thêm, sửa và nhập danh mục chỉ tiêu.' },
      { label: 'Quản lý danh mục đơn vị', value: 'ManageUnitCatalog', description: 'Cho phép xem, thêm, sửa và nhập danh mục đơn vị.' },
      { label: 'Quản lý kỳ báo cáo', value: 'ManageReportingPeriods', description: 'Cho phép tạo và chỉnh sửa các kỳ báo cáo.' }
    ]
  },
  {
    key: 'assignment',
    label: 'Giao và phân bổ',
    description: 'Các chức năng tạo đợt giao và phân bổ chỉ tiêu xuống từng cấp đơn vị.',
    permissions: [
      { label: 'Quản lý đợt giao chỉ tiêu', value: 'ManageAssignmentWaves', description: 'Cho phép tạo và cập nhật đợt giao chỉ tiêu.' },
      { label: 'Giao chỉ tiêu cho CATP', value: 'AssignTargetsToCatp', description: 'Cho phép giao chỉ tiêu cho Công an thành phố.' },
      { label: 'Giao chỉ tiêu cho Phòng', value: 'AssignTargetsToPhong', description: 'Cho phép giao chỉ tiêu cho khối phòng.' },
      { label: 'Giao chỉ tiêu cho CADP phường/xã', value: 'AssignTargetsToCadp', description: 'Cho phép giao chỉ tiêu cho CADP phường/xã.' }
    ]
  },
  {
    key: 'tracking',
    label: 'Theo dõi thực hiện',
    description: 'Nhập báo cáo định kỳ và theo dõi tình hình cập nhật kết quả.',
    permissions: [
      { label: 'Nhập báo cáo định kỳ', value: 'SubmitPeriodicReports', description: 'Cho phép nhập và chỉnh sửa kết quả báo cáo định kỳ.' },
      { label: 'Xem tiến độ thực hiện', value: 'ViewExecutionProgress', description: 'Cho phép xem trang tiến độ thực hiện chỉ tiêu.' },
      { label: 'Xem đơn vị chưa cập nhật', value: 'ViewUnitsPendingUpdate', description: 'Cho phép xem danh sách đơn vị chưa cập nhật báo cáo.' }
    ]
  },
  {
    key: 'evaluation',
    label: 'Đánh giá',
    description: 'Cấu hình ngưỡng, so sánh, xếp hạng và các màn thi đua.',
    permissions: [
      { label: 'Cấu hình ngưỡng đánh giá', value: 'ConfigureEvaluationThresholds', description: 'Cho phép cấu hình ngưỡng và quy tắc đánh giá.' },
      { label: 'Xem tổng hợp đánh giá lũy kế', value: 'ViewAccumulatedEvaluation', description: 'Cho phép mở màn tổng hợp đánh giá lũy kế.' },
      { label: 'Xem cảnh báo rủi ro', value: 'ViewRiskWarnings', description: 'Cho phép xem cảnh báo các KPI có rủi ro.' },
      { label: 'So sánh đơn vị', value: 'CompareUnits', description: 'Cho phép xem trang so sánh giữa các đơn vị.' },
      { label: 'Xếp hạng đơn vị', value: 'RankUnits', description: 'Cho phép xem bảng xếp hạng đơn vị.' },
      { label: 'Thiết lập nhóm thi đua', value: 'ConfigureCompetitionGroups', description: 'Cho phép cấu hình nhóm thi đua, đơn vị và chỉ tiêu áp dụng.' },
      { label: 'Xem nhóm thi đua', value: 'ViewCompetitionGroups', description: 'Cho phép xem tổng hợp và xếp hạng trong nhóm thi đua.' }
    ]
  },
  {
    key: 'reporting',
    label: 'Báo cáo',
    description: 'Các màn báo cáo tổng hợp, theo đơn vị, theo chỉ tiêu và xuất báo cáo.',
    permissions: [
      { label: 'Xem báo cáo tổng hợp', value: 'ViewSummaryReports', description: 'Cho phép mở màn báo cáo tổng hợp KPI.' },
      { label: 'Xem báo cáo theo đơn vị', value: 'ViewReportsByUnit', description: 'Cho phép xem báo cáo tổng hợp theo đơn vị.' },
      { label: 'Xem báo cáo theo chỉ tiêu', value: 'ViewReportsByIndicator', description: 'Cho phép xem báo cáo theo từng chỉ tiêu.' },
      { label: 'Xuất báo cáo', value: 'ExportReports', description: 'Cho phép dùng các màn xuất Excel/PDF.' }
    ]
  },
  {
    key: 'admin',
    label: 'Quản trị hệ thống',
    description: 'Các chức năng người dùng, phân quyền và nhật ký hệ thống.',
    permissions: [
      { label: 'Quản trị người dùng', value: 'ManageUsers', description: 'Cho phép xem danh sách người dùng và gán vai trò.' },
      { label: 'Quản trị phân quyền', value: 'ManagePermissions', description: 'Cho phép mở và cấu hình màn phân quyền.' },
      { label: 'Xem nhật ký hệ thống', value: 'ViewSystemLogs', description: 'Cho phép xem nhật ký hệ thống.' }
    ]
  }
]

const allPermissionOptions = permissionGroups.flatMap(group => group.permissions)
const defaultRoles = ['Admin', 'Công an Thành phố', 'Công an cấp Phòng', 'Công an cấp Xã/Phường']

const users = ref([])
const roles = ref([])
const selectedUserId = ref('')
const selectedRoleId = ref('')
const selectedPermissions = ref([])
const originalPermissions = ref([])
const selectedUserRoles = ref([])
const loadingUsers = ref(false)
const loadingRoles = ref(false)
const loadingDetail = ref(false)
const saving = ref(false)
const error = ref('')
const successMessage = ref('')
const activeTab = ref('USER')
const targetSearch = ref('')
const groupFilter = ref('ALL')
const openedGroups = ref(permissionGroups.reduce((acc, group, index) => {
  acc[group.key] = index < 2
  return acc
}, {}))
const expandAll = ref(false)

const selectedUser = computed(() => users.value.find(item => item.id === selectedUserId.value) || null)
const selectedRole = computed(() => roles.value.find(item => item.id === selectedRoleId.value) || null)
const selectedRoleName = computed(() => selectedRole.value?.roleName || '')
const selectedRoleUserCount = computed(() => selectedRole.value?.userCount || 0)

const activeTargetName = computed(() => {
  if (activeTab.value === 'USER') {
    return selectedUser.value ? (selectedUser.value.fullName || selectedUser.value.userName || selectedUser.value.email) : ''
  }
  return selectedRoleName.value
})

const filteredUsers = computed(() => {
  const keyword = targetSearch.value.trim().toLowerCase()
  if (!keyword) return users.value

  return users.value.filter(item => [
    item.fullName,
    item.userName,
    item.email,
    item.phoneNumber
  ].some(value => String(value || '').toLowerCase().includes(keyword)))
})

const filteredRoles = computed(() => {
  const keyword = targetSearch.value.trim().toLowerCase()
  if (!keyword) return roles.value
  return roles.value.filter(item => String(item.roleName || '').toLowerCase().includes(keyword))
})

const visiblePermissionGroups = computed(() => {
  if (groupFilter.value === 'ALL') return permissionGroups
  return permissionGroups.filter(group => group.key === groupFilter.value)
})

const permissionValueSet = computed(() => new Set(allPermissionOptions.map(item => item.value)))

const handleApiError = (err, fallback) => {
  console.error(err)
  if (err.response?.status === 401) return (error.value = 'Phiên đăng nhập đã hết hạn.')
  if (err.response?.status === 403) return (error.value = 'Bạn không có quyền truy cập chức năng này.')
  error.value = err.response?.data?.message || err.response?.data?.title || fallback
}

const normalizeRole = (item) => ({
  id: item?.id || item?.Id || '',
  roleName: item?.roleName || item?.RoleName || '',
  permissions: Array.isArray(item?.permissions) ? item.permissions : Array.isArray(item?.Permissions) ? item.Permissions : [],
  userCount: Number(item?.userCount ?? item?.UserCount ?? 0)
})

const buildRoleFallbacks = () => {
  const roleNames = new Set(defaultRoles)
  users.value.forEach(user => (user.roles || []).forEach(role => roleNames.add(role)))

  return [...roleNames]
    .map(roleName => ({
      id: `name:${roleName}`,
      roleName,
      permissions: [],
      userCount: users.value.filter(user => (user.roles || []).includes(roleName)).length
    }))
    .sort((left, right) => left.roleName.localeCompare(right.roleName, 'vi'))
}

const fetchUsers = async () => {
  try {
    loadingUsers.value = true
    const res = await httpClient.get('/admin/users')
    users.value = Array.isArray(res.data) ? res.data : []
  } catch (err) {
    handleApiError(err, 'Không tải được danh sách user.')
  } finally {
    loadingUsers.value = false
  }
}

const fetchRoles = async () => {
  try {
    loadingRoles.value = true
    const res = await httpClient.get('/admin/roles')
    roles.value = Array.isArray(res.data) ? res.data.map(normalizeRole) : []
  } catch (err) {
    console.error(err)
    roles.value = buildRoleFallbacks()
  } finally {
    loadingRoles.value = false
  }
}

const normalizePermissions = (permissions) => {
  if (!Array.isArray(permissions)) return []
  return permissions.filter(permission => permissionValueSet.value.has(permission))
}

const fetchUserPermissions = async (userId) => {
  try {
    loadingDetail.value = true
    error.value = ''
    successMessage.value = ''
    const res = await httpClient.get(`/admin/users/${userId}`)
    const permissions = normalizePermissions(res.data?.permissions)
    originalPermissions.value = [...permissions]
    selectedPermissions.value = [...permissions]
    selectedUserRoles.value = Array.isArray(res.data?.roles) ? [...res.data.roles] : []
  } catch (err) {
    originalPermissions.value = []
    selectedPermissions.value = []
    selectedUserRoles.value = []
    handleApiError(err, 'Không tải được quyền của user.')
  } finally {
    loadingDetail.value = false
  }
}

const fetchRolePermissions = async (roleId) => {
  try {
    loadingDetail.value = true
    error.value = ''
    successMessage.value = ''
    const res = await httpClient.get(`/admin/roles/${roleId}`)
    const permissions = normalizePermissions(res.data?.permissions)
    originalPermissions.value = [...permissions]
    selectedPermissions.value = [...permissions]
  } catch (err) {
    originalPermissions.value = []
    selectedPermissions.value = []
    handleApiError(err, 'Không tải được quyền của role.')
  } finally {
    loadingDetail.value = false
  }
}

const handleUserChange = async () => {
  if (!selectedUserId.value) {
    originalPermissions.value = []
    selectedPermissions.value = []
    selectedUserRoles.value = []
    return
  }
  await fetchUserPermissions(selectedUserId.value)
}

const handleRoleChange = async () => {
  if (!selectedRoleId.value) {
    originalPermissions.value = []
    selectedPermissions.value = []
    return
  }
  await fetchRolePermissions(selectedRoleId.value)
}

const saveUserPermissions = async () => {
  const current = new Set(originalPermissions.value)
  const next = new Set(selectedPermissions.value)
  const permissionsToAdd = [...next].filter(permission => !current.has(permission))
  const permissionsToRemove = [...current].filter(permission => !next.has(permission))

  await Promise.all([
    ...permissionsToAdd.map(permissionName => httpClient.post(`/admin/users/${selectedUserId.value}/permissions`, { permissionName })),
    ...permissionsToRemove.map(permissionName => httpClient.delete(`/admin/users/${selectedUserId.value}/permissions`, { data: { permissionName } }))
  ])

  originalPermissions.value = [...selectedPermissions.value]
  successMessage.value = permissionsToAdd.length || permissionsToRemove.length
    ? 'Cập nhật quyền cho user thành công.'
    : 'Không có thay đổi để lưu.'
}

const saveRolePermissions = async () => {
  const current = new Set(originalPermissions.value)
  const next = new Set(selectedPermissions.value)
  const permissionsToAdd = [...next].filter(permission => !current.has(permission))
  const permissionsToRemove = [...current].filter(permission => !next.has(permission))

  await Promise.all([
    ...permissionsToAdd.map(permissionName => httpClient.post(`/admin/roles/${selectedRoleId.value}/permissions`, { permissionName })),
    ...permissionsToRemove.map(permissionName => httpClient.delete(`/admin/roles/${selectedRoleId.value}/permissions`, { data: { permissionName } }))
  ])

  originalPermissions.value = [...selectedPermissions.value]
  successMessage.value = permissionsToAdd.length || permissionsToRemove.length
    ? 'Cập nhật quyền cho role thành công.'
    : 'Không có thay đổi để lưu.'
}

const savePermissions = async () => {
  error.value = ''
  successMessage.value = ''

  if (activeTab.value === 'USER' && !selectedUserId.value) {
    error.value = 'Vui lòng chọn user.'
    return
  }

  if (activeTab.value === 'ROLE' && !selectedRoleId.value) {
    error.value = 'Vui lòng chọn role.'
    return
  }

  try {
    saving.value = true
    if (activeTab.value === 'USER') {
      await saveUserPermissions()
      await fetchUsers()
    } else {
      await saveRolePermissions()
      await fetchRoles()
    }
  } catch (err) {
    handleApiError(err, 'Lưu phân quyền thất bại.')
  } finally {
    saving.value = false
  }
}

const switchTab = (tab) => {
  activeTab.value = tab
  targetSearch.value = ''
  error.value = ''
  successMessage.value = ''
  originalPermissions.value = []
  selectedPermissions.value = []
  if (tab === 'USER') {
    selectedRoleId.value = ''
  } else {
    selectedUserId.value = ''
    selectedUserRoles.value = []
  }
}

const toggleGroup = (groupKey) => {
  openedGroups.value = {
    ...openedGroups.value,
    [groupKey]: !isGroupOpen(groupKey)
  }
}

const isGroupOpen = (groupKey) => {
  return expandAll.value || !!openedGroups.value[groupKey]
}

const countSelectedInGroup = (group) => {
  const selectedSet = new Set(selectedPermissions.value)
  return group.permissions.filter(permission => selectedSet.has(permission.value)).length
}

const checkGroup = (group) => {
  const merged = new Set(selectedPermissions.value)
  group.permissions.forEach(permission => merged.add(permission.value))
  selectedPermissions.value = [...merged]
}

const uncheckGroup = (group) => {
  const values = new Set(group.permissions.map(permission => permission.value))
  selectedPermissions.value = selectedPermissions.value.filter(permission => !values.has(permission))
}

const checkAllVisible = () => {
  const merged = new Set(selectedPermissions.value)
  visiblePermissionGroups.value.forEach(group => {
    group.permissions.forEach(permission => merged.add(permission.value))
  })
  selectedPermissions.value = [...merged]
}

const uncheckAllVisible = () => {
  const visibleValues = new Set(visiblePermissionGroups.value.flatMap(group => group.permissions.map(permission => permission.value)))
  selectedPermissions.value = selectedPermissions.value.filter(permission => !visibleValues.has(permission))
}

onMounted(async () => {
  await Promise.all([fetchUsers(), fetchRoles()])
})
</script>

<style scoped>
.page-shell{padding:20px}
.page-header{display:flex;justify-content:space-between;align-items:center;gap:16px;flex-wrap:wrap;margin-bottom:18px}
.gov-banner{display:flex;align-items:center;gap:16px;padding:20px 24px;border-radius:20px;background:linear-gradient(135deg,#ffffff 0%,#f4f9ff 100%);box-shadow:0 10px 30px rgba(13,110,253,.08);border:1px solid rgba(13,110,253,.08)}
.gov-emblem{width:64px;height:64px;border-radius:18px;display:flex;align-items:center;justify-content:center;background:linear-gradient(135deg,#0d6efd,#4ea1ff);color:#fff;font-size:28px;box-shadow:0 10px 24px rgba(13,110,253,.24)}
.gov-title{font-size:1.3rem;font-weight:800;color:#1f2d3d;margin-bottom:4px}
.gov-sub{font-size:.95rem;color:#4f6b88;opacity:1}
.mode-switch{display:flex;gap:10px;flex-wrap:wrap;margin-bottom:18px}
.summary-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:14px;margin-bottom:18px}
.summary-card{padding:16px 18px;border-radius:16px;background:#fff;border:1px solid #e9ecef;box-shadow:0 10px 20px rgba(15,23,42,.05)}
.summary-label{display:block;font-size:12px;color:#64748b;text-transform:uppercase;font-weight:700;margin-bottom:6px}
.summary-value{font-size:28px;line-height:1}
.selector-grid{display:grid;grid-template-columns:1fr 1fr 1fr;gap:14px}
.loading-state{display:flex;align-items:center;gap:10px;color:#475569;padding:18px 0}
.permission-header{display:flex;justify-content:space-between;align-items:center;gap:12px;flex-wrap:wrap;margin-bottom:18px}
.permission-actions{display:flex;gap:8px;flex-wrap:wrap}
.meta-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:12px}
.meta-card{padding:14px 16px;border:1px solid #e5e7eb;border-radius:14px;background:#f8fafc}
.meta-label{display:block;font-size:12px;font-weight:700;text-transform:uppercase;color:#64748b;margin-bottom:8px}
.meta-value{min-height:24px}
.group-list{display:flex;flex-direction:column;gap:14px}
.group-card{border:1px solid #e2e8f0;border-radius:18px;background:#fff;box-shadow:0 8px 20px rgba(15,23,42,.04);overflow:hidden}
.group-header{width:100%;display:flex;align-items:center;justify-content:space-between;gap:12px;padding:16px 18px;background:#f8fafc;border:none;text-align:left}
.group-header-right{display:flex;align-items:center;gap:10px}
.group-body{padding:16px 18px;border-top:1px solid #e2e8f0}
.group-actions{display:flex;justify-content:flex-end;gap:8px;flex-wrap:wrap;margin-bottom:12px}
.permission-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(280px,1fr));gap:14px}
.permission-card{display:flex;justify-content:space-between;align-items:flex-start;gap:14px;padding:16px 18px;border:1px solid #dbe4ef;border-radius:18px;background:linear-gradient(180deg,#ffffff 0%,#f8fbff 100%);box-shadow:0 10px 24px rgba(15,23,42,.05);cursor:pointer}
.permission-card:hover{border-color:#60a5fa;background:linear-gradient(180deg,#ffffff 0%,#f0f7ff 100%);box-shadow:0 14px 28px rgba(37,99,235,.10)}
.permission-card-main{display:flex;gap:12px;align-items:flex-start}
.permission-copy{min-width:0}
.permission-name{font-size:15px;color:#0f172a}
.permission-description{font-size:13px;color:#475569;margin-top:6px;line-height:1.45}
.permission-code{display:inline-flex;align-items:center;margin-top:10px;padding:5px 12px;border-radius:999px;background:linear-gradient(135deg,#eff6ff,#dbeafe);color:#1d4ed8;font-size:11px;font-weight:700;letter-spacing:.02em;border:1px solid #bfdbfe}
.permission-state-badge{display:inline-flex;align-items:center;justify-content:center;min-width:92px;padding:8px 12px;border-radius:999px;font-size:12px;font-weight:700;line-height:1;border:1px solid transparent;white-space:nowrap}
.permission-state-badge.is-selected{background:#dcfce7;color:#166534;border-color:#bbf7d0;box-shadow:inset 0 1px 0 rgba(255,255,255,.45)}
.permission-state-badge.is-unselected{background:#f8fafc;color:#475569;border-color:#cbd5e1}
.role-chip{background:linear-gradient(135deg,#dbeafe,#eff6ff);color:#1d4ed8;border:1px solid #bfdbfe;font-weight:700}
.save-bar{display:flex;justify-content:space-between;align-items:center;gap:12px;flex-wrap:wrap;margin-top:18px;padding-top:16px;border-top:1px solid #e2e8f0}
@media (max-width: 1100px){.selector-grid{grid-template-columns:1fr 1fr}}
@media (max-width: 992px){.selector-grid{grid-template-columns:1fr}.permission-grid{grid-template-columns:1fr}}
</style>


