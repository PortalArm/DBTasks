--1) ������ ���� ���������, ������� ����� ����� 150 �������. ������������ ��������� SELECT.
declare @orders int = 150
select * from Employees where (Select count(*) from Orders where Orders.EmployeeID = Employees.EmployeeID) > @orders


--2) ����� ���� �����������, �d������ ����� � ����� ������. 
declare @city nchar(6) = 'London'
select * from Customers where Customers.City = @city
--���
select City, count(*) as [Customers in the city] from Customers group by City

--3) ���������� ���������, ������� ����������� ������ 'Western' (������� Region).
declare @reg nchar(7) = 'Western'
select distinct Employees.EmployeeID, Employees.FirstName, Employees.LastName, Employees.HomePhone from Region inner join Territories on Region.RegionID = Territories.RegionID and RegionDescription = @reg inner join EmployeeTerritories on EmployeeTerritories.TerritoryID = Territories.TerritoryID inner join Employees on Employees.EmployeeID = EmployeeTerritories.EmployeeID
go
--���
declare @reg nchar(7) = 'Western'
select * from Employees where exists (select EmployeeTerritories.EmployeeID from EmployeeTerritories inner join Territories on EmployeeTerritories.TerritoryID = Territories.TerritoryID and Territories.RegionID = (select RegionID from Region where RegionDescription = @reg) and EmployeeTerritories.EmployeeID = Employees.EmployeeID)

--4) ������� ����� �� 1996
--customerName |ShipName |ShippedDate |ProductName |UnitPrice
declare @year int = 1996
select Customers.ContactName, Orders.ShipName, Orders.ShippedDate, Products.ProductName, od.UnitPrice from Orders inner join [Order Details] od on Orders.OrderID = od.OrderID and YEAR(Orders.ShippedDate) = @year inner join Products on Products.ProductID = od.ProductID inner join Customers on Customers.CustomerID = Orders.CustomerID

--5) ������� ����� 
--���| �����| ���������� �������| ����� ����� �������
--create view YearReport as
select YEAR(OrderDate) as Year, MONTH(OrderDate) as Month, count(*) as [Order count], sum([Order Details].UnitPrice*[Order Details].Quantity*(1-[Order Details].Discount)) as Sum from Orders inner join [Order Details] on Orders.OrderID = [Order Details].OrderID group by YEAR(OrderDate), MONTH(OrderDate) 
--select * from YearReport order by Year, Month

--6) ����� ������ 5 customers ������ ������� ���� ���������� ������� ����
select top 5 Customers.*,  DATEDIFF(DAY, OrderDate, ShippedDate) as [����� ��������� �� ... ����]from Orders inner join Customers on Orders.CustomerID = Customers.CustomerID order by case when DATEDIFF(DAY, OrderDate, ShippedDate) is null then 1 else 0 end, DATEDIFF(DAY, OrderDate, ShippedDate)

--7) ������� �����
--customerName| ���������� ������� | ����� �����
--create view CustomerReport as
select Customers.ContactName, count(*) as [Order count], sum(od.UnitPrice*od.Quantity*(1-od.Discount)) as Sum from Orders inner join [Order Details] od on Orders.OrderID = od.OrderID inner join Products on Products.ProductID = od.ProductID inner join Customers on Customers.CustomerID = Orders.CustomerID group by Customers.ContactName

--8)�����  ������ 5 customers ��� ����� ����� ������� �������
select top 5 Customers.*, od.UnitPrice as [���� ��������] from Orders inner join [Order Details] od on Orders.OrderID = od.OrderID inner join Products on Products.ProductID = od.ProductID inner join Customers on Customers.CustomerID = Orders.CustomerID order by od.UnitPrice desc
 
--9)����� �����, customers �������, ��������� ������ ����� �� ������� 1997
declare @year int = 1997,
        @month int = 12
select top 1 City, sum([Order Details].UnitPrice*[Order Details].Quantity*(1-[Order Details].Discount)) as Total from Orders inner join Customers on Customers.CustomerID = Orders.CustomerID and YEAR(OrderDate) = @year and MONTH(OrderDate) = @month inner join [Order Details] on [Order Details].OrderID = Orders.OrderID group by City order by Total desc