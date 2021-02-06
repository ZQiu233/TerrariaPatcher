using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace PUI
{
	public class Utils
	{
		public static T OurLoad<T>(string file)
		{
			return Main.instance.Content.Load<T>(file);
		}
	}
}
