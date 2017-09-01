using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace WriteFileHooker
{
	public class WinType
	{
		public struct BY_HANDLE_FILE_INFORMATION
		{
			public UInt32 dwFileAttributes;
			public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
			public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
			public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
			public UInt32 dwVolumeSerialNumber;
			public UInt32 nFileSizeHigh;
			public UInt32 nFileSizeLow;
			public UInt32 nNumberOfLinks;
			public UInt32 nFileIndexHigh;
			public UInt32 nFileIndexLow;
		}

		[StructLayout(LayoutKind.Explicit, Size = 8)]
		public struct LARGE_INTEGER
		{
			[FieldOffset(0)]
			public Int64 QuadPart;
			[FieldOffset(0)]
			public UInt32 LowPart;
			[FieldOffset(4)]
			public Int32 HighPart;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct FILE_FULL_DIR_INFO
		{
			public UInt32 NextEntryOffset;
			public UInt32 FileIndex;
			public LARGE_INTEGER CreationTime;
			public LARGE_INTEGER LastAccessTime;
			public LARGE_INTEGER LastWriteTime;
			public LARGE_INTEGER ChangeTime;
			public LARGE_INTEGER EndOfFile;
			public LARGE_INTEGER AllocationSize;
			public UInt32 FileAttributes;
			public UInt32 FileNameLength;
			public UInt32 EaSize;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 400)]
			public string FileName;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct FILE_NAME_INFO
		{
			public int FileNameLength;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1000)]
			public string FileName;
		}

		public struct IO_STATUS_BLOCK
		{
			UInt64 Status;
			UInt64 Information;
		}

		public struct SYSTEM_HANDLE_INFORMATION
		{
			public ulong Count;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2000)]
			public SYSTEM_HANDLE[] SystemHandles;
		}


		public struct SYSTEM_HANDLE
		{
			public uint dwProcessId;
			public byte bObjectType;
			public byte bFlags;
			public ushort wValue;
			public uint pAddress;
			public uint GrantedAccess;
		}
		

	}
}
