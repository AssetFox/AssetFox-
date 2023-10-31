
Create PROCEDURE dbo.usp_delete_simulationoutput(
--@SimulationOutputId AS uniqueidentifier=NULL,
@SimOutputGuidList NVARCHAR(MAX)=NULL,
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
	DECLARE @BatchSize INT = 200000;  -- Adjust batch size as needed
	DECLARE @RowsDeleted INT = 1;

    CREATE TABLE #SimOutputTempGuids
    (
        --Guid UNIQUEIDENTIFIER
		 Guid NVARCHAR(36)
    );


	IF @SimOutputGuidList IS NULL OR LEN(@SimOutputGuidList) = 0
	BEGIN
		  PRINT 'String is NULL or empty';
		  Set  @SimOutputGuidList = '00000000-0000-0000-0000-000000000000';
	END

    INSERT INTO #SimOutputTempGuids (Guid)
	SELECT LEFT(LTRIM(RTRIM(value)), 36)
    FROM STRING_SPLIT(@SimOutputGuidList, ',');

	UPDATE #SimOutputTempGuids
	SET Guid = '00000000-0000-0000-0000-000000000000'
	WHERE TRY_CAST(Guid AS UNIQUEIDENTIFIER) IS NULL OR Guid = '';
	
	Begin Transaction
	BEGIN TRY

			-----Start AssetSummaryDetail Path-----------------------------------------

			-------SimulationOutput --> AssetSummaryDetail --> AssetSummaryDetailValueIntI----

            BEGIN TRY

          ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all
		  
		  SET @RowsDeleted = 1;
		  --Print 'AssetSummaryDetailValueIntId ';

		   WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
						Begin Transaction

							Delete TOP (@BatchSize) l3
							FROM SimulationOutput AS l1
							JOIN AssetSummaryDetail AS l2 ON l2.SimulationOutputId = l1.Id
							Join AssetSummaryDetailValueIntId As l3 ON l3.AssetSummaryDetailId = l2.Id
							WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

							SET @RowsDeleted = @@ROWCOUNT;
						COMMIT TRANSACTION
						Print 'Rows Affected AssetSummaryDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
							ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all
  							Set @RetMessage = 'Failed';
							Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							Print 'Rolled Back AssetSummaryDetailValueIntId Delete Transaction in SimulationOutput SP:  ' + @ErrorMessage;
							ROLLBACK TRANSACTION;
							RAISERROR  (@RetMessage, 16, 1); 
							Return -1;
					END CATCH;
				END

            ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in AssetSummaryDetailValueIntId'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-----------------------------------------------------------------------

    		-------SimulationOutput --> AssetSummaryDetail -----

            BEGIN TRY

          ALTER TABLE AssetSummaryDetail NOCHECK CONSTRAINT all

			Delete l2 
			FROM SimulationOutput AS l1
			JOIN AssetSummaryDetail AS l2 ON l2.SimulationOutputId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

            ALTER TABLE AssetSummaryDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in AssetSummaryDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH



			----------------------------------------------------------------------
			-----End AssetSummaryDetail Path--------------------------------------

			-----Start SimulationYearDetail Path-----------------------------------------

			--SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail 

			BEGIN TRY

            ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all

			Delete l5 
			FROM SimulationOutput AS l1
			JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			JOIN BudgetUsageDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

            ALTER TABLE BudgetUsageDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in BudgetUsageDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------

			--SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail

			BEGIN TRY

            ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all

			Delete l5 
			FROM SimulationOutput AS l1
			JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			JOIN CashFlowConsiderationDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

            ALTER TABLE CashFlowConsiderationDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CashFlowConsiderationDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------
	
		--SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail 

			BEGIN TRY

            ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all

			Delete l4 
			FROM SimulationOutput AS l1
			JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

            ALTER TABLE TreatmentConsiderationDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in TreatmentConsiderationDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------

--SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentOptionDetail -->  -->  -->  -->  -->  --> 

			BEGIN TRY

            ALTER TABLE TreatmentOptionDetail NOCHECK CONSTRAINT all

			Delete l4 
			FROM SimulationOutput AS l1
			JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			JOIN TreatmentOptionDetail AS l4 ON l4.AssetDetailId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

            ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in TreatmentOptionDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	-----------------------------------------------------------------

	--SimulationOutput --> SimulationYearDetail --> AssetDetail --> AssetDetailValueIntId -->  -->  -->  -->  -->  --> 
	
			BEGIN TRY

            ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all

			SET @RowsDeleted = 1;
		  	--Print 'AssetDetailValueIntId ';

		   WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
						Begin Transaction
						Delete TOP (@BatchSize) l4 
						FROM SimulationOutput AS l1
						JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
						JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
						JOIN AssetDetailValueIntId AS l4 ON l4.AssetDetailId = l3.Id
						WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

						SET @RowsDeleted = @@ROWCOUNT;
						COMMIT TRANSACTION
						Print 'Rows Affected AssetDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
							ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all
  							Set @RetMessage = 'Failed';
							Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							Print 'Rolled Back AssetDetailValueIntId Delete Transaction in SimulationOutput SP  ' + @ErrorMessage;
							ROLLBACK TRANSACTION;
							RAISERROR  (@RetMessage, 16, 1); 
							Return -1;
					END CATCH;
				END

            ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in AssetDetailValueIntId'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
		-----------------------------------------------------------------

		--SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentSchedulingCollisionDetail -->  -->  -->  -->  -->  --> 

			BEGIN TRY

            ALTER TABLE TreatmentRejectionDetail NOCHECK CONSTRAINT all
		  
		   SET @RowsDeleted = 1;
		  --Print 'TreatmentRejectionDetail ';

		   WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
						Begin Transaction

						Delete TOP (@BatchSize) l4
						FROM SimulationOutput AS l1
						JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
						JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
						JOIN TreatmentRejectionDetail AS l4 ON l4.AssetDetailId = l3.Id
						WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

 						SET @RowsDeleted = @@ROWCOUNT;
						COMMIT TRANSACTION
						Print 'Rows Affected TreatmentRejectionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
							ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all
  							Set @RetMessage = 'Failed';
							Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							Print 'Rolled Back TreatmentRejectionDetail Delete Transaction in SimulationOutput SP  ' + @ErrorMessage;
							ROLLBACK TRANSACTION;
							RAISERROR  (@RetMessage, 16, 1); 
							Return -1;
					END CATCH;
				END		
			
			ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in TreatmentRejectionDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	-----------------------------------------------------------------

	--SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentSchedulingCollisionDetail 


			BEGIN TRY

            ALTER TABLE TreatmentSchedulingCollisionDetail NOCHECK CONSTRAINT all

			Delete l4 
			FROM SimulationOutput AS l1
			JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			JOIN TreatmentSchedulingCollisionDetail AS l4 ON l4.AssetDetailId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

            ALTER TABLE TreatmentSchedulingCollisionDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in TreatmentSchedulingCollisionDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	-----------------------------------------------------------------


			--SimulationOutput\SimulationYearDetail\AssetDetail
			
			BEGIN TRY

            ALTER TABLE AssetDetail NOCHECK CONSTRAINT all

			Delete l3 
			FROM SimulationOutput AS l1
			JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			JOIN AssetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

            ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in AssetDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	-----------------------------------------------------------------


			--SimulationOutput\SimulationYearDetail\BudgetDetail
			
			BEGIN TRY

            ALTER TABLE BudgetDetail NOCHECK CONSTRAINT all

			Delete l3 
			FROM SimulationOutput AS l1
			JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			JOIN BudgetDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

            ALTER TABLE BudgetDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in BudgetDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------


				--SimulationOutput\SimulationYearDetail\DeficientConditionGoalDetail
			
			BEGIN TRY

            ALTER TABLE DeficientConditionGoalDetail NOCHECK CONSTRAINT all

			Delete l3 
			FROM SimulationOutput AS l1
			JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			JOIN DeficientConditionGoalDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

            ALTER TABLE DeficientConditionGoalDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in DeficientConditionGoalDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------

	
				--SimulationOutput\SimulationYearDetail\TargetConditionGoalDetail
			
			BEGIN TRY

            ALTER TABLE TargetConditionGoalDetail NOCHECK CONSTRAINT all

			Delete l3 
			FROM SimulationOutput AS l1
			JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			JOIN TargetConditionGoalDetail AS l3 ON l3.SimulationYearDetailId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

            ALTER TABLE TargetConditionGoalDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in TargetConditionGoalDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------

			--SimulationOutput\SimulationYearDetail
			
			BEGIN TRY

            ALTER TABLE SimulationYearDetail NOCHECK CONSTRAINT all

			Delete l2 
			FROM SimulationOutput AS l1
			JOIN SimulationYearDetail AS l2 ON l2.SimulationOutputId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

            ALTER TABLE SimulationYearDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in SimulationYearDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------------------------------------------------------------------

  			--SimulationOutput

            BEGIN TRY

            ALTER TABLE SimulationOutput NOCHECK CONSTRAINT all

			Delete l1 
			FROM  SimulationOutput AS l1
			WHERE l1.Id IN (SELECT Guid FROM #SimOutputTempGuids);

			ALTER TABLE SimulationOutput WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in SimulationOutput'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

		   ---------------------------------------------------------------------------
			------End SimulationOutput Delete-------------------------------------------------------------------

	DROP TABLE #SimOutputTempGuids;
    COMMIT TRANSACTION
    Print 'Delete Transaction Committed';
   	RAISERROR (@RetMessage, 0, 1);
	END TRY
	BEGIN CATCH
  			Set @RetMessage = 'Failed ' + @RetMessage;
			Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			Print 'Rolled Back Entire Delete Transaction in SimulationOutput SP:  ' + @ErrorMessage;
			ROLLBACK TRANSACTION;
			RAISERROR  (@RetMessage, 16, 1);  
	END CATCH;

END 


