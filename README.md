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
