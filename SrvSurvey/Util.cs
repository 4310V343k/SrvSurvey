﻿using DecimalMath;
using SrvSurvey.canonn;
using SrvSurvey.game;
using SrvSurvey.Properties;
using SrvSurvey.units;
using System.Diagnostics;

namespace SrvSurvey
{
    static class Util
    {
        public static double[] sol = new double[] { 0, 0, 0 };

        public static double degToRad(double angle)
        {
            return angle / Angle.ratioDegreesToRadians;
        }
        public static double degToRad(int angle)
        {
            return angle / Angle.ratioDegreesToRadians;
        }

        public const decimal ratioDegreesToRadians = 180 / (decimal)Math.PI;

        public static decimal degToRad(decimal angle)
        {
            return angle / ratioDegreesToRadians;
        }

        public static string lsToString(double m)
        {
            if (m > 1000)
                return (m / 1000).ToString("N1") + "k LS";
            else
                return m.ToString("N0") + " LS";
        }

        public static double lsToM(double ls)
        {
            // 149597870691 M per LS
            return 149_597_870_691d * ls;
        }

        public static double mToLS(double ls)
        {
            // 149597870691 M per LS
            return ls / 149_597_870_691d;
        }

        public static double auToM(double ls)
        {
            // 299792458 M per AU
            return 299_792_458d * ls;
        }

        public static string metersToString(decimal m, bool asDelta = false)
        {
            return metersToString((double)m, asDelta);
        }

        /// <summary>
        /// Returns a count of meters to a string with 4 significant digits, adjusting the units accordinly between: m, km, mm
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string metersToString(double m, bool asDelta = false)
        {
            var prefix = "";
            if (asDelta)
            {
                prefix = m < 0 ? "-" : "+";
            }

            // make number positive
            if (m < 0) m = -m;

            if (m < 1)
            {
                // less than 1 thousand
                return "0m";
            }

            if (m < 1000)
            {
                // less than 1 thousand
                return prefix + m.ToString("#") + "m";
            }

            m = m / 1000;
            if (m < 10)
            {
                // less than 1 ten
                return prefix + m.ToString("##.##") + "km";
            }
            else if (m < 1000)
            {
                // less than 1 thousand
                return prefix + m.ToString("###.#") + "km";
            }

            m = m / 1000;
            // over one 1 million
            return prefix + m.ToString("#.##") + "mm";

        }

        public static string timeSpanToString(TimeSpan ago)
        {
            if (ago.TotalSeconds < 60)
                return "just now";
            if (ago.TotalMinutes < 120)
                return ago.TotalMinutes.ToString("#") + " minutes ago";
            if (ago.TotalHours < 24)
                return ago.TotalHours.ToString("#") + " hours ago";
            return ago.TotalDays.ToString("#") + " days ago";
        }

        public static string getLocationString(string starSystem, string body)
        {
            if (string.IsNullOrEmpty(body))
                return starSystem;


            // for cases like: "StarSystem":"Adenets" .. "Body":"Adenets 8 c"
            if (body.Contains(starSystem))
                return body;

            // for cases like: "StarSystem":"Enki" .. "Body":"Ponce de Leon Vision",
            return $"{starSystem}, {body}";

        }

        public static void openLink(string link)
        {
            var trimmedLink = link.Substring(0, Math.Min(200, link.Length));
            Game.log($"Opening link: (length: {link.Length})\r\n{trimmedLink}\r\n");

            var info = new ProcessStartInfo(link);
            info.UseShellExecute = true;
            Process.Start(info);
        }

        public static string credits(long credits, bool hideUnits = false)
        {
            string txt;

            if (credits < 1_000)
                txt = credits.ToString("N0");
            else if (credits < 100_000)
                txt = (credits / 1_000d).ToString("#.##") + " K";
            else if (credits < 1_000_000)
                txt = (credits / 1_000d).ToString("#") + " K";
            else if (credits < 100_000_000)
                txt = (credits / 1_000_000d).ToString("#.##") + " M";
            else if (credits < 1_000_000_000)
                txt = (credits / 1_000_000d).ToString("#") + " M";
            else
                txt = (credits / 1_000_000_000d).ToString("#.###") + " B";

            //var millions = credits / 1000000.0D;
            //if (millions == 0)
            //    txt = "0 M";
            //else if (millions < 1000)
            //    txt = millions.ToString("#.## M");
            //else
            //    txt = (millions / 1000).ToString("#.### B");

            if (!hideUnits)
                txt += " CR";
            return txt;
        }

        public static string getMinMaxCredits(long min, long max)
        {
            if (min <= 0 && max <= 0) return "";

            return min == max
                ? Util.credits(min, true)
                : Util.credits(min, true) + " ~ " + Util.credits(max, true);
        }

        public static LatLong2 adjustForCockpitOffset(decimal radius, LatLong2 location, string shipType, float shipHeading)
        {
            if (shipType == null) return location;

            var po = ShipCenterOffsets.get(shipType);
            if (po.X == 0 && po.Y == 0)
            {
                Game.log($"No offset for {shipType} yet :/");
                return location;
            }

            var pd = po.rotate((decimal)shipHeading);

            var dpm = 1 / (DecimalEx.TwoPi * radius / 360m); // meters per degree
            var dx = pd.X * dpm;
            var dy = pd.Y * dpm;

            var newLocation = new LatLong2(
                location.Lat + dy,
                location.Long + dx
            );

            var dist = po.dist;
            var angle = po.angle + (decimal)shipHeading;
            Game.log($"Adjusting landing location for: {shipType}, dist: {dist}, angle: {angle}\r\npd:{pd}, po:{po} (alt: {Game.activeGame?.status?.Altitude})\r\n{location} =>\r\n{newLocation}");
            return newLocation;
        }


        public static PointM getOffset(decimal r, LatLong2 p1, float siteHeading = -1)
        {
            return getOffset(r, p1, null, siteHeading);
        }

        /// <summary>
        /// Returns the relative offset (in meters) from one location to the other
        /// </summary>
        public static PointM getOffset(decimal r, LatLong2 p1, LatLong2? p2, float siteHeading = -1)
        {
            if (p2 == null) p2 = Status.here.clone();

            var dist = Util.getDistance(p1, p2, r);
            var radians = Util.getBearingRad(p1, p2);

            // apply siteHeading rotation, if given
            if (siteHeading != -1)
                radians = radians - DecimalEx.ToRad((decimal)siteHeading);

            var dx = DecimalEx.Sin(radians) * dist;
            var dy = DecimalEx.Cos(radians) * dist;

            // Positive latitude is to the north
            // Positive longitude is to the east

            var pf = new PointM(dx, dy);
            return pf;
        }

        /// <summary>
        /// Returns the distance between two lat/long points on body with radius r.
        /// </summary>
        public static decimal getDistance(LatLong2 p1, LatLong2 p2, decimal r)
        {
            // don't bother calculating if positions are identical
            if (p1.Lat == p2.Lat && p1.Long == p2.Long)
                return 0;

            var lat1 = DecimalEx.ToRad(p1.Lat);
            var lat2 = DecimalEx.ToRad(p2.Lat);

            var angleDelta = DecimalEx.ACos(
                DecimalEx.Sin(lat1) * DecimalEx.Sin(lat2) +
                DecimalEx.Cos(lat1) * DecimalEx.Cos(lat2) * DecimalEx.Cos(Util.degToRad(p2.Long - p1.Long))
            );
            var dist = angleDelta * r;

            //// ---
            //var dLat = DecimalEx.ToRad(p2.Lat - p1.Lat);
            //var dLon = DecimalEx.ToRad(p2.Long - p1.Long);

            //var aa = DecimalEx.Sin(dLat / 2) * DecimalEx.Sin(dLat / 2) +
            //    DecimalEx.Cos(DecimalEx.ToRad(p1.Lat)) * DecimalEx.Cos(DecimalEx.ToRad(p2.Lat)) * DecimalEx.Sin(dLon / 2) *
            //    DecimalEx.Sin(dLon / 2)
            //    ;
            //var cc = 2 * DecimalEx.ATan2(DecimalEx.Sqrt(aa), DecimalEx.Sqrt(1 - aa));
            //var dd = r * cc;
            //// ---

            return dist;
        }

        /// <summary>
        /// Returns the bearing between two lat/long points on a body.
        /// </summary>
        public static decimal getBearing(LatLong2 p1, LatLong2 p2)
        {
            var deg = DecimalEx.ToDeg(getBearingRad(p1, p2));
            if (deg < 0) deg += 360;

            return deg;
        }

        public static decimal getBearingRad(LatLong2 p1, LatLong2 p2)
        {
            var lat1 = Util.degToRad((decimal)p1.Lat);
            var lon1 = Util.degToRad((decimal)p1.Long);
            var lat2 = Util.degToRad((decimal)p2.Lat);
            var lon2 = Util.degToRad((decimal)p2.Long);

            var x = DecimalEx.Cos(lat2) * DecimalEx.Sin(lon2 - lon1);
            var y = DecimalEx.Cos(lat1) * DecimalEx.Sin(lat2) - DecimalEx.Sin(lat1) * DecimalEx.Cos(lat2) * DecimalEx.Cos(lon2 - lon1);

            // x is opposite / y is adjacent
            var rad = DecimalEx.ATan2(x, y);
            if (rad < 0) rad += DecimalEx.TwoPi;

            return rad;
        }

        public static PointM rotateLine(decimal angle, decimal length)
        {
            var dx = DecimalEx.Sin(DecimalEx.ToRad(angle)) * length;
            var dy = DecimalEx.Cos(DecimalEx.ToRad(angle)) * length;

            return new PointM(dx, dy);
        }

        public static decimal ToAngle(double opp, double adj)
        {
            return ToAngle((decimal)opp, (decimal)adj);
        }

        public static decimal ToAngle(decimal opp, decimal adj)
        {
            if (opp == 0 && adj == 0) return 0;

            var flip = adj < 0;

            if (adj == 0) adj += 0.00001M;
            var angle = DecimalEx.ToDeg(DecimalEx.ATan(opp / adj));

            if (flip) angle = angle - 180;

            return angle;
        }

        /// <summary>
        /// Extract the species prefix from the name, without the color variant part
        /// </summary>
        public static string getSpeciesPrefix(string name)
        {
            // extract the species prefix from the name, without the color variant part
            var species = name.Replace("$Codex_Ent_", "").Replace("_Name;", "");
            var idx = species.LastIndexOf('_');
            if (species.IndexOf('_') != idx)
                species = species.Substring(0, idx);

            species = $"$Codex_Ent_{species}_";
            return species;
        }

        /// <summary>
        /// Extract the genus prefix from the name, without the color variant part
        /// </summary>
        public static string getGenusNameFromVariant(string name)
        {
            // extract the species prefix from the name, without the color variant part
            var genus = name.Replace("$Codex_Ent_", "").Replace("_Name;", "");
            var idx = genus.LastIndexOf('_');
            if (genus.IndexOf('_') != idx)
                genus = genus.Substring(0, idx);

            idx = genus.LastIndexOf('_');
            if (idx > 0)
                genus = genus.Substring(0, idx);

            genus = $"$Codex_Ent_{genus}_";
            return genus;
        }

        /// <summary>
        /// Extract the genus display name from variant localized
        /// </summary>
        public static string getGenusDisplayNameFromVariant(string variantLocalized)
        {
            var parts = variantLocalized.Split(" ", 2);
            if (variantLocalized.Contains("-"))
            {
                return parts[0];
            }
            else
            {
                return parts[1];
            }
        }

        public static double getSystemDistance(double[] here, double[] there)
        {
            var dist = Math.Sqrt(
                Math.Pow(here[0] - there[0], 2)
                + Math.Pow(here[1] - there[1], 2)
                + Math.Pow(here[2] - there[2], 2)
            );
            return dist;
        }

        public static bool isCloseToScan(LatLong2 location, string? genusName)
        {
            if (Game.activeGame?.cmdr == null || Game.activeGame.systemBody == null || genusName == null) return false;
            var minDist = (decimal)Math.Min(PlotTrackers.highlightDistance, Game.activeGame.cmdr.scanOne?.radius ?? int.MaxValue);

            if (Game.activeGame.cmdr.scanOne != null)
            {
                if (Game.activeGame.cmdr.scanOne.genus != genusName) return false;

                var dist = Util.getDistance(location, Game.activeGame.cmdr.scanOne.location, Game.activeGame.systemBody!.radius);
                if (dist < minDist)
                    return true;
            }
            if (Game.activeGame.cmdr.scanTwo != null)
            {
                var dist = Util.getDistance(location, Game.activeGame.cmdr.scanTwo.location, Game.activeGame.systemBody!.radius);
                if (dist < minDist)
                    return true;
            }

            return false;
        }

        public static void useLastLocation(Form form, Rectangle rect, bool locationOnly = false)
        {
            useLastLocation(form, rect.Location, locationOnly ? Size.Empty : rect.Size);
        }

        public static void useLastLocation(Form form, Point pt)
        {
            useLastLocation(form, pt, Size.Empty);
        }

        public static void useLastLocation(Form form, Point pt, Size sz)
        {
            if (pt == Point.Empty)
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                return;
            }

            if (sz != Size.Empty)
            {
                // not all forms are sizable, so don't change those
                form.Width = Math.Max(sz.Width, form.MinimumSize.Width);
                form.Height = Math.Max(sz.Height, form.MinimumSize.Height);
            }

            // position ourselves within the bound of which ever screen is chosen
            var r = Screen.GetBounds(pt);
            if (pt.X < r.Left) pt.X = r.Left;
            if (pt.X + form.Width > r.Right) pt.X = r.Right - form.Width;

            if (pt.Y < r.Top) pt.Y = r.Top;
            if (pt.Y + form.Height > r.Bottom) pt.Y = r.Bottom - form.Height;

            form.StartPosition = FormStartPosition.Manual;
            form.Location = pt;
        }

        public static double targetAltitudeForSite(GuardianSiteData.SiteType siteType)
        {
            var targetAlt = 0d;
            switch (siteType)
            {
                case GuardianSiteData.SiteType.Alpha:
                    targetAlt = Game.settings.aerialAltAlpha;
                    break;
                case GuardianSiteData.SiteType.Beta:
                    targetAlt = Game.settings.aerialAltBeta;
                    break;
                case GuardianSiteData.SiteType.Gamma:
                    targetAlt = Game.settings.aerialAltGamma;
                    break;
                case GuardianSiteData.SiteType.Robolobster:
                    targetAlt = 1500d; // TODO: setting?
                    break;
                default:
                    targetAlt = 650d;
                    break;
                    // crossroads: 500
            }

            return targetAlt;
        }

        public static StarSystem getRecentStarSystem()
        {
            var cmdr = Game.activeGame?.cmdr;
            if (cmdr == null && Game.settings.lastFid != null)
                cmdr = CommanderSettings.Load(Game.settings.lastFid, true, Game.settings.lastCommander!);

            if (!string.IsNullOrEmpty(cmdr?.currentSystem))
            {
                return new StarSystem
                {
                    systemName = cmdr.currentSystem,
                    pos = cmdr.starPos,
                };
            }
            else
            {
                return StarSystem.Sol;
            }
        }

        public static string flattenStarType(string starType)
        {
            if (starType == null) return null!;
            // Collapse these:
            //  D DA DAB DAO DAZ DAV DB DBZ DBV DO DOV DQ DC DCV DX
            //  W WN WNC WC WO
            //  CS C CN CJ CH CHd
            if (starType[0] == 'D' || starType[0] == 'W' || starType[0] == 'C')
                return starType[0].ToString();
            else
                return starType;
        }

        public static void showForm(Form form, Form? parent = null)
        {
            if (form.Visible == false)
            {
                //// NO! fade in if form doesn't exist yet
                //if (true && !form.IsHandleCreated)
                //{
                //    form.Opacity = 0;
                //    form.Load += new EventHandler((object? sender, EventArgs e) =>
                //    {
                //        // using the quicker fade-out duration
                //        Util.fadeOpacity(form, 1, Game.settings.fadeOutDuration); // 100ms
                //    });
                //}

                if (parent == null)
                    form.Show();
                else
                    form.Show(parent);
            }
            else
            {
                if (form.WindowState == FormWindowState.Minimized)
                    form.WindowState = FormWindowState.Normal;

                form.Activate();
            }
        }

        public static void fadeOpacity(Form form, float targetOpacity, float durationMs = 0)
        {
            if (targetOpacity < 0 || targetOpacity > 1) throw new ArgumentOutOfRangeException(nameof(targetOpacity));
            Debug.WriteLine($"!START! {form.Name} {form.Size} {durationMs}ms / {form.Opacity} => {targetOpacity}");

            // exit early if no-op
            if (targetOpacity == form.Opacity) return;

            // don't animate if duration is zero
            if (durationMs == 0)
                form.Opacity = targetOpacity;
            else
            {
                var started = DateTime.Now.Ticks;
                var s1 = DateTime.Now;

                var delta = 1f / durationMs;
                fadeNext(form, targetOpacity, 0, delta);
                //Application.DoEvents();

                //var fadeTimer = new System.Windows.Forms.Timer()
                //{
                //    Interval = 50,
                //};
                //delta *= fadeTimer.Interval;
                //var nn = 0;

                //fadeTimer.Tick += new EventHandler((object? sender, EventArgs e) =>
                //{
                //    var dd = (DateTime.Now.Ticks - started) / 10_000;
                //    //Debug.WriteLine($"? {dd}");

                //    var wasOpacity = form.Opacity;
                //    var wasSmaller = form.Opacity < targetOpacity;

                //    var newOpacity = form.Opacity + (wasSmaller ? delta : -delta);
                //    var isSmaller = newOpacity < targetOpacity;
                //    ++nn;

                //    //Debug.WriteLine($"{dd} {form.Name} #{++nn} delta:{delta}, junk.Opacity:{wasOpacity}, this.targetOpacity:{targetOpacity}, wasSmaller:{wasSmaller}, isSmaller:{isSmaller}");

                //    // end animation when we reach target
                //    if (wasSmaller != isSmaller || form.Opacity == targetOpacity)
                //    {
                //        fadeTimer.Stop();
                //        Debug.WriteLine($"{dd} {form.Name} #{nn} delta:{delta}, junk.Opacity:{wasOpacity}, this.targetOpacity:{targetOpacity}, wasSmaller:{wasSmaller}, isSmaller:{isSmaller}");
                //        form.Opacity = targetOpacity;
                //        Debug.WriteLine($"!STOP! ({nn}) {form.Name} {DateTime.Now.Subtract(s1).TotalMilliseconds}");
                //    }
                //    else
                //    {
                //        form.Opacity = newOpacity;
                //    }
                //});

                //fadeTimer.Start();
            }
        }

        private static void fadeNext(Form form, float targetOpacity, long lastTick, float delta)
        {
            if (form.IsDisposed) return;

            var wasOpacity = form.Opacity;
            var wasSmaller = form.Opacity < targetOpacity;

            // (there are 10,000 ticks in a millisecond)
            var nextTick = lastTick + 10_000;

            if (nextTick < DateTime.Now.Ticks)
            {
                var newOpacity = form.Opacity + (wasSmaller ? delta : -delta);
                var isSmaller = newOpacity < targetOpacity;

                //Debug.WriteLine($"! delta:{delta}, junk.Opacity:{wasOpacity}, this.targetOpacity:{targetOpacity}, wasSmaller:{wasSmaller}, isSmaller:{isSmaller}");

                // end animation when we reach target
                if (wasSmaller != isSmaller || form.Opacity == targetOpacity)
                {
                    if (form.Opacity != newOpacity)
                        form.Opacity = targetOpacity;
                    Debug.WriteLine($"!STOP! {form.Name} {form.Size} {form.Opacity}");
                    form.Invalidate();
                    return;
                }

                form.Opacity = newOpacity;
                lastTick = DateTime.Now.Ticks;
            }
            else
            {
                //Debug.WriteLine($"~ delta:{delta}, junk.Opacity:{wasOpacity}, this.targetOpacity:{targetOpacity}, skip! {lastTick} // {nextTick} < {DateTime.Now.Ticks}");
            }

            Program.control.BeginInvoke(() => fadeNext(form, targetOpacity, lastTick, delta));
        }

        public static bool isOdyssey = Game.activeGame?.journals == null || Game.activeGame.journals.isOdyssey;

        public static int GetBodyValue(Scan scan, bool cmdrMapped)
        {
            return Util.GetBodyValue(
                scan.PlanetClass ?? scan.StarType, // planetClass
                scan.TerraformState == "Terraformable", // isTerraformable
                scan.MassEM > 0 ? scan.MassEM : scan.StellarMass, // mass
                !scan.WasDiscovered, // isFirstDiscoverer
                cmdrMapped, // isMapped
                !scan.WasMapped // isFirstMapped
            );
        }

        public static int GetBodyValue(SystemBody body, bool cmdrMapped, bool withEfficiencyBonus)
        {
            return Util.GetBodyValue(
                body.type == SystemBodyType.Star ? body.starType : body.planetClass, // planetClass
                body.terraformable, // isTerraformable
                body.mass,
                !body.wasDiscovered, // isFirstDiscoverer
                cmdrMapped,
                !body.wasMapped, // isFirstMapped
                withEfficiencyBonus
            );
        }

        public static double getStarKValue(string starClass)
        {
            var kk = 1200;
            if (starClass == "NS" || starClass == "BH" || starClass == "SupermassiveBlackHole")
                kk = 22628;
            else if (starClass?[0] == 'W')
                kk = 14057;

            return kk;
        }

        public static double getBodyKValue(string planetClass, bool isTerraformable)
        {
            int k = 0;

            if (planetClass == "Metal rich body") // MR - Metal Rich
                k = 21790;
            else if (planetClass == "Ammonia world") // AW
                k = 96932;
            else if (planetClass == "Sudarsky class I gas giant") // GG1
                k = 1656;
            else if (planetClass == "Sudarsky class II gas giant" // GG2
                || planetClass == "High metal content body") // HMC
            {
                k = 9654;
                if (isTerraformable) k += 100677;
            }
            else if (planetClass == "Water world") // WW
            {
                k = 64831;
                if (isTerraformable) k += 116295;
            }
            else if (planetClass == "Earthlike body") // ELW
            {
                k = 64831 + 116295;
            }
            else // RB - Rocky Body
            {
                k = 300;
                if (isTerraformable) k += 93328;
            }

            return k;
        }

        public static int GetBodyValue(string? planetClass, bool isTerraformable, double mass, bool isFirstDiscoverer, bool isMapped, bool isFirstMapped, bool withEfficiencyBonus = true, bool isFleetCarrierSale = false)
        {
            // Logic from https://forums.frontier.co.uk/threads/exploration-value-formulae.232000/

            var isOdyssey = Util.isOdyssey;
            var isStar = planetClass != null && (planetClass.Length < 8 || planetClass[1] == '_' || planetClass == "SupermassiveBlackHole" || planetClass == "Nebula" || planetClass == "StellarRemnantNebula");

            if (isStar)
            {
                var kk = getStarKValue(planetClass!);
                var starValue = kk + (mass * kk / 66.25);
                return (int)Math.Round(starValue);
            }

            // determine base value
            var k = getBodyKValue(planetClass!, isTerraformable);

            // public static int GetBodyValue(int k, double mass, bool isFirstDiscoverer, bool isMapped, bool isFirstMapped, bool withEfficiencyBonus, bool isOdyssey, bool isFleetCarrierSale)
            // based on code from https://forums.frontier.co.uk/threads/exploration-value-formulae.232000/
            const double q = 0.56591828;
            double mappingMultiplier = 1;
            if (isMapped)
            {
                if (isFirstDiscoverer && isFirstMapped)
                {
                    mappingMultiplier = 3.699622554;
                }
                else if (isFirstMapped)
                {
                    mappingMultiplier = 8.0956;
                }
                else
                {
                    mappingMultiplier = 3.3333333333;
                }
            }
            double value = (k + k * q * Math.Pow(mass, 0.2)) * mappingMultiplier;
            if (isMapped)
            {
                if (isOdyssey)
                {
                    value += ((value * 0.3) > 555) ? value * 0.3 : 555;
                }
                if (withEfficiencyBonus)
                {
                    value *= 1.25;
                }
            }
            value = Math.Max(500, value);
            value *= (isFirstDiscoverer) ? 2.6 : 1;
            value *= (isFleetCarrierSale) ? 0.75 : 1;
            return (int)Math.Round(value);
        }

        public static bool isCloseColor(Color actual, Color expected, int tolerance = 5)
        {
            // confirm actual RGB values are within ~5 of expected
            return actual.R > expected.R - tolerance && actual.R < expected.R + tolerance
                && actual.G > expected.G - tolerance && actual.G < expected.G + tolerance
                && actual.B > expected.B - tolerance && actual.B < expected.B + tolerance;
        }

        public static bool isFirewallProblem(Exception? ex)
        {
            if (ex != null) Game.log($"isFirewallProblem?\r\n{ex}");
            if (ex?.Message.Contains("An attempt was made to access a socket in a way forbidden by its access permissions") == true
                || ex?.Message.Contains("A socket operation was attempted to an unreachable network") == true)
            {
                var rslt = MessageBox.Show(
                    $"It appears network calls for SrvSurvey are being blocked by a firewall. This is more likely when running SrvSurvey from within the Downloads folder. Adding the location of SrvSurvey to your filewall will solve this problem.\r\n\r\n{Application.ExecutablePath}\r\n\r\nWould you like to copy that location to the clipboard?",
                    "SrvSurvey",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (rslt == DialogResult.Yes) Clipboard.SetText(Application.ExecutablePath);
                return true;
            }

            if ((ex as HttpRequestException)?.StatusCode == System.Net.HttpStatusCode.InternalServerError
                || ex?.Message.Contains("Response status code does not indicate success: 500") == true
                || ex?.Message.Contains("(Internal Server Error)") == true)
            {
                // strictly speaking this is not a firewall problem, it means some network call failed (hopefully) not due to us. Let's trace this and keep going.
                Game.log($"Ignoring HTTP:500 response");
                return true;
            }
            return false;
        }

        public static void handleError(Exception? ex)
        {
            if (Program.control.InvokeRequired)
            {
                Program.control.Invoke(new Action(() => handleError(ex)));
                return;
            }

            Game.log(ex);
            if ((ex as HttpRequestException)?.StatusCode == System.Net.HttpStatusCode.NotFound || Util.isFirewallProblem(ex))
            {
                // ignore NotFound or firewall responses
            }
            else if (ex != null)
            {
                FormErrorSubmit.Show(ex);
            }
            else
            {
                MessageBox.Show(
                    Main.ActiveForm,
                    "Something unexpected went wrong, please share the logs.\r\nIt is recommended to restart SrvSurvey.",
                    "SrvSurvey",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                ViewLogs.show(Game.logs);
            }

        }

        public static string getLogNameFromChar(char c)
        {
            switch (c)
            {
                case 'C': return "Culture";
                case 'H': return "History";
                case 'B': return "Biology";
                case 'T': return "Technology";
                case 'L': return "Language";
                case '#': return "";
                default:
                    throw new Exception($"Unexpected character '{c}'");
            }
        }

        /// <summary>
        /// Returns True if the two numbers are within tolerance of each other.
        /// </summary>
        public static bool isClose(double left, double right, double tolerance)
        {
            var delta = Math.Abs(left - right);
            return delta <= tolerance;
        }

        /// <summary>
        /// Returns True if the two numbers are within tolerance of each other.
        /// </summary>
        public static bool isClose(decimal left, decimal right, decimal tolerance)
        {
            var delta = Math.Abs(left - right);
            return delta <= tolerance;
        }

        /// <summary>
        /// Returns True is poiType is Casket, Orb, Tablet, Totem or an Urn.
        /// </summary>
        public static bool isBasicPoi(POIType poiType)
        {
            return poiType == POIType.casket || poiType == POIType.orb || poiType == POIType.tablet || poiType == POIType.totem || poiType == POIType.urn || poiType == POIType.unknown;
        }

        public static string camel(string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return "";
            else
                return char.ToUpperInvariant(txt[0]) + txt.Substring(1);
        }

        public static string compositionToCamel(string key)
        {
            // as needed, change "Sulphur dioxide" into "SulphurDioxide" or "Carbon dioxide-rich" into "CarbonDioxideRich"
            key = key.Replace("-rich", "Rich");
            var i = key.IndexOf(' ');
            if (i < 0)
                return key;
            else
                return key.Substring(0, i) + key.Substring(i + 1, 1).ToUpperInvariant() + key.Substring(i + 2);
        }

        public static bool isMatLevelThree(string matName)
        {
            switch (matName.ToLowerInvariant())
            {
                case "cadmium":
                case "mercury":
                case "molybdenum":
                case "niobium":
                case "tin":
                case "tungsten":
                    return true;

                default:
                    return false;
            }
        }
        public static bool isMatLevelFour(string matName)
        {
            switch (matName.ToLowerInvariant())
            {
                case "antimony":
                case "polonium":
                case "ruthenium":
                case "technetium":
                case "tellurium":
                case "yttrium":
                    return true;

                default:
                    return false;
            }
        }

        public static Image getBioImage(string genus, bool large = false)
        {
            //return Resources.tubus18;

            switch (genus)
            {
                case "$Codex_Ent_Aleoids_Genus_Name;": return Resources.aleoida_18;
                case "$Codex_Ent_Bacterial_Genus_Name;": return large ? Resources.bacterium38 : Resources.bacterium18;
                case "$Codex_Ent_Cactoid_Genus_Name;": return Resources.cactoida_18;
                case "$Codex_Ent_Clypeus_Genus_Name;": return Resources.clypeus_18;
                case "$Codex_Ent_Conchas_Genus_Name;": return Resources.concha_18;
                case "$Codex_Ent_Electricae_Genus_Name;": return large ? Resources.electricae38 : Resources.electricae_18;
                case "$Codex_Ent_Fonticulus_Genus_Name;": return Resources.fonticulua_18;
                case "$Codex_Ent_Shrubs_Genus_Name;": return large ? Resources.frutexa38 : Resources.frutexa18;
                case "$Codex_Ent_Fumerolas_Genus_Name;": return Resources.fumerola_18;
                case "$Codex_Ent_Fungoids_Genus_Name;": return large ? Resources.fungoida38 : Resources.fungoida18;
                case "$Codex_Ent_Osseus_Genus_Name;": return large ? Resources.osseus38 : Resources.osseus_18;
                case "$Codex_Ent_Recepta_Genus_Name;": return Resources.recepta_18;
                case "$Codex_Ent_Stratum_Genus_Name;": return large ? Resources.stratum38 : Resources.stratum18;
                case "$Codex_Ent_Tubus_Genus_Name;": return large ? Resources.tubus38 : Resources.tubus18;
                case "$Codex_Ent_Tussocks_Genus_Name;": return large ? Resources.tussock38 : Resources.tussock18;
                case "$Codex_Ent_Vents_Genus_Name;": return Resources.amphora_18;
                case "$Codex_Ent_Sphere_Name;": return Resources.anemone_18;
                case "$Codex_Ent_Cone_Genus_Name;": return Resources.barkmound_18;
                case "$Codex_Ent_Brancae_Genus_Name;": return Resources.braintree_18;
                case "$Codex_Ent_Ground_Struct_Ice_Genus_Name;": return Resources.shards_18;
                case "$Codex_Ent_Tube_Name;":
                case "$Codex_Ent_Tube_Genus_Name;":
                case "$Codex_Ent_TubeABCD_Genus_Name;":
                case "$Codex_Ent_TubeEFGH_Genus_Name;":
                    return Resources.tuber_18;

                default: throw new Exception($"Unexpected genus: '{genus}'");
            }
        }

        /// <summary>
        /// Returns an Economy enum from strings like "$economy_HighTech;"
        /// </summary>
        public static Economy toEconomy(string? txt)
        {
            switch (txt)
            {
                case "$economy_Agri;": return Economy.Agriculture;
                case "$economy_Colony;": return Economy.Colony;
                case "$economy_Damaged;": return Economy.Damaged;
                case "$economy_Extraction;": return Economy.Extraction;
                case "$economy_HighTech;": return Economy.HighTech;
                case "$economy_Industrial;": return Economy.Industrial;
                case "$economy_Military;": return Economy.Military;
                case "$economy_Prison;": return Economy.Prison;
                case "$economy_Carrier;": return Economy.PrivateEnterprise;
                case "$economy_Refinery;": return Economy.Refinery;
                case "$economy_Repair;": return Economy.Repair;
                case "$economy_Rescue;": return Economy.Rescue;
                case "$economy_Service;": return Economy.Service;
                case "$economy_Terraforming;": return Economy.Terraforming;
                case "$economy_Tourism;": return Economy.Tourist;

                default: return Economy.Unknown;
            }
        }
    }
}
