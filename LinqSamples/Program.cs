using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable< Employee> developers = new Employee[]
            {
                new Employee { Id = 1, Name = "Scott" },
                new Employee {Id = 2 , Name = "Chris"}
            };

            List<Employee> sales = new List<Employee>()
            {
                new Employee {Id=3, Name = "Alex"}
            };

            //IEnumerator<Employee> enumerator = developers.GetEnumerator();
            //while (enumerator.MoveNext())
            //{
            //    Console.WriteLine(enumerator.Current.Name);
            //}

            //Func<int, int> square = x => x * x;

            //Console.WriteLine($"Square Value Of {5} = {square(5)}");

            var query = from developer in developers
                        where developer.Name.Length == 5 && developer.Id >= 1
                        orderby developer.Name descending
                        select new { Name = developer.Name};

            foreach (var item in query)
            {
                Console.WriteLine(item.Name);
            }
        }

    }
}
