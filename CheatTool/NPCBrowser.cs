using PUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using PUI.EventArgs;
using Microsoft.Xna.Framework.Graphics;

namespace CheatTool
{
	public class NPCBrowser : Window
	{
		private ImageBox NPCBox;
		private TextBox SearchBox;
		private Button Search;
		private static ImageBox.ImageBoxItem[] AllNPCs = null;
		private static bool[] All;
		private bool[] Selected = null;
		static NPCBrowser()
		{

			All = new bool[Main.maxNPCTypes];


			for (int i = 1; i < Main.maxNPCTypes; i++)
			{
				All[i] = true;
			}
		}
		public NPCBrowser(Rectangle bound) : base(bound)
		{
			Title = "NPCBrowser";
			NPCBox = new ImageBox() { Column = 6 };
			SearchBox = new TextBox(false)
			{
				Position = new Vector2(10, 30),
				Size = new Vector2(120, 30),
			};
			Search = new Button()
			{
				Position = new Vector2(SearchBox.X + SearchBox.Width + 4, 30),
				Size = new Vector2(80, 30),
				Text = "Search",
			};
			Search.OnClick += Search_OnClick;
			Selected = All;
			AddItems();
			Controls.Add(SearchBox);
			Controls.Add(Search);
			Controls.Add(NPCBox);
		}

		private void Search_OnClick(object arg1, OnClickEventArgs arg2)
		{
			bool[] b = new bool[Selected.Length];
			for (int i = 0; i < b.Length; i++)
			{

				if (AllNPCs[i].ToolTip.ToLower().Contains(SearchBox.Text.ToLower()))
					b[i] = true;
			}
			Selected = b;
			AddItems();
		}

		public override void Update()
		{
			base.Update();
		}
		private void Resize()
		{
			NPCBox.Position = new Vector2(10, 70);
			NPCBox.Size = new Vector2((Width - 10 - 10), (Height - 70) * 0.95f);
		}
		public override void Draw(SpriteBatch batch)
		{
			Resize();
			base.Draw(batch);
		}
		private void LoadAllSlot()
		{
			AllNPCs = new ImageBox.ImageBoxItem[Main.maxNPCTypes];
			for (int i = 0; i < AllNPCs.Length; i++)
			{
				NPC n = new NPC();
				n.SetDefaults(i);
				if (Terraria.GameContent.TextureAssets.Npc[i] == null || !Terraria.GameContent.TextureAssets.Npc[i].IsLoaded)
					Main.instance.LoadNPC(i);
				AllNPCs[i] = new ImageBox.ImageBoxItem(Terraria.GameContent.TextureAssets.Npc[i].Value, n.GivenOrTypeName)
				{
					Name = "" + i
				};
				AllNPCs[i].DrawingRectangle = new Rectangle(0, 0, Terraria.GameContent.TextureAssets.Npc[i].Value.Width, Terraria.GameContent.TextureAssets.Npc[i].Value.Height / Main.npcFrameCount[i]);
				AllNPCs[i].OnClick += (s, e) =>
				{
					NPC.NewNPC((int)(Main.LocalPlayer.position.X), (int)(Main.LocalPlayer.position.Y), Convert.ToInt32((s as ImageBox.ImageBoxItem).Name));
				};
			}
		}
		private void AddItems()
		{
			if (AllNPCs == null)
				LoadAllSlot();
			NPCBox.ScrollValue = 0f;
			NPCBox.Items.Clear();
			for (int i = 1; i < Main.maxNPCTypes; i++)
			{
				if (Selected != null)
				{
					if (Selected[i])
					{
						NPCBox.Items.Add(AllNPCs[i]);
					}
				}
			}
		}
	}
}
