DROP TABLE Orderrows
DROP TABLE Orders
DROP TABLE Statuses
DROP TABLE Users
DROP TABLE Products
DROP TABLE Addresses
DROP TABLE Categories
DROP TABLE Roles
DROP TABLE Hashes

CREATE TABLE Hashes (

	Id int not null identity primary key,
	Pass varbinary(max) not null,
	Salt varbinary(max) not null
)
GO

CREATE TABLE Roles(
	
	Id int not null identity primary key,
	Rolename nvarchar(50) not null
)
GO

CREATE TABLE Categories (
	
	Id int not null identity primary key,
	Category nvarchar(50) not null
)
GO

CREATE TABLE Addresses (

	Id int not null identity primary key,
	Street nvarchar(50),
	Zipcode nvarchar(5),
	City nvarchar(50)
)
GO

CREATE TABLE Products (

	Id int not null identity primary key,
	Artnumber nvarchar(50) not null unique,
	ProductName nvarchar(50) null,
	ProductDescription nvarchar(max) null,
	Price money not null,
	CategoryId int not null references Categories(Id)
)
GO

CREATE TABLE Users (

	Id int not null identity primary key,
	Firstname nvarchar(50),
	Lastname nvarchar(50),
	Email varchar(50) unique,
	AddressId int references Addresses(Id),
	HashId int references Hashes(Id),
	RoleId int references Roles(Id)
)
GO

CREATE TABLE Statuses (
	
	Id int not null identity primary key,
	Orderstatus nvarchar(50) not null
)


CREATE TABLE Orders (

	Id int not null identity primary key,
	Created DATETIME2 not null,
	OrderStatusId int not null references Statuses(Id),
	UserId int references Users(Id)
)
GO

CREATE TABLE Orderrows (

	Id int not null identity primary key,
	Quantity int not null,
	ProductId int not null references Products(Id),
	OrderId int not null references Orders(Id)
)
GO





