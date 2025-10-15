using System;

namespace Plugin.WindowAutomation.Dto
{
	[Flags]
	public enum HookType
	{
		None = 0,
		Keyboard = 1 << 0,
		Mouse = 1 << 1,
	}
}