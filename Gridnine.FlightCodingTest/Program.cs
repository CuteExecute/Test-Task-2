using System;
using System.Collections.Generic;

namespace Gridnine.FlightCodingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            FlightBuilder builder = new FlightBuilder();
            FlightFilter filter = new FlightFilter();
            IList<Flight> flights_original = builder.GetFlights();

            ShowList(flights_original, "Original data list");

            // use rule #1
            List<IRule> rules = new List<IRule> 
            { 
                new DeleteFlightBeforeCurrentDateTime()
            }; 
            IList<Flight> flights_sorted = filter.ExecuteFlightFilter(rules, builder.GetFlights());

            ShowList(flights_sorted, "Delete flight than current date");

            // use rule #2
            rules = new List<IRule>
            {
                new DeleteFlightWhenArrivalEarlierDeparture()
            };
            flights_sorted = filter.ExecuteFlightFilter(rules, builder.GetFlights());

            ShowList(flights_sorted, "Delete flight with arrival date earlier than departure date");

            // use rule #3
            rules = new List<IRule>
            {
                new DeleteFlightWhenTimeOnGroundMoreTwoHours()
            };
            flights_sorted = filter.ExecuteFlightFilter(rules, builder.GetFlights());

            ShowList(flights_sorted, "Delete flights where the total time spent on the ground exceeds 2 hours");

            // use all rules in one time
            rules = new List<IRule>
            {
                new DeleteFlightBeforeCurrentDateTime(),
                new DeleteFlightWhenArrivalEarlierDeparture(),
                new DeleteFlightWhenTimeOnGroundMoreTwoHours()
            };
            flights_sorted = filter.ExecuteFlightFilter(rules, builder.GetFlights());

            ShowList(flights_sorted, "Use all rules in one time");

            Console.ReadKey();
        }

        /// <summary>
        /// The assignment, of course, said "Do not consider the UI", but i think even the output to the console should be presentable
        /// </summary>
        /// <param name="fl"> "Flight" elements collection </param>
        /// <param name="description"> a description of the rules applied to the collection </param>
        public static void ShowList(IList<Flight> fl, string description)
        {
            Console.WriteLine("====================== FLIGHTS ======================");
            Console.WriteLine($"description: {description}");
            Console.WriteLine("");
            int flightCount = 1;

            foreach (var item in fl)
            {
                Console.WriteLine($"Flight #{flightCount}:");
                Console.WriteLine($"    Segements:");
                for (int i = 0; i < item.Segments.Count; i++)
                {
                    Console.WriteLine($"        {i + 1}. {item.Segments[i].DepartureDate} - {item.Segments[i].ArrivalDate}");
                }
                Console.WriteLine();

                flightCount++;
            }
        }
    }
}
