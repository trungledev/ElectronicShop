namespace ElectronicShop.Models;

public class MessageViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    public int Success { get; set; }  = 0; //1 true,-1 false, 0 not show
    public string? Message { get; set; }
    public bool IsSigned { get; set; } = true;
    public int ProductId { get; set; }
    public bool IsExistReview { get; set; }
    public bool DeleteReview { get; set; }
    public string? ReturnUrl {get;set;}
}
