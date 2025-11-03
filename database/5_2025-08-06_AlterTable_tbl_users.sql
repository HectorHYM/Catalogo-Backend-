---------------------------------------------------------------------
-- Fecha: 2025-08-06
-- Descripcion: Cambio de nombre del campo 'role' a 'roleID', 
--				 'password_hash' pasa a ser nulleable.
---------------------------------------------------------------------

BEGIN;

ALTER TABLE catalog.tbl_users RENAME COLUMN "role" TO "roleID";

ALTER TABLE catalog.tbl_users ALTER COLUMN "password_hash" DROP NOT NULL;

COMMIT;