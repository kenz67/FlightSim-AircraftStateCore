﻿using AircraftStateCore.Services;

namespace AircraftStateCoreTests.Services;

public class DbCommonTests
{
	[Fact]
	public void DbNameTest()
	{
		Assert.EndsWith("\\AircraftState\\AircraftState2.sqlite", DbCommon.DbName);
	}
}
