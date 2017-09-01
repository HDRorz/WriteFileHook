// InfoReader.h : CInfoReader µÄÉùÃ÷

#pragma once
#include "ATLSysHandleReader_i.h"
#include "resource.h"       // Ö÷·ûºÅ
#include <comsvcs.h>

#include "WinType.h"

using namespace ATL;



// CInfoReader

class ATL_NO_VTABLE CInfoReader :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CInfoReader, &CLSID_InfoReader>,
	public IDispatchImpl<IInfoReader, &IID_IInfoReader, &LIBID_ATLSysHandleReaderLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
	CInfoReader()
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

DECLARE_REGISTRY_RESOURCEID(IDR_INFOREADER)

DECLARE_NOT_AGGREGATABLE(CInfoReader)

BEGIN_COM_MAP(CInfoReader)
	COM_INTERFACE_ENTRY(IInfoReader)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()




// IInfoReader
public:
	STDMETHOD(QueryProcessHandleInfo)(ULONG ProcessId, ULONG* ProcessHandleInfo);
};

OBJECT_ENTRY_AUTO(__uuidof(InfoReader), CInfoReader)
