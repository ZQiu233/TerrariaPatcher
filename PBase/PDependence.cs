using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBase
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
	public class PDependence : Attribute
	{
		public PDependence(PDependenceOption option, bool isClass, string name)
		{

		}
	}
	public enum PDependenceOption
	{
		Error,
		Warning,
		Skip
	}
}
