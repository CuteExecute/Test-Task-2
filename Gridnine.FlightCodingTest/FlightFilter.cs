using System;
using System.Collections.Generic;
using System.Linq;

namespace Gridnine.FlightCodingTest
{
    public class FlightFilter
    {
        /// <summary>
        /// Thanks to this design(pattern Strategy), you can process several rules, in one method call, if necessary.
        /// </summary>
        /// <param name="rules"> List of rules, if there are more than one rules, then the order of the rules matters </param>
        /// <param name="flights"> Flight list which contains original data to filter </param>
        /// <returns></returns>
        public IList<Flight> ExecuteFlightFilter(IList<IRule> rules, IList<Flight> flights)
        {
            if (rules.Count > 0)
            {
                for (int i = 0; i < rules.Count; i++)
                {
                    if (rules[i] != null)
                        flights = rules[i].GetFilter(flights);
                    else
                        throw new Exception("Rules is empty.");
                }
            }
            else
                throw new Exception("List is empty.");

            return flights;
        }
    }

    public interface IRule
    {
        public IList<Flight> GetFilter(IList<Flight> flights);
    }

    // #1 rule
    public class DeleteFlightBeforeCurrentDateTime : IRule
    {
        DateTime _thisDateTime = DateTime.Now;

        public IList<Flight> GetFilter(IList<Flight> flights)
        {
            IList<Flight> temp = new List<Flight>();
            foreach (var item in flights)
            {
                var incorrectSegment = from segment in item.Segments.SkipWhile(x => x.DepartureDate > _thisDateTime) 
                                       select segment;

                if (!incorrectSegment.Any())
                {
                    temp.Add(item);
                }
            }

            flights = temp;
            return flights;
        }
    }

    // #2 rule
    public class DeleteFlightWhenArrivalEarlierDeparture : IRule
    {
        public IList<Flight> GetFilter(IList<Flight> flights)
        {
            IList<Flight> temp = new List<Flight>();
            foreach (var item in flights) 
            {
                var incorrectSegment = from segment in item.Segments.SkipWhile(x => x.DepartureDate < x.ArrivalDate)
                                       select segment;

                if (!incorrectSegment.Any())
                {
                    temp.Add(item);
                }
            }

            flights = temp;
            return flights;
        }
    }

    // #3 rule
    public class DeleteFlightWhenTimeOnGroundMoreTwoHours : IRule
    {
        public IList<Flight> GetFilter(IList<Flight> flights)
        {
            IList<Flight> temp   = new List<Flight>();
            TimeSpan TimeOnEarth = new TimeSpan();
            TimeSpan LimitTime   = new TimeSpan(2, 0, 0);

            for (int i = 0; i < flights.Count; i++)
            {
                for (int j = 0; j < flights[i].Segments.Count; j++)
                {
                    if (flights[i].Segments.Count != 1 && flights[i].Segments.Count - 1 != j )
                    {
                        TimeOnEarth += flights[i].Segments[j + 1].DepartureDate - flights[i].Segments[j].ArrivalDate;
                    }
                    if (TimeOnEarth >= LimitTime)
                    {
                        break;
                    }
                }

                if (TimeOnEarth <= LimitTime)
                {
                    if (flights[i] != null)
                        temp.Add(flights[i]);
                }
                TimeOnEarth = new TimeSpan();
            }

            flights = temp;
            return flights;
        }
    }
}
