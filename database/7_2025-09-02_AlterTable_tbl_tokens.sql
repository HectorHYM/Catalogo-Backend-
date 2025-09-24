----------------------------------------------------------------------
-- Fecha: 2025-09-02
-- Descripción: Eliminación de columna "token" y creación de columna 
--              "token_hash" con tipo VARCHAR(64) en la tabla de tokens.
----------------------------------------------------------------------

BEGIN;

ALTER TABLE tbl_tokens
    DROP COLUMN IF EXISTS "token",
    ADD COLUMN "token_hash" VARCHAR(64);

COMMIT;