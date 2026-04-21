<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-diagram-2-fill"></i>
                    </div>
                    <div class="gov-text">
                        <div class="gov-title">CHỈ TIÊU CHO CÁC ĐƠN VỊ XÃ/PHƯỜNG</div>
                    </div>
                </div>

                <div class="d-flex justify-content-end mb-4">
                    <button class="btn btn-primary btn-action" @click="openCreateModal">
                        <i class="bi bi-plus-circle me-2"></i>
                        Tạo giao chỉ tiêu
                    </button>
                </div>

                <div class="card custom-card mb-4">
                    <div class="card-header bg-white border-0 pb-0">
                        <h5 class="mb-1">Bộ lọc tìm kiếm</h5>
                        <small class="text-muted">Tra cứu nhanh theo nhiều tiêu chí</small>
                    </div>

                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Đợt giao chỉ tiêu</label>
                                <select v-model.number="filters.dotGiaoChiTieuId" class="form-select">
                                    <option :value="null">Tất cả</option>
                                    <option v-for="item in dotOptions" :key="getId(item)" :value="getId(item)">
                                        {{ item.TenDotGiao || item.tenDotGiao || '-' }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Danh mục chỉ tiêu</label>
                                <select v-model.number="filters.danhMucChiTieuId" class="form-select">
                                    <option :value="null">Tất cả</option>
                                    <option v-for="item in danhMucOptions" :key="getId(item)" :value="getId(item)">
                                        {{ item.MaChiTieu || item.maChiTieu || '' }} -
                                        {{ item.TenChiTieu || item.tenChiTieu || '' }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Đơn vị thực hiện chính</label>
                                <select v-model.number="filters.donViThucHienChinhId" class="form-select">
                                    <option :value="null">Tất cả</option>
                                    <option v-for="item in otherDonViOptions" :key="getId(item)" :value="getId(item)">
                                        {{ item.TenDonVi || item.tenDonVi || '-' }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Tần suất báo cáo</label>
                                <select v-model="filters.tanSuatBaoCao" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option v-for="item in tanSuatBaoCaoOptions" :key="item.value" :value="item.value">
                                        {{ item.label }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12">
                                <label class="form-label">Từ khóa</label>
                                <input v-model="filters.keyword" type="text" class="form-control"
                                    placeholder="Tên đợt, mã chỉ tiêu, tên chỉ tiêu, đơn vị tính, đơn vị thực hiện chính, tần suất, ghi chú" />
                            </div>
                        </div>

                        <div class="d-flex flex-wrap gap-2 mt-4">
                            <button class="btn btn-primary" @click="fetchItems">
                                <i class="bi bi-arrow-repeat me-1"></i>
                                Tải lại dữ liệu
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
                            <h5 class="mb-1">Danh sách giao chỉ tiêu</h5>
                            <small class="text-muted">
                                Chỉ hiển thị các bản ghi có đơn vị thực hiện chính khác Công an thành phố Đà Nẵng
                            </small>
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
                            <ColumnVisibilityTools table-id="ChiTieuThanhPhoGiaoPage-table" />
                            <table id="ChiTieuThanhPhoGiaoPage-table" class="table table-hover align-middle mb-0 custom-table managed-table">
                                <thead>
                                    <tr>
                                        <th>Đợt giao</th>
                                        <th>Chỉ tiêu</th>
                                        <th>Đơn vị thực hiện chính</th>
                                        <th>Tần suất báo cáo</th>
                                        <th>Giá trị mục tiêu</th>
                                        <th>Ghi chú</th>
                                        <th class="text-center" style="width: 180px">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="item in filteredItems" :key="getId(item)">
                                        <td>
                                            <div class="fw-semibold">
                                                {{ item.TenDotGiao || item.tenDotGiao || '-' }}
                                            </div>
                                        </td>

                                        <td>
                                            <div class="fw-semibold text-primary">
                                                {{ item.TenChiTieu || item.tenChiTieu || '-' }}
                                            </div>
                                            <small class="text-muted">
                                                {{ item.MaChiTieu || item.maChiTieu || '-' }}
                                            </small>
                                            <div class="mt-1 text-muted">Đơn vị tính: {{ getDonViTinhText(item) }}</div>
                                        </td>

                                        <td><div class="fw-semibold">{{ item.TenDonViThucHienChinh || item.tenDonViThucHienChinh || '-' }}</div></td>

                                        <td>
                                            <span class="badge text-bg-light border">
                                                {{ getTanSuatLabel(item.TanSuatBaoCao || item.tanSuatBaoCao) }}
                                            </span>
                                        </td>

                                        <td>
                                            <div>
                                                {{ formatNumberWithUnit(item.GiaTriMucTieu ?? item.giaTriMucTieu,
                                                getDonViTinhText(item)) }}
                                            </div>
                                            <span v-if="(item.DieuKienHoanThanh ?? item.dieuKienHoanThanh) !== null && (item.DieuKienHoanThanh ?? item.dieuKienHoanThanh) !== undefined"> • Mặc định: {{ formatNumberWithUnit(item.DieuKienHoanThanh ?? item.dieuKienHoanThanh, getDonViTinhText(item)) }}</span>
                                        </td>

                                        <td>{{ item.GhiChu || item.ghiChu || '-' }}</td>

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
                                        {{ isEdit ? 'Cập nhật giao chỉ tiêu' : 'Tạo giao chỉ tiêu mới' }}
                                    </h4>
                                    <p class="text-muted mb-0">
                                        Chọn đợt giao, chỉ tiêu, đơn vị thực hiện chính, tần suất báo cáo và giá trị mục
                                        tiêu
                                    </p>
                                </div>
                                <button type="button" class="btn-close" @click="closeModal"></button>
                            </div>

                            <div class="modal-body pt-3">
                                <div class="row g-3">
                                    <div class="col-12">
                                        <div class="alert alert-info mb-0">
                                            <i class="bi bi-info-circle me-2"></i>
                                            Chỉ cho phép chọn <strong>đơn vị thực hiện chính khác {{ FIXED_DON_VI_NAME
                                                }}</strong>.
                                        </div>
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">
                                            Đợt giao chỉ tiêu <span class="text-danger">*</span>
                                        </label>
                                        <select v-model.number="form.dotGiaoChiTieuId" class="form-select">
                                            <option :value="null">Chọn đợt giao</option>
                                            <option v-for="item in dotOptions" :key="getId(item)" :value="getId(item)">
                                                {{ item.TenDotGiao || item.tenDotGiao || '-' }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">
                                            Danh mục chỉ tiêu <span class="text-danger">*</span>
                                        </label>
                                        <select v-model.number="form.danhMucChiTieuId" class="form-select">
                                            <option :value="null">Chọn chỉ tiêu</option>
                                            <option v-for="item in danhMucOptions" :key="getId(item)"
                                                :value="getId(item)">
                                                {{ item.MaChiTieu || item.maChiTieu || '' }} -
                                                {{ item.TenChiTieu || item.tenChiTieu || '' }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">
                                            Đơn vị thực hiện chính <span class="text-danger">*</span>
                                        </label>
                                        <select v-model.number="form.donViThucHienChinhId" class="form-select">
                                            <option :value="null">Chọn đơn vị</option>
                                            <option v-for="item in otherDonViOptions" :key="getId(item)"
                                                :value="getId(item)">
                                                {{ item.TenDonVi || item.tenDonVi || '-' }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">
                                            Tần suất báo cáo <span class="text-danger">*</span>
                                        </label>
                                        <select v-model="form.tanSuatBaoCao" class="form-select">
                                            <option value="">Chọn tần suất</option>
                                            <option v-for="item in tanSuatBaoCaoOptions" :key="item.value"
                                                :value="item.value">
                                                {{ item.label }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">Đơn vị tính</label>
                                        <input :value="displayDonViTinh" type="text" class="form-control" readonly />
                                    </div>

                                    <div class="col-12 col-md-6">
                                        <label class="form-label">Giá trị mục tiêu gốc</label>
                                        <input :value="displayGiaTriMacDinh" type="text" class="form-control"
                                            readonly />
                                    </div>

                                    <div class="col-12">
                                        <label class="form-label">
                                            Giá trị mục tiêu <span class="text-danger">*</span>
                                        </label>
                                        <div class="input-group">
                                            <input v-model="form.giaTriMucTieu" type="number" step="any"
                                                class="form-control" placeholder="Nhập giá trị mục tiêu" />
                                            <span class="input-group-text">{{ displayDonViTinh }}</span>
                                        </div>
                                    </div>

                                    <div class="col-12">
                                        <label class="form-label">Ghi chú</label>
                                        <textarea v-model="form.ghiChu" rows="4" class="form-control"
                                            placeholder="Nhập ghi chú nếu có"></textarea>
                                    </div>
                                </div>
                            </div>

                            <div class="modal-footer border-0 pt-0">
                                <button class="btn btn-light" @click="closeModal">Hủy</button>
                                <button class="btn btn-primary" :disabled="saving" @click="handleSubmit">
                                    <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                                    {{ saving ? 'Đang lưu...' : isEdit ? 'Cập nhật' : 'Lưu giao chỉ tiêu' }}
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

    const API_PATHS = {
        chiTietGiaoChiTieu: '/ChiTietGiaoChiTieu',
        dotGiaoChiTieu: '/dot-giao-chi-tieu',
        danhMucChiTieu: '/danh-muc-chi-tieu',
        donVi: '/DonVi'
    }

    const FIXED_DON_VI_NAME = 'Công an thành phố Đà Nẵng'

    const loading = ref(false)
    const saving = ref(false)
    const showModal = ref(false)
    const isEdit = ref(false)
    const editingId = ref(null)
    const syncingForm = ref(false)

    const items = ref([])
    const dotOptions = ref([])
    const danhMucOptions = ref([])
    const donViOptions = ref([])

    const tanSuatBaoCaoOptions = [
        { value: 'NGAY', label: 'Ngày' },
        { value: 'TUAN', label: 'Tuần' },
        { value: 'THANG', label: 'Tháng' },
        { value: 'QUY', label: 'Quý' },
        { value: '6_THANG', label: '6 tháng' },
        { value: 'NAM', label: 'Năm' }
    ]

    const filters = reactive({
        dotGiaoChiTieuId: null,
        danhMucChiTieuId: null,
        donViThucHienChinhId: null,
        tanSuatBaoCao: '',
        keyword: ''
    })

    const createDefaultForm = () => ({
        dotGiaoChiTieuId: null,
        danhMucChiTieuId: null,
        donViThucHienChinhId: null,
        tanSuatBaoCao: '',
        giaTriMucTieu: null,
        ghiChu: ''
    })

    const form = reactive(createDefaultForm())

    const resetForm = () => {
        Object.assign(form, createDefaultForm())
    }

    const getId = (item) => Number(item?.Id ?? item?.id ?? 0)

    const normalizeList = (response) => {
        if (Array.isArray(response)) return response
        if (Array.isArray(response?.data)) return response.data
        if (Array.isArray(response?.data?.data)) return response.data.data
        if (Array.isArray(response?.data?.items)) return response.data.items
        if (Array.isArray(response?.items)) return response.items
        return []
    }

    const normalizeText = (value) => String(value || '').trim().toLowerCase()

    const getTanSuatLabel = (value) => {
        const found = tanSuatBaoCaoOptions.find(x => x.value === value)
        return found?.label || value || '-'
    }

    const findDanhMucById = (id) => {
        return danhMucOptions.value.find(x => getId(x) === Number(id)) || null
    }

    const findDonViById = (id) => {
        return donViOptions.value.find(x => getId(x) === Number(id)) || null
    }

    const getGiaTriMacDinhTuDanhMuc = (item) => {
        return item?.DieuKienHoanThanh ?? item?.dieuKienHoanThanh ?? null
    }

    const getDonViTinh = (item) => {
        return (
            item?.DonViTinh ??
            item?.donViTinh ??
            item?.TenDonViTinh ??
            item?.tenDonViTinh ??
            item?.DVT ??
            item?.dvt ??
            ''
        )
    }

    const getDonViTinhText = (item) => {
        const value = getDonViTinh(item)
        return value && String(value).trim() ? String(value).trim() : '-'
    }

    const otherDonViOptions = computed(() => {
        return donViOptions.value.filter(item => {
            const ten = item.TenDonVi || item.tenDonVi || ''
            return normalizeText(ten) !== normalizeText(FIXED_DON_VI_NAME)
        })
    })

    const selectedDanhMuc = computed(() => {
        if (!form.danhMucChiTieuId) return null
        return findDanhMucById(form.danhMucChiTieuId)
    })

    const selectedDonViThucHienChinh = computed(() => {
        if (!form.donViThucHienChinhId) return null
        return findDonViById(form.donViThucHienChinhId)
    })

    const defaultGiaTriMucTieu = computed(() => {
        if (!selectedDanhMuc.value) return null
        return getGiaTriMacDinhTuDanhMuc(selectedDanhMuc.value)
    })

    const displayDonViTinh = computed(() => {
        if (!selectedDanhMuc.value) return '-'
        return getDonViTinhText(selectedDanhMuc.value)
    })

    const formatNumber = (value) => {
        if (value === null || value === undefined || value === '') return '-'

        const numberValue = Number(value)
        if (Number.isNaN(numberValue)) return value

        return numberValue.toLocaleString('vi-VN')
    }

    const formatNumberWithUnit = (value, unit) => {
        const formatted = formatNumber(value)
        if (formatted === '-') return '-'
        if (!unit || unit === '-') return formatted
        return `${formatted} ${unit}`
    }

    const displayGiaTriMacDinh = computed(() => {
        if (
            defaultGiaTriMucTieu.value === null ||
            defaultGiaTriMucTieu.value === undefined ||
            defaultGiaTriMucTieu.value === ''
        ) {
            return '-'
        }
        return formatNumberWithUnit(defaultGiaTriMucTieu.value, displayDonViTinh.value)
    })

    const enrichedItems = computed(() => {
        return items.value.map(item => {
            const dotId = Number(item.DotGiaoChiTieuId ?? item.dotGiaoChiTieuId ?? 0)
            const danhMucId = Number(item.DanhMucChiTieuId ?? item.danhMucChiTieuId ?? 0)
            const donViNhanId = Number(item.DonViNhanId ?? item.donViNhanId ?? 0)
            const donViThucHienChinhId = Number(item.DonViThucHienChinhId ?? item.donViThucHienChinhId ?? 0)

            const dot = dotOptions.value.find(x => getId(x) === dotId)
            const danhMuc = danhMucOptions.value.find(x => getId(x) === danhMucId)
            const donViNhan = donViOptions.value.find(x => getId(x) === donViNhanId)
            const donViThucHienChinh = donViOptions.value.find(x => getId(x) === donViThucHienChinhId)

            return {
                ...item,
                TenDotGiao:
                    item.TenDotGiao ||
                    item.tenDotGiao ||
                    dot?.TenDotGiao ||
                    dot?.tenDotGiao ||
                    '-',

                TenChiTieu:
                    item.TenChiTieu ||
                    item.tenChiTieu ||
                    danhMuc?.TenChiTieu ||
                    danhMuc?.tenChiTieu ||
                    '-',

                MaChiTieu:
                    item.MaChiTieu ||
                    item.maChiTieu ||
                    danhMuc?.MaChiTieu ||
                    danhMuc?.maChiTieu ||
                    '-',

                TenDonViNhan:
                    item.TenDonViNhan ||
                    item.tenDonViNhan ||
                    donViNhan?.TenDonVi ||
                    donViNhan?.tenDonVi ||
                    '-',

                MaDonViNhan:
                    item.MaDonViNhan ||
                    item.maDonViNhan ||
                    donViNhan?.MaDonVi ||
                    donViNhan?.maDonVi ||
                    '-',

                TenDonViThucHienChinh:
                    item.TenDonViThucHienChinh ||
                    item.tenDonViThucHienChinh ||
                    donViThucHienChinh?.TenDonVi ||
                    donViThucHienChinh?.tenDonVi ||
                    '-',

                MaDonViThucHienChinh:
                    item.MaDonViThucHienChinh ||
                    item.maDonViThucHienChinh ||
                    donViThucHienChinh?.MaDonVi ||
                    donViThucHienChinh?.maDonVi ||
                    '-',

                DonViTinh:
                    item.DonViTinh ||
                    item.donViTinh ||
                    item.TenDonViTinh ||
                    item.tenDonViTinh ||
                    item.DVT ||
                    item.dvt ||
                    danhMuc?.DonViTinh ||
                    danhMuc?.donViTinh ||
                    danhMuc?.TenDonViTinh ||
                    danhMuc?.tenDonViTinh ||
                    danhMuc?.DVT ||
                    danhMuc?.dvt ||
                    '-',

                TanSuatBaoCao: item.TanSuatBaoCao || item.tanSuatBaoCao || '',

                DieuKienHoanThanh:
                    item.DieuKienHoanThanh ??
                    item.dieuKienHoanThanh ??
                    getGiaTriMacDinhTuDanhMuc(danhMuc),

                GiaTriMucTieu:
                    item.GiaTriMucTieu ??
                    item.giaTriMucTieu ??
                    getGiaTriMacDinhTuDanhMuc(danhMuc)
            }
        })
    })

    const filteredItems = computed(() => {
        return enrichedItems.value.filter(item => {
            const keyword = filters.keyword.trim().toLowerCase()
            const dotId = Number(item.DotGiaoChiTieuId ?? item.dotGiaoChiTieuId ?? 0)
            const danhMucId = Number(item.DanhMucChiTieuId ?? item.danhMucChiTieuId ?? 0)
            const donViThucHienChinhId = Number(item.DonViThucHienChinhId ?? item.donViThucHienChinhId ?? 0)
            const tanSuatBaoCao = (item.TanSuatBaoCao || item.tanSuatBaoCao || '').trim()

            const tenDonViThucHienChinh = item.TenDonViThucHienChinh || item.tenDonViThucHienChinh || ''
            const isNotFixedDonVi =
                normalizeText(tenDonViThucHienChinh) !== normalizeText(FIXED_DON_VI_NAME)

            const searchText = [
                item.TenDotGiao || '',
                item.MaChiTieu || '',
                item.TenChiTieu || '',
                item.DonViTinh || '',
                item.TenDonViThucHienChinh || '',
                getTanSuatLabel(item.TanSuatBaoCao || item.tanSuatBaoCao || ''),
                item.GhiChu || item.ghiChu || ''
            ].join(' ').toLowerCase()

            const matchKeyword = !keyword || searchText.includes(keyword)
            const matchDot = !filters.dotGiaoChiTieuId || Number(filters.dotGiaoChiTieuId) === dotId
            const matchDanhMuc = !filters.danhMucChiTieuId || Number(filters.danhMucChiTieuId) === danhMucId
            const matchDonVi =
                !filters.donViThucHienChinhId ||
                Number(filters.donViThucHienChinhId) === donViThucHienChinhId
            const matchTanSuat = !filters.tanSuatBaoCao || filters.tanSuatBaoCao === tanSuatBaoCao

            return isNotFixedDonVi && matchKeyword && matchDot && matchDanhMuc && matchDonVi && matchTanSuat
        })
    })

    watch(
        () => form.danhMucChiTieuId,
        (newValue, oldValue) => {
            if (syncingForm.value) return
            if (!newValue) {
                form.giaTriMucTieu = null
                return
            }

            const danhMuc = findDanhMucById(newValue)
            const giaTriMacDinh = getGiaTriMacDinhTuDanhMuc(danhMuc)

            if (!isEdit.value) {
                form.giaTriMucTieu = giaTriMacDinh
                return
            }

            if (newValue !== oldValue) {
                form.giaTriMucTieu = giaTriMacDinh
            }
        },
        { immediate: true }
    )

    const buildPayload = () => ({
        dotGiaoChiTieuId: form.dotGiaoChiTieuId,
        danhMucChiTieuId: form.danhMucChiTieuId,
        donViNhanId: form.donViThucHienChinhId,
        donViThucHienChinhId: form.donViThucHienChinhId,
        tanSuatBaoCao: form.tanSuatBaoCao || null,
        giaTriMucTieu:
            form.giaTriMucTieu === '' || form.giaTriMucTieu === null || form.giaTriMucTieu === undefined
                ? null
                : Number(form.giaTriMucTieu),
        ghiChu: form.ghiChu?.trim() || null
    })

    const fetchItems = async () => {
        const data = await apiRequest(API_PATHS.chiTietGiaoChiTieu)
        items.value = normalizeList(data)
    }

    const loadData = async () => {
        try {
            loading.value = true

            const [chiTietData, dotData, danhMucData, donViData] = await Promise.all([
                apiRequest(API_PATHS.chiTietGiaoChiTieu),
                apiRequest(API_PATHS.dotGiaoChiTieu),
                apiRequest(API_PATHS.danhMucChiTieu),
                apiRequest(API_PATHS.donVi)
            ])

            items.value = normalizeList(chiTietData)
            dotOptions.value = normalizeList(dotData)
            danhMucOptions.value = normalizeList(danhMucData)
            donViOptions.value = normalizeList(donViData)
        } catch (error) {
            console.error('loadData error:', error)
            alert(error.message || 'Không tải được dữ liệu.')
            items.value = []
            dotOptions.value = []
            danhMucOptions.value = []
            donViOptions.value = []
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
        editingId.value = getId(item)
        syncingForm.value = true

        const danhMucId = item.DanhMucChiTieuId ?? item.danhMucChiTieuId ?? null
        const danhMuc = findDanhMucById(danhMucId)
        const donViThucHienChinhId = item.DonViThucHienChinhId ?? item.donViThucHienChinhId ?? null

        Object.assign(form, {
            dotGiaoChiTieuId: item.DotGiaoChiTieuId ?? item.dotGiaoChiTieuId ?? null,
            danhMucChiTieuId: danhMucId,
            donViThucHienChinhId,
            tanSuatBaoCao: item.TanSuatBaoCao || item.tanSuatBaoCao || '',
            giaTriMucTieu:
                item.GiaTriMucTieu ??
                item.giaTriMucTieu ??
                getGiaTriMacDinhTuDanhMuc(danhMuc),
            ghiChu: item.GhiChu || item.ghiChu || ''
        })

        syncingForm.value = false
        showModal.value = true
    }

    const closeModal = () => {
        showModal.value = false
        resetForm()
    }

    const validateForm = () => {
        if (!form.dotGiaoChiTieuId || Number(form.dotGiaoChiTieuId) <= 0) {
            alert('Vui lòng chọn đợt giao chỉ tiêu.')
            return false
        }

        if (!form.danhMucChiTieuId || Number(form.danhMucChiTieuId) <= 0) {
            alert('Vui lòng chọn danh mục chỉ tiêu.')
            return false
        }

        if (!form.donViThucHienChinhId || Number(form.donViThucHienChinhId) <= 0) {
            alert('Vui lòng chọn đơn vị thực hiện chính.')
            return false
        }

        const selectedDonVi = findDonViById(form.donViThucHienChinhId)
        const tenDonVi = selectedDonVi?.TenDonVi || selectedDonVi?.tenDonVi || ''

        if (normalizeText(tenDonVi) === normalizeText(FIXED_DON_VI_NAME)) {
            alert(`Không được chọn đơn vị "${FIXED_DON_VI_NAME}".`)
            return false
        }

        if (!form.tanSuatBaoCao) {
            alert('Vui lòng chọn tần suất báo cáo.')
            return false
        }

        if (form.giaTriMucTieu === null || form.giaTriMucTieu === undefined || form.giaTriMucTieu === '') {
            alert('Vui lòng nhập giá trị mục tiêu.')
            return false
        }

        if (Number.isNaN(Number(form.giaTriMucTieu))) {
            alert('Giá trị mục tiêu không hợp lệ.')
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
                await apiRequest(`${API_PATHS.chiTietGiaoChiTieu}/${editingId.value}`, 'PUT', payload)
            } else {
                await apiRequest(API_PATHS.chiTietGiaoChiTieu, 'POST', payload)
            }

            closeModal()
            await fetchItems()
        } catch (error) {
            console.error('handleSubmit error:', error)
            alert(error.message || 'Lưu giao chỉ tiêu thất bại.')
        } finally {
            saving.value = false
        }
    }

    const handleDelete = async (item) => {
        const tenChiTieu = item.TenChiTieu || item.tenChiTieu || ''
        const ok = window.confirm(`Bạn có chắc muốn xóa giao chỉ tiêu "${tenChiTieu}" không?`)
        if (!ok) return

        try {
            await apiRequest(`${API_PATHS.chiTietGiaoChiTieu}/${getId(item)}`, 'DELETE')
            await fetchItems()
        } catch (error) {
            console.error('handleDelete error:', error)
            alert(error.message || 'Xóa giao chỉ tiêu thất bại.')
        }
    }

    const resetFilters = () => {
        filters.dotGiaoChiTieuId = null
        filters.danhMucChiTieuId = null
        filters.donViThucHienChinhId = null
        filters.tanSuatBaoCao = ''
        filters.keyword = ''
    }

    onMounted(async () => {
        await loadData()
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
        border: none;
        border-radius: 16px;
        box-shadow: 0 6px 18px rgba(15, 23, 42, 0.06);
        overflow: hidden;
    }

    .custom-card .card-header {
        padding: 1rem 1.25rem 0.75rem;
    }

    .custom-card .card-body {
        padding: 1.25rem;
    }

    .btn-action {
        border-radius: 12px;
        font-weight: 600;
        padding: 0.7rem 1.1rem;
    }

    .form-label {
        font-weight: 600;
        color: #374151;
        margin-bottom: 0.45rem;
    }

    .form-control,
    .form-select,
    .input-group-text {
        min-height: 44px;
        border-radius: 12px;
        border-color: #dbe3ef;
        box-shadow: none;
    }

    .form-control:focus,
    .form-select:focus {
        border-color: #89d2ef;
        box-shadow: 0 0 0 0.2rem rgba(137, 210, 239, 0.2);
    }

    textarea.form-control {
        min-height: unset;
    }

    :deep(.table) {
        margin-bottom: 0;
        border-collapse: collapse;
    }

    :deep(.table thead th) {
        border-bottom: 2px solid #dee2e6;
        background: #f8fafc;
        color: #374151;
        font-weight: 700;
        vertical-align: middle;
        white-space: nowrap;
    }

    :deep(.table td),
    :deep(.table th) {
        border-right: 1px solid #eee;
        padding: 0.9rem 0.85rem;
        vertical-align: middle;
    }

    :deep(.table td:last-child),
    :deep(.table th:last-child) {
        border-right: none;
    }

    :deep(.table tbody tr) {
        border-bottom: 1px solid #f1f1f1;
    }

    :deep(.table-hover tbody tr:hover) {
        background-color: rgba(0, 0, 0, 0.03);
    }

    .empty-state {
        min-height: 260px;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        color: #6b7280;
        text-align: center;
        padding: 2rem 1rem;
    }

    .badge.text-bg-light {
        font-weight: 600;
        border-radius: 999px;
        padding: 0.45rem 0.7rem;
    }

    .custom-modal {
        background: rgba(137, 210, 239, 0.5);
    }

    :deep(.modal-content) {
        animation: fadeInUp 0.3s ease;
    }

    @keyframes fadeInUp {
        from {
            transform: translateY(20px);
            opacity: 0;
        }

        to {
            transform: translateY(0);
            opacity: 1;
        }
    }

    .modal-title {
        font-weight: 700;
        color: #1f2937;
    }

    .alert-info {
        border: none;
        border-radius: 14px;
        background: rgba(137, 210, 239, 0.18);
        color: #24566b;
    }

    @media (max-width: 768px) {
        .page-title {
            font-size: 1.45rem;
        }

        .custom-card .card-body {
            padding: 1rem;
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







