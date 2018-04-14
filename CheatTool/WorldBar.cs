using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace CheatTool
{
	class WorldBar:Bar
	{
		public WorldBar()
		{
			Size = new Vector2(50, 50);
			Image RevealMapImage = new Image(Main.buffTexture[BuffID.EyeballSpring]) { ToolTip = "Refresh" };
			RevealMapImage.OnClick += BlockReachImage_OnClick;
			

			Controls.Add(RevealMapImage);
		}
		
		private void BlockReachImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			IUtils.RevealMap();
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
