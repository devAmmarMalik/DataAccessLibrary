using DataAccessLibrary.Data;
using DataAccessLibrary.Logging;
using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Ut_DataAccessLibrary;

public class ut_Accounts
{
    [Fact]
    public void Test_ActiveAccounts()
    {
        LoggingService.Initialize(@"f:\ammar\");

        Properties properties = new Properties
        {
            DateFrom = DateTime.Parse("2024-01-01"),
            DateTo = DateTime.Parse("2024-12-31"),
        };

        GetVFPData getVFPData = new GetVFPData();
        getVFPData.SetProperties(properties, new OutputFiles());
        getVFPData.ReadData();

        Assert.Equal(1, 1);
    }

}
