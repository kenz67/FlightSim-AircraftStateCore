using AircraftStateCore.Models;
using BootstrapBlazor.Components;

namespace AircraftStateCore.Services.Interfaces;

public interface ISettingsData : IPageUpdate
{
    Task<Settings> ReadSettings();
    Task SaveSettings(List<SelectedItem> SelectedItems);
    // Settings Settings { get; set; }
    List<AvailableDataItem> GetSelectedData();
}