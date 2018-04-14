using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace ScheMaker
{
	public partial class MainForm : Form
	{
		private class MenuColorTable : ProfessionalColorTable
		{
			public override Color MenuItemSelected => sColor;
			public override Color MenuBorder => sColor;
			public override Color MenuItemSelectedGradientBegin => sColor;
			public override Color MenuItemSelectedGradientEnd => sColor;

			public override Color MenuItemPressedGradientBegin => sBlackColor;
			public override Color MenuItemPressedGradientMiddle => sBlackColor;
			public override Color MenuItemPressedGradientEnd => sBlackColor;

			public override Color MenuStripGradientBegin => sBlackColor;
			public override Color MenuStripGradientEnd => sBlackColor;

		}
		private class MenuStripRender : ToolStripProfessionalRenderer
		{
			public MenuStripRender() : base(new MenuColorTable())
			{

			}
		}
		private MenuStrip MenuStrip;
		private TabControlEx Tab;
		public static Color bColor = Color.FromArgb(37, 37, 38);
		public static Color sColor = Color.FromArgb(62, 62, 64);
		public static Color sBlackColor = Color.FromArgb(27, 27, 28);

		public struct ColorTile
		{
			public byte A, R, G, B;
			public ushort Tile;
		}

		public static List<ColorTile> TileTable = new List<ColorTile>();

		private static void AddMenuItem(ToolStripMenuItem menu, string text, Action<object, EventArgs> click)
		{
			var item = new ToolStripMenuItem(text)
			{
				BackColor = sBlackColor,
				ForeColor = Color.White
			};
			item.Click += new EventHandler(click);
			menu.DropDownItems.Add(item);
		}

		public static void ReadConfig<T>(string path, ref T config)
		{
			config = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
			File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
		}

		private static void LoadColorTable()
		{
			List<string> table = new List<string>();
			ReadConfig(".\\ColorTile.json", ref table);

			foreach (var t in table)
			{
				string[] s = t.Split(',');
				byte A = Convert.ToByte(s[0], 16);
				byte R = Convert.ToByte(s[1], 16);
				byte G = Convert.ToByte(s[2], 16);
				byte B = Convert.ToByte(s[3], 16);
				ushort ID = Convert.ToUInt16(s[4]);
				TileTable.Add(new ColorTile { A = A, R = R, G = G, B = B, Tile = ID });
			}

		}

		private static int ColorCmp(Microsoft.Xna.Framework.Color A, Microsoft.Xna.Framework.Color B)
		{
			return (int)(
				Math.Pow(A.R - B.R, 2) +
				Math.Pow(A.G - B.G, 2) +
				Math.Pow(A.B - B.B, 2));
		}

		private static ushort TileIDFromColor(Microsoft.Xna.Framework.Color c)
		{
			ushort sum = 0;
			int di = int.MaxValue;
			foreach (var ct in TileTable)
			{
				var r = new Microsoft.Xna.Framework.Color(ct.R, ct.G, ct.B, ct.A);
				int i = ColorCmp(c, r);
				if (i < di)
				{
					di = i;
					sum = ct.Tile;
				}
			}
			return sum;
		}

		private static CTile FromColor(Microsoft.Xna.Framework.Color c)
		{
			var y = TileIDFromColor(new Microsoft.Xna.Framework.Color(c.R, c.G, c.B, c.A));
			CTile t = new CTile()
			{
				FrameX = 18,
				FrameY = 18,
				Type = TileIDFromColor(new Microsoft.Xna.Framework.Color(c.R, c.G, c.B, c.A))
			};
			if (c.A != 0)
			{
				t.Active(true);
			}
			return t;
		}

		public static Dictionary<string, TabPage> Datas = new Dictionary<string, TabPage>();
		private void Open_File(string file)
		{
			TabPage page = new TabPage(Path.GetFileNameWithoutExtension(file))
			{
				BackColor = sBlackColor
			};
			Tab.Controls.Add(page);
			Bitmap img = (Bitmap)Image.FromFile(file);
			Microsoft.Xna.Framework.Color[,] Colors = new Microsoft.Xna.Framework.Color[img.Width, img.Height];
			for (int i = 0; i < Colors.GetLength(0); i++)
			{
				for (int j = 0; j < Colors.GetLength(1); j++)
				{
					var p = img.GetPixel(i, j);
					Colors[i, j] = new Microsoft.Xna.Framework.Color(p.R, p.G, p.B, p.A);
				}
			}
			WPanel panel = new WPanel(Colors)
			{
				AutoScroll = true,
				Location = new Point(0, 0),
				Size = page.Size,
			};
			page.Controls.Add(panel);
			Datas.Add(file, page);
			Tab.SelectTab(page);
		}
		public void Open()
		{
			FileDialog fd = new OpenFileDialog()
			{
				Filter = "Image file(*.png;*.jpg)|*.png;*.jpg"
			};
			if (fd.ShowDialog(this) == DialogResult.OK)
			{
				string file = fd.FileName;
				if (Datas.ContainsKey(file))
				{
					MessageBox.Show("该文件已被加载过");
					return;
				}
				Open_File(file);
			}
		}
		private CTile[,] ParseTile(Microsoft.Xna.Framework.Color[,] Colors)
		{
			CTile[,] Tiles = new CTile[Colors.GetLength(0), Colors.GetLength(1)];
			for (int i = 0; i < Tiles.GetLength(0); i++)
			{
				for (int j = 0; j < Tiles.GetLength(1); j++)
				{
					Tiles[i, j] = FromColor(Colors[i, j]);
				}
			}
			return Tiles;
		}
		public void Save()
		{
			if (Tab.SelectedIndex < 0)
			{
				MessageBox.Show("还没有可以保存的项目");
				return;
			}
			FileDialog fd = new SaveFileDialog()
			{
				Filter = "Schematics file(*.sche)|*.sche"
			};
			if (fd.ShowDialog(this) == DialogResult.OK)
			{
				var Tiles = ParseTile((Tab.TabPages[Tab.SelectedIndex].Controls[0] as WPanel).Data);
				BinaryWriter bw = new BinaryWriter(File.Open(fd.FileName, FileMode.OpenOrCreate));
				bw.Write(Tiles.GetLength(0));
				bw.Write(Tiles.GetLength(1));
				for (int i = 0; i < Tiles.GetLength(0); i++)
				{
					for (int j = 0; j < Tiles.GetLength(1); j++)
					{
						CTile t = Tiles[i, j];
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
				bw.Close();
			}
		}
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.Modifiers & Keys.Control) == Keys.Control)
			{
				if (e.KeyCode == Keys.O)
				{
					Open();
				}
			}
		}

		public MainForm()
		{
			LoadColorTable();
			DoubleBuffered = true;
			InitializeComponent();
			KeyPreview = true;
			KeyDown += MainForm_KeyDown;

			MenuStrip = new MenuStrip()
			{
				BackColor = Color.FromArgb(37, 37, 38),
				ForeColor = Color.White,
				Renderer = new MenuStripRender()
			};
			ToolStripMenuItem FileMenuItem = new ToolStripMenuItem("文件")
			{
				ForeColor = Color.White
			};
			AddMenuItem(FileMenuItem, "打开", (s, e) => Open());
			AddMenuItem(FileMenuItem, "保存", (s, e) => Save());
			AddMenuItem(FileMenuItem, "退出", (s, e) => Environment.Exit(0));
			MenuStrip.Items.Add(FileMenuItem);
			Controls.Add(MenuStrip);
			BackColor = bColor;
			Tab = new TabControlEx()
			{
				Location = new Point(0, 25),
				Size = new Size(1000, 750),
			};
			Controls.Add(Tab);

		}
	}
}
