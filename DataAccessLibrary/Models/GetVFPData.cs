using DataAccessLibrary.Data;
using DataAccessLibrary.Logging;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DataAccessLibrary.Models;

public class GetVFPData
{
    private Properties _properties { get; set; }
    private OutputFiles _outputFiles { get; set; }
    private DataTable _tabActiveAccounts = new DataTable();

    public void SetProperties(Properties Properties, OutputFiles OutputFiles)
    {
        _properties = Properties;
        _outputFiles = OutputFiles;
        ActiveAccounts accSetup = new ActiveAccounts();
        _tabActiveAccounts = accSetup.AccountsTable();
    }

    public void ReadData()
    {
        if (_properties == null)
        {
            throw new InvalidOperationException("Properties must be set before reading data.");
        }

        LoggingService.Initialize(_outputFiles.ExportTo?.Trim());
        int customerCount = ExportCustomers();

        if (customerCount > 0)
        {
            int ProductCount = ExportProductsMaster();
        }
    }

    #region Get Accounts Data and create two files
    private int ExportCustomers()
    {
        LoggingService.Logger.Information("Exporting Customers");
        string ut_connCustomer = "Server=TGFNJSQL01;Database=QQ;Trusted_Connection=True;TrustServerCertificate=True";
        SqlServerClient CustomerClient = new SqlServerClient(ut_connCustomer);
        DataTable _ActiveAccs = CustomerClient.ExecuteQuery("select *, space(30) as Officeloc, space(10) as TimeConvToUTC from account where inactive = 0 order by accountid");
        _ActiveAccs.Columns["Officeloc"]?.ReadOnly = false;
        _ActiveAccs.Columns["TimeConvToUTC"]?.ReadOnly = false;

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

        if (_ActiveAccs.Rows.Count > 0 && _properties.ExportToFiles)
        {
            string filePath = $"{_outputFiles.ExportTo?.Trim()}{_outputFiles.CustomerFilename}.txt";

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
                        _tabActiveAccounts.Rows.Add(
                                row.Field<int?>("accountid") ?? 0,
                                row["Officeloc"]?.ToString()?.Trim() ?? "",
                                row["TimeConvToUTC"]?.ToString()?.Trim() ?? "");
                    }
                }
            }

            CreateRDYFile($"{_outputFiles.ExportTo?.Trim()}{_outputFiles.CustomerFilename}.rdy");
        }
        LoggingService.Logger.Information($"Customers exported with {_tabActiveAccounts.Rows.Count} active accounts from {_ActiveAccs.Rows.Count} total accounts processed");
        return _tabActiveAccounts.Rows.Count;
    }
    #endregion

    #region Get Products Master and create two files
    private int ExportProductsMaster()
    {
        DataTable dtOutput = new DataTable();
        //_filterString
        LoggingService.Logger.Information("Exporting Product Master");
        string filePath = $"{_outputFiles.ExportTo?.Trim()}{_outputFiles.ProductFilename}.txt";
        using (SqlServerClient ProductMasterClient = new SqlServerClient("Server=TGFNJSQL01;Database=WH;Trusted_Connection=True;TrustServerCertificate=True"))
        {
            using (var writer = new StreamWriter(filePath, false))
            {
                // Header information for the output file
                writer.WriteLine("facility|system_id|cust_id|tsi_product_code|upc_code|description|customer_product_code|style|color|size|dim|lbl|lot_tracked|serial_tracked|category|product_class|product_type|product_group|" +
                                 "cost|nmfc_code|nmfc_class_code|ppk_indicator|master_pack_qty|inner_pack_qty|sub_inner_pack_qty|country_of_origin|case_length|case_width|case_height|case_weight|unit_length|unit_width|" +
                                 "unit_height|unit_weight|is_supply");

                foreach (DataRow Account in _tabActiveAccounts.Rows)
                {
                    dtOutput = ProductMasterClient.ExecuteQuery($"select * from upcmast where accountid = {Account.Field<int?>("accountid")} and {_properties._filterString} ");
                    if (dtOutput.Rows.Count > 0 && _properties.ExportToFiles)
                    {
                        foreach (DataRow row in dtOutput.Rows)
                        {
                            string cOffice = TrimSafe(Account["officeLoc"]);

                            string lclength = NormalizeDimension(GetMemoData(row["info"].ToString(), "LENGTH"));
                            string lcwidth = NormalizeDimension(GetMemoData(row["info"].ToString(), "WIDTH"));
                            string lcheight = NormalizeDimension(GetMemoData(row["info"].ToString(), "HEIGHT"));

                            string lcDescrip = CleanText(row["descrip"]);

                            string line =
                                $"{cOffice}|{ToStr(row["accountid"])}|{ToStr(row["accountid"])}|NA|" +
                                $"{TrimSafe(row["upc"])}|{lcDescrip}||{TrimSafe(row["style"])}|" +
                                $"{TrimSafe(row["color"])}|{TrimSafe(row["id"])}|{ToStr(row["cube"])}||0|0|||" +
                                $"{TrimSafe(row["itemtype"])}||0|||0|0|0|0||" +
                                $"{lclength}|{lcwidth}|{lcheight}|{ToStr(row["weight"])}|0|0|0|0|0";

                            writer.WriteLine(line);
                        }

                    }
                }
            }
            CreateRDYFile($"{_outputFiles.ExportTo?.Trim()}{_outputFiles.ProductFilename}.rdy");
        }

        LoggingService.Logger.Information("Product Master exported with {dtOutput.Rows.Count} products");
        return dtOutput.Rows.Count;
    }
    #endregion

    #region Helper Methods
    #region Create a second empty file RDY
    private void CreateRDYFile(string fileToCreate) {
        using (var writer = new StreamWriter(fileToCreate, false))
        {
        }
    }

    #endregion

    #region Equivalent of emptynul()
    private bool IsEmptyNull(string val)
    {
        return string.IsNullOrWhiteSpace(val);
    }
    #endregion

    #region getmemodata()
    private string GetMemoData(string memoText, string pcKey)
    {
        MemoResult getMemo = new MemoResult();
        return getMemo.GetMemoData(memoText, pcKey).Value;
    }
    #endregion

    #region cleanText
    private string CleanText(object cTextToClean)
    {
        if (cTextToClean == null)
            return "NA";

        string cRetText = cTextToClean.ToString().Trim();

        cRetText = cRetText
            .Replace("\"", "")   // remove double quotes
            .Replace("'", "")    // remove single quotes
            .Replace("\n", "")   // remove LF (chr(10))
            .Replace(",", "");   // remove commas

        cRetText = cRetText.Trim();

        // If empty after cleaning → return "NA"
        if (cRetText.Length == 0)
            return "NA";

        return cRetText;
    }
    #endregion

    private string TrimSafe(object value)
    {
        return value == null ? "" : value.ToString().Trim();
    }

    private string ToStr(object value)
    {
        return value == null ? "" : Convert.ToString(value)?.Trim() ?? "";
    }

    private string NormalizeDimension(string val)
    {
        if (string.IsNullOrWhiteSpace(val) || val == "NA")
            return "0";

        return val.Trim();
    }
    #endregion

}
