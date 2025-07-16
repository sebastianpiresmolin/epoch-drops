using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EpochDropsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestRewardDrops2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    InternalId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    Rarity = table.Column<int>(type: "integer", nullable: false),
                    ItemType = table.Column<string>(type: "text", nullable: false),
                    ItemSubType = table.Column<string>(type: "text", nullable: false),
                    EquipSlot = table.Column<string>(type: "text", nullable: false),
                    Tooltip = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.InternalId);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Zone = table.Column<string>(type: "text", nullable: false),
                    SubZone = table.Column<string>(type: "text", nullable: false),
                    X = table.Column<float>(type: "real", nullable: false),
                    Y = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NpcName = table.Column<string>(type: "text", nullable: false),
                    Zone = table.Column<string>(type: "text", nullable: false),
                    SubZone = table.Column<string>(type: "text", nullable: false),
                    X = table.Column<double>(type: "double precision", nullable: false),
                    Y = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LocationId = table.Column<int>(type: "integer", nullable: false),
                    Kills = table.Column<int>(type: "integer", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mobs_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestRewards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Xp = table.Column<int>(type: "integer", nullable: false),
                    Money = table.Column<int>(type: "integer", nullable: false),
                    SourceMobName = table.Column<string>(type: "text", nullable: false),
                    LocationId = table.Column<int>(type: "integer", nullable: false),
                    ItemInternalId = table.Column<int>(type: "integer", nullable: true),
                    QuestSourceId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestRewards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestRewards_Items_ItemInternalId",
                        column: x => x.ItemInternalId,
                        principalTable: "Items",
                        principalColumn: "InternalId");
                    table.ForeignKey(
                        name: "FK_QuestRewards_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestRewards_QuestSources_QuestSourceId",
                        column: x => x.QuestSourceId,
                        principalTable: "QuestSources",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemDrops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    MobId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDrops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemDrops_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "InternalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemDrops_Mobs_MobId",
                        column: x => x.MobId,
                        principalTable: "Mobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestRewardDrops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    QuestRewardId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestRewardDrops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestRewardDrops_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "InternalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestRewardDrops_QuestRewards_QuestRewardId",
                        column: x => x.QuestRewardId,
                        principalTable: "QuestRewards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemDrops_ItemId",
                table: "ItemDrops",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDrops_MobId",
                table: "ItemDrops",
                column: "MobId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Id",
                table: "Items",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mobs_LocationId",
                table: "Mobs",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewardDrops_ItemId",
                table: "QuestRewardDrops",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewardDrops_QuestRewardId",
                table: "QuestRewardDrops",
                column: "QuestRewardId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewards_ItemInternalId",
                table: "QuestRewards",
                column: "ItemInternalId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewards_LocationId",
                table: "QuestRewards",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestRewards_QuestSourceId",
                table: "QuestRewards",
                column: "QuestSourceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemDrops");

            migrationBuilder.DropTable(
                name: "QuestRewardDrops");

            migrationBuilder.DropTable(
                name: "Mobs");

            migrationBuilder.DropTable(
                name: "QuestRewards");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "QuestSources");
        }
    }
}
