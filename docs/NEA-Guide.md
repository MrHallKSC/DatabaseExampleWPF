# AQA 7517 NEA Guide: Planning Your Database Project in C#

A comprehensive, step-by-step guide for A-Level Computer Science students planning a database-driven application for their Non-Exam Assessment (NEA).

**This guide uses the Library Management System as a worked example throughout.**

---

## Table of Contents

1. [Understanding the NEA Requirements](#understanding-the-nea-requirements)
2. [Stage 1: Analysis](#stage-1-analysis)
3. [Stage 2: Design](#stage-2-design)
4. [Stage 3: Prototyping](#stage-3-prototyping)
5. [Stage 4: Development](#stage-4-development)
6. [Stage 5: Testing](#stage-5-testing)
7. [Stage 6: Evaluation](#stage-6-evaluation)
8. [Project Complexity Levels](#project-complexity-levels)
9. [Tips for Success](#tips-for-success)

---

## Understanding the NEA Requirements

### AQA 7517 Specification Overview

Before you begin, **read the AQA specification carefully**. The NEA is worth **20% of your A-Level** and must demonstrate:

1. **Analysis** - Understanding the problem and requirements
2. **Documented Design** - Planning before coding
3. **Technical Solution** - Working, well-coded software
4. **Testing** - Evidence that your solution works
5. **Evaluation** - Critical reflection on your work

### Mark Allocation (Approximate)

| Section | Marks | What Examiners Look For |
|---------|-------|------------------------|
| Analysis | 9 | Clear problem definition, stakeholder input, measurable objectives |
| Documented Design | 12 | Detailed designs that could be followed by another programmer |
| Technical Solution | 42 | Complex techniques, good coding style, working solution |
| Testing | 8 | Comprehensive test plan with evidence |
| Evaluation | 4 | Honest assessment against objectives |

### Key Requirements

- **Real User/Client:** You MUST have a real person to work with (not yourself or an imaginary client)
- **Original Work:** Your code must be YOUR work, not copied
- **Appropriate Complexity:** Must demonstrate Group A technical skills (see specification)
- **Evidence:** Document everything - screenshots, meeting notes, email exchanges

### Referencing and Academic Integrity

**This is extremely important.** Your NEA must be your own work, but you ARE expected to learn from tutorials, documentation, and other resources. The key is **honesty and proper referencing**.

#### What You MUST Reference

Keep a **references log** throughout your project. Record:

| Resource Type | What to Record | Example |
|---------------|----------------|---------|
| **Tutorials** | Title, URL, date accessed | "WPF DataGrid Tutorial", https://docs.microsoft.com/..., accessed 15/01/2024 |
| **Documentation** | Source, section | Microsoft Docs - SqliteConnection Class |
| **Books** | Author, title, pages | Troelsen, A. "Pro C# 9", pp. 245-260 |
| **Stack Overflow** | Question URL, date | https://stackoverflow.com/questions/12345, accessed 20/01/2024 |
| **YouTube videos** | Title, channel, URL | "C# SQLite Tutorial", CodeAcademy channel |
| **AI assistants** | Tool used, what it helped with | "Used ChatGPT to help debug connection string error" |

#### Code You Write Yourself (Gets Full Credit)

Code you write based on **understanding concepts** from tutorials:
- You read a tutorial about DataGrid binding
- You understand HOW it works
- You write your own code applying that concept to YOUR project
- **Reference the tutorial, but the code is yours**

#### Code You Copy (Gets NO Credit)

If you directly copy code from somewhere:
- You **must** clearly mark it with comments
- State **exactly** where it came from
- Explain **why** you used it
- **You will NOT receive marks for copied code**

```csharp
// ============================================================
// CODE COPIED FROM: https://stackoverflow.com/questions/12345
// REASON: Email validation using Regex
// I did not write this code - it is included for functionality only
// ============================================================
public static bool IsValidEmail(string email)
{
    // ... copied code here ...
}
// ============================================================
// END OF COPIED CODE
// ============================================================
```

#### The Golden Rule

**When in doubt, reference it.** It's always better to over-reference than under-reference. Examiners appreciate honesty and will give credit for work that is clearly yours, even if you learned the techniques from tutorials.

**Plagiarism = copying without acknowledgement = serious academic misconduct**

---

## Stage 1: Analysis

**Goal:** Understand what problem you're solving and for whom.

### 1.1 Finding a User/Client

**This is CRITICAL** - you need a real person who:
- Has a genuine problem that software could solve
- Will give you feedback throughout the project
- Can provide test data and validation

**Good clients for a library system:**
- School librarian
- Community library volunteer
- Book club organiser
- Small charity with a book lending scheme

**Document this:** Keep records of all conversations (dated notes, emails, screenshots of messages).

### 1.2 Information to Gather

**Interview your client and collect:**

| Information | Questions to Ask (Library Example) | Why It Matters |
|-------------|-----------------------------------|----------------|
| **Current System** | How do you currently track book loans? Paper cards? Spreadsheet? | Understand what you're replacing |
| **Problems** | Do books go missing? Is it hard to find who has what? | Identify pain points to solve |
| **Requirements** | Must you track overdue books? Multiple authors per book? | Prioritise features |
| **Data** | What information about books/members do you need? | Design your database |
| **Users** | Will just you use it, or other staff too? | Design appropriate UI |
| **Constraints** | How many books? How many members? Any school network restrictions? | Set realistic scope |

### 1.3 Documenting Analysis

**Create these documents:**

#### Problem Statement

*Library System Example:*
```
Mrs Smith, the school librarian at Greenfield Academy, currently manages book
loans using a paper-based card system. Each book has a card that is filed when
loaned out and returned when the book comes back.

This causes problems including:
- Cards get lost or misfiled
- Difficult to find who has a specific book
- No easy way to identify overdue books
- Cannot track which books are most popular
- Time-consuming to search for available books on a topic

She needs a computerised system that will allow her to quickly record loans,
track overdue books, search for books by title or author, and manage library
member information.
```

#### Stakeholder Identification

*Library System Example:*

| Stakeholder | Role | Interest in System |
|-------------|------|-------------------|
| Mrs Smith (Librarian) | Primary user | Must be quick to use, show overdue books |
| Library Assistants | Secondary users | Need simple interface for basic operations |
| Students/Staff | Library members | Affected by loan accuracy, want fair treatment |
| Headteacher | Manager | May want reports on library usage |

#### Requirements Specification

*Library System Example:*

**Essential (Must Have):**
1. Store book records (title, ISBN, year published)
2. Store author records (first name, last name)
3. Link books to authors (including books with multiple authors)
4. Store member records (name, email, member type)
5. Record when books are loaned out
6. Record when books are returned
7. Show which books are currently on loan
8. Show overdue books
9. Search for books by title

**Desirable (Should Have):**
1. Search for books by author
2. View loan history for a member
3. View which books an author has written
4. Filter loans by status (active, returned, overdue)

**Optional (Could Have):**
1. Export overdue list to CSV
2. Print loan receipts
3. Email reminders for overdue books
4. Statistics dashboard

#### Measurable Success Criteria

**CRITICAL:** Your objectives must be **testable**. Compare:

| Bad Objective | Good Objective (Library Example) |
|---------------|----------------------------------|
| "System should be fast" | "Search results display within 2 seconds for a database of 500+ books" |
| "Easy to use" | "Librarian can record a book loan in under 30 seconds" |
| "Store lots of data" | "Database can store at least 1000 books without performance issues" |
| "Track loans properly" | "System correctly identifies all books overdue by more than 14 days" |
| "Handle multiple authors" | "Books can be linked to 1, 2, or more authors" |

*Library System - Example Success Criteria:*
1. System can store at least 1000 book records
2. System can store at least 500 member records
3. A new loan can be recorded in under 30 seconds
4. Search by title returns results within 2 seconds
5. Overdue books are correctly identified (loan > 14 days without return)
6. Books can have multiple authors, authors can have multiple books
7. System validates email format for members
8. User can view all loans for a specific member
9. Librarian (client) confirms system meets her needs

### 1.4 Analysis Checklist

Before moving to Design, ensure you have:

- [ ] Identified a real client/user (e.g., school librarian)
- [ ] Documented at least 2 conversations/meetings with client
- [ ] Written a clear problem statement
- [ ] Listed all data that needs to be stored (books, authors, members, loans)
- [ ] Identified relationships between data (book-author is many-to-many, member-loan is one-to-many)
- [ ] Written measurable success criteria (8-12 specific, testable objectives)
- [ ] Got client sign-off on requirements

---

## Stage 2: Design

**Goal:** Plan your solution in enough detail that another programmer could build it.

### 2.1 Database Design

#### Entity Identification

*Library System Example:*
```
Entities needed:
- Book (the items being loaned)
- Author (who wrote the books)
- Member (who borrows books)
- Loan (the borrowing transaction)
- BookAuthors (junction table for many-to-many relationship)
```

#### Attribute Definition

*Library System Example:*

```
Book:
- BookID (Primary Key, Auto-increment)
- Title (Text, Required)
- ISBN (Text, Optional - not all books have ISBN)
- YearPublished (Integer, Required)

Author:
- AuthorID (Primary Key, Auto-increment)
- FirstName (Text, Required)
- LastName (Text, Required)

Member:
- MemberID (Primary Key, Auto-increment)
- FirstName (Text, Required)
- LastName (Text, Required)
- Email (Text, Required, Must be valid format)
- MemberType (Text, Required - Student/Teacher/Staff)

Loan:
- LoanID (Primary Key, Auto-increment)
- BookID (Foreign Key, Required)
- MemberID (Foreign Key, Required)
- LoanDate (Date, Required)
- DueDate (Date, Required)
- ReturnDate (Date, Optional - NULL if not yet returned)

BookAuthors (Junction Table):
- BookID (Foreign Key, Part of composite PK)
- AuthorID (Foreign Key, Part of composite PK)
```

#### Relationship Identification

*Library System Example:*

| Entity A | Relationship | Entity B | Type | Explanation |
|----------|--------------|----------|------|-------------|
| Book | written by | Author | Many-to-Many | A book can have multiple authors; an author can write multiple books |
| Member | borrows | Book (via Loan) | One-to-Many | A member can have many loans; each loan is for one member |
| Book | loaned in | Loan | One-to-Many | A book can be loaned many times; each loan is for one book |

#### Entity Relationship Diagram (ERD)

*Library System ERD:*
```
┌─────────────┐       ┌──────────────┐       ┌─────────────┐
│   Authors   │       │  BookAuthors │       │    Books    │
├─────────────┤       ├──────────────┤       ├─────────────┤
│ AuthorID PK │──────<│ AuthorID FK  │>──────│ BookID PK   │
│ FirstName   │       │ BookID FK    │       │ Title       │
│ LastName    │       └──────────────┘       │ ISBN        │
└─────────────┘                              │ YearPublished│
                                             └──────┬──────┘
                                                    │
                                                    │ 1
                                                    │
                                                    │ M
                                             ┌──────┴──────┐
                                             │    Loans    │
                                             ├─────────────┤
┌─────────────┐                              │ LoanID PK   │
│   Members   │                              │ BookID FK   │
├─────────────┤       1                    M │ MemberID FK │
│ MemberID PK │──────────────────────────────│ LoanDate    │
│ FirstName   │                              │ DueDate     │
│ LastName    │                              │ ReturnDate  │
│ Email       │                              └─────────────┘
│ MemberType  │
└─────────────┘

Key:
PK = Primary Key
FK = Foreign Key
──< = Many side of relationship
1/M = One-to-Many relationship
```

**Tools for drawing ERDs:** Draw.io (free), Lucidchart, or even hand-drawn (neatly!)

#### Normalisation

*Library System - Demonstrating Normalisation:*

**Unnormalised Data (Before):**
```
LoanRecord:
| LoanID | BookTitle | Authors | MemberName | MemberEmail | LoanDate |
|--------|-----------|---------|------------|-------------|----------|
| 1 | Good Omens | Terry Pratchett, Neil Gaiman | John Smith | john@school.com | 2024-01-15 |
```

**Problems:**
- Authors field has multiple values (not atomic) - violates 1NF
- BookTitle repeated if same book loaned again - redundancy
- Member details repeated for each loan - redundancy

**1NF (First Normal Form):**
- No repeating groups
- All fields are atomic (single values)
- Separate Authors into their own records

**2NF (Second Normal Form):**
- Is in 1NF
- No partial dependencies
- Book details depend only on BookID, not LoanID

**3NF (Third Normal Form):**
- Is in 2NF
- No transitive dependencies
- MemberEmail depends on MemberID, not on LoanID

**Normalised Result:** The five separate tables (Books, Authors, BookAuthors, Members, Loans) shown above.

#### Data Dictionary

*Library System Example:*

| Table | Field | Data Type | Size | Required | Validation | Description |
|-------|-------|-----------|------|----------|------------|-------------|
| Books | BookID | INTEGER | - | Yes | Auto PK | Unique book identifier |
| Books | Title | TEXT | 200 | Yes | Not empty | Book title |
| Books | ISBN | TEXT | 20 | No | - | ISBN (optional) |
| Books | YearPublished | INTEGER | - | Yes | 1000-current year | Publication year |
| Authors | AuthorID | INTEGER | - | Yes | Auto PK | Unique author identifier |
| Authors | FirstName | TEXT | 50 | Yes | Not empty | Author's first name |
| Authors | LastName | TEXT | 50 | Yes | Not empty | Author's last name |
| Members | MemberID | INTEGER | - | Yes | Auto PK | Unique member identifier |
| Members | FirstName | TEXT | 50 | Yes | Not empty | Member's first name |
| Members | LastName | TEXT | 50 | Yes | Not empty | Member's last name |
| Members | Email | TEXT | 100 | Yes | Valid email format | Contact email |
| Members | MemberType | TEXT | 20 | Yes | Student/Teacher/Staff | Type of member |
| Loans | LoanID | INTEGER | - | Yes | Auto PK | Unique loan identifier |
| Loans | BookID | INTEGER | - | Yes | FK to Books | Which book was loaned |
| Loans | MemberID | INTEGER | - | Yes | FK to Members | Who borrowed it |
| Loans | LoanDate | TEXT | 10 | Yes | Valid date | When borrowed |
| Loans | DueDate | TEXT | 10 | Yes | Valid date, after LoanDate | When due back |
| Loans | ReturnDate | TEXT | 10 | No | Valid date or NULL | When returned (NULL if not returned) |

### 2.2 Algorithm Design

#### Pseudocode or Flowcharts

*Library System - Search Algorithm:*
```
FUNCTION SearchBooks(searchTerm)
    results = empty list

    query = "SELECT * FROM Books WHERE Title LIKE '%' + searchTerm + '%'"

    FOR each book in query results
        ADD book TO results
    END FOR

    RETURN results sorted by Title
END FUNCTION
```

*Library System - Loan Validation Algorithm:*
```
FUNCTION ValidateLoan(loan)
    errors = empty list

    IF loan.BookID is empty THEN
        ADD "Please select a book" TO errors
    END IF

    IF loan.MemberID is empty THEN
        ADD "Please select a member" TO errors
    END IF

    IF loan.LoanDate is empty THEN
        ADD "Please enter loan date" TO errors
    END IF

    IF loan.DueDate is empty THEN
        ADD "Please enter due date" TO errors
    ELSE IF loan.DueDate <= loan.LoanDate THEN
        ADD "Due date must be after loan date" TO errors
    END IF

    IF loan.ReturnDate is not empty THEN
        IF loan.ReturnDate < loan.LoanDate THEN
            ADD "Return date cannot be before loan date" TO errors
        END IF
    END IF

    RETURN errors
END FUNCTION
```

*Library System - Check Overdue Algorithm:*
```
FUNCTION IsOverdue(loan)
    IF loan.ReturnDate is not empty THEN
        // Book has been returned, not overdue
        RETURN false
    END IF

    IF today's date > loan.DueDate THEN
        RETURN true
    ELSE
        RETURN false
    END IF
END FUNCTION
```

*Library System - Get Overdue Loans Algorithm:*
```
FUNCTION GetOverdueLoans()
    overdueLoans = empty list

    allLoans = GetAllLoans()

    FOR each loan in allLoans
        IF IsOverdue(loan) THEN
            ADD loan TO overdueLoans
        END IF
    END FOR

    RETURN overdueLoans sorted by DueDate (oldest first)
END FUNCTION
```

#### Complex Algorithm Identification

Identify which algorithms demonstrate "Group A" complexity:

*Library System Examples:*
- **Multi-table JOIN query:** Getting loans with book titles and member names
- **Many-to-many relationship handling:** Managing book-author links
- **Date-based calculations:** Calculating days overdue, filtering by date range
- **Composite validation:** Multiple business rules checked together
- **Search with multiple criteria:** Search by title AND/OR author

### 2.3 User Interface Design

#### Wireframes/Mockups

*Library System - Screens to Design:*

1. **Main Window** - Navigation hub with buttons to each section
2. **Books Window** - DataGrid of books, form to add/edit, author management
3. **Authors Window** - DataGrid of authors, form to add/edit
4. **Members Window** - DataGrid of members, form to add/edit
5. **Loans Window** - DataGrid of loans, form to create new loan, mark as returned
6. **Search/View Loans Window** - Filter by status, search, view details

For each screen, sketch:
- Layout of controls (DataGrid, TextBoxes, Buttons, ComboBoxes)
- Labels for all controls
- Button positions and labels
- Where data will be displayed

**Tools:** Pencil and paper, Balsamiq, Figma, or even PowerPoint

#### Navigation Diagram

*Library System Example:*
```
Main Window
├── Manage Books
│   ├── View All Books (DataGrid)
│   ├── Add New Book (Form)
│   ├── Edit Book (Form)
│   ├── Delete Book
│   └── Manage Book Authors (Add/Remove)
├── Manage Authors
│   ├── View All Authors (DataGrid)
│   ├── Add New Author (Form)
│   ├── Edit Author (Form)
│   └── Delete Author
├── Manage Members
│   ├── View All Members (DataGrid)
│   ├── Add New Member (Form)
│   ├── Edit Member (Form)
│   └── Delete Member
├── Manage Loans
│   ├── View All Loans (DataGrid)
│   ├── Create New Loan (Form)
│   ├── Mark as Returned
│   └── Delete Loan
└── Search & Reports
    ├── View All Loans with Details
    ├── Filter by Status (Active/Returned/Overdue)
    └── Search by Book/Member
```

### 2.4 Class Design

#### Why Create Classes for Each Entity?

Before diving into class diagrams, it's important to understand **why** we create separate classes for each database entity. This is a key concept that demonstrates good software design.

**Without Entity Classes (Bad Approach):**
```csharp
// Scattered data, no structure, error-prone
string bookTitle = reader["Title"].ToString();
int bookYear = Convert.ToInt32(reader["YearPublished"]);
// Have to remember which variables go together
// No validation, no computed properties
// Code is messy and hard to maintain
```

**With Entity Classes (Good Approach):**
```csharp
// Organised, validated, reusable
Book book = new Book
{
    Title = reader["Title"].ToString(),
    YearPublished = Convert.ToInt32(reader["YearPublished"])
};

if (book.IsValid())  // Validation built into the class
{
    // Use the book object
}
```

**Benefits of Entity Classes:**

| Benefit | Explanation |
|---------|-------------|
| **Encapsulation** | Data and behaviour bundled together - the Book class contains both book data AND methods to work with that data |
| **Validation** | Each class can validate its own data (e.g., Member.IsValidEmail()) |
| **Computed Properties** | Derived values like `FullName` from `FirstName` + `LastName` |
| **Type Safety** | Compiler catches errors - can't accidentally pass a Member where a Book is expected |
| **Reusability** | Use the same class everywhere - DataGrid, forms, database operations |
| **Data Binding** | WPF can bind directly to class properties for automatic UI updates |
| **Maintainability** | Changes to how a Book works are made in ONE place |
| **Readability** | `loan.IsOverdue` is clearer than `loan.ReturnDate == null && loan.DueDate < DateTime.Today` |

**This demonstrates Object-Oriented Programming (OOP)** - a key concept for A-Level Computer Science and essential for Group A marks.

#### Class Purpose Descriptions

Before drawing class diagrams, write a brief description of each class's purpose. This shows you understand WHY each class exists.

*Library System Example:*

| Class | Purpose | Key Responsibilities |
|-------|---------|---------------------|
| **Book** | Represents a book in the library catalogue | Store book details (title, ISBN, year); validate book data; maintain list of authors |
| **Author** | Represents a person who writes books | Store author name; provide computed FullName property; maintain list of books written |
| **Member** | Represents a library member who can borrow books | Store member details; validate email format; categorise by member type (Student/Teacher/Staff) |
| **Loan** | Represents a single borrowing transaction | Track which book was borrowed by which member; record dates; calculate overdue status |
| **LoanWithDetails** | View model combining loan, book, and member data | Display loan information with book titles and member names (from JOIN query); provide status messages for UI |
| **DatabaseHelper** | Handles all database operations | Connect to SQLite; execute CRUD operations; manage transactions; handle errors |

#### Class Diagrams

For each class, draw a diagram showing properties (attributes) and methods (operations).

*Library System Example:*

```
┌─────────────────────────────┐
│           Book              │
├─────────────────────────────┤
│ - BookID: int               │
│ - Title: string             │
│ - ISBN: string              │
│ - YearPublished: int        │
│ - Authors: List<Author>     │
├─────────────────────────────┤
│ + IsValid(): bool           │
│ + GetValidationErrors(): string │
│ + ToString(): string        │
└─────────────────────────────┘

┌─────────────────────────────┐
│          Author             │
├─────────────────────────────┤
│ - AuthorID: int             │
│ - FirstName: string         │
│ - LastName: string          │
│ - Books: List<Book>         │
├─────────────────────────────┤
│ + FullName: string {get}    │
│ + IsValid(): bool           │
│ + GetValidationErrors(): string │
│ + ToString(): string        │
└─────────────────────────────┘

┌─────────────────────────────┐
│          Member             │
├─────────────────────────────┤
│ - MemberID: int             │
│ - FirstName: string         │
│ - LastName: string          │
│ - Email: string             │
│ - MemberType: string        │
├─────────────────────────────┤
│ + FullName: string {get}    │
│ + IsValid(): bool           │
│ + IsValidEmail(): bool      │
│ + GetValidationErrors(): string │
└─────────────────────────────┘

┌─────────────────────────────┐
│           Loan              │
├─────────────────────────────┤
│ - LoanID: int               │
│ - BookID: int               │
│ - MemberID: int             │
│ - LoanDate: DateTime        │
│ - DueDate: DateTime         │
│ - ReturnDate: DateTime?     │
├─────────────────────────────┤
│ + IsReturned: bool {get}    │
│ + IsOverdue: bool {get}     │
│ + DaysUntilDue: int {get}   │
│ + IsValid(): bool           │
│ + GetValidationErrors(): string │
└─────────────────────────────┘

┌─────────────────────────────┐
│      LoanWithDetails        │
├─────────────────────────────┤
│ (All Loan properties plus:) │
│ - BookTitle: string         │
│ - BookISBN: string          │
│ - MemberFirstName: string   │
│ - MemberLastName: string    │
│ - MemberEmail: string       │
├─────────────────────────────┤
│ + MemberFullName: string    │
│ + Status: string {get}      │
│ + StatusMessage: string     │
└─────────────────────────────┘
```

### 2.5 Design Checklist

Before moving to Prototyping, ensure you have:

- [ ] Complete ERD with all entities (Books, Authors, Members, Loans, BookAuthors)
- [ ] Data dictionary for all tables and fields
- [ ] Evidence of normalisation (1NF, 2NF, 3NF with before/after example)
- [ ] Pseudocode/flowcharts for key algorithms (search, validation, overdue check)
- [ ] Wireframes for ALL screens (Main, Books, Authors, Members, Loans, Search)
- [ ] Navigation diagram showing how screens connect
- [ ] Class diagrams for all model classes
- [ ] Client review of designs (documented feedback)

---

## Stage 3: Prototyping

**Goal:** Prove your approach works before building the full application.

### Why Prototype?

- Discover problems early (when they're cheap to fix)
- Learn the technology before committing to full development
- Get client feedback on a working example
- Build confidence in your approach

### 3.1 Database Prototype (Command Line First)

**Do this BEFORE building any UI!**

#### Step 1: Create Your Database Structure

Using DB Browser for SQLite:

```sql
-- Create Books table
CREATE TABLE IF NOT EXISTS Books (
    BookID INTEGER PRIMARY KEY AUTOINCREMENT,
    Title TEXT NOT NULL,
    ISBN TEXT,
    YearPublished INTEGER NOT NULL
);

-- Create Authors table
CREATE TABLE IF NOT EXISTS Authors (
    AuthorID INTEGER PRIMARY KEY AUTOINCREMENT,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL
);

-- Create junction table for many-to-many relationship
CREATE TABLE IF NOT EXISTS BookAuthors (
    BookID INTEGER NOT NULL,
    AuthorID INTEGER NOT NULL,
    PRIMARY KEY (BookID, AuthorID),
    FOREIGN KEY (BookID) REFERENCES Books(BookID) ON DELETE CASCADE,
    FOREIGN KEY (AuthorID) REFERENCES Authors(AuthorID) ON DELETE CASCADE
);

-- Create Members table
CREATE TABLE IF NOT EXISTS Members (
    MemberID INTEGER PRIMARY KEY AUTOINCREMENT,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    Email TEXT NOT NULL,
    MemberType TEXT NOT NULL
);

-- Create Loans table
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

#### Step 2: Insert Test Data

```sql
-- Add test books
INSERT INTO Books (Title, ISBN, YearPublished) VALUES ('The Hobbit', '978-0261103283', 1937);
INSERT INTO Books (Title, ISBN, YearPublished) VALUES ('Good Omens', '978-0060853983', 1990);
INSERT INTO Books (Title, ISBN, YearPublished) VALUES ('1984', '978-0451524935', 1949);

-- Add test authors
INSERT INTO Authors (FirstName, LastName) VALUES ('J.R.R.', 'Tolkien');
INSERT INTO Authors (FirstName, LastName) VALUES ('Terry', 'Pratchett');
INSERT INTO Authors (FirstName, LastName) VALUES ('Neil', 'Gaiman');
INSERT INTO Authors (FirstName, LastName) VALUES ('George', 'Orwell');

-- Link books to authors (note: Good Omens has TWO authors)
INSERT INTO BookAuthors (BookID, AuthorID) VALUES (1, 1);  -- Hobbit by Tolkien
INSERT INTO BookAuthors (BookID, AuthorID) VALUES (2, 2);  -- Good Omens by Pratchett
INSERT INTO BookAuthors (BookID, AuthorID) VALUES (2, 3);  -- Good Omens by Gaiman
INSERT INTO BookAuthors (BookID, AuthorID) VALUES (3, 4);  -- 1984 by Orwell

-- Add test members
INSERT INTO Members (FirstName, LastName, Email, MemberType)
VALUES ('Alice', 'Johnson', 'alice@school.edu', 'Student');
INSERT INTO Members (FirstName, LastName, Email, MemberType)
VALUES ('Bob', 'Williams', 'bob@school.edu', 'Teacher');

-- Add test loans (one active, one returned, one overdue)
INSERT INTO Loans (BookID, MemberID, LoanDate, DueDate, ReturnDate)
VALUES (1, 1, '2024-03-01', '2024-03-15', '2024-03-10');  -- Returned

INSERT INTO Loans (BookID, MemberID, LoanDate, DueDate, ReturnDate)
VALUES (2, 1, '2024-03-10', '2024-03-24', NULL);  -- Active (adjust dates as needed)

INSERT INTO Loans (BookID, MemberID, LoanDate, DueDate, ReturnDate)
VALUES (3, 2, '2024-01-01', '2024-01-15', NULL);  -- Overdue!
```

#### Step 3: Test Your Queries

Before writing C# code, test EVERY query you'll need:

```sql
-- Basic SELECT
SELECT * FROM Books;
SELECT * FROM Authors;
SELECT * FROM Members;
SELECT * FROM Loans;

-- SELECT with WHERE
SELECT * FROM Books WHERE BookID = 1;
SELECT * FROM Members WHERE MemberType = 'Student';

-- Get authors for a specific book (JOIN through junction table)
SELECT a.AuthorID, a.FirstName, a.LastName
FROM Authors a
INNER JOIN BookAuthors ba ON a.AuthorID = ba.AuthorID
WHERE ba.BookID = 2;

-- Get books by a specific author
SELECT b.BookID, b.Title, b.YearPublished
FROM Books b
INNER JOIN BookAuthors ba ON b.BookID = ba.BookID
WHERE ba.AuthorID = 3;

-- Get all loans with book and member details (3-table JOIN)
SELECT
    l.LoanID, l.LoanDate, l.DueDate, l.ReturnDate,
    b.Title AS BookTitle,
    m.FirstName || ' ' || m.LastName AS MemberName
FROM Loans l
INNER JOIN Books b ON l.BookID = b.BookID
INNER JOIN Members m ON l.MemberID = m.MemberID;

-- Find overdue loans (not returned and due date passed)
SELECT
    l.LoanID, b.Title, m.FirstName, m.LastName, l.DueDate
FROM Loans l
INNER JOIN Books b ON l.BookID = b.BookID
INNER JOIN Members m ON l.MemberID = m.MemberID
WHERE l.ReturnDate IS NULL AND l.DueDate < date('now');

-- Search books by title
SELECT * FROM Books WHERE Title LIKE '%Hobbit%';

-- UPDATE example
UPDATE Members SET Email = 'alice.j@school.edu' WHERE MemberID = 1;

-- Mark loan as returned
UPDATE Loans SET ReturnDate = '2024-03-20' WHERE LoanID = 2;

-- DELETE example (be careful!)
DELETE FROM Loans WHERE LoanID = 3;
```

**Document this:** Screenshot your queries working in DB Browser for SQLite.

### 3.2 C# Console Prototype

**Build a command-line app BEFORE adding WPF!**

#### Step 1: Create Console Application

```
File → New Project → Console App (.NET)
Name: LibraryConsolePrototype
```

#### Step 2: Install SQLite Package

```
Tools → NuGet Package Manager → Package Manager Console
Install-Package Microsoft.Data.Sqlite
```

#### Step 3: Test Database Connection

```csharp
using Microsoft.Data.Sqlite;

class Program
{
    static string connectionString = "Data Source=LibraryDatabase.db";

    static void Main()
    {
        Console.WriteLine("Library System Console Prototype");
        Console.WriteLine("================================\n");

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            Console.WriteLine("Connected to database successfully!\n");

            // Test: Get all books
            Console.WriteLine("All Books:");
            string sql = "SELECT BookID, Title, YearPublished FROM Books";
            using (var cmd = new SqliteCommand(sql, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"  [{reader["BookID"]}] {reader["Title"]} ({reader["YearPublished"]})");
                    }
                }
            }

            Console.WriteLine("\nAll Members:");
            sql = "SELECT MemberID, FirstName, LastName, MemberType FROM Members";
            using (var cmd = new SqliteCommand(sql, conn))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"  [{reader["MemberID"]}] {reader["FirstName"]} {reader["LastName"]} ({reader["MemberType"]})");
                    }
                }
            }
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
```

#### Step 4: Test CRUD Operations

Write and test methods for each entity:

```csharp
// Test INSERT
static int InsertBook(string title, string isbn, int year)
{
    using (var conn = new SqliteConnection(connectionString))
    {
        conn.Open();
        string sql = @"INSERT INTO Books (Title, ISBN, YearPublished)
                       VALUES (@Title, @ISBN, @Year)";

        using (var cmd = new SqliteCommand(sql, conn))
        {
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@ISBN", isbn);
            cmd.Parameters.AddWithValue("@Year", year);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "SELECT last_insert_rowid()";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}

// Test in Main()
int newBookId = InsertBook("Test Book", "123-456", 2024);
Console.WriteLine($"Inserted book with ID: {newBookId}");
```

**Don't move on until ALL CRUD operations work for ALL entities!**

### 3.3 Model Classes Prototype

#### Step 1: Create Model Classes

```csharp
public class Book
{
    public int BookID { get; set; }
    public string Title { get; set; }
    public string ISBN { get; set; }
    public int YearPublished { get; set; }
    public List<Author> Authors { get; set; } = new List<Author>();

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Title)) return false;
        if (YearPublished < 1000 || YearPublished > DateTime.Now.Year + 1) return false;
        return true;
    }

    public override string ToString() => $"{Title} ({YearPublished})";
}

public class Member
{
    public int MemberID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MemberType { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(FirstName)) return false;
        if (string.IsNullOrWhiteSpace(LastName)) return false;
        if (string.IsNullOrWhiteSpace(Email)) return false;
        if (!IsValidEmail(Email)) return false;
        return true;
    }

    private bool IsValidEmail(string email)
    {
        // Simple check - could use Regex for more thorough validation
        return email.Contains("@") && email.Contains(".");
    }
}

public class Loan
{
    public int LoanID { get; set; }
    public int BookID { get; set; }
    public int MemberID { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }  // Nullable - null if not returned

    public bool IsReturned => ReturnDate.HasValue;

    public bool IsOverdue => !IsReturned && DateTime.Today > DueDate;

    public int DaysUntilDue => IsReturned ? 0 : (DueDate - DateTime.Today).Days;
}
```

#### Step 2: Create DatabaseHelper Class

```csharp
public static class DatabaseHelper
{
    private static string connectionString = "Data Source=LibraryDatabase.db";

    public static List<Book> GetAllBooks()
    {
        List<Book> books = new List<Book>();

        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            string sql = "SELECT BookID, Title, ISBN, YearPublished FROM Books ORDER BY Title";

            using (var cmd = new SqliteCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    books.Add(new Book
                    {
                        BookID = Convert.ToInt32(reader["BookID"]),
                        Title = reader["Title"].ToString(),
                        ISBN = reader["ISBN"]?.ToString() ?? "",
                        YearPublished = Convert.ToInt32(reader["YearPublished"])
                    });
                }
            }
        }

        return books;
    }

    public static int InsertBook(Book book)
    {
        using (var conn = new SqliteConnection(connectionString))
        {
            conn.Open();
            string sql = @"INSERT INTO Books (Title, ISBN, YearPublished)
                           VALUES (@Title, @ISBN, @Year)";

            using (var cmd = new SqliteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Title", book.Title);
                cmd.Parameters.AddWithValue("@ISBN", book.ISBN ?? "");
                cmd.Parameters.AddWithValue("@Year", book.YearPublished);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT last_insert_rowid()";
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }

    // Add similar methods for Update, Delete, GetById, etc.
    // Add methods for Members, Authors, Loans, BookAuthors
}
```

#### Step 3: Test from Console

```csharp
// Test in Main()
var books = DatabaseHelper.GetAllBooks();
Console.WriteLine($"Found {books.Count} books:");
foreach (var book in books)
{
    Console.WriteLine($"  {book}");
}

var newBook = new Book
{
    Title = "Pride and Prejudice",
    ISBN = "978-0141439518",
    YearPublished = 1813
};

if (newBook.IsValid())
{
    int id = DatabaseHelper.InsertBook(newBook);
    Console.WriteLine($"Created book with ID: {id}");
}
else
{
    Console.WriteLine("Book validation failed!");
}
```

### 3.4 Simple WPF Prototype

**Only after console prototype works!**

See the **[WPF-Guide.md](WPF-Guide.md)** for detailed WPF instructions.

#### Step 1: Create WPF Project

```
File → New Project → WPF Application (.NET)
Name: LibraryManagementSystem
```

#### Step 2: Add Your Model and DatabaseHelper Classes

Copy the classes you tested in your console prototype.

#### Step 3: Create ONE Simple Window

Test displaying books in a DataGrid:

```xml
<Window x:Class="LibraryManagementSystem.MainWindow"
        Title="Library System" Height="400" Width="600">
    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <TextBlock Text="Books" FontSize="20" FontWeight="Bold" Margin="0,0,0,10"/>

        <DataGrid Grid.Row="1" x:Name="dgBooks" AutoGenerateColumns="False" IsReadOnly="True">
            <DataGrid.Columns>
                <DataGridTextColumn Header="ID" Binding="{Binding BookID}" Width="50"/>
                <DataGridTextColumn Header="Title" Binding="{Binding Title}" Width="*"/>
                <DataGridTextColumn Header="Year" Binding="{Binding YearPublished}" Width="80"/>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>
</Window>
```

```csharp
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Load books when window opens
        dgBooks.ItemsSource = DatabaseHelper.GetAllBooks();
    }
}
```

#### Step 4: Test ONE CRUD Operation

Add a simple "Add Book" button and form. Test it works before building more.

### 3.5 Prototyping Checklist

Before full development:

- [ ] Database created with all 5 tables (Books, Authors, BookAuthors, Members, Loans)
- [ ] Test data inserted for all tables
- [ ] All SQL queries tested in DB Browser (SELECT, INSERT, UPDATE, DELETE, JOINs)
- [ ] Console app connects to database successfully
- [ ] All CRUD operations work from console for all entities
- [ ] Model classes created with validation (Book, Author, Member, Loan)
- [ ] Simple WPF window displays books from database
- [ ] At least one Add operation works in WPF
- [ ] Client has seen prototype and given feedback (documented)

---

## Stage 4: Development

**Goal:** Build the complete, polished application.

### 4.1 Development Order

Build in this order to catch problems early:

1. **Database layer** (All DatabaseHelper methods)
2. **Model classes** (With full validation)
3. **Main window** (Navigation buttons)
4. **Books window** (Full CRUD + author management)
5. **Authors window** (Full CRUD)
6. **Members window** (Full CRUD)
7. **Loans window** (Full CRUD + mark as returned)
8. **Search/Reports window** (JOINs, filtering)
9. **Polish and refinement**

### 4.2 For Each Feature

Follow this cycle:

1. **Plan:** What exactly should this feature do?
2. **Design:** Sketch the UI, write pseudocode
3. **Implement:** Write the code
4. **Test:** Does it work correctly?
5. **Document:** Comment your code
6. **Review:** Could it be improved?

*Library System Example - Adding "Mark as Returned" feature:*

1. **Plan:** User selects a loan, clicks "Return Book", system sets ReturnDate to today
2. **Design:** Add button to LoansWindow, write SQL UPDATE statement
3. **Implement:** Write event handler, call DatabaseHelper.ReturnBook(loanId)
4. **Test:** Create loan, return it, verify ReturnDate is set, verify it no longer shows as overdue
5. **Document:** Add XML comments to ReturnBook method
6. **Review:** Should we confirm before marking as returned? Add MessageBox.

### 4.3 Coding Standards

Follow these throughout:

- **Meaningful names:** `BookID` not `id1`, `GetOverdueLoans()` not `GetData()`
- **Consistent style:** Same indentation, same naming conventions
- **Comments:** Explain WHY, not WHAT
- **XML documentation:** On all public methods
- **Regions:** Group related code (e.g., `#region Button Event Handlers`)
- **Error handling:** Try-catch around all database operations

### 4.4 Regular Client Contact

**Keep your client (the librarian) involved!**

- Show them progress every 1-2 weeks
- Ask: "Is this how you imagined the loan screen working?"
- Get them to test with real scenarios: "Try adding a book with two authors"
- Document ALL feedback (dates, what was said, what you changed)

*Example documented feedback:*
```
Meeting with Mrs Smith (Librarian) - 15th March 2024

Showed her the Books management screen. She liked being able to see all
books in a list. She asked if she could search by author as well as title.
Added this to the requirements as a "Desirable" feature.

She also mentioned that ISBN should be optional as older books don't have
them. Confirmed this is already handled (ISBN can be empty).

Action: Add author search to Search window.
```

**This is evidence for your write-up!**

### 4.5 Version Control with GitHub

GitHub lets you track changes, undo mistakes, work from home, and provides evidence of your development process. Both Visual Studio and VS Code have built-in GitHub support - no command line needed!

#### Why Use GitHub?

| Benefit | Why It Matters for Your NEA |
|---------|----------------------------|
| **Work from home** | Clone your project to your home PC, work on it, push changes back |
| **Backup** | Your code is safely stored online - never lose work again |
| **History** | See exactly what you changed and when |
| **Evidence** | Proves your code developed over time (not copied at the last minute) |
| **Undo mistakes** | Easily revert to a previous version if something breaks |
| **Professional skill** | Used by real developers everywhere |

#### Setting Up: Create a GitHub Account

1. Go to **github.com** and sign up for a free account
2. Use your school email if possible (you may get free benefits)
3. Choose a sensible username (potential employers may see this!)

#### Creating Your Repository in Visual Studio

**First time setup (at school):**

1. Create your WPF project as normal
2. Go to **Git → Create Git Repository**
3. Select **GitHub** (not Local only)
4. Sign in to your GitHub account when prompted
5. Set:
   - **Repository name:** e.g., `LibraryManagementSystem`
   - **Description:** "A-Level NEA - Library Management System"
   - **Private:** Yes (keep your NEA private!)
6. Click **Create and Push**

Your project is now on GitHub!

#### Making Commits (Save Points)

After making changes, create a commit to save your progress:

**In Visual Studio:**
1. Go to **Git → Commit or Stash** (or Ctrl+Alt+F7)
2. You'll see changed files in the Git Changes panel
3. Write a **commit message** describing what you changed
4. Click **Commit All**
5. Click **Push** (up arrow) to upload to GitHub

**In VS Code:**
1. Click the **Source Control** icon (left sidebar)
2. See changed files listed
3. Click **+** to stage files
4. Type a commit message
5. Click the **tick (✓)** to commit
6. Click **Sync Changes** to upload to GitHub

**Good commit messages (Library System examples):**
- "Added Book model class with validation"
- "Implemented Books CRUD in DatabaseHelper"
- "Created BooksWindow with DataGrid display"
- "Added many-to-many author management"
- "Fixed overdue calculation bug"

#### Working From Home - Cloning Your Repository

This is the key benefit - you can work on your NEA at home!

**First time at home - Clone the repository:**

**In Visual Studio:**
1. Open Visual Studio (without opening a project)
2. Click **Clone a repository**
3. Sign in to GitHub if prompted
4. You'll see your repositories listed - select your NEA project
5. Choose where to save it on your home PC
6. Click **Clone**

**In VS Code:**
1. Open VS Code
2. Press **Ctrl+Shift+P** and type "Git: Clone"
3. Click **Clone from GitHub**
4. Sign in if prompted
5. Select your repository from the list
6. Choose where to save it

**After cloning:** Your entire project is now on your home PC, exactly as you left it at school!

#### The Daily Workflow: School ↔ Home

```
AT SCHOOL:
1. Make changes to your code
2. Commit with a message ("Added member validation")
3. Push to GitHub (click Push/Sync)

AT HOME:
1. Pull latest changes (click Pull/Sync) - gets your school work
2. Make changes to your code
3. Commit with a message ("Added loan window")
4. Push to GitHub

BACK AT SCHOOL:
1. Pull latest changes - gets your home work
2. Continue working...
```

**Golden rule:** Always **Pull** before you start working, always **Push** when you finish!

#### Pulling Changes (Getting Latest Version)

Before you start working (especially when switching between school/home):

**In Visual Studio:**
1. Go to **Git → Pull**
2. This downloads any changes from GitHub

**In VS Code:**
1. Click **Source Control** icon
2. Click **Sync Changes** (or the circular arrows)

#### Viewing History

**In Visual Studio:**
1. Go to **Git → View Branch History**
2. See all your commits with dates and messages
3. Double-click any commit to see what changed

**In VS Code:**
1. Install the **Git Graph** extension (recommended)
2. Click **Git Graph** in the bottom status bar
3. See visual timeline of all your commits

#### Reverting to a Previous Version

Made a mess? Go back to when it worked:

**In Visual Studio:**
1. Go to **Git → View Branch History**
2. Right-click the commit you want to go back to
3. Choose **Reset → Reset and Delete Changes (--hard)**
4. Push the changes to GitHub

**In VS Code:**
1. Open Git Graph
2. Right-click the commit you want
3. Choose **Reset current branch to this commit**
4. Select **Hard** to completely revert
5. Click Sync to update GitHub

**Warning:** This permanently discards changes after that commit!

#### Troubleshooting: Merge Conflicts

If you edited the same file at school AND home without syncing, you'll get a **merge conflict**. This looks scary but is fixable:

1. Visual Studio/VS Code will highlight the conflicting sections
2. You'll see your changes AND the other version
3. Choose which version to keep (or combine them)
4. Save the file
5. Commit the resolved conflict

**Avoiding conflicts:** Always Pull before starting work, always Push when finished!

#### Keep Your Repository Private

Your NEA must be your own work. Keep your GitHub repository **private**:

1. On github.com, go to your repository
2. Click **Settings** (cog icon)
3. Scroll to **Danger Zone**
4. Ensure **Visibility** is set to **Private**

Never share your repository link with other students!

#### Commit Regularly!

Make a commit after each significant change:
- After completing a feature
- Before making major changes
- At the end of each coding session
- When something finally works!

This gives you a detailed history and the ability to work from anywhere.

---

## Stage 5: Testing

**Goal:** Prove your solution works correctly.

### 5.1 Creating a Test Plan

**BEFORE you test**, create a comprehensive test plan.

#### Test Plan Table Format

*Library System Example:*

| Test ID | Description | Test Data | Expected Result | Actual Result | Pass/Fail | Evidence |
|---------|-------------|-----------|-----------------|---------------|-----------|----------|
| T001 | Add book with valid data | Title: "Test Book", Year: 2024 | Book added, appears in list | | | |
| T002 | Add book with empty title | Title: (empty), Year: 2024 | Error message, book not added | | | |
| T003 | Add book with future year | Title: "Future", Year: 2030 | Error message about invalid year | | | |
| T004 | Add author to book | Select book, select author, click Add | Author appears in book's author list | | | |
| T005 | Add multiple authors to book | Add Pratchett, then add Gaiman to same book | Both authors shown for book | | | |
| T006 | Add member with invalid email | Email: "notanemail" | Error message about email format | | | |
| T007 | Create loan for book | Select book, member, set dates | Loan created, appears in list | | | |
| T008 | Create loan with due date before loan date | LoanDate: 15/03, DueDate: 10/03 | Error message, loan not created | | | |
| T009 | Mark loan as returned | Select active loan, click Return | ReturnDate set to today, status changes | | | |
| T010 | Identify overdue loan | Loan with DueDate in past, no ReturnDate | Shows as "Overdue" in status | | | |
| T011 | Search books by title | Search for "Hobbit" | "The Hobbit" appears in results | | | |
| T012 | Filter loans by status | Click "Show Overdue" | Only overdue loans displayed | | | |

### 5.2 Types of Tests to Include

#### Normal Data (Expected inputs)
*Library System Examples:*
- Add book with title "The Great Gatsby", year 1925
- Add member with valid email "student@school.edu"
- Create loan with due date 14 days from today

#### Boundary Data (Edge cases)
*Library System Examples:*
- Book with year = 1000 (minimum valid year)
- Book with year = current year + 1 (maximum valid year)
- Loan due date = loan date + 1 day (minimum loan period)
- Empty ISBN (allowed)

#### Erroneous Data (Invalid inputs)
*Library System Examples:*
- Empty book title (should reject)
- Book year = 999 (too old, should reject)
- Member email without @ symbol (should reject)
- Due date before loan date (should reject)
- SQL injection attempt: Title = "'; DROP TABLE Books; --"

### 5.3 Video Evidence with Commentary

**AQA recommends video evidence.** Here's how:

#### Recording Setup
- Use OBS Studio (free) or Windows Game Bar (Win+G)
- Record your screen AND audio
- Speak clearly, explaining what you're testing

#### Video Structure - Library System Example
```
"This is test T007 - creating a new loan.

As you can see, I'm on the Loans window and the DataGrid shows
there are currently 2 loans in the system.

I'll click the New Loan button... now I'll select 'The Hobbit'
from the Book dropdown... select 'Alice Johnson' as the member...
the loan date defaults to today which is correct... and I'll set
the due date to 14 days from now.

Now I'll click Save... and we can see the loan has been added
to the list. The status shows 'On Loan' because it hasn't been
returned yet.

Test T007 passed."
```

#### Video Length
- Aim for 10-15 minutes total
- Can be multiple shorter videos (by feature area)
- Don't pad with unnecessary content

### 5.4 Test Against Success Criteria

**Explicitly test each objective from Analysis:**

*Library System Example:*

| Objective | Test(s) Performed | Evidence | Met? |
|-----------|-------------------|----------|------|
| Store at least 1000 books | Inserted 1200 test records, all operations still responsive | Video T050, Screenshot T050 | Yes |
| Record loan in under 30 seconds | Timed test: 22 seconds from opening form to saved | Video T051 at 5:30 | Yes |
| Correctly identify overdue books | Created loan due yesterday, status showed "Overdue" | Test T010, Video T010 | Yes |
| Books can have multiple authors | Added 2 authors to "Good Omens", both displayed | Test T005, Screenshot T005 | Yes |
| Validates email format | Entered "bademail", got validation error | Test T006, Screenshot T006 | Yes |
| Client confirms meets needs | Meeting notes 20/03/2024 | Appendix C | Yes |

### 5.5 Testing Checklist

- [ ] Test plan created BEFORE testing
- [ ] Normal, boundary, and erroneous data tested for all entities
- [ ] All CRUD operations tested (Books, Authors, Members, Loans)
- [ ] Many-to-many relationship tested (book with multiple authors)
- [ ] Overdue detection tested
- [ ] Search/filter functionality tested
- [ ] All validation rules tested (valid and invalid data)
- [ ] Video evidence recorded with commentary
- [ ] Each success criterion explicitly tested
- [ ] All tests documented with actual results
- [ ] Screenshots/video timestamps included as evidence

---

## Stage 6: Evaluation

**Goal:** Honestly assess your work.

### 6.1 Evaluation Against Objectives

*Library System Example:*

| Objective | Met? | Evidence | Discussion |
|-----------|------|----------|------------|
| Store at least 1000 books | Yes | Test T050 | Tested with 1200 records, no performance issues |
| Record loan in under 30 seconds | Yes | Test T051 | Average time was 22 seconds |
| Correctly identify overdue books | Yes | Test T010 | Works correctly with various date scenarios |
| Books can have multiple authors | Yes | Test T005 | Many-to-many relationship working as designed |
| Search by title within 2 seconds | Yes | Test T040 | Instant results even with 1200 records |
| Client confirms meets needs | Yes | Meeting 20/03 | Mrs Smith happy with final system |

### 6.2 Client Feedback

*Library System Example:*
```
Final Review Meeting with Mrs Smith (School Librarian) - 20th March 2024

"The system is much better than the card system we were using.
I particularly like being able to see all overdue books at a glance -
this used to take me ages to work out manually.

The search feature is very quick, and I like that I can search by
title or see all books by an author.

If I could improve anything, I would like the ability to print out
an overdue notice to give to students. But overall, this system
will save me a lot of time every day."

Overall satisfaction: 9/10
```

### 6.3 Honest Self-Assessment

**Be critical!** Examiners respect honesty.

*Library System Example:*

**What went well:**
- Database design with many-to-many relationship works correctly
- All CRUD operations function as expected
- Overdue detection is accurate
- Client is happy with the system
- Code is well-organised with XML comments

**What could be improved:**
- No print functionality (client requested, ran out of time)
- Search only works on exact matches (could add fuzzy search)
- UI could be more visually appealing
- No data backup feature

**What I learned:**
- How to implement many-to-many relationships in a database
- The importance of testing SQL queries before writing C# code
- WPF data binding is powerful but takes practice
- Regular client feedback helped shape a useful product

### 6.4 Future Development

*Library System Example:*

If I were to continue developing this system, I would:
1. Add print functionality for overdue notices
2. Implement email reminders for overdue books
3. Add a dashboard showing library statistics
4. Create user accounts with different permission levels
5. Add barcode scanning for faster book lookup

---

## Project Complexity Levels

### Good Project (Competent)

**Database:** 3-4 tables, one-to-many relationships, basic CRUD
**Example:** Simple book catalogue - Books and Categories only

### Great Project (Proficient) - This Exemplar Level

**Database:** 5 tables including junction table, many-to-many relationship, JOINs
**Features:** Full CRUD, search, overdue detection, data validation
**Example:** Library system with Books, Authors, Members, Loans, BookAuthors

### Excellent Project (Exceptional)

**All "Great" features plus:**
- User authentication
- Dashboard with statistics
- Print/export functionality
- More complex business rules
- Data visualisation

### AQA Group A Skills Demonstrated in Library System

| Skill | Example in Library System |
|-------|---------------------------|
| Complex data model | Many-to-many Books-Authors via junction table |
| Parameterised SQL | All queries use @parameters |
| Multi-table queries | 3-table JOIN for LoanWithDetails |
| Data validation | Email regex, date range checks, required fields |
| OOP concepts | Encapsulation in model classes, computed properties |
| Complex algorithms | Overdue calculation, search with multiple criteria |

---

## Tips for Success

### Do's

1. **Start early** - The NEA takes longer than you think
2. **Document as you go** - Don't leave write-up until the end
3. **Keep your client involved** - Regular contact, documented
4. **Test SQL first** - Use DB Browser before writing C#
5. **Build console prototype first** - Prove database works before adding UI
6. **Test continuously** - Don't save testing for the end
7. **Use version control** - Track your progress
8. **Comment your code** - XML documentation on all methods
9. **Back up everything** - Multiple locations, regularly

### Don'ts

1. **Don't copy code** - Understand then write your own
2. **Don't over-scope** - A polished 5-table project beats a broken 10-table one
3. **Don't skip prototyping** - Jumping to WPF leads to problems
4. **Don't ignore client feedback** - It's evidence AND guidance
5. **Don't leave testing late** - You'll find bugs you don't have time to fix
6. **Don't use an imaginary client** - Must be a real person

### Time Management

| Phase | Duration | Activities |
|-------|----------|------------|
| Analysis | 2-3 weeks | Client meetings, requirements, research |
| Design | 2-3 weeks | ERD, wireframes, algorithms |
| Prototyping | 2-3 weeks | DB Browser testing, console app, simple WPF |
| Development | 8-10 weeks | Building the full application |
| Testing | 2-3 weeks | Test plan, execution, video evidence |
| Evaluation | 1-2 weeks | Final write-up, client sign-off |

**Total:** ~18-24 weeks (start in Year 12!)

---

## Summary

```
1. ANALYSIS
   └── Find librarian/client → Gather requirements → Define testable objectives

2. DESIGN
   └── ERD (Books, Authors, Members, Loans, BookAuthors) → Data dictionary → Algorithms → Wireframes

3. PROTOTYPE
   └── Test SQL in DB Browser → Console app with CRUD → Basic WPF → Client feedback

4. DEVELOP
   └── DatabaseHelper → Model classes → Main window → Each entity window → Search/reports

5. TEST
   └── Test plan → Normal/boundary/erroneous data → Video evidence → Document results

6. EVALUATE
   └── Against objectives → Client feedback → Honest reflection
```

**Remember:** Use this guide and the Library Management System exemplar to LEARN concepts, then apply them to YOUR original project.

Good luck!

---

**Document created by:** Claude AI (Anthropic)
**Purpose:** Guidance for AQA 7517 A-Level Computer Science NEA projects
**Reference:** AQA 7517 Specification

**Disclaimer:** Always refer to the official AQA specification and your teacher for definitive guidance.
