<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-building"></i>
                    </div>
                    <div class="gov-text">
                        <div class="wave-title">HỆ THỐNG THEO DÕI CHỈ TIÊU CÔNG TÁC</div>
                        <div class="gov-title">QUẢN LÝ ĐƠN VỊ</div>
                        <div class="gov-sub"></div>
                    </div>
                </div>

                <div class="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
                    <div class="gov-banner">
                        <img src="https://upload.wikimedia.org/wikipedia/commons/a/a3/Emblem_of_Vietnam.svg"
                            class="gov-emblem" />
                    </div>

                    <button class="btn btn-primary btn-action" @click="openCreateModal">
                        <i class="bi bi-plus-circle me-2"></i>
                        Tạo đơn vị
                    </button>
                </div>

                <div class="card custom-card mb-4">
                    <div class="card-header bg-white border-0 pb-0">
                        <h5 class="mb-1">Bộ lọc tìm kiếm</h5>
                        <small class="text-muted">Lọc dữ liệu tại frontend theo API hiện tại</small>
                    </div>

                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-12 col-md-6 col-xl-4">
                                <label class="form-label">Từ khóa</label>
                                <input v-model="filters.keyword" type="text" class="form-control"
                                    placeholder="Mã hoặc tên đơn vị" />
                            </div>

                            <div class="col-12 col-md-6 col-xl-4">
                                <label class="form-label">Đơn vị quản lý trực tiếp</label>
                                <select v-model="filters.donViChaId" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option v-for="parent in parentOptions" :key="parent.id" :value="String(parent.id)">
                                        {{ parent.tenDonVi }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-4">
                                <label class="form-label">Loại đơn vị</label>
                                <select v-model="filters.loaiDonVi" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option value="THANH_PHO">Thành phố</option>
                                    <option value="PHONG">Cấp phòng</option>
                                    <option value="XA">Cấp xã/phường</option>
                                </select>
                            </div>
                        </div>

                        <div class="d-flex flex-wrap gap-2 mt-4">
                            <button class="btn btn-primary" @click="fetchDonVi">
                                <i class="bi bi-arrow-repeat me-1"></i>
                                Tải lại dữ liệu
                            </button>
                            <button class="btn btn-outline-secondary" @click="resetFilters">
                                <i class="bi bi-arrow-clockwise me-1"></i>
                                Đặt lại bộ lọc
                            </button>
                        </div>
                    </div>
                </div>

                <div class="card custom-card">
                    <div class="card-header bg-white d-flex justify-content-between align-items-center border-0">
                        <div>
                            <h5 class="mb-1">Danh sách đơn vị</h5>
                            <small class="text-muted">Theo dõi và quản lý dữ liệu đơn vị</small>
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
                            <table class="table table-hover align-middle mb-0 custom-table">
                                <thead>
                                    <tr>
                                        <th>Tên đơn vị</th>
                                        <th>Loại đơn vị</th>
                                        <th>Đơn vị quản lý trực tiếp</th>
                                        <th>Người đại diện</th>
                                        <th class="text-center" style="width: 180px">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="item in filteredItems" :key="item.id">
                                        <td>{{ item.tenDonVi }}</td>
                                        <td>{{ mapLoaiDonVi(item.loaiDonVi) }}</td>
                                        <td>{{ getParentName(item.donViChaId) }}</td>
                                        <td>{{ item.nguoiDaiDien || '-' }}</td>
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
                                        {{ isEdit ? 'Cập nhật đơn vị' : 'Tạo đơn vị mới' }}
                                    </h4>
                                    <p class="text-muted mb-0">Nhập thông tin đơn vị</p>
                                </div>
                                <button type="button" class="btn-close" @click="closeModal"></button>
                            </div>

                            <div class="modal-body pt-3">
                                <div class="row g-3">
                                    <div class="col-12 col-md-6">
                                        <label class="form-label">
                                            Mã đơn vị <span class="text-danger">*</span>
                                        </label>
                                        <input v-model="form.maDonVi" :disabled="isEdit" type="text"
                                            class="form-control" />
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">
                                            Tên đơn vị <span class="text-danger">*</span>
                                        </label>
                                        <input v-model="form.tenDonVi" type="text" class="form-control" />
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">
                                            Loại đơn vị <span class="text-danger">*</span>
                                        </label>
                                        <select v-model="form.loaiDonVi" class="form-select">
                                            <option value="THANH_PHO">Thành phố</option>
                                            <option value="PHONG">Cấp phòng</option>
                                            <option value="XA">Cấp xã/phường</option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">Đơn vị quản lý trực tiếp</label>
                                        <select v-model="form.donViChaId" class="form-select">
                                            <option :value="null">Không có</option>
                                            <option v-for="parent in availableParentOptions" :key="parent.id"
                                                :value="parent.id">
                                                {{ parent.tenDonVi }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">Người đại diện</label>
                                        <input v-model="form.nguoiDaiDien" type="text" class="form-control" />
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">Số điện thoại</label>
                                        <input v-model="form.soDienThoai" type="text" class="form-control" />
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">Email</label>
                                        <input v-model="form.email" type="email" class="form-control" />
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">Địa chỉ</label>
                                        <input v-model="form.diaChi" type="text" class="form-control" />
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
                                    {{ saving ? 'Đang lưu...' : isEdit ? 'Cập nhật' : 'Lưu đơn vị' }}
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
    import { computed, onMounted, reactive, ref } from 'vue'
    import BaseLayout from '../BaseLayout.vue'
    import { apiRequest } from '../../services/api.js'

    const loading = ref(false)
    const saving = ref(false)
    const showModal = ref(false)
    const isEdit = ref(false)
    const editingId = ref(null)
    const items = ref([])

    const filters = reactive({
        keyword: '',
        loaiDonVi: '',
        donViChaId: ''
    })

    const createDefaultForm = () => ({
        maDonVi: '',
        tenDonVi: '',
        loaiDonVi: 'THANH_PHO',
        donViChaId: null,
        diaChi: '',
        nguoiDaiDien: '',
        soDienThoai: '',
        email: '',
        ghiChu: ''
    })

    const form = reactive(createDefaultForm())

    const resetForm = () => {
        Object.assign(form, createDefaultForm())
    }

    const parentOptions = computed(() => items.value)

    const availableParentOptions = computed(() => {
        return items.value.filter(item => item.id !== editingId.value)
    })

    const filteredItems = computed(() => {
        return items.value.filter(item => {
            const keyword = filters.keyword.trim().toLowerCase()

            const matchKeyword =
                !keyword ||
                item.maDonVi?.toLowerCase().includes(keyword) ||
                item.tenDonVi?.toLowerCase().includes(keyword)

            const matchLoaiDonVi =
                !filters.loaiDonVi || item.loaiDonVi === filters.loaiDonVi

            const matchDonViChaId =
                !filters.donViChaId ||
                String(item.donViChaId ?? '') === String(filters.donViChaId)

            return matchKeyword && matchLoaiDonVi && matchDonViChaId
        })
    })

    const buildPayload = () => ({
        maDonVi: form.maDonVi?.trim(),
        tenDonVi: form.tenDonVi?.trim(),
        loaiDonVi: form.loaiDonVi,
        donViChaId: form.donViChaId || null,
        diaChi: form.diaChi?.trim() || null,
        nguoiDaiDien: form.nguoiDaiDien?.trim() || null,
        soDienThoai: form.soDienThoai?.trim() || null,
        email: form.email?.trim() || null,
        ghiChu: form.ghiChu?.trim() || null
    })

    const fetchDonVi = async () => {
        try {
            loading.value = true
            const data = await apiRequest('/DonVi')
            items.value = Array.isArray(data) ? data : []
        } catch (error) {
            console.error(error)
            alert(error.message || 'Không tải được danh sách đơn vị.')
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
            maDonVi: item.maDonVi || '',
            tenDonVi: item.tenDonVi || '',
            loaiDonVi: item.loaiDonVi || 'THANH_PHO',
            donViChaId: item.donViChaId ?? null,
            diaChi: item.diaChi || '',
            nguoiDaiDien: item.nguoiDaiDien || '',
            soDienThoai: item.soDienThoai || '',
            email: item.email || '',
            ghiChu: item.ghiChu || ''
        })

        showModal.value = true
    }

    const closeModal = () => {
        showModal.value = false
        resetForm()
    }

    const validateForm = () => {
        if (!isEdit.value && !form.maDonVi?.trim()) {
            alert('Vui lòng nhập mã đơn vị.')
            return false
        }

        if (!form.tenDonVi?.trim()) {
            alert('Vui lòng nhập tên đơn vị.')
            return false
        }

        if (!form.loaiDonVi) {
            alert('Vui lòng chọn loại đơn vị.')
            return false
        }

        if (form.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
            alert('Email không đúng định dạng.')
            return false
        }

        if (isEdit.value && editingId.value && form.donViChaId === editingId.value) {
            alert('Đơn vị cha không được trùng với chính đơn vị hiện tại.')
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
                await apiRequest(`/DonVi/${editingId.value}`, 'PUT', payload)
            } else {
                await apiRequest('/DonVi', 'POST', payload)
            }

            closeModal()
            await fetchDonVi()
        } catch (error) {
            console.error(error)
            alert(error.message || 'Lưu đơn vị thất bại.')
        } finally {
            saving.value = false
        }
    }

    const handleDelete = async (item) => {
        const ok = window.confirm(`Bạn có chắc muốn xóa đơn vị "${item.tenDonVi}" không?`)
        if (!ok) return

        try {
            await apiRequest(`/DonVi/${item.id}`, 'DELETE')
            await fetchDonVi()
        } catch (error) {
            console.error(error)
            alert(error.message || 'Xóa đơn vị thất bại.')
        }
    }

    const resetFilters = () => {
        filters.keyword = ''
        filters.loaiDonVi = ''
        filters.donViChaId = ''
    }

    const mapLoaiDonVi = (value) => {
        const map = {
            THANH_PHO: 'Thành phố',
            PHONG: 'Cấp phòng',
            XA: 'Cấp xã/phường'
        }
        return map[value] || value || '-'
    }

    const getParentName = (donViChaId) => {
        if (!donViChaId) return '-'
        const parent = items.value.find(x => x.id === donViChaId)
        return parent?.tenDonVi || '-'
    }

    onMounted(() => {
        fetchDonVi()
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


