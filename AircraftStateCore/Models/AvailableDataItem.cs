namespace AircraftStateCore.Models;

public class AvailableDataItem
{
    public string txt;
    public string value;
    public bool enabled = false;

    public AvailableDataItem(string value, string txt, bool enabled = false)
    {
        this.value = value;
        this.txt = txt;
        this.enabled = enabled;
    }
}