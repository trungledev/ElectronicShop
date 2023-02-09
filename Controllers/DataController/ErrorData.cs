namespace ElectronicShop.Controllers.DataController;

public class ErrorData
{
    public string? Message { get; set; } = null!;
    public bool Sucess { get; set; } = true;
    public ErrorData(string? message, bool success)
    {
        this.Message = message;
        this.Sucess = success;
    }
}