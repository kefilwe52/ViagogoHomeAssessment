using System;
using System.Collections.Generic;
using System.Linq;

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

            foreach (var data in customerQuery)
                AddToEmail(customer, data);

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

                string key = $"{customer.City}-{item.City}";
                try
                {
                    if (storageDictionary.ContainsKey(key))
                        eventDistance.TotalDistance = storageDictionary[key];
                    else
                    {
                        eventDistance.TotalDistance = GetDistance(item.City, customer.City);
                        storageDictionary.Add(key, eventDistance.TotalDistance);
                    }

                    eventDistance.Customer = customer;
                    eventDistance.Price = GetPrice(item);
                    eventDistance.Event = item;

                    eventDistanceList.Add(eventDistance);
                }
                catch
                {
                    // don't do anything if the exception is thrown
                }
            }


            //Thenby method will also order by price after ordering by distance

            var eventQuery = eventDistanceList
                .OrderBy(x => x.TotalDistance)
                .ThenBy(x => x.Price)
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
