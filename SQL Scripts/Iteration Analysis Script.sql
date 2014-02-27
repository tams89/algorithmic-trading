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