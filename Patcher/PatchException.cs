using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patcher
{
	class PatchException : Exception
	{
		public PatchException(string str) : base(str)
		{
		}
	}
}
