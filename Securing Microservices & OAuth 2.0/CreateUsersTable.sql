-- Run this SQL script against your EmployeeServiceDb database
-- to create the Users table without stopping the service

USE EmployeeServiceDb;
GO

CREATE TABLE [dbo].[Users] (
    [UserId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Username] NVARCHAR(50) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL,
    [PasswordHash] NVARCHAR(MAX) NOT NULL,
    [Role] NVARCHAR(20) NOT NULL DEFAULT 'User',
    [CreatedAt] DATETIME2 NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [UQ_Users_Username] UNIQUE ([Username])
);
GO

-- Verify table was created
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users';
GO
