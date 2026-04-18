<template>
    <BaseLayout>
        <div class="page-wrap">
            <div class="container-fluid py-4">
                <div class="gov-banner mb-4">
                    <div class="gov-emblem">
                        <i class="bi bi-list-check"></i>
                    </div>
                    <div class="gov-text">
                        <div class="wave-title">HỆ THỐNG THEO DÕI CHỈ TIÊU</div>
                        <div class="gov-title">QUẢN LÝ DANH MỤC CHỈ TIÊU</div>
                    </div>
                </div>

                <div class="d-flex flex-column flex-md-row align-items-md-center justify-content-between gap-3 mb-4">
                    <div class="gov-banner">
                        <img src="https://upload.wikimedia.org/wikipedia/commons/a/a3/Emblem_of_Vietnam.svg"
                            class="gov-emblem" />
                    </div>

                    <div class="d-flex flex-wrap gap-2">
                        <button class="btn btn-outline-primary btn-action" @click="openImportFilePicker" :disabled="importing">
                            <i class="bi bi-file-earmark-excel me-2"></i>
                            {{ importing ? 'Đang nhập...' : 'Nhập từ Excel' }}
                        </button>
                        <button class="btn btn-primary btn-action" @click="openCreateModal">
                            <i class="bi bi-plus-circle me-2"></i>
                            Tạo chỉ tiêu
                        </button>
                    </div>
                </div>

                <input
                    ref="importFileInput"
                    type="file"
                    class="d-none"
                    accept=".xlsx,.xls"
                    @change="handleImportFileChange"
                />

                <div class="card custom-card mb-4 import-guide-card">
                    <div class="card-body">
                        <div class="import-guide-head">
                            <div>
                                <h5 class="mb-1">Hướng dẫn nhập Excel</h5>
                                <small class="text-muted">Mỗi dòng tương ứng một danh mục chỉ tiêu. Chỉ tiêu phân rã dùng thêm cột JSON cho tiêu chí con.</small>
                            </div>
                            <button class="btn btn-sm btn-outline-secondary" @click="downloadImportTemplate">
                                <i class="bi bi-download me-1"></i>
                                Tải mẫu CSV
                            </button>
                        </div>

                        <div class="import-guide-columns">
                            <span class="guide-chip">MaChiTieu</span>
                            <span class="guide-chip">TenChiTieu</span>
                            <span class="guide-chip">NguonChiTieu</span>
                            <span class="guide-chip">CheDoDanhGia</span>
                            <span class="guide-chip">LoaiChiTieu</span>
                            <span class="guide-chip">TrangThaiSuDung</span>
                            <span class="guide-chip">LinhVucNghiepVu</span>
                            <span class="guide-chip">DonViTinh</span>
                            <span class="guide-chip">TieuChiDanhGiasJson</span>
                        </div>

                        <div class="text-muted small mt-2">
                            `CheDoDanhGia` nhận <strong>DON</strong> hoặc <strong>PHAN_RA</strong>. Nếu là phân rã, cột
                            <strong>TieuChiDanhGiasJson</strong> phải là mảng JSON tối thiểu 2 tiêu chí con.
                        </div>
                    </div>
                </div>

                <div v-if="importResult" class="card custom-card mb-4">
                    <div class="card-body">
                        <div class="import-guide-head">
                            <div>
                                <h5 class="mb-1">Kết quả nhập Excel</h5>
                                <small class="text-muted">{{ importResult.fileName }}</small>
                            </div>
                            <span class="badge text-bg-light border">Tổng dòng: {{ importResult.totalRows }}</span>
                        </div>

                        <div class="import-result-grid">
                            <div class="result-box success">
                                <span>Thành công</span>
                                <strong>{{ importResult.createdCount }}</strong>
                            </div>
                            <div class="result-box warning">
                                <span>Bỏ qua</span>
                                <strong>{{ importResult.skippedCount }}</strong>
                            </div>
                            <div class="result-box danger">
                                <span>Lỗi</span>
                                <strong>{{ importResult.failedCount }}</strong>
                            </div>
                        </div>

                        <div v-if="importResult.messages.length" class="import-message-list">
                            <div
                                v-for="(message, index) in importResult.messages"
                                :key="`${message.type}-${index}`"
                                class="import-message-item"
                                :class="message.type"
                            >
                                {{ message.text }}
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card custom-card mb-4">
                    <div class="card-header bg-white border-0 pb-0">
                        <h5 class="mb-1">Bộ lọc tìm kiếm</h5>
                        <small class="text-muted">Tra cứu nhanh danh mục chỉ tiêu</small>
                    </div>

                    <div class="card-body">
                        <div class="row g-3">
                            <div class="col-12 col-md-6 col-xl-4">
                                <label class="form-label">Từ khóa</label>
                                <input v-model="filters.keyword" type="text" class="form-control"
                                    placeholder="Nhập mã hoặc tên chỉ tiêu" />
                            </div>

                            <div class="col-12 col-md-6 col-xl-2">
                                <label class="form-label">Nguồn</label>
                                <select v-model="filters.nguonChiTieu" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option value="BO">Bộ Công an</option>
                                    <option value="THANH_PHO">Công an Thành phố</option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Loại chỉ tiêu</label>
                                <select v-model="filters.loaiChiTieu" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option value="DINH_TINH">Định tính</option>
                                    <option value="DINH_LUONG_TICH_LUY">Định lượng tích lũy</option>
                                    <option value="DINH_LUONG_SO_SANH">Định lượng so sánh</option>
                                    <option value="PHAN_RA">Phân rã nhiều tiêu chí</option>
                                </select>
                            </div>

                            <div class="col-12 col-md-6 col-xl-3">
                                <label class="form-label">Trạng thái</label>
                                <select v-model="filters.trangThaiSuDung" class="form-select">
                                    <option value="">Tất cả</option>
                                    <option value="DANG_AP_DUNG">Đang áp dụng</option>
                                    <option value="NGUNG_AP_DUNG">Ngừng áp dụng</option>
                                </select>
                            </div>
                        </div>

                        <div class="d-flex flex-wrap gap-2 mt-4">
                            <button class="btn btn-primary" @click="fetchDanhMucChiTieu">
                                <i class="bi bi-search me-1"></i>
                                Tìm kiếm
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
                            <h5 class="mb-1">Danh sách chỉ tiêu</h5>
                                        <small class="text-muted">Bao gồm chỉ tiêu đơn và chỉ tiêu phân rã thành nhiều tiêu chí đánh
                                            giá</small>
                        </div>
                        <span class="badge text-bg-light border">Tổng: {{ items.length }}</span>
                    </div>

                    <div class="card-body p-0">
                        <div v-if="loading" class="empty-state">
                            <div class="spinner-border text-primary mb-3" role="status"></div>
                            <div>Đang tải dữ liệu...</div>
                        </div>

                        <div v-else-if="!items.length" class="empty-state">
                            <i class="bi bi-inbox fs-1 text-muted mb-2"></i>
                            <div>Chưa có dữ liệu</div>
                        </div>

                        <div v-else class="table-responsive">
                            <table class="table table-hover align-middle mb-0 custom-table">
                                <thead>
                                    <tr>
                                        <th>Tên chỉ tiêu</th>
                                        <th>Loại</th>
                                        <th>Bộ tiêu chí</th>
                                        <th>Trạng thái</th>
                                        <th class="text-center" style="width: 180px">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="item in items" :key="item.id">
                                        <td>
                                            <div class="fw-semibold">{{ item.tenChiTieu }}</div>
                                            <div class="small text-muted mt-1">{{ item.nguonChiTieuLabel }} | {{
                                                item.linhVucNghiepVu || 'Chưa khai báo lĩnh vực' }}</div>
                                            <div v-if="item.tieuChiDanhGias.length" class="criterion-tags mt-2">
                                                <span v-for="child in item.tieuChiDanhGias"
                                                    :key="child.id || child.maChiTieu"
                                                    class="badge text-bg-light border">
                                                    {{ child.tenChiTieu }}
                                                </span>
                                            </div>
                                        </td>
                                        <td>{{ mapLoai(item.loaiChiTieu) }}</td>
                                        <td>
                                            <div class="fw-semibold">{{ getCriteriaSummary(item) }} • {{ item.batBuocDatTatCaTieuChiCon && item.tieuChiDanhGias.length ? 'Phải đạt tất cả tiêu chí con' : 'Đánh giá theo 1 tiêu chí' }}</div>
                                        </td>
                                        <td>
                                            <span class="badge rounded-pill"
                                                :class="item.trangThaiSuDung === 'DANG_AP_DUNG' ? 'text-bg-success' : 'text-bg-danger'">
                                                {{ getTrangThaiLabel(item.trangThaiSuDung) }}
                                            </span>
                                        </td>
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
                                    <h4 class="modal-title mb-1">{{ isEdit ? 'Cập nhật chỉ tiêu' : 'Tạo chỉ tiêu mới' }}
                                    </h4>
                                    <p class="text-muted mb-0">Khai báo chỉ tiêu đơn hoặc chỉ tiêu phân rã thành nhiều
                                        tiêu chí đánh giá</p>
                                </div>
                                <button type="button" class="btn-close" @click="closeModal"></button>
                            </div>

                            <div class="modal-body pt-3">
                                <div class="row g-3">
                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Mã chỉ tiêu <span class="text-danger">*</span></label>
                                        <input v-model="form.maChiTieu" :disabled="isEdit" type="text"
                                            class="form-control" />
                                    </div>

                                    <div class="col-12 col-md-8">
                                        <label class="form-label">Tên chỉ tiêu <span
                                                class="text-danger">*</span></label>
                                        <input v-model="form.tenChiTieu" type="text" class="form-control" />
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Nguồn chỉ tiêu <span
                                                class="text-danger">*</span></label>
                                        <select v-model="form.nguonChiTieu" class="form-select">
                                            <option value="BO">Bộ Công an</option>
                                            <option value="THANH_PHO">Công an Thành phố</option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Lĩnh vực nghiệp vụ</label>
                                        <select :value="linhVucNghiepVuSelector" class="form-select"
                                            @change="handleLinhVucSelectorChange($event.target.value)">
                                            <option value="">Chọn lĩnh vực đã có</option>
                                            <option v-for="option in linhVucNghiepVuOptions" :key="option"
                                                :value="option">
                                                {{ option }}
                                            </option>
                                            <option :value="CUSTOM_OPTION_VALUE">Nhập lĩnh vực mới</option>
                                        </select>
                                        <input v-if="linhVucNghiepVuMode === 'custom'"
                                            v-model="form.linhVucNghiepVu" type="text" class="form-control mt-2"
                                            placeholder="Nhập lĩnh vực nghiệp vụ mới" />
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Đơn vị tính</label>
                                        <select :value="donViTinhSelector" class="form-select"
                                            @change="handleDonViTinhSelectorChange($event.target.value)">
                                            <option value="">Chọn đơn vị tính đã có</option>
                                            <option v-for="option in donViTinhOptions" :key="option" :value="option">
                                                {{ option }}
                                            </option>
                                            <option :value="CUSTOM_OPTION_VALUE">Nhập đơn vị tính mới</option>
                                        </select>
                                        <input v-if="donViTinhMode === 'custom'" v-model="form.donViTinh" type="text"
                                            class="form-control mt-2" placeholder="Ví dụ: %, vụ, người" />
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Chế độ đánh giá <span
                                                class="text-danger">*</span></label>
                                        <select v-model="form.cheDoDanhGia" class="form-select">
                                            <option value="DON">Chỉ tiêu đơn</option>
                                            <option value="PHAN_RA">Chỉ tiêu phân rã</option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Trạng thái sử dụng</label>
                                        <select v-model="form.trangThaiSuDung" class="form-select">
                                            <option value="DANG_AP_DUNG">Đang áp dụng</option>
                                            <option value="NGUNG_AP_DUNG">Ngừng áp dụng</option>
                                        </select>
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Bắt đầu hiệu lực</label>
                                        <input v-model="form.ngayHieuLuc" type="date" class="form-control" />
                                    </div>

                                    <div class="col-12 col-md-4">
                                        <label class="form-label">Kết thúc hiệu lực</label>
                                        <input v-model="form.ngayHetHieuLuc" type="date" class="form-control" />
                                    </div>

                                    <div class="col-12">
                                        <label class="form-label">Mô tả</label>
                                        <textarea v-model="form.moTa" rows="2" class="form-control"></textarea>
                                    </div>

                                    <div class="col-12">
                                        <label class="form-label">Hướng dẫn tính toán</label>
                                        <textarea v-model="form.huongDanTinhToan" rows="2"
                                            class="form-control"></textarea>
                                    </div>
                                    <div v-if="form.cheDoDanhGia === 'DON'" class="col-12">
                                        <div class="criteria-block">
                                            <div class="section-title">Tiêu chí đánh giá</div>
                                            <div class="row g-3 mt-1">
                                                <div class="col-12 col-md-4">
                                                    <label class="form-label">Loại tiêu chí <span
                                                            class="text-danger">*</span></label>
                                                    <select v-model="form.loaiChiTieu" class="form-select">
                                                        <option value="DINH_TINH">Định tính</option>
                                                        <option value="DINH_LUONG_TICH_LUY">Định lượng tích lũy</option>
                                                        <option value="DINH_LUONG_SO_SANH">Định lượng so sánh</option>
                                                    </select>
                                                </div>

                                                <div v-if="form.loaiChiTieu === 'DINH_TINH'" class="col-12 col-md-4">
                                                    <label class="form-label">Điều kiện hoàn thành</label>
                                                    <input v-model="form.dieuKienHoanThanh" type="text"
                                                        class="form-control" />
                                                </div>
                                                <div v-if="form.loaiChiTieu === 'DINH_TINH'" class="col-12 col-md-4">
                                                    <label class="form-label">Điều kiện không hoàn thành</label>
                                                    <input v-model="form.dieuKienKhongHoanThanh" type="text"
                                                        class="form-control" />
                                                </div>

                                                <template v-if="form.loaiChiTieu === 'DINH_LUONG_SO_SANH'">
                                                    <div class="col-12 col-md-4">
                                                        <label class="form-label">Tỷ lệ % mục tiêu</label>
                                                        <input v-model.number="form.tyLePhanTramMucTieu" type="number"
                                                            min="0" step="any" class="form-control" />
                                                    </div>
                                                    <div class="col-12 col-md-4">
                                                        <label class="form-label">Loại mốc so sánh</label>
                                                        <select v-model="form.loaiMocSoSanh" class="form-select">
                                                            <option value="">Chọn</option>
                                                            <option value="DAU_KY">Đầu kỳ</option>
                                                            <option value="CUNG_KY">Cùng kỳ</option>
                                                            <option value="KY_TRUOC">Kỳ trước</option>
                                                        </select>
                                                    </div>
                                                    <div class="col-12 col-md-4">
                                                        <label class="form-label">Chiều so sánh</label>
                                                        <select v-model="form.chieuSoSanh" class="form-select">
                                                            <option value="">Chọn</option>
                                                            <option value="TANG">Tăng</option>
                                                            <option value="GIAM">Giảm</option>
                                                        </select>
                                                    </div>
                                                </template>
                                            </div>
                                        </div>
                                    </div>

                                    <div v-else class="col-12">
                                        <div class="criteria-block">
                                            <div
                                                class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-3 mb-3">
                                                <div>
                                                    <div class="section-title mb-1">Bộ tiêu chí đánh giá</div>
                                                    <div class="text-muted small">Mỗi chỉ tiêu con là một tiêu chí đánh
                                                        giá. Kết quả chỉ tiêu cha sẽ dựa trên toàn bộ tiêu chí con.
                                                    </div>
                                                </div>
                                                <button class="btn btn-outline-primary btn-sm"
                                                    @click="addChildCriterion">
                                                    <i class="bi bi-plus-circle me-1"></i>Thêm tiêu chí con
                                                </button>
                                            </div>

                                            <div class="form-check mb-3">
                                                <input id="allChildrenRequired" v-model="form.batBuocDatTatCaTieuChiCon"
                                                    class="form-check-input" type="checkbox" />
                                                <label class="form-check-label" for="allChildrenRequired">Phải đạt tất
                                                    cả tiêu chí con thì chỉ tiêu mới đạt</label>
                                            </div>

                                            <div v-if="!form.tieuChiDanhGias.length" class="empty-inline">
                                                Chưa có tiêu chí con. Hãy thêm ít nhất 2 tiêu chí để mô tả chỉ tiêu phân
                                                rã.
                                            </div>

                                            <div v-for="(child, index) in form.tieuChiDanhGias" :key="child.localKey"
                                                class="criteria-card mb-3">
                                                <div
                                                    class="d-flex justify-content-between align-items-center gap-3 mb-3">
                                                    <div class="fw-semibold">Tiêu chí con {{ index + 1 }}</div>
                                                    <button class="btn btn-sm btn-outline-danger"
                                                        @click="removeChildCriterion(index)">
                                                        <i class="bi bi-trash me-1"></i>Xóa
                                                    </button>
                                                </div>

                                                <div class="row g-3">
                                                    <div class="col-12 col-md-3">
                                                        <label class="form-label">Thứ tự</label>
                                                        <input v-model.number="child.thuTuHienThi" type="number" min="1"
                                                            class="form-control" />
                                                    </div>

                                                    <div class="col-12 col-md-3">
                                                        <label class="form-label">Mã tiêu chí <span
                                                                class="text-danger">*</span></label>
                                                        <input v-model="child.maChiTieu" type="text"
                                                            class="form-control" />
                                                    </div>

                                                    <div class="col-12 col-md-6">
                                                        <label class="form-label">Tên tiêu chí <span
                                                                class="text-danger">*</span></label>
                                                        <input v-model="child.tenChiTieu" type="text"
                                                            class="form-control" />
                                                    </div>

                                                    <div class="col-12 col-md-4">
                                                        <label class="form-label">Loại tiêu chí <span
                                                                class="text-danger">*</span></label>
                                                        <select v-model="child.loaiChiTieu" class="form-select">
                                                            <option value="DINH_TINH">Định tính</option>
                                                            <option value="DINH_LUONG_TICH_LUY">Định lượng tích lũy
                                                            </option>
                                                            <option value="DINH_LUONG_SO_SANH">Định lượng so sánh
                                                            </option>
                                                        </select>
                                                    </div>

                                                    <div class="col-12 col-md-4">
                                                        <label class="form-label">Đơn vị tính</label>
                                                        <input v-model="child.donViTinh" type="text"
                                                            class="form-control"
                                                            placeholder="Để trống nếu dùng đơn vị tính cha" />
                                                    </div>

                                                    <div class="col-12 col-md-4">
                                                        <label class="form-label">Mô tả ngắn</label>
                                                        <input v-model="child.moTa" type="text" class="form-control" />
                                                    </div>

                                                    <div class="col-12">
                                                    <label class="form-label">Hướng dẫn tính toán</label>
                                                    <textarea v-model="child.huongDanTinhToan" rows="2"
                                                        class="form-control"></textarea>
                                                    </div>

                                                    <div v-if="child.loaiChiTieu === 'DINH_TINH'"
                                                        class="col-12 col-md-6">
                                                        <label class="form-label">Điều kiện hoàn thành</label>
                                                        <input v-model="child.dieuKienHoanThanh" type="text"
                                                            class="form-control" />
                                                    </div>
                                                    <div v-if="child.loaiChiTieu === 'DINH_TINH'"
                                                        class="col-12 col-md-6">
                                                        <label class="form-label">Điều kiện không hoàn thành</label>
                                                        <input v-model="child.dieuKienKhongHoanThanh" type="text"
                                                            class="form-control" />
                                                    </div>

                                                    <template v-if="child.loaiChiTieu === 'DINH_LUONG_SO_SANH'">
                                                        <div class="col-12 col-md-4">
                                                            <label class="form-label">Tỷ lệ % mục tiêu</label>
                                                            <input v-model.number="child.tyLePhanTramMucTieu"
                                                                type="number" min="0" step="any" class="form-control" />
                                                        </div>
                                                        <div class="col-12 col-md-4">
                                                            <label class="form-label">Loại mốc so sánh</label>
                                                            <select v-model="child.loaiMocSoSanh" class="form-select">
                                                                <option value="">Chon</option>
                                                                <option value="DAU_KY">Đầu kỳ</option>
                                                                <option value="CUNG_KY">Cùng kỳ</option>
                                                                <option value="KY_TRUOC">Kỳ trước</option>
                                                            </select>
                                                        </div>
                                                        <div class="col-12 col-md-4">
                                                            <label class="form-label">Chiều so sánh</label>
                                                            <select v-model="child.chieuSoSanh" class="form-select">
                                                                <option value="">Chon</option>
                                                                <option value="TANG">Tăng</option>
                                                                <option value="GIAM">Giảm</option>
                                                            </select>
                                                        </div>
                                                    </template>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="modal-footer border-0 pt-0">
                                <button class="btn btn-light" @click="closeModal">Hủy</button>
                                <button class="btn btn-primary" :disabled="saving" @click="handleSubmit">
                                    <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
                                    {{ saving ? 'Đang lưu...' : isEdit ? 'Cập nhật' : 'Lưu chỉ tiêu' }}
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
    import * as XLSX from 'xlsx'
    import BaseLayout from '../BaseLayout.vue'
    import { apiRequest } from '../../services/api.js'

    const DEFAULT_CAP_AP_DUNG = 'THANH_PHO'
    const CUSTOM_OPTION_VALUE = '__custom__'

    const loading = ref(false)
    const saving = ref(false)
    const importing = ref(false)
    const showModal = ref(false)
    const isEdit = ref(false)
    const editingId = ref(null)
    const items = ref([])
    const importFileInput = ref(null)
    const importResult = ref(null)

    const filters = reactive({
        keyword: '',
        nguonChiTieu: '',
        loaiChiTieu: '',
        trangThaiSuDung: ''
    })

    const createChildCriterion = (index = 1) => ({
        localKey: `${Date.now()}-${Math.random()}-${index}`,
        id: null,
        maChiTieu: '',
        tenChiTieu: '',
        loaiChiTieu: 'DINH_TINH',
        thuTuHienThi: index,
        donViTinh: '',
        moTa: '',
        huongDanTinhToan: '',
        dieuKienHoanThanh: '',
        dieuKienKhongHoanThanh: '',
        tyLePhanTramMucTieu: null,
        loaiMocSoSanh: '',
        chieuSoSanh: ''
    })

    const createEmptyImportRow = (rowNumber) => ({
        rowNumber,
        maChiTieu: '',
        tenChiTieu: '',
        nguonChiTieu: '',
        linhVucNghiepVu: '',
        donViTinh: '',
        moTa: '',
        huongDanTinhToan: '',
        trangThaiSuDung: 'DANG_AP_DUNG',
        ngayHieuLuc: '',
        ngayHetHieuLuc: '',
        cheDoDanhGia: 'DON',
        loaiChiTieu: 'DINH_TINH',
        dieuKienHoanThanh: '',
        dieuKienKhongHoanThanh: '',
        tyLePhanTramMucTieu: null,
        loaiMocSoSanh: '',
        chieuSoSanh: '',
        batBuocDatTatCaTieuChiCon: true,
        tieuChiDanhGias: [],
        errors: []
    })

    const createDefaultForm = () => ({
        maChiTieu: '',
        tenChiTieu: '',
        nguonChiTieu: 'BO',
        linhVucNghiepVu: '',
        donViTinh: '',
        moTa: '',
        huongDanTinhToan: '',
        trangThaiSuDung: 'DANG_AP_DUNG',
        ngayHieuLuc: '',
        ngayHetHieuLuc: '',
        cheDoDanhGia: 'DON',
        coChoPhepPhanRa: false,
        batBuocDatTatCaTieuChiCon: true,
        loaiChiTieu: 'DINH_TINH',
        dieuKienHoanThanh: '',
        dieuKienKhongHoanThanh: '',
        tyLePhanTramMucTieu: null,
        loaiMocSoSanh: '',
        chieuSoSanh: '',
        tieuChiDanhGias: []
    })

    const form = reactive(createDefaultForm())
    const linhVucNghiepVuMode = ref('existing')
    const donViTinhMode = ref('existing')

    const normalizeOptionValue = value => String(value || '').trim()

    const buildUniqueOptions = (values) => Array.from(
        new Set(values.map(normalizeOptionValue).filter(Boolean))
    ).sort((left, right) => left.localeCompare(right, 'vi'))

    const linhVucNghiepVuOptions = computed(() => buildUniqueOptions(
        items.value.map(item => item.linhVucNghiepVu)
    ))

    const donViTinhOptions = computed(() => buildUniqueOptions(
        items.value.flatMap(item => [
            item.donViTinh,
            ...item.tieuChiDanhGias.map(child => child.donViTinh)
        ])
    ))

    const linhVucNghiepVuSelector = computed(() => (
        linhVucNghiepVuMode.value === 'custom'
            ? CUSTOM_OPTION_VALUE
            : normalizeOptionValue(form.linhVucNghiepVu)
    ))

    const donViTinhSelector = computed(() => (
        donViTinhMode.value === 'custom'
            ? CUSTOM_OPTION_VALUE
            : normalizeOptionValue(form.donViTinh)
    ))

    const resetForm = () => {
        Object.assign(form, createDefaultForm())
        linhVucNghiepVuMode.value = 'existing'
        donViTinhMode.value = 'existing'
    }

    const syncSuggestionModes = () => {
        const linhVuc = normalizeOptionValue(form.linhVucNghiepVu)
        const donViTinh = normalizeOptionValue(form.donViTinh)

        linhVucNghiepVuMode.value = linhVuc && !linhVucNghiepVuOptions.value.includes(linhVuc) ? 'custom' : 'existing'
        donViTinhMode.value = donViTinh && !donViTinhOptions.value.includes(donViTinh) ? 'custom' : 'existing'
    }

    const handleLinhVucSelectorChange = (value) => {
        if (value === CUSTOM_OPTION_VALUE) {
            linhVucNghiepVuMode.value = 'custom'
            form.linhVucNghiepVu = ''
            return
        }

        linhVucNghiepVuMode.value = 'existing'
        form.linhVucNghiepVu = value
    }

    const handleDonViTinhSelectorChange = (value) => {
        if (value === CUSTOM_OPTION_VALUE) {
            donViTinhMode.value = 'custom'
            form.donViTinh = ''
            return
        }

        donViTinhMode.value = 'existing'
        form.donViTinh = value
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

    const normalizeChild = (child, index = 1) => ({
        localKey: `${pick(child, 'id', 'Id') || 'new'}-${index}-${Math.random()}`,
        id: Number(pick(child, 'id', 'Id')) || null,
        maChiTieu: String(pick(child, 'maChiTieu', 'MaChiTieu') || ''),
        tenChiTieu: String(pick(child, 'tenChiTieu', 'TenChiTieu') || ''),
        loaiChiTieu: String(pick(child, 'loaiChiTieu', 'LoaiChiTieu') || 'DINH_TINH'),
        thuTuHienThi: Number(pick(child, 'thuTuHienThi', 'ThuTuHienThi') || index),
        donViTinh: String(pick(child, 'donViTinh', 'DonViTinh') || ''),
        moTa: String(pick(child, 'moTa', 'MoTa') || ''),
        huongDanTinhToan: String(pick(child, 'huongDanTinhToan', 'HuongDanTinhToan') || ''),
        dieuKienHoanThanh: String(pick(child, 'dieuKienHoanThanh', 'DieuKienHoanThanh') || ''),
        dieuKienKhongHoanThanh: String(pick(child, 'dieuKienKhongHoanThanh', 'DieuKienKhongHoanThanh') || ''),
        tyLePhanTramMucTieu: pick(child, 'tyLePhanTramMucTieu', 'TyLePhanTramMucTieu'),
        loaiMocSoSanh: String(pick(child, 'loaiMocSoSanh', 'LoaiMocSoSanh') || ''),
        chieuSoSanh: String(pick(child, 'chieuSoSanh', 'ChieuSoSanh') || '')
    })

    const normalizeItem = (item) => {
        const children = normalizeList(pick(item, 'tieuChiDanhGias', 'TieuChiDanhGias')).map((child, index) => normalizeChild(child, index + 1))
        const loaiChiTieu = String(pick(item, 'loaiChiTieu', 'LoaiChiTieu') || '')
        const nguonChiTieu = String(pick(item, 'nguonChiTieu', 'NguonChiTieu') || '')

        return {
            id: Number(pick(item, 'id', 'Id') || 0),
            maChiTieu: String(pick(item, 'maChiTieu', 'MaChiTieu') || ''),
            tenChiTieu: String(pick(item, 'tenChiTieu', 'TenChiTieu') || ''),
            nguonChiTieu,
            nguonChiTieuLabel: nguonChiTieu === 'BO' ? 'Bộ Công an' : nguonChiTieu === 'THANH_PHO' ? 'Công an Thành phố' : nguonChiTieu,
            loaiChiTieu,
            capApDung: String(pick(item, 'capApDung', 'CapApDung') || DEFAULT_CAP_AP_DUNG),
            linhVucNghiepVu: String(pick(item, 'linhVucNghiepVu', 'LinhVucNghiepVu') || ''),
            donViTinh: String(pick(item, 'donViTinh', 'DonViTinh') || ''),
            moTa: String(pick(item, 'moTa', 'MoTa') || ''),
            huongDanTinhToan: String(pick(item, 'huongDanTinhToan', 'HuongDanTinhToan') || ''),
            coChoPhepPhanRa: Boolean(pick(item, 'coChoPhepPhanRa', 'CoChoPhepPhanRa')),
            trangThaiSuDung: String(pick(item, 'trangThaiSuDung', 'TrangThaiSuDung') || 'DANG_AP_DUNG'),
            ngayHieuLuc: pick(item, 'ngayHieuLuc', 'NgayHieuLuc'),
            ngayHetHieuLuc: pick(item, 'ngayHetHieuLuc', 'NgayHetHieuLuc'),
            dieuKienHoanThanh: String(pick(item, 'dieuKienHoanThanh', 'DieuKienHoanThanh') || ''),
            dieuKienKhongHoanThanh: String(pick(item, 'dieuKienKhongHoanThanh', 'DieuKienKhongHoanThanh') || ''),
            tyLePhanTramMucTieu: pick(item, 'tyLePhanTramMucTieu', 'TyLePhanTramMucTieu'),
            loaiMocSoSanh: String(pick(item, 'loaiMocSoSanh', 'LoaiMocSoSanh') || ''),
            chieuSoSanh: String(pick(item, 'chieuSoSanh', 'ChieuSoSanh') || ''),
            batBuocDatTatCaTieuChiCon: pick(item, 'batBuocDatTatCaTieuChiCon', 'BatBuocDatTatCaTieuChiCon') !== false,
            tieuChiDanhGias: children
        }
    }

    const mapLoai = (value) => {
        const map = {
            DINH_TINH: 'Định tính',
            DINH_LUONG_TICH_LUY: 'Định lượng tích lũy',
            DINH_LUONG_SO_SANH: 'Định lượng so sánh',
            PHAN_RA: 'Phân rã'
        }

        return map[value] || value || '-'
    }

    const getTrangThaiLabel = value => value === 'DANG_AP_DUNG' ? 'Đang áp dụng' : 'Ngừng áp dụng'

    const getCriteriaSummary = (item) => {
        if (item.tieuChiDanhGias.length > 0) {
            return `${item.tieuChiDanhGias.length} tiêu chí con`
        }

        return '1 tiêu chí'
    }

    const addChildCriterion = () => {
        form.tieuChiDanhGias.push(createChildCriterion(form.tieuChiDanhGias.length + 1))
    }

    const removeChildCriterion = (index) => {
        form.tieuChiDanhGias.splice(index, 1)
        form.tieuChiDanhGias.forEach((child, childIndex) => {
            if (!child.thuTuHienThi || Number(child.thuTuHienThi) <= 0) {
                child.thuTuHienThi = childIndex + 1
            }
        })
    }

    watch(() => form.cheDoDanhGia, (mode) => {
        form.coChoPhepPhanRa = mode === 'PHAN_RA'
        if (mode === 'PHAN_RA' && form.tieuChiDanhGias.length < 2) {
            while (form.tieuChiDanhGias.length < 2) {
                addChildCriterion()
            }
        }
    })

    const buildQueryString = (params) => {
        const searchParams = new URLSearchParams()
        Object.entries(params).forEach(([key, value]) => {
            if (value !== undefined && value !== null && value !== '') {
                searchParams.append(key, value)
            }
        })

        const query = searchParams.toString()
        return query ? `?${query}` : ''
    }

    const normalizeImportHeader = (value) =>
        String(value || '')
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .replace(/[^A-Za-z0-9]/g, '')
            .trim()
            .toUpperCase()

    const normalizeImportValue = (value) => String(value || '').trim().toUpperCase()

    const getImportCell = (source, keys) => {
        for (const key of keys) {
            const value = source.get(key)
            if (value !== undefined && value !== null && String(value).trim() !== '') {
                return String(value).trim()
            }
        }
        return ''
    }

    const getNumericImportCell = (source, keys) => {
        const rawValue = getImportCell(source, keys)
        if (!rawValue) return null
        const parsedValue = Number(rawValue)
        return Number.isFinite(parsedValue) ? parsedValue : null
    }

    const normalizeImportedNguon = (value) => {
        const normalized = normalizeImportHeader(value)
        if (!normalized) return ''
        if (['BO', 'BOCONGAN'].includes(normalized)) return 'BO'
        if (['THANHPHO', 'THANH_PHO', 'CONGANTHANHPHO', 'CATP'].includes(normalized)) return 'THANH_PHO'
        return ''
    }

    const normalizeImportedCheDo = (value) => {
        const normalized = normalizeImportHeader(value)
        if (!normalized) return 'DON'
        if (['PHANRA', 'PHAN_RA'].includes(normalized)) return 'PHAN_RA'
        return 'DON'
    }

    const normalizeImportedLoaiChiTieu = (value) => {
        const normalized = normalizeImportHeader(value)
        if (!normalized) return ''
        if (['DINHTINH', 'DINH_TINH'].includes(normalized)) return 'DINH_TINH'
        if (['DINHLUONGTICHLUY', 'DINH_LUONG_TICH_LUY'].includes(normalized)) return 'DINH_LUONG_TICH_LUY'
        if (['DINHLUONGSOSANH', 'DINH_LUONG_SO_SANH'].includes(normalized)) return 'DINH_LUONG_SO_SANH'
        if (['PHANRA', 'PHAN_RA'].includes(normalized)) return 'PHAN_RA'
        return ''
    }

    const normalizeImportedTrangThai = (value) => {
        const normalized = normalizeImportHeader(value)
        if (!normalized) return 'DANG_AP_DUNG'
        if (['NGUNGAPDUNG', 'NGUNG_AP_DUNG'].includes(normalized)) return 'NGUNG_AP_DUNG'
        return 'DANG_AP_DUNG'
    }

    const normalizeImportedBoolean = (value, defaultValue = false) => {
        if (value === undefined || value === null || String(value).trim() === '') return defaultValue
        const normalized = normalizeImportHeader(value)
        if (['TRUE', '1', 'CO', 'YES', 'X'].includes(normalized)) return true
        if (['FALSE', '0', 'KHONG', 'NO'].includes(normalized)) return false
        return defaultValue
    }

    const normalizeImportedDate = (value) => {
        if (value === undefined || value === null || value === '') return ''
        if (typeof value === 'number') {
            const parsed = XLSX.SSF.parse_date_code(value)
            if (!parsed) return ''
            return `${String(parsed.y).padStart(4, '0')}-${String(parsed.m).padStart(2, '0')}-${String(parsed.d).padStart(2, '0')}`
        }

        const date = new Date(value)
        if (Number.isNaN(date.getTime())) return ''
        return date.toISOString().slice(0, 10)
    }

    const normalizeImportedChild = (child, index = 1) => ({
        id: null,
        maChiTieu: String(child?.maChiTieu || child?.MaChiTieu || '').trim(),
        tenChiTieu: String(child?.tenChiTieu || child?.TenChiTieu || '').trim(),
        loaiChiTieu: normalizeImportedLoaiChiTieu(child?.loaiChiTieu || child?.LoaiChiTieu || 'DINH_TINH') || 'DINH_TINH',
        thuTuHienThi: Number(child?.thuTuHienThi || child?.ThuTuHienThi || index),
        donViTinh: String(child?.donViTinh || child?.DonViTinh || '').trim(),
        moTa: String(child?.moTa || child?.MoTa || '').trim(),
        huongDanTinhToan: String(child?.huongDanTinhToan || child?.HuongDanTinhToan || '').trim(),
        dieuKienHoanThanh: String(child?.dieuKienHoanThanh || child?.DieuKienHoanThanh || '').trim(),
        dieuKienKhongHoanThanh: String(child?.dieuKienKhongHoanThanh || child?.DieuKienKhongHoanThanh || '').trim(),
        tyLePhanTramMucTieu: child?.tyLePhanTramMucTieu ?? child?.TyLePhanTramMucTieu ?? null,
        loaiMocSoSanh: String(child?.loaiMocSoSanh || child?.LoaiMocSoSanh || '').trim(),
        chieuSoSanh: String(child?.chieuSoSanh || child?.ChieuSoSanh || '').trim()
    })

    const parseImportedChildren = (rawValue, rowNumber) => {
        if (!rawValue) return { children: [], errors: [] }

        try {
            const parsed = JSON.parse(rawValue)
            if (!Array.isArray(parsed)) {
                return { children: [], errors: [`Dòng ${rowNumber}: TieuChiDanhGiasJson phải là một mảng JSON.`] }
            }

            return {
                children: parsed.map((item, index) => normalizeImportedChild(item, index + 1)),
                errors: []
            }
        } catch (error) {
            return { children: [], errors: [`Dòng ${rowNumber}: TieuChiDanhGiasJson không phải JSON hợp lệ.`] }
        }
    }

    const mapImportRow = (row, rowNumber) => {
        const mapped = createEmptyImportRow(rowNumber)
        const source = new Map(Object.entries(row).map(([key, value]) => [normalizeImportHeader(key), value]))

        mapped.maChiTieu = getImportCell(source, ['MACHITIEU', 'MA_CHI_TIEU'])
        mapped.tenChiTieu = getImportCell(source, ['TENCHITIEU', 'TEN_CHI_TIEU'])
        mapped.nguonChiTieu = normalizeImportedNguon(getImportCell(source, ['NGUONCHITIEU', 'NGUON_CHI_TIEU']))
        mapped.linhVucNghiepVu = getImportCell(source, ['LINHVUCNGHIEPVU', 'LINH_VUC_NGHIEP_VU'])
        mapped.donViTinh = getImportCell(source, ['DONVITINH', 'DON_VI_TINH'])
        mapped.moTa = getImportCell(source, ['MOTA', 'MO_TA'])
        mapped.huongDanTinhToan = getImportCell(source, ['HUONGDANTINHTOAN', 'HUONG_DAN_TINH_TOAN'])
        mapped.trangThaiSuDung = normalizeImportedTrangThai(getImportCell(source, ['TRANGTHAISUDUNG', 'TRANG_THAI_SU_DUNG']))
        mapped.ngayHieuLuc = normalizeImportedDate(source.get('NGAYHIEULUC') ?? source.get('NGAY_HIEU_LUC'))
        mapped.ngayHetHieuLuc = normalizeImportedDate(source.get('NGAYHETHIEULUC') ?? source.get('NGAY_HET_HIEU_LUC'))
        mapped.cheDoDanhGia = normalizeImportedCheDo(getImportCell(source, ['CHEDODANHGIA', 'CHE_DO_DANH_GIA']))
        mapped.loaiChiTieu = normalizeImportedLoaiChiTieu(getImportCell(source, ['LOAICHITIEU', 'LOAI_CHI_TIEU'])) || 'DINH_TINH'
        mapped.dieuKienHoanThanh = getImportCell(source, ['DIEUKIENHOANTHANH', 'DIEU_KIEN_HOAN_THANH'])
        mapped.dieuKienKhongHoanThanh = getImportCell(source, ['DIEUKIENKHONGHOANTHANH', 'DIEU_KIEN_KHONG_HOAN_THANH'])
        mapped.tyLePhanTramMucTieu = getNumericImportCell(source, ['TYLEPHANTRAMMUCTIEU', 'TY_LE_PHAN_TRAM_MUC_TIEU'])
        mapped.loaiMocSoSanh = getImportCell(source, ['LOAIMOCSOSANH', 'LOAI_MOC_SO_SANH'])
        mapped.chieuSoSanh = getImportCell(source, ['CHIEUSOSANH', 'CHIEU_SO_SANH'])
        mapped.batBuocDatTatCaTieuChiCon = normalizeImportedBoolean(source.get('BATBUOCDATTATCATIEUCHICON') ?? source.get('BAT_BUOC_DAT_TAT_CA_TIEU_CHI_CON'), true)

        const parsedChildren = parseImportedChildren(
            getImportCell(source, ['TIEUCHIDANHGIASJSON', 'TIEU_CHI_DANH_GIAS_JSON']),
            rowNumber
        )
        mapped.tieuChiDanhGias = parsedChildren.children
        mapped.errors.push(...parsedChildren.errors)

        if (!mapped.maChiTieu) mapped.errors.push('Thiếu mã chỉ tiêu.')
        if (!mapped.tenChiTieu) mapped.errors.push('Thiếu tên chỉ tiêu.')
        if (!mapped.nguonChiTieu) mapped.errors.push('Nguồn chỉ tiêu không hợp lệ.')

        if (mapped.cheDoDanhGia === 'PHAN_RA') {
            if (mapped.tieuChiDanhGias.length < 2) {
                mapped.errors.push('Chỉ tiêu phân rã phải có ít nhất 2 tiêu chí con trong TieuChiDanhGiasJson.')
            }
        } else {
            if (!mapped.loaiChiTieu) mapped.errors.push('Loại chỉ tiêu không hợp lệ.')
            if (mapped.loaiChiTieu === 'DINH_TINH' && (!mapped.dieuKienHoanThanh || !mapped.dieuKienKhongHoanThanh)) {
                mapped.errors.push('Chỉ tiêu định tính phải có điều kiện hoàn thành và không hoàn thành.')
            }
            if (mapped.loaiChiTieu === 'DINH_LUONG_SO_SANH') {
                if (mapped.tyLePhanTramMucTieu === null) mapped.errors.push('Thiếu tỷ lệ % mục tiêu.')
                if (!mapped.loaiMocSoSanh || !mapped.chieuSoSanh) mapped.errors.push('Thiếu loại mốc so sánh hoặc chiều so sánh.')
            }
        }

        if (mapped.ngayHieuLuc && mapped.ngayHetHieuLuc && mapped.ngayHetHieuLuc < mapped.ngayHieuLuc) {
            mapped.errors.push('Ngày kết thúc hiệu lực phải lớn hơn hoặc bằng ngày bắt đầu.')
        }

        return mapped
    }

    const prepareImportRows = (rows) => {
        const duplicatedCodes = new Set()
        const seenCodes = new Set()

        const prepared = rows.map((row, index) => {
            const mapped = mapImportRow(row, index + 2)
            const normalizedCode = normalizeImportValue(mapped.maChiTieu)
            if (normalizedCode) {
                if (seenCodes.has(normalizedCode)) duplicatedCodes.add(normalizedCode)
                seenCodes.add(normalizedCode)
            }
            return mapped
        })

        prepared.forEach((item) => {
            if (duplicatedCodes.has(normalizeImportValue(item.maChiTieu))) {
                item.errors.push(`Mã chỉ tiêu "${item.maChiTieu}" bị trùng trong file import.`)
            }
        })

        return prepared
    }

    const buildImportPayload = (row) => ({
        maChiTieu: row.maChiTieu,
        tenChiTieu: row.tenChiTieu,
        nguonChiTieu: row.nguonChiTieu,
        loaiChiTieu: row.cheDoDanhGia === 'PHAN_RA' ? 'PHAN_RA' : row.loaiChiTieu,
        capApDung: DEFAULT_CAP_AP_DUNG,
        linhVucNghiepVu: row.linhVucNghiepVu || null,
        donViTinh: row.donViTinh || null,
        moTa: row.moTa || null,
        huongDanTinhToan: row.huongDanTinhToan || null,
        coChoPhepPhanRa: row.cheDoDanhGia === 'PHAN_RA',
        trangThaiSuDung: row.trangThaiSuDung || 'DANG_AP_DUNG',
        ngayHieuLuc: row.ngayHieuLuc || null,
        ngayHetHieuLuc: row.ngayHetHieuLuc || null,
        dieuKienHoanThanh: row.cheDoDanhGia === 'DON' && row.loaiChiTieu === 'DINH_TINH' ? row.dieuKienHoanThanh || null : null,
        dieuKienKhongHoanThanh: row.cheDoDanhGia === 'DON' && row.loaiChiTieu === 'DINH_TINH' ? row.dieuKienKhongHoanThanh || null : null,
        tyLePhanTramMucTieu: row.cheDoDanhGia === 'DON' && row.loaiChiTieu === 'DINH_LUONG_SO_SANH' ? Number(row.tyLePhanTramMucTieu) : null,
        loaiMocSoSanh: row.cheDoDanhGia === 'DON' && row.loaiChiTieu === 'DINH_LUONG_SO_SANH' ? row.loaiMocSoSanh || null : null,
        chieuSoSanh: row.cheDoDanhGia === 'DON' && row.loaiChiTieu === 'DINH_LUONG_SO_SANH' ? row.chieuSoSanh || null : null,
        batBuocDatTatCaTieuChiCon: row.cheDoDanhGia === 'PHAN_RA' ? !!row.batBuocDatTatCaTieuChiCon : false,
        tieuChiDanhGias: row.cheDoDanhGia === 'PHAN_RA'
            ? row.tieuChiDanhGias.map((criterion, index) => ({
                id: null,
                maChiTieu: criterion.maChiTieu,
                tenChiTieu: criterion.tenChiTieu,
                loaiChiTieu: criterion.loaiChiTieu,
                thuTuHienThi: Number(criterion.thuTuHienThi || index + 1),
                donViTinh: criterion.donViTinh || row.donViTinh || null,
                moTa: criterion.moTa || null,
                huongDanTinhToan: criterion.huongDanTinhToan || null,
                dieuKienHoanThanh: criterion.loaiChiTieu === 'DINH_TINH' ? criterion.dieuKienHoanThanh || null : null,
                dieuKienKhongHoanThanh: criterion.loaiChiTieu === 'DINH_TINH' ? criterion.dieuKienKhongHoanThanh || null : null,
                tyLePhanTramMucTieu: criterion.loaiChiTieu === 'DINH_LUONG_SO_SANH' && criterion.tyLePhanTramMucTieu !== null && criterion.tyLePhanTramMucTieu !== undefined && criterion.tyLePhanTramMucTieu !== ''
                    ? Number(criterion.tyLePhanTramMucTieu)
                    : null,
                loaiMocSoSanh: criterion.loaiChiTieu === 'DINH_LUONG_SO_SANH' ? criterion.loaiMocSoSanh || null : null,
                chieuSoSanh: criterion.loaiChiTieu === 'DINH_LUONG_SO_SANH' ? criterion.chieuSoSanh || null : null
            }))
            : []
    })

    const executeImportRows = async (rows) => {
        const summary = {
            fileName: '',
            totalRows: rows.length,
            createdCount: 0,
            skippedCount: 0,
            failedCount: 0,
            messages: []
        }

        const existingCodes = new Set(items.value.map(item => normalizeImportValue(item.maChiTieu)).filter(Boolean))

        for (const row of rows) {
            const normalizedCode = normalizeImportValue(row.maChiTieu)
            if (existingCodes.has(normalizedCode)) {
                summary.skippedCount += 1
                summary.messages.push({
                    type: 'warning',
                    text: `Dòng ${row.rowNumber}: mã chỉ tiêu "${row.maChiTieu}" đã tồn tại, bỏ qua.`
                })
                continue
            }

            try {
                await apiRequest('/danh-muc-chi-tieu', 'POST', buildImportPayload(row))
                existingCodes.add(normalizedCode)
                summary.createdCount += 1
                summary.messages.push({
                    type: 'success',
                    text: `Dòng ${row.rowNumber}: đã tạo chỉ tiêu "${row.tenChiTieu}".`
                })
            } catch (error) {
                summary.failedCount += 1
                summary.messages.push({
                    type: 'danger',
                    text: `Dòng ${row.rowNumber}: ${error?.message || 'tạo chỉ tiêu thất bại'}.`
                })
            }
        }

        return summary
    }

    const importDanhMucFromExcel = async (file) => {
        try {
            importing.value = true
            importResult.value = null

            const workbook = XLSX.read(await file.arrayBuffer(), { type: 'array' })
            const firstSheetName = workbook.SheetNames?.[0]
            if (!firstSheetName) {
                alert('Không tìm thấy sheet nào trong file Excel.')
                return
            }

            const rows = XLSX.utils.sheet_to_json(workbook.Sheets[firstSheetName], { defval: '' })
            if (!rows.length) {
                alert('File Excel không có dữ liệu để nhập.')
                return
            }

            const preparedRows = prepareImportRows(rows)
            const validRows = preparedRows.filter(item => item.errors.length === 0)
            const invalidRows = preparedRows.filter(item => item.errors.length > 0)

            const previewMessage = [
                `File: ${file.name}`,
                `Tổng dòng dữ liệu: ${rows.length}`,
                `Hợp lệ: ${validRows.length}`,
                `Lỗi khi đọc file: ${invalidRows.length}`,
                '',
                'Tiếp tục nhập các dòng hợp lệ vào hệ thống?'
            ].join('\n')

            if (!window.confirm(previewMessage)) {
                return
            }

            const summary = await executeImportRows(validRows)
            summary.messages.unshift(
                ...invalidRows.flatMap(item => item.errors.map(error => ({
                    type: 'danger',
                    text: `Dòng ${item.rowNumber}: ${error}`
                })))
            )
            summary.failedCount += invalidRows.length
            summary.totalRows = rows.length
            summary.fileName = file.name
            importResult.value = summary

            await fetchDanhMucChiTieu()
            alert(`Nhập Excel hoàn tất. Thành công: ${summary.createdCount}, bỏ qua: ${summary.skippedCount}, lỗi: ${summary.failedCount}.`)
        } catch (error) {
            console.error(error)
            alert(error.message || 'Nhập dữ liệu từ Excel thất bại.')
        } finally {
            importing.value = false
        }
    }

    const openImportFilePicker = () => {
        importFileInput.value?.click()
    }

    const handleImportFileChange = async (event) => {
        const file = event.target?.files?.[0]
        if (!file) return

        try {
            await importDanhMucFromExcel(file)
        } finally {
            event.target.value = ''
        }
    }

    const downloadImportTemplate = () => {
        const headers = [
            'MaChiTieu',
            'TenChiTieu',
            'NguonChiTieu',
            'CheDoDanhGia',
            'LoaiChiTieu',
            'TrangThaiSuDung',
            'LinhVucNghiepVu',
            'DonViTinh',
            'DieuKienHoanThanh',
            'DieuKienKhongHoanThanh',
            'TyLePhanTramMucTieu',
            'LoaiMocSoSanh',
            'ChieuSoSanh',
            'BatBuocDatTatCaTieuChiCon',
            'TieuChiDanhGiasJson'
        ]

        const sampleRows = [
            [
                'CT001',
                'Không để xảy ra vụ việc nghiêm trọng',
                'BO',
                'DON',
                'DINH_TINH',
                'DANG_AP_DUNG',
                'An ninh trật tự',
                '',
                'Không xảy ra',
                'Xảy ra',
                '',
                '',
                '',
                '',
                ''
            ],
            [
                'CT002',
                'Đánh giá tai nạn giao thông',
                'THANH_PHO',
                'PHAN_RA',
                '',
                'DANG_AP_DUNG',
                'Giao thông',
                'vụ',
                '',
                '',
                '',
                '',
                '',
                'true',
                '[{"maChiTieu":"CT002_1","tenChiTieu":"Giảm số vụ","loaiChiTieu":"DINH_LUONG_SO_SANH","thuTuHienThi":1,"donViTinh":"%","tyLePhanTramMucTieu":10,"loaiMocSoSanh":"CUNG_KY","chieuSoSanh":"GIAM"},{"maChiTieu":"CT002_2","tenChiTieu":"Giảm số người tử vong","loaiChiTieu":"DINH_LUONG_SO_SANH","thuTuHienThi":2,"donViTinh":"%","tyLePhanTramMucTieu":20,"loaiMocSoSanh":"CUNG_KY","chieuSoSanh":"GIAM"}]'
            ]
        ]

        const csvContent = [headers, ...sampleRows]
            .map(row => row.map(value => `"${String(value).replace(/"/g, '""')}"`).join(','))
            .join('\n')

        const blob = new Blob([`\uFEFF${csvContent}`], { type: 'text/csv;charset=utf-8;' })
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.setAttribute('download', 'mau-import-danh-muc-chi-tieu.csv')
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)
    }

    const fetchDanhMucChiTieu = async () => {
        try {
            loading.value = true
            const queryString = buildQueryString({
                keyword: filters.keyword || undefined,
                nguonChiTieu: filters.nguonChiTieu || undefined,
                loaiChiTieu: filters.loaiChiTieu || undefined,
                trangThaiSuDung: filters.trangThaiSuDung || undefined
            })

            const data = await apiRequest(`/danh-muc-chi-tieu${queryString}`)
            items.value = normalizeList(data).map(normalizeItem)
        } catch (error) {
            console.error(error)
            alert(error.message || 'Không tải được danh mục chỉ tiêu.')
            items.value = []
        } finally {
            loading.value = false
        }
    }

    const openCreateModal = () => {
        isEdit.value = false
        editingId.value = null
        resetForm()
        showModal.value = true
    }

    const openEditModal = (item) => {
        isEdit.value = true
        editingId.value = item.id

        Object.assign(form, {
            maChiTieu: item.maChiTieu,
            tenChiTieu: item.tenChiTieu,
            nguonChiTieu: item.nguonChiTieu,
            linhVucNghiepVu: item.linhVucNghiepVu,
            donViTinh: item.donViTinh,
            moTa: item.moTa,
            huongDanTinhToan: item.huongDanTinhToan,
            trangThaiSuDung: item.trangThaiSuDung,
            ngayHieuLuc: item.ngayHieuLuc ? String(item.ngayHieuLuc).slice(0, 10) : '',
            ngayHetHieuLuc: item.ngayHetHieuLuc ? String(item.ngayHetHieuLuc).slice(0, 10) : '',
            cheDoDanhGia: item.tieuChiDanhGias.length > 0 ? 'PHAN_RA' : 'DON',
            coChoPhepPhanRa: item.tieuChiDanhGias.length > 0,
            batBuocDatTatCaTieuChiCon: item.batBuocDatTatCaTieuChiCon,
            loaiChiTieu: item.tieuChiDanhGias.length > 0 ? 'DINH_TINH' : item.loaiChiTieu,
            dieuKienHoanThanh: item.dieuKienHoanThanh,
            dieuKienKhongHoanThanh: item.dieuKienKhongHoanThanh,
            tyLePhanTramMucTieu: item.tyLePhanTramMucTieu,
            loaiMocSoSanh: item.loaiMocSoSanh,
            chieuSoSanh: item.chieuSoSanh,
            tieuChiDanhGias: item.tieuChiDanhGias.map((child, index) => normalizeChild(child, index + 1))
        })

        syncSuggestionModes()
        showModal.value = true
    }

    const closeModal = () => {
        showModal.value = false
        resetForm()
    }

    const validateCriterion = (criterion, label) => {
        if (!criterion.maChiTieu?.trim()) {
            alert(`${label}: vui lòng nhập mã tiêu chí.`)
            return false
        }

        if (!criterion.tenChiTieu?.trim()) {
            alert(`${label}: vui lòng nhập tên tiêu chí.`)
            return false
        }

        if (criterion.loaiChiTieu === 'DINH_TINH') {
            if (!criterion.dieuKienHoanThanh?.trim() || !criterion.dieuKienKhongHoanThanh?.trim()) {
                alert(`${label}: chỉ tiêu định tính phải có điều kiện hoàn thành và không hoàn thành.`)
                return false
            }
        }

        if (criterion.loaiChiTieu === 'DINH_LUONG_SO_SANH') {
            if (criterion.tyLePhanTramMucTieu === null || criterion.tyLePhanTramMucTieu === undefined || criterion.tyLePhanTramMucTieu === '') {
                alert(`${label}: vui lòng nhập tỷ lệ % mục tiêu.`)
                return false
            }

            if (!criterion.loaiMocSoSanh || !criterion.chieuSoSanh) {
                alert(`${label}: vui lòng chọn loại mốc và chiều so sánh.`)
                return false
            }
        }

        return true
    }

    const validateForm = () => {
        if (!isEdit.value && !form.maChiTieu?.trim()) {
            alert('Vui lòng nhập mã chỉ tiêu.')
            return false
        }

        if (!form.tenChiTieu?.trim()) {
            alert('Vui lòng nhập tên chỉ tiêu.')
            return false
        }

        if (form.ngayHieuLuc && form.ngayHetHieuLuc && form.ngayHetHieuLuc < form.ngayHieuLuc) {
            alert('Ngày kết thúc hiệu lực phải lớn hơn hoặc bằng ngày bắt đầu.')
            return false
        }

        if (form.cheDoDanhGia === 'PHAN_RA') {
            if (form.tieuChiDanhGias.length < 2) {
                alert('Chỉ tiêu phân rã phải có ít nhất 2 tiêu chí con.')
                return false
            }

            for (let i = 0; i < form.tieuChiDanhGias.length; i += 1) {
                if (!validateCriterion(form.tieuChiDanhGias[i], `Tiêu chí con ${i + 1}`)) {
                    return false
                }
            }

            return true
        }

        return validateCriterion({
            maChiTieu: form.maChiTieu,
            tenChiTieu: form.tenChiTieu,
            loaiChiTieu: form.loaiChiTieu,
            dieuKienHoanThanh: form.dieuKienHoanThanh,
            dieuKienKhongHoanThanh: form.dieuKienKhongHoanThanh,
            tyLePhanTramMucTieu: form.tyLePhanTramMucTieu,
            loaiMocSoSanh: form.loaiMocSoSanh,
            chieuSoSanh: form.chieuSoSanh
        }, 'Chỉ tiêu')
    }

    const buildCriterionPayload = (criterion, fallbackDonViTinh, index) => ({
        id: criterion.id || null,
        maChiTieu: criterion.maChiTieu.trim(),
        tenChiTieu: criterion.tenChiTieu.trim(),
        loaiChiTieu: criterion.loaiChiTieu,
        thuTuHienThi: Number(criterion.thuTuHienThi || index + 1),
        donViTinh: (criterion.donViTinh || fallbackDonViTinh || '').trim() || null,
        moTa: criterion.moTa?.trim() || null,
        huongDanTinhToan: criterion.huongDanTinhToan?.trim() || null,
        dieuKienHoanThanh: criterion.loaiChiTieu === 'DINH_TINH' ? criterion.dieuKienHoanThanh?.trim() || null : null,
        dieuKienKhongHoanThanh: criterion.loaiChiTieu === 'DINH_TINH' ? criterion.dieuKienKhongHoanThanh?.trim() || null : null,
        tyLePhanTramMucTieu: criterion.loaiChiTieu === 'DINH_LUONG_SO_SANH' ? Number(criterion.tyLePhanTramMucTieu) : null,
        loaiMocSoSanh: criterion.loaiChiTieu === 'DINH_LUONG_SO_SANH' ? criterion.loaiMocSoSanh || null : null,
        chieuSoSanh: criterion.loaiChiTieu === 'DINH_LUONG_SO_SANH' ? criterion.chieuSoSanh || null : null
    })

    const buildPayload = () => {
        const payload = {
            maChiTieu: form.maChiTieu.trim(),
            tenChiTieu: form.tenChiTieu.trim(),
            nguonChiTieu: form.nguonChiTieu,
            loaiChiTieu: form.cheDoDanhGia === 'PHAN_RA' ? 'PHAN_RA' : form.loaiChiTieu,
            capApDung: DEFAULT_CAP_AP_DUNG,
            linhVucNghiepVu: form.linhVucNghiepVu?.trim() || null,
            donViTinh: form.donViTinh?.trim() || null,
            moTa: form.moTa?.trim() || null,
            huongDanTinhToan: form.huongDanTinhToan?.trim() || null,
            coChoPhepPhanRa: form.cheDoDanhGia === 'PHAN_RA',
            trangThaiSuDung: form.trangThaiSuDung,
            ngayHieuLuc: form.ngayHieuLuc || null,
            ngayHetHieuLuc: form.ngayHetHieuLuc || null,
            dieuKienHoanThanh: null,
            dieuKienKhongHoanThanh: null,
            tyLePhanTramMucTieu: null,
            loaiMocSoSanh: null,
            chieuSoSanh: null,
            batBuocDatTatCaTieuChiCon: form.cheDoDanhGia === 'PHAN_RA' ? !!form.batBuocDatTatCaTieuChiCon : false,
            tieuChiDanhGias: []
        }

        if (form.cheDoDanhGia === 'PHAN_RA') {
            payload.tieuChiDanhGias = form.tieuChiDanhGias.map((criterion, index) => buildCriterionPayload(criterion, form.donViTinh, index))
            return payload
        }

        payload.dieuKienHoanThanh = form.loaiChiTieu === 'DINH_TINH' ? form.dieuKienHoanThanh?.trim() || null : null
        payload.dieuKienKhongHoanThanh = form.loaiChiTieu === 'DINH_TINH' ? form.dieuKienKhongHoanThanh?.trim() || null : null
        payload.tyLePhanTramMucTieu = form.loaiChiTieu === 'DINH_LUONG_SO_SANH' ? Number(form.tyLePhanTramMucTieu) : null
        payload.loaiMocSoSanh = form.loaiChiTieu === 'DINH_LUONG_SO_SANH' ? form.loaiMocSoSanh || null : null
        payload.chieuSoSanh = form.loaiChiTieu === 'DINH_LUONG_SO_SANH' ? form.chieuSoSanh || null : null
        return payload
    }

    const handleSubmit = async () => {
        if (!validateForm()) return

        try {
            saving.value = true
            const payload = buildPayload()

            if (isEdit.value && editingId.value) {
                await apiRequest(`/danh-muc-chi-tieu/${editingId.value}`, 'PUT', payload)
            } else {
                await apiRequest('/danh-muc-chi-tieu', 'POST', payload)
            }

            closeModal()
            await fetchDanhMucChiTieu()
        } catch (error) {
            console.error(error)
            alert(error.message || 'Lưu chỉ tiêu thất bại.')
        } finally {
            saving.value = false
        }
    }

    const handleDelete = async (item) => {
        const ok = window.confirm(`Bạn có chắc muốn xóa chỉ tiêu "${item.tenChiTieu}" không?`)
        if (!ok) return

        try {
            await apiRequest(`/danh-muc-chi-tieu/${item.id}`, 'DELETE')
            await fetchDanhMucChiTieu()
        } catch (error) {
            console.error(error)
            alert(error.message || 'Xóa chỉ tiêu thất bại.')
        }
    }

    const resetFilters = async () => {
        filters.keyword = ''
        filters.nguonChiTieu = ''
        filters.loaiChiTieu = ''
        filters.trangThaiSuDung = ''
        await fetchDanhMucChiTieu()
    }

    onMounted(() => {
        fetchDanhMucChiTieu()
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
    .criteria-card {
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

    .empty-inline {
        padding: 16px;
        border-radius: 14px;
        background: #fff7ed;
        color: #9a3412;
    }

    .criterion-tags {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
    }

    .import-guide-head {
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        gap: 16px;
        margin-bottom: 14px;
    }

    .import-guide-columns {
        display: flex;
        flex-wrap: wrap;
        gap: 10px;
    }

    .guide-chip {
        display: inline-flex;
        align-items: center;
        padding: 8px 12px;
        border-radius: 999px;
        background: #eff6ff;
        border: 1px solid #bfdbfe;
        color: #1d4ed8;
        font-size: 0.875rem;
        font-weight: 600;
    }

    .import-result-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
        gap: 12px;
        margin-top: 12px;
    }

    .result-box {
        min-width: 140px;
        padding: 14px 16px;
        border-radius: 16px;
        border: 1px solid #dbe2ea;
        background: #fff;
        display: flex;
        flex-direction: column;
        gap: 6px;
    }

    .result-box span {
        font-size: 0.85rem;
        color: #64748b;
    }

    .result-box strong {
        font-size: 1.35rem;
        color: #1f2937;
    }

    .result-box.success {
        border-color: #bbf7d0;
        background: #f0fdf4;
    }

    .result-box.warning {
        border-color: #fde68a;
        background: #fffbeb;
    }

    .result-box.danger {
        border-color: #fecaca;
        background: #fef2f2;
    }

    .import-message-list {
        display: flex;
        flex-direction: column;
        gap: 10px;
        margin-top: 16px;
        max-height: 280px;
        overflow: auto;
    }

    .import-message-item {
        padding: 10px 12px;
        border-radius: 12px;
        font-size: 0.92rem;
    }

    .import-message-item.success {
        background: #f0fdf4;
        color: #166534;
    }

    .import-message-item.warning {
        background: #fffbeb;
        color: #92400e;
    }

    .import-message-item.danger {
        background: #fef2f2;
        color: #991b1b;
    }
</style>

