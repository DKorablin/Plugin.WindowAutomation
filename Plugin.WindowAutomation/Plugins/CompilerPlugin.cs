using System;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.WindowAutomation.Plugins
{
	/// <summary>Обёртка над плагином компилятором</summary>
	internal class CompilerPlugin
	{
		/// <summary>Ссылка на текущий плагин</summary>
		private readonly PluginWindows _plugin;
		private IPluginDescription _pluginCompiler;

		/// <summary>Плагин компиляции</summary>
		private static class PluginConstants
		{
			/// <summary>ID плагина с рантайм компиляцией</summary>
			public const String Name = "425f8b0c-f049-44ee-8375-4cc874d6bf94";

			/// <summary>Публичные методы плагина</summary>
			public static class Methods
			{
				/// <summary>Проверка на существование кода</summary>
				public const String IsMethodExists = "IsMethodExists";

				/// <summary>Получить список всех методов, которые созданы для этого плагина</summary>
				public const String GetMethods = "GetMethods";

				/// <summary>Удалить код</summary>
				public const String DeleteMethod = "DeleteMethod";

				/// <summary>Выполнить динамический код</summary>
				public const String InvokeDynamicMethod = "InvokeDynamicMethod";
			}

			/// <summary>Окна плагина</summary>
			public static class Windows
			{
				/// <summary>Окно редактирования исходного кода на .NET</summary>
				public const String DocumentCompiler = "Plugin.Compiler.DocumentCompiler";

				/// <summary>Событие сохранения данных в окне <c>DocumentCompiler</c></summary>
				public const String SaveEventName = "SaveEvent";
			}
		}

		/// <summary>Ссылка на описание собственного плагина</summary>
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

		/// <summary>Ссылка на описание плагина компиляции</summary>
		public IPluginDescription PluginInstance
			=> this._pluginCompiler ?? (this._pluginCompiler = this._plugin.HostWindows.Plugins[this.Name]);

		/// <summary>Наименование плагина с компиляцией</summary>
		public String Name => PluginConstants.Name;

		/// <summary>Создание инстанса обёртки плагина компилятора</summary>
		/// <param name="plugin">Текущий плагин</param>
		public CompilerPlugin(PluginWindows plugin)
			=> this._plugin = plugin;

		/// <summary>Создать окно с редактированием кода компиляции</summary>
		/// <param name="methodName">Наименование класса, используемом в коде компиляции</param>
		/// <param name="onSave">Событие, вызываемое при сохранении кода в окне</param>
		public void CreateCompilerWindow(String methodName, EventHandler<DataEventArgs> onSave)
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance
				?? throw new ArgumentNullException($"Plugin ID={PluginConstants.Name} not installed");

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

		/// <summary>Удалить метод</summary>
		/// <param name="methodName">Наименование метода для удаления</param>
		/// <returns>Результат удаления метода</returns>
		public Boolean DeleteMethod(String methodName)
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance;

			return pluginCompiler != null
				&& (Boolean)pluginCompiler.Type
					.GetMember<IPluginMethodInfo>(PluginConstants.Methods.DeleteMethod)
					.Invoke(self, methodName);
		}

		/// <summary>Проверка на существование динамического метода в плагине компиляции</summary>
		/// <param name="methodName">Наименование метода в плагине компиляции</param>
		/// <returns>Данный метод существует в плагине компиляции для такого плагина</returns>
		public Boolean IsMethodExists(String methodName)
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance;

			return pluginCompiler !=null
				&& (Boolean)pluginCompiler.Type
					.GetMember<IPluginMethodInfo>(PluginConstants.Methods.IsMethodExists)
					.Invoke(self, methodName);
		}

		/// <summary>Получить список всех методов</summary>
		/// <returns>Список всех методов, созданных для конкретного плагина</returns>
		public String[] GetMethods()
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance;

			return pluginCompiler == null
				? null
				: (String[])pluginCompiler.Type
					.GetMember<IPluginMethodInfo>(PluginConstants.Methods.GetMethods)
					.Invoke(self);
		}

		/// <summary>Вызвать динамический код, написанный заранее через плагин</summary>
		/// <param name="methodName">Наименование класса, испольуемом в коде компиляции</param>
		/// <param name="compilerArgs">Аргументы, передаваемые в скомпилированный класс</param>
		/// <returns>Результат выполнения метода</returns>
		public Object InvokeDynamicMethod(String methodName, params Object[] compilerArgs)
		{
			IPluginDescription self = this.PluginSelf;
			IPluginDescription pluginCompiler = this.PluginInstance
				?? throw new ArgumentNullException($"Plugin ID={PluginConstants.Name} not installed");

			return pluginCompiler.Type
				.GetMember<IPluginMethodInfo>(PluginConstants.Methods.InvokeDynamicMethod)
				.Invoke(self, methodName, compilerArgs);
		}
	}
}