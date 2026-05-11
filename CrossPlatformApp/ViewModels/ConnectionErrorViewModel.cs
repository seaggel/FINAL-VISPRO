namespace CrossPlatformApp.ViewModels;

public sealed class ConnectionErrorViewModel : BaseViewModel
{
    private string statusMessage = string.Empty;

    public string StatusMessage
    {
        get => statusMessage;
        set
        {
            if (statusMessage != value)
            {
                statusMessage = value;
                OnPropertyChanged();
            }
        }
    }
}
