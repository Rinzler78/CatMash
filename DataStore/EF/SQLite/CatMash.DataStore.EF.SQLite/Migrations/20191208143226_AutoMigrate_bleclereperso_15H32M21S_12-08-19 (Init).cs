using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CatMash.DataStore.EF.SQLite.Migrations
{
    public partial class AutoMigrate_bleclereperso_15H32M21S_120819Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: false),
                    URL = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cats",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: false),
                    ImageId = table.Column<int>(nullable: true),
                    NbMash = table.Column<int>(nullable: false),
                    Rate = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cats", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Cats_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mash",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ModificationDate = table.Column<DateTime>(nullable: false),
                    LeftCatName = table.Column<string>(nullable: true),
                    RightCatName = table.Column<string>(nullable: true),
                    WinnerCatName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mash", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mash_Cats_LeftCatName",
                        column: x => x.LeftCatName,
                        principalTable: "Cats",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mash_Cats_RightCatName",
                        column: x => x.RightCatName,
                        principalTable: "Cats",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mash_Cats_WinnerCatName",
                        column: x => x.WinnerCatName,
                        principalTable: "Cats",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cats_ImageId",
                table: "Cats",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Mash_LeftCatName",
                table: "Mash",
                column: "LeftCatName");

            migrationBuilder.CreateIndex(
                name: "IX_Mash_RightCatName",
                table: "Mash",
                column: "RightCatName");

            migrationBuilder.CreateIndex(
                name: "IX_Mash_WinnerCatName",
                table: "Mash",
                column: "WinnerCatName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mash");

            migrationBuilder.DropTable(
                name: "Cats");

            migrationBuilder.DropTable(
                name: "Images");
        }
    }
}
