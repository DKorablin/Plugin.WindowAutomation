using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using Plugin.WindowAutomation.Dto;
using Plugin.WindowAutomation.Native;
using Plugin.WindowAutomation.Plugins;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.WindowAutomation
{
	public class Plugin : IPlugin, IPluginSettings<Settings>
	{
		private Settings _settings;
		private IMenuItem _menuWinApi;
		private IMenuItem _menuWindowFinder;
		private IMenuItem _menuWindowClicker;
		private Dictionary<String, DockState> _documentTypes;

		private GlobalWindowsHookAntiDebounce _antiDebounceHook;

		internal IHostWindows HostWindows { get; }

		internal CompilerPlugin Compiler { get; private set; }

		/// <summary>Settings for interaction from the host</summary>
		Object IPluginSettings.Settings => this.Settings;

		/// <summary>Settings for interaction from the plugin</summary>
		public Settings Settings
		{
			get
			{
				if(this._settings == null)
				{
					this._settings = new Settings(this);
					this.HostWindows.Plugins.Settings(this).LoadAssemblyParameters(this._settings);
					this._settings.PropertyChanged += this.Settings_PropertyChanged;
				}
				return this._settings;
			}
		}

		internal static Plugin Instance { get; private set; }

		internal static ITraceSource Trace { get; private set; }

		private Dictionary<String, DockState> DocumentTypes
		{
			get
			{
				if(this._documentTypes == null)
					this._documentTypes = new Dictionary<String, DockState>()
					{
						{ typeof(PanelWindowFinder).ToString(), DockState.DockTopAutoHide },
						{ typeof(PanelWindowClicker).ToString(), DockState.DockTopAutoHide },
					};
				return this._documentTypes;
			}
		}

		public Plugin(IHostWindows hostWindows, ITraceSource trace)
		{
			this.HostWindows = hostWindows ?? throw new ArgumentNullException(nameof(hostWindows));
			Plugin.Instance = this;
			Plugin.Trace = trace ?? throw new ArgumentNullException(nameof(trace));
		}

		/// <summary>Creates and returns a plugin control window of the specified type.</summary>
		/// <param name="typeName">The fully qualified name of the plugin control type to create. Cannot be null or empty.</param>
		/// <param name="args">An object containing arguments to pass to the plugin control's constructor, or null if no arguments are required.</param>
		/// <returns>An instance of a window implementing the IWindow interface for the specified plugin control type.</returns>
		public IWindow GetPluginControl(String typeName, Object args)
			=> this.CreateWindow(typeName, false, args);

		/// <summary>
		/// Retrieves an array of information about all currently opened top-level windows that are visible and have a non-empty caption.
		/// </summary>
		/// <remarks>This method excludes windows that are not visible or do not have a caption.
		/// The returned windows may include those from other processes.
		/// The order of windows in the array is not guaranteed.
		/// </remarks>
		/// <returns>
		/// An array of <see cref="WindowInfo"/> objects representing the visible, captioned top-level windows currently open on the system.
		/// The array is empty if no such windows are found.
		/// </returns>
		public WindowInfo[] GetOpenedWindows()
		{
			List<WindowInfo> result = new List<WindowInfo>();
			Native.Window.EnumWindows((hWnd, lParam) =>
			{
				WindowInfo info = new WindowInfo(hWnd);
				if(!info.IsVisible)
					return true;

				if(String.IsNullOrEmpty(info.Caption))
					return true;

				result.Add(info);
				return true;
			}, IntPtr.Zero);
			return result.ToArray();
		}

		/// <summary>
		/// Captures a bitmap image of the window specified by its handle and returns the image data as a PNG-encoded byte array.
		/// </summary>
		/// <remarks>
		/// The returned image represents the visible content of the specified window at the time of the call.
		/// If the window is minimized, covered, or otherwise not fully visible, the captured image may reflect its current on-screen state.
		/// </remarks>
		/// <param name="handleId">The handle of the window to capture, represented as a 64-bit integer.
		/// Must correspond to a valid window handle.
		/// </param>
		/// <returns>
		/// A byte array containing the PNG-encoded bitmap of the window's current screen content; or null if the handle is zero or invalid.</returns>
		public Byte[] GetWindowBitmap(Int64 handleId)
		{
			IntPtr handle = new IntPtr(handleId);
			if(handle == IntPtr.Zero)
				return null;

			WindowInfo info = new WindowInfo(handle);
			using(var bitmap = info.GetWindowScreen())
				return WindowInfo.ConvertBitmap(bitmap, ImageFormat.Png);
		}

		/// <summary>
		/// Simulates a mouse click at the specified coordinates, where x=0;y=0 equals to top left corner of the window, within the window identified by the given handle and returns a PNG image of the window after the click.
		/// </summary>
		/// <remarks>
		/// The method brings the target window to the foreground before performing the click.
		/// The returned image reflects the window's appearance approximately two seconds after the click.
		/// This method blocks for at least two seconds to allow the window to update its display.
		/// </remarks>
		/// <param name="handleId">The handle of the target window, as a 64-bit integer. Must not be zero.</param>
		/// <param name="x">The x-coordinate, in pixels, relative to the client area of the window, where the click will be performed.</param>
		/// <param name="y">The y-coordinate, in pixels, relative to the client area of the window, where the click will be performed.</param>
		/// <returns>A byte array containing the PNG-encoded image of the window after the click is performed; or null if the handle is zero or invalid.</returns>
		public Byte[] ClickOnWindow(Int64 handleId, Int32 x, Int32 y)
		{
			IntPtr handle = new IntPtr(handleId);
			if(handle == IntPtr.Zero)
				return null;

			WindowInfo info = new WindowInfo(handle);
			info.Focus(ensureFocused: true)
			.Click(x, y);

			System.Threading.Thread.Sleep(2000);

			using(var changed = info.GetWindowScreen())
				return WindowInfo.ConvertBitmap(changed, ImageFormat.Png);
		}

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			IMenuItem menuTools = this.HostWindows.MainMenu.FindMenuItem("Tools");
			if(menuTools == null)
			{
				Plugin.Trace.TraceEvent(TraceEventType.Error, 10, "Menu item 'Tools' not found");
				return false;
			}

			this.HostWindows.Plugins.PluginsLoaded += this.Host_PluginsLoaded;
			this.Settings_PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(WindowAutomation.Settings.AntiDebounceHookType)));

			this._menuWinApi = menuTools.FindMenuItem("WinAPI");
			if(this._menuWinApi == null)
			{
				this._menuWinApi = menuTools.Create("WinAPI");
				this._menuWinApi.Name = "Tools.WinAPI";
				menuTools.Items.Add(this._menuWinApi);
			}
			this._menuWindowFinder = this._menuWinApi.Create("&Window Finder");
			this._menuWindowFinder.Name = "Tools.WinAPI.WindowFinder";
			this._menuWindowFinder.Click += (sender, e) => { this.CreateWindow(typeof(PanelWindowFinder).ToString(), true); };

			this._menuWindowClicker = this._menuWinApi.Create("Window &Clicker");
			this._menuWindowClicker.Name = "Tools.WinAPI.WindowClicker";
			this._menuWindowClicker.Click += (sender, e)=> { this.CreateWindow(typeof(PanelWindowClicker).ToString(), true); };
			this._menuWinApi.Items.AddRange(new IMenuItem[] { this._menuWindowFinder, this._menuWindowClicker, });
			return true;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
		{
			if(this._menuWindowFinder != null)
				this.HostWindows.MainMenu.Items.Remove(this._menuWindowFinder);
			if(this._menuWindowClicker != null)
				this.HostWindows.MainMenu.Items.Remove(this._menuWindowClicker);
			if(this._menuWinApi != null && this._menuWinApi.Items.Count == 0)
				this.HostWindows.MainMenu.Items.Remove(this._menuWinApi);

			this._antiDebounceHook?.Dispose();
			return true;
		}

		private void Host_PluginsLoaded(Object sender, EventArgs e)
			=> this.Compiler = new CompilerPlugin(this);

		private void Settings_PropertyChanged(Object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
			case nameof(WindowAutomation.Settings.AntiDebounceHookType):
			case nameof(WindowAutomation.Settings.AntiDebounceThresholdMs):
				this._antiDebounceHook?.Dispose();
				this._antiDebounceHook = null;

				if(this.Settings.AntiDebounceHookType != Dto.HookType.None)
					this._antiDebounceHook = new GlobalWindowsHookAntiDebounceWithTrace(this.Settings.AntiDebounceHookType, (UInt32)this.Settings.AntiDebounceThresholdMs);
				break;
			}
		}

		internal IWindow CreateWindow(String typeName, Boolean searchForOpened, Object args = null)
			=> this.DocumentTypes.TryGetValue(typeName, out DockState state)
				? this.HostWindows.Windows.CreateWindow(this, typeName, searchForOpened, state, args)
				: null;

		/// <summary>Generates a unique method name that does not conflict with any existing compiled method names.</summary>
		/// <returns>A unique method name based on the base name "WindowClicker", suffixed with an incrementing number if necessary.</returns>
		public String GetUniqueMethodName()
		{
			const String ConstMethodName = "WindowClicker";
			String methodName = ConstMethodName;
			UInt32 count = 1;
			String[] methods = this.Compiler.GetMethods();
			while(Array.Exists(methods, item => item == methodName))
				methodName = String.Join("_", ConstMethodName, (count++).ToString());

			return methodName;
		}
	}
}