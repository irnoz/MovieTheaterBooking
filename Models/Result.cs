// Models/Result.cs
namespace MovieTheaterBooking.Models
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Value { get; set; }
    }
}