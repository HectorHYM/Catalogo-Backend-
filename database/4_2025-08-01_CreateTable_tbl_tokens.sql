CREATE TABLE catalog.tbl_tokens (
    id UUID PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES catalog.tbl_users(id) ON DELETE CASCADE,
    token VARCHAR(255) NOT NULL UNIQUE,
    type VARCHAR(40) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    expires_at TIMESTAMP WITH TIME ZONE NOT NULL,
    used_at TIMESTAMP WITH TIME ZONE NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT chk_type CHECK (type IN ('access', 'recover')),
    CONSTRAINT chk_expires CHECK (expires_at > created_at)
);
CREATE INDEX idx_tbl_tokens_userid ON catalog.tbl_tokens(user_id);
CREATE INDEX idx_tbl_tokens_token ON catalog.tbl_tokens(token);