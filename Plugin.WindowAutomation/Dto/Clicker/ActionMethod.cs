using System;
using System.ComponentModel;
using System.Drawing.Design;
using Plugin.WindowAutomation.UI;

namespace Plugin.WindowAutomation.Dto.Clicker
{
	internal class ActionMethod : ActionBase
	{
		internal class MethodFake : ActionBase
		{
			[DisplayName("Method Name")]
			[Description("Наименование метода, вызываемого во время выполнения")]
			public String MethodName { get { return "Plugin.Compiler not loaded"; } }

			[Description("To access this action you have to add Plugin.Compiler")]
			public override Boolean IsValid => false;

			public override void Invoke()
				=> throw new NotSupportedException();
		}

		[Editor(typeof(CompilerMethodEditor), typeof(UITypeEditor))]
		[DisplayName("Method Name")]
		[Description("Наименование метода, вызываемого во время выполнения")]
		public String MethodName { get; set; }

		[DefaultValue(false)]
		[Description("Method invocation result. If it's the last action, then actions will repeat from beginning")]
		internal Boolean Result { get; private set; }

		public override Boolean IsValid
			=> !String.IsNullOrEmpty(this.MethodName)
				&& PluginWindows.Instance.Compiler.IsMethodExists(this.MethodName);

		public override void Invoke()
		{
			this.Result = false;//In case of exception
			this.Result = (Boolean)PluginWindows.Instance.Compiler.InvokeDynamicMethod(this.MethodName);
		}
	}
}