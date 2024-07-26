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
		
		public Imbalances()
        {
            VendorLicense("DezoAlgoLLC", "Imbalances", "dezoalgo.com", "dezoalgo@gmail.com");
        }
        private double esa;
		
		private bool varImbBull ,varImbBear = false ;
		
		
		
		private bool gibImbBull ,gibImbBear = false ;
		
		
		private int i = 0;
		
		private int i1 = 0;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "Imbalances";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				Showcats					= false;
				Instances					= 25;
				
				
				
				ShowVibs					= true;
				VIBCbu					= Brushes.Green;
				VIBCbe					= Brushes.Red;
				
				
				
				ShowGibs					= true;
				GIBCbu					= Brushes.Blue;
				GIBCbe					= Brushes.Fuchsia;
				
				
				ShowDi					= true;
				DICbu					= Brushes.DarkGreen;
				DICbe					= Brushes.DarkRed;
				
				
				
				ShowMVibs					= true;
				VIBMCbu					= Brushes.Yellow;
				VIBMCbe					= Brushes.Maroon;
				
				
				
				vibabu = false;
				vibabe =false;
				
				
				
				gibabu = false;
				gibabe =false;
				
				
				
				dibabu = false;
				dibabe =false;
				
				
				mabu = false;
				mabe =false;
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
			
			if(CurrentBar < 2)
				return;
			
			if(Open[0] > Open[1] ){
				if(Close[0] > Close[1]){
					if(Open[0] > Close[1]){
						if(Low[0] <= Close[1]){
							if(Close[0] > Open[0]){
								varImbBull = true;
			                }
							else{
								varImbBull = false;
			                }
						}
						else{
							varImbBull = false;
			            }
					}
					else{
					    varImbBull = false;
			        }
				}
				else{
					    varImbBull = false;
			       }
			}
			else
				varImbBull = false;
			
			
			
			
			
			
			if(Open[0] < Open[1] ){
				if(Close[0] < Close[1]){
					if(Open[0] < Close[1]){
						if( Close[1] <= High[0]){
							if(Close[0] < Open[0]){
								varImbBear = true;
			                }
							else{
								varImbBear = false;
			                }
						}
						else{
							varImbBear = false;
			            }
					}
					else{
					    varImbBear = false;
			        }
				}
				else{
					    varImbBear = false;
			       }
			}
			else
				varImbBear = false;
			
			
			
			if(ShowVibs && varImbBear){
				//Draw.Line(this, "tag1" + i.ToString(), false, 1, Open[0], 0, Close[1], VIBCbe, DashStyleHelper.Solid, 4);
				Draw.Ray(this, "tag1" + i.ToString() , 1, Open[0], 0, Open[0], VIBCbe);
				Draw.Ray(this, "tag2" + i.ToString() , 1, Close[1], 0, Close[1], VIBCbe);


				i++;
				
				if(vibabe) Alert("myAlert", Priority.High, "VIB - Bearish", NinjaTrader.Core.Globals.InstallDir+@"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
				
				if(Showcats) Draw.Diamond(this, "tagz" + CurrentBar.ToString() , true, 0, High[0] +7* TickSize, Brushes.Blue);


			}
			
			
			if(ShowVibs && varImbBull){
				//Draw.Line(this, "tag1" + i.ToString(), false, 1, Open[0], 0, Close[1], VIBCbe, DashStyleHelper.Solid, 4);
				Draw.Ray(this, "tag1" + i.ToString() , 1, Open[0], 0, Open[0], VIBCbu);
				Draw.Ray(this, "tag2" + i.ToString() , 1, Close[1], 0, Close[1], VIBCbu);

                if(vibabe) Alert("myAlert", Priority.High, "VIB - Bullish", NinjaTrader.Core.Globals.InstallDir+@"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
				i++;
				
				if(Showcats) Draw.Diamond(this, "tagz" + CurrentBar.ToString() , true, 0, Low[0] -7* TickSize, Brushes.Blue);
			}
			

			
			if(i== Instances )
				i = 0;
			
			
			
			// GIB
			
			if(Open[0] > Open[1] ){
				if(Close[0] > Close[1]){
					if(Open[0] > Close[1]){
						if(High[1] <= Open[0]){
							if(Close[0] > Open[0]){
								gibImbBull = true;
			                }
							else{
								gibImbBull = false;
			                }
						}
						else{
							gibImbBull = false;
			            }
					}
					else{
					    gibImbBull = false;
			        }
				}
				else{
					    gibImbBull = false;
			       }
			}
			else
				gibImbBull = false;

			
			
			if(Open[0] < Open[1] ){
				if(Close[0] < Close[1]){
					if(Open[0] < Close[1]){
						if(Low[1] >= Open[0]){
							if(Close[0] < Open[0]){
								gibImbBear = true;
			                }
							else{
								gibImbBear = false;
			                }
						}
						else{
							gibImbBear = false;
			            }
					}
					else{
					    gibImbBear = false;
			        }
				}
				else{
					    gibImbBear = false;
			       }
			}
			else
				gibImbBear = false;
			
			
			if(ShowGibs && gibImbBear){
				//Draw.Line(this, "tag1" + i.ToString(), false, 1, Open[0], 0, Close[1], VIBCbe, DashStyleHelper.Solid, 4);
				Draw.Ray(this, "tag3" + i1.ToString() , 1, Open[0], 0, Open[0], GIBCbe);
				Draw.Ray(this, "tag4" + i1.ToString() , 1, Close[1], 0, Close[1], GIBCbe);
				
				
				if(gibabe)  Alert("myAlert", Priority.High, "GIB - Bearish", NinjaTrader.Core.Globals.InstallDir+@"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);


				i1++;
				
				if(Showcats) Draw.TriangleUp(this, "tagz" + CurrentBar.ToString() , true, 0, High[0] +7* TickSize, Brushes.Blue);
			}
			
			
			if(ShowGibs && gibImbBull){
				//Draw.Line(this, "tag1" + i.ToString(), false, 1, Open[0], 0, Close[1], VIBCbe, DashStyleHelper.Solid, 4);
				Draw.Ray(this, "tag3" + i1.ToString() , 1, Open[0], 0, Open[0], GIBCbu);
				Draw.Ray(this, "tag4" + i1.ToString() , 1, Close[1], 0, Close[1], GIBCbu);


				if(gibabu)  Alert("myAlert", Priority.High, "GIB - Bullish", NinjaTrader.Core.Globals.InstallDir+@"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
				i1++;
				
				if(Showcats) Draw.TriangleUp(this, "tagz" + CurrentBar.ToString() , true, 0, Low[0] -7* TickSize, Brushes.Blue);
			}
			
			
			if(i1 == Instances )
				i1 = 0;
			
			
			
			
			
			
			if(ShowDi && Open[0] > Close[0+1] && Low[0] <= High[1] && Close[0] > Close[0+1] && Open[0] >= Open[0+1]){
				Draw.Dot(this, "tag5" + CurrentBar.ToString(), true, 0, Low[0] - TickSize, DICbu);
				
				if(gibabu)  Alert("myAlert", Priority.High, "DIB - Bullish", NinjaTrader.Core.Globals.InstallDir+@"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
			}
			if(ShowDi && Open[0] < Close[0+1] && Open[0] < Open[0+1] && High[0] >= Low[1] && Close[0] < Close[0+1] && Close[0] < Open[0+1]){
				Draw.Dot(this, "tag6" + CurrentBar.ToString(), true, 0, High[0] + TickSize, DICbe);
				if(gibabe)  Alert("myAlert", Priority.High, "DIB - Bearish", NinjaTrader.Core.Globals.InstallDir+@"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
				
				
            }

			if(ShowDi && High[0+1] < Low[0]){
				Draw.Dot(this, "tag5" + CurrentBar.ToString(), true, 0, Low[0] - TickSize, DICbu);
				if(gibabu)  Alert("myAlert", Priority.High, "DIB - Bullish", NinjaTrader.Core.Globals.InstallDir+@"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
            }
			if(ShowDi && Low[0+1] > High[0]){
				Draw.Dot(this, "tag5" + CurrentBar.ToString(), true, 0, High[0] - TickSize, DICbe);
				if(gibabe)  Alert("myAlert", Priority.High, "DIB - Bearish", NinjaTrader.Core.Globals.InstallDir+@"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
			}
			
			
			
			if((Close[0] < Close[1]) && ShowMVibs)
              if(Close[1] > Open[0])
                  if(Low[1] == Open[0])
                    if(Low[1] != Open[1]){
						Draw.Ray(this, "tag7" + CurrentBar.ToString() , 1, Open[0], 0, Open[0], VIBMCbe);
				        Draw.Ray(this, "tag8" + CurrentBar.ToString() , 1, Close[1], 0, Close[1], VIBMCbe);
						if(mabe)  Alert("myAlert", Priority.High, "Magic Vib - Bearish", NinjaTrader.Core.Globals.InstallDir+@"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
						
					}
					
			if((Close[0] > Close[1]) && ShowMVibs)
              if(Close[1] < Open[0])
                if(High[1] == Open[0])
                  if (High[1] != Open[1]){
						Draw.Ray(this, "tag7" + CurrentBar.ToString() , 1, Open[0], 0, Open[0], VIBMCbu);
				        Draw.Ray(this, "tag8" + CurrentBar.ToString() , 1, Close[1], 0, Close[1], VIBMCbu);
					   if(mabu)  Alert("myAlert", Priority.High, "Magic Vib - Bullish", NinjaTrader.Core.Globals.InstallDir+@"\sounds\Alert1.wav", 10, Brushes.Black, Brushes.Yellow);
				  }	
                  
                
			
			
		}

		#region Properties
		[NinjaScriptProperty]
        [Browsable(false)]
        [Display(Name="Showcats", Order=1, GroupName="1-VIB / GIB Settings")]
		public bool Showcats
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Instances", Order=2, GroupName="1-VIB / GIB Settings")]
		public int Instances
		{ get; set; }

		[NinjaScriptProperty]
		[Display(Name="Show VIBs", Order=3, GroupName="2-VIB")]
		public bool ShowVibs
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Bullish", Order=4, GroupName="2-VIB")]
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
		[Display(Name="Bearish", Order=5, GroupName="2-VIB")]
		public Brush VIBCbe
		{ get; set; }

		[Browsable(false)]
		public string VIBCbeSerializable
		{
			get { return Serialize.BrushToString(VIBCbe); }
			set { VIBCbe = Serialize.StringToBrush(value); }
		}		
		
		
		//
		[NinjaScriptProperty]
		[Display(Name="Show DIBs", Order=0, GroupName="3-GIB")]
		public bool ShowGibs
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Bullish", Order=1, GroupName="3-GIB")]
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
		[Display(Name="Bearish", Order=2, GroupName="3-GIB")]
		public Brush GIBCbe
		{ get; set; }

		[Browsable(false)]
		public string GIBCbeSerializable
		{
			get { return Serialize.BrushToString(GIBCbe); }
			set { GIBCbe = Serialize.StringToBrush(value); }
		}
		
		
		
		
		//
		[NinjaScriptProperty]
		[Display(Name="Show DIBs", Order=0, GroupName="4-DIB")]
		public bool ShowDi
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Bullish", Order=1, GroupName="4-DIB")]
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
		[Display(Name="Bearish", Order=2, GroupName="4-DIB")]
		public Brush DICbe
		{ get; set; }

		[Browsable(false)]
		public string DICbeSerializable
		{
			get { return Serialize.BrushToString(DICbe); }
			set { DICbe = Serialize.StringToBrush(value); }
		}
		
		
		
		//
		
		
		
		[NinjaScriptProperty]
		[Display(Name="Show Magic VIBs", Order=3, GroupName="5-Magic VIB")]
		public bool ShowMVibs
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Bullish", Order=4, GroupName="5-Magic VIB")]
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
		[Display(Name="Bearish", Order=5, GroupName="5-Magic VIB")]
		public Brush VIBMCbe
		{ get; set; }

		[Browsable(false)]
		public string VIBMCbeSerializable
		{
			get { return Serialize.BrushToString(VIBMCbe); }
			set { VIBMCbe = Serialize.StringToBrush(value); }
		}
		
		
		
		
		
		
		[NinjaScriptProperty]
		[Display(Name="VIB - Bullish", Order=0, GroupName="6-Alerts")]
		public bool vibabu
		{ get; set; }
		
		
		[NinjaScriptProperty]
		[Display(Name="VIB - Bearish", Order=1, GroupName="6-Alerts")]
		public bool vibabe
		{ get; set; }
		
		
		
		[NinjaScriptProperty]
		[Display(Name="GIB - Bullish", Order=2, GroupName="6-Alerts")]
		public bool gibabu
		{ get; set; }
		
		
		[NinjaScriptProperty]
		[Display(Name="GIB - Bearish", Order=3, GroupName="6-Alerts")]
		public bool gibabe
		{ get; set; }
		
		
		[NinjaScriptProperty]
		[Display(Name="DIB - Bullish", Order=4, GroupName="6-Alerts")]
		public bool dibabu
		{ get; set; }
		
		
		[NinjaScriptProperty]
		[Display(Name="DIB - Bearish", Order=5, GroupName="6-Alerts")]
		public bool dibabe
		{ get; set; }
		
		
		
		[NinjaScriptProperty]
		[Display(Name="Magic VIB - Bullish", Order=6, GroupName="6-Alerts")]
		public bool mabu
		{ get; set; }
		
		
		[NinjaScriptProperty]
		[Display(Name="Magic VIB - Bearish", Order=7, GroupName="6-Alerts")]
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
		public Imbalances Imbalances(bool showcats, int instances, bool showVibs, Brush vIBCbu, Brush vIBCbe, bool showGibs, Brush gIBCbu, Brush gIBCbe, bool showDi, Brush dICbu, Brush dICbe, bool showMVibs, Brush vIBMCbu, Brush vIBMCbe, bool vibabu, bool vibabe, bool gibabu, bool gibabe, bool dibabu, bool dibabe, bool mabu, bool mabe)
		{
			return Imbalances(Input, showcats, instances, showVibs, vIBCbu, vIBCbe, showGibs, gIBCbu, gIBCbe, showDi, dICbu, dICbe, showMVibs, vIBMCbu, vIBMCbe, vibabu, vibabe, gibabu, gibabe, dibabu, dibabe, mabu, mabe);
		}

		public Imbalances Imbalances(ISeries<double> input, bool showcats, int instances, bool showVibs, Brush vIBCbu, Brush vIBCbe, bool showGibs, Brush gIBCbu, Brush gIBCbe, bool showDi, Brush dICbu, Brush dICbe, bool showMVibs, Brush vIBMCbu, Brush vIBMCbe, bool vibabu, bool vibabe, bool gibabu, bool gibabe, bool dibabu, bool dibabe, bool mabu, bool mabe)
		{
			if (cacheImbalances != null)
				for (int idx = 0; idx < cacheImbalances.Length; idx++)
					if (cacheImbalances[idx] != null && cacheImbalances[idx].Showcats == showcats && cacheImbalances[idx].Instances == instances && cacheImbalances[idx].ShowVibs == showVibs && cacheImbalances[idx].VIBCbu == vIBCbu && cacheImbalances[idx].VIBCbe == vIBCbe && cacheImbalances[idx].ShowGibs == showGibs && cacheImbalances[idx].GIBCbu == gIBCbu && cacheImbalances[idx].GIBCbe == gIBCbe && cacheImbalances[idx].ShowDi == showDi && cacheImbalances[idx].DICbu == dICbu && cacheImbalances[idx].DICbe == dICbe && cacheImbalances[idx].ShowMVibs == showMVibs && cacheImbalances[idx].VIBMCbu == vIBMCbu && cacheImbalances[idx].VIBMCbe == vIBMCbe && cacheImbalances[idx].vibabu == vibabu && cacheImbalances[idx].vibabe == vibabe && cacheImbalances[idx].gibabu == gibabu && cacheImbalances[idx].gibabe == gibabe && cacheImbalances[idx].dibabu == dibabu && cacheImbalances[idx].dibabe == dibabe && cacheImbalances[idx].mabu == mabu && cacheImbalances[idx].mabe == mabe && cacheImbalances[idx].EqualsInput(input))
						return cacheImbalances[idx];
			return CacheIndicator<Imbalances>(new Imbalances(){ Showcats = showcats, Instances = instances, ShowVibs = showVibs, VIBCbu = vIBCbu, VIBCbe = vIBCbe, ShowGibs = showGibs, GIBCbu = gIBCbu, GIBCbe = gIBCbe, ShowDi = showDi, DICbu = dICbu, DICbe = dICbe, ShowMVibs = showMVibs, VIBMCbu = vIBMCbu, VIBMCbe = vIBMCbe, vibabu = vibabu, vibabe = vibabe, gibabu = gibabu, gibabe = gibabe, dibabu = dibabu, dibabe = dibabe, mabu = mabu, mabe = mabe }, input, ref cacheImbalances);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.Imbalances Imbalances(bool showcats, int instances, bool showVibs, Brush vIBCbu, Brush vIBCbe, bool showGibs, Brush gIBCbu, Brush gIBCbe, bool showDi, Brush dICbu, Brush dICbe, bool showMVibs, Brush vIBMCbu, Brush vIBMCbe, bool vibabu, bool vibabe, bool gibabu, bool gibabe, bool dibabu, bool dibabe, bool mabu, bool mabe)
		{
			return indicator.Imbalances(Input, showcats, instances, showVibs, vIBCbu, vIBCbe, showGibs, gIBCbu, gIBCbe, showDi, dICbu, dICbe, showMVibs, vIBMCbu, vIBMCbe, vibabu, vibabe, gibabu, gibabe, dibabu, dibabe, mabu, mabe);
		}

		public Indicators.Imbalances Imbalances(ISeries<double> input , bool showcats, int instances, bool showVibs, Brush vIBCbu, Brush vIBCbe, bool showGibs, Brush gIBCbu, Brush gIBCbe, bool showDi, Brush dICbu, Brush dICbe, bool showMVibs, Brush vIBMCbu, Brush vIBMCbe, bool vibabu, bool vibabe, bool gibabu, bool gibabe, bool dibabu, bool dibabe, bool mabu, bool mabe)
		{
			return indicator.Imbalances(input, showcats, instances, showVibs, vIBCbu, vIBCbe, showGibs, gIBCbu, gIBCbe, showDi, dICbu, dICbe, showMVibs, vIBMCbu, vIBMCbe, vibabu, vibabe, gibabu, gibabe, dibabu, dibabe, mabu, mabe);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.Imbalances Imbalances(bool showcats, int instances, bool showVibs, Brush vIBCbu, Brush vIBCbe, bool showGibs, Brush gIBCbu, Brush gIBCbe, bool showDi, Brush dICbu, Brush dICbe, bool showMVibs, Brush vIBMCbu, Brush vIBMCbe, bool vibabu, bool vibabe, bool gibabu, bool gibabe, bool dibabu, bool dibabe, bool mabu, bool mabe)
		{
			return indicator.Imbalances(Input, showcats, instances, showVibs, vIBCbu, vIBCbe, showGibs, gIBCbu, gIBCbe, showDi, dICbu, dICbe, showMVibs, vIBMCbu, vIBMCbe, vibabu, vibabe, gibabu, gibabe, dibabu, dibabe, mabu, mabe);
		}

		public Indicators.Imbalances Imbalances(ISeries<double> input , bool showcats, int instances, bool showVibs, Brush vIBCbu, Brush vIBCbe, bool showGibs, Brush gIBCbu, Brush gIBCbe, bool showDi, Brush dICbu, Brush dICbe, bool showMVibs, Brush vIBMCbu, Brush vIBMCbe, bool vibabu, bool vibabe, bool gibabu, bool gibabe, bool dibabu, bool dibabe, bool mabu, bool mabe)
		{
			return indicator.Imbalances(input, showcats, instances, showVibs, vIBCbu, vIBCbe, showGibs, gIBCbu, gIBCbe, showDi, dICbu, dICbe, showMVibs, vIBMCbu, vIBMCbe, vibabu, vibabe, gibabu, gibabe, dibabu, dibabe, mabu, mabe);
		}
	}
}

#endregion
