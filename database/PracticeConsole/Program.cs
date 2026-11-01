using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.Data.SqlClient;

Console.OutputEncoding = Encoding.UTF8;
var connectionString = Environment.GetEnvironmentVariable("PRACTICE_DB_CONNECTION");
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.Error.WriteLine("Задайте переменную окружения PRACTICE_DB_CONNECTION.");
    return 1;
}
try
{
    await using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();
    var practice = new Practice(connection);
    var action = args.Length == 0 ? "all" : args[0].ToLowerInvariant();
    switch (action)
    {
        case "list": await practice.ListEmployees(); break;
        case "report": practice.PrintReport(); break;
        case "all":
            await practice.ListEmployees();
            practice.PrintReport();
            break;
        case "add":
            if (args.Length != 4 ||
                !decimal.TryParse(args[2].Replace(',', '.'), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture, out var salary) ||
                !int.TryParse(args[3], out var departmentId))
                throw new ArgumentException("Формат: add <имя> 50000.00 1");
            await practice.AddEmployee(args[1], salary, departmentId);
            break;
        case "demo": await practice.DemonstrateInsert(); break;
        case "verify": await practice.Verify(); break;
        default: throw new ArgumentException("Команды: list, add, report, all, demo, verify.");
    }
    return 0;
}
catch (SqlException ex)
{
    Console.Error.WriteLine(Practice.DescribeSqlError(ex));
    return 2;
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
}
catch (Exception ex) when (ex is InvalidOperationException or InvalidDataException)
{
    Console.Error.WriteLine(ex.Message);
    return 3;
}

internal sealed class Practice(SqlConnection connection)
{
    private SqlTransaction? transaction;
    private SqlCommand Command(string text) => new(text, connection, transaction);

    public static string DescribeSqlError(SqlException ex) => ex.Number switch
    {
        547 => "Нарушен внешний ключ: указанного отдела нет или запись используется другими данными.",
        2601 or 2627 => "Запись с таким уникальным ключом уже существует.",
        _ => $"Ошибка SQL Server (код {ex.Number}). Проверьте сервер, базу и права доступа."
    };

    public async Task ListEmployees()
    {
        Console.WriteLine("Задание 28. Сотрудники");
        await using var command = Command(
            "SELECT EmployeeName, Salary FROM dbo.Employees ORDER BY EmployeeId;");
        await using var reader = await command.ExecuteReaderAsync();
        var count = 0;
        while (await reader.ReadAsync())
        {
            Console.WriteLine($"{reader.GetString(0),-24} {reader.GetDecimal(1).ToString("F2", CultureInfo.InvariantCulture),12}");
            count++;
        }
        if (count == 0) Console.WriteLine("Сотрудников нет.");
        Console.WriteLine($"Всего: {count}");
    }

    public async Task<int> AddEmployee(string name, decimal salary, int departmentId)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
            throw new ArgumentException("Имя должно содержать от 1 до 100 символов.");
        if (salary < 0 || salary > 99999999.99m || decimal.Round(salary, 2) != salary)
            throw new ArgumentException("Зарплата должна быть от 0 до 99999999.99, не более двух знаков после запятой.");
        await using var command = Command("""
            INSERT INTO dbo.Employees(EmployeeName,Salary,HireDate,DepartmentId,ManagerId)
            OUTPUT INSERTED.EmployeeId
            VALUES (@name,@salary,CAST(GETDATE() AS DATE),@deptId,NULL);
            """);
        command.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = name.Trim();
        var salaryParameter = command.Parameters.Add("@salary", SqlDbType.Decimal);
        salaryParameter.Precision = 10;
        salaryParameter.Scale = 2;
        salaryParameter.Value = salary;
        command.Parameters.Add("@deptId", SqlDbType.Int).Value = departmentId;
        var id = Convert.ToInt32(await command.ExecuteScalarAsync(), CultureInfo.InvariantCulture);

        await using var confirm = Command("""
            SELECT EmployeeId,EmployeeName,Salary,DepartmentId,HireDate
            FROM dbo.Employees WHERE EmployeeId=@id;
            """);
        confirm.Parameters.Add("@id", SqlDbType.Int).Value = id;
        await using var reader = await confirm.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) throw new InvalidDataException("Не удалось подтвердить вставку.");
        Console.WriteLine($"Добавлен: {reader.GetInt32(0)} | {reader.GetString(1)} | " +
            $"{reader.GetDecimal(2).ToString("F2", CultureInfo.InvariantCulture)} | отдел {reader.GetInt32(3)} | " +
            $"{reader.GetDateTime(4):dd.MM.yyyy}");
        return id;
    }

    // По условию задания 30 используется синхронный ExecuteReader.
    public void PrintReport()
    {
        Console.WriteLine("Задание 30. Отчёт по отделам");
        using var command = Command("""
            SELECT d.DepartmentName, COUNT(e.EmployeeId) AS EmpCount, AVG(e.Salary) AS AvgSalary
            FROM dbo.Departments d
            LEFT JOIN dbo.Employees e ON d.DepartmentId=e.DepartmentId
            GROUP BY d.DepartmentName
            ORDER BY d.DepartmentName;
            """);
        using var reader = command.ExecuteReader();
        Console.WriteLine($"{"Отдел",-30} {"Сотрудников",12} {"Средняя зарплата",18}");
        var count = 0;
        while (reader.Read())
        {
            var average = reader.IsDBNull(2) ? "N/A" : reader.GetDecimal(2).ToString("F2", CultureInfo.InvariantCulture);
            Console.WriteLine($"{reader.GetString(0),-30} {reader.GetInt32(1),12} {average,18}");
            count++;
        }
        if (count == 0) Console.WriteLine("Нет данных для отчёта.");
    }

    public async Task DemonstrateInsert()
    {
        Console.WriteLine("Задание 29. Вставка и проверка");
        int? id = null;
        try
        {
            id = await AddEmployee("Учебный сотрудник ' SQL", 50000.25m, 1);
            // Вставка выполняется без явной транзакции: запись уже сохранена в базе.
            await using var check = Command("SELECT COUNT(*) FROM dbo.Employees WHERE EmployeeId=@id;");
            check.Parameters.Add("@id", SqlDbType.Int).Value = id.Value;
            if (Convert.ToInt32(await check.ExecuteScalarAsync()) != 1)
                throw new InvalidDataException("Запись после вставки отсутствует.");
            Console.WriteLine("Вставка сохранена, SELECT вернул 1 запись.");
        }
        finally
        {
            if (id.HasValue)
            {
                await using var delete = Command("DELETE FROM dbo.Employees WHERE EmployeeId=@id;");
                delete.Parameters.Add("@id", SqlDbType.Int).Value = id.Value;
                await delete.ExecuteNonQueryAsync();
                await using var check = Command("SELECT COUNT(*) FROM dbo.Employees WHERE EmployeeId=@id;");
                check.Parameters.Add("@id", SqlDbType.Int).Value = id.Value;
                Console.WriteLine($"Демонстрационная запись удалена. Осталось: {await check.ExecuteScalarAsync()}");
            }
        }
    }

    public async Task Verify()
    {
        Console.WriteLine("Проверки");
        try
        {
            await AddEmployee("Проверка FK", 50000, -1);
            throw new InvalidDataException("Ожидалась ошибка внешнего ключа.");
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            Console.WriteLine("FK: " + DescribeSqlError(ex));
        }
        try
        {
            await AddEmployee("Проверка зарплаты", -1, 1);
            throw new InvalidDataException("Ожидалась проверка отрицательной зарплаты.");
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Отрицательная зарплата: отклонена.");
        }
        transaction = (SqlTransaction)await connection.BeginTransactionAsync();
        try
        {
            await using (var emptyDepartment = Command(
                "INSERT INTO dbo.Departments(DepartmentName) VALUES(N'Пустой отдел для проверки');"))
                await emptyDepartment.ExecuteNonQueryAsync();
            PrintReport();
            await using (var clear = Command("""
                DELETE FROM dbo.EmployeeProjects;
                UPDATE dbo.Employees SET ManagerId=NULL;
                DELETE FROM dbo.Employees;
                DELETE FROM dbo.Departments;
                """))
                await clear.ExecuteNonQueryAsync();
            await ListEmployees();
            PrintReport();
        }
        finally
        {
            await transaction.RollbackAsync();
            await transaction.DisposeAsync();
            transaction = null;
        }
        Console.WriteLine("Проверочные изменения отменены.");
    }
}
