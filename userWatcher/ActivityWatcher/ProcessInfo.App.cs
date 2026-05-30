using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace userWatcher.ActivityWatcher;

public partial class ProcessInfo
{

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    private string? windowTitle = null;
    public string WindowTitle
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
}
