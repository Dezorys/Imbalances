#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
    public class Imbalances : Indicator
    {
        private bool varImbBull, varImbBear = false;
        private bool gibImbBull, gibImbBear = false;
        private int i = 0;
        private int i1 = 0;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Enter the description for your new custom Indicator here.";
                Name = "Imbalances";
                Calculate = Calculate.OnBarClose;
                IsOverlay = true;
                DisplayInDataBox = true;
                DrawOnPricePanel = true;
                DrawHorizontalGridLines = true;
                DrawVerticalGridLines = true;
                PaintPriceMarkers = true;
                ScaleJustification = NinjaTrader.Gui.Chart.ScaleJustification.Right;
                IsSuspendedWhileInactive = true;

                // Default settings for VIB, GIB, DIB, and Magic VIB
                ShowVibs = true;
                VIBCbu = Brushes.Green;
                VIBCbe = Brushes.Red;

                ShowGibs = true;
                GIBCbu = Brushes.Blue;
                GIBCbe = Brushes.Fuchsia;

                ShowDi = true;
                DICbu = Brushes.DarkGreen;
                DICbe = Brushes.DarkRed;

                ShowMVibs = true;
                VIBMCbu = Brushes.Yellow;
                VIBMCbe = Brushes.Maroon;

                ShowVIBDIB = true;
                Instances = 25;  // Leave this here in case we need to limit the number of Instances
                VIBDIBBullishColor = Brushes.Blue;  // Default color for bullish diamonds
                VIBDIBBearishColor = Brushes.Red;   // Default color for bearish diamonds

                // New properties for toggling symbols and rays
                ShowSymbolsOnly = false;
                ShowRaysOnly = false;

                // Alert settings
                vibabu = false;
                vibabe = false;
                gibabu = false;
                gibabe = false;
                dibabu = false;
                dibabe = false;
                mabu = false;
                mabe = false;
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < 2)
                return;

            // VIB Detection Logic (Section 1: Volume)
            if (Open[0] > Open[1] && Close[0] > Close[1] && Open[0] > Close[1] && Low[0] <= Close[1] && Close[0] > Open[0])
            {
                varImbBull = true;
                Print("Bullish VIB detected at bar: " + CurrentBar);
            }
            else
                varImbBull = false;

            if (Open[0] < Open[1] && Close[0] < Close[1] && Open[0] < Close[1] && Close[1] <= High[0] && Close[0] < Open[0])
            {
                varImbBear = true;
                Print("Bearish VIB detected at bar: " + CurrentBar);
            }
            else
                varImbBear = false;

            // Draw VIB markers (Section 1: Volume)
            if (ShowVibs && varImbBear)
            {
                if (!ShowSymbolsOnly || ShowRaysOnly) // Draw rays if ShowSymbolsOnly is false or ShowRaysOnly is true
                {
                    Draw.Ray(this, "tag1" + i.ToString(), 1, Open[0], 0, Open[0], VIBCbe);
                    Draw.Ray(this, "tag2" + i.ToString(), 1, Close[1], 0, Close[1], VIBCbe);
                }
                if (!ShowRaysOnly) // Draw symbols if ShowRaysOnly is false
                {
                    Draw.TriangleDown(this, "tag_triangle_bear" + CurrentBar.ToString(), true, 0, High[0] + 7 * TickSize, VIBCbe); // Bearish Triangle
                }
                Print("Bearish VIB Ray and Triangle drawn at bar: " + CurrentBar);
                i++;
                if (vibabe) Alert("myAlert", Priority.High, "VIB - Bearish", NinjaTrader.Core.Globals.InstallDir + @"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
            }

            if (ShowVibs && varImbBull)
            {
                if (!ShowSymbolsOnly || ShowRaysOnly) // Draw rays if ShowSymbolsOnly is false or ShowRaysOnly is true
                {
                    Draw.Ray(this, "tag1" + i.ToString(), 1, Open[0], 0, Open[0], VIBCbu);
                    Draw.Ray(this, "tag2" + i.ToString(), 1, Close[1], 0, Close[1], VIBCbu);
                }
                if (!ShowRaysOnly) // Draw symbols if ShowRaysOnly is false
                {
                    Draw.TriangleUp(this, "tag_triangle_bull" + CurrentBar.ToString(), true, 0, Low[0] - 7 * TickSize, VIBCbu); // Bullish Triangle
                }
                Print("Bullish VIB Ray and Triangle drawn at bar: " + CurrentBar);
                if (vibabe) Alert("myAlert", Priority.High, "VIB - Bullish", NinjaTrader.Core.Globals.InstallDir + @"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
                i++;
            }

            // Draw VIB diamonds independently if ShowVIBDIB is true (Section 5: Volume and Directional)
            if (ShowVIBDIB && varImbBear)
            {
                if (!ShowSymbolsOnly || ShowRaysOnly) // Draw rays if ShowSymbolsOnly is false or ShowRaysOnly is true
                {
                    Draw.Ray(this, "tag_vibdib_bear_ray1" + CurrentBar.ToString(), 1, Open[0], 0, Open[0], VIBDIBBearishColor);
                    Draw.Ray(this, "tag_vibdib_bear_ray2" + CurrentBar.ToString(), 1, Close[1], 0, Close[1], VIBDIBBearishColor);
                }
                if (!ShowRaysOnly) // Draw symbols if ShowRaysOnly is false
                {
                    Draw.Diamond(this, "tagz_vib_bear" + CurrentBar.ToString(), true, 0, High[0] + 7 * TickSize, VIBDIBBearishColor);
                }
                Print("Bearish VIB DIB Ray and Diamond drawn at bar: " + CurrentBar);
            }

            if (ShowVIBDIB && varImbBull)
            {
                if (!ShowSymbolsOnly || ShowRaysOnly) // Draw rays if ShowSymbolsOnly is false or ShowRaysOnly is true
                {
                    Draw.Ray(this, "tag_vibdib_bull_ray1" + CurrentBar.ToString(), 1, Open[0], 0, Open[0], VIBDIBBullishColor);
                    Draw.Ray(this, "tag_vibdib_bull_ray2" + CurrentBar.ToString(), 1, Close[1], 0, Close[1], VIBDIBBullishColor);
                }
                if (!ShowRaysOnly) // Draw symbols if ShowRaysOnly is false
                {
                    Draw.Diamond(this, "tagz_vib_bull" + CurrentBar.ToString(), true, 0, Low[0] - 7 * TickSize, VIBDIBBullishColor);
                }
                Print("Bullish VIB DIB Ray and Diamond drawn at bar: " + CurrentBar);
            }

            if (i == Instances)
                i = 0;

            // GIB Detection Logic (Section 2: Gap)
            if (Open[0] > Open[1] && Close[0] > Close[1] && Open[0] > Close[1] && High[1] <= Open[0] && Close[0] > Open[0])
                gibImbBull = true;
            else
                gibImbBull = false;

            if (Open[0] < Open[1] && Close[0] < Close[1] && Open[0] < Close[1] && Low[1] >= Open[0] && Close[0] < Open[0])
                gibImbBear = true;
            else
                gibImbBear = false;

            // Draw GIB markers (Section 2: Gap)
            if (ShowGibs && gibImbBear)
            {
                if (!ShowSymbolsOnly || ShowRaysOnly) // Draw rays if ShowSymbolsOnly is false or ShowRaysOnly is true
                {
                    Draw.Ray(this, "tag3" + i1.ToString(), 1, Open[0], 0, Open[0], GIBCbe);
                    Draw.Ray(this, "tag4" + i1.ToString(), 1, Close[1], 0, Close[1], GIBCbe);
                }
                if (!ShowRaysOnly) // Draw symbols if ShowRaysOnly is false
                {
                    Draw.TriangleDown(this, "tag_triangle_gib_bear" + CurrentBar.ToString(), true, 0, High[0] + 7 * TickSize, GIBCbe); // Bearish Triangle
                }
                if (gibabe) Alert("myAlert", Priority.High, "GIB - Bearish", NinjaTrader.Core.Globals.InstallDir + @"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
                i1++;
            }

            if (ShowGibs && gibImbBull)
            {
                if (!ShowSymbolsOnly || ShowRaysOnly) // Draw rays if ShowSymbolsOnly is false or ShowRaysOnly is true
                {
                    Draw.Ray(this, "tag3" + i1.ToString(), 1, Open[0], 0, Open[0], GIBCbu);
                    Draw.Ray(this, "tag4" + i1.ToString(), 1, Close[1], 0, Close[1], GIBCbu);
                }
                if (!ShowRaysOnly) // Draw symbols if ShowRaysOnly is false
                {
                    Draw.TriangleUp(this, "tag_triangle_gib_bull" + CurrentBar.ToString(), true, 0, Low[0] - 7 * TickSize, GIBCbu); // Bullish Triangle
                }
                if (gibabu) Alert("myAlert", Priority.High, "GIB - Bullish", NinjaTrader.Core.Globals.InstallDir + @"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
                i1++;
            }

            if (i1 == Instances)
                i1 = 0;

            // DIB Logic and Drawing (Section 3: Directional)
            bool dibBull = (Open[0] > Close[0 + 1] && Low[0] <= High[1] && Close[0] > Close[0 + 1] && Open[0] >= Open[0 + 1]) ||
                          (High[0 + 1] < Low[0]);

            bool dibBear = (Open[0] < Close[0 + 1] && Open[0] < Open[0 + 1] && High[0] >= Low[1] && Close[0] < Close[0 + 1] && Close[0] < Open[0 + 1]) ||
                          (Low[0 + 1] > High[0]);

            if (ShowDi && dibBull)
            {
                if (!ShowSymbolsOnly || ShowRaysOnly) // Draw rays if ShowSymbolsOnly is false or ShowRaysOnly is true
                {
                    Draw.Ray(this, "tag5" + CurrentBar.ToString(), 1, Open[0], 0, Open[0], DICbu);
                    Draw.Ray(this, "tag6" + CurrentBar.ToString(), 1, Close[1], 0, Close[1], DICbu);
                }
                if (!ShowRaysOnly) // Draw symbols if ShowRaysOnly is false
                {
                    Draw.Dot(this, "tag5" + CurrentBar.ToString(), true, 0, Low[0] - TickSize, DICbu);
                }
                if (gibabu) Alert("myAlert", Priority.High, "DIB - Bullish", NinjaTrader.Core.Globals.InstallDir + @"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
            }

            if (ShowDi && dibBear)
            {
                if (!ShowSymbolsOnly || ShowRaysOnly) // Draw rays if ShowSymbolsOnly is false or ShowRaysOnly is true
                {
                    Draw.Ray(this, "tag5" + CurrentBar.ToString(), 1, Open[0], 0, Open[0], DICbe);
                    Draw.Ray(this, "tag6" + CurrentBar.ToString(), 1, Close[1], 0, Close[1], DICbe);
                }
                if (!ShowRaysOnly) // Draw symbols if ShowRaysOnly is false
                {
                    Draw.Dot(this, "tag6" + CurrentBar.ToString(), true, 0, High[0] + TickSize, DICbe);
                }
                if (gibabe) Alert("myAlert", Priority.High, "DIB - Bearish", NinjaTrader.Core.Globals.InstallDir + @"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
            }

            // Magic VIBs Logic and Drawing (Section 4: Magic Volume)
            if (ShowMVibs)
            {
                if ((Close[0] < Close[1]) && Close[1] > Open[0] && Low[1] == Open[0] && Low[1] != Open[1])
                {
                    if (!ShowSymbolsOnly || ShowRaysOnly) // Draw rays if ShowSymbolsOnly is false or ShowRaysOnly is true
                    {
                        Draw.Ray(this, "tag7" + CurrentBar.ToString(), 1, Open[0], 0, Open[0], VIBMCbe);
                        Draw.Ray(this, "tag8" + CurrentBar.ToString(), 1, Close[1], 0, Close[1], VIBMCbe);
                    }
                    if (!ShowRaysOnly) // Draw symbols if ShowRaysOnly is false
                    {
                        Draw.Diamond(this, "tag_magic_vib_bear" + CurrentBar.ToString(), true, 0, High[0] + 7 * TickSize, VIBMCbe); // Bearish Diamond
                    }
                    if (mabe) Alert("myAlert", Priority.High, "Magic Vib - Bearish", NinjaTrader.Core.Globals.InstallDir + @"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
                }

                if ((Close[0] > Close[1]) && Close[1] < Open[0] && High[1] == Open[0] && High[1] != Open[1])
                {
                    if (!ShowSymbolsOnly || ShowRaysOnly) // Draw rays if ShowSymbolsOnly is false or ShowRaysOnly is true
                    {
                        Draw.Ray(this, "tag7" + CurrentBar.ToString(), 1, Open[0], 0, Open[0], VIBMCbu);
                        Draw.Ray(this, "tag8" + CurrentBar.ToString(), 1, Close[1], 0, Close[1], VIBMCbu);
                    }
                    if (!ShowRaysOnly) // Draw symbols if ShowRaysOnly is false
                    {
                        Draw.Diamond(this, "tag_magic_vib_bull" + CurrentBar.ToString(), true, 0, Low[0] - 7 * TickSize, VIBMCbu); // Bullish Diamond
                    }
                    if (mabu) Alert("myAlert", Priority.High, "Magic Vib - Bullish", NinjaTrader.Core.Globals.InstallDir + @"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
                }
            }
        }

        #region Properties
        [NinjaScriptProperty]
        [Display(Name = "Show VIBs", Order = 1, GroupName = "1. Volume")]
        public bool ShowVibs
        { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "VIB Bullish", Order = 2, GroupName = "1. Volume")]
        public Brush VIBCbu
        { get; set; }

        [Browsable(false)]
        public string VIBCbuSerializable
        {
            get { return Serialize.BrushToString(VIBCbu); }
            set { VIBCbu = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "VIB Bearish", Order = 3, GroupName = "1. Volume")]
        public Brush VIBCbe
        { get; set; }

        [Browsable(false)]
        public string VIBCbeSerializable
        {
            get { return Serialize.BrushToString(VIBCbe); }
            set { VIBCbe = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty]
        [Display(Name = "Show DIBs", Order = 1, GroupName = "2. Gap")]
        public bool ShowGibs
        { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "GIB Bullish", Order = 2, GroupName = "2. Gap")]
        public Brush GIBCbu
        { get; set; }

        [Browsable(false)]
        public string GIBCbuSerializable
        {
            get { return Serialize.BrushToString(GIBCbu); }
            set { GIBCbu = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "GIB Bearish", Order = 3, GroupName = "2. Gap")]
        public Brush GIBCbe
        { get; set; }

        [Browsable(false)]
        public string GIBCbeSerializable
        {
            get { return Serialize.BrushToString(GIBCbe); }
            set { GIBCbe = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty]
        [Display(Name = "Show DIBs", Order = 1, GroupName = "3. Directional")]
        public bool ShowDi
        { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "DIB Bullish", Order = 2, GroupName = "3. Directional")]
        public Brush DICbu
        { get; set; }

        [Browsable(false)]
        public string DICbuSerializable
        {
            get { return Serialize.BrushToString(DICbu); }
            set { DICbu = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "DIB Bearish", Order = 3, GroupName = "3. Directional")]
        public Brush DICbe
        { get; set; }

        [Browsable(false)]
        public string DICbeSerializable
        {
            get { return Serialize.BrushToString(DICbe); }
            set { DICbe = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty]
        [Display(Name = "Show Magic VIBs", Order = 1, GroupName = "4. Magic Volume")]
        public bool ShowMVibs
        { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Magic VIB Bullish", Order = 2, GroupName = "4. Magic Volume")]
        public Brush VIBMCbu
        { get; set; }

        [Browsable(false)]
        public string VIBMCbuSerializable
        {
            get { return Serialize.BrushToString(VIBMCbu); }
            set { VIBMCbu = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Magic VIB Bearish", Order = 3, GroupName = "4. Magic Volume")]
        public Brush VIBMCbe
        { get; set; }

        [Browsable(false)]
        public string VIBMCbeSerializable
        {
            get { return Serialize.BrushToString(VIBMCbe); }
            set { VIBMCbe = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty]
        [Display(Name = "Show Volume & Directional", Order = 1, GroupName = "5. Volume and Directional")]
        public bool ShowVIBDIB
        { get; set; }

        [NinjaScriptProperty]
        [Browsable(false)]
        [Range(1, int.MaxValue)]
        [Display(Name = "Instances", Order = 2, GroupName = "5. Volume and Directional")]
        public int Instances
        { get; set; }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Volume & Directional Bullish", Order = 2, GroupName = "5. Volume and Directional")]
        public Brush VIBDIBBullishColor
        { get; set; }

        [Browsable(false)]
        public string VIBDIBBullishColorSerializable
        {
            get { return Serialize.BrushToString(VIBDIBBullishColor); }
            set { VIBDIBBullishColor = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty]
        [XmlIgnore]
        [Display(Name = "Volume & Directional Bearish", Order = 3, GroupName = "5. Volume and Directional")]
        public Brush VIBDIBBearishColor
        { get; set; }

        [Browsable(false)]
        public string VIBDIBBearishColorSerializable
        {
            get { return Serialize.BrushToString(VIBDIBBearishColor); }
            set { VIBDIBBearishColor = Serialize.StringToBrush(value); }
        }

        [NinjaScriptProperty]
        [Display(Name = "Show Symbols Only", Order = 4, GroupName = "5. Volume and Directional")]
        public bool ShowSymbolsOnly
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Rays Only", Order = 5, GroupName = "5. Volume and Directional")]
        public bool ShowRaysOnly
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "VIB - Bullish", Order = 0, GroupName = "6. Alerts")]
        public bool vibabu
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "VIB - Bearish", Order = 1, GroupName = "6. Alerts")]
        public bool vibabe
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "GIB - Bullish", Order = 2, GroupName = "6. Alerts")]
        public bool gibabu
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "GIB - Bearish", Order = 3, GroupName = "6. Alerts")]
        public bool gibabe
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "DIB - Bullish", Order = 4, GroupName = "6. Alerts")]
        public bool dibabu
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "DIB - Bearish", Order = 5, GroupName = "6. Alerts")]
        public bool dibabe
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Magic VIB - Bullish", Order = 6, GroupName = "6. Alerts")]
        public bool mabu
        { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Magic VIB - Bearish", Order = 7, GroupName = "6. Alerts")]
        public bool mabe
        { get; set; }

        #endregion
    }
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private Imbalances[] cacheImbalances;
		public Imbalances Imbalances(bool showVibs, Brush vIBCbu, Brush vIBCbe, bool showGibs, Brush gIBCbu, Brush gIBCbe, bool showDi, Brush dICbu, Brush dICbe, bool showMVibs, Brush vIBMCbu, Brush vIBMCbe, bool showVIBDIB, int instances, Brush vIBDIBBullishColor, Brush vIBDIBBearishColor, bool showSymbolsOnly, bool showRaysOnly, bool vibabu, bool vibabe, bool gibabu, bool gibabe, bool dibabu, bool dibabe, bool mabu, bool mabe)
		{
			return Imbalances(Input, showVibs, vIBCbu, vIBCbe, showGibs, gIBCbu, gIBCbe, showDi, dICbu, dICbe, showMVibs, vIBMCbu, vIBMCbe, showVIBDIB, instances, vIBDIBBullishColor, vIBDIBBearishColor, showSymbolsOnly, showRaysOnly, vibabu, vibabe, gibabu, gibabe, dibabu, dibabe, mabu, mabe);
		}

		public Imbalances Imbalances(ISeries<double> input, bool showVibs, Brush vIBCbu, Brush vIBCbe, bool showGibs, Brush gIBCbu, Brush gIBCbe, bool showDi, Brush dICbu, Brush dICbe, bool showMVibs, Brush vIBMCbu, Brush vIBMCbe, bool showVIBDIB, int instances, Brush vIBDIBBullishColor, Brush vIBDIBBearishColor, bool showSymbolsOnly, bool showRaysOnly, bool vibabu, bool vibabe, bool gibabu, bool gibabe, bool dibabu, bool dibabe, bool mabu, bool mabe)
		{
			if (cacheImbalances != null)
				for (int idx = 0; idx < cacheImbalances.Length; idx++)
					if (cacheImbalances[idx] != null && cacheImbalances[idx].ShowVibs == showVibs && cacheImbalances[idx].VIBCbu == vIBCbu && cacheImbalances[idx].VIBCbe == vIBCbe && cacheImbalances[idx].ShowGibs == showGibs && cacheImbalances[idx].GIBCbu == gIBCbu && cacheImbalances[idx].GIBCbe == gIBCbe && cacheImbalances[idx].ShowDi == showDi && cacheImbalances[idx].DICbu == dICbu && cacheImbalances[idx].DICbe == dICbe && cacheImbalances[idx].ShowMVibs == showMVibs && cacheImbalances[idx].VIBMCbu == vIBMCbu && cacheImbalances[idx].VIBMCbe == vIBMCbe && cacheImbalances[idx].ShowVIBDIB == showVIBDIB && cacheImbalances[idx].Instances == instances && cacheImbalances[idx].VIBDIBBullishColor == vIBDIBBullishColor && cacheImbalances[idx].VIBDIBBearishColor == vIBDIBBearishColor && cacheImbalances[idx].ShowSymbolsOnly == showSymbolsOnly && cacheImbalances[idx].ShowRaysOnly == showRaysOnly && cacheImbalances[idx].vibabu == vibabu && cacheImbalances[idx].vibabe == vibabe && cacheImbalances[idx].gibabu == gibabu && cacheImbalances[idx].gibabe == gibabe && cacheImbalances[idx].dibabu == dibabu && cacheImbalances[idx].dibabe == dibabe && cacheImbalances[idx].mabu == mabu && cacheImbalances[idx].mabe == mabe && cacheImbalances[idx].EqualsInput(input))
						return cacheImbalances[idx];
			return CacheIndicator<Imbalances>(new Imbalances(){ ShowVibs = showVibs, VIBCbu = vIBCbu, VIBCbe = vIBCbe, ShowGibs = showGibs, GIBCbu = gIBCbu, GIBCbe = gIBCbe, ShowDi = showDi, DICbu = dICbu, DICbe = dICbe, ShowMVibs = showMVibs, VIBMCbu = vIBMCbu, VIBMCbe = vIBMCbe, ShowVIBDIB = showVIBDIB, Instances = instances, VIBDIBBullishColor = vIBDIBBullishColor, VIBDIBBearishColor = vIBDIBBearishColor, ShowSymbolsOnly = showSymbolsOnly, ShowRaysOnly = showRaysOnly, vibabu = vibabu, vibabe = vibabe, gibabu = gibabu, gibabe = gibabe, dibabu = dibabu, dibabe = dibabe, mabu = mabu, mabe = mabe }, input, ref cacheImbalances);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.Imbalances Imbalances(bool showVibs, Brush vIBCbu, Brush vIBCbe, bool showGibs, Brush gIBCbu, Brush gIBCbe, bool showDi, Brush dICbu, Brush dICbe, bool showMVibs, Brush vIBMCbu, Brush vIBMCbe, bool showVIBDIB, int instances, Brush vIBDIBBullishColor, Brush vIBDIBBearishColor, bool showSymbolsOnly, bool showRaysOnly, bool vibabu, bool vibabe, bool gibabu, bool gibabe, bool dibabu, bool dibabe, bool mabu, bool mabe)
		{
			return indicator.Imbalances(Input, showVibs, vIBCbu, vIBCbe, showGibs, gIBCbu, gIBCbe, showDi, dICbu, dICbe, showMVibs, vIBMCbu, vIBMCbe, showVIBDIB, instances, vIBDIBBullishColor, vIBDIBBearishColor, showSymbolsOnly, showRaysOnly, vibabu, vibabe, gibabu, gibabe, dibabu, dibabe, mabu, mabe);
		}

		public Indicators.Imbalances Imbalances(ISeries<double> input , bool showVibs, Brush vIBCbu, Brush vIBCbe, bool showGibs, Brush gIBCbu, Brush gIBCbe, bool showDi, Brush dICbu, Brush dICbe, bool showMVibs, Brush vIBMCbu, Brush vIBMCbe, bool showVIBDIB, int instances, Brush vIBDIBBullishColor, Brush vIBDIBBearishColor, bool showSymbolsOnly, bool showRaysOnly, bool vibabu, bool vibabe, bool gibabu, bool gibabe, bool dibabu, bool dibabe, bool mabu, bool mabe)
		{
			return indicator.Imbalances(input, showVibs, vIBCbu, vIBCbe, showGibs, gIBCbu, gIBCbe, showDi, dICbu, dICbe, showMVibs, vIBMCbu, vIBMCbe, showVIBDIB, instances, vIBDIBBullishColor, vIBDIBBearishColor, showSymbolsOnly, showRaysOnly, vibabu, vibabe, gibabu, gibabe, dibabu, dibabe, mabu, mabe);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.Imbalances Imbalances(bool showVibs, Brush vIBCbu, Brush vIBCbe, bool showGibs, Brush gIBCbu, Brush gIBCbe, bool showDi, Brush dICbu, Brush dICbe, bool showMVibs, Brush vIBMCbu, Brush vIBMCbe, bool showVIBDIB, int instances, Brush vIBDIBBullishColor, Brush vIBDIBBearishColor, bool showSymbolsOnly, bool showRaysOnly, bool vibabu, bool vibabe, bool gibabu, bool gibabe, bool dibabu, bool dibabe, bool mabu, bool mabe)
		{
			return indicator.Imbalances(Input, showVibs, vIBCbu, vIBCbe, showGibs, gIBCbu, gIBCbe, showDi, dICbu, dICbe, showMVibs, vIBMCbu, vIBMCbe, showVIBDIB, instances, vIBDIBBullishColor, vIBDIBBearishColor, showSymbolsOnly, showRaysOnly, vibabu, vibabe, gibabu, gibabe, dibabu, dibabe, mabu, mabe);
		}

		public Indicators.Imbalances Imbalances(ISeries<double> input , bool showVibs, Brush vIBCbu, Brush vIBCbe, bool showGibs, Brush gIBCbu, Brush gIBCbe, bool showDi, Brush dICbu, Brush dICbe, bool showMVibs, Brush vIBMCbu, Brush vIBMCbe, bool showVIBDIB, int instances, Brush vIBDIBBullishColor, Brush vIBDIBBearishColor, bool showSymbolsOnly, bool showRaysOnly, bool vibabu, bool vibabe, bool gibabu, bool gibabe, bool dibabu, bool dibabe, bool mabu, bool mabe)
		{
			return indicator.Imbalances(input, showVibs, vIBCbu, vIBCbe, showGibs, gIBCbu, gIBCbe, showDi, dICbu, dICbe, showMVibs, vIBMCbu, vIBMCbe, showVIBDIB, instances, vIBDIBBullishColor, vIBDIBBearishColor, showSymbolsOnly, showRaysOnly, vibabu, vibabe, gibabu, gibabe, dibabu, dibabe, mabu, mabe);
		}
	}
}

#endregion
