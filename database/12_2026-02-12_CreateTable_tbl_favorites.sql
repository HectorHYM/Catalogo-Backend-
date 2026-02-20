CREATE TABLE catalog.tbl_favorites (
	user_id UUID NOT NULL,
	product_id UUID NOT NULL,
	created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),

	CONSTRAINT pk_tbl_favorites
	PRIMARY KEY (user_id, product_id)
	
	CONSTRAINT tbl_favorites_user_id_fkey
	FOREIGN KEY user_id
	REFERENCES catalog.tbl_users (id)
	ON DELETE CASCADE,

	CONSTRAINT tbl_favorites_product_id_fkey
	FOREIGN KEY product_id
	REFERENCES catalog.tbl_products (id)
	ON DELETE CASCADE
);

CREATE INDEX idx_tbl_favorites_user_id
ON catalog.tbl_favorites (user_id);

CREATE INDEX idx_tbl_favorites_product_id
ON catalog.tbl_favorites (product_id);