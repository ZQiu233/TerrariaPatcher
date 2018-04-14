using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CheatTool
{
	internal class IUtils
	{
		public static Vector2 MouseToWrold(Vector2 Mouse)
		{
			var v = new Vector2(Terraria.Main.mapFullscreenPos.X - ((Terraria.Main.screenWidth / 2f) - Mouse.X) / Terraria.Main.mapFullscreenScale, Terraria.Main.mapFullscreenPos.Y - ((Terraria.Main.screenHeight / 2f) - Mouse.Y) / Terraria.Main.mapFullscreenScale);
			v.X = v.X >= Terraria.Main.maxTilesX ? Terraria.Main.maxTilesX : v.X;
			v.Y = v.Y >= Terraria.Main.maxTilesY ? Terraria.Main.maxTilesY : v.Y;
			return v;
		}
		public static void HitNPC(NPC npc, int damage, float knockBack = 0, int hitDirection = 0, bool crit = false, bool noEffect = false, bool fromNet = false)
		{
			npc.StrikeNPC(damage, knockBack, hitDirection, crit, noEffect, fromNet);
			if (Terraria.Main.netMode == 1)
			{
				NetMessage.SendData(28, -1, -1, null, npc.whoAmI, (float)damage, knockBack, (float)hitDirection, crit.ToInt(), 0, 0);
			}
		}
		public static void RevealMap()
		{
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					Main.Map.UpdateLighting(i, j, 255);
				}
			}
			Main.refreshMap = true;
		}
	}
}
