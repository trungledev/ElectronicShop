namespace ElectronicShop.Models.MessageViewModels;

public class HomeMessageViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    
}
