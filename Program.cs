namespace Matrix;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

        string mode = "c";
        IntPtr hwnd = IntPtr.Zero;

        if (args.Length > 0)
        {
            string arg = args[0].ToLower().TrimStart('/').TrimStart('-');

            if (arg.StartsWith("s"))
            {
                mode = "s";
            }
            else if (arg.StartsWith("p"))
            {
                mode = "p";
                // /p HWND or /p:HWND
                string hwndStr = arg.Length > 1 && arg[1] == ':'
                    ? arg.Substring(2)
                    : (args.Length > 1 ? args[1] : "0");
                if (long.TryParse(hwndStr, out long h))
                    hwnd = new IntPtr(h);
            }
            else if (arg.StartsWith("c"))
            {
                mode = "c";
                if (arg.Contains(':') && long.TryParse(arg.Substring(arg.IndexOf(':') + 1), out long h))
                    hwnd = new IntPtr(h);
            }
        }

        switch (mode)
        {
            case "s":
                ShowScreensaver();
                break;
            case "p":
                ShowPreview(hwnd);
                break;
            case "c":
            default:
                ShowConfig();
                break;
        }
    }

    static void ShowScreensaver()
    {
        foreach (Screen screen in Screen.AllScreens)
        {
            var form = new ScreensaverForm(screen);
            form.Show();
        }
        Application.Run();
    }

    static void ShowPreview(IntPtr hwnd)
    {
        // Preview in the small Windows dialog - just show a black window
        // WebView2 in a tiny child window is unreliable, so we skip it
        if (hwnd != IntPtr.Zero)
        {
            var form = new PreviewForm(hwnd);
            Application.Run(form);
        }
    }

    static void ShowConfig()
    {
        var dialog = new ConfigDialog();
        dialog.ShowDialog();
    }
}
