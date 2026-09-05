-- 1. Top 3 termék rendelt mennyiség szerint
SELECT
    p.Id,
    p.Name,
    SUM(oi.Quantity) AS TotalOrderedQuantity
FROM Products p
JOIN OrderItems oi ON oi.ProductId = p.Id
GROUP BY p.Id, p.Name
ORDER BY TotalOrderedQuantity DESC
LIMIT 3;

-- 2. Rendelések, amelyek legalább egy veszélyes terméket tartalmaznak
SELECT o.Id, o.OrderDate, c.Name AS CustomerName
FROM Orders o
JOIN Customers c ON c.Id = o.CustomerId
WHERE EXISTS (
    SELECT 1 
    FROM OrderItems oi
    JOIN Products p ON p.Id = oi.ProductId
    WHERE oi.OrderId = o.Id AND p.IsHazardous = 1
);