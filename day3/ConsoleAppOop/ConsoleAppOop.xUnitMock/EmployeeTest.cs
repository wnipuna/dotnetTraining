using Moq;
using Xunit;

namespace ConsoleAppOop.xUnitMock
{
    public class EmployeeServiceTests
    {
        [Fact]
        public void AddEmployee_IncreasesCount()
        {
            var svc = new EmployeeService();
            svc.AddEmployee(new Employee { GetId = 1, GetName = "A", GetDepartment = "IT" });

            Assert.Single(svc.GetAllEmployees());
        }

        [Fact]
        public void GetEmployeeById_ReturnsCorrectEmployee()
        {
            var svc = new EmployeeService();
            svc.AddEmployee(new Employee { GetId = 2, GetName = "B", GetDepartment = "HR" });

            var emp = svc.GetEmployeeById(2);

            Assert.NotNull(emp);
            Assert.Equal(2, emp.GetId);
            Assert.Equal("B", emp.GetName);
        }

        [Fact]
        public void GetEmployeeById_ReturnsNull_WhenNotFound()
        {
            var svc = new EmployeeService();

            var emp = svc.GetEmployeeById(999);

            Assert.Null(emp);
        }

        [Fact]
        public void GetAllEmployees_ReturnsAllAdded()
        {
            var svc = new EmployeeService();
            svc.AddEmployee(new Employee { GetId = 1, GetName = "A", GetDepartment = "IT" });
            svc.AddEmployee(new Manager { GetId = 2, GetName = "M", GetDepartment = "IT", TeamSize = 3 });

            var all = svc.GetAllEmployees();

            Assert.Equal(2, all.Count);
        }
    }

    public class Consumer
    {
        private readonly IEmployeeService _service;
        public Consumer(IEmployeeService service) { _service = service; }

        public bool HasEmployee(int id) => _service.GetEmployeeById(id) != null;
    }

    public class ConsumerTests
    {
        [Fact]
        public void HasEmployee_UsesServiceViaMoq()
        {
            var mock = new Mock<IEmployeeService>();
            mock.Setup(s => s.GetEmployeeById(10))
                .Returns(new Employee { GetId = 10, GetName = "X", GetDepartment = "IT" });

            var consumer = new Consumer(mock.Object);

            Assert.True(consumer.HasEmployee(10));
            mock.Verify(s => s.GetEmployeeById(10), Times.Once);
        }
    }
}
