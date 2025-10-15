using System;
using System.Collections.Generic;
using System.Diagnostics;
using Plugin.WindowAutomation.Native;
using Plugin.WindowAutomation.Plugins;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.WindowAutomation
{
	public class Plugin : IPlugin, IPluginSettings<Settings>
	{
		internal static Plugin Instance;//HACK
		private static TraceSource _trace;
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

		internal static TraceSource Trace => _trace ?? (_trace = Plugin.CreateTraceSource<Plugin>());

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

		public Plugin(IHostWindows hostWindows)
		{
			this.HostWindows = hostWindows ?? throw new ArgumentNullException(nameof(hostWindows));
			Plugin.Instance = this;
		}
		public IWindow GetPluginControl(String typeName, Object args)
			=> this.CreateWindow(typeName, false, args);

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

		private static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}

		/// <summary>Get a unique method name for the new timer</summary>
		/// <returns>Unique method name</returns>
		public String GetUniqueMethodName()
		{
			const String ConstMethodName = "WindowClicker";
			String methodName = ConstMethodName;
			UInt32 count = 1;
			String[] methods = this.Compiler.GetMethods();
			while(Array.Exists(methods, item => item == methodName))
				methodName = String.Join("_", new String[] { ConstMethodName, (count++).ToString(), });

			return methodName;
		}
	}
}