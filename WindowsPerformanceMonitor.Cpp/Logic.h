#pragma once

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
				int Get() const;
		};
	}
}
