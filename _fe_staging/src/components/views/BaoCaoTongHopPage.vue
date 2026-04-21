<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-bar-chart-line"></i>
                    </div>
                    <div class="gov-text">
                        <div class="gov-title">BÁO CÁO TỔNG HỢP KPI TOÀN HỆ THỐNG</div>
                        <div class="gov-sub">
                            Mỗi dòng là một chi tiết giao chỉ tiêu, cộng dồn số thực hiện trong các kỳ và lấy kết quả đánh giá gần nhất trong phạm vi lọc.
                        </div>
                    </div>
                </div>

                <div class="bao-cao-page">
                    <div class="filter-card">
                        <div class="filter-grid">
                            <div class="form-group">
                                <label>Kỳ báo cáo</label>
                                <select v-model="filters.kyBaoCaoKPIId" @change="fetchBaoCaoTongHop">
                                    <option value="">-- Tính đến kỳ mới nhất hiện có --</option>
                                    <option v-for="item in kyBaoCaoOptions" :key="getId(item)" :value="getId(item)">
                                        {{ getKyLabel(item) }}
                                    </option>
                                </select>
                            </div>

                            <div class="form-group">
                                <label>Đơn vị</label>
                                <select v-model="filters.donVi">
                                    <option value="">-- Tất cả đơn vị --</option>
                                    <option v-for="item in donViOptions" :key="item" :value="item">
                                        {{ item }}
                                    </option>
                                </select>
                            </div>

                            <div class="form-group">
                                <label>Xếp loại</label>
                                <select v-model="filters.xepLoai">
                                    <option value="">-- Tất cả xếp loại --</option>
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
                                    placeholder="Mã chỉ tiêu, tên chỉ tiêu, đơn vị, đợt giao..."
                                />
                            </div>

                            <div class="form-group actions">
                                <button class="btn btn-primary" @click="fetchBaoCaoTongHop">Tải dữ liệu</button>
                                <button class="btn btn-secondary" @click="resetFilters">Đặt lại</button>
                                <button class="btn btn-success" @click="exportCsv">Xuất CSV</button>
                            </div>
                        </div>
                    </div>

                    <div class="summary-grid">
                        <div class="summary-card">
                            <span class="label">Tổng chi tiết chỉ tiêu</span>
                            <strong>{{ filteredRows.length }}</strong>
                        </div>
                        <div class="summary-card">
                            <span class="label">Tổng thực hiện cộng dồn</span>
                            <strong>{{ formatNumber(tongGiaTriThucHienCongDon) }}</strong>
                        </div>
                        <div class="summary-card">
                            <span class="label">Hoàn thành trung bình</span>
                            <strong>{{ formatPercent(averageCompletion) }}</strong>
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
                        <div v-if="loading" class="state loading">Đang tải dữ liệu...</div>
                        <div v-else-if="errorMessage" class="state error">{{ errorMessage }}</div>
                        <div v-else class="table-wrapper">
                            <ColumnVisibilityTools table-id="BaoCaoTongHopPage-table" />
                            <table id="BaoCaoTongHopPage-table" class="managed-table">
                                <thead>
                                    <tr>
                                        <th>STT</th>
                                        <th>Danh mục chỉ tiêu</th>
                                        <th>Chỉ tiêu giao</th>
                                        <th>Đơn vị</th>
                                        <th>Đợt giao chỉ tiêu</th>
                                        <th>Kỳ gần nhất</th>
                                        <th>Giá trị mục tiêu</th>
                                        <th>Cuối kỳ gần nhất</th>
                                        <th>Lũy kế hiện tại</th>
                                        <th>Số dư mục tiêu</th>
                                        <th>% hoàn thành</th>
                                        <th>Đánh giá</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-if="filteredRows.length === 0">
                                        <td colspan="13" class="empty-cell">Không có dữ liệu</td>
                                    </tr>

                                    <tr v-for="(row, index) in filteredRows" :key="row.groupKey">
                                        <td>{{ index + 1 }}</td>
                                        <td>{{ row.tenDanhMucChiTieu || row.tenChiTieu || '-' }}</td>
                                        <td>{{ row.tenChiTieuGiao || row.tenChiTieuCha || '-' }}</td>
                                        <td>{{ row.tenDonViNhan || '-' }}</td>
                                        <td>{{ row.tenDotGiaoChiTieu || '-' }}</td>
                                        <td>{{ row.maKyGanNhat || '-' }} - {{ row.tenKyGanNhat || '-' }}</td>
                                        <td class="text-right">{{ row.isComparisonTarget ? formatPercent(row.giaTriMucTieu) : formatNumber(row.giaTriMucTieu, row.donViTinh) }}</td>
                                        <td class="text-right">{{ formatNumber(row.giaTriCuoiKyGanNhat, row.donViTinh) }}</td>
                                        <td class="text-right">{{ formatNumber(row.giaTriLuyKeHienTai, row.donViTinh) }}</td>
                                        <td class="text-right">{{ row.isComparisonTarget ? formatPercent(row.soDuMucTieu) : formatNumber(row.soDuMucTieu, row.donViTinh) }}</td>
                                        <td class="text-right">{{ formatPercent(row.tyLeHoanThanh) }}</td>
                                        <td>
                                            <span class="badge" :class="badgeClass(row.xepLoai)">
                                                {{ getDanhGiaLabel(row.xepLoai) || '-' }}
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
    import BaseLayout from '../BaseLayout.vue'
import ColumnVisibilityTools from '../shared/ColumnVisibilityTools.vue'
    import { useBaoCaoTongHopPage } from './baoCaoTongHopPageState.js'

    const {
        loading,
        errorMessage,
        kyBaoCaoOptions,
        trackedStatusOptions,
        filters,
        donViOptions,
        filteredRows,
        thongKe,
        tongGiaTriThucHienCongDon,
        averageCompletion,
        fetchBaoCaoTongHop,
        resetFilters,
        getId,
        getKyLabel,
        buildDotGiaoMeta,
        buildLatestKyMeta,
        formatDate,
        formatNumber,
        formatPercent,
        badgeClass,
        getDanhGiaLabel,
        exportCsv
    } = useBaoCaoTongHopPage()
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

    .bao-cao-page {
        padding: 20px;
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
        border-color: #2563eb;
        box-shadow: 0 0 0 0.2rem rgba(37, 99, 235, 0.15);
    }

    .actions {
        flex-direction: row;
        gap: 10px;
        align-items: stretch;
        justify-content: flex-end;
        grid-column: 1 / -1;
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

    .table-wrapper {
        width: 100%;
        overflow-x: auto;
    }

    .table-wrapper table {
        width: max-content;
        min-width: 100%;
        border-collapse: collapse;
        table-layout: auto;
        border: 1px solid #dcdfe6;
    }

    .table-wrapper th,
    .table-wrapper td {
        border: 1px solid #dcdfe6;
        padding: 10px 12px;
        vertical-align: middle;
        box-sizing: border-box;
        white-space: nowrap;
    }

    .table-wrapper th {
        background: #f5f7fa;
        text-align: center;
        font-weight: 600;
    }

    .table-wrapper td {
        text-align: left;
    }

    .text-right {
        text-align: right;
    }

    .sub-label {
        margin-top: 4px;
        font-size: 12px;
        color: #6b7280;
        white-space: normal;
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
        display: inline-flex;
        align-items: center;
        justify-content: center;
        min-width: 88px;
        padding: 6px 10px;
        border-radius: 999px;
        font-size: 12px;
        font-weight: 700;
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
    }

    @media (max-width: 768px) {
        .gov-banner {
            padding: 16px;
            align-items: flex-start;
        }

        .gov-title {
            font-size: 1.05rem;
        }

        .bao-cao-page {
            padding: 16px;
        }

        .actions {
            flex-direction: column;
        }
    }
</style>





