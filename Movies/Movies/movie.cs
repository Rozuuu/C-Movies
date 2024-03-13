using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Movies
{
    class movie
    {
        public int MovieID;
        public string MovieTitle;
        public string MovieDescription;
        public string Category;
        public string Origin;
        public DateTime DateLaunched;

        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ict-tsg\Documents\Visual Studio 2013\Projects\Movies\Movies\Database1.mdf;Integrated Security=True";

        public movie()
        {

        }


        public movie(int movieID, string movieTitle, string movieDescription, string category, string origin, DateTime datelaunched)
        {
            this.MovieID = movieID;
            this.MovieTitle = movieTitle;
            this.MovieDescription = movieDescription;
            this.Category = category;
            this.Origin = origin;
            this.DateLaunched = datelaunched;
        }


        public int InsertMovie()
        {
            string query = "INSERT INTO Movie (Movie_ID, Title, Description, Category, Origin, DateLaunched) VALUES (@MovieID, @MovieTitle, @MovieDescription, @Category, @Origin, @DateLaunched)";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MovieID", MovieID);
                        command.Parameters.AddWithValue("@MovieTitle", MovieTitle);
                        command.Parameters.AddWithValue("@MovieDescription", MovieDescription);
                        command.Parameters.AddWithValue("@Category", Category);
                        command.Parameters.AddWithValue("@Origin", Origin);
                        command.Parameters.AddWithValue("@DateLaunched", DateLaunched);

                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting movie: " + ex.Message);
                throw; // Rethrow the exception to propagate it to the caller
            }
        }

    }

}
