using PUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PUI.EventArgs;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Runtime.InteropServices;

namespace CheatTool
{
	public class WorldPainterBar : Bar
	{
		[StructLayout(LayoutKind.Sequential)]
		private unsafe struct CTile
		{
			public ushort Type;
			public byte Wall;
			public byte Liquid;
			public byte BTileHeader;
			public byte BTileHeader2;
			public byte BTileHeader3;
			public short FrameX;
			public short FrameY;
			public short STileHeader;
			public static CTile FromTile(Tile t)
			{
				return new CTile()
				{
					Type = t.type,
					Wall = t.wall,
					Liquid = t.liquid,
					BTileHeader = t.bTileHeader,
					BTileHeader2 = t.bTileHeader2,
					BTileHeader3 = t.bTileHeader3,
					FrameX = t.frameX,
					FrameY = t.frameY,
					STileHeader = t.sTileHeader,
				};
			}
			public Tile ToTile()
			{
				return new Tile()
				{
					type = Type,
					wall = Wall,
					liquid = Liquid,
					bTileHeader = BTileHeader,
					bTileHeader2 = BTileHeader2,
					bTileHeader3 = BTileHeader3,
					frameX = FrameX,
					frameY = FrameY,
					sTileHeader = STileHeader,
				};
			}
		}
		private Image BrushImage, EyeDropperImage, FlipVertical, FlipHorizontal, SaveImage, LoadImage;
		private bool BrushActive = false, EyeDropperActive = false;
		private bool Brushing = false, Dropping = false;
		private Vector2 BeginPos = new Vector2(-1, -1), EndPos = new Vector2(-1, -1);
		private Vector2 BrushBeginPos = new Vector2(-1, -1);
		private static Tile[,] ClipBoard = new Tile[0, 0];
		public WorldPainterBar() : base()
		{

			Size = new Vector2(240, 50);
			BrushImage = new Image(Main.itemTexture[ItemID.Paintbrush]) { ToolTip = "Brush" };
			BrushImage.OnClick += BrushImage_OnClick;

			EyeDropperImage = new Image(Main.itemTexture[ItemID.EmptyDropper]) { ToolTip = "EyeDropper" };
			EyeDropperImage.OnClick += EyeDropperImage_OnClick;

			FlipVertical = new Image(Main.instance.OurLoad<Texture2D>("Qiu/UI/Vertical")) { ToolTip = "Flip Vertical" };
			FlipVertical.OnClick += FlipVertical_OnClick; ;

			FlipHorizontal = new Image(Main.instance.OurLoad<Texture2D>("Qiu/UI/Horizontal")) { ToolTip = "Flip Horizontal" };
			FlipHorizontal.OnClick += FlipHorizon_OnClick; ;

			SaveImage = new Image(Main.itemTexture[ItemID.GoldChest]) { ToolTip = "Save" };
			SaveImage.OnClick += SaveImage_OnClick;

			LoadImage = new Image(Main.itemTexture[ItemID.FastClock]) { ToolTip = "Load" };
			LoadImage.OnClick += LoadImage_OnClick;


			Controls.Add(BrushImage);
			Controls.Add(EyeDropperImage);
			Controls.Add(FlipVertical);
			Controls.Add(FlipHorizontal);
			Controls.Add(SaveImage);
			Controls.Add(LoadImage);
		}

		private void FlipHorizon_OnClick(object arg1, OnClickEventArgs arg2)
		{
			for (int j = 0; j < ClipBoard.GetLength(1); j++)
			{
				for (int i = 0; i < ClipBoard.GetLength(0) / 2; i++)
				{
					Utils.Swap(ref ClipBoard[i, j], ref ClipBoard[ClipBoard.GetLength(0) - 1 - i, j]);
				}
			}
			Main.NewText("Fliped");
		}

		private void FlipVertical_OnClick(object arg1, OnClickEventArgs arg2)
		{
			for (int i = 0; i < ClipBoard.GetLength(0); i++)
			{
				for (int j = 0; j < ClipBoard.GetLength(1) / 2; j++)
				{
					Utils.Swap(ref ClipBoard[i, j], ref ClipBoard[i, ClipBoard.GetLength(1) - 1 - j]);
				}
			}
			Main.NewText("Fliped");
		}

		private void SaveImage_OnClick(object arg1, OnClickEventArgs arg2)
		{
			BrushActive = false;
			EyeDropperActive = false;
			Window w = new Window(new Rectangle(100, 100, 200, 70)) { Title = "Save", Icon = Main.itemTexture[ItemID.GoldChest] };
			TextBox fileName = new TextBox(false) { Position = new Vector2(10, 35), Size = new Vector2(130, 30) };
			w.Controls.Add(fileName);
			Button saveButton = new Button() { Text = "Save", Position = new Vector2(140, 35), Size = new Vector2(50, 30) };
			saveButton.OnClick += (s, e) =>
			{
				if (ClipBoard.GetLength(0) == 0) return;
				if (!Directory.Exists(".\\Content\\Qiu\\Schematics")) Directory.CreateDirectory(".\\Content\\Qiu\\Schematics");
				string name = ".\\Content\\Qiu\\Schematics\\" + fileName.Text + ".sche";
				if (File.Exists(name))
					File.Delete(name);
				var fs = File.Open(name, FileMode.OpenOrCreate);
				BinaryWriter bw = new BinaryWriter(fs);
				lock (ClipBoard)
				{
					int maxX = ClipBoard.GetLength(0);
					int maxY = ClipBoard.GetLength(1);

					bw.Write(maxX);//x
					bw.Write(maxY);//y
					for (int x = 0; x < maxX; x++)
					{
						for (int y = 0; y < maxY; y++)
						{
							CTile t = CTile.FromTile(ClipBoard[x, y]);
							bw.Write(t.Type);
							bw.Write(t.Wall);
							bw.Write(t.Liquid);
							bw.Write(t.BTileHeader);
							bw.Write(t.BTileHeader2);
							bw.Write(t.BTileHeader3);
							bw.Write(t.FrameX);
							bw.Write(t.FrameY);
							bw.Write(t.STileHeader);
						}
					}
					Main.NewText("Saved: " + name);
				}
				bw.Close();
				fs.Close();
			};
			w.Controls.Add(saveButton);
			CheatTool.WManager.Register(w);
		}

		private bool Inside()
		{
			Bar b = this;
			if (b.Inside(MouseState.X, MouseState.Y)) return true;
			while (b.ParentBar != null)
			{
				b = b.ParentBar;
				if (b.Inside(MouseState.X, MouseState.Y)) return true;
			}
			return false;
		}

		private void LoadImage_OnClick(object arg1, OnClickEventArgs arg2)
		{
			BrushActive = false;
			EyeDropperActive = false;
			Window w = new Window(new Rectangle(100, 100, 150, 300)) { Title = "Save", Icon = Main.itemTexture[ItemID.FastClock] };
			ItemListView ls = new ItemListView()
			{
				Position = new Vector2(10, 35),
				Size = new Vector2(130, 260),
			};
			Directory.EnumerateFiles(".\\Content\\Qiu\\Schematics", "*.sche").ToList().ForEach(d =>
			{
				string s = Path.GetFileNameWithoutExtension(d);
				Label l = new Label(s);
				l.OnClick += (sender, e) =>
				{
					if (!Directory.Exists(".\\Content\\Qiu\\Schematics")) Directory.CreateDirectory(".\\Content\\Qiu\\Schematics");
					string fileName = ".\\Content\\Qiu\\Schematics\\" + (sender as Label)?.Text + ".sche";
					var fs = File.Open(fileName, FileMode.Open);
					BinaryReader br = new BinaryReader(fs);
					int maxX = br.ReadInt32();
					int maxY = br.ReadInt32();
					ClipBoard = new Tile[maxX, maxY];
					for (int x = 0; x < ClipBoard.GetLength(0); x++)
					{
						for (int y = 0; y < ClipBoard.GetLength(1); y++)
						{
							CTile t = new CTile()
							{
								Type = br.ReadUInt16(),
								Wall = br.ReadByte(),
								Liquid = br.ReadByte(),
								BTileHeader = br.ReadByte(),
								BTileHeader2 = br.ReadByte(),
								BTileHeader3 = br.ReadByte(),
								FrameX = br.ReadInt16(),
								FrameY = br.ReadInt16(),
								STileHeader = br.ReadInt16()
							};
							ClipBoard[x, y] = t.ToTile();
						}
					}
					Main.NewText("Loaded: " + fileName);
					br.Close();
					fs.Close();
				};
				ls.Add(l);
			});
			w.Controls.Add(ls);
			CheatTool.WManager.Register(w);
		}

		private void BrushImage_OnClick(object arg1, OnClickEventArgs arg2)
		{
			if (ClipBoard.GetLength(0) <= 0)
			{
				Main.NewText("Please select the area you want to copy");
				return;
			}
			BrushActive = !BrushActive;
			EyeDropperActive = false;
		}

		private void EyeDropperImage_OnClick(object arg1, OnClickEventArgs arg2)
		{
			BrushActive = false;
			EyeDropperActive = !EyeDropperActive;
		}

		private static Rectangle GetClippingRectangle(SpriteBatch spriteBatch, Rectangle r)
		{
			Vector2 vector = new Vector2(r.X, r.Y);
			Vector2 position = new Vector2(r.Width, r.Height) + vector;
			vector = Vector2.Transform(vector, Main.UIScaleMatrix);
			position = Vector2.Transform(position, Main.UIScaleMatrix);
			Rectangle result = new Rectangle((int)vector.X, (int)vector.Y, (int)(position.X - vector.X), (int)(position.Y - vector.Y));
			int width = spriteBatch.GraphicsDevice.Viewport.Width;
			int height = spriteBatch.GraphicsDevice.Viewport.Height;
			result.X = Utils.Clamp(result.X, 0, width);
			result.Y = Utils.Clamp(result.Y, 0, height);
			result.Width = Utils.Clamp(result.Width, 0, width - result.X);
			result.Height = Utils.Clamp(result.Height, 0, height - result.Y);
			return result;
		}

		public override void Update()
		{
			base.Update();//events handled
			Player player = Main.LocalPlayer;
			bool Left = MouseState.LeftButton == ButtonState.Pressed;
			bool LastLeft = LastMouseState.LeftButton == ButtonState.Pressed;

			bool LeftDown = Left && !LastLeft;
			bool LeftUp = !Left && LastLeft;

			if (EyeDropperActive)
			{
				if (!Inside() && !Main.gameMenu && !Main.playerInventory)
				{
					if (LeftDown)
					{
						player.mouseInterface = true;
						Dropping = true;
						BeginPos = new Vector2(Player.tileTargetX, Player.tileTargetY);
					}
					else if (LeftUp)
					{
						player.mouseInterface = false;
						Dropping = false;
						EndPos = new Vector2(Player.tileTargetX, Player.tileTargetY);
						Vector2 upperLeft = new Vector2(Math.Min(BeginPos.X, EndPos.X), Math.Min(BeginPos.Y, EndPos.Y));
						Vector2 lowerRight = new Vector2(Math.Max(BeginPos.X, EndPos.X), Math.Max(BeginPos.Y, EndPos.Y));
						int minX = (int)upperLeft.X;
						int maxX = (int)lowerRight.X + 1;
						int minY = (int)upperLeft.Y;
						int maxY = (int)lowerRight.Y + 1;
						ClipBoard = new Tile[maxX - minX, maxY - minY];
						for (int i = 0; i < maxX - minX; i++)
						{
							for (int j = 0; j < maxY - minY; j++)
							{
								ClipBoard[i, j] = new Tile();
							}
						}
						for (int x = minX; x < maxX; x++)
						{
							for (int y = minY; y < maxY; y++)
							{
								if (WorldGen.InWorld(x, y))
								{
									Tile target = Framing.GetTileSafely(x, y);
									ClipBoard[x - minX, y - minY].CopyFrom(target);
								}
							}
						}
						BeginPos = new Vector2(-1, -1);
						EndPos = new Vector2(-1, -1);
						EyeDropperActive = false;
						Main.NewText("Area selected");
					}
					if (Dropping)
					{
						EndPos = new Vector2(Player.tileTargetX, Player.tileTargetY);
					}
				}
			}
			else if (BrushActive)
			{
				if (!Inside() && !Main.gameMenu && !Main.playerInventory)
				{
					int bWidth = ClipBoard.GetLength(0);
					int bHeight = ClipBoard.GetLength(1);
					Vector2 BrusheSize = new Vector2(bWidth, bHeight);
					Point Point = (Main.MouseWorld + new Vector2(bWidth % 2 == 0 ? 1 : 0, bHeight % 2 == 0 ? 1 : 0) * 8).ToTileCoordinates();
					Point.X -= bWidth / 2;
					Point.Y -= bHeight / 2;
					if (LeftDown)
					{
						player.mouseInterface = true;
						BrushBeginPos = Point.ToVector2();
						Brushing = true;
					}
					else if (LeftUp)
					{
						player.mouseInterface = false;
						BrushBeginPos = new Vector2(-1, -1);
						Brushing = false;
					}
					if (Brushing)
					{
						for (int x = 0; x < bWidth; x++)
						{
							for (int y = 0; y < bHeight; y++)
							{
								if (WorldGen.InWorld(x + Point.X, y + Point.Y) && ClipBoard[x, y] != null)
								{
									Tile target = Framing.GetTileSafely(x + Point.X, y + Point.Y);
									int cycledX = ((x + Point.X - (int)BrushBeginPos.X) % bWidth + bWidth) % bWidth;
									int cycledY = ((y + Point.Y - (int)BrushBeginPos.Y) % bHeight + bHeight) % bHeight;
									target.CopyFrom(ClipBoard[cycledX, cycledY]);
								}
							}
						}

						for (int x = 0; x < bWidth; x++)
						{
							for (int y = 0; y < bHeight; y++)
							{
								if (WorldGen.InWorld(x + Point.X, y + Point.Y) && ClipBoard[x, y] != null)
								{
									int cycledX = ((x + Point.X - (int)BrushBeginPos.X) % bWidth + bWidth) % bWidth;
									int cycledY = ((y + Point.Y - (int)BrushBeginPos.Y) % bHeight + bHeight) % bHeight;
									WorldGen.SquareTileFrame(cycledX, cycledY, true);
								}
							}
						}
						if (Main.netMode == 1)
						{
							NetMessage.SendTileSquare(-1, Point.X + bWidth / 2, Point.Y + bHeight / 2, Math.Max(bWidth, bHeight));
						}
					}

				}
			}
		}

		private static Color BuffColor(Color newColor, float R, float G, float B, float A)
		{
			newColor.R = (byte)((float)newColor.R * R);
			newColor.G = (byte)((float)newColor.G * G);
			newColor.B = (byte)((float)newColor.B * B);
			newColor.A = (byte)((float)newColor.A * A);
			return newColor;
		}

		public static void DrawPreview(SpriteBatch sb, Tile[,] BrushTiles, Vector2 position)
		{
			int width = BrushTiles.GetLength(0);
			int height = BrushTiles.GetLength(1);
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Tile tile = BrushTiles[x, y];
					if (tile.active())
					{
						if (!Main.tileSetsLoaded[tile.type])
							Main.instance.LoadTiles(tile.type);
						Texture2D texture = Main.tileTexture[tile.type];
						Color color = Color.White;
						color.A = 160;
						Rectangle? value = new Rectangle(tile.frameX, tile.frameY, 16, 16);
						Vector2 pos = position + new Vector2(x * 16, y * 16);
						sb.Draw(texture, pos, value, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
					}
				}
			}
		}

		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);
			if (Dropping)
			{
				Vector2 upperLeft = new Vector2(Math.Min(BeginPos.X, EndPos.X), Math.Min(BeginPos.Y, EndPos.Y));
				Vector2 lowerRight = new Vector2(Math.Max(BeginPos.X, EndPos.X) + 1, Math.Max(BeginPos.Y, EndPos.Y) + 1);
				Vector2 upperLeftScreen = upperLeft * 16f;
				Vector2 lowerRightScreen = lowerRight * 16f;
				upperLeftScreen -= Main.screenPosition;
				lowerRightScreen -= Main.screenPosition;
				Vector2 brushSize = lowerRight - upperLeft;
				Rectangle value = new Rectangle(0, 0, 1, 1);
				float r = 1f;
				float g = 0.9f;
				float b = 0.1f;
				float a = 1f;
				float scale = 0.6f;
				Color color = BuffColor(Color.White, r, g, b, a);
				Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, 16f * brushSize, SpriteEffects.None, 0f);
				b = 0.3f;
				g = 0.95f;
				scale = (a = 1f);
				color = BuffColor(Color.White, r, g, b, a);
				Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitX * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * brushSize.Y), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitX * 16f * brushSize.X, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * brushSize.Y), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitY * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * brushSize.X, 2f), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Main.magicPixel, upperLeftScreen + Vector2.UnitY * 16f * brushSize.Y, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * brushSize.X, 2f), SpriteEffects.None, 0f);
			}
			if (EyeDropperActive && !Inside() && !Main.gameMenu && !Main.playerInventory)
			{
				Main.LocalPlayer.showItemIcon2 = ItemID.EmptyDropper;
				Main.LocalPlayer.showItemIcon = true;
			}
			if (BrushActive && !Inside() && !Main.gameMenu && !Main.playerInventory)
			{
				Main.LocalPlayer.showItemIcon2 = ItemID.Paintbrush;
				Main.LocalPlayer.showItemIcon = true;
				Vector2 Size = new Vector2(ClipBoard.GetLength(0), ClipBoard.GetLength(1));
				Vector2 dPos = new Vector2(MouseState.X, MouseState.Y) - (Size * 8);

				if (!(MouseState.LeftButton == ButtonState.Pressed)) DrawPreview(batch, ClipBoard, dPos);
				Rectangle value = new Rectangle(0, 0, 1, 1);
				float r = 1f;
				if (!(MouseState.LeftButton == ButtonState.Pressed)) r = .25f;
				float g = 0.9f;
				float b = 0.1f;
				float a = 1f;
				float scale = 0.6f;
				Color color = BuffColor(Color.White, r, g, b, a);
				Main.spriteBatch.Draw(Main.magicPixel, dPos, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, 16f * Size, SpriteEffects.None, 0f);
				b = 0.3f;
				g = 0.95f;
				scale = (a = 1f);
				color = BuffColor(Color.White, r, g, b, a);
				Main.spriteBatch.Draw(Main.magicPixel, dPos + Vector2.UnitX * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * Size.Y), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Main.magicPixel, dPos + Vector2.UnitX * 16f * Size.X, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(2f, 16f * Size.Y), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Main.magicPixel, dPos + Vector2.UnitY * -2f, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * Size.X, 2f), SpriteEffects.None, 0f);
				Main.spriteBatch.Draw(Main.magicPixel, dPos + Vector2.UnitY * 16f * Size.Y, new Microsoft.Xna.Framework.Rectangle?(value), color * scale, 0f, Vector2.Zero, new Vector2(16f * Size.X, 2f), SpriteEffects.None, 0f);
			}
		}
	}
}
