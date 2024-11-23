using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Plugin.WindowAutomation.Dto.Clicker
{
	/// <summary>Проект всех действий кликера</summary>
	internal class ActionsProject
	{
		private static Type[] ActionTypes = new Type[] { typeof(ActionKey), typeof(ActionMouse), typeof(ActionText), typeof(ActionMethod), typeof(System.Drawing.Point), };

		/// <summary>Действия для выполнения</summary>
		public List<ActionBase> Actions { get; set; }

		/// <summary>Проверка валидности всех действий</summary>
		public Boolean IsValid
		{
			get
			{
				if(this.Actions.Count == 0)
					return false;
				foreach(ActionBase action in this.Actions)
					if(!action.IsValid)
						return false;
				return true;
			}
		}

		/// <summary>Загрузка действий из потока</summary>
		/// <param name="stream">Поток, из которого загрузить действия</param>
		public ActionsProject(Stream stream)
			=> this.Actions = ActionsProject.Load(stream);

		/// <summary>Загрузить действия из файла с действиями</summary>
		/// <param name="filePath">Путь к файлу с действиями</param>
		public ActionsProject(String filePath)
		{
			using(FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				this.Actions = ActionsProject.Load(stream);
		}

		/// <summary>Создать новый проект без действий</summary>
		public ActionsProject()
			=> this.Actions = new List<ActionBase>();

		public Boolean Invoke(Func<ActionBase, Boolean> callback)
		{
			for(Int32 loop = 0; loop < this.Actions.Count; loop++)
			{
				ActionBase action = this.Actions[loop];
				if(action.Disabled)
					continue;

				if(callback != null && callback(action) == false)
					return false;

				if(action is ActionMethod)
				{
					ActionMethod mAction = (ActionMethod)action;
					try
					{
						mAction.Invoke();
					} catch(Exception exc)
					{
						PluginWindows.Trace.TraceData(TraceEventType.Error, 10, exc);
					}

					if(mAction.Result == false)
						return false;
					else if(loop + 1 == this.Actions.Count)
						loop = -1;//If last action is runtime method and it returns True, then start from beginning
				} else
					action.Invoke();
				if(action.Timeout > 0)
					System.Threading.Thread.Sleep((Int32)action.Timeout);
			}
			return true;
		}

		/// <summary>Сериализовать действия в строку</summary>
		/// <returns>JSON с действиями кликера</returns>
		public String Serialize()
			=> this.Actions == null || this.Actions.Count == 0
				? null
				: Serializer.JavaScriptSerialize(this.Actions.ToArray(), ActionsProject.ActionTypes);

		public void Save(String filePath)
			=> File.WriteAllText(filePath, this.Serialize());

		private static List<ActionBase> Load(Stream stream)
		{
			using(StreamReader reader = new StreamReader(stream))
				return new List<ActionBase>(Serializer.JavaScriptDeserialize<ActionBase[]>(reader.ReadToEnd(), ActionsProject.ActionTypes));
		}
	}
}