create table [Transactions](
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY , 
	ContractAddress NVARCHAR(42) NOT NULL,
	Hash NVARCHAR(66) NOT NULL UNIQUE,	
	MethodId NVARCHAR(10) NOT NULL, 
	Time DATETIME NOT NULL,
);

CREATE NONCLUSTERED INDEX IX_Transactions_ContractAddress_Method
    ON [dbo].[Transactions] (ContractAddress, MethodId);
