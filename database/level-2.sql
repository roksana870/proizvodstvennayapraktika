USE [Practice.db];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO
PRINT '===TASK 11===';
GO
SELECT e.EmployeeName, m.EmployeeName AS ManagerName
FROM dbo.Employees e
LEFT JOIN dbo.Employees m ON m.EmployeeId = e.ManagerId
ORDER BY e.EmployeeId;
GO
PRINT '===TASK 12===';
GO
SELECT EmployeeName, Salary FROM dbo.Employees
WHERE Salary > (SELECT AVG(Salary) FROM dbo.Employees) ORDER BY EmployeeId;
SELECT EmployeeName, Salary FROM dbo.Employees
WHERE Salary = (SELECT MAX(Salary) FROM dbo.Employees) ORDER BY EmployeeId;
GO
PRINT '===TASK 13===';
GO
SELECT e.EmployeeName, e.Salary, e.DepartmentId,
       CAST(ROUND((SELECT AVG(d.Salary) FROM dbo.Employees d
                   WHERE d.DepartmentId = e.DepartmentId),2) AS DECIMAL(10,2)) AS DepartmentAverage,
       CAST(ROUND(e.Salary - (SELECT AVG(d.Salary) FROM dbo.Employees d
                   WHERE d.DepartmentId = e.DepartmentId),2) AS DECIMAL(10,2)) AS Difference
FROM dbo.Employees e ORDER BY e.EmployeeId;
GO
PRINT '===TASK 14===';
GO
SELECT e.EmployeeId, e.EmployeeName FROM dbo.Employees e
WHERE EXISTS (SELECT 1 FROM dbo.EmployeeProjects ep WHERE ep.EmployeeId = e.EmployeeId)
ORDER BY e.EmployeeId;
SELECT e.EmployeeId, e.EmployeeName FROM dbo.Employees e
WHERE NOT EXISTS (SELECT 1 FROM dbo.EmployeeProjects ep WHERE ep.EmployeeId = e.EmployeeId)
ORDER BY e.EmployeeId;
GO
PRINT '===TASK 15===';
GO
SELECT EmployeeId, EmployeeName FROM dbo.Employees WHERE Salary > 60000
UNION
SELECT EmployeeId, EmployeeName FROM dbo.Employees WHERE HireDate < '20220101'
ORDER BY EmployeeId;
SELECT EmployeeId, EmployeeName FROM dbo.Employees WHERE Salary > 60000
UNION ALL
SELECT EmployeeId, EmployeeName FROM dbo.Employees WHERE HireDate < '20220101'
ORDER BY EmployeeId;
SELECT COUNT(*) AS UnionCount FROM (
SELECT EmployeeId FROM dbo.Employees WHERE Salary > 60000
UNION SELECT EmployeeId FROM dbo.Employees WHERE HireDate < '20220101') u;
SELECT COUNT(*) AS UnionAllCount FROM (
SELECT EmployeeId FROM dbo.Employees WHERE Salary > 60000
UNION ALL SELECT EmployeeId FROM dbo.Employees WHERE HireDate < '20220101') u;
GO
PRINT '===TASK 16===';
GO
BEGIN TRANSACTION;
SELECT EmployeeId, Salary FROM dbo.Employees ORDER BY EmployeeId;
UPDATE dbo.Employees SET Salary = Salary * 1.10 WHERE DepartmentId = 1;
SELECT EmployeeId, Salary FROM dbo.Employees ORDER BY EmployeeId;
SELECT EmployeeId, HireDate, Salary FROM dbo.Employees ORDER BY EmployeeId;
UPDATE dbo.Employees SET Salary = Salary + 5000 WHERE HireDate < '20220101';
SELECT EmployeeId, HireDate, Salary FROM dbo.Employees ORDER BY EmployeeId;
SELECT EmployeeId, HireDate FROM dbo.Employees WHERE EmployeeId = 4;
UPDATE dbo.Employees SET HireDate = '20230211' WHERE EmployeeId = 4;
SELECT EmployeeId, HireDate FROM dbo.Employees WHERE EmployeeId = 4;
ROLLBACK TRANSACTION;
SELECT EmployeeId, Salary, HireDate FROM dbo.Employees ORDER BY EmployeeId;
GO
PRINT '===TASK 17===';
GO
BEGIN TRANSACTION;
-- В исходных данных часов 0 нет. Добавляем тестовое назначение.
INSERT INTO dbo.EmployeeProjects(EmployeeId,ProjectId,Hours) VALUES (9,5,0);
SELECT * FROM dbo.EmployeeProjects WHERE Hours = 0;
DELETE FROM dbo.EmployeeProjects WHERE Hours = 0;
SELECT COUNT(*) AS ZeroHoursAfterDelete FROM dbo.EmployeeProjects WHERE Hours = 0;
SELECT COUNT(*) AS AssignmentsAfterDelete FROM dbo.EmployeeProjects;

-- У сотрудника 8 нет подчинённых, поэтому self-FK не мешает удалению.
SELECT EmployeeId, EmployeeName FROM dbo.Employees WHERE EmployeeId = 8;
SELECT * FROM dbo.EmployeeProjects WHERE EmployeeId = 8;
DELETE FROM dbo.EmployeeProjects WHERE EmployeeId = 8;
SELECT COUNT(*) AS AssignmentsForEmployee8 FROM dbo.EmployeeProjects WHERE EmployeeId = 8;
DELETE FROM dbo.Employees WHERE EmployeeId = 8;
SELECT COUNT(*) AS Employee8AfterDelete FROM dbo.Employees WHERE EmployeeId = 8;
SELECT COUNT(*) AS EmployeesAfterDelete FROM dbo.Employees;

SELECT EmployeeId, EmployeeName INTO #EmployeesCopy FROM dbo.Employees;
SELECT COUNT(*) AS CopyBeforeDelete FROM #EmployeesCopy;
SELECT * FROM #EmployeesCopy WHERE EmployeeId = 4;
DELETE FROM #EmployeesCopy WHERE EmployeeId = 4;
SELECT COUNT(*) AS CopyAfterWhere FROM #EmployeesCopy;
SELECT * FROM #EmployeesCopy ORDER BY EmployeeId;
DELETE FROM #EmployeesCopy;
SELECT COUNT(*) AS CopyAfterDeleteAll FROM #EmployeesCopy;
DROP TABLE #EmployeesCopy;
ROLLBACK TRANSACTION;
SELECT COUNT(*) AS RestoredEmployees FROM dbo.Employees;
SELECT COUNT(*) AS RestoredAssignments FROM dbo.EmployeeProjects;
GO
PRINT '===TASK 18===';
GO
DECLARE @OriginalSalary DECIMAL(10,2);
SELECT @OriginalSalary = Salary FROM dbo.Employees WHERE EmployeeId = 4;
SELECT EmployeeId, Salary AS BeforeTransaction FROM dbo.Employees WHERE EmployeeId = 4;
BEGIN TRANSACTION;
UPDATE dbo.Employees SET Salary = Salary + 1000 WHERE EmployeeId = 4;
SELECT EmployeeId, Salary AS InsideRollbackTransaction FROM dbo.Employees WHERE EmployeeId = 4;
ROLLBACK TRANSACTION;
SELECT EmployeeId, Salary AS AfterRollback FROM dbo.Employees WHERE EmployeeId = 4;
BEGIN TRANSACTION;
UPDATE dbo.Employees SET Salary = Salary + 1000 WHERE EmployeeId = 4;
SELECT EmployeeId, Salary AS InsideCommitTransaction FROM dbo.Employees WHERE EmployeeId = 4;
COMMIT TRANSACTION;
SELECT EmployeeId, Salary AS AfterCommit FROM dbo.Employees WHERE EmployeeId = 4;
-- COMMIT уже выполнен. Отдельным UPDATE восстанавливаем данные для следующих запусков.
SELECT EmployeeId, Salary AS BeforeRestore FROM dbo.Employees WHERE EmployeeId = 4;
UPDATE dbo.Employees SET Salary = @OriginalSalary WHERE EmployeeId = 4;
SELECT EmployeeId, Salary AS AfterRestore FROM dbo.Employees WHERE EmployeeId = 4;
GO
PRINT '===TASK 19===';
GO
CREATE OR ALTER VIEW dbo.vw_EmployeeDetails AS
SELECT e.EmployeeName, d.DepartmentName, p.ProjectName, ep.Hours
FROM dbo.Employees e
INNER JOIN dbo.Departments d ON d.DepartmentId = e.DepartmentId
INNER JOIN dbo.EmployeeProjects ep ON ep.EmployeeId = e.EmployeeId
INNER JOIN dbo.Projects p ON p.ProjectId = ep.ProjectId;
GO
SELECT * FROM dbo.vw_EmployeeDetails
ORDER BY DepartmentName, EmployeeName, ProjectName;
DROP VIEW dbo.vw_EmployeeDetails;
SELECT COUNT(*) AS ViewCountAfterDrop FROM sys.views
WHERE object_id = OBJECT_ID(N'dbo.vw_EmployeeDetails');
GO
PRINT '===TASK 20===';
GO
DROP INDEX IF EXISTS IX_Employees_DepartmentId ON dbo.Employees;
DROP INDEX IF EXISTS IX_Employees_Salary ON dbo.Employees;
SET STATISTICS IO ON;
SET STATISTICS PROFILE ON;
PRINT 'Before indexes';
SELECT EmployeeId, DepartmentId, Salary FROM dbo.Employees
WHERE DepartmentId = 1 AND Salary > 50000;
SET STATISTICS PROFILE OFF;
SET STATISTICS IO OFF;
CREATE INDEX IX_Employees_DepartmentId ON dbo.Employees(DepartmentId) INCLUDE(Salary);
CREATE INDEX IX_Employees_Salary ON dbo.Employees(Salary);
GO
SET STATISTICS IO ON;
SET STATISTICS PROFILE ON;
PRINT 'After indexes: optimizer choice';
SELECT EmployeeId, DepartmentId, Salary FROM dbo.Employees
WHERE DepartmentId = 1 AND Salary > 50000;
PRINT 'After indexes: forced seek for comparison';
SELECT EmployeeId, DepartmentId, Salary FROM dbo.Employees WITH (INDEX(IX_Employees_DepartmentId), FORCESEEK)
WHERE DepartmentId = 1 AND Salary > 50000;
SET STATISTICS PROFILE OFF;
SET STATISTICS IO OFF;
SELECT name, type_desc FROM sys.indexes
WHERE object_id = OBJECT_ID(N'dbo.Employees') AND name LIKE N'IX_Employees_%';
GO
