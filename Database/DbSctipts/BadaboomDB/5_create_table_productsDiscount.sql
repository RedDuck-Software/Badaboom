CREATE TABLE [dbo].[ProductsDiscount](
	[Id]		 INT		NOT NULL PRIMARY KEY IDENTITY,

	[ProductId]  INT		NOT NULL FOREIGN KEY REFERENCES [Products]([Id]),

	[AmountFrom] INT		NOT NULL,

	[Percents]	 TINYINT	NOT NULL,

	CONSTRAINT CHK_Percents CHECK ([Percents]>=1 AND [Percents]<=100)
);