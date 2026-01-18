# WPF Guide for A-Level NEA Projects

A comprehensive introduction to Windows Presentation Foundation (WPF) for A-Level Computer Science students building desktop applications.

**This guide uses the Library Management System as a worked example throughout.**

---

## Table of Contents

1. [What is WPF?](#what-is-wpf)
2. [XAML vs Code-Behind: The Two Parts of a Window](#xaml-vs-code-behind-the-two-parts-of-a-window)
3. [Understanding XAML](#understanding-xaml)
4. [The Event-Driven Paradigm](#the-event-driven-paradigm)
5. [Data Binding](#data-binding)
6. [Common WPF Controls](#common-wpf-controls)
7. [Layout with Grid and StackPanel](#layout-with-grid-and-stackpanel)
8. [Building Your First WPF Window](#building-your-first-wpf-window)
9. [Connecting to Your Database](#connecting-to-your-database)
10. [Design Patterns and Best Practices](#design-patterns-and-best-practices)

---

## What is WPF?

### Overview

**WPF (Windows Presentation Foundation)** is Microsoft's framework for building Windows desktop applications. It's the modern replacement for Windows Forms (WinForms) and offers:

- **Separation of UI and logic** - XAML for appearance, C# for behaviour
- **Rich graphics and styling** - Modern-looking interfaces
- **Powerful data binding** - Automatic UI updates when data changes
- **Flexible layouts** - Responsive designs that adapt to window size

### Why Use WPF for Your NEA?

| Advantage | Why It Matters |
|-----------|----------------|
| Industry standard | Skills transfer to professional development |
| Clean separation | XAML + Code-behind demonstrates good design |
| Data binding | Less code to write, fewer bugs |
| Visual Studio designer | See your UI as you build it |
| DataGrid control | Perfect for displaying database records |

### WPF vs Windows Forms

| Feature | WPF | Windows Forms |
|---------|-----|---------------|
| UI definition | XAML (markup) | Code or designer |
| Data binding | Built-in, powerful | Basic |
| Styling | Fully customisable | Limited |
| Learning curve | Steeper | Gentler |
| Modern look | Yes | Dated |

**Recommendation:** WPF is preferred for NEA projects as it demonstrates more advanced programming concepts.

---

## XAML vs Code-Behind: The Two Parts of a Window

Every WPF window consists of **two files** that work together:

### The Two Files

```
MainWindow.xaml        ← XAML file (defines what the window LOOKS like)
MainWindow.xaml.cs     ← Code-behind file (defines what the window DOES)
```

### XAML File (.xaml)

The XAML file defines the **user interface** - what controls appear and how they're arranged.

*Library System Example - BooksWindow.xaml (simplified):*
```xml
<Window x:Class="LibraryManagementSystem.BooksWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Manage Books" Height="500" Width="800">

    <Grid Margin="10">
        <!-- Title -->
        <TextBlock Text="Books" FontSize="24" FontWeight="Bold"/>

        <!-- DataGrid to display books -->
        <DataGrid x:Name="dgBooks" Margin="0,40,0,50">
            <!-- Column definitions here -->
        </DataGrid>

        <!-- Buttons at the bottom -->
        <Button x:Name="btnAdd" Content="Add Book" Click="BtnAdd_Click"/>
        <Button x:Name="btnDelete" Content="Delete" Click="BtnDelete_Click"/>
    </Grid>

</Window>
```

**Key points:**
- Defines the visual structure
- Uses XML-like syntax
- `x:Name` gives controls a name you can reference in code
- `Click="BtnAdd_Click"` connects the button to an event handler

### Code-Behind File (.xaml.cs)

The code-behind file contains the **C# code** that makes the window work.

*Library System Example - BooksWindow.xaml.cs (simplified):*
```csharp
using System.Windows;
using LibraryManagementSystem.Database;

namespace LibraryManagementSystem
{
    public partial class BooksWindow : Window
    {
        // Constructor - runs when window is created
        public BooksWindow()
        {
            InitializeComponent();  // Required - loads the XAML

            // Load books when window opens
            LoadBooks();
        }

        // Load books from database into DataGrid
        private void LoadBooks()
        {
            dgBooks.ItemsSource = DatabaseHelper.GetAllBooks();
        }

        // Event handler - runs when Add button is clicked
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Code to add a book
        }

        // Event handler - runs when Delete button is clicked
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Code to delete selected book
        }
    }
}
```

**Key points:**
- `partial class` - the class is split across XAML and C#
- `InitializeComponent()` - must be called first, loads the XAML
- Event handlers respond to user actions
- Can access controls by their `x:Name`

### How They Connect

```
XAML:  <Button x:Name="btnAdd" Click="BtnAdd_Click"/>
                    ↓                    ↓
Code:  btnAdd.Content = "Save";    private void BtnAdd_Click(...)
       (access by name)            (event handler method)
```

The `x:Class` attribute in XAML must match the class name in code-behind:
```xml
<Window x:Class="LibraryManagementSystem.BooksWindow" ...>
```
```csharp
public partial class BooksWindow : Window
```

---

## Understanding XAML

### XAML Basics

XAML (eXtensible Application Markup Language) is an XML-based language for defining UI.

#### Elements and Attributes

```xml
<!-- Element with attributes -->
<Button Content="Click Me" Width="100" Height="30"/>

<!-- Same thing with child elements -->
<Button Width="100" Height="30">
    <Button.Content>Click Me</Button.Content>
</Button>
```

#### Naming Controls

Use `x:Name` to give a control a name you can use in code:

```xml
<TextBox x:Name="txtTitle" Width="200"/>
```

```csharp
// In code-behind, you can now access it:
string title = txtTitle.Text;
txtTitle.Text = "New Value";
```

#### Common Attributes

| Attribute | Purpose | Example |
|-----------|---------|---------|
| `x:Name` | Name for code access | `x:Name="txtEmail"` |
| `Content` | Text/content inside | `Content="Save"` |
| `Text` | Text value | `Text="Enter name"` |
| `Width/Height` | Size | `Width="200"` |
| `Margin` | Space around | `Margin="10"` or `Margin="10,5,10,5"` |
| `HorizontalAlignment` | Left/Center/Right/Stretch | `HorizontalAlignment="Center"` |
| `VerticalAlignment` | Top/Center/Bottom/Stretch | `VerticalAlignment="Top"` |
| `IsEnabled` | Can user interact? | `IsEnabled="False"` |
| `Visibility` | Visible/Hidden/Collapsed | `Visibility="Collapsed"` |

#### Margin and Padding

```
Margin = space OUTSIDE the control
Padding = space INSIDE the control

Margin="10"           → 10 on all sides
Margin="10,5"         → 10 left/right, 5 top/bottom
Margin="10,5,20,15"   → left, top, right, bottom
```

### XAML Namespaces

At the top of every XAML file:

```xml
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    ...>
```

- `xmlns` - default namespace for WPF controls (Button, TextBox, etc.)
- `xmlns:x` - XAML namespace for special attributes (x:Name, x:Class)

---

## The Event-Driven Paradigm

### What is Event-Driven Programming?

In event-driven programming, the application **waits for events** (user actions) and **responds** to them.

```
Traditional Programming:        Event-Driven Programming:
1. Do step A                    1. Wait for event
2. Do step B                    2. Event occurs (user clicks button)
3. Do step C                    3. Run event handler
4. End                          4. Return to waiting
                                5. Event occurs (user types in textbox)
                                6. Run event handler
                                7. Return to waiting
                                ... continues until window closes
```

### Events in WPF

**Events** are things that happen - user clicks, types, selects, etc.
**Event handlers** are methods that run when an event occurs.

#### Common Events

| Control | Event | When It Fires |
|---------|-------|---------------|
| Button | `Click` | User clicks the button |
| TextBox | `TextChanged` | User types in the box |
| ComboBox | `SelectionChanged` | User selects different item |
| DataGrid | `SelectionChanged` | User clicks different row |
| CheckBox | `Checked` / `Unchecked` | User ticks/unticks |
| Window | `Loaded` | Window finishes loading |
| Window | `Closing` | Window is about to close |

### Connecting Events to Handlers

#### Method 1: In XAML (Recommended)

```xml
<Button x:Name="btnSave" Content="Save" Click="BtnSave_Click"/>
```

```csharp
private void BtnSave_Click(object sender, RoutedEventArgs e)
{
    // This runs when button is clicked
    MessageBox.Show("Save clicked!");
}
```

#### Method 2: In Code

```csharp
public BooksWindow()
{
    InitializeComponent();

    // Connect event to handler in code
    btnSave.Click += BtnSave_Click;
}

private void BtnSave_Click(object sender, RoutedEventArgs e)
{
    MessageBox.Show("Save clicked!");
}
```

### Event Handler Parameters

Every event handler receives two parameters:

```csharp
private void BtnSave_Click(object sender, RoutedEventArgs e)
{
    // sender = the control that raised the event (the button)
    // e = event arguments with extra information

    Button clickedButton = (Button)sender;
    string buttonText = clickedButton.Content.ToString();
}
```

### Library System Example - Complete Event Flow

*Scenario: User clicks "Add Book" button*

**1. XAML defines the button:**
```xml
<Button x:Name="btnAddBook"
        Content="Add Book"
        Click="BtnAddBook_Click"/>
```

**2. User clicks the button**

**3. WPF fires the Click event**

**4. Event handler runs:**
```csharp
private void BtnAddBook_Click(object sender, RoutedEventArgs e)
{
    // Validate input
    if (string.IsNullOrWhiteSpace(txtTitle.Text))
    {
        MessageBox.Show("Please enter a title.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
        txtTitle.Focus();  // Put cursor in title field
        return;
    }

    // Create book object
    Book newBook = new Book
    {
        Title = txtTitle.Text.Trim(),
        ISBN = txtISBN.Text.Trim(),
        YearPublished = int.Parse(txtYear.Text)
    };

    // Save to database
    int newId = DatabaseHelper.InsertBook(newBook);

    if (newId > 0)
    {
        MessageBox.Show("Book added successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

        // Refresh the DataGrid
        LoadBooks();

        // Clear the form
        ClearForm();
    }
}
```

**5. Control returns to waiting for next event**

---

## Data Binding

### What is Data Binding?

Data binding **automatically connects** UI controls to data. When the data changes, the UI updates. When the user changes the UI, the data updates.

```
Without binding:                    With binding:
dgBooks.ItemsSource = books;        <DataGrid ItemsSource="{Binding Books}"/>
// Must manually refresh            // Automatically stays in sync
```

### Simple Binding in XAML

#### Binding to Control Properties

```xml
<!-- Display a property from the DataContext -->
<TextBlock Text="{Binding Title}"/>

<!-- Two-way binding (UI updates data too) -->
<TextBox Text="{Binding Title, Mode=TwoWay}"/>
```

#### Binding DataGrid Columns

*Library System Example:*
```xml
<DataGrid x:Name="dgBooks" AutoGenerateColumns="False" IsReadOnly="True">
    <DataGrid.Columns>
        <DataGridTextColumn Header="ID" Binding="{Binding BookID}" Width="50"/>
        <DataGridTextColumn Header="Title" Binding="{Binding Title}" Width="*"/>
        <DataGridTextColumn Header="ISBN" Binding="{Binding ISBN}" Width="120"/>
        <DataGridTextColumn Header="Year" Binding="{Binding YearPublished}" Width="80"/>
    </DataGrid.Columns>
</DataGrid>
```

**Explanation:**
- `AutoGenerateColumns="False"` - we define columns ourselves
- `Binding="{Binding PropertyName}"` - binds column to property of the item
- `Width="*"` - takes remaining space

### Setting ItemsSource in Code

The simplest approach (used in this exemplar):

```csharp
private void LoadBooks()
{
    // Get books from database
    List<Book> books = DatabaseHelper.GetAllBooks();

    // Bind to DataGrid
    dgBooks.ItemsSource = books;
}
```

When you set `ItemsSource`, the DataGrid:
1. Creates one row per item in the list
2. Uses bindings to display properties in each column
3. Allows selection (SelectedItem gives you the object)

### Getting Selected Item

```csharp
private void BtnEdit_Click(object sender, RoutedEventArgs e)
{
    // Get selected book from DataGrid
    if (dgBooks.SelectedItem == null)
    {
        MessageBox.Show("Please select a book first.");
        return;
    }

    // Cast to the correct type
    Book selectedBook = (Book)dgBooks.SelectedItem;

    // Now you can use it
    txtTitle.Text = selectedBook.Title;
    txtYear.Text = selectedBook.YearPublished.ToString();
}
```

### Binding ComboBox

*Library System Example - Selecting a member for a loan:*

**XAML:**
```xml
<ComboBox x:Name="cboMember"
          DisplayMemberPath="FullName"
          SelectedValuePath="MemberID"
          Width="200"/>
```

**Code:**
```csharp
// Load members into ComboBox
private void LoadMembers()
{
    List<Member> members = DatabaseHelper.GetAllMembers();
    cboMember.ItemsSource = members;
}

// Get selected member ID
private void BtnCreateLoan_Click(object sender, RoutedEventArgs e)
{
    if (cboMember.SelectedValue == null)
    {
        MessageBox.Show("Please select a member.");
        return;
    }

    int memberId = (int)cboMember.SelectedValue;
    // Use memberId...
}
```

**Key properties:**
- `DisplayMemberPath` - which property to show in dropdown
- `SelectedValuePath` - which property to return as SelectedValue
- `ItemsSource` - the list of items
- `SelectedItem` - the whole selected object
- `SelectedValue` - just the value from SelectedValuePath

---

## Common WPF Controls

### TextBlock (Display text)

```xml
<!-- Simple text -->
<TextBlock Text="Welcome to the Library System"/>

<!-- Styled text -->
<TextBlock Text="Books" FontSize="24" FontWeight="Bold" Foreground="Navy"/>
```

**Use for:** Labels, headings, displaying read-only text

### TextBox (Text input)

```xml
<!-- Basic text input -->
<TextBox x:Name="txtTitle" Width="200"/>

<!-- With placeholder-like text -->
<TextBox x:Name="txtSearch" Text="Search..." GotFocus="TxtSearch_GotFocus"/>

<!-- Multi-line -->
<TextBox x:Name="txtNotes" AcceptsReturn="True" Height="100"
         TextWrapping="Wrap" VerticalScrollBarVisibility="Auto"/>
```

**Code access:**
```csharp
string title = txtTitle.Text;           // Get value
txtTitle.Text = "New Title";            // Set value
txtTitle.Clear();                       // Clear
txtTitle.Focus();                       // Put cursor here
```

### Button

```xml
<!-- Basic button -->
<Button x:Name="btnSave" Content="Save" Click="BtnSave_Click"/>

<!-- Styled button -->
<Button Content="Delete"
        Background="Red" Foreground="White"
        Padding="10,5" Margin="5"/>
```

### ComboBox (Dropdown)

```xml
<!-- Static items -->
<ComboBox x:Name="cboMemberType" Width="150">
    <ComboBoxItem Content="Student"/>
    <ComboBoxItem Content="Teacher"/>
    <ComboBoxItem Content="Staff"/>
</ComboBox>

<!-- Data-bound -->
<ComboBox x:Name="cboBooks"
          DisplayMemberPath="Title"
          SelectedValuePath="BookID"/>
```

**Code access:**
```csharp
// Get selected value
ComboBoxItem selected = (ComboBoxItem)cboMemberType.SelectedItem;
string memberType = selected.Content.ToString();

// Or for data-bound:
int bookId = (int)cboBooks.SelectedValue;
Book selectedBook = (Book)cboBooks.SelectedItem;

// Set selection
cboMemberType.SelectedIndex = 0;  // First item
```

### DatePicker

```xml
<DatePicker x:Name="dpLoanDate" SelectedDate="{x:Static sys:DateTime.Today}"/>
```

**Code access:**
```csharp
// Get date (nullable)
if (dpLoanDate.SelectedDate.HasValue)
{
    DateTime loanDate = dpLoanDate.SelectedDate.Value;
}

// Set date
dpLoanDate.SelectedDate = DateTime.Today;
dpLoanDate.SelectedDate = DateTime.Today.AddDays(14);  // 2 weeks from now
```

### CheckBox

```xml
<CheckBox x:Name="chkReturned"
          Content="Book has been returned"
          Checked="ChkReturned_Checked"
          Unchecked="ChkReturned_Unchecked"/>
```

**Code access:**
```csharp
bool isChecked = chkReturned.IsChecked == true;  // Note: IsChecked is nullable

chkReturned.IsChecked = true;
chkReturned.IsChecked = false;
```

### DataGrid

```xml
<DataGrid x:Name="dgBooks"
          AutoGenerateColumns="False"
          IsReadOnly="True"
          SelectionMode="Single"
          SelectionChanged="DgBooks_SelectionChanged">
    <DataGrid.Columns>
        <DataGridTextColumn Header="ID" Binding="{Binding BookID}" Width="50"/>
        <DataGridTextColumn Header="Title" Binding="{Binding Title}" Width="*"/>
        <DataGridTextColumn Header="Year" Binding="{Binding YearPublished}" Width="80"/>
    </DataGrid.Columns>
</DataGrid>
```

**Key properties:**
- `AutoGenerateColumns="False"` - define columns manually
- `IsReadOnly="True"` - prevent editing in grid
- `SelectionMode="Single"` - one row at a time
- `SelectionChanged` - event when selection changes

**Code access:**
```csharp
// Set data
dgBooks.ItemsSource = booksList;

// Get selected item
Book selected = (Book)dgBooks.SelectedItem;

// Clear selection
dgBooks.SelectedItem = null;

// Refresh display
dgBooks.Items.Refresh();
```

### ListBox

```xml
<ListBox x:Name="lstAuthors"
         DisplayMemberPath="FullName"
         Height="150"/>
```

Similar to DataGrid but simpler - just a list of items.

### MessageBox

Not a XAML control, but essential for user feedback:

```csharp
// Simple message
MessageBox.Show("Book saved successfully!");

// With title
MessageBox.Show("Book saved successfully!", "Success");

// With buttons and icon
MessageBox.Show(
    "Are you sure you want to delete this book?",
    "Confirm Delete",
    MessageBoxButton.YesNo,
    MessageBoxImage.Warning);

// Check which button was clicked
MessageBoxResult result = MessageBox.Show(
    "Delete this book?",
    "Confirm",
    MessageBoxButton.YesNo,
    MessageBoxImage.Question);

if (result == MessageBoxResult.Yes)
{
    // User clicked Yes
}
```

**Button options:** OK, OKCancel, YesNo, YesNoCancel
**Icon options:** None, Information, Warning, Error, Question

---

## Layout with Grid and StackPanel

Understanding layout is crucial for building good WPF interfaces. The two most important layout controls are **Grid** and **StackPanel**.

### Planning Your Layout on Paper First

**Before writing any XAML**, sketch your window layout on paper. This helps you:
- Work out how many rows and columns you need
- Identify where to use Grid vs StackPanel
- Avoid frustrating trial-and-error in code

#### How to Plan a Layout

1. **Sketch the overall window** - draw a rectangle
2. **Draw the major sections** - where does the title go? Where's the DataGrid? Where are the buttons?
3. **Draw grid lines** - horizontal lines for rows, vertical lines for columns
4. **Number the rows and columns** - Row 0, Row 1, etc.
5. **Note the sizing** - which rows/columns are fixed? Which should stretch?

*Library System Example - Planning the BooksWindow:*

```
┌──────────────────────────────────────────────────────────────┐
│  BOOKS  (title)                                  Row 0: Auto │
├────────────────────────────────────────────────┬─────────────┤
│                                                │             │
│     DataGrid                                   │   Form      │
│     (list of books)                            │   Panel     │
│                                                │             │
│                                                │  Title:     │
│                                                │  [_______]  │
│                                                │             │
│                                                │  ISBN:      │
│     Row 1: * (fills space)                     │  [_______]  │
│                                                │             │
│                                                │  Year:      │
│                                                │  [_______]  │
│                                                │             │
│                                                │  [Save]     │
│                                                │             │
├────────────────────────────────────────────────┴─────────────┤
│  [Refresh]  [Delete]                             Row 2: Auto │
└──────────────────────────────────────────────────────────────┘
    Column 0: *                                   Column 1: 300
```

**From this sketch, I can see:**
- Need a Grid with 3 rows and 2 columns
- Row 0: Auto (title), Row 1: * (main content), Row 2: Auto (buttons)
- Column 0: * (DataGrid stretches), Column 1: 300 (fixed width form)
- The form panel can use a StackPanel inside (vertical stack of labels and textboxes)
- The buttons can use a horizontal StackPanel

### Choosing Between Grid and StackPanel

| Use Grid When | Use StackPanel When |
|---------------|---------------------|
| You need precise control over position | You want items in a simple list |
| Controls should align in rows AND columns | Items just need to stack vertically or horizontally |
| Different areas need different sizing | All items flow one after another |
| You need controls to span multiple cells | You're arranging a row of buttons |
| Complex, form-like layouts | Simple, linear arrangements |

**Rule of thumb:** Use Grid for the overall window structure, StackPanel for groups of related controls within Grid cells.

### Grid Layout

Grid is the most flexible layout - divides space into rows and columns. Think of it like a table in a Word document.

#### When to Use Grid

- Main window layout (dividing into sections)
- Form layouts with labels and input fields
- Any layout requiring alignment in two dimensions
- When you need controls to stretch to fill space

#### Basic Structure

```xml
<Grid>
    <!-- Define rows -->
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>      <!-- Row 0: size to content -->
        <RowDefinition Height="*"/>         <!-- Row 1: fill remaining space -->
        <RowDefinition Height="50"/>        <!-- Row 2: fixed 50 pixels -->
    </Grid.RowDefinitions>

    <!-- Define columns -->
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="200"/>     <!-- Column 0: fixed 200 pixels -->
        <ColumnDefinition Width="*"/>       <!-- Column 1: fill remaining space -->
    </Grid.ColumnDefinitions>

    <!-- Place controls using Grid.Row and Grid.Column -->
    <TextBlock Text="Title:" Grid.Row="0" Grid.Column="0"/>
    <TextBox x:Name="txtTitle" Grid.Row="0" Grid.Column="1"/>

    <DataGrid Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="2"/>

    <Button Content="Save" Grid.Row="2" Grid.Column="1"/>
</Grid>
```

#### Height/Width Values

| Value | Meaning |
|-------|---------|
| `Auto` | Size to fit content |
| `*` | Take remaining space |
| `2*` | Take twice as much as `*` |
| `100` | Fixed 100 pixels |

#### Grid.RowSpan and Grid.ColumnSpan

```xml
<!-- Control spans 2 columns -->
<DataGrid Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="2"/>

<!-- Control spans 3 rows -->
<ListBox Grid.Row="0" Grid.Column="2" Grid.RowSpan="3"/>
```

### StackPanel Layout

StackPanel simply stacks controls one after another, either vertically (default) or horizontally. It's much simpler than Grid but less flexible.

#### When to Use StackPanel

- Row of buttons (horizontal)
- Form fields that stack top-to-bottom (vertical)
- Any simple linear arrangement
- Inside Grid cells to group related controls

#### When NOT to Use StackPanel

- Main window layout (use Grid instead)
- When you need things to align in rows AND columns
- When you need controls to stretch to fill space (StackPanel doesn't do this well)

#### Vertical StackPanel (Default)

```xml
<!-- Items stack top to bottom -->
<StackPanel>
    <TextBlock Text="First"/>
    <TextBlock Text="Second"/>
    <TextBlock Text="Third"/>
</StackPanel>
```

Result:
```
First
Second
Third
```

#### Horizontal StackPanel

```xml
<!-- Items stack left to right -->
<StackPanel Orientation="Horizontal">
    <Button Content="Save" Margin="5"/>
    <Button Content="Cancel" Margin="5"/>
    <Button Content="Delete" Margin="5"/>
</StackPanel>
```

Result:
```
[Save] [Cancel] [Delete]
```

#### StackPanel for a Form

*Library System Example - Add Book form using StackPanel:*
```xml
<StackPanel Width="250">
    <TextBlock Text="Title:" FontWeight="Bold"/>
    <TextBox x:Name="txtTitle" Margin="0,0,0,10"/>

    <TextBlock Text="ISBN:"/>
    <TextBox x:Name="txtISBN" Margin="0,0,0,10"/>

    <TextBlock Text="Year Published:"/>
    <TextBox x:Name="txtYear" Margin="0,0,0,15"/>

    <StackPanel Orientation="Horizontal">
        <Button Content="Save" Width="80" Margin="0,0,10,0"/>
        <Button Content="Clear" Width="80"/>
    </StackPanel>
</StackPanel>
```

Notice how we nest a **horizontal** StackPanel inside a **vertical** StackPanel for the button row.

### Combining Grid and StackPanel

The best layouts combine both:
- **Grid** for the overall window structure
- **StackPanel** for groups of controls within Grid cells

This is exactly what the paper sketch showed earlier - Grid divides the window, StackPanels organise controls within each section.

### Library System Example - Complete Layout

*BooksWindow layout structure:*

```xml
<Window x:Class="LibraryManagementSystem.BooksWindow"
        Title="Manage Books" Height="600" Width="900">

    <Grid Margin="10">
        <!-- Define main layout: left side for list, right side for form -->
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*"/>      <!-- Books list -->
            <ColumnDefinition Width="300"/>    <!-- Form panel -->
        </Grid.ColumnDefinitions>

        <!-- LEFT SIDE: Books list -->
        <Grid Grid.Column="0" Margin="0,0,10,0">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>   <!-- Title -->
                <RowDefinition Height="*"/>      <!-- DataGrid -->
                <RowDefinition Height="Auto"/>   <!-- Buttons -->
            </Grid.RowDefinitions>

            <TextBlock Grid.Row="0" Text="Books" FontSize="24" FontWeight="Bold"/>

            <DataGrid Grid.Row="1" x:Name="dgBooks" Margin="0,10"/>

            <StackPanel Grid.Row="2" Orientation="Horizontal">
                <Button Content="Refresh" Click="BtnRefresh_Click" Margin="0,0,5,0"/>
                <Button Content="Delete" Click="BtnDelete_Click"/>
            </StackPanel>
        </Grid>

        <!-- RIGHT SIDE: Add/Edit form -->
        <Border Grid.Column="1" BorderBrush="Gray" BorderThickness="1" Padding="10">
            <StackPanel>
                <TextBlock x:Name="txtFormTitle" Text="Add New Book"
                           FontSize="18" FontWeight="Bold" Margin="0,0,0,15"/>

                <TextBlock Text="Title:"/>
                <TextBox x:Name="txtTitle" Margin="0,0,0,10"/>

                <TextBlock Text="ISBN:"/>
                <TextBox x:Name="txtISBN" Margin="0,0,0,10"/>

                <TextBlock Text="Year Published:"/>
                <TextBox x:Name="txtYear" Margin="0,0,0,15"/>

                <StackPanel Orientation="Horizontal">
                    <Button Content="Save" Click="BtnSave_Click" Width="80" Margin="0,0,10,0"/>
                    <Button Content="Clear" Click="BtnClear_Click" Width="80"/>
                </StackPanel>
            </StackPanel>
        </Border>
    </Grid>

</Window>
```

---

## Building Your First WPF Window

### Step-by-Step: Create the Books Window

#### Step 1: Create the Project

1. Open Visual Studio
2. File → New → Project
3. Select "WPF Application"
4. Name: `LibraryManagementSystem`
5. Choose .NET 8.0 (or latest)
6. Click Create

#### Step 2: Add the Model Class

Right-click project → Add → New Folder → Name it `Models`
Right-click Models → Add → Class → `Book.cs`

```csharp
namespace LibraryManagementSystem.Models
{
    /// <summary>
    /// Represents a book in the library.
    /// </summary>
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int YearPublished { get; set; }

        /// <summary>
        /// Validates that the book has required data.
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Title)) return false;
            if (YearPublished < 1000 || YearPublished > DateTime.Now.Year + 1) return false;
            return true;
        }

        public override string ToString()
        {
            return $"{Title} ({YearPublished})";
        }
    }
}
```

#### Step 3: Design the XAML

Open `MainWindow.xaml` and replace with:

```xml
<Window x:Class="LibraryManagementSystem.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Library Management System - Books" Height="500" Width="700">

    <Grid Margin="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Title -->
        <TextBlock Grid.Row="0" Text="Books" FontSize="24" FontWeight="Bold" Margin="0,0,0,10"/>

        <!-- DataGrid showing books -->
        <DataGrid Grid.Row="1" x:Name="dgBooks"
                  AutoGenerateColumns="False"
                  IsReadOnly="True"
                  SelectionChanged="DgBooks_SelectionChanged">
            <DataGrid.Columns>
                <DataGridTextColumn Header="ID" Binding="{Binding BookID}" Width="50"/>
                <DataGridTextColumn Header="Title" Binding="{Binding Title}" Width="*"/>
                <DataGridTextColumn Header="ISBN" Binding="{Binding ISBN}" Width="120"/>
                <DataGridTextColumn Header="Year" Binding="{Binding YearPublished}" Width="80"/>
            </DataGrid.Columns>
        </DataGrid>

        <!-- Input form -->
        <StackPanel Grid.Row="2" Margin="0,10">
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="80"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="80"/>
                    <ColumnDefinition Width="100"/>
                    <ColumnDefinition Width="80"/>
                    <ColumnDefinition Width="80"/>
                </Grid.ColumnDefinitions>

                <TextBlock Grid.Column="0" Text="Title:" VerticalAlignment="Center"/>
                <TextBox Grid.Column="1" x:Name="txtTitle" Margin="5,0"/>

                <TextBlock Grid.Column="2" Text="ISBN:" VerticalAlignment="Center"/>
                <TextBox Grid.Column="3" x:Name="txtISBN" Margin="5,0"/>

                <TextBlock Grid.Column="4" Text="Year:" VerticalAlignment="Center"/>
                <TextBox Grid.Column="5" x:Name="txtYear"/>
            </Grid>
        </StackPanel>

        <!-- Buttons -->
        <StackPanel Grid.Row="3" Orientation="Horizontal" HorizontalAlignment="Right">
            <Button x:Name="btnAdd" Content="Add" Width="80" Margin="5" Click="BtnAdd_Click"/>
            <Button x:Name="btnUpdate" Content="Update" Width="80" Margin="5" Click="BtnUpdate_Click"/>
            <Button x:Name="btnDelete" Content="Delete" Width="80" Margin="5" Click="BtnDelete_Click"/>
            <Button x:Name="btnClear" Content="Clear" Width="80" Margin="5" Click="BtnClear_Click"/>
        </StackPanel>
    </Grid>

</Window>
```

#### Step 4: Write the Code-Behind

Open `MainWindow.xaml.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem
{
    public partial class MainWindow : Window
    {
        // Store currently selected book for editing
        private Book selectedBook = null;

        // Sample data (replace with DatabaseHelper in real project)
        private List<Book> books = new List<Book>();

        public MainWindow()
        {
            InitializeComponent();

            // Add sample data
            books.Add(new Book { BookID = 1, Title = "The Hobbit", ISBN = "978-0261103283", YearPublished = 1937 });
            books.Add(new Book { BookID = 2, Title = "1984", ISBN = "978-0451524935", YearPublished = 1949 });
            books.Add(new Book { BookID = 3, Title = "Pride and Prejudice", ISBN = "978-0141439518", YearPublished = 1813 });

            // Load data when window opens
            LoadBooks();
        }

        /// <summary>
        /// Loads books into the DataGrid.
        /// </summary>
        private void LoadBooks()
        {
            dgBooks.ItemsSource = null;  // Clear first
            dgBooks.ItemsSource = books;
        }

        /// <summary>
        /// Clears the input form.
        /// </summary>
        private void ClearForm()
        {
            txtTitle.Text = "";
            txtISBN.Text = "";
            txtYear.Text = "";
            selectedBook = null;
            dgBooks.SelectedItem = null;
        }

        /// <summary>
        /// Handles DataGrid selection change.
        /// Populates form with selected book data.
        /// </summary>
        private void DgBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBooks.SelectedItem != null)
            {
                selectedBook = (Book)dgBooks.SelectedItem;
                txtTitle.Text = selectedBook.Title;
                txtISBN.Text = selectedBook.ISBN;
                txtYear.Text = selectedBook.YearPublished.ToString();
            }
        }

        /// <summary>
        /// Adds a new book.
        /// </summary>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a title.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTitle.Focus();
                return;
            }

            if (!int.TryParse(txtYear.Text, out int year))
            {
                MessageBox.Show("Please enter a valid year.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtYear.Focus();
                return;
            }

            // Create new book
            Book newBook = new Book
            {
                BookID = books.Count + 1,  // Simple ID generation
                Title = txtTitle.Text.Trim(),
                ISBN = txtISBN.Text.Trim(),
                YearPublished = year
            };

            // Add to list
            books.Add(newBook);

            // Refresh display
            LoadBooks();
            ClearForm();

            MessageBox.Show("Book added successfully!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Updates the selected book.
        /// </summary>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBook == null)
            {
                MessageBox.Show("Please select a book to update.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!int.TryParse(txtYear.Text, out int year))
            {
                MessageBox.Show("Please enter a valid year.", "Validation Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update the book
            selectedBook.Title = txtTitle.Text.Trim();
            selectedBook.ISBN = txtISBN.Text.Trim();
            selectedBook.YearPublished = year;

            // Refresh display
            LoadBooks();

            MessageBox.Show("Book updated successfully!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Deletes the selected book.
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBook == null)
            {
                MessageBox.Show("Please select a book to delete.", "No Selection",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Confirm deletion
            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete '{selectedBook.Title}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                books.Remove(selectedBook);
                LoadBooks();
                ClearForm();

                MessageBox.Show("Book deleted successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Clears the form.
        /// </summary>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
    }
}
```

#### Step 5: Run and Test

Press F5 to run. You should be able to:
- See the sample books in the DataGrid
- Click a book to select it and populate the form
- Add new books
- Update selected books
- Delete books with confirmation

---

## Connecting to Your Database

Once your WPF prototype works with sample data, connect it to your real database.

### Step 1: Add DatabaseHelper

Create `Database` folder and add `DatabaseHelper.cs`:

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Database
{
    public static class DatabaseHelper
    {
        private static string connectionString = "Data Source=LibraryDatabase.db";

        public static List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();

            try
            {
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting books: {ex.Message}");
            }

            return books;
        }

        // Add InsertBook, UpdateBook, DeleteBook methods...
    }
}
```

### Step 2: Update Window Code

Replace sample data with database calls:

```csharp
// Before:
private List<Book> books = new List<Book>();

// After:
private void LoadBooks()
{
    dgBooks.ItemsSource = DatabaseHelper.GetAllBooks();
}

// Before (in BtnAdd_Click):
books.Add(newBook);

// After:
int newId = DatabaseHelper.InsertBook(newBook);
if (newId > 0)
{
    MessageBox.Show("Book added successfully!");
    LoadBooks();
    ClearForm();
}
```

---

## Design Patterns and Best Practices

### 1. Separation of Concerns

Keep different responsibilities in different places:

```
Models/           → Data structures (Book, Member, Loan)
Database/         → Database operations (DatabaseHelper)
Windows/          → User interface (MainWindow, BooksWindow)
```

### 2. Single Responsibility for Methods

Each method should do ONE thing:

```csharp
// GOOD: Each method has one job
private void LoadBooks() { /* load books */ }
private void ClearForm() { /* clear form */ }
private bool ValidateInput() { /* validate */ }
private void SaveBook() { /* save to database */ }

// BAD: One method does everything
private void DoEverything() { /* load, clear, validate, save... */ }
```

### 3. Consistent Error Handling

Always wrap database operations in try-catch:

```csharp
private void LoadBooks()
{
    try
    {
        dgBooks.ItemsSource = DatabaseHelper.GetAllBooks();
    }
    catch (Exception ex)
    {
        MessageBox.Show(
            $"Error loading books:\n\n{ex.Message}",
            "Database Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
```

### 4. Use Regions to Organise Code

```csharp
public partial class BooksWindow : Window
{
    #region Fields
    private Book selectedBook = null;
    #endregion

    #region Constructor
    public BooksWindow()
    {
        InitializeComponent();
        LoadBooks();
    }
    #endregion

    #region Data Loading Methods
    private void LoadBooks() { /* ... */ }
    #endregion

    #region Form Methods
    private void ClearForm() { /* ... */ }
    private bool ValidateForm() { /* ... */ }
    #endregion

    #region Event Handlers
    private void BtnSave_Click(object sender, RoutedEventArgs e) { /* ... */ }
    private void BtnDelete_Click(object sender, RoutedEventArgs e) { /* ... */ }
    #endregion
}
```

### 5. XML Documentation

Document your methods:

```csharp
/// <summary>
/// Loads all books from the database and displays them in the DataGrid.
/// </summary>
private void LoadBooks()
{
    dgBooks.ItemsSource = DatabaseHelper.GetAllBooks();
}

/// <summary>
/// Validates the form input before saving.
/// </summary>
/// <returns>True if valid, false otherwise.</returns>
private bool ValidateForm()
{
    // ...
}
```

---

## Summary

### Key Concepts Covered

1. **WPF Structure:** XAML for appearance, code-behind for behaviour
2. **XAML:** XML-based markup for defining UI
3. **Event-Driven:** Application waits for and responds to user actions
4. **Data Binding:** Connecting UI controls to data automatically
5. **Common Controls:** TextBox, Button, ComboBox, DataGrid, etc.
6. **Layout:** Grid and StackPanel for arranging controls

### Development Order for Your NEA

1. Design your database and test with DB Browser
2. Create Console prototype to test CRUD operations
3. Create WPF project and add Model classes
4. Build simple window with DataGrid
5. Add data binding to display records
6. Add form controls for input
7. Connect event handlers for CRUD operations
8. Connect to DatabaseHelper
9. Add validation and error handling
10. Polish and refine

### Quick Reference

| Task | How To |
|------|--------|
| Access control by name | Use `x:Name="controlName"` in XAML, then `controlName.Property` in code |
| Handle button click | Add `Click="HandlerName"` in XAML, create matching method in code |
| Display list in DataGrid | Set `dgName.ItemsSource = yourList` |
| Get selected item | `(YourType)dgName.SelectedItem` |
| Show message to user | `MessageBox.Show("message", "title")` |
| Clear a TextBox | `txtName.Text = ""` or `txtName.Clear()` |
| Put focus on control | `txtName.Focus()` |

---

**Document created by:** Claude AI (Anthropic)
**Purpose:** Educational resource for AQA 7517 A-Level Computer Science NEA projects

**Disclaimer:** Always refer to official Microsoft documentation for definitive guidance on WPF.
