using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using WinFormsGraphicsDevice;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Content;

namespace ScheMaker
{
	public class TileView : GraphicsDeviceControl
	{
		public static Texture2D[] TileTexture = new Texture2D[1024];
		private static Texture2D PixelTexture;
		private ContentManager Content;
		private SpriteBatch Batch;
		private Stopwatch Timer;
		private Color bColor = new Color(27, 27, 28);
		private Color[,] Colors = new Color[0, 0];

		public Color[,] GetColors()
		{
			return Colors;
		}

		public TileView(Color[,] colors)
		{
			Colors = colors;
		}

		public Texture2D LoadTile(int type)
		{
			if (TileTexture[type] != null) return TileTexture[type];
			TileTexture[type] = Content.Load<Texture2D>("Images/Tiles_" + type);
			return TileTexture[type];
		}
		protected override void Initialize()
		{
			using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("ScheMaker.Pixel.png"))
			{
				PixelTexture = Texture2D.FromStream(GraphicsDevice, s);
			}
			Content = new ContentManager(Services, "Content");
			Batch = new SpriteBatch(GraphicsDevice);
			Timer = Stopwatch.StartNew();
			Application.Idle += (s, e) =>
			{
				Invalidate();
			};
		}
		private bool Inside()
		{
			var p = PointToClient(MousePosition);
			return p.X > 0 &&
				p.X < Width &&
				p.Y > 0 &&
				p.Y < Height;
		}
		/*private bool Down = false;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Down = true;
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			Down = false;
		}*/
		private float _Scale = 1f;
		public void ScaleTo(float f)
		{
			_Scale *= f;
		}
		protected override void Draw()
		{
			int MaxX = Colors.GetLength(0);
			int MaxY = Colors.GetLength(1);
			float unitW = (float)(Width - 1) / MaxX;
			float unitH = (float)(Height - 1) / MaxY;
			Batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null);
			GraphicsDevice.Clear(bColor);

			for (int i = 0; i < MaxX; i++)
			{
				for (int j = 0; j < MaxY; j++)
				{
					Color c = Colors[i, j];
					Batch.Draw(PixelTexture, new Rectangle((int)(i * unitW * _Scale), (int)(j * unitH * _Scale), (int)((unitW + 1) * _Scale), (int)((unitH + 1) * _Scale)), c * 0.80f);
					/*CTile tile = Colors[i, j];
					if (tile.Active())
					{
						Color c;
						switch (tile.Color())
						{
								break;
						}
						//Batch.Draw(PixelTexture, new Rectangle((int)(i * unitW), (int)(j * unitH), (int)(unitW + 1), (int)(unitH + 1)), Color.White);
						//Batch.Draw(LoadTile(tile.Type), new Rectangle((int)(i * unitW), (int)(j * unitH), (int)(unitW + 1), (int)(unitH + 1)), new Rectangle(tile.FrameX, tile.FrameY, 16, 16), Color.White);
					}*/
				}
			}

			/*if (Inside())
			{
				var p = PointToClient(MousePosition);
				int X = (int)Math.Floor((float)p.X / unitW);
				int Y = (int)Math.Floor((float)p.Y / unitH);
				if (Down)//brushing
				{
					Batch.Draw(PixelTexture, new Rectangle((int)(X * unitW), (int)(Y * unitH), (int)(unitW + 1), (int)(unitH + 1)), null, new Color(0, 200, 0, 100));
				}
				else
				{
					Batch.Draw(PixelTexture, new Rectangle((int)(X * unitW), (int)(Y * unitH), (int)(unitW + 1), (int)(unitH + 1)), null, new Color(0, 0, 200, 100));
				}
			}*/

			for (int i = 0; i <= MaxX; i++)
			{
				Batch.Draw(PixelTexture, new Rectangle((int)(i * unitW), 0, 1, Height), null, Color.White);
			}
			for (int i = 0; i <= MaxY; i++)
			{
				Batch.Draw(PixelTexture, new Rectangle(0, (int)(i * unitH), Width, 1), null, Color.White);
			}


			Batch.End();
		}
	}
}