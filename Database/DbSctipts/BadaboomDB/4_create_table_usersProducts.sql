CREATE TABLE [dbo].[UsersProducts](
	[UserId] INT NOT NULL FOREIGN KEY REFERENCES [Users]([UserId]) ,

	[ProductId] INT NOT NULL FOREIGN KEY REFERENCES [Products]([Id]),

	[Quantity] INT NOT NULL,

	PRIMARY KEY ([UserId], [ProductId]),

	UNIQUE ([UserId], [ProductId])
);