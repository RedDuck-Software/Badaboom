create table [Transactions](
	TransactionId INT NOT NULL IDENTITY(1,1) PRIMARY KEY , 
	Hash NVARCHAR(66) NOT NULL UNIQUE,	
	Time DATETIME NULL,
);

create table [Calls] (
	CallId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	TransactionId INT NOT NULL FOREIGN KEY REFERENCES Transactions(TransactionId),

	Error NVARCHAR(500) NULL, 
	[Type] NVARCHAR(20) NULL,

	[From] NVARCHAR(42) NOT NULL,

	ContractAddress NVARCHAR(42) NULL,

	[Value] NVARCHAR(66) NULL,

	MethodId NVARCHAR(10) NOT NULL,
);
