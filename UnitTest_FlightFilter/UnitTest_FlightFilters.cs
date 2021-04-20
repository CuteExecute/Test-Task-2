using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Gridnine.FlightCodingTest;
using System.Linq;

namespace UnitTest_FlightFilter
{
    [TestClass]
    public class UnitTest_FlightFilters
    {
        /// <summary>
        /// Comparison of list items using CollectionAssert.AreEqual() and even HashSet() did not show the correct result. 
        /// I had to compare the elements of the lists in a loop, which, of course, is not very good, but nevertheless it works.
        /// </summary>
        private void Compare(IList<Flight> expected, IList<Flight> actual)
        {
            Flight[] temp_expected = expected.ToArray();
            Flight[] temp_actual = actual.ToArray();
            if (temp_expected.Length != temp_actual.Length)
            {
                throw new System.Exception("Lists have different count of items.");
            }
            for (int i = 0; i < expected.Count; i++)
            {
                for (int j = 0; j < expected[i].Segments.Count; j++)
                {
                    var incorrectSegment = from segment in expected[i].Segments.SkipWhile(x => x.DepartureDate != actual[i].Segments[j].DepartureDate)
                                           select segment;

                    if (!incorrectSegment.Any())
                    {
                        throw new System.Exception("One or more items do not match.");
                    }
                }
            }
        }

        [TestMethod]
        public void Delete_Flight_Before_Current_Time() // test rule #1
        {
            // Arrange
            FlightBuilder builder = new FlightBuilder();
            FlightFilter filter   = new FlightFilter();

            IList<Flight> expected = builder.GetFlights();
            expected.RemoveAt(2); 

            List<IRule> rules = new List<IRule>
            {
                new DeleteFlightBeforeCurrentDateTime() // use rule #1
            };

            // Act
            IList<Flight> actual = filter.ExecuteFlightFilter(rules, builder.GetFlights());

            // Assert
            Compare(expected, actual);
        }

        [TestMethod]
        public void Delete_Flight_When_Arrival_Earlier_Departure() // test rule #2
        {
            // Arrange
            FlightBuilder builder = new FlightBuilder();
            FlightFilter filter = new FlightFilter();

            IList<Flight> expected = builder.GetFlights();
            expected.RemoveAt(3);

            List<IRule> rules = new List<IRule>
            {
                new DeleteFlightWhenArrivalEarlierDeparture() // use rule #2
            };

            // Act
            IList<Flight> actual = filter.ExecuteFlightFilter(rules, builder.GetFlights());

            // Assert
            Compare(expected, actual);
        }

        [TestMethod]
        public void Delete_Flight_When_Timne_On_Ground_More_Two_Hours() // test rule #3
        {
            // Arrange
            FlightBuilder builder = new FlightBuilder();
            FlightFilter filter = new FlightFilter();

            IList<Flight> expected = builder.GetFlights();
            expected.RemoveAt(5);
            expected.RemoveAt(4);

            List<IRule> rules = new List<IRule>
            {
                new DeleteFlightWhenTimeOnGroundMoreTwoHours() // use rule #3
            };

            // Act
            IList<Flight> actual = filter.ExecuteFlightFilter(rules, builder.GetFlights());

            // Assert
            Compare(expected, actual);
        }

        [TestMethod]
        public void Use_all_rules() // test all rules
        {
            // Arrange
            FlightBuilder builder = new FlightBuilder();
            FlightFilter filter = new FlightFilter();

            IList<Flight> expected = builder.GetFlights();
            expected.RemoveAt(5);
            expected.RemoveAt(4);
            expected.RemoveAt(3);
            expected.RemoveAt(2);

            List<IRule> rules = new List<IRule>
            {
                new DeleteFlightBeforeCurrentDateTime(),
                new DeleteFlightWhenArrivalEarlierDeparture(),
                new DeleteFlightWhenTimeOnGroundMoreTwoHours()
            };

            // Act
            IList<Flight> actual = filter.ExecuteFlightFilter(rules, builder.GetFlights());

            // Assert
            Compare(expected, actual);
        }
    }
}
