CREATE TABLE [dbo].[UsersProducts](
	[UserId] INT NOT NULL FOREIGN KEY REFERENCES [Users]([UserId]) ,

	[PriceId] INT NOT NULL FOREIGN KEY REFERENCES [Products]([Id]),

	[Quantity] INT NOT NULL,

	PRIMARY KEY ([UserId], [PriceId]),

	UNIQUE ([UserId], [PriceId])
);