using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DatabaseExampleWPF.Database;
using DatabaseExampleWPF.Models;

namespace DatabaseExampleWPF
{
    /// <summary>
    /// Interaction logic for BooksWindow.xaml
    /// 
    /// This window demonstrates:
    /// - Displaying data in a DataGrid
    /// - INSERT operations (adding new books)
    /// - UPDATE operations (editing existing books)
    /// - DELETE operations (removing books)
    /// - Managing many-to-many relationships (Books-Authors via BookAuthors table)
    /// - Form validation
    /// - Data binding between UI controls and objects
    /// </summary>
    public partial class BooksWindow : Window
    {
        #region Fields

        /// <summary>
        /// Stores the currently selected book for editing
        /// null if we're adding a new book
        /// This is a common pattern - reusing the same form for add and edit
        /// </summary>
        private Book selectedBook = null;

        #endregion

        #region Constructor and Initialization

        /// <summary>
        /// Constructor - initializes the window
        /// </summary>
        public BooksWindow()
        {
            InitializeComponent();

            // Load data when window opens
            LoadBooks();
            LoadAuthors();

            // Hide author management until a book is selected
            UpdateAuthorManagementVisibility(false);
        }

        #endregion

        #region Data Loading Methods

        /// <summary>
        /// Loads all books from database and displays them in the DataGrid
        /// This demonstrates reading data and binding it to UI controls
        /// </summary>
        private void LoadBooks()
        {
            try
            {
                // Get all books from database using DatabaseHelper
                List<Book> books = DatabaseHelper.GetAllBooks();

                // Set the DataGrid's ItemsSource to the list of books
                // This is DATA BINDING - connecting data to UI
                // The DataGrid will automatically display all books
                dgBooks.ItemsSource = books;

                // If there are no books, show a message
                if (books.Count == 0)
                {
                    MessageBox.Show(
                        "No books found in the database.\n\n" +
                        "Add your first book using the form on the right!",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading books:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Loads all authors into the ComboBox for adding to books
        /// </summary>
        private void LoadAuthors()
        {
            try
            {
                List<Author> authors = DatabaseHelper.GetAllAuthors();

                // Set ItemsSource for the ComboBox
                cboAvailableAuthors.ItemsSource = authors;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading authors:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Loads authors assigned to the currently selected book
        /// Demonstrates querying the many-to-many relationship
        /// </summary>
        private void LoadAssignedAuthors()
        {
            try
            {
                if (selectedBook == null || selectedBook.BookID <= 0)
                {
                    lstAssignedAuthors.ItemsSource = null;
                    return;
                }

                // Get authors for this book from the BookAuthors junction table
                List<Author> authors = DatabaseHelper.GetAuthorsForBook(selectedBook.BookID);

                // Display in the ListBox
                lstAssignedAuthors.ItemsSource = authors;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading assigned authors:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Form Management Methods

        /// <summary>
        /// Clears all input fields in the form
        /// Used when adding a new book or after saving
        /// </summary>
        private void ClearForm()
        {
            txtTitle.Text = string.Empty;
            txtISBN.Text = string.Empty;
            txtYear.Text = string.Empty;

            selectedBook = null;

            // Update form title to show we're adding a new book
            txtFormTitle.Text = "Add New Book";

            // Clear selection in DataGrid
            dgBooks.SelectedItem = null;

            // Hide author management (no book selected)
            UpdateAuthorManagementVisibility(false);
        }

        /// <summary>
        /// Populates the form with data from a selected book for editing
        /// This demonstrates the EDIT part of CRUD
        /// </summary>
        /// <param name="book">The book to edit</param>
        private void PopulateForm(Book book)
        {
            if (book == null) return;

            // Store the selected book
            selectedBook = book;

            // Populate the form fields
            txtTitle.Text = book.Title;
            txtISBN.Text = book.ISBN;
            txtYear.Text = book.YearPublished.ToString();

            // Update form title to show we're editing
            txtFormTitle.Text = $"Edit Book (ID: {book.BookID})";

            // Show author management for this book
            UpdateAuthorManagementVisibility(true);
            LoadAssignedAuthors();
        }

        /// <summary>
        /// Shows or hides the author management section
        /// </summary>
        /// <param name="show">True to show, false to hide</param>
        private void UpdateAuthorManagementVisibility(bool show)
        {
            // Show/hide the controls
            lstAssignedAuthors.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            cboAvailableAuthors.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            btnAddAuthor.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            btnRemoveAuthor.Visibility = show ? Visibility.Visible : Visibility.Collapsed;

            // Show/hide the "no book selected" message
            txtNoBookSelected.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Validates the form input before saving
        /// Returns a Book object if valid, null if invalid
        /// This demonstrates INPUT VALIDATION
        /// </summary>
        /// <returns>Valid Book object or null</returns>
        private Book ValidateAndCreateBook()
        {
            // Create a new book object (or use existing for updates)
            Book book = selectedBook ?? new Book();

            // Validate Title
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show(
                    "Please enter a book title.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtTitle.Focus(); // Put cursor in the title field
                return null;
            }
            book.Title = txtTitle.Text.Trim();

            // ISBN is optional, so just set it
            book.ISBN = txtISBN.Text.Trim();

            // Validate Year
            if (string.IsNullOrWhiteSpace(txtYear.Text))
            {
                MessageBox.Show(
                    "Please enter the year published.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtYear.Focus();
                return null;
            }

            // Try to parse the year as an integer
            if (!int.TryParse(txtYear.Text, out int year))
            {
                MessageBox.Show(
                    "Year must be a valid number.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtYear.Focus();
                return null;
            }
            book.YearPublished = year;

            // Use the Book class's validation method
            if (!book.IsValid())
            {
                MessageBox.Show(
                    $"Validation failed:\n\n{book.GetValidationErrors()}",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }

            return book;
        }

        #endregion

        #region Button Event Handlers

        /// <summary>
        /// Saves a book (either INSERT for new or UPDATE for existing)
        /// Demonstrates both CREATE and UPDATE operations
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate the form and get a Book object
                Book book = ValidateAndCreateBook();
                if (book == null) return; // Validation failed

                bool success;
                string action;

                if (book.BookID == 0)
                {
                    // INSERT - adding a new book
                    action = "added";
                    int newId = DatabaseHelper.InsertBook(book);
                    success = newId > 0;

                    if (success)
                    {
                        book.BookID = newId; // Store the new ID
                    }
                }
                else
                {
                    // UPDATE - editing existing book
                    action = "updated";
                    success = DatabaseHelper.UpdateBook(book);
                }

                if (success)
                {
                    MessageBox.Show(
                        $"Book '{book.Title}' {action} successfully!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Refresh the list to show changes
                    LoadBooks();

                    // Clear the form for next entry
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(
                        $"Failed to {action.TrimEnd('d')} book.\n\n" +
                        "Check the debug output for details.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error saving book:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Clears the form
        /// </summary>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        /// <summary>
        /// Refreshes the books list from the database
        /// </summary>
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadBooks();
            LoadAuthors(); // Also refresh authors in case they were added in another window
        }

        /// <summary>
        /// Deletes the selected book
        /// Demonstrates DELETE operation with user confirmation
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if a book is selected in the DataGrid
                if (dgBooks.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Please select a book to delete.",
                        "No Selection",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                Book bookToDelete = (Book)dgBooks.SelectedItem;

                // Ask for confirmation before deleting
                // MessageBoxResult stores which button the user clicked
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete the book:\n\n" +
                    $"'{bookToDelete.Title}' ({bookToDelete.YearPublished})?\n\n" +
                    "This action cannot be undone!",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                // Only delete if user clicked Yes
                if (result == MessageBoxResult.Yes)
                {
                    bool success = DatabaseHelper.DeleteBook(bookToDelete.BookID);

                    if (success)
                    {
                        MessageBox.Show(
                            $"Book '{bookToDelete.Title}' deleted successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Refresh the list
                        LoadBooks();

                        // Clear the form if we were editing this book
                        if (selectedBook?.BookID == bookToDelete.BookID)
                        {
                            ClearForm();
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to delete book.\n\n" +
                            "The book may have active loans or other dependencies.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error deleting book:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Adds an author to the selected book
        /// Demonstrates INSERT into the BookAuthors junction table (many-to-many)
        /// </summary>
        private void BtnAddAuthor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate that a book is selected
                if (selectedBook == null || selectedBook.BookID <= 0)
                {
                    MessageBox.Show(
                        "Please select or save a book first.",
                        "No Book Selected",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                // Validate that an author is selected in the ComboBox
                if (cboAvailableAuthors.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Please select an author to add.",
                        "No Author Selected",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                Author authorToAdd = (Author)cboAvailableAuthors.SelectedItem;

                // Add the relationship to the BookAuthors table
                bool success = DatabaseHelper.AddBookAuthor(selectedBook.BookID, authorToAdd.AuthorID);

                if (success)
                {
                    MessageBox.Show(
                        $"Author '{authorToAdd.FullName}' added to book '{selectedBook.Title}'!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Refresh the assigned authors list
                    LoadAssignedAuthors();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to add author to book.\n\n" +
                        "This author may already be assigned to this book.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error adding author to book:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Removes an author from the selected book
        /// Demonstrates DELETE from the BookAuthors junction table (many-to-many)
        /// </summary>
        private void BtnRemoveAuthor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate that a book is selected
                if (selectedBook == null || selectedBook.BookID <= 0)
                {
                    MessageBox.Show(
                        "Please select a book first.",
                        "No Book Selected",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                // Validate that an author is selected in the ListBox
                if (lstAssignedAuthors.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Please select an author to remove from the list.",
                        "No Author Selected",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                Author authorToRemove = (Author)lstAssignedAuthors.SelectedItem;

                // Ask for confirmation
                MessageBoxResult result = MessageBox.Show(
                    $"Remove '{authorToRemove.FullName}' from '{selectedBook.Title}'?",
                    "Confirm Remove",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Remove the relationship from BookAuthors table
                    bool success = DatabaseHelper.RemoveBookAuthor(selectedBook.BookID, authorToRemove.AuthorID);

                    if (success)
                    {
                        MessageBox.Show(
                            $"Author '{authorToRemove.FullName}' removed from book '{selectedBook.Title}'!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Refresh the assigned authors list
                        LoadAssignedAuthors();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to remove author from book.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error removing author from book:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region DataGrid Event Handlers

        /// <summary>
        /// Handles when a user selects a different row in the DataGrid
        /// Populates the form with the selected book's data for editing
        /// </summary>
        private void DgBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if a row is selected
            if (dgBooks.SelectedItem != null)
            {
                // Get the selected book
                Book book = (Book)dgBooks.SelectedItem;

                // Populate the form for editing
                PopulateForm(book);
            }
        }

        #endregion
    }
}