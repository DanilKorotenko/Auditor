using System;
using System.Collections.Generic;
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

        Dictionary<string, string> windowText = GetAllAvailableWindowText(handle);
        foreach (KeyValuePair<string, string> entry in windowText)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
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

    public static Dictionary<string, string> GetAllAvailableWindowText(IntPtr aWindowHandle)
    {
        Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        AutomationElement rootElement;
        try
        {
            rootElement = AutomationElement.FromHandle(aWindowHandle);
        }
        catch
        {
            return result;
        }

        if (rootElement == null)
        {
            return result;
        }

        AddElementTextToDictionary(rootElement, result);

        AutomationElementCollection descendants;
        try
        {
            descendants = rootElement.FindAll(TreeScope.Descendants, Condition.TrueCondition);
        }
        catch
        {
            return result;
        }

        foreach (AutomationElement element in descendants)
        {
            AddElementTextToDictionary(element, result);
        }

        return result;
    }

    private static void AddElementTextToDictionary(AutomationElement aElement, Dictionary<string, string> aDictionary)
    {
        try
        {
            string name = GetElementName(aElement);
            string? value = GetElementValue(aElement);

            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = $"Element_{aDictionary.Count + 1}";
            }

            AddOrUpdateUniqueKey(aDictionary, name, value);
        }
        catch
        {
            // Some elements can disappear while traversing UI Automation tree.
        }
    }

    private static string GetElementName(AutomationElement aElement)
    {
        string name = (aElement.Current.Name ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        string automationId = (aElement.Current.AutomationId ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(automationId))
        {
            return automationId;
        }

        return aElement.Current.ControlType.ProgrammaticName;
    }

    private static string? GetElementValue(AutomationElement aElement)
    {
        if (aElement.TryGetCurrentPattern(ValuePattern.Pattern, out object valuePatternObj))
        {
            ValuePattern valuePattern = (ValuePattern)valuePatternObj;
            string currentValue = (valuePattern.Current.Value ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(currentValue))
            {
                return currentValue;
            }
        }

        if (aElement.TryGetCurrentPattern(TextPattern.Pattern, out object textPatternObj))
        {
            TextPattern textPattern = (TextPattern)textPatternObj;
            string currentText = (textPattern.DocumentRange.GetText(-1) ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(currentText))
            {
                return currentText;
            }
        }

        string fallback = (aElement.Current.Name ?? string.Empty).Trim();
        return string.IsNullOrWhiteSpace(fallback) ? null : fallback;
    }

    private static void AddOrUpdateUniqueKey(Dictionary<string, string> aDictionary, string aName, string aValue)
    {
        if (!aDictionary.TryGetValue(aName, out string? existingValue))
        {
            aDictionary[aName] = aValue;
            return;
        }

        if (string.Equals(existingValue, aValue, StringComparison.Ordinal))
        {
            return;
        }

        int suffix = 2;
        string candidateKey = $"{aName}_{suffix}";
        while (aDictionary.ContainsKey(candidateKey))
        {
            suffix++;
            candidateKey = $"{aName}_{suffix}";
        }

        aDictionary[candidateKey] = aValue;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    [DllImport("user32.dll")]
    static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    public static uint GetIdleTime()
    {
        LASTINPUTINFO lastInput = new LASTINPUTINFO();
        lastInput.cbSize = (uint)Marshal.SizeOf(lastInput);
        GetLastInputInfo(ref lastInput);

        // Environment.TickCount is the time since the system started
        return (uint)Environment.TickCount - lastInput.dwTime; // Returns idle time in milliseconds
    }


}
