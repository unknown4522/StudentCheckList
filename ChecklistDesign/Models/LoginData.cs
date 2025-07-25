namespace ChecklistDesign.Models
{
    public class LoginData
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
        public string? Profilepic { get; set; }

        public string? Message { get; set; }
    }
    public class UploadResponse
    {
        public string Message { get; set; } = "";
        public string Profilepic { get; set; } = "";
    }
}
