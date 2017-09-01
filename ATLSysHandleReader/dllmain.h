// dllmain.h: 模块类的声明。

class CATLSysHandleReaderModule : public ATL::CAtlDllModuleT< CATLSysHandleReaderModule >
{
public :
	DECLARE_LIBID(LIBID_ATLSysHandleReaderLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ATLSYSHANDLEREADER, "{38E345BF-65AC-44A1-9C5A-0ED8DFAD9D55}")
};

extern class CATLSysHandleReaderModule _AtlModule;
