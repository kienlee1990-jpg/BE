import { createRouter, createWebHistory } from "vue-router"

import Login from "../components/login.vue"
import Dashboard from "../components/Dashboard.vue"
import BaoCaoChiTieuCongAnThanhPhoPage from "../components/views/BaoCaoChiTieuCongAnThanhPhoPage.vue"

import DanhMucChiTieuPage from "../components/views/DanhMucChiTieuPage.vue"
import DanhMucDonViPage from "../components/views/DanhMucDonViPage.vue"
import CanhBaoRuiRoPage from "../components/views/CanhBaoRuiRoPage.vue"
import BaoCaoTheoChiTieuPage from "../components/views/BaoCaoTheoChiTieuPage.vue"
import CauHinhDanhGiaChiTieuPage from "../components/views/CauHinhDanhGiaChiTieuPage.vue"
import BaoCaoTongHopPage from "../components/views/BaoCaoTongHopPage.vue"
import BaoCaoTheoDonViPage from "../components/views/BaoCaoTheoDonViPage.vue"

import DonViChuaCapNhatPage from "../components/views/DonViChuaCapNhatPage.vue"
import DotGiaoChiTieuPage from "../components/views/DotGiaoChiTieuPage.vue"
import GiaoChoCatpPage from "../components/views/GiaoChoCatpPage.vue"
import GiaoChoPhongPage from "../components/views/GiaoChoPhongPage.vue"
import GiaoChoCadpPhuongXaPage from "../components/views/GiaoChoCadpPhuongXaPage.vue"
import KyBaoCaoPage from "../components/views/KyBaoCaoPage.vue"
import NhapKetQuaPage from "../components/views/NhapKetQuaPage.vue"
import NhomThiDuaPage from "../components/views/NhomThiDuaPage.vue"
import ThietLapNhomThiDuaPage from "../components/views/ThietLapNhomThiDuaPage.vue"
import NhatKyHeThongPage from "../components/views/NhatKyHeThongPage.vue"
import Permissions from "../components/views/Permissions.vue"
import SoSanhDonViPage from "../components/views/SoSanhDonViPage.vue"
import TienDoThucHienPage from "../components/views/TienDoThucHienPage.vue"
import TongHopDanhGiaLuyKePage from "../components/views/TongHopDanhGiaLuyKePage.vue"
import Users from "../components/views/Users.vue"
import XepHangDonViPage from "../components/views/XepHangDonViPage.vue"
import XuatBaoCaoPage from "../components/views/XuatBaoCaoPage.vue"

const routes = [
    {
        path: "/login",
        name: "Đăng nhập",
        component: Login
    },

    {
        path: "/dashboard",
        name: "Dashboard",
        component: Dashboard,
        meta: { requiresAuth: true }
    },
    {
        path: "/bao-cao-chi-tieu-cong-an-thanh-pho",
        name: "BaoCaoChiTieuCongAnThanhPho",
        component: BaoCaoChiTieuCongAnThanhPhoPage,
        meta: { requiresAuth: true }
    },



    // Quản lý chỉ tiêu
    {
        path: "/danh-muc-chi-tieu",
        name: "DanhMucChiTieu",
        component: DanhMucChiTieuPage,
        meta: { requiresAuth: true }
    },

    {
        path: "/giao-cho-catp",
        alias: "/chi-tieu-bo-giao",
        name: "GiaoChoCATP",
        component: GiaoChoCatpPage,
        meta: { requiresAuth: true }
    },

    {
        path: "/giao-cho-phong",
        alias: "/chi-tieu-thanh-pho-giao",
        name: "GiaoChoPhong",
        component: GiaoChoPhongPage,
        meta: { requiresAuth: true }
    },

    {
        path: "/don-vi",
        name: "DonVi",
        component: DanhMucDonViPage,
        meta: { requiresAuth: true }
    },


    {
        path: "/ky-bao-cao",
        name: "KyBaoCao",
        component: KyBaoCaoPage,
        meta: { requiresAuth: true }
    },

    // Giao chỉ tiêu
    {
        path: "/dot-giao-chi-tieu",
        name: "DotGiaoChiTieu",
        component: DotGiaoChiTieuPage,
        meta: { requiresAuth: true }
    },
    {
        path: "/cau-hinh-danh-gia-chi-tieu",
        name: "CauHinhDanhGiaChiTieu",
        component: CauHinhDanhGiaChiTieuPage,
        meta: { requiresAuth: true }
    },
    {
        path: "/giao-cho-cadp-phuong-xa",
        alias: "/giao-chi-tieu-don-vi",
        name: "GiaoChoCadpPhuongXa",
        component: GiaoChoCadpPhuongXaPage,
        meta: { requiresAuth: true }
    },

    // Theo dõi và giám sát
    {
        path: "/nhap-ket-qua",
        name: "NhapKetQua",
        component: NhapKetQuaPage,
        meta: { requiresAuth: true }
    },
    {
        path: "/tien-do-thuc-hien",
        name: "TienDoThucHien",
        component: TienDoThucHienPage,
        meta: { requiresAuth: true }
    },
    {
        path: "/don-vi-chua-cap-nhat",
        name: "DonViChuaCapNhat",
        component: DonViChuaCapNhatPage,
        meta: { requiresAuth: true }
    },
    {
        path: "/so-sanh-don-vi",
        name: "SoSanhDonVi",
        component: SoSanhDonViPage,
        meta: { requiresAuth: true }
    },

    // Đánh giá và báo cáo
    {
        path: "/tong-hop-danh-gia-luy-ke",
        alias: "/tong-hop-danh-gia",
        name: "TongHopDanhGia",
        component: TongHopDanhGiaLuyKePage,
        meta: { requiresAuth: true }
    },
    {
        path: "/xep-hang-don-vi",
        name: "XepHangDonVi",
        component: XepHangDonViPage,
        meta: { requiresAuth: true }
    },
    {
        path: "/thiet-lap-nhom-thi-dua",
        name: "ThietLapNhomThiDua",
        component: ThietLapNhomThiDuaPage,
        meta: { requiresAuth: true }
    },
    {
        path: "/nhom-thi-dua",
        name: "NhomThiDua",
        component: NhomThiDuaPage,
        meta: { requiresAuth: true }
    },
    {
        path: "/bao-cao-tong-hop",
        name: "BaoCaoTongHop",
        component: BaoCaoTongHopPage,
        meta: { requiresAuth: true }
    },
    {
        path: "/bao-cao-theo-don-vi",
        name: "BaoCaoTheoDonVi",
        component: BaoCaoTheoDonViPage,
        meta: { requiresAuth: true }
    },

    {
        path: "/xuat-bao-cao",
        name: "XuatBaoCao",
        component: XuatBaoCaoPage,
        meta: { requiresAuth: true }
    },
    {
        path: "/canh-bao-rui-ro",
        name: "CanhBaoRuiRo",
        component: CanhBaoRuiRoPage,
        meta: { requiresAuth: true }
    },
    {
        path: "/bao-cao-theo-chi-tieu",
        name: "BaoCaoTheoChiTieu",
        component: BaoCaoTheoChiTieuPage,
        meta: { requiresAuth: true }
    },

    // Quản trị
    {
        path: "/users",
        name: "Users",
        component: Users,
        meta: { requiresAuth: true }
    },
    {
        path: "/permissions",
        name: "Permissions",
        component: Permissions,
        meta: { requiresAuth: true }
    },

    {
        path: "/nhat-ky-he-thong",
        name: "NhatKyHeThong",
        component: NhatKyHeThongPage,
        meta: { requiresAuth: true }
    },

    {
        path: "/",
        redirect: "/login"
    },
    {
        path: "/:pathMatch(.*)*",
        redirect: "/login"
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

router.beforeEach((to) => {
    const token = localStorage.getItem("token")

    if (to.meta.requiresAuth && !token) {
        return { path: "/login" }
    }

    if (to.path === "/login" && token) {
        return { path: "/dashboard" }
    }

    return true
})

export default router


