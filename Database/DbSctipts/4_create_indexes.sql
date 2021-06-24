CREATE NONCLUSTERED INDEX IX_Calls_ContractAddress_Method
    ON [dbo].[Calls]([To], MethodId);

CREATE NONCLUSTERED INDEX IX_Calls_ContractAddress_Method_Error
    ON [dbo].[Calls]([To], MethodId, Error);

CREATE NONCLUSTERED INDEX IX_Calls_ContractAddress
    ON [dbo].[Calls]([To]);

CREATE NONCLUSTERED INDEX IX_Calls_TransactionHash
    ON [dbo].[Calls](TransactionHash);

CREATE NONCLUSTERED INDEX IX_Transactions_Hash
    ON [dbo].[Transactions]([TransactionHash]);