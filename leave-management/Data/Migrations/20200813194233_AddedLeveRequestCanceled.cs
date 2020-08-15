using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace leave_management.Data.Migrations
{
    public partial class AddedLeveRequestCanceled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateActioned",
                table: "LeaveRequests",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "Canceled",
                table: "LeaveRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "LeaveRequestVM",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestingEmployeeId = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    LeaveTypeId = table.Column<int>(nullable: false),
                    DateRequested = table.Column<DateTime>(nullable: false),
                    DateActioned = table.Column<DateTime>(nullable: false),
                    Approved = table.Column<bool>(nullable: true),
                    ApprovedById = table.Column<string>(nullable: true),
                    Cancelled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveRequestVM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeaveRequestVM_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LeaveRequestVM_LeaveTypes_LeaveTypeId",
                        column: x => x.LeaveTypeId,
                        principalTable: "LeaveTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeaveRequestVM_AspNetUsers_RequestingEmployeeId",
                        column: x => x.RequestingEmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequestVM_ApprovedById",
                table: "LeaveRequestVM",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequestVM_LeaveTypeId",
                table: "LeaveRequestVM",
                column: "LeaveTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequestVM_RequestingEmployeeId",
                table: "LeaveRequestVM",
                column: "RequestingEmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaveRequestVM");

            migrationBuilder.DropColumn(
                name: "Canceled",
                table: "LeaveRequests");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateActioned",
                table: "LeaveRequests",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
