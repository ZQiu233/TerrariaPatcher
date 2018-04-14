using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet.Emit;

namespace ILFactory
{
	public enum ILEmitOption
	{
		None,
		Replace,
		Insert,
		Remove
	}
	public class ILEmit
	{
		public UInt32 Offset { get; private set; }
		public ILEmitOption EmitOption;
		public Instruction IL;
		public ILEmit SetOffset(UInt32 Offset)
		{
			this.Offset = Offset;
			return this;
		}
		public static ILEmit Create(ILEmitOption option, Instruction IL)
		{
			return new ILEmit() { EmitOption = option, IL = IL };
		}
		public static ILEmit None()
		{
			return ILEmit.Create(ILEmitOption.None, null);
		}
		public static ILEmit Replace(Instruction IL)
		{
			return ILEmit.Create(ILEmitOption.Replace, IL);
		}
		public static ILEmit Insert(Instruction IL)
		{
			return ILEmit.Create(ILEmitOption.Insert, IL);
		}
		public static ILEmit Remove()
		{
			return ILEmit.Create(ILEmitOption.Remove, null);
		}

		private ILEmit()
		{
		}
	}

}
