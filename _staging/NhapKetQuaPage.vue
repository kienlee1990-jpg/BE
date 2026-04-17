<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-clipboard-data"></i>
                    </div>
                    <div class="gov-text">
                        <div class="wave-title">HỆ THỐNG THEO DÕI CHỈ TIÊU CÔNG TÁC</div>
                        <div class="gov-title">NHẬP BÁO CÁO ĐỊNH KỲ</div>
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
                        Nhập báo cáo định kỳ
                    </button>
                </div>

                <div class="card custom-card mb-4">
                    <div class="card-header bg-white border-0 pb-0">
                        <h5 class="mb-1">Bộ lọc tìm kiếm</h5>
                        <small class="text-muted">Tra cứu nhanh dữ liệu báo cáo định kỳ</small>
                    </div>

                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-12 col-md-4">
                                <label class="form-label">Kỳ báo cáo</label>
                                <select v-model.number="filters.kyBaoCaoKPIId" class="form-select">
                                    <option :value="null">Tất cả</option>
                                    <option v-for="item in kyBaoCaoOptions" :key="getId(item)" :value="getId(item)">
                                        {{ getKyBaoCaoDisplay(item) }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-4">
                                <label class="form-label">Đơn vị thực hiện</label>
                                <select v-model.number="filters.donViNhanId" class="form-select">
                                    <option :value="null">Tất cả</option>
                                    <option v-for="item in donViNhanFilterOptions" :key="item.id" :value="item.id">
                                        {{ item.ten }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-4">
                                <label class="form-label">Chỉ tiêu giao</label>
                                <select v-model.number="filters.chiTietGiaoChiTieuId" class="form-select">
                                    <option :value="null">Tất cả</option>
                                    <option v-for="item in filteredChiTietGiaoFilterOptions" :key="getId(item)"
                                        :title="getChiTietOptionLabel(item)"
                                        :value="getId(item)">
                                        {{ getChiTietOptionLabel(item) }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12">
                                <label class="form-label">Từ khóa</label>
                                <input v-model="filters.keyword" type="text" class="form-control"
                                    placeholder="Mã kỳ, mã chỉ tiêu..." />
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
                            <h5 class="mb-1">Danh sách báo cáo định kỳ</h5>
                            <small class="text-muted">Số liệu thực hiện được cập nhật theo từng kỳ báo cáo</small>
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
                                        <th>Kỳ báo cáo</th>
                                        <th>Chỉ tiêu giao</th>
                                        <th>Đầu kỳ</th>
                                        <th>Thực hiện trong kỳ</th>
                                        <th>Cuối kỳ</th>
                                        <th>Lũy kế</th>
                                        <th class="text-center" style="width: 180px">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="item in filteredItems" :key="getId(item)">
                                        <td>
                                            <div class="fw-semibold">
                                                {{ item.TenKyHienThi }}
                                            </div>
                                            <small class="text-muted">
                                                {{ item.MaKy || item.maKy || '-' }}
                                            </small>
                                        </td>
                                        <td>
                                            <div class="fw-semibold text-primary">
                                                {{ item.TenChiTieu || item.tenChiTieu || '-' }}
                                            </div>
                                            <small class="text-muted">
                                                {{ item.MaChiTieu || item.maChiTieu || '' }}
                                                <span v-if="item.TenDonViNhan || item.tenDonViNhan">
                                                    - {{ item.TenDonViNhan || item.tenDonViNhan }}
                                                </span>
                                            </small>
                                            <small v-if="item.NhanXet || item.nhanXet" class="d-block text-muted">
                                                Kết quả nhập: {{ item.NhanXet || item.nhanXet }}
                                            </small>
                                        </td>
                                        <td>{{ formatNumber(item.GiaTriDauKy ?? item.giaTriDauKy) }}</td>
                                        <td>{{ formatNumber(item.GiaTriThucHienTrongKy ?? item.giaTriThucHienTrongKy) }}
                                        </td>
                                        <td>{{ formatNumber(item.GiaTriCuoiKy ?? item.giaTriCuoiKy) }}</td>
                                        <td>{{ formatNumber(item.GiaTriLuyKe ?? item.giaTriLuyKe) }}</td>
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
                                        {{ isEdit ? 'Cập nhật báo cáo định kỳ' : 'Nhập báo cáo định kỳ' }}
                                    </h4>
                                    <p class="text-muted mb-0">
                                        Chọn đơn vị, chỉ tiêu được giao, rồi nhập số liệu của kỳ báo cáo
                                    </p>
                                </div>
                                <button type="button" class="btn-close" @click="closeModal"></button>
                            </div>

                            <div class="modal-body pt-3">
                                <div class="row g-3">
                                    <div class="col-12">
                                        <div class="alert alert-info mb-0">
                                            <i class="bi bi-info-circle me-2"></i>
                                            Chỉ được chọn kỳ báo cáo đang mở và phù hợp với tần suất của chỉ tiêu giao.
                                            Chỉ tiêu phân rã chỉ nhập kết quả ở tiêu chí con. Giá trị đầu kỳ được giữ
                                            theo số gốc cố định, không cộng dồn sang các tháng hoặc kỳ sau.
                                        </div>
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">
                                            Đơn vị thực hiện <span class="text-danger">*</span>
                                        </label>
                                        <select v-model.number="form.donViNhanId" class="form-select">
                                            <option :value="null">Chọn đơn vị thực hiện</option>
                                            <option v-for="item in donViNhanFilterOptions" :key="item.id"
                                                :value="item.id">
                                                {{ item.ten }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">
                                            Chỉ tiêu giao <span class="text-danger">*</span>
                                        </label>
                                        <select v-model.number="form.chiTietGiaoChiTieuId" class="form-select">
                                            <option :value="null">Chọn chỉ tiêu giao</option>
                                            <option v-for="item in filteredChiTietGiaoModalOptions" :key="getId(item)"
                                                :title="getChiTietOptionLabel(item)"
                                                :value="getId(item)">
                                                {{ getChiTietOptionLabel(item) }}
                                            </option>
                                        </select>
                                        <div v-if="selectedChiTietGiao" class="selection-summary mt-2">
                                            <div class="selection-summary-title">
                                                {{ getChiTietSelectionTitle(selectedChiTietGiao) }}
                                            </div>
                                            <div class="selection-summary-highlight">
                                                Tiêu chí giao: {{ getCriterionDisplay(selectedChiTietGiao) }}
                                            </div>
                                            <div class="selection-summary-meta">
                                                <span>Chỉ tiêu: {{ getChiTietDisplayWithoutDonVi(selectedChiTietGiao) }}</span>
                                                <span>Tần suất: {{ mapTanSuat(getTanSuatBaoCao(selectedChiTietGiao)) }}</span>
                                            </div>
                                            <div class="selection-summary-meta">
                                                <span>Đơn vị: {{ selectedChiTietGiao.TenDonViNhanHienThi || '-' }}</span>
                                                <span>Đợt giao: {{ getDotGiaoDisplay(selectedChiTietGiao) }}</span>
                                            </div>
                                            <div v-if="selectedChiTietGiao.LaTieuChiCon" class="selection-summary-meta">
                                                <span>Thuộc chỉ tiêu cha: {{ getParentChiTietDisplay(selectedChiTietGiao) }}</span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">
                                            Kỳ báo cáo <span class="text-danger">*</span>
                                        </label>
                                        <select v-model.number="form.kyBaoCaoKPIId" class="form-select">
                                            <option :value="null">Chọn kỳ báo cáo</option>
                                            <option v-for="item in filteredKyBaoCaoOptions" :key="getId(item)"
                                                :value="getId(item)">
                                                {{ getKyBaoCaoDisplay(item) }}
                                            </option>
                                        </select>
                                        <small class="text-muted" v-if="selectedChiTietGiao">
                                            Chỉ hiển thị các kỳ đang mở, đúng kỳ báo cáo đã giao và có ngày cuối kỳ nằm
                                            trong khoảng thời gian của đợt giao chỉ tiêu.
                                        </small>
                                    </div>

                                    <div v-if="form.chiTietGiaoChiTieuId && form.kyBaoCaoKPIId && !nopDuocTheoNgayKetThucDotGiao"
                                        class="col-12">
                                        <div class="alert alert-warning mb-0">
                                            <i class="bi bi-exclamation-triangle me-2"></i>
                                            Chỉ được nộp khi ngày cuối kỳ của kỳ báo cáo lớn hơn hoặc bằng ngày bắt đầu
                                            và nhỏ hơn hoặc bằng ngày kết thúc của đợt giao chỉ tiêu.
                                        </div>
                                    </div>

                                    <div v-if="isDinhTinh" class="col-12">
                                        <label class="form-label">
                                            Kết quả đánh giá định tính <span class="text-danger">*</span>
                                        </label>
                                        <select v-model="form.nhanXet" class="form-select">
                                            <option value="">Chọn kết quả đánh giá</option>
                                            <option v-for="option in qualitativeOptions" :key="option.value"
                                                :value="option.value">
                                                {{ option.label }}
                                            </option>
                                        </select>
                                        <small class="text-muted d-block mt-1">
                                            Các lựa chọn này được dùng trực tiếp để xác định trạng thái hoàn thành của
                                            chỉ tiêu định tính.
                                        </small>
                                    </div>

                                    <template v-else>
                                        <div v-if="isDinhLuongSoSanh" class="col-12">
                                            <div class="alert alert-secondary mb-0">
                                                <i class="bi bi-graph-up-arrow me-2"></i>
                                                Hệ thống sẽ tự tính tỷ lệ hoàn thành dựa trên kết quả kỳ này, mốc so
                                                sánh
                                                <strong>{{ comparisonSourceLabel }}</strong> và chiều so sánh
                                                <strong>{{ comparisonDirectionLabel }}</strong> theo quy tắc
                                                <strong>{{ comparisonRuleLabel }}</strong>.
                                            </div>
                                        </div>

                                        <div v-else class="col-12">
                                            <div class="alert alert-secondary mb-0">
                                                <i class="bi bi-speedometer2 me-2"></i>
                                                Hệ thống sẽ đánh giá theo kết quả lũy kế, chiều đánh giá
                                                <strong>{{ comparisonDirectionLabel }}</strong> và quy tắc
                                                <strong>{{ comparisonRuleLabel }}</strong>.
                                            </div>
                                        </div>

                                        <div class="col-12 col-md-4">
                                            <label class="form-label">
                                                Giá trị đầu kỳ cố định
                                            </label>
                                            <input :value="formatEditableNumber(fixedGiaTriDauKy)" type="text"
                                                class="form-control" readonly />
                                            <small class="text-muted">
                                                Giá trị này lấy từ bản giao chỉ tiêu và được giữ cố định cho mọi kỳ báo cáo.
                                            </small>
                                        </div>

                                        <div class="col-12 col-md-4">
                                            <label class="form-label">
                                                {{ currentValueLabel }} <span class="text-danger">*</span>
                                            </label>
                                            <input v-model.number="form.giaTriThucHienTrongKy" type="number" step="0.01"
                                                min="0" class="form-control" :placeholder="currentValuePlaceholder" />
                                        </div>

                                        <div class="col-12 col-md-4">
                                            <label class="form-label">Giá trị cuối kỳ</label>
                                            <input :value="formatEditableNumber(giaTriCuoiKyPreview)" type="text"
                                                class="form-control" readonly />
                                        </div>
                                    </template>

                                    <div v-if="!isDinhTinh" class="col-12">
                                        <label class="form-label">Ghi chú báo cáo</label>
                                        <textarea v-model="form.nhanXet" class="form-control"
                                            placeholder="Nhập ghi chú hoặc diễn giải thêm nếu cần"></textarea>
                                    </div>
                                </div>
                            </div>

                            <div class="modal-footer border-0 pt-0">
                                <button class="btn btn-light" @click="closeModal">Hủy</button>
                                <button class="btn btn-primary" :disabled="saving" @click="handleSubmit">
                                    <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                                    {{ saving ? 'Đang lưu...' : isEdit ? 'Cập nhật' : 'Lưu báo cáo định kỳ' }}
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
    import { computed, nextTick, onMounted, reactive, ref, watch } from 'vue'
    import BaseLayout from '../BaseLayout.vue'
    import httpClient from '../../services/httpClient'

    const API_PATHS = {
        theoDoiThucHienKPI: '/TheoDoiThucHienKPI',
        chiTietGiaoChiTieu: '/ChiTietGiaoChiTieu',
        kyBaoCaoKPI: '/KyBaoCaoKPI',
        danhMucChiTieu: '/danh-muc-chi-tieu',
        donVi: '/DonVi',
        dotGiaoChiTieu: '/dot-giao-chi-tieu'
    }

    const loading = ref(false)
    const saving = ref(false)
    const showModal = ref(false)
    const isEdit = ref(false)
    const editingId = ref(null)
    const isHydratingForm = ref(false)

    const items = ref([])
    const chiTietGiaoChiTieuOptions = ref([])
    const kyBaoCaoOptions = ref([])
    const danhMucChiTieuOptions = ref([])
    const donViOptions = ref([])
    const dotGiaoChiTieuOptions = ref([])

    const filters = reactive({
        kyBaoCaoKPIId: null,
        donViNhanId: null,
        chiTietGiaoChiTieuId: null,
        keyword: ''
    })

    const createDefaultForm = () => ({
        donViNhanId: null,
        chiTietGiaoChiTieuId: null,
        kyBaoCaoKPIId: null,
        giaTriThucHienTrongKy: 0,
        nhanXet: ''
    })

    const form = reactive(createDefaultForm())

    const resetForm = () => {
        Object.assign(form, createDefaultForm())
    }

    const hydrateFormSafely = async (nextState) => {
        isHydratingForm.value = true
        Object.assign(form, nextState)
        await nextTick()
        isHydratingForm.value = false
    }

    const getId = (item) => Number(item?.Id ?? item?.id ?? 0)

    const normalizeList = (response) => {
        if (Array.isArray(response?.data)) return response.data
        if (Array.isArray(response?.data?.data)) return response.data.data
        if (Array.isArray(response?.data?.items)) return response.data.items
        return []
    }

    const getKyBaoCaoDisplay = (item) => {
        return item?.TenKy || item?.tenKy || item?.TenKyBaoCao || item?.tenKyBaoCao || '-'
    }

    const getTanSuatBaoCao = (item) => {
        return item?.TanSuatBaoCao || item?.tanSuatBaoCao || ''
    }

    const getLoaiKy = (item) => {
        return item?.LoaiKy || item?.loaiKy || item?.LoaiKyBaoCao || item?.loaiKyBaoCao || ''
    }

    const getThuTuKy = (item) => {
        return Number(item?.ThuTuKy ?? item?.thuTuKy ?? item?.ThuTu ?? item?.thuTu ?? item?.SoKy ?? item?.soKy ?? 0)
    }

    const getTrangThaiKy = (item) => {
        return item?.TrangThai || item?.trangThai || item?.TrangThaiKy || item?.trangThaiKy || ''
    }

    const getDanhMucChiTieuId = (item) => {
        return Number(item?.DanhMucChiTieuId ?? item?.danhMucChiTieuId ?? 0)
    }

    const normalizeCode = (value) => String(value || '').trim().toUpperCase()

    const getLoaiChiTieu = (item) => {
        return normalizeCode(
            item?.TieuChiDanhGia ||
            item?.tieuChiDanhGia ||
            item?.LoaiChiTieu ||
            item?.loaiChiTieu
        )
    }

    const getDonViNhanId = (item) => {
        return Number(item?.DonViNhanId ?? item?.donViNhanId ?? 0)
    }

    const getDotGiaoChiTieuId = (item) => {
        return Number(item?.DotGiaoChiTieuId ?? item?.dotGiaoChiTieuId ?? 0)
    }

    const getTieuChiCon = (item) => {
        const children = item?.TieuChiCon ?? item?.tieuChiCon ?? []
        return Array.isArray(children) ? children : []
    }

    const hasChildCriteria = (item) => {
        const coTieuChiCon = item?.CoTieuChiCon ?? item?.coTieuChiCon
        if (typeof coTieuChiCon === 'boolean') return coTieuChiCon
        return getTieuChiCon(item).length > 0
    }

    const getNgayBatDauDotGiao = (item) => {
        return item?.NgayBatDau ?? item?.ngayBatDau ?? null
    }

    const getNgayKetThucDotGiao = (item) => {
        return item?.NgayKetThuc ?? item?.ngayKetThuc ?? null
    }

    const getNgayCuoiKy = (item) => {
        return item?.NgayCuoiKy ?? item?.ngayCuoiKy ?? null
    }

    const getGiaTriDauKyCoDinhFromChiTiet = (item) => {
        const rawValue = item?.GiaTriDauKyCoDinh ?? item?.giaTriDauKyCoDinh
        if (rawValue === null || rawValue === undefined || rawValue === '') {
            return null
        }

        const numericValue = Number(rawValue)
        return Number.isNaN(numericValue) ? null : numericValue
    }

    const toDateOnly = (value) => {
        if (!value) return null
        const d = new Date(value)
        if (Number.isNaN(d.getTime())) return null
        return new Date(d.getFullYear(), d.getMonth(), d.getDate())
    }

    const flattenDanhMucChiTieuTree = (items, parent = null) => {
        const flattened = []

        items.forEach((item) => {
            const normalizedItem = {
                ...item,
                ChiTieuChaId:
                    item?.ChiTieuChaId ??
                    item?.chiTieuChaId ??
                    parent?.Id ??
                    parent?.id ??
                    null
            }

            flattened.push(normalizedItem)

            const children =
                item?.TieuChiDanhGias ??
                item?.tieuChiDanhGias ??
                item?.TieuChiCons ??
                item?.tieuChiCons ??
                []

            if (Array.isArray(children) && children.length > 0) {
                flattened.push(...flattenDanhMucChiTieuTree(children, normalizedItem))
            }
        })

        return flattened
    }

    const flattenedDanhMucChiTieuOptions = computed(() => {
        return flattenDanhMucChiTieuTree(danhMucChiTieuOptions.value)
    })

    const findDanhMucById = (id) => {
        return flattenedDanhMucChiTieuOptions.value.find((x) => getId(x) === Number(id)) || null
    }

    const findDonViById = (id) => {
        return donViOptions.value.find((x) => getId(x) === Number(id)) || null
    }

    const findDotGiaoById = (id) => {
        return dotGiaoChiTieuOptions.value.find((x) => getId(x) === Number(id)) || null
    }

    const mapTanSuat = (value) => {
        const map = {
            THANG: 'Tháng',
            QUY: 'Quý',
            '6THANG': '6 tháng',
            NAM: 'Năm'
        }
        return map[value] || value || '-'
    }

    const mapLoaiMocSoSanh = (value) => {
        const map = {
            DAU_KY: 'đầu kỳ',
            CUNG_KY: 'cùng kỳ năm trước',
            KY_TRUOC: 'kỳ trước',
            TONG_NAM_TRUOC: 'tổng năm trước'
        }

        return map[String(value || '').trim().toUpperCase()] || 'mốc cấu hình'
    }

    const mapChieuSoSanh = (value) => {
        const map = {
            TANG: 'tăng',
            GIAM: 'giảm'
        }

        return map[String(value || '').trim().toUpperCase()] || 'so sánh'
    }

    const mapQuyTacDanhGia = (value) => {
        const map = {
            MAC_DINH: 'mặc định',
            DAT_TOI_THIEU: 'đạt tối thiểu',
            KHONG_VUOT_NGUONG: 'không vượt ngưỡng'
        }

        return map[String(value || '').trim().toUpperCase()] || 'đạt tối thiểu'
    }

    const qualitativeOptions = [
        { value: 'KHONG_XAY_RA', label: 'Không xảy ra' },
        { value: 'DAM_BAO', label: 'Đảm bảo' },
        { value: 'DAT_100', label: 'Đạt 100%' },
        { value: 'XAY_RA', label: 'Xảy ra' },
        { value: 'KHONG_DAM_BAO', label: 'Không đảm bảo' },
        { value: 'KHONG_DAT', label: 'Không đạt' }
    ]

    const isKyDangMo = (item) => {
        return getTrangThaiKy(item) === 'DANG_MO'
    }

    const isKyPhuHopTanSuat = (kyItem, tanSuatBaoCao) => {
        if (!tanSuatBaoCao) return true

        const loaiKy = getLoaiKy(kyItem)

        const map = {
            THANG: ['THANG'],
            QUY: ['QUY'],
            '6THANG': ['6THANG', 'SAU_THANG', 'BAN_NIEN'],
            NAM: ['NAM']
        }

        return (map[tanSuatBaoCao] || []).includes(loaiKy)
    }

    const isChiTietHopLeTheoNgayKetThuc = (chiTiet, kyBaoCao) => {
        if (!chiTiet || !kyBaoCao) return false

        const dotGiaoId = getDotGiaoChiTieuId(chiTiet)
        if (!dotGiaoId) return false

        const dotGiao = findDotGiaoById(dotGiaoId)
        if (!dotGiao) return false

        const ngayBatDauDotGiao = toDateOnly(getNgayBatDauDotGiao(dotGiao))
        const ngayKetThucDotGiao = toDateOnly(getNgayKetThucDotGiao(dotGiao))
        const ngayCuoiKy = toDateOnly(getNgayCuoiKy(kyBaoCao))

        if (!ngayBatDauDotGiao || !ngayKetThucDotGiao || !ngayCuoiKy) return false

        return ngayCuoiKy >= ngayBatDauDotGiao && ngayCuoiKy <= ngayKetThucDotGiao
    }

    const enrichedChiTietGiaoChiTieuOptions = computed(() => {
        const enrichItem = (item, parent = null) => {
            const danhMuc = findDanhMucById(getDanhMucChiTieuId(item))
            const donVi = findDonViById(getDonViNhanId(item) || getDonViNhanId(parent))
            const parentDanhMuc = parent ? findDanhMucById(getDanhMucChiTieuId(parent)) : null
            const dotGiao = findDotGiaoById(getDotGiaoChiTieuId(item) || getDotGiaoChiTieuId(parent))

            return {
                ...item,
                MaChiTieuHienThi:
                    item.MaChiTieu ||
                    item.maChiTieu ||
                    item.MaDanhMucChiTieu ||
                    item.maDanhMucChiTieu ||
                    danhMuc?.MaChiTieu ||
                    danhMuc?.maChiTieu ||
                    '',
                TenChiTieuHienThi:
                    item.TenChiTieu ||
                    item.tenChiTieu ||
                    item.TenDanhMucChiTieu ||
                    item.tenDanhMucChiTieu ||
                    danhMuc?.TenChiTieu ||
                    danhMuc?.tenChiTieu ||
                    '',
                TenDonViNhanHienThi:
                    item.TenDonViNhan ||
                    item.tenDonViNhan ||
                    donVi?.TenDonVi ||
                    donVi?.tenDonVi ||
                    '',
                MaDotGiaoHienThi:
                    item.MaDotGiaoChiTieu ||
                    item.maDotGiaoChiTieu ||
                    item.MaDotGiao ||
                    item.maDotGiao ||
                    dotGiao?.MaDotGiao ||
                    dotGiao?.maDotGiao ||
                    '',
                TenDotGiaoHienThi:
                    item.TenDotGiaoChiTieu ||
                    item.tenDotGiaoChiTieu ||
                    item.TenDotGiao ||
                    item.tenDotGiao ||
                    dotGiao?.TenDotGiao ||
                    dotGiao?.tenDotGiao ||
                    '',
                LaTieuChiCon: !!parent,
                MaChiTieuChaHienThi:
                    parent?.MaChiTieu ||
                    parent?.maChiTieu ||
                    parentDanhMuc?.MaChiTieu ||
                    parentDanhMuc?.maChiTieu ||
                    '',
                TenChiTieuChaHienThi:
                    parent?.TenChiTieu ||
                    parent?.tenChiTieu ||
                    parent?.TenDanhMucChiTieu ||
                    parent?.tenDanhMucChiTieu ||
                    parentDanhMuc?.TenChiTieu ||
                    parentDanhMuc?.tenChiTieu ||
                    parent?.TenChiTieuHienThi ||
                    parent?.tenChiTieuHienThi ||
                    ''
            }
        }

        const flattenedItems = []

        chiTietGiaoChiTieuOptions.value.forEach((item) => {
            flattenedItems.push(enrichItem(item))

            getTieuChiCon(item).forEach((child) => {
                flattenedItems.push(enrichItem(child, item))
            })
        })

        return flattenedItems
    })

    const leafChiTietGiaoOptions = computed(() => {
        return enrichedChiTietGiaoChiTieuOptions.value.filter((item) => !hasChildCriteria(item))
    })

    const getParentChiTietDisplay = (item) => {
        const maCha = item?.MaChiTieuChaHienThi || ''
        const tenCha = item?.TenChiTieuChaHienThi || ''
        return [maCha, tenCha].filter(Boolean).join(' - ') || '-'
    }

    const getDotGiaoDisplay = (item) => {
        const maDot = item?.MaDotGiaoHienThi || ''
        const tenDot = item?.TenDotGiaoHienThi || ''
        return [maDot, tenDot].filter(Boolean).join(' - ') || '-'
    }

    const getChiTietDisplayWithoutDonVi = (item) => {
        const ma = item?.MaChiTieuHienThi || ''
        const ten = item?.TenChiTieuHienThi || ''
        const mainDisplay = [ma, ten].filter(Boolean).join(' - ')

        if (item?.LaTieuChiCon) {
            const parentDisplay = getParentChiTietDisplay(item)
            return parentDisplay !== '-' ? `${mainDisplay} (thuộc ${parentDisplay})` : mainDisplay || '-'
        }

        return mainDisplay || '-'
    }

    const getChiTietSelectionTitle = (item) => {
        if (!item) return '-'

        const ma = item?.MaChiTieuHienThi || ''
        const ten = item?.TenChiTieuHienThi || ''

        if (item?.LaTieuChiCon) {
            return [`Tiêu chí con: ${ten || '-'}`, ma ? `(${ma})` : ''].filter(Boolean).join(' ')
        }

        return [ma, ten].filter(Boolean).join(' - ') || '-'
    }

    const getCriterionDisplay = (item) => {
        if (!item) return '-'

        const danhMuc = findDanhMucById(getDanhMucChiTieuId(item))
        const tenDanhMucCon =
            danhMuc?.TenChiTieu ||
            danhMuc?.tenChiTieu ||
            item?.TenDanhMucChiTieu ||
            item?.tenDanhMucChiTieu ||
            item?.TenChiTieuHienThi ||
            item?.tenChiTieuHienThi ||
            ''

        if (item?.LaTieuChiCon) {
            return tenDanhMucCon || '-'
        }

        return [item?.MaChiTieuHienThi || '', tenDanhMucCon || item?.TenChiTieuHienThi || '']
            .filter(Boolean)
            .join(' - ') || '-'
    }

    const getChiTietOptionLabel = (item) => {
        const mainDisplay = item?.LaTieuChiCon
            ? `Tiêu chí giao: ${getCriterionDisplay(item)}`
            : getChiTietDisplayWithoutDonVi(item)
        const metaParts = []

        if (item?.LaTieuChiCon) {
            metaParts.push(`Chỉ tiêu cha: ${getParentChiTietDisplay(item)}`)
        }

        const dotGiaoDisplay = getDotGiaoDisplay(item)
        if (dotGiaoDisplay !== '-') {
            metaParts.push(`Đợt giao: ${dotGiaoDisplay}`)
        }

        if (item?.TenDonViNhanHienThi) {
            metaParts.push(`Đơn vị: ${item.TenDonViNhanHienThi}`)
        }

        return metaParts.length ? `${mainDisplay} • ${metaParts.join(' • ')}` : mainDisplay
    }

    const donViNhanFilterOptions = computed(() => {
        const map = new Map()

        leafChiTietGiaoOptions.value.forEach((item) => {
            const id = getDonViNhanId(item)
            const ten = item.TenDonViNhanHienThi

            if (id > 0 && ten && !map.has(id)) {
                map.set(id, { id, ten })
            }
        })

        return Array.from(map.values()).sort((a, b) => a.ten.localeCompare(b.ten, 'vi'))
    })

    const filteredChiTietGiaoFilterOptions = computed(() => {
        const donViId = Number(filters.donViNhanId || 0)
        if (!donViId) return []

        return leafChiTietGiaoOptions.value.filter(
            (item) => getDonViNhanId(item) === donViId
        )
    })

    const filteredChiTietGiaoModalOptions = computed(() => {
        const donViId = Number(form.donViNhanId || 0)
        if (!donViId) return []

        return leafChiTietGiaoOptions.value.filter(
            (item) => getDonViNhanId(item) === donViId
        )
    })

    const selectedChiTietGiao = computed(() => {
        return (
            enrichedChiTietGiaoChiTieuOptions.value.find(
                (x) => getId(x) === Number(form.chiTietGiaoChiTieuId ?? 0)
            ) || null
        )
    })

    const selectedDanhMucChiTieu = computed(() => {
        const chiTiet = selectedChiTietGiao.value
        if (!chiTiet) return null
        return findDanhMucById(getDanhMucChiTieuId(chiTiet))
    })

    const getTieuChiDanhGiaForChiTiet = (chiTiet) => {
        return getLoaiChiTieu(chiTiet) || getLoaiChiTieu(selectedDanhMucChiTieu.value)
    }

    const getLoaiMocSoSanhForChiTiet = (chiTiet) => {
        return normalizeCode(
            chiTiet?.LoaiMocSoSanh ||
            chiTiet?.loaiMocSoSanh ||
            selectedDanhMucChiTieu.value?.LoaiMocSoSanh ||
            selectedDanhMucChiTieu.value?.loaiMocSoSanh
        )
    }

    const getChieuSoSanhForChiTiet = (chiTiet) => {
        return normalizeCode(
            chiTiet?.ChieuSoSanh ||
            chiTiet?.chieuSoSanh ||
            selectedDanhMucChiTieu.value?.ChieuSoSanh ||
            selectedDanhMucChiTieu.value?.chieuSoSanh
        )
    }

    const getQuyTacDanhGiaForChiTiet = (chiTiet) => {
        const tieuChiDanhGia = getTieuChiDanhGiaForChiTiet(chiTiet)
        return normalizeCode(
            chiTiet?.QuyTacDanhGia ||
            chiTiet?.quyTacDanhGia ||
            (tieuChiDanhGia === 'DINH_TINH' ? 'MAC_DINH' : 'DAT_TOI_THIEU')
        )
    }

    const selectedKyBaoCao = computed(() => {
        return (
            kyBaoCaoOptions.value.find(
                (x) => getId(x) === Number(form.kyBaoCaoKPIId ?? 0)
            ) || null
        )
    })

    const selectedDotGiao = computed(() => {
        const chiTiet = selectedChiTietGiao.value
        if (!chiTiet) return null

        const dotGiaoId = getDotGiaoChiTieuId(chiTiet)
        if (!dotGiaoId) return null

        return findDotGiaoById(dotGiaoId)
    })

    const filteredKyBaoCaoOptions = computed(() => {
        const chiTiet = selectedChiTietGiao.value
        if (!chiTiet) return []

        const tanSuat = getTanSuatBaoCao(chiTiet)

        return kyBaoCaoOptions.value.filter((item) => {
            if (!isKyDangMo(item)) return false
            if (!isKyPhuHopTanSuat(item, tanSuat)) return false
            return isChiTietHopLeTheoNgayKetThuc(chiTiet, item)
        })
    })

    const currentLoaiChiTieu = computed(() => {
        return getTieuChiDanhGiaForChiTiet(selectedChiTietGiao.value)
    })

    const isDinhTinh = computed(() => currentLoaiChiTieu.value === 'DINH_TINH')
    const isDinhLuongSoSanh = computed(() => currentLoaiChiTieu.value === 'DINH_LUONG_SO_SANH')

    const comparisonSourceLabel = computed(() => mapLoaiMocSoSanh(getLoaiMocSoSanhForChiTiet(selectedChiTietGiao.value)))

    const comparisonDirectionLabel = computed(() => mapChieuSoSanh(getChieuSoSanhForChiTiet(selectedChiTietGiao.value)))

    const comparisonRuleLabel = computed(() => mapQuyTacDanhGia(getQuyTacDanhGiaForChiTiet(selectedChiTietGiao.value)))

    const currentValueLabel = computed(() => {
        if (isDinhLuongSoSanh.value) {
            return 'Kết quả đạt được trong kỳ'
        }

        return 'Giá trị thực hiện trong kỳ'
    })

    const currentValuePlaceholder = computed(() => {
        if (isDinhLuongSoSanh.value) {
            return 'Nhập kết quả đạt được trong kỳ báo cáo'
        }

        return 'Nhập giá trị thực hiện trong kỳ'
    })

    const getThuTuKyFromRecord = (item) => {
        const kyId = Number(item?.KyBaoCaoKPIId ?? item?.kyBaoCaoKPIId ?? 0)
        const kyItem = kyBaoCaoOptions.value.find((x) => getId(x) === kyId)
        return getThuTuKy(kyItem)
    }

    const recordsForSelectedChiTiet = computed(() => {
        const chiTietId = Number(form.chiTietGiaoChiTieuId ?? 0)
        if (!chiTietId) return []

        return enrichedItems.value
            .filter((item) => Number(item.ChiTietGiaoChiTieuId ?? item.chiTietGiaoChiTieuId ?? 0) === chiTietId)
            .sort((left, right) => {
                const kyDiff = getThuTuKyFromRecord(left) - getThuTuKyFromRecord(right)
                if (kyDiff !== 0) return kyDiff
                return getId(left) - getId(right)
            })
    })

    const giaTriCuoiKyPreview = computed(() => {
        const dauKy = Number(fixedGiaTriDauKy.value ?? 0)
        const thucHien = Number(form.giaTriThucHienTrongKy ?? 0)
        return dauKy + thucHien
    })

    const nopDuocTheoNgayKetThucDotGiao = computed(() => {
        return isChiTietHopLeTheoNgayKetThuc(selectedChiTietGiao.value, selectedKyBaoCao.value)
    })

    const enrichedItems = computed(() => {
        return items.value.map((item) => {
            const chiTietId = Number(item.ChiTietGiaoChiTieuId ?? item.chiTietGiaoChiTieuId ?? 0)
            const kyId = Number(item.KyBaoCaoKPIId ?? item.kyBaoCaoKPIId ?? 0)

            const chiTiet = enrichedChiTietGiaoChiTieuOptions.value.find((x) => getId(x) === chiTietId)
            const kyBaoCao = kyBaoCaoOptions.value.find((x) => getId(x) === kyId)

            return {
                ...item,
                DonViNhanId: getDonViNhanId(chiTiet),
                MaChiTieu: item.MaChiTieu || item.maChiTieu || chiTiet?.MaChiTieuHienThi || '-',
                TenChiTieu: item.TenChiTieu || item.tenChiTieu || chiTiet?.TenChiTieuHienThi || '-',
                TenDonViNhan: item.TenDonViNhan || item.tenDonViNhan || chiTiet?.TenDonViNhanHienThi || '-',
                TenKyHienThi:
                    getKyBaoCaoDisplay(item) !== '-'
                        ? getKyBaoCaoDisplay(item)
                        : getKyBaoCaoDisplay(kyBaoCao),
                MaKy:
                    item.MaKy ||
                    item.maKy ||
                    kyBaoCao?.MaKy ||
                    kyBaoCao?.maKy ||
                    '-'
            }
        })
    })

    const fixedGiaTriDauKy = computed(() => {
        const giaTriDauKyCoDinh = getGiaTriDauKyCoDinhFromChiTiet(selectedChiTietGiao.value)
        if (giaTriDauKyCoDinh !== null) {
            return giaTriDauKyCoDinh
        }

        const editingRecord = isEdit.value
            ? recordsForSelectedChiTiet.value.find((item) => getId(item) === Number(editingId.value))
            : null

        return Number(
            editingRecord?.GiaTriDauKy ??
            editingRecord?.giaTriDauKy ??
            0
        )
    })

    const filteredItems = computed(() => {
        return enrichedItems.value.filter((item) => {
            const keyword = filters.keyword.trim().toLowerCase()
            const kyId = Number(item.KyBaoCaoKPIId ?? item.kyBaoCaoKPIId ?? 0)
            const donViNhanId = Number(item.DonViNhanId ?? item.donViNhanId ?? 0)
            const chiTietId = Number(item.ChiTietGiaoChiTieuId ?? item.chiTietGiaoChiTieuId ?? 0)

            const searchText = [
                item.MaKy || '',
                item.MaChiTieu || '',
                item.TenChiTieu || '',
                item.TenDonViNhan || ''
            ]
                .join(' ')
                .toLowerCase()

            const matchKeyword = !keyword || searchText.includes(keyword)
            const matchKy = !filters.kyBaoCaoKPIId || Number(filters.kyBaoCaoKPIId) === kyId
            const matchDonViNhan = !filters.donViNhanId || Number(filters.donViNhanId) === donViNhanId
            const matchChiTiet =
                !filters.chiTietGiaoChiTieuId || Number(filters.chiTietGiaoChiTieuId) === chiTietId

            return matchKeyword && matchKy && matchDonViNhan && matchChiTiet
        })
    })

    const buildPayload = () => ({
        chiTietGiaoChiTieuId: form.chiTietGiaoChiTieuId,
        kyBaoCaoKPIId: form.kyBaoCaoKPIId,
        giaTriThucHienTrongKy: isDinhTinh.value ? 0 : Number(form.giaTriThucHienTrongKy ?? 0),
        nhanXet: String(form.nhanXet || '').trim() || null
    })

    const fetchItems = async () => {
        try {
            loading.value = true
            const response = await httpClient.get(API_PATHS.theoDoiThucHienKPI)
            items.value = normalizeList(response)
        } catch (error) {
            console.error('fetchItems error:', error?.response?.status, error?.config?.url, error)
            alert(error?.response?.data?.message || 'Không tải được danh sách báo cáo định kỳ.')
        } finally {
            loading.value = false
        }
    }

    const fetchChiTietGiaoChiTieuOptions = async () => {
        try {
            const response = await httpClient.get(API_PATHS.chiTietGiaoChiTieu)
            chiTietGiaoChiTieuOptions.value = normalizeList(response)
        } catch (error) {
            console.error('fetchChiTietGiaoChiTieuOptions error:', error?.response?.status, error?.config?.url, error)
            chiTietGiaoChiTieuOptions.value = []
        }
    }

    const fetchKyBaoCaoOptions = async () => {
        try {
            const response = await httpClient.get(API_PATHS.kyBaoCaoKPI)
            kyBaoCaoOptions.value = normalizeList(response)
        } catch (error) {
            console.error('fetchKyBaoCaoOptions error:', error?.response?.status, error?.config?.url, error)
            kyBaoCaoOptions.value = []
        }
    }

    const fetchDanhMucChiTieuOptions = async () => {
        try {
            const response = await httpClient.get(API_PATHS.danhMucChiTieu)
            danhMucChiTieuOptions.value = normalizeList(response)
        } catch (error) {
            console.error('fetchDanhMucChiTieuOptions error:', error?.response?.status, error?.config?.url, error)
            danhMucChiTieuOptions.value = []
        }
    }

    const fetchDonViOptions = async () => {
        try {
            const response = await httpClient.get(API_PATHS.donVi)
            donViOptions.value = normalizeList(response)
        } catch (error) {
            console.error('fetchDonViOptions error:', error?.response?.status, error?.config?.url, error)
            donViOptions.value = []
        }
    }

    const fetchDotGiaoChiTieuOptions = async () => {
        try {
            const response = await httpClient.get(API_PATHS.dotGiaoChiTieu)
            dotGiaoChiTieuOptions.value = normalizeList(response)
        } catch (error) {
            console.error('fetchDotGiaoChiTieuOptions error:', error?.response?.status, error?.config?.url, error)
            dotGiaoChiTieuOptions.value = []
        }
    }

    const openCreateModal = async () => {
        isEdit.value = false
        editingId.value = null
        await hydrateFormSafely(createDefaultForm())
        showModal.value = true
    }

    const openEditModal = async (item) => {
        const chiTietId = Number(item.ChiTietGiaoChiTieuId ?? item.chiTietGiaoChiTieuId ?? 0)
        const chiTiet = enrichedChiTietGiaoChiTieuOptions.value.find((x) => getId(x) === chiTietId)

        if (chiTiet && hasChildCriteria(chiTiet)) {
            alert('Bản ghi này đang gắn với chỉ tiêu cha của cấu hình phân rã. Vui lòng nhập và sửa số liệu ở từng tiêu chí con.')
            return
        }

        isEdit.value = true
        editingId.value = getId(item)

        await hydrateFormSafely({
            donViNhanId: getDonViNhanId(chiTiet) || null,
            chiTietGiaoChiTieuId: chiTietId || null,
            kyBaoCaoKPIId: item.KyBaoCaoKPIId ?? item.kyBaoCaoKPIId ?? null,
            giaTriThucHienTrongKy: item.GiaTriThucHienTrongKy ?? item.giaTriThucHienTrongKy ?? 0,
            nhanXet: item.NhanXet ?? item.nhanXet ?? ''
        })

        showModal.value = true
    }

    const closeModal = async () => {
        showModal.value = false
        await hydrateFormSafely(createDefaultForm())
    }

    const validateForm = () => {
        if (!form.donViNhanId || Number(form.donViNhanId) <= 0) {
            alert('Vui lòng chọn đơn vị thực hiện.')
            return false
        }

        if (!form.chiTietGiaoChiTieuId || Number(form.chiTietGiaoChiTieuId) <= 0) {
            alert('Vui lòng chọn chỉ tiêu giao.')
            return false
        }

        const chiTietDangChon = selectedChiTietGiao.value
        if (!chiTietDangChon || getDonViNhanId(chiTietDangChon) !== Number(form.donViNhanId)) {
            alert('Chỉ tiêu giao không thuộc đơn vị thực hiện đã chọn.')
            return false
        }

        if (hasChildCriteria(chiTietDangChon)) {
            alert('Chỉ tiêu phân rã phải nhập kết quả ở tiêu chí con, không nhập ở chỉ tiêu cha.')
            return false
        }

        if (!form.kyBaoCaoKPIId || Number(form.kyBaoCaoKPIId) <= 0) {
            alert('Vui lòng chọn kỳ báo cáo.')
            return false
        }

        const kyDangChon = selectedKyBaoCao.value
        const tanSuat = getTanSuatBaoCao(selectedChiTietGiao.value)

        if (!isKyDangMo(kyDangChon)) {
            alert('Chỉ được chọn kỳ báo cáo đang mở.')
            return false
        }

        if (!isKyPhuHopTanSuat(kyDangChon, tanSuat)) {
            alert('Kỳ báo cáo không phù hợp với kỳ báo cáo đã giao cho chỉ tiêu này.')
            return false
        }

        const dotGiao = selectedDotGiao.value
        if (!dotGiao) {
            alert('Chỉ tiêu giao chưa gắn với đợt giao chỉ tiêu hợp lệ.')
            return false
        }

        if (!nopDuocTheoNgayKetThucDotGiao.value) {
            alert('Không được nộp vì ngày cuối kỳ của kỳ báo cáo phải lớn hơn hoặc bằng ngày bắt đầu và nhỏ hơn hoặc bằng ngày kết thúc của đợt giao chỉ tiêu.')
            return false
        }

        if (isDinhTinh.value) {
            if (!form.nhanXet) {
                alert('Vui lòng chọn kết quả đánh giá định tính.')
                return false
            }
        } else {
            if (Number(fixedGiaTriDauKy.value) < 0) {
                alert('Giá trị đầu kỳ cố định không hợp lệ.')
                return false
            }

            if (Number(form.giaTriThucHienTrongKy) < 0) {
                alert('Giá trị thực hiện trong kỳ không hợp lệ.')
                return false
            }
        }

        return true
    }

    const handleSubmit = async () => {
        if (!validateForm()) return

        try {
            saving.value = true
            const payload = buildPayload()

            if (isEdit.value && editingId.value) {
            await httpClient.put(`${API_PATHS.theoDoiThucHienKPI}/${editingId.value}`, payload)
        } else {
            await httpClient.post(API_PATHS.theoDoiThucHienKPI, payload)
        }

            closeModal()
            await fetchItems()
        } catch (error) {
            console.error('handleSubmit error:', error?.response?.status, error?.config?.url, error)

            const message =
                error?.response?.data?.message ||
                error?.response?.data?.title ||
                JSON.stringify(error?.response?.data?.errors || {}, null, 2) ||
                'Lưu báo cáo định kỳ thất bại.'

            alert(message)
        } finally {
            saving.value = false
        }
    }

    const handleDelete = async (item) => {
        const ok = window.confirm('Bạn có chắc muốn xóa bản ghi báo cáo định kỳ này không?')
        if (!ok) return

        try {
            await httpClient.delete(`${API_PATHS.theoDoiThucHienKPI}/${getId(item)}`)
            await fetchItems()
        } catch (error) {
            console.error('handleDelete error:', error?.response?.status, error?.config?.url, error)
            alert(error?.response?.data?.message || 'Xóa báo cáo định kỳ thất bại.')
        }
    }

    const resetFilters = () => {
        filters.kyBaoCaoKPIId = null
        filters.donViNhanId = null
        filters.chiTietGiaoChiTieuId = null
        filters.keyword = ''
    }

    const formatNumber = (value) => {
        if (value === null || value === undefined || value === '') return '-'
        return Number(value).toLocaleString('vi-VN')
    }

    const formatEditableNumber = (value) => {
        if (value === null || value === undefined || Number.isNaN(Number(value))) return ''
        return String(value)
    }

    watch(
        () => filters.donViNhanId,
        () => {
            if (
                filters.chiTietGiaoChiTieuId &&
                !filteredChiTietGiaoFilterOptions.value.some(
                    (x) => getId(x) === Number(filters.chiTietGiaoChiTieuId)
                )
            ) {
                filters.chiTietGiaoChiTieuId = null
            }
        }
    )

    watch(
        () => form.donViNhanId,
        () => {
            if (isHydratingForm.value) return
            form.chiTietGiaoChiTieuId = null
            form.kyBaoCaoKPIId = null
            form.giaTriThucHienTrongKy = 0
            form.nhanXet = ''
        }
    )

    watch(
        () => form.chiTietGiaoChiTieuId,
        () => {
            if (isHydratingForm.value) return
            const selected = selectedChiTietGiao.value

            if (selected) {
                const donViNhanId = getDonViNhanId(selected)
                if (donViNhanId > 0) {
                    form.donViNhanId = donViNhanId
                }
            }

            if (
                form.kyBaoCaoKPIId &&
                !filteredKyBaoCaoOptions.value.some(
                    (x) => getId(x) === Number(form.kyBaoCaoKPIId)
                )
            ) {
                form.kyBaoCaoKPIId = null
            }

            form.giaTriThucHienTrongKy = 0
            form.nhanXet = ''
        }
    )

    watch(
        () => form.kyBaoCaoKPIId,
        () => {
            if (isHydratingForm.value) return
            if (!form.kyBaoCaoKPIId) {
                form.giaTriThucHienTrongKy = 0
            }
        }
    )

    onMounted(async () => {
        await Promise.all([
            fetchItems(),
            fetchChiTietGiaoChiTieuOptions(),
            fetchKyBaoCaoOptions(),
            fetchDanhMucChiTieuOptions(),
            fetchDonViOptions(),
            fetchDotGiaoChiTieuOptions()
        ])
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

    .selection-summary {
        padding: 12px 14px;
        border-radius: 14px;
        background: #f8fbff;
        border: 1px solid #d9e8f8;
    }

    .selection-summary-title {
        font-weight: 700;
        color: #1e3a5f;
        line-height: 1.45;
    }

    .selection-summary-highlight {
        margin-top: 8px;
        font-weight: 700;
        color: #0d4f8b;
        line-height: 1.5;
    }

    .selection-summary-meta {
        display: flex;
        flex-wrap: wrap;
        gap: 8px 16px;
        margin-top: 6px;
        font-size: 0.9rem;
        color: #64748b;
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
