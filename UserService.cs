using System;
using System.Data.SqlClient;
using System.Net.Mail;

namespace TestApp
{
    // Esta classe viola vários princípios SOLID e tem problemas de segurança
    public class UserService
    {
        private string connectionString = "Server=localhost;Database=TestDB;Trusted_Connection=true;";
        
        // Método que faz MUITAS coisas (viola SRP)
        public void CreateUser(string email, string name, string password)
        {
            // Validação inline (deveria ser uma classe separada)
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("Email é obrigatório");
            
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Nome é obrigatório");
            
            if (password.Length < 6)
                throw new ArgumentException("Senha deve ter pelo menos 6 caracteres");
            
            // PROBLEMA CRÍTICO: SQL Injection vulnerability
            var connection = new SqlConnection(connectionString);
            connection.Open();
            var query = $"INSERT INTO Users (Email, Name, Password) VALUES ('{email}', '{name}', '{password}')";
            var command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
            connection.Close();
            
            // Envio de email (deveria ser um serviço separado)
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new System.Net.NetworkCredential("user@gmail.com", "password123"); // Credenciais hardcoded!
            
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("noreply@test.com");
            mailMessage.To.Add(email);
            mailMessage.Subject = "Bem-vindo!";
            mailMessage.Body = "Sua conta foi criada com sucesso!";
            
            smtpClient.Send(mailMessage);
            
            // Log (deveria usar um logger apropriado)
            Console.WriteLine($"Usuário {name} criado em {DateTime.Now}");
            
            // Não está fazendo dispose dos recursos!
        }
        
        // Método com problema de performance
        public void SendBulkEmails(string[] emails)
        {
            foreach (var email in emails)
            {
                // Problema: criando nova conexão SMTP para cada email
                var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new System.Net.NetworkCredential("user@gmail.com", "password123");
                
                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("noreply@test.com");
                mailMessage.To.Add(email);
                mailMessage.Subject = "Newsletter";
                mailMessage.Body = "Conteúdo da newsletter";
                
                smtpClient.Send(mailMessage);
                // Não está fazendo dispose!
            }
        }
        
        // Método que viola LSP (se fosse herdar de uma interface)
        public string GetUserData(int userId)
        {
            // Retorna string ao invés de um objeto tipado
            // Mistura lógica de acesso a dados com formatação
            var connection = new SqlConnection(connectionString);
            var query = $"SELECT * FROM Users WHERE Id = {userId}"; // Outro SQL injection!
            var command = new SqlCommand(query, connection);
            
            connection.Open();
            var reader = command.ExecuteReader();
            
            string result = "";
            if (reader.Read())
            {
                result = $"{reader["Name"]},{reader["Email"]},{reader["Password"]}"; // Expondo senha!
            }
            
            reader.Close();
            connection.Close();
            
            return result;
        }
    }
    
    // Classe que também tem problemas
    public class EmailValidator
    {
        // Método estático dificulta testes
        public static bool IsValid(string email)
        {
            // Validação muito simples
            return email.Contains("@");
        }
        
        // Método que faz mais do que deveria (viola SRP)
        public static bool ValidateAndLog(string email)
        {
            var isValid = IsValid(email);
            Console.WriteLine($"Email {email} é válido: {isValid}"); // Log misturado com validação
            return isValid;
        }
    }
}