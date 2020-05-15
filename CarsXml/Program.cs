using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CarsXml
{
    class Program
    {
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
        static void Main(string[] args)
        {
            CreateXml();

            QueryXml();
        }

        private static void QueryXml()
        {
            var document = XDocument.Load("fuel.xml");

            var query =
                document.Element("Cars")
                    ?.Elements("Car")
                    ?.Where(e => e.Attribute("Manufacturer")?.Value == "BMW")
                    ?.Select(e => e.Attribute("Name").Value);

            foreach (var item in query)
            {
                Console.WriteLine(item);
            }
        }

        private static void CreateXml()
        {
            var records = ProcessFile("fuel.csv");

            var document = new XDocument();
            var cars = new XElement("Cars");

            foreach (var record in records)
            {
                var car = new XElement("Car");
                var name = new XElement("Name", record.Name);
                var combined = new XElement("Combined", record.Combined);

                car.Add(name);
                car.Add(combined);
                cars.Add(car);

            }

            document.Add(cars);
            document.Save("fuel.xml");


            // option 2

            var document2 = new XDocument();
            document2.Add(new XElement("Cars",
                records.Select(record => new XElement("Car",
                                            new XAttribute("Name", record.Name),
                                            new XAttribute("Combined", record.Combined),
                                            new XAttribute("Manufacturer", record.Manufacturer)
                                            ))));


            document2.Save("fuel.xml");
        }



    }
}
