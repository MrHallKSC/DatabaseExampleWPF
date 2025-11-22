using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DatabaseExampleWPF.Database;
using DatabaseExampleWPF.Models;

namespace DatabaseExampleWPF
{
    /// <summary>
    /// Interaction logic for AuthorsWindow.xaml
    /// 
    /// This window demonstrates:
    /// - CRUD operations for the Authors table
    /// - Displaying data in a DataGrid
    /// - Form validation for text fields
    /// - Querying the many-to-many relationship from the Author side
    /// - Using a simpler form (only two fields) compared to Books
    /// </summary>
    public partial class AuthorsWindow : Window
    {
        #region Fields

        /// <summary>
        /// Stores the currently selected author for editing
        /// null if we're adding a new author
        /// </summary>
        private Author selectedAuthor = null;

        #endregion

        #region Constructor and Initialization

        /// <summary>
        /// Constructor - initializes the window
        /// </summary>
        public AuthorsWindow()
        {
            InitializeComponent();

            // Load data when window opens
            LoadAuthors();

            // Hide books display until an author is selected
            UpdateBooksDisplayVisibility(false);
        }

        #endregion

        #region Data Loading Methods

        /// <summary>
        /// Loads all authors from database and displays them in the DataGrid
        /// </summary>
        private void LoadAuthors()
        {
            try
            {
                // Get all authors from database
                List<Author> authors = DatabaseHelper.GetAllAuthors();

                // Bind to DataGrid
                dgAuthors.ItemsSource = authors;

                // Show message if no authors found
                if (authors.Count == 0)
                {
                    MessageBox.Show(
                        "No authors found in the database.\n\n" +
                        "Add your first author using the form on the right!",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
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
        /// Loads books written by the selected author
        /// Demonstrates querying the many-to-many relationship from Author side
        /// Uses the GetBooksForAuthor method which performs a JOIN query
        /// </summary>
        private void LoadAuthorBooks()
        {
            try
            {
                if (selectedAuthor == null || selectedAuthor.AuthorID <= 0)
                {
                    lstAuthorBooks.ItemsSource = null;
                    return;
                }

                // Get books for this author from the BookAuthors junction table
                // This uses a JOIN query: Books JOIN BookAuthors ON BookID WHERE AuthorID = X
                List<Book> books = DatabaseHelper.GetBooksForAuthor(selectedAuthor.AuthorID);

                // Display in the ListBox
                lstAuthorBooks.ItemsSource = books;

                // Optional: Show a message if author has no books
                if (books.Count == 0)
                {
                    // You could show a message or update the UI to indicate no books
                    System.Diagnostics.Debug.WriteLine($"Author {selectedAuthor.FullName} has no books assigned.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading books for author:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Form Management Methods

        /// <summary>
        /// Clears all input fields in the form
        /// </summary>
        private void ClearForm()
        {
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;

            selectedAuthor = null;

            // Update form title
            txtFormTitle.Text = "Add New Author";

            // Clear DataGrid selection
            dgAuthors.SelectedItem = null;

            // Hide books display
            UpdateBooksDisplayVisibility(false);
        }

        /// <summary>
        /// Populates the form with data from a selected author for editing
        /// </summary>
        /// <param name="author">The author to edit</param>
        private void PopulateForm(Author author)
        {
            if (author == null) return;

            // Store the selected author
            selectedAuthor = author;

            // Populate the form fields
            txtFirstName.Text = author.FirstName;
            txtLastName.Text = author.LastName;

            // Update form title
            txtFormTitle.Text = $"Edit Author (ID: {author.AuthorID})";

            // Show books by this author
            UpdateBooksDisplayVisibility(true);
            LoadAuthorBooks();
        }

        /// <summary>
        /// Shows or hides the books display section
        /// </summary>
        /// <param name="show">True to show, false to hide</param>
        private void UpdateBooksDisplayVisibility(bool show)
        {
            lstAuthorBooks.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            txtNoAuthorSelected.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Validates the form input before saving
        /// Returns an Author object if valid, null if invalid
        /// </summary>
        /// <returns>Valid Author object or null</returns>
        private Author ValidateAndCreateAuthor()
        {
            // Create a new author object (or use existing for updates)
            Author author = selectedAuthor ?? new Author();

            // Validate First Name
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show(
                    "Please enter the author's first name.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtFirstName.Focus();
                return null;
            }
            author.FirstName = txtFirstName.Text.Trim();

            // Validate Last Name
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show(
                    "Please enter the author's last name.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtLastName.Focus();
                return null;
            }
            author.LastName = txtLastName.Text.Trim();

            // Use the Author class's validation method
            if (!author.IsValid())
            {
                MessageBox.Show(
                    $"Validation failed:\n\n{author.GetValidationErrors()}",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }

            return author;
        }

        #endregion

        #region Button Event Handlers

        /// <summary>
        /// Saves an author (either INSERT for new or UPDATE for existing)
        /// Demonstrates CREATE and UPDATE operations
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate the form and get an Author object
                Author author = ValidateAndCreateAuthor();
                if (author == null) return; // Validation failed

                bool success;
                string action;

                if (author.AuthorID == 0)
                {
                    // INSERT - adding a new author
                    action = "added";
                    int newId = DatabaseHelper.InsertAuthor(author);
                    success = newId > 0;

                    if (success)
                    {
                        author.AuthorID = newId; // Store the new ID
                    }
                }
                else
                {
                    // UPDATE - editing existing author
                    action = "updated";
                    success = DatabaseHelper.UpdateAuthor(author);
                }

                if (success)
                {
                    MessageBox.Show(
                        $"Author '{author.FullName}' {action} successfully!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Refresh the list to show changes
                    LoadAuthors();

                    // Clear the form for next entry
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(
                        $"Failed to {action.TrimEnd('d')} author.\n\n" +
                        "Check the debug output for details.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error saving author:\n\n{ex.Message}",
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
        /// Refreshes the authors list from the database
        /// </summary>
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAuthors();
        }

        /// <summary>
        /// Deletes the selected author
        /// Demonstrates DELETE operation with user confirmation
        /// 
        /// Note: Due to CASCADE DELETE in BookAuthors table,
        /// deleting an author will automatically remove all BookAuthor relationships
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if an author is selected
                if (dgAuthors.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Please select an author to delete.",
                        "No Selection",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                Author authorToDelete = (Author)dgAuthors.SelectedItem;

                // Get books by this author to warn user
                List<Book> authorBooks = DatabaseHelper.GetBooksForAuthor(authorToDelete.AuthorID);

                string warningMessage = $"Are you sure you want to delete author:\n\n" +
                                      $"'{authorToDelete.FullName}'?\n\n";

                if (authorBooks.Count > 0)
                {
                    warningMessage += $"This author is associated with {authorBooks.Count} book(s).\n" +
                                    "The author will be removed from those books.\n\n";
                }

                warningMessage += "This action cannot be undone!";

                // Ask for confirmation before deleting
                MessageBoxResult result = MessageBox.Show(
                    warningMessage,
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                // Only delete if user clicked Yes
                if (result == MessageBoxResult.Yes)
                {
                    bool success = DatabaseHelper.DeleteAuthor(authorToDelete.AuthorID);

                    if (success)
                    {
                        MessageBox.Show(
                            $"Author '{authorToDelete.FullName}' deleted successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Refresh the list
                        LoadAuthors();

                        // Clear the form if we were editing this author
                        if (selectedAuthor?.AuthorID == authorToDelete.AuthorID)
                        {
                            ClearForm();
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to delete author.\n\n" +
                            "An unexpected error occurred.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error deleting author:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region DataGrid Event Handlers

        /// <summary>
        /// Handles when a user selects a different row in the DataGrid
        /// Populates the form with the selected author's data for editing
        /// </summary>
        private void DgAuthors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if a row is selected
            if (dgAuthors.SelectedItem != null)
            {
                // Get the selected author
                Author author = (Author)dgAuthors.SelectedItem;

                // Populate the form for editing
                PopulateForm(author);
            }
        }

        #endregion
    }
}