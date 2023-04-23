using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AircraftStateCore.Database.Migrations
{
	/// <inheritdoc />
	public partial class Init : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "applicationSettings",
				columns: table => new
				{
					DataKey = table.Column<string>(type: "VARCHAR(100)", nullable: false),
					DataValue = table.Column<string>(type: "VARCHAR(100)", nullable: false)
				},
				constraints: table => table.PrimaryKey("PK_applicationSettings", x => x.DataKey));

			migrationBuilder.CreateTable(
				name: "profileData",
				columns: table => new
				{
					profileName = table.Column<string>(type: "VARCHAR(100)", nullable: false),
					data = table.Column<string>(type: "TEXT", nullable: true)
				},
				constraints: table => table.PrimaryKey("PK_profileData", x => x.profileName));
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "applicationSettings");

			migrationBuilder.DropTable(
				name: "profileData");
		}
	}
}
