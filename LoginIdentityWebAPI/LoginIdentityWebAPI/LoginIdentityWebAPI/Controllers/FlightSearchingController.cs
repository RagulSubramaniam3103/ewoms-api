using LoginIdentityWebAPI.Data;
using LoginIdentityWebAPI.FlightModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace LoginIdentityWebAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]")]
    [ApiController]
    public class FlightSearchingController : ControllerBase
    {
        public readonly AppDBContext _context;
        private readonly JwtTokenService _jwtTokenService;

        public FlightSearchingController(AppDBContext appDB,JwtTokenService jwt)
        {
            _context = appDB;
            _jwtTokenService = jwt;
        }
        [HttpPost("AddAirportCity")]
        public IActionResult AddAirportCity([FromBody] List<AirportDetails> airportDetails)
        {
            _context.AirportDetails.AddRange(airportDetails);
            var addairportcity = _context.SaveChanges();
            if (addairportcity > 0)
            {
                return Ok("Airport City Added Successfully");
            }
            else
            {
                return BadRequest("Not Inserted Airport City");
            }
        }

        [HttpPost("AddTimeZoneAirport")]
        public IActionResult AddTimeZone([FromBody] List<TimeZoneairport> timeZoneairport)
        {
            _context.timeZoneairports.AddRange(timeZoneairport);
            var timezoneadd = _context.SaveChanges();
            if (timezoneadd > 0)
            {
                return Ok("TimeZone Added Successfully");
            }

            return Ok("Not TimeZone Added");
        }

        [HttpPost("InsertCapacityAirLine")]
        public IActionResult InsertAirlineCapacity([FromBody] List<FlightseatDetails> flightseatDetails)
        {
            _context.flightseatDetails.AddRange(flightseatDetails);
            var Resultdata = _context.SaveChanges();
            return Ok("Inserted Flight seat Details");
        }

        [HttpPost("SetSeatPrice")]
        public IActionResult SetSeatPrice([FromBody] FlightSeatPrice FlightSeatPrice)
        {

            var existingrecord = _context.flightseatDetails.FirstOrDefault(x => x.FlightNumber == FlightSeatPrice.FlightNumber);
            if (existingrecord != null)
            {
                _context.FlightSeatPrice.Add(FlightSeatPrice);
                _context.SaveChanges();
                return Ok("Seat Priced Successfully");
            }
            return Ok();
        }

        [HttpPost("New_FlightDetails")]
        public IActionResult AddNewFlightDetails([FromBody] ListFlight FlightDetails)
        {
            _context.FlightDetails.Add(FlightDetails);
            var finaladdition = _context.SaveChanges();
            if (finaladdition > 0)
            {
                var travelDetails = new FlightTravelDetails
                {

                    FlightNumber = FlightDetails.FlightNumber,
                    FlightName = FlightDetails.AirLine,
                    FromCode = FlightDetails.DepartureCity,
                    ToCode = FlightDetails.ArrivalCity,
                    DepartureTime = null,
                    ArrivalTime = null,
                    IsSchedule = false,
                    IsAvailable = FlightDetails.IsAvailable
                };
                _context.FlightTravelDetails.Add(travelDetails);
                var result = _context.SaveChanges();
                if (result > 0)
                {
                    return Ok("Inserted Successfully Wait for Approval Time Details");
                }
                else
                {
                    return BadRequest("Not Inserted Data");
                }

            }
            else
            {
                return BadRequest("Not Inserted Data");
            }
        }


        [HttpGet("UnScheduleAirline")]
        public IActionResult GetUnScheduleAirline()
        {
            var unscheduleAirlinelist = _context.FlightTravelDetails.Where(x => x.IsSchedule == false && x.Created_At.Date == DateTime.Today);
            return Ok(unscheduleAirlinelist);
        }

        [HttpPost("UpdateUnscheduleAirlines")]
        public IActionResult UpdateUnScheduleAirline(string FlightNumber, DateTime DepatureTime, DateTime ArrivalTime)
        {
            var existingairlineslist = _context.FlightTravelDetails.FirstOrDefault(x => x.FlightNumber == FlightNumber && x.IsSchedule == false);
            if (existingairlineslist != null)
            {
                existingairlineslist.DepartureTime = DepatureTime;
                existingairlineslist.ArrivalTime = ArrivalTime;
                existingairlineslist.IsSchedule = true;

                _context.SaveChanges();
                return Ok("Scheduled Successfully");
            }
            return BadRequest();
        }


        [HttpGet("ScheduledAirLine")]
        public IActionResult GetScheduleAirline()
        {
            var getscheduledAirlines = _context.FlightTravelDetails.Where(x => x.IsSchedule == true).Select(x => x).ToList();
            return Ok(getscheduledAirlines);
        }

        [HttpPost("StatusUpdateBooking")]
        public IActionResult UpdateStatusBookingAirline(string FlightNumber)
        {
            var getscheduledflights = _context.FlightTravelDetails.FirstOrDefault(x => x.IsSchedule == true && x.FlightNumber == FlightNumber);
            if (getscheduledflights != null)
            {
                getscheduledflights.BookingStatus = true;
                _context.SaveChanges();
                return Ok("Booking Open Now");
            }
            else
            {
                return Ok("Not Available for Open");
            }
            return BadRequest();
        }

        [HttpGet("GetAirportDetails")]
        public IActionResult GetAirportDetails([FromQuery] string? Searchairport = null)
        {
            var getallairportdetails = _context.AirportDetails.AsQueryable();

            if (!string.IsNullOrEmpty(Searchairport))
            {
                getallairportdetails = getallairportdetails.Where(x => x.Code.Contains(Searchairport) || x.City.Contains(Searchairport));
            }

            var getairport = getallairportdetails.Select(x => new
            {
                Code = x.Code,
                Name = x.Aiport_Name,
            }).ToList();

            if (getairport.Count() > 0)
            {
                return Ok(getairport);
            }
            else
            {
                return Ok(new
                {
                    Message = "No Airport Available"
                });
            }
        }


        [HttpGet("GetFlightTickets")]
        public IActionResult GetFlightTicket([FromQuery] Flightsearch FSearch)
        {

            if (FSearch.stops == 0)
            {
                var SearchFlight = from Flight in _context.FlightTravelDetails
                                   join FromAirport in _context.AirportDetails
                                   on Flight.FromCode equals FromAirport.Code
                                   join ToAirport in _context.AirportDetails
                                   on Flight.ToCode equals ToAirport.Code
                                   join FromTZ in _context.timeZoneairports
                                   on FromAirport.Code equals FromTZ.AirpotCode
                                   join ToTZ in _context.timeZoneairports
                                   on ToAirport.Code equals ToTZ.AirpotCode
                                   join FlightPrice in _context.FlightSeatPrice
                                   on FSearch.Class equals FlightPrice.SeatClass
                                   where Flight.IsSchedule == true && FlightPrice.FlightNumber == Flight.FlightNumber && (Flight.FromCode == FSearch.Departed & Flight.ToCode == FSearch.Arrival)
                                   && Flight.BookingStatus && Flight.DepartureTime.Value == FSearch.FlightDate.Value

                                   let departureUtc =
                                       Flight.DepartureTime.Value.AddHours(-(double)FromTZ.UtcOffsetHours)

                                   let arrivalUtc =
                                       Flight.ArrivalTime.Value.AddHours(-(double)ToTZ.UtcOffsetHours)


                                   let totalHours =
                                       (arrivalUtc - departureUtc).TotalHours


                                   select new
                                   {
                                       FlightId = Flight.FlightId,
                                       FlightNumber = Flight.FlightNumber,
                                       FlightName = Flight.FlightName,
                                       Departure = FromAirport.Aiport_Name + "(" + FromAirport.Code + ")",
                                       DepartureDate = Flight.DepartureTime,
                                       Arrival = ToAirport.Aiport_Name + "(" + ToAirport.Code + ")",
                                       ArrivalDate = Flight.ArrivalTime,
                                       Class = FSearch.Class,
                                       Price = FlightPrice.Price,
                                       Duration = totalHours
                                   };


                if (FSearch.TimeSession == 1)
                {
                    SearchFlight = SearchFlight.Where(x =>
                        x.DepartureDate.Value.Hour >= 1 &&
                        x.DepartureDate.Value.Hour < 6);
                }
                else if (FSearch.TimeSession == 2) 
                {
                    SearchFlight = SearchFlight.Where(x =>
                        x.DepartureDate.Value.Hour >= 6 &&
                        x.DepartureDate.Value.Hour < 12);
                }
                else if (FSearch.TimeSession == 3) 
                {
                    SearchFlight = SearchFlight.Where(x =>
                        x.DepartureDate.Value.Hour >= 12 &&
                        x.DepartureDate.Value.Hour < 18);
                }
                else if (FSearch.TimeSession == 4)
                {
                    SearchFlight = SearchFlight.Where(x =>
                        x.DepartureDate.Value.Hour >= 18 &&
                        x.DepartureDate.Value.Hour < 24);
                }
                else
                {
                    SearchFlight = SearchFlight.Select(x=>x);
                }
                if(FSearch.Price1 ==0 && FSearch.Price2 !=0)
                    SearchFlight = SearchFlight.Where(x => x.Price >= FSearch.Price1 && x.Price <= FSearch.Price2);

                if(FSearch.SortBy != 0)
                {
                    SearchFlight = FSearch.SortBy switch
                    {
                        1 => SearchFlight.OrderBy(x=>x.Price),
                        2 => SearchFlight.OrderByDescending(x=>x.Price),
                        3 => SearchFlight.OrderBy(x=>x.Duration),
                        4 => SearchFlight.OrderByDescending(x=>x.Duration)
                    };
                }

                if (SearchFlight.Count() > 0)
                {
                    var result = SearchFlight.Select(f => new
                    {
                        flightId = f.FlightId,
                        flightNumber = f.FlightNumber,
                        flightName = f.FlightName,
                        departure = f.Departure,
                        departureDate = f.DepartureDate,
                        arrival = f.Arrival,
                        arrivalDate = f.ArrivalDate,
                        @class = f.Class,
                        price = f.Price,
                        duration = f.Duration,
                        token = _jwtTokenService.GenerateFlightToken(f.FlightId,f.FlightNumber,f.Class,f.Price)
                    }).ToList();

                    return Ok(result);
                }
                else
                {
                    return Ok(new
                    {
                        Message = "No Flight Search"
                    });
                }
            }
            else if (FSearch.stops == 1)
            {

                var getflightsearchdetails = from flight1 in _context.FlightTravelDetails
                                             join fromairport1 in _context.AirportDetails
                                                on flight1.FromCode equals fromairport1.Code
                                             join fromairport1tz in _context.timeZoneairports
                                                on flight1.FromCode equals fromairport1tz.AirpotCode
                                             join toairport1 in _context.AirportDetails
                                                on flight1.ToCode equals toairport1.Code
                                             join toairport1tz in _context.timeZoneairports
                                                on flight1.ToCode equals toairport1tz.AirpotCode

                                             join FlightPrice1 in _context.FlightSeatPrice
                                                on flight1.FlightNumber equals FlightPrice1.FlightNumber

                                             join flight2 in _context.FlightTravelDetails
                                                on flight1.ToCode equals flight2.FromCode
                                             join fromairport2 in _context.AirportDetails
                                                on flight2.FromCode equals fromairport2.Code
                                             join fromairport2tz in _context.timeZoneairports
                                                on flight2.FromCode equals fromairport2tz.AirpotCode
                                             join toairport2 in _context.AirportDetails
                                                on flight2.ToCode equals toairport2.Code
                                             join toairport2tz in _context.timeZoneairports
                                                on flight2.ToCode equals toairport2tz.AirpotCode

                                             join FlightPrice2 in _context.FlightSeatPrice
                                                on flight2.FlightNumber equals FlightPrice2.FlightNumber

                                             where flight1.FromCode == FSearch.Departed &&
                                                   flight2.ToCode == FSearch.Arrival &&
                                                   flight1.IsSchedule &&
                                                   flight2.IsSchedule &&
                                                   flight2.DepartureTime >= flight1.ArrivalTime.Value.AddHours(1) && flight1.BookingStatus && flight2.BookingStatus &&
                                                   flight1.DepartureTime.Value.Date == FSearch.FlightDate.Value.Date &&
                                                   flight2.DepartureTime.Value > flight1.ArrivalTime.Value &&
                                                   FlightPrice1.SeatClass == FSearch.Class && FlightPrice2.SeatClass == FSearch.Class

                                             let departure1Utc = flight1.DepartureTime.Value.AddHours(-(double)fromairport1tz.UtcOffsetHours)

                                             let arrival1Utc = flight1.ArrivalTime.Value.AddHours(-(double)toairport1tz.UtcOffsetHours)

                                             let totalHours1 = (arrival1Utc - departure1Utc).TotalHours

                                             let departure2Utc = flight2.DepartureTime.Value.AddHours(-(double)fromairport2tz.UtcOffsetHours)

                                             let arrival2Utc = flight2.ArrivalTime.Value.AddHours(-(double)toairport2tz.UtcOffsetHours)

                                             let totalHours2 = (arrival2Utc - departure2Utc).TotalHours

                                             let layoverduration = (departure2Utc - arrival1Utc).TotalHours
                                             let totalJourneyDuration = (arrival2Utc - departure1Utc).TotalHours

                                             select new
                                             {
                                                 DepartureDate = flight1.DepartureTime,
                                                 Layover = layoverduration,
                                                 TotalJourneyHours = totalJourneyDuration,
                                                 Segments = new[]
                                                 {
                                                     new
                                                        {
                                                            FlightId = flight1.FlightId,
                                                            FlightNumber = flight1.FlightNumber,
                                                            Airline = flight1.FlightName,
                                                            From = fromairport1.Aiport_Name +"("+ fromairport1.Code +")",
                                                            To = toairport1.Aiport_Name +"("+ toairport1.Code +")",
                                                            DepartureTime = flight1.DepartureTime,
                                                            ArrivalTime = flight1.ArrivalTime,
                                                            Duration = totalHours1,
                                                            Class = FlightPrice1.SeatClass,
                                                            Price = FlightPrice1.Price
                                                        },
                                                        new
                                                        {
                                                            FlightId = flight2.FlightId,
                                                            FlightNumber = flight2.FlightNumber,
                                                            Airline = flight2.FlightName,
                                                            From = fromairport2.Aiport_Name +"("+ fromairport2.Code +")",
                                                            To = toairport2.Aiport_Name +"("+ toairport2.Code +")",
                                                            DepartureTime = flight2.DepartureTime,
                                                            ArrivalTime = flight2.ArrivalTime,
                                                            Duration = totalHours2,
                                                            Class = FlightPrice2.SeatClass,
                                                            Price = FlightPrice2.Price
                                                        }
                                                 },
                                                 token = ""
                                             };

                if (FSearch.TimeSession == 1)
                {
                    getflightsearchdetails = getflightsearchdetails.Where(x =>
                        x.DepartureDate.Value.Hour >= 1 &&
                        x.DepartureDate.Value.Hour < 6);
                }
                else if (FSearch.TimeSession == 2)
                {
                    getflightsearchdetails = getflightsearchdetails.Where(x =>
                        x.DepartureDate.Value.Hour >= 6 &&
                        x.DepartureDate.Value.Hour < 12);
                }
                else if (FSearch.TimeSession == 3)
                {
                    getflightsearchdetails = getflightsearchdetails.Where(x =>
                        x.DepartureDate.Value.Hour >= 12 &&
                        x.DepartureDate.Value.Hour < 18);
                }
                else if (FSearch.TimeSession == 4)
                {
                    getflightsearchdetails = getflightsearchdetails.Where(x =>
                        x.DepartureDate.Value.Hour >= 18 &&
                        x.DepartureDate.Value.Hour < 24);
                }
                else
                {
                    getflightsearchdetails = getflightsearchdetails.Select(x => x);
                }

                return Ok(getflightsearchdetails);
            }
            else
            {
                return Ok();
            }
        }
        [HttpPost("BookTicket")]
        public IActionResult BookTicket([FromBody] string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var flightId = jwtToken.Claims.First(x => x.Type == "FlightId").Value;
            var flightNumber = jwtToken.Claims.First(x => x.Type == "FlightNumber").Value;
            var seatClass = jwtToken.Claims.First(x => x.Type == "Class").Value;
            var price = jwtToken.Claims.First(x => x.Type == "Price").Value;

            return Ok(new
            {
                FlightId = flightId,
                FlightNumber = flightNumber,
                Class = seatClass,
                Price = price
            });
        }

    }
}
