<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-clipboard2-data"></i>
                    </div>
                    <div class="gov-text">
                        <div class="wave-title">HỆ THỐNG THEO DÕI CHỈ TIÊU CÔNG TÁC</div>
                        <div class="gov-title">BÁO CÁO CHỈ TIÊU CÔNG AN THÀNH PHỐ</div>
                        <div class="gov-sub">
                            Danh sách chi tiết và kết quả đánh giá mới nhất của các chỉ tiêu do Công an thành phố Đà Nẵng trực tiếp thực hiện.
                        </div>
                    </div>
                </div>

                <div class="filter-card">
                    <div class="filter-grid">
                        <div class="form-group">
                            <label>Kỳ báo cáo</label>
                            <select v-model="filters.kyBaoCaoKPIId" @change="fetchData">
                                <option value="">-- Lấy đánh giá mới nhất --</option>
                                <option v-for="item in kyBaoCaoOptions" :key="item.id" :value="String(item.id)">
                                    {{ buildKyLabel(item) }}
                                </option>
                            </select>
                        </div>

                        <div class="form-group">
                            <label>Xếp loại</label>
                            <select v-model="filters.xepLoai">
                                <option value="">-- Tất cả xếp loại --</option>
                                <option v-for="status in xepLoaiOptions" :key="status.value" :value="status.value">
                                    {{ status.label }}
                                </option>
                            </select>
                        </div>

                        <div class="form-group keyword-group">
                            <label>Từ khóa</label>
                            <input
                                v-model.trim="filters.keyword"
                                type="text"
                                placeholder="Nhập mã chỉ tiêu, tên chỉ tiêu, đợt giao hoặc đơn vị nhận"
                            />
                        </div>

                        <div class="form-group actions">
                            <button class="btn btn-primary" @click="fetchData">Tải dữ liệu</button>
                            <button class="btn btn-secondary" @click="resetFilters">Đặt lại</button>
                        </div>
                    </div>
                </div>

                <div class="summary-grid">
                    <div class="summary-card">
                        <span class="label">Tổng số chỉ tiêu</span>
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
                    <div class="summary-card">
                        <span class="label">Chưa đánh giá</span>
                        <strong>{{ chuaDanhGiaCount }}</strong>
                    </div>
                </div>

                <div class="table-card">
                    <div class="table-toolbar">
                        <button class="btn btn-success" @click="exportCsv">Xuất CSV</button>
                    </div>

                    <div v-if="loading" class="state loading">Đang tải dữ liệu...</div>
                    <div v-else-if="errorMessage" class="state error">{{ errorMessage }}</div>
                    <div v-else class="table-wrapper">
                        <ColumnVisibilityTools table-id="BaoCaoChiTieuCongAnThanhPhoPage-table" />
                            <table id="BaoCaoChiTieuCongAnThanhPhoPage-table" class="managed-table">
                            <thead>
                                <tr>
                                    <th>STT</th>
                                    <th>Tên chỉ tiêu</th>
                                    <th>Đơn vị nhận</th>
                                    <th>Đợt giao chỉ tiêu</th>
                                    <th>Kỳ báo cáo</th>
                                    <th>Giá trị mục tiêu</th>
                                    <th>Kỳ đánh giá</th>
                                    <th>Kết quả đánh giá</th>
                                    <th>Nhận xét</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-if="filteredRows.length === 0">
                                    <td colspan="9" class="empty-cell">Không có dữ liệu</td>
                                </tr>

                                <tr v-for="(row, index) in filteredRows" :key="row.id">
                                    <td>{{ index + 1 }}</td>
                                    <td>
                                        <div class="fw-semibold">{{ row.tenChiTieu || '-' }}</div>
                                        <div v-if="row.tenChiTieuCha" class="sub-label">
                                            Thuộc chỉ tiêu cha: {{ row.tenChiTieuCha }}
                                        </div>
                                    </td>
                                    <td>{{ row.tenDonViNhan || '-' }}</td>
                                    <td>{{ row.tenDotGiaoChiTieu || row.maDotGiao || '-' }}</td>
                                    <td>{{ formatKyBaoCao(row.tanSuatBaoCao) }}</td>
                                    <td class="text-right">{{ formatTarget(row) }}</td>
                                    <td>{{ row.tenKy || row.maKy || 'Chưa có kỳ đánh giá' }}</td>
                                    <td>
                                        <span class="badge" :class="badgeClass(row.xepLoai)">
                                            {{ getDanhGiaLabel(row.xepLoai) || 'Chưa đánh giá' }}
                                        </span>
                                    </td>
                                    <td>{{ row.nhanXetDanhGia || '-' }}</td>
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
    import { computed, onMounted } from 'vue'
    import BaseLayout from '../BaseLayout.vue'
import ColumnVisibilityTools from '../shared/ColumnVisibilityTools.vue'
    import {
        countTrackedStatuses,
        DANH_GIA_STATUS_OPTIONS,
        getDanhGiaBadgeClass,
        getDanhGiaLabel,
        getDanhGiaStatusCode
    } from '../../utils/danhGiaStatusClean.js'
    import {
        buildKyLabel,
        formatTarget,
        normalizeText,
        useCatpKpiData
    } from '../../composables/useCatpKpiData.js'

    const {
        errorMessage,
        fetchData,
        filters,
        kyBaoCaoOptions,
        loading,
        reportRows,
        resetFilters
    } = useCatpKpiData()

    const xepLoaiOptions = [
        ...DANH_GIA_STATUS_OPTIONS,
        { value: 'CHUA_DANH_GIA', label: 'Chưa đánh giá' }
    ]

    onMounted(fetchData)

    const filteredRows = computed(() => {
        let rows = [...reportRows.value]

        if (filters.xepLoai) {
            rows = rows.filter(item => getDanhGiaStatusCode(item.xepLoai) === filters.xepLoai)
        }

        if (filters.keyword) {
            const keyword = normalizeText(filters.keyword)
            rows = rows.filter(item =>
                [
                    item.maChiTieu,
                    item.tenChiTieu,
                    item.tenChiTieuCha,
                    item.tenDonViNhan,
                    item.tenDotGiaoChiTieu,
                    item.maDotGiao,
                    item.maKy,
                    item.tenKy,
                    item.ketQua,
                    item.nhanXetDanhGia
                ]
                    .filter(Boolean)
                    .some(value => normalizeText(value).includes(keyword))
            )
        }

        return rows.sort((left, right) => {
            const leftValue = `${left.tenDotGiaoChiTieu || ''} ${left.maChiTieu || ''}`.toLowerCase()
            const rightValue = `${right.tenDotGiaoChiTieu || ''} ${right.maChiTieu || ''}`.toLowerCase()
            return leftValue.localeCompare(rightValue, 'vi')
        })
    })

    const thongKe = computed(() => countTrackedStatuses(filteredRows.value, item => item.xepLoai))

    const chuaDanhGiaCount = computed(() => {
        return filteredRows.value.filter(item => getDanhGiaStatusCode(item.xepLoai) === 'CHUA_DANH_GIA').length
    })

    function badgeClass(xepLoai) {
        return getDanhGiaBadgeClass(xepLoai)
    }

    function formatKyBaoCao(value) {
        const map = {
            THANG: 'Tháng',
            QUY: 'Quý',
            '6THANG': '6 tháng',
            NAM: 'Năm'
        }

        return map[value] || value || '-'
    }

    function exportCsv() {
        const headers = [
            'Mã chỉ tiêu',
            'Tên chỉ tiêu',
            'Đơn vị nhận',
            'Đợt giao chỉ tiêu',
            'Kỳ báo cáo',
            'Giá trị mục tiêu',
            'Mã kỳ',
            'Tên kỳ',
            'Xếp loại',
            'Kết quả',
            'Nhận xét'
        ]

        const csvRows = filteredRows.value.map(item => [
            item.maChiTieu || '',
            item.tenChiTieu || '',
            item.tenDonViNhan || '',
            item.tenDotGiaoChiTieu || item.maDotGiao || '',
            formatKyBaoCao(item.tanSuatBaoCao),
            formatTarget(item),
            item.maKy || '',
            item.tenKy || '',
            getDanhGiaLabel(item.xepLoai) || '',
            item.ketQua || '',
            item.nhanXetDanhGia || ''
        ])

        const csvContent = [headers, ...csvRows]
            .map(row => row.map(value => `"${String(value).replace(/"/g, '""')}"`).join(','))
            .join('\n')

        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.setAttribute('download', 'bao-cao-chi-tieu-cong-an-thanh-pho.csv')
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

    .keyword-group {
        grid-column: span 1;
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
        grid-template-columns: repeat(6, minmax(0, 1fr));
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

    .fw-semibold {
        font-weight: 700;
    }

    .sub-label {
        margin-top: 4px;
        font-size: 12px;
        color: #6b7280;
        white-space: normal;
    }

    .text-right {
        text-align: right;
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
    }
</style>


