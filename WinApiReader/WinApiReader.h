// WinApiReader.h

#pragma once

using namespace System;

namespace WinApiReader {



	public ref class SystemHandle
	{
	public:

		unsigned int ProcessId;
		unsigned char ObjectType;
		unsigned char Flags;
		unsigned short Value;
		unsigned int Address;
		unsigned int GrantedAccess;
	};


	public ref class SystemHandleInfo
	{
	public:

		int Count;
		array<SystemHandle^> ^SystemHandles;
	};

	public ref class WinApiReader
	{
	public:

		int QueryProcessHandleInfo(int ProcessId, [System::Runtime::InteropServices::OutAttribute] SystemHandleInfo^% HandleInfo);
	};
}
