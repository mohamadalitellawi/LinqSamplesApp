using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var cars = ProcessFile("fuel.csv");

            var query = cars.OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name);

            foreach (var car in query.Take(20))
            {
                Console.WriteLine($"{car.Name} : {car.Combined}");
            }

            Console.Clear();

            var query2 = from car in cars
                         where car.Manufacturer == "BMW"
                         orderby car.Combined descending, car.Name ascending
                         select car;
                         

            foreach (var car in query2.Take(20))
            {
                Console.WriteLine($"{car.Manufacturer} {car.Name} : {car.Combined}");
            }
        }

        private static List<Car> ProcessFile(string path)
        {
            var results = new List<Car>();

            results = File.ReadAllLines(path)
                .Skip(1)
                .Where(line => line.Length > 1)
                .Select(Car.ParseFromCsv)
                .ToList();

            return results;
        }
    }
}
