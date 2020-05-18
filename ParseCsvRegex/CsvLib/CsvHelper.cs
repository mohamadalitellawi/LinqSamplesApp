using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ParseCsvRegex.CsvLib
{
    public enum Column
    {
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        AA, AB, AC, AD, AE, AF, AG, AH, AI, AJ, AK, AL, AM, AN, AO, AP, AQ, AR, AS, AT, AU, AV, AW, AX, AY, AZ
    }
    public partial class CsvHelper
    {

        public static List<CsvDataRow> ProcessFile(string path, out List<CsvDataRow> failedRows , int rowsToSkip = 0)
        {
            var results = new List<CsvDataRow>();
            var badResults = new List<CsvDataRow>();

            results = File.ReadAllLines(path)
                .Skip(rowsToSkip)
                .Where(line => line.Length > 1)
                .Select(line => new CsvDataRow(line,path))
                .Where(row => row.ParsingSuccessed)
                .ToList();

            badResults = File.ReadAllLines(path)
                .Skip(rowsToSkip)
                .Where(line => line.Length > 1)
                .Select(line => new CsvDataRow(line, path))
                .Where(row => !row.ParsingSuccessed)
                .ToList();

            failedRows = badResults;

            return results;
        }
    }
}
