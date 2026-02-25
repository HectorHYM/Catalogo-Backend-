ALTER TABLE catalog.tbl_categories
DROP CONSTRAINT chk_name;

ALTER TABLE catalog.tbl_categories
ADD CONSTRAINT chk_name
CHECK (
    name IN (
        'electronics',
        'home',
        'clothes',
        'sports',
        'beauty',
        'games',
        'toys',
        'healthy',
        'automotive',
        'books',
        'yard',
        'tools',
        'pets',
        'children',
        'jewelry',
        'others'
    )
);