--* CREACIÓN DE ESQUEMA PARA LA APLICACIÓN DE CATALOGO

--? Se requiere la extensión pgcrypto para generar UUIDs
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

--? Creación del esquema lógico 'catalog'
CREATE SCHEMA IF NOT EXISTS catalog;