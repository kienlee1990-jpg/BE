<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-sliders"></i>
                    </div>
                    <div class="gov-text">
                        <div class="wave-title">HỆ THỐNG THEO DÕI CHỈ TIÊU CÔNG TÁC</div>
                        <div class="gov-title">CẤU HÌNH NGƯỠNG ĐÁNH GIÁ KPI</div>
                    </div>
                </div>

                <div class="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
                    <div class="gov-banner">
                        <img src="https://upload.wikimedia.org/wikipedia/commons/a/a3/Emblem_of_Vietnam.svg"
                            class="gov-emblem" />
                    </div>

                    <button class="btn btn-primary btn-action" @click="openCreateModal">
                        <i class="bi bi-plus-circle me-2"></i>
                        Tạo cấu hình
                    </button>
                </div>

                <div class="card custom-card mb-4">
                    <div class="card-header bg-white border-0 pb-0">
                        <h5 class="mb-1">Bộ lọc tìm kiếm</h5>
                        <small class="text-muted">Tra cứu nhanh theo danh mục chỉ tiêu, tiêu chí đánh giá hoặc từ khóa</small>
                    </div>

                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-12 col-md-4">
                                <label class="form-label">Danh mục chỉ tiêu ID</label>
                                <input v-model="filters.danhMucChiTieuId" type="number" min="1" class="form-control"
                                    placeholder="Nhập ID danh mục chỉ tiêu" />
                            </div>

                            <div class="col-12 col-md-4">
                                <label class="form-label">Tiêu chí đánh giá</label>
                                <select v-model="filters.tieuChiDanhGia" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option v-for="item in TIEU_CHI_DANH_GIA_OPTIONS" :key="item.value" :value="item.value">
                                        {{ item.label }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-4">
                                <label class="form-label">Quy tắc đánh giá</label>
                                <select v-model="filters.quyTacDanhGia" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option v-for="item in QUY_TAC_DANH_GIA_OPTIONS" :key="item.value" :value="item.value">
                                        {{ item.label }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-4">
                                <label class="form-label">Từ khóa</label>
                                <input v-model="filters.keyword" type="text" class="form-control"
                                    placeholder="Tìm theo xếp loại, tiêu chí đánh giá hoặc ghi chú" />
                            </div>
                        </div>

                        <div class="d-flex flex-wrap gap-2 mt-4">
                            <button class="btn btn-primary" @click="fetchData">
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
                            <h5 class="mb-1">Danh sách cấu hình ngưỡng đánh giá</h5>
                            <small class="text-muted">Theo dõi và quản lý cấu hình trong hệ thống</small>
                        </div>
                        <span class="badge text-bg-light border">Tổng: {{ items.length }}</span>
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
                            <ColumnVisibilityTools table-id="CauHinhDanhGiaChiTieuPage-table" />
                            <table id="CauHinhDanhGiaChiTieuPage-table" class="table table-hover align-middle mb-0 custom-table managed-table">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Danh mục chỉ tiêu ID</th>
                                        <th>Tiêu chí đánh giá</th>
                                        <th>Quy tắc đánh giá</th>
                                        <th>Từ tỷ lệ</th>
                                        <th>Đến tỷ lệ</th>
                                        <th>Điều kiện thời hạn</th>
                                        <th>Xếp loại</th>
                                        <th>Điểm</th>
                                        <th>Ghi chú</th>
                                        <th class="text-center" style="width: 180px">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="item in items" :key="item.id">
                                        <td class="fw-semibold text-primary">{{ item.id }}</td>
                                        <td>{{ item.danhMucChiTieuId ?? '-' }}</td>
                                        <td>{{ getTieuChiDanhGiaLabel(item.tieuChiDanhGia) }}</td>
                                        <td>{{ getQuyTacDanhGiaLabel(item.quyTacDanhGia) }}</td>
                                        <td>{{ formatNumber(item.tuTyLe) }}</td>
                                        <td>{{ formatNumber(item.denTyLe) }}</td>
                                        <td>{{ getThoiHanLabel(item.dieuKienThoiHan) }}</td>
                                        <td>
                                            <span :class="['rank-badge', getRankClass(item.xepLoai)]">
                                                {{ getDanhGiaLabel(item.xepLoai) }}
                                            </span>
                                        </td>
                                        <td>{{ formatNumber(item.diem) }}</td>
                                        <td>{{ item.ghiChu || '-' }}</td>
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
                    <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                        <div class="modal-content border-0 shadow-lg rounded-4">
                            <div class="modal-header border-0 pb-0">
                                <div>
                                    <h4 class="modal-title mb-1">{{ isEdit ? 'Cập nhật cấu hình ngưỡng đánh giá KPI' :
                                        'Tạo cấu hình ngưỡng đánh giá KPI mới' }}</h4>
                                    <p class="text-muted mb-0">Nhập thông tin cấu hình ngưỡng đánh giá theo từng tiêu chí</p>
                                </div>
                                <button type="button" class="btn-close" @click="closeModal"></button>
                            </div>

                            <div class="modal-body pt-3">
                                <div class="row g-3">
                                    <div class="col-12 col-md-6">
                                        <label class="form-label">Danh mục chỉ tiêu ID</label>
                                        <input v-model="form.danhMucChiTieuId" type="number" min="1"
                                            class="form-control" placeholder="Nhập ID danh mục chỉ tiêu" />
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">
                                            Tiêu chí đánh giá <span class="text-danger">*</span>
                                        </label>
                                        <select v-model="form.tieuChiDanhGia" class="form-select">
                                            <option value="">Chọn tiêu chí đánh giá</option>
                                            <option v-for="item in TIEU_CHI_DANH_GIA_OPTIONS" :key="item.value"
                                                :value="item.value">
                                                {{ item.label }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">
                                            Quy tắc đánh giá <span class="text-danger">*</span>
                                        </label>
                                        <select v-model="form.quyTacDanhGia" class="form-select" :disabled="form.tieuChiDanhGia === 'DINH_TINH'">
                                            <option value="">Chọn quy tắc đánh giá</option>
                                            <option v-for="item in getQuyTacOptionsByCriterion(form.tieuChiDanhGia)" :key="item.value"
                                                :value="item.value">
                                                {{ item.label }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">
                                            Điều kiện thời hạn <span class="text-danger">*</span>
                                        </label>
                                        <select v-model="form.dieuKienThoiHan" class="form-select">
                                            <option value="">Chọn điều kiện thời hạn</option>
                                            <option v-for="item in DIEU_KIEN_THOI_HAN_OPTIONS" :key="item.value"
                                                :value="item.value">
                                                {{ item.label }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">
                                            Xếp loại <span class="text-danger">*</span>
                                        </label>
                                        <select v-model="form.xepLoai" class="form-select">
                                            <option value="">Chọn xếp loại</option>
                                            <option v-for="item in availableXepLoaiOptions" :key="item.value"
                                                :value="item.value">
                                                {{ item.label }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">
                                            Từ tỷ lệ <span class="text-danger">*</span>
                                        </label>
                                        <input v-model="form.tuTyLe" type="number" step="0.01" class="form-control"
                                            placeholder="Ví dụ: 80" />
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">
                                            Đến tỷ lệ <span class="text-danger">*</span>
                                        </label>
                                        <input v-model="form.denTyLe" type="number" step="0.01" class="form-control"
                                            placeholder="Ví dụ: 89.99" />
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">Điểm</label>
                                        <input v-model="form.diem" type="number" step="0.01" class="form-control"
                                            placeholder="Ví dụ: 8.5" />
                                    </div>

                                    <div class="col-12">
                                        <label class="form-label">Ghi chú</label>
                                        <textarea v-model="form.ghiChu" rows="4" class="form-control"
                                            placeholder="Nhập ghi chú"></textarea>
                                    </div>
                                </div>
                            </div>

                            <div class="modal-footer border-0 pt-0">
                                <button class="btn btn-light" @click="closeModal">Hủy</button>
                                <button class="btn btn-primary" :disabled="saving" @click="handleSubmit">
                                    <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                                    {{ saving ? 'Đang lưu...' : isEdit ? 'Cập nhật' : 'Lưu cấu hình' }}
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
    import httpClient from '../../services/httpClient'
    import {
        DANH_GIA_TRACKED_STATUS_OPTIONS,
        DIEU_KIEN_THOI_HAN_OPTIONS,
        getQuyTacDanhGiaLabel,
        getQuyTacOptionsByCriterion,
        isKhongVuotNguongRule,
        TIEU_CHI_DANH_GIA_OPTIONS,
        QUY_TAC_DANH_GIA_OPTIONS,
        getDanhGiaBadgeClass,
        getDanhGiaLabel,
        getThoiHanLabel,
        getTieuChiDanhGiaLabel
    } from '../../utils/danhGiaStatusClean.js'

    const loading = ref(false)
    const saving = ref(false)
    const showModal = ref(false)
    const isEdit = ref(false)
    const editingId = ref(null)
    const items = ref([])

    const filters = reactive({
        danhMucChiTieuId: '',
        tieuChiDanhGia: '',
        quyTacDanhGia: '',
        keyword: ''
    })

    const createDefaultForm = () => ({
        danhMucChiTieuId: '',
        tieuChiDanhGia: '',
        quyTacDanhGia: '',
        tuTyLe: '',
        denTyLe: '',
        dieuKienThoiHan: '',
        xepLoai: '',
        diem: '',
        ghiChu: ''
    })

    const form = reactive(createDefaultForm())

    const availableXepLoaiOptions = computed(() => {
        if (form.tieuChiDanhGia === 'DINH_TINH' || isKhongVuotNguongRule(form.quyTacDanhGia)) {
            return DANH_GIA_TRACKED_STATUS_OPTIONS.filter((item) => item.value !== 'HOAN_THANH_VUOT_MUC')
        }

        return DANH_GIA_TRACKED_STATUS_OPTIONS
    })

    watch(
        () => form.tieuChiDanhGia,
        () => {
            if (form.tieuChiDanhGia === 'DINH_TINH') {
                form.quyTacDanhGia = 'MAC_DINH'
            } else if (!form.quyTacDanhGia || form.quyTacDanhGia === 'MAC_DINH') {
                form.quyTacDanhGia = 'DAT_TOI_THIEU'
            }

            if ((form.tieuChiDanhGia === 'DINH_TINH' || isKhongVuotNguongRule(form.quyTacDanhGia)) && form.xepLoai === 'HOAN_THANH_VUOT_MUC') {
                form.xepLoai = ''
            }
        }
    )

    watch(
        () => form.quyTacDanhGia,
        () => {
            if ((form.tieuChiDanhGia === 'DINH_TINH' || isKhongVuotNguongRule(form.quyTacDanhGia)) && form.xepLoai === 'HOAN_THANH_VUOT_MUC') {
                form.xepLoai = ''
            }
        }
    )

    const resetForm = () => {
        Object.assign(form, createDefaultForm())
    }

    const normalizeResponseData = (response) => {
        if (Array.isArray(response?.data)) return response.data
        if (Array.isArray(response?.data?.data)) return response.data.data
        return []
    }

    const buildPayload = () => ({
        danhMucChiTieuId: form.danhMucChiTieuId ? Number(form.danhMucChiTieuId) : null,
        tieuChiDanhGia: form.tieuChiDanhGia,
        quyTacDanhGia: form.quyTacDanhGia || null,
        tuTyLe: Number(form.tuTyLe),
        denTyLe: Number(form.denTyLe),
        dieuKienThoiHan: form.dieuKienThoiHan,
        xepLoai: form.xepLoai,
        diem: form.diem === '' || form.diem === null ? null : Number(form.diem),
        ghiChu: form.ghiChu?.trim() || null
    })

    const fetchData = async () => {
        try {
            loading.value = true
            const response = await httpClient.get('/cau-hinh-nguong-danh-gia-kpi', {
                params: {
                    danhMucChiTieuId: filters.danhMucChiTieuId ? Number(filters.danhMucChiTieuId) : undefined,
                    tieuChiDanhGia: filters.tieuChiDanhGia || undefined,
                    quyTacDanhGia: filters.quyTacDanhGia || undefined,
                    keyword: filters.keyword || undefined
                }
            })
            items.value = normalizeResponseData(response)
        } catch (error) {
            console.error(error)
            alert(error?.response?.data?.message || 'Không tải được danh sách cấu hình.')
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
            danhMucChiTieuId: item.danhMucChiTieuId ?? '',
            tieuChiDanhGia: item.tieuChiDanhGia || '',
            quyTacDanhGia: item.quyTacDanhGia || '',
            tuTyLe: item.tuTyLe ?? '',
            denTyLe: item.denTyLe ?? '',
            dieuKienThoiHan: item.dieuKienThoiHan || '',
            xepLoai: item.xepLoai || '',
            diem: item.diem ?? '',
            ghiChu: item.ghiChu || ''
        })

        showModal.value = true
    }

    const closeModal = () => {
        showModal.value = false
        resetForm()
    }

    const validateForm = () => {
        if (form.danhMucChiTieuId !== '' && Number(form.danhMucChiTieuId) <= 0) {
            alert('Danh mục chỉ tiêu ID không hợp lệ.')
            return false
        }

        if (!form.tieuChiDanhGia?.trim()) {
            alert('Vui lòng chọn tiêu chí đánh giá.')
            return false
        }

        if (!form.quyTacDanhGia?.trim()) {
            alert('Vui lòng chọn quy tắc đánh giá.')
            return false
        }

        if (form.tuTyLe === '' || form.denTyLe === '') {
            alert('Vui lòng nhập đầy đủ từ tỷ lệ và đến tỷ lệ.')
            return false
        }

        const tuTyLe = Number(form.tuTyLe)
        const denTyLe = Number(form.denTyLe)

        if (Number.isNaN(tuTyLe) || Number.isNaN(denTyLe)) {
            alert('Tỷ lệ không hợp lệ.')
            return false
        }

        if (tuTyLe < 0 || denTyLe < 0) {
            alert('Tỷ lệ không được nhỏ hơn 0.')
            return false
        }

        if (tuTyLe > denTyLe) {
            alert('Từ tỷ lệ phải nhỏ hơn hoặc bằng đến tỷ lệ.')
            return false
        }

        if (!form.dieuKienThoiHan?.trim()) {
            alert('Vui lòng chọn điều kiện thời hạn.')
            return false
        }

        if (!form.xepLoai?.trim()) {
            alert('Vui lòng chọn xếp loại.')
            return false
        }

        if (form.diem !== '' && form.diem !== null && Number.isNaN(Number(form.diem))) {
            alert('Điểm không hợp lệ.')
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
                await httpClient.put(`/cau-hinh-nguong-danh-gia-kpi/${editingId.value}`, payload)
            } else {
                await httpClient.post('/cau-hinh-nguong-danh-gia-kpi', payload)
            }

            closeModal()
            await fetchData()
        } catch (error) {
            console.error(error)
            alert(error?.response?.data?.message || 'Lưu cấu hình thất bại.')
        } finally {
            saving.value = false
        }
    }

    const handleDelete = async (item) => {
        const ok = window.confirm(`Bạn có chắc muốn xóa cấu hình xếp loại "${getDanhGiaLabel(item.xepLoai)}" không?`)
        if (!ok) return

        try {
            await httpClient.delete(`/cau-hinh-nguong-danh-gia-kpi/${item.id}`)
            await fetchData()
        } catch (error) {
            console.error(error)
            alert(error?.response?.data?.message || 'Xóa cấu hình thất bại.')
        }
    }

    const resetFilters = async () => {
        filters.danhMucChiTieuId = ''
        filters.tieuChiDanhGia = ''
        filters.quyTacDanhGia = ''
        filters.keyword = ''
        await fetchData()
    }

    const formatNumber = (value) => {
        if (value === null || value === undefined || value === '') return '-'
        return Number(value).toLocaleString('vi-VN')
    }

    const getRankClass = (value) => getDanhGiaBadgeClass(value).replace('badge-', 'rank-')

    onMounted(() => {
        fetchData()
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

    :deep(.table tbody tr) {
        border-bottom: 1px solid #f1f1f1;
    }

    :deep(.table tbody tr:hover) {
        background: rgba(0, 0, 0, 0.03);
    }

    .rank-badge {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        min-width: 110px;
        padding: 6px 12px;
        border-radius: 999px;
        font-size: 0.82rem;
        font-weight: 700;
    }

    .rank-excellent {
        background: #d1e7dd;
        color: #0f5132;
    }

    .rank-good {
        background: #cff4fc;
        color: #055160;
    }

    .rank-pass {
        background: #fff3cd;
        color: #856404;
    }

    .rank-fail {
        background: #f8d7da;
        color: #842029;
    }

    .rank-default {
        background: #e9ecef;
        color: #495057;
    }

    .custom-modal {
        background: rgba(137, 210, 239, 0.5);
    }

    :deep(.modal-content) {
        animation: fadeInUp 0.3s ease;
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

    @media (max-width: 768px) {
        .gov-banner {
            padding: 16px;
            align-items: flex-start;
        }

        .gov-title {
            font-size: 1.05rem;
        }

        .page-title {
            font-size: 1.4rem;
        }
    }
</style>




