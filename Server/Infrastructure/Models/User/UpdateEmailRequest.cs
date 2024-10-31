namespace Models.User
{
    public class UpdateEmailRequest
    {
        public string NewEmail { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
    }
}