using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.Models;
using BootstrapBlazor.Components;
using Assert = Xunit.Assert;

namespace AircraftStateCore.Services.Tests
{
	public class SettingsDataTests
	{
		bool CalledOnChange = false;

		static AvailableData allData = new AvailableData();

		readonly Settings settings = new Settings
		{
			AutoSave = true,
			ShowApplyForm = true,
			BlockFuel = false,
			BlockLocation = false,
			Version = "testing",
			SelectedData = new List<AvailableDataItem>
				{
					new(allData.Items[10].value, allData.Items[10].txt, true),
					new(allData.Items[11].value, allData.Items[11].txt)
				}
		};

		Mock<ISettingsRepo> mockRepo = new Mock<ISettingsRepo>();
		SettingsData sut;

		public SettingsDataTests()
		{
			mockRepo.Setup(m => m.GetSettings()).ReturnsAsync(settings);
			sut = new SettingsData(mockRepo.Object);
		}

		[Fact()]
		public async Task ReadSettingsTest()
		{
			var result = await sut.ReadSettings();

			Assert.Equal(new AvailableData().Items.Count, result.SelectedData.Count);

			var t = result.SelectedData.Where(r => r.enabled && !r.value.EndsWith(".0")).ToList();

			Assert.Single(t);
			Assert.Equal(allData.Items[10].txt, t[0].txt);
		}

		[Fact()]
		public async Task UpdatePageTest()
		{
			sut.OnChangeAsync += OnChange;
			await sut.UpdatePage();
			Assert.True(CalledOnChange);
		}

		private Task OnChange()
		{
			CalledOnChange = true;
			return Task.CompletedTask;
		}

		[Fact()]
		public async Task GetSelectedDataTest()
		{
			await sut.ReadSettings();
			var data = sut.GetSelectedData();

			Assert.Equal(10, data.Count);  //8 headings + 1 enabled
			Assert.Single(data.Where(d => d.txt.Equals(allData.Items[10].txt)).ToList());
		}

		[Fact()]
		public async Task SaveSettingsTest()
		{
			var newItems = new List<SelectedItem>
			{
				new(allData.Items[11].value, allData.Items[11].txt)
			};

			await sut.ReadSettings();

			await sut.SaveSettings(newItems);

			var t = sut.Settings.SelectedData.Where(r => r.enabled && !r.value.EndsWith(".0")).ToList();

			Assert.Single(t);
			Assert.Equal(allData.Items[11].txt, t[0].txt);
		}
	}
}