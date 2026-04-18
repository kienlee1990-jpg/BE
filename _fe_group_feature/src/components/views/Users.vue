<template>
  <BaseLayout>
    <div class="page-shell">
      <div class="page-header">
        <div class="gov-banner">
          <img src="https://upload.wikimedia.org/wikipedia/commons/a/a3/Emblem_of_Vietnam.svg" class="gov-emblem" alt="Vietnam emblem" />
          <div>
            <div class="gov-title">NGÆ¯á»œI DÃ™NG VÃ€ VAI TRÃ’</div>
            <div class="gov-sub">Quáº£n lÃ½ tÃ i khoáº£n vÃ  gÃ¡n vai trÃ² cho tá»«ng ngÆ°á»i dÃ¹ng</div>
          </div>
        </div>
        <div class="header-actions">
          <RouterLink to="/permissions" class="btn btn-outline-primary">Má»Ÿ mÃ n phÃ¢n quyá»n chi tiáº¿t</RouterLink>
          <button @click="openAdd" class="btn btn-primary">+ ThÃªm ngÆ°á»i dÃ¹ng</button>
        </div>
      </div>

      <div class="alert alert-info border-0 shadow-sm">
        <strong>MÃ n nÃ y chá»‰ dÃ¹ng Ä‘á»ƒ gÃ¡n vai trÃ².</strong>
        Quyá»n chi tiáº¿t theo user vÃ  theo role Ä‘Æ°á»£c cáº¥u hÃ¬nh táº¡i mÃ n PhÃ¢n quyá»n.
      </div>

      <div class="summary-grid">
        <div class="summary-card">
          <span class="summary-label">Tá»•ng ngÆ°á»i dÃ¹ng</span>
          <strong class="summary-value">{{ users.length }}</strong>
        </div>
        <div class="summary-card">
          <span class="summary-label">Äang hoáº¡t Ä‘á»™ng</span>
          <strong class="summary-value text-success">{{ activeUsersCount }}</strong>
        </div>
        <div class="summary-card">
          <span class="summary-label">Ngá»«ng hoáº¡t Ä‘á»™ng</span>
          <strong class="summary-value text-danger">{{ inactiveUsersCount }}</strong>
        </div>
        <div class="summary-card">
          <span class="summary-label">CÃ³ vai trÃ²</span>
          <strong class="summary-value text-primary">{{ usersWithRolesCount }}</strong>
        </div>
      </div>

      <div class="card shadow-sm border-0 mb-3">
        <div class="card-body">
          <div class="filter-grid">
            <div>
              <label class="form-label">Tá»« khÃ³a</label>
              <input
                v-model.trim="searchTerm"
                type="text"
                class="form-control"
                placeholder="Nháº­p há» tÃªn, username, email hoáº·c sá»‘ Ä‘iá»‡n thoáº¡i"
              />
            </div>
            <div>
              <label class="form-label">Tráº¡ng thÃ¡i</label>
              <select v-model="statusFilter" class="form-select">
                <option value="ALL">Táº¥t cáº£</option>
                <option value="ACTIVE">Äang hoáº¡t Ä‘á»™ng</option>
                <option value="INACTIVE">Ngá»«ng hoáº¡t Ä‘á»™ng</option>
              </select>
            </div>
            <div>
              <label class="form-label">Vai trÃ²</label>
              <select v-model="roleFilter" class="form-select">
                <option value="ALL">Táº¥t cáº£ vai trÃ²</option>
                <option value="NONE">ChÆ°a cÃ³ vai trÃ²</option>
                <option v-for="role in availableRoles" :key="role" :value="role">{{ role }}</option>
              </select>
            </div>
          </div>
        </div>
      </div>

      <div v-if="loading" class="loading-state">
        <div class="spinner-border text-primary"></div>
        <span>Äang táº£i danh sÃ¡ch ngÆ°á»i dÃ¹ng...</span>
      </div>

      <div v-else-if="filteredUsers.length" class="card shadow-sm border-0">
        <div class="card-body p-0">
          <ColumnVisibilityTools table-id="Users-table" />
                            <table id="Users-table" class="table table-hover align-middle mb-0 permission-table managed-table">
            <thead>
              <tr>
                <th>NgÆ°á»i dÃ¹ng</th>
                <th>LiÃªn há»‡</th>
                <th>Vai trÃ²</th>
                <th>Quyá»n trá»±c tiáº¿p</th>
                <th class="text-center">Tráº¡ng thÃ¡i</th>
                <th class="text-end">Thao tÃ¡c</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in filteredUsers" :key="item.id">
                <td class="user-cell">
                  <strong class="d-block">{{ item.fullName || item.userName || item.email || "ChÆ°a Ä‘áº·t tÃªn" }}</strong>
                  <span class="muted-line">@{{ item.userName || "khÃ´ng cÃ³ username" }}</span>
                </td>
                <td>
                  <strong class="d-block">{{ item.email || "-" }}</strong>
                  <span class="muted-line">{{ item.phoneNumber || item.address || "ChÆ°a cáº­p nháº­t liÃªn há»‡" }}</span>
                </td>
                <td>
                  <div class="chip-wrap">
                    <span v-for="role in item.roles || []" :key="role" class="badge role-chip">{{ role }}</span>
                    <span v-if="!(item.roles || []).length" class="text-muted">ChÆ°a cÃ³ vai trÃ²</span>
                  </div>
                </td>
                <td>
                  <div class="chip-wrap">
                    <span v-for="permission in item.permissions || []" :key="permission" class="badge permission-chip">{{ permission }}</span>
                    <span v-if="!(item.permissions || []).length" class="text-muted">KhÃ´ng cÃ³ quyá»n trá»±c tiáº¿p</span>
                  </div>
                </td>
                <td class="text-center">
                  <template v-if="!isProtectedUser(item)">
                    <button
                      type="button"
                      class="status-toggle"
                      :class="item.isActive ? 'status-active' : 'status-inactive'"
                      :disabled="item._loading"
                      @click="confirmToggle(item)"
                    >
                      <span v-if="item._loading" class="spinner-border spinner-border-sm"></span>
                      <span v-else>{{ item.isActive ? "Äang hoáº¡t Ä‘á»™ng" : "Ngá»«ng hoáº¡t Ä‘á»™ng" }}</span>
                    </button>
                  </template>
                  <span v-else class="badge text-bg-warning">TÃ i khoáº£n há»‡ thá»‘ng</span>
                </td>
                <td class="text-end">
                  <div class="action-stack">
                    <button @click="openEdit(item)" class="btn btn-sm btn-outline-warning">Sá»­a</button>
                    <button
                      v-if="!isProtectedUser(item)"
                      @click="openRole(item)"
                      class="btn btn-sm btn-primary"
                    >
                      GÃ¡n vai trÃ²
                    </button>
                    <span v-else class="text-muted small">KhÃ´ng chá»‰nh vai trÃ²</span>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <div v-else class="alert alert-light border shadow-sm">
        KhÃ´ng tÃ¬m tháº¥y ngÆ°á»i dÃ¹ng phÃ¹ há»£p vá»›i bá»™ lá»c hiá»‡n táº¡i.
      </div>

      <div v-if="showForm" class="modal fade show d-block" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered">
          <div class="modal-content shadow-lg border-0 rounded-4">
            <div class="modal-header bg-primary text-white rounded-top-4">
              <h5 class="modal-title">{{ isEdit ? "Cáº­p nháº­t ngÆ°á»i dÃ¹ng" : "ThÃªm ngÆ°á»i dÃ¹ng má»›i" }}</h5>
              <button class="btn-close btn-close-white" @click="closeForm"></button>
            </div>
            <div class="modal-body px-4 py-3">
              <template v-if="!isEdit">
                <div class="mb-3">
                  <label class="form-label">Há» tÃªn</label>
                  <input v-model="form.fullName" placeholder="Nháº­p há» tÃªn" class="form-control shadow-sm" />
                </div>
                <div class="mb-3">
                  <label class="form-label">TÃªn Ä‘Äƒng nháº­p</label>
                  <input v-model="form.userName" placeholder="Nháº­p username" class="form-control shadow-sm" />
                </div>
                <div class="mb-3">
                  <label class="form-label">Email</label>
                  <input v-model="form.email" placeholder="Nháº­p email" class="form-control shadow-sm" />
                </div>
                <div class="mb-3">
                  <label class="form-label">Máº­t kháº©u</label>
                  <input type="password" v-model="form.password" placeholder="Nháº­p máº­t kháº©u" class="form-control shadow-sm" />
                </div>
                <div class="mb-3">
                  <label class="form-label">Äá»‹a chá»‰</label>
                  <input v-model="form.address" placeholder="Nháº­p Ä‘á»‹a chá»‰" class="form-control shadow-sm" />
                </div>
                <div>
                  <label class="form-label">Sá»‘ Ä‘iá»‡n thoáº¡i</label>
                  <input v-model="form.phoneNumber" placeholder="Nháº­p sá»‘ Ä‘iá»‡n thoáº¡i" class="form-control shadow-sm" />
                </div>
              </template>
              <template v-else>
                <div class="mb-3">
                  <label class="form-label">Há» tÃªn</label>
                  <input v-model="form.fullName" placeholder="Nháº­p há» tÃªn" class="form-control shadow-sm" />
                </div>
                <div class="mb-3">
                  <label class="form-label">Äá»‹a chá»‰</label>
                  <input v-model="form.address" placeholder="Nháº­p Ä‘á»‹a chá»‰" class="form-control shadow-sm" />
                </div>
                <div>
                  <label class="form-label">Sá»‘ Ä‘iá»‡n thoáº¡i</label>
                  <input v-model="form.phoneNumber" placeholder="Nháº­p sá»‘ Ä‘iá»‡n thoáº¡i" class="form-control shadow-sm" />
                </div>
              </template>
            </div>
            <div class="modal-footer border-0 px-4 pb-4">
              <button class="btn btn-outline-secondary rounded-pill px-4" @click="closeForm">Há»§y</button>
              <button class="btn btn-primary rounded-pill px-4 shadow-sm" @click="saveUser">LÆ°u</button>
            </div>
          </div>
        </div>
      </div>

      <div v-if="showRoleForm" class="modal fade show d-block">
        <div class="modal-dialog modal-dialog-centered modal-lg">
          <div class="modal-content shadow-lg border-0 rounded-4">
            <div class="modal-header bg-success text-white rounded-top-4">
              <div>
                <h5 class="modal-title mb-1">GÃ¡n vai trÃ²</h5>
                <div class="small opacity-75">{{ currentUserName }}</div>
              </div>
              <button class="btn-close btn-close-white" @click="showRoleForm = false"></button>
            </div>
            <div class="modal-body px-4 py-3">
              <div v-if="roleLoading" class="text-center py-4">
                <div class="spinner-border text-primary"></div>
              </div>
              <div v-else>
                <div class="role-modal-toolbar">
                  <div class="alert alert-light border mb-0 flex-grow-1">
                    Chá»n vai trÃ² cáº§n gÃ¡n cho ngÆ°á»i dÃ¹ng nÃ y. Náº¿u cáº§n phÃ¢n quyá»n chi tiáº¿t hÆ¡n, má»Ÿ mÃ n PhÃ¢n quyá»n Ä‘á»ƒ gÃ¡n trá»±c tiáº¿p theo user hoáº·c role.
                  </div>
                  <div class="d-flex gap-2">
                    <button type="button" class="btn btn-outline-success btn-sm" @click="selectAllRoles">Chá»n táº¥t cáº£</button>
                    <button type="button" class="btn btn-outline-secondary btn-sm" @click="clearRoles">Bá» háº¿t</button>
                  </div>
                </div>

                <div class="role-grid mt-3">
                  <label v-for="role in availableRoles" :key="role" class="role-option">
                    <input type="checkbox" :value="role" v-model="selectedRoles" />
                    <span>
                      <strong class="d-block">{{ role }}</strong>
                      <small class="text-muted">{{ selectedRoles.includes(role) ? "ÄÃ£ chá»n" : "ChÆ°a chá»n" }}</small>
                    </span>
                  </label>
                </div>
              </div>
            </div>
            <div class="modal-footer border-0 px-4 pb-4">
              <button class="btn btn-outline-secondary rounded-pill px-4" @click="showRoleForm = false">Há»§y</button>
              <button class="btn btn-success rounded-pill px-4 shadow-sm" @click="saveRole">LÆ°u vai trÃ²</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </BaseLayout>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import BaseLayout from '../BaseLayout.vue'
import ColumnVisibilityTools from '../shared/ColumnVisibilityTools.vue'
import httpClient from '../../services/httpClient'

const users = ref([])
const loading = ref(false)
const showForm = ref(false)
const showRoleForm = ref(false)
const roleLoading = ref(false)
const isEdit = ref(false)
const searchTerm = ref('')
const statusFilter = ref('ALL')
const roleFilter = ref('ALL')
const defaultRoles = ['Admin', 'CÃ´ng an ThÃ nh phá»‘', 'CÃ´ng an cáº¥p PhÃ²ng', 'CÃ´ng an cáº¥p XÃ£/PhÆ°á»ng']
const selectedRoles = ref([])
const currentUserId = ref(null)
const currentUserName = ref('')
const form = ref({ id: null, email: '', password: '', userName: '', fullName: '', phoneNumber: '', address: '', isActive: true })
const protectedUserId = 'd12c48e3-825f-4799-b6f1-a55857131796'

const isProtectedUser = (user) => user?.id === protectedUserId

const availableRoles = computed(() => {
  const merged = new Set(defaultRoles)
  users.value.forEach(item => (item.roles || []).forEach(role => merged.add(role)))
  return [...merged].sort((left, right) => left.localeCompare(right, 'vi'))
})

const activeUsersCount = computed(() => users.value.filter(item => item.isActive).length)
const inactiveUsersCount = computed(() => users.value.filter(item => !item.isActive).length)
const usersWithRolesCount = computed(() => users.value.filter(item => (item.roles || []).length > 0).length)

const filteredUsers = computed(() => {
  const keyword = searchTerm.value.trim().toLowerCase()

  return users.value.filter(item => {
    const matchesKeyword = !keyword || [
      item.fullName,
      item.userName,
      item.email,
      item.phoneNumber,
      item.address
    ].some(value => String(value || '').toLowerCase().includes(keyword))

    const matchesStatus =
      statusFilter.value === 'ALL' ||
      (statusFilter.value === 'ACTIVE' && item.isActive) ||
      (statusFilter.value === 'INACTIVE' && !item.isActive)

    const userRoles = item.roles || []
    const matchesRole =
      roleFilter.value === 'ALL' ||
      (roleFilter.value === 'NONE' && !userRoles.length) ||
      userRoles.includes(roleFilter.value)

    return matchesKeyword && matchesStatus && matchesRole
  })
})

const fetchUsers = async () => {
  loading.value = true
  try {
    const res = await httpClient.get('/admin/users')
    users.value = (Array.isArray(res.data) ? res.data : [])
      .map(item => ({ ...item, _loading: false }))
      .sort((a, b) => (a.id === protectedUserId ? -1 : b.id === protectedUserId ? 1 : 0))
  } catch (err) {
    console.error(err)
    alert('KhÃ´ng táº£i Ä‘Æ°á»£c danh sÃ¡ch ngÆ°á»i dÃ¹ng.')
  } finally {
    loading.value = false
  }
}

const openAdd = () => {
  isEdit.value = false
  form.value = { id: null, email: '', password: '', userName: '', fullName: '', phoneNumber: '', address: '', isActive: true }
  showForm.value = true
}

const openEdit = (user) => {
  isEdit.value = true
  form.value = {
    id: user.id,
    fullName: user.fullName || '',
    address: user.address || '',
    phoneNumber: user.phoneNumber || '',
    isActive: user.isActive
  }
  showForm.value = true
}

const saveUser = async () => {
  try {
    if (isEdit.value) {
      await httpClient.put(`/admin/users/${form.value.id}`, {
        fullName: form.value.fullName,
        address: form.value.address,
        phoneNumber: form.value.phoneNumber,
        isActive: form.value.isActive
      })
    } else {
      await httpClient.post('/auth/register', {
        email: form.value.email,
        password: form.value.password,
        userName: form.value.userName,
        fullName: form.value.fullName,
        phoneNumber: form.value.phoneNumber,
        address: form.value.address
      })
    }
    showForm.value = false
    await fetchUsers()
  } catch (err) {
    console.error(err)
    alert('LÆ°u ngÆ°á»i dÃ¹ng tháº¥t báº¡i.')
  }
}

const confirmToggle = (user) => {
  const ok = confirm(`Báº¡n cÃ³ cháº¯c muá»‘n ${user.isActive ? 'ngá»«ng' : 'kÃ­ch hoáº¡t'} ngÆ°á»i dÃ¹ng nÃ y khÃ´ng?`)
  if (!ok) return
  toggleActive(user)
}

const toggleActive = async (user) => {
  try {
    user._loading = true
    await httpClient.put(`/admin/users/${user.id}`, {
      fullName: user.fullName,
      address: user.address,
      phoneNumber: user.phoneNumber,
      isActive: !user.isActive
    })
    user.isActive = !user.isActive
  } catch (err) {
    console.error(err)
    alert('Äá»•i tráº¡ng thÃ¡i tháº¥t báº¡i.')
  } finally {
    user._loading = false
  }
}

const openRole = (user) => {
  currentUserId.value = user.id
  currentUserName.value = user.fullName || user.email || user.userName || ''
  selectedRoles.value = [...(user.roles || [])]
  showRoleForm.value = true
}

const selectAllRoles = () => {
  selectedRoles.value = [...availableRoles.value]
}

const clearRoles = () => {
  selectedRoles.value = []
}

const saveRole = async () => {
  if (!currentUserId.value) return
  try {
    roleLoading.value = true
    const user = users.value.find(item => item.id === currentUserId.value)
    const currentRoles = user?.roles || []
    const rolesToAdd = selectedRoles.value.filter(role => !currentRoles.includes(role))
    const rolesToRemove = currentRoles.filter(role => !selectedRoles.value.includes(role))

    for (const roleName of rolesToAdd) {
      await httpClient.post(`/admin/users/${currentUserId.value}/roles`, { roleName })
    }
    for (const roleName of rolesToRemove) {
      await httpClient.delete(`/admin/users/${currentUserId.value}/roles`, { data: { roleName } })
    }

    showRoleForm.value = false
    await fetchUsers()
    alert('Cáº­p nháº­t vai trÃ² thÃ nh cÃ´ng.')
  } catch (err) {
    console.error(err)
    alert('Cáº­p nháº­t vai trÃ² tháº¥t báº¡i.')
  } finally {
    roleLoading.value = false
  }
}

const closeForm = () => {
  showForm.value = false
}

onMounted(fetchUsers)
</script>

<style scoped>
.page-shell{padding:20px}
.page-header{display:flex;justify-content:space-between;align-items:center;gap:16px;flex-wrap:wrap;margin-bottom:18px}
.header-actions{display:flex;gap:12px;flex-wrap:wrap}
.gov-banner{display:flex;align-items:center;gap:14px;max-width:520px;background:linear-gradient(180deg,#03943f 0%,#05ce48dd 100%);padding:14px 18px;border-radius:14px;color:#fff;box-shadow:0 10px 24px rgba(3,148,63,.22)}
.gov-emblem{width:48px;height:48px;object-fit:contain;filter:drop-shadow(0 2px 6px rgba(0,0,0,.3))}
.gov-title{font-size:18px;font-weight:700;letter-spacing:.4px}
.gov-sub{font-size:12px;opacity:.92}
.summary-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(180px,1fr));gap:14px;margin-bottom:18px}
.summary-card{padding:16px 18px;border-radius:16px;background:#fff;border:1px solid #e9ecef;box-shadow:0 10px 20px rgba(15,23,42,.05)}
.summary-label{display:block;font-size:12px;color:#64748b;text-transform:uppercase;font-weight:700;margin-bottom:6px}
.summary-value{font-size:28px;line-height:1}
.filter-grid{display:grid;grid-template-columns:2fr 1fr 1fr;gap:14px}
.loading-state{display:flex;align-items:center;gap:10px;color:#475569;padding:18px 0}
.permission-table thead th{background:#f8fafc;border-bottom:1px solid #dbe4ef;color:#0f172a;font-size:13px;text-transform:uppercase;letter-spacing:.03em}
.permission-table td,.permission-table th{padding:16px;border-color:#eef2f7}
.user-cell strong{font-size:15px}
.muted-line{display:block;font-size:12px;color:#64748b;margin-top:3px}
.chip-wrap{display:flex;flex-wrap:wrap;gap:6px}
.role-chip{background:#dbeafe;color:#1d4ed8}
.permission-chip{background:#f8fafc;color:#334155;border:1px solid #cbd5e1}
.status-toggle{border:none;border-radius:999px;padding:8px 14px;font-size:13px;font-weight:600;min-width:140px}
.status-active{background:#dcfce7;color:#166534}
.status-inactive{background:#fee2e2;color:#991b1b}
.action-stack{display:flex;justify-content:flex-end;gap:8px;flex-wrap:wrap}
.modal{background:rgba(15,23,42,.45)}
.role-modal-toolbar{display:flex;gap:12px;align-items:flex-start;flex-wrap:wrap}
.role-grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(220px,1fr));gap:12px}
.role-option{display:flex;gap:10px;align-items:flex-start;padding:14px 16px;border:1px solid #e2e8f0;border-radius:14px;background:#fff;cursor:pointer}
.role-option:hover{border-color:#86efac;background:#f0fdf4}
@media (max-width: 992px){.filter-grid{grid-template-columns:1fr}.permission-table{min-width:900px}}
</style>

