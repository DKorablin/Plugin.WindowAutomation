﻿using System.Reflection;
using System.Runtime.InteropServices;

[assembly: Guid("e8d66370-032d-453a-8bbd-7dbea49feb54")]
[assembly: System.CLSCompliant(false)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/")]
#else

[assembly: AssemblyDescription("Browse and automate user events")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2021-2025")]
#endif