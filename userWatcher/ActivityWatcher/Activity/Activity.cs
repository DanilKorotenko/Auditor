using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace userWatcher.ActivityWatcher.Activity;

public class Activity
{
    public enum ActivityType : uint
    {
        General =       0,
        Login =         1,
        Logout =        2,
        Browser =       3,
        Application =   4,
        AFK =           5
    }

    public Activity()
    {
        Timestamp = DateTime.Now;
        ProcessName = string.Empty;
        ProcessExePath = string.Empty;

        string name = Environment.UserName;
        string domain = Environment.UserDomainName;
        UserName = $"{domain}\\{name}";
    }
    public virtual ActivityType Type
    { 
        get 
        {
            return ActivityType.General;
        } 
    }
    public DateTime Timestamp { get; }
    public string ProcessName { get; set; }
    public string ProcessExePath { get; set; }
    public string UserName { get; }

    public override string ToString()
    {
        return $"{Timestamp} {Type} {UserName} {ProcessName} {ProcessExePath}";
    }

    public virtual void SetProcessInfo(ProcessInfo aProcessInfo)
    {
        ProcessName = aProcessInfo.Name;
        ProcessExePath = aProcessInfo.ExePath;
    }
}
