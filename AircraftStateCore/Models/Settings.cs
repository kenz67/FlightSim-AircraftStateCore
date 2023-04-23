namespace AircraftStateCore.Models;

public class Settings
{
    public bool BlockLocation { get; set; } = false;
    public bool BlockFuel { get; set; } = false;
    public bool ShowApplyForm { get; set; } = false;
    public bool AutoSave { get; set; } = false;
    public bool ShowSaveAs { get; set; } = false;
    public string Version { get; set; }
    public List<AvailableDataItem> SelectedData { get; set; }
}