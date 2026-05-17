using System.Windows.Forms;
using FluentAssertions;
using Plugin.WindowAutomation.Tests.Helpers;
using Xunit;

namespace Plugin.WindowAutomation.Tests
{
	public class PanelWindowFinderTests
	{
		[Fact]
		public void Constructor_DoesNotThrow()
			=> StaRunner.Run(() =>
			{
				using(PanelWindowFinder panel = new PanelWindowFinder())
					panel.Should().NotBeNull();
			});

		[Fact]
		public void IsAssignableFrom_UserControl()
			=> typeof(PanelWindowFinder).Should().BeAssignableTo<UserControl>();

		[Fact]
		public void Dispose_DoesNotThrow()
			=> StaRunner.Run(() =>
			{
				PanelWindowFinder panel = new PanelWindowFinder();
				panel.Invoking(p => p.Dispose()).Should().NotThrow();
			});
	}
}
