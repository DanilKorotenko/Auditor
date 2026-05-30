namespace userWatcher.ActivityWatcher.Activity;

public class AppActivity : Activity
{

    public AppActivity() : base()
    {
        WindowTitle = string.Empty;
    }
    public override ActivityType Type 
    { 
        get 
        {
            return ActivityType.Application;
        } 
    }

    public string WindowTitle { get; set; }

    public override string ToString()
    {
        return $"{Timestamp} {Type} {UserName} {ProcessName} {WindowTitle}";
    }

    public override void SetProcessInfo(ProcessInfo aProcessInfo)
    {
        base.SetProcessInfo(aProcessInfo);

        WindowTitle = aProcessInfo.WindowTitle;
    }
}
