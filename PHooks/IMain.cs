using Microsoft.Xna.Framework;
using PBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace PHooks
{
	[PPatch(PPatchOption.Merge, "Terraria", "Main", "None", true, true, true)]
	internal class IMain : Main
	{
		[PMethod(PMethodOption.Replace, "DoUpdate", "None")]
		public void Update_Hooked(GameTime time)
		{
			Hooks.Update.DispatchPre(this, time);
			DoUpdate(time);
			Hooks.Update.DispatchAfter(this, time);
		}
		[PMethod(PMethodOption.Replace, "DoDraw", "None")]
		public void Draw_Hooked(GameTime time)
		{
			Hooks.Draw.DispatchPre(this, time);
			DoDraw(time);
			Hooks.Draw.DispatchAfter(this, time);
		}
		[PMethod(PMethodOption.Replace, "SetupDrawInterfaceLayers", "None")]
		public void SetupDrawInterfaceLayers_Hooked()
		{
			Hooks.InterfaceLayersSetup.DispatchPre(this);
			SetupDrawInterfaceLayers();
			Hooks.InterfaceLayersSetup.DispatchAfter(this);
		}
	}
}
