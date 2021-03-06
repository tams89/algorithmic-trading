-- Select stocks where price has decreased from its starting value over the year for each year present in the data.
SELECT  DATEPART(YY, X.Date) AS 'Year' ,
        (((SELECT [Close] FROM SivaguruCapital.InterDay.HistoricalStock WHERE Date = Max(X.Date)) - (SELECT [Close] FROM SivaguruCapital.InterDay.HistoricalStock WHERE Date = Min(X.Date))) 
		/ (SELECT [Close] FROM SivaguruCapital.InterDay.HistoricalStock WHERE Date = Min(X.Date))) * 100 AS 'Percent Decrease'
FROM    SivaguruCapital.InterDay.HistoricalStock X
WHERE X.[Close] > 0.0
GROUP BY DATEPART(YY, X.Date)
HAVING 
 ((SELECT [Close] FROM SivaguruCapital.InterDay.HistoricalStock WHERE Date = MAX(X.Date)) 
  / 
 (SELECT [Close] FROM SivaguruCapital.InterDay.HistoricalStock WHERE Date = MIN(X.Date))) 
 < 1
ORDER BY 'Percent Decrease'