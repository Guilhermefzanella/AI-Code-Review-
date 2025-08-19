using System;
using System.Collections.Generic;

namespace CodeReviewExample
{
    // Interface para padronizar repositórios
    public interface IRepository<T>
    {
        void Add(T item);
        IEnumerable<T> GetAll();
    }

    // Implementação simples em memória
    public class InMemoryRepository<T> : IRepository<T>
    {
        private readonly List<T> _items = new List<T>();

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _items.Add(item);
        }

        public IEnumerable<T> GetAll() => _items;
    }

    // Classe de domínio
    public class Customer
    {
        public int Id { get; }
        public string Name { get; }
        public string Email { get; }

        public Customer(int id, string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.", nameof(name));

            if (!email.Contains("@"))
                throw new ArgumentException("Invalid email format.", nameof(email));

            Id = id;
            Name = name;
            Email = email;
        }

        public override string ToString() => $"{Id} - {Name} ({Email})";
    }

    // Serviço de negócios
    public class CustomerService
    {
        private readonly IRepository<Customer> _repository;

        public CustomerService(IRepository<Customer> repository)
        {
            _repository = repository;
        }

        public void RegisterCustomer(int id, string name, string email)
        {
            var customer = new Customer(id, name, email);
            _repository.Add(customer);
        }

        public void ListCustomers()
        {
            foreach (var c in _repository.GetAll())
                Console.WriteLine(c);
        }
    }

    // Programa principal
    class Program
    {
        static void Main()
        {
            IRepository<Customer> repo = new InMemoryRepository<Customer>();
            var service = new CustomerService(repo);

            service.RegisterCustomer(1, "Alice", "alice@example.com");
            service.RegisterCustomer(2, "Bob", "bob@example.com");

            Console.WriteLine("Clientes cadastrados:");
            service.ListCustomers();
        }
    }
}
