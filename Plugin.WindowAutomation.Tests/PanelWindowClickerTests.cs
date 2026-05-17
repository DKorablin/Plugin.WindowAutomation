using System.Windows.Forms;
using FluentAssertions;
using Plugin.WindowAutomation.Tests.Helpers;
using SAL.Flatbed;
using Xunit;

namespace Plugin.WindowAutomation.Tests
{
	public class PanelWindowClickerTests
	{
		[Fact]
		public void Constructor_DoesNotThrow()
			=> StaRunner.Run(() =>
			{
				using(PanelWindowClicker panel = new PanelWindowClicker())
					panel.Should().NotBeNull();
			});

		[Fact]
		public void IsAssignableFrom_UserControl()
			=> typeof(PanelWindowClicker).Should().BeAssignableTo<UserControl>();

		[Fact]
		public void Settings_IsNotNull()
			=> StaRunner.Run(() =>
			{
				using(PanelWindowClicker panel = new PanelWindowClicker())
					panel.Settings.Should().NotBeNull();
			});

		[Fact]
		public void Settings_ProjectFileName_IsNullByDefault()
			=> StaRunner.Run(() =>
			{
				using(PanelWindowClicker panel = new PanelWindowClicker())
					panel.Settings.ProjectFileName.Should().BeNull();
			});

		[Fact]
		public void Settings_ReturnsSameInstance()
			=> StaRunner.Run(() =>
			{
				using(PanelWindowClicker panel = new PanelWindowClicker())
					panel.Settings.Should().BeSameAs(panel.Settings);
			});

		[Fact]
		public void IPluginSettings_Settings_MatchesTypedSettings()
			=> StaRunner.Run(() =>
			{
				using(PanelWindowClicker panel = new PanelWindowClicker())
					((IPluginSettings)panel).Settings.Should().BeSameAs(panel.Settings);
			});

		[Fact]
		public void Dispose_DoesNotThrow()
			=> StaRunner.Run(() =>
			{
				PanelWindowClicker panel = new PanelWindowClicker();
				panel.Invoking(p => p.Dispose()).Should().NotThrow();
			});
	}
}
