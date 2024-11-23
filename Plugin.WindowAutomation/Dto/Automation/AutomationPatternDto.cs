using System;
using System.Windows.Automation;

namespace Plugin.WindowAutomation.Dto.Automation
{
	internal class AutomationPatternDto
	{
		public AutomationPattern Automation { get; }

		public AutomationPatternDto(AutomationPattern automation)
			=> this.Automation = automation;

		public override String ToString()
			=> this.Automation.ProgrammaticName;
	}
}