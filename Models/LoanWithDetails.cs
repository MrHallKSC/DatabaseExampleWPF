using System;

namespace DatabaseExampleWPF.Models
{
    /// <summary>
    /// View Model class for displaying Loan data with joined information from related tables.
    /// This class demonstrates an important concept for AQA 7517 NEA:
    /// - The difference between database entities (Loan, Book, Member) and view models
    /// - How to structure data from JOIN queries
    /// - Flattening related data for display purposes
    /// 
    /// This class is NOT directly mapped to a database table.
    /// Instead, it combines data from multiple tables (Loans, Books, Members)
    /// that have been joined together in a SELECT query.
    /// 
    /// Example SQL that would populate this class:
    /// SELECT 
    ///     Loans.LoanID, Loans.LoanDate, Loans.DueDate, Loans.ReturnDate,
    ///     Books.Title, Books.ISBN,
    ///     Members.FirstName, Members.LastName, Members.Email
    /// FROM Loans
    /// INNER JOIN Books ON Loans.BookID = Books.BookID
    /// INNER JOIN Members ON Loans.MemberID = Members.MemberID
    /// </summary>
    public class LoanWithDetails
    {
        #region Loan Properties

        /// <summary>
        /// The unique loan ID from the Loans table
        /// </summary>
        public int LoanID { get; set; }

        /// <summary>
        /// Foreign key to Books table
        /// Included so we can identify which book if needed
        /// </summary>
        public int BookID { get; set; }

        /// <summary>
        /// Foreign key to Members table
        /// Included so we can identify which member if needed
        /// </summary>
        public int MemberID { get; set; }

        /// <summary>
        /// Date the book was borrowed
        /// </summary>
        public DateTime LoanDate { get; set; }

        /// <summary>
        /// Date the book is due to be returned
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Date the book was actually returned (null if still on loan)
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        #endregion

        #region Book Properties (from JOIN)

        /// <summary>
        /// Title of the borrowed book
        /// Comes from the Books table via JOIN
        /// </summary>
        public string BookTitle { get; set; }

        /// <summary>
        /// ISBN of the borrowed book
        /// Comes from the Books table via JOIN
        /// </summary>
        public string BookISBN { get; set; }

        #endregion

        #region Member Properties (from JOIN)

        /// <summary>
        /// First name of the member who borrowed the book
        /// Comes from the Members table via JOIN
        /// </summary>
        public string MemberFirstName { get; set; }

        /// <summary>
        /// Last name of the member who borrowed the book
        /// Comes from the Members table via JOIN
        /// </summary>
        public string MemberLastName { get; set; }

        /// <summary>
        /// Email of the member who borrowed the book
        /// Comes from the Members table via JOIN
        /// Useful for contacting members about overdue books
        /// </summary>
        public string MemberEmail { get; set; }

        /// <summary>
        /// Type of member (Student, Teacher, Staff)
        /// Comes from the Members table via JOIN
        /// </summary>
        public string MemberType { get; set; }

        #endregion

        #region Computed Properties

        /// <summary>
        /// Full name of the member (computed from first and last name)
        /// Not stored in database - calculated for display
        /// </summary>
        public string MemberFullName
        {
            get { return $"{MemberFirstName} {MemberLastName}"; }
        }

        /// <summary>
        /// Checks if the book has been returned
        /// </summary>
        public bool IsReturned
        {
            get { return ReturnDate.HasValue; }
        }

        /// <summary>
        /// Checks if the loan is overdue
        /// A loan is overdue if not returned and past due date
        /// </summary>
        public bool IsOverdue
        {
            get
            {
                if (IsReturned)
                    return false;

                return DateTime.Today > DueDate;
            }
        }

        /// <summary>
        /// Gets the status of the loan as a string for display
        /// </summary>
        public string Status
        {
            get
            {
                if (IsReturned)
                    return "Returned";
                else if (IsOverdue)
                    return "OVERDUE";
                else
                    return "On Loan";
            }
        }

        /// <summary>
        /// Number of days until due (negative if overdue)
        /// Only relevant for books not yet returned
        /// </summary>
        public int DaysUntilDue
        {
            get
            {
                if (IsReturned)
                    return 0;

                return (DueDate - DateTime.Today).Days;
            }
        }

        /// <summary>
        /// User-friendly message about the loan status
        /// Useful for displaying in the UI
        /// </summary>
        public string StatusMessage
        {
            get
            {
                if (IsReturned)
                {
                    return $"Returned on {ReturnDate:dd/MM/yyyy}";
                }
                else if (IsOverdue)
                {
                    int daysOverdue = Math.Abs(DaysUntilDue);
                    return $"OVERDUE by {daysOverdue} day(s)";
                }
                else
                {
                    return $"Due in {DaysUntilDue} day(s)";
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor - creates an empty LoanWithDetails object
        /// </summary>
        public LoanWithDetails()
        {
            LoanID = 0;
            BookID = 0;
            MemberID = 0;
            LoanDate = DateTime.Today;
            DueDate = DateTime.Today.AddDays(14);
            ReturnDate = null;
            BookTitle = string.Empty;
            BookISBN = string.Empty;
            MemberFirstName = string.Empty;
            MemberLastName = string.Empty;
            MemberEmail = string.Empty;
            MemberType = string.Empty;
        }

        /// <summary>
        /// Parameterized constructor - creates a LoanWithDetails with all values
        /// This would typically be used when reading from a JOIN query result
        /// </summary>
        public LoanWithDetails(int loanId, int bookId, int memberId,
            DateTime loanDate, DateTime dueDate, DateTime? returnDate,
            string bookTitle, string bookISBN,
            string memberFirstName, string memberLastName, string memberEmail, string memberType)
        {
            LoanID = loanId;
            BookID = bookId;
            MemberID = memberId;
            LoanDate = loanDate;
            DueDate = dueDate;
            ReturnDate = returnDate;
            BookTitle = bookTitle;
            BookISBN = bookISBN;
            MemberFirstName = memberFirstName;
            MemberLastName = memberLastName;
            MemberEmail = memberEmail;
            MemberType = memberType;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Override ToString for easier debugging and display
        /// Shows comprehensive loan information
        /// </summary>
        /// <returns>String representation of the loan with details</returns>
        public override string ToString()
        {
            return $"'{BookTitle}' borrowed by {MemberFullName} - {Status}";
        }

        #endregion
    }
}