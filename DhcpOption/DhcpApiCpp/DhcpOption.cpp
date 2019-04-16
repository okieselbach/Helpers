// DhcpApiCpp.cpp : Defines the entry point for the console application.
//

// Important Information for WinPE Development!
// 
// http://social.msdn.microsoft.com/Forums/en-US/vcgeneral/thread/a4befcd6-b40b-49d2-a188-47e9ce55a775
// Even I had same issue when I try to run VC++ 2008 console based application in Win PE 3.0.
//
// Installation of vcredist_x86 was not allowing in PE. So I tried linking CRT statically, It have solved issue.
//
// You can use the /MT switch to statically link to CRT, by default it's using 
// /MD (or /MDd for Debug). To do this in Visual Studio 2008: 
//
// 1) Open the Project Properties dialog 
// 2) in "Configuration Properties -> C/C++ -> Code Generation", in the field 
// "Runtime Library", choose "Multi-threaded (/MT)"

// Windows PE Developer’s Guide
// http://www.microsoft.com/en-us/download/details.aspx?id=21452

#include "stdafx.h"
#include <windows.h>

#include <dhcpcsdk.h>
#include <iphlpapi.h>

#pragma comment( lib, "dhcpcsvc.lib" )
#pragma comment( lib, "iphlpapi.lib" )

LPWSTR doMultiByteToWideChar(char *pszStringToConvert)
{
	LPWSTR pszConverted = NULL;
	DWORD ccb;

	if (pszStringToConvert)
	{
		ccb = MultiByteToWideChar(CP_UTF8, 0, (LPCSTR)pszStringToConvert, -1, NULL, 0);
		if (ccb)
		{
			pszConverted = (LPWSTR)HeapAlloc(GetProcessHeap(), 0, ccb * sizeof(*pszConverted));
			if (pszConverted)
			{
				ccb = MultiByteToWideChar(CP_UTF8, 0, (LPCSTR)pszStringToConvert, -1, pszConverted, ccb);
				if (ccb)
				{
					return pszConverted;
				}
			}
		}
	}

	return pszConverted;
}

int DHCPOptionIdQuery(int optionId, bool log)
{
	if (log)
		printf_s("Start DHCPOptionIdQuery for OptionId: %d\n", optionId);

	int result = 1, counter = 0;
	PIP_ADAPTER_INFO pAdapterInfo, pAdapter;
	LPWSTR pszAdapterName;
	ULONG ulOutBufLen = sizeof(IP_ADAPTER_INFO);
	DWORD dwDHCPVersion;
	DWORD dwError, dwSize;
	char szOption[1000];
	DHCPCAPI_PARAMS dpParams;               
	DHCPCAPI_PARAMS_ARRAY dpReqArray;
	DHCPCAPI_PARAMS_ARRAY dpSendArray;

	if (DhcpCApiInitialize(&dwDHCPVersion) != ERROR_SUCCESS)
	{
		if (log)
			printf_s("-DhcpCApiInitialize failed.\n");

		return 1;
	}

	pAdapterInfo = (PIP_ADAPTER_INFO)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, ulOutBufLen);
	if (pAdapterInfo == NULL)
	{
		return 1;
	}

	dwError = GetAdaptersInfo(pAdapterInfo, &ulOutBufLen);
	if (dwError == ERROR_BUFFER_OVERFLOW)
	{
		HeapFree(GetProcessHeap(), 0, pAdapterInfo);

		pAdapterInfo = (PIP_ADAPTER_INFO)HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, ulOutBufLen);
		if (pAdapterInfo == NULL)
		{
			return 1;
		}

		dwError = GetAdaptersInfo(pAdapterInfo, &ulOutBufLen);
	}

	if (dwError == NO_ERROR) 
	{
		pAdapter = pAdapterInfo;

		while (pAdapter)
		{
			if (log)
				printf_s("-pAdapter %d:\n", counter);

			counter++;
			
			// use DHCP enabled Ethernet network adapters only to request the DHCP Option ID!
			//if ((pAdapter->Type == MIB_IF_TYPE_ETHERNET) && pAdapter->DhcpEnabled)

			// use DHCP enabled network adapters only to request the DHCP Option ID!
			if (pAdapter->DhcpEnabled)
			{
				pszAdapterName = doMultiByteToWideChar(pAdapter->AdapterName);
				
				if (log)
				{
					wprintf_s(L"--pszAdapterDescription: %ls\n", doMultiByteToWideChar(pAdapter->Description));
					wprintf_s(L"--pszAdapterDhcpServer: %ls\n", doMultiByteToWideChar(pAdapter->DhcpServer.IpAddress.String));
					wprintf_s(L"--pszAdapterName: %ls\n", pszAdapterName);
				}

				dpParams.Flags = 0;
				dpParams.OptionId = optionId; // That's what we're looking for - DHCP Option ID x
				dpParams.IsVendor = FALSE;
				dpParams.Data = NULL;
				dpParams.nBytesData = 0;

				dpReqArray.nParams = 1;
				dpReqArray.Params = &dpParams;
								
				dpSendArray.nParams = 0;
				dpSendArray.Params = NULL;

				dwSize = sizeof(szOption);

				// Send the DHCP request
				dwError = DhcpRequestParams(DHCPCAPI_REQUEST_SYNCHRONOUS,
											NULL,
											pszAdapterName,
											NULL,
											dpSendArray,
											dpReqArray,
											(PBYTE)szOption,
											&dwSize,
											NULL);

				if (log)
					printf_s("--DhcpRequestParams executed!\n");
				
				if (ERROR_SUCCESS == dwError && dpParams.nBytesData > 0)
				{
					if (log)
						printf_s("---Data received\n");

					// print the successfully received result, there is no special handling we expect a string result
					for (int i=0; i < dpParams.nBytesData; i++)
					{
						printf_s("%c", dpParams.Data[i]);
					}
					printf_s("\n");

					result = 0;
				}
				else
				{
					if (log)
						printf_s("---No Data received\n");

					if (ERROR_SUCCESS != dwError && log)
						printf_s("---Unable to retrieve DHCP option : error %08X.\n", dwError);
				}

				HeapFree(GetProcessHeap(), 0, pszAdapterName);
			}

			pAdapter = pAdapter->Next;
		}
	}

	if (pAdapterInfo)
	{
		HeapFree(GetProcessHeap(), 0, pAdapterInfo);
	}

	DhcpCApiCleanup();

	return result;
}

int _tmain(int argc, _TCHAR* argv[])
{
	int optionId, result;
	bool log = false;

	if (argc == 2)
	{
		swscanf_s(argv[1], L"%ld", &optionId);
	}
	else if (argc > 2)
	{
		swscanf_s(argv[1], L"%ld", &optionId);
		log = true;
	}
	else
	{
		printf_s("Query DHCP Options\n------------------\nOnly string values are supported!\n\n");
		printf_s("USAGE: DhcpOption.exe OptionsID [DEBUG]\n\n");
		return 0;
	}

	if (log)
		printf_s("Query DHCP Options (only string values are supported!)\n");
	
	if (log)
		printf_s("#Start#\n");
	
	// Send the DHCP request for Option ID
	result = DHCPOptionIdQuery(optionId, log);
	
	if (log)
		printf_s("#Stop#\n");

	return result;
}

