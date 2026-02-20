CREATE UNIQUE INDEX idx_tbl_users_username
ON catalog.tbl_users (username);

CREATE UNIQUE INDEX idx_tbl_users_email
ON catalog.tbl_users (email);