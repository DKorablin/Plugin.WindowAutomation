using System;
using System.Windows.Automation;

namespace Plugin.WindowAutomation.Dto.Automation
{
	internal class TransformPatternDto
	{
		private readonly TransformPattern _pattern;

		public Boolean CanMove => this._pattern.Current.CanMove;

		public Boolean CanResize => this._pattern.Current.CanResize;

		public Boolean CanRotate => this._pattern.Current.CanRotate;

		public TransformPatternDto(TransformPattern pattern)
			=> this._pattern = pattern;
	}
}