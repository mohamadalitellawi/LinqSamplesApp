using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ParseCsvRegex.CsvLib
{
    public class CsvDataRow
    {
        public  string SourceLine { get; private set; }
        public string SourceFile { get; private set; }
        public Dictionary<Column,string> RowData { get; private set; }

        public bool ParsingSuccessed { get; private set; } = false;

        public CsvDataRow(string sourceline, string sourceFile = "")
        {
            SourceLine = sourceline;
            SourceFile = sourceFile;

            RowData = new Dictionary<Column, string>();

            try
            {
                ProcessTextLine();

                if (RowData != null)
                    ParsingSuccessed = true;
            }
            catch (System.Exception)
            {
                ParsingSuccessed = false;
            }
            
        }
        private  void ProcessTextLine()
        {
            var pattern = @"(,|^)([^"",]+| ""(?:[^""]| """")*"")?";
            var regex = new Regex(pattern, RegexOptions.IgnorePatternWhitespace);

            var matches = regex.Matches(SourceLine);
            if (matches.Count <= 0)
                return;
            Column currentColumn = Column.A;

            foreach (Match match in matches)
            {
                int count = 0;
                // Console.WriteLine(match.Value);
                foreach (Group group in match.Groups)
                {
                    // Collect third item in each group
                    count++;
                    if (count == 3)
                    {
                        // Console.WriteLine($"\tValue: {group.Value} at index {group.Index}");
                        RowData[currentColumn++] = group.Value;
                        count = 0;
                    }
                    
                }
            }
        }
    }
}
