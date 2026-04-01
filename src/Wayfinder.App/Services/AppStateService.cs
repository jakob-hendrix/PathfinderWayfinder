namespace Wayfinder.App.Services;

public class AppStateService
{
    public bool IsLoading { get; private set; }

    // Notify the UI that data as been reloaded
    public event Action? OnDataRefreshed;

    public event Action? OnLoadingStateChanged;

    public void SetLoading(bool loading)
    {
        IsLoading = loading;
        OnLoadingStateChanged?.Invoke();
    }
    public void NotifyDataRefreshed() => OnDataRefreshed?.Invoke();
}
