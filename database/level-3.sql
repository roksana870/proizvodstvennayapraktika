USE [Practice.db];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
PRINT '===TASK 21===';
GO
CREATE OR ALTER PROCEDURE dbo.sp_GetEmployeesByDepartment @DeptId INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT EmployeeId, EmployeeName, Salary FROM dbo.Employees
    WHERE DepartmentId = @DeptId ORDER BY EmployeeId;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_GetDepartmentAverageSalary
    @DeptId INT, @AverageSalary DECIMAL(10,2) OUTPUT AS
BEGIN
    SET NOCOUNT ON;
    SELECT @AverageSalary = CAST(ROUND(AVG(Salary),2) AS DECIMAL(10,2))
    FROM dbo.Employees WHERE DepartmentId = @DeptId;
END;
GO
EXEC dbo.sp_GetEmployeesByDepartment @DeptId = 1;
DECLARE @Average DECIMAL(10,2);
EXEC dbo.sp_GetDepartmentAverageSalary @DeptId = 1, @AverageSalary = @Average OUTPUT;
SELECT @Average AS AverageSalary;
EXEC dbo.sp_GetEmployeesByDepartment @DeptId = -1;
EXEC dbo.sp_GetDepartmentAverageSalary @DeptId = -1, @AverageSalary = @Average OUTPUT;
SELECT @Average AS EmptyDepartmentAverage;
GO
PRINT '===TASK 22===';
GO
CREATE OR ALTER FUNCTION dbo.fn_GetAnnualSalary(@MonthlySalary DECIMAL(10,2))
RETURNS DECIMAL(12,2) AS
BEGIN
    RETURN @MonthlySalary * 12;
END;
GO
CREATE OR ALTER FUNCTION dbo.fn_GetEmployeesBySalaryRange(
    @Min DECIMAL(10,2), @Max DECIMAL(10,2))
RETURNS TABLE AS RETURN (
    SELECT EmployeeId, EmployeeName, Salary FROM dbo.Employees
    WHERE Salary BETWEEN @Min AND @Max
);
GO
SELECT EmployeeName, Salary, dbo.fn_GetAnnualSalary(Salary) AS AnnualSalary
FROM dbo.Employees ORDER BY EmployeeId;
SELECT * FROM dbo.fn_GetEmployeesBySalaryRange(50000,65000) ORDER BY EmployeeId;
SELECT dbo.fn_GetAnnualSalary(NULL) AS NullAnnualSalary;
SELECT COUNT(*) AS ReversedRangeCount FROM dbo.fn_GetEmployeesBySalaryRange(65000,50000);
GO
PRINT '===TASK 23===';
GO
SELECT EmployeeName, DepartmentId, Salary,
       ROW_NUMBER() OVER(ORDER BY Salary DESC, EmployeeId) AS RowNumber,
       RANK() OVER(PARTITION BY DepartmentId ORDER BY Salary DESC) AS SalaryRank,
       DENSE_RANK() OVER(PARTITION BY DepartmentId ORDER BY Salary DESC) AS DenseSalaryRank
FROM dbo.Employees ORDER BY Salary DESC, EmployeeId;
-- В исходных данных нет равных зарплат. Пример не меняет Employees.
WITH Example AS (
    SELECT * FROM (VALUES
        (1,N'Сотрудник 1',1,CAST(70000 AS DECIMAL(10,2))),
        (2,N'Сотрудник 2',1,CAST(70000 AS DECIMAL(10,2))),
        (3,N'Сотрудник 3',1,CAST(60000 AS DECIMAL(10,2)))
    ) v(EmployeeId,EmployeeName,DepartmentId,Salary)
)
SELECT EmployeeName, Salary,
       ROW_NUMBER() OVER(ORDER BY Salary DESC,EmployeeId) AS RowNumber,
       RANK() OVER(PARTITION BY DepartmentId ORDER BY Salary DESC) AS SalaryRank,
       DENSE_RANK() OVER(PARTITION BY DepartmentId ORDER BY Salary DESC) AS DenseSalaryRank
FROM Example ORDER BY Salary DESC,EmployeeId;
GO
PRINT '===TASK 24===';
GO
SELECT EmployeeName, DepartmentId, Salary,
       SUM(Salary) OVER(PARTITION BY DepartmentId) AS DepartmentTotal,
       CAST(ROUND(AVG(Salary) OVER(PARTITION BY DepartmentId),2) AS DECIMAL(10,2)) AS DepartmentAverage,
       CAST(ROUND(Salary - AVG(Salary) OVER(PARTITION BY DepartmentId),2) AS DECIMAL(10,2)) AS Difference
FROM dbo.Employees ORDER BY DepartmentId,EmployeeId;
SELECT DepartmentId,SUM(Salary) AS DepartmentTotal,
       CAST(ROUND(AVG(Salary),2) AS DECIMAL(10,2)) AS DepartmentAverage
FROM dbo.Employees GROUP BY DepartmentId ORDER BY DepartmentId;
GO
PRINT '===TASK 25===';
GO
WITH AvgSalary AS (SELECT AVG(Salary) AS AverageSalary FROM dbo.Employees)
SELECT e.EmployeeName,e.Salary FROM dbo.Employees e
CROSS JOIN AvgSalary a WHERE e.Salary > a.AverageSalary ORDER BY e.EmployeeId;
SELECT EmployeeName,Salary FROM dbo.Employees
WHERE Salary > (SELECT AVG(Salary) FROM dbo.Employees) ORDER BY EmployeeId;
WITH DepartmentAverages AS (
    SELECT DepartmentId,AVG(Salary) AS AverageSalary
    FROM dbo.Employees GROUP BY DepartmentId
)
SELECT TOP(3) d.DepartmentName,CAST(a.AverageSalary AS DECIMAL(10,2)) AS AverageSalary
FROM DepartmentAverages a
INNER JOIN dbo.Departments d ON d.DepartmentId=a.DepartmentId
ORDER BY a.AverageSalary DESC,a.DepartmentId;
GO
PRINT '===TASK 26===';
GO
SELECT EmployeeName,Salary,
       CASE WHEN Salary > 60000 THEN 'High'
            WHEN Salary BETWEEN 40000 AND 60000 THEN 'Medium'
            ELSE 'Low' END AS SalaryLevel,
       HireDate,
       CASE WHEN HireDate < '20200101' THEN 'Senior'
            WHEN HireDate < '20230101' THEN 'Mid'
            ELSE 'Junior' END AS Tenure
FROM dbo.Employees ORDER BY EmployeeId;
GO
PRINT '===TASK 27===';
GO
DECLARE @Today DATE = CAST(GETDATE() AS DATE);
SELECT @Today AS CalculationDate;
SELECT EmployeeName,HireDate,
       DATEDIFF(YEAR,HireDate,@Today) AS CalendarYearBoundaries,
       DATEDIFF(YEAR,HireDate,@Today)
         - CASE WHEN DATEADD(YEAR,DATEDIFF(YEAR,HireDate,@Today),HireDate) > @Today THEN 1 ELSE 0 END AS FullYears,
       DATEDIFF(MONTH,HireDate,@Today) AS CalendarMonthBoundaries,
       DATEDIFF(MONTH,HireDate,@Today)
         - CASE WHEN DATEADD(MONTH,DATEDIFF(MONTH,HireDate,@Today),HireDate) > @Today THEN 1 ELSE 0 END AS FullMonths,
       DATEADD(YEAR,1,HireDate) AS OneYearAfterHire,
       FORMAT(HireDate,'dd.MM.yyyy') AS FormattedHireDate
FROM dbo.Employees ORDER BY EmployeeId;
-- Проверка перед годовщиной, в годовщину и при найме 29 февраля.
SELECT HireDate,AsOfDate,
       DATEDIFF(YEAR,HireDate,AsOfDate)
       - CASE WHEN DATEADD(YEAR,DATEDIFF(YEAR,HireDate,AsOfDate),HireDate) > AsOfDate THEN 1 ELSE 0 END AS FullYears
FROM (VALUES
    (CAST('20211120' AS DATE),CAST('20261101' AS DATE)),
    (CAST('20211120' AS DATE),CAST('20261120' AS DATE)),
    (CAST('20200229' AS DATE),CAST('20210228' AS DATE)))
    v(HireDate,AsOfDate);
GO
