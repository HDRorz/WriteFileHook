#pragma once



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
