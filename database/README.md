# Scripts de Base de Datos - PostgreSQL

Todos los scripts deben seguir el siguiente formato de nombre:

`N_YYYY-MM-DD_Acción_Entidad.sql`

## Orden de Ejecución

1. Crear esquema y extensiones → `1_2025-07-17_CreateSchema_App.sql`
2. Crear tabla de usuarios → `2_2025-07-17_CreateTable_tbl_users.sql`
3. Crear tabla de productos → `3_2025-07-17_CreateTable_tbl_products.sql`

## Convenciones

- Todas las tablas tienen prefijo `tbl_`
- Soft delete: `is_active`, `deleted_at`
- Trazabilidad: `created_by`, `updated_by`, `created_at`, `updated_at`
- FK entre `tbl_products` y `tbl_users`
