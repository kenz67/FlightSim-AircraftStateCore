using Microsoft.FlightSimulator.SimConnect;
using static Microsoft.FlightSimulator.SimConnect.SimConnect;

//SimConnect has no Interface which makes it hard to UT, so add proxy class to pass 
//calls along and allow for Mocked UT
namespace AircraftStateCore.Services
{
	public class SimConnectProxy : ISimConnectProxy
	{
		private SimConnect sim;

		public bool ConnectToSim(string Name, nint WindowHandle, uint UserEvent, WaitHandle EventHandle, uint ConfigIndex)
		{
			try
			{
				sim = new SimConnect(Name, WindowHandle, UserEvent, EventHandle, ConfigIndex);
			}
			catch /*(COMException ex)*/
			{
				return false;
			}

			return true;
		}

		public bool CloseConnection()
		{
			bool ret = false;

			if (sim != null)
			{
				sim.Dispose();
				sim = null;
				ret = true;
			}

			return ret;
		}

		public void Disconnect()
		{
			if (sim != null)
			{
				sim.Dispose();
				sim = null;
			}
		}

		public bool IsConnected()
		{
			return sim != null;
		}

		public void ReceiveMessage()
		{
			sim?.ReceiveMessage();
		}

		public void AddOnRecvOpen(RecvOpenEventHandler handler)
		{
			if (IsConnected())
			{
				sim.OnRecvOpen += handler;
			}
		}

		public void AddOnRecvQuit(RecvQuitEventHandler handler)
		{
			if (IsConnected())
			{
				sim.OnRecvQuit += handler;
			}
		}

		public void AddOnRecvEvent(RecvSimobjectDataEventHandler handler)
		{
			if (IsConnected())
			{
				sim.OnRecvSimobjectData += handler;
			}
			//Sim.OnRecvEvent += new SimConnect.RecvEventEventHandler(SimConnect_OnRecvEvent);  //https://docs.flightsimulator.com/html/Programming_Tools/SimConnect/API_Reference/Events_And_Data/SimConnect_SubscribeToSystemEvent.htm
		}

		public void AddOnRecvException(RecvExceptionEventHandler handler)
		{
			if (IsConnected())
			{
				sim.OnRecvException += handler;
			}
		}

		public void AddToDataDefinition(Enum DefineId, string DatumName, string UnitsName, SIMCONNECT_DATATYPE DatumType, float Epsilon, uint DatumId)
		{
			sim?.AddToDataDefinition(DefineId, DatumName, UnitsName, DatumType, Epsilon, DatumId);
		}

		public void RegisterDataDefineStruct<T>(Enum dwId)
		{
			sim?.RegisterDataDefineStruct<T>(dwId);
		}

		public void RequestDataOnSimObject(Enum RequestId, Enum DefineId, uint ObjectId, SIMCONNECT_PERIOD Period, SIMCONNECT_DATA_REQUEST_FLAG Flags, uint Origin, uint Interval, uint Limit)
		{
			sim?.RequestDataOnSimObject(RequestId, DefineId, ObjectId, Period, Flags, Origin, Interval, Limit);
		}

		public void MapClientEventToSimEvent(Enum EventId, string EventName)
		{
			sim?.MapClientEventToSimEvent(EventId, EventName);
		}

		public void TransmitClientEvent(uint ObjectId, Enum EventId, uint Data, Enum GroupId, SIMCONNECT_EVENT_FLAG Flags)
		{
			sim?.TransmitClientEvent(ObjectId, EventId, Data, GroupId, Flags);
		}

		public void TransmitClientEvent_Ex1(uint ObjectId, Enum EventId, Enum GroupId, SIMCONNECT_EVENT_FLAG Flags, uint dwData0, uint dwData1, uint dwData2, uint dwData3, uint dwData4)
		{
			sim?.TransmitClientEvent_EX1(ObjectId, EventId, GroupId, Flags, dwData0, dwData1, dwData2, dwData3, dwData4);
		}

		public void SetDataOnSimObject(Enum DefineId, uint ObjectId, SIMCONNECT_DATA_SET_FLAG Flags, object DataSet)
		{
			sim?.SetDataOnSimObject(DefineId, ObjectId, Flags, DataSet);
		}
	}
}
