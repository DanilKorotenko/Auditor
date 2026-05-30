using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace userWatcher.ActivityWatcher;

public partial class ProcessInfo
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    public static ProcessInfo? GetCurrentProcessInfo()
    {
        IntPtr handle = GetForegroundWindow();
        if (handle == IntPtr.Zero)
        {
            return null;
        }

        ProcessInfo result = new ProcessInfo(handle);

        return result;
    }

    private ProcessInfo(IntPtr anActiveWindowHandle) 
    {
        activeWindowHandle = anActiveWindowHandle;
    }

    private IntPtr activeWindowHandle { get; }

    private string? windowTitle = null;
    private string WindowTitle
    {
        get
        {
            if (windowTitle == null)
            {
                // Get the length of the title
                int length = GetWindowTextLength(this.activeWindowHandle);
                if (length == 0)
                {
                    windowTitle = "No Title or Hidden Window";
                }

                // Create a buffer to hold the title
                StringBuilder builder = new StringBuilder(length + 1);

                // Fill the buffer with the window text
                GetWindowText(this.activeWindowHandle, builder, builder.Capacity);

                windowTitle = builder.ToString();
            }
            return windowTitle;
        }
    }

    private Process? parentProcess = null;
    private Process? ParentProcess
    {
        get 
        {
            if (parentProcess == null)
            {
                GetWindowThreadProcessId(this.activeWindowHandle, out uint pid);
                if (pid != 0)
                {
                    try
                    {
                        parentProcess = Process.GetProcessById((int)pid);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Parent process has already exited: {ex.Message}");
                    }
                }
            }
            return parentProcess;
        }
    }

    public string Name
    {
        get 
        {
            Process? process = ParentProcess;
            if (process != null)
            {
                return process.ProcessName;
            }
            return string.Empty;
        }
    }

    public string ExePath
    {
        get 
        {
            Process? process = ParentProcess;
            if (process != null)
            {
                ProcessModule? module = process.MainModule;
                if (module != null)
                {
                    string? exePath = module.FileName;
                    if (exePath != null)
                    {
                        return exePath;
                    }
                }
            }
            return string.Empty;
        }
    }
}
