using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PR.Web.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoordinateSystem",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoordinateSystem", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObjectItems",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    AlternativeIdentificationText = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectItems", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    ArchiveID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Superseded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    Surname = table.Column<string>(type: "TEXT", nullable: true),
                    Nickname = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    ZipCode = table.Column<string>(type: "TEXT", nullable: true),
                    City = table.Column<string>(type: "TEXT", nullable: true),
                    Birthday = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Dead = table.Column<bool>(type: "INTEGER", nullable: true),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.ArchiveID);
                });

            migrationBuilder.CreateTable(
                name: "Smurfs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Smurfs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VerticalDistance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Dimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerticalDistance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Line",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Line", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Line_Location_Id",
                        column: x => x.Id,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Point",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Point", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Point_Location_Id",
                        column: x => x.Id,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Surface",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surface", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Surface_Location_Id",
                        column: x => x.Id,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    NickName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Organisations_ObjectItems_ID",
                        column: x => x.ID,
                        principalTable: "ObjectItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonAssociation",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArchiveID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Superseded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SubjectPersonID = table.Column<Guid>(type: "TEXT", nullable: false),
                    SubjectPersonArchiveID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ObjectPersonID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ObjectPersonArchiveID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonAssociation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PersonAssociation_People_ObjectPersonArchiveID",
                        column: x => x.ObjectPersonArchiveID,
                        principalTable: "People",
                        principalColumn: "ArchiveID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonAssociation_People_SubjectPersonArchiveID",
                        column: x => x.SubjectPersonArchiveID,
                        principalTable: "People",
                        principalColumn: "ArchiveID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonComments",
                columns: table => new
                {
                    ArchiveID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Superseded = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    PersonID = table.Column<Guid>(type: "TEXT", nullable: false),
                    PersonArchiveID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonComments", x => x.ArchiveID);
                    table.ForeignKey(
                        name: "FK_PersonComments_People_PersonArchiveID",
                        column: x => x.PersonArchiveID,
                        principalTable: "People",
                        principalColumn: "ArchiveID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GeometricVolume",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LowerVerticalDistanceID = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpperVerticalDistanceID = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeometricVolume", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeometricVolume_Location_Id",
                        column: x => x.Id,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeometricVolume_VerticalDistance_LowerVerticalDistanceID",
                        column: x => x.LowerVerticalDistanceID,
                        principalTable: "VerticalDistance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeometricVolume_VerticalDistance_UpperVerticalDistanceID",
                        column: x => x.UpperVerticalDistanceID,
                        principalTable: "VerticalDistance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AbsolutePoint",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LatitudeCoordinate = table.Column<double>(type: "REAL", nullable: false),
                    LongitudeCoordinate = table.Column<double>(type: "REAL", nullable: false),
                    VerticalDistanceId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbsolutePoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbsolutePoint_Point_Id",
                        column: x => x.Id,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbsolutePoint_VerticalDistance_VerticalDistanceId",
                        column: x => x.VerticalDistanceId,
                        principalTable: "VerticalDistance",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LinePoint",
                columns: table => new
                {
                    LineID = table.Column<Guid>(type: "TEXT", nullable: false),
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    PointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SequenceQuantity = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinePoint", x => new { x.LineID, x.Index });
                    table.ForeignKey(
                        name: "FK_LinePoint_Line_LineID",
                        column: x => x.LineID,
                        principalTable: "Line",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LinePoint_Point_PointId",
                        column: x => x.PointId,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointReference",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginPointID = table.Column<Guid>(type: "TEXT", nullable: false),
                    XVectorPointID = table.Column<Guid>(type: "TEXT", nullable: false),
                    YVectorPointID = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointReference", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PointReference_CoordinateSystem_ID",
                        column: x => x.ID,
                        principalTable: "CoordinateSystem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PointReference_Point_OriginPointID",
                        column: x => x.OriginPointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PointReference_Point_XVectorPointID",
                        column: x => x.XVectorPointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PointReference_Point_YVectorPointID",
                        column: x => x.YVectorPointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RelativePoint",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CoordinateSystemID = table.Column<Guid>(type: "TEXT", nullable: false),
                    XCoordinateDimension = table.Column<double>(type: "REAL", nullable: false),
                    YCoordinateDimension = table.Column<double>(type: "REAL", nullable: false),
                    ZCoordinateDimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelativePoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelativePoint_CoordinateSystem_CoordinateSystemID",
                        column: x => x.CoordinateSystemID,
                        principalTable: "CoordinateSystem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RelativePoint_Point_Id",
                        column: x => x.Id,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorridorArea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CenterLineID = table.Column<Guid>(type: "TEXT", nullable: false),
                    WidthDimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorridorArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorridorArea_Line_CenterLineID",
                        column: x => x.CenterLineID,
                        principalTable: "Line",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CorridorArea_Surface_Id",
                        column: x => x.Id,
                        principalTable: "Surface",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ellipse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CentrePointID = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstConjugateDiameterPointID = table.Column<Guid>(type: "TEXT", nullable: false),
                    SecondConjugateDiameterPointID = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ellipse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ellipse_Point_CentrePointID",
                        column: x => x.CentrePointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ellipse_Point_FirstConjugateDiameterPointID",
                        column: x => x.FirstConjugateDiameterPointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ellipse_Point_SecondConjugateDiameterPointID",
                        column: x => x.SecondConjugateDiameterPointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ellipse_Surface_Id",
                        column: x => x.Id,
                        principalTable: "Surface",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FanArea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VertexPointID = table.Column<Guid>(type: "TEXT", nullable: false),
                    MinimumRangeDimension = table.Column<double>(type: "REAL", nullable: false),
                    MaximumRangeDimension = table.Column<double>(type: "REAL", nullable: false),
                    OrientationAngle = table.Column<double>(type: "REAL", nullable: false),
                    SectorSizeAngle = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FanArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FanArea_Point_VertexPointID",
                        column: x => x.VertexPointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FanArea_Surface_Id",
                        column: x => x.Id,
                        principalTable: "Surface",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrbitArea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstPointID = table.Column<Guid>(type: "TEXT", nullable: false),
                    SecondPointID = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrbitAreaAlignmentCode = table.Column<int>(type: "INTEGER", nullable: false),
                    WidthDimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbitArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrbitArea_Point_FirstPointID",
                        column: x => x.FirstPointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrbitArea_Point_SecondPointID",
                        column: x => x.SecondPointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrbitArea_Surface_Id",
                        column: x => x.Id,
                        principalTable: "Surface",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolyArcArea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DefiningLineID = table.Column<Guid>(type: "TEXT", nullable: false),
                    BearingOriginPointID = table.Column<Guid>(type: "TEXT", nullable: false),
                    BeginBearingAngle = table.Column<double>(type: "REAL", nullable: false),
                    EndBearingAngle = table.Column<double>(type: "REAL", nullable: false),
                    ArcRadiusDimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolyArcArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolyArcArea_Line_DefiningLineID",
                        column: x => x.DefiningLineID,
                        principalTable: "Line",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolyArcArea_Point_BearingOriginPointID",
                        column: x => x.BearingOriginPointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolyArcArea_Surface_Id",
                        column: x => x.Id,
                        principalTable: "Surface",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolygonArea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BoundingLineID = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolygonArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolygonArea_Line_BoundingLineID",
                        column: x => x.BoundingLineID,
                        principalTable: "Line",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolygonArea_Surface_Id",
                        column: x => x.Id,
                        principalTable: "Surface",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackArea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BeginPointID = table.Column<Guid>(type: "TEXT", nullable: false),
                    EndPointID = table.Column<Guid>(type: "TEXT", nullable: false),
                    LeftWidthDimension = table.Column<double>(type: "REAL", nullable: false),
                    RightWidthDimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackArea_Point_BeginPointID",
                        column: x => x.BeginPointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackArea_Point_EndPointID",
                        column: x => x.EndPointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackArea_Surface_Id",
                        column: x => x.Id,
                        principalTable: "Surface",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "TEXT", nullable: false),
                    FormalAbbreviatedName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Units_Organisations_ID",
                        column: x => x.ID,
                        principalTable: "Organisations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConeVolume",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DefiningSurfaceID = table.Column<Guid>(type: "TEXT", nullable: false),
                    VertexPointID = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConeVolume", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConeVolume_GeometricVolume_Id",
                        column: x => x.Id,
                        principalTable: "GeometricVolume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConeVolume_Point_VertexPointID",
                        column: x => x.VertexPointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConeVolume_Surface_DefiningSurfaceID",
                        column: x => x.DefiningSurfaceID,
                        principalTable: "Surface",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SphereVolume",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CentrePointID = table.Column<Guid>(type: "TEXT", nullable: false),
                    RadiusDimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SphereVolume", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SphereVolume_GeometricVolume_Id",
                        column: x => x.Id,
                        principalTable: "GeometricVolume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SphereVolume_Point_CentrePointID",
                        column: x => x.CentrePointID,
                        principalTable: "Point",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurfaceVolume",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DefiningSurfaceID = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurfaceVolume", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurfaceVolume_GeometricVolume_Id",
                        column: x => x.Id,
                        principalTable: "GeometricVolume",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SurfaceVolume_Surface_DefiningSurfaceID",
                        column: x => x.DefiningSurfaceID,
                        principalTable: "Surface",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbsolutePoint_VerticalDistanceId",
                table: "AbsolutePoint",
                column: "VerticalDistanceId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConeVolume_DefiningSurfaceID",
                table: "ConeVolume",
                column: "DefiningSurfaceID");

            migrationBuilder.CreateIndex(
                name: "IX_ConeVolume_VertexPointID",
                table: "ConeVolume",
                column: "VertexPointID");

            migrationBuilder.CreateIndex(
                name: "IX_CorridorArea_CenterLineID",
                table: "CorridorArea",
                column: "CenterLineID");

            migrationBuilder.CreateIndex(
                name: "IX_Ellipse_CentrePointID",
                table: "Ellipse",
                column: "CentrePointID");

            migrationBuilder.CreateIndex(
                name: "IX_Ellipse_FirstConjugateDiameterPointID",
                table: "Ellipse",
                column: "FirstConjugateDiameterPointID");

            migrationBuilder.CreateIndex(
                name: "IX_Ellipse_SecondConjugateDiameterPointID",
                table: "Ellipse",
                column: "SecondConjugateDiameterPointID");

            migrationBuilder.CreateIndex(
                name: "IX_FanArea_VertexPointID",
                table: "FanArea",
                column: "VertexPointID");

            migrationBuilder.CreateIndex(
                name: "IX_GeometricVolume_LowerVerticalDistanceID",
                table: "GeometricVolume",
                column: "LowerVerticalDistanceID");

            migrationBuilder.CreateIndex(
                name: "IX_GeometricVolume_UpperVerticalDistanceID",
                table: "GeometricVolume",
                column: "UpperVerticalDistanceID");

            migrationBuilder.CreateIndex(
                name: "IX_LinePoint_PointId",
                table: "LinePoint",
                column: "PointId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitArea_FirstPointID",
                table: "OrbitArea",
                column: "FirstPointID");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitArea_SecondPointID",
                table: "OrbitArea",
                column: "SecondPointID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonAssociation_ObjectPersonArchiveID",
                table: "PersonAssociation",
                column: "ObjectPersonArchiveID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonAssociation_SubjectPersonArchiveID",
                table: "PersonAssociation",
                column: "SubjectPersonArchiveID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonComments_PersonArchiveID",
                table: "PersonComments",
                column: "PersonArchiveID");

            migrationBuilder.CreateIndex(
                name: "IX_PointReference_OriginPointID",
                table: "PointReference",
                column: "OriginPointID");

            migrationBuilder.CreateIndex(
                name: "IX_PointReference_XVectorPointID",
                table: "PointReference",
                column: "XVectorPointID");

            migrationBuilder.CreateIndex(
                name: "IX_PointReference_YVectorPointID",
                table: "PointReference",
                column: "YVectorPointID");

            migrationBuilder.CreateIndex(
                name: "IX_PolyArcArea_BearingOriginPointID",
                table: "PolyArcArea",
                column: "BearingOriginPointID");

            migrationBuilder.CreateIndex(
                name: "IX_PolyArcArea_DefiningLineID",
                table: "PolyArcArea",
                column: "DefiningLineID");

            migrationBuilder.CreateIndex(
                name: "IX_PolygonArea_BoundingLineID",
                table: "PolygonArea",
                column: "BoundingLineID");

            migrationBuilder.CreateIndex(
                name: "IX_RelativePoint_CoordinateSystemID",
                table: "RelativePoint",
                column: "CoordinateSystemID");

            migrationBuilder.CreateIndex(
                name: "IX_SphereVolume_CentrePointID",
                table: "SphereVolume",
                column: "CentrePointID");

            migrationBuilder.CreateIndex(
                name: "IX_SurfaceVolume_DefiningSurfaceID",
                table: "SurfaceVolume",
                column: "DefiningSurfaceID");

            migrationBuilder.CreateIndex(
                name: "IX_TrackArea_BeginPointID",
                table: "TrackArea",
                column: "BeginPointID");

            migrationBuilder.CreateIndex(
                name: "IX_TrackArea_EndPointID",
                table: "TrackArea",
                column: "EndPointID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbsolutePoint");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ConeVolume");

            migrationBuilder.DropTable(
                name: "CorridorArea");

            migrationBuilder.DropTable(
                name: "Ellipse");

            migrationBuilder.DropTable(
                name: "FanArea");

            migrationBuilder.DropTable(
                name: "LinePoint");

            migrationBuilder.DropTable(
                name: "OrbitArea");

            migrationBuilder.DropTable(
                name: "PersonAssociation");

            migrationBuilder.DropTable(
                name: "PersonComments");

            migrationBuilder.DropTable(
                name: "PointReference");

            migrationBuilder.DropTable(
                name: "PolyArcArea");

            migrationBuilder.DropTable(
                name: "PolygonArea");

            migrationBuilder.DropTable(
                name: "RelativePoint");

            migrationBuilder.DropTable(
                name: "Smurfs");

            migrationBuilder.DropTable(
                name: "SphereVolume");

            migrationBuilder.DropTable(
                name: "SurfaceVolume");

            migrationBuilder.DropTable(
                name: "TrackArea");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Line");

            migrationBuilder.DropTable(
                name: "CoordinateSystem");

            migrationBuilder.DropTable(
                name: "GeometricVolume");

            migrationBuilder.DropTable(
                name: "Point");

            migrationBuilder.DropTable(
                name: "Surface");

            migrationBuilder.DropTable(
                name: "Organisations");

            migrationBuilder.DropTable(
                name: "VerticalDistance");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "ObjectItems");
        }
    }
}
