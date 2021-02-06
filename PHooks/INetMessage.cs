using PBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat;
using Terraria.GameContent.NetModules;
using Terraria.Net;

namespace PHooks
{
	[PPatch(PPatchOption.Merge, "Terraria", "NetMessage", "None", true, true, true)]
	internal class INetMessage
	{
		/*[PMethod(PMethodOption.Replace, "SendChatMessageFromClient", "None")]
		public static void SendChatMessageFromClient_Hooked(ChatMessage text)
		{
			if (!Hooks.Chat.DispatchPre(text)) return;
			Terraria.NetMessage.SendChatMessageFromClient(text);
			Hooks.Chat.DispatchAfter(text);
		}*/
	}
}
