-- Normal table
CREATE TABLE `employees` (
  `Id` int NOT NULL,
  `BirthDay` date NOT NULL,
  `FirstName` varchar(14) NOT NULL,
  `LastName` varchar(16) NOT NULL,
  `Gender` enum('M','F') NOT NULL,
  `HireDate` date NOT NULL,
  `FullName` longtext,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Table added fts
CREATE TABLE `employees_fts` (
  `Id` int NOT NULL,
  `BirthDay` date NOT NULL,
  `FirstName` varchar(14) NOT NULL,
  `LastName` varchar(16) NOT NULL,
  `Gender` enum('M','F') NOT NULL,
  `HireDate` date NOT NULL,
  `FullName` longtext,
  PRIMARY KEY (`Id`),
  FULLTEXT KEY `FTS_FullName` (`FullName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


-- Query
SELECT COUNT(*) FROM employees;
SELECT * FROM employees WHERE FullName LIKE "%Sumali%" OR FullName Like "%A%";
SELECT * FROM employees_fts WHERE MATCH(FullName) AGAINST("A Sumali B Kushner" IN BOOLEAN MODE);


