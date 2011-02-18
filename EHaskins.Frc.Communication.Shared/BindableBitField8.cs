using System;
	using System.IO;
using System.ComponentModel;
using MiscUtil.IO;

namespace EHaskins.Frc.Communication
{


	public class BindableBitField8 : BitField8
	{
		public BindableBitField8()
		{

		}
		public BindableBitField8(byte value)
			: base(value)
		{

		}

		public override byte RawValue
		{
			get
			{
				return base.RawValue;
			}
			set
			{
				base.RawValue = value;
			}
		}
	}
}
