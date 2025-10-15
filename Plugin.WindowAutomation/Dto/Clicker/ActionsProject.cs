using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Plugin.WindowAutomation.Dto.Clicker
{
	/// <summary>Project containing all clicker actions.</summary>
	internal class ActionsProject
	{
		private static readonly Type[] ActionTypes = new Type[] { typeof(ActionKey), typeof(ActionMouse), typeof(ActionText), typeof(ActionMethod), typeof(System.Drawing.Point), };

		/// <summary>Actions to execute.</summary>
		public List<ActionBase> Actions { get; set; }

		/// <summary>Validate that all actions are valid and list is not empty.</summary>
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

		/// <summary>Load actions from a stream.</summary>
		/// <param name="stream">Stream to read serialized actions from.</param>
		public ActionsProject(Stream stream)
			=> this.Actions = ActionsProject.Load(stream);

		/// <summary>Load actions from a file.</summary>
		/// <param name="filePath">Path to a file containing serialized actions.</param>
		public ActionsProject(String filePath)
		{
			using(FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				this.Actions = ActionsProject.Load(stream);
		}

		/// <summary>Create an empty project with no actions.</summary>
		public ActionsProject()
			=> this.Actions = new List<ActionBase>();

		public Boolean Invoke(Func<ActionBase, Boolean> callback)
		{
			for(Int32 loop = 0; loop < this.Actions.Count; loop++)
			{
				ActionBase action = this.Actions[loop];
				if(action.Disabled)
					continue;

				if(callback != null && !callback(action))
					return false;

				if(action is ActionMethod mAction)
				{
					try
					{
						mAction.Invoke();
					} catch(Exception exc)
					{
						Plugin.Trace.TraceData(TraceEventType.Error, 10, exc);
					}

					if(!mAction.Result)
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

		/// <summary>Serialize actions to JSON string.</summary>
		/// <returns>JSON containing clicker actions or null if there are none.</returns>
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