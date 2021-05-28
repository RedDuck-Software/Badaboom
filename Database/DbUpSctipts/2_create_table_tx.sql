create table [Transactions](
	TransactionId INT NOT NULL IDENTITY(1,1) PRIMARY KEY , 
	Hash NVARCHAR(66) NOT NULL UNIQUE,	
	Time DATETIME NULL,
);

create table [Calls] (
	CallId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	TransactionId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Transactions(TransactionId),

	ContractAddress NVARCHAR(42) NOT NULL,
	MethodId NVARCHAR(10) NOT NULL,
);

CREATE NONCLUSTERED INDEX IX_Transactions_ContractAddress_Method
    ON [dbo].[Calls](ContractAddress, MethodId);
