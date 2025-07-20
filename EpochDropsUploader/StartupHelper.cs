using Microsoft.Win32;
using System.Reflection;

public static class StartupHelper
{
    public static void AddToStartup(string appName, string exePath)
    {
        try
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rk == null) return;

            if (rk.GetValue(appName) == null)
            {
                rk.SetValue(appName, $"\"{exePath}\"");
            }
        }
        catch (Exception ex)
        {
            // You might want to log this
            Console.WriteLine("⚠️ Failed to set startup: " + ex.Message);
        }
    }
}
