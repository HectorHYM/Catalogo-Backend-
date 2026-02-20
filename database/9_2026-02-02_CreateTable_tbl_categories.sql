--- Creación de columna "category_id" en la tabla de productos.
ALTER TABLE catalog.tbl_products
ADD COLUMN category_id UUID NOT NULL
DEFAULT '00000000-0000-0000-0000-000000000000';

--- Creación de entidad para las categorias de los productos.
CREATE TABLE catalog.tbl_categories (
	id UUID NOT NULL,
	name VARCHAR(100) NOT NULL DEFAULT 'others',

	CONSTRAINT "PK_tbl_categories" PRIMARY KEY (id),

	CONSTRAINT chk_name CHECK (
		name IN (
			'all', 'electronics', 'home', 'clothes', 'sports',
            'beauty', 'games', 'toys', 'healthy', 'automotive',
            'books', 'yard', 'tools', 'pets',
            'children', 'jewelry', 'others'
		)
	)
);

--- Creación de index en la columna "category_id" de la entidad de productos.
CREATE INDEX IX_tbl_products_category_id
ON catalog.tbl_products (category_id);

--- Creación de llave foránea para "category_id" en la entidad de productos haciendo referencia al "id" de la entidad de categorias. 
ALTER TABLE catalog.tbl_products
ADD CONSTRAINT tbl_products_category_fkey
FOREIGN KEY (category_id)
REFERENCES catalog.tbl_categories(id)
ON DELETE RESTRICT;