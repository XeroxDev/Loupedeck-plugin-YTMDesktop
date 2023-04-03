namespace Loupedeck.YTMDesktopPlugin.GUI
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using Actions;

    using Services;

    using Point = System.Drawing.Point;

    public partial class Settings : Form
    {
        private readonly YTMDesktopPlugin _plugin;
        private Boolean _moving;
        private Point _offset;
        public static Boolean IsOpen;

        private Settings()
        {
            this._plugin = YTMDesktopPlugin.Instance;
            this.InitializeComponent();
            this.LoadSettings();
        }

        private void LoadSettings()
        {
            this._plugin.TryGetSetting("host", out var host);
            this._plugin.TryGetSetting("port", out var port);
            this._plugin.TryGetSetting("password", out var password);
            this._plugin.TryGetSetting("playPauseFormat", out var playPauseFormat);
            this.hostBox.Text = host ?? "127.0.0.1";
            this.portBox.Text = port ?? "9863";
            this.passwordBox.Text = password ?? "";
            this.textBox1.Text = playPauseFormat ?? "{current}";
            this.displayPreviewLabel.Text = PlayPauseCommand.FormatTitle(playPauseFormat, 91, 180, 89);
        }

        private void panel1_MouseDown(Object sender, MouseEventArgs e)
        {
            this._moving = true;
            this._offset = new Point(e.X, e.Y);
        }

        private void panel1_MouseMove(Object sender, MouseEventArgs e)
        {
            if (!this._moving)
            {
                return;
            }

            Point newlocation = this.Location;
            newlocation.X += e.X - this._offset.X;
            newlocation.Y += e.Y - this._offset.Y;
            this.Location = newlocation;
        }

        private void panel1_MouseUp(Object sender, MouseEventArgs e)
        {
            if (this._moving)
            {
                this._moving = false;
            }
        }

        private void portBox_KeyPress(Object sender, KeyPressEventArgs e)
        {
            e.Handled = !Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar);
        }

        public static void Open()
        {
            if (IsOpen)
            {
                return;
            }

            Thread s = new Thread(() =>
            {
                var settingsWindow = new Settings();
                settingsWindow.Show();
                // put in foreground
                settingsWindow.Activate();
                System.Windows.Threading.Dispatcher.Run();
            });

            s.SetApartmentState(ApartmentState.STA);
            s.Start();
        }

        private void saveButton_Click(Object sender, EventArgs e)
        {
            this._plugin.SetSetting("host", this.hostBox.Text);
            this._plugin.SetSetting("port", this.portBox.Text);
            this._plugin.SetSetting("password", this.passwordBox.Text);
            this._plugin.SetSetting("playPauseFormat", this.textBox1.Text);
            PlayPauseCommand.Format = this.textBox1.Text;
            SocketService.Instance.SetSettings(this.hostBox.Text, this.portBox.Text, this.passwordBox.Text)
                .GetAwaiter();
            this.Close();
        }

        private void Settings_Shown(Object sender, EventArgs e) => IsOpen = true;

        private void Settings_FormClosed(Object sender, FormClosedEventArgs e) => IsOpen = false;

        private void textBox1_TextChanged(Object sender, EventArgs e) => this.displayPreviewLabel.Text = PlayPauseCommand.FormatTitle(this.textBox1.Text, 91, 180, 89);
    }
}