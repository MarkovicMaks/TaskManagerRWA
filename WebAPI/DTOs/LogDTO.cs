namespace WebAPI.DTOs
{
    public class LogDTO
    {
        
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        
    }
}
