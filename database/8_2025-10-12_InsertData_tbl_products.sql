INSERT INTO catalog.tbl_products (name, price, stock, is_active, category_id)
VALUES 
	('Lentes', 288, 1, true, 16),
	('Sueter', 499, 1, true, 16),
	('Taza', 50, 1, true, 16)
RETURNING id, name;