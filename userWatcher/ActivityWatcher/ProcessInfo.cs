using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace userWatcher.ActivityWatcher;

public partial class ProcessInfo
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();
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
