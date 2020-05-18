using ParseCsvRegex.CsvLib;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ParseCsvRegex
{
    class Program
    {
        static void Main(string[] args)
        {

            List<CsvDataRow> allData = CsvHelper.ProcessFile("sample.csv", out _, 1);

            allData.ForEach(row => Console.WriteLine(row.SourceLine));

            foreach (CsvDataRow item in allData)
            {
                Console.WriteLine($"{item.RowData.Count};");
                foreach (KeyValuePair<Column, string> cell in item.RowData)
                {
                    Console.WriteLine($"Value = {cell.Value}\t\t\tat column = {cell.Key}");
                }
            }

            Console.Clear();

            List<ClipboardLib.ClipboardDataRow> clipboardDataRows = ClipboardLib.ClipboardHelper.ProcessClipboardData(out _);
            clipboardDataRows.ForEach(row => Console.WriteLine(row.SourceText));

            foreach (var item in clipboardDataRows)
            {
                Console.WriteLine(item.RowData.Count);
                foreach (var cell in item.RowData)
                {
                    Console.WriteLine($"Value = {cell.Value}\t\t\tat column = {cell.Key}");
                }
            }
        }
    }
}
