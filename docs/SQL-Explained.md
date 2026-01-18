# SQL and SQLite Explained for A-Level NEA

This document explains all the SQL concepts used in this Library Management System project, designed specifically for AQA 7517 A-Level Computer Science students working on their NEA (Non-Exam Assessment) projects.

---

## Table of Contents

1. [What is SQLite and Why Use It?](#what-is-sqlite-and-why-use-it)
2. [Connecting to SQLite in C#](#connecting-to-sqlite-in-c)
3. [Database Design (DDL - Data Definition Language)](#database-design-ddl---data-definition-language)
4. [CRUD Operations (DML - Data Manipulation Language)](#crud-operations-dml---data-manipulation-language)
5. [JOIN Queries - Combining Tables](#join-queries---combining-tables)
6. [Parameterised Queries - Preventing SQL Injection](#parameterised-queries---preventing-sql-injection)
7. [Working with NULL Values](#working-with-null-values)
8. [Design Decisions and Alternatives](#design-decisions-and-alternatives)
9. [Using DB Browser for SQLite](#using-db-browser-for-sqlite)
10. [Common Mistakes to Avoid](#common-mistakes-to-avoid)
11. [SQL Quick Reference](#sql-quick-reference)

---

## What is SQLite and Why Use It?

### What is SQLite?

SQLite is a **serverless, file-based relational database**. Unlike MySQL or SQL Server, SQLite:

- **Doesn't require a separate server** - the entire database is stored in a single `.db` file
- **Is self-contained** - no installation or configuration needed on the target machine
- **Works offline** - perfect for desktop applications
- **Is lightweight** - the library is tiny and fast

### Why Choose SQLite for A-Level NEA Projects?

| Advantage | Why It Matters for NEA |
|-----------|------------------------|
| **No server setup** | Works on school networks without admin rights |
| **Single file database** | Easy to backup, move, and submit with your project |
| **Free and open source** | No licensing concerns |
| **Standard SQL syntax** | Skills transfer to other databases |
| **Portable** | Your project works on any Windows PC |
| **Easy debugging** | Can view data with DB Browser for SQLite |

### Alternatives You Could Use Instead

| Database | Pros | Cons | When to Use |
|----------|------|------|-------------|
| **SQLite** | Simple, portable, no setup | Limited concurrent access | Desktop apps, single-user systems |
| **SQL Server LocalDB** | Full SQL Server features | Requires installation | If you need advanced SQL features |
| **MySQL** | Industry standard | Requires server | Web applications |
| **MS Access** | Visual designer | Limited programming support | Very simple databases |
| **JSON files** | No SQL needed | No relationships, slow queries | Very simple data storage |

**Recommendation for NEA:** SQLite is ideal for most A-Level projects because it demonstrates proper database concepts without infrastructure complexity.

---

## Connecting to SQLite in C#

### Installing the SQLite Package

Before writing any code, you must install the SQLite NuGet package:

**Method 1 - NuGet Package Manager (Recommended):**
1. Right-click your project in Solution Explorer
2. Select "Manage NuGet Packages..."
3. Click the "Browse" tab
4. Search for **"Microsoft.Data.Sqlite"**
5. Select "Microsoft.Data.Sqlite" by Microsoft
6. Click "Install"

**Method 2 - Package Manager Console:**
```
Tools → NuGet Package Manager → Package Manager Console
Type: Install-Package Microsoft.Data.Sqlite
Press Enter
```

### The Connection String

The connection string tells SQLite where to find your database file:

```csharp
// This creates/opens a file called "LibraryDatabase.db" in the same folder as your .exe
private static string connectionString = "Data Source=LibraryDatabase.db";
```

**Understanding connection strings:**
- `Data Source=` specifies the database file path
- If the file doesn't exist, SQLite creates it automatically
- Use a relative path so the database travels with your application

**Alternative connection strings:**
```csharp
// Absolute path (not recommended - less portable)
"Data Source=C:\\MyProject\\Database.db"

// In-memory database (data lost when app closes - useful for testing)
"Data Source=:memory:"

// Read-only mode
"Data Source=LibraryDatabase.db;Mode=ReadOnly"
```

### The Connection Pattern

Always use the `using` statement when working with database connections:

```csharp
// CORRECT: using statement ensures connection is properly closed
using (SqliteConnection conn = new SqliteConnection(connectionString))
{
    conn.Open();
    // ... do database work here ...
}  // Connection automatically closed here, even if an error occurs

// INCORRECT: manual open/close - risky if error occurs
SqliteConnection conn = new SqliteConnection(connectionString);
conn.Open();
// ... if error here, connection never closes!
conn.Close();  // Might not execute
```

**Why `using` is important:**
- Connections are **limited resources**
- Unclosed connections can lock your database
- `using` guarantees cleanup even if exceptions occur
- It's the **industry standard pattern**

---

## Database Design (DDL - Data Definition Language)

DDL statements define the **structure** of your database (tables, columns, constraints).

### CREATE TABLE Syntax

```sql
CREATE TABLE TableName (
    ColumnName1 DataType CONSTRAINTS,
    ColumnName2 DataType CONSTRAINTS,
    ...
    TABLE_CONSTRAINTS
);
```

### SQLite Data Types

SQLite uses a simplified type system:

| SQLite Type | What It Stores | C# Equivalent | Example Values |
|-------------|----------------|---------------|----------------|
| `INTEGER` | Whole numbers | `int`, `long` | 1, 42, -100 |
| `TEXT` | Strings | `string` | "Hello", "ISBN-123" |
| `REAL` | Decimal numbers | `double`, `float` | 3.14, -0.5 |
| `BLOB` | Binary data | `byte[]` | Images, files |
| `NULL` | No value | `null` | (empty) |

**Note:** SQLite is "type-affinity" rather than strictly typed. It will try to convert values, but it's best practice to use the correct types.

### Column Constraints

| Constraint | Purpose | Example |
|------------|---------|---------|
| `PRIMARY KEY` | Uniquely identifies each row | `BookID INTEGER PRIMARY KEY` |
| `AUTOINCREMENT` | Auto-generates increasing numbers | `BookID INTEGER PRIMARY KEY AUTOINCREMENT` |
| `NOT NULL` | Prevents empty values | `Title TEXT NOT NULL` |
| `UNIQUE` | No duplicate values allowed | `ISBN TEXT UNIQUE` |
| `DEFAULT` | Sets default value | `MemberType TEXT DEFAULT 'Student'` |
| `CHECK` | Validates data | `CHECK(YearPublished > 1000)` |

### Tables in This Project

#### Books Table
```sql
CREATE TABLE IF NOT EXISTS Books (
    BookID INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    ISBN TEXT,
    YearPublished INTEGER NOT NULL
);
```

**Explanation:**
- `BookID INTEGER PRIMARY KEY AUTOINCREMENT`:
  - `INTEGER PRIMARY KEY` makes this the unique identifier
  - `AUTOINCREMENT` means SQLite assigns 1, 2, 3, 4... automatically
- `Title TEXT NOT NULL`: Book must have a title (cannot be empty)
- `ISBN TEXT`: Optional field (can be NULL)
- `YearPublished INTEGER NOT NULL`: Must have a year

#### Authors Table
```sql
CREATE TABLE IF NOT EXISTS Authors (
    AuthorID INTEGER PRIMARY KEY AUTOINCREMENT,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL
);
```

#### Members Table
```sql
CREATE TABLE IF NOT EXISTS Members (
    MemberID INTEGER PRIMARY KEY AUTOINCREMENT,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    Email TEXT NOT NULL,
    MemberType TEXT NOT NULL
);
```

**Design decision:** `MemberType` is stored as TEXT rather than a separate lookup table. This is simpler for an A-Level project. In a larger system, you might create a separate `MemberTypes` table.

#### BookAuthors Junction Table (Many-to-Many)
```sql
CREATE TABLE IF NOT EXISTS BookAuthors (
    BookID INTEGER NOT NULL,
    AuthorID INTEGER NOT NULL,
    PRIMARY KEY (BookID, AuthorID),
    FOREIGN KEY (BookID) REFERENCES Books(BookID) ON DELETE CASCADE,
    FOREIGN KEY (AuthorID) REFERENCES Authors(AuthorID) ON DELETE CASCADE
);
```

**This is the most complex table - let's break it down:**

1. **Why a junction table?**
   - A book can have multiple authors (e.g., "Good Omens" by Pratchett AND Gaiman)
   - An author can write multiple books
   - This is a **many-to-many relationship**
   - You cannot represent this with a single foreign key

2. **`PRIMARY KEY (BookID, AuthorID)`**:
   - This is a **composite primary key** (two columns together)
   - The combination must be unique
   - Prevents adding the same author to the same book twice

3. **`FOREIGN KEY (BookID) REFERENCES Books(BookID)`**:
   - Ensures BookID exists in the Books table
   - You can't add a BookAuthor entry for a non-existent book

4. **`ON DELETE CASCADE`**:
   - If you delete a book, automatically delete all its BookAuthor entries
   - Prevents orphaned records

#### Loans Table
```sql
CREATE TABLE IF NOT EXISTS Loans (
    LoanID INTEGER PRIMARY KEY AUTOINCREMENT,
    BookID INTEGER NOT NULL,
    MemberID INTEGER NOT NULL,
    LoanDate TEXT NOT NULL,
    DueDate TEXT NOT NULL,
    ReturnDate TEXT,
    FOREIGN KEY (BookID) REFERENCES Books(BookID),
    FOREIGN KEY (MemberID) REFERENCES Members(MemberID)
);
```

**Key points:**
- `ReturnDate TEXT` has no `NOT NULL` - it can be NULL (book not yet returned)
- Dates are stored as TEXT in ISO 8601 format: `YYYY-MM-DD`
- SQLite doesn't have a native DATE type, so TEXT is the standard approach

### IF NOT EXISTS

Always use `CREATE TABLE IF NOT EXISTS` instead of just `CREATE TABLE`:

```sql
-- SAFE: Won't error if table already exists
CREATE TABLE IF NOT EXISTS Books (...)

-- RISKY: Errors if table exists, could lose data if you try to drop first
CREATE TABLE Books (...)
```

---

## CRUD Operations (DML - Data Manipulation Language)

CRUD stands for **C**reate, **R**ead, **U**pdate, **D**elete - the four fundamental database operations.

### CREATE (INSERT)

Adds new records to a table.

**Syntax:**
```sql
INSERT INTO TableName (Column1, Column2, Column3)
VALUES (@Value1, @Value2, @Value3);
```

**Example - Inserting a Book:**
```sql
INSERT INTO Books (Title, ISBN, YearPublished)
VALUES (@Title, @ISBN, @YearPublished);
```

**C# Code:**
```csharp
string sql = @"
    INSERT INTO Books (Title, ISBN, YearPublished)
    VALUES (@Title, @ISBN, @YearPublished)";

using (SqliteCommand cmd = new SqliteCommand(sql, conn))
{
    // Parameters prevent SQL injection (see section below)
    cmd.Parameters.AddWithValue("@Title", book.Title);
    cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
    cmd.Parameters.AddWithValue("@YearPublished", book.YearPublished);

    cmd.ExecuteNonQuery();  // ExecuteNonQuery for INSERT/UPDATE/DELETE

    // Get the auto-generated ID
    cmd.CommandText = "SELECT last_insert_rowid()";
    long newId = (long)cmd.ExecuteScalar();  // ExecuteScalar returns single value
}
```

**Key points:**
- We don't include `BookID` because it's `AUTOINCREMENT`
- `ExecuteNonQuery()` returns number of rows affected (1 for successful insert)
- `last_insert_rowid()` gets the auto-generated ID

### READ (SELECT)

Retrieves data from the database.

**Basic SELECT:**
```sql
SELECT Column1, Column2, Column3
FROM TableName;
```

**SELECT with WHERE clause:**
```sql
SELECT BookID, Title, ISBN, YearPublished
FROM Books
WHERE BookID = @BookID;
```

**SELECT with ORDER BY:**
```sql
SELECT * FROM Authors
ORDER BY LastName, FirstName;
```

**C# Code - Reading Multiple Rows:**
```csharp
string sql = "SELECT BookID, Title, ISBN, YearPublished FROM Books ORDER BY Title";

using (SqliteCommand cmd = new SqliteCommand(sql, conn))
{
    using (SqliteDataReader reader = cmd.ExecuteReader())
    {
        while (reader.Read())  // Read() moves to next row, returns false when done
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
```

**C# Code - Reading Single Row:**
```csharp
using (SqliteDataReader reader = cmd.ExecuteReader())
{
    if (reader.Read())  // Use if instead of while for single row
    {
        return new Book
        {
            BookID = Convert.ToInt32(reader["BookID"]),
            // ... other properties
        };
    }
}
return null;  // Not found
```

### UPDATE

Modifies existing records.

**Syntax:**
```sql
UPDATE TableName
SET Column1 = @Value1,
    Column2 = @Value2
WHERE ConditionColumn = @ConditionValue;
```

**Example - Updating a Book:**
```sql
UPDATE Books
SET Title = @Title,
    ISBN = @ISBN,
    YearPublished = @YearPublished
WHERE BookID = @BookID;
```

**C# Code:**
```csharp
string sql = @"
    UPDATE Books
    SET Title = @Title,
        ISBN = @ISBN,
        YearPublished = @YearPublished
    WHERE BookID = @BookID";

using (SqliteCommand cmd = new SqliteCommand(sql, conn))
{
    cmd.Parameters.AddWithValue("@Title", book.Title);
    cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
    cmd.Parameters.AddWithValue("@YearPublished", book.YearPublished);
    cmd.Parameters.AddWithValue("@BookID", book.BookID);  // Don't forget WHERE!

    int rowsAffected = cmd.ExecuteNonQuery();
    return rowsAffected > 0;  // True if update succeeded
}
```

**CRITICAL WARNING:** Always include a WHERE clause! Without it, you'll update EVERY row:
```sql
-- DANGEROUS: Updates ALL books!
UPDATE Books SET Title = 'Wrong'

-- SAFE: Updates only one book
UPDATE Books SET Title = 'Correct' WHERE BookID = 5
```

### DELETE

Removes records from the database.

**Syntax:**
```sql
DELETE FROM TableName
WHERE ConditionColumn = @ConditionValue;
```

**Example:**
```sql
DELETE FROM Books WHERE BookID = @BookID;
```

**C# Code:**
```csharp
string sql = "DELETE FROM Books WHERE BookID = @BookID";

using (SqliteCommand cmd = new SqliteCommand(sql, conn))
{
    cmd.Parameters.AddWithValue("@BookID", bookId);

    int rowsAffected = cmd.ExecuteNonQuery();
    return rowsAffected > 0;  // True if something was deleted
}
```

**CRITICAL WARNING:** Always include a WHERE clause!
```sql
-- CATASTROPHIC: Deletes ALL books!
DELETE FROM Books

-- SAFE: Deletes only one book
DELETE FROM Books WHERE BookID = 5
```

---

## JOIN Queries - Combining Tables

JOIN queries are essential for relational databases. They combine data from multiple related tables.

### Understanding Relationships

In this project:
- **One-to-Many:** One Member can have many Loans; One Book can have many Loans
- **Many-to-Many:** Books and Authors (via BookAuthors junction table)

### INNER JOIN Explained

`INNER JOIN` returns only rows where there's a match in both tables.

**Visual representation:**
```
Books table:                    Loans table:
BookID | Title                  LoanID | BookID | MemberID
1      | The Hobbit             1      | 1      | 5
2      | 1984                   2      | 1      | 3
3      | Pride and Prejudice    3      | 3      | 5

INNER JOIN Books ON Loans.BookID = Books.BookID:
LoanID | BookID | Title              | MemberID
1      | 1      | The Hobbit         | 5
2      | 1      | The Hobbit         | 3
3      | 3      | Pride and Prejudice| 5

Notice: Book 2 (1984) doesn't appear - it has no loans
```

### Two-Table JOIN

**Getting authors for a specific book:**
```sql
SELECT a.AuthorID, a.FirstName, a.LastName
FROM Authors a
INNER JOIN BookAuthors ba ON a.AuthorID = ba.AuthorID
WHERE ba.BookID = @BookID
ORDER BY a.LastName, a.FirstName;
```

**Breaking this down:**
1. `FROM Authors a` - Start with Authors table, alias it as 'a'
2. `INNER JOIN BookAuthors ba` - Join with BookAuthors, alias as 'ba'
3. `ON a.AuthorID = ba.AuthorID` - How the tables connect
4. `WHERE ba.BookID = @BookID` - Filter for specific book
5. `ORDER BY` - Sort the results

### Three-Table JOIN

**Getting loans with book and member details:**
```sql
SELECT
    l.LoanID, l.LoanDate, l.DueDate, l.ReturnDate,
    b.BookID, b.Title AS BookTitle, b.ISBN AS BookISBN,
    m.MemberID, m.FirstName AS MemberFirstName,
    m.LastName AS MemberLastName, m.Email AS MemberEmail
FROM Loans l
INNER JOIN Books b ON l.BookID = b.BookID
INNER JOIN Members m ON l.MemberID = m.MemberID
ORDER BY l.LoanDate DESC;
```

**Explanation:**
1. Start with `Loans` table (aliased as `l`)
2. JOIN to `Books` where the BookID matches
3. JOIN to `Members` where the MemberID matches
4. Use `AS` to rename columns (prevents confusion when column names are the same)
5. Result is a "flattened" view of all three tables combined

**C# Code:**
```csharp
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
                BookTitle = reader["BookTitle"].ToString(),
                MemberFirstName = reader["MemberFirstName"].ToString(),
                // ... etc
            };
            loans.Add(loan);
        }
    }
}
```

### Why Use JOIN Instead of Multiple Queries?

**Without JOIN (inefficient):**
```csharp
List<Loan> loans = GetAllLoans();          // 1 query
foreach (Loan loan in loans)
{
    Book book = GetBookById(loan.BookID);     // N queries
    Member member = GetMemberById(loan.MemberID); // N queries
}
// Total: 1 + 2N queries (if 100 loans = 201 queries!)
```

**With JOIN (efficient):**
```csharp
List<LoanWithDetails> loans = GetAllLoansWithDetails();  // 1 query
// Total: 1 query regardless of number of loans!
```

---

## Parameterised Queries - Preventing SQL Injection

**This is the MOST IMPORTANT security concept for any database application.**

### What is SQL Injection?

SQL injection is when an attacker inserts malicious SQL code through user input.

**Vulnerable code (NEVER DO THIS):**
```csharp
string userInput = txtSearch.Text;  // User types: ' OR '1'='1
string sql = "SELECT * FROM Books WHERE Title = '" + userInput + "'";
// Results in: SELECT * FROM Books WHERE Title = '' OR '1'='1'
// This returns ALL books!
```

**Worse example:**
```csharp
string userInput = "'; DROP TABLE Books; --";
string sql = "SELECT * FROM Books WHERE Title = '" + userInput + "'";
// Results in: SELECT * FROM Books WHERE Title = ''; DROP TABLE Books; --'
// This DELETES YOUR ENTIRE TABLE!
```

### The Solution: Parameterised Queries

**Safe code (ALWAYS DO THIS):**
```csharp
string sql = "SELECT * FROM Books WHERE Title = @Title";

using (SqliteCommand cmd = new SqliteCommand(sql, conn))
{
    cmd.Parameters.AddWithValue("@Title", userInput);
    // SQLite treats the entire input as a literal value
    // Even if user enters "'; DROP TABLE Books; --"
    // It searches for a book literally titled "'; DROP TABLE Books; --"
}
```

### How Parameters Work

1. The SQL and data are sent **separately** to the database
2. The database engine knows `@Title` is a **placeholder for data**
3. Whatever value you provide is treated as **pure data, never code**
4. Special characters are automatically **escaped**

### Examples from This Project

**INSERT with parameters:**
```csharp
string sql = @"
    INSERT INTO Members (FirstName, LastName, Email, MemberType)
    VALUES (@FirstName, @LastName, @Email, @MemberType)";

cmd.Parameters.AddWithValue("@FirstName", member.FirstName);
cmd.Parameters.AddWithValue("@LastName", member.LastName);
cmd.Parameters.AddWithValue("@Email", member.Email);
cmd.Parameters.AddWithValue("@MemberType", member.MemberType);
```

**UPDATE with parameters:**
```csharp
string sql = @"
    UPDATE Books
    SET Title = @Title, ISBN = @ISBN, YearPublished = @YearPublished
    WHERE BookID = @BookID";

cmd.Parameters.AddWithValue("@Title", book.Title);
cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
cmd.Parameters.AddWithValue("@YearPublished", book.YearPublished);
cmd.Parameters.AddWithValue("@BookID", book.BookID);
```

**LIKE with parameters (for search):**
```csharp
string sql = @"SELECT * FROM Books WHERE Title LIKE @SearchTerm";

// Add wildcards to the VALUE, not the SQL
cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchText}%");
```

---

## Working with NULL Values

NULL represents "no value" or "unknown" - it's different from empty string or zero.

### NULL in SQL

**Checking for NULL:**
```sql
-- CORRECT: Use IS NULL / IS NOT NULL
SELECT * FROM Loans WHERE ReturnDate IS NULL;

-- INCORRECT: This doesn't work!
SELECT * FROM Loans WHERE ReturnDate = NULL;
```

### NULL in C# with SQLite

**Reading NULL values:**
```csharp
// Check if the value is NULL before converting
if (reader["ReturnDate"] == DBNull.Value)
{
    loan.ReturnDate = null;  // C# null
}
else
{
    loan.ReturnDate = DateTime.Parse(reader["ReturnDate"].ToString());
}

// Shorthand using ternary operator:
loan.ReturnDate = reader["ReturnDate"] == DBNull.Value
    ? (DateTime?)null
    : DateTime.Parse(reader["ReturnDate"].ToString());
```

**Writing NULL values:**
```csharp
// If the C# nullable has a value, use it; otherwise, write DBNull
if (loan.ReturnDate.HasValue)
{
    cmd.Parameters.AddWithValue("@ReturnDate", loan.ReturnDate.Value.ToString("yyyy-MM-dd"));
}
else
{
    cmd.Parameters.AddWithValue("@ReturnDate", DBNull.Value);
}
```

### Nullable Types in C#

C# uses `?` to indicate a nullable value type:

```csharp
// Regular DateTime - cannot be null
DateTime loanDate;

// Nullable DateTime - can be null
DateTime? returnDate;

// Checking if it has a value
if (returnDate.HasValue)
{
    DateTime actualDate = returnDate.Value;
}
else
{
    Console.WriteLine("No return date set");
}
```

---

## Design Decisions and Alternatives

### Why Static DatabaseHelper Class?

**Our approach:**
```csharp
public class DatabaseHelper
{
    public static List<Book> GetAllBooks() { ... }
    public static int InsertBook(Book book) { ... }
}

// Usage anywhere:
List<Book> books = DatabaseHelper.GetAllBooks();
```

**Pros:**
- Simple to use - no object creation needed
- Single point of access for all database operations
- Easy for students to understand

**Cons:**
- Harder to unit test
- Less flexible for advanced scenarios

**Alternative - Instance-based Repository Pattern:**
```csharp
public interface IBookRepository
{
    List<Book> GetAll();
    Book GetById(int id);
    int Insert(Book book);
}

public class SqliteBookRepository : IBookRepository
{
    private string connectionString;

    public SqliteBookRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public List<Book> GetAll() { ... }
}
```

**When to use:** Larger projects, teams, testable code. More complex but more flexible.

### Why Store Dates as TEXT?

SQLite doesn't have a native DATE type. Options:

| Approach | Example | Pros | Cons |
|----------|---------|------|------|
| **TEXT (ISO 8601)** | "2024-03-15" | Human-readable, sorts correctly | String manipulation needed |
| INTEGER (Unix timestamp) | 1710460800 | Compact, easy arithmetic | Not human-readable |
| REAL (Julian day) | 2460385.5 | SQLite date functions work | Complex to understand |

**We chose TEXT (ISO 8601)** because:
- Easy to read when debugging
- Sorts alphabetically = sorts chronologically
- Easy to convert to/from C# DateTime
- Format: `YYYY-MM-DD` ensures consistent ordering

### Why Separate Entity and View Model Classes?

**Entity class (Loan):** Matches the database table structure
```csharp
public class Loan
{
    public int BookID { get; set; }     // Just the foreign key
    public int MemberID { get; set; }   // Just the foreign key
}
```

**View Model class (LoanWithDetails):** Optimised for display
```csharp
public class LoanWithDetails
{
    public int BookID { get; set; }
    public string BookTitle { get; set; }     // From JOIN
    public string MemberFullName { get; set; } // Computed
    public string Status { get; set; }         // Computed
}
```

**Why separate?**
- **Single Responsibility:** Entity represents database structure; View Model represents UI needs
- **Clarity:** Makes JOIN queries explicit and understandable
- **Flexibility:** Can create multiple view models for different screens
- **Performance:** One JOIN query instead of multiple SELECTs

---

## Using DB Browser for SQLite

**DB Browser for SQLite** is a free, visual tool for working with SQLite databases. It's invaluable for debugging and understanding your data.

### Download and Installation

**Download:** https://sqlitebrowser.org/

**Important for school networks:** DB Browser has a **portable version** that doesn't require installation:
1. Download the "Portable" version (ZIP file)
2. Extract to a USB drive or local folder
3. Run directly - no admin rights needed!

### Finding Your Database File

Your database file is in the `bin\Debug` folder of your project:
```
YourProject\
├── bin\
│   └── Debug\
│       └── net8.0-windows\
│           └── LibraryDatabase.db  ← Open this file
```

### Useful Features

**1. Browse Data Tab**
- View all data in any table
- Sort by clicking column headers
- Edit data directly (useful for testing)

**2. Execute SQL Tab**
- Run any SQL query
- Test queries before putting them in code
- Great for learning SQL

**3. Database Structure Tab**
- View all tables and their columns
- See indexes and constraints
- Understand relationships

### Example Queries to Try

```sql
-- See all books with their authors
SELECT b.Title, a.FirstName, a.LastName
FROM Books b
INNER JOIN BookAuthors ba ON b.BookID = ba.BookID
INNER JOIN Authors a ON ba.AuthorID = a.AuthorID
ORDER BY b.Title;

-- Find overdue loans
SELECT b.Title, m.FirstName, m.LastName, l.DueDate
FROM Loans l
INNER JOIN Books b ON l.BookID = b.BookID
INNER JOIN Members m ON l.MemberID = m.MemberID
WHERE l.ReturnDate IS NULL
  AND l.DueDate < date('now');

-- Count loans per member
SELECT m.FirstName, m.LastName, COUNT(*) as LoanCount
FROM Members m
LEFT JOIN Loans l ON m.MemberID = l.MemberID
GROUP BY m.MemberID
ORDER BY LoanCount DESC;

-- Books that have never been borrowed
SELECT b.Title
FROM Books b
LEFT JOIN Loans l ON b.BookID = l.BookID
WHERE l.LoanID IS NULL;
```

---

## Common Mistakes to Avoid

### 1. String Concatenation in SQL (SQL Injection)

```csharp
// NEVER do this!
string sql = "SELECT * FROM Books WHERE Title = '" + userInput + "'";

// ALWAYS use parameters
string sql = "SELECT * FROM Books WHERE Title = @Title";
cmd.Parameters.AddWithValue("@Title", userInput);
```

### 2. Forgetting WHERE in UPDATE/DELETE

```sql
-- CATASTROPHE: Changes ALL books
UPDATE Books SET Title = 'Oops'

-- CORRECT: Changes one book
UPDATE Books SET Title = 'Correct' WHERE BookID = 5
```

### 3. Not Closing Connections

```csharp
// BAD: Connection might not close on error
SqliteConnection conn = new SqliteConnection(connectionString);
conn.Open();
// ... if error here, connection leaks
conn.Close();

// GOOD: using statement guarantees cleanup
using (SqliteConnection conn = new SqliteConnection(connectionString))
{
    conn.Open();
    // Connection closes automatically, even on error
}
```

### 4. Not Handling NULL Values

```csharp
// CRASH: If ReturnDate is NULL
DateTime returnDate = DateTime.Parse(reader["ReturnDate"].ToString());

// SAFE: Check for NULL first
DateTime? returnDate = reader["ReturnDate"] == DBNull.Value
    ? null
    : DateTime.Parse(reader["ReturnDate"].ToString());
```

### 5. Using = Instead of IS NULL

```sql
-- WRONG: This finds nothing
SELECT * FROM Loans WHERE ReturnDate = NULL

-- CORRECT: This finds unreturned loans
SELECT * FROM Loans WHERE ReturnDate IS NULL
```

### 6. Wrong Date Format

```csharp
// WRONG: UK format doesn't sort correctly
"15/03/2024"  // Sorts as text: 15/03/2024 > 14/04/2024 (wrong!)

// CORRECT: ISO 8601 sorts correctly
"2024-03-15"  // Sorts as text: 2024-03-15 < 2024-04-14 (correct!)
```

---

## SQL Quick Reference

### Data Definition Language (DDL)

| Command | Purpose | Example |
|---------|---------|---------|
| `CREATE TABLE` | Create new table | `CREATE TABLE Books (...)` |
| `DROP TABLE` | Delete table | `DROP TABLE Books` |
| `ALTER TABLE` | Modify table | `ALTER TABLE Books ADD Column TEXT` |

### Data Manipulation Language (DML)

| Command | Purpose | Example |
|---------|---------|---------|
| `INSERT` | Add new row | `INSERT INTO Books VALUES (...)` |
| `SELECT` | Read data | `SELECT * FROM Books` |
| `UPDATE` | Modify existing row | `UPDATE Books SET Title = @Title WHERE BookID = @ID` |
| `DELETE` | Remove row | `DELETE FROM Books WHERE BookID = @ID` |

### Common Clauses

| Clause | Purpose | Example |
|--------|---------|---------|
| `WHERE` | Filter rows | `WHERE BookID = 5` |
| `ORDER BY` | Sort results | `ORDER BY Title ASC` |
| `LIMIT` | Limit results | `LIMIT 10` |
| `LIKE` | Pattern matching | `WHERE Title LIKE '%Potter%'` |
| `IN` | Multiple values | `WHERE BookID IN (1, 2, 3)` |
| `BETWEEN` | Range | `WHERE Year BETWEEN 2000 AND 2024` |
| `IS NULL` | Check for NULL | `WHERE ReturnDate IS NULL` |

### Aggregate Functions

| Function | Purpose | Example |
|----------|---------|---------|
| `COUNT()` | Count rows | `SELECT COUNT(*) FROM Books` |
| `SUM()` | Total values | `SELECT SUM(Price) FROM Books` |
| `AVG()` | Average value | `SELECT AVG(YearPublished) FROM Books` |
| `MIN()` | Minimum value | `SELECT MIN(YearPublished) FROM Books` |
| `MAX()` | Maximum value | `SELECT MAX(YearPublished) FROM Books` |

### JOIN Types

| Join Type | Returns |
|-----------|---------|
| `INNER JOIN` | Only matching rows from both tables |
| `LEFT JOIN` | All rows from left table, matching from right |
| `RIGHT JOIN` | All rows from right table, matching from left |
| `FULL JOIN` | All rows from both tables |

---

## Summary

This document covered:

1. **Why SQLite** - Ideal for A-Level NEA projects due to simplicity and portability
2. **Connecting** - Use `using` statements and connection strings
3. **DDL** - Creating tables with appropriate constraints
4. **CRUD** - The four fundamental database operations
5. **JOINs** - Combining data from multiple tables efficiently
6. **Parameterised Queries** - CRITICAL for security (prevents SQL injection)
7. **NULL Handling** - Working with missing/unknown values
8. **Design Decisions** - Understanding why this project is structured as it is
9. **DB Browser** - Essential tool for debugging and learning

**Remember:** Your NEA should demonstrate YOUR understanding of these concepts. Use this exemplar to learn, but implement your own original solutions for your project.

---

**Document created by:** Claude AI (Anthropic)
**Purpose:** Educational resource for AQA 7517 A-Level Computer Science NEA
**Last updated:** 2024
