// 这是主 DLL 文件。

#include "stdafx.h"

#include <Windows.h>

#include "WinApiReader.h"

//int QueryProcessHandleInfo(int process)

//WinApi用的结构体，句柄信息
typedef struct _SYSTEM_HANDLE
{
	DWORD    dwProcessId;
	BYTE     bObjectType;
	BYTE     bFlags;
	WORD     wValue;
	PVOID    pAddress;
	DWORD    GrantedAccess;
}
SYSTEM_HANDLE;

//WinApi用的结构体，句柄信息数组
typedef struct _SYSTEM_HANDLE_INFORMATION
{
	ULONG Count;
	SYSTEM_HANDLE SystemHandles[];
}
SYSTEM_HANDLE_INFORMATION;

//NTSTAUS 长度不匹配，这里引用ntstaus.h会编译过不了（太迷了）
#define STATUS_INFO_LENGTH_MISMATCH 0xC0000004L

//ZwQuerySystemInformation函数指针
typedef unsigned long(*ZWQUERYSYSTEMINFORMATION)(int SystemInformationClass, PVOID SystemInformation, ULONG SystemInformationLength, PULONG ReturnLength);

//获取目标进程的所有句柄
int WinApiReader::WinApiReader::QueryProcessHandleInfo(int ProcessId, [System::Runtime::InteropServices::OutAttribute] SystemHandleInfo^% HandleInfo)
{
	HMODULE hNtDLL = LoadLibrary(L"NTDLL.DLL");
	if (!hNtDLL)
	{
		return FALSE;
	}

	ZWQUERYSYSTEMINFORMATION ZwQuerySystemInformation = (ZWQUERYSYSTEMINFORMATION)GetProcAddress(hNtDLL, "ZwQuerySystemInformation");
	if (ZwQuerySystemInformation == NULL)
	{
		return FALSE;
	}

	ULONG mSize = 0x8000;
	PVOID mPtr;
	ULONG status;
	//进程的堆内存
	HANDLE hHeap = GetProcessHeap();

	//这里用了一个循环来开辟一个堆内存空间用来存放函数调用结果（因为太♂大了）
	do
	{
		mPtr = HeapAlloc(hHeap, 0, mSize);
		if (!mPtr) return NULL;
		memset(mPtr, 0, mSize);

		status = ZwQuerySystemInformation(16, mPtr, mSize, NULL);

		if (status == STATUS_INFO_LENGTH_MISMATCH)
		{
			HeapFree(hHeap, 0, mPtr);
			mSize = mSize * 2;
		}
	} while (status == STATUS_INFO_LENGTH_MISMATCH);

	int allCount = ((SYSTEM_HANDLE_INFORMATION*)mPtr)->Count;
	int outArrLen = allCount / 10;

	PVOID pOutArr;
	int outArrSize = sizeof(SYSTEM_HANDLE) * outArrLen;
	pOutArr = HeapAlloc(hHeap, 0, outArrSize);
	memset(pOutArr, 0, outArrSize);
	SYSTEM_HANDLE* outArr = (SYSTEM_HANDLE*)pOutArr;

	//筛选出目标进程的所有句柄
	int outCount = 0;
	SYSTEM_HANDLE item;
	for (int i = 0; i < allCount; i++)
	{
		item = ((SYSTEM_HANDLE_INFORMATION*)mPtr)->SystemHandles[i];
		if (item.dwProcessId == ProcessId)
		{
			outArr[outCount].bFlags = item.bFlags;
			outArr[outCount].bObjectType = item.bObjectType;
			outArr[outCount].dwProcessId = item.dwProcessId;
			outArr[outCount].GrantedAccess = item.GrantedAccess;
			outArr[outCount].pAddress = item.pAddress;
			outArr[outCount].wValue = item.wValue;

			outCount++;
		}
	}

	/*PVOID pOutHandleInfo;
	int outTrueArr = sizeof(SYSTEM_HANDLE) * (outCount - 1);
	int outHandleInfoSize = sizeof(SYSTEM_HANDLE_INFORMATION) + outTrueArr;
	pOutHandleInfo = HeapAlloc(hHeap, 0, outHandleInfoSize);
	memset(pOutHandleInfo, 0, outHandleInfoSize);
	SYSTEM_HANDLE_INFORMATION* outHandleInfo = (SYSTEM_HANDLE_INFORMATION*)pOutHandleInfo;
	outHandleInfo->Count = outCount - 1;
	memcpy(outHandleInfo->SystemHandles, outArr, outTrueArr);

	ProcessHandleInfo = (ULONG*)pOutHandleInfo;*/

	//手工装箱
	HandleInfo = gcnew SystemHandleInfo();

	HandleInfo->Count = outCount;
	HandleInfo->SystemHandles = gcnew array<SystemHandle^>(outCount);

	for (int i = 0; i < outCount; i++)
	{
		HandleInfo->SystemHandles[i] = gcnew SystemHandle();
		HandleInfo->SystemHandles[i]->ProcessId = outArr[i].dwProcessId;
		HandleInfo->SystemHandles[i]->Address = (unsigned int)outArr[i].pAddress;
		HandleInfo->SystemHandles[i]->Flags = outArr[i].bFlags;
		HandleInfo->SystemHandles[i]->ObjectType = outArr[i].bObjectType;
		HandleInfo->SystemHandles[i]->GrantedAccess = outArr[i].GrantedAccess;
		HandleInfo->SystemHandles[i]->Value = outArr[i].wValue;
	}
	
	//释放堆内存
	HeapFree(hHeap, 0, mPtr);
	HeapFree(hHeap, 0, pOutArr);

	return 1;
}