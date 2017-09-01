// dllmain.cpp: DllMain 的实现。

#include "stdafx.h"
#include "resource.h"
#include "SysHandleReader_i.h"
#include "dllmain.h"
#include "compreg.h"
#include "xdlldata.h"

CSysHandleReaderModule _AtlModule;

class CSysHandleReaderApp : public CWinApp
{
public:

// 重写
	virtual BOOL InitInstance();
	virtual int ExitInstance();

	DECLARE_MESSAGE_MAP()
};

BEGIN_MESSAGE_MAP(CSysHandleReaderApp, CWinApp)
END_MESSAGE_MAP()

CSysHandleReaderApp theApp;

BOOL CSysHandleReaderApp::InitInstance()
{
#ifdef _MERGE_PROXYSTUB
	if (!PrxDllMain(m_hInstance, DLL_PROCESS_ATTACH, NULL))
		return FALSE;
#endif
	return CWinApp::InitInstance();
}

int CSysHandleReaderApp::ExitInstance()
{
	return CWinApp::ExitInstance();
}
