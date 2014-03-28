USE [SivaguruCapital]

-- All iterations
SELECT * 
FROM Portfolio.Iteration
ORDER BY CurrentCash DESC

-- Number of orders by iteration
SELECT TOP 1 COUNT(IterationId) AS 'Total Orders', IterationId, Symbol
FROM Portfolio.[Order]
GROUP BY IterationId, Symbol
ORDER BY 
COUNT(IterationId) DESC

-- Number of orders per day by the iteration with the most orders
-- Watch to see if number of trades begins to approach a significant
-- portion of the volume for that day.
SELECT Date, COUNT(Date) AS 'Orders'
FROM Portfolio.[Order] A
 INNER JOIN
 (SELECT TOP 1 IterationId
  FROM Portfolio.[Order]
  GROUP BY IterationId, Symbol
  ORDER BY COUNT(IterationId) DESC) B 
  ON A.IterationId = B.IterationId
GROUP BY Date
ORDER BY COUNT(DATE) DESC

SELECT O.Date, O.Quantity, O.Value
FROM SivaguruCapital.Portfolio.[Order] O
INNER JOIN SivaguruCapital.Portfolio.Iteration I
ON O.IterationId = I.IterationId
WHERE I.IterationId = '36EC880C-0C15-4F2C-8904-56E3EC434D88'
ORDER BY Date