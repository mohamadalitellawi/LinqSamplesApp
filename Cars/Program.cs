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
            var manufacturers = ProcessManufacturers("manufacturers.csv");

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

            Console.Clear();

            var asking1 = cars.Any(c => c.Manufacturer == "Ford");
            var asking2 = cars.All(c => c.Manufacturer == "BMW");

            Console.WriteLine(asking1);
            Console.WriteLine(asking2);


            //Console.Clear();


            //// flatting operator

            //var result = cars.Select(c => c.Name);

            //foreach (var name in result)
            //{
            //    foreach (var letter in name)
            //    {
            //        Console.WriteLine(letter);
            //    }
            //}

            //Console.Clear();

            //// same result of the above
            //var result2 = cars.SelectMany(c => c.Name).OrderBy(c => c);
            //foreach (var letter in result2)
            //{
            //    Console.WriteLine(letter);
            //}


            Console.Clear();

            var query3 =
                from car in cars
                join manufacturer in manufacturers on car.Manufacturer equals manufacturer.Name
                orderby car.Combined descending, car.Name ascending
                select new
                {
                    manufacturer.Headquarters,
                    car.Name,
                    car.Combined
                };

            var query4 = cars.Join(manufacturers,
                c => c.Manufacturer,
                m => m.Name,
                (c, m) => new
                {
                    m.Headquarters,
                    c.Name,
                    c.Combined
                })
                .OrderByDescending(obj => obj.Combined)
                .ThenBy(obj => obj.Name);


            foreach (var car in query4.Take(10))
            {
                Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
            }


            var query5 = cars.Join(manufacturers,
                c => c.Manufacturer,
                m => m.Name,
                (c, m) => new
                {
                    Car = c,
                    Manufacturer = m
                })
                .OrderByDescending(obj => obj.Car.Combined)
                .ThenBy(obj => obj.Car.Name);

            foreach (var car in query5.Take(10))
            {
                Console.WriteLine($"{car.Manufacturer.Headquarters} {car.Car.Name} : {car.Car.Combined}");
            }


            Console.Clear();

            var query6 =
                from car in cars
                join manufacturer in manufacturers
                    on new { car.Manufacturer, car.Year }
                    equals new { Manufacturer = manufacturer.Name, manufacturer.Year }
                orderby car.Combined descending, car.Name ascending
                select new
                {
                    manufacturer.Headquarters,
                    car.Name,
                    car.Combined
                };

            var query7 = cars.Join(manufacturers,
                c => new { c.Manufacturer, c.Year },
                m => new { Manufacturer = m.Name, m.Year },
                (c, m) => new
                {
                    m.Headquarters,
                    c.Name,
                    c.Combined
                })
                .OrderByDescending(obj => obj.Combined)
                .ThenBy(obj => obj.Name);

            foreach (var car in query7.Take(10))
            {
                Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
            }

            Console.Clear();

            var query8 =
                from car in cars
                group car by car.Manufacturer;

            foreach (var item in query8)
            {
                Console.WriteLine($"{item.Key} has {item.Count()} cars");
            }





            Console.Clear();

            var query9 =
                from car in cars
                group car by car.Manufacturer.ToUpper() into newGroup
                orderby newGroup.Key
                select newGroup;

            var query10 = cars.GroupBy(c => c.Manufacturer.ToUpper()).OrderBy(g => g.Key);

            foreach (var group in query10)
            {
                Console.WriteLine(group.Key);
                foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }


            Console.Clear();

            var query11 =
                from manufacturer in manufacturers
                join car in cars on manufacturer.Name equals car.Manufacturer
                    into carGroup
                orderby manufacturer.Name
                select new
                {
                    Manufacturer = manufacturer,
                    Cars = carGroup
                };


            var query12 =
                manufacturers.GroupJoin(cars,
                m => m.Name,
                c => c.Manufacturer,
                (m, g) => new
                {
                    Manufacturer = m,
                    Cars = g
                })
                .OrderBy(m => m.Manufacturer.Name);

            foreach (var group in query12)
            {
                Console.WriteLine(group.Manufacturer.Name + " : " + group.Manufacturer.Headquarters);
                foreach (var car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }

            Console.Clear();

            var query13 =
                cars.GroupBy(c => c.Manufacturer)
                .Select(g =>
                {
                    var results = g.Aggregate(new CarStatistics(),
                                        (acc, c) => acc.Accumulate(c),
                                        acc => acc.Compute());

                    return new
                    {
                        Name = g.Key,
                        Avg = results.Average,
                        Max = results.Max,
                        Min = results.Min
                    };
                })
                .OrderByDescending(r => r.Max);

            foreach (var result in query13)
            {
                Console.WriteLine($"{result.Name}");
                Console.WriteLine($"\tMax: {result.Max}");
                Console.WriteLine($"\tMin: {result.Min}");
                Console.WriteLine($"\tAverage: {result.Avg}");
            }
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query = File.ReadAllLines(path)
                .Where(l => l.Length > 1)
                .ToManufacturer();

            return query.ToList();
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

    public class CarStatistics
    {
        public int Max { get; set; }
        public int Min { get; set; }
        public double Average { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
        public CarStatistics()
        {
            Max = int.MinValue;
            Min = int.MaxValue;
        }

        public CarStatistics Accumulate(Car car)
        {
            Count += 1;
            Total += car.Combined;
            Max = Math.Max(Max, car.Combined);
            Min = Math.Min(Min, car.Combined);
            return this;
        }

        public CarStatistics Compute()
        {
            Average = Total / Count;
            return this;
        }
    }
    public static class ManufacturerExtension
    {
        public static IEnumerable<Manufacturer> ToManufacturer(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new Manufacturer
                {
                    Name = columns[0],
                    Headquarters = columns[1],
                    Year = int.Parse(columns[2])
                };
            }
        }
    }
}
