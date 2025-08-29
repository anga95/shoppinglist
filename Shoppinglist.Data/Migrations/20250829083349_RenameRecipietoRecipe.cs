using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shoppinglist.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameRecipietoRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipieItems");

            migrationBuilder.DropTable(
                name: "Recipies");

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipeItems",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeItems", x => new { x.RecipeId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_RecipeItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeItems_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipeItems_ItemId",
                table: "RecipeItems",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeItems");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.CreateTable(
                name: "Recipies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecipieItems",
                columns: table => new
                {
                    RecipieId = table.Column<int>(type: "INTEGER", nullable: false),
                    ItemId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipieItems", x => new { x.RecipieId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_RecipieItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipieItems_Recipies_RecipieId",
                        column: x => x.RecipieId,
                        principalTable: "Recipies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecipieItems_ItemId",
                table: "RecipieItems",
                column: "ItemId");
        }
    }
}
