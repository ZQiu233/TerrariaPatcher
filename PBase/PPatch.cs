using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBase
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public class PPatch : Attribute
	{
		public PPatch(PPatchOption option, string Namespace, string TypeName, string Description, bool moveStaticVarible = false, bool moveDynamicVarible = false, bool moveAnonymousType = false)
		{

		}
	}
	public enum PPatchOption
	{
		Merge,
		Move
	}
}
