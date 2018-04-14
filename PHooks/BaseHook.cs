using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHooks
{
	public class BaseHook
	{
		public event Func<object[], bool> Pre = o => true;
		public event Action<object[]> After = o => { };
		internal bool DispatchPre(params object[] args)
		{
			return !Pre.GetInvocationList().Select(d => ((Func<object[], bool>)d)(args)).Any(b => !b);
		}
		internal void DispatchAfter(params object[] args)
		{
			After.GetInvocationList().ToList().ForEach(d => ((Action<object[]>)d)(args));
		}
	}
}
