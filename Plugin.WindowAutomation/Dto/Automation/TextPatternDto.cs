using System;
using System.Linq;
using System.Windows.Automation;

namespace Plugin.WindowAutomation.Dto.Automation
{
	internal class TextPatternDto
	{
		private readonly TextPattern _pattern;

		public Object StrikethroughStyleAttribute
			=> this._pattern.DocumentRange.GetAttributeValue(TextPattern.StrikethroughStyleAttribute);

		public Object StrikethroughColorAttribute
			=> this._pattern.DocumentRange.GetAttributeValue(TextPattern.StrikethroughColorAttribute);

		public Object OverlineColorAttribute
			=> this._pattern.DocumentRange.GetAttributeValue(TextPattern.OverlineColorAttribute);

		public Object BackgroundColorAttribute
			=> this._pattern.DocumentRange.GetAttributeValue(TextPattern.BackgroundColorAttribute);

		public Object OverlineStyleAttribute
			=> this._pattern.DocumentRange.GetAttributeValue(TextPattern.OverlineStyleAttribute);

		public SupportedTextSelection SupportedTextSelection
			=> this._pattern.SupportedTextSelection;

		public String[] SelectedText
		{
			get
			{
				String[] result = this._pattern.GetSelection().Select(p => p.GetText(Int32.MaxValue)).Where(p => p != null && p.Length > 0).ToArray();
				return result.Length == 0
					? null
					: result;
			}
		}

		public TextPatternDto(TextPattern pattern)
			=> this._pattern = pattern;
	}
}