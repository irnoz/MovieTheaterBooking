// Models/IBookingManager.cs
using System.Collections.Generic;

namespace MovieTheaterBooking.Models
{
    public interface IBookingManager
    {
        void Init(int rowsCount, int columnsCount);
        Result CreateBooking(int k, BookingRequestType type);
        Result DeleteBooking(string bookingNumber);
        List<string> GetBoard();
        List<string> GetCurrentBookings();
    }
}