using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Nektra.Deviare2;

namespace WriteFileHooker
{
	public class WriteFileHooker
	{
		private NktSpyMgr _spyMgr;
		private NktProcess _process;

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

			ReadBuffer(lpBuffer.Address, nNumberOfBytesToWrite.Address);
		}


		private void Output(string strOutput)
		{
			
		}

		/// <summary>
		/// 老子直接读内存
		/// </summary>
		/// <param name="p_buffer"></param>
		/// <param name="p_size"></param>
		private void ReadBuffer(IntPtr p_buffer, IntPtr p_size)
		{
			IntPtr processHandle = WinApi.OpenProcess(WinApi.PROCESS_WM_READ, false, _process.Id);

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
