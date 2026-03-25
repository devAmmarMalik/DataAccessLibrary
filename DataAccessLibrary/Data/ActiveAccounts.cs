using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLibrary.Data;

public class ActiveAccounts
{
    public int AccountId { get; set; }
    public string? OfficeLoc { get; set; }
    public string? TimeConvToUTC { get; set; }
}
