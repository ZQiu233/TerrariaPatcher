using PUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Microsoft.Xna.Framework;

namespace Tindows
{
	public class StartMenu : Control
	{
		private Container Content = new Container();
		private ItemListView ItemListView = new ItemListView();

		public StartMenu(Label[] items)
		{
			Focusable = true;
			Size = new Vector2(200, 250);

			items.ToList().ForEach((e) =>
			{
				ItemListView.Add(e);
			});
			Content.Controls.Add(ItemListView);
		}

		public override void Update()
		{
			base.Update();
			Content.Update();
		}
		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);
			Content.Size = Size;
			Content.Position = DrawPosition;
			ItemListView.Size = Content.Size;
			Content.Draw(batch);
		}
	}
}
