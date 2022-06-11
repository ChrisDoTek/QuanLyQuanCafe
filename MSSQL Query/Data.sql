CREATE DATABASE QuanLyQuanCafe
GO

USE QuanLyQuanCafe
GO

-- Food
-- Table
-- Account
-- FoodCategory
-- Bill
-- BillInfo

CREATE TABLE TableFood
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Bàn chưa đặt tên',
	status NVARCHAR(100) NOT NULL DEFAULT N'Trống', --Trống || Có người
)
GO

-- Bảng history lưu lịch sử thay đổi giá 06/06/2020
CREATE TABLE History
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Món chưa đặt tên',	
	oldPrice FLOAT NOT NULL DEFAULT 0,
	newPrice FLOAT NOT NULL DEFAULT 0,
	changeDate DATE,
	idFood INT NOT NULL DEFAULT 0,
	FOREIGN KEY (idFood) REFERENCES dbo.Food(id)
)
GO

CREATE TABLE Account
(
	UserName NVARCHAR(100) NOT NULL PRIMARY KEY,
	DisplayName NVARCHAR(100) NOT NULL DEFAULT N'Kter',	
	PassWord NVARCHAR(1000) NOT NULL DEFAULT 0,
	Type INT NOT NULL DEFAULT 0 -- 1: admin && 0: staff
)
GO

CREATE TABLE FoodCategory
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Loại món chưa đặt tên',
)
GO

CREATE TABLE Food
(
	id INT IDENTITY PRIMARY KEY,
	name NVARCHAR(100) NOT NULL DEFAULT N'Món chưa đặt tên',
	idCategory INT NOT NULL,
	price FLOAT NOT NULL DEFAULT 0

	FOREIGN KEY (idCategory) REFERENCES dbo.FoodCategory(id)
)
GO

CREATE TABLE Bill
(
	id INT IDENTITY PRIMARY KEY,
	DateCheckIn DATE NOT NULL DEFAULT GETDATE(),
	DateCheckOut DATE,
	idTable INT NOT NULL,
	status INT NOT NULL DEFAULT 0 -- 1: đã thanh toán, 0: chưa thanh toán

	FOREIGN KEY (idTable) REFERENCES dbo.TableFood(id)
)
GO

CREATE TABLE BillInfo
(
	id INT IDENTITY PRIMARY KEY,
	idBill INT NOT NULL,
	idFood INT NOT NULL,
	count INT NOT NULL DEFAULT 0

	FOREIGN KEY (idBill) REFERENCES dbo.Bill(id),
	FOREIGN KEY (idFood) REFERENCES dbo.Food(id)

)
GO

INSERT INTO dbo.Account
(
	UserName,
	DisplayName,
	PassWord,
	Type
)
VALUES 
(
	N'Admin',
	N'Do Doan Ket',
	N'1',
	1
)

INSERT INTO dbo.Account
(
	UserName,
	DisplayName,
	PassWord,
	Type
)
VALUES 
(
	N'Staff01',
	N'Ngo Duy Manh',
	N'1',
	0
)
GO

CREATE PROC USP_GetAccountByUserName
@userName nvarchar(100)
AS
BEGIN
SELECT * FROM dbo.Account WHERE UserName = @userName
END
GO

EXEC dbo.USP_GetAccountByUserName @userName = N'Admin'
GO

--SELECT * FROM dbo.Account WHERE UserName= N'Admin' AND PassWord= N'' OR 1=1 --SQL injection

CREATE PROC USP_Login
@userName nvarchar(100), @passWord nvarchar(100)
AS
BEGIN
SELECT * FROM dbo.Account WHERE UserName = @userName AND PassWord = @passWord
END
GO

--Thêm bàn
DECLARE @i INT =0
WHILE @i <= 10 
BEGIN
INSERT dbo.TableFood (name) VALUES (N'Bàn ' + CAST(@i AS nvarchar(100)))
SET @i = @i +1
END
GO

CREATE PROC USP_GetTableList
AS SELECT * FROM dbo.TableFood
GO

EXEC USP_GetTableList

--Thêm category
INSERT dbo.FoodCategory
(name)
VALUES (N'Trà')
INSERT dbo.FoodCategory
(name)
VALUES (N'Cà phê')
INSERT dbo.FoodCategory
(name)
VALUES (N'Sinh tố')
INSERT dbo.FoodCategory
(name)
VALUES (N'Chè')

--Thêm món
--Trà
INSERT dbo.Food
(name, idCategory, price)
VALUES
(N'Trà quất', 1, 15000)
INSERT dbo.Food
(name, idCategory, price)
VALUES
(N'Trà đào', 1, 18000)
--Cà phê
INSERT dbo.Food
(name, idCategory, price)
VALUES
(N'Nâu đá', 2, 20000)
INSERT dbo.Food
(name, idCategory, price)
VALUES
(N'Sữa', 2, 20000)
--Sinh tố
INSERT dbo.Food
(name, idCategory, price)
VALUES
(N'Chanh leo', 3, 25000)
INSERT dbo.Food
(name, idCategory, price)
VALUES
(N'Cam ép', 3, 30000)
--Chè
INSERT dbo.Food
(name, idCategory, price)
VALUES
(N'Chè thập cẩm', 4, 15000)
INSERT dbo.Food
(name, idCategory, price)
VALUES
(N'Chè bưởi', 4, 20000)
INSERT dbo.Food
(name, idCategory, price)
VALUES
(N'Chè sương sa hạt lựu', 4, 20000)

--Thêm Bill
INSERT dbo.Bill
(DateCheckIn, DateCheckOut, idTable, status)
VALUES
(GETDATE(), NULL, 1, 0)

INSERT dbo.Bill
(DateCheckIn, DateCheckOut, idTable, status)
VALUES
(GETDATE(), NULL, 2, 0)

INSERT dbo.Bill
(DateCheckIn, DateCheckOut, idTable, status)
VALUES
(GETDATE(), GETDATE(), 2, 1)

--Thêm Bill info
INSERT dbo.BillInfo
(idBill, idFood, count)
VALUES
(1,1,2)

INSERT dbo.BillInfo
(idBill, idFood, count)
VALUES
(1,3,4)

INSERT dbo.BillInfo
(idBill, idFood, count)
VALUES
(1,5,1)

INSERT dbo.BillInfo
(idBill, idFood, count)
VALUES
(2,1,2)

INSERT dbo.BillInfo
(idBill, idFood, count)
VALUES
(2,6,2)

INSERT dbo.BillInfo
(idBill, idFood, count)
VALUES
(4,1,1)
GO

--SELECT * FROM dbo.Bill WHERE idTable = 1 AND status = 0
--SELECT f.name, bi.count, f.price, f.price*bi.count AS totalPrice FROM dbo.BillInfo AS bi, dbo.Bill as b,  dbo.Food as f 
--WHERE bi.idBill=b.id AND bi.idFood=f.id AND b.status=0 AND b.idTable=1 --Lấy thông tin hóa đơn

CREATE PROC USP_InsertBill
@idTable INT
AS
BEGIN
INSERT dbo.Bill
(DateCheckIn, DateCheckOut, idTable, status, discount)
VALUES
(GETDATE(), NULL, @idTable, 0, 0) 
END
GO

ALTER PROC USP_InsertBillInfo
@idBill INT, @idFood INT, @count INT
AS
BEGIN
	DECLARE @isExistBillInfo INT;
	DECLARE @foodCount INT =1

	SELECT @isExistBillInfo = id, @foodCount = b.count FROM dbo.BillInfo AS b WHERE idBill = @idBill AND idFood = @idFood
	IF (@isExistBillInfo > 0)
	BEGIN
		DECLARE @newCount INT = @foodCount + @count
		IF (@newCount >0)
			UPDATE dbo.BillInfo SET count = @foodCount + @count
		ELSE
			DELETE dbo.BillInfo WHERE idBill = @idBill AND idFood = @idFood
	END
	ELSE
	BEGIN
		INSERT dbo.BillInfo 
		(idBill, idFood, count)
		VALUES
		(@idBill, @idFood, @count)
	END
END
GO

--SELECT MAX(id) from dbo.Bill

--Fix Bug (thêm món vào ko làm thay đổi món khác, thêm món + vào món đã có chứ ko tạo món mới trùng tên)
ALTER PROC USP_InsertBillInfo
@idBill int, @idFood int, @count int
AS
BEGIN
 DECLARE @isExistBillInfo int
 DECLARE @foodCount int = 1
 SELECT @isExistBillInfo = id, @foodCount = count FROM BillInfo WHERE idBill = @idBill AND idFood = @idFood
 if(@isExistBillInfo > 0)
 BEGIN
  DECLARE @newCount int = @foodcount + @count
  if(@newCount > 0)
  BEGIN
   UPDATE BillInfo SET count = @newCount Where id = @isExistBillInfo

  END
  else
  BEGIN
   DELETE BillInfo Where id = @isExistBillInfo
  END
 END
 else
 BEGIN
  if(@count <= 0)
   BEGIN
   return 1;
   END
  else
   BEGIN
   INSERT INTO BillInfo
   (idBill,
   idFood,
   count)
   VALUES
   (@idBill,
   @idFood,
   @count)
   END
 END
END

GO

CREATE TRIGGER UTG_UpdateBillInfo
ON dbo.BillInfo FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @idBill INT
	SELECT @idBill = idBill FROM Inserted
	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill AND status = 0	
	UPDATE dbo.TableFood SET status = N'Có người' WHERE id = @idTable

END
GO

CREATE TRIGGER UTG_UpdateBill
ON dbo.Bill FOR UPDATE
AS
BEGIN
	DECLARE @idBill INT
	SELECT @idBill = id FROM Inserted
	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill
	DECLARE @count INT = 0
	SELECT @count = COUNT(*) FROM dbo.Bill WHERE idTable = @idTable AND status = 0
	IF (@count = 0)
	UPDATE dbo.TableFood SET status = N'Trống' WHERE id =@idTable
END
GO


ALTER TABLE dbo.Bill
ADD discount INT

UPDATE dbo.Bill SET discount = 0
GO

ALTER PROC USP_SwitchTable
@idTable1 INT, @idTable2 INT
AS
BEGIN

DECLARE @idFirstBill INT
DECLARE @idSecondBill INT

DECLARE @isFirstTableEmpty INT = 1
DECLARE @isSecondTableEmpty INT = 1


SELECT @idSecondBill = id FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0
SELECT @idFirstBill = id FROM dbo.Bill WHERE idTable = @idTable1 AND status = 0

IF (@idFirstBill IS NULL)
BEGIN
	INSERT dbo.Bill
	(DateCheckIn, DateCheckOut, idTable, status)
	VALUES
	(GETDATE(), NULL, @idTable1, 0)
	SELECT @idFirstBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable1 AND status = 0

	
END

	SELECT @isFirstTableEmpty = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idFirstBill

IF (@idSecondBill IS NULL)
BEGIN
	INSERT dbo.Bill
	(DateCheckIn, DateCheckOut, idTable, status)
	VALUES
	(GETDATE(), NULL, @idTable2, 0)
	SELECT @idSecondBill = MAX(id) FROM dbo.Bill WHERE idTable = @idTable2 AND status = 0
	
	
END

	SELECT @isSecondTableEmpty = COUNT(*) FROM dbo.BillInfo WHERE idBill = @idSecondBill

	SELECT id INTO IDBillInfoTable FROM dbo.BillInfo WHERE idBill = @idSecondBill

	UPDATE dbo.BillInfo SET idBill = @idSecondBill WHERE idBill = @idFirstBill
	UPDATE dbo.BillInfo SET idBill = @idFirstBill WHERE id IN (SELECT * FROM IDBillInfoTable)

	DROP TABLE IDBillInfoTable

	IF (@isFirstTableEmpty = 0)
		UPDATE dbo.TableFood set status = N'Trống' WHERE id = @idTable2

	IF (@isSecondTableEmpty = 0)
		UPDATE dbo.TableFood set status = N'Trống' WHERE id = @idTable1

END
GO

ALTER TABLE dbo.Bill ADD totalPrice FLOAT

GO

ALTER PROC USP_GetListBillByDate
@checkIn DATE, @checkOut DATE
AS
BEGIN

SELECT t.Name AS [Tên bàn], b.totalPrice AS [Tổng tiền], DateCheckIn AS [Ngày vào], DateCheckOut AS [Ngày ra], discount AS [Giảm giá], a.DisplayName AS [Nhân viên]
FROM dbo.Bill as b, dbo.TableFood as t, dbo.Account as a
WHERE DateCheckin >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1 
AND t.id = b.idTable AND b.EmployeeCode = a.UserName

END
GO

CREATE PROC USP_UpdateAccount
@userName NVARCHAR(100), @displayName NVARCHAR(100), @password NVARCHAR(100), @newPassword NVARCHAR(100)
AS
BEGIN
	DECLARE @isRightPass INT = 0

	SELECT @isRightPass = COUNT(*) FROM dbo.Account WHERE UserName = @userName AND PassWord = @password 

	IF (@isRightPass = 1)
	BEGIN
		IF (@newPassword = NULL OR @newPassword = '')
		BEGIN
			UPDATE dbo.Account SET DisplayName = @displayName WHERE UserName = @userName
		END
		ELSE
			UPDATE dbo.Account SET DisplayName = @displayName, PassWord = @newPassword WHERE UserName = @userName
	END
END
GO

ALTER TRIGGER UTG_DeleteBillInfo
ON dbo.BillInfo FOR DELETE
AS 
BEGIN
	DECLARE @idBillInfo INT
	DECLARE @idBill INT
	SELECT @idBillInfo = id, @idBill = Deleted.idBill FROM Deleted
	
	DECLARE @idTable INT
	SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill
	
	DECLARE @count INT = 0
	
	SELECT @count = COUNT(*) FROM dbo.BillInfo AS bi, dbo.Bill AS b WHERE b.id = bi.idBill AND b.id = @idBill AND b.status = 0
	
	IF (@count = 0)
		UPDATE dbo.TableFood SET status = N'Trống' WHERE id = @idTable
END
GO

CREATE FUNCTION [dbo].[fuConvertToUnsign1] ( @strInput NVARCHAR(4000) ) RETURNS NVARCHAR(4000) AS BEGIN IF @strInput IS NULL RETURN @strInput IF @strInput = '' RETURN @strInput DECLARE @RT NVARCHAR(4000) DECLARE @SIGN_CHARS NCHAR(136) DECLARE @UNSIGN_CHARS NCHAR (136) SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý ĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍ ÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' +NCHAR(272)+ NCHAR(208) SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee iiiiiooooooooooooooouuuuuuuuuuyyyyy AADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIII OOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD' DECLARE @COUNTER int DECLARE @COUNTER1 int SET @COUNTER = 1 WHILE (@COUNTER <=LEN(@strInput)) BEGIN SET @COUNTER1 = 1 WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1) BEGIN IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) ) BEGIN IF @COUNTER=1 SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1) ELSE SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER) BREAK END SET @COUNTER1 = @COUNTER1 +1 END SET @COUNTER = @COUNTER +1 END SET @strInput = replace(@strInput,' ','-') RETURN @strInput END
GO

SELECT * FROM dbo.Food WHERE dbo.fuConvertToUnsign1(name) LIKE N'%' + dbo.fuConvertToUnsign1(N'sua') + '%'
GO

ALTER PROC USP_GetListBillByDateAndPage
@checkIn DATE, @checkOut DATE, @page INT
AS
BEGIN

	DECLARE @pageRows INT = 10
	DECLARE @selectRows INT = @pageRows
	DECLARE @exceptRows INT = (@page -1)* @pageRows

	;WITH BillShow AS (SELECT b.id, t.Name AS [Tên bàn], b.totalPrice AS [Tổng tiền], DateCheckIn AS [Ngày vào], DateCheckOut AS [Ngày ra], discount AS [Giảm giá]
	FROM dbo.Bill as b, dbo.TableFood as t
	WHERE DateCheckin >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1 
	AND t.id = b.idTable)	

	SELECT TOP (@selectRows) * FROM BillShow WHERE id NOT IN (SELECT TOP (@exceptRows) id FROM BillShow)
END
GO

exec USP_GetListBillByDateAndPage
@checkIn = '2020-04-01', @checkOut= '2020-04-30', @page = 1

CREATE PROC USP_GetNumBillByDate
@checkIn DATE, @checkOut DATE
AS
BEGIN

SELECT COUNT(*) 
FROM dbo.Bill as b, dbo.TableFood as t
WHERE DateCheckin >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1 
AND t.id = b.idTable

END
GO

ALTER PROC USP_GetListBillByDateForReport
@checkIn DATE, @checkOut DATE
AS
BEGIN

SELECT t.Name, b.totalPrice, DateCheckIn, DateCheckOut, discount, a.DisplayName
FROM dbo.Bill as b, dbo.TableFood as t, dbo.Account as a
WHERE DateCheckin >= @checkIn AND DateCheckOut <= @checkOut AND b.status = 1 
AND t.id = b.idTable AND b.EmployeeCode = a.UserName

END
GO

CREATE TRIGGER UTG_ChangePrice
ON dbo.Food FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @idFood INT
	SELECT @idFood = id FROM Inserted
	--DECLARE @idTable INT
	--SELECT @idTable = idTable FROM dbo.Bill WHERE id = @idBill AND status = 0	
	--UPDATE dbo.TableFood SET status = N'Có người' WHERE id = @idTable


END
GO


CREATE PROC USP_BillPDF
@id INT
AS
BEGIN
SELECT * FROM dbo.Bill WHERE idTable = @id
END
GO

ALTER TABLE dbo.Bill
ADD EmployeeCode NVARCHAR(100) NOT NULL DEFAULT N'Chua co Employee Code';

ALTER TABLE dbo.Account
ADD IsActive NVARCHAR(10) DEFAULT N'FALSE';
GO

CREATE PROC USP_EmployeeSumPerMonth
@checkIn DATE, @checkOut DATE
AS
BEGIN
SELECT a.DisplayName, SUM(b.totalPrice), count(*)
FROM dbo.Bill AS b, dbo.Account AS a WHERE a.UserName = b.EmployeeCode AND b.DateCheckin >= @checkIn AND b.DateCheckOut <= @checkOut GROUP BY DisplayName
END
GO






