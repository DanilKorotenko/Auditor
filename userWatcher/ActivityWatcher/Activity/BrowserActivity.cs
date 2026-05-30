using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace userWatcher.ActivityWatcher.Activity;

public class BrowserActivity : Activity
{

    public BrowserActivity() : base()
    {
        CurrentURL = string.Empty;
    }
    public override ActivityType Type 
    { 
        get 
        {
            return ActivityType.Browser;
        } 
    }

    public string CurrentURL { get; set; }

    public override string ToString()
    {
        return $"{Timestamp} {Type} {UserName} {ProcessName} {CurrentURL}";
    }

    public override void SetProcessInfo(ProcessInfo aProcessInfo)
    {
        base.SetProcessInfo(aProcessInfo);

        CurrentURL = aProcessInfo.GetCurrentURL();
    }
}
