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
        ProcessName = string.Empty;
        ProcessExePath = string.Empty;
        UserName = string.Empty;
        AdditionalInfo = new Dictionary<string, string>();
    }
    public string ProcessName { get; }
    public string ProcessExePath { get; }
    public string UserName { get; }
    public Dictionary<string, string> AdditionalInfo { get; }

}
