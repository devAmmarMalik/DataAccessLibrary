using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Data;

public class MemoResult
{
    public string Value { get; set; } = "";
    public string UpdatedMemo { get; set; } = "";
    public bool Success { get; set; }

    public MemoResult GetMemoData(
        string memoText,          // replaces &pMemofld
        string pcKey,
        bool xremove = false,
        bool xreturnblank = false,
        string xdelimiter = "*",
        int xoccurrence = 1)
    {
        var result = new MemoResult
        {
            Value = "",
            UpdatedMemo = memoText,
            Success = false
        };

        if (string.IsNullOrEmpty(memoText))
        {
            result.Value = xreturnblank ? "" : "NA";
            return result;
        }

        if (xremove && !string.IsNullOrEmpty(xdelimiter) && xdelimiter != "*")
            throw new InvalidOperationException("Cannot send a 'remove' action with a delimiter specified! Must use '*'");

        int xgetoccurrence = 0;

        if (string.IsNullOrEmpty(xdelimiter))
            xdelimiter = "*";

        // Split like ALINES(..., CHR(10), CHR(13))
        var lines = memoText
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Split('\n');

        foreach (var line in lines)
        {
            int lnend = line.IndexOf(xdelimiter, StringComparison.Ordinal);

            if (lnend > -1)
            {
                string lcThiskey = line.Substring(0, lnend);

                if (pcKey == lcThiskey)
                {
                    string valuePart = line.Substring(lnend + xdelimiter.Length);

                    if (!xremove)
                    {
                        xgetoccurrence++;

                        if (xgetoccurrence == xoccurrence)
                        {
                            result.Value = valuePart;
                            result.Success = true;
                            return result;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        // Match VFP multiple replace patterns
                        string patternBase = pcKey + "*" + valuePart;

                        result.UpdatedMemo = result.UpdatedMemo
                            .Replace(patternBase + "\r\n", "")
                            .Replace(patternBase + "\r", "")
                            .Replace(patternBase + "\n", "")
                            .Replace(patternBase, "");

                        result.Success = true;
                        return result;
                    }
                }
            }
        }

        // Not found logic
        if (!xremove)
        {
            result.Value = xreturnblank ? "" : "NA";
            return result;
        }

        return result; // Success = false
    }
}