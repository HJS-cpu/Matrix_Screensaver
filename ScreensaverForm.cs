using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Matrix;

internal class ScreensaverForm : Form
{
    private WebView2? webView;
    private static IntPtr keyboardHookId = IntPtr.Zero;
    private static IntPtr mouseHookId = IntPtr.Zero;
    private static NativeMethods.LowLevelHookProc? keyboardProc;
    private static NativeMethods.LowLevelHookProc? mouseProc;
    private static NativeMethods.POINT? initialMousePos;
    private const int MOUSE_MOVE_THRESHOLD = 20;
    private static bool hooksInstalled = false;
    private static bool hooksActive = false;
    private static readonly Stopwatch startupTimer = new();

    public ScreensaverForm(Screen screen)
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Normal;
        this.StartPosition = FormStartPosition.Manual;
        this.Bounds = screen.Bounds;
        this.TopMost = true;
        this.ShowInTaskbar = false;
        this.BackColor = Color.Black;
        this.DoubleBuffered = true;

        Cursor.Hide();

        this.Shown += async (s, e) =>
        {
            try
            {
                await InitializeWebView();

                // Install hooks with a grace period after the form is shown and WebView loaded
                if (!hooksInstalled)
                {
                    startupTimer.Start();
                    InstallHooks();
                    hooksInstalled = true;

                    // Activate hooks after 2 seconds to ignore initial mouse jitter
                    var timer = new System.Windows.Forms.Timer { Interval = 2000 };
                    timer.Tick += (_, _) =>
                    {
                        hooksActive = true;
                        timer.Stop();
                        timer.Dispose();
                    };
                    timer.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Starten des Screensavers:\n\n{ex.Message}\n\n{ex.StackTrace}",
                    "Matrix - Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        };
    }

    private async Task InitializeWebView()
    {
        webView = new WebView2();
        webView.Dock = DockStyle.Fill;
        webView.DefaultBackgroundColor = Color.Black;
        this.Controls.Add(webView);

        var userDataFolder = Path.Combine(Path.GetTempPath(), "Matrix");
        var env = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
        await webView.EnsureCoreWebView2Async(env);

        var settings = webView.CoreWebView2.Settings;
        settings.AreDefaultContextMenusEnabled = false;
        settings.AreDevToolsEnabled = false;
        settings.IsStatusBarEnabled = false;
        settings.IsZoomControlEnabled = false;
        settings.AreBrowserAcceleratorKeysEnabled = false;

        string webRoot = GetWebAssetsPath();
        webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
            "matrix.local",
            webRoot,
            CoreWebView2HostResourceAccessKind.Allow
        );

        Settings.Load();
        string url = Settings.BuildUrl();
        webView.CoreWebView2.Navigate(url);
    }

    private static string GetWebAssetsPath()
    {
        // Try next to the executable
        string? exePath = Process.GetCurrentProcess().MainModule?.FileName;
        if (exePath != null)
        {
            string exeDir = Path.GetDirectoryName(exePath)!;
            string webDir = Path.Combine(exeDir, "web");
            if (Directory.Exists(webDir))
                return webDir;
        }

        // Try next to the entry assembly (for dotnet run scenarios)
        string? assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        if (!string.IsNullOrEmpty(assemblyPath))
        {
            string asmDir = Path.GetDirectoryName(assemblyPath)!;
            string webDir = Path.Combine(asmDir, "web");
            if (Directory.Exists(webDir))
                return webDir;
        }

        // Try the current working directory
        string cwdWeb = Path.Combine(Environment.CurrentDirectory, "web");
        if (Directory.Exists(cwdWeb))
            return cwdWeb;

        // Fallback: ProgramData
        string pdDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Matrix", "web"
        );
        if (Directory.Exists(pdDir))
            return pdDir;

        throw new DirectoryNotFoundException(
            $"Matrix web assets not found.\n" +
            $"Searched:\n" +
            $"  - {Path.Combine(Path.GetDirectoryName(exePath ?? "?") ?? "?", "web")}\n" +
            $"  - {cwdWeb}\n" +
            $"  - {pdDir}");
    }

    private static void InstallHooks()
    {
        keyboardProc = KeyboardHookCallback;
        mouseProc = MouseHookCallback;

        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule!;
        IntPtr moduleHandle = NativeMethods.GetModuleHandle(curModule.ModuleName);

        keyboardHookId = NativeMethods.SetWindowsHookEx(
            NativeMethods.WH_KEYBOARD_LL, keyboardProc, moduleHandle, 0);
        mouseHookId = NativeMethods.SetWindowsHookEx(
            NativeMethods.WH_MOUSE_LL, mouseProc, moduleHandle, 0);
    }

    private static void UninstallHooks()
    {
        if (keyboardHookId != IntPtr.Zero)
        {
            NativeMethods.UnhookWindowsHookEx(keyboardHookId);
            keyboardHookId = IntPtr.Zero;
        }
        if (mouseHookId != IntPtr.Zero)
        {
            NativeMethods.UnhookWindowsHookEx(mouseHookId);
            mouseHookId = IntPtr.Zero;
        }
        hooksInstalled = false;
        hooksActive = false;
    }

    private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && hooksActive && wParam == (IntPtr)NativeMethods.WM_KEYDOWN)
        {
            ExitScreensaver();
        }
        return NativeMethods.CallNextHookEx(keyboardHookId, nCode, wParam, lParam);
    }

    private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && hooksActive)
        {
            if (wParam == (IntPtr)NativeMethods.WM_LBUTTONDOWN ||
                wParam == (IntPtr)NativeMethods.WM_RBUTTONDOWN ||
                wParam == (IntPtr)NativeMethods.WM_MBUTTONDOWN)
            {
                ExitScreensaver();
            }
            else if (wParam == (IntPtr)NativeMethods.WM_MOUSEMOVE)
            {
                var hookStruct = Marshal.PtrToStructure<NativeMethods.MSLLHOOKSTRUCT>(lParam);
                if (initialMousePos == null)
                {
                    initialMousePos = hookStruct.pt;
                }
                else
                {
                    int dx = Math.Abs(hookStruct.pt.x - initialMousePos.Value.x);
                    int dy = Math.Abs(hookStruct.pt.y - initialMousePos.Value.y);
                    if (dx > MOUSE_MOVE_THRESHOLD || dy > MOUSE_MOVE_THRESHOLD)
                    {
                        ExitScreensaver();
                    }
                }
            }
        }
        return NativeMethods.CallNextHookEx(mouseHookId, nCode, wParam, lParam);
    }

    private static void ExitScreensaver()
    {
        UninstallHooks();
        Application.Exit();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        webView?.Dispose();
        base.OnFormClosing(e);
    }
}
