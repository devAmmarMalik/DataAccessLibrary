using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Data;

public class Properties
{
    public bool UseDataRange { get; set; } = false;
    public int NoOfDaysPrior { get; set; } = 10;

    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }

    public DateTime DSTStart { get; set; } = DateTime.Parse(DateTime.Now.Year + "-11-02");
    public DateTime DSTEnd { get; set; } = DateTime.Parse(DateTime.Now.Year + "-3-9");
    public bool ExportToFiles { get; set; } = true;

    public Properties()
    {
        InitializeDates();
    }

    private void InitializeDates()
    {
        if (!UseDataRange)
        {
            DateTo = DateTime.Today;
            DateFrom = DateTime.Today.AddDays(-NoOfDaysPrior);
        }
    }
}
