// Copyright (c) Bruno Brant. All rights reserved.

using System.Runtime.InteropServices;

namespace TakeABreak.Infra;

/// <summary>
/// User32 functions.
/// </summary>
internal static partial class NativeMethods
{
	/// <summary>
	/// Retrieves the time of the last input event.
	/// </summary>
	/// <param name="plii">
	/// A pointer to a LASTINPUTINFO structure that receives the time of the last input event.
	/// </param>
	/// <returns>
	/// If the function succeeds, the return value is true.
	/// If the function fails, the return value is false.
	/// </returns>
	[LibraryImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool GetLastInputInfo(ref LASTINPUTINFO plii);

	/// <summary>
	/// Contains the time of the last input.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct LASTINPUTINFO
	{
		/// <summary>
		/// Gets the value of sizeof(LASTINPUTINFO).
		/// </summary>
		public static readonly uint Size = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO));

		/// <summary>
		/// The size of the structure, in bytes. This member must be set to sizeof(LASTINPUTINFO).
		/// </summary>
		[MarshalAs(UnmanagedType.U4)]
		private uint cbSize;

		/// <summary>
		/// The tick count when the last input event was received.
		/// </summary>
		[MarshalAs(UnmanagedType.U4)]
		private uint dwTime;

		/// <summary>
		/// Gets or sets the tick count when the last input event was received.
		/// </summary>
		public uint DwTime { readonly get => dwTime; set => dwTime = value; }

		/// <summary>
		/// Gets or sets the size of the structure, in bytes. This member must be set to sizeof(LASTINPUTINFO).
		/// </summary>
		public uint CbSize { readonly get => cbSize; set => cbSize = value; }
	}
}
