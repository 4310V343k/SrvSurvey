﻿using SrvSurvey.game;
using System.Drawing.Drawing2D;

namespace SrvSurvey
{
    internal class PlotSysStatus : PlotBase, PlotterForm
    {
        public string? nextSystem;
        private Font boldFont = GameColors.fontMiddleBold;

        private PlotSysStatus() : base()
        {
            this.Width = 420;
            this.Height = 48;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            this.Font = GameColors.fontMiddle;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // ??
            }

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.initialize();
            this.reposition(Elite.getWindowRect(true));
        }

        public override void reposition(Rectangle gameRect)
        {
            if (gameRect == Rectangle.Empty)
            {
                this.Opacity = 0;
                return;
            }

            this.Opacity = Game.settings.Opacity;

            //Elite.floatLeftTop(this, gameRect, 4, 10);
            Elite.floatLeftBottom(this, gameRect, 44, 12);

            this.Invalidate();
        }

        public static bool allowPlotter
        {
            get => Game.activeGame != null && Game.activeGame.isMode(GameMode.SuperCruising, GameMode.SAA, GameMode.FSS, GameMode.ExternalPanel, GameMode.Orrery, GameMode.SystemMap, GameMode.CommsPanel);
        }

        protected override void Game_modeChanged(GameMode newMode, bool force)
        {
            if (this.IsDisposed) return;

            var targetMode = this.game.isMode(GameMode.SuperCruising, GameMode.SAA, GameMode.FSS, GameMode.ExternalPanel, GameMode.Orrery, GameMode.SystemMap, GameMode.CommsPanel);
            if (this.Opacity > 0 && !targetMode)
                this.Opacity = 0;
            else if (this.Opacity == 0 && targetMode)
                this.reposition(Elite.getWindowRect());

            this.Invalidate();
        }

        protected override void onJournalEntry(FSSBodySignals entry)
        {
            Game.log($"PlotSysStatus: FSSBodySignals event: {entry.Bodyname}");
            this.Invalidate();
        }

        protected override void onJournalEntry(FSSDiscoveryScan entry)
        {
            Game.log($"PlotSysStatus: Scan event: {entry.SystemName}");
            this.nextSystem = null;
            this.Invalidate();
        }

        protected override void onJournalEntry(Scan entry)
        {
            Game.log($"PlotSysStatus: Scan event: {entry.Bodyname}");
            this.nextSystem = null;
            this.Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.IsDisposed || game.systemData == null) return;

            this.g = e.Graphics;
            this.g.SmoothingMode = SmoothingMode.HighQuality;

            base.OnPaintBackground(e);
            var txt = "";
            if (Game.settings.skipLowValueDSS) txt += $">{Util.credits(Game.settings.skipLowValueAmount)}";
            if (!Game.settings.skipRingsDSS) txt += " +Rings";
            g.DrawString($"System survey remaining: ({txt})", GameColors.fontSmall, GameColors.brushGameOrange, 4, 7);

            this.dtx = 6.0f;
            this.dty = 19.0f;

            var sys = this.game.systemStatus;
            var destinationBody = game.status.Destination?.Name?.Replace(sys.name, "").Replace(" ", "");

            try
            {
                if (this.nextSystem != null)
                {
                    // render next system only, if populated
                    this.drawTextAt("Next system:");
                    this.drawTextAt(this.nextSystem, GameColors.brushCyan);
                    return;
                }

                var dssRemaining = game.systemData.getDssRemainingNames();

                if (!game.systemData.honked)
                {
                    this.drawTextAt($"FSS not started", GameColors.brushCyan);
                }
                else if (!game.systemData.fssComplete)
                {
                    var fssProgress = 100.0 / (float)game.systemData.bodyCount * (float)game.systemData.fssBodyCount;
                    this.drawTextAt($"FSS {(int)fssProgress}% complete", GameColors.brushCyan);
                }
                else if (dssRemaining.Count > 0)
                {
                    this.drawTextAt($"{dssRemaining.Count}x bodies: ");
                    this.drawRemainingBodies(destinationBody, dssRemaining);
                }
                else
                {
                    this.drawTextAt("No DSS scans needed");
                }

                var bioRemaining = game.systemData.getBioRemainingNames();
                if (bioRemaining.Count > 0)
                {
                    this.drawTextAt($"| {game.systemData.bioSignalsRemaining}x Bio signals on: ");
                    this.drawRemainingBodies(destinationBody, bioRemaining);
                }
            }
            finally
            {
                // resize window to fit as necessary
                this.Width = Math.Max((int)this.dtx + 6, 170);
            }
        }

        /// <summary>
        /// Render names in a horizontal list, highlighting any in the same group as the destination
        /// </summary>
        private void drawRemainingBodies(string? destination, List<string> names)
        {
            const TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;

            // draw each remaining body, highlighting color if they are in the same group as the destination, or all of them if no destination
            foreach (var bodyName in names)
            {
                var isLocal = string.IsNullOrEmpty(destination) || bodyName[0] == destination[0];

                var font = this.Font;

                if (destination == bodyName) font = this.boldFont;
                var color = isLocal ? GameColors.Cyan : GameColors.Orange;

                var sz = g.MeasureString(bodyName, font).ToSize();
                var rect = new Rectangle((int)this.dtx, (int)this.dty, sz.Width, sz.Height);

                TextRenderer.DrawText(g, bodyName, font, rect, color, flags);
                this.dtx += sz.Width;
            }
        }
    }
}

