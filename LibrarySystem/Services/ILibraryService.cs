/**
 * ILibraryService.cs
 * 
 * Defines the contract for the Library System's core business logic.
 * This interface decouples the controllers from the underlying data storage implementation.
 * 
 * This is the updated version of the file.
 */

namespace LibrarySystem.Services
{
    public interface ILibraryService
    {
        // User Operations
        InMemoryUser? GetUserById(string userId);
        InMemoryUser? GetUserByEmail(string email);
        InMemoryUser? GetUserByUserName(string userName);
        bool CreateUser(InMemoryUser user);
        bool ValidateUser(string userNameOrEmail, string password);
        List<InMemoryUser> GetAllUsers();
        bool UpdateUser(InMemoryUser user);
        bool ToggleUserActiveStatus(string userId);
        bool DeleteUser(string userId);

        // Book Operations
        InMemoryBook? GetBookById(int bookId);
        List<InMemoryBook> GetAllBooks();
        List<InMemoryBook> GetBooksByCategory(string categoryId);
        bool AddBook(InMemoryBook book);
        bool UpdateBook(InMemoryBook book);
        bool DeleteBook(int bookId);

        // Category Operations
        List<InMemoryCategory> GetAllCategories();
        InMemoryCategory? GetCategoryById(string categoryId);
        bool AddCategory(InMemoryCategory category);
        bool UpdateCategory(InMemoryCategory category);
        bool DeleteCategory(string categoryId);

        // Transaction Operations
        InMemoryBorrowTransaction? BorrowBook(string userId, int bookId, DateTime? requestedEndDate = null, int borrowDays = 14);
        bool ApproveBorrowRequest(int transactionId, int borrowDays = 14);
        bool DeclineBorrowRequest(int transactionId);
        bool ClaimBorrowRequest(int transactionId, int borrowDays = 14);
        bool ReturnBook(int transactionId);
        List<InMemoryBorrowTransaction> GetUserBorrowedBooks(string userId);
        List<InMemoryBorrowTransaction> GetUserReturnedBooks(string userId);
        List<InMemoryBorrowTransaction> GetAllTransactions();
        List<InMemoryBorrowTransaction> GetActiveBorrows();
        List<InMemoryBorrowTransaction> GetOverdueBooks();

        // Reservation Operations
        InMemoryReservation? ReserveBook(string userId, int bookId, int reserveDays = 7);
        bool ApproveReservation(int reservationId, int claimHours = 24);
        bool DeclineReservation(int reservationId);
        bool ClaimReservation(int reservationId);
        bool CancelReservation(int reservationId);
        List<InMemoryReservation> GetUserReservations(string userId);
        List<InMemoryReservation> GetAllReservations();
    }
}
