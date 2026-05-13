using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace userWatcher.ActivityWatcher;

public class Activity
{
    public Activity()
    {
        Timestamp = DateTime.Now;
        ProcessName = string.Empty;
        ProcessExePath = string.Empty;
        UserName = string.Empty;
        AdditionalInfo = new Dictionary<string, string>();
    }
    public DateTime Timestamp { get; }
    public string ProcessName { get; }
    public string ProcessExePath { get; }
    public string UserName { get; }
    public Dictionary<string, string> AdditionalInfo { get; }

    public override string ToString()
    {
        return $"{Timestamp} {UserName} {ProcessName} {AdditionalInfo}";
    }
}
