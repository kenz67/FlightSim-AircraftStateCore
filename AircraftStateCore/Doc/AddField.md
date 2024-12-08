# Display New Setting

* **PlaneData.cs**
  * Add variable for new field before the line "Junk" or put in a particular place if desired.
* **SimConnect** Service
  * Add definition for the new data point at the same place in the list as the variable above
	* e.g.: _proxy.AddToDataDefinition(DATA_DEFINITIONS.SimPlaneDataStructure, "TRANSPONDER CODE:1", "Bco16", SIMCONNECT_DATATYPE.INT32, 0.0f, SimConnect.SIMCONNECT_UNUSED);
* **FieldText.cd**
  * Add a definition for the field.  This is how it will appear on the display page
* **Components/Pages/Index.razor**
  * Add display information for the new field in the appropriate category on the page
* **Formatter.cs** (in necesary)
  * If needed, add a method here to fix how the display appears on the page


# Database

# Apply

