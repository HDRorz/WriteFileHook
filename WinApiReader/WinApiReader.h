// WinApiReader.h

#pragma once

using namespace System;

namespace WinApiReader {



	public ref class SystemHandle
	{
	public:

		//��������C++���ͣ���C++������ʹ�û�Ƚϼ򵥣����йܻ����»��Զ�ת�����й�����
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
		//û�뵽�ɣ���������ô�����ģ������Ǹ�^��ָ�����˼��
		array<SystemHandle^> ^SystemHandles;
	};

	public ref class WinApiReader
	{
	public:

		//û�뵽�ɣ�����[System::Runtime::InteropServices::OutAttribute]���ֲ������������ʵ��C#��out�ؼ���
		int QueryProcessHandleInfo(int ProcessId, [System::Runtime::InteropServices::OutAttribute] SystemHandleInfo^% HandleInfo);
	};
}
