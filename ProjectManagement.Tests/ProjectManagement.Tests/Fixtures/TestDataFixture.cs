using ProjectManagement.DAL.Entities;

namespace ProjectManagement.Tests.Fixtures;

public static class TestDataFixture
{
    public static List<Employee> GetTestEmployees()
    {
        return new List<Employee>
        {
            new Employee
            {
                Id = 1,
                FirstName = "Иван",
                LastName = "Петров",
                Patronymic = "Иванович",
                Email = "ivan@test.com"
            },
            new Employee
            {
                Id = 2,
                FirstName = "Мария",
                LastName = "Сидорова",
                Patronymic = "Алексеевна",
                Email = "maria@test.com"
            },
            new Employee
            {
                Id = 3,
                FirstName = "Алексей",
                LastName = "Смирнов",
                Patronymic = "Дмитриевич",
                Email = "alexey@test.com"
            }
        };
    }

    public static List<Project> GetTestProjects()
    {
        return new List<Project>
        {
            new Project
            {
                Id = 1,
                Name = "Тестовый проект 1",
                CustomerCompany = "ООО Ромашка",
                ExecutorCompany = "ООО Сибирские технологии",
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2026, 12, 31),
                Priority = 5,
                ProjectManagerId = 1
            },
            new Project
            {
                Id = 2,
                Name = "Тестовый проект 2",
                CustomerCompany = "ООО ТехноСервис",
                ExecutorCompany = "ООО Сибирские технологии",
                StartDate = new DateTime(2026, 5, 1),
                EndDate = new DateTime(2026, 10, 31),
                Priority = 8,
                ProjectManagerId = 1
            }
        };
    }
}