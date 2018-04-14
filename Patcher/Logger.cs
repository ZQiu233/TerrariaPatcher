using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Patcher
{
	class Logger
	{
		public int Level = 0;
		public Logger()
		{
			Console.WriteLine(DateTime.Now);
		}

		public void Log(string str)
		{
			for (int i = 0; i < Level; i++)
			{
				Console.Write(">");
			}
			Console.WriteLine(str);
		}
		
	}
}
