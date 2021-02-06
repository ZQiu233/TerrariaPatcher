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
	public class Class2
	{
		private static void A()
		{
			JArray arr = new JArray();
			for (int i = 0; i < Main.maxNPCTypes; i++)
			{
				NPC npc = new NPC();
				npc.SetDefaults(i);
				JObject j = new JObject();
				j["Type"] = i;
				j["Name"] = npc.FullName;
				arr.Add(j);
			}
			File.WriteAllText("./NPCName.json", arr.ToString(Formatting.Indented));
		}
		private static void B()
		{
			JArray arr = new JArray();
			for (int i = 0; i < Main.maxNPCTypes; i++)
			{
				NPC npc = new NPC();
				npc.SetDefaults(i);
				JObject j = new JObject();
				j["Type"] = i;
				j["Name"] = npc.FullName;
				j["AiStyle"] = npc.aiStyle;
				j["Width"] = npc.width;
				j["Height"] = npc.height;
				j["Color"] = new JObject();
				j["Color"]["R"] = npc.color.R;
				j["Color"]["G"] = npc.color.G;
				j["Color"]["B"] = npc.color.B;
				j["Color"]["A"] = npc.color.A;
				j["Value"] = npc.value;
				j["TownNPC"] = npc.townNPC;
				j["Friendly"] = npc.friendly;
				j["Boss"] = npc.boss;
				j["DefDamage"] = npc.defDamage;
				j["DefDefense"] = npc.defDefense;
				j["LifeMax"] = npc.lifeMax;
				j["KnockBackResist"] = npc.knockBackResist;
				arr.Add(j);
			}
			File.WriteAllText("./NPCInfo.json", arr.ToString(Formatting.Indented));
		}
	}
}
