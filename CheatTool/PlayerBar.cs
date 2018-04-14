using PUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace CheatTool
{
	public class PlayerBar : Bar
	{
		public PlayerBar()
		{
			Size = new Vector2(100, 50);
			Image BlockReachImage = new Image(Main.itemTexture[ItemID.Toolbelt]) { ToolTip = "Block Reach" };
			BlockReachImage.OnClick += BlockReachImage_OnClick;

			Image GhostModeImage = new Image(Main.itemTexture[ItemID.GhostMask]) { ToolTip = "Ghost Mode" };
			GhostModeImage.OnClick += GhostModeImage_OnClick;

			Controls.Add(BlockReachImage);
			Controls.Add(GhostModeImage);
		}

		private void GhostModeImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			Main.LocalPlayer.ghost = !Main.LocalPlayer.ghost;
			if (Main.LocalPlayer.ghost)
				Main.NewText("Enable");
			else
				Main.NewText("Disable");
		}

		private void BlockReachImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			CheatTool.BlockReach = !CheatTool.BlockReach;
			if (CheatTool.BlockReach)
				Main.NewText("Enable");
			else
				Main.NewText("Disable");
		}

		public override void Update()
		{
			base.Update();
		}
		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);
		}
	}
}
