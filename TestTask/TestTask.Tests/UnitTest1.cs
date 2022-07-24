using NUnit.Framework;
using TestTask;

namespace TestTask.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Test1()
        {
            UserHandler userHandler = new UserHandler();
            FileManager fileManager = new FileManager("TestUsers.json");



            Assert.Pass();
        }
    }
}