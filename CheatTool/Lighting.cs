using Microsoft.Xna.Framework;
using PBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace CheatTool
{
	[PPatch(PPatchOption.Merge, "Terraria", "Lighting", "None")]
	class Lighting : Terraria.Lighting
	{
		[PMethod(PMethodOption.Replace, "LightTiles", "None")]
		public static void LightTiles_Patched(int firstX, int lastX, int firstY, int lastY)
		{
			Main.render = true;
			Terraria.Lighting.oldSkyColor = Terraria.Lighting.skyColor;
			float num = (float)Main.tileColor.R / 255f;
			float num2 = (float)Main.tileColor.G / 255f;
			float num3 = (float)Main.tileColor.B / 255f;
			Terraria.Lighting.skyColor = (num + num2 + num3) / 3f;
			if (Terraria.Lighting.lightMode < 2)
			{
				Terraria.Lighting.brightness = 1.2f;
				Terraria.Lighting.offScreenTiles2 = 34;
				Terraria.Lighting.offScreenTiles = 40;
			}
			else
			{
				Terraria.Lighting.brightness = 1f;
				Terraria.Lighting.offScreenTiles2 = 18;
				Terraria.Lighting.offScreenTiles = 23;
			}
			Terraria.Lighting.brightness = 1.2f;
			if (Main.player[Main.myPlayer].blind)
			{
				Terraria.Lighting.brightness = 1f;
			}
			Terraria.Lighting.defBrightness = Terraria.Lighting.brightness;
			Terraria.Lighting.firstTileX = firstX;
			Terraria.Lighting.lastTileX = lastX;
			Terraria.Lighting.firstTileY = firstY;
			Terraria.Lighting.lastTileY = lastY;
			Terraria.Lighting.firstToLightX = Terraria.Lighting.firstTileX - Terraria.Lighting.offScreenTiles;
			Terraria.Lighting.firstToLightY = Terraria.Lighting.firstTileY - Terraria.Lighting.offScreenTiles;
			Terraria.Lighting.lastToLightX = Terraria.Lighting.lastTileX + Terraria.Lighting.offScreenTiles;
			Terraria.Lighting.lastToLightY = Terraria.Lighting.lastTileY + Terraria.Lighting.offScreenTiles;
			Terraria.Lighting.lightCounter++;
			Main.renderCount++;
			int num4 = Main.screenWidth / 16 + Terraria.Lighting.offScreenTiles * 2;
			int num5 = Main.screenHeight / 16 + Terraria.Lighting.offScreenTiles * 2;
			Vector2 vector = Main.screenLastPosition;
			if (Main.renderCount < 3)
			{
				Terraria.Lighting.doColors();
			}
			if (Main.renderCount == 2)
			{
				vector = Main.screenPosition;
				int num6 = (int)Math.Floor((double)(Main.screenPosition.X / 16f)) - Terraria.Lighting.scrX;
				int num7 = (int)Math.Floor((double)(Main.screenPosition.Y / 16f)) - Terraria.Lighting.scrY;
				if (num6 > 16)
				{
					num6 = 0;
				}
				if (num7 > 16)
				{
					num7 = 0;
				}
				int num8 = 0;
				int num9 = num4;
				int num10 = 0;
				int num11 = num5;
				if (num6 < 0)
				{
					num8 -= num6;
				}
				else
				{
					num9 -= num6;
				}
				if (num7 < 0)
				{
					num10 -= num7;
				}
				else
				{
					num11 -= num7;
				}
				if (Terraria.Lighting.RGB)
				{
					int num12 = num4;
					if (Terraria.Lighting.states.Length <= num12 + num6)
					{
						num12 = Terraria.Lighting.states.Length - num6 - 1;
					}
					for (int i = num8; i < num12; i++)
					{
						Terraria.Lighting.LightingState[] array = Terraria.Lighting.states[i];
						Terraria.Lighting.LightingState[] array2 = Terraria.Lighting.states[i + num6];
						int num13 = num11;
						if (array2.Length <= num13 + num6)
						{
							num13 = array2.Length - num7 - 1;
						}
						for (int j = num10; j < num13; j++)
						{
							Terraria.Lighting.LightingState arg_276_0 = array[j];
							Terraria.Lighting.LightingState lightingState = array2[j + num7];
							if (CheatTool.FullBright)
							{
								lightingState.r2 = 0.5f;
								lightingState.g2 = 0.5f;
								lightingState.b2 = 0.5f;
							}
							arg_276_0.r = lightingState.r2;
							arg_276_0.g = lightingState.g2;
							arg_276_0.b = lightingState.b2;
						}
					}
				}
				else
				{
					int num14 = num9;
					if (Terraria.Lighting.states.Length <= num14 + num6)
					{
						num14 = Terraria.Lighting.states.Length - num6 - 1;
					}
					for (int k = num8; k < num14; k++)
					{
						Terraria.Lighting.LightingState[] array3 = Terraria.Lighting.states[k];
						Terraria.Lighting.LightingState[] array4 = Terraria.Lighting.states[k + num6];
						int num15 = num11;
						if (array4.Length <= num15 + num6)
						{
							num15 = array4.Length - num7 - 1;
						}
						for (int l = num10; l < num15; l++)
						{
							Terraria.Lighting.LightingState arg_328_0 = array3[l];
							Terraria.Lighting.LightingState lightingState2 = array4[l + num7];
							if (CheatTool.FullBright) lightingState2.r2 = 0.5f;
							arg_328_0.r = lightingState2.r2;
							arg_328_0.g = lightingState2.r2;
							arg_328_0.b = lightingState2.r2;
						}
					}
				}
			}
			else if (!Main.renderNow)
			{
				int num16 = (int)Math.Floor((double)(Main.screenPosition.X / 16f)) - (int)Math.Floor((double)(vector.X / 16f));
				if (num16 > 5 || num16 < -5)
				{
					num16 = 0;
				}
				int num17;
				int num18;
				int num19;
				if (num16 < 0)
				{
					num17 = -1;
					num16 *= -1;
					num18 = num4;
					num19 = num16;
				}
				else
				{
					num17 = 1;
					num18 = 0;
					num19 = num4 - num16;
				}
				int num20 = (int)Math.Floor((double)(Main.screenPosition.Y / 16f)) - (int)Math.Floor((double)(vector.Y / 16f));
				if (num20 > 5 || num20 < -5)
				{
					num20 = 0;
				}
				int num21;
				int num22;
				int num23;
				if (num20 < 0)
				{
					num21 = -1;
					num20 *= -1;
					num22 = num5;
					num23 = num20;
				}
				else
				{
					num21 = 1;
					num22 = 0;
					num23 = num5 - num20;
				}
				if (num16 != 0 || num20 != 0)
				{
					for (int num24 = num18; num24 != num19; num24 += num17)
					{
						Terraria.Lighting.LightingState[] array5 = Terraria.Lighting.states[num24];
						Terraria.Lighting.LightingState[] array6 = Terraria.Lighting.states[num24 + num16 * num17];
						for (int num25 = num22; num25 != num23; num25 += num21)
						{
							Terraria.Lighting.LightingState arg_478_0 = array5[num25];
							Terraria.Lighting.LightingState lightingState3 = array6[num25 + num20 * num21];
							arg_478_0.r = lightingState3.r;
							arg_478_0.g = lightingState3.g;
							arg_478_0.b = lightingState3.b;
						}
					}
				}
				if (Netplay.Connection.StatusMax > 0)
				{
					Main.mapTime = 1;
				}
				if (Main.mapTime == 0 && Main.mapEnabled && Main.renderCount == 3)
				{
					try
					{
						Main.mapTime = Main.mapTimeMax;
						Main.updateMap = true;
						Main.mapMinX = Terraria.Utils.Clamp<int>(Terraria.Lighting.firstToLightX + Terraria.Lighting.offScreenTiles, 0, Main.maxTilesX - 1);
						Main.mapMaxX = Terraria.Utils.Clamp<int>(Terraria.Lighting.lastToLightX - Terraria.Lighting.offScreenTiles, 0, Main.maxTilesX - 1);
						Main.mapMinY = Terraria.Utils.Clamp<int>(Terraria.Lighting.firstToLightY + Terraria.Lighting.offScreenTiles, 0, Main.maxTilesY - 1);
						Main.mapMaxY = Terraria.Utils.Clamp<int>(Terraria.Lighting.lastToLightY - Terraria.Lighting.offScreenTiles, 0, Main.maxTilesY - 1);
						for (int m = Main.mapMinX; m < Main.mapMaxX; m++)
						{
							Terraria.Lighting.LightingState[] array7 = Terraria.Lighting.states[m - Terraria.Lighting.firstTileX + Terraria.Lighting.offScreenTiles];
							for (int n = Main.mapMinY; n < Main.mapMaxY; n++)
							{
								Terraria.Lighting.LightingState lightingState4 = array7[n - Terraria.Lighting.firstTileY + Terraria.Lighting.offScreenTiles];
								Tile tile = Main.tile[m, n];
								float num26 = 0f;
								if (lightingState4.r > num26)
								{
									num26 = lightingState4.r;
								}
								if (lightingState4.g > num26)
								{
									num26 = lightingState4.g;
								}
								if (lightingState4.b > num26)
								{
									num26 = lightingState4.b;
								}
								if (Terraria.Lighting.lightMode < 2)
								{
									num26 *= 1.5f;
								}
								byte b = (byte)Math.Min(255f, num26 * 255f);
								if ((double)n < Main.worldSurface && !tile.active() && tile.wall == 0 && tile.liquid == 0)
								{
									b = 22;
								}
								if (b > 18 || Main.Map[m, n].Light > 0)
								{
									if (b < 22)
									{
										b = 22;
									}
									Main.Map.UpdateLighting(m, n, b);
								}
							}
						}
					}
					catch
					{
					}
				}
				if (Terraria.Lighting.oldSkyColor != Terraria.Lighting.skyColor)
				{
					int num27 = Terraria.Utils.Clamp<int>(Terraria.Lighting.firstToLightX, 0, Main.maxTilesX - 1);
					int num28 = Terraria.Utils.Clamp<int>(Terraria.Lighting.lastToLightX, 0, Main.maxTilesX - 1);
					int num29 = Terraria.Utils.Clamp<int>(Terraria.Lighting.firstToLightY, 0, Main.maxTilesY - 1);
					int num30 = Terraria.Utils.Clamp<int>(Terraria.Lighting.lastToLightY, 0, (int)Main.worldSurface - 1);
					if ((double)num29 < Main.worldSurface)
					{
						for (int num31 = num27; num31 < num28; num31++)
						{
							Terraria.Lighting.LightingState[] array8 = Terraria.Lighting.states[num31 - Terraria.Lighting.firstToLightX];
							for (int num32 = num29; num32 < num30; num32++)
							{
								Terraria.Lighting.LightingState lightingState5 = array8[num32 - Terraria.Lighting.firstToLightY];
								Tile tile2 = Main.tile[num31, num32];
								if (tile2 == null)
								{
									tile2 = new Tile();
									Main.tile[num31, num32] = tile2;
								}
								if ((!tile2.active() || !Main.tileNoSunLight[(int)tile2.type]) && lightingState5.r < Terraria.Lighting.skyColor && tile2.liquid < 200 && (Main.wallLight[(int)tile2.wall] || tile2.wall == 73))
								{
									lightingState5.r = num;
									if (lightingState5.g < Terraria.Lighting.skyColor)
									{
										lightingState5.g = num2;
									}
									if (lightingState5.b < Terraria.Lighting.skyColor)
									{
										lightingState5.b = num3;
									}
								}
							}
						}
					}
				}
			}
			else
			{
				Terraria.Lighting.lightCounter = 0;
			}
			if (Main.renderCount > Terraria.Lighting.maxRenderCount)
			{
				Terraria.Lighting.PreRenderPhase();
			}
		}
	}
}
