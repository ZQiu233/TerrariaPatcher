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
	public class EnviromentBar : Bar
	{

		public static Texture2D DayTimeIcon = Main.instance.OurLoad<Texture2D>("Qiu/UI/SunIcon");
		public static Texture2D NightTimeIcon = Main.instance.OurLoad<Texture2D>("Qiu/UI/MoonIcon");
		public static Texture2D RainIcon = Main.instance.OurLoad<Texture2D>("Qiu/UI/RainIcon");
		public static Texture2D RainStopIcon = Main.instance.OurLoad<Texture2D>("Qiu/UI/RainStop");

		public EnviromentBar()
		{
			Size = new Vector2(160, 50);
			Image DayTimeImage = new Image(DayTimeIcon) { ToolTip = "Daytime" };
			DayTimeImage.OnClick += DayTimeImage_OnClick;

			Image NightTimeImage = new Image(NightTimeIcon) { ToolTip = "Nighttime" };
			NightTimeImage.OnClick += NightTimeImage_OnClick;


			Image RainImage = new Image(RainIcon) { ToolTip = "Rain" };
			RainImage.OnClick += RainImage_OnClick;
			Image RainStopImage = new Image(RainStopIcon) { ToolTip = "Stop Rain" };
			RainStopImage.OnClick += RainStopImage_OnClick;

			Controls.Add(DayTimeImage);
			Controls.Add(NightTimeImage);

			Controls.Add(RainImage);
			Controls.Add(RainStopImage);

		}

		private void RainStopImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			Main.StopRain();
		}

		private void RainImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			Main.StartRain();
		}

		private void NightTimeImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			Main.dayTime = false;
			Main.time = 1;
		}

		private void DayTimeImage_OnClick(object arg1, PUI.EventArgs.OnClickEventArgs arg2)
		{
			Main.dayTime = true;
			Main.time = 1;
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
