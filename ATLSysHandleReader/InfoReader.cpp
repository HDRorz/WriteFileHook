// InfoReader.cpp : CInfoReader 的实现

#include "stdafx.h"
#include "InfoReader.h"


// CInfoReader


#define STATUS_INFO_LENGTH_MISMATCH 0xC0000004L

#include "WinType.h"

typedef int(*ZWQUERYSYSTEMINFORMATION)(int SystemInformationClass, PVOID SystemInformation, ULONG SystemInformationLength, PULONG ReturnLength);

STDMETHODIMP CInfoReader::QueryProcessHandleInfo(ULONG ProcessId, ULONG* ProcessHandleInfo)
{
	// TODO: 在此添加实现代码

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
	int outArrLen = allCount / 50;

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

	PVOID pOutHandleInfo;
	int outTrueArr = sizeof(SYSTEM_HANDLE) * outCount;
	int outHandleInfoSize = sizeof(SYSTEM_HANDLE_INFORMATION) + outTrueArr;
	pOutHandleInfo = HeapAlloc(hHeap, 0, outHandleInfoSize);
	memset(pOutHandleInfo, 0, outHandleInfoSize);
	SYSTEM_HANDLE_INFORMATION* outHandleInfo = (SYSTEM_HANDLE_INFORMATION*)pOutHandleInfo;
	outHandleInfo->Count = outCount;
	memcpy(outHandleInfo->SystemHandles, outArr, outTrueArr);

	ProcessHandleInfo = (ULONG*)pOutHandleInfo;

	return S_OK;
}
