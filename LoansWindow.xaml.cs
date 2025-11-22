using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DatabaseExampleWPF.Database;
using DatabaseExampleWPF.Models;

namespace DatabaseExampleWPF
{
    /// <summary>
    /// Interaction logic for LoansWindow.xaml
    /// 
    /// This window demonstrates:
    /// - CRUD operations for the Loans table
    /// - Working with foreign keys (BookID, MemberID)
    /// - Using ComboBoxes to select related entities
    /// - Working with DatePicker controls
    /// - Handling nullable dates (ReturnDate)
    /// - Special operations (marking a book as returned)
    /// - Inserting data into multiple related tables
    /// </summary>
    public partial class LoansWindow : Window
    {
        #region Fields

        /// <summary>
        /// Stores the currently selected loan for editing
        /// null if we're adding a new loan
        /// </summary>
        private Loan selectedLoan = null;

        #endregion

        #region Constructor and Initialization

        /// <summary>
        /// Constructor - initializes the window
        /// </summary>
        public LoansWindow()
        {
            InitializeComponent();

            // Load data when window opens
            LoadLoans();
            LoadBooks();
            LoadMembers();

            // Set default dates
            dpLoanDate.SelectedDate = DateTime.Today;
            dpDueDate.SelectedDate = DateTime.Today.AddDays(14); // 2 weeks from today
        }

        #endregion

        #region Data Loading Methods

        /// <summary>
        /// Loads all loans from database and displays them in the DataGrid
        /// </summary>
        private void LoadLoans()
        {
            try
            {
                // Get all loans from database
                List<Loan> loans = DatabaseHelper.GetAllLoans();

                // Bind to DataGrid
                dgLoans.ItemsSource = loans;

                // Show message if no loans found
                if (loans.Count == 0)
                {
                    MessageBox.Show(
                        "No loans found in the database.\n\n" +
                        "Add your first loan using the form on the right!",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading loans:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Loads all books into the ComboBox for selection
        /// This demonstrates populating a ComboBox with database data
        /// </summary>
        private void LoadBooks()
        {
            try
            {
                List<Book> books = DatabaseHelper.GetAllBooks();

                // Set ItemsSource for the ComboBox
                // DisplayMemberPath and SelectedValuePath are set in XAML
                cboBook.ItemsSource = books;

                if (books.Count == 0)
                {
                    MessageBox.Show(
                        "No books found in the database.\n\n" +
                        "You need to add books before creating loans.\n" +
                        "Use the 'Manage Books' window to add books.",
                        "No Books Available",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
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
        /// Loads all members into the ComboBox for selection
        /// </summary>
        private void LoadMembers()
        {
            try
            {
                List<Member> members = DatabaseHelper.GetAllMembers();

                cboMember.ItemsSource = members;

                if (members.Count == 0)
                {
                    MessageBox.Show(
                        "No members found in the database.\n\n" +
                        "You need to add members before creating loans.\n" +
                        "Use the 'Manage Members' window to add members.",
                        "No Members Available",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading members:\n\n{ex.Message}",
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
            cboBook.SelectedIndex = -1; // Clear selection (-1 means nothing selected)
            cboMember.SelectedIndex = -1;
            dpLoanDate.SelectedDate = DateTime.Today;
            dpDueDate.SelectedDate = DateTime.Today.AddDays(14);
            dpReturnDate.SelectedDate = null;
            chkReturnDate.IsChecked = false;
            dpReturnDate.IsEnabled = false;

            selectedLoan = null;

            // Update form title
            txtFormTitle.Text = "Add New Loan";

            // Clear DataGrid selection
            dgLoans.SelectedItem = null;
        }

        /// <summary>
        /// Populates the form with data from a selected loan for editing
        /// This demonstrates setting ComboBox selections and DatePicker values
        /// </summary>
        /// <param name="loan">The loan to edit</param>
        private void PopulateForm(Loan loan)
        {
            if (loan == null) return;

            // Store the selected loan
            selectedLoan = loan;

            // Set ComboBox selections by value
            // SelectedValue matches against SelectedValuePath (BookID/MemberID)
            cboBook.SelectedValue = loan.BookID;
            cboMember.SelectedValue = loan.MemberID;

            // Set DatePicker values
            // DatePicker.SelectedDate is a nullable DateTime (DateTime?)
            dpLoanDate.SelectedDate = loan.LoanDate;
            dpDueDate.SelectedDate = loan.DueDate;

            // Handle nullable ReturnDate
            if (loan.ReturnDate.HasValue)
            {
                chkReturnDate.IsChecked = true;
                dpReturnDate.IsEnabled = true;
                dpReturnDate.SelectedDate = loan.ReturnDate.Value;
            }
            else
            {
                chkReturnDate.IsChecked = false;
                dpReturnDate.IsEnabled = false;
                dpReturnDate.SelectedDate = null;
            }

            // Update form title
            txtFormTitle.Text = $"Edit Loan (ID: {loan.LoanID})";
        }

        /// <summary>
        /// Validates the form input before saving
        /// Returns a Loan object if valid, null if invalid
        /// </summary>
        /// <returns>Valid Loan object or null</returns>
        private Loan ValidateAndCreateLoan()
        {
            // Create a new loan object (or use existing for updates)
            Loan loan = selectedLoan ?? new Loan();

            // Validate Book selection
            if (cboBook.SelectedValue == null)
            {
                MessageBox.Show(
                    "Please select a book.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                cboBook.Focus();
                return null;
            }
            loan.BookID = (int)cboBook.SelectedValue;

            // Validate Member selection
            if (cboMember.SelectedValue == null)
            {
                MessageBox.Show(
                    "Please select a member.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                cboMember.Focus();
                return null;
            }
            loan.MemberID = (int)cboMember.SelectedValue;

            // Validate Loan Date
            if (!dpLoanDate.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    "Please select a loan date.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                dpLoanDate.Focus();
                return null;
            }
            loan.LoanDate = dpLoanDate.SelectedDate.Value;

            // Validate Due Date
            if (!dpDueDate.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    "Please select a due date.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                dpDueDate.Focus();
                return null;
            }
            loan.DueDate = dpDueDate.SelectedDate.Value;

            // Handle optional Return Date
            if (chkReturnDate.IsChecked == true && dpReturnDate.SelectedDate.HasValue)
            {
                loan.ReturnDate = dpReturnDate.SelectedDate.Value;
            }
            else
            {
                loan.ReturnDate = null;
            }

            // Use the Loan class's validation method
            if (!loan.IsValid())
            {
                MessageBox.Show(
                    $"Validation failed:\n\n{loan.GetValidationErrors()}",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }

            return loan;
        }

        #endregion

        #region Button Event Handlers

        /// <summary>
        /// Saves a loan (either INSERT for new or UPDATE for existing)
        /// This demonstrates working with foreign keys - we're inserting BookID and MemberID
        /// which must reference existing records in Books and Members tables
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate the form and get a Loan object
                Loan loan = ValidateAndCreateLoan();
                if (loan == null) return; // Validation failed

                bool success;
                string action;

                if (loan.LoanID == 0)
                {
                    // INSERT - adding a new loan
                    // This demonstrates inserting data that references other tables via foreign keys
                    action = "added";
                    int newId = DatabaseHelper.InsertLoan(loan);
                    success = newId > 0;

                    if (success)
                    {
                        loan.LoanID = newId; // Store the new ID
                    }
                }
                else
                {
                    // UPDATE - editing existing loan
                    action = "updated";
                    success = DatabaseHelper.UpdateLoan(loan);
                }

                if (success)
                {
                    // Get the book and member names for display
                    Book book = DatabaseHelper.GetBookById(loan.BookID);
                    Member member = DatabaseHelper.GetMemberById(loan.MemberID);

                    string bookTitle = book != null ? book.Title : $"Book ID {loan.BookID}";
                    string memberName = member != null ? member.FullName : $"Member ID {loan.MemberID}";

                    MessageBox.Show(
                        $"Loan {action} successfully!\n\n" +
                        $"Book: {bookTitle}\n" +
                        $"Member: {memberName}\n" +
                        $"Due Date: {loan.DueDate:dd/MM/yyyy}",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Refresh the list to show changes
                    LoadLoans();

                    // Clear the form for next entry
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(
                        $"Failed to {action.TrimEnd('d')} loan.\n\n" +
                        "Check the debug output for details.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error saving loan:\n\n{ex.Message}",
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
        /// Refreshes the loans list from the database
        /// </summary>
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadLoans();
            LoadBooks(); // Also refresh books and members in case they were added
            LoadMembers();
        }

        /// <summary>
        /// Marks the selected loan as returned
        /// This demonstrates a specialized UPDATE operation
        /// Sets ReturnDate to today's date
        /// </summary>
        private void BtnReturnBook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if a loan is selected
                if (dgLoans.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Please select a loan to mark as returned.",
                        "No Selection",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                Loan loanToReturn = (Loan)dgLoans.SelectedItem;

                // Check if already returned
                if (loanToReturn.IsReturned)
                {
                    MessageBox.Show(
                        $"This book was already returned on {loanToReturn.ReturnDate:dd/MM/yyyy}.",
                        "Already Returned",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                // Ask for confirmation
                MessageBoxResult result = MessageBox.Show(
                    $"Mark this loan as returned today?\n\n" +
                    $"Loan ID: {loanToReturn.LoanID}\n" +
                    $"Book ID: {loanToReturn.BookID}\n" +
                    $"Member ID: {loanToReturn.MemberID}\n" +
                    $"Due Date: {loanToReturn.DueDate:dd/MM/yyyy}",
                    "Confirm Return",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Call the specialized ReturnBook method in DatabaseHelper
                    bool success = DatabaseHelper.ReturnBook(loanToReturn.LoanID);

                    if (success)
                    {
                        MessageBox.Show(
                            $"Book returned successfully!\n\n" +
                            $"Return Date: {DateTime.Today:dd/MM/yyyy}",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Refresh the list
                        LoadLoans();

                        // Clear selection
                        dgLoans.SelectedItem = null;
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to mark book as returned.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error returning book:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Deletes the selected loan
        /// Demonstrates DELETE operation with user confirmation
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if a loan is selected
                if (dgLoans.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Please select a loan to delete.",
                        "No Selection",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                Loan loanToDelete = (Loan)dgLoans.SelectedItem;

                // Ask for confirmation before deleting
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete this loan record?\n\n" +
                    $"Loan ID: {loanToDelete.LoanID}\n" +
                    $"Book ID: {loanToDelete.BookID}\n" +
                    $"Member ID: {loanToDelete.MemberID}\n" +
                    $"Loan Date: {loanToDelete.LoanDate:dd/MM/yyyy}\n\n" +
                    "This action cannot be undone!",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    bool success = DatabaseHelper.DeleteLoan(loanToDelete.LoanID);

                    if (success)
                    {
                        MessageBox.Show(
                            "Loan record deleted successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Refresh the list
                        LoadLoans();

                        // Clear the form if we were editing this loan
                        if (selectedLoan?.LoanID == loanToDelete.LoanID)
                        {
                            ClearForm();
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to delete loan record.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error deleting loan:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region DataGrid and Control Event Handlers

        /// <summary>
        /// Handles when a user selects a different row in the DataGrid
        /// Populates the form with the selected loan's data for editing
        /// </summary>
        private void DgLoans_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if a row is selected
            if (dgLoans.SelectedItem != null)
            {
                // Get the selected loan
                Loan loan = (Loan)dgLoans.SelectedItem;

                // Populate the form for editing
                PopulateForm(loan);
            }
        }

        /// <summary>
        /// Handles when the "Book has been returned" checkbox is checked
        /// Enables the ReturnDate DatePicker and sets it to today
        /// </summary>
        private void ChkReturnDate_Checked(object sender, RoutedEventArgs e)
        {
            dpReturnDate.IsEnabled = true;

            // Set to today if not already set
            if (!dpReturnDate.SelectedDate.HasValue)
            {
                dpReturnDate.SelectedDate = DateTime.Today;
            }
        }

        /// <summary>
        /// Handles when the "Book has been returned" checkbox is unchecked
        /// Disables the ReturnDate DatePicker and clears its value
        /// </summary>
        private void ChkReturnDate_Unchecked(object sender, RoutedEventArgs e)
        {
            dpReturnDate.IsEnabled = false;
            dpReturnDate.SelectedDate = null;
        }

        #endregion
    }
}
