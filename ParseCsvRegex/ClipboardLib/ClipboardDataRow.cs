using System;
using System.Collections.Generic;
using System.Text;

namespace ParseCsvRegex.ClipboardLib
{
    public class ClipboardDataRow
    {
        public string SourceText { get; set; }

        public Dictionary<Column, string> RowData { get; private set; }

        public bool ParsingSuccessed { get; private set; } = false;

        public ClipboardDataRow(string sourceText)
        {
            SourceText = sourceText;

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

        private void ProcessTextLine()
        {
            string[] columns = SourceText.Split('\t');

            Column currentColumn = Column.A;

            foreach (var item in columns)
            {
                RowData[currentColumn++] = item;
            }
        }
    }
}
