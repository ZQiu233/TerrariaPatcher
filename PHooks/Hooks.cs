using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHooks
{
	public class Hooks
	{
		public static BaseHook Chat = new BaseHook();
		public static BaseHook Draw = new BaseHook();
		public static BaseHook Update = new BaseHook();
		public static BaseHook PlayerHurt = new BaseHook();
		public static BaseHook PlayerAddBuff = new BaseHook();
		public static BaseHook PlayerUpdateArmorSets = new BaseHook();
		public static BaseHook PlayerItemCheckWrapped = new BaseHook();


		public static BaseHook InterfaceLayersSetup = new BaseHook();
		public static BaseHook ResetEffects = new BaseHook();
	}
}
