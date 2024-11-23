using System;
using System.ComponentModel;

namespace Plugin.WindowAutomation.Dto.Clicker
{
	internal abstract class ActionBase
	{
		/// <summary>Timeout before next action</summary>
		[DefaultValue(0)]
		[Description("Timeout in milliseconds before next action will be performed")]
		public UInt32 Timeout { get; set; }

		[Description("Custom description for action")]
		public String Description { get; set; }

		protected ActionBase()
		{ }

		[Description("All required information is required for this action to work")]
		public virtual Boolean IsValid => false;

		[DefaultValue(false)]
		[Description("This action is disabled")]
		public Boolean Disabled { get; set; }

		public abstract void Invoke();
	}
}