namespace Matrix;

internal class PreviewForm : Form
{
    public PreviewForm(IntPtr parentHwnd)
    {
        NativeMethods.GetClientRect(parentHwnd, out NativeMethods.RECT rect);
        NativeMethods.SetParent(this.Handle, parentHwnd);

        this.FormBorderStyle = FormBorderStyle.None;
        this.Bounds = new Rectangle(0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top);

        int style = NativeMethods.GetWindowLong(this.Handle, NativeMethods.GWL_STYLE);
        NativeMethods.SetWindowLong(this.Handle, NativeMethods.GWL_STYLE, style | NativeMethods.WS_CHILD);

        this.BackColor = Color.Black;

        // Show a simple "Matrix" label in the preview - WebView2 is too heavy for the tiny preview window
        var label = new Label
        {
            Text = "Matrix",
            ForeColor = Color.FromArgb(99, 216, 93),
            Font = new Font("Consolas", 8f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.Black
        };
        this.Controls.Add(label);
    }
}
