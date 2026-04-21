<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-diagram-3"></i>
                    </div>
                    <div class="gov-text">
                        <div class="gov-title">PHÂN RÃ CHỈ TIÊU</div>
                        <div class="gov-sub">Quản lý phân rã chỉ tiêu từ chỉ tiêu cha xuống đơn vị thực hiện</div>
                    </div>
                </div>

                <div class="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
                    <div class="text-muted">
                        Chỉ hiển thị các chi tiết giao chỉ tiêu có danh mục chỉ tiêu <strong>cho phép phân rã</strong>.
                    </div>

                    <button class="btn btn-primary btn-action" @click="openCreateModal">
                        <i class="bi bi-plus-circle me-2"></i>
                        Tạo phân rã
                    </button>
                </div>

                <div class="card custom-card mb-4">
                    <div class="card-header bg-white border-0 pb-0">
                        <h5 class="mb-1">Bộ lọc tìm kiếm</h5>
                        <small class="text-muted">Tra cứu nhanh các chỉ tiêu phân rã</small>
                    </div>

                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Đợt giao chỉ tiêu</label>
                                <select v-model="filters.dotGiaoChiTieuId" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option v-for="item in dotOptions" :key="getId(item)" :value="String(getId(item))">
                                        {{ item.TenDotGiao || item.tenDotGiao || '-' }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Chỉ tiêu cha</label>
                                <select v-model="filters.chiTietGiaoChaId" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option v-for="item in eligibleParentOptions" :key="getId(item)"
                                        :value="String(getId(item))">
                                        {{ getParentDisplay(item) }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Đơn vị nhận</label>
                                <select v-model="filters.donViNhanId" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option v-for="item in donViOptions" :key="getId(item)"
                                        :value="String(getId(item))">
                                        {{ item.TenDonVi || item.tenDonVi || '-' }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Từ khóa</label>
                                <input v-model="filters.keyword" type="text" class="form-control"
                                    placeholder="Chỉ tiêu cha, đơn vị, ghi chú..." />
                            </div>
                        </div>

                        <div class="d-flex flex-wrap gap-2 mt-4">
                            <button class="btn btn-primary" @click="applyFilters">
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
                            <h5 class="mb-1">Danh sách chỉ tiêu phân rã</h5>
                            <small class="text-muted">Chỉ gồm các bản ghi có chỉ tiêu cha thuộc danh mục cho phép phân
                                rã</small>
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
                            <div>Chưa có dữ liệu phân rã</div>
                        </div>

                        <div v-else class="table-responsive">
                            <ColumnVisibilityTools table-id="XuatBaoCaoPage-table" />
                            <table id="XuatBaoCaoPage-table" class="table table-hover align-middle mb-0 custom-table managed-table">
                                <thead>
                                    <tr>
                                        <th>Đợt giao</th>
                                        <th>Chỉ tiêu cha</th>
                                        <th>Đơn vị nhận</th>
                                        <th>Đơn vị thực hiện chính</th>
                                        <th>Tần suất</th>
                                        <th>Đơn vị tính</th>
                                        <th>Giá trị mục tiêu gốc</th>
                                        <th>Giá trị phân rã</th>
                                        <th>Ghi chú</th>
                                        <th class="text-center" style="width: 180px">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="item in filteredItems" :key="getId(item)">
                                        <td>{{ getDotGiaoLabel(item.dotGiaoChiTieuId || item.DotGiaoChiTieuId) }}</td>
                                        <td class="fw-semibold text-primary">
                                            {{ getParentName(item.chiTietGiaoChaId || item.ChiTietGiaoChaId) }}
                                        </td>
                                        <td>{{ getDonViLabel(item.donViNhanId || item.DonViNhanId) }}</td>
                                        <td>{{ getDonViLabel(item.donViThucHienChinhId || item.DonViThucHienChinhId) }}
                                        </td>
                                        <td>{{ mapTanSuat(item.tanSuatBaoCao || item.TanSuatBaoCao) }}</td>
                                        <td>{{ getDonViTinhByDanhMuc(item.danhMucChiTieuId || item.DanhMucChiTieuId) }}
                                        </td>
                                        <td>{{ getParentGiaTri(item.chiTietGiaoChaId || item.ChiTietGiaoChaId) }}</td>
                                        <td>{{ formatNumber(item.giaTriMucTieu || item.GiaTriMucTieu) }}</td>
                                        <td>{{ item.ghiChu || item.GhiChu || '' }}</td>
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
                                        {{ isEdit ? 'Cập nhật phân rã chỉ tiêu' : 'Tạo phân rã chỉ tiêu mới' }}
                                    </h4>
                                    <p class="text-muted mb-0">
                                        Chọn chỉ tiêu cha, đơn vị nhận, đơn vị thực hiện và nhập giá trị mục tiêu phân
                                        rã
                                    </p>
                                </div>
                                <button type="button" class="btn-close" @click="closeModal"></button>
                            </div>

                            <div class="modal-body pt-3">
                                <div class="row g-3">
                                    <div class="col-12 col-md-3">
                                        <label class="form-label">
                                            Chỉ tiêu cha <span class="text-danger">*</span>
                                        </label>
                                        <select v-model.number="form.chiTietGiaoChaId" class="form-select"
                                            @change="syncFromParent">
                                            <option :value="null">Chọn chỉ tiêu cha</option>
                                            <option v-for="item in eligibleParentOptions" :key="getId(item)"
                                                :value="getId(item)">
                                                {{ getParentDisplay(item) }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">
                                            Đơn vị nhận <span class="text-danger">*</span>
                                        </label>
                                        <select v-model.number="form.donViNhanId" class="form-select">
                                            <option :value="null">Chọn đơn vị nhận</option>
                                            <option v-for="item in donViOptions" :key="getId(item)"
                                                :value="getId(item)">
                                                {{ item.TenDonVi || item.tenDonVi || '-' }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">
                                            Đơn vị thực hiện chính <span class="text-danger">*</span>
                                        </label>
                                        <select v-model.number="form.donViThucHienChinhId" class="form-select">
                                            <option :value="null">Chọn đơn vị thực hiện chính</option>
                                            <option v-for="item in donViOptions" :key="getId(item)"
                                                :value="getId(item)">
                                                {{ item.TenDonVi || item.tenDonVi || '-' }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">
                                            Tần suất báo cáo <span class="text-danger">*</span>
                                        </label>
                                        <input :value="displayTanSuatBaoCao" type="text" class="form-control"
                                            readonly />
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
                                            Giá trị mục tiêu phân rã <span class="text-danger">*</span>
                                        </label>
                                        <div class="input-group">
                                            <input v-model="form.giaTriMucTieu" type="number" step="any"
                                                class="form-control" placeholder="Nhập giá trị mục tiêu phân rã" />
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
                                    {{ saving ? 'Đang lưu...' : isEdit ? 'Cập nhật' : 'Lưu phân rã chỉ tiêu' }}
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
import ColumnVisibilityTools from '../shared/ColumnVisibilityTools.vue'
    import { apiRequest } from '../../services/api.js'

    const loading = ref(false)
    const saving = ref(false)
    const showModal = ref(false)
    const isEdit = ref(false)
    const editingId = ref(null)

    const chiTietGiaoItems = ref([])
    const danhMucChiTieuItems = ref([])
    const dotGiaoItems = ref([])
    const donViItems = ref([])

    const filters = reactive({
        keyword: '',
        dotGiaoChiTieuId: '',
        chiTietGiaoChaId: '',
        donViNhanId: ''
    })

    const createDefaultForm = () => ({
        chiTietGiaoChaId: null,
        dotGiaoChiTieuId: null,
        danhMucChiTieuId: null,
        donViNhanId: null,
        donViThucHienChinhId: null,
        tanSuatBaoCao: '',
        giaTriMucTieu: '',
        ghiChu: ''
    })

    const form = reactive(createDefaultForm())

    const resetForm = () => {
        Object.assign(form, createDefaultForm())
    }

    const getId = item => item?.Id ?? item?.id ?? null

    const dotOptions = computed(() => dotGiaoItems.value || [])
    const donViOptions = computed(() => donViItems.value || [])

    const eligibleDanhMucOptions = computed(() => {
        return danhMucChiTieuItems.value.filter(
            x => !!(x.coChoPhepPhanRa ?? x.CoChoPhepPhanRa)
        )
    })

    const eligibleDanhMucIdSet = computed(() => {
        return new Set(eligibleDanhMucOptions.value.map(x => Number(getId(x))))
    })

    const eligibleParentOptions = computed(() => {
        return chiTietGiaoItems.value.filter(item => {
            const danhMucId = Number(item.danhMucChiTieuId ?? item.DanhMucChiTieuId)
            return eligibleDanhMucIdSet.value.has(danhMucId)
        })
    })

    const selectedParent = computed(() => {
        return eligibleParentOptions.value.find(x => Number(getId(x)) === Number(form.chiTietGiaoChaId)) || null
    })

    const selectedDanhMuc = computed(() => {
        if (!selectedParent.value) return null
        return danhMucChiTieuItems.value.find(
            x => Number(getId(x)) === Number(selectedParent.value.danhMucChiTieuId ?? selectedParent.value.DanhMucChiTieuId)
        ) || null
    })

    const displayDonViTinh = computed(() => {
        return selectedDanhMuc.value?.donViTinh || selectedDanhMuc.value?.DonViTinh || ''
    })

    const displayGiaTriMacDinh = computed(() => {
        const value = selectedParent.value?.giaTriMucTieu ?? selectedParent.value?.GiaTriMucTieu
        if (value === null || value === undefined || value === '') return ''
        return Number(value).toLocaleString('vi-VN')
    })

    const displayTanSuatBaoCao = computed(() => {
        const value = selectedParent.value?.tanSuatBaoCao || selectedParent.value?.TanSuatBaoCao || ''
        return mapTanSuat(value)
    })

    const decomposedItems = computed(() => {
        return chiTietGiaoItems.value.filter(item => {
            const parentId = Number(item.chiTietGiaoChaId ?? item.ChiTietGiaoChaId)
            if (!parentId) return false

            const parent = chiTietGiaoItems.value.find(x => Number(getId(x)) === parentId)
            if (!parent) return false

            const parentDanhMucId = Number(parent.danhMucChiTieuId ?? parent.DanhMucChiTieuId)
            return eligibleDanhMucIdSet.value.has(parentDanhMucId)
        })
    })

    const filteredItems = computed(() => {
        const keyword = filters.keyword.trim().toLowerCase()

        return decomposedItems.value.filter(item => {
            const itemDotGiaoId = String(item.dotGiaoChiTieuId ?? item.DotGiaoChiTieuId ?? '')
            const itemParentId = String(item.chiTietGiaoChaId ?? item.ChiTietGiaoChaId ?? '')
            const itemDonViNhanId = String(item.donViNhanId ?? item.DonViNhanId ?? '')

            const parentText = getParentName(item.chiTietGiaoChaId ?? item.ChiTietGiaoChaId).toLowerCase()
            const donViNhanText = getDonViLabel(item.donViNhanId ?? item.DonViNhanId).toLowerCase()
            const donViThucHienText = getDonViLabel(item.donViThucHienChinhId ?? item.DonViThucHienChinhId).toLowerCase()
            const ghiChu = String(item.ghiChu ?? item.GhiChu ?? '').toLowerCase()

            const matchKeyword =
                !keyword ||
                parentText.includes(keyword) ||
                donViNhanText.includes(keyword) ||
                donViThucHienText.includes(keyword) ||
                ghiChu.includes(keyword)

            const matchDotGiao = !filters.dotGiaoChiTieuId || itemDotGiaoId === String(filters.dotGiaoChiTieuId)
            const matchParent = !filters.chiTietGiaoChaId || itemParentId === String(filters.chiTietGiaoChaId)
            const matchDonViNhan = !filters.donViNhanId || itemDonViNhanId === String(filters.donViNhanId)

            return matchKeyword && matchDotGiao && matchParent && matchDonViNhan
        })
    })

    const fetchAllData = async () => {
        try {
            loading.value = true

            const [chiTietGiao, danhMuc, dotGiao, donVi] = await Promise.all([
                apiRequest('/ChiTietGiaoChiTieu'),
                apiRequest('/danh-muc-chi-tieu'),
                apiRequest('/dot-giao-chi-tieu'),
                apiRequest('/DonVi')
            ])

            chiTietGiaoItems.value = Array.isArray(chiTietGiao) ? chiTietGiao : []
            danhMucChiTieuItems.value = Array.isArray(danhMuc) ? danhMuc : []
            dotGiaoItems.value = Array.isArray(dotGiao) ? dotGiao : []
            donViItems.value = Array.isArray(donVi) ? donVi : []
        } catch (error) {
            console.error(error)
            alert(error.message || 'Không tải được dữ liệu phân rã chỉ tiêu.')
            chiTietGiaoItems.value = []
            danhMucChiTieuItems.value = []
            dotGiaoItems.value = []
            donViItems.value = []
        } finally {
            loading.value = false
        }
    }

    const applyFilters = async () => {
    }

    const resetFilters = async () => {
        filters.keyword = ''
        filters.dotGiaoChiTieuId = ''
        filters.chiTietGiaoChaId = ''
        filters.donViNhanId = ''
    }

    const syncFromParent = () => {
        if (!selectedParent.value) {
            form.dotGiaoChiTieuId = null
            form.danhMucChiTieuId = null
            form.tanSuatBaoCao = ''
            return
        }

        form.dotGiaoChiTieuId = selectedParent.value.dotGiaoChiTieuId ?? selectedParent.value.DotGiaoChiTieuId ?? null
        form.danhMucChiTieuId = selectedParent.value.danhMucChiTieuId ?? selectedParent.value.DanhMucChiTieuId ?? null
        form.tanSuatBaoCao = selectedParent.value.tanSuatBaoCao ?? selectedParent.value.TanSuatBaoCao ?? ''
    }

    const openCreateModal = () => {
        isEdit.value = false
        editingId.value = null
        resetForm()
        showModal.value = true
    }

    const openEditModal = item => {
        isEdit.value = true
        editingId.value = getId(item)

        Object.assign(form, {
            chiTietGiaoChaId: item.chiTietGiaoChaId ?? item.ChiTietGiaoChaId ?? null,
            dotGiaoChiTieuId: item.dotGiaoChiTieuId ?? item.DotGiaoChiTieuId ?? null,
            danhMucChiTieuId: item.danhMucChiTieuId ?? item.DanhMucChiTieuId ?? null,
            donViNhanId: item.donViNhanId ?? item.DonViNhanId ?? null,
            donViThucHienChinhId: item.donViThucHienChinhId ?? item.DonViThucHienChinhId ?? null,
            tanSuatBaoCao: item.tanSuatBaoCao ?? item.TanSuatBaoCao ?? '',
            giaTriMucTieu: item.giaTriMucTieu ?? item.GiaTriMucTieu ?? '',
            ghiChu: item.ghiChu ?? item.GhiChu ?? ''
        })

        showModal.value = true
    }

    const closeModal = () => {
        showModal.value = false
        resetForm()
    }

    const validateForm = () => {
        if (!form.chiTietGiaoChaId) {
            alert('Vui lòng chọn chỉ tiêu cha.')
            return false
        }

        if (!form.donViNhanId) {
            alert('Vui lòng chọn đơn vị nhận.')
            return false
        }

        if (!form.donViThucHienChinhId) {
            alert('Vui lòng chọn đơn vị thực hiện chính.')
            return false
        }

        if (!form.giaTriMucTieu && form.giaTriMucTieu !== 0) {
            alert('Vui lòng nhập giá trị mục tiêu phân rã.')
            return false
        }

        return true
    }

    const buildPayload = () => {
        return {
            chiTietGiaoChaId: form.chiTietGiaoChaId,
            dotGiaoChiTieuId: form.dotGiaoChiTieuId,
            danhMucChiTieuId: form.danhMucChiTieuId,
            donViNhanId: form.donViNhanId,
            donViThucHienChinhId: form.donViThucHienChinhId,
            tanSuatBaoCao: form.tanSuatBaoCao || null,
            giaTriMucTieu: form.giaTriMucTieu !== '' && form.giaTriMucTieu !== null ? Number(form.giaTriMucTieu) : null,
            ghiChu: form.ghiChu?.trim() || null
        }
    }

    const handleSubmit = async () => {
        if (!validateForm()) return

        try {
            saving.value = true
            const payload = buildPayload()

            if (isEdit.value && editingId.value) {
                await apiRequest(`/ChiTietGiaoChiTieu/${editingId.value}`, 'PUT', payload)
            } else {
                await apiRequest('/ChiTietGiaoChiTieu', 'POST', payload)
            }

            closeModal()
            await fetchAllData()
        } catch (error) {
            console.error(error)
            alert(error.message || 'Lưu phân rã chỉ tiêu thất bại.')
        } finally {
            saving.value = false
        }
    }

    const handleDelete = async item => {
        const ok = window.confirm('Bạn có chắc muốn xóa bản ghi phân rã này không?')
        if (!ok) return

        try {
            await apiRequest(`/ChiTietGiaoChiTieu/${getId(item)}`, 'DELETE')
            await fetchAllData()
        } catch (error) {
            console.error(error)
            alert(error.message || 'Xóa phân rã chỉ tiêu thất bại.')
        }
    }

    const getDotGiaoLabel = id => {
        const item = dotGiaoItems.value.find(x => Number(getId(x)) === Number(id))
        if (!item) return ''
        return item.TenDotGiao || item.tenDotGiao || ''
    }

    const getDonViLabel = id => {
        const item = donViItems.value.find(x => Number(getId(x)) === Number(id))
        return item?.TenDonVi || item?.tenDonVi || ''
    }

    const getDanhMucById = id => {
        return danhMucChiTieuItems.value.find(x => Number(getId(x)) === Number(id)) || null
    }

    const getDanhMucLabel = id => {
        const item = getDanhMucById(id)
        if (!item) return ''
        const ma = item.MaChiTieu || item.maChiTieu || ''
        const ten = item.TenChiTieu || item.tenChiTieu || ''
        return `${ma} - ${ten}`.trim()
    }

    const getDonViTinhByDanhMuc = id => {
        const item = getDanhMucById(id)
        return item?.DonViTinh || item?.donViTinh || ''
    }

    const getParentDisplay = item => {
        const danhMucLabel = getDanhMucLabel(item.danhMucChiTieuId ?? item.DanhMucChiTieuId)
        const donViLabel = getDonViLabel(item.donViNhanId ?? item.DonViNhanId)
        const tanSuat = mapTanSuat(item.tanSuatBaoCao ?? item.TanSuatBaoCao)
        return `${danhMucLabel} / ${donViLabel} / ${tanSuat}`
    }

    const getParentName = id => {
        const parent = chiTietGiaoItems.value.find(x => Number(getId(x)) === Number(id))
        if (!parent) return ''
        return getParentDisplay(parent)
    }

    const getParentGiaTri = id => {
        const parent = chiTietGiaoItems.value.find(x => Number(getId(x)) === Number(id))
        if (!parent) return ''
        return formatNumber(parent.giaTriMucTieu ?? parent.GiaTriMucTieu)
    }

    const mapTanSuat = value => {
        const map = {
            NGAY: 'Ngày',
            TUAN: 'Tuần',
            THANG: 'Tháng',
            QUY: 'Quý',
            SAU_THANG: '6 tháng',
            NAM: 'Năm'
        }
        return map[value] || value || ''
    }

    const formatNumber = value => {
        if (value === null || value === undefined || value === '') return ''
        const num = Number(value)
        if (Number.isNaN(num)) return value
        return num.toLocaleString('vi-VN')
    }

    onMounted(() => {
        fetchAllData()
    })
</script>

<style scoped>
    .page-wrap {
        background: #f6f8fb;
        min-height: 100vh;
    }

    .gov-banner {
        display: flex;
        align-items: center;
        gap: 16px;
        padding: 18px 22px;
        border-radius: 18px;
        background: linear-gradient(135deg, #ffffff 0%, #f6fbff 100%);
        border: 1px solid rgba(137, 210, 239, 0.35);
        box-shadow: 0 10px 30px rgba(15, 23, 42, 0.06);
    }

    .gov-emblem {
        width: 64px;
        height: 64px;
        min-width: 64px;
        border-radius: 18px;
        display: grid;
        place-items: center;
        background: linear-gradient(135deg, #e8f7ff 0%, #dff4ff 100%);
        color: #0d6efd;
        font-size: 28px;
        object-fit: contain;
    }

    .gov-text {
        flex: 1;
    }

    .wave-title {
        font-size: 12px;
        font-weight: 700;
        letter-spacing: 1.4px;
        color: #0d6efd;
        margin-bottom: 4px;
        text-transform: uppercase;
    }

    .gov-title {
        font-size: 26px;
        font-weight: 800;
        color: #1f2937;
        line-height: 1.2;
    }

    .gov-sub {
        font-size: 14px;
        color: #6b7280;
        margin-top: 6px;
    }

    .custom-card {
        border: none;
        border-radius: 20px;
        overflow: hidden;
        box-shadow: 0 10px 28px rgba(15, 23, 42, 0.06);
    }

    :deep(.table) {
        border-collapse: collapse !important;
    }

    :deep(.table thead th) {
        background: #fff;
        border-bottom: 2px solid #dee2e6 !important;
        white-space: nowrap;
        font-weight: 700;
        color: #334155;
    }

    :deep(.table td),
    :deep(.table th) {
        padding: 14px 16px;
        border-right: 1px solid #eee;
        vertical-align: middle;
    }

    :deep(.table td:last-child),
    :deep(.table th:last-child) {
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
        text-align: center;
        color: #64748b;
    }

    .custom-modal {
        background: rgba(137, 210, 239, 0.5);
    }

    .modal-content {
        animation: fadeInUp 0.3s ease;
    }

    .btn-action {
        min-width: 170px;
    }

    @keyframes fadeInUp {
        from {
            opacity: 0;
            transform: translate3d(0, 18px, 0);
        }

        to {
            opacity: 1;
            transform: translate3d(0, 0, 0);
        }
    }
</style>





