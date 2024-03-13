using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Movies
{
    public partial class Form1 : Form
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ict-tsg\Documents\Visual Studio 2013\Projects\Movies\Movies\Database1.mdf;Integrated Security=True";
        public Form1()
        {
            InitializeComponent();
            InitializeListView(); // Call method to initialize the ListView
            PopulateListView(); // Populate ListView with data from the database

            try
            {
                // Add predefined categories to comboBox1
                comboBox1.Items.Clear();
                comboBox1.Items.Add("Horror");
                comboBox1.Items.Add("Adventure");
                comboBox1.Items.Add("Comedy");
                comboBox1.Items.Add("Action");
                comboBox1.Items.Add("Romance");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding predefined categories: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void SaveMovie_Click(object sender, EventArgs e)
        {
            int movieID;
            if (!int.TryParse(MovieID.Text, out movieID))
            {
                MessageBox.Show("Please enter a valid movie ID.");
                return;
            }

            string movieTitle = MovieTitle.Text;
            string movieDescription = MovieDescription.Text;

            // Check if an item is selected in comboBox1
            string category = null;
            if (comboBox1.SelectedItem != null)
            {
                category = comboBox1.SelectedItem.ToString();
            }

            string origin = Origin.Text;
            //Validate the Date
            DateTime dateLaunched = dateTimePicker1.Value;

            // Validate dateLaunched (optional)
            if (dateLaunched > DateTime.Now)
            {
                MessageBox.Show("Please enter a valid launch date.");
                return;
            }

            // Create a new movie object
            movie newMovie = new movie(movieID, movieTitle, movieDescription, category, origin, dateLaunched);

            // Call InsertMovie method to save the movie to the database
            int rowsAffected = newMovie.InsertMovie();

            if (rowsAffected > 0)
            {
                MessageBox.Show("Movie saved successfully.");
                // Refresh ListView to display the latest data
                PopulateListView();
                // You can clear the input fields here if needed
                MovieID.Text = "";
                MovieTitle.Text = "";
                MovieDescription.Text = "";
                comboBox1.SelectedIndex = -1;
                Origin.Text = "";
                dateTimePicker1.Value = DateTime.Now;
            }
            else
            {
                MessageBox.Show("Failed to save movie.");
            }
        }

        private void MovieID_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                // Retrieve the selected item
                ListViewItem selectedItem = listView1.SelectedItems[0];

                // Extract data from the selected item and display in text boxes
                MovieID.Text = selectedItem.SubItems[0].Text;
                MovieTitle.Text = selectedItem.SubItems[1].Text;
                MovieDescription.Text = selectedItem.SubItems[2].Text;
                comboBox1.SelectedItem = selectedItem.SubItems[3].Text; // Assuming category is at index 3
                Origin.Text = selectedItem.SubItems[4].Text;

                // Parse birthdate from the selected item
                DateTime birthdate;
                if (DateTime.TryParseExact(selectedItem.SubItems[5].Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthdate))
                {
                    dateTimePicker1.Value = birthdate;
                }
                else
                {
                    // Handle invalid birthdate string
                    // For example, you could set a default value or display an error message
                    dateTimePicker1.Value = DateTime.Today; // Set a default value
                    // MessageBox.Show("Invalid birthdate format!"); // Display an error message
                }

                // You can continue displaying other information in the same manner
            }
        }
        private void InitializeListView()
        {
            // Define columns for the ListView
            listView1.View = View.Details; // Set the view to show details (columns)

            // Add columns with text at the top
            listView1.Columns.Add("MovieID", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("MovieTitle", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("MovieDescription", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Category", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Origin", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("DateLaunched", 100, HorizontalAlignment.Left);

            // Set the header text
            listView1.HeaderStyle = ColumnHeaderStyle.Nonclickable; // Optional: To make columns non-clickable
            listView1.GridLines = true; // Optional: To show grid lines
        }
        private void PopulateListView()
        {
            string query = "SELECT * FROM Movie";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        // Clear existing items from ListView only
                        listView1.Items.Clear();

                        while (reader.Read())
                        {
                            ListViewItem item = new ListViewItem(reader["Movie_ID"].ToString());
                            item.SubItems.Add(reader["Title"].ToString());
                            item.SubItems.Add(reader["Description"].ToString());
                            item.SubItems.Add(reader["Category"].ToString());
                            item.SubItems.Add(reader["Origin"].ToString());
                            item.SubItems.Add(reader["DateLaunched"].ToString());

                            listView1.Items.Add(item);

                            // Add category to comboBox1 only if it's not already present
                            string category = reader["Category"].ToString();
                            if (!comboBox1.Items.Contains(category))
                            {
                                comboBox1.Items.Add(category);
                            }
                        }

                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();

            // If the search text is empty, repopulate the ListView with all items
            if (string.IsNullOrEmpty(searchText))
            {
                PopulateListView();
                return;
            }

            // Create a list to hold items that should be removed
            List<ListViewItem> itemsToRemove = new List<ListViewItem>();

            foreach (ListViewItem item in listView1.Items)
            {
                bool itemMatchesSearch = false;

                foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                {
                    if (subItem.Text.ToLower().Contains(searchText))
                    {
                        itemMatchesSearch = true;
                        break;
                    }
                }

                if (!itemMatchesSearch)
                {
                    // If the item doesn't match the search criteria, add it to the removal list
                    itemsToRemove.Add(item);
                }
            }

            // Remove items that do not match the search criteria
            foreach (var itemToRemove in itemsToRemove)
            {
                listView1.Items.Remove(itemToRemove);
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();

            foreach (ListViewItem item in listView1.Items)
            {
                foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                {
                    int index = subItem.Text.ToLower().IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
                    if (index >= 0)
                    {
                        subItem.ForeColor = Color.Red;
                        subItem.Font = new Font(subItem.Font, FontStyle.Bold);
                    }
                }
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a movie to delete.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this movie?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Retrieve the selected item
                ListViewItem selectedItem = listView1.SelectedItems[0];

                // Extract movie ID from the selected item
                int movieID = int.Parse(selectedItem.SubItems[0].Text);

                // Delete the movie from the database
                if (DeleteMovie(movieID))
                {
                    MessageBox.Show("Movie deleted successfully.");

                    // Refresh ListView to reflect the changes
                    PopulateListView();
                }
                else
                {
                    MessageBox.Show("Failed to delete movie.");
                }
            }
        }

        private bool DeleteMovie(int movieID)
        {
            string query = "DELETE FROM Movie WHERE Movie_ID = @MovieID";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MovieID", movieID);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting movie: " + ex.Message);
                return false;
            }
        }

        private void Update_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a movie to update.");
                return;
            }

            int movieID;
            if (!int.TryParse(MovieID.Text, out movieID))
            {
                MessageBox.Show("Please enter a valid movie ID.");
                return;
            }

            string movieTitle = MovieTitle.Text;
            string movieDescription = MovieDescription.Text;

            // Check if an item is selected in comboBox1
            string category = null;
            if (comboBox1.SelectedItem != null)
            {
                category = comboBox1.SelectedItem.ToString();
            }

            string origin = Origin.Text;
            //Validate the Date
            DateTime dateLaunched = dateTimePicker1.Value;

            // Validate dateLaunched (optional)
            if (dateLaunched > DateTime.Now)
            {
                MessageBox.Show("Please enter a valid launch date.");
                return;
            }

            if (MessageBox.Show("Are you sure you want to update this movie?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Update the movie in the database
                if (UpdateMovie(movieID, movieTitle, movieDescription, category, origin, dateLaunched))
                {
                    MessageBox.Show("Movie updated successfully.");

                    // Refresh ListView to reflect the changes
                    PopulateListView();
                }
                else
                {
                    MessageBox.Show("Failed to update movie.");
                }
            }
        }

        private bool UpdateMovie(int movieID, string movieTitle, string movieDescription, string category, string origin, DateTime dateLaunched)
        {
            string query = "UPDATE Movie SET Title = @MovieTitle, Description = @MovieDescription, Category = @Category, Origin = @Origin, DateLaunched = @DateLaunched WHERE Movie_ID = @MovieID";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MovieID", movieID);
                        command.Parameters.AddWithValue("@MovieTitle", movieTitle);
                        command.Parameters.AddWithValue("@MovieDescription", movieDescription);
                        command.Parameters.AddWithValue("@Category", category);
                        command.Parameters.AddWithValue("@Origin", origin);
                        command.Parameters.AddWithValue("@DateLaunched", dateLaunched);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating movie: " + ex.Message);
                return false;
            }
        }
        
    }
}
