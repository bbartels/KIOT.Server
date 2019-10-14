using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KIOT.Server.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplianceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    ApplianceTypeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplianceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileDevices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    DeviceName = table.Column<string>(nullable: true),
                    MobileOS = table.Column<string>(nullable: false),
                    InstallationId = table.Column<string>(nullable: true),
                    LastLoginAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    IdentityId = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Code = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplianceCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplianceCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplianceCategories_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaretakerForCustomerRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    CaretakerId = table.Column<int>(nullable: false),
                    Handled = table.Column<bool>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaretakerForCustomerRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaretakerForCustomerRequests_Users_CaretakerId",
                        column: x => x.CaretakerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaretakerForCustomerRequests_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    CaretakerId = table.Column<int>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: true),
                    Completed = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerTasks_Users_CaretakerId",
                        column: x => x.CaretakerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerTasks_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IsCaredForBys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    CaretakerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsCaredForBys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IsCaredForBys_Users_CaretakerId",
                        column: x => x.CaretakerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IsCaredForBys_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MobileDeviceForUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    MobileDeviceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileDeviceForUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MobileDeviceForUser_MobileDevices_MobileDeviceId",
                        column: x => x.MobileDeviceId,
                        principalTable: "MobileDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MobileDeviceForUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PushTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    RegisteredAt = table.Column<DateTime>(nullable: false),
                    EndedAt = table.Column<DateTime>(nullable: true),
                    MobileDeviceId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PushTokens_MobileDevices_MobileDeviceId",
                        column: x => x.MobileDeviceId,
                        principalTable: "MobileDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PushTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    RemoteAddress = table.Column<string>(nullable: true),
                    ExpiresOn = table.Column<DateTime>(nullable: false),
                    UserId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAppliances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    ApplianceId = table.Column<int>(nullable: false),
                    Alias = table.Column<string>(nullable: true),
                    ApplianceTypeId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: true),
                    CustomerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAppliances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAppliances_ApplianceTypes_ApplianceTypeId",
                        column: x => x.ApplianceTypeId,
                        principalTable: "ApplianceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerAppliances_ApplianceCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ApplianceCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerAppliances_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Discriminator", "FirstName", "Guid", "IdentityId", "LastName", "PhoneNumber", "Username" },
                values: new object[,]
                {
                    { -4, "Caretaker", "Taylor", new Guid("9f746084-8796-4ff5-874e-75eb33bfe9e3"), "0254effb-2ca2-4403-97c0-71065525ec3d", "Williams", "+4418821003", "twilliams" },
                    { -5, "Caretaker", "Jack", new Guid("912217dd-b725-47aa-96b9-8fd73e7e6fa0"), "11fb19f4-e22c-4b0e-91ca-0917ad68ee80", "Smith", "+4418821004", "jsmith" },
                    { -6, "Caretaker", "Jared", new Guid("e0e26424-f4cd-4400-bc78-28b0bfef729a"), "be52b267-353c-45c5-9ab0-9592f5d8e3e0", "Cole", "+4418821005", "jcole" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Discriminator", "FirstName", "Guid", "IdentityId", "LastName", "PhoneNumber", "Username", "Code" },
                values: new object[,]
                {
                    { -1, "Customer", "Matt", new Guid("d53a5412-efdd-4905-9f5d-5c2a624726a2"), "24cfabf2-30e5-4e59-9be1-88dd861dec3c", "Wilson", "+4418821000", "mwilson", "0055_ALT-CM0103" },
                    { -2, "Customer", "Frank", new Guid("48dd8db1-2c48-4f8b-a318-8a85cc286557"), "dfe6beec-0f05-4fe9-851e-078160950468", "Brown", "+4418821001", "fbrown", "0055_ALT-CM0104" },
                    { -3, "Customer", "Will", new Guid("cf0415f9-ab05-47ba-a0ec-2c0e3f874bbc"), "586e130d-2583-4adb-b894-05304f68d1cf", "Jones", "+4418821002", "wjones", "0055_ALT-CM0105" }
                });

            migrationBuilder.InsertData(
                table: "CaretakerForCustomerRequests",
                columns: new[] { "Id", "CaretakerId", "CustomerId", "Guid", "Handled", "Timestamp" },
                values: new object[] { -1, -4, -1, new Guid("f61ee92d-792e-45dd-a664-309178a71830"), false, new DateTime(2019, 3, 5, 18, 47, 11, 105, DateTimeKind.Utc).AddTicks(7470) });

            migrationBuilder.InsertData(
                table: "CaretakerForCustomerRequests",
                columns: new[] { "Id", "CaretakerId", "CustomerId", "Guid", "Handled", "Timestamp" },
                values: new object[] { -2, -4, -2, new Guid("ccf08264-5f4b-48bc-9620-83257f14e6df"), false, new DateTime(2019, 3, 5, 18, 47, 11, 105, DateTimeKind.Utc).AddTicks(8147) });

            migrationBuilder.InsertData(
                table: "CaretakerForCustomerRequests",
                columns: new[] { "Id", "CaretakerId", "CustomerId", "Guid", "Handled", "Timestamp" },
                values: new object[] { -3, -4, -3, new Guid("4af29383-bb2e-4b3f-8645-ba1f6318ea0f"), false, new DateTime(2019, 3, 5, 18, 47, 11, 105, DateTimeKind.Utc).AddTicks(8161) });

            migrationBuilder.CreateIndex(
                name: "IX_ApplianceCategories_CustomerId",
                table: "ApplianceCategories",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplianceTypes_ApplianceTypeId",
                table: "ApplianceTypes",
                column: "ApplianceTypeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaretakerForCustomerRequests_CustomerId",
                table: "CaretakerForCustomerRequests",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CaretakerForCustomerRequests_Guid",
                table: "CaretakerForCustomerRequests",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaretakerForCustomerRequests_CaretakerId_CustomerId",
                table: "CaretakerForCustomerRequests",
                columns: new[] { "CaretakerId", "CustomerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAppliances_ApplianceId",
                table: "CustomerAppliances",
                column: "ApplianceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAppliances_ApplianceTypeId",
                table: "CustomerAppliances",
                column: "ApplianceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAppliances_CategoryId",
                table: "CustomerAppliances",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAppliances_CustomerId",
                table: "CustomerAppliances",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTasks_CaretakerId",
                table: "CustomerTasks",
                column: "CaretakerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTasks_CustomerId",
                table: "CustomerTasks",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_IsCaredForBys_CustomerId",
                table: "IsCaredForBys",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_IsCaredForBys_CaretakerId_CustomerId",
                table: "IsCaredForBys",
                columns: new[] { "CaretakerId", "CustomerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MobileDeviceForUser_UserId",
                table: "MobileDeviceForUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MobileDeviceForUser_MobileDeviceId_UserId",
                table: "MobileDeviceForUser",
                columns: new[] { "MobileDeviceId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MobileDevices_InstallationId",
                table: "MobileDevices",
                column: "InstallationId",
                unique: true,
                filter: "[InstallationId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PushTokens_MobileDeviceId",
                table: "PushTokens",
                column: "MobileDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_PushTokens_UserId",
                table: "PushTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Guid",
                table: "RefreshTokens",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId1",
                table: "RefreshTokens",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Guid",
                table: "Users",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Code",
                table: "Users",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Guid1",
                table: "Users",
                column: "Guid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaretakerForCustomerRequests");

            migrationBuilder.DropTable(
                name: "CustomerAppliances");

            migrationBuilder.DropTable(
                name: "CustomerTasks");

            migrationBuilder.DropTable(
                name: "IsCaredForBys");

            migrationBuilder.DropTable(
                name: "MobileDeviceForUser");

            migrationBuilder.DropTable(
                name: "PushTokens");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ApplianceTypes");

            migrationBuilder.DropTable(
                name: "ApplianceCategories");

            migrationBuilder.DropTable(
                name: "MobileDevices");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
