using Microsoft.Xna.Framework;
using PUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Forms;

namespace CheatTool
{
	public class CheatTool
	{
		internal static bool BlockReach = false;
		internal static bool FullBright = false;
		private static WindowManager _WManager = new WindowManager();
		internal static WindowManager WManager
		{
			get => _WManager;
			private set => _WManager = value;
		}
		public static Bar Hotbar = new Bar();
		private static bool Bar_Hovered = false;
		static CheatTool()
		{
			PHooks.Hooks.InterfaceLayersSetup.After += InterfaceLayersSetup_After;
			PHooks.Hooks.Update.After += Update_After;
			PHooks.Hooks.Update.Pre += Update_Pre;
			PHooks.Hooks.ResetEffects.After += ResetEffects_After;

			Hotbar.OnHover += Hotbar_OnHover;
			Main.instance.LoadItem(ItemID.WorkBench);
			Main.instance.LoadItem(ItemID.GuideVoodooDoll);
			Main.instance.LoadItem(ItemID.Paintbrush);
			Main.instance.LoadItem(ItemID.DirtBlock);
			Main.instance.LoadItem(ItemID.ObsidianSkull);
			Main.instance.LoadItem(ItemID.IceTorch);
			Main.instance.LoadNPC(NPCID.BlueSlime);
			Image ItemBrowserImage = new Image(Terraria.GameContent.TextureAssets.Item[ItemID.WorkBench].Value) { ToolTip = "Item Browser" };
			ItemBrowserImage.OnClick += ItemBrowserImage_OnClick;
			Image NPCBrowserImage = new Image(Terraria.GameContent.TextureAssets.Npc[NPCID.BlueSlime].Value) { ToolTip = "NPC Browser" };
			NPCBrowserImage.DrawingRectangle = new Rectangle(0, 0, NPCBrowserImage.Texture.Width, NPCBrowserImage.Texture.Height / Main.npcFrameCount[NPCID.BlueSlime]);
			NPCBrowserImage.OnClick += NPCBrowserImage_OnClick;

			Image PlayerImage = new Image(Terraria.GameContent.TextureAssets.Item[ItemID.GuideVoodooDoll].Value) { ToolTip = "Player" };
			PlayerImage.OnClick += PlayerImage_OnClick;

			Image EnviromentImage = new Image(EnviromentBar.DayTimeIcon) { ToolTip = "Enviroment" };
			EnviromentImage.OnClick += TimeWeather_OnClick;

			Image WorldPainterImage = new Image(Terraria.GameContent.TextureAssets.Item[ItemID.Paintbrush].Value) { ToolTip = "World Painter" };
			WorldPainterImage.OnClick += WorldPainterImage_OnClick;

			Image WorldBarImage = new Image(Terraria.GameContent.TextureAssets.Item[ItemID.DirtBlock].Value) { ToolTip = "World" };
			WorldBarImage.OnClick += WorldBarImage_OnClick;

			Image ButcherImage = new Image(Terraria.GameContent.TextureAssets.Item[ItemID.ObsidianSkull].Value) { ToolTip = "Butcher" };
			ButcherImage.OnClick += ButcherImage_OnClick;

			Image FullBrightImage = new Image(Terraria.GameContent.TextureAssets.Item[ItemID.IceTorch].Value) { ToolTip = "Full Bright" };
			FullBrightImage.OnClick += FullBrightImage_OnClick;


			Hotbar.Controls.Add(ItemBrowserImage);
			Hotbar.Controls.Add(NPCBrowserImage);
			Hotbar.Controls.Add(EnviromentImage);

			Hotbar.Controls.Add(PlayerImage);
			Hotbar.Controls.Add(WorldPainterImage);
			Hotbar.Controls.Add(WorldBarImage);
			Hotbar.Controls.Add(ButcherImage);
			Hotbar.Controls.Add(FullBrightImage);
		}

		private static void WorldBarImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			if (Hotbar.SubBar is WorldBar)
			{
				Hotbar.SubBar = null;
				return;
			}
			var pBar = new WorldBar();
			Hotbar.SubBar = pBar;
		}

		private static void ButcherImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			if (Hotbar.SubBar is ButcherBar)
			{
				Hotbar.SubBar = null;
				return;
			}
			var pBar = new ButcherBar();
			Hotbar.SubBar = pBar;
		}

		private static void TimeWeather_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			if (Hotbar.SubBar is EnviromentBar)
			{
				Hotbar.SubBar = null;
				return;
			}
			var pBar = new EnviromentBar();
			Hotbar.SubBar = pBar;
		}

		private static void NPCBrowserImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			WManager.Register(new NPCBrowser(new Rectangle(50, 50, 360, 350)));
		}

		private static void ResetEffects_After(object[] obj)
		{
			if (BlockReach)
			{
				Player.tileRangeX = 9999;
				Player.tileRangeY = 9999;
			}
		}

		private static void PlayerImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			if (Hotbar.SubBar is PlayerBar)
			{
				Hotbar.SubBar = null;
				return;
			}
			var pBar = new PlayerBar();
			Hotbar.SubBar = pBar;
		}

		private static void FullBrightImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			FullBright = !FullBright;
			if (FullBright)
				Main.NewText("Enable");
			else
				Main.NewText("Disable");
		}

		private static void WorldPainterImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			if (Hotbar.SubBar is WorldPainterBar)
			{
				Hotbar.SubBar = null;
				return;
			}
			var wpBar = new WorldPainterBar();
			Hotbar.SubBar = wpBar;
		}

		private static void ItemBrowserImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			WManager.Register(new ItemBrowser(new Rectangle(50, 50, 500, 400)));
		}

		private static void Hotbar_OnHover(object arg1, PUI.EventArgs.MouseEvent arg2)
		{
			Bar_Hovered = true;
		}

		private static bool Update_Pre(object[] arg)
		{
			Bar_Hovered = false;
			Hotbar.Size = new Vector2(500, 50);
			Hotbar.Position = new Vector2(Main.screenWidth / 2 - Hotbar.Width / 2, Main.screenHeight - Hotbar.Height - 30);
			WManager.Update();
			Hotbar.Update();
			return true;
		}

		private static void Update_After(object[] obj)
		{
			if (Main.mapFullscreen)
			{
				if (Main.mouseRight && Main.mouseRightRelease)
				{
					Main.mouseRightRelease = false;
					Main.mapFullscreen = false;
					Main.player[Main.myPlayer].position = IUtils.MouseToWrold(new Vector2(Main.mouseX, Main.mouseY)) * 16;
				}
			}
		}



		private static void InterfaceLayersSetup_After(object[] obj)
		{
			int MouseTextIndex = Main.instance._gameInterfaceLayers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex != -1)
			{
				Main.instance._gameInterfaceLayers.Insert(MouseTextIndex + 1, new LegacyGameInterfaceLayer(
					"CheatTool: UI",
					delegate
					{
						WManager.Draw(Main.spriteBatch);
						Hotbar.Draw(Main.spriteBatch);
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
