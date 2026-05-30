using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;

namespace userWatcher.ActivityWatcher;

public partial class ProcessInfo
{
    // Initialize using C# 12 collection expressions
    private static HashSet<string> BrowsersNames = ["chrome", "edge", "firefox"];

    public bool IsBrowser
    {
        get 
        {
            return BrowsersNames.Contains(this.Name);
        }
    }

    public string GetCurrentURL()
    {
        AutomationElement element = AutomationElement.FromHandle(this.activeWindowHandle);
        if (element == null)
        {
            return string.Empty;
        }

        // Search for the Edit box (the address bar)
        // Chrome usually has the address bar as a descendant with ControlType.Edit
        var conditions = new AndCondition(
            new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit),
            new PropertyCondition(AutomationElement.IsValuePatternAvailableProperty, true)
        );

        AutomationElement editBox = element.FindFirst(TreeScope.Descendants, conditions);

        if (editBox != null)
        {
            ValuePattern val = (ValuePattern)editBox.GetCurrentPattern(ValuePattern.Pattern);
            return val.Current.Value; // This is the URL
        }
        return string.Empty;
    }

}
