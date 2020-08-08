using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleApp2
{

    public class Program
    {

        public class Book
        {
            public bool Is18Plus { get; set; }
            public string Code { get; set; }
        }

        public class Client
        {
            public int Age { get; set; }
            public string PassportId { get; set; }
        }

        public interface IClientRepository
        {
            Client GetClientByPassportId(string passportId);
        }

        public class ClientRepository : IClientRepository
        {
            private readonly List<Client> _clients;

            public ClientRepository()
            {
                _clients = new List<Client>();
                _clients.Add(new Client { PassportId = "888", Age = 12 });
                _clients.Add(new Client { PassportId = "bigboy", Age = 18 });
            }

            public Client GetClientByPassportId(string passportId)
            {
                foreach (var client in _clients)
                {
                    if (client.PassportId == passportId)
                    {
                        return client;
                    }
                }
                return null;
            }
        }

        public interface IBookRepository
        {
            Book GetBookByCode(string code);
            void AddBook(Book book);
            List<Book> GetAllBooks();
        }

        public class BookRepository : IBookRepository
        {
            private readonly List<Book> _books;

            public BookRepository()
            {
                _books = new List<Book>();
            }

            public void AddBook(Book book)
            {
                _books.Add(book);
            }

            public List<Book> GetAllBooks()
            {
                return _books;
            }

            public Book GetBookByCode(string code)
            {
                foreach (var book in _books)
                {
                    if (book.Code == code)
                    {
                        return book;
                    }
                }
                return null;
            }
        }

        public class Library
        {
            private readonly IBookRepository _bookRepository;
            private readonly IClientRepository _clientRepository;

            public Library(IBookRepository bookRepository,
                IClientRepository clientRepository)
            {
                _bookRepository = bookRepository;
                _clientRepository = clientRepository;
                Console.WriteLine("Library created");
            }

            public void AddBook(Book book)
            {
                _bookRepository.AddBook(book);
            }

            public List<Book> GetAllBooks()
            {
                return _bookRepository.GetAllBooks();
            }

            public bool GiveBook(string clientPassportId, string bookCode)
            {
                var client = _clientRepository.GetClientByPassportId(clientPassportId);
                var book = _bookRepository.GetBookByCode(bookCode);

                if (client == null || book == null)
                    return false;

                if (client.Age < 18 && book.Is18Plus)
                {
                    return false;
                }

                return true;
            }
        }

        static void Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<IBookRepository, BookRepository>();
            services.AddSingleton<IClientRepository, ClientRepository>();
            services.AddSingleton<Library>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            Library library1 = serviceProvider.GetRequiredService<Library>();
            library1.AddBook(new Book { Code = "888", Is18Plus = true });

            var lib1Books = library1.GetAllBooks();

            foreach (var book in lib1Books)
            {
                Console.WriteLine(book.Code);
                Console.WriteLine(book.Is18Plus);
                Console.WriteLine("______________________");
            }


            Library library2 = serviceProvider.GetRequiredService<Library>();
            library2.AddBook(new Book { Code = "777", Is18Plus = false });

            var lib2Books = library2.GetAllBooks();

            foreach (var book in lib2Books)
            {
                Console.WriteLine(book.Code);
                Console.WriteLine(book.Is18Plus);
                Console.WriteLine("______________________");
            }
        }
    }
}
