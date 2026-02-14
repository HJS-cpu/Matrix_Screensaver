namespace Matrix;

internal class ConfigDialog : Form
{
    private ComboBox cmbVersion = null!;
    private CheckBox chkVolumetric = null!;
    private TrackBar trkSpeed = null!;
    private Label lblSpeedValue = null!;
    private TrackBar trkResolution = null!;
    private Label lblResolutionValue = null!;
    private NumericUpDown nudColumns = null!;
    private CheckBox chkSkipIntro = null!;

    public ConfigDialog()
    {
        this.Text = "Matrix - Einstellungen";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new Size(420, 380);
        this.BackColor = Color.FromArgb(30, 30, 30);
        this.ForeColor = Color.FromArgb(99, 216, 93);
        this.Font = new Font("Segoe UI", 9f);

        BuildUI();
        Settings.Load();
        LoadSettings();
    }

    private void BuildUI()
    {
        int y = 20;
        int controlX = 150;
        int controlWidth = 230;

        // Version
        AddLabel("Version:", 20, y);
        cmbVersion = new ComboBox
        {
            Location = new Point(controlX, y - 2),
            Size = new Size(controlWidth, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.FromArgb(99, 216, 93)
        };
        cmbVersion.Items.AddRange(new object[] {
            "Classic", "3D", "Megacity", "Operator", "Nightmare",
            "Paradise", "Resurrections", "Palimpsest", "Twilight",
            "Morpheus", "Bugs", "Trinity", "1999", "2003", "2021",
            "Throwback", "Updated"
        });
        this.Controls.Add(cmbVersion);
        y += 40;

        // Volumetric
        chkVolumetric = new CheckBox
        {
            Text = "Volumetrischer 3D-Effekt",
            Location = new Point(controlX, y),
            Size = new Size(controlWidth, 25),
            ForeColor = Color.FromArgb(99, 216, 93)
        };
        AddLabel("3D-Modus:", 20, y + 2);
        this.Controls.Add(chkVolumetric);
        y += 40;

        // Animation Speed
        AddLabel("Geschwindigkeit:", 20, y + 2);
        trkSpeed = new TrackBar
        {
            Location = new Point(controlX, y),
            Size = new Size(controlWidth - 40, 25),
            Minimum = 1,
            Maximum = 30,
            Value = 10,
            TickFrequency = 5,
            BackColor = Color.FromArgb(30, 30, 30)
        };
        lblSpeedValue = new Label
        {
            Location = new Point(controlX + controlWidth - 35, y + 2),
            Size = new Size(40, 20),
            Text = "1.0",
            ForeColor = Color.FromArgb(99, 216, 93)
        };
        trkSpeed.ValueChanged += (s, e) =>
        {
            lblSpeedValue.Text = (trkSpeed.Value / 10.0).ToString("F1");
        };
        this.Controls.Add(trkSpeed);
        this.Controls.Add(lblSpeedValue);
        y += 50;

        // Resolution
        AddLabel("Auflösung:", 20, y + 2);
        trkResolution = new TrackBar
        {
            Location = new Point(controlX, y),
            Size = new Size(controlWidth - 40, 25),
            Minimum = 25,
            Maximum = 100,
            Value = 100,
            TickFrequency = 25,
            BackColor = Color.FromArgb(30, 30, 30)
        };
        lblResolutionValue = new Label
        {
            Location = new Point(controlX + controlWidth - 35, y + 2),
            Size = new Size(40, 20),
            Text = "1.00",
            ForeColor = Color.FromArgb(99, 216, 93)
        };
        trkResolution.ValueChanged += (s, e) =>
        {
            lblResolutionValue.Text = (trkResolution.Value / 100.0).ToString("F2");
        };
        this.Controls.Add(trkResolution);
        this.Controls.Add(lblResolutionValue);
        y += 50;

        // Columns
        AddLabel("Spalten:", 20, y + 2);
        nudColumns = new NumericUpDown
        {
            Location = new Point(controlX, y),
            Size = new Size(80, 25),
            Minimum = 20,
            Maximum = 200,
            Value = 80,
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.FromArgb(99, 216, 93)
        };
        this.Controls.Add(nudColumns);
        y += 40;

        // Skip Intro
        chkSkipIntro = new CheckBox
        {
            Text = "Intro überspringen",
            Location = new Point(controlX, y),
            Size = new Size(controlWidth, 25),
            ForeColor = Color.FromArgb(99, 216, 93)
        };
        AddLabel("Intro:", 20, y + 2);
        this.Controls.Add(chkSkipIntro);
        y += 50;

        // Buttons
        var btnOk = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(this.ClientSize.Width - 190, y),
            Size = new Size(80, 30),
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.FromArgb(99, 216, 93),
            FlatStyle = FlatStyle.Flat
        };
        btnOk.Click += (s, e) => { SaveSettings(); this.Close(); };

        var btnCancel = new Button
        {
            Text = "Abbrechen",
            DialogResult = DialogResult.Cancel,
            Location = new Point(this.ClientSize.Width - 100, y),
            Size = new Size(80, 30),
            BackColor = Color.FromArgb(50, 50, 50),
            ForeColor = Color.FromArgb(99, 216, 93),
            FlatStyle = FlatStyle.Flat
        };

        var creditFont = new Font("Segoe UI", 8f);
        int linkGap = 2;

        var lblProjekt = new Label
        {
            Text = "Matrix Web-App by",
            Location = new Point(20, y + 2),
            AutoSize = true,
            ForeColor = Color.FromArgb(99, 216, 93),
            Font = creditFont
        };
        int rezmasonX = 20 + TextRenderer.MeasureText(lblProjekt.Text, creditFont).Width - 4 + linkGap;

        var lnkRezmason = new LinkLabel
        {
            Text = "Rezmason",
            Location = new Point(rezmasonX, y + 2),
            AutoSize = true,
            LinkColor = Color.FromArgb(130, 230, 130),
            ActiveLinkColor = Color.White,
            VisitedLinkColor = Color.FromArgb(130, 230, 130),
            Font = creditFont
        };
        lnkRezmason.LinkClicked += (s, e) =>
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/Rezmason/matrix",
                UseShellExecute = true
            });
        };

        var lblCooked = new Label
        {
            Text = "(C)ooked by",
            Location = new Point(20, y + 16),
            AutoSize = true,
            ForeColor = Color.FromArgb(99, 216, 93),
            Font = creditFont
        };
        int hjsX = 20 + TextRenderer.MeasureText(lblCooked.Text, creditFont).Width - 4 + linkGap;

        var lnkHJS = new LinkLabel
        {
            Text = "HJS",
            Location = new Point(hjsX, y + 16),
            AutoSize = true,
            LinkColor = Color.FromArgb(130, 230, 130),
            ActiveLinkColor = Color.White,
            VisitedLinkColor = Color.FromArgb(130, 230, 130),
            Font = creditFont
        };
        lnkHJS.LinkClicked += (s, e) =>
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/HJS-cpu/Matrix_Screensaver",
                UseShellExecute = true
            });
        };

        this.Controls.Add(lblProjekt);
        this.Controls.Add(lnkRezmason);
        this.Controls.Add(lblCooked);
        this.Controls.Add(lnkHJS);
        this.Controls.Add(btnOk);
        this.Controls.Add(btnCancel);
        this.AcceptButton = btnOk;
        this.CancelButton = btnCancel;
    }

    private Label AddLabel(string text, int x, int y)
    {
        var label = new Label
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(120, 20),
            ForeColor = Color.FromArgb(99, 216, 93)
        };
        this.Controls.Add(label);
        return label;
    }

    private void LoadSettings()
    {
        cmbVersion.SelectedItem = Settings.Version;
        if (cmbVersion.SelectedIndex < 0) cmbVersion.SelectedIndex = 0;
        chkVolumetric.Checked = Settings.Volumetric;
        trkSpeed.Value = Math.Clamp((int)(Settings.AnimationSpeed * 10), 1, 30);
        lblSpeedValue.Text = Settings.AnimationSpeed.ToString("F1");
        trkResolution.Value = Math.Clamp((int)(Settings.Resolution * 100), 25, 100);
        lblResolutionValue.Text = Settings.Resolution.ToString("F2");
        nudColumns.Value = Math.Clamp(Settings.NumColumns, 20, 200);
        chkSkipIntro.Checked = Settings.SkipIntro;
    }

    private void SaveSettings()
    {
        Settings.Version = cmbVersion.SelectedItem?.ToString() ?? "classic";
        Settings.Volumetric = chkVolumetric.Checked;
        Settings.AnimationSpeed = trkSpeed.Value / 10.0;
        Settings.Resolution = trkResolution.Value / 100.0;
        Settings.NumColumns = (int)nudColumns.Value;
        Settings.SkipIntro = chkSkipIntro.Checked;
        Settings.Save();
    }
}
