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
	public class ButcherBar : Bar
	{
		public ButcherBar()
		{
			Main.instance.LoadItem(ItemID.DemonHeart);
			Main.instance.LoadItem(ItemID.ObsidianSkull);
			Main.instance.LoadItem(ItemID.MechanicalSkull);
			Size = new Vector2(130, 50);
			Image KillMobs = new Image(Terraria.GameContent.TextureAssets.Item[ItemID.DemonHeart].Value) { ToolTip = "Kill Mobs" };
			KillMobs.OnClick += KillMobs_OnClick;

			Image KillNPCs = new Image(Terraria.GameContent.TextureAssets.Item[ItemID.ObsidianSkull].Value) { ToolTip = "Kill NPCs" };
			KillNPCs.OnClick += KillNPCs_OnClick;

			Image KillAll = new Image(Terraria.GameContent.TextureAssets.Item[ItemID.MechanicalSkull].Value) { ToolTip = "Kill all" };
			KillAll.OnClick += KillAll_OnClick;

			Controls.Add(KillMobs);
			Controls.Add(KillNPCs);
			Controls.Add(KillAll);

		}

		private void KillAll_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			KillMobs();
			KillNPCs();
		}

		private void KillMobs()
		{
			foreach (var n in Main.npc)
				if (!n.friendly)
					IUtils.HitNPC(n, n.lifeMax + 50);
		}

		private void KillNPCs()
		{
			foreach (var n in Main.npc)
				if (n.friendly)
					IUtils.HitNPC(n, n.lifeMax + 50);
		}

		private void KillNPCs_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			KillNPCs();
		}

		private void KillMobs_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			KillMobs();
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
