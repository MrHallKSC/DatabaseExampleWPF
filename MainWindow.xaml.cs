using DatabaseExampleWPF.Database;
using System.Windows;

namespace DatabaseExampleWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// This is the "code-behind" file - it contains the C# code that handles events
    /// and logic for the MainWindow.xaml user interface
    /// 
    /// This demonstrates for AQA 7517 NEA:
    /// - Event handlers (responding to button clicks)
    /// - Opening new windows
    /// - Calling database methods
    /// - User feedback with MessageBox
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor - called when the window is created
        /// InitializeComponent() is automatically generated and loads the XAML
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Check if database exists and inform user
            CheckDatabaseStatus();
        }

        /// <summary>
        /// Checks if the database exists and updates UI accordingly
        /// This runs when the application starts
        /// </summary>
        private void CheckDatabaseStatus()
        {
            try
            {
                // Check if all required tables exist
                bool allTablesExist = DatabaseHelper.TableExists("Books") &&
                                     DatabaseHelper.TableExists("Authors") &&
                                     DatabaseHelper.TableExists("BookAuthors") &&
                                     DatabaseHelper.TableExists("Members") &&
                                     DatabaseHelper.TableExists("Loans");

                if (allTablesExist)
                {
                    // Change button text to indicate database is ready
                    btnCreateDatabase.Content = "✓ Database Ready (Click to Recreate)";
                    btnCreateDatabase.Background = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(76, 175, 80)); // Green
                }
                else
                {
                    // Database doesn't exist - keep default appearance
                    btnCreateDatabase.Content = "Create Database & Tables";
                }
            }
            catch (Exception ex)
            {
                // If there's an error checking database status, log it
                System.Diagnostics.Debug.WriteLine($"Error checking database status: {ex.Message}");
            }
        }

        #region Button Event Handlers

        /// <summary>
        /// Event handler for Create Database button
        /// Creates all database tables when clicked
        /// 
        /// Event handlers are methods that respond to user actions (like clicking a button)
        /// The signature must match: void MethodName(object sender, RoutedEventArgs e)
        /// - sender: The control that raised the event (the button in this case)
        /// - e: Event arguments with additional information about the event
        /// </summary>
        private void BtnCreateDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Call the DatabaseHelper method to create tables
                bool success = DatabaseHelper.CreateTables();

                if (success)
                {
                    // MessageBox: Shows a pop-up dialog to the user
                    // Parameters: message text, title bar text, button options, icon
                    MessageBox.Show(
                        "Database and tables created successfully!\n\n" +
                        "You can now use the buttons below to manage data.\n\n" +
                        "Tip: You can view the database using DB Browser for SQLite.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Update the UI to show database is ready
                    CheckDatabaseStatus();
                }
                else
                {
                    MessageBox.Show(
                        "Failed to create database tables.\n\n" +
                        "Check the debug output for error details.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Catch any unexpected errors and show them to the user
                MessageBox.Show(
                    $"An error occurred while creating the database:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Opens the Books management window
        /// Demonstrates how to open a new window from the main window
        /// </summary>
        private void BtnManageBooks_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a new instance of the BooksWindow
                BooksWindow booksWindow = new BooksWindow();

                // Set the owner so the new window appears on top of this one
                booksWindow.Owner = this;

                // Show the window (non-modal - user can still interact with main window)
                // Alternative: ShowDialog() would be modal - blocks interaction with main window
                booksWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Books window:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Opens the Authors management window
        /// </summary>
        private void BtnManageAuthors_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AuthorsWindow authorsWindow = new AuthorsWindow();
                authorsWindow.Owner = this;
                authorsWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Authors window:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Opens the Members management window
        /// </summary>
        private void BtnManageMembers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MembersWindow membersWindow = new MembersWindow();
                membersWindow.Owner = this;
                membersWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Members window:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Opens the Loans management window
        /// </summary>
        private void BtnManageLoans_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoansWindow loansWindow = new LoansWindow();
                loansWindow.Owner = this;
                loansWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Loans window:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Opens the View Loans with Details window
        /// This window demonstrates JOIN queries and search functionality
        /// </summary>
        private void BtnViewLoansWithDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SearchLoansWindow searchWindow = new SearchLoansWindow();
                searchWindow.Owner = this;
                searchWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error opening Search Loans window:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Populates the database with sample data for testing
        /// </summary>
        private void BtnPopulateSampleData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if database exists first
                if (!DatabaseHelper.DatabaseExists())
                {
                    MessageBox.Show(
                        "Please create the database first by clicking 'Create Database & Tables'.",
                        "Database Not Found",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Ask for confirmation
                MessageBoxResult result = MessageBox.Show(
                    "This will add sample data to the database:\n\n" +
                    "• 10 Books\n" +
                    "• 10 Authors\n" +
                    "• Book-Author relationships\n" +
                    "• 10 Members\n" +
                    "• 10 Loans (some active, some returned, some overdue)\n\n" +
                    "Continue?",
                    "Populate Sample Data",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Call the method to populate sample data
                    bool success = DatabaseHelper.PopulateSampleData();

                    if (success)
                    {
                        MessageBox.Show(
                            "Sample data added successfully!\n\n" +
                            "You can now explore the application with test data.\n\n" +
                            "Tip: Use the different management windows to view and modify the data.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to add sample data.\n\n" +
                            "Check the debug output for error details.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while adding sample data:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        #endregion
    }
}