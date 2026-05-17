using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Plugin.WindowAutomation.Tests.Helpers
{
	/// <summary>Executes actions on a dedicated STA thread (required for WinForms controls).</summary>
	internal static class StaRunner
	{
		public static void Run(Action action)
		{
			Exception exception = null;
			Thread thread = new Thread(() =>
			{
				try
				{
					action();
				} catch(Exception e)
				{
					exception = e;
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
			if(exception != null)
				ExceptionDispatchInfo.Capture(exception).Throw();
		}
	}
}
