using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MCPHub.Data.Migrations; 
/// <inheritdoc />
public partial class InitialCreateWithIdentity : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.CreateTable(
            name: "AspNetRoles",
            columns: table => new {
                Id = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_AspNetRoles", x => x.Id));

        migrationBuilder.CreateTable(
            name: "AspNetUsers",
            columns: table => new {
                Id = table.Column<string>(type: "text", nullable: false),
                DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                GitHubUsername = table.Column<string>(type: "character varying(39)", maxLength: 39, nullable: true),
                TwitterHandle = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                Bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                AvatarUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                IsPublisher = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                PasswordHash = table.Column<string>(type: "text", nullable: true),
                SecurityStamp = table.Column<string>(type: "text", nullable: true),
                ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                PhoneNumber = table.Column<string>(type: "text", nullable: true),
                PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_AspNetUsers", x => x.Id));

        migrationBuilder.CreateTable(
            name: "AspNetRoleClaims",
            columns: table => new {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                RoleId = table.Column<string>(type: "text", nullable: false),
                ClaimType = table.Column<string>(type: "text", nullable: true),
                ClaimValue = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => {
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
            columns: table => new {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<string>(type: "text", nullable: false),
                ClaimType = table.Column<string>(type: "text", nullable: true),
                ClaimValue = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => {
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
            columns: table => new {
                LoginProvider = table.Column<string>(type: "text", nullable: false),
                ProviderKey = table.Column<string>(type: "text", nullable: false),
                ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                UserId = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => {
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
            columns: table => new {
                UserId = table.Column<string>(type: "text", nullable: false),
                RoleId = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => {
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
            columns: table => new {
                UserId = table.Column<string>(type: "text", nullable: false),
                LoginProvider = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                table.ForeignKey(
                    name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Publishers",
            columns: table => new {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "text", nullable: false),
                OrganizationName = table.Column<string>(type: "text", nullable: true),
                Website = table.Column<string>(type: "text", nullable: true),
                Verified = table.Column<bool>(type: "boolean", nullable: false),
                Type = table.Column<string>(type: "text", nullable: false),
                UserId = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_Publishers", x => x.Id);
                table.ForeignKey(
                    name: "FK_Publishers_AspNetUsers_UserId",
                    column: x => x.UserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "Packages",
            columns: table => new {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                PublisherId = table.Column<Guid>(type: "uuid", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false),
                Repository = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                License = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Tags = table.Column<string>(type: "jsonb", nullable: false),
                ScanResult_Status = table.Column<string>(type: "text", nullable: true),
                ScanResult_VulnerabilityCount = table.Column<int>(type: "integer", nullable: true),
                ScanResult_HighestSeverity = table.Column<string>(type: "text", nullable: true),
                ScanResult_Vulnerabilities = table.Column<string>(type: "jsonb", nullable: true),
                ScanResult_ScannedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ScanResult_ScannerVersion = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                ScanResult_ScanLog = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                TrustTier = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_Packages", x => x.Id);
                table.ForeignKey(
                    name: "FK_Packages_Publishers_PublisherId",
                    column: x => x.PublisherId,
                    principalTable: "Publishers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Servers",
            columns: table => new {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                PublisherId = table.Column<Guid>(type: "uuid", nullable: false),
                Repository = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                License = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Tags = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                TrustTier = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_Servers", x => x.Id);
                table.ForeignKey(
                    name: "FK_Servers_Publishers_PublisherId",
                    column: x => x.PublisherId,
                    principalTable: "Publishers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PackageVersions",
            columns: table => new {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                PackageId = table.Column<Guid>(type: "uuid", nullable: false),
                Version = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                ReleaseNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                DownloadUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                ChecksumSha256 = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                FileSize = table.Column<long>(type: "bigint", nullable: false),
                IsPrerelease = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                ScanResult_Status = table.Column<string>(type: "text", nullable: true),
                ScanResult_VulnerabilityCount = table.Column<int>(type: "integer", nullable: true),
                ScanResult_HighestSeverity = table.Column<string>(type: "text", nullable: true),
                ScanResult_Vulnerabilities = table.Column<string>(type: "jsonb", nullable: true),
                ScanResult_ScannedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ScanResult_ScannerVersion = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                ScanResult_ScanLog = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_PackageVersions", x => x.Id);
                table.ForeignKey(
                    name: "FK_PackageVersions_Packages_PackageId",
                    column: x => x.PackageId,
                    principalTable: "Packages",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ServerVersions",
            columns: table => new {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Version = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                ServerId = table.Column<Guid>(type: "uuid", nullable: false),
                ReleaseNotes = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                Status = table.Column<string>(type: "text", nullable: false),
                SecurityScan = table.Column<string>(type: "text", nullable: true),
                PackageUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                PackageSize = table.Column<long>(type: "bigint", nullable: true),
                Checksum = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_ServerVersions", x => x.Id);
                table.ForeignKey(
                    name: "FK_ServerVersions_Servers_ServerId",
                    column: x => x.ServerId,
                    principalTable: "Servers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "SecurityScans",
            columns: table => new {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                VersionId = table.Column<Guid>(type: "uuid", nullable: false),
                ServerVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                ScanType = table.Column<string>(type: "text", nullable: false),
                ScanStartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ScanCompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Status = table.Column<string>(type: "text", nullable: false),
                Result = table.Column<string>(type: "text", nullable: true),
                CriticalIssues = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                ErrorMessage = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                ScannerVersion = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Metadata = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedBy = table.Column<string>(type: "text", nullable: true),
                UpdatedBy = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_SecurityScans", x => x.Id);
                table.ForeignKey(
                    name: "FK_SecurityScans_ServerVersions_ServerVersionId",
                    column: x => x.ServerVersionId,
                    principalTable: "ServerVersions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

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
            name: "IX_AspNetUsers_CreatedAt",
            table: "AspNetUsers",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_GitHubUsername",
            table: "AspNetUsers",
            column: "GitHubUsername",
            unique: true,
            filter: "[GitHubUsername] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_IsPublisher",
            table: "AspNetUsers",
            column: "IsPublisher");

        migrationBuilder.CreateIndex(
            name: "IX_AspNetUsers_TwitterHandle",
            table: "AspNetUsers",
            column: "TwitterHandle",
            unique: true,
            filter: "[TwitterHandle] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "UserNameIndex",
            table: "AspNetUsers",
            column: "NormalizedUserName",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Packages_CreatedAt",
            table: "Packages",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_Packages_FullText",
            table: "Packages",
            columns: new[] { "Name", "Description" })
            .Annotation("Npgsql:IndexMethod", "gin");

        migrationBuilder.CreateIndex(
            name: "IX_Packages_Name",
            table: "Packages",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Packages_PublisherId",
            table: "Packages",
            column: "PublisherId");

        migrationBuilder.CreateIndex(
            name: "IX_Packages_Status",
            table: "Packages",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_Packages_TrustTier",
            table: "Packages",
            column: "TrustTier");

        migrationBuilder.CreateIndex(
            name: "IX_PackageVersions_CreatedAt",
            table: "PackageVersions",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_PackageVersions_PackageId",
            table: "PackageVersions",
            column: "PackageId");

        migrationBuilder.CreateIndex(
            name: "IX_PackageVersions_PackageId_Version",
            table: "PackageVersions",
            columns: new[] { "PackageId", "Version" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_PackageVersions_Version",
            table: "PackageVersions",
            column: "Version");

        migrationBuilder.CreateIndex(
            name: "IX_Publishers_CreatedAt",
            table: "Publishers",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_Publishers_Name",
            table: "Publishers",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Publishers_Type",
            table: "Publishers",
            column: "Type");

        migrationBuilder.CreateIndex(
            name: "IX_Publishers_UserId",
            table: "Publishers",
            column: "UserId");

        migrationBuilder.CreateIndex(
            name: "IX_SecurityScans_CriticalIssues",
            table: "SecurityScans",
            column: "CriticalIssues");

        migrationBuilder.CreateIndex(
            name: "IX_SecurityScans_ScanCompletedAt",
            table: "SecurityScans",
            column: "ScanCompletedAt");

        migrationBuilder.CreateIndex(
            name: "IX_SecurityScans_ScanStartedAt",
            table: "SecurityScans",
            column: "ScanStartedAt");

        migrationBuilder.CreateIndex(
            name: "IX_SecurityScans_ScanType",
            table: "SecurityScans",
            column: "ScanType");

        migrationBuilder.CreateIndex(
            name: "IX_SecurityScans_ServerVersionId",
            table: "SecurityScans",
            column: "ServerVersionId");

        migrationBuilder.CreateIndex(
            name: "IX_SecurityScans_Status",
            table: "SecurityScans",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_SecurityScans_VersionId",
            table: "SecurityScans",
            column: "VersionId");

        migrationBuilder.CreateIndex(
            name: "IX_SecurityScans_VersionId_ScanType",
            table: "SecurityScans",
            columns: new[] { "VersionId", "ScanType" });

        migrationBuilder.CreateIndex(
            name: "IX_Servers_CreatedAt",
            table: "Servers",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_Servers_Name",
            table: "Servers",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Servers_PublisherId",
            table: "Servers",
            column: "PublisherId");

        migrationBuilder.CreateIndex(
            name: "IX_Servers_Status",
            table: "Servers",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_Servers_TrustTier",
            table: "Servers",
            column: "TrustTier");

        migrationBuilder.CreateIndex(
            name: "IX_ServerVersions_CreatedAt",
            table: "ServerVersions",
            column: "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_ServerVersions_ServerId",
            table: "ServerVersions",
            column: "ServerId");

        migrationBuilder.CreateIndex(
            name: "IX_ServerVersions_ServerId_Version",
            table: "ServerVersions",
            columns: new[] { "ServerId", "Version" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_ServerVersions_Status",
            table: "ServerVersions",
            column: "Status");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
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
            name: "PackageVersions");

        migrationBuilder.DropTable(
            name: "SecurityScans");

        migrationBuilder.DropTable(
            name: "AspNetRoles");

        migrationBuilder.DropTable(
            name: "Packages");

        migrationBuilder.DropTable(
            name: "ServerVersions");

        migrationBuilder.DropTable(
            name: "Servers");

        migrationBuilder.DropTable(
            name: "Publishers");

        migrationBuilder.DropTable(
            name: "AspNetUsers");
    }
}