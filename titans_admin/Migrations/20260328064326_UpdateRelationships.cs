using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace titans_admin.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComplianceRecords_Users_ReviewedByUserId",
                table: "ComplianceRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRecords_Users_ReviewedByUserId",
                table: "ComplianceRecords",
                column: "ReviewedByUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComplianceRecords_Users_ReviewedByUserId",
                table: "ComplianceRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_ComplianceRecords_Users_ReviewedByUserId",
                table: "ComplianceRecords",
                column: "ReviewedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
