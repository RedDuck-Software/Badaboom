create table [Users](
	[UserId] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,

	[Address] binary(20) NOT NULL UNIQUE, 

	[Nonce] varchar(12) NOT NULL
);

create table [RefreshTokens] (
	[TokenId] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,

	[UserId] BIGINT NOT NULL FOREIGN KEY REFERENCES [Users]([UserId]),

	[Token] varchar(256) NOT NULL UNIQUE,

	[Expires] DateTime NOT NULL,
	
	[Created] DateTime NOT NULL,

	[Revoked] DateTime NULL,

	
	[CreatedByIp] varchar(15) NULL,

	[RevokedByIp] varchar(15) NULL
);
