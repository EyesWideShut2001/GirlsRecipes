using Humanizer;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace RecepiesByGirls.Data
{
    public class RecipeDBController
    {
        static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("DataSource=database.db;Version=3;New=True;Compress=True;");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {

            }
            return sqlite_conn;
        }

        private static bool CheckIfTableExists(string tableName)
        {
            SQLiteConnection conn = CreateConnection();
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT count(*) FROM sqlite_master WHERE type='table' AND name='{tableName}';";               
            object result = sqlite_cmd.ExecuteScalar();
            int resultCount = Convert.ToInt32(result);
            
            if (resultCount > 0)
                return true;

            return false;
        }

        public static void Init()
        {
            if (CheckIfTableExists("RecipesFavTable") == false)
            {
                CreateTable();
            }
        }

        static void CreateTable()
        {
            SQLiteConnection conn = CreateConnection();
            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE RecipesFavTable(Col1 VARCHAR(100), Col2 VARCHAR(100))";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        public static void SaveRecipe(string Label, string Url)
        {
            SQLiteConnection conn = CreateConnection();
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO RecipesFavTable(Col1, Col2) VALUES('" + Label + "', '" + Url + "');";
            sqlite_cmd.ExecuteNonQuery();
        }

        public static List<Recipe> GetRecipes()
        {
            SQLiteConnection conn = CreateConnection();
            var RecipesList = new List<Recipe>();
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM RecipesFavTable";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string label = sqlite_datareader.GetString(0);
                string url = sqlite_datareader.GetString(1);
                RecipesList.Add(new Recipe(label, url));
            }
            conn.Close();
            return RecipesList;
        }
    }
}