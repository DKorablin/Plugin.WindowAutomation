using System;
using System.Windows.Automation;

namespace Plugin.WindowAutomation.Dto.Automation
{
	internal class SelectionPatternDto
	{
		private readonly SelectionPattern _pattern;

		public Boolean CanSelectMultiple => this._pattern.Current.CanSelectMultiple;

		public Boolean IsSelectionRequired => this._pattern.Current.IsSelectionRequired;

		public SelectionPatternDto(SelectionPattern pattern)
			=> this._pattern = pattern;
	}
}