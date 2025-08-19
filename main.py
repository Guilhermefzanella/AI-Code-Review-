import sqlite3
import smtplib
from email.mime.text import MIMEText

class UserService:
    def __init__(self):
        self.db_path = "users.db"
    
    # Método que faz muitas coisas (viola SRP)
    def create_user(self, email, name, password):
        # Validação inline
        if not email:
            raise ValueError("Email é obrigatório")
        if not name:
            raise ValueError("Nome é obrigatório")
        if len(password) < 6:
            raise ValueError("Senha deve ter pelo menos 6 caracteres")
        
        # SQL Injection vulnerability
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()
        query = f"INSERT INTO users (email, name, password) VALUES ('{email}', '{name}', '{password}')"
        cursor.execute(query)  # PERIGOSO!
        conn.commit()
        conn.close()
        
        # Envio de email (deveria ser separado)
        self.send_welcome_email(email, name)
        
        # Log (deveria usar logging apropriado)
        print(f"Usuário {name} criado")
    
    def send_welcome_email(self, email, name):
        # Credenciais hardcoded
        smtp_server = smtplib.SMTP('smtp.gmail.com', 587)
        smtp_server.starttls()
        smtp_server.login('user@gmail.com', 'password123')  # PERIGOSO!
        
        msg = MIMEText(f"Bem-vindo, {name}!")
        msg['Subject'] = 'Conta criada'
        msg['From'] = 'noreply@test.com'
        msg['To'] = email
        
        smtp_server.send_message(msg)
        smtp_server.quit()
    
    # Método com problema de performance
    def get_all_users(self):
        conn = sqlite3.connect(self.db_path)
        cursor = conn.cursor()
        
        # Busca todos os usuários sem paginação
        cursor.execute("SELECT * FROM users")  # Pode ser muito lento
        users = cursor.fetchall()  # Carrega tudo na memória
        
        conn.close()
        return users