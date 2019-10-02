#pragma once
#include "..\WindowsPerformanceMonitor.Cpp\Logic.h"
#include <vector>

namespace PerformanceMonitor
{
	namespace Cpp
	{
		class Logic; // This allows us to mention it in this header file

		namespace CLI
		{
			// Wrapper
			public ref class Logic
			{
			public:
				Logic();
				~Logic();
				!Logic();
				int getCPULogicalProcessorCores();
				int getCPUPhysicalCores();
				std::string getCPUFrequency();
				std::vector<PROCESSENTRY32> getProcessList();
				void Destroy(); // Helper function
				DWORD getppid(int pid);
			private:
				// Pointer to our implementation
				Cpp::Logic* _impl;
			};
		}
	}
}