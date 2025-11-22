using System;
using System.Collections.Generic;

namespace DatabaseExampleWPF.Models
{
    /// <summary>
    /// Represents a Loan entity in the library system.
    /// This class demonstrates OOP principles for AQA 7517 NEA:
    /// - Encapsulation with properties
    /// - Working with dates and nullable types
    /// - Foreign key relationships (one-to-many)
    /// 
    /// A Loan represents a book being borrowed by a member.
    /// Demonstrates one-to-many relationships:
    /// - One Member can have many Loans
    /// - One Book can have many Loans (over time)
    /// </summary>
    public class Loan
    {
        #region Properties

        /// <summary>
        /// Unique identifier for the loan (Primary Key in database)
        /// Set to 0 for new loans (database will auto-generate)
        /// </summary>
        public int LoanID { get; set; }

        /// <summary>
        /// Foreign Key - references BookID in Books table
        /// Identifies which book was borrowed
        /// </summary>
        public int BookID { get; set; }

        /// <summary>
        /// Foreign Key - references MemberID in Members table
        /// Identifies which member borrowed the book
        /// </summary>
        public int MemberID { get; set; }

        /// <summary>
        /// Date the book was borrowed
        /// Required field - cannot be null
        /// Stored as TEXT in SQLite (ISO 8601 format: YYYY-MM-DD)
        /// </summary>
        public DateTime LoanDate { get; set; }

        /// <summary>
        /// Date the book is due to be returned
        /// Required field - cannot be null
        /// Should be after LoanDate
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Date the book was actually returned
        /// Optional field - NULL if book hasn't been returned yet
        /// Nullable DateTime indicated by DateTime? syntax
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Computed property - checks if the book has been returned
        /// True if ReturnDate has a value, False if it's null
        /// </summary>
        public bool IsReturned
        {
            get { return ReturnDate.HasValue; }
        }

        /// <summary>
        /// Computed property - checks if the loan is overdue
        /// A loan is overdue if:
        /// - The book hasn't been returned (ReturnDate is null) AND
        /// - Today's date is past the DueDate
        /// </summary>
        public bool IsOverdue
        {
            get
            {
                if (IsReturned)
                    return false; // Can't be overdue if already returned

                return DateTime.Today > DueDate;
            }
        }

        /// <summary>
        /// Number of days until due (negative if overdue)
        /// Useful for displaying urgency to users
        /// </summary>
        public int DaysUntilDue
        {
            get
            {
                if (IsReturned)
                    return 0; // Not relevant if already returned

                return (DueDate - DateTime.Today).Days;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor - creates an empty loan object
        /// LoanID defaults to 0 (indicates new loan not yet in database)
        /// LoanDate defaults to today
        /// DueDate defaults to 14 days from today (2 week loan period)
        /// ReturnDate defaults to null (not yet returned)
        /// </summary>
        public Loan()
        {
            LoanID = 0;
            BookID = 0;
            MemberID = 0;
            LoanDate = DateTime.Today;
            DueDate = DateTime.Today.AddDays(14); // Standard 2-week loan
            ReturnDate = null; // Not yet returned
        }

        /// <summary>
        /// Parameterized constructor - creates a loan with specified values
        /// Useful when reading data from the database
        /// </summary>
        /// <param name="loanId">Loan ID (0 for new loans)</param>
        /// <param name="bookId">ID of the book being borrowed</param>
        /// <param name="memberId">ID of the member borrowing the book</param>
        /// <param name="loanDate">Date the book was borrowed</param>
        /// <param name="dueDate">Date the book is due back</param>
        /// <param name="returnDate">Date the book was returned (null if not returned)</param>
        public Loan(int loanId, int bookId, int memberId, DateTime loanDate, DateTime dueDate, DateTime? returnDate)
        {
            LoanID = loanId;
            BookID = bookId;
            MemberID = memberId;
            LoanDate = loanDate;
            DueDate = dueDate;
            ReturnDate = returnDate;
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates that the loan object has valid data before saving to database
        /// This demonstrates date validation and logical business rules
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            // BookID must be greater than 0 (must reference a real book)
            if (BookID <= 0)
            {
                return false;
            }

            // MemberID must be greater than 0 (must reference a real member)
            if (MemberID <= 0)
            {
                return false;
            }

            // DueDate must be after LoanDate (can't be due before it's borrowed!)
            if (DueDate <= LoanDate)
            {
                return false;
            }

            // If ReturnDate is set, it must be on or after LoanDate
            // (can't return a book before you borrowed it!)
            if (ReturnDate.HasValue && ReturnDate.Value < LoanDate)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a detailed validation error message
        /// Useful for displaying to users what's wrong with their input
        /// </summary>
        /// <returns>Error message or empty string if valid</returns>
        public string GetValidationErrors()
        {
            List<string> errors = new List<string>();

            if (BookID <= 0)
            {
                errors.Add("Book must be selected");
            }

            if (MemberID <= 0)
            {
                errors.Add("Member must be selected");
            }

            if (DueDate <= LoanDate)
            {
                errors.Add("Due date must be after loan date");
            }

            if (ReturnDate.HasValue && ReturnDate.Value < LoanDate)
            {
                errors.Add("Return date cannot be before loan date");
            }

            return string.Join(", ", errors);
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Override ToString for easier debugging and display
        /// Shows loan status and dates
        /// </summary>
        /// <returns>String representation of the loan</returns>
        public override string ToString()
        {
            string status = IsReturned ? "Returned" : (IsOverdue ? "OVERDUE" : "On Loan");
            return $"Loan {LoanID}: {status} - Due: {DueDate:dd/MM/yyyy}";
        }

        #endregion
    }
}
