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


