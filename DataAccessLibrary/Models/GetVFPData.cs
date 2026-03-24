using DataAccessLibrary.Data;
using DataAccessLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccessLibrary.Models;

public class GetVFPData
{
    private Properties _properties { get; set; }
    private OutputFiles _outputFiles { get; set; }

    public void SetProperties(Properties Properties, OutputFiles OutputFiles)
    {
        _properties = Properties;
        _outputFiles = OutputFiles;
    }

    public void ReadData()
    {
        if (_properties == null)
        {
            throw new InvalidOperationException("Properties must be set before reading data.");
        }

        LoggingService.Initialize(@"f:\ammar\");
        string ut_connection = "Server=TGFNJSQL01;Database=WH;Trusted_Connection=True;TrustServerCertificate=True";
        SqlServerClient client = new SqlServerClient(ut_connection);

        int customerCount = ExportCustomers(client);
    }

    #region get accounts data
    private int ExportCustomers(SqlServerClient client)
    {
        DataTable _ActiveAccs = client.ExecuteQuery("select *, space(30) as Officeloc, space(10) as TimeConvToUTC from account where inactive = 0 order by accountid ");

        var officeMap = new List<(Func<DataRow, bool> Condition, string Office, string Offset)>
        {
            (r => r.Field<bool>("njinven"), "CARTERET", "+4"),
            (r => r.Field<bool>("cainven"), "SAN PEDRO", "+8"),
            (r => r.Field<bool>("mlinven"), "MIRA LOMA", "+3"),
            (r => r.Field<bool>("flinven"), "MIAMI", "+4"),
            (r => r.Field<bool>("kyinven"), "LOUISVILLE", "+4"),
            (r => r.Field<bool>("ddinven"), "CARSON", "+3"),
            (r => r.Field<bool>("eeinven"), "EDISON 1", "+4"),
            (r => r.Field<bool>("riinven"), "EDISON 2", "+4"),
            (r => r.Field<bool>("xxinven"), "EDISON 3", "+4"),
            (r => r.Field<bool>("ssinven"), "SAVANNAH", "+4"),
            (r => r.Field<bool>("oninven"), "ONTARIO", "+4"),
            (r => r.Field<bool>("hhinven"), "CHINO", "+3"),
        };

        foreach (DataRow row in _ActiveAccs.Rows)
        {
            var match = officeMap.FirstOrDefault(m => m.Condition(row));

            row["Officeloc"] = match == default ? "-----" : match.Office;
            row["TimeConvToUTC"] = match == default ? "0" : match.Offset;
        }

        if (_ActiveAccs.Rows.Count > 0)
        {
            string filePath = _outputFiles.CustomerFilename + ".txt";

            using (var writer = new StreamWriter(filePath, false))
            {
                // Customer Header information for the log file header
                writer.WriteLine("facility|system_id|tfs_id|trading_partner_id|master_cust_id|cust_id|cust_name|cust_add1|cust_add2|cust_add3|cust_city|cust_state|cust_zip|cust_country|cust_contact_name|cust_contact_phone");

                foreach (DataRow row in _ActiveAccs.Rows)
                {
                    string cOffice = row["Officeloc"]?.ToString()?.Trim() ?? "";

                    string acctName = row["acctname"]?.ToString()?.Trim() ?? "";
                    int accountId = row.Field<int?>("accountid") ?? 0;

                    if (cOffice != "-----" && !string.IsNullOrWhiteSpace(acctName) && accountId > 0)
                    {
                        string address = row["address"]?.ToString()?.Trim() ?? "";
                        string city = row["city"]?.ToString()?.Trim() ?? "";
                        string state = row["state"]?.ToString()?.Trim() ?? "";
                        string zip = row["zip"]?.ToString()?.Trim() ?? "";

                        string line =
                            $"{cOffice}|{accountId}||||{accountId}|{acctName}|{address}|||{city}|{state}|{zip}|||";

                        writer.WriteLine(line);
                    }
                }
            }

            filePath = _outputFiles.CustomerFilename + ".rdy";

            using (var writer = new StreamWriter(filePath, false))
            {
            }
        }
        return _ActiveAccs.Rows.Count;
    }
    #endregion
}
