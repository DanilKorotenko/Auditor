using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;
using System.Management;

class Program
{
    // 1. Import the necessary Windows API functions
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


    static void Main()
    {
        // This is the magic line
        Console.OutputEncoding = Encoding.UTF8;

        //string chromeURL = GetChromeUrl();

        //Console.WriteLine(chromeURL);

        while (true)
        {
            Update();
            System.Threading.Thread.Sleep(500); // Check every half-second
        }
    }

    public static void Update()
    {
        // Get the handle (IntPtr) of the foreground window
        IntPtr handle = GetForegroundWindow();

        if (handle == IntPtr.Zero)
        {
            return;
        }

        string title = GetWindowTitle(handle);
        Console.WriteLine($"Window title: {title}");

        Process? parentProcess = GetParentProcessForWindow(handle);

        if (parentProcess == null)
        {
            Console.WriteLine($"process: null");
            return;
        }

        Console.WriteLine($"process: {parentProcess.ProcessName}");

        if (IsBrowser(parentProcess))
        {
            string? url = GetChromeWindowUrl(handle);
            if (url != null)
            {
                Console.WriteLine(url);
            }
        }

        Console.WriteLine();
    }

    public static string GetWindowTitle(IntPtr aWindowHandle)
    {

        // Get the length of the title
        int length = GetWindowTextLength(aWindowHandle);
        if (length == 0) 
        { 
            return "No Title or Hidden Window"; 
        }

        // Create a buffer to hold the title
        StringBuilder builder = new StringBuilder(length + 1);

        // Fill the buffer with the window text
        GetWindowText(aWindowHandle, builder, builder.Capacity);

        string result = builder.ToString();

        return result;
    }

    public static Process? GetParentProcessForWindow(IntPtr aWindowHandle)
    {
        GetWindowThreadProcessId(aWindowHandle, out uint pid);

        Process? parentProc = null;

        if (pid != 0)
        {
            try
            {
                parentProc = Process.GetProcessById((int)pid);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Parent process has already exited: {ex.Message}");
            }
        }
        return parentProc;
    }

    public static bool IsBrowser(Process aProcess)
    {
        return IsChrome(aProcess) || IsEdge(aProcess);
    }

    public static bool IsChrome(Process aProcess)
    {
        return string.Equals(aProcess.ProcessName, "chrome");
    }

    public static bool IsEdge(Process aProcess)
    {
        return string.Equals(aProcess.ProcessName, "msedge");
    }

    public static string? GetChromeWindowUrl(IntPtr aChromeWindowHandle)
    {
        AutomationElement element = AutomationElement.FromHandle(aChromeWindowHandle);
        if (element == null)
        {
            return null;
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
        return "URL not found";
    }

}
