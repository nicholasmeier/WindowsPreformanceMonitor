#include "Logic.h"
#include "..\WindowsPerformanceMonitor.Cpp\Logic.h"
#include <string>
#include <Windows.h>

using namespace std;

PerformanceMonitor::Cpp::CLI::Logic::Logic()
	: _impl(new Cpp::Logic())
	// Allocate some memory for the native implementation
{
}

int PerformanceMonitor::Cpp::CLI::Logic::Get()
{
	return _impl->Get(); // Call native Get
}

void PerformanceMonitor::Cpp::CLI::Logic::Destroy()
{
	if (_impl != nullptr)
	{
		delete _impl;
		_impl = nullptr;
	}
}

PerformanceMonitor::Cpp::CLI::Logic::~Logic()
{
	Destroy(); // Clean-up any native resources 
}

PerformanceMonitor::Cpp::CLI::Logic::!Logic()
{
	Destroy(); // Clean-up any native resources 
}
