using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Win32;


namespace ParseCsvRegex.ClipboardLib
{
    public enum Column
    {
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        AA, AB, AC, AD, AE, AF, AG, AH, AI, AJ, AK, AL, AM, AN, AO, AP, AQ, AR, AS, AT, AU, AV, AW, AX, AY, AZ
    }
    public class ClipboardHelper
    {
        public static List<ClipboardDataRow> ProcessClipboardData( out List<ClipboardDataRow> failedRows)
        {
            var results = new List<ClipboardDataRow>();
            var badResults = new List<ClipboardDataRow>();

            
            results = Clipboard.GetText().Split(Environment.NewLine)
                .Where(line => line.Length > 1)
                .Select(line => new ClipboardDataRow(line))
                .Where(row => row.ParsingSuccessed)
                .ToList();
            
            badResults = Clipboard.GetText().Split(Environment.NewLine)
                .Where(line => line.Length > 1)
                .Select(line => new ClipboardDataRow(line))
                .Where(row => !row.ParsingSuccessed)
                .ToList();

            failedRows = badResults;

            return results;
        }

    }
}
