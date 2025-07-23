namespace EpochDropsUploader;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            File.WriteAllText("crash.log", args.ExceptionObject.ToString());
            MessageBox.Show("Unhandled exception occurred. See crash.log.");
        };

        try
        {
            Application.Run(new Form1());
        }
        catch (Exception ex)
        {
            File.WriteAllText("fatal.log", ex.ToString());
            MessageBox.Show("Fatal error. See fatal.log.");
        }
    }
}