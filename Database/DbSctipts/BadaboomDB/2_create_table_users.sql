create table [Users](
	[UserId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,

	[Address] binary(20) NOT NULL UNIQUE, 

	[Nonce] varchar(12) NOT NULL
);

create table [RefreshTokens] (
	[TokenId] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,

	[UserId] INT NOT NULL FOREIGN KEY REFERENCES [Users]([UserId]),

	[Token] varchar(256) NOT NULL UNIQUE,

	[Expires] DateTime NOT NULL,
	
	[Created] DateTime NOT NULL,
	
	[CreatedByIp] varchar(25) NULL,
);
