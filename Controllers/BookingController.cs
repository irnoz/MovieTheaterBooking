// Controllers/BookingController.cs
using Microsoft.AspNetCore.Mvc;
using MovieTheaterBooking.Models;

namespace MovieTheaterBooking.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingManager _bookingManager;

        public BookingController(IBookingManager bookingManager)
        {
            _bookingManager = bookingManager;
        }

        public IActionResult Index()
        {
            var model = new BookingViewModel
            {
                Board = _bookingManager.GetBoard(),
                CurrentBookings = _bookingManager.GetCurrentBookings()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Init(int rowsCount, int columnsCount)
        {
            _bookingManager.Init(rowsCount, columnsCount);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateBooking(int seatsCount, string bookingType)
        {
            if (!Enum.TryParse<BookingRequestType>(bookingType, out var type))
            {
                TempData["Error"] = "Invalid booking type.";
                return RedirectToAction("Index");
            }

            var result = _bookingManager.CreateBooking(seatsCount, type);
            if (result.IsSuccess)
            {
                TempData["Success"] = $"Booking successful! Your booking number is {result.Value}.";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CancelBooking(string bookingNumber)
        {
            var result = _bookingManager.DeleteBooking(bookingNumber);
            if (result.IsSuccess)
            {
                TempData["Success"] = $"Booking {bookingNumber} canceled successfully.";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToAction("Index");
        }
    }
    
    public class BookingViewModel
    {
        public List<string> Board { get; set; } = new List<string>();
        public List<string> CurrentBookings { get; set; } = new List<string>();
    }
}
