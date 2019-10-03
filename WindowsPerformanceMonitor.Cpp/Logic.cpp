#include "Logic.h"

std::string PerformanceMonitor::Cpp::Logic::getCPUClock() {
	return "3400";
}

/*
* This function is used to get the current process list from the OS.
*/
std::vector<PROCESSENTRY32> PerformanceMonitor::Cpp::Logic::getProcessList()
{
	HANDLE hProcessSnapShot;
	PROCESSENTRY32 ProcessEntry = { 0 };
	BOOL Return = FALSE;
	std::vector<PROCESSENTRY32> processlist;

	// Get snapshot with process listing.
	hProcessSnapShot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

	ProcessEntry.dwSize = sizeof(ProcessEntry);
	Return = Process32First(hProcessSnapShot, &ProcessEntry);
	if (!Return) {
		return processlist;
	}

	do {
		processlist.push_back(ProcessEntry);
	} while (Process32Next(hProcessSnapShot, &ProcessEntry));

	// Close handle
	CloseHandle(hProcessSnapShot);
	return processlist;
}

int PerformanceMonitor::Cpp::Logic::getCPULogicalProcessorCores()
{
	SYSTEM_INFO sysinfo;
	GetSystemInfo(&sysinfo);
	return sysinfo.dwNumberOfProcessors;
}

int PerformanceMonitor::Cpp::Logic::getCPUPhysicalCores()
{
	return 1;
}

DWORD PerformanceMonitor::Cpp::Logic::getppid(int pid)
{
	HANDLE hSnapshot;
	PROCESSENTRY32 pe32;
	DWORD ppid = 0;

	hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	__try {
		if (hSnapshot == INVALID_HANDLE_VALUE) __leave;

		ZeroMemory(&pe32, sizeof(pe32));
		pe32.dwSize = sizeof(pe32);
		if (!Process32First(hSnapshot, &pe32)) __leave;

		do {
			if (pe32.th32ProcessID == pid) {
				ppid = pe32.th32ParentProcessID;
				break;
			}
		} while (Process32Next(hSnapshot, &pe32));

	}
	__finally {
		if (hSnapshot != INVALID_HANDLE_VALUE) CloseHandle(hSnapshot);
	}
	return ppid;
}
