using NUnit.Framework;
using System.Linq;

namespace TestTask.Tests
{
    public class Tests
    {
        private UserHandler _userHandler;
        private FileManager _fileManager;
        private CheckArguments _checkArguments;

        private const string TestFilePath = "Users.json";

        [SetUp]
        public void Setup()
        {
            _fileManager = new FileManager(TestFilePath);
            _userHandler = new UserHandler();
            _checkArguments = new CheckArguments();
        }

        [Test]
        public void AddUserTest()
        {
            string firstName = "Аракадий";
            string lastName = "Цареградцев";
            string salary = "200,00";

            var bedoreAddUsersList = _fileManager.ReadJsonFile();

            Assert.IsNotNull(bedoreAddUsersList);

            _userHandler.AddUser(firstName, lastName, salary);

            var afterAddusersList = _fileManager.ReadJsonFile();

            Assert.IsNotNull(afterAddusersList);

            Assert.AreNotEqual(bedoreAddUsersList.Count(), afterAddusersList.Count());

            var lastUser = afterAddusersList.LastOrDefault();

            Assert.AreEqual(lastUser.FirstName, firstName);
            Assert.AreEqual(lastUser.LastName, lastName);
            Assert.AreEqual(lastUser.SalaryPerHour, _checkArguments.GetAddSalaryUser(salary));
        }

        [Test]
        public void AddNullUserTest()
        {
            string firstName = null;
            string lastName = null;
            string salary = null;

            var bedoreAddUsersList = _fileManager.ReadJsonFile();

            Assert.IsNotNull(bedoreAddUsersList);

            _userHandler.AddUser(firstName, lastName, salary);

            var afterAddusersList = _fileManager.ReadJsonFile();

            Assert.IsNotNull(afterAddusersList);

            Assert.AreNotEqual(bedoreAddUsersList.Count(), afterAddusersList.Count());

            var lastUser = afterAddusersList.LastOrDefault();

            Assert.AreEqual(lastUser.FirstName, string.Empty);
            Assert.AreEqual(lastUser.LastName, string.Empty);
            Assert.AreEqual(lastUser.SalaryPerHour, decimal.Zero);
        }

        [Test]
        public void AddUserSalryTextTest()
        {
            string firstName = "Георгий";
            string lastName = "Чивчян";
            string salary = "Тумасович ";

            var bedoreAddUsersList = _fileManager.ReadJsonFile();

            Assert.IsNotNull(bedoreAddUsersList);

            _userHandler.AddUser(firstName, lastName, salary);

            var afterAddusersList = _fileManager.ReadJsonFile();

            Assert.IsNotNull(afterAddusersList);

            Assert.AreNotEqual(bedoreAddUsersList.Count(), afterAddusersList.Count());

            var lastUser = afterAddusersList.LastOrDefault();

            Assert.AreEqual(lastUser.FirstName, firstName);
            Assert.AreEqual(lastUser.LastName, lastName);
            Assert.AreEqual(lastUser.SalaryPerHour, decimal.Zero);
        }
    }
}