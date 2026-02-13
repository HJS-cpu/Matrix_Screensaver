using Microsoft.Win32;

namespace Matrix;

internal static class Settings
{
    private const string RegKey = @"SOFTWARE\MatrixScreensaver";

    public static string Version { get; set; } = "classic";
    public static bool Volumetric { get; set; } = true;
    public static double AnimationSpeed { get; set; } = 1.0;
    public static bool SkipIntro { get; set; } = true;
    public static double Resolution { get; set; } = 1.0;
    public static int NumColumns { get; set; } = 80;

    public static void Load()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegKey);
            if (key == null) return;

            Version = key.GetValue("Version", "classic")?.ToString() ?? "classic";
            Volumetric = (int)(key.GetValue("Volumetric", 1) ?? 1) == 1;
            if (double.TryParse(key.GetValue("AnimationSpeed", "1.0")?.ToString(), System.Globalization.CultureInfo.InvariantCulture, out double speed))
                AnimationSpeed = speed;
            SkipIntro = (int)(key.GetValue("SkipIntro", 1) ?? 1) == 1;
            if (double.TryParse(key.GetValue("Resolution", "1.0")?.ToString(), System.Globalization.CultureInfo.InvariantCulture, out double res))
                Resolution = res;
            if (int.TryParse(key.GetValue("NumColumns", "80")?.ToString(), out int cols))
                NumColumns = cols;
        }
        catch
        {
            // Use defaults on any error
        }
    }

    public static void Save()
    {
        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(RegKey);
            key.SetValue("Version", Version);
            key.SetValue("Volumetric", Volumetric ? 1 : 0);
            key.SetValue("AnimationSpeed", AnimationSpeed.ToString(System.Globalization.CultureInfo.InvariantCulture));
            key.SetValue("SkipIntro", SkipIntro ? 1 : 0);
            key.SetValue("Resolution", Resolution.ToString(System.Globalization.CultureInfo.InvariantCulture));
            key.SetValue("NumColumns", NumColumns);
        }
        catch
        {
            // Silently fail
        }
    }

    public static string BuildUrl()
    {
        var parts = new List<string>
        {
            $"version={Version}",
            $"volumetric={Volumetric.ToString().ToLower()}",
            $"animationSpeed={AnimationSpeed.ToString(System.Globalization.CultureInfo.InvariantCulture)}",
            $"skipIntro={SkipIntro.ToString().ToLower()}",
            $"resolution={Resolution.ToString(System.Globalization.CultureInfo.InvariantCulture)}",
            $"numColumns={NumColumns}",
            "suppressWarnings=true"
        };
        return "https://matrix.local/index.html?" + string.Join("&", parts);
    }
}
