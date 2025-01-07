using AircraftStateCore.DAL.Repositories.Interfaces;
using AircraftStateCore.Models;

namespace AircraftStateCore.Services.Tests
{
	public class PlaneDataTests
	{
		bool EvtCalled = false;

		PlaneDataStruct planeData = new PlaneDataStruct
		{
			adfActive = 123.4,
			adfCard = 567.8,
			masterAvionics = true,
			masterBattery = false,
		};

		[Fact()]
		public async Task LookUpProfileTest()
		{
			var repoMock = new Mock<IPlaneDataRepo>();

			//set and check a few fields
			repoMock.Setup(r => r.GetDataForProfile(It.IsAny<string>()))
				.ReturnsAsync(planeData);

			var sut = new PlaneData(repoMock.Object);
			sut.OnChangeAsync += Evt;

			await sut.LookUpProfile("test");

			Assert.Equal(123.4, sut.CurrentData.adfActive);
			Assert.Equal(567.8, sut.CurrentData.adfCard);
			Assert.True(sut.CurrentData.masterAvionics);
			Assert.False(sut.CurrentData.masterBattery);

			Assert.True(EvtCalled);
		}

		[Fact()]
		public async Task DeleteProfileTest()
		{
			var repoMock = new Mock<IPlaneDataRepo>();
			var sut = new PlaneData(repoMock.Object);

			await sut.DeleteProfile("delete me");

			repoMock.Verify(r => r.DeleteSavedProfile("delete me"), Times.Once);
		}

		[Fact()]
		public async Task SaveProfileTest()
		{
			var repoMock = new Mock<IPlaneDataRepo>();
			var sut = new PlaneData(repoMock.Object);

			sut.CurrentData = planeData;
			sut.Profiles = new List<string>
			{
				{ "Z Profile 1" }
			};

			await sut.SaveProfile("save me");

			repoMock.Verify(r => r.SaveDataForProfile("save me", planeData), Times.Once);
			Assert.Equal(2, sut.Profiles.Count);
			Assert.Equal("save me", sut.Profiles[0]);
			Assert.Equal("Z Profile 1", sut.Profiles[1]);
		}

		private Task Evt()
		{
			EvtCalled = true;
			return Task.CompletedTask;
		}
	}
}