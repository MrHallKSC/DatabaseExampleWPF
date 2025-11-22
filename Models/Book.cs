using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseExampleWPF.Models
{
    /// <summary>
    /// Represents a Book entity in the library system.
    /// This class demonstrates OOP principles for AQA 7517 NEA:
    /// - Encapsulation: Private fields with public properties
    /// - Data validation in property setters
    /// - Clear separation between data model and database operations
    /// 
    /// This class is used to store book data when working with it in memory,
    /// separate from how it's stored in the database.
    /// </summary>
    public class Book
    {
        #region Properties

        /// <summary>
        /// Unique identifier for the book (Primary Key in database)
        /// Set to 0 for new books (database will auto-generate)
        /// </summary>
        public int BookID { get; set; }

        /// <summary>
        /// Title of the book
        /// Required field - should not be null or empty
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// International Standard Book Number
        /// Format: Can be ISBN-10 or ISBN-13
        /// Optional field
        /// </summary>
        public string ISBN { get; set; }

        /// <summary>
        /// Year the book was published
        /// Should be validated to be reasonable (e.g., not in the future)
        /// </summary>
        public int YearPublished { get; set; }

        /// <summary>
        /// List of authors associated with this book
        /// Used when displaying book information with author details
        /// This is NOT stored directly in the Books table - it comes from a JOIN query
        /// </summary>
        public List<Author> Authors { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor - creates an empty book object
        /// BookID defaults to 0 (indicates a new book not yet in database)
        /// Authors list is initialized to prevent null reference errors
        /// </summary>
        public Book()
        {
            BookID = 0;
            Title = string.Empty;
            ISBN = string.Empty;
            YearPublished = DateTime.Now.Year;
            Authors = new List<Author>();
        }

        /// <summary>
        /// Parameterized constructor - creates a book with specified values
        /// Useful when reading data from the database
        /// </summary>
        /// <param name="bookId">Book ID (0 for new books)</param>
        /// <param name="title">Book title</param>
        /// <param name="isbn">ISBN number</param>
        /// <param name="yearPublished">Publication year</param>
        public Book(int bookId, string title, string isbn, int yearPublished)
        {
            BookID = bookId;
            Title = title;
            ISBN = isbn;
            YearPublished = yearPublished;
            Authors = new List<Author>();
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates that the book object has valid data before saving to database
        /// This demonstrates input validation for NEA projects
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            // Title is required
            if (string.IsNullOrWhiteSpace(Title))
            {
                return false;
            }

            // Year should be reasonable (between 1000 and current year + 1)
            // Adding 1 to current year allows for upcoming publications
            if (YearPublished < 1000 || YearPublished > DateTime.Now.Year + 1)
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

            if (string.IsNullOrWhiteSpace(Title))
            {
                errors.Add("Title is required");
            }

            if (YearPublished < 1000 || YearPublished > DateTime.Now.Year + 1)
            {
                errors.Add($"Year must be between 1000 and {DateTime.Now.Year + 1}");
            }

            return string.Join(", ", errors);
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Override ToString for easier debugging and display
        /// Shows book information in a readable format
        /// </summary>
        /// <returns>String representation of the book</returns>
        public override string ToString()
        {
            return $"{Title} ({YearPublished}) - ISBN: {ISBN}";
        }

        #endregion
    }
}
