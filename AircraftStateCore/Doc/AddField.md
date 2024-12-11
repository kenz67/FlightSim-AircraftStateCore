# Display New Setting

* **PlaneData.cs**
  * Add variable for new field before the line "Junk" or put in a particular place if desired.
* **SimConnect** Service
  * Add definition for the new data point at the same place in the list as the variable above
	* e.g.: _proxy.AddToDataDefinition(DATA_DEFINITIONS.SimPlaneDataStructure, "TRANSPONDER CODE:1", "Bco16", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
* **FieldText.cs**
  * Add a definition for the field.  This is how it will appear on the display page
* **Components/Pages/Home.razor**
  * Add display information for the new field in the appropriate category on the page
* **Formatter.cs** (in necesary)
  * If needed, add a method here to fix how the display appears on the page


# Settings

* Add new field to the AvailableData.cs class

# Apply

* **Apply.razor**
  * Add field (copy from Index.razor above) and add appropriate "If" statements
  * Add new event to EVENT_IDS.cs
  * Add Event Map
	* e.g.: _proxy.MapClientEventToSimEvent(EVENT_IDS.TRANSPONDER1000DEC, "XPNDR_1000_DEC");
  * Add to appropriate "Send" method in SimConnectService

