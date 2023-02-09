namespace ElectronicShop.Models;

public class ReviewViewModel
{
    public int SanPhamId { get; set; }
    [Display(Name ="Tiêu đề")]
    public string? TieuDe { get; set; }
    [Display(Name ="Nội dung")]
    public string? NoiDung { get; set; }
    public double SoSaoTrungBinh { get; set; }
    [Display(Name ="Số sao")]
    public double SoSao { get; set; }
    public IDictionary<string,int>? SoSaoCuThe {get; set; }
    public int SoLuot { get; set; }
    public DateTime NgayDang { get; set; }
    public string? TacGia { get; set; }
}