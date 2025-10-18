INSERT INTO catalog.tbl_products (name, price, stock, is_active)
VALUES 
	('Lentes', 288, 1, true),
	('Sueter', 499, 1, true),
	('Taza', 50, 1, true)
RETURNING id, name;