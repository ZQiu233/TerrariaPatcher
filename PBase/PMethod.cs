using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBase
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false, AllowMultiple = true)]
	public class PMethod : Attribute
	{
		public PMethod(PMethodOption option, string MethodName, string Description, bool willCheck = false)
		{

		}
	}
	public enum PMethodOption
	{
		Move,
		Replace,
		Hook
	}
}
