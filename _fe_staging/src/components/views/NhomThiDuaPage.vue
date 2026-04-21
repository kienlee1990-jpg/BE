<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="hero-card">
                    <div class="hero-head">
                        <div class="hero-icon">
                            <i class="bi bi-diagram-3-fill"></i>
                        </div>
                        <div>
                            <div class="hero-kicker">ĐÁNH GIÁ VÀ SO SÁNH</div>
                            <h1>Nhóm thi đua</h1>
                            <p>Xếp hạng đơn vị trong một nhóm thi đua theo đúng đơn vị và chỉ tiêu chi tiết đã cấu hình.</p>
                        </div>
                    </div>
                </div>

                <div class="filter-card">
                    <div class="filter-grid">
                        <div class="form-group">
                            <label>Nhóm thi đua</label>
                            <select v-model="filters.groupId">
                                <option v-for="group in groups" :key="group.id" :value="group.id">
                                    {{ group.tenNhom }}
                                </option>
                            </select>
                        </div>

                        <div class="form-group">
                            <label>Kỳ báo cáo</label>
                            <select v-model="reportFilters.kyBaoCaoKPIId" @change="fetchBaoCaoTongHop">
                                <option value="">-- Tính đến kỳ mới nhất hiện có --</option>
                                <option v-for="item in kyBaoCaoOptions" :key="getId(item)" :value="getId(item)">
                                    {{ getKyLabel(item) }}
                                </option>
                            </select>
                        </div>

                        <div class="form-group">
                            <label>Đợt giao chỉ tiêu</label>
                            <select v-model="filters.dotGiaoChiTieu">
                                <option value="">-- Tất cả đợt giao --</option>
                                <option v-for="item in dotGiaoOptions" :key="item" :value="item">
                                    {{ item }}
                                </option>
                            </select>
                        </div>

                        <div class="form-group">
                            <label>Từ khóa</label>
                            <input
                                v-model.trim="filters.keyword"
                                type="text"
                                placeholder="Nhập tên đơn vị, mã hoặc tên chỉ tiêu"
                            />
                        </div>

                        <div class="form-group actions">
                            <button class="btn btn-primary" type="button" @click="reloadData">Tải dữ liệu</button>
                            <button class="btn btn-secondary" type="button" @click="resetFilters">Đặt lại</button>
                            <button class="btn btn-success" type="button" @click="exportCsv" :disabled="rankedRows.length === 0">
                                Xuất CSV
                            </button>
                        </div>
                    </div>
                </div>

                <div class="summary-grid">
                    <div class="summary-card summary-highlight">
                        <span class="label">Nhóm đang xem</span>
                        <strong>{{ selectedGroup?.tenNhom || '-' }}</strong>
                        <small>{{ selectedGroup?.moTa || 'Chưa có mô tả nhóm' }}</small>
                    </div>
                    <div class="summary-card">
                        <span class="label">Số đơn vị trong nhóm</span>
                        <strong>{{ rankedRows.length }}</strong>
                    </div>
                    <div class="summary-card">
                        <span class="label">Số chỉ tiêu chi tiết áp dụng</span>
                        <strong>{{ selectedGroup?.chiTieus?.length || 0 }}</strong>
                    </div>
                    <div class="summary-card">
                        <span class="label">Tổng KPI được tính</span>
                        <strong>{{ totalKpi }}</strong>
                    </div>
                    <div class="summary-card">
                        <span class="label">Đơn vị dẫn đầu</span>
                        <strong>{{ rankedRows[0]?.tenDonVi || '-' }}</strong>
                    </div>
                </div>

                <div class="table-card">
                    <div class="table-head">
                        <div class="table-note">
                            Bảng này chỉ tổng hợp các dòng KPI có <strong>đơn vị thuộc nhóm</strong> và
                            <strong>chỉ tiêu chi tiết nằm trong cấu hình nhóm</strong>.
                        </div>

                        <div class="table-tools">
                            <button class="icon-btn" type="button" @click="showColumnMenu = !showColumnMenu" title="Tùy chọn cột hiển thị">
                                <i class="bi bi-layout-three-columns"></i>
                            </button>

                            <div v-if="showColumnMenu" class="column-menu">
                                <div class="column-menu-title">Cột hiển thị</div>
                                <label
                                    v-for="column in columnOptions"
                                    :key="column.key"
                                    class="column-option"
                                    :class="{ locked: column.locked }"
                                >
                                    <input v-model="visibleColumns[column.key]" type="checkbox" :disabled="column.locked" />
                                    <span>{{ column.label }}</span>
                                </label>
                            </div>
                        </div>
                    </div>

                    <div v-if="combinedLoading" class="state loading">Đang tải dữ liệu...</div>
                    <div v-else-if="combinedError" class="state error">{{ combinedError }}</div>
                    <div v-else-if="!selectedGroup" class="state empty">Chưa có nhóm thi đua để hiển thị</div>
                    <div v-else-if="rankedRows.length === 0" class="state empty">Không có dữ liệu phù hợp cho nhóm đang chọn</div>
                    <div v-else class="table-wrapper">
                        <ColumnVisibilityTools table-id="NhomThiDuaPage-table" />
                            <table id="NhomThiDuaPage-table" class="managed-table">
                            <thead>
                                <tr>
                                    <th v-if="visibleColumns.ranking">Hạng</th>
                                    <th v-if="visibleColumns.unit">Đơn vị</th>
                                    <th v-if="visibleColumns.soKpi">Số KPI</th>
                                    <th v-if="visibleColumns.hoanThanh">Hoàn thành / Vượt</th>
                                    <th v-if="visibleColumns.chuaHoanThanh">Chưa / Không hoàn thành</th>
                                    <th v-if="visibleColumns.tongMucTieu">Tổng mục tiêu</th>
                                    <th v-if="visibleColumns.tongLuyKe">Tổng lũy kế</th>
                                    <th v-if="visibleColumns.avgCompletion">% hoàn thành TB</th>
                                    <th v-if="visibleColumns.dotGiaoGanNhat">Đợt giao gần nhất</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="item in rankedRows" :key="item.uniqueKey">
                                    <td v-if="visibleColumns.ranking" class="text-center">
                                        <span class="rank-badge">{{ item.ranking }}</span>
                                    </td>
                                    <td v-if="visibleColumns.unit">
                                        <div class="fw-semibold">{{ item.tenDonVi }}</div>
                                            <div class="sub-label">{{ item.soKpi }} KPI được tính trong nhóm</div>
                                    </td>
                                    <td v-if="visibleColumns.soKpi" class="text-center">{{ item.soKpi }}</td>
                                    <td v-if="visibleColumns.hoanThanh" class="text-center">{{ item.hoanThanhDatChuan }}</td>
                                    <td v-if="visibleColumns.chuaHoanThanh" class="text-center">{{ item.chuaHoanThanhTong }}</td>
                                    <td v-if="visibleColumns.tongMucTieu" class="text-right">{{ formatNumber(item.tongMucTieu) }}</td>
                                    <td v-if="visibleColumns.tongLuyKe" class="text-right">{{ formatNumber(item.tongLuyKe) }}</td>
                                    <td v-if="visibleColumns.avgCompletion" class="text-right">{{ formatPercent(item.tyLeHoanThanhTrungBinh) }}</td>
                                    <td v-if="visibleColumns.dotGiaoGanNhat">{{ item.dotGiaoGanNhat || '-' }}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </BaseLayout>
</template>

<script setup>
    import { computed, onMounted, reactive, ref, watch } from 'vue'
    import BaseLayout from '../BaseLayout.vue'
import ColumnVisibilityTools from '../shared/ColumnVisibilityTools.vue'
    import { apiRequest } from '../../services/api'
    import { useBaoCaoTongHopPage } from './baoCaoTongHopPageState.js'

    const {
        loading,
        errorMessage,
        kyBaoCaoOptions,
        filters: reportFilters,
        groupedRows,
        fetchBaoCaoTongHop,
        getId,
        getKyLabel,
        formatNumber,
        formatPercent
    } = useBaoCaoTongHopPage()

    const groups = ref([])
    const groupLoading = ref(false)
    const groupError = ref('')
    const showColumnMenu = ref(false)

    const filters = reactive({
        groupId: '',
        dotGiaoChiTieu: '',
        keyword: ''
    })

    const columnOptions = [
        { key: 'ranking', label: 'Hạng', locked: true },
        { key: 'unit', label: 'Đơn vị', locked: true },
        { key: 'soKpi', label: 'Số KPI' },
        { key: 'hoanThanh', label: 'Hoàn thành / Vượt' },
        { key: 'chuaHoanThanh', label: 'Chưa / Không hoàn thành' },
        { key: 'tongMucTieu', label: 'Tổng mục tiêu' },
        { key: 'tongLuyKe', label: 'Tổng lũy kế' },
        { key: 'avgCompletion', label: '% hoàn thành TB' },
        { key: 'dotGiaoGanNhat', label: 'Đợt giao gần nhất' }
    ]

    const visibleColumns = reactive({
        ranking: true,
        unit: true,
        soKpi: true,
        hoanThanh: true,
        chuaHoanThanh: true,
        tongMucTieu: true,
        tongLuyKe: true,
        avgCompletion: true,
        dotGiaoGanNhat: true
    })

    const selectedGroup = computed(() => groups.value.find(item => String(item.id) === String(filters.groupId)) || null)

    const dotGiaoOptions = computed(() => {
        return [...new Set(groupedRows.value.map(item => item.tenDotGiaoChiTieu).filter(Boolean))].sort((a, b) =>
            a.localeCompare(b, 'vi')
        )
    })

    const filteredGroupRows = computed(() => {
        if (!selectedGroup.value) {
            return []
        }

        const unitIds = new Set((selectedGroup.value.donVis || []).map(item => Number(item.donViId || item.id || 0)))
        const criteriaIds = new Set((selectedGroup.value.chiTieus || []).map(item => Number(item.danhMucChiTieuId || item.id || 0)))
        const keyword = normalizeText(filters.keyword)

        return groupedRows.value.filter(item => {
            if (!unitIds.has(Number(item.donViNhanId || 0))) {
                return false
            }

            if (!criteriaIds.has(Number(item.danhMucChiTieuId || 0))) {
                return false
            }

            if (filters.dotGiaoChiTieu && item.tenDotGiaoChiTieu !== filters.dotGiaoChiTieu) {
                return false
            }

            if (keyword) {
                const haystack = [
                    item.tenDonViNhan,
                    item.maChiTieu,
                    item.tenChiTieu,
                    item.tenDotGiaoChiTieu
                ]
                    .map(normalizeText)
                    .join(' ')

                if (!haystack.includes(keyword)) {
                    return false
                }
            }

            return true
        })
    })

    const rankedRows = computed(() => {
        const grouped = new Map()

        filteredGroupRows.value.forEach(item => {
            const key = String(item.donViNhanId || item.tenDonViNhan || item.groupKey)
            if (!grouped.has(key)) {
                grouped.set(key, {
                    uniqueKey: key,
                    tenDonVi: item.tenDonViNhan || '-',
                    soKpi: 0,
                    tongMucTieu: 0,
                    tongLuyKe: 0,
                    tongTyLeHoanThanh: 0,
                    hoanThanh: 0,
                    hoanThanhVuotMuc: 0,
                    chuaHoanThanh: 0,
                    khongHoanThanh: 0,
                    dotGiaoGanNhat: item.tenDotGiaoChiTieu || '-'
                })
            }

            const target = grouped.get(key)
            target.soKpi += 1
            target.tongMucTieu += Number(item.giaTriMucTieu || 0)
            target.tongLuyKe += Number(item.giaTriLuyKeHienTai || 0)
            target.tongTyLeHoanThanh += Number(item.tyLeHoanThanh || 0)

            if (item.xepLoai === 'HOAN_THANH') target.hoanThanh += 1
            if (item.xepLoai === 'HOAN_THANH_VUOT_MUC') target.hoanThanhVuotMuc += 1
            if (item.xepLoai === 'CHUA_HOAN_THANH') target.chuaHoanThanh += 1
            if (item.xepLoai === 'KHONG_HOAN_THANH') target.khongHoanThanh += 1
        })

        return [...grouped.values()]
            .map(item => ({
                ...item,
                tyLeHoanThanhTrungBinh: item.soKpi ? item.tongTyLeHoanThanh / item.soKpi : 0,
                hoanThanhDatChuan: item.hoanThanh + item.hoanThanhVuotMuc,
                chuaHoanThanhTong: item.chuaHoanThanh + item.khongHoanThanh
            }))
            .sort((left, right) => {
                if (right.tyLeHoanThanhTrungBinh !== left.tyLeHoanThanhTrungBinh) {
                    return right.tyLeHoanThanhTrungBinh - left.tyLeHoanThanhTrungBinh
                }

                if (right.hoanThanhDatChuan !== left.hoanThanhDatChuan) {
                    return right.hoanThanhDatChuan - left.hoanThanhDatChuan
                }

                if (left.chuaHoanThanhTong !== right.chuaHoanThanhTong) {
                    return left.chuaHoanThanhTong - right.chuaHoanThanhTong
                }

                return left.tenDonVi.localeCompare(right.tenDonVi, 'vi')
            })
            .map((item, index) => ({
                ...item,
                ranking: index + 1
            }))
    })

    const totalKpi = computed(() => rankedRows.value.reduce((sum, item) => sum + item.soKpi, 0))
    const combinedLoading = computed(() => loading.value || groupLoading.value)
    const combinedError = computed(() => groupError.value || errorMessage.value || '')

    watch(
        groups,
        value => {
            if (!value.length) {
                filters.groupId = ''
                return
            }

            const stillExists = value.some(item => String(item.id) === String(filters.groupId))
            if (!stillExists) {
                filters.groupId = String(value[0].id)
            }
        },
        { immediate: true }
    )

    onMounted(async () => {
        await fetchGroups()
    })

    async function fetchGroups() {
        groupLoading.value = true
        groupError.value = ''

        try {
            groups.value = await apiRequest('/NhomThiDua')
        } catch (error) {
            groupError.value = error.message || 'Không thể tải nhóm thi đua.'
        } finally {
            groupLoading.value = false
        }
    }

    async function reloadData() {
        await Promise.all([fetchGroups(), fetchBaoCaoTongHop()])
    }

    function resetFilters() {
        reportFilters.kyBaoCaoKPIId = ''
        filters.dotGiaoChiTieu = ''
        filters.keyword = ''
        if (groups.value.length) {
            filters.groupId = String(groups.value[0].id)
        }
        fetchBaoCaoTongHop()
    }

    function exportCsv() {
        const rows = [
            [
                'Hạng',
                'Đơn vị',
                'So KPI',
                'Hoàn thành/Vượt',
                'Chưa/Không hoàn thành',
                'Tổng mục tiêu',
                'Tổng lũy kế',
                'Tỷ lệ hoàn thành TB',
                'Đợt giao gần nhất'
            ],
            ...rankedRows.value.map(item => [
                item.ranking,
                item.tenDonVi,
                item.soKpi,
                item.hoanThanhDatChuan,
                item.chuaHoanThanhTong,
                item.tongMucTieu,
                item.tongLuyKe,
                item.tyLeHoanThanhTrungBinh,
                item.dotGiaoGanNhat
            ])
        ]

        const csv = rows
            .map(row => row.map(value => `"${String(value ?? '').replace(/"/g, '""')}"`).join(','))
            .join('\n')

        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' })
        const link = document.createElement('a')
        link.href = URL.createObjectURL(blob)
        link.setAttribute('download', 'nhom-thi-dua.csv')
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
    }

    function normalizeText(value) {
        return String(value || '')
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .replace(/đ/g, 'd')
            .replace(/Đ/g, 'D')
            .toLowerCase()
    }
</script>

<style scoped>
    .page-wrap {
        min-height: 100vh;
        background: linear-gradient(180deg, #f8fbff 0%, #eef5fb 100%);
    }

    .hero-card,
    .filter-card,
    .table-card,
    .summary-card {
        background: #fff;
        border-radius: 20px;
        border: 1px solid rgba(15, 23, 42, 0.08);
        box-shadow: 0 14px 32px rgba(15, 23, 42, 0.06);
    }

    .hero-card {
          padding: 24px;
          margin-bottom: 24px;
      }

    .hero-head {
        display: flex;
        align-items: center;
        gap: 18px;
    }

    .hero-icon {
        width: 64px;
        height: 64px;
        border-radius: 18px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: linear-gradient(180deg, #03943f 0%, #05ce48dd 100%);
        color: #fff;
        font-size: 30px;
        box-shadow: 0 10px 24px rgba(3, 148, 63, 0.22);
    }

    .hero-kicker {
        font-size: 0.82rem;
        font-weight: 800;
        color: #0d6efd;
        letter-spacing: 0.08em;
        text-transform: uppercase;
    }

    .hero-card h1 {
        margin: 6px 0 8px;
        font-size: 1.8rem;
        font-weight: 800;
        color: #0f172a;
    }

    .hero-card p {
        margin: 0;
        color: #475569;
    }

    .filter-card,
    .table-card {
        padding: 24px;
        margin-bottom: 24px;
    }

    .filter-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
        gap: 16px;
        align-items: end;
    }

    .form-group label {
        display: block;
        margin-bottom: 8px;
        font-weight: 700;
        color: #0f172a;
    }

    .form-group input,
    .form-group select {
        width: 100%;
        min-height: 46px;
        border-radius: 14px;
        border: 1px solid #cbd5e1;
        padding: 0 14px;
        background: #fff;
        color: #0f172a;
    }

    .actions {
        display: flex;
        gap: 10px;
        flex-wrap: wrap;
    }

    .btn {
        border: none;
        border-radius: 14px;
        min-height: 46px;
        padding: 0 16px;
        font-weight: 700;
        cursor: pointer;
    }

    .btn-primary {
        background: #0d6efd;
        color: #fff;
    }

    .btn-secondary {
        background: #e2e8f0;
        color: #0f172a;
    }

    .btn-success {
        background: #198754;
        color: #fff;
    }

    .summary-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
        gap: 16px;
        margin-bottom: 24px;
    }

    .summary-card {
        padding: 18px 20px;
        display: flex;
        flex-direction: column;
        gap: 8px;
    }

    .summary-highlight {
        background: linear-gradient(135deg, #eff6ff 0%, #ffffff 100%);
        border-color: #bfdbfe;
    }

    .summary-card small {
        color: #64748b;
    }

    .label {
        color: #64748b;
        font-size: 0.88rem;
    }

    .summary-card strong {
        color: #0f172a;
        font-size: 1.4rem;
    }

    .table-head {
        display: flex;
        justify-content: space-between;
        gap: 16px;
        align-items: flex-start;
        margin-bottom: 16px;
    }

    .table-note {
        flex: 1;
        padding: 12px 14px;
        border-radius: 14px;
        background: #eff6ff;
        color: #1e3a8a;
    }

    .table-tools {
        position: relative;
    }

    .icon-btn {
        width: 46px;
        height: 46px;
        display: grid;
        place-items: center;
        border-radius: 14px;
        border: 1px solid #cbd5e1;
        background: #fff;
        color: #0f172a;
        cursor: pointer;
    }

    .column-menu {
        position: absolute;
        right: 0;
        top: calc(100% + 10px);
        width: 260px;
        padding: 14px;
        border-radius: 18px;
        border: 1px solid #dbeafe;
        background: #fff;
        box-shadow: 0 18px 40px rgba(15, 23, 42, 0.12);
        z-index: 20;
    }

    .column-menu-title {
        margin-bottom: 10px;
        font-weight: 800;
        color: #0f172a;
    }

    .column-option {
        display: flex;
        align-items: center;
        gap: 10px;
        min-height: 42px;
        padding: 10px 12px;
        border-radius: 14px;
        background: #fff;
        border: 1px solid #dbeafe;
        color: #0f172a;
        font-weight: 600;
        cursor: pointer;
        margin-bottom: 8px;
    }

    .column-option:last-child {
        margin-bottom: 0;
    }

    .column-option.locked {
        background: #eff6ff;
        border-color: #bfdbfe;
    }

    .table-wrapper {
        overflow-x: auto;
    }

    table {
        width: 100%;
        border-collapse: collapse;
        min-width: 960px;
    }

    th,
    td {
        padding: 14px 16px;
        border-bottom: 1px solid #e2e8f0;
        vertical-align: middle;
    }

    th {
        position: sticky;
        top: 0;
        background: #f8fafc;
        color: #334155;
        font-size: 0.85rem;
        text-transform: uppercase;
        letter-spacing: 0.04em;
    }

    tbody tr:hover {
        background: #f8fbff;
    }

    .rank-badge {
        display: inline-grid;
        place-items: center;
        width: 36px;
        height: 36px;
        border-radius: 999px;
        background: linear-gradient(135deg, #0d6efd 0%, #5aa9ff 100%);
        color: #fff;
        font-weight: 800;
    }

    .sub-label {
        margin-top: 4px;
        color: #64748b;
        font-size: 0.84rem;
    }

    .text-center {
        text-align: center;
    }

    .text-right {
        text-align: right;
    }

    .state {
        padding: 24px 18px;
        border-radius: 16px;
        font-weight: 700;
    }

    .loading {
        background: #eff6ff;
        color: #1d4ed8;
    }

    .error {
        background: #fef2f2;
        color: #b91c1c;
    }

    .empty {
        background: #f8fafc;
        color: #475569;
    }

    @media (max-width: 768px) {
        .filter-card,
        .table-card,
        .hero-card {
            padding: 18px;
        }

        .hero-head {
            align-items: flex-start;
        }

        .actions {
            display: grid;
            grid-template-columns: 1fr;
        }

        .btn,
        .icon-btn {
            width: 100%;
        }

        .table-head {
            flex-direction: column;
        }
    }
</style>



