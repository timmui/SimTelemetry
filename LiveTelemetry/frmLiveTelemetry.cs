﻿/*************************************************************************
 *                         SimTelemetry                                  *
 *        providing live telemetry read-out for simulators               *
 *             Copyright (C) 2011-2012 Hans de Jong                      *
 *                                                                       *
 *  This program is free software: you can redistribute it and/or modify *
 *  it under the terms of the GNU General Public License as published by *
 *  the Free Software Foundation, either version 3 of the License, or    *
 *  (at your option) any later version.                                  *
 *                                                                       *
 *  This program is distributed in the hope that it will be useful,      *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 *  GNU General Public License for more details.                         *
 *                                                                       *
 *  You should have received a copy of the GNU General Public License    *
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.*
 *                                                                       *
 * Source code only available at https://github.com/nlhans/SimTelemetry/ *
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LiveTelemetry.Gauges;
using LiveTelemetry.UI;
using SimTelemetry;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Plugins;
using Triton;
using Triton.Joysticks;
using System.Globalization;

namespace LiveTelemetry
{
    public partial class frmLiveTelemetry : Form
    {
        private static frmLiveTelemetry _frmLiveTelemetry;
        
        // Joystick data for cycling through panels.
        private Joystick Joystick_Instance;
        public static int Joystick_Button;

        // Interface update timers.
        // See end of init function for speed settings.
        private Timer Tmr_HiSpeed; 
        private Timer Tmr_MdSpeed;
        private Timer Tmr_LwSpeed;

        // User interface controls.
        private Button btGarage;
        private Button btSettings;
        private Button btNetwork;

        private LapChart ucLapChart;
        private LiveTrackMap ucTrackmap;
        private UcGaugeA1Gp ucA1GP;
        private Gauge_Tyres ucTyres;
        private Gauge_Laps ucLaps;
        private Gauge_Splits ucSplits;
        private ucSessionInfo ucSessionData;

        /// <summary>
        /// Gets or sets the current menu panel it's in (0-3). If set the interface will be updated.
        /// Static property allows it to be changed from external sources as well.
        /// </summary>
        public static int StatusMenu
        {
            get { return _statusMenu; }
            set
            {
                _statusMenu = value;
                _frmLiveTelemetry.SetStatusControls(null, null);
            }
        }
        private static int _statusMenu;

        /// <summary>
        /// Initializes for the window. This setups the data back-end framework, as well searches for joystick
        /// configuration. Set-ups interface controls.
        /// </summary>
        public frmLiveTelemetry()
        {
            // Use EN-US for compatibility with functions as Convert.ToDouble etc.
            // This is mainly used within track parsers.
            Application.CurrentCulture = new CultureInfo("en-US");
            
            // Boot up the Telemetry Domain
            TelemetryApplication.Init();

            GlobalEvents.Hook<SessionStarted>(mUpdateUI, true);
            GlobalEvents.Hook<SessionStopped>(mUpdateUI, true);
            GlobalEvents.Hook<SimulatorStarted>(mUpdateUI, true);
            GlobalEvents.Hook<SimulatorStopped>(mUpdateUI, true);

            // TODO: Detect hardware devices (COM-ports or USB devices)

            // Form of singleton for public StatusMenu access.
            _frmLiveTelemetry = this;

            // Read joystick configuration.
            if(File.Exists("config.txt") == false)
            {
                // TODO: Needs fancy dialogs to first-time setup.
                File.Create("config.txt");
                MessageBox.Show(
                    "Please edit config.txt:\r\ncontroller=[your controller you want to use for cyclign through panels]\r\nbutton=[the button on the controller you want to use]\r\n\r\nJust type the name of your controller (G25 alone is enough usually) and look up the button in Devices -> Game Controllers.");
            }else
            {
                string[] lines = File.ReadAllLines("config.txt");
                string controller = "";
                bool controlleruseindex = false;
                int controllerindex = 0;
                foreach (string line in lines)
                {
                    string[] p = line.Trim().Split("=".ToCharArray());
                    if (line.StartsWith("button")) Joystick_Button = Convert.ToInt32(p[1]);
                    if (line.StartsWith("index"))
                    {
                        controlleruseindex = true;
                        controllerindex = Convert.ToInt32(p[1]);
                    }
                    if (line.StartsWith("controller"))
                        controller = p[1];


                }

                // Search for the joystick.
                List<JoystickDevice> devices = JoystickDevice.Search();
                if (devices.Count == 0)
                {
                    //MessageBox.Show("No (connected) joystick found for display panel control.\r\nTo utilize this please connect a joystick, configure and restart this program.");
                }
                else
                {
                    if (controlleruseindex)
                    {
                        Joystick_Instance = new Joystick(devices[controllerindex]);
                        Joystick_Instance.Release += Joy_Release;
                    }
                    else
                    {
                        int i = 0;
                        foreach (JoystickDevice jd in devices)
                        {
                            if (jd.Name.Contains(controller.Trim()))
                            {
                                Joystick_Instance = new Joystick(jd);
                                Joystick_Instance.Release += Joy_Release;
                            }
                            i++;
                        }
                    }
                }
            }

            // Set-up the main interface.
            FormClosing += LiveTelemetry_FormClosing;
            SizeChanged += Telemetry_ResizePanels;

            InitializeComponent();

            SuspendLayout();
            BackColor = Color.Black;
            ucLaps = new Gauge_Laps();
            ucSplits = new Gauge_Splits();
            ucA1GP = new UcGaugeA1Gp(Joystick_Instance);
            ucTyres = new Gauge_Tyres();
            ucSessionData = new ucSessionInfo();
            ucTrackmap = new LiveTrackMap();
            ucLapChart = new LapChart();
            btGarage = new Button();
            btNetwork = new Button();
            btSettings = new Button();

            // Garage button
            btGarage.Text = "Garage";
            btGarage.Size = new Size(75, 25);
            btGarage.BackColor = Color.White;
            btGarage.Location = new Point(10, 10);
            btGarage.Click += new EventHandler(btGarage_Click);

            // Settings button
            btSettings.Text = "Settings";
            btSettings.Size = new Size(75, 25);
            btSettings.BackColor = Color.White;
            btSettings.Location = new Point(95, 10);
            btSettings.Click += new EventHandler(btSettings_Click);

            // Network
            btNetwork.Text = "Network";
            btNetwork.Size = new Size(75, 25);
            btNetwork.BackColor = Color.White;
            btNetwork.Location = new Point(180, 10);
            btNetwork.Click += new EventHandler(btNetwork_Click);

            // Timers
            Tmr_HiSpeed = new Timer{Interval=20}; // 30fps
            Tmr_MdSpeed = new Timer{Interval = 450}; // ~2fps
            Tmr_LwSpeed = new Timer{Interval=1000}; // 1fps

            Tmr_HiSpeed.Tick += Tmr_HiSpeed_Tick;
            Tmr_MdSpeed.Tick += Tmr_MdSpeed_Tick;
            Tmr_LwSpeed.Tick += Tmr_LwSpeed_Tick;

            Tmr_HiSpeed.Start();
            Tmr_MdSpeed.Start();
            Tmr_LwSpeed.Start();

            System.Threading.Thread.Sleep(500);

            SetupUI();
            ResumeLayout(false);
        }

        private void btNetwork_Click(object sender, EventArgs e)
        {
            fNetwork ntwk = new fNetwork();
            ntwk.ShowDialog();
        }

        private void btSettings_Click(object sender, EventArgs e)
        {
            // TODO: Need to create this panel
        }

        private void btGarage_Click(object sender, EventArgs e)
        {
            fGarage g = new fGarage();
            g.Show();
        }

        /// <summary>
        /// Reinitializes the panel changable with user defined button/joystick. The function invokes execution
        /// to the main interface thread. 
        /// 0: general data/wear A1GP Style (Default)
        /// 1: Tyre temperature, wear and brakes.
        /// 2: Lap times list
        /// 3: Split times.
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">EventArgs of event</param>
        internal void SetStatusControls(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(SetStatusControls), new object[2] { sender, e });
                return;
            }
            if (Controls.Contains(ucA1GP))
                Controls.Remove(ucA1GP);
            if (Controls.Contains(ucTyres))
                Controls.Remove(ucTyres);
            if (Controls.Contains(ucLaps))
                Controls.Remove(ucLaps);
            if (Controls.Contains(ucSplits))
                Controls.Remove(ucSplits);

            switch (StatusMenu)
            {
                case 0:
                    Controls.Add(ucA1GP);
                    break;
                case 1:
                    Controls.Add(ucTyres);
                    break;
                case 2:
                    Controls.Add(ucLaps);
                    break;
                case 3:
                    Controls.Add(ucSplits);
                    break;
                default:
                    StatusMenu = 0;
                    SetStatusControls(null, null);
                    break;
            }

        }

        /// <summary>
        /// Completely re updates user interfaces upon sim start/stop or session start/stop.
        /// It will call SetupUI(true) in the windows UI context.
        /// </summary>
        /// <param name="sender">Parameter fed from anonymous signal. Unused</param>
        private void mUpdateUI(object sender)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Signal(mUpdateUI), new object[1] { sender });
                return;
            }
            SetupUI();
        }

        /// <summary>
        /// Completely redraws the user interface. It will bring this window into 3 modes:
        /// A) Waiting for simulator.
        /// B) Waiting for session.
        /// C) Telemetry window.
        /// 
        /// The simulator panel displays all installed modules of simulators (if an image is found).
        /// The session panel displays the full-size simulator image (if exists) with "waiting for session".
        /// The telemetry window adds the controls track map, lap chart, session status. It initializes panel sizes
        /// via Telemetry_ResizePanels and sets the user panel via SetStatusControls.
        /// </summary>
        private void SetupUI()
        {
            if (TelemetryApplication.SessionAvailable)
            {
                this.Controls.Clear();
                this.Padding = new Padding(0);

                Controls.Add(ucTrackmap);
                Controls.Add(ucLapChart);
                Controls.Add(ucSessionData);
                //Controls.Add(FuelData);
                Telemetry_ResizePanels(null, null);
                SetStatusControls(null, null);

            }
            else if (TelemetryApplication.SimulatorAvailable)
            {
                this.Controls.Clear();
                this.Padding = new Padding(0);
                // draw sim pic.

                Label t = new Label { Text = "Waiting for session" };
                t.Font = new Font("Arial", 32.0f, FontStyle.Italic | FontStyle.Bold);
                t.ForeColor = Color.White;
                t.Size = new Size(this.Size.Width - 80, 60);
                t.TextAlign = ContentAlignment.MiddleCenter;

                FlowLayoutPanel panel = new FlowLayoutPanel();

                if (File.Exists("Simulators/" + TelemetryApplication.Simulator.Name + ".png") && this.Width > 80 && this.Height > 100)
                {
                    ucResizableImage pb = new ucResizableImage("Simulators/" + TelemetryApplication.Simulator.Name + ".png");
                    pb.Crop(Size.Width - 80, Size.Height - 70);
                    panel.Controls.Add(pb);
                    panel.Location = new Point(40, (Height - pb.Size.Height - 60) / 2);
                    panel.Size = new Size(Width - 80, 60 + pb.Size.Height);

                }
                else
                {
                    panel.Size = new Size(Width, 60);
                    panel.Location = new Point(40, (Height - 60) / 2);
                }
                panel.Controls.Add(t);

                Controls.Add(panel);

            }
            else
            {
                Controls.Clear();

                // draw sim gallery
                FlowLayoutPanel panel = new FlowLayoutPanel();
                this.Padding = new Padding(35);

                int columns = (int)Math.Ceiling(Math.Sqrt(TelemetryApplication.Plugins.Simulators.Count));
                if (columns == 0) columns = 1;
                if (TelemetryApplication.Plugins.Simulators.Count % columns == 1)
                    columns++;
                if (this.Width > 233)
                {
                    while (233 * columns > this.Width)
                        columns--;
                }
                int rows = (int)Math.Ceiling(TelemetryApplication.Plugins.Simulators.Count * 1.0 / columns) + 1;

                panel.Size = new Size(233 * columns, rows * 140);
                panel.Location = new Point((this.Width - panel.Size.Width) / 2, (this.Height - panel.Size.Height) / 2);

                foreach (IPluginSimulator sim in TelemetryApplication.Plugins.Simulators)
                {
                    if (File.Exists("Simulators/" + sim.Name + ".png"))
                    {
                        ucResizableImage pb = new ucResizableImage("Simulators/" + sim.Name + ".png");
                        pb.Margin = new Padding(10);
                        pb.Crop(213, 120);
                        panel.Controls.Add(pb);
                    }
                }

                Label t = new Label { Text = "Waiting for simulator" };
                t.Font = new Font("Arial", 32.0f, FontStyle.Italic | FontStyle.Bold);
                t.ForeColor = Color.White;
                t.Size = new Size(panel.Size.Width, 50);
                t.TextAlign = ContentAlignment.MiddleCenter;
                panel.Controls.Add(t);

                Controls.Add(panel);
            }

            if (btGarage != null)
            {
                Controls.Add(btGarage);
                Controls.Add(btSettings);
                Controls.Add(btNetwork);

#if DEBUG
                Button dataDebug = new Button
                                       {Text = "Data debug", Size = new Size(75, 25), Location = new Point(280, 10), BackColor= Color.Red};
                dataDebug.Click += (s, e) =>
                                       {
                                           Data d = new Data();
                                           d.Show();
                                       };
                Controls.Add(dataDebug);
#endif
            }
        }

        /// <summary>
        /// Close the Triton framework properly. This stops all services and what not is running.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LiveTelemetry_FormClosing(object sender, FormClosingEventArgs e)
        { 
            TritonBase.TriggerExit();
        }

        /// <summary>
        /// This method listens for joystick button releases and updates the StatusMenu property.
        /// This will in turn update the user interface.
        /// </summary>
        /// <param name="joystick">Joystick class from which the event was wired.</param>
        /// <param name="button">The button that was released.</param>
        private void Joy_Release(Joystick joystick, int button)
        {
            if (button == Joystick_Button)
            {
                StatusMenu++;

            }
        }

        /// <summary>
        /// This method resizes the interface to accommodate different window sizes possible on different systems and monitors.
        /// This method is dependant on the different display modes as described at the method SetupUI().
        /// 
        /// The exact details still need further testing across various resolutions and aspect ratio's.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Telemetry_ResizePanels(object sender, EventArgs e)
        {
            try
            {
                if (TelemetryApplication.TelemetryAvailable)
                {
                    if (ucA1GP != null)
                    {
                        int tmp = StatusMenu;
                        StatusMenu = 0;

                        ucA1GP.Size = new Size(450, 325);
                        ucA1GP.Location = new Point(this.Size.Width - ucA1GP.Size.Width - 20,
                                                    this.Size.Height - ucA1GP.Height - 40);


                        this.ucLapChart.Location = new Point(this.Size.Width - ucLapChart.Size.Width - 30, 10);
                        if (this.ucLapChart.Height + this.ucA1GP.Height > this.Height - 40)
                            this.ucLapChart.Size = new Size(this.ucLapChart.Width, this.Height - this.ucA1GP.Height - 40);

                        this.ucTrackmap.Size = new Size(ucLapChart.Location.X - 50, this.Size.Height);
                        this.ucTrackmap.Location = new Point(10, 40);
                        ucSessionData.Location = new Point(ucA1GP.Location.X,
                                                           ucA1GP.Location.Y - ucSessionData.Size.Height - 10);

                        this.ucTyres.Size = ucA1GP.Size;
                        this.ucTyres.Location = ucA1GP.Location;

                        this.ucLaps.Size = ucA1GP.Size;
                        this.ucLaps.Location = ucA1GP.Location;

                        ucSplits.Size = ucA1GP.Size;
                        ucSplits.Location = ucA1GP.Location;
                        StatusMenu = tmp;
                    }
                }
                else
                {
                    SetupUI();
                }
            }
            catch (Exception ex)
            {
                // This exception is often fired because the resize event is fired before the panels are placed.
            }
        }

        /// <summary>
        /// High-speed user interface updates. This runs gauges that need fluent updates like the A1GP(general data) and tyres.
        /// The execution is invoked into the windows interface context.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmr_HiSpeed_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(Tmr_HiSpeed_Tick), new object[2] { sender, e });
                return;
            }

            ucA1GP.Update();
            if (Controls.Contains(ucA1GP)) ucA1GP.Invalidate();
            if (Controls.Contains(ucTyres)) ucTyres.Invalidate();
        }

        /// <summary>
        /// Medium-speed interface updates. This is the time keeper (Session status) and track map. The track map
        /// could also been placed into high-speed, but takes up too much CPU-time for little gain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tmr_MdSpeed_Tick(object sender, EventArgs e)
        {
            ucTrackmap.Invalidate();
            ucSessionData.Invalidate();

        }

        /// <summary>
        /// Low-speed interface updater. All things that keep track of lap times which don't have to be super fast.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Tmr_LwSpeed_Tick(object sender, EventArgs e)
        {
            ucLapChart.Invalidate();
            if (Controls.Contains(ucLaps)) ucLaps.Invalidate();
            if (Controls.Contains(ucSplits)) ucSplits.Invalidate();
        }

    }

}
