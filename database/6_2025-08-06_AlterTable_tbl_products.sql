---------------------------------------------------------------------
-- Fecha: 2025-08-06
-- Descripcion: Cambio de tipo de dato para stock de INT a SMALLINT.
----------------------------------------------------------------------

BEGIN;

ALTER TABLE catalog.tbl_products ALTER COLUMN "stock" TYPE SMALLINT USING stock::SMALLINT; 

COMMIT;