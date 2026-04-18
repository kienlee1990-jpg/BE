<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-shield-exclamation"></i>
                    </div>
                    <div class="gov-text">
                        <div class="wave-title">HỆ THỐNG THEO DÕI CHỈ TIÊU CÔNG TÁC</div>
                        <div class="gov-title">CẢNH BÁO RỦI RO KPI</div>
                        <div class="gov-sub">
                        </div>
                    </div>
                </div>

                <div class="risk-page">
                    <div class="page-header">
                        <div class="gov-banner">
                            <img src="https://upload.wikimedia.org/wikipedia/commons/a/a3/Emblem_of_Vietnam.svg"
                                class="gov-emblem" />
                        </div>
                        <div class="header-actions">
                            <button class="btn btn-primary" @click="fetchRiskData">Tải dữ liệu</button>
                            <button class="btn btn-secondary" @click="resetFilters">Đặt lại</button>
                        </div>
                    </div>

                    <div class="filter-card">
                        <div class="filter-grid">
                            <div class="form-group">
                                <label>Kỳ báo cáo</label>
                                <select v-model="filters.kyBaoCaoKPIId" @change="fetchRiskData">
                                    <option value="">-- Tất cả kỳ báo cáo --</option>
                                    <option v-for="item in kyBaoCaoOptions" :key="item.id" :value="item.id">
                                        {{ item.tenKy }}
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
                                <label>Mức rủi ro</label>
                                <select v-model="filters.mucRuiRo">
                                    <option value="">-- Tất cả mức --</option>
                                    <option value="critical">Nguy cơ rất cao</option>
                                    <option value="high">Nguy cơ cao</option>
                                    <option value="medium">Nguy cơ trung bình</option>
                                    <option value="low">Nguy cơ thấp</option>
                                </select>
                            </div>

                            <div class="form-group">
                                <label>Sắp xếp</label>
                                <select v-model="filters.sortBy">
                                    <option value="riskScore">Theo điểm rủi ro</option>
                                    <option value="completionAsc">Theo % hoàn thành tăng dần</option>
                                    <option value="growthAsc">Theo tăng trưởng đầu kỳ tăng dần</option>
                                </select>
                            </div>

                            <div class="form-group keyword-group">
                                <label>Từ khóa</label>
                                <input v-model.trim="filters.keyword" type="text"
                                    placeholder="Mã KPI, tên KPI, đơn vị, nhận xét..." />
                            </div>
                        </div>
                    </div>

                    <div v-if="loading" class="state loading">Đang tải dữ liệu cảnh báo rủi ro...</div>
                    <div v-else-if="errorMessage" class="state error">{{ errorMessage }}</div>

                    <template v-else>
                        <div class="stats-grid">
                            <div class="stat-card">
                                <span class="stat-label">Tổng KPI</span>
                                <strong class="stat-value">{{ filteredRiskRows.length }}</strong>
                                <span class="stat-note">Trong phạm vi lọc</span>
                            </div>

                            <div class="stat-card critical">
                                <span class="stat-label">Rất cao</span>
                                <strong class="stat-value">{{ riskStats.critical }}</strong>
                                <span class="stat-note">{{ buildRate(riskStats.critical) }}</span>
                            </div>

                            <div class="stat-card danger">
                                <span class="stat-label">Cao</span>
                                <strong class="stat-value">{{ riskStats.high }}</strong>
                                <span class="stat-note">{{ buildRate(riskStats.high) }}</span>
                            </div>

                            <div class="stat-card warning">
                                <span class="stat-label">Trung bình</span>
                                <strong class="stat-value">{{ riskStats.medium }}</strong>
                                <span class="stat-note">{{ buildRate(riskStats.medium) }}</span>
                            </div>

                            <div class="stat-card success">
                                <span class="stat-label">Thấp</span>
                                <strong class="stat-value">{{ riskStats.low }}</strong>
                                <span class="stat-note">{{ buildRate(riskStats.low) }}</span>
                            </div>

                            <div class="stat-card">
                                <span class="stat-label">Điểm rủi ro TB</span>
                                <strong class="stat-value">{{ formatNumber(avgRiskScore) }}</strong>
                                <span class="stat-note">Toàn bộ KPI sau lọc</span>
                            </div>
                        </div>

                        <div class="dashboard-grid two-columns">
                            <section class="panel-card">
                                <div class="panel-header">
                                    <h3>Cơ cấu mức rủi ro</h3>
                                    <span>{{ filteredRiskRows.length }} KPI</span>
                                </div>

                                <div v-if="filteredRiskRows.length === 0" class="empty-panel">Không có dữ liệu</div>
                                <div v-else class="chart-wrapper">
                                    <apexchart type="donut" height="320" :options="riskDonutOptions"
                                        :series="riskDonutSeries" />
                                </div>
                            </section>

                            <section class="panel-card">
                                <div class="panel-header">
                                    <h3>Insight nhanh</h3>
                                    <span>Điểm nhấn cần theo dõi</span>
                                </div>

                                <div class="insight-grid">
                                    <div class="insight-card">
                                        <span class="insight-label">KPI rủi ro nhất</span>
                                        <strong class="insight-title">
                                            {{ topRiskKpi?.tenChiTieu || topRiskKpi?.maChiTieu || '-' }}
                                        </strong>
                                        <span class="insight-value">
                                            {{ topRiskKpi ? `Điểm ${formatNumber(topRiskKpi.riskScore)}` : '-' }}
                                        </span>
                                    </div>

                                    <div class="insight-card">
                                        <span class="insight-label">Đơn vị nhiều rủi ro nhất</span>
                                        <strong class="insight-title">{{ topRiskUnit?.tenDonVi || '-' }}</strong>
                                        <span class="insight-value">
                                            {{ topRiskUnit ? `${topRiskUnit.critical + topRiskUnit.high} KPI rủi ro cao`
                                            : '-'
                                            }}
                                        </span>
                                    </div>

                                    <div class="insight-card">
                                        <span class="insight-label">% hoàn thành thấp nhất</span>
                                        <strong class="insight-title">
                                            {{ lowestCompletionKpi?.tenChiTieu || lowestCompletionKpi?.maChiTieu || '-'
                                            }}
                                        </strong>
                                        <span class="insight-value">
                                            {{ lowestCompletionKpi ? formatPercent(lowestCompletionKpi.tyLeHoanThanh) :
                                            '-' }}
                                        </span>
                                    </div>

                                    <div class="insight-card">
                                        <span class="insight-label">Tăng trưởng đầu kỳ thấp nhất</span>
                                        <strong class="insight-title">
                                            {{ worstGrowthKpi?.tenChiTieu || worstGrowthKpi?.maChiTieu || '-' }}
                                        </strong>
                                        <span class="insight-value">
                                            {{ worstGrowthKpi ? formatPercent(worstGrowthKpi.tyLeTangTruongSoVoiDauKy) :
                                            '-' }}
                                        </span>
                                    </div>
                                </div>

                                <div class="trend-detail compact">
                                    <div class="trend-row">
                                        <span>KPI không hoàn thành</span>
                                        <strong>{{ failedCount }}</strong>
                                    </div>
                                    <div class="trend-row">
                                        <span>KPI dưới 70%</span>
                                        <strong>{{ lowCompletionCount }}</strong>
                                    </div>
                                    <div class="trend-row">
                                        <span>Tăng trưởng âm đầu kỳ</span>
                                        <strong>{{ negativeDauKyCount }}</strong>
                                    </div>
                                    <div class="trend-row">
                                        <span>Tăng trưởng âm cùng kỳ</span>
                                        <strong>{{ negativeCungKyCount }}</strong>
                                    </div>
                                </div>
                            </section>
                        </div>

                        <div class="dashboard-grid two-columns">
                            <section class="panel-card">
                                <div class="panel-header">
                                    <h3>Top 10 KPI rủi ro cao nhất</h3>
                                    <span>Ưu tiên xử lý</span>
                                </div>

                                <div v-if="topRiskRows.length === 0" class="empty-panel">Không có dữ liệu</div>
                                <div v-else class="chart-wrapper">
                                    <apexchart type="bar" height="340" :options="topRiskChartOptions"
                                        :series="topRiskChartSeries" />
                                </div>
                            </section>

                            <section class="panel-card">
                                <div class="panel-header">
                                    <h3>Top đơn vị có rủi ro cao</h3>
                                    <span>Theo số KPI mức cao và rất cao</span>
                                </div>

                                <div v-if="riskUnitSummaries.length === 0" class="empty-panel">Không có dữ liệu</div>
                                <div v-else class="chart-wrapper">
                                    <apexchart type="bar" height="340" :options="riskUnitChartOptions"
                                        :series="riskUnitChartSeries" />
                                </div>
                            </section>
                        </div>

                        <section class="panel-card">
                            <div class="panel-header">
                                <h3>Danh sách cảnh báo rủi ro</h3>
                                <span>{{ filteredRiskRows.length }} bản ghi</span>
                            </div>

                            <div v-if="filteredRiskRows.length === 0" class="empty-panel">Không có dữ liệu cảnh báo
                            </div>
                            <div v-else class="table-wrapper">
                                <ColumnVisibilityTools table-id="CanhBaoRuiRoPage-table" />
                            <table id="CanhBaoRuiRoPage-table" class="data-table managed-table">
                                    <thead>
                                        <tr>
                                            <th>#</th>
                                            <th>Tên KPI</th>
                                            <th>Đơn vị</th>
                                            <th>Kỳ báo cáo</th>
                                            <th>% hoàn thành</th>
                                            <th>Xếp loại</th>
                                            <th>Tăng trưởng đầu kỳ</th>
                                            <th>Tăng trưởng cùng kỳ</th>
                                            <th>Điểm rủi ro</th>
                                            <th>Mức rủi ro</th>
                                            <th>Lý do cảnh báo</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="(item, index) in filteredRiskRows"
                                            :key="item.id || `${item.maChiTieu}-${index}`">
                                            <td>{{ index + 1 }}</td>
                                            <td>{{ item.tenChiTieu || '-' }}</td>
                                            <td>{{ item.tenDonViNhan || '-' }}</td>
                                            <td>{{ item.tenKy || item.maKy || '-' }}</td>
                                            <td>{{ formatPercent(item.tyLeHoanThanh) }}</td>
                                            <td>
                                                <span :class="['badge', badgeClass(item.xepLoai)]">
                                                    {{ getDanhGiaLabel(item.xepLoai) || 'Chưa có' }}
                                                </span>
                                            </td>
                                            <td>{{ formatPercent(item.tyLeTangTruongSoVoiDauKy) }}</td>
                                            <td>{{ formatPercent(item.tyLeTangTruongSoVoiCungKyNamTruoc) }}</td>
                                            <td>{{ formatNumber(item.riskScore) }}</td>
                                            <td>
                                                <span :class="['risk-badge', `risk-${item.riskLevel}`]">
                                                    {{ riskLabel(item.riskLevel) }}
                                                </span>
                                            </td>
                                            <td class="reason-cell">
                                                {{ item.riskReasons.join(', ') || '-' }}
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </section>
                    </template>
                </div>
            </div>
        </div>
    </BaseLayout>
</template>

<script setup>
    import { computed, onMounted, reactive, ref } from 'vue'
    import BaseLayout from '../BaseLayout.vue'
import ColumnVisibilityTools from '../shared/ColumnVisibilityTools.vue'
    import { apiRequest } from '../../services/api.js'
    import VueApexCharts from 'vue3-apexcharts'
    import {
        getDanhGiaBadgeClass,
        getDanhGiaLabel,
        getDanhGiaStatusCode
    } from '../../utils/danhGiaStatusClean.js'

    const apexchart = VueApexCharts

    const loading = ref(false)
    const errorMessage = ref('')
    const rows = ref([])
    const kyBaoCaoOptions = ref([])

    const filters = reactive({
        kyBaoCaoKPIId: '',
        donVi: '',
        mucRuiRo: '',
        sortBy: 'riskScore',
        keyword: ''
    })

    const warningKeywords = [
        'chậm',
        'tre',
        'trễ',
        'khó khăn',
        'vướng',
        'rủi ro',
        'nguy cơ',
        'không đạt',
        'giảm',
        'thấp',
        'chưa đạt'
    ]

    const donViOptions = computed(() => {
        return [...new Set(rows.value.map(item => item.tenDonViNhan).filter(Boolean))].sort((a, b) =>
            a.localeCompare(b, 'vi')
        )
    })

    const enrichedRows = computed(() => {
        return rows.value.map(item => {
            const risk = evaluateRisk(item)
            return {
                ...item,
                riskScore: risk.score,
                riskLevel: risk.level,
                riskReasons: risk.reasons
            }
        })
    })

    const filteredRiskRows = computed(() => {
        let data = [...enrichedRows.value]

        if (filters.donVi) {
            data = data.filter(item => item.tenDonViNhan === filters.donVi)
        }

        if (filters.mucRuiRo) {
            data = data.filter(item => item.riskLevel === filters.mucRuiRo)
        }

        if (filters.keyword) {
            const keyword = normalizeText(filters.keyword)
            data = data.filter(item =>
                [
                    item.maChiTieu,
                    item.tenChiTieu,
                    item.tenDonViNhan,
                    item.tenKy,
                    item.maKy,
                    item.xepLoai,
                    item.nhanXetDanhGia,
                    ...(item.riskReasons || [])
                ]
                    .filter(Boolean)
                    .some(value => normalizeText(String(value)).includes(keyword))
            )
        }

        return data.sort((a, b) => {
            if (filters.sortBy === 'completionAsc') {
                return numberOrZero(a.tyLeHoanThanh) - numberOrZero(b.tyLeHoanThanh)
            }
            if (filters.sortBy === 'growthAsc') {
                return numberOrZero(a.tyLeTangTruongSoVoiDauKy) - numberOrZero(b.tyLeTangTruongSoVoiDauKy)
            }
            return b.riskScore - a.riskScore
        })
    })

    const riskStats = computed(() => {
        return filteredRiskRows.value.reduce(
            (acc, item) => {
                if (item.riskLevel === 'critical') acc.critical++
                else if (item.riskLevel === 'high') acc.high++
                else if (item.riskLevel === 'medium') acc.medium++
                else acc.low++
                return acc
            },
            { critical: 0, high: 0, medium: 0, low: 0 }
        )
    })

    const avgRiskScore = computed(() => averageOf(filteredRiskRows.value, 'riskScore'))

    const riskDonutSeries = computed(() => [
        riskStats.value.critical,
        riskStats.value.high,
        riskStats.value.medium,
        riskStats.value.low
    ])

    const riskDonutOptions = computed(() => ({
        chart: {
            toolbar: { show: false }
        },
        labels: ['Rất cao', 'Cao', 'Trung bình', 'Thấp'],
        legend: {
            position: 'bottom'
        },
        dataLabels: {
            enabled: true,
            formatter: value => `${value.toFixed(1)}%`
        },
        tooltip: {
            y: {
                formatter: value => `${formatNumber(value)} KPI`
            }
        },
        colors: ['#991b1b', '#dc2626', '#f59e0b', '#16a34a']
    }))

    const topRiskRows = computed(() => filteredRiskRows.value.slice(0, 10))

    const topRiskChartSeries = computed(() => [
        {
            name: 'Điểm rủi ro',
            data: topRiskRows.value.map(item => roundNumber(item.riskScore))
        }
    ])

    const topRiskChartOptions = computed(() => ({
        chart: {
            type: 'bar',
            toolbar: { show: false }
        },
        plotOptions: {
            bar: {
                horizontal: true,
                borderRadius: 4
            }
        },
        dataLabels: {
            enabled: true
        },
        xaxis: {
            categories: topRiskRows.value.map(item => buildKpiChartLabel(item))
        },
        tooltip: {
            y: {
                formatter: value => `Điểm ${value}`
            }
        },
        colors: ['#dc2626']
    }))

    const riskUnitSummaries = computed(() => {
        const grouped = groupBy(filteredRiskRows.value, item => item.tenDonViNhan || 'Chưa xác định')

        return Object.entries(grouped)
            .map(([tenDonVi, items]) => ({
                tenDonVi,
                total: items.length,
                critical: items.filter(item => item.riskLevel === 'critical').length,
                high: items.filter(item => item.riskLevel === 'high').length,
                medium: items.filter(item => item.riskLevel === 'medium').length,
                low: items.filter(item => item.riskLevel === 'low').length,
                avgRiskScore: averageOf(items, 'riskScore')
            }))
            .sort((a, b) => (b.critical + b.high) - (a.critical + a.high) || b.avgRiskScore - a.avgRiskScore)
            .slice(0, 10)
    })

    const riskUnitChartSeries = computed(() => [
        {
            name: 'Rất cao',
            data: riskUnitSummaries.value.map(item => item.critical)
        },
        {
            name: 'Cao',
            data: riskUnitSummaries.value.map(item => item.high)
        }
    ])

    const riskUnitChartOptions = computed(() => ({
        chart: {
            type: 'bar',
            stacked: true,
            toolbar: { show: false }
        },
        plotOptions: {
            bar: {
                horizontal: true,
                borderRadius: 4
            }
        },
        dataLabels: {
            enabled: true
        },
        xaxis: {
            categories: riskUnitSummaries.value.map(item => item.tenDonVi)
        },
        legend: {
            position: 'bottom'
        },
        colors: ['#991b1b', '#dc2626']
    }))

    const topRiskKpi = computed(() => filteredRiskRows.value[0] || null)

    const topRiskUnit = computed(() => riskUnitSummaries.value[0] || null)

    const lowestCompletionKpi = computed(() => {
        return [...filteredRiskRows.value]
            .filter(item => Number.isFinite(Number(item.tyLeHoanThanh)))
            .sort((a, b) => Number(a.tyLeHoanThanh) - Number(b.tyLeHoanThanh))[0] || null
    })

    const worstGrowthKpi = computed(() => {
        return [...filteredRiskRows.value]
            .filter(item => Number.isFinite(Number(item.tyLeTangTruongSoVoiDauKy)))
            .sort((a, b) => Number(a.tyLeTangTruongSoVoiDauKy) - Number(b.tyLeTangTruongSoVoiDauKy))[0] || null
    })

    const failedCount = computed(() =>
        filteredRiskRows.value.filter(item => getDanhGiaStatusCode(item.xepLoai) === 'KHONG_HOAN_THANH').length
    )

    const lowCompletionCount = computed(() =>
        filteredRiskRows.value.filter(item => numberOrZero(item.tyLeHoanThanh) < 70).length
    )

    const negativeDauKyCount = computed(() =>
        filteredRiskRows.value.filter(item => numberOrZero(item.tyLeTangTruongSoVoiDauKy) < 0).length
    )

    const negativeCungKyCount = computed(() =>
        filteredRiskRows.value.filter(item => numberOrZero(item.tyLeTangTruongSoVoiCungKyNamTruoc) < 0).length
    )

    onMounted(async () => {
        await Promise.all([fetchKyBaoCao(), fetchRiskData()])
    })

    async function fetchKyBaoCao() {
        try {
            const data = await apiRequest('/KyBaoCaoKPI')
            kyBaoCaoOptions.value = Array.isArray(data) ? data : []
        } catch (error) {
            console.error('Lỗi tải kỳ báo cáo:', error)
            kyBaoCaoOptions.value = []
        }
    }

    async function fetchRiskData() {
        loading.value = true
        errorMessage.value = ''

        try {
            const data = filters.kyBaoCaoKPIId
                ? await apiRequest(`/DanhGiaKPI/by-ky-bao-cao/${filters.kyBaoCaoKPIId}`)
                : await apiRequest('/DanhGiaKPI')

            rows.value = Array.isArray(data) ? data : []
        } catch (error) {
            console.error(error)
            errorMessage.value = error.message || 'Không thể tải dữ liệu cảnh báo rủi ro KPI.'
            rows.value = []
        } finally {
            loading.value = false
        }
    }

    function resetFilters() {
        filters.kyBaoCaoKPIId = ''
        filters.donVi = ''
        filters.mucRuiRo = ''
        filters.sortBy = 'riskScore'
        filters.keyword = ''
        fetchRiskData()
    }

    function evaluateRisk(item) {
        let score = 0
        const reasons = []

        const completion = numberOrZero(item.tyLeHoanThanh)
        const dauKyGrowth = numberOrZero(item.tyLeTangTruongSoVoiDauKy)
        const cungKyGrowth = numberOrZero(item.tyLeTangTruongSoVoiCungKyNamTruoc)
        const xepLoai = getDanhGiaStatusCode(item.xepLoai)
        const nhanXet = normalizeText(item.nhanXetDanhGia)

        if (xepLoai === 'KHONG_HOAN_THANH') {
            score += 45
            reasons.push('Xếp loại không hoàn thành')
        } else if (xepLoai === 'CHUA_HOAN_THANH') {
            score += 24
            reasons.push('Xếp loại chưa hoàn thành')
        }

        if (completion < 50) {
            score += 35
            reasons.push('% hoàn thành dưới 50%')
        } else if (completion < 70) {
            score += 25
            reasons.push('% hoàn thành dưới 70%')
        } else if (completion < 85) {
            score += 10
            reasons.push('% hoàn thành thấp hơn kỳ vọng')
        }

        if (dauKyGrowth < -20) {
            score += 20
            reasons.push('Giảm mạnh so với đầu kỳ')
        } else if (dauKyGrowth < 0) {
            score += 12
            reasons.push('Tăng trưởng âm so với đầu kỳ')
        }

        if (cungKyGrowth < -20) {
            score += 16
            reasons.push('Giảm mạnh so với cùng kỳ năm trước')
        } else if (cungKyGrowth < 0) {
            score += 10
            reasons.push('Tăng trưởng âm so với cùng kỳ năm trước')
        }

        const matchedKeywords = warningKeywords.filter(keyword => nhanXet.includes(normalizeText(keyword)))
        if (matchedKeywords.length > 0) {
            score += Math.min(16, matchedKeywords.length * 4)
            reasons.push('Nhận xét có dấu hiệu cảnh báo')
        }

        const level =
            score >= 70 ? 'critical' :
                score >= 45 ? 'high' :
                    score >= 20 ? 'medium' :
                        'low'

        return {
            score: roundNumber(score),
            level,
            reasons
        }
    }

    function groupBy(items, getKey) {
        return items.reduce((acc, item) => {
            const key = getKey(item)
            if (!acc[key]) acc[key] = []
            acc[key].push(item)
            return acc
        }, {})
    }

    function averageOf(items, field) {
        const values = items
            .map(item => Number(item[field]))
            .filter(value => Number.isFinite(value))

        if (values.length === 0) return 0
        return roundNumber(values.reduce((sum, value) => sum + value, 0) / values.length)
    }

    function buildKpiChartLabel(item) {
        const ma = item.maChiTieu || 'N/A'
        const ten = item.tenChiTieu || 'Chưa có tên'
        return `${ma} - ${truncateText(ten, 28)}`
    }

    function truncateText(value, length = 30) {
        const text = String(value || '')
        if (text.length <= length) return text
        return `${text.slice(0, length)}...`
    }

    function normalizeText(value) {
        return String(value || '')
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .toLowerCase()
            .trim()
    }

    function numberOrZero(value) {
        const number = Number(value)
        return Number.isFinite(number) ? number : 0
    }

    function roundNumber(value) {
        return Number(Number(value || 0).toFixed(2))
    }

    function buildRate(value) {
        if (!filteredRiskRows.value.length) return '0%'
        return `${roundNumber((value / filteredRiskRows.value.length) * 100)}% tổng KPI`
    }

    function riskLabel(level) {
        if (level === 'critical') return 'Nguy cơ rất cao'
        if (level === 'high') return 'Nguy cơ cao'
        if (level === 'medium') return 'Nguy cơ trung bình'
        return 'Nguy cơ thấp'
    }

    function formatNumber(value) {
        if (value === null || value === undefined || value === '') return '-'
        const number = Number(value)
        if (!Number.isFinite(number)) return '-'
        return number.toLocaleString('vi-VN', {
            minimumFractionDigits: 0,
            maximumFractionDigits: 2
        })
    }

    function formatPercent(value) {
        if (value === null || value === undefined || value === '') return '-'
        const number = Number(value)
        if (!Number.isFinite(number)) return '-'
        return `${number.toLocaleString('vi-VN', {
            minimumFractionDigits: 0,
            maximumFractionDigits: 2
        })}%`
    }

    function badgeClass(xepLoai) {
        return getDanhGiaBadgeClass(xepLoai)
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

    .risk-page {
        display: flex;
        flex-direction: column;
        gap: 20px;
    }

    .page-header {
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        gap: 16px;
        flex-wrap: wrap;
    }

    .page-header h2 {
        margin: 0 0 6px;
        font-size: 28px;
        font-weight: 700;
        color: #0f172a;
    }

    .page-header p {
        margin: 0;
        color: #64748b;
    }

    .header-actions {
        display: flex;
        gap: 10px;
    }

    .btn {
        border: none;
        border-radius: 10px;
        padding: 10px 16px;
        cursor: pointer;
        font-weight: 600;
    }

    .btn-primary {
        background: #2563eb;
        color: #fff;
    }

    .btn-secondary {
        background: #e2e8f0;
        color: #0f172a;
    }

    .filter-card,
    .panel-card,
    .stat-card {
        background: #fff;
        border-radius: 18px;
        box-shadow: 0 8px 24px rgba(15, 23, 42, 0.08);
    }

    .filter-card,
    .panel-card {
        padding: 20px;
    }

    .filter-grid {
        display: grid;
        grid-template-columns: repeat(5, minmax(0, 1fr));
        gap: 16px;
    }

    .form-group {
        display: flex;
        flex-direction: column;
        gap: 8px;
    }

    .form-group label {
        font-weight: 600;
        color: #334155;
    }

    .form-group input,
    .form-group select {
        height: 42px;
        border: 1px solid #cbd5e1;
        border-radius: 10px;
        padding: 0 12px;
        outline: none;
    }

    .form-group input:focus,
    .form-group select:focus {
        border-color: #89d2ef;
        box-shadow: 0 0 0 0.2rem rgba(137, 210, 239, 0.2);
    }

    .state {
        padding: 18px;
        border-radius: 14px;
        font-weight: 600;
    }

    .loading {
        background: #eff6ff;
        color: #1d4ed8;
    }

    .error {
        background: #fef2f2;
        color: #dc2626;
    }

    .stats-grid {
        display: grid;
        grid-template-columns: repeat(6, minmax(0, 1fr));
        gap: 16px;
    }

    .stat-card {
        padding: 18px;
        display: flex;
        flex-direction: column;
        gap: 6px;
    }

    .stat-card.critical {
        background: linear-gradient(135deg, #fee2e2, #fecaca);
    }

    .stat-card.danger {
        background: linear-gradient(135deg, #fef2f2, #fff1f2);
    }

    .stat-card.warning {
        background: linear-gradient(135deg, #fffbeb, #fef3c7);
    }

    .stat-card.success {
        background: linear-gradient(135deg, #ecfdf5, #f0fdf4);
    }

    .stat-label {
        color: #64748b;
        font-size: 14px;
    }

    .stat-value {
        font-size: 24px;
        font-weight: 700;
        color: #0f172a;
    }

    .stat-note {
        color: #475569;
        font-size: 13px;
    }

    .dashboard-grid {
        display: grid;
        gap: 20px;
    }

    .two-columns {
        grid-template-columns: repeat(2, minmax(0, 1fr));
    }

    .panel-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 16px;
        gap: 12px;
    }

    .panel-header h3 {
        margin: 0;
        font-size: 18px;
        color: #0f172a;
    }

    .panel-header span {
        color: #64748b;
        font-size: 14px;
    }

    .empty-panel {
        padding: 28px;
        text-align: center;
        color: #94a3b8;
        border: 1px dashed #cbd5e1;
        border-radius: 14px;
    }

    .chart-wrapper {
        width: 100%;
    }

    .insight-grid {
        display: grid;
        grid-template-columns: repeat(2, minmax(0, 1fr));
        gap: 14px;
        margin-bottom: 16px;
    }

    .insight-card {
        background: #f8fafc;
        border: 1px solid #e2e8f0;
        border-radius: 14px;
        padding: 14px;
        display: flex;
        flex-direction: column;
        gap: 6px;
    }

    .insight-label {
        font-size: 13px;
        color: #64748b;
    }

    .insight-title {
        color: #0f172a;
        font-size: 16px;
        font-weight: 700;
    }

    .insight-value {
        color: #334155;
        font-size: 14px;
    }

    .trend-detail.compact {
        display: grid;
        grid-template-columns: repeat(2, minmax(0, 1fr));
        gap: 12px;
    }

    .trend-row {
        display: flex;
        justify-content: space-between;
        gap: 12px;
        padding: 12px;
        border: 1px solid #e2e8f0;
        background: #fff;
        border-radius: 12px;
    }

    .table-wrapper {
        overflow-x: auto;
    }

    .data-table {
        width: 100%;
        min-width: 1500px;
        border-collapse: collapse;
    }

    .data-table th,
    .data-table td {
        padding: 12px;
        border-bottom: 1px solid #e2e8f0;
        text-align: left;
        white-space: nowrap;
        vertical-align: top;
    }

    .data-table th {
        background: #f8fafc;
        color: #334155;
        font-weight: 700;
    }

    .data-table tbody tr:hover {
        background: #f8fafc;
    }

    .reason-cell {
        white-space: normal;
        min-width: 260px;
    }

    .badge,
    .risk-badge {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        min-height: 28px;
        padding: 4px 10px;
        border-radius: 999px;
        font-size: 12px;
        font-weight: 700;
    }

    .badge-excellent {
        background: #dcfce7;
        color: #166534;
    }

    .badge-good {
        background: #dbeafe;
        color: #1d4ed8;
    }

    .badge-pass {
        background: #fef3c7;
        color: #b45309;
    }

    .badge-fail {
        background: #fee2e2;
        color: #b91c1c;
    }

    .badge-default {
        background: #e2e8f0;
        color: #334155;
    }

    .risk-critical {
        background: #7f1d1d;
        color: #fff;
    }

    .risk-high {
        background: #dc2626;
        color: #fff;
    }

    .risk-medium {
        background: #f59e0b;
        color: #fff;
    }

    .risk-low {
        background: #16a34a;
        color: #fff;
    }

    @media (max-width: 1200px) {

        .filter-grid,
        .stats-grid,
        .two-columns,
        .insight-grid,
        .trend-detail.compact {
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

        .page-header h2 {
            font-size: 24px;
        }
    }
</style>



