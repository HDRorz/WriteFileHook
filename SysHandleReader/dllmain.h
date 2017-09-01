// dllmain.h: 模块类的声明。

class CSysHandleReaderModule : public ATL::CAtlDllModuleT< CSysHandleReaderModule >
{
public :
	DECLARE_LIBID(LIBID_SysHandleReaderLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_SYSHANDLEREADER, "{5AD7BA6C-ED5D-4E99-984F-5687BF29CC00}")
};

extern class CSysHandleReaderModule _AtlModule;
