create table [Transactions](
	[TransactionHash] VARCHAR(64) NOT NULL PRIMARY KEY , 

	[BlockId] BIGINT NOT NULL FOREIGN KEY REFERENCES Blocks(BlockNumber),

	[TimeStamp] INT NULL,
);

create table [Calls] (
	[CallId] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,

	[TransactionHash] VARCHAR(64) NOT NULL FOREIGN KEY REFERENCES Transactions([TransactionHash]),

	[Error] VARCHAR(50) NULL, 

	[Type] tinyint NOT NULL default(0),

	[From] VARCHAR(40) NOT NULL,

	[To] VARCHAR(40) NULL,

	[MethodId] VARCHAR(8) NOT NULL,
);
