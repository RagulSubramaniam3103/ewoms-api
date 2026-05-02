using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginIdentityWebAPI.FlightModels
{
    public class ListFlight
    {
        [Key]
        public int FlightId { get; set; }
        public string AirLine { get; set; }
        public string FlightNumber { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public bool IsAvailable { get; set; }
    }
    public class AirportDetails
    {

        [Key]
        public int Id { get; set; }
        public int AirportId { get; set; }
        public string Code { get; set; }        
        public string City { get; set; }        
        public string Aiport_Name { get; set; }
    }

    public class FlightTravelDetails
    {
        [Key]
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string FlightName { get; set; }
        public string FromCode { get; set; }
        public string ToCode { get; set; }
        public DateTime? DepartureTime { get; set; }
        public DateTime? ArrivalTime { get; set; }
        public bool IsSchedule { get; set;}
        public bool IsAvailable { get; set; }
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
        public bool BookingStatus { get; set; }
    }

    public class FlightseatDetails
    {
        [Key]
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public DateTime? Created_At { get; set; }
        public bool Businessclass { get; set; }
        public int TotalSeat_Businessclass { get; set; }
        public bool Economicclass { get; set; }
        public int TotalSeat_Economicclass { get; set; }
        public bool Firstclass { get; set; }
        public int TotalSeat_Firstclass { get; set; }
    }
    public class FlightSeatPrice
    {
        [Key]
        public int Id { get; set; }
        public string FlightNumber { get; set; }
        public string SeatClass { get; set; }
        public long Price { get; set; }
        public DateTime? Created_At { get; set; }

    }


    public class Flightsearch
    {
        public string Departed { get; set; }
        public string Arrival { get; set; }
        public DateTime? FlightDate { get; set; }
        public string Class { get; set; }
        public int stops { get; set; }
        public int TimeSession { get; set; }
        public long Price1 { get; set; }
        public long Price2 { get; set; }
        public int? SortBy { get; set; }
        public int? LayoverJourney { get; set; }
    }
    public class TimeZoneairport
    {
        [Key]
        public int Tno { get; set; }
        public string AirpotCode { get; set; }
        public string TimeZoneName { get; set; }
        public string DisplayName { get; set; }
        public decimal UtcOffsetHours { get; set; }
    }
    public class BookFlightTicket
    {
        [Key]
        public long BId { get; set; }
        public string BookPNR { get; set; }
        public string FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string Depature { get; set; }
        public string Arrival { get; set; }
    }
}
