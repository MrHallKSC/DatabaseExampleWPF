# Library Management System - Database Exemplar Project

## AQA 7517 Computer Science A-Level NEA Exemplar

This is a comprehensive WPF application demonstrating database programming concepts for A-Level Computer Science students. The project showcases best practices for database design, SQL operations, and object-oriented programming in C#.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features Demonstrated](#features-demonstrated)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Project Structure](#project-structure)
- [Database Schema](#database-schema)
- [How to Use](#how-to-use)
- [Key Concepts for Students](#key-concepts-for-students)
- [Troubleshooting](#troubleshooting)
- [Learning Resources](#learning-resources)
- [Credits](#credits)

---

## 🎯 Overview

This Library Management System is designed as an exemplar project for AQA 7517 Computer Science NEA coursework. It demonstrates professional database programming practices including:

- **Database Design**: Normalized database with multiple related tables
- **CRUD Operations**: Create, Read, Update, and Delete functionality
- **Relationships**: Both many-to-many and one-to-many relationships
- **Security**: Parameterized queries to prevent SQL injection
- **Validation**: Client-side and model-level data validation
- **User Interface**: Professional WPF application with data binding
- **Search Functionality**: Database queries with LIKE operator
- **JOIN Queries**: Complex queries combining multiple tables

---

## ✨ Features Demonstrated

### Database Operations

✅ **SQLite Database Integration**
- Creating and managing a local SQLite database
- DDL operations (CREATE TABLE with constraints)
- Connection management with proper disposal patterns

✅ **Full CRUD Operations**
- **Create**: Inserting new records with auto-generated IDs
- **Read**: Selecting data with and without JOIN queries
- **Update**: Modifying existing records
- **Delete**: Removing records with cascade effects

✅ **Parameterized Queries**
- Protection against SQL injection attacks
- Proper parameter binding for all data types
- Handling nullable values (DateTime?)

✅ **Relationships**
- **Many-to-Many**: Books ↔ Authors via BookAuthors junction table
- **One-to-Many**: Members → Loans, Books → Loans
- Foreign key constraints and referential integrity

✅ **Advanced Queries**
- INNER JOIN across multiple tables
- Search with LIKE operator and wildcards
- Filtering (active loans, overdue loans, returned loans)
- ORDER BY for sorted results

### Object-Oriented Programming

✅ **Model Classes**
- Encapsulation with properties
- Data validation methods
- Computed properties
- Constructors (default and parameterized)

✅ **View Models**
- Separation of database entities and display models
- LoanWithDetails demonstrates flattening joined data

✅ **Separation of Concerns**
- Database operations in dedicated DatabaseHelper class
- Business logic in model classes
- UI logic in window code-behind files

### User Interface

✅ **WPF Controls**
- DataGrid for tabular data display
- ComboBox for dropdown selections
- DatePicker for date input
- ListBox for related data display
- CheckBox for boolean options

✅ **Data Binding**
- Binding collections to ItemsSource
- DisplayMemberPath and SelectedValuePath
- String formatting in bindings
- Conditional formatting (DataTriggers)

✅ **User Experience**
- Form validation with user-friendly error messages
- Confirmation dialogs for destructive actions
- Real-time updates after database operations
- Visual feedback (button states, row colors)

---

## 📦 Prerequisites

### Software Requirements

- **Visual Studio 2019 or later** (Community Edition is free)
  - Download from: https://visualstudio.microsoft.com/
  
- **.NET Framework 4.7.2 or later** (usually included with Visual Studio)

- **DB Browser for SQLite** (optional but recommended for viewing database)
  - Download from: https://sqlitebrowser.org/

### Knowledge Prerequisites for Students

- Basic C# programming (variables, loops, conditional statements)
- Object-oriented programming concepts (classes, objects, properties)
- Basic SQL knowledge (SELECT, INSERT, UPDATE, DELETE)
- Understanding of WPF basics (XAML, event handlers)

---

## 🚀 Installation

### Step 1: Clone or Download the Project

1. Download the project files to your computer
2. Extract the ZIP file if necessary
3. Open Visual Studio

### Step 2: Open the Solution

1. In Visual Studio, go to **File → Open → Project/Solution**
2. Navigate to the project folder
3. Select the `.sln` (solution) file
4. Click **Open**

### Step 3: Install NuGet Packages

**IMPORTANT**: This step is critical for the project to work!

1. Right-click on the project in **Solution Explorer**
2. Select **Manage NuGet Packages...**
3. Click the **Browse** tab
4. Search for `Microsoft.Data.Sqlite`
5. Select **Microsoft.Data.Sqlite** by Microsoft
6. Click **Install**
7. Accept any license agreements

**Alternative Method** (using Package Manager Console):
1. Go to **Tools → NuGet Package Manager → Package Manager Console**
2. Type: `Install-Package Microsoft.Data.Sqlite`
3. Press **Enter**

### Step 4: Build the Project

1. Click **Build → Build Solution** (or press `Ctrl+Shift+B`)
2. Check the **Output** window for any errors
3. All errors should be resolved after installing the NuGet package

### Step 5: Run the Application

1. Press **F5** or click the **Start** button (green play button)
2. The main window should appear
3. Click **"Create Database & Tables"** to initialize the database

---

## 📁 Project Structure
```
DatabaseExampleWPF/
│
├── Models/                          # Data model classes
│   ├── Book.cs                      # Book entity with validation
│   ├── Author.cs                    # Author entity
│   ├── Member.cs                    # Member entity with email validation
│   ├── Loan.cs                      # Loan entity with date handling
│   └── LoanWithDetails.cs           # View model for joined data
│
├── Database/                        # Database operations
│   └── DatabaseHelper.cs            # All CRUD operations and queries
│
├── Windows/                         # UI Windows
│   ├── MainWindow.xaml              # Main navigation window
│   ├── MainWindow.xaml.cs
│   ├── BooksWindow.xaml             # Book management (many-to-many demo)
│   ├── BooksWindow.xaml.cs
│   ├── AuthorsWindow.xaml           # Author management
│   ├── AuthorsWindow.xaml.cs
│   ├── MembersWindow.xaml           # Member management
│   ├── MembersWindow.xaml.cs
│   ├── LoansWindow.xaml             # Loan management (foreign keys)
│   ├── LoansWindow.xaml.cs
│   ├── SearchLoansWindow.xaml       # Search with JOIN queries
│   └── SearchLoansWindow.xaml.cs
│
├── App.xaml                         # Application configuration
├── App.xaml.cs
└── LibraryDatabase.db               # SQLite database file (created at runtime)
```

---

## 🗄️ Database Schema

The database consists of **5 tables**:

### Books Table
| Column | Type | Constraints |
|--------|------|-------------|
| BookID | INTEGER | PRIMARY KEY, AUTOINCREMENT |
| Title | TEXT | NOT NULL |
| ISBN | TEXT | |
| YearPublished | INTEGER | NOT NULL |

### Authors Table
| Column | Type | Constraints |
|--------|------|-------------|
| AuthorID | INTEGER | PRIMARY KEY, AUTOINCREMENT |
| FirstName | TEXT | NOT NULL |
| LastName | TEXT | NOT NULL |

### BookAuthors Table (Junction Table)
| Column | Type | Constraints |
|--------|------|-------------|
| BookID | INTEGER | FOREIGN KEY → Books(BookID), ON DELETE CASCADE |
| AuthorID | INTEGER | FOREIGN KEY → Authors(AuthorID), ON DELETE CASCADE |
| | | PRIMARY KEY (BookID, AuthorID) |

**Purpose**: Implements many-to-many relationship between Books and Authors

### Members Table
| Column | Type | Constraints |
|--------|------|-------------|
| MemberID | INTEGER | PRIMARY KEY, AUTOINCREMENT |
| FirstName | TEXT | NOT NULL |
| LastName | TEXT | NOT NULL |
| Email | TEXT | NOT NULL |
| MemberType | TEXT | NOT NULL (Student/Teacher/Staff) |

### Loans Table
| Column | Type | Constraints |
|--------|------|-------------|
| LoanID | INTEGER | PRIMARY KEY, AUTOINCREMENT |
| BookID | INTEGER | FOREIGN KEY → Books(BookID) |
| MemberID | INTEGER | FOREIGN KEY → Members(MemberID) |
| LoanDate | TEXT | NOT NULL (ISO 8601 format) |
| DueDate | TEXT | NOT NULL |
| ReturnDate | TEXT | Nullable |

**Relationships**:
- One-to-Many: Member → Loans (one member can have many loans)
- One-to-Many: Book → Loans (one book can have many loans over time)

---

## 📖 How to Use

### First-Time Setup

1. **Launch the application**
2. **Click "Create Database & Tables"** on the main window
   - This creates `LibraryDatabase.db` in the application folder
   - Creates all 5 tables with proper constraints
   - Safe to run multiple times (uses `IF NOT EXISTS`)

### Adding Data

**Recommended Order**:
1. **Add Authors first** (Books → Manage Authors)
2. **Add Books** (Books → Manage Books)
3. **Link Books to Authors** (in Books window, use author management section)
4. **Add Members** (Members → Manage Members)
5. **Create Loans** (Loans → Manage Loans)

### Managing Books and Authors (Many-to-Many Relationship)

**Books Window demonstrates the many-to-many relationship**:

1. Click **"📚 Manage Books"** from main window
2. Add a book using the form on the right
3. Select the book from the list
4. In the "Manage Authors" section:
   - Select an author from the dropdown
   - Click "➕ Add Author to Book"
   - The author appears in the list
5. To remove: Select author in list, click "➖ Remove Selected Author"

**Example**: Add "Good Omens" by Terry Pratchett AND Neil Gaiman
1. Add Terry Pratchett in Authors window
2. Add Neil Gaiman in Authors window
3. Add "Good Omens" in Books window
4. Select "Good Omens"
5. Add Terry Pratchett to the book
6. Add Neil Gaiman to the book
7. Now the book shows both authors!

### Creating Loans (Foreign Key Relationships)

**Loans Window demonstrates foreign keys**:

1. Click **"📋 Manage Loans"** from main window
2. **Select a Book** from the dropdown (shows all books)
3. **Select a Member** from the dropdown (shows all members)
4. **Set Loan Date** (defaults to today)
5. **Set Due Date** (defaults to 2 weeks from today)
6. Click **"💾 Save Loan"**

**To mark a book as returned**:
- Method 1: Edit the loan and check "Book has been returned"
- Method 2: Select loan in list, click "📥 Return Selected Book"

### Searching with JOIN Queries

**Search Loans Window demonstrates JOIN queries**:

1. Click **"🔍 View All Loans (with Search)"** from main window
2. The grid shows data from **three tables joined together**:
   - Loan information (dates, status)
   - Book information (title, ISBN)
   - Member information (name, email, type)
3. **Search**: Type book title or member name, click "Search"
4. **Quick Filters**: Click buttons to filter by status
   - All Loans
   - Active Loans (not returned)
   - Overdue Loans (past due date, not returned)
   - Returned Loans

**Notice**: Overdue loans show in red, returned loans in green!

### Viewing the Database Directly

Use **DB Browser for SQLite** to view/edit the database:

1. Open DB Browser for SQLite
2. Click **Open Database**
3. Navigate to your project's `bin/Debug` folder
4. Select `LibraryDatabase.db`
5. Click **Browse Data** tab to see table contents
6. Click **Execute SQL** tab to run custom queries

**Example Queries to Try**:
```sql
-- View all books with their authors
SELECT Books.Title, Authors.FirstName, Authors.LastName
FROM Books
INNER JOIN BookAuthors ON Books.BookID = BookAuthors.BookID
INNER JOIN Authors ON BookAuthors.AuthorID = Authors.AuthorID;

-- Find overdue loans
SELECT Books.Title, Members.FirstName, Members.LastName, Loans.DueDate
FROM Loans
INNER JOIN Books ON Loans.BookID = Books.BookID
INNER JOIN Members ON Loans.MemberID = Members.MemberID
WHERE Loans.ReturnDate IS NULL 
  AND Loans.DueDate < date('now');
```

---

## 🎓 Key Concepts for Students

### 1. Parameterized Queries (SQL Injection Prevention)

**❌ NEVER do this** (vulnerable to SQL injection):
```csharp
string sql = "INSERT INTO Books VALUES ('" + title + "')";
```

**✅ ALWAYS do this** (safe):
```csharp
string sql = "INSERT INTO Books (Title) VALUES (@Title)";
cmd.Parameters.AddWithValue("@Title", title);
```

**Why?** If a user enters: `'; DROP TABLE Books; --` as a title, the first method would execute it! Parameterized queries prevent this.

### 2. Many-to-Many Relationships

**Problem**: A book can have multiple authors, and an author can write multiple books.

**Solution**: Use a **junction table** (BookAuthors)

- Books table: stores books
- Authors table: stores authors
- BookAuthors table: stores which authors wrote which books

**Example Data**:

Books:
| BookID | Title |
|--------|-------|
| 1 | Good Omens |
| 2 | The Colour of Magic |

Authors:
| AuthorID | Name |
|----------|------|
| 1 | Terry Pratchett |
| 2 | Neil Gaiman |

BookAuthors:
| BookID | AuthorID |
|--------|----------|
| 1 | 1 | (Good Omens by Terry Pratchett)
| 1 | 2 | (Good Omens by Neil Gaiman)
| 2 | 1 | (The Colour of Magic by Terry Pratchett)

### 3. JOIN Queries

**Purpose**: Combine data from multiple tables

**Example** - Get all loans with book titles and member names:
```sql
SELECT 
    Loans.LoanID,
    Books.Title AS BookTitle,
    Members.FirstName || ' ' || Members.LastName AS MemberName,
    Loans.DueDate
FROM Loans
INNER JOIN Books ON Loans.BookID = Books.BookID
INNER JOIN Members ON Loans.MemberID = Members.MemberID
```

**Without JOIN**, you'd need multiple queries and manual matching!

### 4. Data Validation

**Three Levels of Validation**:

1. **UI Level** (BooksWindow.xaml.cs):
```csharp
   if (string.IsNullOrWhiteSpace(txtTitle.Text)) {
       MessageBox.Show("Please enter a title");
       return;
   }
```

2. **Model Level** (Book.cs):
```csharp
   public bool IsValid() {
       if (string.IsNullOrWhiteSpace(Title)) return false;
       if (YearPublished < 1000) return false;
       return true;
   }
```

3. **Database Level** (CREATE TABLE):
```sql
   CREATE TABLE Books (
       Title TEXT NOT NULL,  -- Database enforces this!
       ...
   )
```

### 5. Using Statement (Resource Management)

**Always use `using` for database connections**:
```csharp
using (SqliteConnection conn = new SqliteConnection(connectionString))
{
    conn.Open();
    // ... do database work ...
}  // Connection automatically closed and disposed here!
```

**Why?** Ensures the connection is properly closed even if an error occurs.

### 6. Try-Catch Error Handling

**Wrap database operations in try-catch**:
```csharp
try
{
    // Database operation that might fail
    DatabaseHelper.InsertBook(book);
}
catch (Exception ex)
{
    // Handle the error gracefully
    MessageBox.Show($"Error: {ex.Message}");
}
```

**Why?** Database operations can fail (connection issues, constraint violations, etc.)

### 7. Nullable Types

**DateTime?** means "DateTime that can be null"
```csharp
public DateTime? ReturnDate { get; set; }  // Note the ?

// Check if it has a value
if (loan.ReturnDate.HasValue)
{
    // Access the value
    DateTime date = loan.ReturnDate.Value;
}
```

**Why?** Some loans haven't been returned yet, so ReturnDate should be null.

### 8. Data Binding in WPF

**Connect data to UI without manual updates**:
```csharp
// Load data
List<Book> books = DatabaseHelper.GetAllBooks();

// Bind to DataGrid - it automatically displays the data!
dgBooks.ItemsSource = books;
```

The DataGrid columns are bound to Book properties:
```xml
<DataGridTextColumn Header="Title" Binding="{Binding Title}" />
```

### 9. Foreign Keys and Referential Integrity

**Foreign keys ensure data consistency**:
```sql
CREATE TABLE Loans (
    BookID INTEGER NOT NULL,
    FOREIGN KEY (BookID) REFERENCES Books(BookID)
)
```

**Effect**:
- Can't create a loan for a non-existent book
- Prevents "orphaned" records
- CASCADE DELETE: deleting a book deletes its loans

### 10. Computed Properties

**Properties calculated from other properties** (not stored in database):
```csharp
public bool IsOverdue
{
    get
    {
        if (IsReturned) return false;
        return DateTime.Today > DueDate;
    }
}
```

**Why?** Status changes based on the current date - can't store it!

---

## 🔧 Troubleshooting

### "Unable to load DLL 'e_sqlite3'"

**Problem**: SQLite native libraries not found

**Solution**:
1. Uninstall `System.Data.SQLite` if installed
2. Install `Microsoft.Data.Sqlite` instead
3. Use the updated DatabaseHelper.cs provided

### "Object reference not set to an instance of an object"

**Common Causes**:
1. **Database not created**: Click "Create Database & Tables" first
2. **No items selected**: Check if `dgBooks.SelectedItem` is null before using it
3. **ComboBox not populated**: Ensure `LoadBooks()` and `LoadMembers()` are called

**Fix**: Add null checks:
```csharp
if (dgBooks.SelectedItem != null)
{
    Book book = (Book)dgBooks.SelectedItem;
    // ... use book
}
```

### Books/Members don't appear in dropdown

**Problem**: ComboBox shows empty or shows "DatabaseExampleWPF.Models.Book"

**Solution**: Check ComboBox properties:
```xml
<ComboBox DisplayMemberPath="Title"      <!-- What to show -->
          SelectedValuePath="BookID"     <!-- What value to use -->
          ItemsSource="{Binding}" />
```

### "Foreign key constraint failed"

**Problem**: Trying to create a loan for a deleted book/member

**Solution**:
1. Ensure books and members exist before creating loans
2. Use ON DELETE CASCADE to auto-delete related records
3. Check foreign key values before inserting

### Database file locked

**Problem**: "Database is locked" error

**Solution**:
1. Close DB Browser for SQLite if it's open
2. Ensure all database connections are closed (`using` statements)
3. Stop debugging, delete `LibraryDatabase.db`, restart app

### Changes don't appear in DataGrid

**Problem**: Added/updated data but grid doesn't update

**Solution**: Refresh the grid after changes:
```csharp
LoadBooks();  // Reloads data from database
```

---

## 📚 Learning Resources

### Official Documentation

- **Microsoft C# Documentation**: https://docs.microsoft.com/en-us/dotnet/csharp/
- **SQLite Documentation**: https://www.sqlite.org/docs.html
- **WPF Tutorial**: https://docs.microsoft.com/en-us/dotnet/desktop/wpf/

### Recommended Learning Path

1. **Week 1**: Understand model classes (Book, Author, Member, Loan)
2. **Week 2**: Study DatabaseHelper.cs - focus on one table at a time
3. **Week 3**: Learn CRUD operations (Create, Read, Update, Delete)
4. **Week 4**: Understand BooksWindow (many-to-many relationships)
5. **Week 5**: Study SearchLoansWindow (JOIN queries)
6. **Week 6**: Build your own window or feature

### Key Files to Study (in order)

1. **Book.cs** - Start here, simplest model
2. **DatabaseHelper.cs** - `CreateTables()` method
3. **DatabaseHelper.cs** - Book CRUD methods
4. **BooksWindow.xaml.cs** - UI and CRUD operations
5. **LoanWithDetails.cs** - View model concept
6. **SearchLoansWindow.xaml.cs** - JOIN queries and search

### Practice Exercises

1. **Beginner**: Add a "Publisher" field to the Books table
2. **Intermediate**: Create a "Reservations" feature (members can reserve books)
3. **Advanced**: Add a "Fines" feature (calculate late fees for overdue books)
4. **Challenge**: Create an export feature (save data to CSV)

---

## 💡 Tips for NEA Projects

### What to Include

✅ **User Requirements**: Who will use it? What do they need?
✅ **Design Documentation**: Database schema diagrams, class diagrams
✅ **Commented Code**: Explain WHY, not just WHAT
✅ **Testing**: Test each CRUD operation, test edge cases
✅ **Evaluation**: What worked? What could be improved?

### Common Mistakes to Avoid

❌ No validation (always validate user input!)
❌ SQL injection vulnerabilities (use parameters!)
❌ Poor error handling (always use try-catch for database operations)
❌ No comments (examiners need to understand your code)
❌ Over-ambitious scope (a small working project is better than incomplete large one)

### Documentation Tips

- **Screenshot everything**: UI, database structure, testing results
- **Explain decisions**: Why SQLite? Why these tables?
- **Show SQL queries**: Include example SELECT, INSERT, UPDATE, DELETE
- **Test thoroughly**: Create a test plan and evidence
- **Annotate code**: Use XML comments (`///`) for methods

---

## 🙏 Credits

**Created by**: Oliver (Computer Science Teacher)
**Purpose**: AQA 7517 A-Level Computer Science NEA Exemplar
**Framework**: WPF (.NET Framework 4.7.2+)
**Database**: SQLite with Microsoft.Data.Sqlite
**License**: Free to use for educational purposes

### Acknowledgments

- AQA for the A-Level Computer Science specification
- Microsoft for WPF and SQLite libraries
- SQLite team for the excellent database engine
- DB Browser for SQLite team for the database viewer tool

---

## 📝 Version History

**Version 1.0** (November 2024)
- Initial release
- 5 model classes
- Complete CRUD operations
- Many-to-many and one-to-many relationships
- JOIN queries and search functionality
- Comprehensive documentation and comments

---

## 🤝 Support

For questions or issues:
1. Check the **Troubleshooting** section above
2. Review the **code comments** in each file
3. Consult the **Learning Resources** section
4. Ask your teacher for guidance

---

## 📄 License

This project is released for **educational purposes only**. Students may:
- Study the code for learning
- Use it as a reference for NEA projects
- Modify it for practice

Students **must not**:
- Submit this code as their own NEA work
- Copy code without understanding it
- Share solutions without attribution

**Remember**: The purpose is to LEARN, not to copy!

---

**Good luck with your NEA! Remember: understanding is more important than working code. Make sure you can explain every line!** 🚀