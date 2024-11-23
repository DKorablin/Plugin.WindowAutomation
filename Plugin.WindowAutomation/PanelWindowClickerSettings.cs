using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Plugin.WindowAutomation
{
	public class PanelWindowClickerSettings : INotifyPropertyChanged
	{
		private String _projectFileName;
		public String ProjectFileName
		{
			get => this._projectFileName;
			set => this.SetField(ref this._projectFileName, value, nameof(this.ProjectFileName));
		}

		internal void SetValues(String projectFileName)
			=> this._projectFileName = projectFileName;

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