<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-exclamation-circle"></i>
                    </div>
                    <div class="gov-text">
                        <div class="gov-title">ĐƠN VỊ CHƯA NỘP BÁO CÁO KPI</div>
                        <div class="gov-sub">
                        </div>
                    </div>
                </div>

                <div class="d-flex justify-content-end mb-4">
                    <button class="btn btn-primary btn-action" @click="fetchData">
                        <i class="bi bi-arrow-repeat me-2"></i>
                        Tải lại dữ liệu
                    </button>
                </div>

                <div class="card custom-card mb-4">
                    <div class="card-header bg-white border-0 pb-0">
                        <h5 class="mb-1">Bộ lọc tìm kiếm</h5>
                        <small class="text-muted">Tra cứu nhanh đơn vị chưa nộp báo cáo</small>
                    </div>

                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-12 col-md-4">
                                <label class="form-label">Kỳ báo cáo KPI</label>
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
                                    <option v-for="item in donViOptions" :key="getId(item)" :value="getId(item)">
                                        {{ getTenDonVi(item) }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-4">
                                <label class="form-label">Từ khóa</label>
                                <input v-model="filters.keyword" type="text" class="form-control"
                                    placeholder="Mã chỉ tiêu, tên chỉ tiêu, tên đơn vị..." />
                            </div>
                        </div>

                        <div class="d-flex flex-wrap gap-2 mt-4">
                            <button class="btn btn-primary" @click="fetchData">
                                <i class="bi bi-search me-1"></i>
                                Tải dữ liệu
                            </button>
                            <button class="btn btn-outline-secondary" @click="resetFilters">
                                <i class="bi bi-arrow-clockwise me-1"></i>
                                Đặt lại
                            </button>
                        </div>
                    </div>
                </div>

                <div class="row g-3 mb-4">
                    <div class="col-12 col-md-4">
                        <div class="card custom-card stat-card">
                            <div class="card-body">
                                <div class="stat-label">Tổng chỉ tiêu phải báo cáo</div>
                                <div class="stat-value">{{ requiredReportItems.length }}</div>
                            </div>
                        </div>
                    </div>

                    <div class="col-12 col-md-4">
                        <div class="card custom-card stat-card">
                            <div class="card-body">
                                <div class="stat-label">Đã nộp báo cáo</div>
                                <div class="stat-value text-success">{{ submittedItems.length }}</div>
                            </div>
                        </div>
                    </div>

                    <div class="col-12 col-md-4">
                        <div class="card custom-card stat-card">
                            <div class="card-body">
                                <div class="stat-label">Chưa nộp báo cáo</div>
                                <div class="stat-value text-danger">{{ filteredMissingItems.length }}</div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card custom-card">
                    <div class="card-header bg-white d-flex justify-content-between align-items-center border-0">
                        <div>
                            <h5 class="mb-1">Danh sách chưa nộp</h5>
                            <small class="text-muted">Các đơn vị/chỉ tiêu chưa có bản ghi theo dõi KPI</small>
                        </div>
                        <span class="badge text-bg-light border">Tổng: {{ filteredMissingItems.length }}</span>
                    </div>

                    <div class="card-body p-0">
                        <div v-if="loading" class="empty-state">
                            <div class="spinner-border text-primary mb-3" role="status"></div>
                            <div>Đang tải dữ liệu...</div>
                        </div>

                        <div v-else-if="!filteredMissingItems.length" class="empty-state">
                            <i class="bi bi-check-circle fs-1 text-success mb-2"></i>
                            <div>Không có đơn vị nào chưa nộp báo cáo</div>
                        </div>

                        <div v-else class="table-responsive">
                            <ColumnVisibilityTools table-id="DonViChuaCapNhatPage-table" />
                            <table id="DonViChuaCapNhatPage-table" class="table table-hover align-middle mb-0 custom-table managed-table">
                                <thead>
                                    <tr>
                                        <th>Kỳ báo cáo</th>
                                        <th>Đơn vị thực hiện</th>
                                        <th>Tên chỉ tiêu</th>
                                        <th>Tần suất</th>
                                        <th>Trạng thái</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="item in filteredMissingItems" :key="item.uniqueKey">
                                        <td>{{ item.tenKy || item.maKy || '-' }}</td>
                                        <td>{{ item.tenDonViNhan || '-' }}</td>
                                        <td>{{ item.tenChiTieu || '-' }}</td>
                                        <td>{{ mapTanSuat(item.tanSuatBaoCao) }}</td>
                                        <td>
                                            <span class="badge rounded-pill text-bg-danger">
                                                Chưa nộp
                                            </span>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </BaseLayout>
</template>

<script setup>
    import { computed, onMounted, reactive, ref } from 'vue'
    import BaseLayout from '../BaseLayout.vue'
import ColumnVisibilityTools from '../shared/ColumnVisibilityTools.vue'
    import httpClient from '../../services/httpClient'

    const API_PATHS = {
        theoDoiThucHienKPI: '/TheoDoiThucHienKPI',
        chiTietGiaoChiTieu: '/ChiTietGiaoChiTieu',
        kyBaoCaoKPI: '/KyBaoCaoKPI',
        danhMucChiTieu: '/danh-muc-chi-tieu',
        donVi: '/DonVi'
    }

    const loading = ref(false)

    const theoDoiItems = ref([])
    const chiTietGiaoChiTieuOptions = ref([])
    const kyBaoCaoOptions = ref([])
    const danhMucChiTieuOptions = ref([])
    const donViOptions = ref([])

    const filters = reactive({
        kyBaoCaoKPIId: null,
        donViNhanId: null,
        keyword: ''
    })

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

    const getMaKy = (item) => {
        return item?.MaKy || item?.maKy || '-'
    }

    const getLoaiKy = (item) => {
        return item?.LoaiKy || item?.loaiKy || item?.LoaiKyBaoCao || item?.loaiKyBaoCao || ''
    }

    const getTrangThaiKy = (item) => {
        return item?.TrangThai || item?.trangThai || item?.TrangThaiKy || item?.trangThaiKy || ''
    }

    const getTanSuatBaoCao = (item) => {
        return item?.TanSuatBaoCao || item?.tanSuatBaoCao || ''
    }

    const getDanhMucChiTieuId = (item) => {
        return Number(item?.DanhMucChiTieuId ?? item?.danhMucChiTieuId ?? 0)
    }

    const getDonViNhanId = (item) => {
        return Number(item?.DonViNhanId ?? item?.donViNhanId ?? 0)
    }

    const getTenDonVi = (item) => {
        return item?.TenDonVi || item?.tenDonVi || '-'
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

    const findDanhMucById = (id) => {
        return danhMucChiTieuOptions.value.find((x) => getId(x) === Number(id)) || null
    }

    const findDonViById = (id) => {
        return donViOptions.value.find((x) => getId(x) === Number(id)) || null
    }

    const enrichedChiTietGiaoChiTieuOptions = computed(() => {
        return chiTietGiaoChiTieuOptions.value.map((item) => {
            const danhMuc = findDanhMucById(getDanhMucChiTieuId(item))
            const donVi = findDonViById(getDonViNhanId(item))

            return {
                ...item,
                id: getId(item),
                donViNhanId: getDonViNhanId(item),
                maChiTieu:
                    item?.MaChiTieu ||
                    item?.maChiTieu ||
                    danhMuc?.MaChiTieu ||
                    danhMuc?.maChiTieu ||
                    '',
                tenChiTieu:
                    item?.TenChiTieu ||
                    item?.tenChiTieu ||
                    danhMuc?.TenChiTieu ||
                    danhMuc?.tenChiTieu ||
                    '',
                tenDonViNhan:
                    item?.TenDonViNhan ||
                    item?.tenDonViNhan ||
                    donVi?.TenDonVi ||
                    donVi?.tenDonVi ||
                    '',
                tanSuatBaoCao: getTanSuatBaoCao(item) || getTanSuatBaoCao(danhMuc)
            }
        })
    })

    const reportedKeys = computed(() => {
        return new Set(
            theoDoiItems.value.map((item) => {
                const chiTietId = Number(item?.ChiTietGiaoChiTieuId ?? item?.chiTietGiaoChiTieuId ?? 0)
                const kyId = Number(item?.KyBaoCaoKPIId ?? item?.kyBaoCaoKPIId ?? 0)
                return `${chiTietId}_${kyId}`
            })
        )
    })

    const requiredReportItems = computed(() => {
        const result = []

        enrichedChiTietGiaoChiTieuOptions.value.forEach((chiTiet) => {
            kyBaoCaoOptions.value.forEach((ky) => {
                if (!isKyDangMo(ky)) return
                if (!isKyPhuHopTanSuat(ky, chiTiet.tanSuatBaoCao)) return

                result.push({
                    uniqueKey: `${chiTiet.id}_${getId(ky)}`,
                    chiTietGiaoChiTieuId: chiTiet.id,
                    kyBaoCaoKPIId: getId(ky),
                    donViNhanId: chiTiet.donViNhanId,
                    tenDonViNhan: chiTiet.tenDonViNhan,
                    maChiTieu: chiTiet.maChiTieu,
                    tenChiTieu: chiTiet.tenChiTieu,
                    tanSuatBaoCao: chiTiet.tanSuatBaoCao,
                    tenKy: getKyBaoCaoDisplay(ky),
                    maKy: getMaKy(ky)
                })
            })
        })

        return result
    })

    const submittedItems = computed(() => {
        return requiredReportItems.value.filter((item) =>
            reportedKeys.value.has(`${item.chiTietGiaoChiTieuId}_${item.kyBaoCaoKPIId}`)
        )
    })

    const missingItems = computed(() => {
        return requiredReportItems.value.filter((item) =>
            !reportedKeys.value.has(`${item.chiTietGiaoChiTieuId}_${item.kyBaoCaoKPIId}`)
        )
    })

    const parentOptions = computed(() => {
        return donViOptions.value
    })

    const filteredMissingItems = computed(() => {
        const keyword = filters.keyword.trim().toLowerCase()

        return missingItems.value.filter((item) => {
            const matchKy =
                !filters.kyBaoCaoKPIId ||
                Number(filters.kyBaoCaoKPIId) === Number(item.kyBaoCaoKPIId)

            const matchDonVi =
                !filters.donViNhanId ||
                Number(filters.donViNhanId) === Number(item.donViNhanId)

            const searchText = [
                item.maChiTieu || '',
                item.tenChiTieu || '',
                item.tenDonViNhan || '',
                item.tenKy || '',
                item.maKy || ''
            ]
                .join(' ')
                .toLowerCase()

            const matchKeyword = !keyword || searchText.includes(keyword)

            return matchKy && matchDonVi && matchKeyword
        })
    })

    const fetchTheoDoiItems = async () => {
        const response = await httpClient.get(API_PATHS.theoDoiThucHienKPI)
        theoDoiItems.value = normalizeList(response)
    }

    const fetchChiTietGiaoChiTieuOptions = async () => {
        const response = await httpClient.get(API_PATHS.chiTietGiaoChiTieu)
        chiTietGiaoChiTieuOptions.value = normalizeList(response)
    }

    const fetchKyBaoCaoOptions = async () => {
        const response = await httpClient.get(API_PATHS.kyBaoCaoKPI)
        kyBaoCaoOptions.value = normalizeList(response)
    }

    const fetchDanhMucChiTieuOptions = async () => {
        const response = await httpClient.get(API_PATHS.danhMucChiTieu)
        danhMucChiTieuOptions.value = normalizeList(response)
    }

    const fetchDonViOptions = async () => {
        const response = await httpClient.get(API_PATHS.donVi)
        donViOptions.value = normalizeList(response)
    }

    const fetchData = async () => {
        try {
            loading.value = true
            await Promise.all([
                fetchTheoDoiItems(),
                fetchChiTietGiaoChiTieuOptions(),
                fetchKyBaoCaoOptions(),
                fetchDanhMucChiTieuOptions(),
                fetchDonViOptions()
            ])
        } catch (error) {
            console.error('fetchData error:', error?.response?.status, error?.config?.url, error)
            alert(error?.response?.data?.message || 'Không tải được dữ liệu.')
        } finally {
            loading.value = false
        }
    }

    const resetFilters = () => {
        filters.kyBaoCaoKPIId = null
        filters.donViNhanId = null
        filters.keyword = ''
    }

    onMounted(async () => {
        await fetchData()
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

    .stat-card .card-body {
        padding: 1.25rem 1.5rem;
    }

    .stat-label {
        color: #64748b;
        font-weight: 600;
        margin-bottom: 0.4rem;
    }

    .stat-value {
        font-size: 2rem;
        font-weight: 700;
        color: #0f172a;
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






