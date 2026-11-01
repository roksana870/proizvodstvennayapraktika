USE PracticeDB;
GO
SET NOCOUNT ON;

PRINT '===TASK 1===';
GO
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME;
SELECT fk.name AS ForeignKeyName,
       OBJECT_NAME(fk.parent_object_id) AS SourceTable,
       COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS SourceColumn,
       OBJECT_NAME(fk.referenced_object_id) AS TargetTable,
       COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS TargetColumn
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fkc ON fkc.constraint_object_id = fk.object_id
ORDER BY SourceTable, SourceColumn;
SELECT t.name AS TableName, kc.name AS PrimaryKeyName
FROM sys.tables AS t
INNER JOIN sys.key_constraints AS kc ON kc.parent_object_id = t.object_id
WHERE kc.type = 'PK'
ORDER BY t.name;
SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME, ORDINAL_POSITION;
GO

PRINT '===TASK 2===';
GO
SELECT N'Departments' AS TableName, COUNT(*) AS [RowCount] FROM dbo.Departments
UNION ALL SELECT N'Employees', COUNT(*) FROM dbo.Employees
UNION ALL SELECT N'Projects', COUNT(*) FROM dbo.Projects
UNION ALL SELECT N'EmployeeProjects', COUNT(*) FROM dbo.EmployeeProjects;
GO

PRINT '===TASK 3===';
GO
SELECT EmployeeId, EmployeeName, Salary FROM dbo.Employees
WHERE Salary > 50000 ORDER BY EmployeeId;
SELECT EmployeeId, EmployeeName, HireDate FROM dbo.Employees
WHERE YEAR(HireDate) = 2023 ORDER BY EmployeeId;
SELECT EmployeeId, EmployeeName FROM dbo.Employees
WHERE EmployeeName LIKE N'А%' ORDER BY EmployeeId;
SELECT EmployeeId, EmployeeName, DepartmentId FROM dbo.Employees
WHERE DepartmentId = 2 ORDER BY EmployeeId;
SELECT EmployeeId, EmployeeName, Salary FROM dbo.Employees
WHERE Salary BETWEEN 40000 AND 60000 ORDER BY EmployeeId;
GO

PRINT '===TASK 4===';
GO
SELECT TOP (3) EmployeeName, Salary FROM dbo.Employees
ORDER BY Salary DESC, EmployeeId;
SELECT TOP (5) EmployeeName, HireDate FROM dbo.Employees
ORDER BY HireDate, EmployeeId;
SELECT EmployeeName, DepartmentId, Salary FROM dbo.Employees
ORDER BY DepartmentId, Salary DESC, EmployeeId;
GO

PRINT '===TASK 5===';
GO
SELECT COUNT(*) AS EmployeeCount,
       SUM(Salary) AS TotalSalary,
       CAST(ROUND(AVG(Salary), 2) AS DECIMAL(10,2)) AS AverageSalary,
       MIN(Salary) AS MinSalary,
       MAX(Salary) AS MaxSalary
FROM dbo.Employees;
SELECT COUNT(*) AS EmployeesAbove50000
FROM dbo.Employees WHERE Salary > 50000;
GO

PRINT '===TASK 6===';
GO
SELECT DepartmentId, COUNT(*) AS EmployeeCount,
       CAST(ROUND(AVG(Salary), 2) AS DECIMAL(10,2)) AS AverageSalary,
       SUM(Salary) AS TotalSalary
FROM dbo.Employees
GROUP BY DepartmentId
ORDER BY AverageSalary DESC, DepartmentId;
GO

PRINT '===TASK 7===';
GO
SELECT DepartmentId, COUNT(*) AS EmployeeCount FROM dbo.Employees
GROUP BY DepartmentId HAVING COUNT(*) > 2 ORDER BY DepartmentId;
SELECT DepartmentId,
       CAST(ROUND(AVG(Salary), 2) AS DECIMAL(10,2)) AS AverageSalary
FROM dbo.Employees
GROUP BY DepartmentId HAVING AVG(Salary) > 55000 ORDER BY DepartmentId;
SELECT DepartmentId, SUM(Salary) AS TotalSalary FROM dbo.Employees
GROUP BY DepartmentId HAVING SUM(Salary) > 150000 ORDER BY DepartmentId;
GO

PRINT '===TASK 8===';
GO
SELECT e.EmployeeName, d.DepartmentName, e.Salary
FROM dbo.Employees AS e
INNER JOIN dbo.Departments AS d ON d.DepartmentId = e.DepartmentId
ORDER BY d.DepartmentName, e.Salary DESC, e.EmployeeId;
GO

PRINT '===TASK 9===';
GO
SELECT e.EmployeeId, e.EmployeeName
FROM dbo.Employees AS e
LEFT JOIN dbo.EmployeeProjects AS ep ON ep.EmployeeId = e.EmployeeId
WHERE ep.EmployeeId IS NULL
ORDER BY e.EmployeeId;
GO

PRINT '===TASK 10===';
GO
SELECT e.EmployeeName, d.DepartmentName, p.ProjectName, ep.Hours
FROM dbo.Employees AS e
INNER JOIN dbo.Departments AS d ON d.DepartmentId = e.DepartmentId
INNER JOIN dbo.EmployeeProjects AS ep ON ep.EmployeeId = e.EmployeeId
INNER JOIN dbo.Projects AS p ON p.ProjectId = ep.ProjectId
ORDER BY d.DepartmentName, e.EmployeeName, p.ProjectName, e.EmployeeId, p.ProjectId;
GO
