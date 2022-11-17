using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ViagogoHomeAssessment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Process");

            var events = new List<Event>{
                new Event{ Name = "Phantom of the Opera", City = "New York"},
                new Event{ Name = "Metallica", City = "Los Angeles"},
                new Event{ Name = "Metallica", City = "New York"},
                new Event{ Name = "Metallica", City = "Boston"},
                new Event{ Name = "LadyGaGa", City = "New York"},
                new Event{ Name = "LadyGaGa", City = "Boston"},
                new Event{ Name = "LadyGaGa", City = "Chicago"},
                new Event{ Name = "LadyGaGa", City = "San Francisco"},
                new Event{ Name = "LadyGaGa", City = "Washington"}
            };

            var customer = new Customer { Name = "John Smith", City = "New York" };

            var customerQuery = events.Where(x => x.City == customer.City);

            Console.WriteLine("Displaying events in the same city");
            foreach (var data in customerQuery)
                AddToEmail(customer, data);


            Console.WriteLine("---------------------------------------");

            Console.WriteLine("Displaying closes distance");
            ClosesDistance(events, customer);

            Console.WriteLine("End Process");

            Console.ReadLine();
        }

        public static void ClosesDistance(List<Event> events, Customer customer)
        {
            var storageDictionary = new Dictionary<string, int>();
            var eventDistanceList = new List<EventDistance>();

            foreach (var item in events)
            {
                var eventDistance = new EventDistance();

                var key = $"{customer.City}{item.City}";

                var orderedKey = string.Concat(key.OrderBy(c => c)); //To avoid saving duplications of the distance

                if (storageDictionary.ContainsKey(orderedKey))
                    eventDistance.TotalDistance = storageDictionary[orderedKey];
                else
                {
                    eventDistance.TotalDistance = GetDistanceRetryCatch(item.City, customer.City);
                    storageDictionary.Add(orderedKey
                        , eventDistance.TotalDistance);
                }

                eventDistance.Customer = customer;
                eventDistance.Price = GetPrice(item);
                eventDistance.Event = item;

                eventDistanceList.Add(eventDistance);
            }


            //Thenby method will also order by price after ordering by distance and you can also order by customer or event

            var eventQuery = eventDistanceList
                .OrderBy(x => x.TotalDistance)
                .ThenBy(x => x.Price)
                .ThenBy(x => x.Event.City)
                .Take(5)
                .ToList();

            foreach (var data in eventQuery)
                AddToEmail(data.Customer, data.Event, data.Price);
        }



        static int GetPrice(Event e)
        {
            return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
        }

        static void AddToEmail(Customer c, Event e, int? price = null)
        {
            var distance = GetDistance(c.City, e.City);
            Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
                                  + (distance > 0 ? $" ({distance} miles away)" : "")
                                  + (price.HasValue ? $" for ${price}" : ""));
        }

        static int GetDistance(string fromCity, string toCity)
        {
            return AlphebiticalDistance(fromCity, toCity);
        }

        public static int GetDistanceRetryCatch(string fromCity, string toCity)
        {
            var tries = 3;
            while (true)
            {
                try
                {
                    return GetDistance(fromCity, toCity);
                }
                catch
                {
                    if (--tries == 0)
                    {
                        Console.WriteLine(
                            $"from city : {fromCity} and to city : {toCity} fail to calculate the distance");
                        return 0;
                    }

                    Thread.Sleep(1000);
                }
            }
        }

        private static int AlphebiticalDistance(string s, string t)
        {
            var result = 0;
            var i = 0;
            for (i = 0; i < Math.Min(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                result += Math.Abs(s[i] - t[i]);
            }
            for (; i < Math.Max(s.Length, t.Length); i++)
            {
                // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                result += s.Length > t.Length ? s[i] : t[i];
            }
            return result;
        }
    }
}
