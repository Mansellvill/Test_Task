using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using CommandLine;


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
        private const string FilePath = "Users.json";

        FileManager fileManager = new FileManager(FilePath);
        ChekArguments chekArguments = new ChekArguments();

        public void AddUser(string firstName, string lastName, string salary) 
        {
            var user = new User();
            var usersList = fileManager.ReadJsonFile();
            var lastUser = usersList.LastOrDefault();

            user.Id = lastUser == null ? 0 : lastUser.Id + 1;
            user.FirstName = chekArguments.AddNamesUser(firstName, user.FirstName);
            user.LastName = chekArguments.AddNamesUser(lastName, user.LastName);
            user.SalaryPerHour = chekArguments.AddSalaryUser(salary, user.SalaryPerHour);

            usersList.Add(user);

            var jsonFile = JsonConvert.SerializeObject(usersList);
            fileManager.WrriteJsonFile(jsonFile);

            Console.WriteLine($"Добавлен сотрудник с id: {user.Id}");
            GetUser(user.Id);
        }

        public void UpadateUser(int id, string firstName, string lastName, string salary)
        {
            var usersList = fileManager.ReadJsonFile();
            var user = usersList.FirstOrDefault(user => user.Id == id);
    
            if (user == null)
            {
                Console.WriteLine($"Сотрудник с id {id} не найден.");
            }
            else
            {
                user.FirstName = chekArguments.AddNamesUser(firstName, user.FirstName);
                user.LastName = chekArguments.AddNamesUser(lastName, user.LastName);
                user.SalaryPerHour = chekArguments.AddSalaryUser(salary, user.SalaryPerHour);

                var jsonFile = JsonConvert.SerializeObject(usersList);
                fileManager.WrriteJsonFile(jsonFile);

                Console.WriteLine($"Изменен сотрудник с id {id}:");
                GetUser(user.Id);
            }
            

        }

        public void GetUser(int id)
        {
            var user = fileManager.ReadJsonFile().FirstOrDefault(user => user.Id == id);

            if (user == null)
            {
                Console.WriteLine($"Сотрудник с id {id} не найден.");
            }
            else
            {
                Console.WriteLine($"Id = {user.Id}, FirstName = {user.FirstName}, LastName = {user.LastName}, SalaryPerHour = {user.SalaryPerHour}");
            }
            
        }

        public void DeleteUser(int id)
        {
            var usersList = fileManager.ReadJsonFile();
            var user = usersList.FirstOrDefault(user => user.Id == id);

            if (user == null)
            {
                Console.WriteLine($"Сотрудник с id {id} не найден.");
            }

            else 
            {
                usersList.Remove(user);
                var jsonFile = JsonConvert.SerializeObject(usersList);
                fileManager.WrriteJsonFile(jsonFile);
                Console.WriteLine($"Сотрудник c id: {id} удален!");
            }  
        }

        public void GetAllUser()
        {
            foreach (var user in fileManager.ReadJsonFile())
            {
                Console.WriteLine($"Id = {user.Id}, FirstName = {user.FirstName}, LastName = {user.LastName}, SalaryPerHour = {user.SalaryPerHour}");
            }
        }
    }

    public class ChekArguments
    {
        public string AddNamesUser(String newValue, String currentValue)
        {
            if (CheckValues(newValue, currentValue))
            {
                currentValue = newValue;
            }

            return currentValue;
        }

        public bool CheckValues(string newValue, string curentValue)
        {
            return !String.IsNullOrEmpty(newValue) && !String.Equals(newValue, curentValue);
        }

        public decimal AddSalaryUser(string newSalary, decimal currnetSalary)
        {
            if (Decimal.TryParse(newSalary, out var value) && !String.IsNullOrEmpty(newSalary))
            {
                if (value != currnetSalary)
                {
                    currnetSalary = value;
                }
            }

            return currnetSalary;
        }
    }

    public class FileManager
    {
        private string _filePath;

        public FileManager (string filePatch)
        {
            _filePath = filePatch;
        }

        public List<User> ReadJsonFile()
        {
            try
            {
                List<User> readjson = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(_filePath));
                return readjson;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new List<User>();
            }
        }

        public void WrriteJsonFile(string json)
        {
            try
            {
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

        [Option(longName: "Id", Required = false, HelpText = "id сотрудника", Default = 0)]
        public int ID { get; set; }

        [Option(longName: "FirstName", Required = false, HelpText = "Имя сотрудника", Default = "")]
        public string FristName { get; set; }

        [Option(longName: "LastName", Required = false, HelpText = "Фамилия сотрудника", Default = "")]
        public string LastName { get; set; }

        [Option(longName: "Salary", Required = false, HelpText = "Зараплата сотрудника", Default = "")]
        public string Salary { get; set; }
    }

    class Program
    {
        private static UserHandler _userHandler;
        static void Main(string[] args)
        {
            try
            {
                _userHandler = new UserHandler();
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
                    _userHandler.UpadateUser(options.ID, options.FristName, options.LastName, options.Salary);
                    break;

                case "get": // get --Id *значение*                 
                    _userHandler.GetUser(options.ID);
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
