using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Data;

public class Properties
{
    public bool UseDataRange { get; set; }
    public int NoOfDaysPrior { get; set; }

    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }

    public DateTime DSTStart { get; set; }
        //= DateTime.Parse(DateTime.Now.Year + "-11-02");
    public DateTime DSTEnd { get; set; }
        //= DateTime.Parse(DateTime.Now.Year + "-3-9");
    public bool ExportToFiles { get; set; }
        //= true;
    public string _filterString = string.Empty;

    public void Init()
    {
        InitializeDates();
        _filterString = $" (adddt >= '{DateFrom.Date.ToShortDateString()}' and adddt <= '{DateTo.Date.ToShortDateString()}') or (updatedt >= '{DateFrom.Date.ToShortDateString()}' and updatedt <= '{DateTo.Date.ToShortDateString()}') ";
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
