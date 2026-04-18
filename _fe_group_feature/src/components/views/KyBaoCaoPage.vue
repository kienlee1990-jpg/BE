<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-calendar3"></i>
                    </div>
                    <div class="gov-text">
                        <div class="wave-title">HỆ THỐNG THEO DÕI CHỈ TIÊU CÔNG TÁC</div>
                        <div class="gov-title">QUẢN LÝ KỲ BÁO CÁO KPI</div>
                    </div>
                </div>

                <div class="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
                    <div class="gov-banner">
                        <img src="https://upload.wikimedia.org/wikipedia/commons/a/a3/Emblem_of_Vietnam.svg"
                            class="gov-emblem" />
                    </div>

                    <button class="btn btn-primary btn-action" @click="openCreateModal">
                        <i class="bi bi-plus-circle me-2"></i>
                        Tạo kỳ báo cáo
                    </button>
                </div>

                <div class="card custom-card mb-4">
                    <div class="card-header bg-white border-0 pb-0">
                        <h5 class="mb-1">Bộ lọc tìm kiếm</h5>
                        <small class="text-muted">Lọc theo năm, loại kỳ và trạng thái</small>
                    </div>

                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-12 col-md-4 col-xl-3">
                                <label class="form-label">Loại kỳ</label>
                                <select v-model="filters.loaiKy" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option value="THANG">Tháng</option>
                                    <option value="QUY">Quý</option>
                                    <option value="6THANG">6 tháng</option>
                                    <option value="NAM">Năm</option>
                                </select>
                            </div>

                            <div class="col-12 col-md-4 col-xl-3">
                                <label class="form-label">Năm</label>
                                <input v-model="filters.nam" type="number" class="form-control"
                                    placeholder="VD: 2026" />
                            </div>

                            <div class="col-12 col-md-4 col-xl-3">
                                <label class="form-label">Trạng thái</label>
                                <select v-model="filters.trangThai" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option value="CHUA_MO">Chưa mở</option>
                                    <option value="DANG_MO">Đang mở</option>
                                    <option value="DA_DONG">Đã đóng</option>
                                </select>
                            </div>
                        </div>

                        <div class="d-flex flex-wrap gap-2 mt-4">
                            <button class="btn btn-primary" @click="fetchItems">
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
                            <h5 class="mb-1">Danh sách kỳ báo cáo</h5>
                            <small class="text-muted">Theo dõi các kỳ báo cáo KPI trong hệ thống</small>
                        </div>
                        <span class="badge text-bg-light border">Tổng: {{ filteredItems.length }}</span>
                    </div>

                    <div class="card-body p-0">
                        <div v-if="loading" class="empty-state">
                            <div class="spinner-border text-primary mb-3" role="status"></div>
                            <div>Đang tải dữ liệu...</div>
                        </div>

                        <div v-else-if="!filteredItems.length" class="empty-state">
                            <i class="bi bi-inbox fs-1 text-muted mb-2"></i>
                            <div>Chưa có dữ liệu</div>
                        </div>

                        <div v-else class="table-responsive">
                            <ColumnVisibilityTools table-id="KyBaoCaoPage-table" />
                            <table id="KyBaoCaoPage-table" class="table table-hover align-middle mb-0 custom-table managed-table">
                                <thead>
                                    <tr>
                                        <th>Tên kỳ</th>
                                        <th>Loại kỳ</th>
                                        <th>Năm</th>
                                        <th>Số kỳ</th>
                                        <th>Trạng thái</th>
                                        <th>Ngày đầu kỳ</th>
                                        <th>Ngày cuối kỳ</th>
                                        <th>Ghi chú</th>
                                        <th class="text-center" style="width: 180px">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="item in filteredItems" :key="item.id">
                                        <td>{{ item.tenKy }}</td>
                                        <td>{{ mapLoaiKy(item.loaiKy) }}</td>
                                        <td>{{ item.nam }}</td>
                                        <td>{{ item.soKy ?? '-' }}</td>
                                        <td>
                                            <span class="badge" :class="getTrangThaiBadgeClass(item.trangThai)">
                                                {{ mapTrangThai(item.trangThai) }}
                                            </span>
                                        </td>
                                        <td>{{ formatDate(item.ngayDauKy) }}</td>
                                        <td>{{ formatDate(item.ngayCuoiKy) }}</td>
                                        <td class="text-truncate" style="max-width: 220px">
                                            {{ item.ghiChu || '-' }}
                                        </td>
                                        <td class="text-center">
                                            <div class="d-flex justify-content-center gap-2">
                                                <button class="btn btn-sm btn-outline-primary"
                                                    @click="openEditModal(item)">
                                                    <i class="bi bi-pencil-square me-1"></i>Sửa
                                                </button>
                                                <button class="btn btn-sm btn-outline-danger"
                                                    @click="handleDelete(item)">
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

                <div v-if="showModal" class="modal fade show d-block custom-modal" tabindex="-1"
                    @click.self="closeModal">
                    <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
                        <div class="modal-content border-0 shadow-lg rounded-4">
                            <div class="modal-header border-0 pb-0">
                                <div>
                                    <h4 class="modal-title mb-1">
                                        {{ isEdit ? 'Cập nhật kỳ báo cáo KPI' : 'Tạo kỳ báo cáo KPI mới' }}
                                    </h4>
                                    <p class="text-muted mb-0">Nhập thông tin kỳ báo cáo</p>
                                </div>
                                <button type="button" class="btn-close" @click="closeModal"></button>
                            </div>

                            <div class="modal-body pt-3">
                                <div class="row g-3">
                                    <div class="col-12 col-md-4">
                                        <label class="form-label">
                                            Mã kỳ <span class="text-danger">*</span>
                                        </label>
                                        <input v-model="form.maKy" :disabled="isEdit" type="text" class="form-control"
                                            placeholder="VD: QUY_1_2026" />
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">
                                            Tên kỳ <span class="text-danger">*</span>
                                        </label>
                                        <input v-model="form.tenKy" type="text" class="form-control"
                                            placeholder="VD: Quý 1/2026" />
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">
                                            Loại kỳ <span class="text-danger">*</span>
                                        </label>
                                        <select v-model="form.loaiKy" class="form-select">
                                            <option value="THANG">Tháng</option>
                                            <option value="QUY">Quý</option>
                                            <option value="6THANG">6 tháng</option>
                                            <option value="NAM">Năm</option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">
                                            Năm <span class="text-danger">*</span>
                                        </label>
                                        <input v-model="form.nam" type="number" class="form-control" />
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">Số kỳ</label>
                                        <input v-model="form.soKy" type="number" class="form-control"
                                            :readonly="form.loaiKy === 'NAM'" :disabled="form.loaiKy === 'NAM'"
                                            placeholder="VD: 1, 2, 3..." />
                                        <small v-if="form.loaiKy === 'NAM'" class="text-muted">
                                            Loại kỳ năm sẽ tự động đặt số kỳ = 1.
                                        </small>
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">
                                            Trạng thái <span class="text-danger">*</span>
                                        </label>
                                        <select v-model="form.trangThai" class="form-select">
                                            <option value="CHUA_MO">Chưa mở</option>
                                            <option value="DANG_MO">Đang mở</option>
                                            <option value="DA_DONG">Đã đóng</option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">
                                            Người cập nhật <span class="text-danger">*</span>
                                        </label>
                                        <input v-model="form.updatedBy" type="text" class="form-control"
                                            placeholder="VD: kienlee1990" />
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">
                                            Ngày đầu kỳ <span class="text-danger">*</span>
                                        </label>
                                        <input v-model="form.ngayDauKy" type="date" class="form-control" />
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">
                                            Ngày cuối kỳ <span class="text-danger">*</span>
                                        </label>
                                        <input v-model="form.ngayCuoiKy" type="date" class="form-control" />
                                    </div>

                                    <div class="col-12">
                                        <label class="form-label">Ghi chú</label>
                                        <textarea v-model="form.ghiChu" rows="3" class="form-control"></textarea>
                                    </div>
                                </div>
                            </div>

                            <div class="modal-footer border-0 pt-0">
                                <button class="btn btn-light" @click="closeModal">Hủy</button>
                                <button class="btn btn-primary" :disabled="saving" @click="handleSubmit">
                                    <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                                    {{ saving ? 'Đang lưu...' : isEdit ? 'Cập nhật' : 'Lưu kỳ báo cáo' }}
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
    import { computed, onMounted, reactive, ref, watch } from 'vue'
    import BaseLayout from '../BaseLayout.vue'
import ColumnVisibilityTools from '../shared/ColumnVisibilityTools.vue'
    import { apiRequest } from '../../services/api.js'

    const API_CONTROLLER = '/KyBaoCaoKPI'

    const loading = ref(false)
    const saving = ref(false)
    const showModal = ref(false)
    const isEdit = ref(false)
    const editingId = ref(null)
    const items = ref([])

    const filters = reactive({
        loaiKy: '',
        nam: '',
        trangThai: ''
    })

    const createDefaultForm = () => ({
        maKy: '',
        tenKy: '',
        loaiKy: 'QUY',
        nam: new Date().getFullYear(),
        soKy: '',
        trangThai: 'CHUA_MO',
        ngayDauKy: '',
        ngayCuoiKy: '',
        ghiChu: '',
        updatedBy:
            localStorage.getItem('username') ||
            localStorage.getItem('userName') ||
            localStorage.getItem('fullName') ||
            'admin'
    })

    const form = reactive(createDefaultForm())

    const resetForm = () => {
        Object.assign(form, createDefaultForm())
    }

    const normalizeList = (response) => {
        if (Array.isArray(response)) return response
        if (Array.isArray(response?.data)) return response.data
        if (Array.isArray(response?.data?.data)) return response.data.data
        return []
    }

    const toIsoDateStart = (dateValue) => {
        if (!dateValue) return null
        return new Date(`${dateValue}T00:00:00`).toISOString()
    }

    const toIsoDateEnd = (dateValue) => {
        if (!dateValue) return null
        return new Date(`${dateValue}T23:59:59`).toISOString()
    }

    const buildPayload = () => ({
        maKy: form.maKy?.trim(),
        tenKy: form.tenKy?.trim(),
        loaiKy: form.loaiKy,
        nam: Number(form.nam),
        soKy: form.loaiKy === 'NAM'
            ? 1
            : (form.soKy !== '' && form.soKy !== null ? Number(form.soKy) : 0),
        trangThai: form.trangThai,
        ngayDauKy: toIsoDateStart(form.ngayDauKy),
        ngayCuoiKy: toIsoDateEnd(form.ngayCuoiKy),
        ghiChu: form.ghiChu?.trim() || '',
        updatedBy: form.updatedBy?.trim()
    })

    const filteredItems = computed(() => {
        return items.value.filter((item) => {
            const matchLoaiKy = !filters.loaiKy || item.loaiKy === filters.loaiKy
            const matchNam = !filters.nam || Number(item.nam) === Number(filters.nam)
            const matchTrangThai = !filters.trangThai || item.trangThai === filters.trangThai
            return matchLoaiKy && matchNam && matchTrangThai
        })
    })

    const fetchItems = async () => {
        try {
            loading.value = true
            const data = await apiRequest(API_CONTROLLER)
            items.value = normalizeList(data)
        } catch (error) {
            console.error('Fetch error:', error)
            alert(error.message || 'Không tải được danh sách kỳ báo cáo KPI.')
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

    const openEditModal = (item) => {
        isEdit.value = true
        editingId.value = item.id

        Object.assign(form, {
            maKy: item.maKy || '',
            tenKy: item.tenKy || '',
            loaiKy: item.loaiKy || 'QUY',
            nam: item.nam || new Date().getFullYear(),
            soKy: item.loaiKy === 'NAM' ? 1 : (item.soKy ?? ''),
            trangThai: item.trangThai || 'CHUA_MO',
            ngayDauKy: toInputDate(item.ngayDauKy),
            ngayCuoiKy: toInputDate(item.ngayCuoiKy),
            ghiChu: item.ghiChu || '',
            updatedBy:
                localStorage.getItem('username') ||
                localStorage.getItem('userName') ||
                localStorage.getItem('fullName') ||
                'admin'
        })

        showModal.value = true
    }

    const closeModal = () => {
        showModal.value = false
        resetForm()
    }

    const validateForm = () => {
        if (!form.maKy && !isEdit.value) {
            alert('Vui lòng nhập mã kỳ.')
            return false
        }

        if (!form.tenKy?.trim()) {
            alert('Vui lòng nhập tên kỳ.')
            return false
        }

        if (!form.loaiKy) {
            alert('Vui lòng chọn loại kỳ.')
            return false
        }

        if (!form.nam) {
            alert('Vui lòng nhập năm.')
            return false
        }

        if (form.loaiKy !== 'NAM' && (form.soKy === '' || form.soKy === null)) {
            alert('Vui lòng nhập số kỳ.')
            return false
        }

        if (!form.trangThai) {
            alert('Vui lòng chọn trạng thái.')
            return false
        }

        if (!form.ngayDauKy || !form.ngayCuoiKy) {
            alert('Vui lòng nhập ngày đầu kỳ và ngày cuối kỳ.')
            return false
        }

        if (!form.updatedBy?.trim()) {
            alert('Vui lòng nhập người cập nhật.')
            return false
        }

        if (new Date(form.ngayDauKy) > new Date(form.ngayCuoiKy)) {
            alert('Ngày đầu kỳ không được lớn hơn ngày cuối kỳ.')
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
                await apiRequest(`${API_CONTROLLER}/${editingId.value}`, 'PUT', payload)
            } else {
                await apiRequest(API_CONTROLLER, 'POST', payload)
            }

            closeModal()
            await fetchItems()
        } catch (error) {
            console.error('Submit error:', error)
            alert(error.message || 'Lưu kỳ báo cáo KPI thất bại.')
        } finally {
            saving.value = false
        }
    }

    const handleDelete = async (item) => {
        const ok = window.confirm(`Bạn có chắc muốn xóa kỳ báo cáo "${item.tenKy}" không?`)
        if (!ok) return

        try {
            await apiRequest(`${API_CONTROLLER}/${item.id}`, 'DELETE')
            await fetchItems()
        } catch (error) {
            console.error('Delete error:', error)
            alert(error.message || 'Xóa kỳ báo cáo KPI thất bại.')
        }
    }

    const resetFilters = async () => {
        filters.loaiKy = ''
        filters.nam = ''
        filters.trangThai = ''
        await fetchItems()
    }

    const mapLoaiKy = (value) => {
        const map = {
            THANG: 'Tháng',
            QUY: 'Quý',
            '6THANG': '6 tháng',
            NAM: 'Năm'
        }
        return map[value] || value || '-'
    }

    const mapTrangThai = (value) => {
        const map = {
            CHUA_MO: 'Chưa mở',
            DANG_MO: 'Đang mở',
            DA_DONG: 'Đã đóng'
        }
        return map[value] || value || '-'
    }

    const getTrangThaiBadgeClass = (value) => {
        const map = {
            CHUA_MO: 'text-bg-secondary',
            DANG_MO: 'text-bg-success',
            DA_DONG: 'text-bg-dark'
        }
        return map[value] || 'text-bg-light'
    }

    const formatDate = (value) => {
        if (!value) return '-'
        const date = new Date(value)
        if (Number.isNaN(date.getTime())) return '-'
        return date.toLocaleDateString('vi-VN')
    }

    const toInputDate = (value) => {
        if (!value) return ''
        const date = new Date(value)
        if (Number.isNaN(date.getTime())) return ''
        return date.toISOString().split('T')[0]
    }

    watch(
        () => form.loaiKy,
        (newValue) => {
            if (newValue === 'NAM') {
                form.soKy = 1
            }
        },
        { immediate: true }
    )

    onMounted(() => {
        fetchItems()
    })
</script>

<style scoped>
    .page-wrap {
        min-height: 100vh;
        background: linear-gradient(180deg, #f8fbff 0%, #eef5fb 100%);
    }

    .page-title {
        font-size: 1.75rem;
        font-weight: 700;
        color: #1f2d3d;
    }

    .page-subtitle {
        color: #6b7280;
        font-size: 0.95rem;
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
        margin-bottom: 18px;
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

    .gov-sub {
        color: #6b7280;
        margin-top: 4px;
        font-size: 0.95rem;
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

    .form-control,
    .form-select {
        border-radius: 12px;
        min-height: 44px;
        border-color: #dbe2ea;
        box-shadow: none;
    }

    .form-control:focus,
    .form-select:focus {
        border-color: #89d2ef;
        box-shadow: 0 0 0 0.2rem rgba(137, 210, 239, 0.2);
    }

    .form-label {
        font-weight: 600;
        color: #334155;
        margin-bottom: 0.45rem;
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

    :deep(.table tbody tr) {
        border-bottom: 1px solid #f1f1f1;
    }

    :deep(.table tbody tr:hover) {
        background: rgba(0, 0, 0, 0.03);
    }

    .empty-state {
        min-height: 260px;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;
        color: #64748b;
        font-weight: 500;
    }

    .custom-modal {
        background: rgba(137, 210, 239, 0.5);
    }

    :deep(.modal-content) {
        animation: fadeInUp 0.3s ease;
        border-radius: 24px;
    }

    @keyframes fadeInUp {
        from {
            opacity: 0;
            transform: translate3d(0, 20px, 0);
        }

        to {
            opacity: 1;
            transform: translate3d(0, 0, 0);
        }
    }

    textarea.form-control {
        min-height: 100px;
        resize: vertical;
    }

    @media (max-width: 768px) {
        .page-title {
            font-size: 1.4rem;
        }

        .gov-banner {
            padding: 16px;
            align-items: flex-start;
        }

        .gov-title {
            font-size: 1.05rem;
        }
    }
</style>



