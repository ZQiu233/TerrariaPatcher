using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScheMaker
{
	[StructLayout(LayoutKind.Sequential)]
	public struct CTile:ICloneable
	{
		public ushort Type;
		public byte Wall;
		public byte Liquid;
		public byte BTileHeader;
		public byte BTileHeader2;
		public byte BTileHeader3;
		public short FrameX;
		public short FrameY;
		public short STileHeader;

		public byte Color()
		{
			return (byte)(this.STileHeader & 31);
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x00293CDF File Offset: 0x00291EDF
		public void Color(byte color)
		{
			if (color > 30)
			{
				color = 30;
			}
			this.STileHeader = (short)(((int)this.STileHeader & 65504) | (int)color);
		}

		public void Active(bool active)
		{
			if (active)
			{
				STileHeader |= 32;
				return;
			}
			STileHeader = (short)((int)STileHeader & 65503);
		}
		public bool Active()
		{
			return (STileHeader & 32) == 32;
		}

		public object Clone()
		{
			return new CTile() { Type = Type, Wall = Wall, Liquid = Liquid, BTileHeader = BTileHeader, BTileHeader2 = BTileHeader2, BTileHeader3 = BTileHeader3, FrameX = FrameX, FrameY = FrameY, STileHeader = STileHeader };
		}
	}
}
