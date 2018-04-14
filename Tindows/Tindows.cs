using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.UI;
using PUI.EventArgs;
using Terraria.GameInput;
using PUI;
using System.IO;
using System.Reflection;

namespace Tindows
{
	public class Tindows
	{
		private static WindowManager WManager = new WindowManager();
		private static TaskBar TaskBar = new TaskBar();
		private static bool TaskBar_Hovered = false;
		private static StartMenu StartMenu = null;
		private static float BarOff = 0f;
		private static Texture2D TindowsLogo = Main.instance.OurLoad<Texture2D>("Qiu/UI/Windows10_F");
		static Tindows()
		{

			PHooks.Hooks.InterfaceLayersSetup.After += InterfaceLayersSetup_After;
			PHooks.Hooks.Update.After += Update_After;
			PHooks.Hooks.Update.Pre += Update_Pre;
			var Start = new Image(TindowsLogo)
			{
				ToolTip = "Start",
			};
			Start.OnClick += Start_OnClick;
			TaskBar.Add(Start);
			TaskBar.OnHover += TaskBar_OnHover;
		}

		private static Dictionary<Window, Image> Icons = new Dictionary<Window, Image>();

		public static void RegWindow(Window instance)
		{
			instance.OnClosing += Instance_OnClosing;
			WManager.Register(instance);
			Image taskIcon = new Image(instance.Icon) { ToolTip = instance.Title };
			taskIcon.OnClick += (s, e) =>
			{
				instance.Minimized = !instance.Minimized;
			};
			TaskBar.Add(taskIcon);
			Icons.Add(instance, taskIcon);
		}

		public static void UnregWindow(Window instance)
		{
			WManager.DisposeWindow(instance);
			TaskBar.Remove(Icons[instance]);
			Icons.Remove(instance);
		}

		private static void Instance_OnClosing(object arg1, PUI.EventArgs.EventArgs arg2)
		{
			UnregWindow((Window)arg1);
		}

		private static void Start_OnClick(object arg1, OnClickEventArgs arg2)
		{
			if (StartMenu == null)
			{
				List<Label> ls = new List<Label>();
				Directory.EnumerateFiles(".\\Content\\Qiu\\VM\\Apps", "*.dll").ToList().ForEach(d =>
				 {
					 string s = Path.GetFileNameWithoutExtension(d);
					 Label l = new Label(s);
					 l.OnClick += (sender, e) =>
					 {
						 var ASM = Assembly.Load(File.ReadAllBytes(d));
						 var j = ASM.GetTypes().Where(t =>
						   {
							   return t.CustomAttributes.Where(c =>
							   {
								   return c.AttributeType == typeof(AppMain);
							   }).Count() > 0;
						   });
						 if (j.Count() <= 0)
						 {
							 return;
						 }
						 var TType = j.ElementAt(0);
						 var obj = Activator.CreateInstance(TType);
						 TType.GetMethod("Main").Invoke(obj, new object[] { });
					 };
					 ls.Add(l);
				 });


				StartMenu = new StartMenu(ls.ToArray());
				StartMenu.Position = new Vector2(TaskBar.DrawPosition.X, TaskBar.DrawPosition.Y - StartMenu.Height);
				StartMenu.Focused = true;
			}
			else
			{
				StartMenu = null;
			}
		}

		private static void TaskBar_OnHover(object arg1, MouseEvent arg2)
		{
			TaskBar_Hovered = true;
			if (BarOff > 0)
			{
				BarOff--;
			}
		}

		private static bool Update_Pre(object[] arg)
		{
			return true;
		}

		private static void Update_After(object[] obj)
		{
			TaskBar_Hovered = false;
			TaskBar.Update();
			WManager.Update();
			if (!TaskBar_Hovered && BarOff < 25f && StartMenu == null)
			{
				BarOff++;
			}
			if (StartMenu != null)
			{
				StartMenu.Update();
				if (Main.mouseLeft)
				{
					if (!StartMenu.Inside(Main.mouseX, Main.mouseY) && !TaskBar.Inside(Main.mouseX, Main.mouseY))
					{
						StartMenu = null;
					}
				}
			}
		}

		private static void InterfaceLayersSetup_After(object[] obj)
		{
			int MouseTextIndex = Main.instance._gameInterfaceLayers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (MouseTextIndex != -1)
			{
				Main.instance._gameInterfaceLayers.Insert(MouseTextIndex + 1, new LegacyGameInterfaceLayer(
					"Tindows: UI",
					delegate
					{
						TaskBar.Size = new Vector2(600, 40);
						TaskBar.Position = new Vector2(Main.screenWidth / 2 - TaskBar.Width / 2, Main.screenHeight - TaskBar.Height + BarOff);
						TaskBar.Draw(Main.spriteBatch);
						WManager.Draw(Main.spriteBatch);
						if (StartMenu != null)
						{
							StartMenu.Draw(Main.spriteBatch);
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
