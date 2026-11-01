# Производственная практика

Учебные задания по программированию на C#.

Все задания находятся в отдельных папках.
## Базы данных — базовый уровень (1–10)

Решения и фактические результаты: [database/sql-notes.md](database/sql-notes.md).

Создание базы: [database/init.sql](database/init.sql). Запросы: [database/level-1.sql](database/level-1.sql).

В SSMS выполните сначала init.sql, затем level-1.sql. Внимание: init.sql удаляет существующую Practice.db и создаёт её заново. level-1.sql только читает данные.

Проверено на SQL Server 2022. Журналы выполнения сохранены в database/results.txt и database/init-result.txt.

## Базы данных — средний уровень (11–20)

Запросы: [database/level-2.sql](database/level-2.sql).
SQL, результаты и выводы добавлены в [database/sql-notes.md](database/sql-notes.md).
Полный журнал: [database/results-level-2.txt](database/results-level-2.txt).

Выполнить level-2.sql в SSMS на базе Practice.db после init.sql.
Изменения заданий 16–18 после демонстрации восстанавливаются; исходные 10 сотрудников и 14 назначений сохраняются.
В задании 20 сохранены фактические текстовые планы и показатели STATISTICS IO.
После выполнения остаются два индекса, а представление задания 19 удаляется.

## Базы данных — продвинутый уровень (21–30)

- [SQL-задания 21–27](database/level-3.sql).
- [Консольное приложение C# для заданий 28–30](database/PracticeConsole/README.md).
- [Запросы, результаты и выводы](database/sql-notes.md).
- Журналы: database/results-level-3.txt и database/results-console.txt.

Все задания проверены на Practice.db. Приложение собрано без предупреждений и ошибок.
Для демонстрации C# потребуется задать PRACTICE_DB_CONNECTION в окружении и выполнить команду из инструкции проекта.

## C# — уровень 2 (задания 16–35)

Отдельный проект для каждого задания. Task2.1 соответствует заданию 16; Task2.20 — заданию 35.

| Работа | Задание | Тема |
| --- | --- | --- |
| [Task2.1](Task2.1/README.md) | 16 | Методы расширения строк |
| [Task2.2](Task2.2/README.md) | 17 | StringBuilder и конкатенация |
| [Task2.3](Task2.3/README.md) | 18 | TryParse и try/catch |
| [Task2.4](Task2.4/README.md) | 19 | Стек и очередь на массиве |
| [Task2.5](Task2.5/README.md) | 20 | Обобщённые методы |
| [Task2.6](Task2.6/README.md) | 21 | Дни недели |
| [Task2.7](Task2.7/README.md) | 22 | Сумма с params |
| [Task2.8](Task2.8/README.md) | 23 | Nullable-типы |
| [Task2.9](Task2.9/README.md) | 24 | Иерархия животных |
| [Task2.10](Task2.10/README.md) | 25 | Фигуры и сортировка |
| [Task2.11](Task2.11/README.md) | 26 | Стратегии скидок |
| [Task2.12](Task2.12/README.md) | 27 | Сортировка студентов |
| [Task2.13](Task2.13/README.md) | 28 | Равенство точек |
| [Task2.14](Task2.14/README.md) | 29 | Фабрика уведомлений |
| [Task2.15](Task2.15/README.md) | 30 | Банковский счёт |
| [Task2.16](Task2.16/README.md) | 31 | IDisposable и using |
| [Task2.17](Task2.17/README.md) | 32 | Температура |
| [Task2.18](Task2.18/README.md) | 33 | LINQ по сотрудникам |
| [Task2.19](Task2.19/README.md) | 34 | Группировка слов |
| [Task2.20](Task2.20/README.md) | 35 | Git, Pull Request и rebase |

Все программы входят в Level2.sln и используют .NET 9.

Сборка:

    dotnet build Level2.sln -c Release

Запуск, например:

    dotnet run --project Task2.1

В папке каждого проекта есть result.txt с фактическим выводом.
Все 19 проектов собраны без ошибок и предупреждений, демонстрационные сценарии выполнены.

В задании 22 исправлена несовместимость условия с C#: params int[] и int[] имеют одинаковую сигнатуру. Метод с params обслуживает все требуемые вызовы; отдельная перегрузка принимает IEnumerable<int>. Подробности — в Task2.7.
