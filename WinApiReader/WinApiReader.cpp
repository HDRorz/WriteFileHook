// 这是主 DLL 文件。

#include "stdafx.h"

#include <Windows.h>

#include "WinApiReader.h"

//int QueryProcessHandleInfo(int process)


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

typedef struct _SYSTEM_HANDLE_INFORMATION
{
	ULONG Count;
	SYSTEM_HANDLE SystemHandles[];
}
SYSTEM_HANDLE_INFORMATION;


#define STATUS_INFO_LENGTH_MISMATCH 0xC0000004L

typedef unsigned long(*ZWQUERYSYSTEMINFORMATION)(int SystemInformationClass, PVOID SystemInformation, ULONG SystemInformationLength, PULONG ReturnLength);


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
	HANDLE hHeap = GetProcessHeap();
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
	
	HeapFree(hHeap, 0, mPtr);
	HeapFree(hHeap, 0, pOutArr);

	return 1;
}