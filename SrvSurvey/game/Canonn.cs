﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS0649

namespace SrvSurvey.game.canonn
{
    internal class Coords
    {
        public double x;
        public double y;
        public double z;
    }

    internal class Region
    {
        public string name;
        public int region;
    }

    internal class Timestamps
    {
        public DateTimeOffset distanceToArrival; //: 2023-03-18T18:29:28, 
        public DateTimeOffset meanAnomaly; //: 2023-03-18T18:29:28
    }

    internal enum CanonnType
    {
        Star,
        Planet,
        Barycentre
    }

    internal class BodyBase
    {
        public long id64; // : 36036076585428353, 
        public CanonnType type; // : Star, 
        public DateTimeOffset updateTime; // : 2023-03-18 18:29:28+00
        public int bodyId; // : 1, 
        public string name; // : Col 173 Sector PF-E b28-3 A, 
        public double distanceToArrival; // : 0.0, 
        public Timestamps timestamps;
        // timestamps: {
        //   distanceToArrival: 2023-03-18T18:29:28, 
        //   meanAnomaly: 2023-03-18T18:29:28
        // }, 
    }

    internal class Star : BodyBase
    {
        public double absoluteMagnitude; // : 9.629074, 
        public int age; // : 6018, 
        public double argOfPeriapsis; // : 269.337595, 
        public double ascendingNode; // : -132.232712, 
        public double axialTilt; // : 0.0, 
        public string luminosity; // : Va, 
        public bool mainStar; // : true, 
        public double meanAnomaly; // : 346.993797, 
        public double orbitalEccentricity; // : 0.250834, 
        public double orbitalInclination; // : 18.490047, 
        public double orbitalPeriod; // : 4112.48752088458, 
        //parents: [
        //  {
        //    Null: 0
        //  }
        //], 
        public double rotationalPeriod; // : 2.46148771125, 
        public bool rotationalPeriodTidallyLocked; // : false, 
        public double semiMajorAxis; // : 1.61398112830458, 
        public double solarMasses; // : 0.332031, 
        public double solarRadius; // : 0.489135712437096, 
        public string spectralClass; // : M5, 
                                     // stations: [], 
        public string subType; // : M (Red dwarf) Star, 
        public double surfaceTemperature; // : 2737.0, 
    }

    internal class Signals
    {
        public List<string> biology;
        public List<string> genuses;
        public List<string> geology;
        public List<string> guardian;
        public Dictionary<string, int> signals;
    }

    internal class Planet : BodyBase
    {
        public double argOfPeriapsis; // : 132.02895, 
        public double ascendingNode; // : -179.836934, 
        public string atmosphereType; // : null, 
        public double axialTilt; // : 0.165382, 

        public double earthMasses; // : 0.006787, 
        public double gravity; // : 0.178385846844091, 
        public bool isLandable; // : true, 
        public Dictionary<string, double> materials;
        /*materials: {
          Cadmium: 1.622977, 
          Carbon: 12.454605, 
          Chromium: 9.399409, 
          Iron: 20.899969, 
          Manganese: 8.631474, 
          Nickel: 15.807859, 
          Niobium: 1.428401, 
          Phosphorus: 7.973651, 
          Ruthenium: 1.290706, 
          Sulphur: 14.811105, 
          Zinc: 5.679829
        },*/
        public double meanAnomaly; // : 244.993055, 

        public double orbitalEccentricity; // : 0.025323, 
        public double orbitalInclination; // : -7.49158, 
        public double orbitalPeriod; // : 0.946358901759259, 
        public List<Dictionary<string, int>> parents;
        /*parents: [
          {
            Null: 8
          }, 
          {
            Star: 2
          }, 
          {
            Null: 0
          }
        ],*/
        public double radius; // : 1243.482625, 
        public double rotationalPeriod; // : 1.39955643063657, 
        public bool rotationalPeriodTidallyLocked; // : true, 
        public double semiMajorAxis; // : 2.88837836692284e-05, 
        public Signals signals;
        /*signals: {
          biology: [
            Roseum Brain Tree
          ], 
          genuses: [
            $Codex_Ent_Brancae_Name;
          ], 
          geology: [
            Silicate Vapour Fumarole, 
            Silicate Magma Lava Spout, 
            Silicate Vapour Gas Vent
          ], 
          guardian: [
            Guardian Codex, 
            Guardian Data Terminal, 
            Guardian Relic Tower, 
            Guardian Pylon, 
            Guardian Sentinel
          ], 
          signals: {
            $SAA_SignalType_Biological;: 1, 
            $SAA_SignalType_Geological;: 3, 
            $SAA_SignalType_Guardian;: 3
          }, 
          updateTime: 2023-03-26 00:57:36+00
        }, */
        public Dictionary<string, double> solidComposition;
        /*solidComposition: {
          Ice: 0.0, 
          Metal: 32.9064, 
          Rock: 67.0936
        },*/
        //stations: [], 
        public string subType; // : High metal content world, 
        public double surfacePressure; // : 0.0, 
        public double surfaceTemperature; // : 285.962128, 
        public string terraformingState; // : Not terraformable, 
        public string volcanismType; // : Minor Silicate Vapour Geysers
    }

    internal class System
    {
        public string allegiance;
        public List<BodyBase> bodies;
        public int bodyCount;
        public Coords coords;
        public DateTimeOffset date;
        public string government;
        public long id64;
        public string name;
        public long population;
        public string primaryEconomy;
        public Region region;
        public string secondaryEconomy;
        public string security;
    }

    internal class BioStats
    {
        public System system;
    }

    internal class SAAsignals
    {
        public string body; //: "B 1",
        public int count; //: 3,
        public string english_name; //: "Guardian",
        public string hud_category; //: "Guardian"
    }

    internal class Codex
    {
        public string body; //: "A 4",
        public string english_name; //: "Bacterium Cerbrus - Teal",
        public long entryid; //: 2321207,
        public string hud_category; //: "Biology",
        public long index_id; // : null,
        public double latitude; //: null,
        public double longitude; //: null,
        public bool scanned; //: "false"
    }

    internal class SystemPoi
    {
        public List<SAAsignals> SAAsignals;
        public string cmdrName;
        public List<Codex> codex;
        public string odyssey;
        public string system;
    }

    internal class OrganicRewards
    {
        public long entryid; //         2100201
        //public string name; //         "$Codex_Ent_Seed_Name;"
        public long reward; //          1593700
    }

    internal class RefCodexEntry
    {
        // https://us-central1-canonn-api-236217.cloudfunctions.net/query/codex/ref
        public string category; //     "$Codex_Category_Biology;"
        public string english_name; // "Roseum Brain Tree"
        public long entryid; //         2100201
        public string hud_category; // "Biology"
        public string name; //         "$Codex_Ent_Seed_Name;"
        public string platform; //     "legacy"
        public long? reward; //          1593700
        public string sub_category; // "$Codex_SubCategory_Organic_Structures;"
        public string sub_class; //    "Brain Tree"
    }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS0649