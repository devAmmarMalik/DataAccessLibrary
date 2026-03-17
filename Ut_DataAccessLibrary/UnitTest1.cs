using DataAccessLibrary.Data;
using System.Data;

namespace Ut_DataAccessLibrary;

public class UnitTestDataAccess
{
    [Fact]
    public void Test_ConnectionString()
    {
        string ut_connection = "Server=TGFNJSQL01;Database=WH;Trusted_Connection=True;TrustServerCertificate=True";

        SqlServerClient client = new SqlServerClient(ut_connection);
        DataTable returnVal = client.ExecuteQuery("Select * from [wh].[dbo].[outship] where accountid = 6769 and bol_no = '04714370000251882         ' ");

        Assert.Equal(1, returnVal.Rows.Count);
    }
}
