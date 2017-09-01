using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Nektra.Deviare2;
using static WriteFileHooker.WinApi;
using static WriteFileHooker.WinType;
using ATLSysHandleReaderLib;
using WinApiReader;

namespace WriteFileHooker
{
	public class WriteFileHooker
	{
		private NktSpyMgr _spyMgr;
		private NktProcess _process;

		private IntPtr processHandle;

		public WriteFileHooker(string proccessName)
		{

			_spyMgr = new NktSpyMgr();
			_spyMgr.Initialize();
			_spyMgr.OnFunctionCalled += new DNktSpyMgrEvents_OnFunctionCalledEventHandler(OnWriteFileCalled);

			GetProcess(proccessName);
			if (_process == null)
			{
				//TODO: 没有监听进程时怎么办
				//Environment.Exit(0);
				throw new Exception("没找到进程" + proccessName);
			}

			NktHook hook = _spyMgr.CreateHook("Kernel32.dll!WriteFile", (int)(eNktHookFlags.flgOnlyPostCall & eNktHookFlags.flgRestrictAutoHookToSameExecutable));
			hook.Hook(true);
			hook.Attach(_process, true);

			processHandle = WinApi.OpenProcess(WinEnum.PROCESS_WM_READ | WinEnum.PROCESS_DUP_HANDLE, false, _process.Id);
		}

		private bool GetProcess(string proccessName)
		{
			NktProcessesEnum enumProcess = _spyMgr.Processes();
			NktProcess tempProcess = enumProcess.First();
			while (tempProcess != null)
			{
				//Console.Out.WriteLine(tempProcess.Name);

				if (tempProcess.Name.Contains(proccessName) && tempProcess.PlatformBits > 0 && tempProcess.PlatformBits <= IntPtr.Size * 8)
				{
					_process = tempProcess;
					return true;
				}
				tempProcess = enumProcess.Next();
			}

			_process = null;
			return false;
		}

		private void OnWriteFileCalled(NktHook hook, NktProcess process, NktHookCallInfo hookCallInfo)
		{
			string strDocument = "Document: ";

			INktParamsEnum paramsEnum = hookCallInfo.Params();

			INktParam hFile = paramsEnum.First();

			//paramsEnum.Next();
			//paramsEnum.Next();
			//paramsEnum.Next();
			//paramsEnum.Next();

			INktParam lpBuffer = paramsEnum.Next();

			INktParam nNumberOfBytesToWrite = paramsEnum.Next();

			#region 毛用没有
			if (hFile.PointerVal != IntPtr.Zero)
			{
				INktParamsEnum hFileEnumStruct = hFile.Evaluate().Fields();
				INktParam hFileStruct = hFileEnumStruct.First();
			}

			Console.Out.WriteLine(lpBuffer.ReadString());

			Console.Out.WriteLine(lpBuffer.Address);

			if (lpBuffer.PointerVal != IntPtr.Zero)
			{
				strDocument += lpBuffer.ReadString();
				strDocument += "\n";
			}

			Output(strDocument);
			#endregion

			var h_file = QueryFileHandle(hFile.Address);

			//ReadFileInfo(hFile.Address);

			ReadBuffer(lpBuffer.Address, nNumberOfBytesToWrite.Address);
		}


		private void Output(string strOutput)
		{
			
		}

		/// <summary>
		/// 获得的hfile的值内容是进程中文件handle的下标
		/// 绕一圈取真正的handle
		/// </summary>
		/// <param name="p_hfile"></param>
		/// <returns></returns>
		private unsafe int QueryFileHandle(IntPtr p_hfile)
		{
			try
			{
				byte[] _hfile = new byte[4];

				int readedbtyes = 0;

				bool result = false;

				result = WinApi.ReadProcessMemory(processHandle.ToInt32(), p_hfile.ToInt32(), _hfile, 4, ref readedbtyes);

				int idx_file = WinApi.ToInt32(_hfile);

				#region dllimport
				//SYSTEM_HANDLE_INFORMATION handleInfos = new SYSTEM_HANDLE_INFORMATION();
				//uint size = Convert.ToUInt32(Marshal.SizeOf(handleInfos));
				//uint len = 0;
				//int result = WinApi.ZwQuerySystemInformation(WinEnum.SYSTEMHANDLEINFORMATION, ref handleInfos, size, ref len);
				//List<SYSTEM_HANDLE> handles = handleInfos.SystemHandles.Where(e => e.dwProcessId == _process.Id).ToList();
				#endregion

				#region com
				//SYSTEM_HANDLE_INFORMATION handleInfos2 = new SYSTEM_HANDLE_INFORMATION();
				//var reader = new InfoReader();
				//uint pHandleInfos2 = 0;
				//reader.QueryProcessHandleInfo((uint)_process.Id, ref pHandleInfos2);
				//IntPtr p2 = new IntPtr(pHandleInfos2);

				//Marshal.PtrToStructure(p2, handleInfos2);
				#endregion

				#region cli
				var reader = new WinApiReader.WinApiReader();
				var handleInfo = new SystemHandleInfo();

				var ret = reader.QueryProcessHandleInfo(_process.Id, out handleInfo);
				#endregion

				//foreach (var handle_type in handleInfo.SystemHandles.Select(e => e.ObjectType).Distinct())
				//{
				//	var handle = handleInfo.SystemHandles.Where(e => e.ObjectType == handle_type).First();
				//	ReadFileInfo(new IntPtr(handle.Value));
				//}

				var hfile = handleInfo.SystemHandles.Where(e => e.ObjectType == 28).ToList()[idx_file];

				ReadFileInfo(new IntPtr(hfile.Value));

			}
			catch (Exception ex)
			{
				Console.Out.WriteLine(ex);
			}

			return 0;
		}

		/// <summary>
		/// 把远程file handle复制到本地进程
		/// 然后获取文件名
		/// </summary>
		/// <param name="p_hfile"></param>
		private unsafe void ReadFileInfo(IntPtr p_hfile)
		{

			IntPtr my_hfile = new IntPtr(-1);

			int hCurProcess = -1;//WinApi.GetCurrentProcess();

			bool result;

			result = WinApi.DuplicateHandle(processHandle.ToInt32(), p_hfile.ToInt32(), hCurProcess, ref my_hfile, 0x80000000, false, 2);

			#region GetFileInformationByHandle 文件最基本信息
			var fileinfo = new BY_HANDLE_FILE_INFORMATION();

			result = WinApi.GetFileInformationByHandle(my_hfile.ToInt32(), ref fileinfo);

			//Console.Out.WriteLine(fileinfo.dwFileAttributes);
			#endregion

			#region GetFileInformationByHandleEx
			var fileinfo2 = new FILE_FULL_DIR_INFO();

			int fileInfo2Type = 0xe;

			//本来想用开辟非托管内存的操作，传指针到winapi，后来发现那块内存有数据后用
			//PtrToStructure转结构体报错，也许是操作问题，还不如直接传结构体
			int size2 = Marshal.SizeOf(fileinfo2);
			IntPtr p_fileInfo2 = Marshal.AllocHGlobal(size2);
			Marshal.StructureToPtr(fileinfo2, p_fileInfo2, false);

			//调用失败 大概是LARGE_INTEGER 这个union的结构不对
			result = WinApi.GetFileInformationByHandleEx(my_hfile.ToInt32(), fileInfo2Type, ref fileinfo2, size2);

			var fileinfo3 = new FILE_NAME_INFO();
			//fileinfo3.FileName = "";

			int fileInfo3Type = 0x2;
			int size3 = Marshal.SizeOf(fileinfo3) + 1000*2;
			IntPtr p_fileInfo3 = Marshal.AllocHGlobal(size3);
			Marshal.StructureToPtr(fileinfo3, p_fileInfo3, false);

			//\otherproject\WriteFileHook\Log4netLoger\bin\x86\Debug\Logs\debug.log20170901
			//result = WinApi.GetFileInformationByHandleEx(my_hfile.ToInt32(), fileInfo3Type, p_fileInfo3, size3);
			result = WinApi.GetFileInformationByHandleEx(my_hfile.ToInt32(), fileInfo3Type, ref fileinfo3, size3);


			StringBuilder fn3 = new StringBuilder(1000);

			int len3 = WinApi.GetFullPathName(fileinfo3.FileName, 1000, fn3, null);

			//输出正常路径
			//E:\otherproject\WriteFileHook\Log4netLoger\bin\x86\Debug\Logs\debug.log20170901
			Console.Out.WriteLine(fn3);
			#endregion

			#region GetFinalPathNameByHandle
			//char[] fileName = new char[1000];
			StringBuilder fileName = new StringBuilder(1000);

			//获取的带设备路径
			//\Device\HarddiskVolume4\otherproject\WriteFileHook\Log4netLoger\bin\x86\Debug\Logs\debug.log20170901
			int size4 = WinApi.GetFinalPathNameByHandle(my_hfile.ToInt32(), fileName, 1000, 0x2);

			StringBuilder fn4 = new StringBuilder(1000);

			//会在前面在加个盘符，然而没去掉设备路径，超谐
			//有专门根据设备路径获取盘符路径的方法
			//E:\Device\HarddiskVolume4\otherproject\WriteFileHook\Log4netLoger\bin\x86\Debug\Logs\debug.log20170901
			int len4 = WinApi.GetFullPathName(fileName.ToString(), 1000, fn4, null);

			Console.Out.WriteLine(fn4);
			#endregion


			#region ZwQueryInformationFile
			//GetFileInformationByHandleEx 内部调用的就是这个吧（

			var fileStatus5 = new IO_STATUS_BLOCK();

			int ssize5 = Marshal.SizeOf(fileStatus5);
			IntPtr p_fileStatus5 = Marshal.AllocHGlobal(ssize5);
			Marshal.StructureToPtr(fileStatus5, p_fileStatus5, false);

			var fileinfo5 = new FILE_NAME_INFO();
			//fileinfo5.FileName = "";

			int fileInfo5Type = 9;
			int size5 = Marshal.SizeOf(fileinfo5) + 1000*2;
			IntPtr p_fileInfo5 = Marshal.AllocHGlobal(size5);
			Marshal.StructureToPtr(fileinfo5, p_fileInfo5, false);
			
			int ret = WinApi.ZwQueryInformationFile(my_hfile.ToInt32(), ref fileStatus5, ref fileinfo5, size5, fileInfo5Type);

			//\otherproject\WriteFileHook\Log4netLoger\bin\x86\Debug\Logs\debug.log20170901
			Console.Out.WriteLine(fileinfo5.FileName);
			#endregion

		}

		/// <summary>
		/// 老子直接读内存
		/// </summary>
		/// <param name="p_buffer"></param>
		/// <param name="p_size"></param>
		private void ReadBuffer(IntPtr p_buffer, IntPtr p_size)
		{
			byte[] _size = new byte[4];

			int readedbtyes = 0;

			var result = WinApi.ReadProcessMemory(processHandle.ToInt32(), p_size.ToInt32(), _size, 4, ref readedbtyes);

			int size = WinApi.ToInt32(_size);

			byte[] _bufferpoint = new byte[4];

			result = WinApi.ReadProcessMemory(processHandle.ToInt32(), p_buffer.ToInt32(), _bufferpoint, 4, ref readedbtyes);

			int bufferpoint = WinApi.ToInt32(_bufferpoint);

			byte[] _buffer = new byte[size];

			result = WinApi.ReadProcessMemory(processHandle.ToInt32(), bufferpoint, _buffer, size, ref readedbtyes);

			Console.Out.WriteLine(Encoding.Default.GetString(_buffer));
			Console.Out.WriteLine(Encoding.UTF8.GetString(_buffer));

		}


	}
}
