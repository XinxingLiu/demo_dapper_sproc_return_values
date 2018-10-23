DROP PROCEDURE IF EXISTS [dbo].[InsertIntoEnumValue]
GO
CREATE PROCEDURE [dbo].[InsertIntoEnumValue]
(
    @Name NVARCHAR (255),
    @Value INT,
    @DataTypeId INT,
    @ErrorMessage NVARCHAR(255)=null out  
)
AS
BEGIN

BEGIN TRY
    BEGIN TRANSACTION
	    
	SET NOCOUNT ON

    IF NOT EXISTS (SELECT [Id] FROM [dbo].[DataType] AS dt WHERE dt.[Id] = @DataTypeId) 
    BEGIN
      SET @ErrorMessage = 'DataType with Id ' + CAST(@DataTypeId AS VARCHAR(11)) + ' does not exist.';
      ROLLBACK TRANSACTION;
      RETURN 1;
    END

    INSERT INTO [dbo].[EnumValue]([Name],[Value],[DataTypeId])
    VALUES(@Name, @Value, @DataTypeId)

	SELECT SCOPE_IDENTITY() AS NewEnumId;

    COMMIT TRANSACTION
END TRY

BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH

END