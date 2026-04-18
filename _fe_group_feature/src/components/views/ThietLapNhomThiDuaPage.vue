<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="page-hero">
                    <div>
                        <div class="hero-kicker">THIẾT LẬP NỀN</div>
                        <h1>Thiết lập nhóm thi đua</h1>
                        <p>Tạo nhóm, chọn đơn vị thuộc nhóm và cấu hình những chỉ tiêu chi tiết được tính vào thi đua.</p>
                    </div>
                    <button class="btn btn-primary" type="button" @click="startCreateGroup">Tạo nhóm mới</button>
                </div>

                <div class="layout-grid">
                    <section class="card group-list-card">
                        <div class="card-head">
                            <div>
                                <h2>Danh sách nhóm</h2>
                                <p>{{ groups.length }} nhóm đang có trong hệ thống</p>
                            </div>
                        </div>

                        <div v-if="loadingGroups" class="state loading">Đang tải nhóm thi đua...</div>
                        <div v-else-if="groupError" class="state error">{{ groupError }}</div>
                        <div v-else class="group-list">
                            <button
                                v-for="group in groups"
                                :key="group.id"
                                type="button"
                                class="group-item"
                                :class="{ active: editForm.id === group.id }"
                                @click="startEditGroup(group)"
                            >
                                <div class="group-item-content">
                                    <strong>{{ group.tenNhom }}</strong>
                                    <span>{{ group.moTa || 'Chưa có mô tả nhóm' }}</span>
                                </div>
                                <span class="group-badge">{{ group.donVis?.length || 0 }} đơn vị</span>
                            </button>
                        </div>
                    </section>

                    <section class="card form-card">
                        <div class="card-head">
                            <div>
                                <h2>{{ editForm.id ? 'Cập nhật nhóm thi đua' : 'Tạo nhóm thi đua' }}</h2>
                                <p>Thiết lập đơn vị và chỉ tiêu chi tiết được dùng khi tổng hợp thi đua nhóm.</p>
                            </div>
                        </div>

                        <div class="form-grid">
                            <div class="form-group">
                                <label>Tên nhóm</label>
                                <input v-model.trim="editForm.tenNhom" type="text" placeholder="Ví dụ: Khối Điều tra" />
                            </div>

                            <div class="form-group">
                                <label>Trạng thái</label>
                                <select v-model="editForm.trangThai">
                                    <option value="HOAT_DONG">Hoạt động</option>
                                    <option value="TAM_DUNG">Tạm dừng</option>
                                </select>
                            </div>

                            <div class="form-group full-width">
                                <label>Mô tả</label>
                                <textarea
                                    v-model.trim="editForm.moTa"
                                    rows="3"
                                    placeholder="Mô tả ngắn cho nhóm thi đua"
                                ></textarea>
                            </div>
                        </div>

                        <div class="selection-grid">
                            <div class="selection-card">
                                <div class="selection-head">
                                    <div>
                                        <h3>Đơn vị trong nhóm</h3>
                                        <p>{{ editForm.donViIds.length }} đơn vị được chọn</p>
                                    </div>
                                </div>

                                <div class="search-box">
                                    <input
                                        v-model.trim="unitKeyword"
                                        type="text"
                                        placeholder="Tìm theo mã hoặc tên đơn vị"
                                    />
                                </div>

                                <div class="checkbox-toolbar">
                                    <button class="btn btn-light btn-sm" type="button" @click="toggleVisibleUnits(true)">
                                        Chọn tất cả đang lọc
                                    </button>
                                    <button class="btn btn-light btn-sm" type="button" @click="toggleVisibleUnits(false)">
                                        Bỏ tất cả
                                    </button>
                                </div>

                                <div class="checkbox-list">
                                    <label v-for="unit in filteredUnits" :key="unit.id" class="checkbox-item">
                                        <input
                                            :checked="editForm.donViIds.includes(unit.id)"
                                            type="checkbox"
                                            @change="toggleSelection(editForm.donViIds, unit.id)"
                                        />
                                        <span>
                                            <strong>{{ unit.tenDonVi }}</strong>
                                            <small>{{ unit.maDonVi }} · {{ unit.loaiDonVi }}</small>
                                        </span>
                                    </label>
                                </div>
                            </div>

                            <div class="selection-card">
                                <div class="selection-head">
                                    <div>
                                        <h3>Chỉ tiêu chi tiết tính vào thi đua</h3>
                                        <p>{{ editForm.danhMucChiTieuIds.length }} chỉ tiêu được chọn</p>
                                    </div>
                                </div>

                                <div class="search-box">
                                    <input
                                        v-model.trim="criteriaKeyword"
                                        type="text"
                                        placeholder="Tìm theo mã, tên chỉ tiêu hoặc chỉ tiêu cha"
                                    />
                                </div>

                                <div class="checkbox-toolbar">
                                    <button class="btn btn-light btn-sm" type="button" @click="toggleVisibleCriteria(true)">
                                        Chọn tất cả đang lọc
                                    </button>
                                    <button class="btn btn-light btn-sm" type="button" @click="toggleVisibleCriteria(false)">
                                        Bỏ tất cả
                                    </button>
                                </div>

                                <div class="checkbox-list">
                                    <label v-for="criteria in filteredCriteria" :key="criteria.id" class="checkbox-item">
                                        <input
                                            :checked="editForm.danhMucChiTieuIds.includes(criteria.id)"
                                            type="checkbox"
                                            @change="toggleSelection(editForm.danhMucChiTieuIds, criteria.id)"
                                        />
                                        <span>
                                            <strong>{{ criteria.tenChiTieu }}</strong>
                                            <small>{{ criteria.maChiTieu }} · {{ criteria.tenChiTieuCha || 'Chỉ tiêu độc lập' }}</small>
                                        </span>
                                    </label>
                                </div>
                            </div>
                        </div>

                        <div class="form-actions">
                            <button class="btn btn-primary" type="button" @click="saveGroup" :disabled="saving">
                                {{ saving ? 'Đang lưu...' : 'Lưu nhóm' }}
                            </button>
                            <button class="btn btn-secondary" type="button" @click="startCreateGroup">Làm mới</button>
                            <button
                                v-if="editForm.id"
                                class="btn btn-danger"
                                type="button"
                                @click="deleteGroup"
                                :disabled="saving"
                            >
                                Xóa nhóm
                            </button>
                        </div>
                    </section>
                </div>
            </div>
        </div>
    </BaseLayout>
</template>

<script setup>
    import { computed, onMounted, reactive, ref } from 'vue'
    import BaseLayout from '../BaseLayout.vue'
    import { apiRequest } from '../../services/api'

    const groups = ref([])
    const donViOptions = ref([])
    const criteriaOptions = ref([])
    const loadingGroups = ref(false)
    const saving = ref(false)
    const groupError = ref('')
    const unitKeyword = ref('')
    const criteriaKeyword = ref('')

    const editForm = reactive(createEmptyForm())

    const filteredUnits = computed(() => {
        const keyword = normalizeText(unitKeyword.value)
        if (!keyword) {
            return donViOptions.value
        }

        return donViOptions.value.filter(item =>
            [item.maDonVi, item.tenDonVi, item.loaiDonVi].some(value => normalizeText(value).includes(keyword))
        )
    })

    const filteredCriteria = computed(() => {
        const keyword = normalizeText(criteriaKeyword.value)
        if (!keyword) {
            return criteriaOptions.value
        }

        return criteriaOptions.value.filter(item =>
            [item.maChiTieu, item.tenChiTieu, item.tenChiTieuCha].some(value => normalizeText(value).includes(keyword))
        )
    })

    onMounted(async () => {
        await Promise.all([fetchGroups(), fetchUnits(), fetchCriteria()])
    })

    async function fetchGroups() {
        loadingGroups.value = true
        groupError.value = ''

        try {
            groups.value = await apiRequest('/NhomThiDua')
            if (!editForm.id && groups.value.length) {
                startEditGroup(groups.value[0])
            }
        } catch (error) {
            groupError.value = error.message || 'Không thể tải nhóm thi đua.'
        } finally {
            loadingGroups.value = false
        }
    }

    async function fetchUnits() {
        const data = await apiRequest('/DonVi')
        donViOptions.value = [...data].sort((left, right) =>
            `${left.tenDonVi}`.localeCompare(`${right.tenDonVi}`, 'vi')
        )
    }

    async function fetchCriteria() {
        const data = await apiRequest('/ChiTietGiaoChiTieu')
        criteriaOptions.value = flattenCriteriaFromAssignments(data)
    }

    function startCreateGroup() {
        Object.assign(editForm, createEmptyForm())
    }

    function startEditGroup(group) {
        Object.assign(editForm, {
            id: group.id,
            tenNhom: group.tenNhom || '',
            moTa: group.moTa || '',
            trangThai: group.trangThai || 'HOAT_DONG',
            donViIds: Array.isArray(group.donVis) ? group.donVis.map(item => Number(item.donViId || item.id || 0)).filter(Boolean) : [],
            danhMucChiTieuIds: Array.isArray(group.chiTieus)
                ? group.chiTieus.map(item => Number(item.danhMucChiTieuId || item.id || 0)).filter(Boolean)
                : []
        })
    }

    async function saveGroup() {
        if (!editForm.tenNhom.trim()) {
            window.alert('Bạn cần nhập tên nhóm thi đua.')
            return
        }

        if (editForm.donViIds.length === 0) {
            window.alert('Bạn cần chọn ít nhất một đơn vị trong nhóm.')
            return
        }

        if (editForm.danhMucChiTieuIds.length === 0) {
            window.alert('Bạn cần chọn ít nhất một chỉ tiêu chi tiết để tính vào thi đua.')
            return
        }

        saving.value = true

        try {
            const payload = {
                tenNhom: editForm.tenNhom.trim(),
                moTa: editForm.moTa.trim() || null,
                trangThai: editForm.trangThai,
                donViIds: [...editForm.donViIds],
                danhMucChiTieuIds: [...editForm.danhMucChiTieuIds]
            }

            if (editForm.id) {
                await apiRequest(`/NhomThiDua/${editForm.id}`, 'PUT', payload)
            } else {
                await apiRequest('/NhomThiDua', 'POST', payload)
            }

            await fetchGroups()

            const updated = groups.value.find(item => item.tenNhom === payload.tenNhom)
            if (updated) {
                startEditGroup(updated)
            }

            window.alert('Đã lưu nhóm thi đua.')
        } catch (error) {
            window.alert(error.message || 'Không thể lưu nhóm thi đua.')
        } finally {
            saving.value = false
        }
    }

    async function deleteGroup() {
        if (!editForm.id) {
            return
        }

        const ok = window.confirm(`Bạn có chắc muốn xóa nhóm "${editForm.tenNhom}"?`)
        if (!ok) {
            return
        }

        saving.value = true
        try {
            await apiRequest(`/NhomThiDua/${editForm.id}`, 'DELETE')
            startCreateGroup()
            await fetchGroups()
            window.alert('Đã xóa nhóm thi đua.')
        } catch (error) {
            window.alert(error.message || 'Không thể xóa nhóm thi đua.')
        } finally {
            saving.value = false
        }
    }

    function toggleSelection(collection, value) {
        const index = collection.indexOf(value)
        if (index >= 0) {
            collection.splice(index, 1)
        } else {
            collection.push(value)
        }
    }

    function toggleVisibleUnits(checked) {
        syncSelection(editForm.donViIds, filteredUnits.value.map(item => item.id), checked)
    }

    function toggleVisibleCriteria(checked) {
        syncSelection(editForm.danhMucChiTieuIds, filteredCriteria.value.map(item => item.id), checked)
    }

    function syncSelection(collection, ids, checked) {
        ids.forEach(id => {
            const index = collection.indexOf(id)
            if (checked && index < 0) {
                collection.push(id)
            }
            if (!checked && index >= 0) {
                collection.splice(index, 1)
            }
        })
    }

    function flattenCriteriaFromAssignments(items) {
        const map = new Map()

        const walk = (rows, parent = null) => {
            normalizeList(rows).forEach(item => {
                const children = normalizeList(item.tieuChiCon || item.TieuChiCon)
                const danhMucChiTieuId = Number(item.danhMucChiTieuId || item.DanhMucChiTieuId || 0)
                const normalized = {
                    id: danhMucChiTieuId,
                    maChiTieu: item.maDanhMucChiTieu || item.MaDanhMucChiTieu || item.maChiTieu || item.MaChiTieu || '',
                    tenChiTieu: item.tenDanhMucChiTieu || item.TenDanhMucChiTieu || item.tenChiTieu || item.TenChiTieu || '',
                    tenChiTieuCha: parent?.tenChiTieu || '',
                    chiTietGiaoId: Number(item.id || item.Id || 0),
                    tenDotGiaoChiTieu: item.tenDotGiaoChiTieu || item.TenDotGiaoChiTieu || '',
                    tenDonViNhan: item.tenDonViNhan || item.TenDonViNhan || ''
                }

                if (children.length > 0) {
                    walk(children, normalized)
                    return
                }

                if (normalized.id <= 0 || !normalized.tenChiTieu) {
                    return
                }

                if (!map.has(normalized.id)) {
                    map.set(normalized.id, normalized)
                }
            })
        }

        walk(items)

        return [...map.values()].sort((left, right) =>
            `${left.tenChiTieu}`.localeCompare(`${right.tenChiTieu}`, 'vi')
        )
    }

    function normalizeList(value) {
        return Array.isArray(value) ? value : []
    }

    function normalizeText(value) {
        return String(value || '')
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .replace(/đ/g, 'd')
            .replace(/Đ/g, 'D')
            .toLowerCase()
    }

    function createEmptyForm() {
        return {
            id: null,
            tenNhom: '',
            moTa: '',
            trangThai: 'HOAT_DONG',
            donViIds: [],
            danhMucChiTieuIds: []
        }
    }
</script>

<style scoped>
    .page-wrap {
        min-height: 100vh;
        background: linear-gradient(180deg, #f8fbff 0%, #eef5fb 100%);
    }

    .page-hero,
    .card {
        background: #fff;
        border-radius: 20px;
        border: 1px solid rgba(15, 23, 42, 0.08);
        box-shadow: 0 14px 32px rgba(15, 23, 42, 0.06);
    }

    .page-hero {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 16px;
        padding: 24px;
        margin-bottom: 24px;
    }

    .hero-kicker {
        font-size: 0.82rem;
        font-weight: 800;
        color: #0d6efd;
        letter-spacing: 0.08em;
        text-transform: uppercase;
    }

    .page-hero h1 {
        margin: 6px 0 8px;
        font-size: 1.8rem;
        font-weight: 800;
        color: #0f172a;
    }

    .page-hero p {
        margin: 0;
        color: #475569;
    }

    .layout-grid {
        display: grid;
        grid-template-columns: 320px minmax(0, 1fr);
        gap: 20px;
    }

    .card {
        padding: 20px;
    }

    .card-head {
        margin-bottom: 16px;
    }

    .card-head h2 {
        margin: 0 0 6px;
        font-size: 1.15rem;
        font-weight: 800;
        color: #0f172a;
    }

    .card-head p {
        margin: 0;
        color: #64748b;
    }

    .group-list {
        display: flex;
        flex-direction: column;
        gap: 10px;
        max-height: 720px;
        overflow: auto;
    }

    .group-item {
        width: 100%;
        display: flex;
        justify-content: space-between;
        gap: 12px;
        align-items: flex-start;
        padding: 14px 16px;
        border-radius: 16px;
        border: 1px solid #dbeafe;
        background: #fff;
        text-align: left;
        cursor: pointer;
    }

    .group-item.active {
        background: #eff6ff;
        border-color: #93c5fd;
    }

    .group-item-content strong {
        display: block;
        color: #0f172a;
        margin-bottom: 4px;
    }

    .group-item-content span {
        color: #64748b;
        font-size: 0.88rem;
    }

    .group-badge {
        flex-shrink: 0;
        padding: 4px 10px;
        border-radius: 999px;
        background: #dbeafe;
        color: #1d4ed8;
        font-size: 0.78rem;
        font-weight: 700;
    }

    .form-grid {
        display: grid;
        grid-template-columns: repeat(2, minmax(0, 1fr));
        gap: 16px;
        margin-bottom: 18px;
    }

    .form-group label {
        display: block;
        margin-bottom: 8px;
        font-weight: 700;
        color: #0f172a;
    }

    .form-group input,
    .form-group select,
    .form-group textarea {
        width: 100%;
        border-radius: 14px;
        border: 1px solid #cbd5e1;
        padding: 12px 14px;
        background: #fff;
        color: #0f172a;
    }

    .form-group input,
    .form-group select {
        min-height: 46px;
    }

    .form-group textarea {
        resize: vertical;
    }

    .full-width {
        grid-column: 1 / -1;
    }

    .selection-grid {
        display: grid;
        grid-template-columns: repeat(2, minmax(0, 1fr));
        gap: 18px;
    }

    .selection-card {
        border: 1px solid #e2e8f0;
        border-radius: 18px;
        background: #f8fbff;
        padding: 16px;
    }

    .selection-head {
        margin-bottom: 12px;
    }

    .selection-head h3 {
        margin: 0 0 4px;
        font-size: 1rem;
        font-weight: 800;
        color: #0f172a;
    }

    .selection-head p {
        margin: 0;
        color: #64748b;
    }

    .search-box {
        margin-bottom: 12px;
    }

    .search-box input {
        width: 100%;
        min-height: 42px;
        border-radius: 12px;
        border: 1px solid #cbd5e1;
        padding: 0 12px;
    }

    .checkbox-toolbar,
    .form-actions {
        display: flex;
        gap: 10px;
        flex-wrap: wrap;
    }

    .checkbox-toolbar {
        margin-bottom: 12px;
    }

    .checkbox-list {
        display: flex;
        flex-direction: column;
        gap: 10px;
        max-height: 460px;
        overflow: auto;
    }

    .checkbox-item {
        display: flex;
        gap: 12px;
        align-items: flex-start;
        padding: 12px 14px;
        border-radius: 14px;
        background: #fff;
        border: 1px solid #dbeafe;
        cursor: pointer;
    }

    .checkbox-item input {
        margin-top: 3px;
    }

    .checkbox-item strong {
        display: block;
        color: #0f172a;
        margin-bottom: 4px;
    }

    .checkbox-item small {
        color: #64748b;
    }

    .form-actions {
        margin-top: 20px;
    }

    .btn {
        border: none;
        border-radius: 14px;
        min-height: 44px;
        padding: 0 16px;
        font-weight: 700;
        cursor: pointer;
    }

    .btn-sm {
        min-height: 36px;
        padding: 0 12px;
        border-radius: 12px;
    }

    .btn-primary {
        background: #0d6efd;
        color: #fff;
    }

    .btn-secondary,
    .btn-light {
        background: #e2e8f0;
        color: #0f172a;
    }

    .btn-danger {
        background: #dc2626;
        color: #fff;
    }

    .state {
        padding: 18px;
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

    @media (max-width: 1200px) {
        .layout-grid,
        .selection-grid {
            grid-template-columns: 1fr;
        }
    }

    @media (max-width: 768px) {
        .page-hero {
            flex-direction: column;
            align-items: flex-start;
        }

        .form-grid {
            grid-template-columns: 1fr;
        }

        .btn {
            width: 100%;
        }
    }
</style>
