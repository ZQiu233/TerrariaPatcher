using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILFactory
{
	public class ILElement
	{
		public OpCode opCode;
		public string operand;
		public ILElement(OpCode opCode, string operand)
		{
			this.opCode = opCode;
			this.operand = operand;
		}
		public bool Match(Instruction ins)
		{
			return Match(this, ins);
		}
		public static bool Match(ILElement il, Instruction ins)
		{
			if (ins.OpCode.Value == il.opCode.Value)
			{
				if (il.operand == null)
					return true;
				if (il.operand == "*")
					return true;
				if (ins.Operand != null && ins.Operand.ToString().Contains(il.operand))
					return true;
				return false;
			}
			return false;
		}
	}
}
