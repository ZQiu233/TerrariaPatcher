using PBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace PHooks
{
	[PPatch(PPatchOption.Merge, "Terraria", "Player", "None", true, true, true)]
	internal class IPlayer : Player
	{
		[PMethod(PMethodOption.Replace, "Hurt", "None")]
		public double Hurt_Hooked(PlayerDeathReason damageSource, int Damage, int hitDirection, bool pvp, bool quiet, bool Crit, int cooldownCounter)
		{
			if (!PHooks.Hooks.PlayerHurt.DispatchPre(this, damageSource, Damage, hitDirection, pvp, quiet, Crit, cooldownCounter)) return Damage;
			double d = Hurt(damageSource, Damage, hitDirection, pvp, quiet, Crit, cooldownCounter);
			PHooks.Hooks.PlayerHurt.DispatchAfter(this, damageSource, Damage, hitDirection, pvp, quiet, Crit, cooldownCounter);
			return d;
		}
		[PMethod(PMethodOption.Replace, "UpdateArmorSets", "None")]
		public void UpdateArmorSets_Hooked(int i)
		{
			PHooks.Hooks.PlayerUpdateArmorSets.DispatchPre(this, i);
			UpdateArmorSets(i);
			PHooks.Hooks.PlayerUpdateArmorSets.DispatchAfter(this, i);
		}
		[PMethod(PMethodOption.Replace, "AddBuff", "None")]
		public void AddBuff_Hooked(int type, int time, bool quiet)
		{
			PHooks.Hooks.PlayerAddBuff.DispatchPre(this, type, time, quiet);
			AddBuff(type, time, quiet);
			PHooks.Hooks.PlayerUpdateArmorSets.DispatchAfter(this, type, time, quiet);
		}
		[PMethod(PMethodOption.Replace, "ResetEffects", "None")]
		public void ResetEffects_Hooked()
		{
			PHooks.Hooks.ResetEffects.DispatchPre(this);
			ResetEffects();
			PHooks.Hooks.ResetEffects.DispatchAfter(this);
		}
	}
}
