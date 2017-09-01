// Reader.h : CReader µÄÉùÃ÷

#pragma once
#include "SysHandleReader_i.h"
#include "resource.h"       // Ö÷·ûºÅ
#include <comsvcs.h>

using namespace ATL;



// CReader

class ATL_NO_VTABLE CReader :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CReader, &CLSID_Reader>,
	public IDispatchImpl<IReader, &IID_IReader, &LIBID_SysHandleReaderLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
	CReader()
	{
	}

	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_READER)

DECLARE_NOT_AGGREGATABLE(CReader)

BEGIN_COM_MAP(CReader)
	COM_INTERFACE_ENTRY(IReader)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()




// IReader
public:
	STDMETHOD(QueryProcessHandleInfo)(ULONG processid, LONG* ProcessHandleInfo);
};

OBJECT_ENTRY_AUTO(__uuidof(Reader), CReader)
