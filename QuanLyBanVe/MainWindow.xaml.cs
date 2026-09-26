using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyBanVe
{
    public partial class MainWindow : Window
    {
        QuanLyBanVeEntities1 db = new QuanLyBanVeEntities1();

        public MainWindow()
        {
            InitializeComponent();
            TaiDuLieu();
        }

        private void TaiDuLieu()
        {
            cbo_ChuyenXe.ItemsSource = db.CHUYENXEs.Select(c => new
            {
                MaChuyen = c.MaChuyen,
                ThongTinHienThi = c.TuyenDuong + " (" + c.GioXuatBen + ")"
            }).ToList();

            cbo_ChuyenXe.DisplayMemberPath = "ThongTinHienThi"; // Hiển thị chuỗi vừa ghép
            cbo_ChuyenXe.SelectedValuePath = "MaChuyen";        // Vẫn giữ lại mã để lưu CSDL
            dtg_DanhSachVe.ItemsSource = db.VEXEs.Select(v => new
            {
                MaVe = v.MaVe,
                TenKhach = v.TenKhach,
                SDT = v.SDT,
                NgayDi = v.NgayDi,
                YeuCauTrungChuyen = v.YeuCauTrungChuyen,
                TuyenDuong = v.CHUYENXE.TuyenDuong,
                GioXuatBen = v.CHUYENXE.GioXuatBen,
                MaChuyen = v.MaChuyen
            }).ToList();
        }

        private void dtg_DanhSachVe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtg_DanhSachVe.SelectedItem != null)
            {
                dynamic row = dtg_DanhSachVe.SelectedItem;

                txt_MaVe.Text = row.MaVe;
                txt_TenKhach.Text = row.TenKhach;
                txt_SDT.Text = row.SDT;
                dp_NgayDi.SelectedDate = row.NgayDi;
                txt_TrungChuyen.Text = row.YeuCauTrungChuyen;
                cbo_ChuyenXe.SelectedValue = row.MaChuyen;
            }
        }

        private void btn_Them_Click(object sender, RoutedEventArgs e)
        {
            VEXE veMoi = new VEXE();

            veMoi.MaVe = txt_MaVe.Text;
            veMoi.TenKhach = txt_TenKhach.Text;
            veMoi.SDT = txt_SDT.Text;
            veMoi.NgayDi = dp_NgayDi.SelectedDate; 
            veMoi.YeuCauTrungChuyen = txt_TrungChuyen.Text;
            veMoi.MaChuyen = cbo_ChuyenXe.SelectedValue.ToString();

            db.VEXEs.Add(veMoi);
            db.SaveChanges();

            TaiDuLieu();
            MessageBox.Show("Thêm thành công");
        }

        private void btn_Sua_Click(object sender, RoutedEventArgs e)
        {
            string ma = txt_MaVe.Text;
            VEXE veSua = db.VEXEs.FirstOrDefault(v => v.MaVe == ma);

            if (veSua != null)
            {
                veSua.TenKhach = txt_TenKhach.Text;
                veSua.SDT = txt_SDT.Text;
                veSua.NgayDi = dp_NgayDi.SelectedDate;
                veSua.YeuCauTrungChuyen = txt_TrungChuyen.Text;
                veSua.MaChuyen = cbo_ChuyenXe.SelectedValue.ToString();

                db.SaveChanges();

                TaiDuLieu();
                MessageBox.Show("Sửa thành công");
            }
        }

        private void btn_Xoa_Click(object sender, RoutedEventArgs e)
        {
            string ma = txt_MaVe.Text;
            VEXE veXoa = db.VEXEs.FirstOrDefault(v => v.MaVe == ma);

            if (veXoa != null)
            {
                db.VEXEs.Remove(veXoa);
                db.SaveChanges();

                TaiDuLieu();
                MessageBox.Show("Xóa thành công");
            }
        }

        private void btn_LamMoi_Click(object sender, RoutedEventArgs e)
        {
            txt_MaVe.Text = "";
            txt_TenKhach.Text = "";
            txt_SDT.Text = "";
            dp_NgayDi.SelectedDate = null;
            txt_TrungChuyen.Text = "";
            cbo_ChuyenXe.SelectedIndex = -1;
        }

        private void btn_TimKiem_Click(object sender, RoutedEventArgs e)
        {
            string sdtTimKiem = txt_SDT.Text.Trim();

            // neu o sdt trong an tim kiem se hien thi toan bo danh sach
            if (string.IsNullOrEmpty(sdtTimKiem))
            {
                TaiDuLieu();
                return;
            }

            // tim thong tin qua sdt
            var ketQuaTimKiem = db.VEXEs.Where(v => v.SDT.Contains(sdtTimKiem)).Select(v => new
            {
                MaVe = v.MaVe,
                TenKhach = v.TenKhach,
                SDT = v.SDT,
                NgayDi = v.NgayDi,
                YeuCauTrungChuyen = v.YeuCauTrungChuyen,
                TuyenDuong = v.CHUYENXE.TuyenDuong,
                GioXuatBen = v.CHUYENXE.GioXuatBen,
                MaChuyen = v.MaChuyen
            }).ToList();

            // hien thi ket qua len dg
            dtg_DanhSachVe.ItemsSource = ketQuaTimKiem;

            // thong bao neu ko co du lieu khop
            if (ketQuaTimKiem.Count == 0)
            {
                MessageBox.Show("Ko tìm thấy vé nào với số điện thoại này", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}