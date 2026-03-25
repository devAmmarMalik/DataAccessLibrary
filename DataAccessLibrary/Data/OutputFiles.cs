namespace DataAccessLibrary.Data;

public class OutputFiles
{
    public string? ExportTo { get; set; } = @"f:\ammar\projects\DL_Export\";
    public string CustomerFilename { get; set; } = $"tfscustomer_fox_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";
}
