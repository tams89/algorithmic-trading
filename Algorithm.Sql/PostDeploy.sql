USE [SivaguruCapital]

TRUNCATE TABLE [dbo].[Tick]

CREATE TABLE #TickStaging
    (
      [Date] VARCHAR(10) ,
      [Time] VARCHAR(10) ,
      [Open] VARCHAR(10) ,
      [High] VARCHAR(10) ,
      [Low] VARCHAR(10) ,
      [Close] VARCHAR(10) ,
      [Volume] VARCHAR(50)
    )

BULK INSERT #TickStaging
FROM 'C:\Users\Tam\Documents\Repositories\sivaguru-capital\Algorithm.Sql\SeedData\IBM_adjusted.txt'
WITH 
(
    FIRSTROW = 2,
    FIELDTERMINATOR = ',',  --CSV field delimiter
    ROWTERMINATOR = '\n',   --Use to shift the control to next row
    TABLOCK
)

INSERT  INTO [dbo].[Tick]
        SELECT  'IBM Minute' ,
				CONVERT(DATETIME, [DATE] + ' ' + [Time]) ,
                CONVERT(DECIMAL, [Open]) ,
                CONVERT(DECIMAL, [High]) ,
                CONVERT(DECIMAL, [Low]) ,
                CONVERT(DECIMAL, [Close]) ,
                CONVERT(DECIMAL, [Volume])
        FROM    [#TickStaging]

DROP TABLE #TickStaging