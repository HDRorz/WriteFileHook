// WinApiReader.h

#pragma once

using namespace System;

namespace WinApiReader {



	public ref class SystemHandle
	{
	public:

		//这里用了C++类型，在C++环境下使用会比较简单，到托管环境下会自动转换成托管类型
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
		//没想到吧，数组是这么声明的，后面那个^是指针的意思吧
		array<SystemHandle^> ^SystemHandles;
	};

	public ref class WinApiReader
	{
	public:

		//没想到吧，还有[System::Runtime::InteropServices::OutAttribute]这种操作，这个可以实现C#中out关键字
		int QueryProcessHandleInfo(int ProcessId, [System::Runtime::InteropServices::OutAttribute] SystemHandleInfo^% HandleInfo);
	};
}
