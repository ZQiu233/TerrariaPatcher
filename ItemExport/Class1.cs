using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace ItemExport
{
	public class Class1
	{
		static Class1()
		{
			PHooks.Hooks.PlayerHurt.Pre += PlayerHurt_Pre;
		}

		private static void A()
		{
			JArray arr = new JArray();
			for (int i = 0; i < Main.maxItemTypes; i++)
			{
				Item item = new Item();
				item.SetDefaults(i);
				arr.Add(item.Name);
			}
			File.WriteAllText("./ItemName_cn.json", arr.ToString(Formatting.Indented));
		}

		private static void D()
		{
			string[] items = new string[Main.maxItemTypes];
			for (int i = 0; i < Main.maxItemTypes; i++)
			{
				Item itm = new Item();
				itm.SetDefaults(i);
				string s = "";
				for (int j = 0; j < itm.ToolTip.Lines; j++)
				{
					s += itm.ToolTip.GetLine(j);
					s += "\r\n";
				}
				items[i] = s;
			}
			File.WriteAllText("./D.json", JsonConvert.SerializeObject(items, Formatting.Indented));
		}

		private static void C()
		{
			Item[] items = new Item[Main.maxItemTypes];
			for (int i = 0; i < Main.maxItemTypes; i++)
			{
				items[i] = new Item();
				items[i].SetDefaults(i);
			}
			File.WriteAllText("./C.json", JsonConvert.SerializeObject(items, Formatting.Indented));
		}

		private static void B()
		{
			JArray arr1 = new JArray();
			for (int i = 0; i < Recipe.maxRecipes; i++)
			{
				Recipe rcp = Main.recipe[i];
				var t = new JObject();
				arr1.Add(t);
				if (rcp == null)
					continue;
				JArray a = new JArray();
				JArray b = new JArray();

				t.Add("item", new JObject(
					new JProperty("type", rcp.createItem.type),
					new JProperty("stack", rcp.createItem.stack)));
				t.Add("rItems", a);
				t.Add("rTiles", b);
				for (int j = 0; j < Recipe.maxRequirements; j++)
				{

					if (rcp.requiredItem[j] != null)
						a.Add(
							new JObject(
								new JProperty("type", rcp.requiredItem[j].type),
								new JProperty("stack", rcp.requiredItem[j].stack)));
					else
						a.Add(new JObject(
								new JProperty("type", 0),
								new JProperty("stack", 0)));
					b.Add(rcp.requiredTile[j]);
				}
			}
			File.WriteAllText("./B.json", arr1.ToString(Formatting.Indented));
		}


		private static bool PlayerHurt_Pre(object[] arg)
		{
			A();
			B();
			C();

			return true;
		}
	}
}
