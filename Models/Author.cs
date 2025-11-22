using System;
using System.Collections.Generic;

namespace DatabaseExampleWPF.Models
{
    /// <summary>
    /// Represents an Author entity in the library system.
    /// This class demonstrates OOP principles for AQA 7517 NEA:
    /// - Encapsulation with properties
    /// - Data validation
    /// - Relationship to Book entities through many-to-many relationship
    /// 
    /// Authors can write multiple books, and books can have multiple authors.
    /// This is managed through the BookAuthors join table in the database.
    /// </summary>
    public class Author
    {
        #region Properties

        /// <summary>
        /// Unique identifier for the author (Primary Key in database)
        /// Set to 0 for new authors (database will auto-generate)
        /// </summary>
        public int AuthorID { get; set; }

        /// <summary>
        /// Author's first name
        /// Required field
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Author's last name
        /// Required field
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Full name of the author (computed property)
        /// This is not stored in the database - it's calculated when needed
        /// Demonstrates the difference between stored data and computed properties
        /// </summary>
        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        /// <summary>
        /// List of books written by this author
        /// Populated from JOIN queries when needed
        /// This is NOT stored in the Authors table directly
        /// </summary>
        public List<Book> Books { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor - creates an empty author object
        /// AuthorID defaults to 0 (indicates new author not yet in database)
        /// Books list is initialized to prevent null reference errors
        /// </summary>
        public Author()
        {
            AuthorID = 0;
            FirstName = string.Empty;
            LastName = string.Empty;
            Books = new List<Book>();
        }

        /// <summary>
        /// Parameterized constructor - creates an author with specified values
        /// Useful when reading data from the database
        /// </summary>
        /// <param name="authorId">Author ID (0 for new authors)</param>
        /// <param name="firstName">Author's first name</param>
        /// <param name="lastName">Author's last name</param>
        public Author(int authorId, string firstName, string lastName)
        {
            AuthorID = authorId;
            FirstName = firstName;
            LastName = lastName;
            Books = new List<Book>();
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates that the author object has valid data before saving to database
        /// This demonstrates input validation for NEA projects
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            // First name is required
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                return false;
            }

            // Last name is required
            if (string.IsNullOrWhiteSpace(LastName))
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

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                errors.Add("First name is required");
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                errors.Add("Last name is required");
            }

            return string.Join(", ", errors);
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Override ToString for easier debugging and display
        /// Shows author's full name
        /// </summary>
        /// <returns>String representation of the author</returns>
        public override string ToString()
        {
            return FullName;
        }

        #endregion
    }
}
