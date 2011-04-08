#if NETMF
using System;
using System.Runtime.InteropServices;

namespace MiscUtil.Conversion
{
	/// <summary>
	/// Equivalent of System.BitConverter, but with either endianness.
	/// </summary>
	public abstract class EndianBitConverter
	{
		#region Endianness of this converter
		/// <summary>
		/// Indicates the byte order ("endianess") in which data is converted using this class.
		/// </summary>
		/// <remarks>
		/// Different computer architectures store data using different byte orders. "Big-endian"
		/// means the most significant byte is on the left end of a word. "Little-endian" means the 
		/// most significant byte is on the right end of a word.
		/// </remarks>
		/// <returns>true if this converter is little-endian, false otherwise.</returns>
		public abstract bool IsLittleEndian();

		/// <summary>
		/// Indicates the byte order ("endianess") in which data is converted using this class.
		/// </summary>
		public abstract Endianness Endianness { get; }
		#endregion

		#region Factory properties
		static LittleEndianBitConverter little = new LittleEndianBitConverter();
		/// <summary>
		/// Returns a little-endian bit converter instance. The same instance is
		/// always returned.
		/// </summary>
		public static LittleEndianBitConverter Little
		{
			get { return little; }
		}

		static BigEndianBitConverter big = new BigEndianBitConverter();
		/// <summary>
		/// Returns a big-endian bit converter instance. The same instance is
		/// always returned.
		/// </summary>
		public static BigEndianBitConverter Big
		{
			get { return big; }
		}
		#endregion

		#region To(PrimitiveType) conversions

		/// <summary>
		/// Returns a Unicode character converted from two bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="startIndex">The starting position within value.</param>
		/// <returns>A character formed by two bytes beginning at startIndex.</returns>
		public char ToChar (byte[] value, int startIndex)
		{
			return unchecked((char) (CheckedFromBytes(value, startIndex, 2)));
		}

		/// <summary>
		/// Returns a 16-bit signed integer converted from two bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="startIndex">The starting position within value.</param>
		/// <returns>A 16-bit signed integer formed by two bytes beginning at startIndex.</returns>
		public short ToInt16 (byte[] value, int startIndex)
		{
			return unchecked((short) (CheckedFromBytes(value, startIndex, 2)));
		}

		/// <summary>
		/// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="startIndex">The starting position within value.</param>
		/// <returns>A 32-bit signed integer formed by four bytes beginning at startIndex.</returns>
		public int ToInt32 (byte[] value, int startIndex)
		{
			return unchecked((int) (CheckedFromBytes(value, startIndex, 4)));
		}

		/// <summary>
		/// Returns a 64-bit signed integer converted from eight bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="startIndex">The starting position within value.</param>
		/// <returns>A 64-bit signed integer formed by eight bytes beginning at startIndex.</returns>
		public long ToInt64 (byte[] value, int startIndex)
		{
			return CheckedFromBytes(value, startIndex, 8);
		}

		/// <summary>
		/// Returns a 16-bit unsigned integer converted from two bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="startIndex">The starting position within value.</param>
		/// <returns>A 16-bit unsigned integer formed by two bytes beginning at startIndex.</returns>
		public ushort ToUInt16 (byte[] value, int startIndex)
		{
			return unchecked((ushort) (CheckedFromBytes(value, startIndex, 2)));
		}

		/// <summary>
		/// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="startIndex">The starting position within value.</param>
		/// <returns>A 32-bit unsigned integer formed by four bytes beginning at startIndex.</returns>
		public uint ToUInt32 (byte[] value, int startIndex)
		{
			return unchecked((uint) (CheckedFromBytes(value, startIndex, 4)));
		}

		/// <summary>
		/// Returns a 64-bit unsigned integer converted from eight bytes at a specified position in a byte array.
		/// </summary>
		/// <param name="value">An array of bytes.</param>
		/// <param name="startIndex">The starting position within value.</param>
		/// <returns>A 64-bit unsigned integer formed by eight bytes beginning at startIndex.</returns>
		public ulong ToUInt64 (byte[] value, int startIndex)
		{
			return unchecked((ulong) (CheckedFromBytes(value, startIndex, 8)));
		}

		/// <summary>
		/// Checks the given argument for validity.
		/// </summary>
		/// <param name="value">The byte array passed in</param>
		/// <param name="startIndex">The start index passed in</param>
		/// <param name="bytesRequired">The number of bytes required</param>
		/// <exception cref="ArgumentNullException">value is a null reference</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// startIndex is less than zero or greater than the length of value minus bytesRequired.
		/// </exception>
		static void CheckByteArgument(byte[] value, int startIndex, int bytesRequired)
		{
			if (value==null)
			{
				throw new ArgumentNullException("value");
			}
			if (startIndex < 0 || startIndex > value.Length-bytesRequired)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
		}

        /// <summary>
        /// Checks the arguments for validity before calling FromBytes
        /// (which can therefore assume the arguments are valid).
        /// </summary>
        /// <param name="value">The bytes to convert after checking</param>
        /// <param name="startIndex">The index of the first byte to convert</param>
        /// <param name="bytesToConvert">The number of bytes to convert</param>
        /// <returns></returns>
		long CheckedFromBytes(byte[] value, int startIndex, int bytesToConvert)
		{
			CheckByteArgument(value, startIndex, bytesToConvert);
			return FromBytes(value, startIndex, bytesToConvert);
		}

		/// <summary>
		/// Convert the given number of bytes from the given array, from the given start
		/// position, into a long, using the bytes as the least significant part of the long.
		/// By the time this is called, the arguments have been checked for validity.
		/// </summary>
		/// <param name="value">The bytes to convert</param>
		/// <param name="startIndex">The index of the first byte to convert</param>
		/// <param name="bytesToConvert">The number of bytes to use in the conversion</param>
		/// <returns>The converted number</returns>
		protected abstract long FromBytes(byte[] value, int startIndex, int bytesToConvert);
		#endregion

		#region GetBytes conversions
		/// <summary>
		/// Returns an array with the given number of bytes formed
		/// from the least significant bytes of the specified value.
		/// This is used to implement the other GetBytes methods.
		/// </summary>
		/// <param name="value">The value to get bytes for</param>
		/// <param name="bytes">The number of significant bytes to return</param>
		byte[] GetBytes(long value, int bytes)
		{
			byte[] buffer = new byte[bytes];
			CopyBytes(value, bytes, buffer, 0);
			return buffer;
		}

		/// <summary>
		/// Returns the specified Unicode character value as an array of bytes.
		/// </summary>
		/// <param name="value">A character to convert.</param>
		/// <returns>An array of bytes with length 2.</returns>
		public byte[] GetBytes(char value)
		{
			return GetBytes(value, 2);
		}
		
		/// <summary>
		/// Returns the specified 16-bit signed integer value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 2.</returns>
		public byte[] GetBytes(short value)
		{
			return GetBytes(value, 2);
		}

		/// <summary>
		/// Returns the specified 32-bit signed integer value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 4.</returns>
		public byte[] GetBytes(int value)
		{
			return GetBytes(value, 4);
		}

		/// <summary>
		/// Returns the specified 64-bit signed integer value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 8.</returns>
		public byte[] GetBytes(long value)
		{
			return GetBytes(value, 8);
		}

		/// <summary>
		/// Returns the specified 16-bit unsigned integer value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 2.</returns>
		public byte[] GetBytes(ushort value)
		{
			return GetBytes(value, 2);
		}

		/// <summary>
		/// Returns the specified 32-bit unsigned integer value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 4.</returns>
		public byte[] GetBytes(uint value)
		{
			return GetBytes(value, 4);
		}

		/// <summary>
		/// Returns the specified 64-bit unsigned integer value as an array of bytes.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <returns>An array of bytes with length 8.</returns>
		public byte[] GetBytes(ulong value)
		{
			return GetBytes(unchecked((long)value), 8);
		}

		#endregion

		#region CopyBytes conversions
		/// <summary>
		/// Copies the given number of bytes from the least-specific
		/// end of the specified value into the specified byte array, beginning
		/// at the specified index.
		/// This is used to implement the other CopyBytes methods.
		/// </summary>
		/// <param name="value">The value to copy bytes for</param>
		/// <param name="bytes">The number of significant bytes to copy</param>
		/// <param name="buffer">The byte array to copy the bytes into</param>
		/// <param name="index">The first index into the array to copy the bytes into</param>
		void CopyBytes(long value, int bytes, byte[] buffer, int index)
		{
			if (buffer==null)
			{
				throw new ArgumentNullException("buffer", "Byte array must not be null");
			}
			if (buffer.Length < index+bytes)
			{
				throw new ArgumentOutOfRangeException("Buffer not big enough for value");
			}
			CopyBytesImpl(value, bytes, buffer, index);
		}

		/// <summary>
		/// Copies the given number of bytes from the least-specific
		/// end of the specified value into the specified byte array, beginning
		/// at the specified index.
		/// This must be implemented in concrete derived classes, but the implementation
		/// may assume that the value will fit into the buffer.
		/// </summary>
		/// <param name="value">The value to copy bytes for</param>
		/// <param name="bytes">The number of significant bytes to copy</param>
		/// <param name="buffer">The byte array to copy the bytes into</param>
		/// <param name="index">The first index into the array to copy the bytes into</param>
		protected abstract void CopyBytesImpl(long value, int bytes, byte[] buffer, int index);

		/// <summary>
		/// Copies the specified Boolean value into the specified byte array,
		/// beginning at the specified index.
		/// </summary>
		/// <param name="value">A Boolean value.</param>
		/// <param name="buffer">The byte array to copy the bytes into</param>
		/// <param name="index">The first index into the array to copy the bytes into</param>
		public void CopyBytes(bool value, byte[] buffer, int index)
		{
			CopyBytes(value ? 1 : 0, 1, buffer, index);
		}

		/// <summary>
		/// Copies the specified Unicode character value into the specified byte array,
		/// beginning at the specified index.
		/// </summary>
		/// <param name="value">A character to convert.</param>
		/// <param name="buffer">The byte array to copy the bytes into</param>
		/// <param name="index">The first index into the array to copy the bytes into</param>
		public void CopyBytes(char value, byte[] buffer, int index)
		{
			CopyBytes(value, 2, buffer, index);
		}

		
		/// <summary>
		/// Copies the specified 16-bit signed integer value into the specified byte array,
		/// beginning at the specified index.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <param name="buffer">The byte array to copy the bytes into</param>
		/// <param name="index">The first index into the array to copy the bytes into</param>
		public void CopyBytes(short value, byte[] buffer, int index)
		{
			CopyBytes(value, 2, buffer, index);
		}

		/// <summary>
		/// Copies the specified 32-bit signed integer value into the specified byte array,
		/// beginning at the specified index.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <param name="buffer">The byte array to copy the bytes into</param>
		/// <param name="index">The first index into the array to copy the bytes into</param>
		public void CopyBytes(int value, byte[] buffer, int index)
		{
			CopyBytes(value, 4, buffer, index);
		}

		/// <summary>
		/// Copies the specified 64-bit signed integer value into the specified byte array,
		/// beginning at the specified index.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <param name="buffer">The byte array to copy the bytes into</param>
		/// <param name="index">The first index into the array to copy the bytes into</param>
		public void CopyBytes(long value, byte[] buffer, int index)
		{
			CopyBytes(value, 8, buffer, index);
		}

		/// <summary>
		/// Copies the specified 16-bit unsigned integer value into the specified byte array,
		/// beginning at the specified index.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <param name="buffer">The byte array to copy the bytes into</param>
		/// <param name="index">The first index into the array to copy the bytes into</param>
		public void CopyBytes(ushort value, byte[] buffer, int index)
		{
			CopyBytes(value, 2, buffer, index);
		}

		/// <summary>
		/// Copies the specified 32-bit unsigned integer value into the specified byte array,
		/// beginning at the specified index.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <param name="buffer">The byte array to copy the bytes into</param>
		/// <param name="index">The first index into the array to copy the bytes into</param>
		public void CopyBytes(uint value, byte[] buffer, int index)
		{
			CopyBytes(value, 4, buffer, index);
		}

		/// <summary>
		/// Copies the specified 64-bit unsigned integer value into the specified byte array,
		/// beginning at the specified index.
		/// </summary>
		/// <param name="value">The number to convert.</param>
		/// <param name="buffer">The byte array to copy the bytes into</param>
		/// <param name="index">The first index into the array to copy the bytes into</param>
		public void CopyBytes(ulong value, byte[] buffer, int index)
		{
			CopyBytes(unchecked((long)value), 8, buffer, index);
		}

		#endregion
	}
}
#endif