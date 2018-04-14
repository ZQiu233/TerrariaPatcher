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
	public class TaskBar : ListView<Image>
	{
		private float PaddingVertical = 5f;
		private float Spacing = 5f;
		private List<Image> Images = new List<Image>();
		public Image this[int k]
		{
			get
			{
				return Images[k];
			}
		}
		public override void Add(Image item)
		{
			Images.Add(item);
		}

		public override void Add(IEnumerable<Image> items)
		{
			Images.AddRange(items);
		}

		public override void Update()
		{
			base.Update();
			foreach(var s in Images)
			{
				s.Update();
			}
		}

		private void DrawIcons(SpriteBatch batch)
		{
			var pos = DrawPosition;
			pos.Y += PaddingVertical;
			pos.X += Spacing;
			for (int i = 0; i < Images.Count; i++)
			{
				var image = Images[i];
				image.Size = new Vector2(Height - PaddingVertical * 2, Height - PaddingVertical * 2);
				image.Position = new Vector2(pos.X, pos.Y);
				pos.X += image.Width + Spacing;
				image.Draw(batch);
			}
		}

		public override void Draw(SpriteBatch batch)
		{
			Utils.DrawInvBG(batch, DrawPosition.X, DrawPosition.Y, Width, Height, Window.WindowBackground);
			base.Draw(batch);
			DrawIcons(batch);
		}

		public override Image ElementAt(int index)
		{
			return this[index];
		}

		public override bool Remove(Image item)
		{
			return Images.Remove(item);
		}
	}
}
