using System.Reflection;
using System.Runtime.InteropServices;

[assembly: ComVisible(false)]
[assembly: Guid("e8d66370-032d-453a-8bbd-7dbea49feb54")]
[assembly: System.CLSCompliant(false)]

#if NETCOREAPP
[assembly: AssemblyMetadata("ProjectUrl", "https://dkorablin.ru/")]
#else

[assembly: AssemblyTitle("Plugin.WindowAutomation")]
[assembly: AssemblyDescription("Browse and automate user events")]
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Danila Korablin")]
[assembly: AssemblyProduct("Plugin.WindowAutomation")]
[assembly: AssemblyCopyright("Copyright © Danila Korablin 2021-2024")]
#endif