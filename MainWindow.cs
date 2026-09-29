using System.Drawing.Drawing2D;

namespace OctoWoW_VF_Installer;

public sealed class MainWindow : Form
{
    readonly InstallerService service = new();
    readonly TextBox log = new();
    readonly ProgressBar progress = new();
    readonly Label status = new();
    readonly Label pathLabel = new();

    public MainWindow()
    {
        using (var iconStream = typeof(MainWindow).Assembly.GetManifestResourceStream("OctoWoW_VF_Installer.Resources.octowow-vf-icon.ico"))
            if (iconStream is not null) Icon = new Icon(iconStream);
        Text = "OctoWoW — Version française"; Width = 920; Height = 650; MinimumSize = new Size(760, 520);
        BackColor = Color.FromArgb(9, 16, 32); ForeColor = Color.White; StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle; MaximizeBox = false;
        var outer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(28), BackColor = Color.FromArgb(9, 16, 32) };
        Controls.Add(outer);
        var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(28), Radius = 18, BackColor = Color.FromArgb(20, 30, 53) };
        outer.Controls.Add(card);
        var title = new Label { Text = "OctoWoW — Version française", Dock = DockStyle.Top, Height = 52, Font = new Font("Segoe UI Semibold", 22), ForeColor = Color.FromArgb(240, 194, 85) };
        card.Controls.Add(title);
        var intro = new Label { Text = "Installe les textes et les voix françaises de WoW 1.12.1 sans écraser les patchs OctoWoW.", Dock = DockStyle.Top, Height = 38, Font = new Font("Segoe UI", 10), ForeColor = Color.FromArgb(200, 211, 231) };
        card.Controls.Add(intro);
        pathLabel.Text = "Client : détection automatique"; pathLabel.Dock = DockStyle.Top; pathLabel.Height = 30; pathLabel.ForeColor = Color.FromArgb(151, 178, 211);
        card.Controls.Add(pathLabel);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 98, WrapContents = true, Padding = new Padding(0, 12, 0, 8), BackColor = Color.Transparent };
        card.Controls.Add(actions);
        AddButton(actions, "Installer la VF", Color.FromArgb(37, 106, 176), async (_, _) => await Run(() => service.Install(Report)));
        AddButton(actions, "Désinstaller la VF", Color.FromArgb(113, 54, 62), async (_, _) => await Run(() => service.Uninstall(Report)));
        AddButton(actions, "Vérifier l’installation", Color.FromArgb(55, 79, 116), async (_, _) => await Run(() => service.Verify(Report)));
        AddButton(actions, "Lancer OctoWoW", Color.FromArgb(129, 91, 35), (_, _) => service.Launch(Report));
        AddButton(actions, "Quitter", Color.FromArgb(67, 73, 89), (_, _) => Close());
        progress.Dock = DockStyle.Top; progress.Height = 15; progress.Style = ProgressBarStyle.Continuous; progress.ForeColor = Color.FromArgb(240, 194, 85); card.Controls.Add(progress);
        status.Text = "Prêt"; status.Dock = DockStyle.Top; status.Height = 30; status.Padding = new Padding(0, 8, 0, 0); status.ForeColor = Color.FromArgb(240, 194, 85); card.Controls.Add(status);
        log.Multiline = true; log.ReadOnly = true; log.ScrollBars = ScrollBars.Vertical; log.Dock = DockStyle.Fill; log.BackColor = Color.FromArgb(8, 13, 25); log.ForeColor = Color.FromArgb(194, 209, 230); log.BorderStyle = BorderStyle.None; log.Font = new Font("Consolas", 9); card.Controls.Add(log);
    }

    void AddButton(Control parent, string text, Color color, EventHandler handler)
    {
        var b = new Button { Text = text, Width = 155, Height = 42, FlatStyle = FlatStyle.Flat, BackColor = color, ForeColor = Color.White, Font = new Font("Segoe UI Semibold", 9), Margin = new Padding(0, 0, 10, 8), Cursor = Cursors.Hand };
        b.FlatAppearance.BorderSize = 0; b.Click += handler; parent.Controls.Add(b);
    }
    async Task Run(Action action)
    { try { Toggle(false); await Task.Run(action); } catch (Exception ex) { Report("ERREUR : " + ex.Message); MessageBox.Show(this, ex.Message, "OctoWoW VF", MessageBoxButtons.OK, MessageBoxIcon.Error); } finally { Toggle(true); } }
    void Toggle(bool enabled) { foreach (Control c in Controls.OfType<Panel>().SelectMany(p => p.Controls.OfType<Panel>()).SelectMany(p => p.Controls.OfType<Control>())) if (c is Button b) b.Enabled = enabled; }
    void Report(string message) { if (InvokeRequired) { BeginInvoke(() => Report(message)); return; } log.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n"); status.Text = message; if (message.StartsWith("Progress:")) progress.Value = Math.Clamp(int.Parse(message[9..]), 0, 100); }
    sealed class RoundedPanel : Panel { public int Radius { get; set; } = 16; protected override void OnPaint(PaintEventArgs e) { using var path = new GraphicsPath(); var r = ClientRectangle; int d = Radius * 2; path.AddArc(r.X, r.Y, d, d, 180, 90); path.AddArc(r.Right-d, r.Y, d, d, 270, 90); path.AddArc(r.Right-d, r.Bottom-d, d, d, 0, 90); path.AddArc(r.X, r.Bottom-d, d, d, 90, 90); path.CloseFigure(); e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; using var brush = new SolidBrush(BackColor); e.Graphics.FillPath(brush, path); } }
}
