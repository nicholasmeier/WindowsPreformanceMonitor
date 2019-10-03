#include "Logic.h"

using namespace std;

PerformanceMonitor::Cpp::CLI::Logic::Logic()
	: _impl(new Cpp::Logic())
	// Allocate some memory for the native implementation
{
}

std::vector<PROCESSENTRY32> PerformanceMonitor::Cpp::CLI::Logic::getProcessList()
{
	return _impl->getProcessList();
}

int PerformanceMonitor::Cpp::CLI::Logic::getCPULogicalProcessorCores()
{
	return _impl->getCPULogicalProcessorCores();
}

int PerformanceMonitor::Cpp::CLI::Logic::getCPUPhysicalCores()
{
	return _impl->getCPUPhysicalCores();
}

std::string PerformanceMonitor::Cpp::CLI::Logic::getCPUFrequency()
{
	return _impl->getCPUClock();
}

void PerformanceMonitor::Cpp::CLI::Logic::Destroy()
{
	if (_impl != nullptr)
	{
		delete _impl;
		_impl = nullptr;
	}
}

DWORD PerformanceMonitor::Cpp::CLI::Logic::getppid(int pid) {
	return _impl->getppid(pid);
}

PerformanceMonitor::Cpp::CLI::Logic::~Logic()
{
	Destroy(); // Clean-up any native resources 
}

PerformanceMonitor::Cpp::CLI::Logic::!Logic()
{
	Destroy(); // Clean-up any native resources 
}
