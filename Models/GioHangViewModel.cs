namespace ElectronicShop.Models;

public class GioHangViewModel
{
    public int Id { get; set; }
    [Display(Name = "Tên sản phẩm")]
    public string? TenSanPham { get; set; }

    public string? ThongTin { get; set; }
    [Display(Name = "Số lượng")]
    public int SoLuong { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}")]
    public double DonGia { get; set; }
    [DisplayFormat(DataFormatString = "{0:N0}")]
    [Display(Name = "Thành tiền")]
    public double ThanhTien { get; set; }
}