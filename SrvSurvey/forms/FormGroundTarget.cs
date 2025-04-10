﻿using SrvSurvey.forms;
using SrvSurvey.game;
using SrvSurvey.Properties;
using SrvSurvey.units;
using System.Text.RegularExpressions;

namespace SrvSurvey
{
    internal partial class FormGroundTarget : FixedForm
    {
        public FormGroundTarget()
        {
            InitializeComponent();
            this.Icon = Icons.set_square;

            Util.applyTheme(this);
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            try
            {
                Game.settings.targetLatLong = new LatLong2(
                    double.Parse(txtLat.Text), // match culture
                    double.Parse(txtLong.Text) // match culture
                    );
                Game.settings.targetLatLongActive = true;
                Game.settings.Save();
            }
            catch (Exception ex)
            {
                Game.log("Parse error: " + ex.Message);
            }

            this.DialogResult = DialogResult.OK;
        }

        private void FormGroundTarget_Load(object sender, EventArgs e)
        {
            txtLat.Text = Game.settings.targetLatLong.Lat.ToString();
            txtLong.Text = Game.settings.targetLatLong.Long.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Game.settings.targetLatLong = LatLong2.Empty;
            Game.settings.targetLatLongActive = false;
            Game.settings.Save();
            this.DialogResult = DialogResult.OK;
        }

        private void btnTargetCurrent_Click(object sender, EventArgs e)
        {
            txtLat.Text = Status.here.Lat.ToString();
            txtLong.Text = Status.here.Long.ToString();
        }

        private static Regex matchPaste = new Regex("([\\+-.0-9]*)\\s*[ ,|`/]\\s*([\\+-.0-9]*)", RegexOptions.Singleline);

        public static LatLong2? pasteFromClipboard()
        {
            var txt = Clipboard.GetText(TextDataFormat.Text);
            txt = txt.Replace("°N", ", ").Replace("°W", "");
            if (string.IsNullOrEmpty(txt)) return null;

            var match = matchPaste.Match(txt);
            if (match.Success && match.Groups.Count == 3 && double.TryParse(match.Groups[1].Value, out var newLat) && double.TryParse(match.Groups[2].Value, out var newLong))
            {
                return new LatLong2(newLat, newLong);
            }

            return null;
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            var newLocation = pasteFromClipboard();
            if (newLocation != null)
            {
                txtLat.Text = newLocation.Lat.ToString();
                txtLong.Text = newLocation.Long.ToString();
            }
        }
    }

}
