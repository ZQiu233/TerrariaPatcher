using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using Terraria.UI.Chat;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Graphics.Shaders;
using PBase;
using Terraria.Social;

namespace Utils
{
	[PPatch(PPatchOption.Merge, "Terraria.Social", "SocialAPI", "NoSteam")]
	public class NoSteam
	{
		[PMethod(PMethodOption.Replace, "Initialize", "None")]
		public static void Initialize_Patched(SocialMode? mode = null)
		{
			if (!mode.HasValue)
			{
				mode = new SocialMode?(SocialMode.None);
				mode = new SocialMode?(SocialMode.Steam);
			}
			SocialAPI._mode = mode.Value;
			SocialAPI._modules = new List<ISocialModule>();
			using (List<ISocialModule>.Enumerator enumerator = SocialAPI._modules.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					enumerator.Current.Initialize();
				}
			}
		}
	}
	[PPatch(PPatchOption.Merge, "Terraria", "Main", "", true, true, true)]
	public class Utils
	{
		public static bool BackspaceProtection;

		[PMethod(PMethodOption.Replace, "GetInputText", "Fixed input bug")]
		public static string GetInputText_Patched(string oldString)
		{
			if (!Main.hasFocus)
			{
				return oldString;
			}
			Main.inputTextEnter = false;
			Main.inputTextEscape = false;
			string text = oldString;
			string text2 = "";
			if (text == null)
			{
				text = "";
			}
			bool flag = false;
			if (Main.inputText.IsKeyDown(Keys.LeftControl) || Main.inputText.IsKeyDown(Keys.RightControl))
			{
				if (Main.inputText.IsKeyDown(Keys.Z) && !Main.oldInputText.IsKeyDown(Keys.Z))
				{
					text = "";
				}
				else if (Main.inputText.IsKeyDown(Keys.X) && !Main.oldInputText.IsKeyDown(Keys.X))
				{
					Platform.Current.Clipboard = oldString;
					text = "";
				}
				else if ((Main.inputText.IsKeyDown(Keys.C) && !Main.oldInputText.IsKeyDown(Keys.C)) || (Main.inputText.IsKeyDown(Keys.Insert) && !Main.oldInputText.IsKeyDown(Keys.Insert)))
				{
					Platform.Current.Clipboard = oldString;
				}
				else if (Main.inputText.IsKeyDown(Keys.V) && !Main.oldInputText.IsKeyDown(Keys.V))
				{
					text2 += Platform.Current.Clipboard;
				}
			}
			else
			{
				if (Main.inputText.PressingShift())
				{
					if (Main.inputText.IsKeyDown(Keys.Delete) && !Main.oldInputText.IsKeyDown(Keys.Delete))
					{
						Platform.Current.Clipboard = oldString;
						text = "";
					}
					if (Main.inputText.IsKeyDown(Keys.Insert) && !Main.oldInputText.IsKeyDown(Keys.Insert))
					{
						string text3 = Platform.Current.Clipboard;
						for (int i = 0; i < text3.Length; i++)
						{
							if (text3[i] < ' ' || text3[i] == '\u007f')
							{
								text3 = text3.Replace(text3[i--].ToString() ?? "", "");
							}
						}
						text2 += text3;
					}
				}
				for (int j = 0; j < Main.keyCount; j++)
				{
					int num = Main.keyInt[j];
					string str = Main.keyString[j];
					if (num == 13)
					{
						Main.inputTextEnter = true;
					}
					else if (num == 27)
					{
						Main.inputTextEscape = true;
					}
					else if (num >= 32 && num != 127)
					{
						text2 += str;
					}
				}
			}
			Main.keyCount = 0;
			text += text2;
			Main.oldInputText = Main.inputText;
			Main.inputText = Keyboard.GetState();
			Keys[] pressedKeys = Main.inputText.GetPressedKeys();
			Keys[] pressedKeys2 = Main.oldInputText.GetPressedKeys();
			if (!Main.inputText.IsKeyDown(Keys.Back))
			{
				Utils.BackspaceProtection = false;
			}
			if (!string.IsNullOrWhiteSpace(Platform.Current.Ime.CompositionString))
			{
				Utils.BackspaceProtection = true;
			}
			else if (Main.oldInputText.IsKeyDown(Keys.Back) && !Utils.BackspaceProtection)
			{
				if (Main.backSpaceCount == 0)
				{
					Main.backSpaceCount = 7;
					flag = true;
				}
				Main.backSpaceCount--;
			}
			else
			{
				Main.backSpaceCount = 15;
			}
			if (!Utils.BackspaceProtection && text.Length > 0)
			{
				for (int k = 0; k < pressedKeys.Length; k++)
				{
					bool flag2 = true;
					for (int l = 0; l < pressedKeys2.Length; l++)
					{
						if (pressedKeys[k] == pressedKeys2[l])
						{
							flag2 = false;
						}
					}
					if (string.Concat(pressedKeys[k]) == "Back" && (flag2 | flag))
					{
						TextSnippet[] array = ChatManager.ParseMessage(text, Color.White).ToArray();
						if (array[array.Length - 1].DeleteWhole)
						{
							text = text.Substring(0, text.Length - array[array.Length - 1].TextOriginal.Length);
						}
						else
						{
							text = text.Substring(0, text.Length - 1);
						}
					}
				}
			}
			return text;
		}
	}
}
