CREATE NONCLUSTERED INDEX IX_Calls_ContractAddress_Method
    ON [dbo].[Calls](ContractAddress, MethodId);

CREATE NONCLUSTERED INDEX IX_Calls_ContractAddress_Method_Error
    ON [dbo].[Calls](ContractAddress, MethodId, Error);

CREATE NONCLUSTERED INDEX IX_Calls_ContractAddress
    ON [dbo].[Calls](ContractAddress);

CREATE NONCLUSTERED INDEX IX_Calls_TransactionId
    ON [dbo].[Calls](TransactionId);

CREATE NONCLUSTERED INDEX IX_Transactions_Hash
    ON [dbo].[Transactions]([Hash]);