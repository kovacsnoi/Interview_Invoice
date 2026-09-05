-- =============================================================================
-- INVOICING SYSTEM - DATABASE SCHEMA & QUERIES (SQLite Compatible)
-- =============================================================================

-- 1. TÁBLÁK LÉTREHOZÁSA (DDL)
-- -----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS Customers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Country TEXT NOT NULL,
    Address TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Products (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Category TEXT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL,
    IsHazardous INTEGER NOT NULL DEFAULT 0,
    IsFragile INTEGER NOT NULL DEFAULT 0,
    IsDiscountEligible INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS Orders (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerId INTEGER NOT NULL,
    OrderDate TEXT NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id) ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS OrderItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    Quantity INTEGER NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE RESTRICT
);

-- -----------------------------------------------------------------------------
-- 2. MINTAADATOK (SEED DATA)
-- -----------------------------------------------------------------------------

INSERT INTO Customers (Id, Name, Country, Address) VALUES
(1, 'ABC Ltd', 'Hungary', 'Budapest, Fo u. 1.'),
(2, 'Tech Solutions Kft', 'Hungary', 'Debrecen, Piac u. 12.'),
(3, 'Nordic Imports AS', 'Norway', 'Oslo, Karl Johans gate 5.');

INSERT INTO Products (Id, Name, Category, UnitPrice, IsHazardous, IsFragile, IsDiscountEligible) VALUES
(1, 'Laptop', 'Electronics', 1000.00, 0, 1, 0),
(2, 'Mouse', 'Electronics', 20.00, 0, 0, 1),
(3, 'Battery Pack', 'Supplies', 50.00, 1, 0, 0),
(4, 'Monitor', 'Electronics', 300.00, 0, 1, 0),
(5, 'Keyboard', 'Electronics', 45.00, 0, 0, 1);

INSERT INTO Orders (Id, CustomerId, OrderDate) VALUES
(1, 1, '2025-03-15 00:00:00'),
(2, 1, '2025-04-01 00:00:00'),
(3, 2, '2025-05-10 00:00:00'),
(4, 3, '2025-06-20 00:00:00');

INSERT INTO OrderItems (Id, OrderId, ProductId, Quantity) VALUES
(1, 1, 1, 2),
(2, 1, 2, 3),
(3, 2, 3, 1),
(4, 3, 1, 5),
(5, 3, 4, 3),
(6, 3, 5, 4),
(7, 4, 2, 10),
(8, 4, 3, 2),
(9, 4, 4, 1);
