using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Services
{
    // Classe que viola vários princípios SOLID
    public class UserService
    {
        private string connectionString = "Server=localhost;Database=MyDB;Trusted_Connection=true;";
        
        // Violação: SQL Injection + método muito grande + múltiplas responsabilidades
        public List<User> GetUsersByName(string name)
        {
            var users = new List<User>();
            
            var query = "SELECT * FROM Users WHERE Name = '" + name + "'";
            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    var user = new User();
                    user.Id = (int)reader["Id"];
                    user.Name = reader["Name"].ToString();
                    user.Email = reader["Email"].ToString();
                    
                    if (user.Email.Contains("@admin.com"))
                    {
                        user.IsAdmin = true;
                        // PROBLEMA: Log de informação sensível
                        Console.WriteLine("Admin user found: " + user.Email);
                    }
                    
                    // PROBLEMA: Validação no lugar errado
                    if (string.IsNullOrEmpty(user.Name))
                    {
                        throw new Exception("Invalid user name");
                    }
                    
                    users.Add(user);
                }
            }
            
            return users.OrderBy(u => u.Name).ToList();
        }
        
        public bool DeleteUser(int userId)
        {
            var query = "DELETE FROM Users WHERE Id = " + userId;
            
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand(query, connection);
                var result = command.ExecuteNonQuery();
                
                // PROBLEMA: Magic number
                return result > 0;
            }
        }
        
        public string ProcessUser(User user)
        {
            // Validação
            if (user == null) return "Error: User is null";
            
            // Formatação
            var formattedName = user.Name.ToUpper().Trim();
            
            // Cálculo de idade (lógica de negócio)
            var age = DateTime.Now.Year - user.BirthDate.Year;
            
            // Logging
            Console.WriteLine("Processing user: " + user.Name);
            
            // Envio de email (responsabilidade externa)
            SendWelcomeEmail(user.Email);
            
            // Retorno inconsistente
            return age > 18 ? "Adult" : "Minor";
        }
        
        private void SendWelcomeEmail(string email)
        {
            // Simulação de envio de email
            Console.WriteLine("Sending email to: " + email);
        }
    }
    
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsAdmin { get; set; }
    }
}
