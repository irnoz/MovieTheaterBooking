// Models/BookingManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieTheaterBooking.Models
{
    public class BookingManager : IBookingManager
    {
        private int _rows;
        private int _columns;
        private char[,] _seats;
        private Dictionary<string, List<(int Row, int Col)>> _bookings;
        private Random _random;

        public BookingManager()
        {
            _bookings = new Dictionary<string, List<(int Row, int Col)>>();
            _random = new Random();
        }

        public void Init(int rowsCount, int columnsCount)
        {
            _rows = rowsCount;
            _columns = columnsCount;
            _seats = new char[_rows, _columns];

            for (int i = 0; i < _rows; i++)
                for (int j = 0; j < _columns; j++)
                    _seats[i, j] = 'O';

            _bookings.Clear();
        }

        public Result CreateBooking(int k, BookingRequestType type)
        {
            if (k <= 0)
                return new Result { IsSuccess = false, ErrorMessage = "Number of seats must be positive." };

            List<(int Row, int Col)> allocatedSeats = new List<(int Row, int Col)>();

            if (type == BookingRequestType.Gather)
            {
                for (int i = 0; i < _rows; i++)
                {
                    int consecutive = 0;
                    List<(int Row, int Col)> tempSeats = new List<(int Row, int Col)>();

                    for (int j = 0; j < _columns; j++)
                    {
                        if (_seats[i, j] == 'O')
                        {
                            consecutive++;
                            tempSeats.Add((i, j));

                            if (consecutive == k)
                            {
                                allocatedSeats = new List<(int Row, int Col)>(tempSeats);
                                break;
                            }
                        }
                        else
                        {
                            consecutive = 0;
                            tempSeats.Clear();
                        }
                    }

                    if (allocatedSeats.Any())
                        break;
                }

                if (!allocatedSeats.Any())
                    return new Result { IsSuccess = false, ErrorMessage = "Not enough contiguous seats available for Gather booking." };
            }
            else 
            {
                var availableSeats = new List<(int Row, int Col)>();

                for (int i = 0; i < _rows; i++)
                    for (int j = 0; j < _columns; j++)
                        if (_seats[i, j] == 'O')
                            availableSeats.Add((i, j));

                if (availableSeats.Count < k)
                    return new Result { IsSuccess = false, ErrorMessage = "Not enough seats available for Scatter booking." };
                
                allocatedSeats = availableSeats.OrderBy(x => _random.Next()).Take(k).ToList();
            }
            
            foreach (var seat in allocatedSeats)
                _seats[seat.Row, seat.Col] = 'X';
            
            string bookingNumber = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            _bookings[bookingNumber] = allocatedSeats;

            return new Result { IsSuccess = true, Value = bookingNumber };
        }

        public Result DeleteBooking(string bookingNumber)
        {
            if (!_bookings.ContainsKey(bookingNumber))
                return new Result { IsSuccess = false, ErrorMessage = "Booking number not found." };

            var seatsToFree = _bookings[bookingNumber];

            foreach (var seat in seatsToFree)
                _seats[seat.Row, seat.Col] = 'O';

            _bookings.Remove(bookingNumber);

            return new Result { IsSuccess = true };
        }

        public List<string> GetBoard()
        {
            List<string> board = new List<string>();
            for (int i = 0; i < _rows; i++)
            {
                StringBuilder row = new StringBuilder();
                for (int j = 0; j < _columns; j++)
                {
                    row.Append(_seats[i, j]);
                    if (j < _columns - 1)
                        row.Append(" ");
                }
                board.Add(row.ToString());
            }
            return board;
        }

        public List<string> GetCurrentBookings()
        {
            return _bookings.Keys.Select(k => $"Booking #{k}").ToList();
        }
    }
}
