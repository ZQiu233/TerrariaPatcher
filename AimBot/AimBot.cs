using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.GameInput;

namespace AimBot
{
	public class AimBot
	{
		public static int _timer = 0;
		public static bool Enable = false;
		public static float BulletSpeed = 48f;//初始值
		public static float Scalar = 0.5f;//初始值
		public static float Dist = 50 * 16f;
		public static float Dist_Mouse = 128;

		static AimBot()
		{
			PHooks.Hooks.Update.Pre += Update_Pre;
			PHooks.Hooks.PlayerItemCheckWrapped.Pre += PlayerItemCheckWrapped_Pre;
		}

		private static bool PlayerItemCheckWrapped_Pre(object[] arg)
		{
			var i = Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem].shootSpeed;
			if (i != 0)
			{
				BulletSpeed = i * Scalar;
			}
			return true;
		}

		private static bool Chat_Pre(object[] arg)
		{
			try
			{
				string c = ((ChatMessage)arg[0]).Text;
				if (c.StartsWith("."))
				{
					string[] s = c.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
					if (s[0] == ".cs")
					{
						if (s.Length < 2)
						{
							Main.NewText("需要一个参数");
							return false;
						}
						Scalar = Convert.ToSingle(s[1]);
						Main.NewText("提前量常数已修改为：" + Scalar);
					}
					else if (s[0] == ".cd")
					{
						if (s.Length < 2)
						{
							Main.NewText("需要一个参数");
							return false;
						}
						Dist = Convert.ToSingle(s[1]) * 16f;
						Main.NewText("怪物探索距离已修改为：" + Dist + "个方块距离");
					}
					return false;
				}
			}
			catch (Exception e)
			{
				Main.NewText(e.Message);
				return false;
			}
			return true;
		}

		private static bool Update_Pre(object[] obj)
		{
			if (_timer > 0) _timer--;
			if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.G) && Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && _timer == 0)
			{
				_timer = 60;
				Enable = !Enable;
				if (Enable)
					Main.NewText("自瞄已打开，按下Ctrl+G可关闭自瞄");
				else
					Main.NewText("自瞄已关闭，按下Ctrl+G可打开自瞄");
			}
			if (!Enable) return true;
			NPC p = null;
			if ((p = FindNPC()) != null)
			{
				if (!p.friendly)
				{
					Vector2 mobV = p.velocity;
					Vector2 a = Main.LocalPlayer.Center - p.Center;
					if (mobV.LengthSquared() == 0 || a.LengthSquared() == 0)
					{
						Vector2 t = p.Center - Main.LocalPlayer.Center;
						Vector2 y = Main.LocalPlayer.Center - Main.screenPosition + Dist_Mouse * Vector2.Normalize(t);
						PlayerInput.MouseX = (int)Math.Round(y.X);
						PlayerInput.MouseY = (int)Math.Round(y.Y);
					}
					else
					{
						float cos_t = Vector2.Dot(mobV, a) / (mobV.Length() * a.Length());
						float d = a.Length();
						float k = mobV.Length() / BulletSpeed;//48f是猜测的子弹速度

						{//开始复杂的计算
						 //delta  = 4k^2d^2cos_t^2-4d^2(k^2-1)
							double _delta = 4 * Math.Pow(k, 2) * Math.Pow(d, 2) * Math.Pow(cos_t, 2) - 4 * Math.Pow(d, 2) * (Math.Pow(k, 2) - 1);
							//negative b  = 2kd*cos_t
							double _nb = 2 * k * d * cos_t;
							//2a  = 2(k^2-1)
							double _2a = 2 * (Math.Pow(k, 2) - 1);
							float t = (float)((_nb - Math.Sqrt(_delta)) / _2a);//t为子弹要运动的距离，这里的正负号取负，如果取正则计算出的目标坐标在怪物身后
							float kt = k * t;//kt为怪物的运动距离
							Vector2 g = Vector2.Normalize(mobV) * kt;//g为怪物的运动向量
							Vector2 j = p.Center + g - Main.LocalPlayer.Center;//j为子弹的运动向量
							Vector2 y = Main.LocalPlayer.Center - Main.screenPosition + Dist_Mouse * Vector2.Normalize(j);//将其缩放并应用到玩家坐标上
							PlayerInput.MouseX = (int)Math.Round(y.X);
							PlayerInput.MouseY = (int)Math.Round(y.Y);
						}
					}
				}
			}
			return true;
		}



		private static Vector2 GetFormLocation()
		{
			return Main.instance.Window.ClientBounds.Location.ToVector2();
		}

		private static NPC FindNPC()
		{
			NPC npc = null;
			Vector2 v = new Vector2(float.MaxValue, float.MaxValue);
			foreach (var n in Main.npc)
			{
				if (Vector2.Distance(n.Center, Main.LocalPlayer.Center) > Dist)
					continue;
				Vector2 y = Main.LocalPlayer.Center - n.Center;
				if (n.active && (v = (v.Length() > y.Length() ? y : v)) == y)
					npc = n;
			}
			return npc;
		}
	}
}
