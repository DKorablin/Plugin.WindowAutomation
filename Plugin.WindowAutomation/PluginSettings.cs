using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Plugin.WindowAutomation.Dto.Clicker;

namespace Plugin.WindowAutomation
{
	public class PluginSettings : INotifyPropertyChanged
	{
		private const String ClickerJson = "ClickerJson";

		private Keys _start;
		private Keys _record;
		private ActionsProject _clickerActions;
		private readonly PluginWindows _plugin;

		[Category("Shortcuts")]
		[Description("Start or Stop window clicker")]
		[Editor(typeof(ShortcutKeysEditor), typeof(UITypeEditor))]
		[DefaultValue(Keys.None)]
		public Keys Start
		{
			get => this._start;
			set => this.SetField(ref this._start, value, nameof(this.Start));
		}

		[Category("Shortcuts")]
		[Description("Save mouse clicks and keyboards inputs")]
		[Editor(typeof(ShortcutKeysEditor), typeof(UITypeEditor))]
		[DefaultValue(Keys.None)]
		public Keys Record
		{
			get => this._record;
			set => this.SetField(ref this._record, value, nameof(this.Record));
		}

		internal ActionsProject ClickerActions
			=> this._clickerActions ?? (this._clickerActions = this.GetActions());

		internal PluginSettings(PluginWindows plugin)
			=> this._plugin = plugin;

		private ActionsProject GetActions()
		{
			using(Stream stream = this._plugin.HostWindows.Plugins.Settings(this._plugin).LoadAssemblyBlob(PluginSettings.ClickerJson))
				return stream == null
					? new ActionsProject()
					: this.GetActions(stream);
		}

		internal ActionsProject GetActions(Stream stream)
			=> new ActionsProject(stream);

		internal void SaveActions(ActionsProject project)
		{
			String json = project.Serialize();
			if(json == null)
				this._plugin.HostWindows.Plugins.Settings(this._plugin).SaveAssemblyBlob(PluginSettings.ClickerJson, null);
			else
			{
				Byte[] payload = Encoding.UTF8.GetBytes(json);
				using(MemoryStream stream = new MemoryStream(payload))
					this._plugin.HostWindows.Plugins.Settings(this._plugin).SaveAssemblyBlob(PluginSettings.ClickerJson, stream);
			}

			this._clickerActions = project;
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private Boolean SetField<T>(ref T field, T value, String propertyName)
		{
			if(EqualityComparer<T>.Default.Equals(field, value))
				return false;

			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
		#endregion INotifyPropertyChanged
	}
}