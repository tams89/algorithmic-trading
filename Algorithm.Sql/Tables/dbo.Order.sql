CREATE TABLE [dbo].[Order]
(
	[OrderId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [IterationId] INT NOT NULL, 
    [Symbol] VARCHAR(10) NOT NULL, 
    [Date] DATETIME NOT NULL, 
    [Quantity] INT NOT NULL, 
    [OrderType] VARCHAR(10) NOT NULL, 
    [Value] DECIMAL(18, 2) NOT NULL, 
    [IsClosed] BIT NOT NULL,
	CONSTRAINT [FK_OrderIteration] FOREIGN KEY (IterationId) REFERENCES [Iteration] ([IterationId])
)
