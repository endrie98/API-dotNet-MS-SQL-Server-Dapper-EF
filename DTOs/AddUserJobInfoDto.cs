namespace DotnetAPI.DTOs
{
    public class AddUserJobInfoDto
    {
        public int UserId { get; set; }
        public string? JobTitle { get; set; }
        public string? Department { get; set; }
    }
}