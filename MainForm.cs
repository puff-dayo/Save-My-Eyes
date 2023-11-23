using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Save_My_Eyes
{
    public partial class MainForm : Form
    {
        private static MainForm _instance;

        private Form brightnessForm;
        private TrackBar brightnessTrackBar;
        public int PWMThreshold = 0;
        public int OverlayDarkness = 0;
        private OverlayForm overlayForm = null;

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle |= CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        public MainForm()
        {
            InitializeComponent();
            _instance = this;


            InitializeBrightnessForm();

            LoadSettings();

            label4.Text = "(Current: " + trackBar2.Value.ToString() + ")";
            label3.Text = "(Current: " + trackBar1.Value.ToString() + ")";

        }

        public static void ShowMainForm()
        {
            _instance?.Show();
        }

        private void Exit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private Label brightnessLabel;
        private Button closeButton;

        private void InitializeBrightnessForm()
        {
            brightnessForm = new Form()
            {
                Width = 200,
                Height = 100,
                StartPosition = FormStartPosition.Manual,
                FormBorderStyle = FormBorderStyle.None,
                TopMost = true,
                ShowInTaskbar = false
            };

            brightnessTrackBar = new TrackBar()
            {
                Orientation = Orientation.Horizontal,
                Dock = DockStyle.Top,
                Minimum = 0,
                Maximum = 100,
                Value = GetSystemBrightness(),
                TickStyle = TickStyle.None
            };
            brightnessTrackBar.Scroll += new EventHandler(BrightnessTrackBar_Scroll);

            brightnessLabel = new Label()
            {
                Dock = DockStyle.Top,
                Text = "Brightness: " + brightnessTrackBar.Value.ToString(),
                TextAlign = ContentAlignment.MiddleCenter
            };

            closeButton = new Button()
            {
                Dock = DockStyle.Bottom,
                Text = "Close"
            };
            closeButton.Click += new EventHandler(CloseButton_Click);

            brightnessForm.Controls.Add(brightnessTrackBar);
            brightnessForm.Controls.Add(brightnessLabel);
            brightnessForm.Controls.Add(closeButton);
        }

        private void BrightnessTrackBar_Scroll(object sender, EventArgs e)
        {
            if (brightnessTrackBar.Value < Properties.Settings.Default.PWMThreshold)
            {
                if (overlayForm == null || !overlayForm.Visible)
                {
                    overlayForm = new OverlayForm();
                    overlayForm.Show();
                }
                UpdateOverlayOpacity();
            }
            else
            {
                if (overlayForm != null)
                {
                    overlayForm.Hide();
                    overlayForm.Dispose();
                    overlayForm = null;
                }
                SetSystemBrightness(brightnessTrackBar.Value);
            }
            brightnessLabel.Text = "Brightness: " + brightnessTrackBar.Value.ToString();
        }


        private void UpdateOverlayOpacity()
        {
            if (overlayForm != null)
            {
                double opacity = 1.0 - (double)brightnessTrackBar.Value * Properties.Settings.Default.OverlayDarkness / 10000;

                opacity = Math.Max(0.0, Math.Min(1.0, opacity));

                overlayForm.Opacity = opacity;
            }
        }


        private void CloseButton_Click(object sender, EventArgs e)
        {
            brightnessForm.Hide();
        }

        private void ShowBrightnessControl()
        {
            if (brightnessForm.Visible)
            {
                brightnessForm.Hide();
            }
            else
            {
                Point cursorPosition = Cursor.Position;
                AdjustFormPosition(ref cursorPosition, brightnessForm.Width, brightnessForm.Height);

                brightnessForm.Location = cursorPosition;

                brightnessTrackBar.Value = GetSystemBrightness();
                brightnessForm.ShowDialog();
            }
        }

        private void AdjustFormPosition(ref Point position, int formWidth, int formHeight)
        {
            Rectangle screenBounds = Screen.FromPoint(position).Bounds;

            if (position.X + formWidth > screenBounds.Right)
            {
                position.X = screenBounds.Right - formWidth;
            }
            if (position.Y + formHeight > screenBounds.Bottom)
            {
                position.Y = screenBounds.Bottom - formHeight;
            }
        }

        private int GetSystemBrightness()
        {
            try
            {
                ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\WMI");
                ObjectQuery query = new ObjectQuery("SELECT * FROM WmiMonitorBrightness");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    foreach (ManagementObject m in searcher.Get().Cast<ManagementObject>())
                    {
                        return Convert.ToInt32(m["CurrentBrightness"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting brightness: " + ex.Message);
            }
            return 50;
        }



        private void SetSystemBrightness(int brightness)
        {
            try
            {
                ManagementScope scope = new ManagementScope("\\\\.\\ROOT\\WMI");
                ObjectQuery query = new ObjectQuery("SELECT * FROM WmiMonitorBrightnessMethods");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    foreach (ManagementObject m in searcher.Get().Cast<ManagementObject>())
                    {
                        m.InvokeMethod("WmiSetBrightness", new Object[] { UInt32.MaxValue, brightness });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error setting brightness: " + ex.Message);
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;

            if (me.Button == MouseButtons.Left)
            {
                ShowBrightnessControl();
            }
            else if (me.Button == MouseButtons.Right)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            Properties.Settings.Default["PWMThreshold"] = trackBar1.Value;
            Properties.Settings.Default["OverlayDarkness"] = trackBar2.Value;

            Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {
            trackBar1.Value = (int)Properties.Settings.Default["PWMThreshold"];
            trackBar2.Value = (int)Properties.Settings.Default["OverlayDarkness"];
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label3.Text = "(Current: " + trackBar1.Value.ToString() + ")";
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            label4.Text = "(Current: " + trackBar2.Value.ToString() + ")";
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowMainForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    public class OverlayForm : Form
    {
        public OverlayForm()
        {
            this.BackColor = Color.Black;
            this.Opacity = 0.5;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.AllowTransparency = true;

            // Handle mouse event bypass
            int exStyle = (int)GetWindowLong(this.Handle, GWL_EXSTYLE);
            exStyle |= WS_EX_TRANSPARENT | WS_EX_LAYERED;
            SetWindowLong(this.Handle, GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, IntPtr newStyle);
    }

}
