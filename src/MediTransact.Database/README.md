# MediTransact.Database

This project contains PostgreSQL DDL scripts for creating the MediTransact schema independent of EF runtime initialization.

## Apply schema

From a PostgreSQL client:

```sql
\i Scripts/001_create_tables.sql
```

Or with `psql`:

```bash
psql "$CONNECTION_STRING" -f src/MediTransact.Database/Scripts/001_create_tables.sql
```
