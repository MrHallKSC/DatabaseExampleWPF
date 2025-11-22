using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DatabaseExampleWPF.Database;
using DatabaseExampleWPF.Models;

namespace DatabaseExampleWPF
{
    /// <summary>
    /// Interaction logic for SearchLoansWindow.xaml
    /// 
    /// This window demonstrates:
    /// - Complex JOIN queries across multiple tables (Loans, Books, Members)
    /// - Using the LoanWithDetails view model class
    /// - Search/filter functionality with LIKE operator
    /// - Displaying computed properties (Status, IsOverdue, etc.)
    /// - Conditional formatting in DataGrid (colored rows based on status)
    /// - Read-only data display from joined tables
    /// - Client-side filtering with LINQ
    /// 
    /// This is an important example for NEA projects as it shows how to
    /// effectively display and search data from multiple related tables.
    /// </summary>
    public partial class SearchLoansWindow : Window
    {
        #region Fields

        /// <summary>
        /// Stores all loans with details for client-side filtering
        /// Loaded once from the database, then filtered in memory
        /// </summary>
        private List<LoanWithDetails> allLoans;

        #endregion

        #region Constructor and Initialization

        /// <summary>
        /// Constructor - initializes the window
        /// </summary>
        public SearchLoansWindow()
        {
            InitializeComponent();

            // Load all loans when window opens
            LoadAllLoans();
        }

        #endregion

        #region Data Loading Methods

        /// <summary>
        /// Loads all loans with details from the database using a JOIN query
        /// This demonstrates the most important concept in this window:
        /// Using JOIN queries to combine data from multiple tables
        /// 
        /// The SQL query executed by GetAllLoansWithDetails() is:
        /// SELECT Loans.*, Books.Title, Books.ISBN, Members.FirstName, Members.LastName, etc.
        /// FROM Loans
        /// INNER JOIN Books ON Loans.BookID = Books.BookID
        /// INNER JOIN Members ON Loans.MemberID = Members.MemberID
        /// </summary>
        private void LoadAllLoans()
        {
            try
            {
                // Call DatabaseHelper method that performs the JOIN query
                // This returns a List<LoanWithDetails> with data from all three tables
                allLoans = DatabaseHelper.GetAllLoansWithDetails();

                // Display in the DataGrid
                dgLoansWithDetails.ItemsSource = allLoans;

                // Update the window title to show count
                this.Title = $"View and Search Loans ({allLoans.Count} total)";

                // Show message if no loans found
                if (allLoans.Count == 0)
                {
                    MessageBox.Show(
                        "No loans found in the database.\n\n" +
                        "Use the 'Manage Loans' window to add loans.",
                        "No Loans",
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

        #endregion

        #region Search and Filter Methods

        /// <summary>
        /// Performs a search using the database LIKE operator
        /// This demonstrates server-side searching (search is done in SQL)
        /// 
        /// The DatabaseHelper.SearchLoansWithDetails method uses:
        /// WHERE BookTitle LIKE '%searchTerm%' OR MemberFirstName LIKE '%searchTerm%' ...
        /// </summary>
        private void PerformSearch()
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    // If search box is empty, show all loans
                    LoadAllLoans();
                    return;
                }

                // Call the search method with the search term
                // This executes a SQL query with LIKE operator
                List<LoanWithDetails> results = DatabaseHelper.SearchLoansWithDetails(searchTerm);

                // Display results
                dgLoansWithDetails.ItemsSource = results;

                // Update title with result count
                this.Title = $"View and Search Loans ({results.Count} matching '{searchTerm}')";

                // Show message if no results found
                if (results.Count == 0)
                {
                    MessageBox.Show(
                        $"No loans found matching '{searchTerm}'.\n\n" +
                        "Search looks for matches in:\n" +
                        "- Book titles\n" +
                        "- Member first names\n" +
                        "- Member last names",
                        "No Results",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error searching loans:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Filters loans to show only active (unreturned) loans
        /// This demonstrates client-side filtering using LINQ
        /// 
        /// LINQ (Language Integrated Query) allows querying collections in C#
        /// It's very useful for filtering data that's already in memory
        /// </summary>
        private void ShowActiveLoans()
        {
            try
            {
                if (allLoans == null)
                {
                    LoadAllLoans();
                    return;
                }

                // Use LINQ to filter the list
                // Where() filters items based on a condition
                // lambda expression: loan => !loan.IsReturned means "where loan is not returned"
                var activeLoans = allLoans.Where(loan => !loan.IsReturned).ToList();

                dgLoansWithDetails.ItemsSource = activeLoans;
                this.Title = $"View and Search Loans ({activeLoans.Count} active)";

                if (activeLoans.Count == 0)
                {
                    MessageBox.Show(
                        "No active loans found.\n\n" +
                        "All books have been returned!",
                        "No Active Loans",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error filtering loans:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Filters loans to show only overdue loans
        /// Uses LINQ with the IsOverdue computed property
        /// </summary>
        private void ShowOverdueLoans()
        {
            try
            {
                if (allLoans == null)
                {
                    LoadAllLoans();
                    return;
                }

                // Filter for overdue loans
                var overdueLoans = allLoans.Where(loan => loan.IsOverdue).ToList();

                dgLoansWithDetails.ItemsSource = overdueLoans;
                this.Title = $"View and Search Loans ({overdueLoans.Count} overdue)";

                if (overdueLoans.Count == 0)
                {
                    MessageBox.Show(
                        "No overdue loans found.\n\n" +
                        "Great! All books are either returned or on time.",
                        "No Overdue Loans",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error filtering loans:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Filters loans to show only returned loans
        /// </summary>
        private void ShowReturnedLoans()
        {
            try
            {
                if (allLoans == null)
                {
                    LoadAllLoans();
                    return;
                }

                // Filter for returned loans
                var returnedLoans = allLoans.Where(loan => loan.IsReturned).ToList();

                dgLoansWithDetails.ItemsSource = returnedLoans;
                this.Title = $"View and Search Loans ({returnedLoans.Count} returned)";

                if (returnedLoans.Count == 0)
                {
                    MessageBox.Show(
                        "No returned loans found.\n\n" +
                        "No books have been returned yet.",
                        "No Returned Loans",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error filtering loans:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region Details Display Methods

        /// <summary>
        /// Displays detailed information about the selected loan
        /// This demonstrates accessing properties from the joined data
        /// </summary>
        /// <param name="loan">The loan to display details for</param>
        private void DisplayLoanDetails(LoanWithDetails loan)
        {
            if (loan == null)
            {
                // Clear all details if no loan selected
                txtLoanDetails.Text = "Select a loan to view details";
                txtLoanDetails.FontStyle = FontStyles.Italic;
                txtLoanDetails.Foreground = System.Windows.Media.Brushes.Gray;

                txtBookDetails.Text = "Select a loan to view book details";
                txtBookDetails.FontStyle = FontStyles.Italic;
                txtBookDetails.Foreground = System.Windows.Media.Brushes.Gray;

                txtMemberDetails.Text = "Select a loan to view member details";
                txtMemberDetails.FontStyle = FontStyles.Italic;
                txtMemberDetails.Foreground = System.Windows.Media.Brushes.Gray;

                return;
            }

            // Display loan details
            txtLoanDetails.FontStyle = FontStyles.Normal;
            txtLoanDetails.Foreground = System.Windows.Media.Brushes.Black;
            txtLoanDetails.Text = $"Loan ID: {loan.LoanID}\n" +
                                 $"Loan Date: {loan.LoanDate:dd/MM/yyyy}\n" +
                                 $"Due Date: {loan.DueDate:dd/MM/yyyy}\n" +
                                 $"Return Date: {(loan.ReturnDate.HasValue ? loan.ReturnDate.Value.ToString("dd/MM/yyyy") : "Not returned")}\n" +
                                 $"\n" +
                                 $"Status: {loan.StatusMessage}";

            // Display book details (from Books table via JOIN)
            txtBookDetails.FontStyle = FontStyles.Normal;
            txtBookDetails.Foreground = System.Windows.Media.Brushes.Black;
            txtBookDetails.Text = $"Book ID: {loan.BookID}\n" +
                                 $"Title: {loan.BookTitle}\n" +
                                 $"ISBN: {loan.BookISBN}";

            // Display member details (from Members table via JOIN)
            txtMemberDetails.FontStyle = FontStyles.Normal;
            txtMemberDetails.Foreground = System.Windows.Media.Brushes.Black;
            txtMemberDetails.Text = $"Member ID: {loan.MemberID}\n" +
                                   $"Name: {loan.MemberFullName}\n" +
                                   $"Email: {loan.MemberEmail}\n" +
                                   $"Type: {loan.MemberType}";
        }

        #endregion

        #region Button Event Handlers

        /// <summary>
        /// Performs a search when the Search button is clicked
        /// Also can be triggered by pressing Enter in the search box
        /// </summary>
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        /// <summary>
        /// Clears the search and shows all loans
        /// </summary>
        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadAllLoans();
        }

        /// <summary>
        /// Shows all loans (removes any filters)
        /// </summary>
        private void BtnShowAll_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadAllLoans();
        }

        /// <summary>
        /// Shows only active (unreturned) loans
        /// </summary>
        private void BtnShowActive_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            ShowActiveLoans();
        }

        /// <summary>
        /// Shows only overdue loans
        /// </summary>
        private void BtnShowOverdue_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            ShowOverdueLoans();
        }

        /// <summary>
        /// Shows only returned loans
        /// </summary>
        private void BtnShowReturned_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            ShowReturnedLoans();
        }

        #endregion

        #region DataGrid Event Handlers

        /// <summary>
        /// Handles when a user selects a different row in the DataGrid
        /// Displays detailed information about the selected loan
        /// </summary>
        private void DgLoansWithDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgLoansWithDetails.SelectedItem != null)
            {
                LoanWithDetails loan = (LoanWithDetails)dgLoansWithDetails.SelectedItem;
                DisplayLoanDetails(loan);
            }
            else
            {
                DisplayLoanDetails(null);
            }
        }

        #endregion
    }
}