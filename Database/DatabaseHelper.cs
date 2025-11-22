using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using System.IO;
using DatabaseExampleWPF.Models;

namespace DatabaseExampleWPF.Database
{
    /// <summary>
    /// DatabaseHelper class - handles all database operations for the Library Management System
    /// 
    /// INSTALLING SQLITE PACKAGE (UPDATED INSTRUCTIONS):
    /// Before this code will work, you MUST install the Microsoft.Data.Sqlite package.
    /// 
    /// To install via NuGet Package Manager:
    /// 1. Right-click on your project in Solution Explorer
    /// 2. Select "Manage NuGet Packages..."
    /// 3. Click the "Browse" tab
    /// 4. Search for "Microsoft.Data.Sqlite"
    /// 5. Select "Microsoft.Data.Sqlite" by Microsoft
    /// 6. Click "Install"
    /// 7. Accept any license agreements
    /// 
    /// Alternative - Via Package Manager Console:
    /// Tools → NuGet Package Manager → Package Manager Console
    /// Type: Install-Package Microsoft.Data.Sqlite
    /// Press Enter
    /// 
    /// This class demonstrates for AQA 7517 NEA:
    /// - Database connection management
    /// - DDL operations (CREATE TABLE)
    /// - DML operations (INSERT, UPDATE, DELETE, SELECT)
    /// - Parameterized queries to prevent SQL injection
    /// - Try-catch error handling for database operations
    /// - Working with foreign keys and relationships
    /// - JOIN queries to combine data from multiple tables
    /// </summary>
    public class DatabaseHelper
    {
        #region Fields and Properties

        /// <summary>
        /// The connection string tells SQLite where to find the database file
        /// "Data Source=" specifies the path to the .db file
        /// 
        /// The database file will be created in the same folder as the executable
        /// if it doesn't already exist.
        /// </summary>
        private static string connectionString = "Data Source=LibraryDatabase.db";

        #endregion

        #region Database Creation Methods

        /// <summary>
        /// Creates all tables in the database if they don't already exist.
        /// This method should be called when the application starts or when
        /// the user clicks a "Create Database" button.
        /// 
        /// Uses "IF NOT EXISTS" so it's safe to call multiple times -
        /// it won't drop existing tables or delete data.
        /// 
        /// This demonstrates DDL (Data Definition Language) operations.
        /// </summary>
        /// <returns>True if successful, false if an error occurred</returns>
        public static bool CreateTables()
        {
            try
            {
                // Open a connection to the database
                // 'using' ensures the connection is properly closed even if an error occurs
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open(); // Open the connection

                    // Create Books table
                    // PRIMARY KEY means BookID is unique and identifies each row
                    // AUTOINCREMENT means SQLite will automatically assign increasing numbers
                    // NOT NULL means the field cannot be empty
                    string createBooksTable = @"
                        CREATE TABLE IF NOT EXISTS Books (
                            BookID INTEGER PRIMARY KEY AUTOINCREMENT,
                            Title TEXT NOT NULL,
                            ISBN TEXT,
                            YearPublished INTEGER NOT NULL
                        )";

                    ExecuteNonQuery(conn, createBooksTable);

                    // Create Authors table
                    string createAuthorsTable = @"
                        CREATE TABLE IF NOT EXISTS Authors (
                            AuthorID INTEGER PRIMARY KEY AUTOINCREMENT,
                            FirstName TEXT NOT NULL,
                            LastName TEXT NOT NULL
                        )";

                    ExecuteNonQuery(conn, createAuthorsTable);

                    // Create BookAuthors junction table
                    // This implements the many-to-many relationship between Books and Authors
                    // A book can have multiple authors, and an author can write multiple books
                    // 
                    // FOREIGN KEY constraints ensure referential integrity:
                    // - You can't add a BookAuthor entry for a BookID that doesn't exist
                    // - You can't add a BookAuthor entry for an AuthorID that doesn't exist
                    // 
                    // PRIMARY KEY (BookID, AuthorID) means the combination must be unique
                    // (prevents adding the same author to the same book twice)
                    string createBookAuthorsTable = @"
                        CREATE TABLE IF NOT EXISTS BookAuthors (
                            BookID INTEGER NOT NULL,
                            AuthorID INTEGER NOT NULL,
                            PRIMARY KEY (BookID, AuthorID),
                            FOREIGN KEY (BookID) REFERENCES Books(BookID) ON DELETE CASCADE,
                            FOREIGN KEY (AuthorID) REFERENCES Authors(AuthorID) ON DELETE CASCADE
                        )";

                    ExecuteNonQuery(conn, createBookAuthorsTable);

                    // Create Members table
                    string createMembersTable = @"
                        CREATE TABLE IF NOT EXISTS Members (
                            MemberID INTEGER PRIMARY KEY AUTOINCREMENT,
                            FirstName TEXT NOT NULL,
                            LastName TEXT NOT NULL,
                            Email TEXT NOT NULL,
                            MemberType TEXT NOT NULL
                        )";

                    ExecuteNonQuery(conn, createMembersTable);

                    // Create Loans table
                    // This demonstrates one-to-many relationships:
                    // - One Book can have many Loans (over time)
                    // - One Member can have many Loans
                    // 
                    // Dates are stored as TEXT in ISO 8601 format (YYYY-MM-DD)
                    // This is SQLite's recommended format for dates
                    string createLoansTable = @"
                        CREATE TABLE IF NOT EXISTS Loans (
                            LoanID INTEGER PRIMARY KEY AUTOINCREMENT,
                            BookID INTEGER NOT NULL,
                            MemberID INTEGER NOT NULL,
                            LoanDate TEXT NOT NULL,
                            DueDate TEXT NOT NULL,
                            ReturnDate TEXT,
                            FOREIGN KEY (BookID) REFERENCES Books(BookID),
                            FOREIGN KEY (MemberID) REFERENCES Members(MemberID)
                        )";

                    ExecuteNonQuery(conn, createLoansTable);

                    conn.Close(); // Close the connection
                }

                return true; // Success
            }
            catch (Exception ex)
            {
                // If any error occurs, log it and return false
                // In a real application, you might want to display this to the user
                System.Diagnostics.Debug.WriteLine($"Error creating tables: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Helper method to execute a non-query SQL command (like CREATE TABLE)
        /// Non-query means it doesn't return data - it modifies the database structure
        /// </summary>
        /// <param name="conn">Open database connection</param>
        /// <param name="commandText">SQL command to execute</param>
        private static void ExecuteNonQuery(SqliteConnection conn, string commandText)
        {
            using (SqliteCommand cmd = new SqliteCommand(commandText, conn))
            {
                cmd.ExecuteNonQuery(); // Execute the command
            }
        }

        #endregion

        #region Book CRUD Operations

        /// <summary>
        /// Inserts a new book into the Books table
        /// 
        /// This demonstrates PARAMETERIZED QUERIES - the most important concept for preventing SQL injection!
        /// 
        /// NEVER do this: "INSERT INTO Books VALUES ('" + title + "')" 
        /// This is vulnerable to SQL injection attacks!
        /// 
        /// ALWAYS use parameters like: "INSERT INTO Books VALUES (@Title)"
        /// Then add parameters: cmd.Parameters.AddWithValue("@Title", title)
        /// 
        /// Parameters are automatically escaped and safe from SQL injection.
        /// </summary>
        /// <param name="book">Book object to insert</param>
        /// <returns>The BookID of the newly inserted book, or -1 if failed</returns>
        public static int InsertBook(Book book)
        {
            try
            {
                // Validate the book data before attempting to insert
                if (!book.IsValid())
                {
                    System.Diagnostics.Debug.WriteLine($"Validation failed: {book.GetValidationErrors()}");
                    return -1;
                }

                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    // SQL INSERT statement with parameters (indicated by @ParameterName)
                    // We don't include BookID because it's AUTOINCREMENT
                    string sql = @"
                        INSERT INTO Books (Title, ISBN, YearPublished)
                        VALUES (@Title, @ISBN, @YearPublished)";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        // Add parameters - this prevents SQL injection
                        // @Title in the SQL is replaced with the actual value
                        cmd.Parameters.AddWithValue("@Title", book.Title);
                        cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                        cmd.Parameters.AddWithValue("@YearPublished", book.YearPublished);

                        // ExecuteNonQuery returns the number of rows affected
                        cmd.ExecuteNonQuery();

                        // Get the ID of the newly inserted book
                        // SQLite provides last_insert_rowid() function for this
                        cmd.CommandText = "SELECT last_insert_rowid()";
                        long id = (long)cmd.ExecuteScalar(); // ExecuteScalar gets a single value

                        return (int)id;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inserting book: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Updates an existing book in the Books table
        /// Demonstrates UPDATE operation with parameterized query
        /// </summary>
        /// <param name="book">Book object with updated data (must have valid BookID)</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool UpdateBook(Book book)
        {
            try
            {
                if (!book.IsValid() || book.BookID <= 0)
                {
                    return false;
                }

                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    // UPDATE statement - modifies existing rows that match the WHERE clause
                    string sql = @"
                        UPDATE Books
                        SET Title = @Title,
                            ISBN = @ISBN,
                            YearPublished = @YearPublished
                        WHERE BookID = @BookID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        // Add parameters for all values including the WHERE clause
                        cmd.Parameters.AddWithValue("@Title", book.Title);
                        cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
                        cmd.Parameters.AddWithValue("@YearPublished", book.YearPublished);
                        cmd.Parameters.AddWithValue("@BookID", book.BookID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        // If rowsAffected is 0, no book with that ID was found
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating book: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes a book from the Books table
        /// Demonstrates DELETE operation with parameterized query
        /// 
        /// Note: Because of ON DELETE CASCADE in BookAuthors table,
        /// deleting a book will automatically delete all BookAuthor entries for that book
        /// </summary>
        /// <param name="bookId">ID of the book to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool DeleteBook(int bookId)
        {
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    // DELETE statement - removes rows that match the WHERE clause
                    string sql = "DELETE FROM Books WHERE BookID = @BookID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@BookID", bookId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting book: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets all books from the database
        /// Demonstrates SELECT query and reading data into objects
        /// </summary>
        /// <returns>List of all books</returns>
        public static List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT BookID, Title, ISBN, YearPublished FROM Books ORDER BY Title";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        // ExecuteReader returns a DataReader - used for reading multiple rows
                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            // Read() moves to the next row and returns true if there is one
                            while (reader.Read())
                            {
                                // Create a Book object from the database row
                                // reader[index] or reader["ColumnName"] gets the value
                                // Convert.ToInt32() converts the value to an integer
                                Book book = new Book
                                {
                                    BookID = Convert.ToInt32(reader["BookID"]),
                                    Title = reader["Title"].ToString(),
                                    ISBN = reader["ISBN"].ToString(),
                                    YearPublished = Convert.ToInt32(reader["YearPublished"])
                                };

                                books.Add(book);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting books: {ex.Message}");
            }

            return books;
        }

        /// <summary>
        /// Gets a single book by ID
        /// Useful when you need details of a specific book
        /// </summary>
        /// <param name="bookId">ID of the book to retrieve</param>
        /// <returns>Book object or null if not found</returns>
        public static Book GetBookById(int bookId)
        {
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT BookID, Title, ISBN, YearPublished FROM Books WHERE BookID = @BookID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@BookID", bookId);

                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Book
                                {
                                    BookID = Convert.ToInt32(reader["BookID"]),
                                    Title = reader["Title"].ToString(),
                                    ISBN = reader["ISBN"].ToString(),
                                    YearPublished = Convert.ToInt32(reader["YearPublished"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting book: {ex.Message}");
            }

            return null;
        }

        #endregion

        #region Author CRUD Operations

        /// <summary>
        /// Inserts a new author into the Authors table
        /// Similar to InsertBook but for authors
        /// </summary>
        /// <param name="author">Author object to insert</param>
        /// <returns>The AuthorID of the newly inserted author, or -1 if failed</returns>
        public static int InsertAuthor(Author author)
        {
            try
            {
                if (!author.IsValid())
                {
                    System.Diagnostics.Debug.WriteLine($"Validation failed: {author.GetValidationErrors()}");
                    return -1;
                }

                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        INSERT INTO Authors (FirstName, LastName)
                        VALUES (@FirstName, @LastName)";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", author.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", author.LastName);

                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "SELECT last_insert_rowid()";
                        long id = (long)cmd.ExecuteScalar();

                        return (int)id;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inserting author: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Updates an existing author
        /// </summary>
        /// <param name="author">Author object with updated data</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool UpdateAuthor(Author author)
        {
            try
            {
                if (!author.IsValid() || author.AuthorID <= 0)
                {
                    return false;
                }

                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        UPDATE Authors
                        SET FirstName = @FirstName,
                            LastName = @LastName
                        WHERE AuthorID = @AuthorID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", author.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", author.LastName);
                        cmd.Parameters.AddWithValue("@AuthorID", author.AuthorID);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating author: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes an author from the Authors table
        /// Note: ON DELETE CASCADE will also remove BookAuthor entries
        /// </summary>
        /// <param name="authorId">ID of the author to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool DeleteAuthor(int authorId)
        {
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "DELETE FROM Authors WHERE AuthorID = @AuthorID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@AuthorID", authorId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting author: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets all authors from the database
        /// </summary>
        /// <returns>List of all authors</returns>
        public static List<Author> GetAllAuthors()
        {
            List<Author> authors = new List<Author>();

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT AuthorID, FirstName, LastName FROM Authors ORDER BY LastName, FirstName";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Author author = new Author
                                {
                                    AuthorID = Convert.ToInt32(reader["AuthorID"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString()
                                };

                                authors.Add(author);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting authors: {ex.Message}");
            }

            return authors;
        }

        /// <summary>
        /// Gets a single author by ID
        /// </summary>
        /// <param name="authorId">ID of the author to retrieve</param>
        /// <returns>Author object or null if not found</returns>
        public static Author GetAuthorById(int authorId)
        {
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT AuthorID, FirstName, LastName FROM Authors WHERE AuthorID = @AuthorID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@AuthorID", authorId);

                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Author
                                {
                                    AuthorID = Convert.ToInt32(reader["AuthorID"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting author: {ex.Message}");
            }

            return null;
        }

        #endregion

        #region Many-to-Many Book-Author Operations

        /// <summary>
        /// Links a book to an author in the BookAuthors junction table
        /// This demonstrates how to manage many-to-many relationships
        /// 
        /// Example: If "The Good Omens" was written by both Terry Pratchett and Neil Gaiman,
        /// you would call this method twice:
        /// - AddBookAuthor(goodOmensBookID, terryPratchettAuthorID)
        /// - AddBookAuthor(goodOmensBookID, neilGaimanAuthorID)
        /// </summary>
        /// <param name="bookId">ID of the book</param>
        /// <param name="authorId">ID of the author</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool AddBookAuthor(int bookId, int authorId)
        {
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    // Check if this relationship already exists
                    string checkSql = "SELECT COUNT(*) FROM BookAuthors WHERE BookID = @BookID AND AuthorID = @AuthorID";
                    using (SqliteCommand checkCmd = new SqliteCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@BookID", bookId);
                        checkCmd.Parameters.AddWithValue("@AuthorID", authorId);

                        long count = (long)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            // Relationship already exists
                            return true;
                        }
                    }

                    // Insert the new relationship
                    string sql = @"
                        INSERT INTO BookAuthors (BookID, AuthorID)
                        VALUES (@BookID, @AuthorID)";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@BookID", bookId);
                        cmd.Parameters.AddWithValue("@AuthorID", authorId);

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding book-author relationship: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Removes the link between a book and an author
        /// </summary>
        /// <param name="bookId">ID of the book</param>
        /// <param name="authorId">ID of the author</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool RemoveBookAuthor(int bookId, int authorId)
        {
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "DELETE FROM BookAuthors WHERE BookID = @BookID AND AuthorID = @AuthorID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@BookID", bookId);
                        cmd.Parameters.AddWithValue("@AuthorID", authorId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing book-author relationship: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets all authors for a specific book
        /// This demonstrates a JOIN query to retrieve related data
        /// 
        /// SQL explanation:
        /// - We're selecting from Authors table
        /// - INNER JOIN means only include authors that have a matching entry in BookAuthors
        /// - ON clause specifies how the tables are related
        /// - WHERE filters to only the book we're interested in
        /// </summary>
        /// <param name="bookId">ID of the book</param>
        /// <returns>List of authors for that book</returns>
        public static List<Author> GetAuthorsForBook(int bookId)
        {
            List<Author> authors = new List<Author>();

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    // JOIN query - combines data from Authors and BookAuthors tables
                    string sql = @"
                        SELECT a.AuthorID, a.FirstName, a.LastName
                        FROM Authors a
                        INNER JOIN BookAuthors ba ON a.AuthorID = ba.AuthorID
                        WHERE ba.BookID = @BookID
                        ORDER BY a.LastName, a.FirstName";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@BookID", bookId);

                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Author author = new Author
                                {
                                    AuthorID = Convert.ToInt32(reader["AuthorID"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString()
                                };

                                authors.Add(author);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting authors for book: {ex.Message}");
            }

            return authors;
        }

        /// <summary>
        /// Gets all books for a specific author
        /// Another example of a JOIN query
        /// </summary>
        /// <param name="authorId">ID of the author</param>
        /// <returns>List of books by that author</returns>
        public static List<Book> GetBooksForAuthor(int authorId)
        {
            List<Book> books = new List<Book>();

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        SELECT b.BookID, b.Title, b.ISBN, b.YearPublished
                        FROM Books b
                        INNER JOIN BookAuthors ba ON b.BookID = ba.BookID
                        WHERE ba.AuthorID = @AuthorID
                        ORDER BY b.Title";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@AuthorID", authorId);

                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Book book = new Book
                                {
                                    BookID = Convert.ToInt32(reader["BookID"]),
                                    Title = reader["Title"].ToString(),
                                    ISBN = reader["ISBN"].ToString(),
                                    YearPublished = Convert.ToInt32(reader["YearPublished"])
                                };

                                books.Add(book);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting books for author: {ex.Message}");
            }

            return books;
        }

        #endregion

        #region Member CRUD Operations

        /// <summary>
        /// Inserts a new member into the Members table
        /// </summary>
        /// <param name="member">Member object to insert</param>
        /// <returns>The MemberID of the newly inserted member, or -1 if failed</returns>
        public static int InsertMember(Member member)
        {
            try
            {
                if (!member.IsValid())
                {
                    System.Diagnostics.Debug.WriteLine($"Validation failed: {member.GetValidationErrors()}");
                    return -1;
                }

                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        INSERT INTO Members (FirstName, LastName, Email, MemberType)
                        VALUES (@FirstName, @LastName, @Email, @MemberType)";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", member.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", member.LastName);
                        cmd.Parameters.AddWithValue("@Email", member.Email);
                        cmd.Parameters.AddWithValue("@MemberType", member.MemberType);

                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "SELECT last_insert_rowid()";
                        long id = (long)cmd.ExecuteScalar();

                        return (int)id;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inserting member: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Updates an existing member
        /// </summary>
        /// <param name="member">Member object with updated data</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool UpdateMember(Member member)
        {
            try
            {
                if (!member.IsValid() || member.MemberID <= 0)
                {
                    return false;
                }

                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        UPDATE Members
                        SET FirstName = @FirstName,
                            LastName = @LastName,
                            Email = @Email,
                            MemberType = @MemberType
                        WHERE MemberID = @MemberID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", member.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", member.LastName);
                        cmd.Parameters.AddWithValue("@Email", member.Email);
                        cmd.Parameters.AddWithValue("@MemberType", member.MemberType);
                        cmd.Parameters.AddWithValue("@MemberID", member.MemberID);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating member: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes a member from the Members table
        /// </summary>
        /// <param name="memberId">ID of the member to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool DeleteMember(int memberId)
        {
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "DELETE FROM Members WHERE MemberID = @MemberID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MemberID", memberId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting member: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets all members from the database
        /// </summary>
        /// <returns>List of all members</returns>
        public static List<Member> GetAllMembers()
        {
            List<Member> members = new List<Member>();

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT MemberID, FirstName, LastName, Email, MemberType FROM Members ORDER BY LastName, FirstName";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Member member = new Member
                                {
                                    MemberID = Convert.ToInt32(reader["MemberID"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    MemberType = reader["MemberType"].ToString()
                                };

                                members.Add(member);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting members: {ex.Message}");
            }

            return members;
        }

        /// <summary>
        /// Gets a single member by ID
        /// </summary>
        /// <param name="memberId">ID of the member to retrieve</param>
        /// <returns>Member object or null if not found</returns>
        public static Member GetMemberById(int memberId)
        {
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT MemberID, FirstName, LastName, Email, MemberType FROM Members WHERE MemberID = @MemberID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MemberID", memberId);

                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Member
                                {
                                    MemberID = Convert.ToInt32(reader["MemberID"]),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    MemberType = reader["MemberType"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting member: {ex.Message}");
            }

            return null;
        }

        #endregion

        #region Loan CRUD Operations

        /// <summary>
        /// Inserts a new loan into the Loans table
        /// 
        /// This demonstrates inserting data that involves foreign keys
        /// and working with dates in SQLite
        /// 
        /// Dates are stored as TEXT in ISO 8601 format (YYYY-MM-DD)
        /// We use ToString("yyyy-MM-dd") to format the DateTime
        /// </summary>
        /// <param name="loan">Loan object to insert</param>
        /// <returns>The LoanID of the newly inserted loan, or -1 if failed</returns>
        public static int InsertLoan(Loan loan)
        {
            try
            {
                if (!loan.IsValid())
                {
                    System.Diagnostics.Debug.WriteLine($"Validation failed: {loan.GetValidationErrors()}");
                    return -1;
                }

                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        INSERT INTO Loans (BookID, MemberID, LoanDate, DueDate, ReturnDate)
                        VALUES (@BookID, @MemberID, @LoanDate, @DueDate, @ReturnDate)";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@BookID", loan.BookID);
                        cmd.Parameters.AddWithValue("@MemberID", loan.MemberID);

                        // Format dates as ISO 8601 strings (YYYY-MM-DD)
                        cmd.Parameters.AddWithValue("@LoanDate", loan.LoanDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@DueDate", loan.DueDate.ToString("yyyy-MM-dd"));

                        // For nullable DateTime, use DBNull.Value if null
                        if (loan.ReturnDate.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@ReturnDate", loan.ReturnDate.Value.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ReturnDate", DBNull.Value);
                        }

                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "SELECT last_insert_rowid()";
                        long id = (long)cmd.ExecuteScalar();

                        return (int)id;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inserting loan: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Updates an existing loan
        /// Commonly used to set the ReturnDate when a book is returned
        /// </summary>
        /// <param name="loan">Loan object with updated data</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool UpdateLoan(Loan loan)
        {
            try
            {
                if (!loan.IsValid() || loan.LoanID <= 0)
                {
                    return false;
                }

                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        UPDATE Loans
                        SET BookID = @BookID,
                            MemberID = @MemberID,
                            LoanDate = @LoanDate,
                            DueDate = @DueDate,
                            ReturnDate = @ReturnDate
                        WHERE LoanID = @LoanID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@BookID", loan.BookID);
                        cmd.Parameters.AddWithValue("@MemberID", loan.MemberID);
                        cmd.Parameters.AddWithValue("@LoanDate", loan.LoanDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@DueDate", loan.DueDate.ToString("yyyy-MM-dd"));

                        if (loan.ReturnDate.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@ReturnDate", loan.ReturnDate.Value.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ReturnDate", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("@LoanID", loan.LoanID);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating loan: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Marks a loan as returned by setting the ReturnDate to today
        /// This is a specialized update operation for common use case
        /// </summary>
        /// <param name="loanId">ID of the loan to mark as returned</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool ReturnBook(int loanId)
        {
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "UPDATE Loans SET ReturnDate = @ReturnDate WHERE LoanID = @LoanID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ReturnDate", DateTime.Today.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@LoanID", loanId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error returning book: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Deletes a loan from the Loans table
        /// </summary>
        /// <param name="loanId">ID of the loan to delete</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool DeleteLoan(int loanId)
        {
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "DELETE FROM Loans WHERE LoanID = @LoanID";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoanID", loanId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting loan: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets all loans from the database
        /// Returns basic Loan objects without joined data
        /// </summary>
        /// <returns>List of all loans</returns>
        public static List<Loan> GetAllLoans()
        {
            List<Loan> loans = new List<Loan>();

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = "SELECT LoanID, BookID, MemberID, LoanDate, DueDate, ReturnDate FROM Loans ORDER BY LoanDate DESC";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Loan loan = new Loan
                                {
                                    LoanID = Convert.ToInt32(reader["LoanID"]),
                                    BookID = Convert.ToInt32(reader["BookID"]),
                                    MemberID = Convert.ToInt32(reader["MemberID"]),
                                    LoanDate = DateTime.Parse(reader["LoanDate"].ToString()),
                                    DueDate = DateTime.Parse(reader["DueDate"].ToString()),
                                    // Check if ReturnDate is null before parsing
                                    ReturnDate = reader["ReturnDate"] == DBNull.Value ?
                                        (DateTime?)null :
                                        DateTime.Parse(reader["ReturnDate"].ToString())
                                };

                                loans.Add(loan);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting loans: {ex.Message}");
            }

            return loans;
        }

        /// <summary>
        /// Gets all loans with full details from joined tables
        /// This demonstrates a complex JOIN query across multiple tables
        /// 
        /// SQL explanation:
        /// - SELECT lists all columns we need from all three tables
        /// - FROM Loans (start with the Loans table)
        /// - INNER JOIN Books - only include loans that have a valid book
        /// - INNER JOIN Members - only include loans that have a valid member
        /// - The ON clauses specify how tables are related via foreign keys
        /// </summary>
        /// <returns>List of loans with book and member details</returns>
        public static List<LoanWithDetails> GetAllLoansWithDetails()
        {
            List<LoanWithDetails> loans = new List<LoanWithDetails>();

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    // Complex JOIN query - combines Loans, Books, and Members tables
                    string sql = @"
                        SELECT 
                            l.LoanID, l.BookID, l.MemberID, l.LoanDate, l.DueDate, l.ReturnDate,
                            b.Title AS BookTitle, b.ISBN AS BookISBN,
                            m.FirstName AS MemberFirstName, m.LastName AS MemberLastName, 
                            m.Email AS MemberEmail, m.MemberType AS MemberType
                        FROM Loans l
                        INNER JOIN Books b ON l.BookID = b.BookID
                        INNER JOIN Members m ON l.MemberID = m.MemberID
                        ORDER BY l.LoanDate DESC";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LoanWithDetails loan = new LoanWithDetails
                                {
                                    LoanID = Convert.ToInt32(reader["LoanID"]),
                                    BookID = Convert.ToInt32(reader["BookID"]),
                                    MemberID = Convert.ToInt32(reader["MemberID"]),
                                    LoanDate = DateTime.Parse(reader["LoanDate"].ToString()),
                                    DueDate = DateTime.Parse(reader["DueDate"].ToString()),
                                    ReturnDate = reader["ReturnDate"] == DBNull.Value ?
                                        (DateTime?)null :
                                        DateTime.Parse(reader["ReturnDate"].ToString()),

                                    // Book details from JOIN
                                    BookTitle = reader["BookTitle"].ToString(),
                                    BookISBN = reader["BookISBN"].ToString(),

                                    // Member details from JOIN
                                    MemberFirstName = reader["MemberFirstName"].ToString(),
                                    MemberLastName = reader["MemberLastName"].ToString(),
                                    MemberEmail = reader["MemberEmail"].ToString(),
                                    MemberType = reader["MemberType"].ToString()
                                };

                                loans.Add(loan);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting loans with details: {ex.Message}");
            }

            return loans;
        }

        /// <summary>
        /// Searches loans with details based on search criteria
        /// This demonstrates filtering joined data with LIKE operator
        /// 
        /// LIKE operator allows partial matching:
        /// - '%' matches any sequence of characters
        /// - So '%search%' matches if 'search' appears anywhere in the text
        /// </summary>
        /// <param name="searchTerm">Search term to match against book title or member name</param>
        /// <returns>List of matching loans with details</returns>
        public static List<LoanWithDetails> SearchLoansWithDetails(string searchTerm)
        {
            List<LoanWithDetails> loans = new List<LoanWithDetails>();

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    // Search across book titles and member names using LIKE
                    string sql = @"
                        SELECT 
                            l.LoanID, l.BookID, l.MemberID, l.LoanDate, l.DueDate, l.ReturnDate,
                            b.Title AS BookTitle, b.ISBN AS BookISBN,
                            m.FirstName AS MemberFirstName, m.LastName AS MemberLastName, 
                            m.Email AS MemberEmail, m.MemberType AS MemberType
                        FROM Loans l
                        INNER JOIN Books b ON l.BookID = b.BookID
                        INNER JOIN Members m ON l.MemberID = m.MemberID
                        WHERE b.Title LIKE @SearchTerm
                           OR m.FirstName LIKE @SearchTerm
                           OR m.LastName LIKE @SearchTerm
                        ORDER BY l.LoanDate DESC";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        // Add % wildcards for partial matching
                        cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LoanWithDetails loan = new LoanWithDetails
                                {
                                    LoanID = Convert.ToInt32(reader["LoanID"]),
                                    BookID = Convert.ToInt32(reader["BookID"]),
                                    MemberID = Convert.ToInt32(reader["MemberID"]),
                                    LoanDate = DateTime.Parse(reader["LoanDate"].ToString()),
                                    DueDate = DateTime.Parse(reader["DueDate"].ToString()),
                                    ReturnDate = reader["ReturnDate"] == DBNull.Value ?
                                        (DateTime?)null :
                                        DateTime.Parse(reader["ReturnDate"].ToString()),
                                    BookTitle = reader["BookTitle"].ToString(),
                                    BookISBN = reader["BookISBN"].ToString(),
                                    MemberFirstName = reader["MemberFirstName"].ToString(),
                                    MemberLastName = reader["MemberLastName"].ToString(),
                                    MemberEmail = reader["MemberEmail"].ToString(),
                                    MemberType = reader["MemberType"].ToString()
                                };

                                loans.Add(loan);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching loans: {ex.Message}");
            }

            return loans;
        }

        /// <summary>
        /// Gets loans for a specific member
        /// Useful for viewing a member's borrowing history
        /// </summary>
        /// <param name="memberId">ID of the member</param>
        /// <returns>List of loans for that member</returns>
        public static List<Loan> GetLoansForMember(int memberId)
        {
            List<Loan> loans = new List<Loan>();

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        SELECT LoanID, BookID, MemberID, LoanDate, DueDate, ReturnDate 
                        FROM Loans 
                        WHERE MemberID = @MemberID
                        ORDER BY LoanDate DESC";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MemberID", memberId);

                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Loan loan = new Loan
                                {
                                    LoanID = Convert.ToInt32(reader["LoanID"]),
                                    BookID = Convert.ToInt32(reader["BookID"]),
                                    MemberID = Convert.ToInt32(reader["MemberID"]),
                                    LoanDate = DateTime.Parse(reader["LoanDate"].ToString()),
                                    DueDate = DateTime.Parse(reader["DueDate"].ToString()),
                                    ReturnDate = reader["ReturnDate"] == DBNull.Value ?
                                        (DateTime?)null :
                                        DateTime.Parse(reader["ReturnDate"].ToString())
                                };

                                loans.Add(loan);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting loans for member: {ex.Message}");
            }

            return loans;
        }

        /// <summary>
        /// Gets currently active (unreturned) loans
        /// Useful for seeing what books are currently on loan
        /// </summary>
        /// <returns>List of active loans</returns>
        public static List<LoanWithDetails> GetActiveLoans()
        {
            List<LoanWithDetails> loans = new List<LoanWithDetails>();

            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    // WHERE ReturnDate IS NULL finds loans that haven't been returned
                    string sql = @"
                        SELECT 
                            l.LoanID, l.BookID, l.MemberID, l.LoanDate, l.DueDate, l.ReturnDate,
                            b.Title AS BookTitle, b.ISBN AS BookISBN,
                            m.FirstName AS MemberFirstName, m.LastName AS MemberLastName, 
                            m.Email AS MemberEmail, m.MemberType AS MemberType
                        FROM Loans l
                        INNER JOIN Books b ON l.BookID = b.BookID
                        INNER JOIN Members m ON l.MemberID = m.MemberID
                        WHERE l.ReturnDate IS NULL
                        ORDER BY l.DueDate ASC";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        using (SqliteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LoanWithDetails loan = new LoanWithDetails
                                {
                                    LoanID = Convert.ToInt32(reader["LoanID"]),
                                    BookID = Convert.ToInt32(reader["BookID"]),
                                    MemberID = Convert.ToInt32(reader["MemberID"]),
                                    LoanDate = DateTime.Parse(reader["LoanDate"].ToString()),
                                    DueDate = DateTime.Parse(reader["DueDate"].ToString()),
                                    ReturnDate = null, // We know it's null because of WHERE clause
                                    BookTitle = reader["BookTitle"].ToString(),
                                    BookISBN = reader["BookISBN"].ToString(),
                                    MemberFirstName = reader["MemberFirstName"].ToString(),
                                    MemberLastName = reader["MemberLastName"].ToString(),
                                    MemberEmail = reader["MemberEmail"].ToString(),
                                    MemberType = reader["MemberType"].ToString()
                                };

                                loans.Add(loan);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting active loans: {ex.Message}");
            }

            return loans;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if the database file exists
        /// Useful for determining if tables need to be created
        /// </summary>
        /// <returns>True if database file exists</returns>
        public static bool DatabaseExists()
        {
            // Extract filename from connection string
            string dbFile = "LibraryDatabase.db";
            return File.Exists(dbFile);
        }

        /// <summary>
        /// Checks if a specific table exists in the database
        /// </summary>
        /// <param name="tableName">Name of the table to check</param>
        /// <returns>True if table exists</returns>
        public static bool TableExists(string tableName)
        {
            try
            {
                using (SqliteConnection conn = new SqliteConnection(connectionString))
                {
                    conn.Open();

                    // SQLite stores table information in sqlite_master
                    string sql = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@TableName";

                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TableName", tableName);
                        long count = (long)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking if table exists: {ex.Message}");
                return false;
            }
        }

        #endregion
        #region Sample Data Population

        /// <summary>
        /// Populates the database with sample data for testing and demonstration
        /// This demonstrates inserting multiple related records
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool PopulateSampleData()
        {
            try
            {
                // Arrays of sample data
                string[] bookTitles = {
            "The Hobbit",
            "1984",
            "To Kill a Mockingbird",
            "Pride and Prejudice",
            "The Great Gatsby",
            "Harry Potter and the Philosopher's Stone",
            "The Catcher in the Rye",
            "Animal Farm",
            "Lord of the Flies",
            "Good Omens"
        };

                string[] bookISBNs = {
            "978-0547928227",
            "978-0451524935",
            "978-0061120084",
            "978-0141439518",
            "978-0743273565",
            "978-0439708180",
            "978-0316769488",
            "978-0451526342",
            "978-0399501487",
            "978-0060853983"
        };

                int[] bookYears = { 1937, 1949, 1960, 1813, 1925, 1997, 1951, 1945, 1954, 1990 };

                string[] authorFirstNames = {
            "J.R.R.",
            "George",
            "Harper",
            "Jane",
            "F. Scott",
            "J.K.",
            "J.D.",
            "William",
            "Terry",
            "Neil"
        };

                string[] authorLastNames = {
            "Tolkien",
            "Orwell",
            "Lee",
            "Austen",
            "Fitzgerald",
            "Rowling",
            "Salinger",
            "Golding",
            "Pratchett",
            "Gaiman"
        };

                string[] memberFirstNames = {
            "Alice", "Bob", "Charlie", "Diana", "Edward",
            "Fiona", "George", "Hannah", "Ian", "Julia"
        };

                string[] memberLastNames = {
            "Smith", "Johnson", "Williams", "Brown", "Jones",
            "Garcia", "Miller", "Davis", "Rodriguez", "Martinez"
        };

                string[] memberTypes = { "Student", "Student", "Student", "Teacher", "Student",
                                "Staff", "Student", "Teacher", "Student", "Staff" };

                // Step 1: Insert Authors
                List<int> authorIds = new List<int>();
                for (int i = 0; i < 10; i++)
                {
                    Author author = new Author
                    {
                        FirstName = authorFirstNames[i],
                        LastName = authorLastNames[i]
                    };

                    int authorId = InsertAuthor(author);
                    if (authorId > 0)
                    {
                        authorIds.Add(authorId);
                    }
                }

                // Step 2: Insert Books
                List<int> bookIds = new List<int>();
                for (int i = 0; i < 10; i++)
                {
                    Book book = new Book
                    {
                        Title = bookTitles[i],
                        ISBN = bookISBNs[i],
                        YearPublished = bookYears[i]
                    };

                    int bookId = InsertBook(book);
                    if (bookId > 0)
                    {
                        bookIds.Add(bookId);
                    }
                }

                // Step 3: Create Book-Author relationships
                // Most books have one author, but "Good Omens" has two (Terry Pratchett and Neil Gaiman)
                for (int i = 0; i < bookIds.Count; i++)
                {
                    if (i < authorIds.Count)
                    {
                        AddBookAuthor(bookIds[i], authorIds[i]);
                    }
                }

                // Add Neil Gaiman (index 9) as co-author of Good Omens (index 9)
                if (bookIds.Count >= 10 && authorIds.Count >= 10)
                {
                    AddBookAuthor(bookIds[9], authorIds[8]); // Terry Pratchett
                                                             // Already added above: AddBookAuthor(bookIds[9], authorIds[9]); // Neil Gaiman
                }

                // Step 4: Insert Members
                List<int> memberIds = new List<int>();
                for (int i = 0; i < 10; i++)
                {
                    Member member = new Member
                    {
                        FirstName = memberFirstNames[i],
                        LastName = memberLastNames[i],
                        Email = $"{memberFirstNames[i].ToLower()}.{memberLastNames[i].ToLower()}@school.com",
                        MemberType = memberTypes[i]
                    };

                    int memberId = InsertMember(member);
                    if (memberId > 0)
                    {
                        memberIds.Add(memberId);
                    }
                }

                // Step 5: Insert Loans
                // Create a variety of loan scenarios:
                // - Some returned on time
                // - Some currently on loan
                // - Some overdue
                if (bookIds.Count >= 10 && memberIds.Count >= 10)
                {
                    // Loan 1: Returned on time
                    Loan loan1 = new Loan
                    {
                        BookID = bookIds[0],
                        MemberID = memberIds[0],
                        LoanDate = DateTime.Today.AddDays(-20),
                        DueDate = DateTime.Today.AddDays(-6),
                        ReturnDate = DateTime.Today.AddDays(-7)
                    };
                    InsertLoan(loan1);

                    // Loan 2: Currently on loan, not overdue
                    Loan loan2 = new Loan
                    {
                        BookID = bookIds[1],
                        MemberID = memberIds[1],
                        LoanDate = DateTime.Today.AddDays(-5),
                        DueDate = DateTime.Today.AddDays(9),
                        ReturnDate = null
                    };
                    InsertLoan(loan2);

                    // Loan 3: OVERDUE (not returned, past due date)
                    Loan loan3 = new Loan
                    {
                        BookID = bookIds[2],
                        MemberID = memberIds[2],
                        LoanDate = DateTime.Today.AddDays(-25),
                        DueDate = DateTime.Today.AddDays(-5),
                        ReturnDate = null
                    };
                    InsertLoan(loan3);

                    // Loan 4: Returned on time
                    Loan loan4 = new Loan
                    {
                        BookID = bookIds[3],
                        MemberID = memberIds[3],
                        LoanDate = DateTime.Today.AddDays(-30),
                        DueDate = DateTime.Today.AddDays(-16),
                        ReturnDate = DateTime.Today.AddDays(-17)
                    };
                    InsertLoan(loan4);

                    // Loan 5: Currently on loan, not overdue
                    Loan loan5 = new Loan
                    {
                        BookID = bookIds[4],
                        MemberID = memberIds[4],
                        LoanDate = DateTime.Today.AddDays(-3),
                        DueDate = DateTime.Today.AddDays(11),
                        ReturnDate = null
                    };
                    InsertLoan(loan5);

                    // Loan 6: OVERDUE
                    Loan loan6 = new Loan
                    {
                        BookID = bookIds[5],
                        MemberID = memberIds[5],
                        LoanDate = DateTime.Today.AddDays(-20),
                        DueDate = DateTime.Today.AddDays(-2),
                        ReturnDate = null
                    };
                    InsertLoan(loan6);

                    // Loan 7: Returned late
                    Loan loan7 = new Loan
                    {
                        BookID = bookIds[6],
                        MemberID = memberIds[6],
                        LoanDate = DateTime.Today.AddDays(-40),
                        DueDate = DateTime.Today.AddDays(-26),
                        ReturnDate = DateTime.Today.AddDays(-20)
                    };
                    InsertLoan(loan7);

                    // Loan 8: Currently on loan, due soon
                    Loan loan8 = new Loan
                    {
                        BookID = bookIds[7],
                        MemberID = memberIds[7],
                        LoanDate = DateTime.Today.AddDays(-10),
                        DueDate = DateTime.Today.AddDays(4),
                        ReturnDate = null
                    };
                    InsertLoan(loan8);

                    // Loan 9: OVERDUE
                    Loan loan9 = new Loan
                    {
                        BookID = bookIds[8],
                        MemberID = memberIds[8],
                        LoanDate = DateTime.Today.AddDays(-35),
                        DueDate = DateTime.Today.AddDays(-7),
                        ReturnDate = null
                    };
                    InsertLoan(loan9);

                    // Loan 10: Returned on time
                    Loan loan10 = new Loan
                    {
                        BookID = bookIds[9],
                        MemberID = memberIds[9],
                        LoanDate = DateTime.Today.AddDays(-15),
                        DueDate = DateTime.Today.AddDays(-1),
                        ReturnDate = DateTime.Today.AddDays(-2)
                    };
                    InsertLoan(loan10);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error populating sample data: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}