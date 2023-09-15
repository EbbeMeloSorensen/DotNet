using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C2IEDM.Web.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                name: "CoordinateSystems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ObjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Superseded = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoordinateSystems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ObjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Superseded = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObjectItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    AlternativeIdentificationText = table.Column<string>(type: "TEXT", nullable: true),
                    ObjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Superseded = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerticalDistances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Dimension = table.Column<double>(type: "REAL", nullable: false),
                    ObjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Superseded = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerticalDistances", x => x.Id);
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
                name: "Lines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lines_Locations_Id",
                        column: x => x.Id,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Points",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Points", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Points_Locations_Id",
                        column: x => x.Id,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Surfaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surfaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Surfaces_Locations_Id",
                        column: x => x.Id,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NickName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organisations_ObjectItems_Id",
                        column: x => x.Id,
                        principalTable: "ObjectItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeometricVolumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LowerVerticalDistanceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpperVerticalDistanceId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeometricVolumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeometricVolumes_Locations_Id",
                        column: x => x.Id,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeometricVolumes_VerticalDistances_LowerVerticalDistanceId",
                        column: x => x.LowerVerticalDistanceId,
                        principalTable: "VerticalDistances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeometricVolumes_VerticalDistances_UpperVerticalDistanceId",
                        column: x => x.UpperVerticalDistanceId,
                        principalTable: "VerticalDistances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AbsolutePoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LatitudeCoordinate = table.Column<double>(type: "REAL", nullable: false),
                    LongitudeCoordinate = table.Column<double>(type: "REAL", nullable: false),
                    VerticalDistanceId = table.Column<Guid>(type: "TEXT", nullable: true),
                    VerticalDistanceObjectId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbsolutePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbsolutePoints_Points_Id",
                        column: x => x.Id,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbsolutePoints_VerticalDistances_VerticalDistanceId",
                        column: x => x.VerticalDistanceId,
                        principalTable: "VerticalDistances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LinePoints",
                columns: table => new
                {
                    LineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    LineObjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PointObjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SequenceQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    ObjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Superseded = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinePoints", x => new { x.LineId, x.Index });
                    table.ForeignKey(
                        name: "FK_LinePoints_Lines_LineId",
                        column: x => x.LineId,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LinePoints_Points_PointId",
                        column: x => x.PointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointReferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OriginPointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    XVectorPointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    YVectorPointId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointReferences_CoordinateSystems_Id",
                        column: x => x.Id,
                        principalTable: "CoordinateSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PointReferences_Points_OriginPointId",
                        column: x => x.OriginPointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PointReferences_Points_XVectorPointId",
                        column: x => x.XVectorPointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PointReferences_Points_YVectorPointId",
                        column: x => x.YVectorPointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RelativePoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CoordinateSystemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    XCoordinateDimension = table.Column<double>(type: "REAL", nullable: false),
                    YCoordinateDimension = table.Column<double>(type: "REAL", nullable: false),
                    ZCoordinateDimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelativePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelativePoints_CoordinateSystems_CoordinateSystemId",
                        column: x => x.CoordinateSystemId,
                        principalTable: "CoordinateSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RelativePoints_Points_Id",
                        column: x => x.Id,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorridorAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CenterLineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    WidthDimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorridorAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorridorAreas_Lines_CenterLineId",
                        column: x => x.CenterLineId,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CorridorAreas_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ellipses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CentrePointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstConjugateDiameterPointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SecondConjugateDiameterPointId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ellipses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ellipses_Points_CentrePointId",
                        column: x => x.CentrePointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ellipses_Points_FirstConjugateDiameterPointId",
                        column: x => x.FirstConjugateDiameterPointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ellipses_Points_SecondConjugateDiameterPointId",
                        column: x => x.SecondConjugateDiameterPointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ellipses_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FanAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VertexPointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MinimumRangeDimension = table.Column<double>(type: "REAL", nullable: false),
                    MaximumRangeDimension = table.Column<double>(type: "REAL", nullable: false),
                    OrientationAngle = table.Column<double>(type: "REAL", nullable: false),
                    SectorSizeAngle = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FanAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FanAreas_Points_VertexPointId",
                        column: x => x.VertexPointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FanAreas_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrbitAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstPointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SecondPointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrbitAreaAlignmentCode = table.Column<int>(type: "INTEGER", nullable: false),
                    WidthDimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbitAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrbitAreas_Points_FirstPointId",
                        column: x => x.FirstPointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrbitAreas_Points_SecondPointId",
                        column: x => x.SecondPointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrbitAreas_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolyArcAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DefiningLineId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BearingOriginPointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BeginBearingAngle = table.Column<double>(type: "REAL", nullable: false),
                    EndBearingAngle = table.Column<double>(type: "REAL", nullable: false),
                    ArcRadiusDimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolyArcAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolyArcAreas_Lines_DefiningLineId",
                        column: x => x.DefiningLineId,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolyArcAreas_Points_BearingOriginPointId",
                        column: x => x.BearingOriginPointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolyArcAreas_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolygonAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BoundingLineId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolygonAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolygonAreas_Lines_BoundingLineId",
                        column: x => x.BoundingLineId,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolygonAreas_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    BeginPointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EndPointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LeftWidthDimension = table.Column<double>(type: "REAL", nullable: false),
                    RightWidthDimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackAreas_Points_BeginPointId",
                        column: x => x.BeginPointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackAreas_Points_EndPointId",
                        column: x => x.EndPointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackAreas_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FormalAbbreviatedName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_Organisations_Id",
                        column: x => x.Id,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConeVolumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DefiningSurfaceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VertexPointId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConeVolumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConeVolumes_GeometricVolumes_Id",
                        column: x => x.Id,
                        principalTable: "GeometricVolumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConeVolumes_Points_VertexPointId",
                        column: x => x.VertexPointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConeVolumes_Surfaces_DefiningSurfaceId",
                        column: x => x.DefiningSurfaceId,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SphereVolumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CentrePointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RadiusDimension = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SphereVolumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SphereVolumes_GeometricVolumes_Id",
                        column: x => x.Id,
                        principalTable: "GeometricVolumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SphereVolumes_Points_CentrePointId",
                        column: x => x.CentrePointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurfaceVolumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DefiningSurfaceId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurfaceVolumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurfaceVolumes_GeometricVolumes_Id",
                        column: x => x.Id,
                        principalTable: "GeometricVolumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SurfaceVolumes_Surfaces_DefiningSurfaceId",
                        column: x => x.DefiningSurfaceId,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbsolutePoints_VerticalDistanceId",
                table: "AbsolutePoints",
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
                name: "IX_ConeVolumes_DefiningSurfaceId",
                table: "ConeVolumes",
                column: "DefiningSurfaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConeVolumes_VertexPointId",
                table: "ConeVolumes",
                column: "VertexPointId");

            migrationBuilder.CreateIndex(
                name: "IX_CorridorAreas_CenterLineId",
                table: "CorridorAreas",
                column: "CenterLineId");

            migrationBuilder.CreateIndex(
                name: "IX_Ellipses_CentrePointId",
                table: "Ellipses",
                column: "CentrePointId");

            migrationBuilder.CreateIndex(
                name: "IX_Ellipses_FirstConjugateDiameterPointId",
                table: "Ellipses",
                column: "FirstConjugateDiameterPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Ellipses_SecondConjugateDiameterPointId",
                table: "Ellipses",
                column: "SecondConjugateDiameterPointId");

            migrationBuilder.CreateIndex(
                name: "IX_FanAreas_VertexPointId",
                table: "FanAreas",
                column: "VertexPointId");

            migrationBuilder.CreateIndex(
                name: "IX_GeometricVolumes_LowerVerticalDistanceId",
                table: "GeometricVolumes",
                column: "LowerVerticalDistanceId");

            migrationBuilder.CreateIndex(
                name: "IX_GeometricVolumes_UpperVerticalDistanceId",
                table: "GeometricVolumes",
                column: "UpperVerticalDistanceId");

            migrationBuilder.CreateIndex(
                name: "IX_LinePoints_PointId",
                table: "LinePoints",
                column: "PointId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitAreas_FirstPointId",
                table: "OrbitAreas",
                column: "FirstPointId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitAreas_SecondPointId",
                table: "OrbitAreas",
                column: "SecondPointId");

            migrationBuilder.CreateIndex(
                name: "IX_PointReferences_OriginPointId",
                table: "PointReferences",
                column: "OriginPointId");

            migrationBuilder.CreateIndex(
                name: "IX_PointReferences_XVectorPointId",
                table: "PointReferences",
                column: "XVectorPointId");

            migrationBuilder.CreateIndex(
                name: "IX_PointReferences_YVectorPointId",
                table: "PointReferences",
                column: "YVectorPointId");

            migrationBuilder.CreateIndex(
                name: "IX_PolyArcAreas_BearingOriginPointId",
                table: "PolyArcAreas",
                column: "BearingOriginPointId");

            migrationBuilder.CreateIndex(
                name: "IX_PolyArcAreas_DefiningLineId",
                table: "PolyArcAreas",
                column: "DefiningLineId");

            migrationBuilder.CreateIndex(
                name: "IX_PolygonAreas_BoundingLineId",
                table: "PolygonAreas",
                column: "BoundingLineId");

            migrationBuilder.CreateIndex(
                name: "IX_RelativePoints_CoordinateSystemId",
                table: "RelativePoints",
                column: "CoordinateSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_SphereVolumes_CentrePointId",
                table: "SphereVolumes",
                column: "CentrePointId");

            migrationBuilder.CreateIndex(
                name: "IX_SurfaceVolumes_DefiningSurfaceId",
                table: "SurfaceVolumes",
                column: "DefiningSurfaceId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAreas_BeginPointId",
                table: "TrackAreas",
                column: "BeginPointId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAreas_EndPointId",
                table: "TrackAreas",
                column: "EndPointId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbsolutePoints");

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
                name: "ConeVolumes");

            migrationBuilder.DropTable(
                name: "CorridorAreas");

            migrationBuilder.DropTable(
                name: "Ellipses");

            migrationBuilder.DropTable(
                name: "FanAreas");

            migrationBuilder.DropTable(
                name: "LinePoints");

            migrationBuilder.DropTable(
                name: "OrbitAreas");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "PointReferences");

            migrationBuilder.DropTable(
                name: "PolyArcAreas");

            migrationBuilder.DropTable(
                name: "PolygonAreas");

            migrationBuilder.DropTable(
                name: "RelativePoints");

            migrationBuilder.DropTable(
                name: "SphereVolumes");

            migrationBuilder.DropTable(
                name: "SurfaceVolumes");

            migrationBuilder.DropTable(
                name: "TrackAreas");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Lines");

            migrationBuilder.DropTable(
                name: "CoordinateSystems");

            migrationBuilder.DropTable(
                name: "GeometricVolumes");

            migrationBuilder.DropTable(
                name: "Points");

            migrationBuilder.DropTable(
                name: "Surfaces");

            migrationBuilder.DropTable(
                name: "Organisations");

            migrationBuilder.DropTable(
                name: "VerticalDistances");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "ObjectItems");
        }
    }
}
