namespace LibrarySystem.Services
{
    public interface ILibraryService
    {
        InMemoryUser? GetUserById(string userId);
        InMemoryUser? GetUserByEmail(string email);
        InMemoryUser? GetUserByUserName(string userName);
        bool CreateUser(InMemoryUser user);
        bool ValidateUser(string userNameOrEmail, string password);
        InMemoryBook? GetBookById(int bookId);
        List<InMemoryBook> GetAllBooks();
        List<InMemoryBook> GetBooksByCategory(string categoryId);
        bool AddBook(InMemoryBook book);
        bool UpdateBook(InMemoryBook book);
        bool DeleteBook(int bookId);
        List<InMemoryCategory> GetAllCategories();
        InMemoryCategory? GetCategoryById(string categoryId);
        bool AddCategory(InMemoryCategory category);
        bool UpdateCategory(InMemoryCategory category);
        bool DeleteCategory(string categoryId);
        InMemoryBorrowTransaction? BorrowBook(string userId, int bookId, int borrowDays = 14);
        bool ReturnBook(int transactionId);
        List<InMemoryBorrowTransaction> GetUserBorrowedBooks(string userId);
        List<InMemoryBorrowTransaction> GetUserReturnedBooks(string userId);
        List<InMemoryBorrowTransaction> GetAllTransactions();
        List<InMemoryBorrowTransaction> GetActiveBorrows();
        List<InMemoryBorrowTransaction> GetOverdueBooks();
        InMemoryReservation? ReserveBook(string userId, int bookId, int reserveDays = 7);
        bool CancelReservation(int reservationId);
        List<InMemoryReservation> GetUserReservations(string userId);
        List<InMemoryReservation> GetAllReservations();
        List<InMemoryUser> GetAllUsers();
        bool UpdateUser(InMemoryUser user);
        bool ToggleUserActiveStatus(string userId);
    }
}
