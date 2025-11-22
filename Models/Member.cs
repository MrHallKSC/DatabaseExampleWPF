using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DatabaseExampleWPF.Models
{
    /// <summary>
    /// Represents a Library Member entity in the library system.
    /// This class demonstrates OOP principles for AQA 7517 NEA:
    /// - Encapsulation with properties
    /// - Data validation including email format checking
    /// - Enumeration for member types
    /// 
    /// Members can borrow books from the library.
    /// This is tracked through the Loans table (one-to-many relationship).
    /// </summary>
    public class Member
    {
        #region Properties

        /// <summary>
        /// Unique identifier for the member (Primary Key in database)
        /// Set to 0 for new members (database will auto-generate)
        /// </summary>
        public int MemberID { get; set; }

        /// <summary>
        /// Member's first name
        /// Required field
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Member's last name
        /// Required field
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Member's email address
        /// Required field - should be validated for correct format
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Type of membership (Student, Teacher, Staff)
        /// Stored as TEXT in the database
        /// Different member types might have different borrowing privileges
        /// </summary>
        public string MemberType { get; set; }

        /// <summary>
        /// Full name of the member (computed property)
        /// Not stored in database - calculated when needed
        /// </summary>
        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        /// <summary>
        /// List of loans for this member
        /// Populated from JOIN queries when needed
        /// This demonstrates the one-to-many relationship
        /// </summary>
        public List<Loan> Loans { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor - creates an empty member object
        /// MemberID defaults to 0 (indicates new member not yet in database)
        /// MemberType defaults to "Student"
        /// Loans list is initialized to prevent null reference errors
        /// </summary>
        public Member()
        {
            MemberID = 0;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            MemberType = "Student";
            Loans = new List<Loan>();
        }

        /// <summary>
        /// Parameterized constructor - creates a member with specified values
        /// Useful when reading data from the database
        /// </summary>
        /// <param name="memberId">Member ID (0 for new members)</param>
        /// <param name="firstName">Member's first name</param>
        /// <param name="lastName">Member's last name</param>
        /// <param name="email">Member's email address</param>
        /// <param name="memberType">Type of membership</param>
        public Member(int memberId, string firstName, string lastName, string email, string memberType)
        {
            MemberID = memberId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            MemberType = memberType;
            Loans = new List<Loan>();
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates that the member object has valid data before saving to database
        /// This demonstrates input validation including email format checking
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

            // Email is required and must be valid format
            if (string.IsNullOrWhiteSpace(Email) || !IsValidEmail(Email))
            {
                return false;
            }

            // Member type must be one of the allowed values
            if (string.IsNullOrWhiteSpace(MemberType) ||
                (MemberType != "Student" && MemberType != "Teacher" && MemberType != "Staff"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates email format using a regular expression
        /// This demonstrates basic email validation for NEA projects
        /// Note: This is a simplified validation - production systems might use more complex rules
        /// </summary>
        /// <param name="email">Email address to validate</param>
        /// <returns>True if email format is valid</returns>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Basic email pattern: something@something.something
                // Pattern breakdown:
                // ^         - Start of string
                // [^@\s]+   - One or more characters that aren't @ or whitespace
                // @         - The @ symbol
                // [^@\s]+   - One or more characters that aren't @ or whitespace
                // \.        - A literal dot (period)
                // [^@\s]+   - One or more characters that aren't @ or whitespace
                // $         - End of string
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
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

            if (string.IsNullOrWhiteSpace(Email))
            {
                errors.Add("Email is required");
            }
            else if (!IsValidEmail(Email))
            {
                errors.Add("Email format is invalid");
            }

            if (string.IsNullOrWhiteSpace(MemberType))
            {
                errors.Add("Member type is required");
            }
            else if (MemberType != "Student" && MemberType != "Teacher" && MemberType != "Staff")
            {
                errors.Add("Member type must be Student, Teacher, or Staff");
            }

            return string.Join(", ", errors);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the list of valid member types
        /// Useful for populating dropdown lists in the UI
        /// </summary>
        /// <returns>List of valid member type strings</returns>
        public static List<string> GetMemberTypes()
        {
            return new List<string> { "Student", "Teacher", "Staff" };
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Override ToString for easier debugging and display
        /// Shows member's full name and type
        /// </summary>
        /// <returns>String representation of the member</returns>
        public override string ToString()
        {
            return $"{FullName} ({MemberType})";
        }

        #endregion
    }
}
