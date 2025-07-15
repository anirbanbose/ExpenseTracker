
namespace ExpenseTracker.Application.DTO.Email;

public class EmailJobDTO
{
    public string Email { get; set; } = default!;
    public string Subject { get; set; } = default!;
    public string Body { get; set; } = default!;
}
