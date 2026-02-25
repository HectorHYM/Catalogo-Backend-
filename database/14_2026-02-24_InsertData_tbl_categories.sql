INSERT INTO catalog.tbl_categories (name)
VALUES 
	('electronics'),
	('home'),
	('clothes'),
	('sports'),
	('beauty'),
	('games'),
	('toys'),
	('healthy'),
	('automotive'),
	('books'),
	('yard'),
	('tools'),
	('pets'),
	('children'),
	('jewelry'),
	('others')
RETURNING id, name;