<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-bar-chart-line-fill"></i>
                    </div>
                    <div class="gov-text">
                        <div class="wave-title">HỆ THỐNG THEO DÕI CHỈ TIÊU CÔNG TÁC</div>
                        <div class="gov-title">QUẢN LÝ ĐỢT GIAO CHỈ TIÊU</div>
                    </div>
                </div>

                <div class="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
                    <div class="gov-banner">
                        <img src="https://upload.wikimedia.org/wikipedia/commons/a/a3/Emblem_of_Vietnam.svg" class="gov-emblem" />
                    </div>

                    <button class="btn btn-primary btn-action" @click="openCreateModal">
                        <i class="bi bi-plus-circle me-2"></i>
                        Tạo đợt giao
                    </button>
                </div>

                <div class="card custom-card mb-4">
                    <div class="card-header bg-white border-0 pb-0">
                        <h5 class="mb-1">Bộ lọc tìm kiếm</h5>
                        <small class="text-muted">Tra cuu nhanh theo nhieu tieu chi</small>
                    </div>

                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Tu khoa</label>
                                <input v-model="filters.keyword" type="text" class="form-control" placeholder="Ma hoac ten dot giao" />
                            </div>

                            <div class="col-12 col-md-6 col-xl-2">
                                <label class="form-label">Năm ap dung</label>
                                <input v-model="filters.namApDung" type="number" class="form-control" placeholder="Vi du: 2026" />
                            </div>

                            <div class="col-12 col-md-6 col-xl-2">
                                <label class="form-label">Nguồn đợt giao</label>
                                <select v-model="filters.nguonDotGiao" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option value="BO_GIAO">Bo giao</option>
                                    <option value="THANH_PHO_GIAO">Thanh pho giao</option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-2">
                                <label class="form-label">Cap giao</label>
                                <select v-model="filters.capGiao" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option value="BO">Bo</option>
                                    <option value="THANH_PHO">Thanh pho</option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Trạng thái</label>
                                <select v-model="filters.trangThai" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option value="DRAFT">Nhap</option>
                                    <option value="PUBLISHED">Da phat hanh</option>
                                    <option value="CLOSED">Da dong</option>
                                </select>
                            </div>
                        </div>

                        <div class="d-flex flex-wrap gap-2 mt-4">
                            <button class="btn btn-primary" @click="fetchDotGiaoChiTieu">
                                <i class="bi bi-search me-1"></i>
                                Tìm kiếm
                            </button>
                            <button class="btn btn-outline-secondary" @click="resetFilters">
                                <i class="bi bi-arrow-clockwise me-1"></i>
                                Đặt lại
                            </button>
                        </div>
                    </div>
                </div>

                <div class="card custom-card">
                    <div class="card-header bg-white d-flex justify-content-between align-items-center border-0">
                        <div>
                            <h5 class="mb-1">Danh sách đợt giao chỉ tiêu</h5>
                            <small class="text-muted">Theo doi va quan ly du lieu dot giao</small>
                        </div>
                        <span class="badge text-bg-light border">Tong: {{ items.length }}</span>
                    </div>

                    <div class="card-body p-0">
                        <div v-if="loading" class="empty-state">
                            <div class="spinner-border text-primary mb-3" role="status"></div>
                            <div>Đang tải dữ liệu...</div>
                        </div>

                        <div v-else-if="!items.length" class="empty-state">
                            <i class="bi bi-inbox fs-1 text-muted mb-2"></i>
                            <div>Chưa có dữ liệu</div>
                        </div>

                        <div v-else class="table-responsive">
                            <ColumnVisibilityTools table-id="DotGiaoChiTieuPage-table" />
                            <table id="DotGiaoChiTieuPage-table" class="table table-hover align-middle mb-0 custom-table managed-table">
                                <thead>
                                    <tr>
                                        <th>Ma dot</th>
                                        <th>Tên đợt giao</th>
                                        <th>Năm ap dung</th>
                                        <th>Nguồn đợt giao</th>
                                        <th>Cap giao</th>
                                        <th>Ngày bắt đầu</th>
                                        <th>Ngày kết thúc</th>
                                        <th>Trạng thái</th>
                                        <th class="text-center" style="width: 180px">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="item in items" :key="getId(item)">
                                        <td class="fw-semibold text-primary">{{ item.maDotGiao }}</td>
                                        <td>{{ item.tenDotGiao }}</td>
                                        <td>{{ item.namApDung ?? '-' }}</td>
                                        <td>{{ mapNguồnDotGiao(item.nguonDotGiao) }}</td>
                                        <td>{{ mapCapGiao(item.capGiao) }}</td>
                                        <td>{{ formatDateTime(item.ngayBatDau) }}</td>
                                        <td>{{ formatDateTime(item.ngayKetThuc) }}</td>
                                        <td>
                                            <span :class="['status-badge', getStatusClass(item.trangThai)]">
                                                {{ mapTrangThai(item.trangThai) }}
                                            </span>
                                        </td>
                                        <td class="text-center">
                                            <div class="d-flex justify-content-center gap-2">
                                                <button class="btn btn-sm btn-outline-primary" @click="openEditModal(item)">
                                                    <i class="bi bi-pencil-square me-1"></i>Sửa
                                                </button>
                                                <button class="btn btn-sm btn-outline-danger" @click="handleDelete(item)">
                                                    <i class="bi bi-trash me-1"></i>Xóa
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>

                <div v-if="showModal" class="modal fade show d-block custom-modal" tabindex="-1" @click.self="closeModal">
                    <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                        <div class="modal-content border-0 shadow-lg rounded-4">
                            <div class="modal-header border-0 pb-0">
                                <div>
                                    <h4 class="modal-title mb-1">{{ isEdit ? 'Cập nhật đợt giao chỉ tiêu' : 'Tạo đợt giao chỉ tiêu mới' }}</h4>
                                    <p class="text-muted mb-0">Nhập thông tin đợt giao chỉ tiêu</p>
                                </div>
                                <button type="button" class="btn-close" @click="closeModal"></button>
                            </div>

                            <div class="modal-body pt-3">
                                <div class="row g-3">
                                    <div class="col-12 col-md-6">
                                        <label class="form-label">Mã đợt giao <span class="text-danger">*</span></label>
                                        <input v-model="form.maDotGiao" :disabled="isEdit" type="text" class="form-control" placeholder="Nhập mã đợt giao" />
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">Tên đợt giao <span class="text-danger">*</span></label>
                                        <input v-model="form.tenDotGiao" type="text" class="form-control" placeholder="Nhập tên đợt giao" />
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Năm ap dung <span class="text-danger">*</span></label>
                                        <input v-model="form.namApDung" type="number" class="form-control" min="2000" max="2100" />
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Ngày bắt đầu <span class="text-danger">*</span></label>
                                        <input v-model="form.ngayBatDau" type="datetime-local" class="form-control" />
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Ngày kết thúc</label>
                                        <input v-model="form.ngayKetThuc" type="datetime-local" class="form-control" />
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Nguồn đợt giao <span class="text-danger">*</span></label>
                                        <select v-model="form.nguonDotGiao" class="form-select">
                                            <option value="BO_GIAO">Bo giao</option>
                                            <option value="THANH_PHO_GIAO">Thanh pho giao</option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Cap giao <span class="text-danger">*</span></label>
                                        <select v-model="form.capGiao" class="form-select">
                                            <option value="BO">Bo</option>
                                            <option value="THANH_PHO">Thanh pho</option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Trạng thái <span class="text-danger">*</span></label>
                                        <select v-model="form.trangThai" class="form-select">
                                            <option value="DRAFT">Nhap</option>
                                            <option value="PUBLISHED">Da phat hanh</option>
                                            <option value="CLOSED">Da dong</option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">ID đơn vị giao <span class="text-danger">*</span></label>
                                        <input v-model="form.donViGiaoId" type="number" min="1" class="form-control" placeholder="Nhập ID đơn vị giao" />
                                    </div>

                                    <div class="col-12">
                                        <label class="form-label">Ghi chú</label>
                                        <textarea v-model="form.ghiChu" rows="4" class="form-control" placeholder="Nhập ghi chú"></textarea>
                                    </div>
                                </div>
                            </div>

                            <div class="modal-footer border-0 pt-0">
                                <button class="btn btn-light" @click="closeModal">Hủy</button>
                                <button class="btn btn-primary" :disabled="saving" @click="handleSubmit">
                                    <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                                    {{ saving ? 'Đang lưu...' : isEdit ? 'Cập nhật' : 'Lưu đợt giao' }}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>

                <div v-if="showModal" class="modal-backdrop fade show"></div>
            </div>
        </div>
    </BaseLayout>
</template>

<script setup>
import { onMounted, reactive, ref, watch } from 'vue'
import BaseLayout from '../BaseLayout.vue'
import ColumnVisibilityTools from '../shared/ColumnVisibilityTools.vue'
import { apiRequest } from '../../services/api.js'

const loading = ref(false)
const saving = ref(false)
const showModal = ref(false)
const isEdit = ref(false)
const editingId = ref(null)
const items = ref([])

const filters = reactive({
    keyword: '',
    namApDung: '',
    nguonDotGiao: '',
    capGiao: '',
    trangThai: ''
})

const createDefaultForm = () => ({
    maDotGiao: '',
    tenDotGiao: '',
    namApDung: new Date().getFullYear(),
    nguonDotGiao: 'BO_GIAO',
    capGiao: 'BO',
    donViGiaoId: '',
    ngayBatDau: '',
    ngayKetThuc: '',
    trangThai: 'DRAFT',
    ghiChu: ''
})

const form = reactive(createDefaultForm())

const resetForm = () => {
    Object.assign(form, createDefaultForm())
}

watch(
    () => form.nguonDotGiao,
    (value) => {
        if (value === 'BO_GIAO') {
            form.capGiao = 'BO'
        } else if (value === 'THANH_PHO_GIAO') {
            form.capGiao = 'THANH_PHO'
        }
    }
)

const getId = (item) => Number(item?.id ?? item?.Id ?? 0)

const normalizeList = (response) => {
    if (Array.isArray(response)) return response
    if (Array.isArray(response?.data)) return response.data
    if (Array.isArray(response?.data?.data)) return response.data.data
    if (Array.isArray(response?.data?.items)) return response.data.items
    if (Array.isArray(response?.items)) return response.items
    return []
}

const normalizeItem = (item) => ({
    id: item.id ?? item.Id,
    maDotGiao: item.maDotGiao ?? item.MaDotGiao ?? '',
    tenDotGiao: item.tenDotGiao ?? item.TenDotGiao ?? '',
    namApDung: item.namApDung ?? item.NămApDung ?? '',
    nguonDotGiao: item.nguonDotGiao ?? item.NguồnDotGiao ?? '',
    capGiao: item.capGiao ?? item.CapGiao ?? '',
    donViGiaoId: item.donViGiaoId ?? item.DonViGiaoId ?? '',
    ngayBatDau: item.ngayBatDau ?? item.NgayBatDau ?? item.ngayGiao ?? item.NgayGiao ?? null,
    ngayKetThuc: item.ngayKetThuc ?? item.NgayKetThuc ?? null,
    trangThai: item.trangThai ?? item.TrangThai ?? 'DRAFT',
    ghiChu: item.ghiChu ?? item.GhiChu ?? ''
})

const buildPayload = () => ({
    maDotGiao: form.maDotGiao.trim(),
    tenDotGiao: form.tenDotGiao.trim(),
    namApDung: Number(form.namApDung),
    nguonDotGiao: form.nguonDotGiao,
    capGiao: form.capGiao,
    donViGiaoId: Number(form.donViGiaoId),
    ngayBatDau: form.ngayBatDau ? new Date(form.ngayBatDau).toISOString() : null,
    ngayKetThuc: form.ngayKetThuc ? new Date(form.ngayKetThuc).toISOString() : null,
    trangThai: form.trangThai,
    ghiChu: form.ghiChu?.trim() || null
})

const fetchDotGiaoChiTieu = async () => {
    try {
        loading.value = true
        const data = await apiRequest('/dot-giao-chi-tieu')
        let list = normalizeList(data).map(normalizeItem)
        const keyword = filters.keyword.trim().toLowerCase()

        list = list.filter((item) => {
            const matchKeyword = !keyword || [item.maDotGiao, item.tenDotGiao, item.ghiChu].join(' ').toLowerCase().includes(keyword)
            const matchNăm = !filters.namApDung || Number(item.namApDung) === Number(filters.namApDung)
            const matchNguồn = !filters.nguonDotGiao || item.nguonDotGiao === filters.nguonDotGiao
            const matchCap = !filters.capGiao || item.capGiao === filters.capGiao
            const matchTrangThai = !filters.trangThai || item.trangThai === filters.trangThai
            return matchKeyword && matchNăm && matchNguồn && matchCap && matchTrangThai
        })

        items.value = list
    } catch (error) {
        console.error(error)
        alert(error.message || 'Không tải được danh sách đợt giao chỉ tiêu.')
        items.value = []
    } finally {
        loading.value = false
    }
}

const openCreateModal = () => {
    isEdit.value = false
    editingId.value = null
    resetForm()
    showModal.value = true
}

const toDateTimeLocal = (value) => {
    if (!value) return ''
    const date = new Date(value)
    if (Number.isNaN(date.getTime())) return ''
    const offset = date.getTimezoneOffset()
    return new Date(date.getTime() - offset * 60000).toISOString().slice(0, 16)
}

const openEditModal = (item) => {
    isEdit.value = true
    editingId.value = getId(item)
    Object.assign(form, {
        maDotGiao: item.maDotGiao,
        tenDotGiao: item.tenDotGiao,
        namApDung: item.namApDung,
        nguonDotGiao: item.nguonDotGiao || 'BO_GIAO',
        capGiao: item.capGiao || 'BO',
        donViGiaoId: item.donViGiaoId,
        ngayBatDau: toDateTimeLocal(item.ngayBatDau),
        ngayKetThuc: toDateTimeLocal(item.ngayKetThuc),
        trangThai: item.trangThai || 'DRAFT',
        ghiChu: item.ghiChu || ''
    })
    showModal.value = true
}

const closeModal = () => {
    showModal.value = false
    resetForm()
}

const validateForm = () => {
    if (!form.maDotGiao && !isEdit.value) {
        alert('Vui lòng nhập mã đợt giao.')
        return false
    }

    if (!form.tenDotGiao?.trim()) {
        alert('Vui lòng nhập tên đợt giao.')
        return false
    }

    const namApDung = Number(form.namApDung)
    if (!namApDung || namApDung < 2000 || namApDung > 2100) {
        alert('Năm ap dung không hợp lệ.')
        return false
    }

    if (!['BO_GIAO', 'THANH_PHO_GIAO'].includes(form.nguonDotGiao)) {
        alert('Nguồn đợt giao không hợp lệ.')
        return false
    }

    if (!['BO', 'THANH_PHO'].includes(form.capGiao)) {
        alert('Cap giao không hợp lệ.')
        return false
    }

    if (form.nguonDotGiao === 'BO_GIAO' && form.capGiao !== 'BO') {
        alert('Nguồn Bo giao phai di kem cap giao la BO.')
        return false
    }

    if (form.nguonDotGiao === 'THANH_PHO_GIAO' && form.capGiao !== 'THANH_PHO') {
        alert('Nguồn Thanh pho giao phai di kem cap giao la THANH_PHO.')
        return false
    }

    const donViGiaoId = Number(form.donViGiaoId)
    if (!donViGiaoId || donViGiaoId <= 0) {
        alert('Vui lòng nhập ID đơn vị giao hợp lệ.')
        return false
    }

    if (!form.ngayBatDau) {
        alert('Vui lòng chọn ngày bắt đầu.')
        return false
    }

    if (form.ngayKetThuc && new Date(form.ngayKetThuc) < new Date(form.ngayBatDau)) {
        alert('Ngày kết thúc phai lon hon hoac bang ngay bat dau.')
        return false
    }

    if (!['DRAFT', 'PUBLISHED', 'CLOSED'].includes(form.trangThai)) {
        alert('Trạng thái không hợp lệ.')
        return false
    }

    return true
}

const handleSubmit = async () => {
    if (!validateForm()) return

    try {
        saving.value = true
        const payload = buildPayload()

        if (isEdit.value && editingId.value) {
            await apiRequest(`/dot-giao-chi-tieu/${editingId.value}`, 'PUT', payload)
        } else {
            await apiRequest('/dot-giao-chi-tieu', 'POST', payload)
        }

        closeModal()
        await fetchDotGiaoChiTieu()
    } catch (error) {
        console.error(error)
        alert(error.message || 'Lưu đợt giao chi tieu that bai.')
    } finally {
        saving.value = false
    }
}

const handleDelete = async (item) => {
    const ok = window.confirm(`Bạn có chắc muốn xóa đợt giao "${item.tenDotGiao}" không?`)
    if (!ok) return

    try {
        await apiRequest(`/dot-giao-chi-tieu/${item.id}`, 'DELETE')
        await fetchDotGiaoChiTieu()
    } catch (error) {
        console.error(error)
        alert(error.message || 'Xóa đợt giao chỉ tiêu thất bại.')
    }
}

const resetFilters = async () => {
    filters.keyword = ''
    filters.namApDung = ''
    filters.nguonDotGiao = ''
    filters.capGiao = ''
    filters.trangThai = ''
    await fetchDotGiaoChiTieu()
}

const mapNguồnDotGiao = (value) => ({ BO_GIAO: 'Bo giao', THANH_PHO_GIAO: 'Thanh pho giao' }[value] || value || '-')
const mapCapGiao = (value) => ({ BO: 'Bo', THANH_PHO: 'Thanh pho' }[value] || value || '-')
const mapTrangThai = (value) => ({ DRAFT: 'Nhap', PUBLISHED: 'Da phat hanh', CLOSED: 'Da dong' }[value] || value || '-')
const getStatusClass = (value) => ({ DRAFT: 'status-draft', PUBLISHED: 'status-published', CLOSED: 'status-closed' }[value] || 'status-default')

const formatDateTime = (value) => {
    if (!value) return '-'
    const date = new Date(value)
    if (Number.isNaN(date.getTime())) return '-'
    return date.toLocaleString('vi-VN')
}

onMounted(() => {
    fetchDotGiaoChiTieu()
})
</script>

<style scoped>
.page-wrap {
    min-height: 100vh;
    background: linear-gradient(180deg, #f8fbff 0%, #eef5fb 100%);
}

.wave-title {
    font-weight: 700;
    letter-spacing: 0.08em;
    font-size: 0.8rem;
    color: #0d6efd;
    margin-bottom: 6px;
    text-transform: uppercase;
}

.gov-banner {
    display: flex;
    align-items: center;
    gap: 16px;
    padding: 20px 24px;
    border-radius: 20px;
    background: linear-gradient(135deg, #ffffff 0%, #f4f9ff 100%);
    box-shadow: 0 10px 30px rgba(13, 110, 253, 0.08);
    border: 1px solid rgba(13, 110, 253, 0.08);
}

.gov-emblem {
    width: 64px;
    height: 64px;
    border-radius: 18px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: linear-gradient(135deg, #0d6efd, #4ea1ff);
    color: #fff;
    font-size: 1.6rem;
    flex-shrink: 0;
}

.gov-text {
    flex: 1;
}

.gov-title {
    font-size: 1.3rem;
    font-weight: 800;
    color: #1f2d3d;
    line-height: 1.3;
}

.custom-card {
    border: 0;
    border-radius: 20px;
    box-shadow: 0 8px 24px rgba(15, 23, 42, 0.06);
    overflow: hidden;
}

.btn-action {
    border-radius: 12px;
    padding: 10px 18px;
    font-weight: 600;
}

.empty-state {
    min-height: 240px;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
    color: #6b7280;
}

:deep(.table) {
    margin-bottom: 0;
    border-collapse: collapse;
}

:deep(.table thead th) {
    background: #f8fafc;
    color: #334155;
    font-weight: 700;
    white-space: nowrap;
    border-bottom: 2px solid #dee2e6;
}

:deep(.table th),
:deep(.table td) {
    padding: 14px 16px;
    vertical-align: middle;
    border-right: 1px solid #eee;
}

:deep(.table th:last-child),
:deep(.table td:last-child) {
    border-right: none;
}

.status-badge {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    min-width: 110px;
    padding: 6px 12px;
    border-radius: 999px;
    font-size: 0.82rem;
    font-weight: 700;
}

.status-draft {
    background: #fff3cd;
    color: #856404;
}

.status-published {
    background: #d1e7dd;
    color: #0f5132;
}

.status-closed {
    background: #f8d7da;
    color: #842029;
}

.status-default {
    background: #e9ecef;
    color: #495057;
}

.custom-modal {
    background: rgba(137, 210, 239, 0.5);
}

@media (max-width: 768px) {
    .gov-banner {
        padding: 16px;
        align-items: flex-start;
    }

    .gov-title {
        font-size: 1.05rem;
    }
}
</style>




