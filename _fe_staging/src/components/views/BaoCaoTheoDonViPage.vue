<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-buildings"></i>
                    </div>
                    <div class="gov-text">
                        <div class="gov-title">BÁO CÁO THEO ĐƠN VỊ</div>
                        <div class="gov-sub">
                            Tổng hợp kết quả đánh giá KPI theo từng đơn vị nhận chỉ tiêu để theo dõi chất lượng thực hiện và
                            xác định nhóm đơn vị cần chú ý.
                        </div>
                    </div>
                </div>

                <div class="bao-cao-don-vi-page">
                    <div class="filter-card">
                        <div class="filter-grid">
                            <div class="form-group">
                                <label>Kỳ báo cáo</label>
                                <select v-model="baseFilters.kyBaoCaoKPIId" @change="fetchBaoCaoTongHop">
                                    <option value="">-- Tất cả kỳ báo cáo --</option>
                                    <option v-for="item in kyBaoCaoOptions" :key="item.id" :value="item.id">
                                        {{ item.tenKy }}
                                    </option>
                                </select>
                            </div>

                            <div class="form-group">
                                <label>Trạng thái nổi bật</label>
                                <select v-model="filters.xepLoaiNoiBat">
                                    <option value="">-- Tất cả trạng thái --</option>
                                    <option v-for="status in trackedStatusOptions" :key="status.value" :value="status.value">
                                        {{ status.label }}
                                    </option>
                                </select>
                            </div>

                            <div class="form-group">
                                <label>Từ khóa</label>
                                <input
                                    v-model.trim="filters.keyword"
                                    type="text"
                                    placeholder="Tên đơn vị, chỉ tiêu nổi bật, chỉ tiêu cần chú ý..."
                                />
                            </div>

                            <div class="form-group actions">
                                <button class="btn btn-primary" @click="fetchBaoCaoTongHop">Tải dữ liệu</button>
                                <button class="btn btn-secondary" @click="resetFilters">Đặt lại</button>
                            </div>
                        </div>
                    </div>

                    <div class="summary-grid">
                        <div class="summary-card">
                            <span class="label">Tổng số đơn vị</span>
                            <strong>{{ filteredRows.length }}</strong>
                        </div>
                        <div class="summary-card">
                            <span class="label">Hoàn thành vượt mức</span>
                            <strong>{{ thongKe.HOAN_THANH_VUOT_MUC }}</strong>
                        </div>
                        <div class="summary-card">
                            <span class="label">Hoàn thành</span>
                            <strong>{{ thongKe.HOAN_THANH }}</strong>
                        </div>
                        <div class="summary-card">
                            <span class="label">Chưa hoàn thành</span>
                            <strong>{{ thongKe.CHUA_HOAN_THANH }}</strong>
                        </div>
                        <div class="summary-card">
                            <span class="label">Không hoàn thành</span>
                            <strong>{{ thongKe.KHONG_HOAN_THANH }}</strong>
                        </div>
                    </div>

                    <div class="table-card">
                        <div class="table-toolbar">
                            <button class="btn btn-success" @click="exportCsv">Xuất CSV</button>
                        </div>

                        <div v-if="loading" class="state loading">Đang tải dữ liệu...</div>
                        <div v-else-if="errorMessage" class="state error">{{ errorMessage }}</div>
                        <div v-else class="table-wrapper">
                            <ColumnVisibilityTools table-id="BaoCaoTheoDonViPage-table" />
                            <table id="BaoCaoTheoDonViPage-table" class="managed-table">
                                <thead>
                                    <tr>
                                        <th>STT</th>
                                        <th>Đơn vị</th>
                                        <th>Tổng KPI</th>
                                        <th>% hoàn thành bình quân</th>
                                        <th>Hoàn thành vượt mức</th>
                                        <th>Hoàn thành</th>
                                        <th>Chưa hoàn thành</th>
                                        <th>Không hoàn thành</th>
                                        <th>KPI cần chú ý</th>
                                        <th>Trạng thái nổi bật</th>
                                        <th>Chỉ tiêu nổi bật</th>
                                        <th>Chỉ tiêu cần chú ý nhất</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-if="filteredRows.length === 0">
                                        <td colspan="12" class="empty-cell">Không có dữ liệu</td>
                                    </tr>
                                    <tr v-for="(row, index) in filteredRows" :key="row.tenDonVi || index">
                                        <td>{{ index + 1 }}</td>
                                        <td class="unit-cell">
                                            <div class="unit-name">{{ row.tenDonVi || '-' }}</div>
                                            <div class="unit-sub">{{ row.tenKyApDung || 'Tất cả kỳ báo cáo' }}</div>
                                        </td>
                                        <td class="text-center">{{ row.tongChiTieu }}</td>
                                        <td class="text-right">{{ formatPercent(row.tyLeHoanThanhBinhQuan) }}</td>
                                        <td class="text-center">{{ row.statusCounts.HOAN_THANH_VUOT_MUC }}</td>
                                        <td class="text-center">{{ row.statusCounts.HOAN_THANH }}</td>
                                        <td class="text-center">{{ row.statusCounts.CHUA_HOAN_THANH }}</td>
                                        <td class="text-center">{{ row.statusCounts.KHONG_HOAN_THANH }}</td>
                                        <td class="text-center text-danger">{{ row.kpiCanChuY }}</td>
                                        <td>
                                            <span class="badge" :class="badgeClass(row.xepLoaiNoiBat)">
                                                {{ getDanhGiaLabel(row.xepLoaiNoiBat) || '-' }}
                                            </span>
                                        </td>
                                        <td>{{ row.chiTieuNoiBat || '-' }}</td>
                                        <td>{{ row.chiTieuCanChuYNhat || '-' }}</td>
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
    import { computed, reactive } from 'vue'
    import BaseLayout from '../BaseLayout.vue'
import ColumnVisibilityTools from '../shared/ColumnVisibilityTools.vue'
    import { useBaoCaoTongHopPage } from './baoCaoTongHopPageState.js'
    import {
        countTrackedStatuses,
        createTrackedStatusCounter,
        DANH_GIA_TRACKED_STATUS_OPTIONS,
        DANH_GIA_TRACKED_STATUS_ORDER,
        getDanhGiaBadgeClass,
        getDanhGiaLabel,
        getDanhGiaRank
    } from '../../utils/danhGiaStatusClean.js'

    const trackedStatusOptions = DANH_GIA_TRACKED_STATUS_OPTIONS

    const {
        loading,
        errorMessage,
        kyBaoCaoOptions: baseKyBaoCaoOptions,
        groupedRows,
        filters: baseFilters,
        fetchBaoCaoTongHop
    } = useBaoCaoTongHopPage()

    const filters = reactive({
        xepLoaiNoiBat: '',
        keyword: ''
    })

    const kyBaoCaoOptions = computed(() => {
        return baseKyBaoCaoOptions.value.map(item => ({
            id: Number(item?.id ?? item?.Id ?? 0),
            tenKy: item?.tenKy ?? item?.TenKy ?? '-'
        }))
    })

    const reportRows = computed(() => {
        const grouped = new Map()
        const selectedKy = kyBaoCaoOptions.value.find(item => String(item.id) === String(baseFilters.kyBaoCaoKPIId))
        const tenKyApDung = selectedKy?.tenKy || 'Tính đến kỳ mới nhất hiện có'

        groupedRows.value.forEach(item => {
            const key = item.tenDonViNhan || 'Chưa xác định'

            if (!grouped.has(key)) {
                grouped.set(key, {
                    tenDonVi: key,
                    tenKyApDung,
                    tongChiTieu: 0,
                    tongTyLeHoanThanh: 0,
                    soKpiCoTyLe: 0,
                    statusCounts: createTrackedStatusCounter(),
                    topItem: null,
                    bottomItem: null
                })
            }

            const group = grouped.get(key)
            const tyLeHoanThanh = toNumber(item.tyLeHoanThanh)

            group.tongChiTieu += 1

            if (Number.isFinite(tyLeHoanThanh)) {
                group.tongTyLeHoanThanh += tyLeHoanThanh
                group.soKpiCoTyLe += 1
            }

            if (Object.prototype.hasOwnProperty.call(group.statusCounts, item.xepLoai)) {
                group.statusCounts[item.xepLoai] += 1
            }

            if (!group.topItem || compareByCompletion(item, group.topItem) > 0) {
                group.topItem = item
            }

            if (!group.bottomItem || compareByCompletion(item, group.bottomItem) < 0) {
                group.bottomItem = item
            }
        })

        return Array.from(grouped.values()).map(item => {
            const tyLeHoanThanhBinhQuan =
                item.soKpiCoTyLe > 0 ? item.tongTyLeHoanThanh / item.soKpiCoTyLe : 0
            const xepLoaiNoiBat = getDominantStatus(item.statusCounts)

            return {
                tenDonVi: item.tenDonVi,
                tenKyApDung: item.tenKyApDung,
                tongChiTieu: item.tongChiTieu,
                tyLeHoanThanhBinhQuan,
                statusCounts: item.statusCounts,
                kpiCanChuY: item.statusCounts.CHUA_HOAN_THANH + item.statusCounts.KHONG_HOAN_THANH,
                xepLoaiNoiBat,
                chiTieuNoiBat: buildItemLabel(item.topItem),
                chiTieuCanChuYNhat: buildItemLabel(item.bottomItem)
            }
        })
    })

    const filteredRows = computed(() => {
        let data = [...reportRows.value]

        if (filters.xepLoaiNoiBat) {
            data = data.filter(item => item.xepLoaiNoiBat === filters.xepLoaiNoiBat)
        }

        if (filters.keyword) {
            const keyword = normalizeText(filters.keyword)
            data = data.filter(item =>
                [item.tenDonVi, item.chiTieuNoiBat, item.chiTieuCanChuYNhat, getDanhGiaLabel(item.xepLoaiNoiBat)]
                    .filter(Boolean)
                    .some(value => normalizeText(value).includes(keyword))
            )
        }

        return data.sort((a, b) => {
            if (b.kpiCanChuY !== a.kpiCanChuY) return b.kpiCanChuY - a.kpiCanChuY
            return b.tyLeHoanThanhBinhQuan - a.tyLeHoanThanhBinhQuan
        })
    })

    const thongKe = computed(() => countTrackedStatuses(filteredRows.value, item => item.xepLoaiNoiBat))

    function resetFilters() {
        baseFilters.kyBaoCaoKPIId = ''
        filters.xepLoaiNoiBat = ''
        filters.keyword = ''
    }

    function toNumber(value) {
        if (value === null || value === undefined || value === '') return Number.NaN
        const parsed = Number(value)
        return Number.isNaN(parsed) ? Number.NaN : parsed
    }

    function normalizeText(value) {
        return String(value || '')
            .toLowerCase()
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .replace(/đ/g, 'd')
            .replace(/Đ/g, 'D')
            .trim()
    }

    function compareByCompletion(left, right) {
        const leftValue = toNumber(left?.tyLeHoanThanh)
        const rightValue = toNumber(right?.tyLeHoanThanh)

        if (!Number.isFinite(leftValue) && !Number.isFinite(rightValue)) return 0
        if (!Number.isFinite(leftValue)) return -1
        if (!Number.isFinite(rightValue)) return 1
        return leftValue - rightValue
    }

    function buildItemLabel(item) {
        if (!item) return ''
        const tenDanhMuc = item.tenDanhMucChiTieu || item.tenChiTieu || ''
        const chiTieuGiao = item.tenChiTieuGiao || item.tenChiTieuCha || ''
        return [
            item.maChiTieu,
            tenDanhMuc,
            chiTieuGiao ? `Chỉ tiêu giao: ${chiTieuGiao}` : ''
        ].filter(Boolean).join(' - ')
    }

    function getDominantStatus(statusCounts) {
        return DANH_GIA_TRACKED_STATUS_ORDER.reduce((selectedCode, currentCode) => {
            if (!selectedCode) return currentCode

            const currentCount = statusCounts[currentCode] || 0
            const selectedCount = statusCounts[selectedCode] || 0

            if (currentCount > selectedCount) return currentCode
            if (currentCount === selectedCount && getDanhGiaRank(currentCode) < getDanhGiaRank(selectedCode)) {
                return currentCode
            }

            return selectedCode
        }, '')
    }

    function formatPercent(value) {
        if (value === null || value === undefined || value === '') return '-'
        const parsed = Number(value)
        if (!Number.isFinite(parsed)) return '-'
        return `${parsed.toLocaleString('vi-VN', {
            minimumFractionDigits: 0,
            maximumFractionDigits: 2
        })}%`
    }

    function badgeClass(xepLoai) {
        return getDanhGiaBadgeClass(xepLoai)
    }

    function exportCsv() {
        const headers = [
            'Đơn vị',
            'Tổng KPI',
            '% hoàn thành bình quân',
            'Hoàn thành vượt mức',
            'Hoàn thành',
            'Chưa hoàn thành',
            'Không hoàn thành',
            'KPI cần chú ý',
            'Trạng thái nổi bật',
            'Chỉ tiêu nổi bật',
            'Chỉ tiêu cần chú ý nhất'
        ]

        const csvRows = filteredRows.value.map(item => [
            item.tenDonVi || '',
            item.tongChiTieu ?? 0,
            item.tyLeHoanThanhBinhQuan ?? '',
            item.statusCounts.HOAN_THANH_VUOT_MUC ?? 0,
            item.statusCounts.HOAN_THANH ?? 0,
            item.statusCounts.CHUA_HOAN_THANH ?? 0,
            item.statusCounts.KHONG_HOAN_THANH ?? 0,
            item.kpiCanChuY ?? 0,
            getDanhGiaLabel(item.xepLoaiNoiBat) || '',
            item.chiTieuNoiBat || '',
            item.chiTieuCanChuYNhat || ''
        ])

        const csvContent = [headers, ...csvRows]
            .map(row => row.map(value => `"${String(value).replace(/"/g, '""')}"`).join(','))
            .join('\n')

        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.setAttribute('download', 'bao-cao-theo-don-vi.csv')
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)
    }
</script>

<style scoped>
    .page-wrap {
        min-height: 100vh;
        background: linear-gradient(180deg, #f8fbff 0%, #eef5fb 100%);
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

    .wave-title {
        font-weight: 700;
        letter-spacing: 0.08em;
        font-size: 0.8rem;
        color: #0d6efd;
        margin-bottom: 6px;
        text-transform: uppercase;
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

    .bao-cao-don-vi-page {
        padding: 24px;
        background: #f6f8fb;
        min-height: 100%;
    }

    .filter-card,
    .table-card {
        background: #ffffff;
        border-radius: 12px;
        padding: 16px;
        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
        margin-bottom: 16px;
    }

    .filter-grid {
        display: grid;
        grid-template-columns: repeat(4, minmax(0, 1fr));
        gap: 16px;
        align-items: end;
    }

    .form-group {
        display: flex;
        flex-direction: column;
        gap: 8px;
    }

    .form-group label {
        font-weight: 600;
        color: #374151;
    }

    .form-group input,
    .form-group select {
        height: 40px;
        border: 1px solid #d1d5db;
        border-radius: 8px;
        padding: 0 12px;
        outline: none;
    }

    .form-group input:focus,
    .form-group select:focus {
        border-color: #89d2ef;
        box-shadow: 0 0 0 0.2rem rgba(137, 210, 239, 0.2);
    }

    .actions {
        flex-direction: row;
        gap: 10px;
    }

    .btn {
        height: 40px;
        border: none;
        border-radius: 8px;
        padding: 0 16px;
        cursor: pointer;
        font-weight: 600;
    }

    .btn-primary {
        background: #2563eb;
        color: #fff;
    }

    .btn-secondary {
        background: #e5e7eb;
        color: #111827;
    }

    .btn-success {
        background: #16a34a;
        color: #fff;
    }

    .summary-grid {
        display: grid;
        grid-template-columns: repeat(5, minmax(0, 1fr));
        gap: 16px;
        margin-bottom: 16px;
    }

    .summary-card {
        background: #ffffff;
        border-radius: 12px;
        padding: 16px;
        box-shadow: 0 2px 10px rgba(0, 0, 0, 0.05);
        display: flex;
        flex-direction: column;
        gap: 8px;
    }

    .summary-card .label {
        color: #6b7280;
        font-size: 14px;
    }

    .summary-card strong {
        font-size: 24px;
        color: #111827;
    }

    .table-toolbar {
        display: flex;
        justify-content: flex-end;
        margin-bottom: 12px;
    }

    .table-wrapper {
        width: 100%;
        overflow-x: auto;
    }

    table {
        width: max-content;
        min-width: 100%;
        border-collapse: collapse;
        table-layout: auto;
        border: 1px solid #dcdfe6;
    }

    th,
    td {
        border: 1px solid #dcdfe6;
        padding: 10px 12px;
        vertical-align: middle;
        box-sizing: border-box;
        white-space: nowrap;
    }

    th {
        background: #f5f7fa;
        text-align: center;
        font-weight: 600;
    }

    td {
        text-align: left;
    }

    .unit-cell {
        min-width: 240px;
    }

    .unit-name {
        font-weight: 700;
        color: #1f2937;
    }

    .unit-sub {
        margin-top: 4px;
        font-size: 12px;
        color: #6b7280;
        white-space: normal;
    }

    .text-center {
        text-align: center;
    }

    .text-right {
        text-align: right;
    }

    .text-danger {
        color: #dc2626;
        font-weight: 700;
    }

    .empty-cell {
        text-align: center;
        padding: 20px;
        color: #909399;
    }

    .state {
        padding: 20px;
        border-radius: 8px;
    }

    .state.loading {
        background: #eff6ff;
        color: #1d4ed8;
    }

    .state.error {
        background: #fef2f2;
        color: #dc2626;
    }

    .badge {
        display: inline-block;
        padding: 4px 10px;
        border-radius: 999px;
        font-size: 12px;
        font-weight: 600;
        white-space: nowrap;
    }

    .badge-excellent {
        background: #ecfdf3;
        color: #067647;
    }

    .badge-good {
        background: #eff8ff;
        color: #175cd3;
    }

    .badge-pass {
        background: #fffaeb;
        color: #b54708;
    }

    .badge-fail {
        background: #fef3f2;
        color: #d92d20;
    }

    .badge-default,
    .badge-pending,
    .badge-muted {
        background: #f2f4f7;
        color: #344054;
    }

    @media (max-width: 1200px) {
        .filter-grid,
        .summary-grid {
            grid-template-columns: 1fr;
        }

        .actions {
            flex-direction: column;
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

        .bao-cao-don-vi-page {
            padding: 16px;
        }
    }
</style>



