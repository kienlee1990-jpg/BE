<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-graph-up-arrow"></i>
                    </div>
                    <div class="gov-text">
                        <div class="wave-title">HỆ THỐNG THEO DÕI CHỈ TIÊU CÔNG TÁC</div>
                        <div class="gov-title">TIẾN ĐỘ THỰC HIỆN KPI</div>
                        <div class="gov-sub"></div>
                    </div>
                </div>

                <div class="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
                    <div class="gov-banner">
                        <img src="https://upload.wikimedia.org/wikipedia/commons/a/a3/Emblem_of_Vietnam.svg"
                            class="gov-emblem" />
                    </div>

                    <button class="btn btn-primary btn-action" @click="fetchData">
                        <i class="bi bi-arrow-repeat me-2"></i>
                        Tải lại dữ liệu
                    </button>
                </div>

                <div class="card custom-card mb-4">
                    <div class="card-header bg-white border-0 pb-0">
                        <h5 class="mb-1">Bộ lọc tìm kiếm</h5>
                        <small class="text-muted">Lọc tiến độ theo đợt giao, chỉ tiêu, đơn vị và kỳ báo cáo</small>
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
                                <label class="form-label">Chỉ tiêu</label>
                                <select v-model.number="filters.chiTietGiaoChiTieuId" class="form-select">
                                    <option :value="null">Tất cả</option>
                                    <option v-for="item in chiTietGiaoChiTieuOptions" :key="getId(item)"
                                        :value="getId(item)">
                                        {{ getChiTietDisplay(item) }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Đơn vị</label>
                                <select v-model.number="filters.donViId" class="form-select">
                                    <option :value="null">Tất cả</option>
                                    <option v-for="item in donViOptions" :key="getId(item)" :value="getId(item)">
                                        {{ item.TenDonVi || item.tenDonVi || '-' }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Kỳ báo cáo</label>
                                <select v-model.number="filters.kyBaoCaoKPIId" class="form-select">
                                    <option :value="null">Tất cả</option>
                                    <option v-for="item in kyBaoCaoOptions" :key="getId(item)" :value="getId(item)">
                                        {{ item.TenKy || item.tenKy || item.TenKyBaoCao || item.tenKyBaoCao || '-' }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12">
                                <label class="form-label">Từ khóa</label>
                                <input v-model="filters.keyword" type="text" class="form-control"
                                    placeholder="Mã chỉ tiêu, tên chỉ tiêu, đơn vị, kỳ báo cáo..." />
                            </div>
                        </div>

                        <div class="d-flex flex-wrap gap-2 mt-4">
                            <button class="btn btn-primary" @click="fetchData">
                                <i class="bi bi-search me-1"></i>
                                Lọc dữ liệu
                            </button>
                            <button class="btn btn-outline-secondary" @click="resetFilters">
                                <i class="bi bi-arrow-clockwise me-1"></i>
                                Đặt lại
                            </button>
                        </div>
                    </div>
                </div>

                <div class="row g-3 mb-4">
                    <div class="col-12 col-md-6 col-xl-3">
                        <div class="card summary-card h-100">
                            <div class="card-body">
                                <div class="summary-label">Tổng chỉ tiêu</div>
                                <div class="summary-value">{{ groupedProgress.length }}</div>
                            </div>
                        </div>
                    </div>

                    <div class="col-12 col-md-6 col-xl-3">
                        <div class="card summary-card h-100">
                            <div class="card-body">
                                <div class="summary-label">Đạt/Vượt mục tiêu</div>
                                <div class="summary-value text-success">{{ summary.completed }}</div>
                            </div>
                        </div>
                    </div>

                    <div class="col-12 col-md-6 col-xl-3">
                        <div class="card summary-card h-100">
                            <div class="card-body">
                                <div class="summary-label">Đang thực hiện</div>
                                <div class="summary-value text-warning">{{ summary.inProgress }}</div>
                            </div>
                        </div>
                    </div>

                    <div class="col-12 col-md-6 col-xl-3">
                        <div class="card summary-card h-100">
                            <div class="card-body">
                                <div class="summary-label">Chưa bắt đầu</div>
                                <div class="summary-value text-secondary">{{ summary.notStarted }}</div>
                            </div>
                        </div>
                    </div>
                </div>

                <div v-if="loading" class="card custom-card">
                    <div class="card-body empty-state">
                        <div class="spinner-border text-primary mb-3" role="status"></div>
                        <div>Đang tải dữ liệu...</div>
                    </div>
                </div>

                <div v-else-if="!groupedProgress.length" class="card custom-card">
                    <div class="card-body empty-state">
                        <i class="bi bi-inbox fs-1 text-muted mb-2"></i>
                        <div>Chưa có dữ liệu tiến độ</div>
                    </div>
                </div>

                <div v-else class="d-flex flex-column gap-4">
                    <div v-for="item in groupedProgress" :key="item.key" class="card custom-card">
                        <div class="card-header bg-white border-0">
                            <div class="d-flex flex-column flex-lg-row justify-content-between gap-3">
                                <div>
                                    <h5 class="mb-1 text-primary">
                                        {{ item.tenChiTieu || '-' }}
                                    </h5>
                                    <div class="text-muted small d-flex flex-wrap gap-3">
                                        <span><strong>Đơn vị:</strong> {{ item.tenDonVi || '-' }}</span>
                                        <span><strong>Đợt giao:</strong> {{ item.tenDotGiao || '-' }}</span>
                                    </div>
                                </div>

                                <div class="d-flex flex-column align-items-start align-items-lg-end gap-2">
                                    <span class="badge rounded-pill" :class="getTienDoBadgeClass(item.trangThaiTienDo)">
                                        {{ item.trangThaiTienDo }}
                                    </span>
                                    <small class="text-muted">
                                        Mục tiêu: {{ formatNumber(item.giaTriMucTieu) }}
                                        <span v-if="item.giaTriMucTieuText">({{ item.giaTriMucTieuText }})</span>
                                    </small>
                                    <button class="btn btn-sm btn-outline-primary" @click="toggleExpand(item.key)">
                                        <i
                                            :class="expandedKeys.has(item.key) ? 'bi bi-chevron-up me-1' : 'bi bi-chevron-down me-1'"></i>
                                        {{ expandedKeys.has(item.key) ? 'Thu gọn' : 'Xem lịch sử' }}
                                    </button>
                                </div>
                            </div>
                        </div>

                        <div class="card-body pt-0">
                            <div class="row g-3 mb-4">
                                <div class="col-12 col-md-6 col-xl-3">
                                    <div class="metric-box">
                                        <div class="metric-label">Lũy kế hiện tại</div>
                                        <div class="metric-value">{{ formatNumber(item.giaTriLuyKe) }}</div>
                                    </div>
                                </div>

                                <div class="col-12 col-md-6 col-xl-3">
                                    <div class="metric-box">
                                        <div class="metric-label">% hoàn thành</div>
                                        <div class="metric-value">{{ formatPercent(item.tyLeHoanThanh) }}</div>
                                    </div>
                                </div>

                                <div class="col-12 col-md-6 col-xl-3">
                                    <div class="metric-box">
                                        <div class="metric-label">Còn thiếu</div>
                                        <div class="metric-value">{{ formatNumber(item.conThieu) }}</div>
                                    </div>
                                </div>

                                <div class="col-12 col-md-6 col-xl-3">
                                    <div class="metric-box">
                                        <div class="metric-label">Kỳ gần nhất</div>
                                        <div class="metric-value metric-value-sm">
                                            {{ item.tenKyGanNhat || '-' }}
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="progress mb-4 progress-custom">
                                <div class="progress-bar" role="progressbar"
                                    :style="{ width: `${Math.min(item.tyLeHoanThanh, 100)}%` }"
                                    :aria-valuenow="item.tyLeHoanThanh" aria-valuemin="0" aria-valuemax="100">
                                    {{ formatPercent(item.tyLeHoanThanh) }}
                                </div>
                            </div>

                            <div class="mini-chart-wrap mb-4">
                                <div class="mini-chart-title">Diễn biến lũy kế theo kỳ</div>
                                <svg v-if="item.chartPoints.length > 1" class="mini-chart" viewBox="0 0 100 32"
                                    preserveAspectRatio="none">
                                    <polyline fill="none" stroke="currentColor" stroke-width="2"
                                        :points="item.chartPolyline" />
                                    <circle v-for="(point, idx) in item.chartPoints" :key="idx" :cx="point.x"
                                        :cy="point.y" r="1.8" />
                                </svg>
                                <div v-else class="text-muted small">Chưa đủ dữ liệu để hiển thị biểu đồ</div>

                                <div class="d-flex flex-wrap gap-2 mt-2">
                                    <span v-for="history in item.lichSu" :key="history.id"
                                        class="badge text-bg-light border">
                                        {{ history.tenKy || history.tenKyBaoCao || '-' }}:
                                        {{ formatNumber(history.giaTriLuyKe) }}
                                    </span>
                                </div>
                            </div>

                            <div class="d-flex flex-wrap gap-2 mb-3">
                                <button class="btn btn-sm btn-primary" @click="goToDetail(item.chiTietGiaoChiTieuId)">
                                    <i class="bi bi-box-arrow-up-right me-1"></i>
                                    Xem chi tiết
                                </button>
                                <button class="btn btn-sm btn-outline-secondary" @click="goToDetailWithLatestKy(item)">
                                    <i class="bi bi-calendar-check me-1"></i>
                                    Mở kỳ gần nhất
                                </button>
                            </div>

                            <div v-if="expandedKeys.has(item.key)" class="table-responsive">
                                <table class="table table-hover align-middle mb-0 custom-table">
                                    <thead>
                                        <tr>
                                            <th>Kỳ báo cáo</th>
                                            <th>Đầu kỳ</th>
                                            <th>Thực hiện trong kỳ</th>
                                            <th>Cuối kỳ</th>
                                            <th>Lũy kế</th>
                                            <th>Trạng thái</th>
                                            <th>Nhận xét</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="history in item.lichSu" :key="history.id">
                                            <td><div class="fw-semibold">{{ history.tenKy || history.tenKyBaoCao || history.maKy || '-' }}</div></td>
                                            <td>{{ formatNumber(history.giaTriDauKy) }}</td>
                                            <td>{{ formatNumber(history.giaTriThucHienTrongKy) }}</td>
                                            <td>{{ formatNumber(history.giaTriCuoiKy) }}</td>
                                            <td>{{ formatNumber(history.giaTriLuyKe) }}</td>
                                            <td>
                                                <span class="badge rounded-pill"
                                                    :class="getTrangThaiClass(history.trangThai)">
                                                    {{ mapTrangThai(history.trangThai) }}
                                                </span>
                                            </td>
                                            <td>{{ history.nhanXet || '-' }}</td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
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
    import { apiRequest } from '../../services/api.js'

    const API_PATHS = {
        theoDoiThucHienKPI: '/TheoDoiThucHienKPI',
        chiTietGiaoChiTieu: '/ChiTietGiaoChiTieu',
        kyBaoCaoKPI: '/KyBaoCaoKPI',
        dotGiaoChiTieu: '/dot-giao-chi-tieu',
        donVi: '/DonVi'
    }

    const DETAIL_PAGE_PATH = '/TheoDoiThucHienKPI'

    const loading = ref(false)
    const expandedKeys = ref(new Set())

    const progressItems = ref([])
    const chiTietGiaoChiTieuOptions = ref([])
    const kyBaoCaoOptions = ref([])
    const dotOptions = ref([])
    const donViOptions = ref([])

    const filters = reactive({
        dotGiaoChiTieuId: null,
        chiTietGiaoChiTieuId: null,
        donViId: null,
        kyBaoCaoKPIId: null,
        keyword: ''
    })

    const getId = (item) => Number(item?.Id ?? item?.id ?? 0)

    const normalizeList = (response) => {
        if (Array.isArray(response)) return response
        if (Array.isArray(response?.data)) return response.data
        if (Array.isArray(response?.data?.data)) return response.data.data
        if (Array.isArray(response?.data?.items)) return response.data.items
        if (Array.isArray(response?.items)) return response.items
        return []
    }

    const getChiTietDisplay = (item) => {
        const ma = item?.MaChiTieu || item?.maChiTieu || ''
        const ten = item?.TenChiTieu || item?.tenChiTieu || ''
        const donVi = item?.TenDonViNhan || item?.tenDonViNhan || ''
        return [ma, ten, donVi].filter(Boolean).join(' - ') || '-'
    }

    const fetchData = async () => {
        try {
            loading.value = true

            const [
                progressResponse,
                chiTietResponse,
                kyBaoCaoResponse,
                dotResponse,
                donViResponse
            ] = await Promise.all([
                apiRequest(API_PATHS.theoDoiThucHienKPI),
                apiRequest(API_PATHS.chiTietGiaoChiTieu),
                apiRequest(API_PATHS.kyBaoCaoKPI),
                apiRequest(API_PATHS.dotGiaoChiTieu),
                apiRequest(API_PATHS.donVi)
            ])

            progressItems.value = normalizeList(progressResponse)
            chiTietGiaoChiTieuOptions.value = normalizeList(chiTietResponse)
            kyBaoCaoOptions.value = normalizeList(kyBaoCaoResponse)
            dotOptions.value = normalizeList(dotResponse)
            donViOptions.value = normalizeList(donViResponse)
        } catch (error) {
            console.error('fetchData error:', error)
            alert(error.message || 'Không tải được dữ liệu tiến độ KPI.')
        } finally {
            loading.value = false
        }
    }

    const enrichedProgressItems = computed(() => {
        return progressItems.value.map((item) => {
            const chiTietId = Number(item.ChiTietGiaoChiTieuId ?? item.chiTietGiaoChiTieuId ?? 0)
            const kyId = Number(item.KyBaoCaoKPIId ?? item.kyBaoCaoKPIId ?? 0)

            const chiTiet = chiTietGiaoChiTieuOptions.value.find((x) => getId(x) === chiTietId)
            const ky = kyBaoCaoOptions.value.find((x) => getId(x) === kyId)

            const dotId = Number(
                item.DotGiaoChiTieuId ??
                item.dotGiaoChiTieuId ??
                chiTiet?.DotGiaoChiTieuId ??
                chiTiet?.dotGiaoChiTieuId ??
                0
            )

            const donViId = Number(
                item.DonViNhanId ??
                item.donViNhanId ??
                chiTiet?.DonViNhanId ??
                chiTiet?.donViNhanId ??
                0
            )

            const dot = dotOptions.value.find((x) => getId(x) === dotId)
            const donVi = donViOptions.value.find((x) => getId(x) === donViId)

            return {
                id: getId(item),
                chiTietGiaoChiTieuId: chiTietId,
                kyBaoCaoKPIId: kyId,
                dotGiaoChiTieuId: dotId,
                donViId,
                maChiTieu:
                    item.MaChiTieu ||
                    item.maChiTieu ||
                    chiTiet?.MaChiTieu ||
                    chiTiet?.maChiTieu ||
                    '',
                tenChiTieu:
                    item.TenChiTieu ||
                    item.tenChiTieu ||
                    chiTiet?.TenChiTieu ||
                    chiTiet?.tenChiTieu ||
                    '',
                tenDonVi:
                    item.TenDonViNhan ||
                    item.tenDonViNhan ||
                    chiTiet?.TenDonViNhan ||
                    chiTiet?.tenDonViNhan ||
                    donVi?.TenDonVi ||
                    donVi?.tenDonVi ||
                    '',
                tenDotGiao:
                    item.TenDotGiao ||
                    item.tenDotGiao ||
                    dot?.TenDotGiao ||
                    dot?.tenDotGiao ||
                    '',
                maKy:
                    item.MaKy ||
                    item.maKy ||
                    ky?.MaKy ||
                    ky?.maKy ||
                    '',
                tenKy:
                    item.TenKy ||
                    item.tenKy ||
                    item.TenKyBaoCao ||
                    item.tenKyBaoCao ||
                    ky?.TenKy ||
                    ky?.tenKy ||
                    ky?.TenKyBaoCao ||
                    ky?.tenKyBaoCao ||
                    '',
                nam: item.Nam ?? item.nam ?? ky?.Nam ?? ky?.nam ?? 0,
                loaiKy: item.LoaiKy || item.loaiKy || ky?.LoaiKy || ky?.loaiKy || '',
                soKy: item.SoKy ?? item.soKy ?? ky?.SoKy ?? ky?.soKy ?? null,
                giaTriMucTieu:
                    item.GiaTriMucTieu ??
                    item.giaTriMucTieu ??
                    chiTiet?.GiaTriMucTieu ??
                    chiTiet?.giaTriMucTieu ??
                    0,
                giaTriMucTieuText:
                    item.GiaTriMucTieuText ||
                    item.giaTriMucTieuText ||
                    chiTiet?.GiaTriMucTieuText ||
                    chiTiet?.giaTriMucTieuText ||
                    '',
                giaTriDauKy: Number(item.GiaTriDauKy ?? item.giaTriDauKy ?? 0),
                giaTriThucHienTrongKy: Number(item.GiaTriThucHienTrongKy ?? item.giaTriThucHienTrongKy ?? 0),
                giaTriCuoiKy: Number(item.GiaTriCuoiKy ?? item.giaTriCuoiKy ?? 0),
                giaTriLuyKe: Number(item.GiaTriLuyKe ?? item.giaTriLuyKe ?? 0),
                nhanXet: item.NhanXet || item.nhanXet || '',
                trangThai: item.TrangThai || item.trangThai || 'MOI_TAO'
            }
        })
    })

    const filteredProgressItems = computed(() => {
        return enrichedProgressItems.value.filter((item) => {
            const keyword = filters.keyword.trim().toLowerCase()

            const searchText = [
                item.maChiTieu,
                item.tenChiTieu,
                item.tenDonVi,
                item.tenDotGiao,
                item.maKy,
                item.tenKy,
                item.nhanXet
            ]
                .join(' ')
                .toLowerCase()

            const matchKeyword = !keyword || searchText.includes(keyword)
            const matchDot = !filters.dotGiaoChiTieuId || Number(filters.dotGiaoChiTieuId) === item.dotGiaoChiTieuId
            const matchChiTiet =
                !filters.chiTietGiaoChiTieuId || Number(filters.chiTietGiaoChiTieuId) === item.chiTietGiaoChiTieuId
            const matchDonVi = !filters.donViId || Number(filters.donViId) === item.donViId
            const matchKy = !filters.kyBaoCaoKPIId || Number(filters.kyBaoCaoKPIId) === item.kyBaoCaoKPIId

            return matchKeyword && matchDot && matchChiTiet && matchDonVi && matchKy
        })
    })

    const groupedProgress = computed(() => {
        const groups = new Map()

        for (const item of filteredProgressItems.value) {
            const key = `${item.chiTietGiaoChiTieuId}`

            if (!groups.has(key)) {
                groups.set(key, {
                    key,
                    chiTietGiaoChiTieuId: item.chiTietGiaoChiTieuId,
                    maChiTieu: item.maChiTieu,
                    tenChiTieu: item.tenChiTieu,
                    tenDonVi: item.tenDonVi,
                    tenDotGiao: item.tenDotGiao,
                    giaTriMucTieu: item.giaTriMucTieu,
                    giaTriMucTieuText: item.giaTriMucTieuText,
                    lichSu: []
                })
            }

            groups.get(key).lichSu.push(item)
        }

        const result = Array.from(groups.values()).map((group) => {
            const lichSu = [...group.lichSu].sort(compareKy)
            const kyGanNhat = lichSu[lichSu.length - 1] || null
            const giaTriLuyKe = kyGanNhat?.giaTriLuyKe ?? 0
            const giaTriMucTieu = Number(group.giaTriMucTieu ?? 0)
            const tyLeHoanThanh = giaTriMucTieu > 0 ? (giaTriLuyKe / giaTriMucTieu) * 100 : 0
            const conThieu = Math.max(giaTriMucTieu - giaTriLuyKe, 0)

            const chartPoints = buildChartPoints(lichSu.map((x) => Number(x.giaTriLuyKe ?? 0)))
            const chartPolyline = chartPoints.map((p) => `${p.x},${p.y}`).join(' ')

            return {
                ...group,
                lichSu,
                tenKyGanNhat: kyGanNhat?.tenKy || '',
                kyBaoCaoKPIIdGanNhat: kyGanNhat?.kyBaoCaoKPIId ?? null,
                giaTriLuyKe,
                tyLeHoanThanh,
                conThieu,
                trangThaiTienDo: getTienDoText(giaTriLuyKe, giaTriMucTieu),
                chartPoints,
                chartPolyline
            }
        })

        return result.sort((a, b) => a.tenChiTieu.localeCompare(b.tenChiTieu))
    })

    const summary = computed(() => {
        let completed = 0
        let inProgress = 0
        let notStarted = 0

        for (const item of groupedProgress.value) {
            if (item.trangThaiTienDo === 'Đạt/Vượt mục tiêu') completed++
            else if (item.trangThaiTienDo === 'Đang thực hiện') inProgress++
            else notStarted++
        }

        return { completed, inProgress, notStarted }
    })

    const compareKy = (a, b) => {
        if ((a.nam ?? 0) !== (b.nam ?? 0)) return (a.nam ?? 0) - (b.nam ?? 0)

        const loaiA = thuTuLoaiKy(a.loaiKy)
        const loaiB = thuTuLoaiKy(b.loaiKy)
        if (loaiA !== loaiB) return loaiA - loaiB

        return (a.soKy ?? 0) - (b.soKy ?? 0)
    }

    const thuTuLoaiKy = (loaiKy) => {
        switch ((loaiKy || '').trim().toUpperCase()) {
            case 'THANG':
                return 1
            case 'QUY':
                return 2
            case '6THANG':
                return 3
            case 'NAM':
                return 4
            default:
                return 99
        }
    }

    const buildChartPoints = (values) => {
        if (!values.length) return []

        const min = Math.min(...values)
        const max = Math.max(...values)
        const range = max - min || 1

        return values.map((value, index) => {
            const x = values.length === 1 ? 50 : (index * 100) / (values.length - 1)
            const y = 28 - ((value - min) / range) * 24
            return { x, y }
        })
    }

    const toggleExpand = (key) => {
        const next = new Set(expandedKeys.value)
        if (next.has(key)) next.delete(key)
        else next.add(key)
        expandedKeys.value = next
    }

    const goToDetail = (chiTietGiaoChiTieuId) => {
        const url = `${DETAIL_PAGE_PATH}?chiTietGiaoChiTieuId=${chiTietGiaoChiTieuId}`
        window.location.href = url
    }

    const goToDetailWithLatestKy = (item) => {
        let url = `${DETAIL_PAGE_PATH}?chiTietGiaoChiTieuId=${item.chiTietGiaoChiTieuId}`
        if (item.kyBaoCaoKPIIdGanNhat) {
            url += `&kyBaoCaoKPIId=${item.kyBaoCaoKPIIdGanNhat}`
        }
        window.location.href = url
    }

    const getTienDoText = (giaTriLuyKe, giaTriMucTieu) => {
        const luyKe = Number(giaTriLuyKe ?? 0)
        const mucTieu = Number(giaTriMucTieu ?? 0)

        if (luyKe <= 0) return 'Chưa bắt đầu'
        if (mucTieu > 0 && luyKe >= mucTieu) return 'Đạt/Vượt mục tiêu'
        return 'Đang thực hiện'
    }

    const getTienDoBadgeClass = (status) => {
        switch (status) {
            case 'Đạt/Vượt mục tiêu':
                return 'text-bg-success'
            case 'Đang thực hiện':
                return 'text-bg-warning'
            default:
                return 'text-bg-secondary'
        }
    }

    const mapTrangThai = (value) => {
        const map = {
            MOI_TAO: 'Mới tạo',
            CHO_DUYET: 'Chờ duyệt',
            DA_DUYET: 'Đã duyệt',
            TU_CHOI: 'Từ chối'
        }
        return map[value] || value || '-'
    }

    const getTrangThaiClass = (value) => {
        const map = {
            MOI_TAO: 'text-bg-secondary',
            CHO_DUYET: 'text-bg-warning',
            DA_DUYET: 'text-bg-success',
            TU_CHOI: 'text-bg-danger'
        }
        return map[value] || 'text-bg-light'
    }

    const formatNumber = (value) => {
        if (value === null || value === undefined || value === '') return '-'
        return Number(value).toLocaleString('vi-VN')
    }

    const formatPercent = (value) => {
        if (value === null || value === undefined || Number.isNaN(Number(value))) return '0%'
        return `${Number(value).toFixed(1)}%`
    }

    const resetFilters = () => {
        filters.dotGiaoChiTieuId = null
        filters.chiTietGiaoChiTieuId = null
        filters.donViId = null
        filters.kyBaoCaoKPIId = null
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
        min-height: 44px;
        border-radius: 12px;
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

    .summary-card,
    .metric-box {
        border: 1px solid #e9ecef;
        border-radius: 1rem;
        background: #fff;
    }

    .summary-card .card-body,
    .metric-box {
        padding: 1rem;
    }

    .summary-label,
    .metric-label {
        color: #6c757d;
        font-size: 0.9rem;
        margin-bottom: 0.35rem;
    }

    .summary-value,
    .metric-value {
        font-size: 1.6rem;
        font-weight: 700;
        line-height: 1.2;
    }

    .metric-value-sm {
        font-size: 1.1rem;
    }

    .progress-custom {
        height: 1.25rem;
        border-radius: 999px;
    }

    .empty-state {
        min-height: 220px;
        display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;
        color: #64748b;
    }

    .mini-chart-wrap {
        border: 1px solid #e9ecef;
        border-radius: 1rem;
        padding: 1rem;
        background: #fafbfc;
    }

    .mini-chart-title {
        font-size: 0.9rem;
        color: #6c757d;
        margin-bottom: 0.75rem;
    }

    .mini-chart {
        width: 100%;
        height: 80px;
        display: block;
        color: #0d6efd;
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


