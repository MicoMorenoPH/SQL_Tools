1. Sql constraint
CREATE TABLE SqlConstraint
(
id INT IDENTITY,
employeeId VARCHAR(255),
firstName VARCHAR(255),
lastName VARCHAR(255),
CONSTRAINT pk_SqlConstraint PRIMARY KEY (employeeId,firstName)
)

2. Using Joins
  2.1 Inner Joins
  SELECT a.*,b.*
  FROM sqlconstraint a
  INNER JOIN EmployeeDetails b ON a.employeeId = b.employeeId

  2.2 Left Join
  (present on the base table and absent on the child table)
  SELECT a.*,b.*
  FROM sqlconstraint a
  LEFT JOIN EmployeeDetails b ON a.employeeId = b.employeeId
  WHERE a.employeeId ='002'

  2.3 Right Join
  (present on the child table and absent on the base table)
  SELECT a.*,b.*
  FROM sqlconstraint a
  RIGHT JOIN EmployeeDetails b ON a.employeeId = b.employeeId
  WHERE b.employeeId ='003'

  2.4 Union All
  (columns / fields must be same in all script te be execute)
  SELECT * FROM sqlconstraint WHERE employeeid ='001'
  UNION ALL
  SELECT * FROM sqlconstraint WHERE employeeid ='002'

3. Indexes
  Create script > highlight and click Display Estimated Execution Plan

4. View
   https://www.c-sharpcorner.com/blogs/advantages-and-disadvantages-of-views-in-sql-server1
  (Advantage
    a. security
    - cannot easily update the tables beacause the script is inside of view tables
    b. query simplicity
    - you can draw data from several tables and present it as a single table ,
      turning multiple-table queries into single-table queries by creating a view.
  )

  https://www.mssqltips.com/sqlservertip/5147/limitations-when-working-with-sql-server-views/
  (Disadvantage / Limitation
    a. Cannot pass parameters
    b. Cannot use order by clause
    c. Cannot create view with temporary tables
  )

  >> script to create view
  CREATE VIEW viewName
  AS
  SELECT *
  FROM SqlConstraint

5. Triggers

  >> CREATE LIVE table
    CREATE TABLE product
    (
    id INT IDENTITY,
    name VARCHAR(255)
    )

  >> CREATE audit trail table
    CREATE TABLE productLogs
    (
    id INT IDENTITY,
    docentry INT,
    name VARCHAR(255),
    operation CHAR(4),
    CHECK(operation = 'INS' OR operation='DEL' OR operation='UP')
    )

  >> create trigger every function on the live table
    CREATE TRIGGER trgProductLogs
    ON product
    AFTER INSERT,DELETE,UPDATE
    AS
    BEGIN
    	SET NOCOUNT ON;
    	DECLARE @ACTION CHAR(1)

     SET @ACTION = (CASE WHEN EXISTS(SELECT 1 FROM inserted) AND EXISTS(SELECT 1 FROM deleted)
                         THEN 'U'
                         WHEN EXISTS(SELECT 1 FROM inserted)
                         THEN 'I'
                         WHEN EXISTS(SELECT 1 FROM deleted)
                         THEN 'D'
                         ELSE NULL
     END)

        IF @ACTION = 'I'
        BEGIN
             INSERT INTO dbo.productLogs( docentry, name, operation )
             SELECT id,name,'INS' FROM inserted
        END

        IF @ACTION = 'U'
        BEGIN
    	 INSERT INTO dbo.productLogs( docentry, name, operation )
    	 SELECT id,name,'UP' FROM inserted
        END

        IF @ACTION = 'D'
        BEGIN
             INSERT INTO dbo.productLogs( docentry, name, operation )
             SELECT id,name,'DEL' FROM deleted
        END

    END

7. Function
  (basically used for condition in query)

  >> create function and return scalar
  CREATE FUNCTION ufnReturnScalar
  (@employeeID varchar(255))
  RETURNS varchar(max)
  AS
  BEGIN
  	DECLARE @retValue AS varchar(255)
  	SELECT @retValue = firstName + '--' + lastName
  	FROM SqlConstraint
  	WHERE employeeId = @employeeID
  	RETURN @retValue
  END

  >>execute
  SELECT dbo.ufnReturnScalar ('001')

8. Stored Procedure
  (create script and store it in stored procedure)

  >> create stored procedure
  CREATE PROCEDURE [spDemo]
  (@function varchar(255),
   @employeeId varchar(255) = null
  )
  AS
  BEGIN

  IF @function ='SqlConstraint'
  BEGIN
  	SELECT * FROM SqlConstraint
  END


  IF @function  ='EmployeeDetails'
  BEGIN
  	SELECT * FROM EmployeeDetails
  END

  END

  >> execute
  exec spDemo @function ='SqlConstraint'

9. Temporary tables

  CREATE TABLE #tmp
  (
  id INT IDENTITY,
  name VARCHAR(255)
  )

  >>execute
  select * from #tmp

10. Sub Queries

SELECT Ord.firstname, Ord.lastname,
    (SELECT MAX(age)
     FROM EmployeeDetails AS OrdDet
     WHERE Ord.employeeId = OrdDet.employeeId) AS MaxUnitPrice
FROM SqlConstraint AS Ord;


SELECT *
FROM SqlConstraint
WHERE employeeId IN (SELECT DISTINCT employeeId FROM EmployeeDetails)

11. crosstab queries
