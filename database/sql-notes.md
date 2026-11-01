# Практикум по базам данных

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


## Уровень 2 — задания 11–20

Запросы выполнены на Practice.db через sqlcmd. Скрипт: level-2.sql. В заданиях 16–17 изменения откатываются; в задании 18 после проверки COMMIT зарплата восстанавливается отдельным UPDATE. После задания 19 представление удалено, индексы задания 20 сохранены.

## Задание 11. Руководители сотрудников

### SQL

```sql
USE [Practice.db];
GO
SELECT e.EmployeeName, m.EmployeeName AS ManagerName
FROM dbo.Employees e
LEFT JOIN dbo.Employees m ON m.EmployeeId = e.ManagerId
ORDER BY e.EmployeeId;
```

### Результат

```text
EmployeeName ManagerName
------------ -----------
Анна Иванова NULL
Пётр Сидоров NULL
Мария Кузнецова Пётр Сидоров
Иван Петров Анна Иванова
Ольга Смирнова NULL
Дмитрий Орлов Ольга Смирнова
Елена Волкова NULL
Сергей Морозов NULL
Наталья Соколова Алексей Лебедев
Алексей Лебедев NULL
```

### Вывод

У 4 сотрудников указан руководитель, у остальных 6 — NULL. LEFT JOIN сохраняет сотрудников без руководителя.

## Задание 12. Подзапросы в WHERE

### SQL

```sql
USE [Practice.db];
GO
SELECT EmployeeName, Salary FROM dbo.Employees
WHERE Salary > (SELECT AVG(Salary) FROM dbo.Employees) ORDER BY EmployeeId;
SELECT EmployeeName, Salary FROM dbo.Employees
WHERE Salary = (SELECT MAX(Salary) FROM dbo.Employees) ORDER BY EmployeeId;
```

### Результат

```text
EmployeeName Salary
------------ ------
Анна Иванова 65000.00
Пётр Сидоров 72000.00
Ольга Смирнова 61000.00
Алексей Лебедев 90000.00
EmployeeName Salary
------------ ------
Алексей Лебедев 90000.00
```

### Вывод

Выше средней зарплаты 58900 — 4 сотрудника. Максимальная зарплата 90000 у Алексея Лебедева; при равенстве максимума запрос вернул бы всех таких сотрудников.

## Задание 13. Отклонение зарплаты от средней

### SQL

```sql
USE [Practice.db];
GO
SELECT e.EmployeeName, e.Salary, e.DepartmentId,
       CAST(ROUND((SELECT AVG(d.Salary) FROM dbo.Employees d
                   WHERE d.DepartmentId = e.DepartmentId),2) AS DECIMAL(10,2)) AS DepartmentAverage,
       CAST(ROUND(e.Salary - (SELECT AVG(d.Salary) FROM dbo.Employees d
                   WHERE d.DepartmentId = e.DepartmentId),2) AS DECIMAL(10,2)) AS Difference
FROM dbo.Employees e ORDER BY e.EmployeeId;
```

### Результат

```text
EmployeeName Salary DepartmentId DepartmentAverage Difference
------------ ------ ------------ ----------------- ----------
Анна Иванова 65000.00 1 60000.00 5000.00
Пётр Сидоров 72000.00 1 60000.00 12000.00
Мария Кузнецова 55000.00 1 60000.00 -5000.00
Иван Петров 48000.00 1 60000.00 -12000.00
Ольга Смирнова 61000.00 2 57000.00 4000.00
Дмитрий Орлов 58000.00 2 57000.00 1000.00
Елена Волкова 52000.00 2 57000.00 -5000.00
Сергей Морозов 45000.00 3 44000.00 1000.00
Наталья Соколова 43000.00 3 44000.00 -1000.00
Алексей Лебедев 90000.00 4 90000.00 .00
```

### Вывод

Средние зарплаты отделов: 60000, 57000, 44000 и 90000. Положительное отклонение означает зарплату выше средней по своему отделу.

## Задание 14. EXISTS и NOT EXISTS

### SQL

```sql
USE [Practice.db];
GO
SELECT e.EmployeeId, e.EmployeeName FROM dbo.Employees e
WHERE EXISTS (SELECT 1 FROM dbo.EmployeeProjects ep WHERE ep.EmployeeId = e.EmployeeId)
ORDER BY e.EmployeeId;
SELECT e.EmployeeId, e.EmployeeName FROM dbo.Employees e
WHERE NOT EXISTS (SELECT 1 FROM dbo.EmployeeProjects ep WHERE ep.EmployeeId = e.EmployeeId)
ORDER BY e.EmployeeId;
```

### Результат

```text
EmployeeId EmployeeName
---------- ------------
1 Анна Иванова
2 Пётр Сидоров
3 Мария Кузнецова
4 Иван Петров
5 Ольга Смирнова
6 Дмитрий Орлов
7 Елена Волкова
8 Сергей Морозов
10 Алексей Лебедев
EmployeeId EmployeeName
---------- ------------
9 Наталья Соколова
```

### Вывод

Проекты есть у 9 сотрудников; нет проектов у Натальи Соколовой (9), как в задании 9. В условии есть неточность: задание 8 выводит всех 10 сотрудников с отделами, а не только участников проектов.

## Задание 15. UNION и UNION ALL

### SQL

```sql
USE [Practice.db];
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
```

### Результат

```text
EmployeeId EmployeeName
---------- ------------
1 Анна Иванова
2 Пётр Сидоров
3 Мария Кузнецова
5 Ольга Смирнова
8 Сергей Морозов
10 Алексей Лебедев
EmployeeId EmployeeName
---------- ------------
1 Анна Иванова
1 Анна Иванова
2 Пётр Сидоров
2 Пётр Сидоров
3 Мария Кузнецова
5 Ольга Смирнова
5 Ольга Смирнова
8 Сергей Морозов
10 Алексей Лебедев
10 Алексей Лебедев
UnionCount
----------
6
UnionAllCount
-------------
10
```

### Вывод

UNION возвращает 6 строк, UNION ALL — 10. Четыре сотрудника входят в обе выборки и повторяются при UNION ALL.

## Задание 16. Изменение зарплат и даты найма

### SQL

```sql
USE [Practice.db];
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
```

### Результат

```text
EmployeeId Salary
---------- ------
1 65000.00
2 72000.00
3 55000.00
4 48000.00
5 61000.00
6 58000.00
7 52000.00
8 45000.00
9 43000.00
10 90000.00
EmployeeId Salary
---------- ------
1 71500.00
2 79200.00
3 60500.00
4 52800.00
5 61000.00
6 58000.00
7 52000.00
8 45000.00
9 43000.00
10 90000.00
EmployeeId HireDate Salary
---------- -------- ------
1 2020-03-15 71500.00
2 2019-07-01 79200.00
3 2021-11-20 60500.00
4 2023-02-10 52800.00
5 2020-09-05 61000.00
6 2022-04-18 58000.00
7 2023-06-01 52000.00
8 2021-12-12 45000.00
9 2024-01-15 43000.00
10 2018-05-25 90000.00
EmployeeId HireDate Salary
---------- -------- ------
1 2020-03-15 76500.00
2 2019-07-01 84200.00
3 2021-11-20 65500.00
4 2023-02-10 52800.00
5 2020-09-05 66000.00
6 2022-04-18 58000.00
7 2023-06-01 52000.00
8 2021-12-12 50000.00
9 2024-01-15 43000.00
10 2018-05-25 95000.00
EmployeeId HireDate
---------- --------
4 2023-02-10
EmployeeId HireDate
---------- --------
4 2023-02-11
EmployeeId Salary HireDate
---------- ------ --------
1 65000.00 2020-03-15
2 72000.00 2019-07-01
3 55000.00 2021-11-20
4 48000.00 2023-02-10
5 61000.00 2020-09-05
6 58000.00 2022-04-18
7 52000.00 2023-06-01
8 45000.00 2021-12-12
9 43000.00 2024-01-15
10 90000.00 2018-05-25
```

### Вывод

Зарплаты отдела 1 увеличены на 10%, затем 6 сотрудников с наймом до 2022 года получили ещё 5000. Дата найма сотрудника 4 изменена на 2023-02-11. Проверки показывают изменения целевых строк. В конце ROLLBACK восстанавливает учебные данные.

## Задание 17. Удаление записей

### SQL

```sql
USE [Practice.db];
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
```

### Результат

```text
EmployeeId ProjectId Hours
---------- --------- -----
9 5 0
ZeroHoursAfterDelete
--------------------
0
AssignmentsAfterDelete
----------------------
14
EmployeeId EmployeeName
---------- ------------
8 Сергей Морозов
EmployeeId ProjectId Hours
---------- --------- -----
8 5 130
AssignmentsForEmployee8
-----------------------
0
Employee8AfterDelete
--------------------
0
EmployeesAfterDelete
--------------------
9
CopyBeforeDelete
----------------
9
EmployeeId EmployeeName
---------- ------------
4 Иван Петров
CopyAfterWhere
--------------
8
EmployeeId EmployeeName
---------- ------------
1 Анна Иванова
2 Пётр Сидоров
3 Мария Кузнецова
5 Ольга Смирнова
6 Дмитрий Орлов
7 Елена Волкова
9 Наталья Соколова
10 Алексей Лебедев
CopyAfterDeleteAll
------------------
0
RestoredEmployees
-----------------
10
RestoredAssignments
-------------------
14
```

### Вывод

Удалено тестовое назначение с нулём часов, затем назначение сотрудника 8 и сам сотрудник. На временной копии DELETE с WHERE оставляет 8 строк, без WHERE — 0. После ROLLBACK снова 10 сотрудников и 14 назначений.

## Задание 18. Транзакции

### SQL

```sql
USE [Practice.db];
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
```

### Результат

```text
EmployeeId BeforeTransaction
---------- -----------------
4 48000.00
EmployeeId InsideRollbackTransaction
---------- -------------------------
4 49000.00
EmployeeId AfterRollback
---------- -------------
4 48000.00
EmployeeId InsideCommitTransaction
---------- -----------------------
4 49000.00
EmployeeId AfterCommit
---------- -----------
4 49000.00
EmployeeId BeforeRestore
---------- -------------
4 49000.00
EmployeeId AfterRestore
---------- ------------
4 48000.00
```

### Вывод

Внутри транзакции зарплата 49000; после ROLLBACK — 48000; после COMMIT — 49000. Затем отдельный UPDATE возвращает 48000. Это восстановление после проверки, а не откат уже завершённого COMMIT.

## Задание 19. Представление назначений

### SQL

```sql
USE [Practice.db];
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
ViewCountAfterDrop
------------------
0
```

### Вывод

Представление вернуло 14 назначений. После DROP VIEW проверка показывает 0. Порядок задаётся в SELECT, а не внутри VIEW.

## Задание 20. Индексы и планы выполнения

### SQL

```sql
USE [Practice.db];
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
```

### Результат

```text
Before indexes
EmployeeId DepartmentId Salary
---------- ------------ ------
1 1 65000.00
2 1 72000.00
3 1 55000.00
Таблица "Employees". Сканирований 1, логических операций чтения 2, физических операций чтения 0, операций чтения страничного сервера 0, операций чтения, выполненных с упреждением 0, операций чтения страничного сервера, выполненных с упреждением 0, логических операций чтения LOB 0, физических операций чтения LOB 0, операций чтения LOB страничного сервера 0, операций чтения LOB, выполненных с упреждением 0, операций чтения LOB страничного сервера, выполненных с упреждением 0.
Rows Executes StmtText StmtId NodeId Parent PhysicalOp LogicalOp Argument DefinedValues EstimateRows EstimateIO EstimateCPU AvgRowSize TotalSubtreeCost OutputList Warnings Type Parallel EstimateExecutions
---- -------- -------- ------ ------ ------ ---------- --------- -------- ------------- ------------ ---------- ----------- ---------- ---------------- ---------- -------- ---- -------- ------------------
3 1 SELECT [EmployeeId],[DepartmentId],[Salary] FROM [dbo].[Employees] WHERE [DepartmentId]=@1 AND [Salary]>@2 1 1 0 NULL NULL NULL NULL 3.3466401 NULL NULL NULL 3.2929999E-3 NULL NULL SELECT 0 NULL
3 1   |--Clustered Index Scan(OBJECT:([Practice.db].[dbo].[Employees].[PK__Employee__7AD04F11301DD124]), WHERE:([Practice.db].[dbo].[Employees].[DepartmentId]=CONVERT_IMPLICIT(int,[@1],0) AND [Practice.db].[dbo].[Employees].[Salary]>CONVERT_IMPLICIT(decimal(10,0),[@2],0))) 1 2 1 Clustered Index Scan Clustered Index Scan OBJECT:([Practice.db].[dbo].[Employees].[PK__Employee__7AD04F11301DD124]), WHERE:([Practice.db].[dbo].[Employees].[DepartmentId]=CONVERT_IMPLICIT(int,[@1],0) AND [Practice.db].[dbo].[Employees].[Salary]>CONVERT_IMPLICIT(decimal(10,0),[@2],0)) [Practice.db].[dbo].[Employees].[EmployeeId], [Practice.db].[dbo].[Employees].[Salary], [Practice.db].[dbo].[Employees].[DepartmentId] 3.3466401 0.003125 0.000168 24 3.2929999E-3 [Practice.db].[dbo].[Employees].[EmployeeId], [Practice.db].[dbo].[Employees].[Salary], [Practice.db].[dbo].[Employees].[DepartmentId] NULL PLAN_ROW 0 1.0
After indexes: optimizer choice
EmployeeId DepartmentId Salary
---------- ------------ ------
1 1 65000.00
2 1 72000.00
3 1 55000.00
Таблица "Employees". Сканирований 1, логических операций чтения 2, физических операций чтения 0, операций чтения страничного сервера 0, операций чтения, выполненных с упреждением 0, операций чтения страничного сервера, выполненных с упреждением 0, логических операций чтения LOB 0, физических операций чтения LOB 0, операций чтения LOB страничного сервера 0, операций чтения LOB, выполненных с упреждением 0, операций чтения LOB страничного сервера, выполненных с упреждением 0.
Rows Executes StmtText StmtId NodeId Parent PhysicalOp LogicalOp Argument DefinedValues EstimateRows EstimateIO EstimateCPU AvgRowSize TotalSubtreeCost OutputList Warnings Type Parallel EstimateExecutions
---- -------- -------- ------ ------ ------ ---------- --------- -------- ------------- ------------ ---------- ----------- ---------- ---------------- ---------- -------- ---- -------- ------------------
3 1 SELECT [EmployeeId],[DepartmentId],[Salary] FROM [dbo].[Employees] WHERE [DepartmentId]=@1 AND [Salary]>@2 1 1 0 NULL NULL NULL NULL 3.3466401 NULL NULL NULL 3.2864001E-3 NULL NULL SELECT 0 NULL
3 1   |--Index Seek(OBJECT:([Practice.db].[dbo].[Employees].[IX_Employees_DepartmentId]), SEEK:([Practice.db].[dbo].[Employees].[DepartmentId]=(1)),  WHERE:([Practice.db].[dbo].[Employees].[Salary]>(50000.)) ORDERED FORWARD) 1 2 1 Index Seek Index Seek OBJECT:([Practice.db].[dbo].[Employees].[IX_Employees_DepartmentId]), SEEK:([Practice.db].[dbo].[Employees].[DepartmentId]=(1)),  WHERE:([Practice.db].[dbo].[Employees].[Salary]>(50000.)) ORDERED FORWARD [Practice.db].[dbo].[Employees].[EmployeeId], [Practice.db].[dbo].[Employees].[Salary], [Practice.db].[dbo].[Employees].[DepartmentId] 3.3466401 0.003125 1.6140001E-4 24 3.2864001E-3 [Practice.db].[dbo].[Employees].[EmployeeId], [Practice.db].[dbo].[Employees].[Salary], [Practice.db].[dbo].[Employees].[DepartmentId] NULL PLAN_ROW 0 1.0
After indexes: forced seek for comparison
EmployeeId DepartmentId Salary
---------- ------------ ------
1 1 65000.00
2 1 72000.00
3 1 55000.00
Таблица "Employees". Сканирований 1, логических операций чтения 2, физических операций чтения 0, операций чтения страничного сервера 0, операций чтения, выполненных с упреждением 0, операций чтения страничного сервера, выполненных с упреждением 0, логических операций чтения LOB 0, физических операций чтения LOB 0, операций чтения LOB страничного сервера 0, операций чтения LOB, выполненных с упреждением 0, операций чтения LOB страничного сервера, выполненных с упреждением 0.
Rows Executes StmtText StmtId NodeId Parent PhysicalOp LogicalOp Argument DefinedValues EstimateRows EstimateIO EstimateCPU AvgRowSize TotalSubtreeCost OutputList Warnings Type Parallel EstimateExecutions
---- -------- -------- ------ ------ ------ ---------- --------- -------- ------------- ------------ ---------- ----------- ---------- ---------------- ---------- -------- ---- -------- ------------------
3 1 SELECT EmployeeId, DepartmentId, Salary FROM dbo.Employees WITH (INDEX(IX_Employees_DepartmentId), FORCESEEK)
WHERE DepartmentId = 1 AND Salary > 50000 2 1 0 NULL NULL NULL NULL 3.3466401 NULL NULL NULL 3.2864001E-3 NULL NULL SELECT 0 NULL
3 1   |--Index Seek(OBJECT:([Practice.db].[dbo].[Employees].[IX_Employees_DepartmentId]), SEEK:([Practice.db].[dbo].[Employees].[DepartmentId]=(1)),  WHERE:([Practice.db].[dbo].[Employees].[Salary]>(50000.)) ORDERED FORWARD) 2 2 1 Index Seek Index Seek OBJECT:([Practice.db].[dbo].[Employees].[IX_Employees_DepartmentId]), SEEK:([Practice.db].[dbo].[Employees].[DepartmentId]=(1)),  WHERE:([Practice.db].[dbo].[Employees].[Salary]>(50000.)) ORDERED FORWARD, FORCEDINDEX [Practice.db].[dbo].[Employees].[EmployeeId], [Practice.db].[dbo].[Employees].[Salary], [Practice.db].[dbo].[Employees].[DepartmentId] 3.3466401 0.003125 1.6140001E-4 24 3.2864001E-3 [Practice.db].[dbo].[Employees].[EmployeeId], [Practice.db].[dbo].[Employees].[Salary], [Practice.db].[dbo].[Employees].[DepartmentId] NULL PLAN_ROW 0 1.0
name type_desc
---- ---------
IX_Employees_DepartmentId NONCLUSTERED
IX_Employees_Salary NONCLUSTERED
```

### Вывод

До создания индексов использован Clustered Index Scan. После создания оптимизатор самостоятельно выбрал Index Seek по IX_Employees_DepartmentId. Принудительный поиск приведён отдельным дополнительным запросом. Во всех трёх случаях logical reads = 2: на 10 строках изменения числа чтений нет. INCLUDE(Salary) покрывает запрос, а EmployeeId хранится как ключ кластерного индекса. Обе созданные структуры — NONCLUSTERED. Полные текстовые планы и STATISTICS IO приведены в результате.
