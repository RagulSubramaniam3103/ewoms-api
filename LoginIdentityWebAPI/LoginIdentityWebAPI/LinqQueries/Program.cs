using System.Data;
using System.Data.SqlClient;

public class Program
{
    public class TripDetails
    {
        public int TripId { get; set; }
        public string FromSource { get; set; }
        public string Destination { get; set; }
        public List<Flights> Flights { get; set; }
    }

    public class Flights
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Airline { get; set; }
        public List<Seats> Seats { get; set; }
        public bool IsAvailable { get; set; }
    }
    public class Seats
    {
        public string FlightNumber { get; set; }
        public string SeatNo { get; set; }
        public List<Class> Class { get; set; }
        public decimal Price { get; set; }
    }
    public class Class
    {
        public string ClassType { get; set; }
    }
    public static void Main(String[] args)
    {
        var Sourcefrom = "Bangalore";
        var Destinationto = "Coimbatore";

       

    }
}