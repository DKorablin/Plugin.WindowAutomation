using System;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.WindowAutomation.Plugins
{
	/// <summary>Wrapper around the runtime compiler plugin.</summary>
	internal class CompilerPlugin
	{
		/// <summary>Reference to current plugin instance.</summary>
		private readonly Plugin _plugin;
		private IPluginDescription _pluginCompiler;

		/// <summary>Compilation plugin constants.</summary>
		private static class PluginConstants
		{
			/// <summary>Runtime compiler plugin ID.</summary>
			public const String Name = "425f8b0c-f049-44ee-8375-4cc874d6bf94";

			/// <summary>Public plugin method names.</summary>
			public static class Methods
			{
				/// <summary>Check whether a method exists.</summary>
				public const String IsMethodExists = "IsMethodExists";

				/// <summary>Get list of all methods created for this plugin.</summary>
				public const String GetMethods = "GetMethods";

				/// <summary>Delete a method.</summary>
				public const String DeleteMethod = "DeleteMethod";

				/// <summary>Invoke dynamic code.</summary>
				public const String InvokeDynamicMethod = "InvokeDynamicMethod";
			}

			/// <summary>Plugin window identifiers.</summary>
			public static class Windows
			{
				/// <summary>.NET source code editor window.</summary>
				public const String DocumentCompiler = "Plugin.Compiler.DocumentCompiler";

				/// <summary>Save event name for the <c>DocumentCompiler</c> window.</summary>
				public const String SaveEventName = "SaveEvent";
			}
		}

		/// <summary>Gets IPluginDescription for this plugin instance.</summary>
		private IPluginDescription PluginSelf
		{
			get
			{
				foreach(IPluginDescription plugin in this._plugin.HostWindows.Plugins)
					if(plugin.Instance == this._plugin)
						return plugin;
				throw new InvalidOperationException();
			}
		}

		/// <summary>Gets IPluginDescription for the compiler plugin.</summary>
		public IPluginDescription PluginInstance
			=> this._pluginCompiler ?? (this._pluginCompiler = this._plugin.HostWindows.Plugins[PluginConstants.Name]);

		public static String Name => PluginConstants.Name;

		/// <summary>Create wrapper instance.</summary>
		/// <param name="plugin">Current plugin.</param>
		public CompilerPlugin(Plugin plugin)
			=> this._plugin = plugin;

		/// <summary>Create a code editor window for compiling dynamic method.</summary>
		/// <param name="methodName">Class name used in dynamic code.</param>
		/// <param name="onSave">Callback invoked when code is saved.</param>
		public void CreateCompilerWindow(String methodName, EventHandler<DataEventArgs> onSave)
		{
			IPluginDescription self = this.PluginSelf;
			_ = this.PluginInstance
				?? throw new InvalidOperationException($"Plugin ID={PluginConstants.Name} not installed");

			IWindow wnd = this._plugin.HostWindows.Windows.CreateWindow(PluginConstants.Name,
				PluginConstants.Windows.DocumentCompiler,
				false,
				new
				{
					CallerPluginId = self.ID,
					ArgumentsType = new Type[] { },
					ReturnType = typeof(Boolean),
					ClassName = methodName,
				});

			if(wnd != null && onSave != null)
				wnd.AddEventHandler(PluginConstants.Windows.SaveEventName, onSave);
		}

		/// <summary>Delete a dynamic method.</summary>
		/// <param name="methodName">Method name.</param>
		/// <returns>True if deleted.</returns>
		public Boolean DeleteMethod(String methodName)
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance;

			return pluginCompiler != null
				&& (Boolean)pluginCompiler.Type
					.GetMember<IPluginMethodInfo>(PluginConstants.Methods.DeleteMethod)
					.Invoke(self, methodName);
		}

		/// <summary>Check if a dynamic method exists for this plugin.</summary>
		/// <param name="methodName">Method name.</param>
		/// <returns>True if method exists.</returns>
		public Boolean IsMethodExists(String methodName)
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance;

			return pluginCompiler !=null
				&& (Boolean)pluginCompiler.Type
					.GetMember<IPluginMethodInfo>(PluginConstants.Methods.IsMethodExists)
					.Invoke(self, methodName);
		}

		/// <summary>Get all dynamic methods created for this plugin.</summary>
		/// <returns>List of method names or null if not available.</returns>
		public String[] GetMethods()
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance;

			return pluginCompiler == null
				? new String[] { }
				: (String[])pluginCompiler.Type
					.GetMember<IPluginMethodInfo>(PluginConstants.Methods.GetMethods)
					.Invoke(self);
		}

		/// <summary>Invoke pre-compiled dynamic code.</summary>
		/// <param name="methodName">Class name used in dynamic code.</param>
		/// <param name="compilerArgs">Arguments passed to the compiled class.</param>
		/// <returns>Method result.</returns>
		public Object InvokeDynamicMethod(String methodName, params Object[] compilerArgs)
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance
				?? throw new InvalidOperationException($"Plugin ID={PluginConstants.Name} not installed");

			return pluginCompiler.Type
				.GetMember<IPluginMethodInfo>(PluginConstants.Methods.InvokeDynamicMethod)
				.Invoke(self, methodName, compilerArgs);
		}
	}
}