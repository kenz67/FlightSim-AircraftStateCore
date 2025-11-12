using AircraftStateCore.Services;
using Assert = Xunit.Assert;

namespace AircraftStateCoreTests.Services;

public class DbCommonTests
{
	[Fact]
	public void DbNameTest()
	{
		Assert.EndsWith("\\AircraftState\\AircraftState2.sqlite", DbCommon.DbName);
	}
}
