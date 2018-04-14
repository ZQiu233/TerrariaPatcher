using Microsoft.Xna.Framework;
using PUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using PUI.EventArgs;

namespace CheatTool
{
	public class ItemBrowser : Window
	{
		private static Dictionary<string, bool[]> TypeCategories = new Dictionary<string, bool[]>();
		private ImageBox ItemBox;
		private ItemListView Types;
		private TextBox SearchBox;
		private Button Search;
		private static ImageBox.ImageBoxItem[] AllItems = null;
		private static bool[] All, Melee, Ranged, Magic, Summon, Blocks, Walls, Head, Body, Leg, Accessory, Consumable, Buff, Material;
		private bool[] Selected = null;
		static ItemBrowser()
		{

			All = new bool[Main.maxItemTypes];
			Melee = new bool[Main.maxItemTypes];
			Ranged = new bool[Main.maxItemTypes];
			Magic = new bool[Main.maxItemTypes];
			Summon = new bool[Main.maxItemTypes];
			Blocks = new bool[Main.maxItemTypes];
			Walls = new bool[Main.maxItemTypes];
			Head = new bool[Main.maxItemTypes];
			Body = new bool[Main.maxItemTypes];
			Leg = new bool[Main.maxItemTypes];
			Accessory = new bool[Main.maxItemTypes];
			Consumable = new bool[Main.maxItemTypes];
			Buff = new bool[Main.maxItemTypes];
			Material = new bool[Main.maxItemTypes];

			TypeCategories.Add("All", All);
			TypeCategories.Add("Melee", Melee);
			TypeCategories.Add("Ranged", Ranged);
			TypeCategories.Add("Magic", Magic);
			TypeCategories.Add("Summon", Summon);
			TypeCategories.Add("Blocks", Blocks);
			TypeCategories.Add("Walls", Walls);
			TypeCategories.Add("Head", Head);
			TypeCategories.Add("Body", Body);
			TypeCategories.Add("Leg", Leg);
			TypeCategories.Add("Accessory", Accessory);
			TypeCategories.Add("Consumable", Consumable);
			TypeCategories.Add("Buff", Buff);
			TypeCategories.Add("Material", Material);

			for (int i = 1; i < Main.maxItemTypes; i++)
			{
				Item item = new Item();
				item.SetDefaults(i);
				All[i] = true;
				if (item.melee) Melee[i] = true;
				if (item.ranged) Ranged[i] = true;
				if (item.magic) Magic[i] = true;
				if (item.summon) Summon[i] = true;
				if (item.createTile != -1) Blocks[i] = true;
				if (item.createWall != -1) Walls[i] = true;
				if (item.headSlot > 0) Head[i] = true;
				if (item.bodySlot > 0) Body[i] = true;
				if (item.legSlot > 0) Leg[i] = true;
				if (item.accessory) Accessory[i] = true;
				if (item.consumable) Consumable[i] = true;
				if (item.buffType > 0) Buff[i] = true;
				if (item.material) Material[i] = true;
			}
		}
		public ItemBrowser(Rectangle bound) : base(bound)
		{
			Title = "ItemBrowser";
			ItemBox = new ImageBox() { Column = 6 };
			Types = new ItemListView();
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
			Selected = TypeCategories["All"];
			AddItems();
			Controls.Add(SearchBox);
			Controls.Add(Search);
			Controls.Add(Types);
			Controls.Add(ItemBox);
			foreach (var s in TypeCategories)
			{
				Label l = new Label(s.Key);
				l.OnClick += (sender, args) =>
				{
					Selected = TypeCategories[(sender as Label).Text];
					AddItems();
				};
				Types.Add(l);
			}
		}

		private void Search_OnClick(object arg1, OnClickEventArgs arg2)
		{
			bool[] b = new bool[Selected.Length];
			for (int i = 0; i < b.Length; i++)
			{

				if (AllItems[i].ToolTip.ToLower().Contains(SearchBox.Text.ToLower()))
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
			ItemBox.Position = new Vector2(10, 70);
			ItemBox.Size = new Vector2((Width - 10) * 0.66f, (Height - 70) * 0.95f);
			Types.Position = new Vector2(ItemBox.X + ItemBox.Width + 3, 70);
			Types.Size = new Vector2(Width - 10 - Types.X, ItemBox.Height);
		}
		public override void Draw(SpriteBatch batch)
		{
			Resize();
			base.Draw(batch);
		}
		private void LoadAllSlot()
		{
			AllItems = new ImageBox.ImageBoxItem[Main.maxItemTypes];
			for (int i = 0; i < AllItems.Length; i++)
			{
				Item item = new Item();
				item.SetDefaults(i);
				AllItems[i] = new ImageBox.ImageBoxItem(Main.itemTexture[i], item.Name)
				{
					Name = "" + i
				};
				AllItems[i].OnClick += (s, e) =>
				{
					int tid = Convert.ToInt32((s as ImageBox.ImageBoxItem).Name);
					if (Main.mouseItem.type == 0)
					{
						Main.mouseItem.SetDefaults(tid);
						Main.mouseItem.stack = e.Button == MouseButtons.Left ? Main.mouseItem.maxStack : 1;
					}
					if (Main.mouseItem.type == tid)
					{
						if (e.Button == MouseButtons.Left)
							Main.mouseItem.stack = Main.mouseItem.maxStack;
						else
						{
							if (Main.mouseItem.stack < Main.mouseItem.maxStack)
							{
								Main.mouseItem.stack++;
							}
						}
					}
					Main.PlaySound(18);
				};
			}
		}
		private void AddItems()
		{
			if (AllItems == null)
				LoadAllSlot();
			ItemBox.ScrollValue = 0f;
			ItemBox.Items.Clear();
			for (int i = 1; i < Main.maxItemTypes; i++)
			{
				if (Selected != null)
				{
					if (Selected[i])
					{
						ItemBox.Items.Add(AllItems[i]);
					}
				}
			}
		}
	}
}
