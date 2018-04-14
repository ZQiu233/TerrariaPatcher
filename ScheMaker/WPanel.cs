using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheMaker
{
	public class WPanel : Panel
	{
		public TileView TileView
		{
			get;
			private set;
		}
		public Microsoft.Xna.Framework.Color[,] Data
		{
			get => TileView.GetColors();
		}
		public WPanel(Microsoft.Xna.Framework.Color[,] Colors)
		{
			TileView = new TileView(Colors);
			TileView.Size = new Size(Colors.GetLength(0) * 10, Colors.GetLength(1) * 10);
			HandleCreated += WPanel_HandleCreated;
			Controls.Add(TileView);
		}
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			/*Size s = new SizeF(TileView.Width * (1f + (e.Delta > 0 ? 0.1f : -0.1f)), TileView.Height * (1f + (e.Delta > 0 ? 0.1f : -0.1f))).ToSize();
			if (s.Width / Data.GetLength(0) < 10 || s.Width / Data.GetLength(0) > 20 || s.Height / Data.GetLength(1) < 10 || s.Height / Data.GetLength(1) > 20)
				return;
			TileView.Size = s;
			TileView.Location = new Point(0, 0);*/
			TileView.ScaleTo(e.Delta > 0 ? 1.1f : 0.9f);
		}
		private void WPanel_HandleCreated(object sender, EventArgs e)
		{

		}
	}
}
