namespace Matrix;

internal class PreviewForm : Form
{
    private readonly System.Windows.Forms.Timer timer;
    private readonly Random rng = new();
    private readonly int cols;
    private readonly int rows;
    private readonly int[] dropY;
    private readonly int[] trailLen;
    private readonly char[,] grid;
    private const int CellW = 7;
    private const int CellH = 10;
    private static readonly string Glyphs = "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲン0123456789";

    public PreviewForm(IntPtr parentHwnd)
    {
        NativeMethods.GetClientRect(parentHwnd, out NativeMethods.RECT rect);
        NativeMethods.SetParent(this.Handle, parentHwnd);

        this.FormBorderStyle = FormBorderStyle.None;
        this.Bounds = new Rectangle(0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top);

        int style = NativeMethods.GetWindowLong(this.Handle, NativeMethods.GWL_STYLE);
        NativeMethods.SetWindowLong(this.Handle, NativeMethods.GWL_STYLE, style | NativeMethods.WS_CHILD);

        this.BackColor = Color.Black;
        this.DoubleBuffered = true;

        cols = Math.Max(1, this.Width / CellW);
        rows = Math.Max(1, this.Height / CellH);
        grid = new char[cols, rows];
        dropY = new int[cols];
        trailLen = new int[cols];

        for (int c = 0; c < cols; c++)
            ResetDrop(c);

        // Fill grid with random chars
        for (int c = 0; c < cols; c++)
            for (int r = 0; r < rows; r++)
                grid[c, r] = RandomGlyph();

        timer = new System.Windows.Forms.Timer { Interval = 80 };
        timer.Tick += (s, e) => { Step(); Invalidate(); };
        timer.Start();
    }

    private char RandomGlyph() => Glyphs[rng.Next(Glyphs.Length)];

    private void ResetDrop(int col)
    {
        dropY[col] = rng.Next(-rows, 0);
        trailLen[col] = rng.Next(4, rows);
    }

    private void Step()
    {
        for (int c = 0; c < cols; c++)
        {
            dropY[c]++;
            if (dropY[c] - trailLen[c] > rows)
                ResetDrop(c);

            // Update the head character
            int head = dropY[c];
            if (head >= 0 && head < rows)
                grid[c, head] = RandomGlyph();

            // Randomly mutate a character in the trail
            if (rng.Next(5) == 0)
            {
                int r = rng.Next(rows);
                grid[c, r] = RandomGlyph();
            }
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Color.Black);

        using var font = new Font("Consolas", 6f);

        for (int c = 0; c < cols; c++)
        {
            int head = dropY[c];
            for (int r = 0; r < rows; r++)
            {
                int dist = head - r;
                if (dist < 0 || dist > trailLen[c])
                    continue;

                Color color;
                if (dist == 0)
                    color = Color.White;
                else if (dist == 1)
                    color = Color.FromArgb(180, 255, 180);
                else
                {
                    float fade = 1f - (float)dist / trailLen[c];
                    int brightness = (int)(200 * fade);
                    if (brightness < 20) continue;
                    color = Color.FromArgb(0, brightness, 0);
                }

                using var brush = new SolidBrush(color);
                g.DrawString(grid[c, r].ToString(), font, brush, c * CellW, r * CellH);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            timer?.Dispose();
        base.Dispose(disposing);
    }
}
