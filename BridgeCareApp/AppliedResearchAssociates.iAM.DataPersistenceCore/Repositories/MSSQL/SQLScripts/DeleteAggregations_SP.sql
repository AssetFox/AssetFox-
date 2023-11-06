
Create PROCEDURE dbo.usp_delete_aggregations(
@NetworkId AS uniqueidentifier=NULL,
@RetMessage VARCHAR(250) OUTPUT
)
AS 

    BEGIN 

 	DECLARE @CustomErrorMessage NVARCHAR(MAX),
	@ErrorNumber int,
	@ErrorSeverity int,
	@ErrorState int,
	@ErrorProcedure nvarchar(126),
	@ErrorLine int,
	@ErrorMessage nvarchar(4000);
	Set  @RetMessage = 'Success';
	DECLARE @CurrentDateTime DATETIME;
	DECLARE @BatchSize INT = 100000;
	DECLARE @RowsDeleted INT = 1;

	BEGIN TRY
-----------------------------------------------------------------------

--AggregatedResult
--AttributeDatum
--AttributeDatumLocation


	-----Start AggregatedResult Path-----------------------------------------

			--MaintainableAsset --> AggregatedResult

	BEGIN TRY

          ALTER TABLE AggregatedResult NOCHECK CONSTRAINT all
		  SET @RowsDeleted = 1;
		  --Print 'AggregatedResult ';

		   WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
						Begin Transaction

						--Delete TOP (@BatchSize) l3
						SELECT TOP  (@BatchSize) l3.Id  INTO #tempAggregatedResult
						FROM Network AS l1
						Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
						JOIN AggregatedResult AS l3 ON l3.MaintainableAssetId = l2.Id
						WHERE l1.Id IN (@NetworkId);
						
						--DELETE ar
						--FROM AggregatedResult As ar
						--JOIN #tempAggregatedResult T ON T.Id = ar.Id;

						DELETE FROM AggregatedResult WHERE Id in (SELECT Id FROM #tempAggregatedResult);

						SET @RowsDeleted = @@ROWCOUNT;

						DROP TABLE #tempAggregatedResult;
						--WAITFOR DELAY '00:00:01';

						COMMIT TRANSACTION
						
						Print 'Rows Affected Network --> MaintainableAsset-->AggregatedResult: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
							ALTER TABLE AggregatedResult WITH CHECK CHECK CONSTRAINT all
  							Set @RetMessage = 'Failed';
							Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							Print 'Rolled Back AggregatedResult Delete Transaction in NetworkDelete SP:  ' + @ErrorMessage;
							ROLLBACK TRANSACTION;
							RAISERROR  (@RetMessage, 16, 1); 
							Return -1;
					END CATCH;
				END

            ALTER TABLE AggregatedResult WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in  Network --> MaintainableAsset-->AggregatedResult'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------End -----------------------------------------------------------------


			-------Start AttributeDatum Path-

			--MaintainableAsset --> AttributeDatum --> AttributeDatumLocation -->  -->  --> 

            BEGIN TRY

          ALTER TABLE AttributeDatumLocation NOCHECK CONSTRAINT all
		  SET @RowsDeleted = 1;
		  	--Print 'AttributeDatumLocation ';

		  WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
						Begin Transaction

							SELECT TOP (@BatchSize) l4.Id  INTO #tempAttributeDatumLocation
							FROM Network AS l1
							Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
							JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
							Join AttributeDatumLocation As l4 ON l4.AttributeDatumId = l3.Id
							WHERE l1.Id IN (@NetworkId);

						--DELETE adl
						--FROM AttributeDatumLocation As adl
						--JOIN #tempAttributeDatumLocation T ON T.Id = adl.Id;

						DELETE FROM AttributeDatumLocation WHERE Id in (SELECT Id FROM #tempAttributeDatumLocation);

						SET @RowsDeleted = @@ROWCOUNT;

						DROP TABLE #tempAttributeDatumLocation;
						--WAITFOR DELAY '00:00:01';

						COMMIT TRANSACTION
						
						Print 'Rows Affected Network --> MaintainableAsset-->AttributeDatumLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
							ALTER TABLE AttributeDatumLocation WITH CHECK CHECK CONSTRAINT all
  							Set @RetMessage = 'Failed';
							Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							Print 'Rolled Back AttributeDatumLocation Delete Transaction in NetworkDelete SP:  ' + @ErrorMessage;
							ROLLBACK TRANSACTION;
							RAISERROR  (@RetMessage, 16, 1); 
							Return -1;
					END CATCH;
				END

            ALTER TABLE AttributeDatumLocation WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network --> MaintainableAsset-->AttributeDatum-->AttributeDatumLocation'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-----------------------------------------------------------------------

    		--------MaintainableAsset --> AttributeDatum--

            BEGIN TRY

			ALTER TABLE AttributeDatum NOCHECK CONSTRAINT all
			SET @RowsDeleted = 1;
			Print 'AttributeDatum ';

			WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
					Begin Transaction

						Select  TOP (@BatchSize) l3.Id  INTO #tempAttributeDatum
						FROM Network AS l1
						Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
						JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
						WHERE l1.Id IN (@NetworkId);

						--DELETE ad
						--FROM AttributeDatum As ad
						--JOIN #tempAttributeDatum T ON T.Id = ad.Id;

						DELETE FROM AttributeDatum WHERE Id in (SELECT Id FROM #tempAttributeDatum);

						SET @RowsDeleted = @@ROWCOUNT;

						DROP TABLE #tempAttributeDatum;
						--WAITFOR DELAY '00:00:01';

						COMMIT TRANSACTION
						
						Print 'Rows Affected Network --> MaintainableAsset-->AttributeDatum: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
						ALTER TABLE AttributeDatum WITH CHECK CHECK CONSTRAINT all
						Set @RetMessage = 'Failed';
						Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
						Print 'Rolled Back AttributeDatum Delete Transaction in NetworkDelete SP:  ' + @ErrorMessage;
						ROLLBACK TRANSACTION;
						RAISERROR  (@RetMessage, 16, 1); 
						Return -1;
					END CATCH;
				END

            ALTER TABLE AttributeDatum WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network --> MaintainableAsset-->AttributeDatum'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH


			----------------------------------------------------------------------
			-----End AttributeDatum Path--------------------------------------

   --COMMIT TRANSACTION
    Print 'Delete Attribute End';
   	RAISERROR (@RetMessage, 0, 1);
	END TRY
	BEGIN CATCH
  			Set @RetMessage = 'Failed ' + @RetMessage;
			Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			Print 'Error in AttributeDelete SP:  ' + @ErrorMessage;
			--ROLLBACK TRANSACTION;
			RAISERROR  (@RetMessage, 16, 1);  
	END CATCH;

END 

--  DECLARE @RetMessage varchar(100); EXEC usp_delete_aggregations '119AD446-3330-426B-864D-E9D471949D6B' , @RetMessage OUTPUT 

