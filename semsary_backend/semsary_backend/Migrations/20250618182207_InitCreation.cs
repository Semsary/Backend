using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace semsary_backend.Migrations
{
    /// <inheritdoc />
    public partial class InitCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Balance = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExPosedPy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SermsaryUsers",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address_id = table.Column<int>(type: "int", nullable: false),
                    Address__gover = table.Column<int>(type: "int", nullable: false),
                    Address__city = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address_street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    CustomerService_DeviceTokens = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: true),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: true),
                    Balance = table.Column<int>(type: "int", nullable: true),
                    SocialId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceTokens = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompletedProfile = table.Column<bool>(type: "bit", nullable: true),
                    height = table.Column<int>(type: "int", nullable: true),
                    weight = table.Column<float>(type: "real", nullable: true),
                    NumberOfPeople = table.Column<int>(type: "int", nullable: true),
                    FavouriteRentalType = table.Column<int>(type: "int", nullable: true),
                    gender = table.Column<int>(type: "int", nullable: true),
                    age = table.Column<int>(type: "int", nullable: true),
                    IsSmoker = table.Column<bool>(type: "bit", nullable: true),
                    NeedPublicService = table.Column<bool>(type: "bit", nullable: true),
                    NeedPublicTransportation = table.Column<bool>(type: "bit", nullable: true),
                    NeedNearUniversity = table.Column<bool>(type: "bit", nullable: true),
                    NeedNearVitalPlaces = table.Column<bool>(type: "bit", nullable: true),
                    Tenant_DeviceTokens = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PremiumBegin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PremiumEnd = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SermsaryUsers", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "BlockedIds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SocialId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlockedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlockedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LandlordId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedIds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockedIds_SermsaryUsers_BlockedBy",
                        column: x => x.BlockedBy,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockedIds_SermsaryUsers_LandlordId",
                        column: x => x.LandlordId,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Emails",
                columns: table => new
                {
                    email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    ownerUsername = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    otp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtpExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumberofMismatch = table.Column<int>(type: "int", nullable: false),
                    otpType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emails", x => x.email);
                    table.ForeignKey(
                        name: "FK_Emails_SermsaryUsers_ownerUsername",
                        column: x => x.ownerUsername,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Houses",
                columns: table => new
                {
                    HouseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LandlordUsername = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    governorate = table.Column<int>(type: "int", nullable: false),
                    city = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvrageRate = table.Column<double>(type: "float", nullable: false),
                    NumOfRaters = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Houses", x => x.HouseId);
                    table.ForeignKey(
                        name: "FK_Houses_SermsaryUsers_LandlordUsername",
                        column: x => x.LandlordUsername,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "identityDocuments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerUsername = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageURLS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmitedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewerUsername = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantUsername = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_identityDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_identityDocuments_SermsaryUsers_OwnerUsername",
                        column: x => x.OwnerUsername,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_identityDocuments_SermsaryUsers_ReviewerUsername",
                        column: x => x.ReviewerUsername,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username");
                    table.ForeignKey(
                        name: "FK_identityDocuments_SermsaryUsers_TenantUsername",
                        column: x => x.TenantUsername,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantUsername = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LandlordUsername = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_SermsaryUsers_LandlordUsername",
                        column: x => x.LandlordUsername,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username");
                    table.ForeignKey(
                        name: "FK_Notifications_SermsaryUsers_TenantUsername",
                        column: x => x.TenantUsername,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "Advertisements",
                columns: table => new
                {
                    AdvertisementId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HouseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    houseDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rentalType = table.Column<int>(type: "int", nullable: false),
                    HouseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisements", x => x.AdvertisementId);
                    table.ForeignKey(
                        name: "FK_Advertisements_Houses_HouseId",
                        column: x => x.HouseId,
                        principalTable: "Houses",
                        principalColumn: "HouseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HouseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantUsername = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CommentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CommentDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantUsername1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comment_Houses_HouseId",
                        column: x => x.HouseId,
                        principalTable: "Houses",
                        principalColumn: "HouseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comment_SermsaryUsers_TenantUsername",
                        column: x => x.TenantUsername,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_SermsaryUsers_TenantUsername1",
                        column: x => x.TenantUsername1,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "HouseInspections",
                columns: table => new
                {
                    HouseInspectionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    longitude = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    latitude = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FloorNumber = table.Column<int>(type: "int", nullable: false),
                    NumberOfAirConditionnar = table.Column<int>(type: "int", nullable: false),
                    NumberOfPathRooms = table.Column<int>(type: "int", nullable: false),
                    NumberOfBedRooms = table.Column<int>(type: "int", nullable: false),
                    NumberOfBeds = table.Column<int>(type: "int", nullable: false),
                    NumberOfBalacons = table.Column<int>(type: "int", nullable: false),
                    NumberOfTables = table.Column<int>(type: "int", nullable: false),
                    NumberOfChairs = table.Column<int>(type: "int", nullable: false),
                    HouseFeature = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false),
                    InspectionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InspectionRequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InspectorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HouseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    inspectionStatus = table.Column<int>(type: "int", nullable: false),
                    HouseImages = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InspectionReport = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseInspections", x => x.HouseInspectionId);
                    table.ForeignKey(
                        name: "FK_HouseInspections_Houses_HouseId",
                        column: x => x.HouseId,
                        principalTable: "Houses",
                        principalColumn: "HouseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HouseInspections_SermsaryUsers_InspectorId",
                        column: x => x.InspectorId,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "Rates",
                columns: table => new
                {
                    RateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HouseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantUsername = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StarsNumber = table.Column<byte>(type: "tinyint", nullable: false),
                    TenantUsername1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rates", x => x.RateId);
                    table.ForeignKey(
                        name: "FK_Rates_Houses_HouseId",
                        column: x => x.HouseId,
                        principalTable: "Houses",
                        principalColumn: "HouseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rates_SermsaryUsers_TenantUsername",
                        column: x => x.TenantUsername,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rates_SermsaryUsers_TenantUsername1",
                        column: x => x.TenantUsername1,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    RentalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarrantyMoney = table.Column<int>(type: "int", nullable: false),
                    NumOfComments = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RentalType = table.Column<int>(type: "int", nullable: false),
                    TenantUsername = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HouseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    RentalUnitIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantUsername1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.RentalId);
                    table.ForeignKey(
                        name: "FK_Rentals_Houses_HouseId",
                        column: x => x.HouseId,
                        principalTable: "Houses",
                        principalColumn: "HouseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rentals_SermsaryUsers_TenantUsername",
                        column: x => x.TenantUsername,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rentals_SermsaryUsers_TenantUsername1",
                        column: x => x.TenantUsername1,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "RentalUnits",
                columns: table => new
                {
                    RentalUnitId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MonthlyCost = table.Column<int>(type: "int", nullable: false),
                    DailyCost = table.Column<int>(type: "int", nullable: false),
                    AdvertisementId = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalUnits", x => x.RentalUnitId);
                    table.ForeignKey(
                        name: "FK_RentalUnits_Advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisements",
                        principalColumn: "AdvertisementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    ComplaintId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: false),
                    VerifiedBy = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ComplaintReview = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComplaintDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubmittingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RentalId = table.Column<int>(type: "int", nullable: false),
                    TenantUsername = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.ComplaintId);
                    table.ForeignKey(
                        name: "FK_Complaints_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "RentalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Complaints_SermsaryUsers_SubmittedBy",
                        column: x => x.SubmittedBy,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complaints_SermsaryUsers_TenantUsername",
                        column: x => x.TenantUsername,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username");
                    table.ForeignKey(
                        name: "FK_Complaints_SermsaryUsers_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "SermsaryUsers",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RentalRentalUnit",
                columns: table => new
                {
                    RentalUnitId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RentalsRentalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalRentalUnit", x => new { x.RentalUnitId, x.RentalsRentalId });
                    table.ForeignKey(
                        name: "FK_RentalRentalUnit_RentalUnits_RentalUnitId",
                        column: x => x.RentalUnitId,
                        principalTable: "RentalUnits",
                        principalColumn: "RentalUnitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RentalRentalUnit_Rentals_RentalsRentalId",
                        column: x => x.RentalsRentalId,
                        principalTable: "Rentals",
                        principalColumn: "RentalId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_HouseId",
                table: "Advertisements",
                column: "HouseId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockedIds_BlockedBy",
                table: "BlockedIds",
                column: "BlockedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BlockedIds_LandlordId",
                table: "BlockedIds",
                column: "LandlordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_HouseId",
                table: "Comment",
                column: "HouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_TenantUsername",
                table: "Comment",
                column: "TenantUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_TenantUsername1",
                table: "Comment",
                column: "TenantUsername1");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_RentalId",
                table: "Complaints",
                column: "RentalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_SubmittedBy",
                table: "Complaints",
                column: "SubmittedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_TenantUsername",
                table: "Complaints",
                column: "TenantUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_VerifiedBy",
                table: "Complaints",
                column: "VerifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_ownerUsername",
                table: "Emails",
                column: "ownerUsername");

            migrationBuilder.CreateIndex(
                name: "IX_HouseInspections_HouseId",
                table: "HouseInspections",
                column: "HouseId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseInspections_InspectorId",
                table: "HouseInspections",
                column: "InspectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Houses_LandlordUsername",
                table: "Houses",
                column: "LandlordUsername");

            migrationBuilder.CreateIndex(
                name: "IX_identityDocuments_OwnerUsername",
                table: "identityDocuments",
                column: "OwnerUsername");

            migrationBuilder.CreateIndex(
                name: "IX_identityDocuments_ReviewerUsername",
                table: "identityDocuments",
                column: "ReviewerUsername");

            migrationBuilder.CreateIndex(
                name: "IX_identityDocuments_TenantUsername",
                table: "identityDocuments",
                column: "TenantUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_LandlordUsername",
                table: "Notifications",
                column: "LandlordUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TenantUsername",
                table: "Notifications",
                column: "TenantUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_HouseId",
                table: "Rates",
                column: "HouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_TenantUsername",
                table: "Rates",
                column: "TenantUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_TenantUsername1",
                table: "Rates",
                column: "TenantUsername1");

            migrationBuilder.CreateIndex(
                name: "IX_RentalRentalUnit_RentalsRentalId",
                table: "RentalRentalUnit",
                column: "RentalsRentalId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_HouseId",
                table: "Rentals",
                column: "HouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_TenantUsername",
                table: "Rentals",
                column: "TenantUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_TenantUsername1",
                table: "Rentals",
                column: "TenantUsername1");

            migrationBuilder.CreateIndex(
                name: "IX_RentalUnits_AdvertisementId",
                table: "RentalUnits",
                column: "AdvertisementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockedIds");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "Emails");

            migrationBuilder.DropTable(
                name: "HouseInspections");

            migrationBuilder.DropTable(
                name: "identityDocuments");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Rates");

            migrationBuilder.DropTable(
                name: "RentalRentalUnit");

            migrationBuilder.DropTable(
                name: "RentalUnits");

            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "Advertisements");

            migrationBuilder.DropTable(
                name: "Houses");

            migrationBuilder.DropTable(
                name: "SermsaryUsers");
        }
    }
}
