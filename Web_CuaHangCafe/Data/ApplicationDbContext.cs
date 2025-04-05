using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Models;

namespace Web_CuaHangCafe.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbChiTietHoaDonBan> TbChiTietHoaDonBans { get; set; }

    public virtual DbSet<TbGioHang> TbGioHangs { get; set; }

    public virtual DbSet<TbHoaDonBan> TbHoaDonBans { get; set; }

    public virtual DbSet<TbKhachHang> TbKhachHangs { get; set; }

    public virtual DbSet<TbNguyenLieu> TbNguyenLieus { get; set; }

    public virtual DbSet<TbNhaCungCap> TbNhaCungCaps { get; set; }

    public virtual DbSet<TbNhanVien> TbNhanViens { get; set; }

    public virtual DbSet<TbNhomSanPham> TbNhomSanPhams { get; set; }

    public virtual DbSet<TbPhieuNhapChiTiet> TbPhieuNhapChiTiets { get; set; }

    public virtual DbSet<TbPhieuNhapHang> TbPhieuNhapHangs { get; set; }

    public virtual DbSet<TbQuanCafe> TbQuanCafes { get; set; }

    public virtual DbSet<TbQuanTriVien> TbQuanTriViens { get; set; }

    public virtual DbSet<TbQuyen> TbQuyens { get; set; }

    public virtual DbSet<TbSanPham> TbSanPhams { get; set; }

    public virtual DbSet<TbTaiKhoan> TbTaiKhoans { get; set; }

    public virtual DbSet<TbTaiKhoanKh> TbTaiKhoanKhs { get; set; }

    public virtual DbSet<TbTinTuc> TbTinTucs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DucAnh\\SQLEXPRESS;Initial Catalog=TheSpaceCoffee2;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbChiTietHoaDonBan>(entity =>
        {
            entity.HasKey(e => new { e.MaHoaDon, e.MaSanPham }).HasName("PK__tbChiTie__4CF2A579240107DD");

            entity.Property(e => e.ThanhTien).HasComputedColumnSql("([SoLuong]*[DonGia])", true);

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.TbChiTietHoaDonBans).HasConstraintName("FK__tbChiTiet__MaHoa__10566F31");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.TbChiTietHoaDonBans).HasConstraintName("FK__tbChiTiet__MaSan__114A936A");
        });

        modelBuilder.Entity<TbGioHang>(entity =>
        {
            entity.HasKey(e => new { e.MaKhachHang, e.MaSanPham }).HasName("PK__tbGioHan__477E84A7AFDFA7C8");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.TbGioHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbGioHang__MaKha__68487DD7");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.TbGioHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbGioHang__MaSan__693CA210");
        });

        modelBuilder.Entity<TbHoaDonBan>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__tbHoaDon__835ED13BA36D2778");

            entity.Property(e => e.MaHoaDon).HasDefaultValueSql("(newid())");
            entity.Property(e => e.NgayLap).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.TbHoaDonBans).HasConstraintName("FK__tbHoaDonB__MaKha__0A9D95DB");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.TbHoaDonBans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbHoaDonB__MaNha__09A971A2");

            entity.HasOne(d => d.MaQuanNavigation).WithMany(p => p.TbHoaDonBans).HasConstraintName("FK__tbHoaDonB__MaQua__0B91BA14");
        });

        modelBuilder.Entity<TbKhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__tbKhachH__88D2F0E5BD8515BA");
        });

        modelBuilder.Entity<TbNguyenLieu>(entity =>
        {
            entity.HasKey(e => e.MaNguyenLieu).HasName("PK__tbNguyen__C7519355B17A6827");
        });

        modelBuilder.Entity<TbNhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNhaCungCap).HasName("PK__tbNhaCun__53DA920556E95B15");
        });

        modelBuilder.Entity<TbNhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__tbNhanVi__77B2CA47C2E313F3");

            entity.Property(e => e.HeSoLuong).HasDefaultValue(1m);

            entity.HasOne(d => d.MaQuanNavigation).WithMany(p => p.TbNhanViens).HasConstraintName("FK__tbNhanVie__MaQua__619B8048");
        });

        modelBuilder.Entity<TbNhomSanPham>(entity =>
        {
            entity.HasKey(e => e.MaNhomSp).HasName("PK__tbNhomSa__5A1AD2F9F3C637B9");
        });

        modelBuilder.Entity<TbPhieuNhapChiTiet>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieuNhap, e.MaNguyenLieu }).HasName("PK__tbPhieuN__5805F60ED905FB15");

            entity.Property(e => e.ThanhTien).HasComputedColumnSql("([SoLuong]*[DonGia])", true);

            entity.HasOne(d => d.MaNguyenLieuNavigation).WithMany(p => p.TbPhieuNhapChiTiets).HasConstraintName("FK__tbPhieuNh__MaNgu__02084FDA");

            entity.HasOne(d => d.MaPhieuNhapNavigation).WithMany(p => p.TbPhieuNhapChiTiets).HasConstraintName("FK__tbPhieuNh__MaPhi__01142BA1");
        });

        modelBuilder.Entity<TbPhieuNhapHang>(entity =>
        {
            entity.HasKey(e => e.MaPhieuNhap).HasName("PK__tbPhieuN__1470EF3B969EE937");

            entity.Property(e => e.MaPhieuNhap).HasDefaultValueSql("(newid())");
            entity.Property(e => e.NgayLap).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaNhaCungCapNavigation).WithMany(p => p.TbPhieuNhapHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbPhieuNh__MaNha__7B5B524B");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.TbPhieuNhapHangs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tbPhieuNh__MaNha__7A672E12");

            entity.HasOne(d => d.MaQuanNavigation).WithMany(p => p.TbPhieuNhapHangs).HasConstraintName("FK__tbPhieuNh__MaQua__7C4F7684");
        });

        modelBuilder.Entity<TbQuanCafe>(entity =>
        {
            entity.HasKey(e => e.MaQuan).HasName("PK__tbQuanCa__60AB417D75866867");
        });

        modelBuilder.Entity<TbQuanTriVien>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbQuanTr__3214EC0743EDC130");
        });

        modelBuilder.Entity<TbQuyen>(entity =>
        {
            entity.HasKey(e => e.MaQuyen).HasName("PK__tbQuyen__1D4B7ED444101442");
        });

        modelBuilder.Entity<TbSanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__tbSanPha__FAC7442DDBD14475");

            entity.HasOne(d => d.MaNhomSpNavigation).WithMany(p => p.TbSanPhams).HasConstraintName("FK__tbSanPham__MaNho__5629CD9C");
        });

        modelBuilder.Entity<TbTaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__tbTaiKho__AD7C652927DEA365");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.TbTaiKhoans).HasConstraintName("FK__tbTaiKhoa__MaNha__6FE99F9F");

            entity.HasOne(d => d.MaQuyenNavigation).WithMany(p => p.TbTaiKhoans).HasConstraintName("FK__tbTaiKhoa__MaQuy__70DDC3D8");
        });

        modelBuilder.Entity<TbTaiKhoanKh>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__tbTaiKho__AD7C65293457EF74");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.TbTaiKhoanKhs).HasConstraintName("FK__tbTaiKhoa__MaKha__74AE54BC");

            entity.HasOne(d => d.MaQuyenNavigation).WithMany(p => p.TbTaiKhoanKhs).HasConstraintName("FK__tbTaiKhoa__MaQuy__75A278F5");
        });

        modelBuilder.Entity<TbTinTuc>(entity =>
        {
            entity.HasKey(e => e.MaTinTuc).HasName("PK__tbTinTuc__B53648C0C1272344");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
