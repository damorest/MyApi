using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;



namespace ForPostMAn
{
    public class User
    {
        public string? gender { get; set; }
        public string? email { get; set; }
        public string? id { get; set; }
       
    }

   
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static string url = "https://randomuser.me/api/";

        public static void SaveUser(User user)
        {
            SQLiteConnection sqlite_conn = CreateConnection();
           // CreateTable(sqlite_conn);
            InsertData(sqlite_conn,user);


        }
        static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return sqlite_conn;
        }
        static void CreateTable(SQLiteConnection conn)
        {

            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS Users (gender VARCHAR(20), email VARCHAR(20), id VARCHAR(20))";

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();           
        }
        static void InsertData(SQLiteConnection conn, User user)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO Users (gender, email, id)  VALUES('{user.gender}', '{user.email}', '{user.id}'); ";
            Console.WriteLine($"{user.gender}, '{user.email}', '{user.id}");
            sqlite_cmd.ExecuteNonQuery();

        }
        public static User GetFromDB(string userId)
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
           return ReadData(sqlite_conn, userId);
        }
        static User ReadData(SQLiteConnection conn, string userId)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT * FROM Users WHERE id = '{userId}'";
            //sqlite_cmd.CommandText = "SELECT * FROM Names WHERE name like '%t%'";
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            User user = null;
            while (sqlite_datareader.Read())
            {
                string gender = sqlite_datareader.GetString(0);
                string email = sqlite_datareader.GetString(1);
                string id = sqlite_datareader.GetString(2);
               
                 user = new User
                 {
                     gender = gender,
                     email = email,
                     id = id
                 }; ;
               
              //  Console.WriteLine(gender +" "+ email +" "+ id);
            }
            conn.Close();
            return user;
        }
            async static Task Main(string[] args)
        {
            await getUser();
            

            async Task getUser()
            {
                Console.WriteLine("Input name user");
                var id = Console.ReadLine();
          
                User newUser = GetFromDB(id);
                
                if (newUser == null)
                {
                    var responseString = await client.GetStringAsync(url);
                   
                    //Console.WriteLine("Parsing JSON...");
                    var responseData = (JObject)JsonConvert.DeserializeObject(responseString);
                    var gender = responseData.SelectToken("results[0].gender");
                    var email = responseData.SelectToken("results[0].email");

                User user = new User
                 {
                     gender = (string)gender,
                     email = (string)email,
                     id = id
                 };
                    SaveUser(user);
                    Console.WriteLine($" Новый user c именем = {user.id}, пол = {user.gender}, email = {user.email} создан");
                    

                }
                else
                {
                    Console.WriteLine("Такой user уже существует");
                }
                /*  var responseString = await client.GetStringAsync(url);
                 // await Console.Out.WriteLineAsync(responseString);
                  Console.WriteLine("Parsing JSON...");
                  var responseData = (JObject)JsonConvert.DeserializeObject(responseString);
                  var gender = responseData.SelectToken("results[0].gender");
                  var email = responseData.SelectToken("results[0].email");*/

                /* User user = new User
                 {
                     gender = (string)gender,
                     email = (string)email,
                     id = id
                 };*/
                //Console.WriteLine($"id = {user.id}, gender = {user.gender}, email = {user.email}");
                // SaveUser(user);

      
                

               


            }
        }
    }
}
