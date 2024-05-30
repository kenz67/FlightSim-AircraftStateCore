namespace AircraftStateCore.Services.Interfaces;

public interface IPageUpdate
{
    public event Func<Task> OnChangeAsync;
}
