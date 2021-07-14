create table [Transactions](
	[TransactionHash] binary(32) NOT NULL PRIMARY KEY , 

	[BlockId] int NOT NULL FOREIGN KEY REFERENCES Blocks(BlockNumber),

	[TimeStamp] INT NULL,

	[GasPrice] binary(8) NULL,
);

create table [Calls] (
	[CallId] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY,

	[TransactionHash] binary(32) NOT NULL FOREIGN KEY REFERENCES Transactions([TransactionHash]),

	[Error] VARCHAR(50) NULL, 

	[Type] tinyint NOT NULL default(0),

	[From] binary(20) NOT NULL,

	[To] binary(20) NULL,

	[MethodId] binary(4) NOT NULL,
	
	[Input] varbinary(MAX) NOT NULL,

	[GasUsed] binary(8) NOT NULL,

	[GasSended] binary(8) NOT NULL,

	[Value] binary(32) NOT NULL,
);
