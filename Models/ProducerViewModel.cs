namespace ElectronicShop.Models;

public class ProducerViewModel : LookupViewModel
{
    public string Address { get; set; } = string.Empty;
    [RegularExpression("^0(3[2-9]|5(6|8|9)|7(0|[6-9])|8[1-9]|9([0-4]|[6-9]))[0-9]{7}$", ErrorMessage ="So Dien Thoai khong phu hop")]
    public string PhoneNumber { get; set; } = string.Empty;
}