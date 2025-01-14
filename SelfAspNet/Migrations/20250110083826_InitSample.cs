using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfAspNet.Migrations
{
    /// <inheritdoc />
    public partial class InitSample : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Samples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTitle = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samples", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SampleRelation1",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleId = table.Column<int>(type: "int", nullable: false),
                    RTitle = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleRelation1", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SampleRelation1_Samples_SampleId",
                        column: x => x.SampleId,
                        principalTable: "Samples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleRelation1_SampleId",
                table: "SampleRelation1",
                column: "SampleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SampleRelation1");

            migrationBuilder.DropTable(
                name: "Samples");
        }
    }
}
