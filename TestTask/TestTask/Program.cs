using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;

namespace TestTask
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("salaryPerHour")]
        public decimal SalaryPerHour { get; set; }
    }

    public class UserHandler
    {

        private readonly FileManager _fileManager;
        private readonly CheckArguments _checkArguments;
        public UserHandler(string filePatch)
        {
            _fileManager = new FileManager(filePatch);
            _checkArguments = new CheckArguments();
        }

        public void AddUser(string firstName, string lastName, string salary)
        {
            var user = new User();
            var usersList = _fileManager.ReadJsonFile();
            var lastUser = usersList.LastOrDefault();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(salary))
            {
                Console.WriteLine("Одно из полей не заполненно!");
                return;
            }

            user.Id = lastUser == null ? 0 : lastUser.Id + 1;
            user.FirstName = _checkArguments.GetAddNameUser(firstName);
            user.LastName = _checkArguments.GetAddNameUser(lastName);
            user.SalaryPerHour = _checkArguments.GetAddSalaryUser(salary);

            usersList.Add(user);

            _fileManager.WriteJsonFile(usersList);

            Console.WriteLine($"Добавлен сотрудник с id: {user.Id}.");
            GetUserInfo(user.Id);
        }

        public void UpdateUser(int? id, string firstName, string lastName, string salary)
        {
            var usersList = _fileManager.ReadJsonFile();

            if (usersList.Count == 0)
            {
                Console.WriteLine("Список сотрудников пуст, добавьте сотрудника!");
                return;
            }

            if (id == null)
            {
                Console.WriteLine("Укажите id сотрудника!");
                return;
            }

            var user = usersList.FirstOrDefault(user => user.Id == id);

            if (user == null)
            {
                Console.WriteLine($"Сотрудник с id {id} не найден.");
                return;
            }
            else
            {
                user.FirstName = _checkArguments.GetUpdateNameUser(firstName, user.FirstName);
                user.LastName = _checkArguments.GetUpdateNameUser(lastName, user.LastName);
                user.SalaryPerHour = _checkArguments.GetUpdateSalaryUser(salary, user.SalaryPerHour);

                _fileManager.WriteJsonFile(usersList);

                Console.WriteLine($"Изменен сотрудник с id {id}:");
                GetUserInfo(user.Id);
            }
        }

        public void GetUserInfo(int? id)
        {
            var usersList = _fileManager.ReadJsonFile();

            if (usersList.Count == 0)
            {
                Console.WriteLine("Список сотрудников пуст, добавьте сотрудника!");
                return;
            }

            if (id == null)
            {
                Console.WriteLine("Укажите id сотрудника!");
                return;
            }

            var user = usersList.FirstOrDefault(user => user.Id == id);

            if (user == null)
            {
                Console.WriteLine($"Сотрудник с id {id} не найден.");
                return;
            }

            Console.WriteLine($"Id = {user.Id}, FirstName = {user.FirstName}, LastName = {user.LastName}," +
                              $" SalaryPerHour = {user.SalaryPerHour}");
        }

        public void DeleteUser(int? id)
        {
            var usersList = _fileManager.ReadJsonFile();

            if (usersList.Count == 0)
            {
                Console.WriteLine("Список сотрудников пуст, добавьте сотрудника!");
                return;
            }

            if (id == null)
            {
                Console.WriteLine("Укажите id сотрудника!");
                return;
            }

            var user = usersList.FirstOrDefault(user => user.Id == id);

            if (user == null)
            {
                Console.WriteLine($"Сотрудник с id {id} не найден.");
                return;
            }
            usersList.Remove(user);

            _fileManager.WriteJsonFile(usersList);

            Console.WriteLine($"Сотрудник c id: {id} удален!");
        }

        public void GetAllUser()
        {
            var usersList = _fileManager.ReadJsonFile();

            if (usersList.Count == 0)
            {
                Console.WriteLine("Список сотрудников пуст, добавьте сотрудника!");
                return;
            }

            foreach (var user in usersList)
            {
                Console.WriteLine($"Id = {user.Id}, FirstName = {user.FirstName}, LastName = {user.LastName}," +
                                  $" SalaryPerHour = {user.SalaryPerHour}");
            }
        }
    }

    public class CheckArguments
    {
        public string GetAddNameUser(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                newValue = string.Empty;
            }

            return newValue;
        }

        public string GetUpdateNameUser(string newValue, string currentValue)
        {
            if (!string.IsNullOrEmpty(newValue) && !string.Equals(newValue, currentValue))
            {
                currentValue = newValue;
            }

            return currentValue;
        }

        public decimal GetAddSalaryUser(string newSalary)
        {
            decimal salary = decimal.Zero;

            if (decimal.TryParse(newSalary, out var value) && !string.IsNullOrEmpty(newSalary))
            {
                salary = value;
            }
            return salary;
        }

        public decimal GetUpdateSalaryUser(string newSalary, decimal currentSalary)
        {
            if (decimal.TryParse(newSalary, out var value) && !string.IsNullOrEmpty(newSalary))
            {
                if (value != currentSalary)
                {
                    currentSalary = value;
                }
            }
            return currentSalary;
        }
    }

    public class FileManager
    {
        private readonly string _filePath;

        public FileManager(string filePatch)
        {
            _filePath = filePatch;
        }

        public List<User> ReadJsonFile()
        {
            try
            {
                List<User> readjson = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(_filePath));
                if (readjson == null)
                {
                    readjson = new List<User>();
                }
                return readjson;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<User>();
            }
        }

        public void WriteJsonFile(List<User> usersList)
        {
            try
            {
                var json = JsonConvert.SerializeObject(usersList.ToArray(), Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class CommandLineOptions
    {
        [Value(index: 0, Required = true, HelpText = "Введите команду для выоплнения")]
        public string Command { get; set; }

        [Option(longName: "Id", Required = false, HelpText = "id сотрудника")]
        public int? ID { get; set; }

        [Option(longName: "FirstName", Required = false, HelpText = "Имя сотрудника", Default = "")]
        public string FristName { get; set; }

        [Option(longName: "LastName", Required = false, HelpText = "Фамилия сотрудника", Default = "")]
        public string LastName { get; set; }

        [Option(longName: "Salary", Required = false, HelpText = "Зараплата сотрудника", Default = "")]
        public string Salary { get; set; }
    }

    class Program
    {
        private const string _FilePath = "Users.json";

        private static UserHandler _userHandler;

        static void Main(string[] args)
        {
            try
            {
                _userHandler = new UserHandler(_FilePath);
                var parserResults = Parser.Default.ParseArguments<CommandLineOptions>(args);
                parserResults.WithParsed(options => Run(options));
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Run(CommandLineOptions options)
        {
            switch (options.Command)
            {
                case "add": // add --FirstName *значение* --LastName *значение* --Salary *значние*             
                    _userHandler.AddUser(options.FristName, options.LastName, options.Salary);
                    break;

                case "update": // update --Id *вводим нужный id*             
                    _userHandler.UpdateUser(options.ID, options.FristName, options.LastName, options.Salary);
                    break;

                case "get": // get --Id *значение*                 
                    _userHandler.GetUserInfo(options.ID);
                    break;

                case "delete": //delete --Id *значение* 
                    _userHandler.DeleteUser(options.ID);
                    break;

                case "getall": //getall
                    _userHandler.GetAllUser();
                    break;
            }
        }
    }
}
