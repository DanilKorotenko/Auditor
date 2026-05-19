using Microsoft.Win32;

namespace common;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]

public class PreferencesManager
{
    private static readonly string REGISTRY_KEY = "Software\\DPSU\\AUDITOR";
    private static readonly string DEFAULT_SERVER = "<https:// dpsu auditor....>";

    private static readonly string SERVER_KEY = "server";
    private static readonly string DEPARTMENT_KEY = "department";
    private static readonly string FINGERPRINT_KEY = "fingerprint";
    private static readonly string AFK_TIMEOUT_KEY = "afk-timeout";

    private static readonly uint DEFAULT_AFK_TIMEOUT = 5 * 1000;

    public static string ServerURL()
    {
        string result = DEFAULT_SERVER;

        RegistryKey? key = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY);
        if (key != null)
        {
            string? setting = (string?)key.GetValue(SERVER_KEY);
            if (setting != null)
            {
                result = setting;
            }
            else
            {
                key.SetValue(SERVER_KEY, result);
            }
            key.Close();
        }
        else
        {
            key = Registry.LocalMachine.CreateSubKey(REGISTRY_KEY);
            if (key != null)
            {
                key.SetValue(SERVER_KEY, result);
                key.Close();
            }
        }
        return result;
    }

    public static string Department()
    {
        string result = "DPSU";

        RegistryKey? key = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY);
        if (key != null)
        {
            string? setting = (string?)key.GetValue(DEPARTMENT_KEY);
            if (setting != null)
            {
                result = setting;
            }
            key.Close();
        }
        return result;
    }

    private static string? fingerprint = null;
    public static string? Fingerprint
    {
        get
        {
            try
            {
                RegistryKey? key = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY);
                if (key != null)
                {
                    fingerprint = (string?)key.GetValue(FINGERPRINT_KEY);
                    key.Close();
                }
            }
            catch { }
            return fingerprint;
        }
        set
        {
            try
            {
                if (value != null)
                {
                    fingerprint = value;
                    RegistryKey? key = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY);
                    if (key != null)
                    {
                        key.SetValue(FINGERPRINT_KEY, fingerprint);
                        key.Close();
                    }
                    else
                    {
                        key = Registry.LocalMachine.CreateSubKey(REGISTRY_KEY);
                        if (key != null)
                        {
                            key.SetValue(FINGERPRINT_KEY, fingerprint);
                            key.Close();
                        }
                    }
                }
            }
            catch { }
        }
    }

    public static uint AfkTimeout
    {
        get
        {
            try
            {
                RegistryKey? key = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY);
                if (key != null)
                {
                    uint? result = (uint?)key.GetValue(AFK_TIMEOUT_KEY);
                    if (result != null)
                    {
                        return (uint)result;
                    }
                    key.Close();
                }
            }
            catch { }
            return DEFAULT_AFK_TIMEOUT;
        }
        set
        {
            try
            {
                RegistryKey? key = Registry.LocalMachine.OpenSubKey(REGISTRY_KEY);
                if (key != null)
                {
                    key.SetValue(FINGERPRINT_KEY, value);
                    key.Close();
                }
                else
                {
                    key = Registry.LocalMachine.CreateSubKey(REGISTRY_KEY);
                    if (key != null)
                    {
                        key.SetValue(FINGERPRINT_KEY, value);
                        key.Close();
                    }
                }
            }
            catch { }
        }
    }

}
