create table [Transactions](
	[TransactionHash] NVARCHAR(66) NOT NULL PRIMARY KEY , 

	[BlockId] BIGINT NOT NULL FOREIGN KEY REFERENCES Blocks(BlockNumber),

	[Time] DATETIME NULL,
);

create table [Calls] (
	[CallId] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,

	[TransactionHash] NVARCHAR(66) NOT NULL FOREIGN KEY REFERENCES Transactions([TransactionHash]),

	[Error] NVARCHAR(500) NULL, 
	[Type] NVARCHAR(20) NULL,

	[From] NVARCHAR(42) NOT NULL,

	[To] NVARCHAR(42) NULL,

	[Time] NVARCHAR(20) NULL,

	[MethodId] NVARCHAR(10) NOT NULL,
);
