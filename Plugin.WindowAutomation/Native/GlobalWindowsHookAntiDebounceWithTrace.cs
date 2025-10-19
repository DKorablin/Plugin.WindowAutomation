using System;
using System.Threading;
using Plugin.WindowAutomation.Dto;

namespace Plugin.WindowAutomation.Native
{
	internal class GlobalWindowsHookAntiDebounceWithTrace : GlobalWindowsHookAntiDebounce
	{
		// Aggregated suppression counters
		private readonly Int32[] _suppressedKeyCounts = new Int32[256];
		private readonly Int32[] _suppressedMouseCounts = new Int32[5];
		private Int32 _flushScheduled;
		private Int32 _lastFlushTick;
		private const Int32 FlushIntervalMs = 1000; // batching interval

		public GlobalWindowsHookAntiDebounceWithTrace(HookType hookType, UInt32 thresholdMs = 50)
			: base(hookType, thresholdMs: thresholdMs)
			=> Plugin.Trace.TraceInformation("Debounce started: HookType={0}, ThresholdMs={1:N0}", hookType, thresholdMs);

		protected override void OnSuppressedKey(Int32 virtualKey, Int32 nowTick)
		{
			unchecked { _suppressedKeyCounts[virtualKey]++; }
			this.TryScheduleFlush(nowTick);
		}

		protected override void OnSuppressedMouse(Int32 buttonIndex, Int32 nowTick)
		{
			unchecked { _suppressedMouseCounts[buttonIndex]++; }
			this.TryScheduleFlush(nowTick);
		}

		private void TryScheduleFlush(Int32 nowTick)
		{
			if(unchecked(nowTick - this._lastFlushTick) < FlushIntervalMs)
				return;
			if(Interlocked.CompareExchange(ref _flushScheduled, 1, 0) == 0)
				ThreadPool.UnsafeQueueUserWorkItem(_ => this.FlushSuppressedCounts(), null);
		}

		private void FlushSuppressedCounts()
		{
			try
			{
				Int32 now = Environment.TickCount;
				this._lastFlushTick = now;
				System.Text.StringBuilder sb = null;

				for(Int32 i = 0; i < this._suppressedKeyCounts.Length; i++)
				{
					Int32 c = this._suppressedKeyCounts[i];
					if(c == 0) continue;

					this._suppressedKeyCounts[i] = 0;
					if(sb == null)
						sb = new System.Text.StringBuilder(256).Append("Debounce suppressed:");

					sb.Append($" Key {(System.Windows.Forms.Keys)i} ({c:N0});");
				}
				for(Int32 i = 0; i < this._suppressedMouseCounts.Length; i++)
				{
					Int32 c = this._suppressedMouseCounts[i];
					if(c == 0) continue;

					this._suppressedMouseCounts[i] = 0;
					if(sb == null)
						sb = new System.Text.StringBuilder(256).Append("Debounce suppressed:");

					sb.Append($" Mouse {GetMouseButtonName(i)} '({c:N0})';");
				}

				if(sb != null)
					Plugin.Trace.TraceInformation(sb.ToString());
			} finally
			{
				Interlocked.Exchange(ref _flushScheduled, 0);
			}

			String GetMouseButtonName(Int32 index)
			{
				switch(index)
				{
				case 0: return "Left";
				case 1: return "Right";
				case 2: return "Middle";
				case 3: return "XButton1";
				case 4: return "XButton2";
				default: return "Unknown";
				}
			}
		}
	}
}