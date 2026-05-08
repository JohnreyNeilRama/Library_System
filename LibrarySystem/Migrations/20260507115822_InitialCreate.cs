using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibrarySystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Course = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    YearLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MembershipTier = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PublishedYear = table.Column<int>(type: "int", nullable: false),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvailableCopies = table.Column<int>(type: "int", nullable: false),
                    TotalCopies = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BorrowTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    BorrowDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorrowTransactions_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BorrowTransactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    ReservationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { "data-science", "Data science, machine learning, and AI", "Data Science" },
                    { "database", "Database management and SQL books", "Database" },
                    { "devops", "DevOps and site reliability engineering", "DevOps" },
                    { "mobile-development", "Mobile app development for iOS and Android", "Mobile Development" },
                    { "networking", "Computer networking and protocols", "Networking" },
                    { "programming", "Books on programming languages and software development", "Programming" },
                    { "security", "Cybersecurity and ethical hacking", "Security" },
                    { "software-engineering", "Software engineering principles and practices", "Software Engineering" },
                    { "web-development", "Web design and web application development", "Web Development" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Course", "CreatedAt", "Email", "FullName", "IdNumber", "IsActive", "IsAdmin", "IsVerified", "MembershipTier", "PasswordHash", "ProfilePictureUrl", "UserName", "YearLevel" },
                values: new object[,]
                {
                    { "admin-seed-id-001", "N/A", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@library.com", "System Administrator", "ADMIN001", true, true, true, "Admin", "$2b$11$5Nv71ylXSG5plFrlGUwd/.8xkefOOMhcEQ/qHhiVnG9WuoQ72vBdu", "https://ui-avatars.com/api/?name=Admin&background=dc3545&color=fff&size=80", "admin", "N/A" },
                    { "student-seed-id-001", "Computer Science", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "john.doe@university.edu", "John Doe", "20210001", true, false, true, "Gold Member", "$2b$11$3kmO0B3kYPvG1QHXrRmKsel64iSCPAKO5UCtdWHlehQIReJp4NULe", "https://ui-avatars.com/api/?name=John+Doe&background=0d6efd&color=fff&size=80", "johndoe", "3rd Year" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "AvailableCopies", "CategoryId", "CoverImageUrl", "Description", "ISBN", "PublishedYear", "Title", "TotalCopies" },
                values: new object[,]
                {
                    { 1, "Robert C. Martin", 5, "programming", null, "A handbook of agile software craftsmanship", "978-0132350884", 2008, "Clean Code", 5 },
                    { 2, "Andrew Hunt", 3, "programming", null, "Your journey to mastery", "978-0201616224", 1999, "The Pragmatic Programmer", 3 },
                    { 3, "Steve McConnell", 2, "programming", null, "A practical handbook of software construction", "978-0735619678", 2004, "Code Complete", 2 },
                    { 4, "Douglas Crockford", 8, "programming", null, "Uncovering the beauty of JavaScript", "978-0596517748", 2008, "JavaScript: The Good Parts", 8 },
                    { 5, "Eric Freeman", 4, "programming", null, "A brain-friendly guide", "978-0596007126", 2004, "Head First Design Patterns", 4 },
                    { 6, "Thomas H. Cormen", 0, "programming", null, "The comprehensive algorithms textbook", "978-0262033848", 2009, "Introduction to Algorithms", 0 },
                    { 7, "Ben Forta", 3, "database", null, "Sams teach yourself SQL in 10 minutes", "978-0672336079", 2020, "SQL in 10 Minutes", 3 },
                    { 8, "Abraham Silberschatz", 2, "database", null, "The definitive database textbook", "978-0073523325", 2019, "Database System Concepts", 2 },
                    { 9, "Pramod J. Sadalage", 1, "database", null, "A brief guide to the emerging world of polyglot persistence", "978-0321826626", 2012, "NoSQL Distilled", 1 },
                    { 10, "Andrew S. Tanenbaum", 4, "networking", null, "A top-down approach", "978-0133594140", 2020, "Computer Networking", 4 },
                    { 11, "W. Richard Stevens", 2, "networking", null, "The protocols", "978-0201633467", 1994, "TCP/IP Illustrated", 2 },
                    { 12, "Jon Duckett", 6, "web-development", null, "A visual guide to HTML and CSS", "978-1118008188", 2011, "HTML & CSS: Design and Build Websites", 6 },
                    { 13, "Stoyan Stefanov", 0, "web-development", null, "Building web applications", "978-1491931824", 2016, "React: Up and Running", 0 },
                    { 14, "Daniel Khalil", 3, "web-development", null, "Over 80 practical recipes", "978-1788996129", 2020, "Vue.js 3 Cookbook", 3 },
                    { 15, "Bill Phillips", 2, "mobile-development", null, "The big nerd ranch guide", "978-0135245125", 2019, "Android Programming: The Big Nerd Ranch Guide", 2 },
                    { 16, "Christian Keur", 1, "mobile-development", null, "The big nerd ranch guide", "978-0135594533", 2020, "iOS Programming: The Big Nerd Ranch Guide", 1 },
                    { 17, "Gene Kim", 4, "devops", null, "A novel about IT, DevOps, and helping your business win", "978-0988262506", 2013, "The Phoenix Project", 4 },
                    { 18, "Google SRE Team", 2, "devops", null, "How Google runs production systems", "978-1491929124", 2016, "Site Reliability Engineering", 2 },
                    { 19, "Aurelien Geron", 3, "data-science", null, "Scikit-Learn, Keras, and TensorFlow", "978-1492032632", 2019, "Hands-On Machine Learning", 3 },
                    { 20, "Joel Grus", 0, "data-science", null, "First principles with Python", "978-1492041139", 2019, "Data Science from Scratch", 0 },
                    { 21, "Dafydd Stuttard", 2, "security", null, "Finding and exploiting security flaws", "978-1118026470", 2011, "The Web Application Hacker's Handbook", 2 },
                    { 22, "Justin Seitz", 1, "security", null, "Python programming for hackers and pentesters", "978-1593275902", 2014, "Black Hat Python", 1 },
                    { 23, "Erich Gamma et al.", 5, "software-engineering", null, "Elements of reusable object-oriented software", "978-0201633610", 1994, "Design Patterns", 5 },
                    { 24, "Martin Fowler", 3, "software-engineering", null, "Improving the design of existing code", "978-0201485677", 1999, "Refactoring", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryId",
                table: "Books",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowTransactions_BookId",
                table: "BorrowTransactions",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowTransactions_UserId",
                table: "BorrowTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_BookId",
                table: "Reservations",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowTransactions");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
