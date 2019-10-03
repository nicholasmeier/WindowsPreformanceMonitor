#pragma once
#include <string>
#include <vector>
#include <stdio.h>
#include <sstream>
#include <tchar.h>
#include <iostream>
#include <malloc.h>
#include <windows.h>
#include <tlhelp32.h>

typedef BOOL(WINAPI* LPFN_GLPI)(PSYSTEM_LOGICAL_PROCESSOR_INFORMATION, PDWORD);

/*
* This is a helper function to count set bits in the processor mask.
*/
inline DWORD CountSetBits(ULONG_PTR bitMask) {
	DWORD LSHIFT = sizeof(ULONG_PTR) * 8 - 1;
	DWORD bitSetCount = 0;
	ULONG_PTR bitTest = (ULONG_PTR)1 << LSHIFT;
	DWORD i;

	for (i = 0; i <= LSHIFT; ++i) {
		bitSetCount += ((bitMask & bitTest) ? 1 : 0);
		bitTest /= 2;
	}

	return bitSetCount;
}

namespace PerformanceMonitor
{
	namespace Cpp
	{
		// This is our native implementation
		// It's marked with __declspec(dllexport) 
		// to be visible from outside the DLL boundaries
		class __declspec(dllexport) Logic
		{
		public:
			std::vector<PROCESSENTRY32> getProcessList();
			std::string getCPUClock();
			int getCPULogicalProcessorCores();
			int getCPUPhysicalCores();
			DWORD getppid(int pid);
		private:
		};
	}
}