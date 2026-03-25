using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DataAccessLibrary.Data;

public class ActiveAccounts
{
    public int AccountId { get; set; }
    public string? OfficeLoc { get; set; }
    public string? TimeConvToUTC { get; set; }

    public DataTable AccountsTable()
    {
        DataTable table = new DataTable("ActiveAccounts");

        // AccountId column (non-nullable int)
        DataColumn accountIdCol = new DataColumn("AccountId", typeof(int))
        {
            AllowDBNull = false
        };
        table.Columns.Add(accountIdCol);

        // OfficeLoc column (nullable string)
        DataColumn officeLocCol = new DataColumn("OfficeLoc", typeof(string))
        {
            AllowDBNull = true
        };
        table.Columns.Add(officeLocCol);

        // TimeConvToUTC column (nullable string)
        DataColumn timeConvCol = new DataColumn("TimeConvToUTC", typeof(string))
        {
            AllowDBNull = true
        };
        table.Columns.Add(timeConvCol);

        return table;
    }
}

/*
 * using System;
using System.Data;

public record ActiveAccounts
{
    public int AccountId { get; set; }
    public string? OfficeLoc { get; set; }
    public string? TimeConvToUTC { get; set; }
}

class Program
{
    static void Main()
    {
        // Create the DataTable
        DataTable activeAccountsTable = CreateActiveAccountsTable();

        // Example: Add a row
        activeAccountsTable.Rows.Add(101, "New York", "2026-03-25T15:00:00Z");
        activeAccountsTable.Rows.Add(102, DBNull.Value, DBNull.Value); // Null values

        // Display the table contents
        foreach (DataRow row in activeAccountsTable.Rows)
        {
            Console.WriteLine(
                $"AccountId: {row["AccountId"]}, " +
                $"OfficeLoc: {row["OfficeLoc"]}, " +
                $"TimeConvToUTC: {row["TimeConvToUTC"]}"
            );
        }
    }

    /// <summary>
    /// Creates a DataTable with the same structure as the ActiveAccounts record.
    /// </summary>
    static DataTable CreateActiveAccountsTable()
    {
        DataTable table = new DataTable("ActiveAccounts");

        // AccountId column (non-nullable int)
        DataColumn accountIdCol = new DataColumn("AccountId", typeof(int))
        {
            AllowDBNull = false
        };
        table.Columns.Add(accountIdCol);

        // OfficeLoc column (nullable string)
        DataColumn officeLocCol = new DataColumn("OfficeLoc", typeof(string))
        {
            AllowDBNull = true
        };
        table.Columns.Add(officeLocCol);

        // TimeConvToUTC column (nullable string)
        DataColumn timeConvCol = new DataColumn("TimeConvToUTC", typeof(string))
        {
            AllowDBNull = true
        };
        table.Columns.Add(timeConvCol);

        return table;
    }
}

 */