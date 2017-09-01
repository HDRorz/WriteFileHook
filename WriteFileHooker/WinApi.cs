using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using static WriteFileHooker.WinType;

namespace WriteFileHooker
{
	public class WinApi
	{

		

		public static int ToInt32(byte[] buf)
		{
			return (int)((buf[0] & 0xFF) | ((buf[1] & 0xFF) << 8) | ((buf[2] & 0xFF) << 16) | ((buf[3] & 0xFF) << 24));
		}

		[DllImport("kernel32.dll")]
		public static extern int GetLastError();

		[DllImport("kernel32.dll")]
		public static extern int GetCurrentProcess();

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

		[DllImport("kernel32", SetLastError = true)]
		public static extern unsafe bool DuplicateHandle(int hSourceProcessHandle, int hSourceHandle, int hTargetProcessHandle, ref IntPtr lpTargetHandle, uint dwDesiredAccess, bool bInheritHandle, uint dwOptions);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern unsafe bool GetFileInformationByHandle(int hFile, ref BY_HANDLE_FILE_INFORMATION lpFileInformation);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern unsafe bool GetFileInformationByHandleEx(int hFile, int FileInformationClass, IntPtr lpFileInformation, int dwBufferSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern unsafe bool GetFileInformationByHandleEx(int hFile, int FileInformationClass, ref FILE_FULL_DIR_INFO lpFileInformation, int dwBufferSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern unsafe bool GetFileInformationByHandleEx(int hFile, int FileInformationClass, ref FILE_NAME_INFO lpFileInformation, int dwBufferSize);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern unsafe int GetFinalPathNameByHandle(int hFile, [Out, MarshalAs(UnmanagedType.LPTStr)]StringBuilder lpszFilePath, int cchFilePath, int dwFlags);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern unsafe int GetFullPathName(string lpFileName, int nBufferLength, [Out, MarshalAs(UnmanagedType.LPTStr)]StringBuilder lpBuffer, [Out, MarshalAs(UnmanagedType.LPTStr)]StringBuilder lpFilePart);

		[DllImport("ntdll.dll", SetLastError = true)]
		public static extern int ZwQueryInformationFile(int hfile, ref int ioStatusBlock, ref int fileInformation, int length, int FileInformationClass);

		[DllImport("ntdll.dll", SetLastError = true)]
		public static extern int ZwQueryInformationFile(int hfile, ref IO_STATUS_BLOCK ioStatusBlock, ref FILE_NAME_INFO fileInformation, int length, int FileInformationClass);

		[DllImport("ntdll.dll", SetLastError = true)]
		public static extern unsafe int ZwQuerySystemInformation(int SystemInformationClass,  ref SYSTEM_HANDLE_INFORMATION SystemInformation, uint SystemInformationLength, ref uint ReturnLength);


	}
}
