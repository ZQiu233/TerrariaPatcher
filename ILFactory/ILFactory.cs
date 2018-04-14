using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILRepacking;
using System.IO;

namespace ILFactory
{
	public class ILFactory
	{
		private dnlib.Threading.Collections.IList<Instruction> ILCode;
		public static ILFactory Create(dnlib.Threading.Collections.IList<Instruction> ILCode)
		{
			return new ILFactory() { ILCode = ILCode };
		}
		internal int FindFirst(int begin, ILElement il)
		{
			if (begin >= ILCode.Count) return -1;
			for (int i = begin; i < ILCode.Count; i++)
			{
				if (il.Match(ILCode[i])) return i;
			}
			return -1;
		}
		public static int Find(ILFactory fac, int begin, ILElement[] il)
		{
			return fac.Find(begin, il);
		}
		public int Find(int begin, ILElement[] il)
		{
			if (il.Length > ILCode.Count || begin >= ILCode.Count)
			{
				return -1;
			}
			int first = FindFirst(begin, il[0]);
			if (first == -1 || il.Length > ILCode.Count - first)
			{
				return -1;
			}
			for (int i = 0; i < il.Length; i++)
			{
				if (!ILElement.Match(il[i], ILCode[first + i]))
				{
					return Find(begin + 1, il);
				}
			}
			return first;
		}
		public int Find(ILElement[] il)
		{
			return Find(0, il);
		}

		public static void Emit(ILFactory fac, int index, ILEmit[] emitions)
		{
			fac.Emit(index, emitions);
		}
		public void Emit(int index, ILEmit[] emitions)
		{
			if (index == -1) throw new Exception("Faild to search the array of IL code");
			int i = 0, j = 0;
			while (i < emitions.Count())
			{
				if (Emit(index + j, emitions[i]))
					j++;
				i++;
			}
		}

		public void FindAndEmit(ILElement[] il, ILEmit[] emitions)
		{
			Emit(Find(il), emitions);
		}

		public bool Emit(int index, ILEmit emit)
		{
			switch (emit.EmitOption)
			{
				case ILEmitOption.None:
					return true;
				case ILEmitOption.Replace:
					ILCode[index] = new Instruction(emit.IL.OpCode, emit.IL.Operand);
					return true;
				case ILEmitOption.Insert:
					ILCode.Insert(index, new Instruction(emit.IL.OpCode, emit.IL.Operand));
					return true;
				case ILEmitOption.Remove:
					ILCode.RemoveAt(index);
					return false;
			}
			return true;
		}



		private ILFactory()
		{
		}

	}
}
