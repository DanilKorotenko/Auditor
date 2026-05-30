namespace userWatcher.ActivityWatcher.Activity;

public class AppActivity : Activity
{

    public AppActivity() : base()
    {
        ProcessName = string.Empty;
        ProcessExePath = string.Empty;
        WindowTitle = string.Empty;
    }
    public override ActivityType Type 
    { 
        get 
        {
            return ActivityType.Application;
        } 
    }

    public string ProcessName { get; set; }
    public string ProcessExePath { get; set; }
    public string WindowTitle { get; set; }

    public override string ToString()
    {
        return $"{Timestamp} {Type} {UserName} {ProcessName} {WindowTitle}";
    }

    public override void SetProcessInfo(ProcessInfo aProcessInfo)
    {
        base.SetProcessInfo(aProcessInfo);
        ProcessName = aProcessInfo.Name;
        ProcessExePath = aProcessInfo.ExePath;
        WindowTitle = aProcessInfo.WindowTitle;
    }
}
