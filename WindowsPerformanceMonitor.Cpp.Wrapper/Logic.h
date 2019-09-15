#pragma once

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
				int Get();
				void Destroy(); // Helper function
			private:
				// Pointer to our implementation
				Cpp::Logic* _impl;
			};
		}
	}
}
