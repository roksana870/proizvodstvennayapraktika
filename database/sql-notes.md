# SQL notes — Уровень 1 (задания 1–10)

Учебная база: Practice.db. Проверено 01.11.2026 на локальном Microsoft SQL Server 2022 (16.0.1000.6) через sqlcmd. Ниже приведён фактический вывод сервера. Порядок кириллических имён определяется сортировкой базы.

Запуск: сначала init.sql, затем level-1.sql. **init.sql удаляет и заново создаёт Practice.db: повторный запуск уничтожит изменения в этой учебной базе.** Запросы level-1.sql только читают данные и безопасны для повторного выполнения.

## Задание 1. Создание базы и таблиц

### SQL

Создание базы и схемы (из init.sql):

```sql
USE master;
GO
-- =====================================================
-- init.sql — учебная БД Practice.db
-- =====================================================
IF DB_ID('Practice.db') IS NOT NULL
BEGIN
    ALTER DATABASE [Practice.db] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [Practice.db];
END
GO
CREATE DATABASE [Practice.db];
GO
USE [Practice.db];
GO
CREATE TABLE Departments (
    DepartmentId   INT PRIMARY KEY IDENTITY(1,1),
    DepartmentName NVARCHAR(100) NOT NULL
);
CREATE TABLE Employees (
    EmployeeId   INT PRIMARY KEY IDENTITY(1,1),
    EmployeeName NVARCHAR(100) NOT NULL,
    Salary       DECIMAL(10,2) NOT NULL,
    HireDate     DATE NOT NULL,
    DepartmentId INT NOT NULL,
    ManagerId    INT NULL,
    CONSTRAINT FK_Employees_Departments FOREIGN KEY (DepartmentId)
        REFERENCES Departments(DepartmentId),
    CONSTRAINT FK_Employees_Manager FOREIGN KEY (ManagerId)
        REFERENCES Employees(EmployeeId)
);
CREATE TABLE Projects (
    ProjectId   INT PRIMARY KEY IDENTITY(1,1),
    ProjectName NVARCHAR(100) NOT NULL,
    StartDate   DATE NOT NULL
);
CREATE TABLE EmployeeProjects (
    EmployeeId INT NOT NULL,
    ProjectId  INT NOT NULL,
    Hours      INT NOT NULL DEFAULT 0,
    CONSTRAINT PK_EmployeeProjects PRIMARY KEY (EmployeeId, ProjectId),
    CONSTRAINT FK_EP_Employees FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId),
    CONSTRAINT FK_EP_Projects  FOREIGN KEY (ProjectId)  REFERENCES Projects(ProjectId)
);
GO

```

Проверка схемы:

```sql
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
```

### Результат

```text
TABLE_NAME
----------
Departments
EmployeeProjects
Employees
Projects
ForeignKeyName SourceTable SourceColumn TargetTable TargetColumn
-------------- ----------- ------------ ----------- ------------
FK_EP_Employees EmployeeProjects EmployeeId Employees EmployeeId
FK_EP_Projects EmployeeProjects ProjectId Projects ProjectId
FK_Employees_Departments Employees DepartmentId Departments DepartmentId
FK_Employees_Manager Employees ManagerId Employees EmployeeId
TableName PrimaryKeyName
--------- --------------
Departments PK__Departme__B2079BED14BE24F6
EmployeeProjects PK_EmployeeProjects
Employees PK__Employee__7AD04F11301DD124
Projects PK__Projects__761ABEF086B5BD10
TABLE_NAME COLUMN_NAME DATA_TYPE IS_NULLABLE
---------- ----------- --------- -----------
Departments DepartmentId int NO
Departments DepartmentName nvarchar NO
EmployeeProjects EmployeeId int NO
EmployeeProjects ProjectId int NO
EmployeeProjects Hours int NO
Employees EmployeeId int NO
Employees EmployeeName nvarchar NO
Employees Salary decimal NO
Employees HireDate date NO
Employees DepartmentId int NO
Employees ManagerId int YES
Projects ProjectId int NO
Projects ProjectName nvarchar NO
Projects StartDate date NO
```

### Вывод

Проверены 4 таблицы, 4 первичных и 4 внешних ключей, включая связь руководителя с Employees. Типы и обязательность столбцов соответствуют схеме.

## Задание 2. Заполнение данными

### SQL

Заполнение (из init.sql; выполняется один раз после создания таблиц):

```sql
INSERT INTO Departments (DepartmentName) VALUES
    (N'Разработка'),
    (N'Аналитика'),
    (N'Тестирование'),
    (N'Менеджмент');
GO
INSERT INTO Employees (EmployeeName, Salary, HireDate, DepartmentId, ManagerId) VALUES
    (N'Анна Иванова',    65000, '2020-03-15', 1, NULL),
    (N'Пётр Сидоров',    72000, '2019-07-01', 1, NULL),
    (N'Мария Кузнецова', 55000, '2021-11-20', 1, NULL),
    (N'Иван Петров',     48000, '2023-02-10', 1, NULL),
    (N'Ольга Смирнова',  61000, '2020-09-05', 2, NULL),
    (N'Дмитрий Орлов',   58000, '2022-04-18', 2, NULL),
    (N'Елена Волкова',   52000, '2023-06-01', 2, NULL),
    (N'Сергей Морозов',  45000, '2021-12-12', 3, NULL),
    (N'Наталья Соколова',43000, '2024-01-15', 3, NULL),
    (N'Алексей Лебедев', 90000, '2018-05-25', 4, NULL);
GO
UPDATE Employees SET ManagerId = 1  WHERE EmployeeId = 4;
UPDATE Employees SET ManagerId = 2  WHERE EmployeeId = 3;
UPDATE Employees SET ManagerId = 5  WHERE EmployeeId = 6;
UPDATE Employees SET ManagerId = 10 WHERE EmployeeId = 9;
GO
INSERT INTO Projects (ProjectName, StartDate) VALUES
    (N'CRM System',      '2023-01-10'),
    (N'Mobile App',      '2023-05-01'),
    (N'Data Warehouse',  '2022-11-20'),
    (N'AI Assistant',    '2024-02-01'),
    (N'Legacy Refactor', '2021-06-15');
GO
INSERT INTO EmployeeProjects (EmployeeId, ProjectId, Hours) VALUES
    (1, 1, 120), (1, 3, 80),
    (2, 1, 150), (2, 4, 60),
    (3, 2, 100), (4, 2, 90),
    (4, 5, 40),  (5, 3, 110),
    (6, 3, 70),  (6, 4, 50),
    (7, 2, 95),  (8, 5, 130),
    (10, 4, 200),(10, 1, 30);
GO


```

Проверка количества строк:

```sql
SELECT N'Departments' AS TableName, COUNT(*) AS [RowCount] FROM dbo.Departments
UNION ALL SELECT N'Employees', COUNT(*) FROM dbo.Employees
UNION ALL SELECT N'Projects', COUNT(*) FROM dbo.Projects
UNION ALL SELECT N'EmployeeProjects', COUNT(*) FROM dbo.EmployeeProjects;
```

### Результат

```text
TableName RowCount
--------- --------
Departments 4
Employees 10
Projects 5
EmployeeProjects 14
```

### Вывод

Загружены 4 отдела, 10 сотрудников, 5 проектов и 14 назначений. Все INSERT находятся в init.sql; повторно отдельно вставлять их не нужно.

## Задание 3. SELECT и WHERE

### SQL

```sql
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
```

### Результат

```text
EmployeeId EmployeeName Salary
---------- ------------ ------
1 Анна Иванова 65000.00
2 Пётр Сидоров 72000.00
3 Мария Кузнецова 55000.00
5 Ольга Смирнова 61000.00
6 Дмитрий Орлов 58000.00
7 Елена Волкова 52000.00
10 Алексей Лебедев 90000.00
EmployeeId EmployeeName HireDate
---------- ------------ --------
4 Иван Петров 2023-02-10
7 Елена Волкова 2023-06-01
EmployeeId EmployeeName
---------- ------------
1 Анна Иванова
10 Алексей Лебедев
EmployeeId EmployeeName DepartmentId
---------- ------------ ------------
5 Ольга Смирнова 2
6 Дмитрий Орлов 2
7 Елена Волкова 2
EmployeeId EmployeeName Salary
---------- ------------ ------
3 Мария Кузнецова 55000.00
4 Иван Петров 48000.00
6 Дмитрий Орлов 58000.00
7 Елена Волкова 52000.00
8 Сергей Морозов 45000.00
9 Наталья Соколова 43000.00
```

### Вывод

Пять выборок возвращают соответственно 7, 3, 2, 3 и 6 строк. BETWEEN включает границы; префикс N сохраняет кириллицу.

## Задание 4. ORDER BY и TOP

### SQL

```sql
SELECT TOP (3) EmployeeName, Salary FROM dbo.Employees
ORDER BY Salary DESC, EmployeeId;
SELECT TOP (5) EmployeeName, HireDate FROM dbo.Employees
ORDER BY HireDate, EmployeeId;
SELECT EmployeeName, DepartmentId, Salary FROM dbo.Employees
ORDER BY DepartmentId, Salary DESC, EmployeeId;
```

### Результат

```text
EmployeeName Salary
------------ ------
Алексей Лебедев 90000.00
Пётр Сидоров 72000.00
Анна Иванова 65000.00
EmployeeName HireDate
------------ --------
Алексей Лебедев 2018-05-25
Пётр Сидоров 2019-07-01
Анна Иванова 2020-03-15
Ольга Смирнова 2020-09-05
Мария Кузнецова 2021-11-20
EmployeeName DepartmentId Salary
------------ ------------ ------
Пётр Сидоров 1 72000.00
Анна Иванова 1 65000.00
Мария Кузнецова 1 55000.00
Иван Петров 1 48000.00
Ольга Смирнова 2 61000.00
Дмитрий Орлов 2 58000.00
Елена Волкова 2 52000.00
Сергей Морозов 3 45000.00
Наталья Соколова 3 43000.00
Алексей Лебедев 4 90000.00
```

### Вывод

Топ зарплат: 90000, 72000, 65000. Самый ранний найм — Алексей Лебедев (2018-05-25). EmployeeId делает порядок при равных значениях однозначным.

## Задание 5. Агрегатные функции

### SQL

```sql
SELECT COUNT(*) AS EmployeeCount,
       SUM(Salary) AS TotalSalary,
       CAST(ROUND(AVG(Salary), 2) AS DECIMAL(10,2)) AS AverageSalary,
       MIN(Salary) AS MinSalary,
       MAX(Salary) AS MaxSalary
FROM dbo.Employees;
SELECT COUNT(*) AS EmployeesAbove50000
FROM dbo.Employees WHERE Salary > 50000;
```

### Результат

```text
EmployeeCount TotalSalary AverageSalary MinSalary MaxSalary
------------- ----------- ------------- --------- ---------
10 589000.00 58900.00 43000.00 90000.00
EmployeesAbove50000
-------------------
7
```

### Вывод

10 сотрудников; сумма 589000.00; средняя 58900.00; минимум 43000.00; максимум 90000.00; выше 50000 — 7 сотрудников. COUNT на пустой таблице даёт 0, прочие агрегаты — NULL.

## Задание 6. GROUP BY

### SQL

```sql
SELECT DepartmentId, COUNT(*) AS EmployeeCount,
       CAST(ROUND(AVG(Salary), 2) AS DECIMAL(10,2)) AS AverageSalary,
       SUM(Salary) AS TotalSalary
FROM dbo.Employees
GROUP BY DepartmentId
ORDER BY AverageSalary DESC, DepartmentId;
```

### Результат

```text
DepartmentId EmployeeCount AverageSalary TotalSalary
------------ ------------- ------------- -----------
4 1 90000.00 90000.00
1 4 60000.00 240000.00
2 3 57000.00 171000.00
3 2 44000.00 88000.00
```

### Вывод

Порядок отделов: 4, 1, 2, 3. Средние зарплаты: 90000.00, 60000.00, 57000.00, 44000.00; суммы: 90000.00, 240000.00, 171000.00, 88000.00.

## Задание 7. GROUP BY и HAVING

### SQL

```sql
SELECT DepartmentId, COUNT(*) AS EmployeeCount FROM dbo.Employees
GROUP BY DepartmentId HAVING COUNT(*) > 2 ORDER BY DepartmentId;
SELECT DepartmentId,
       CAST(ROUND(AVG(Salary), 2) AS DECIMAL(10,2)) AS AverageSalary
FROM dbo.Employees
GROUP BY DepartmentId HAVING AVG(Salary) > 55000 ORDER BY DepartmentId;
SELECT DepartmentId, SUM(Salary) AS TotalSalary FROM dbo.Employees
GROUP BY DepartmentId HAVING SUM(Salary) > 150000 ORDER BY DepartmentId;
```

### Результат

```text
DepartmentId EmployeeCount
------------ -------------
1 4
2 3
DepartmentId AverageSalary
------------ -------------
1 60000.00
2 57000.00
4 90000.00
DepartmentId TotalSalary
------------ -----------
1 240000.00
2 171000.00
```

### Вывод

Больше 2 сотрудников — отделы 1 и 2; средняя выше 55000 — 1, 2, 4; сумма выше 150000 — 1 и 2. HAVING фильтрует уже сформированные группы.

## Задание 8. INNER JOIN

### SQL

```sql
SELECT e.EmployeeName, d.DepartmentName, e.Salary
FROM dbo.Employees AS e
INNER JOIN dbo.Departments AS d ON d.DepartmentId = e.DepartmentId
ORDER BY d.DepartmentName, e.Salary DESC, e.EmployeeId;
```

### Результат

```text
EmployeeName DepartmentName Salary
------------ -------------- ------
Ольга Смирнова Аналитика 61000.00
Дмитрий Орлов Аналитика 58000.00
Елена Волкова Аналитика 52000.00
Алексей Лебедев Менеджмент 90000.00
Пётр Сидоров Разработка 72000.00
Анна Иванова Разработка 65000.00
Мария Кузнецова Разработка 55000.00
Иван Петров Разработка 48000.00
Сергей Морозов Тестирование 45000.00
Наталья Соколова Тестирование 43000.00
```

### Вывод

Получены 10 сотрудников с названиями отделов. INNER JOIN возвращает строки, у которых есть соответствующий отдел; внешний ключ и NOT NULL обеспечивают это для всех сотрудников.

## Задание 9. LEFT JOIN: сотрудники без проектов

### SQL

```sql
SELECT e.EmployeeId, e.EmployeeName
FROM dbo.Employees AS e
LEFT JOIN dbo.EmployeeProjects AS ep ON ep.EmployeeId = e.EmployeeId
WHERE ep.EmployeeId IS NULL
ORDER BY e.EmployeeId;
```

### Результат

```text
EmployeeId EmployeeName
---------- ------------
9 Наталья Соколова
```

### Вывод

Без проектов только Наталья Соколова, EmployeeId = 9. Проверяется NULL в ключе правой таблицы: он появляется при отсутствии назначения.

## Задание 10. Соединение четырёх таблиц

### SQL

```sql
SELECT e.EmployeeName, d.DepartmentName, p.ProjectName, ep.Hours
FROM dbo.Employees AS e
INNER JOIN dbo.Departments AS d ON d.DepartmentId = e.DepartmentId
INNER JOIN dbo.EmployeeProjects AS ep ON ep.EmployeeId = e.EmployeeId
INNER JOIN dbo.Projects AS p ON p.ProjectId = ep.ProjectId
ORDER BY d.DepartmentName, e.EmployeeName, p.ProjectName, e.EmployeeId, p.ProjectId;
```

### Результат

```text
EmployeeName DepartmentName ProjectName Hours
------------ -------------- ----------- -----
Дмитрий Орлов Аналитика AI Assistant 50
Дмитрий Орлов Аналитика Data Warehouse 70
Елена Волкова Аналитика Mobile App 95
Ольга Смирнова Аналитика Data Warehouse 110
Алексей Лебедев Менеджмент AI Assistant 200
Алексей Лебедев Менеджмент CRM System 30
Анна Иванова Разработка CRM System 120
Анна Иванова Разработка Data Warehouse 80
Иван Петров Разработка Legacy Refactor 40
Иван Петров Разработка Mobile App 90
Мария Кузнецова Разработка Mobile App 100
Пётр Сидоров Разработка AI Assistant 60
Пётр Сидоров Разработка CRM System 150
Сергей Морозов Тестирование Legacy Refactor 130
```

### Вывод

Получены 14 строк — по одной на назначение, с сотрудником, отделом, проектом и часами. Сотрудник без проекта не входит в отчёт назначений.
