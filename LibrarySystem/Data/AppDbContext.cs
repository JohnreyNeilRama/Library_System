using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using LibrarySystem.Models;

namespace LibrarySystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BorrowTransaction> BorrowTransactions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── Unique indexes ──────────────────────────────────────────────
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName).IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();

            // ── Relationships ───────────────────────────────────────────────
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BorrowTransaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.BorrowTransactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BorrowTransaction>()
                .HasOne(t => t.Book)
                .WithMany(b => b.BorrowTransactions)
                .HasForeignKey(t => t.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reservations)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Seed Data ───────────────────────────────────────────────────
            SeedCategories(modelBuilder);
            SeedBooks(modelBuilder);
            SeedUsers(modelBuilder);
        }

        private static void SeedCategories(ModelBuilder m)
        {
            m.Entity<Category>().HasData(
                new Category { Id = "programming", Name = "Programming", Description = "Books on programming languages and software development" },
                new Category { Id = "database", Name = "Database", Description = "Database management and SQL books" },
                new Category { Id = "networking", Name = "Networking", Description = "Computer networking and protocols" },
                new Category { Id = "web-development", Name = "Web Development", Description = "Web design and web application development" },
                new Category { Id = "mobile-development", Name = "Mobile Development", Description = "Mobile app development for iOS and Android" },
                new Category { Id = "devops", Name = "DevOps", Description = "DevOps and site reliability engineering" },
                new Category { Id = "data-science", Name = "Data Science", Description = "Data science, machine learning, and AI" },
                new Category { Id = "security", Name = "Security", Description = "Cybersecurity and ethical hacking" },
                new Category { Id = "software-engineering", Name = "Software Engineering", Description = "Software engineering principles and practices" }
            );
        }

        private static void SeedBooks(ModelBuilder m)
        {
            m.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Clean Code", Author = "Robert C. Martin", Description = "A handbook of agile software craftsmanship", ISBN = "978-0132350884", PublishedYear = 2008, AvailableCopies = 5, TotalCopies = 5, CategoryId = "programming" },
                new Book { Id = 2, Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Description = "Your journey to mastery", ISBN = "978-0201616224", PublishedYear = 1999, AvailableCopies = 3, TotalCopies = 3, CategoryId = "programming" },
                new Book { Id = 3, Title = "Code Complete", Author = "Steve McConnell", Description = "A practical handbook of software construction", ISBN = "978-0735619678", PublishedYear = 2004, AvailableCopies = 2, TotalCopies = 2, CategoryId = "programming" },
                new Book { Id = 4, Title = "JavaScript: The Good Parts", Author = "Douglas Crockford", Description = "Uncovering the beauty of JavaScript", ISBN = "978-0596517748", PublishedYear = 2008, AvailableCopies = 8, TotalCopies = 8, CategoryId = "programming" },
                new Book { Id = 5, Title = "Head First Design Patterns", Author = "Eric Freeman", Description = "A brain-friendly guide", ISBN = "978-0596007126", PublishedYear = 2004, AvailableCopies = 4, TotalCopies = 4, CategoryId = "programming" },
                new Book { Id = 6, Title = "Introduction to Algorithms", Author = "Thomas H. Cormen", Description = "The comprehensive algorithms textbook", ISBN = "978-0262033848", PublishedYear = 2009, AvailableCopies = 0, TotalCopies = 0, CategoryId = "programming" },
                new Book { Id = 7, Title = "SQL in 10 Minutes", Author = "Ben Forta", Description = "Sams teach yourself SQL in 10 minutes", ISBN = "978-0672336079", PublishedYear = 2020, AvailableCopies = 3, TotalCopies = 3, CategoryId = "database" },
                new Book { Id = 8, Title = "Database System Concepts", Author = "Abraham Silberschatz", Description = "The definitive database textbook", ISBN = "978-0073523325", PublishedYear = 2019, AvailableCopies = 2, TotalCopies = 2, CategoryId = "database" },
                new Book { Id = 9, Title = "NoSQL Distilled", Author = "Pramod J. Sadalage", Description = "A brief guide to the emerging world of polyglot persistence", ISBN = "978-0321826626", PublishedYear = 2012, AvailableCopies = 1, TotalCopies = 1, CategoryId = "database" },
                new Book { Id = 10, Title = "Computer Networking", Author = "Andrew S. Tanenbaum", Description = "A top-down approach", ISBN = "978-0133594140", PublishedYear = 2020, AvailableCopies = 4, TotalCopies = 4, CategoryId = "networking" },
                new Book { Id = 11, Title = "TCP/IP Illustrated", Author = "W. Richard Stevens", Description = "The protocols", ISBN = "978-0201633467", PublishedYear = 1994, AvailableCopies = 2, TotalCopies = 2, CategoryId = "networking" },
                new Book { Id = 12, Title = "HTML & CSS: Design and Build Websites", Author = "Jon Duckett", Description = "A visual guide to HTML and CSS", ISBN = "978-1118008188", PublishedYear = 2011, AvailableCopies = 6, TotalCopies = 6, CategoryId = "web-development" },
                new Book { Id = 13, Title = "React: Up and Running", Author = "Stoyan Stefanov", Description = "Building web applications", ISBN = "978-1491931824", PublishedYear = 2016, AvailableCopies = 0, TotalCopies = 0, CategoryId = "web-development" },
                new Book { Id = 14, Title = "Vue.js 3 Cookbook", Author = "Daniel Khalil", Description = "Over 80 practical recipes", ISBN = "978-1788996129", PublishedYear = 2020, AvailableCopies = 3, TotalCopies = 3, CategoryId = "web-development" },
                new Book { Id = 15, Title = "Android Programming: The Big Nerd Ranch Guide", Author = "Bill Phillips", Description = "The big nerd ranch guide", ISBN = "978-0135245125", PublishedYear = 2019, AvailableCopies = 2, TotalCopies = 2, CategoryId = "mobile-development" },
                new Book { Id = 16, Title = "iOS Programming: The Big Nerd Ranch Guide", Author = "Christian Keur", Description = "The big nerd ranch guide", ISBN = "978-0135594533", PublishedYear = 2020, AvailableCopies = 1, TotalCopies = 1, CategoryId = "mobile-development" },
                new Book { Id = 17, Title = "The Phoenix Project", Author = "Gene Kim", Description = "A novel about IT, DevOps, and helping your business win", ISBN = "978-0988262506", PublishedYear = 2013, AvailableCopies = 4, TotalCopies = 4, CategoryId = "devops" },
                new Book { Id = 18, Title = "Site Reliability Engineering", Author = "Google SRE Team", Description = "How Google runs production systems", ISBN = "978-1491929124", PublishedYear = 2016, AvailableCopies = 2, TotalCopies = 2, CategoryId = "devops" },
                new Book { Id = 19, Title = "Hands-On Machine Learning", Author = "Aurelien Geron", Description = "Scikit-Learn, Keras, and TensorFlow", ISBN = "978-1492032632", PublishedYear = 2019, AvailableCopies = 3, TotalCopies = 3, CategoryId = "data-science" },
                new Book { Id = 20, Title = "Data Science from Scratch", Author = "Joel Grus", Description = "First principles with Python", ISBN = "978-1492041139", PublishedYear = 2019, AvailableCopies = 0, TotalCopies = 0, CategoryId = "data-science" },
                new Book { Id = 21, Title = "The Web Application Hacker's Handbook", Author = "Dafydd Stuttard", Description = "Finding and exploiting security flaws", ISBN = "978-1118026470", PublishedYear = 2011, AvailableCopies = 2, TotalCopies = 2, CategoryId = "security" },
                new Book { Id = 22, Title = "Black Hat Python", Author = "Justin Seitz", Description = "Python programming for hackers and pentesters", ISBN = "978-1593275902", PublishedYear = 2014, AvailableCopies = 1, TotalCopies = 1, CategoryId = "security" },
                new Book { Id = 23, Title = "Design Patterns", Author = "Erich Gamma et al.", Description = "Elements of reusable object-oriented software", ISBN = "978-0201633610", PublishedYear = 1994, AvailableCopies = 5, TotalCopies = 5, CategoryId = "software-engineering" },
                new Book { Id = 24, Title = "Refactoring", Author = "Martin Fowler", Description = "Improving the design of existing code", ISBN = "978-0201485677", PublishedYear = 1999, AvailableCopies = 3, TotalCopies = 3, CategoryId = "software-engineering" }
            );
        }

        private static void SeedUsers(ModelBuilder m)
        {
            // Pre-computed BCrypt hashes — static so EF Core doesn't detect changes
            // Admin123!
            const string adminHash = "$2b$11$5Nv71ylXSG5plFrlGUwd/.8xkefOOMhcEQ/qHhiVnG9WuoQ72vBdu";
            const string studentHash = "$2b$11$3kmO0B3kYPvG1QHXrRmKsel64iSCPAKO5UCtdWHlehQIReJp4NULe";

            m.Entity<User>().HasData(
                new User
                {
                    Id = "admin-seed-id-001",
                    UserName = "admin",
                    Email = "admin@library.com",
                    PasswordHash = adminHash,
                    FullName = "System Administrator",
                    IdNumber = "ADMIN001",
                    Course = "N/A",
                    YearLevel = "N/A",
                    MembershipTier = "Admin",
                    ProfilePictureUrl = "https://ui-avatars.com/api/?name=Admin&background=dc3545&color=fff&size=80",
                    IsAdmin = true,
                    IsVerified = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new User
                {
                    Id = "student-seed-id-001",
                    UserName = "johndoe",
                    Email = "john.doe@university.edu",
                    PasswordHash = studentHash,
                    FullName = "John Doe",
                    IdNumber = "20210001",
                    Course = "Computer Science",
                    YearLevel = "3rd Year",
                    MembershipTier = "Gold Member",
                    ProfilePictureUrl = "https://ui-avatars.com/api/?name=John+Doe&background=0d6efd&color=fff&size=80",
                    IsAdmin = false,
                    IsVerified = true,
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1)
                }
            );
        }
    }
}