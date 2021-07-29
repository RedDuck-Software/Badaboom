CREATE NONCLUSTERED INDEX IX_Calls_ContractAddress_Method
    ON [dbo].[Calls]([To], MethodId);

CREATE NONCLUSTERED INDEX IX_Calls_ContractAddress
    ON [dbo].[Calls]([To]);

CREATE NONCLUSTERED INDEX IX_Calls_From
    ON [dbo].[Calls]([From]);

CREATE NONCLUSTERED INDEX IX_Calls_TransactionHash
    ON [dbo].[Calls](TransactionHash);
