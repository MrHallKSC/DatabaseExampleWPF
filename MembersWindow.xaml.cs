using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DatabaseExampleWPF.Database;
using DatabaseExampleWPF.Models;

namespace DatabaseExampleWPF
{
    /// <summary>
    /// Interaction logic for MembersWindow.xaml
    /// 
    /// This window demonstrates:
    /// - CRUD operations for the Members table
    /// - Email validation using Regular Expressions
    /// - ComboBox for selecting from fixed options (Member Type)
    /// - Querying one-to-many relationship (Member has many Loans)
    /// - Working with computed properties (Status from Loan class)
    /// </summary>
    public partial class MembersWindow : Window
    {
        #region Fields

        /// <summary>
        /// Stores the currently selected member for editing
        /// null if we're adding a new member
        /// </summary>
        private Member selectedMember = null;

        #endregion

        #region Constructor and Initialization

        /// <summary>
        /// Constructor - initializes the window
        /// </summary>
        public MembersWindow()
        {
            InitializeComponent();

            // Load data when window opens
            LoadMembers();

            // Set default member type to "Student"
            cboMemberType.SelectedIndex = 0; // Selects first item (Student)

            // Hide loans display until a member is selected
            UpdateLoansDisplayVisibility(false);
        }

        #endregion

        #region Data Loading Methods

        /// <summary>
        /// Loads all members from database and displays them in the DataGrid
        /// </summary>
        private void LoadMembers()
        {
            try
            {
                // Get all members from database
                List<Member> members = DatabaseHelper.GetAllMembers();

                // Bind to DataGrid
                dgMembers.ItemsSource = members;

                // Show message if no members found
                if (members.Count == 0)
                {
                    MessageBox.Show(
                        "No members found in the database.\n\n" +
                        "Add your first member using the form on the right!",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
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

        /// <summary>
        /// Loads loans for the selected member
        /// Demonstrates querying the one-to-many relationship
        /// One Member can have many Loans
        /// </summary>
        private void LoadMemberLoans()
        {
            try
            {
                if (selectedMember == null || selectedMember.MemberID <= 0)
                {
                    lstMemberLoans.ItemsSource = null;
                    return;
                }

                // Get loans for this member from the Loans table
                // This uses: SELECT * FROM Loans WHERE MemberID = selectedMember.MemberID
                List<Loan> loans = DatabaseHelper.GetLoansForMember(selectedMember.MemberID);

                // Display in the ListBox
                lstMemberLoans.ItemsSource = loans;

                // Optional: Debug message if member has no loans
                if (loans.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Member {selectedMember.FullName} has no loans.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading loans for member:\n\n{ex.Message}",
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
            txtEmail.Text = string.Empty;
            cboMemberType.SelectedIndex = 0; // Reset to "Student"

            selectedMember = null;

            // Update form title
            txtFormTitle.Text = "Add New Member";

            // Clear DataGrid selection
            dgMembers.SelectedItem = null;

            // Hide loans display
            UpdateLoansDisplayVisibility(false);
        }

        /// <summary>
        /// Populates the form with data from a selected member for editing
        /// </summary>
        /// <param name="member">The member to edit</param>
        private void PopulateForm(Member member)
        {
            if (member == null) return;

            // Store the selected member
            selectedMember = member;

            // Populate the form fields
            txtFirstName.Text = member.FirstName;
            txtLastName.Text = member.LastName;
            txtEmail.Text = member.Email;

            // Set the ComboBox selection based on MemberType
            // This demonstrates selecting a ComboBox item by its content
            switch (member.MemberType)
            {
                case "Student":
                    cboMemberType.SelectedIndex = 0;
                    break;
                case "Teacher":
                    cboMemberType.SelectedIndex = 1;
                    break;
                case "Staff":
                    cboMemberType.SelectedIndex = 2;
                    break;
                default:
                    cboMemberType.SelectedIndex = 0; // Default to Student
                    break;
            }

            // Update form title
            txtFormTitle.Text = $"Edit Member (ID: {member.MemberID})";

            // Show loans for this member
            UpdateLoansDisplayVisibility(true);
            LoadMemberLoans();
        }

        /// <summary>
        /// Shows or hides the loans display section
        /// </summary>
        /// <param name="show">True to show, false to hide</param>
        private void UpdateLoansDisplayVisibility(bool show)
        {
            lstMemberLoans.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            txtNoMemberSelected.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Validates the form input before saving
        /// Returns a Member object if valid, null if invalid
        /// This demonstrates email validation using the Member class
        /// </summary>
        /// <returns>Valid Member object or null</returns>
        private Member ValidateAndCreateMember()
        {
            // Create a new member object (or use existing for updates)
            Member member = selectedMember ?? new Member();

            // Validate First Name
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show(
                    "Please enter the member's first name.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtFirstName.Focus();
                return null;
            }
            member.FirstName = txtFirstName.Text.Trim();

            // Validate Last Name
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show(
                    "Please enter the member's last name.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtLastName.Focus();
                return null;
            }
            member.LastName = txtLastName.Text.Trim();

            // Validate Email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show(
                    "Please enter the member's email address.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                txtEmail.Focus();
                return null;
            }
            member.Email = txtEmail.Text.Trim();

            // Validate Member Type (ComboBox selection)
            if (cboMemberType.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select a member type.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                cboMemberType.Focus();
                return null;
            }

            // Get the Content of the selected ComboBoxItem
            // This demonstrates extracting the value from a ComboBox
            ComboBoxItem selectedItem = (ComboBoxItem)cboMemberType.SelectedItem;
            member.MemberType = selectedItem.Content.ToString();

            // Use the Member class's validation method
            // This will validate email format using Regular Expressions
            if (!member.IsValid())
            {
                MessageBox.Show(
                    $"Validation failed:\n\n{member.GetValidationErrors()}",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return null;
            }

            return member;
        }

        #endregion

        #region Button Event Handlers

        /// <summary>
        /// Saves a member (either INSERT for new or UPDATE for existing)
        /// Demonstrates CREATE and UPDATE operations
        /// </summary>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate the form and get a Member object
                Member member = ValidateAndCreateMember();
                if (member == null) return; // Validation failed

                bool success;
                string action;

                if (member.MemberID == 0)
                {
                    // INSERT - adding a new member
                    action = "added";
                    int newId = DatabaseHelper.InsertMember(member);
                    success = newId > 0;

                    if (success)
                    {
                        member.MemberID = newId; // Store the new ID
                    }
                }
                else
                {
                    // UPDATE - editing existing member
                    action = "updated";
                    success = DatabaseHelper.UpdateMember(member);
                }

                if (success)
                {
                    MessageBox.Show(
                        $"Member '{member.FullName}' {action} successfully!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Refresh the list to show changes
                    LoadMembers();

                    // Clear the form for next entry
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(
                        $"Failed to {action.TrimEnd('d')} member.\n\n" +
                        "Check the debug output for details.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error saving member:\n\n{ex.Message}",
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
        /// Refreshes the members list from the database
        /// </summary>
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadMembers();
        }

        /// <summary>
        /// Deletes the selected member
        /// Demonstrates DELETE operation with user confirmation
        /// </summary>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if a member is selected
                if (dgMembers.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Please select a member to delete.",
                        "No Selection",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                Member memberToDelete = (Member)dgMembers.SelectedItem;

                // Check if member has any loans
                List<Loan> memberLoans = DatabaseHelper.GetLoansForMember(memberToDelete.MemberID);

                string warningMessage = $"Are you sure you want to delete member:\n\n" +
                                      $"'{memberToDelete.FullName}' ({memberToDelete.Email})?\n\n";

                if (memberLoans.Count > 0)
                {
                    warningMessage += $"Warning: This member has {memberLoans.Count} loan record(s).\n" +
                                    "Deleting the member may affect loan history.\n\n";
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
                    bool success = DatabaseHelper.DeleteMember(memberToDelete.MemberID);

                    if (success)
                    {
                        MessageBox.Show(
                            $"Member '{memberToDelete.FullName}' deleted successfully!",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // Refresh the list
                        LoadMembers();

                        // Clear the form if we were editing this member
                        if (selectedMember?.MemberID == memberToDelete.MemberID)
                        {
                            ClearForm();
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Failed to delete member.\n\n" +
                            "The member may have active loans or other dependencies.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error deleting member:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        #endregion

        #region DataGrid Event Handlers

        /// <summary>
        /// Handles when a user selects a different row in the DataGrid
        /// Populates the form with the selected member's data for editing
        /// </summary>
        private void DgMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if a row is selected
            if (dgMembers.SelectedItem != null)
            {
                // Get the selected member
                Member member = (Member)dgMembers.SelectedItem;

                // Populate the form for editing
                PopulateForm(member);
            }
        }

        #endregion
    }
}