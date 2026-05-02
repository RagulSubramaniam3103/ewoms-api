using LoginIdentityWebAPI.Controllers;
using LoginIdentityWebAPI.Data;
using LoginIdentityWebAPI.FlightModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Flightsearchxunit
{
    public class FlightControllerXunit
    {
        private readonly ITestOutputHelper _output;
        public FlightControllerXunit(ITestOutputHelper testOutput)
        {
            _output = testOutput;
        }

        public FlightSearchingController GetController()
        {
            var options = new DbContextOptionsBuilder<AppDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDBContext(options);

            context.AirportDetails.AddRange(
                new AirportDetails
                {
                    Code = "MAA",
                    City = "Chennai",
                    Aiport_Name = "Chennai International Airport"
                },
                new AirportDetails
                {
                    Code = "SIN",
                    City = "Singapore",
                    Aiport_Name = "Singapore Changi Airport"
                }
            );

            context.timeZoneairports.AddRange(
                new TimeZoneairport
                {
                    AirpotCode = "MAA",
                    TimeZoneName = "India Standard Time",
                    DisplayName = "(UTC+05:30) Chennai",
                    UtcOffsetHours = 5.5m
                },
                new TimeZoneairport
                {
                    AirpotCode = "SIN",
                    TimeZoneName = "Singapore Standard Time",
                    DisplayName = "(UTC+08:00) Singapore",
                    UtcOffsetHours = 8m
                }
            );

            context.FlightTravelDetails.Add(new FlightTravelDetails
            {
                FlightId = 1,
                FlightNumber = "AI101",
                FlightName = "Air India",
                FromCode = "MAA",
                ToCode = "SIN",
                DepartureTime = new DateTime(2026, 2, 3, 8, 30, 0),
                ArrivalTime = new DateTime(2026, 2, 3, 14, 30, 0),
                IsSchedule = true,
                BookingStatus = true
            });

            context.FlightSeatPrice.Add(new FlightSeatPrice
            {
                FlightNumber = "AI101",
                SeatClass = "",
                Price = 25000
            });

            context.SaveChanges();

            return new FlightSearchingController(context, null);
        }

        [Fact]
        public void AdditionWorks()
        {
            int inputa = 10;
            int inputb = 20;
            var result = inputa + inputb;
            Assert.Equal(30, result);
        }

        [Fact]
        public void AddAirpotCodeCity()
        {
            var controller = GetController();
            var aiportdetails = new List<AirportDetails>
            {
                new AirportDetails{AirportId = 1,Code = "HND",City = "Tokyo", Aiport_Name = "Tokyo Haneda Airport"},
                new AirportDetails{AirportId = 2,Code = "NRT",City = "Chiba/Tokyo", Aiport_Name = "Narita International Airport"},
                new AirportDetails{AirportId = 3,Code = "KIX",City = "Osaka", Aiport_Name = "Kansai International Airport"},
            };
            var result = controller.AddAirportCity(aiportdetails) as OkObjectResult;
            Assert.NotNull(result);
            Assert.Equal("Airport City Added Successfully", result.Value);
        }

        [Fact]
        public void FlightSearchTicket_Returns_Flight_When_Direct()
        {
            var controller = GetController();

            var request = new Flightsearch
            {
                Departed = "MAA",
                Arrival = "SIN",
                FlightDate = new DateTime(2026, 2, 3, 8, 30, 0),
                Class = "",
                stops = 0,
                TimeSession = 2,
                Price1 = 4444444,
                Price2 = 5000000,
                SortBy = 2
            };

            var result = controller.GetFlightTicket(request);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var flights = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);

            Assert.NotEmpty(flights);
        }


    }
}
