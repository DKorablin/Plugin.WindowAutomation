using System;
using System.Windows.Automation;

namespace Plugin.WindowAutomation.Dto.Automation
{
	internal class ExpandCollapsePatternDto
	{
		private readonly ExpandCollapsePattern _pattern;

		public ExpandCollapseState State => this._pattern.Current.ExpandCollapseState;

		public Boolean Expand
		{
			get => this.State != ExpandCollapseState.Collapsed;
			set
			{
				if(value)
					this._pattern.Expand();
			}
		}

		public ExpandCollapsePatternDto(ExpandCollapsePattern pattern)
			=> this._pattern = pattern;
	}
}