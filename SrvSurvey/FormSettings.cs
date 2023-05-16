﻿using SrvSurvey.game;
using System.Diagnostics;
using System.Reflection;

namespace SrvSurvey
{
    public partial class FormSettings : Form
    {
        private static string releaseVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

        private Game game = Game.activeGame!;
        private readonly Dictionary<string, FieldInfo> map = new Dictionary<string, FieldInfo>();

        public FormSettings()
        {
            InitializeComponent();

            // build a map of fields on the setting objects by name
            foreach (var fieldInfo in typeof(Settings).GetRuntimeFields())
                this.map.Add(fieldInfo.Name, fieldInfo);

            // only show this button if there are multiple copies of EliteDengerous running at the same time
            var procED = Process.GetProcessesByName("EliteDangerous64");
            btnNextProc.Visible = procED.Length > 1;
            this.Text += $" ({releaseVersion})";
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            updateFormFromSettings(this);
        }

        private void updateFormFromSettings(Control parentControl)
        {
            foreach (Control ctrl in parentControl.Controls)
            {
                if (!string.IsNullOrWhiteSpace(ctrl.Tag?.ToString()))
                {
                    if (!map.ContainsKey(ctrl.Tag.ToString()!))
                    {
                        throw new Exception($"Missing setting: {ctrl.Tag}");
                    }

                    var name = ctrl.Tag.ToString()!;
                    // Game.log($"Read setting: {name} => {map[name].GetValue(Game.settings)}");

                    switch (ctrl.GetType().Name)
                    {
                        case nameof(LinkLabel):
                        case nameof(TextBox):
                            ctrl.Text = (string)map[name].GetValue(Game.settings)!;
                            break;

                        case nameof(CheckBox):
                            ((CheckBox)ctrl).Checked = (bool)map[name].GetValue(Game.settings)!;
                            break;

                        case nameof(NumericUpDown):
                            ((NumericUpDown)ctrl).Value = (decimal)((double)map[name].GetValue(Game.settings)! * 100.0);
                            break;

                        default:
                            throw new Exception($"Unexpected control type: {ctrl.GetType().Name}");
                    }
                }

                // recurse if there are children
                if (ctrl.HasChildren)
                    updateFormFromSettings(ctrl);
            }
        }

        private void updateSettingsFromForm(Control parentControl)
        {
            foreach (Control ctrl in parentControl.Controls)
            {
                if (ctrl.Tag != null && map.ContainsKey(ctrl.Tag.ToString()!))
                {
                    var name = ctrl.Tag.ToString()!;

                    object? val = null;
                    switch (ctrl.GetType().Name)
                    {
                        case nameof(LinkLabel):
                        case nameof(TextBox):
                            val = ctrl.Text;
                            break;

                        case nameof(CheckBox):
                            val = ((CheckBox)ctrl).Checked;
                            break;

                        case nameof(NumericUpDown):
                            val = (double)((NumericUpDown)ctrl).Value / 100;
                            break;

                        default:
                            throw new Exception($"Unexpected control type: {ctrl.GetType().Name}");
                    }

                    // Game.log($"Write setting: {name} => {val}");
                    map[name].SetValue(Game.settings, val);
                }

                // recurse if there are children
                if (ctrl.HasChildren)
                    updateSettingsFromForm(ctrl);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // restart the app if these are different:
            var restartApp = txtCommander.Text != Game.settings.preferredCommander
                || checkEnableGuardianFeatures.Checked != Game.settings.enableGuardianSites;


            updateSettingsFromForm(this);
            Game.settings.Save();
            this.DialogResult = DialogResult.OK;

            // kill current process and reload
            if (restartApp)
            {
                Application.DoEvents();
                Process.Start(Application.ExecutablePath);

                Application.DoEvents();
                Application.Exit();
            }
            else
            {
                // force all plotters to reposition themselves
                var rect = Elite.getWindowRect();
                Program.repositionPlotters(rect);
            }
        }

        private void trackOpacity_Scroll(object sender, EventArgs e)
        {
            if (numOpacity.Value != trackOpacity.Value)
                numOpacity.Value = trackOpacity.Value;
        }

        private void numOpacity_ValueChanged(object sender, EventArgs e)
        {
            if (numOpacity.Value != trackOpacity.Value)
                trackOpacity.Value = (int)numOpacity.Value;

        }

        private void btnNextProc_Click(object sender, EventArgs e)
        {
            Program.hideActivePlotters();

            // increment process idx and make plotters adjust
            Game.settings.processIdx++;
            Application.DoEvents();
            Elite.setFocusED();
        }

        private void btnClearUnclaimed_Click(object sender, EventArgs e)
        {
            var rslt = MessageBox.Show($"Are you sure you want to clear {Util.credits(game.cmdr.organicRewards)} from {game.cmdr.scannedOrganics.Count} organisms?", "Clear unclaimed rewards", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (rslt == DialogResult.Yes)
            {
                Game.log($"Clearing in {game.cmdr.organicRewards} unclaimed rewards.");
                game.cmdr.organicRewards = 0;
                game.cmdr.scannedOrganics.Clear();
                game.cmdr.Save();
            }
        }

        private void checkProcessSAcreenshots_CheckedChanged(object sender, EventArgs e)
        {
            foreach(Control ctrl in this.checkProcessScreenshots.Parent.Controls)
                if (ctrl != checkProcessScreenshots)
                    ctrl.Enabled = checkProcessScreenshots.Checked;
        }

        private void chooseScreenshotFolder(LinkLabel linkLabel)
        {
            var dialog = new FolderBrowserDialog()
            {
                SelectedPath = Game.settings.screenshotSourceFolder ?? Elite.defaultScreenshotFolder,
            };

            var rslt = dialog.ShowDialog(this);
            if (rslt == DialogResult.OK)
                linkLabel.Text = dialog.SelectedPath;

        }

        private void btnChooseScreenshotSourceFolder_Click(object sender, EventArgs e)
        {
            this.chooseScreenshotFolder(linkScreenshotSourceFolder);
        }

        private void btnChooseScreenshotTargetFolder_Click(object sender, EventArgs e)
        {
            this.chooseScreenshotFolder(linkTargetScreenshotFolder);
        }

        private void linkScreenshotFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var linkLabel = (LinkLabel)sender;
            Game.log($"Opening screenshot folder:\r\n{linkLabel.Text}");
            Util.openLink(linkLabel.Text);
        }

        private void linkDataFiles_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.openLink(Application.UserAppDataPath);
        }
    }
}
