USE [SivaguruCapital]

-- All iterations
SELECT * 
FROM Portfolio.Iteration
ORDER BY CurrentCash DESC

-- Number of orders by iteration
SELECT COUNT(IterationId) AS 'Total Orders', IterationId, Symbol
FROM Portfolio.[Order]
GROUP BY IterationId, Symbol
ORDER BY 
COUNT(IterationId) DESC

-- Number of orders per day by the iteration with the most orders
SELECT COUNT(IterationId), [Date]
FROM Portfolio.[Order]
GROUP BY IterationId, Symbol