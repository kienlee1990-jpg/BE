<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i :class="scopeMeta.icon"></i>
                    </div>
                    <div class="gov-text">
                        <div class="wave-title">HỆ THỐNG THEO DÕI CHỈ TIÊU</div>
                        <div class="gov-title">{{ scopeMeta.pageTitle }}</div>
                    </div>
                </div>

                <div class="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
                    <div class="gov-banner">
                        <img src="https://upload.wikimedia.org/wikipedia/commons/a/a3/Emblem_of_Vietnam.svg"
                            class="gov-emblem" />
                    </div>

                    <button class="btn btn-primary btn-action" @click="openCreateModal">
                        <i class="bi bi-plus-circle me-2"></i>
                        Tạo giao chỉ tiêu
                    </button>
                </div>

                <div class="card custom-card mb-4">
                    <div class="card-header bg-white border-0 pb-0">
                        <h5 class="mb-1">Bộ lọc tìm kiếm</h5>
                        <small class="text-muted">{{ scopeMeta.filterHint }}</small>
                    </div>

                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-12 col-md-6 col-xl-4">
                                <label class="form-label">Đợt giao</label>
                                <select v-model.number="filters.dotGiaoChiTieuId" class="form-select">
                                    <option :value="null">Tất cả</option>
                                    <option v-for="item in dotOptions" :key="item.id" :value="item.id">{{
                                        item.tenDotGiao }}</option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-4">
                                <label class="form-label">Danh mục chỉ tiêu</label>
                                <select v-model.number="filters.danhMucChiTieuId" class="form-select">
                                    <option :value="null">Tất cả</option>
                                    <option v-for="item in assignableDanhMucOptions" :key="item.id" :value="item.id">
                                        {{ item.maChiTieu }} - {{ item.tenChiTieu }}
                                    </option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-4">
                                <label class="form-label">{{ scopeMeta.receiverLabel }}</label>
                                <select v-model.number="filters.donViId" class="form-select">
                                    <option :value="null">Tất cả</option>
                                    <optgroup v-for="group in groupedDonViOptions" :key="group.key" :label="group.label">
                                        <option v-for="item in group.items" :key="item.id" :value="item.id">
                                            {{ item.maDonVi ? `${item.maDonVi} - ` : '' }}{{ item.tenDonVi }}
                                        </option>
                                    </optgroup>
                                </select>
                            </div>

                            <div class="col-12">
                                <label class="form-label">Từ khóa</label>
                                <input v-model="filters.keyword" type="text" class="form-control"
                                    placeholder="Nhập tên đợt, mã chỉ tiêu, tên chỉ tiêu, tên tiêu chí con hoặc tên đơn vị" />
                            </div>
                        </div>

                        <div class="d-flex flex-wrap gap-2 mt-4">
                            <button class="btn btn-primary" @click="loadData">
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
                            <h5 class="mb-1">{{ scopeMeta.listTitle }}</h5>
                            <small class="text-muted">{{ scopeMeta.listHint }}</small>
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
                            <ColumnVisibilityTools table-id="GiaoChiTieuChoDonViPage-table" />
                            <table id="GiaoChiTieuChoDonViPage-table" class="table table-hover align-middle mb-0 custom-table managed-table">
                                <thead>
                                    <tr v-if="isCatpScope">
                                        <th>Đợt giao</th>
                                        <th>Chỉ tiêu giao</th>
                                        <th>Tiêu chí giao</th>
                                        <th>Kỳ báo cáo</th>
                                        <th>Mục tiêu giao</th>
                                        <th>Đầu kỳ cố định</th>
                                        <th>Ghi chú</th>
                                        <th class="text-center" style="width: 180px">Thao tác</th>
                                    </tr>
                                    <tr v-else>
                                        <th>Đợt giao</th>
                                        <th>Chỉ tiêu giao</th>
                                        <th>{{ scopeMeta.receiverLabel }}</th>
                                        <th>Kỳ báo cáo</th>
                                        <th>Mục tiêu giao</th>
                                        <th>Ghi chú</th>
                                        <th class="text-center" style="width: 180px">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody v-if="isCatpScope">
                                    <tr v-for="item in filteredItems" :key="item.id">
                                        <td><div class="fw-semibold">{{ item.tenDotGiaoChiTieu || '-' }}</div></td>
                                        <td>
                                            <div class="fw-semibold text-primary">{{ item.tenDanhMucChiTieu || '-' }}</div>
                                        </td>
                                        <td>
                                            <div v-if="item.tieuChiCon.length" class="table-stack">
                                                <div v-for="child in item.tieuChiCon" :key="child.id" class="table-stack-line">
                                                    <div class="fw-semibold">{{ child.tenDanhMucChiTieu || '-' }}</div>
                                                </div>
                                            </div>
                                            <span v-else class="text-muted">Giao trực tiếp</span>
                                        </td>
                                        <td>
                                            <span class="badge text-bg-light border">
                                                {{ getKyBaoCaoLabel(item.tanSuatBaoCao) }}
                                            </span>
                                        </td>
                                        <td>
                                            <div v-if="item.tieuChiCon.length" class="table-stack">
                                                <div v-for="child in item.tieuChiCon" :key="`${child.id}-target`" class="table-stack-line">
                                                    <div>{{ formatAssignmentTarget(child) }}</div>
                                                </div>
                                            </div>
                                            <div v-else>{{ formatAssignmentTarget(item) }}</div>
                                        </td>
                                        <td>
                                            <div v-if="item.tieuChiCon.length" class="table-stack">
                                                <div v-for="child in item.tieuChiCon" :key="`${child.id}-baseline`" class="table-stack-line">
                                                    <div>{{ formatAssignmentBaseline(child) }}</div>
                                                </div>
                                            </div>
                                            <div v-else>{{ formatAssignmentBaseline(item) }}</div>
                                        </td>
                                        <td>{{ item.ghiChu || '-' }}</td>
                                        <td class="text-center">
                                            <div class="d-flex justify-content-center gap-2">
                                                <button class="btn btn-sm btn-outline-primary" @click="openEditModal(item)">
                                                    <i class="bi bi-pencil-square me-1"></i>Sửa
                                                </button>
                                                <button class="btn btn-sm btn-outline-danger" @click="handleDelete(item)">
                                                    <i class="bi bi-trash me-1"></i>Xóa
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                </tbody>
                                <tbody v-else>
                                    <tr v-for="item in filteredItems" :key="`${item.id}-default`">
                                        <td><div class="fw-semibold">{{ item.tenDotGiaoChiTieu || '-' }}</div></td>
                                        <td>
                                            <div class="fw-semibold text-primary">{{ item.tenDanhMucChiTieu || '-' }}
                                            </div>
                                            <small class="text-muted">{{ item.maDanhMucChiTieu || '-' }}</small>
                                            <div class="small text-muted mt-1">
                                                {{ mapLoai(getEffectiveCriterion(item)) }}
                                                <template v-if="!isQualitative(getEffectiveCriterion(item))">
                                                    • {{ getQuyTacDanhGiaLabel(getEffectiveQuyTacDanhGia(item)) }}
                                                </template>
                                            </div>
                                            <div v-if="item.tieuChiCon.length" class="criteria-tags mt-2">
                                                <span v-for="child in item.tieuChiCon" :key="child.id"
                                                    class="badge text-bg-light border">{{ child.tenDanhMucChiTieu
                                                    }}</span>
                                            </div>
                                        </td>
                                        <td>
                                            <div class="fw-semibold">{{ item.tenDonViNhan || '-' }}</div>
                                        </td>
                                        <td>
                                            <span class="badge text-bg-light border">{{
                                                getKyBaoCaoLabel(item.tanSuatBaoCao) }}</span>
                                        </td>
                                        <td>
                                            <div v-if="!item.tieuChiCon.length">
                                                <div>{{ formatAssignmentTarget(item) }}<span v-if="showGiaTriDauKyCoDinh(item)"> • Đầu kỳ: {{ formatAssignmentBaseline(item) }}</span></div>
                                            </div>
                                            <div v-else class="d-flex flex-column gap-2">
                                                <div v-for="child in item.tieuChiCon" :key="child.id"
                                                    class="target-line">
                                                    <div class="fw-semibold">{{ child.tenDanhMucChiTieu }}</div>
                                                    <div>{{ formatAssignmentTarget(child) }}<span v-if="showGiaTriDauKyCoDinh(child)"> • Đầu kỳ: {{ formatAssignmentBaseline(child) }}</span></div>
                                                </div>
                                            </div>
                                        </td>
                                        <td>{{ item.ghiChu || '-' }}</td>
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
                                    <h4 class="modal-title mb-1">{{ isEdit ? 'Cập nhật giao chỉ tiêu' : 'Tạo giao chỉ tiêu' }}</h4>
                                    <p class="text-muted mb-0">{{ scopeMeta.modalHint }}</p>
                                </div>
                                <button type="button" class="btn-close" @click="closeModal"></button>
                            </div>

                            <div class="modal-body pt-3">
                                <div class="row g-3">
                                    <div class="col-12 col-md-3">
                                        <label class="form-label">Đợt giao <span class="text-danger">*</span></label>
                                        <select v-model.number="form.dotGiaoChiTieuId" class="form-select">
                                            <option :value="null">Chọn đợt giao</option>
                                            <option v-for="item in dotOptions" :key="item.id" :value="item.id">
                                                {{ item.tenDotGiao }}{{ formatDotRange(item) ? ` • ${formatDotRange(item)}` : '' }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">Danh mục chỉ tiêu <span
                                                class="text-danger">*</span></label>
                                        <select v-model.number="form.danhMucChiTieuId" class="form-select">
                                            <option :value="null">Chọn chỉ tiêu</option>
                                            <option v-for="item in assignableDanhMucOptions" :key="item.id" :value="item.id">
                                                {{ item.maChiTieu }} - {{ item.tenChiTieu }}{{ item.tieuChiDanhGias.length ? ` (${item.tieuChiDanhGias.length} tiêu chí con)` : '' }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-3">
                                        <label class="form-label">Kỳ báo cáo <span
                                                class="text-danger">*</span></label>
                                        <select v-model="form.tanSuatBaoCao" class="form-select">
                                            <option value="">Chọn kỳ báo cáo</option>
                                            <option v-for="item in tanSuatBaoCaoOptions" :key="item.value"
                                                :value="item.value">{{ item.label }}</option>
                                        </select>
                                    </div>

                                    <div v-if="props.scope === 'CATP' || isEdit" class="col-12 col-md-6">
                                        <label class="form-label">{{ scopeMeta.receiverLabel }} <span class="text-danger">*</span></label>
                                        <select v-model.number="form.donViId" class="form-select" :disabled="props.scope === 'CATP'">
                                            <option :value="null">Chọn đơn vị</option>
                                            <optgroup v-for="group in groupedDonViOptions" :key="group.key" :label="group.label">
                                                <option v-for="item in group.items" :key="item.id" :value="item.id">
                                                    {{ item.maDonVi ? `${item.maDonVi} - ` : '' }}{{ item.tenDonVi }}
                                                </option>
                                            </optgroup>
                                        </select>
                                        <small v-if="props.scope === 'CATP'" class="text-muted d-block mt-1">
                                            Màn này mặc định giao cho Công an thành phố.
                                        </small>
                                        <small v-else-if="isEdit" class="text-muted d-block mt-1">
                                            Chế độ sửa giữ nguyên một đơn vị nhận của bản giao đã tạo.
                                        </small>
                                    </div>

                                    <div v-else class="col-12">
                                        <div class="receiver-picker">
                                            <div class="receiver-picker-head">
                                                <div>
                                                    <div class="section-title mb-1">{{ scopeMeta.receiverLabel }}</div>
                                                    <div class="text-muted small">
                                                        Chọn nhiều đơn vị trực tiếp hoặc giao nhanh theo nhóm thi đua.
                                                    </div>
                                                </div>
                                                <div class="receiver-mode-switch">
                                                    <button
                                                        type="button"
                                                        class="btn btn-sm"
                                                        :class="form.phuongThucChonDonVi === 'DON_VI' ? 'btn-primary' : 'btn-outline-secondary'"
                                                        @click="switchUnitSelectionMode('DON_VI')"
                                                    >
                                                        Chọn nhiều đơn vị
                                                    </button>
                                                    <button
                                                        type="button"
                                                        class="btn btn-sm"
                                                        :class="form.phuongThucChonDonVi === 'NHOM_THI_DUA' ? 'btn-primary' : 'btn-outline-secondary'"
                                                        @click="switchUnitSelectionMode('NHOM_THI_DUA')"
                                                    >
                                                        Theo nhóm thi đua
                                                    </button>
                                                </div>
                                            </div>

                                            <div v-if="form.phuongThucChonDonVi === 'NHOM_THI_DUA'" class="row g-3">
                                                <div class="col-12 col-lg-5">
                                                    <label class="form-label">Nhóm thi đua <span class="text-danger">*</span></label>
                                                    <select v-model.number="form.nhomThiDuaId" class="form-select">
                                                        <option :value="null">Chọn nhóm thi đua</option>
                                                        <option v-for="group in scopedNhomThiDuaOptions" :key="group.id" :value="group.id">
                                                            {{ group.tenNhom }} ({{ group.donVis.length }} đơn vị)
                                                        </option>
                                                    </select>
                                                    <small class="text-muted d-block mt-1">
                                                        Khi lưu, hệ thống sẽ tạo một bản giao riêng cho từng đơn vị thuộc nhóm đã chọn.
                                                    </small>
                                                </div>
                                                <div class="col-12 col-lg-7">
                                                    <label class="form-label">Đơn vị thuộc nhóm</label>
                                                    <div class="selected-unit-board">
                                                        <div v-if="selectedNhomThiDuaTargetUnits.length" class="selected-unit-list">
                                                            <span v-for="unit in selectedNhomThiDuaTargetUnits" :key="`group-${unit.id}`" class="selected-unit-chip">
                                                                {{ unit.maDonVi ? `${unit.maDonVi} - ` : '' }}{{ unit.tenDonVi }}
                                                            </span>
                                                        </div>
                                                        <div v-else class="text-muted small">
                                                            Chưa có đơn vị nào hợp lệ trong nhóm thi đua đang chọn.
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div v-else class="row g-3">
                                                <div class="col-12 col-lg-4">
                                                    <label class="form-label">Tìm đơn vị</label>
                                                    <input
                                                        v-model.trim="unitSelectionKeyword"
                                                        type="text"
                                                        class="form-control"
                                                        placeholder="Nhập mã hoặc tên đơn vị"
                                                    />
                                                </div>
                                                <div class="col-12 col-lg-8">
                                                    <label class="form-label">Đơn vị được chọn</label>
                                                    <div class="selected-unit-board">
                                                        <div v-if="selectedTargetUnits.length" class="selected-unit-list">
                                                            <span v-for="unit in selectedTargetUnits" :key="unit.id" class="selected-unit-chip">
                                                                {{ unit.maDonVi ? `${unit.maDonVi} - ` : '' }}{{ unit.tenDonVi }}
                                                                <button type="button" class="btn-close btn-close-sm ms-2" aria-label="Bỏ chọn" @click="toggleDonViSelection(unit.id)"></button>
                                                            </span>
                                                        </div>
                                                        <div v-else class="text-muted small">
                                                            Chưa chọn đơn vị nào.
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-12">
                                                    <div class="selection-actions">
                                                        <button type="button" class="btn btn-sm btn-outline-secondary" @click="toggleVisibleSelectableUnits(true)">
                                                            Chọn tất cả đang lọc
                                                        </button>
                                                        <button type="button" class="btn btn-sm btn-outline-secondary" @click="toggleVisibleSelectableUnits(false)">
                                                            Bỏ tất cả
                                                        </button>
                                                    </div>
                                                    <div class="receiver-checkbox-list mt-2">
                                                        <label v-for="unit in filteredSelectableUnits" :key="`pick-${unit.id}`" class="receiver-checkbox-item">
                                                            <input
                                                                :checked="form.donViIds.includes(unit.id)"
                                                                type="checkbox"
                                                                @change="toggleDonViSelection(unit.id)"
                                                            />
                                                            <span>
                                                                <strong>{{ unit.tenDonVi }}</strong>
                                                                <small>{{ unit.maDonVi || 'Chưa có mã' }} • {{ getDonViGroupLabel(unit.loaiDonVi, unit.tenDonVi) }}</small>
                                                            </span>
                                                        </label>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div v-if="selectedDot || selectedDanhMuc || selectedTargetUnits.length || selectedNhomThiDua || selectedDonVi" class="col-12">
                                        <div class="selection-summary">
                                            <div v-if="selectedDot" class="selection-card">
                                                <div class="selection-label">Đợt giao đã chọn</div>
                                                <div class="selection-value">{{ selectedDot.tenDotGiao }}</div>
                                                <div class="selection-meta">{{ formatDotRange(selectedDot) || 'Chưa khai báo thời gian áp dụng' }}</div>
                                            </div>
                                            <div v-if="selectedDanhMuc" class="selection-card">
                                                <div class="selection-label">Chỉ tiêu được giao</div>
                                                <div class="selection-value">{{ selectedDanhMuc.maChiTieu }} - {{ selectedDanhMuc.tenChiTieu }}</div>
                                                <div class="selection-meta">
                                                    {{ mapLoai(form.tieuChiDanhGia || selectedDanhMuc.tieuChiDanhGia || selectedDanhMuc.loaiChiTieu) }}{{ selectedDanhMuc.tieuChiDanhGias.length ? ` • ${selectedDanhMuc.tieuChiDanhGias.length} tiêu chí con` : '' }}
                                                </div>
                                            </div>
                                            <div v-if="selectedNhomThiDua && !isEdit && props.scope !== 'CATP' && form.phuongThucChonDonVi === 'NHOM_THI_DUA'" class="selection-card">
                                                <div class="selection-label">Nhóm thi đua nhận chỉ tiêu</div>
                                                <div class="selection-value">{{ selectedNhomThiDua.tenNhom }}</div>
                                                <div class="selection-meta">{{ selectedNhomThiDuaTargetUnits.length }} đơn vị sẽ được tạo giao chỉ tiêu</div>
                                            </div>
                                            <div v-else-if="selectedTargetUnits.length" class="selection-card">
                                                <div class="selection-label">{{ scopeMeta.receiverLabel }}</div>
                                                <div class="selection-value">
                                                    {{ selectedTargetUnits.length === 1 ? selectedTargetUnits[0].tenDonVi : `${selectedTargetUnits.length} đơn vị được chọn` }}
                                                </div>
                                                <div class="selection-meta">
                                                    <template v-if="selectedTargetUnits.length === 1">
                                                        {{ selectedTargetUnits[0].maDonVi || 'Chưa khai báo mã đơn vị' }} • {{ getDonViGroupLabel(selectedTargetUnits[0].loaiDonVi, selectedTargetUnits[0].tenDonVi) }}
                                                    </template>
                                                    <template v-else>
                                                        {{ selectedTargetUnits.slice(0, 3).map(unit => unit.tenDonVi).join(', ') }}<span v-if="selectedTargetUnits.length > 3">...</span>
                                                    </template>
                                                </div>
                                            </div>
                                            <div v-else-if="selectedDonVi" class="selection-card">
                                                <div class="selection-label">{{ scopeMeta.receiverLabel }}</div>
                                                <div class="selection-value">{{ selectedDonVi.tenDonVi }}</div>
                                                <div class="selection-meta">{{ selectedDonVi.maDonVi || 'Chưa khai báo mã đơn vị' }} • {{ getDonViGroupLabel(selectedDonVi.loaiDonVi, selectedDonVi.tenDonVi) }}</div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Tiêu chí đánh giá <span class="text-danger">*</span></label>
                                        <select v-model="form.tieuChiDanhGia" class="form-select">
                                            <option value="">Chọn tiêu chí đánh giá</option>
                                            <option v-for="item in TIEU_CHI_DANH_GIA_OPTIONS" :key="item.value" :value="item.value">
                                                {{ item.label }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Quy tắc đánh giá <span v-if="!isQualitative(form.tieuChiDanhGia || selectedDanhMuc?.loaiChiTieu)" class="text-danger">*</span></label>
                                        <select v-model="form.quyTacDanhGia" class="form-select" :disabled="isQualitative(form.tieuChiDanhGia || selectedDanhMuc?.loaiChiTieu)">
                                            <option value="">Chọn quy tắc đánh giá</option>
                                            <option v-for="item in getQuyTacOptionsByCriterion(form.tieuChiDanhGia || selectedDanhMuc?.loaiChiTieu)" :key="item.value" :value="item.value">
                                                {{ item.label }}
                                            </option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Đơn vị tính</label>
                                        <input :value="selectedDanhMuc ? (selectedDanhMuc.donViTinh || '-') : '-'"
                                            type="text" class="form-control" readonly />
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Phương thức giao</label>
                                        <input
                                            :value="selectedDanhMuc && selectedDanhMuc.tieuChiDanhGias.length ? 'Giao theo tiêu chí con' : 'Giao trực tiếp cho chỉ tiêu'"
                                            type="text" class="form-control" readonly />
                                    </div>

                                    <div v-if="selectedDanhMuc && !selectedDanhMuc.tieuChiDanhGias.length"
                                        class="col-12">
                                        <div class="criteria-block">
                                            <div class="section-title mb-2">Mục tiêu giao</div>
                                            <div class="text-muted small mb-3">{{ buildCatalogHint({ ...selectedDanhMuc, tieuChiDanhGia: form.tieuChiDanhGia, loaiMocSoSanh: form.loaiMocSoSanh, chieuSoSanh: form.chieuSoSanh }) }}
                                            </div>

                                            <div v-if="!isQualitative(form.tieuChiDanhGia || selectedDanhMuc.loaiChiTieu)" class="row g-3 mb-3">
                                                <div class="col-12 col-md-6">
                                                    <label class="form-label">Chiều đánh giá <span class="text-danger">*</span></label>
                                                    <select v-model="form.chieuSoSanh" class="form-select">
                                                        <option value="">Chọn chiều đánh giá</option>
                                                        <option v-for="item in CHIEU_SO_SANH_OPTIONS" :key="item.value" :value="item.value">
                                                            {{ item.label }}
                                                        </option>
                                                    </select>
                                                </div>

                                                <div v-if="isDinhLuongSoSanhCriterion(form.tieuChiDanhGia)" class="col-12 col-md-6">
                                                    <label class="form-label">Mốc so sánh <span class="text-danger">*</span></label>
                                                    <select v-model="form.loaiMocSoSanh" class="form-select">
                                                        <option value="">Chọn mốc so sánh</option>
                                                        <option v-for="item in LOAI_MOC_SO_SANH_OPTIONS" :key="item.value" :value="item.value">
                                                            {{ item.label }}
                                                        </option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div v-if="isQualitative(form.tieuChiDanhGia || selectedDanhMuc.loaiChiTieu)" class="row g-3">
                                                <div class="col-12">
                                                    <label class="form-label">Mô tả mục tiêu <span
                                                            class="text-danger">*</span></label>
                                                    <textarea v-model="form.giaTriMucTieuText" rows="3"
                                                        class="form-control"
                                                        placeholder="Nhập mô tả mục tiêu cần đạt"></textarea>
                                                </div>
                                            </div>

                                            <div v-else class="row g-3">
                                                <div class="col-12 col-md-4">
                                                    <label class="form-label">Giá trị mục tiêu <span
                                                            class="text-danger">*</span></label>
                                                    <input v-model="form.giaTriMucTieu" type="number" step="any"
                                                        class="form-control" placeholder="Nhập giá trị mục tiêu" />
                                                </div>
                                                <div class="col-12 col-md-4">
                                                    <label class="form-label">Giá trị đầu kỳ cố định <span
                                                            class="text-danger">*</span></label>
                                                    <input v-model="form.giaTriDauKyCoDinh" type="number" step="any"
                                                        class="form-control" placeholder="Nhập giá trị đầu kỳ cố định" />
                                                    <small class="text-muted d-block mt-1">
                                                        Giá trị này được giữ cố định cho mọi kỳ báo cáo của chỉ tiêu đã giao.
                                                    </small>
                                                </div>
                                                <div class="col-12 col-md-4">
                                                    <label class="form-label">Đơn vị tính</label>
                                                    <input :value="selectedDanhMuc.donViTinh || '-'" type="text"
                                                        class="form-control" readonly />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div v-if="selectedDanhMuc && selectedDanhMuc.tieuChiDanhGias.length"
                                        class="col-12">
                                        <div class="criteria-block">
                                            <div class="section-title mb-1">Mục tiêu theo tiêu chí con</div>
                                            <div class="text-muted small mb-3">
                                                {{ selectedDanhMuc.batBuocDatTatCaTieuChiCon
                                                    ? 'Chỉ tiêu cha chỉ đạt khi tất cả tiêu chí con đều đạt.'
                                                    : 'Chỉ tiêu cha được đánh giá theo tập tiêu chí con.' }}
                                            </div>

                                            <div v-for="(child, index) in form.tieuChiCon" :key="child.localKey"
                                                class="criteria-card mb-3">
                                                <div
                                                    class="d-flex justify-content-between align-items-center gap-3 mb-2">
                                                    <div>
                                                        <div class="fw-semibold">{{ index + 1 }}. {{ child.tenChiTieu }}
                                                        </div>
                                                        <div class="small text-muted">{{ mapLoai(child.tieuChiDanhGia || child.loaiChiTieu) }}<template v-if="!isQualitative(child.tieuChiDanhGia || child.loaiChiTieu)"> • {{ getQuyTacDanhGiaLabel(child.quyTacDanhGia) }}</template> |
                                                            {{ child.donViTinh || '-' }}</div>
                                                    </div>
                                                    <span class="badge text-bg-light border">{{ child.maChiTieu
                                                        }}</span>
                                                </div>

                                                <div class="small text-muted mb-3">{{ buildCatalogHint(child) }}</div>

                                                <div class="row g-3 mb-3">
                                                    <div class="col-12 col-md-4">
                                                        <label class="form-label">Tiêu chí đánh giá <span class="text-danger">*</span></label>
                                                        <select v-model="child.tieuChiDanhGia" class="form-select">
                                                            <option value="">Chọn tiêu chí đánh giá</option>
                                                            <option v-for="item in TIEU_CHI_DANH_GIA_OPTIONS" :key="item.value" :value="item.value">
                                                                {{ item.label }}
                                                            </option>
                                                        </select>
                                                    </div>

                                                        <div class="col-12 col-md-4">
                                                            <label class="form-label">Quy tắc đánh giá <span v-if="!isQualitative(child.tieuChiDanhGia || child.loaiChiTieu)" class="text-danger">*</span></label>
                                                            <select v-model="child.quyTacDanhGia" class="form-select" :disabled="isQualitative(child.tieuChiDanhGia || child.loaiChiTieu)">
                                                                <option value="">Chọn quy tắc đánh giá</option>
                                                                <option v-for="item in getQuyTacOptionsByCriterion(child.tieuChiDanhGia || child.loaiChiTieu)" :key="item.value" :value="item.value">
                                                                    {{ item.label }}
                                                                </option>
                                                            </select>
                                                        </div>

                                                    <template v-if="!isQualitative(child.tieuChiDanhGia || child.loaiChiTieu)">
                                                        <div class="col-12 col-md-4">
                                                            <label class="form-label">Chiều đánh giá <span class="text-danger">*</span></label>
                                                            <select v-model="child.chieuSoSanh" class="form-select">
                                                                <option value="">Chọn chiều đánh giá</option>
                                                                <option v-for="item in CHIEU_SO_SANH_OPTIONS" :key="item.value" :value="item.value">
                                                                    {{ item.label }}
                                                                </option>
                                                            </select>
                                                        </div>

                                                        <div v-if="isDinhLuongSoSanhCriterion(child.tieuChiDanhGia)" class="col-12 col-md-4">
                                                            <label class="form-label">Mốc so sánh <span class="text-danger">*</span></label>
                                                            <select v-model="child.loaiMocSoSanh" class="form-select">
                                                                <option value="">Chọn mốc so sánh</option>
                                                                <option v-for="item in LOAI_MOC_SO_SANH_OPTIONS" :key="item.value" :value="item.value">
                                                                    {{ item.label }}
                                                                </option>
                                                            </select>
                                                        </div>
                                                    </template>
                                                </div>

                                                <div class="row g-3">
                                                    <div v-if="isQualitative(child.tieuChiDanhGia || child.loaiChiTieu)" class="col-12">
                                                        <label class="form-label">Mô tả mục tiêu <span
                                                                class="text-danger">*</span></label>
                                                        <textarea v-model="child.giaTriMucTieuText" rows="3"
                                                            class="form-control"
                                                            placeholder="Nhập mô tả mục tiêu cho tiêu chí con"></textarea>
                                                    </div>

                                                    <div v-else class="col-12 col-md-8">
                                                        <label class="form-label">Giá trị mục tiêu <span
                                                                class="text-danger">*</span></label>
                                                        <input v-model="child.giaTriMucTieu" type="number" step="any"
                                                            class="form-control" placeholder="Nhập giá trị mục tiêu" />
                                                    </div>

                                                    <div v-if="!isQualitative(child.tieuChiDanhGia || child.loaiChiTieu)"
                                                        class="col-12 col-md-4">
                                                        <label class="form-label">Đầu kỳ cố định <span
                                                                class="text-danger">*</span></label>
                                                        <input v-model="child.giaTriDauKyCoDinh" type="number" step="any"
                                                            class="form-control" placeholder="Nhập đầu kỳ cố định" />
                                                    </div>

                                                    <div v-if="!isQualitative(child.tieuChiDanhGia || child.loaiChiTieu)"
                                                        class="col-12 col-md-4">
                                                        <label class="form-label">Đơn vị tính</label>
                                                        <input :value="child.donViTinh || '-'" type="text"
                                                            class="form-control" readonly />
                                                    </div>

                                                    <div class="col-12">
                                                        <label class="form-label">Ghi chú cho tiêu chí con</label>
                                                        <input v-model="child.ghiChu" type="text" class="form-control"
                                                            placeholder="Nhập ghi chú nếu có" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-12">
                                        <label class="form-label">Ghi chú giao chỉ tiêu</label>
                                        <textarea v-model="form.ghiChu" rows="3" class="form-control"
                                            placeholder="Nhập ghi chú nếu có"></textarea>
                                    </div>
                                </div>
                            </div>

                            <div class="modal-footer border-0 pt-0">
                                <button class="btn btn-light" @click="closeModal">Hủy</button>
                                <button class="btn btn-primary" :disabled="saving" @click="handleSubmit">
                                    <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                                    {{ saving ? 'Đang lưu...' : isEdit ? 'Cập nhật' : 'Lưu giao chỉ tiêu' }}
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
    import { computed, onMounted, reactive, ref, watch } from 'vue'
    import BaseLayout from '../BaseLayout.vue'
import ColumnVisibilityTools from '../shared/ColumnVisibilityTools.vue'
    import { apiRequest } from '../../services/api.js'
    import {
        CHIEU_SO_SANH_OPTIONS,
        LOAI_MOC_SO_SANH_OPTIONS,
        QUY_TAC_DANH_GIA_OPTIONS,
        TIEU_CHI_DANH_GIA_OPTIONS,
        getLoaiMocSoSanhLabel,
        getQuyTacDanhGiaLabel,
        getQuyTacOptionsByCriterion,
        getTieuChiDanhGiaLabel,
        isDinhLuongTichLuyCriterion,
        isDinhLuongSoSanhCriterion,
        isDinhTinhCriterion,
        isKhongVuotNguongRule,
        isQuantitativeCriterion
    } from '../../utils/danhGiaStatusClean.js'

    const props = defineProps({
        scope: {
            type: String,
            default: 'CADP_PHUONG_XA'
        }
    })

    const API_PATHS = {
        chiTietGiaoChiTieu: '/ChiTietGiaoChiTieu',
        dotGiaoChiTieu: '/dot-giao-chi-tieu',
        danhMucChiTieu: '/danh-muc-chi-tieu',
        donVi: '/DonVi',
        nhomThiDua: '/NhomThiDua'
    }

    const CATP_TEN_DON_VI = 'Công an thành phố Đà Nẵng'

    const SCOPE_CONFIGS = {
        CATP: {
            icon: 'bi bi-buildings-fill',
            pageTitle: 'GIAO CHỈ TIÊU CHO CATP',
            filterHint: 'Tra cứu các bản giao chỉ tiêu cho Công an thành phố Đà Nẵng',
            listTitle: 'Danh sách giao cho CATP',
            listHint: 'Theo dõi các chỉ tiêu được giao trực tiếp cho Công an thành phố Đà Nẵng',
            modalHint: 'Chọn đợt giao, chỉ tiêu và khai báo mục tiêu, đầu kỳ cố định cho Công an thành phố Đà Nẵng.',
            receiverLabel: 'Đơn vị nhận'
        },
        PHONG: {
            icon: 'bi bi-building-fill-gear',
            pageTitle: 'GIAO CHỈ TIÊU CHO PHÒNG',
            filterHint: 'Tra cứu các bản giao chỉ tiêu cho khối phòng nghiệp vụ',
            listTitle: 'Danh sách giao cho Phòng',
            listHint: 'Hỗ trợ giao chỉ tiêu đơn và chỉ tiêu phân rã cho các phòng nghiệp vụ',
            modalHint: 'Chọn đợt giao, chỉ tiêu, phòng nhận chỉ tiêu và khai báo mục tiêu, đầu kỳ cố định.',
            receiverLabel: 'Phòng nhận chỉ tiêu'
        },
        CADP_PHUONG_XA: {
            icon: 'bi bi-diagram-3-fill',
            pageTitle: 'GIAO CHỈ TIÊU CHO CADP PHƯỜNG/XÃ',
            filterHint: 'Tra cứu các bản giao chỉ tiêu cho Công an cấp xã, phường',
            listTitle: 'Danh sách giao cho CADP phường/xã',
            listHint: 'Hỗ trợ giao chỉ tiêu đơn và chỉ tiêu phân rã cho Công an cấp xã, phường',
            modalHint: 'Chọn đợt giao, chỉ tiêu, đơn vị nhận và khai báo mục tiêu, đầu kỳ cố định.',
            receiverLabel: 'Đơn vị nhận'
        }
    }

    const tanSuatBaoCaoOptions = [
        { value: 'THANG', label: 'Tháng' },
        { value: 'QUY', label: 'Quý' },
        { value: '6THANG', label: '6 tháng' },
        { value: 'NAM', label: 'Năm' }
    ]

    const loading = ref(false)
    const saving = ref(false)
    const showModal = ref(false)
    const isEdit = ref(false)
    const editingId = ref(null)
    const syncingForm = ref(false)

    const items = ref([])
    const dotOptions = ref([])
    const danhMucOptions = ref([])
    const donViOptions = ref([])
    const nhomThiDuaOptions = ref([])
    const unitSelectionKeyword = ref('')

    const filters = reactive({
        dotGiaoChiTieuId: null,
        danhMucChiTieuId: null,
        donViId: null,
        keyword: ''
    })

    const createDefaultForm = () => ({
        dotGiaoChiTieuId: null,
        danhMucChiTieuId: null,
        donViId: null,
        donViIds: [],
        nhomThiDuaId: null,
        phuongThucChonDonVi: props.scope === 'CATP' ? 'DON_VI' : 'DON_VI',
        tanSuatBaoCao: '',
        tieuChiDanhGia: '',
        loaiMocSoSanh: '',
        chieuSoSanh: '',
        quyTacDanhGia: '',
        giaTriMucTieu: '',
        giaTriDauKyCoDinh: '',
        giaTriMucTieuText: '',
        ghiChu: '',
        tieuChiCon: []
    })

    const form = reactive(createDefaultForm())

    const resetForm = () => {
        Object.assign(form, createDefaultForm())
    }

    const normalizeList = (response) => {
        if (Array.isArray(response)) return response
        if (Array.isArray(response?.data)) return response.data
        if (Array.isArray(response?.data?.data)) return response.data.data
        if (Array.isArray(response?.items)) return response.items
        return []
    }

    const pick = (item, ...keys) => {
        for (const key of keys) {
            if (item?.[key] !== undefined && item?.[key] !== null) return item[key]
        }
        return null
    }

    const normalizeDot = (item) => ({
        id: Number(pick(item, 'id', 'Id') || 0),
        maDotGiao: String(pick(item, 'maDotGiao', 'MaDotGiao') || ''),
        tenDotGiao: String(pick(item, 'tenDotGiao', 'TenDotGiao') || ''),
        ngayBatDau: pick(item, 'ngayBatDau', 'NgayBatDau'),
        ngayKetThuc: pick(item, 'ngayKetThuc', 'NgayKetThuc')
    })

    const normalizeDonVi = (item) => ({
        id: Number(pick(item, 'id', 'Id') || 0),
        maDonVi: String(pick(item, 'maDonVi', 'MaDonVi') || ''),
        tenDonVi: String(pick(item, 'tenDonVi', 'TenDonVi') || ''),
        loaiDonVi: String(pick(item, 'loaiDonVi', 'LoaiDonVi') || ''),
        tenDonViCha: String(pick(item, 'tenDonViCha', 'TenDonViCha') || '')
    })

    const normalizeNhomThiDua = (item) => ({
        id: Number(pick(item, 'id', 'Id') || 0),
        tenNhom: String(pick(item, 'tenNhom', 'TenNhom') || ''),
        moTa: String(pick(item, 'moTa', 'MoTa') || ''),
        donVis: normalizeList(pick(item, 'donVis', 'DonVis')).map((member) => ({
            id: Number(pick(member, 'id', 'Id') || 0),
            donViId: Number(pick(member, 'donViId', 'DonViId', 'id', 'Id') || 0)
        }))
    })

    const normalizeCatalogChild = (item, index = 1) => ({
        id: Number(pick(item, 'id', 'Id') || 0),
        maChiTieu: String(pick(item, 'maChiTieu', 'MaChiTieu') || ''),
        tenChiTieu: String(pick(item, 'tenChiTieu', 'TenChiTieu') || ''),
        loaiChiTieu: String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || 'DINH_TINH'),
        tieuChiDanhGia: String(pick(item, 'tieuChiDanhGia', 'TieuChiDanhGia', 'loaiChiTieu', 'LoaiChiTieu') || 'DINH_TINH'),
        donViTinh: String(pick(item, 'donViTinh', 'DonViTinh') || ''),
        dieuKienHoanThanh: String(pick(item, 'dieuKienHoanThanh', 'DieuKienHoanThanh') || ''),
        dieuKienKhongHoanThanh: String(pick(item, 'dieuKienKhongHoanThanh', 'DieuKienKhongHoanThanh') || ''),
        tyLePhanTramMucTieu: pick(item, 'tyLePhanTramMucTieu', 'TyLePhanTramMucTieu'),
        loaiMocSoSanh: String(pick(item, 'loaiMocSoSanh', 'LoaiMocSoSanh') || ''),
        chieuSoSanh: String(pick(item, 'chieuSoSanh', 'ChieuSoSanh') || ''),
        quyTacDanhGia: String(pick(item, 'quyTacDanhGia', 'QuyTacDanhGia') || ''),
        thuTuHienThi: Number(pick(item, 'thuTuHienThi', 'ThuTuHienThi') || index)
    })

    const normalizeDanhMuc = (item) => ({
        id: Number(pick(item, 'id', 'Id') || 0),
        maChiTieu: String(pick(item, 'maChiTieu', 'MaChiTieu') || ''),
        tenChiTieu: String(pick(item, 'tenChiTieu', 'TenChiTieu') || ''),
        loaiChiTieu: String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || ''),
        tieuChiDanhGia: String(pick(item, 'tieuChiDanhGia', 'TieuChiDanhGia', 'loaiChiTieu', 'LoaiChiTieu') || ''),
        chiTieuChaId: pick(item, 'chiTieuChaId', 'ChiTieuChaId'),
        thuTuHienThi: Number(pick(item, 'thuTuHienThi', 'ThuTuHienThi') || 0),
        donViTinh: String(pick(item, 'donViTinh', 'DonViTinh') || ''),
        dieuKienHoanThanh: String(pick(item, 'dieuKienHoanThanh', 'DieuKienHoanThanh') || ''),
        dieuKienKhongHoanThanh: String(pick(item, 'dieuKienKhongHoanThanh', 'DieuKienKhongHoanThanh') || ''),
        tyLePhanTramMucTieu: pick(item, 'tyLePhanTramMucTieu', 'TyLePhanTramMucTieu'),
        loaiMocSoSanh: String(pick(item, 'loaiMocSoSanh', 'LoaiMocSoSanh') || ''),
        chieuSoSanh: String(pick(item, 'chieuSoSanh', 'ChieuSoSanh') || ''),
        quyTacDanhGia: String(pick(item, 'quyTacDanhGia', 'QuyTacDanhGia') || ''),
        batBuocDatTatCaTieuChiCon: pick(item, 'batBuocDatTatCaTieuChiCon', 'BatBuocDatTatCaTieuChiCon') !== false,
        tieuChiDanhGias: normalizeList(pick(item, 'tieuChiDanhGias', 'TieuChiDanhGias')).map((child, index) => normalizeCatalogChild(child, index + 1))
    })

    const normalizeAssignmentChild = (item) => ({
        id: Number(pick(item, 'id', 'Id') || 0),
        danhMucChiTieuId: Number(pick(item, 'danhMucChiTieuId', 'DanhMucChiTieuId') || 0),
        tenDanhMucChiTieu: String(pick(item, 'tenDanhMucChiTieu', 'TenDanhMucChiTieu') || ''),
        maDanhMucChiTieu: String(pick(item, 'maDanhMucChiTieu', 'MaDanhMucChiTieu') || ''),
        loaiChiTieu: String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || 'DINH_TINH'),
        tieuChiDanhGia: String(pick(item, 'tieuChiDanhGia', 'TieuChiDanhGia', 'loaiChiTieu', 'LoaiChiTieu') || 'DINH_TINH'),
        donViTinh: String(pick(item, 'donViTinh', 'DonViTinh') || ''),
        quyTacDanhGia: String(pick(item, 'quyTacDanhGia', 'QuyTacDanhGia') || ''),
        giaTriMucTieu: pick(item, 'giaTriMucTieu', 'GiaTriMucTieu'),
        giaTriDauKyCoDinh: pick(item, 'giaTriDauKyCoDinh', 'GiaTriDauKyCoDinh'),
        giaTriMucTieuText: String(pick(item, 'giaTriMucTieuText', 'GiaTriMucTieuText') || ''),
        ghiChu: String(pick(item, 'ghiChu', 'GhiChu') || ''),
        thuTuHienThi: Number(pick(item, 'thuTuHienThi', 'ThuTuHienThi') || 0)
    })

    const normalizeAssignment = (item) => ({
        id: Number(pick(item, 'id', 'Id') || 0),
        dotGiaoChiTieuId: Number(pick(item, 'dotGiaoChiTieuId', 'DotGiaoChiTieuId') || 0),
        maDotGiaoChiTieu: String(pick(item, 'maDotGiaoChiTieu', 'MaDotGiaoChiTieu', 'maDotGiao', 'MaDotGiao') || ''),
        tenDotGiaoChiTieu: String(pick(item, 'tenDotGiaoChiTieu', 'TenDotGiaoChiTieu') || ''),
        danhMucChiTieuId: Number(pick(item, 'danhMucChiTieuId', 'DanhMucChiTieuId') || 0),
        maDanhMucChiTieu: String(pick(item, 'maDanhMucChiTieu', 'MaDanhMucChiTieu') || ''),
        tenDanhMucChiTieu: String(pick(item, 'tenDanhMucChiTieu', 'TenDanhMucChiTieu') || ''),
        loaiChiTieu: String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || ''),
        tieuChiDanhGia: String(pick(item, 'tieuChiDanhGia', 'TieuChiDanhGia', 'loaiChiTieu', 'LoaiChiTieu') || ''),
        loaiMocSoSanh: String(pick(item, 'loaiMocSoSanh', 'LoaiMocSoSanh') || ''),
        chieuSoSanh: String(pick(item, 'chieuSoSanh', 'ChieuSoSanh') || ''),
        quyTacDanhGia: String(pick(item, 'quyTacDanhGia', 'QuyTacDanhGia') || ''),
        donViTinh: String(pick(item, 'donViTinh', 'DonViTinh') || ''),
        tanSuatBaoCao: String(pick(item, 'tanSuatBaoCao', 'TanSuatBaoCao') || ''),
        donViNhanId: Number(pick(item, 'donViNhanId', 'DonViNhanId') || 0),
        tenDonViNhan: String(pick(item, 'tenDonViNhan', 'TenDonViNhan') || ''),
        donViThucHienChinhId: Number(pick(item, 'donViThucHienChinhId', 'DonViThucHienChinhId') || 0),
        tenDonViThucHienChinh: String(pick(item, 'tenDonViThucHienChinh', 'TenDonViThucHienChinh') || ''),
        giaTriMucTieu: pick(item, 'giaTriMucTieu', 'GiaTriMucTieu'),
        giaTriDauKyCoDinh: pick(item, 'giaTriDauKyCoDinh', 'GiaTriDauKyCoDinh'),
        giaTriMucTieuText: String(pick(item, 'giaTriMucTieuText', 'GiaTriMucTieuText') || ''),
        ghiChu: String(pick(item, 'ghiChu', 'GhiChu') || ''),
        tieuChiCon: normalizeList(pick(item, 'tieuChiCon', 'TieuChiCon')).map(normalizeAssignmentChild)
    })

    const getKyBaoCaoLabel = (value) => tanSuatBaoCaoOptions.find(x => x.value === value)?.label || value || '-'

    const normalizeCode = (value) => String(value || '').trim().toUpperCase()
    const getEffectiveCriterion = (item) => normalizeCode(item?.tieuChiDanhGia || item?.loaiChiTieu)
    const getEffectiveLoaiMocSoSanh = (item) => normalizeCode(item?.loaiMocSoSanh)
    const getEffectiveChieuSoSanh = (item) => normalizeCode(item?.chieuSoSanh)
    const getEffectiveQuyTacDanhGia = (item) => normalizeCode(item?.quyTacDanhGia) || (isQualitative(getEffectiveCriterion(item)) ? 'MAC_DINH' : 'DAT_TOI_THIEU')

    const mapLoai = (value) => {
        if (normalizeCode(value) === 'PHAN_RA') {
            return 'Phân rã'
        }

        return getTieuChiDanhGiaLabel(value)
    }

    const isQualitative = (criterion) => isDinhTinhCriterion(normalizeCode(criterion))
    const isQuantitative = (criterion) => isQuantitativeCriterion(normalizeCode(criterion))

    const scopeMeta = computed(() => SCOPE_CONFIGS[props.scope] || SCOPE_CONFIGS.CADP_PHUONG_XA)
    const isCatpScope = computed(() => props.scope === 'CATP')

    const normalizeLoaiDonVi = (value) => String(value || '').trim().toUpperCase()
    const normalizeSearch = (value) =>
        String(value || '')
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .trim()
            .toUpperCase()

    const getDonViScopeKey = (item) => {
        const normalizedLoai = normalizeLoaiDonVi(item?.loaiDonVi)
        const normalizedTen = normalizeSearch(item?.tenDonVi)

        if (normalizedLoai.includes('CATP') || normalizedTen.includes(normalizeSearch(CATP_TEN_DON_VI))) {
            return 'CATP'
        }

        if (
            normalizedLoai.includes('PHUONG') ||
            normalizedLoai.includes('XA') ||
            normalizedLoai.includes('CAP_XA') ||
            normalizedLoai.includes('CAPXA')
        ) {
            return 'CADP_PHUONG_XA'
        }

        if (normalizedLoai.includes('PHONG')) {
            return 'PHONG'
        }

        return 'KHAC'
    }

    const matchesScopeForDonVi = (item) => {
        if (!item) return false

        const scopeKey = getDonViScopeKey(item)
        if (props.scope === 'CATP') return scopeKey === 'CATP'
        if (props.scope === 'PHONG') return scopeKey === 'PHONG'
        if (props.scope === 'CADP_PHUONG_XA') return scopeKey === 'CADP_PHUONG_XA'
        return scopeKey !== 'KHAC'
    }

    const getDonViGroupKey = (value, tenDonVi = '') => {
        return getDonViScopeKey({ loaiDonVi: value, tenDonVi })
    }

    const getDonViGroupLabel = (value, tenDonVi = '') => {
        const map = {
            CATP: 'Công an thành phố',
            PHONG: 'Khối phòng',
            CADP_PHUONG_XA: 'CADP phường/xã',
            KHAC: 'Đơn vị khác'
        }

        return map[getDonViGroupKey(value, tenDonVi)] || 'Đơn vị khác'
    }

    const getDonViGroupOrder = (value, tenDonVi = '') => {
        const orderMap = {
            CATP: 1,
            PHONG: 2,
            CADP_PHUONG_XA: 3,
            KHAC: 3
        }

        return orderMap[getDonViGroupKey(value, tenDonVi)] || 99
    }

    const getMocSoSanhLabel = (value) => {
        const map = {
            DAU_KY: 'đầu kỳ',
            CUNG_KY: 'cùng kỳ',
            KY_TRUOC: 'kỳ trước',
            TONG_NAM_TRUOC: 'tổng năm trước'
        }

        return map[value] || value || 'mốc so sánh'
    }

    const formatDate = (value) => {
        if (!value) return ''
        const date = new Date(value)
        if (Number.isNaN(date.getTime())) return ''
        return date.toLocaleDateString('vi-VN')
    }

    const formatDotRange = (item) => {
        if (!item) return ''

        const start = formatDate(item.ngayBatDau)
        const end = formatDate(item.ngayKetThuc)

        if (start && end) return `${start} - ${end}`
        return start || end || ''
    }

    const findDotById = (id) => dotOptions.value.find(item => item.id === Number(id)) || null

    const findDonViById = (id) => donViOptions.value.find(item => item.id === Number(id)) || null

    const formatDotRangeFromAssignment = (item) => {
        const dot = findDotById(item?.dotGiaoChiTieuId)
        return formatDotRange(dot)
    }

    const getDonViGroupLabelById = (donViId) => {
        const donVi = findDonViById(donViId)
        return getDonViGroupLabel(donVi?.loaiDonVi, donVi?.tenDonVi)
    }

    const formatNumber = (value) => {
        if (value === null || value === undefined || value === '') return '-'
        const numberValue = Number(value)
        if (Number.isNaN(numberValue)) return String(value)
        return numberValue.toLocaleString('vi-VN')
    }

    const formatAssignmentTarget = (item) => {
        if (isQualitative(getEffectiveCriterion(item))) {
            return item.giaTriMucTieuText || 'Chưa nhập mô tả mục tiêu'
        }

        if (item.giaTriMucTieu === null || item.giaTriMucTieu === undefined || item.giaTriMucTieu === '') {
            return 'Chưa nhập giá trị mục tiêu'
        }

        return `${formatNumber(item.giaTriMucTieu)}${item.donViTinh ? ` ${item.donViTinh}` : ''}`
    }

    const showGiaTriDauKyCoDinh = (item) => !isQualitative(getEffectiveCriterion(item))

    const formatAssignmentBaseline = (item) => {
        if (!showGiaTriDauKyCoDinh(item)) {
            return '-'
        }

        const value = item?.giaTriDauKyCoDinh
        if (value === null || value === undefined || value === '') {
            return 'Chưa nhập'
        }

        return `${formatNumber(value)}${item?.donViTinh ? ` ${item.donViTinh}` : ''}`
    }

    const buildCatalogHint = (item) => {
        if (!item) return ''

        const criterion = getEffectiveCriterion(item)
        const quyTac = getEffectiveQuyTacDanhGia(item)
        const chieu = getEffectiveChieuSoSanh(item)
        if (criterion === 'DINH_TINH') {
            return `Điều kiện hoàn thành: ${item.dieuKienHoanThanh || 'chưa khai báo'}`
        }

        if (criterion === 'DINH_LUONG_SO_SANH') {
            return `Đánh giá theo quy tắc ${getQuyTacDanhGiaLabel(quyTac).toLowerCase()}, chiều ${chieu === 'GIAM' ? 'giảm' : 'tăng'} và mốc ${getLoaiMocSoSanhLabel(getEffectiveLoaiMocSoSanh(item)).toLowerCase()}`
        }

        return `Đánh giá theo kết quả lũy kế, quy tắc ${getQuyTacDanhGiaLabel(quyTac).toLowerCase()} và chiều ${chieu === 'GIAM' ? 'giảm' : 'tăng'}${item.donViTinh ? ` (${item.donViTinh})` : ''}`
    }

    const selectedDanhMuc = computed(() => {
        if (!form.danhMucChiTieuId) return null
        return danhMucOptions.value.find(item => item.id === Number(form.danhMucChiTieuId)) || null
    })

    const selectedDot = computed(() => {
        if (!form.dotGiaoChiTieuId) return null
        return dotOptions.value.find(item => item.id === Number(form.dotGiaoChiTieuId)) || null
    })

    const selectedDonVi = computed(() => {
        if (!form.donViId) return null
        return scopedDonViOptions.value.find(item => item.id === Number(form.donViId)) || null
    })

    const scopedNhomThiDuaOptions = computed(() =>
        nhomThiDuaOptions.value
            .map((group) => {
                const donVis = group.donVis.filter((member) => {
                    const donVi = findDonViById(member.donViId)
                    return donVi && matchesScopeForDonVi(donVi)
                })
                return {
                    ...group,
                    donVis
                }
            })
            .filter(group => group.donVis.length > 0)
            .sort((left, right) => left.tenNhom.localeCompare(right.tenNhom, 'vi'))
    )

    const selectedNhomThiDua = computed(() => {
        if (!form.nhomThiDuaId) return null
        return scopedNhomThiDuaOptions.value.find(item => item.id === Number(form.nhomThiDuaId)) || null
    })

    const selectedNhomThiDuaUnitIds = computed(() => {
        if (!selectedNhomThiDua.value) return []
        return [...new Set(selectedNhomThiDua.value.donVis.map(item => Number(item.donViId)).filter(Boolean))]
    })

    const selectedNhomThiDuaTargetUnits = computed(() =>
        selectedNhomThiDuaUnitIds.value
            .map(id => findDonViById(id))
            .filter(Boolean)
    )

    const filteredSelectableUnits = computed(() => {
        const keyword = normalizeSearch(unitSelectionKeyword.value)
        if (!keyword) {
            return scopedDonViOptions.value
        }

        return scopedDonViOptions.value.filter((item) =>
            [item.maDonVi, item.tenDonVi, item.loaiDonVi].some((value) => normalizeSearch(value).includes(keyword))
        )
    })

    const selectedTargetUnitIds = computed(() => {
        if (isEdit.value) {
            return form.donViId ? [Number(form.donViId)] : []
        }

        if (props.scope === 'CATP' && defaultScopedDonVi.value) {
            return [Number(defaultScopedDonVi.value.id)]
        }

        if (form.phuongThucChonDonVi === 'NHOM_THI_DUA') {
            return selectedNhomThiDuaUnitIds.value
        }

        return [...new Set((form.donViIds || []).map(value => Number(value)).filter(Boolean))]
    })

    const selectedTargetUnits = computed(() =>
        selectedTargetUnitIds.value
            .map(id => findDonViById(id))
            .filter(Boolean)
    )

    const scopedDonViOptions = computed(() => {
        return donViOptions.value.filter(matchesScopeForDonVi)
    })

    const defaultScopedDonVi = computed(() => {
        if (props.scope !== 'CATP') return null
        return scopedDonViOptions.value[0] || null
    })

    const groupedDonViOptions = computed(() => {
        const groups = new Map()

        ;['CATP', 'PHONG', 'CADP_PHUONG_XA', 'KHAC'].forEach((key) => {
            groups.set(key, {
                key,
                label: getDonViGroupLabel(key),
                items: []
            })
        })

        scopedDonViOptions.value.forEach((item) => {
            const key = getDonViGroupKey(item.loaiDonVi, item.tenDonVi)
            groups.get(key)?.items.push(item)
        })

        return Array.from(groups.values())
            .map(group => ({
                ...group,
                items: [...group.items].sort((left, right) =>
                    `${left.maDonVi} ${left.tenDonVi}`.localeCompare(`${right.maDonVi} ${right.tenDonVi}`, 'vi')
                )
            }))
            .filter(group => group.items.length > 0)
    })

    const assignableDanhMucOptions = computed(() => {
        return [...danhMucOptions.value]
            .filter(item => !item.chiTieuChaId)
            .sort((left, right) => {
                const orderDiff = (left.thuTuHienThi || Number.MAX_SAFE_INTEGER) - (right.thuTuHienThi || Number.MAX_SAFE_INTEGER)
                if (orderDiff !== 0) return orderDiff
                return `${left.maChiTieu} ${left.tenChiTieu}`.localeCompare(`${right.maChiTieu} ${right.tenChiTieu}`, 'vi')
            })
    })

    const filteredItems = computed(() => {
        const filtered = items.value.filter(item => {
            const donViNhan = findDonViById(item.donViNhanId)
            if (!matchesScopeForDonVi(donViNhan)) {
                return false
            }

            const keyword = filters.keyword.trim().toLowerCase()
            const searchText = [
                item.tenDotGiaoChiTieu,
                item.maDotGiaoChiTieu,
                item.maDanhMucChiTieu,
                item.tenDanhMucChiTieu,
                item.tenDonViNhan,
                item.tenDonViThucHienChinh,
                item.ghiChu,
                ...item.tieuChiCon.flatMap(child => [
                    child.maDanhMucChiTieu,
                    child.tenDanhMucChiTieu,
                    child.ghiChu
                ])
            ].join(' ').toLowerCase()

            const matchKeyword = !keyword || searchText.includes(keyword)
            const matchDot = !filters.dotGiaoChiTieuId || item.dotGiaoChiTieuId === Number(filters.dotGiaoChiTieuId)
            const matchDanhMuc = !filters.danhMucChiTieuId || item.danhMucChiTieuId === Number(filters.danhMucChiTieuId)
            const matchDonVi = !filters.donViId || item.donViNhanId === Number(filters.donViId)

            return matchKeyword && matchDot && matchDanhMuc && matchDonVi
        })

        return filtered.sort((left, right) => {
            const donViLeft = findDonViById(left.donViNhanId)
            const donViRight = findDonViById(right.donViNhanId)

            const groupOrderDiff =
                getDonViGroupOrder(donViLeft?.loaiDonVi, donViLeft?.tenDonVi) -
                getDonViGroupOrder(donViRight?.loaiDonVi, donViRight?.tenDonVi)
            if (groupOrderDiff !== 0) return groupOrderDiff

            const donViDiff = `${donViLeft?.maDonVi || ''} ${left.tenDonViNhan || ''}`.localeCompare(
                `${donViRight?.maDonVi || ''} ${right.tenDonViNhan || ''}`,
                'vi'
            )
            if (donViDiff !== 0) return donViDiff

            const dotDiff = `${left.tenDotGiaoChiTieu || ''} ${left.maDotGiaoChiTieu || ''}`.localeCompare(
                `${right.tenDotGiaoChiTieu || ''} ${right.maDotGiaoChiTieu || ''}`,
                'vi'
            )
            if (dotDiff !== 0) return dotDiff

            return `${left.maDanhMucChiTieu || ''} ${left.tenDanhMucChiTieu || ''}`.localeCompare(
                `${right.maDanhMucChiTieu || ''} ${right.tenDanhMucChiTieu || ''}`,
                'vi'
            )
        })
    })

    const hydrateTargetsFromDanhMuc = (existingAssignment = null) => {
        const danhMuc = selectedDanhMuc.value

        if (!danhMuc) {
            form.tieuChiDanhGia = ''
            form.loaiMocSoSanh = ''
            form.chieuSoSanh = ''
            form.giaTriMucTieu = ''
            form.giaTriDauKyCoDinh = ''
            form.giaTriMucTieuText = ''
            form.tieuChiCon = []
            return
        }

        form.tieuChiDanhGia = existingAssignment?.tieuChiDanhGia || danhMuc.tieuChiDanhGia || danhMuc.loaiChiTieu || 'DINH_TINH'
        form.loaiMocSoSanh = existingAssignment?.loaiMocSoSanh || danhMuc.loaiMocSoSanh || ''
        form.chieuSoSanh = existingAssignment?.chieuSoSanh || danhMuc.chieuSoSanh || 'TANG'
        form.quyTacDanhGia = existingAssignment?.quyTacDanhGia || (isQualitative(form.tieuChiDanhGia) ? 'MAC_DINH' : 'DAT_TOI_THIEU')

        if (danhMuc.tieuChiDanhGias.length > 0) {
            const assignmentByCatalogId = new Map((existingAssignment?.tieuChiCon || []).map(child => [child.danhMucChiTieuId, child]))
            form.giaTriMucTieu = ''
            form.giaTriMucTieuText = ''
            form.tieuChiCon = danhMuc.tieuChiDanhGias.map((criterion, index) => {
                const assigned = assignmentByCatalogId.get(criterion.id)
                return {
                    localKey: `${criterion.id}-${index}-${Math.random()}`,
                    danhMucChiTieuId: criterion.id,
                    maChiTieu: criterion.maChiTieu,
                    tenChiTieu: criterion.tenChiTieu,
                    loaiChiTieu: criterion.loaiChiTieu,
                    tieuChiDanhGia: assigned?.tieuChiDanhGia || criterion.tieuChiDanhGia || criterion.loaiChiTieu || 'DINH_TINH',
                    donViTinh: criterion.donViTinh || danhMuc.donViTinh,
                    giaTriMucTieu: assigned?.giaTriMucTieu ?? '',
                    giaTriDauKyCoDinh: assigned?.giaTriDauKyCoDinh ?? 0,
                    giaTriMucTieuText: assigned?.giaTriMucTieuText || '',
                    ghiChu: assigned?.ghiChu || '',
                    thuTuHienThi: assigned?.thuTuHienThi || criterion.thuTuHienThi || index + 1,
                    dieuKienHoanThanh: criterion.dieuKienHoanThanh,
                    dieuKienKhongHoanThanh: criterion.dieuKienKhongHoanThanh,
                    tyLePhanTramMucTieu: criterion.tyLePhanTramMucTieu,
                    loaiMocSoSanh: assigned?.loaiMocSoSanh || criterion.loaiMocSoSanh || '',
                    chieuSoSanh: assigned?.chieuSoSanh || criterion.chieuSoSanh || 'TANG',
                    quyTacDanhGia: assigned?.quyTacDanhGia || (isQualitative(criterion.tieuChiDanhGia || criterion.loaiChiTieu) ? 'MAC_DINH' : 'DAT_TOI_THIEU')
                }
            })
            return
        }

        form.tieuChiCon = []
        form.giaTriMucTieu = existingAssignment?.giaTriMucTieu ?? ''
        form.giaTriDauKyCoDinh = existingAssignment?.giaTriDauKyCoDinh ?? 0
        form.giaTriMucTieuText = existingAssignment?.giaTriMucTieuText || ''
    }

    watch(() => form.danhMucChiTieuId, () => {
        if (syncingForm.value) return
        hydrateTargetsFromDanhMuc()
    })

    const switchUnitSelectionMode = (mode) => {
        if (props.scope === 'CATP' || isEdit.value) return
        form.phuongThucChonDonVi = mode
        if (mode === 'DON_VI') {
            form.nhomThiDuaId = null
        } else {
            form.donViIds = []
        }
    }

    const toggleDonViSelection = (donViId) => {
        const normalizedId = Number(donViId)
        const currentValues = new Set((form.donViIds || []).map(value => Number(value)).filter(Boolean))

        if (currentValues.has(normalizedId)) {
            currentValues.delete(normalizedId)
        } else {
            currentValues.add(normalizedId)
        }

        form.donViIds = Array.from(currentValues)
    }

    const toggleVisibleSelectableUnits = (shouldSelect) => {
        const visibleIds = filteredSelectableUnits.value.map(item => Number(item.id)).filter(Boolean)
        const currentValues = new Set((form.donViIds || []).map(value => Number(value)).filter(Boolean))

        visibleIds.forEach((id) => {
            if (shouldSelect) {
                currentValues.add(id)
            } else {
                currentValues.delete(id)
            }
        })

        form.donViIds = Array.from(currentValues)
    }

    const ensureCriterionDefaults = (target) => {
        const criterion = target?.tieuChiDanhGia || target?.loaiChiTieu
        if (!criterion) return

        if (isQualitative(criterion)) {
            target.quyTacDanhGia = 'MAC_DINH'
            target.loaiMocSoSanh = ''
            target.chieuSoSanh = ''
            return
        }

        if (!target.quyTacDanhGia || target.quyTacDanhGia === 'MAC_DINH') {
            target.quyTacDanhGia = 'DAT_TOI_THIEU'
        }

        if (!target.chieuSoSanh) {
            target.chieuSoSanh = 'TANG'
        }

        if (!isDinhLuongSoSanhCriterion(criterion)) {
            target.loaiMocSoSanh = ''
        }
    }

    watch(() => form.tieuChiDanhGia, () => {
        if (syncingForm.value) return
        ensureCriterionDefaults(form)
    })

    watch(
        () => form.tieuChiCon.map(child => child.tieuChiDanhGia),
        () => {
            if (syncingForm.value) return
            form.tieuChiCon.forEach(ensureCriterionDefaults)
        },
        { deep: true }
    )

    const getResolvedDonViId = () => {
        if (form.donViId) {
            return Number(form.donViId)
        }

        if (defaultScopedDonVi.value) {
            return Number(defaultScopedDonVi.value.id)
        }

        return null
    }

    const applyScopeDefaultDonVi = () => {
        if (props.scope === 'CATP' && defaultScopedDonVi.value) {
            form.donViId = Number(defaultScopedDonVi.value.id)
            form.donViIds = [Number(defaultScopedDonVi.value.id)]
        }
    }

    const buildPayloadForDonVi = (donViId) => ({
        dotGiaoChiTieuId: form.dotGiaoChiTieuId,
        danhMucChiTieuId: form.danhMucChiTieuId,
        donViNhanId: Number(donViId),
        donViThucHienChinhId: Number(donViId),
        tanSuatBaoCao: form.tanSuatBaoCao || null,
        tieuChiDanhGia: form.tieuChiDanhGia || null,
        quyTacDanhGia: isQualitative(form.tieuChiDanhGia || selectedDanhMuc.value?.loaiChiTieu) ? null : form.quyTacDanhGia || null,
        loaiMocSoSanh: isDinhLuongSoSanhCriterion(form.tieuChiDanhGia) ? form.loaiMocSoSanh || null : null,
        chieuSoSanh: isQualitative(form.tieuChiDanhGia || selectedDanhMuc.value?.loaiChiTieu) ? null : form.chieuSoSanh || null,
        giaTriMucTieu: selectedDanhMuc.value && !selectedDanhMuc.value.tieuChiDanhGias.length && !isQualitative(form.tieuChiDanhGia || selectedDanhMuc.value.loaiChiTieu)
            ? Number(form.giaTriMucTieu)
            : null,
        giaTriDauKyCoDinh: selectedDanhMuc.value && !selectedDanhMuc.value.tieuChiDanhGias.length && !isQualitative(form.tieuChiDanhGia || selectedDanhMuc.value.loaiChiTieu)
            ? Number(form.giaTriDauKyCoDinh)
            : null,
        giaTriMucTieuText: selectedDanhMuc.value && !selectedDanhMuc.value.tieuChiDanhGias.length && isQualitative(form.tieuChiDanhGia || selectedDanhMuc.value.loaiChiTieu)
            ? form.giaTriMucTieuText?.trim() || null
            : null,
        ghiChu: form.ghiChu?.trim() || null,
        tieuChiCon: form.tieuChiCon.map((child) => ({
            danhMucChiTieuId: child.danhMucChiTieuId,
            tieuChiDanhGia: child.tieuChiDanhGia || null,
            quyTacDanhGia: isQualitative(child.tieuChiDanhGia || child.loaiChiTieu) ? null : child.quyTacDanhGia || null,
            loaiMocSoSanh: isDinhLuongSoSanhCriterion(child.tieuChiDanhGia) ? child.loaiMocSoSanh || null : null,
            chieuSoSanh: isQualitative(child.tieuChiDanhGia || child.loaiChiTieu) ? null : child.chieuSoSanh || null,
            giaTriMucTieu: isQualitative(child.tieuChiDanhGia || child.loaiChiTieu) ? null : Number(child.giaTriMucTieu),
            giaTriDauKyCoDinh: isQualitative(child.tieuChiDanhGia || child.loaiChiTieu) ? null : Number(child.giaTriDauKyCoDinh),
            giaTriMucTieuText: isQualitative(child.tieuChiDanhGia || child.loaiChiTieu) ? child.giaTriMucTieuText?.trim() || null : null,
            ghiChu: child.ghiChu?.trim() || null,
            thuTuHienThi: child.thuTuHienThi || null
        }))
    })

    const validateCriterionConfig = (item, label) => {
        if (!item?.tieuChiDanhGia) {
            alert(`Vui lòng chọn tiêu chí đánh giá cho ${label}.`)
            return false
        }

        if (!isQualitative(item.tieuChiDanhGia) && !item?.quyTacDanhGia) {
            alert(`Vui lòng chọn quy tắc đánh giá cho ${label}.`)
            return false
        }

        if (!isQualitative(item.tieuChiDanhGia) && item?.quyTacDanhGia === 'MAC_DINH') {
            alert(`Quy tắc đánh giá của ${label} chưa hợp lệ với chỉ tiêu định lượng.`)
            return false
        }

        if (!isQualitative(item.tieuChiDanhGia) && !item?.chieuSoSanh) {
            alert(`Vui lòng chọn chiều đánh giá cho ${label}.`)
            return false
        }

        if (isDinhLuongSoSanhCriterion(item.tieuChiDanhGia)) {
            if (!item.loaiMocSoSanh) {
                alert(`Vui lòng chọn mốc so sánh cho ${label}.`)
                return false
            }
        }

        return true
    }

    const validateForm = () => {
        if (!form.dotGiaoChiTieuId) {
            alert('Vui lòng chọn đợt giao chỉ tiêu.')
            return false
        }

        if (!form.danhMucChiTieuId) {
            alert('Vui lòng chọn danh mục chỉ tiêu.')
            return false
        }

        if (!selectedTargetUnitIds.value.length) {
            alert(`Vui lòng chọn ít nhất một ${scopeMeta.value.receiverLabel.toLowerCase()}.`)
            return false
        }

        if (!form.tanSuatBaoCao) {
            alert('Vui lòng chọn kỳ báo cáo.')
            return false
        }

        if (!selectedDanhMuc.value) {
            alert('Không tìm thấy danh mục chỉ tiêu đã chọn.')
            return false
        }

        if (!validateCriterionConfig(form, 'chỉ tiêu giao')) {
            return false
        }

        if (selectedDanhMuc.value.tieuChiDanhGias.length > 0) {
            for (let i = 0; i < form.tieuChiCon.length; i += 1) {
                const child = form.tieuChiCon[i]
                if (!validateCriterionConfig(child, `tiêu chí con ${i + 1}`)) {
                    return false
                }

                if (isQualitative(child.tieuChiDanhGia || child.loaiChiTieu)) {
                    if (!child.giaTriMucTieuText?.trim()) {
                        alert(`Vui lòng nhập mô tả mục tiêu cho tiêu chí con ${i + 1}.`)
                        return false
                    }
                } else {
                    if (child.giaTriMucTieu === '' || child.giaTriMucTieu === null || child.giaTriMucTieu === undefined) {
                        alert(`Vui lòng nhập giá trị mục tiêu cho tiêu chí con ${i + 1}.`)
                        return false
                    }
                    if (Number.isNaN(Number(child.giaTriMucTieu))) {
                        alert(`Giá trị mục tiêu của tiêu chí con ${i + 1} không hợp lệ.`)
                        return false
                    }
                    if (child.giaTriDauKyCoDinh === '' || child.giaTriDauKyCoDinh === null || child.giaTriDauKyCoDinh === undefined) {
                        alert(`Vui lòng nhập đầu kỳ cố định cho tiêu chí con ${i + 1}.`)
                        return false
                    }
                    if (Number.isNaN(Number(child.giaTriDauKyCoDinh)) || Number(child.giaTriDauKyCoDinh) < 0) {
                        alert(`Đầu kỳ cố định của tiêu chí con ${i + 1} không hợp lệ.`)
                        return false
                    }
                }
            }
            return true
        }

        if (isQualitative(form.tieuChiDanhGia || selectedDanhMuc.value.loaiChiTieu)) {
            if (!form.giaTriMucTieuText?.trim()) {
                alert('Vui lòng nhập mô tả mục tiêu.')
                return false
            }
            return true
        }

        if (form.giaTriMucTieu === '' || form.giaTriMucTieu === null || form.giaTriMucTieu === undefined) {
            alert('Vui lòng nhập giá trị mục tiêu.')
            return false
        }

        if (Number.isNaN(Number(form.giaTriMucTieu))) {
            alert('Giá trị mục tiêu không hợp lệ.')
            return false
        }

        if (form.giaTriDauKyCoDinh === '' || form.giaTriDauKyCoDinh === null || form.giaTriDauKyCoDinh === undefined) {
            alert('Vui lòng nhập giá trị đầu kỳ cố định.')
            return false
        }

        if (Number.isNaN(Number(form.giaTriDauKyCoDinh)) || Number(form.giaTriDauKyCoDinh) < 0) {
            alert('Giá trị đầu kỳ cố định không hợp lệ.')
            return false
        }

        return true
    }

    const loadData = async () => {
        try {
            loading.value = true
            const [chiTietData, dotData, danhMucData, donViData, nhomThiDuaData] = await Promise.all([
                apiRequest(API_PATHS.chiTietGiaoChiTieu),
                apiRequest(API_PATHS.dotGiaoChiTieu),
                apiRequest(API_PATHS.danhMucChiTieu),
                apiRequest(API_PATHS.donVi),
                apiRequest(API_PATHS.nhomThiDua)
            ])

            items.value = normalizeList(chiTietData).map(normalizeAssignment)
            dotOptions.value = normalizeList(dotData).map(normalizeDot)
            danhMucOptions.value = normalizeList(danhMucData).map(normalizeDanhMuc)
            donViOptions.value = normalizeList(donViData).map(normalizeDonVi)
            nhomThiDuaOptions.value = normalizeList(nhomThiDuaData).map(normalizeNhomThiDua)
            applyScopeDefaultDonVi()
        } catch (error) {
            console.error(error)
            alert(error.message || 'Không tải được dữ liệu giao chỉ tiêu.')
            items.value = []
            dotOptions.value = []
            danhMucOptions.value = []
            donViOptions.value = []
            nhomThiDuaOptions.value = []
        } finally {
            loading.value = false
        }
    }

    const openCreateModal = () => {
        isEdit.value = false
        editingId.value = null
        resetForm()
        applyScopeDefaultDonVi()
        showModal.value = true
    }

    const openEditModal = (item) => {
        isEdit.value = true
        editingId.value = item.id
        syncingForm.value = true
        Object.assign(form, {
            dotGiaoChiTieuId: item.dotGiaoChiTieuId,
            danhMucChiTieuId: item.danhMucChiTieuId,
            donViId: item.donViNhanId,
            donViIds: item.donViNhanId ? [item.donViNhanId] : [],
            nhomThiDuaId: null,
            phuongThucChonDonVi: 'DON_VI',
            tanSuatBaoCao: item.tanSuatBaoCao,
            tieuChiDanhGia: item.tieuChiDanhGia || item.loaiChiTieu || '',
            loaiMocSoSanh: item.loaiMocSoSanh || '',
            chieuSoSanh: item.chieuSoSanh || 'TANG',
            quyTacDanhGia: item.quyTacDanhGia || (isQualitative(item.tieuChiDanhGia || item.loaiChiTieu) ? 'MAC_DINH' : 'DAT_TOI_THIEU'),
            giaTriMucTieu: item.giaTriMucTieu ?? '',
            giaTriDauKyCoDinh: item.giaTriDauKyCoDinh ?? 0,
            giaTriMucTieuText: item.giaTriMucTieuText || '',
            ghiChu: item.ghiChu || '',
            tieuChiCon: []
        })
        syncingForm.value = false
        hydrateTargetsFromDanhMuc(item)
        ensureCriterionDefaults(form)
        form.tieuChiCon.forEach(ensureCriterionDefaults)
        showModal.value = true
    }

    const closeModal = () => {
        showModal.value = false
        resetForm()
        applyScopeDefaultDonVi()
    }

    const handleSubmit = async () => {
        if (!validateForm()) return

        try {
            saving.value = true

            if (isEdit.value && editingId.value) {
                const payload = buildPayloadForDonVi(getResolvedDonViId())
                await apiRequest(`${API_PATHS.chiTietGiaoChiTieu}/${editingId.value}`, 'PUT', payload)
            } else {
                const targetIds = selectedTargetUnitIds.value
                for (const donViId of targetIds) {
                    const payload = buildPayloadForDonVi(donViId)
                    await apiRequest(API_PATHS.chiTietGiaoChiTieu, 'POST', payload)
                }
            }

            closeModal()
            await loadData()
        } catch (error) {
            console.error(error)
            alert(error.message || 'Lưu giao chỉ tiêu thất bại.')
        } finally {
            saving.value = false
        }
    }

    const handleDelete = async (item) => {
        const ok = window.confirm(`Bạn có chắc muốn xóa giao chỉ tiêu "${item.tenDanhMucChiTieu}" của đơn vị "${item.tenDonViNhan}" không?`)
        if (!ok) return

        try {
            await apiRequest(`${API_PATHS.chiTietGiaoChiTieu}/${item.id}`, 'DELETE')
            await loadData()
        } catch (error) {
            console.error(error)
            alert(error.message || 'Xóa giao chỉ tiêu thất bại.')
        }
    }

    const resetFilters = () => {
        filters.dotGiaoChiTieuId = null
        filters.danhMucChiTieuId = null
        filters.donViId = null
        filters.keyword = ''

        if (props.scope === 'CATP' && defaultScopedDonVi.value) {
            filters.donViId = Number(defaultScopedDonVi.value.id)
        }
    }

    watch(
        () => props.scope,
        () => {
            resetForm()
            resetFilters()
            applyScopeDefaultDonVi()
        }
    )

    watch(
        scopedDonViOptions,
        () => {
            if (props.scope === 'CATP' && defaultScopedDonVi.value) {
                form.donViId = Number(defaultScopedDonVi.value.id)
                form.donViIds = [Number(defaultScopedDonVi.value.id)]
            }
        },
        { immediate: true }
    )

    watch(
        () => form.nhomThiDuaId,
        () => {
            if (form.phuongThucChonDonVi !== 'NHOM_THI_DUA') return
            form.donViIds = []
        }
    )

    onMounted(() => {
        loadData()
    })
</script>

<style scoped>
    .page-wrap {
        min-height: 100vh;
        background: linear-gradient(180deg, #f8fbff 0%, #eef5fb 100%);
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

    .gov-title {
        font-size: 1.3rem;
        font-weight: 800;
        color: #1f2d3d;
        line-height: 1.3;
    }

    .custom-card {
        border: none;
        border-radius: 20px;
        box-shadow: 0 8px 24px rgba(15, 23, 42, 0.06);
        overflow: hidden;
    }

    .custom-card .card-header,
    .custom-card .card-body {
        padding: 1.25rem;
    }

    .btn-action {
        border-radius: 12px;
        font-weight: 600;
        padding: 10px 18px;
    }

    .form-label {
        font-weight: 600;
        color: #374151;
        margin-bottom: 0.45rem;
    }

    .form-control,
    .form-select {
        min-height: 44px;
        border-radius: 12px;
        border-color: #dbe3ef;
    }

    .empty-state {
        padding: 48px 20px;
        text-align: center;
        color: #64748b;
    }

    .criteria-block,
    .criteria-card,
    .target-line {
        border: 1px solid #dbeafe;
        background: #f8fbff;
        border-radius: 18px;
        padding: 16px;
    }

    .section-title {
        font-size: 1rem;
        font-weight: 700;
        color: #1f2d3d;
    }

    .criteria-tags {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
    }

    .table-stack {
        display: flex;
        flex-direction: column;
        gap: 8px;
    }

    .table-stack-line {
        padding-bottom: 8px;
        border-bottom: 1px dashed #dbe3ef;
    }

    .table-stack-line:last-child {
        padding-bottom: 0;
        border-bottom: none;
    }

    .selection-summary {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
        gap: 12px;
    }

    .selection-card {
        border: 1px solid #dbeafe;
        background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
        border-radius: 18px;
        padding: 14px 16px;
    }

    .selection-label {
        font-size: 0.75rem;
        font-weight: 700;
        color: #2563eb;
        letter-spacing: 0.04em;
        text-transform: uppercase;
        margin-bottom: 6px;
    }

    .selection-value {
        color: #1f2d3d;
        font-weight: 700;
        line-height: 1.4;
    }

    .selection-meta {
        color: #64748b;
        font-size: 0.875rem;
        margin-top: 4px;
    }

    .receiver-picker {
        border: 1px solid #dbeafe;
        background: linear-gradient(180deg, #ffffff 0%, #f8fbff 100%);
        border-radius: 20px;
        padding: 18px;
    }

    .receiver-picker-head {
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        gap: 16px;
        margin-bottom: 16px;
    }

    .receiver-mode-switch {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
    }

    .selected-unit-board {
        min-height: 44px;
        padding: 10px 12px;
        border: 1px solid #dbe3ef;
        border-radius: 14px;
        background: #fff;
    }

    .selected-unit-list {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
    }

    .selected-unit-chip {
        display: inline-flex;
        align-items: center;
        padding: 6px 10px;
        border-radius: 999px;
        background: #eff6ff;
        border: 1px solid #bfdbfe;
        color: #1e3a8a;
        font-size: 0.875rem;
        font-weight: 600;
    }

    .selection-actions {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
    }

    .receiver-checkbox-list {
        max-height: 260px;
        overflow: auto;
        border: 1px solid #dbe3ef;
        border-radius: 16px;
        background: #fff;
        padding: 10px;
    }

    .receiver-checkbox-item {
        display: flex;
        align-items: flex-start;
        gap: 10px;
        padding: 10px 8px;
        border-radius: 12px;
    }

    .receiver-checkbox-item:hover {
        background: #f8fbff;
    }

    .receiver-checkbox-item input {
        margin-top: 4px;
        flex-shrink: 0;
    }

    .receiver-checkbox-item span {
        display: flex;
        flex-direction: column;
        gap: 2px;
    }

    .receiver-checkbox-item small {
        color: #64748b;
    }

    @media (max-width: 767.98px) {
        .receiver-picker-head {
            flex-direction: column;
        }
    }
</style>



