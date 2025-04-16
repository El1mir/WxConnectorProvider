using FlaUI.Core.AutomationElements;

namespace WxConnectorProvider.Providers;

public class DataProvider
{
    private static DataProvider? _instance;
    private static readonly object Lock = new object();
    public List<Window> ListeningWindows { get; set; } = new List<Window>();

    public static DataProvider Get()
    {
        if (_instance != null) return _instance;
        lock (Lock)
        {
            _instance ??= new DataProvider();
        }
        return _instance;
    }
}
