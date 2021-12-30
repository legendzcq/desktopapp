// FixToolConsole.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

typedef BOOL(WINAPI *LPFN_ISWOW64PROCESS) (HANDLE, PBOOL);
LPFN_ISWOW64PROCESS fnIsWow64Process;

BOOL IsWow64()
{
	BOOL bIsWow64 = FALSE;

	//IsWow64Process is not available on all supported versions of Windows.  
	//Use GetModuleHandle to get a handle to the DLL that contains the function  
	//and GetProcAddress to get a pointer to the function if available.  

	fnIsWow64Process = (LPFN_ISWOW64PROCESS)GetProcAddress(
		GetModuleHandle(TEXT("kernel32")), "IsWow64Process");

	if (NULL != fnIsWow64Process)
	{
		if (!fnIsWow64Process(GetCurrentProcess(), &bIsWow64))
		{
			//handle error  
		}
	}
	return bIsWow64;
}

int _tmain(int argc, _TCHAR* argv[])
{
	if (IsWow64())
		_tprintf(TEXT("The process is running under WOW64.\n"));
	else
		_tprintf(TEXT("The process is not running under WOW64.\n"));

	printf(sizeof(int *) == 4 ? "32 bit" : "16 bit");

	return 0;
}

