
Create PROCEDURE dbo.usp_delete_network(
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

		Begin Transaction
	BEGIN TRY
-----------------------------------------------------------------------

--Network --> BenefitQuantifier - Done
--Network --> NetworkRollupDetail- Done
--Network --> NetworkAttribute - Done
--Network --> AnalysisMaintainableAsset- Done
--Network --> MaintainableAsset- Done
--Network --> Simulation - Done
--Network --> Network- Done


-----Start BenefitQuantifier Path-----------------------------------------

			--Network --> BenefitQuantifier

            BEGIN TRY

          ALTER TABLE BenefitQuantifier NOCHECK CONSTRAINT all

			Delete l2 
			FROM Network AS l1
			JOIN BenefitQuantifier AS l2 ON l2.NetworkId = l1.Id
			WHERE l1.Id IN (@NetworkId);

            ALTER TABLE BenefitQuantifier WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network --> BenefitQuantifier'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------End BenefitQuantifier------------------------------------------

-----Start NetworkRollupDetail Path-----------------------------------------

			--Network --> NetworkRollupDetail

            BEGIN TRY

          ALTER TABLE NetworkRollupDetail NOCHECK CONSTRAINT all

			Delete l2 
			FROM Network AS l1
			JOIN NetworkRollupDetail AS l2 ON l2.NetworkId = l1.Id
			WHERE l1.Id IN (@NetworkId);

            ALTER TABLE NetworkRollupDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network -->  NetworkRollupDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------End NetworkRollupDetail------------------------------------------
			-----Start NetworkAttribute Path-----------------------------------------

			--Network --> NetworkAttribute

            BEGIN TRY

          ALTER TABLE NetworkAttribute NOCHECK CONSTRAINT all

			Delete l2 
			FROM Network AS l1
			JOIN NetworkAttribute AS l2 ON l2.NetworkId = l1.Id
			WHERE l1.Id IN (@NetworkId);

            ALTER TABLE NetworkAttribute WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network --> NetworkAttribute'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------End NetworkAttribute------------------------------------------
			-----Start AnalysisMaintainableAsset Path-----------------------------------------

			--Network --> AnalysisMaintainableAsset

            BEGIN TRY

          ALTER TABLE AnalysisMaintainableAsset NOCHECK CONSTRAINT all

			Delete l2 
			FROM Network AS l1
			JOIN AnalysisMaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			WHERE l1.Id IN (@NetworkId);

            ALTER TABLE AnalysisMaintainableAsset WITH CHECK CHECK CONSTRAINT all;


            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network --> AnalysisMaintainableAsset'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

   COMMIT TRANSACTION
    -- for simple recovery model
    Print 'Partial Network Delete Transaction Committed in Network --> AnalysisMaintainableAsset';
	END TRY
	BEGIN CATCH
  			Set @RetMessage = 'Failed';
			Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			Print 'Rolled Back Network Delete Transaction in Stored Procedure:  ' + @ErrorMessage;
			ROLLBACK TRANSACTION;
			RAISERROR  (@RetMessage, 16, 1); 
			Return -1;
	END CATCH;


    ------End AnalysisMaintainableAsset------------------------------------------
	----- Start MaintainableAsset Path
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
						Join MaintainableAsset AS l2 ON l2.NetworkId = l2.Id
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

			-----Start AssetSummaryDetail Path-----------------------------------------

			--MaintainableAsset --> AssetSummaryDetail
			--MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId -->  -->  --> 

            BEGIN TRY

          ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all
		  SET @RowsDeleted = 1;
		  Print 'AssetSummaryDetailValueIntId ';

			WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
					Begin Transaction

						--Delete TOP (@BatchSize) l4 
						--FROM Network AS l1
						--Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
						--JOIN AssetSummaryDetail AS l3 ON l3.MaintainableAssetId = l2.Id
						--Join AssetSummaryDetailValueIntId As l4 ON l4.AssetSummaryDetailId = l3.Id
						--WHERE l1.Id IN (@NetworkId);

						SELECT TOP (@BatchSize) l4.Id INTO #tempAssetSummaryDetailId
						FROM Network AS l1
						Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
						JOIN AssetSummaryDetail AS l3 ON l3.MaintainableAssetId = l2.Id
						Join AssetSummaryDetailValueIntId As l4 ON l4.AssetSummaryDetailId = l3.Id
						WHERE l1.Id IN (@NetworkId);

						--DELETE asd
						--FROM AssetSummaryDetailValueIntId As asd
						--JOIN #tempAssetSummaryDetailId T ON T.Id = asd.Id;

						DELETE FROM AssetSummaryDetailValueIntId WHERE Id in (SELECT Id FROM #tempAssetSummaryDetailId);

						SET @RowsDeleted = @@ROWCOUNT;

						DROP TABLE #tempAssetSummaryDetailId;
						--WAITFOR DELAY '00:00:01';

						COMMIT TRANSACTION
						
						Print 'Rows Affected MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
						ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all
						Set @RetMessage = 'Failed';
						Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
						Print 'Rolled Back AssetSummaryDetailValueIntId Delete Transaction in NetworkDelete SP:  ' + @ErrorMessage;
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

 		         SELECT @CustomErrorMessage = 'Query Error in Network --> MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntI'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-----------------------------------------------------------------------

			--Restart Transaction
			Begin Transaction
			BEGIN TRY

    		-------MaintainableAsset --> AssetSummaryDetail -----

            BEGIN TRY

			ALTER TABLE AssetSummaryDetail NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			JOIN AssetSummaryDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			WHERE l1.Id IN (@NetworkId);

 		    SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected --MaintainableAsset --> AssetSummaryDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
 
			ALTER TABLE AssetSummaryDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network --> MaintainableAsset --> AssetSummaryDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH


			----------------------------------------------------------------------
			-----End AssetSummaryDetail Path--------------------------------------


		BEGIN TRY

		--MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail -->  --> 

            ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all

		    Delete l5 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l2.Id
			JOIN BudgetUsageDetail AS l5 ON l5.TreatmentConsiderationDetailId = l3.Id
			WHERE l1.Id IN (@NetworkId);

		    SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

            ALTER TABLE BudgetUsageDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network -> BudgetUsageDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------

				-- MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail -->  --> 

			BEGIN TRY

            ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all

				Delete l5 
				FROM Network AS l1
				Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
				Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
				JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l2.Id
				JOIN CashFlowConsiderationDetail AS l5 ON l5.TreatmentConsiderationDetailId = l3.Id
				WHERE l1.Id IN (@NetworkId);

				SET @RowsDeleted = @@ROWCOUNT;
				Print 'Rows Affected --MaintainableAsset --> AssetDetail --> CashFlowConsiderationDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);


            ALTER TABLE CashFlowConsiderationDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network -> CashFlowConsiderationDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------
	
		--MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail 

			BEGIN TRY

            ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all

		    Delete l4
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l2.Id
			WHERE l1.Id IN (@NetworkId);
			
			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

            ALTER TABLE TreatmentConsiderationDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network -> TreatmentConsiderationDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------

	-- Network --> MaintainableAsset --> AssetDetail --> AssetDetailValueIntId -


			BEGIN TRY

            ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			JOIN AssetDetailValueIntId AS l4 ON l4.AssetDetailId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network --> MaintainableAsset --> AssetDetail --> AssetDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);

            ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network -> AssetDetailValueIntId'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
		-----------------------------------------------------------------

			-- MaintainableAsset --> AssetDetail --> TreatmentOptionDetail -->  -->  --> 

			BEGIN TRY

            ALTER TABLE TreatmentOptionDetail NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			JOIN TreatmentOptionDetail AS l4 ON l4.AssetDetailId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected MaintainableAsset --> AssetDetail --> TreatmentOptionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

            ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network -> TreatmentOptionDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

   COMMIT TRANSACTION
   
    Print 'Partial Network Delete Transaction Committed in Network --> MaintainableAsset --> AssetDetail --> TreatmentOptionDetail';
	END TRY
	BEGIN CATCH
  			Set @RetMessage = 'Failed';
			Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			Print 'Rolled Back Network Delete Transaction in Stored Procedure:  ' + @ErrorMessage;
			ROLLBACK TRANSACTION;
			RAISERROR  (@RetMessage, 16, 1); 
			Return -1;
	END CATCH;
	
	-----------------------------------------------------------------

		-- AssetDetail --> TreatmentRejectionDetail 

			BEGIN TRY

            ALTER TABLE TreatmentRejectionDetail NOCHECK CONSTRAINT all
  		    Print 'TreatmentRejectionDetail ';

			WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
					Begin Transaction

				--Delete l4 
				SELECT TOP (@BatchSize) l4.Id  INTO #tempTreatmentRejectionDet
				FROM Network AS l1
				Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
				Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
				JOIN TreatmentRejectionDetail AS l4 ON l4.AssetDetailId = l1.Id
				WHERE l1.Id IN (@NetworkId);

				--DELETE tr
				--FROM TreatmentRejectionDetail As tr
				--JOIN #tempTreatmentRejectionDet T ON T.Id = tr.Id;

				DELETE FROM TreatmentRejectionDetail WHERE Id in (SELECT Id FROM #tempTreatmentRejectionDet);

				SET @RowsDeleted = @@ROWCOUNT;

				DROP TABLE #tempTreatmentRejectionDet;
				--WAITFOR DELAY '00:00:01';
				
				COMMIT TRANSACTION
				
				Print 'Rows Affected Network --> MaintainableAsset-->AssetDetail --> TreatmentRejectionDetail : ' +  convert(NVARCHAR(50), @RowsDeleted);
            
					END TRY
					BEGIN CATCH
						ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all
						Set @RetMessage = 'Failed';
						Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
						Print 'Rolled Back Network --> MaintainableAsset-->AssetDetail --> TreatmentRejectionDetail  Delete Transaction in NetworkDelete SP:  ' + @ErrorMessage;
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

 		         SELECT @CustomErrorMessage = 'Query Error in Network -> TreatmentRejectionDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	-----------------------------------------------------------------

	-- AssetDetail --> TreatmentSchedulingCollisionDetail 


			BEGIN TRY

            ALTER TABLE TreatmentSchedulingCollisionDetail NOCHECK CONSTRAINT all

			Begin Transaction

			Delete l4 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			JOIN TreatmentSchedulingCollisionDetail AS l4 ON l4.AssetDetailId = l1.Id
			WHERE l1.Id IN (@NetworkId);

 			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network --> MaintainableAsset-->AssetDetail --> TreatmentSchedulingCollisionDetail : ' +  convert(NVARCHAR(50), @RowsDeleted);

			 COMMIT TRANSACTION
			 
			Print 'Partial Network Delete Transaction Committed at Network --> MaintainableAsset-->AssetDetail --> TreatmentSchedulingCollisionDetail';
			END TRY
			BEGIN CATCH
			ALTER TABLE TreatmentSchedulingCollisionDetail WITH CHECK CHECK CONSTRAINT all
  					Set @RetMessage = 'Failed';
					Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
					Print 'Rolled Back Network Delete Transaction in Stored Procedure:  ' + @ErrorMessage;
					SELECT @CustomErrorMessage = 'Query Error in Network -> TreatmentSchedulingCollisionDetail'
		            SELECT ERROR_NUMBER() AS ErrorNumber
                    ,ERROR_SEVERITY() AS ErrorSeverity
                    ,ERROR_STATE() AS ErrorState
                    ,ERROR_PROCEDURE() AS ErrorProcedure
                    ,ERROR_LINE() AS ErrorLine
                    ,ERROR_MESSAGE() AS ErrorMessage;
					
					Set @RetMessage = @CustomErrorMessage;
					ROLLBACK TRANSACTION;
					RAISERROR  (@RetMessage, 16, 1); 
					Return -1;
			END CATCH;

			ALTER TABLE TreatmentSchedulingCollisionDetail WITH CHECK CHECK CONSTRAINT all

	
	-----------------------------------------------------------------

			--MaintainableAsset\AssetDetail
			
			BEGIN TRY

            ALTER TABLE AssetDetail NOCHECK CONSTRAINT all

			SET @RowsDeleted = 1;
		  Print 'AssetDetail ';

		   WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
						Begin Transaction

							--Delete  TOP (@BatchSize) l3
							SELECT TOP (@BatchSize) l3.Id INTO #tempAssetDetailId
							FROM Network AS l1
							Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
							Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
							WHERE l1.Id IN (@NetworkId);

							--DELETE ad
							--FROM AssetDetail As ad
							--JOIN #tempAssetDetailId T ON T.Id = ad.Id;

							DELETE FROM AssetDetail WHERE Id in (SELECT Id FROM #tempAssetDetailId);

						    SET @RowsDeleted = @@ROWCOUNT;

							DROP TABLE #tempAssetDetailId;
							----WAITFOR DELAY '00:00:01';

						COMMIT TRANSACTION
						
					Print 'Rows Affected Network --> MaintainableAsset-->AssetDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
				END TRY
				BEGIN CATCH
						ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all
  						Set @RetMessage = 'Failed';
						Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
						Print 'Rolled Back AssetDetail Delete Transaction in NetworkDelete SP:  ' + @ErrorMessage;
						ROLLBACK TRANSACTION;
						RAISERROR  (@RetMessage, 16, 1); 
						Return -1;
				END CATCH;
			END

            ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network -> AssetDetail'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH


		---End  --Network -->MaintainableAsset --> AssetDetail Path---------

		--Restart Transaction
			Begin Transaction
			BEGIN TRY

		----- Start CommittedProject Path
		--Network --> MaintainableAsset --> CommittedProject --> CommittedProjectConsequence -->  -->  -->  -->  -->  --> 
		--Network --> MaintainableAsset --> CommittedProject --> CommittedProjectLocation -->  -->  -->  -->  -->  -->
		--Network --> MaintainableAsset --> CommittedProject 

		PRINT 'Start CommittedProject Path;'

			-----Start CommittedProject Path-----------------------------------------
			
		--Network --> MaintainableAsset --> CommittedProject --> CommittedProjectConsequence -->  -->  -->  -->  -->  --> 


         BEGIN TRY

            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join CommittedProject  AS l3 ON l3.MaintainableAssetEntityId = l2.Id
			JOIN CommittedProjectConsequence AS l4 ON l4.CommittedProjectId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network Delete CommittedProjectConsequence: ' +  convert(NVARCHAR(50), @RowsDeleted);

			ALTER TABLE CommittedProjectConsequence WITH CHECK CHECK CONSTRAINT all
 	
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CommittedProjectConsequence'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-------------------------------------

			--Network --> MaintainableAsset --> CommittedProject --> CommittedProjectLocation -->  -->  -->  -->  -->  -->

			 BEGIN TRY

            ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join CommittedProject  AS l3 ON l3.MaintainableAssetEntityId = l2.Id
			JOIN CommittedProjectLocation AS l4 ON l4.CommittedProjectId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network Delete CommittedProjectLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);

			ALTER TABLE CommittedProjectLocation WITH CHECK CHECK CONSTRAINT all
 	
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CommittedProjectLocation'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			--------------------------------------

			--Network --> MaintainableAsset --> CommittedProject 

            BEGIN TRY

			ALTER TABLE CommittedProject NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join CommittedProject AS l3 ON l3.MaintainableAssetEntityId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network Delete CommittedProject: ' +  convert(NVARCHAR(50), @RowsDeleted);

			ALTER TABLE CommittedProject WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CommittedProject'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH 

			-------------------------------------

			-----End CommittedProject Path-----------------------------------------

		----Start Network --> MaintainableAsset --> MaintainableAssetLocation  Path
			
			BEGIN TRY

            ALTER TABLE MaintainableAssetLocation NOCHECK CONSTRAINT all

		  	Print 'MaintainableAssetLocation ';

			Delete l3
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			JOIN MaintainableAssetLocation AS l3 ON l3.MaintainableAssetId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network --> MaintainableAsset-->MaintainableAssetLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);

            ALTER TABLE MaintainableAssetLocation WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network --> MaintainableAsset-->MaintainableAssetLocation'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

	-------End -------MaintainableAssetLocation----------------------------------------------------

			-- Start MaintainableAsset

 			BEGIN TRY

            ALTER TABLE MaintainableAsset NOCHECK CONSTRAINT all

			Print 'MaintainableAsset';

			Delete l2 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network --> MaintainableAsset: ' +  convert(NVARCHAR(50), @RowsDeleted);

            ALTER TABLE MaintainableAsset WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network --> MaintainableAsset'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------Network --> MaintainableAsset Path--------------------------------------------------------------------

					   			 
		----- Start Simulation Path
			
		    --SET @CurrentDateTime = GETDATE();
			--PRINT 'Start Simulation Delete: ' + CONVERT(NVARCHAR, @CurrentDateTime, 120);
			-----Start AnalysisMethod Path-----------------------------------------

            BEGIN TRY
 
            ALTER TABLE Benefit NOCHECK CONSTRAINT all

			--AnalysisMethod	Benefit							FK_Benefit_AnalysisMethod_AnalysisMethodId
			--AnalysisMethod	CriterionLibrary_AnalysisMethod	FK_CriterionLibrary_AnalysisMethod_AnalysisMethod_AnalysisMethodId

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN AnalysisMethod AS l3 ON l3.SimulationId = l2.Id
			JOIN Benefit AS l4 ON l4.AnalysisMethodId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network --> Benefit: ' +  convert(NVARCHAR(50), @RowsDeleted);

            ALTER TABLE Benefit WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Benefit'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-----------------------------------------------------------------------

            BEGIN TRY


            ALTER TABLE CriterionLibrary_AnalysisMethod NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN AnalysisMethod AS l3 ON l3.SimulationId = l2.Id
			JOIN CriterionLibrary_AnalysisMethod AS l4 ON l4.AnalysisMethodId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network --> CriterionLibrary_AnalysisMethod: ' +  convert(NVARCHAR(50), @RowsDeleted);

            ALTER TABLE CriterionLibrary_AnalysisMethod WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_AnalysisMethod'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
			------------------------------------------------------------------

            BEGIN TRY

            ALTER TABLE AnalysisMethod NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN AnalysisMethod AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network --> AnalysisMethod: ' +  convert(NVARCHAR(50), @RowsDeleted);

			ALTER TABLE AnalysisMethod WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in AnalysisMethod'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
 
 			-----End Simulation --> AnalysisMethod Path-----------------------------------------
 			-----------------------------------------------------------------------
			-----Start Simulation --> CommittedProject Path-----------------------------------------

			--Simulation --> CommittedProject --> CommittedProjectConsequence 

         BEGIN TRY

            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN CommittedProject AS l3 ON l3.SimulationId = l2.Id
			JOIN CommittedProjectConsequence AS l4 ON l4.CommittedProjectId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network --> CommittedProjectConsequence: ' +  convert(NVARCHAR(50), @RowsDeleted);

			ALTER TABLE CommittedProjectConsequence WITH CHECK CHECK CONSTRAINT all
 	
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CommittedProjectConsequence'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-------------------------------------

			--Simulation --> CommittedProject --> CommittedProjectLocation 

			 BEGIN TRY

            ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN CommittedProject AS l3 ON l3.SimulationId = l2.Id
			JOIN CommittedProjectLocation AS l4 ON l4.CommittedProjectId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network --> CommittedProjectLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);

			ALTER TABLE CommittedProjectLocation WITH CHECK CHECK CONSTRAINT all
 	
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CommittedProjectLocation'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			--------------------------------------

			--Simulation --> CommittedProject

            BEGIN TRY

			ALTER TABLE CommittedProject NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN CommittedProject AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId)

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected Network --> CommittedProject: ' +  convert(NVARCHAR(50), @RowsDeleted);

			ALTER TABLE CommittedProject WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CommittedProject'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH 


			-----End CommittedProject Path-----------------------------------------


		   -----Start Simulation --> InvestmentPlan 


            BEGIN TRY

			Print 'InvestmentPlan';
			ALTER TABLE InvestmentPlan NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN InvestmentPlan AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE InvestmentPlan WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in InvestmentPlan'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-----End Simulation --> InvestmentPlan 

		-----Start Simulation --> ReportIndex 


            BEGIN TRY
			Print 'ReportIndex';

			ALTER TABLE ReportIndex NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ReportIndex AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ReportIndex WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ReportIndex'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-----End Simulation --> ReportIndex ---------------------------
			-- Start Network --> Simulation --> ScenarioBudget --> BudgetPercentagePair -->

            BEGIN TRY
			Print 'BudgetPercentagePair';

			ALTER TABLE BudgetPercentagePair NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			JOIN BudgetPercentagePair AS l4 ON l4.ScenarioBudgetId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE BudgetPercentagePair WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in BudgetPercentagePair'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-- End Simulation --> ScenarioBudget --> BudgetPercentagePair -->

			-- Start Simulation --> ScenarioBudget --> CommittedProjectConsequence -->

            BEGIN TRY

			Print 'CommittedProjectConsequence';

            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			JOIN CommittedProject AS l4 ON l4.ScenarioBudgetId = l3.Id
			JOIN CommittedProjectConsequence AS l5 ON l5.CommittedProjectId = l4.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CommittedProjectConsequence WITH CHECK CHECK CONSTRAINT all
 	
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CommittedProjectConsequence'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-------------------------------------

			--Simulation --> ScenarioBudget --> CommittedProject --> CommittedProjectLocation

			 BEGIN TRY

            ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			JOIN CommittedProject AS l4 ON l4.ScenarioBudgetId = l3.Id
			JOIN CommittedProjectLocation AS l5 ON l5.CommittedProjectId = l4.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CommittedProjectLocation WITH CHECK CHECK CONSTRAINT all
 	
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CommittedProjectLocation'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			--------------------------------------

		--Simulation --> ScenarioBudget --> CommittedProject

            BEGIN TRY

			ALTER TABLE CommittedProject NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			JOIN CommittedProject AS l4 ON l4.ScenarioBudgetId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CommittedProject WITH CHECK CHECK CONSTRAINT all

            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CommittedProject'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH 

			-----End ScenarioBudget -CommittedProject Path------------

		-----Start Network --> Simulation --> ScenarioBudget --> CriterionLibrary_ScenarioBudget

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioBudget NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			JOIN CriterionLibrary_ScenarioBudget AS l4 ON l4.ScenarioBudgetId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_ScenarioBudget WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioBudget'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
			-----------------------------------------

			--Network --> Simulation --> ScenarioBudget --> ScenarioBudgetAmount --> 

            BEGIN TRY

			Print 'ScenarioBudgetAmount';
			
			ALTER TABLE ScenarioBudgetAmount NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioBudgetAmount AS l4 ON l4.ScenarioBudgetId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioBudgetAmount WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioBudgetAmount'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			----------------------------------------------

		--Network --> Simulation --> ScenarioBudget --> ScenarioSelectableTreatment_ScenarioBudget --> 

            BEGIN TRY

			Print 'ScenarioSelectableTreatment_ScenarioBudget';

			ALTER TABLE ScenarioSelectableTreatment_ScenarioBudget NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioSelectableTreatment_ScenarioBudget AS l4 ON l4.ScenarioBudgetId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioSelectableTreatment_ScenarioBudget WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioSelectableTreatment_ScenarioBudget'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
  
			------------------------------------------------------------

			--Network --> Simulation --> ScenarioBudget 

            BEGIN TRY

			Print 'ScenarioBudget';

			ALTER TABLE ScenarioBudget NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioBudget AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioBudget WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioBudget'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-----End --Network --> Simulation --> ScenarioBudget Path-----------------------------------------
			------Start  --Network --> Simulation --> ScenarioBudgetPriority -----------

			--Network --> Simulation --> ScenarioBudgetPriority --> CriterionLibrary_ScenarioBudgetPriority --> 

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioBudgetPriority NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioBudgetPriority AS l3 ON l3.SimulationId = l2.Id
			JOIN CriterionLibrary_ScenarioBudgetPriority AS l4 ON l4.ScenarioBudgetPriorityId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_ScenarioBudgetPriority WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioBudgetPriority'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------------------------------------------------------------

			--Network --> Simulation --> ScenarioBudgetPriority --> BudgetPercentagePair --> 

            BEGIN TRY

			Print 'BudgetPercentagePair';

			ALTER TABLE BudgetPercentagePair NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioBudgetPriority AS l3 ON l3.SimulationId = l2.Id
			JOIN BudgetPercentagePair AS l4 ON l4.ScenarioBudgetPriorityId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE BudgetPercentagePair WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in BudgetPercentagePair'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH


			------------------------------------------------------------

			--Network --> Simulation --> ScenarioBudgetPriority

            BEGIN TRY

			ALTER TABLE ScenarioBudgetPriority NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioBudgetPriority AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioBudgetPriority WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioBudgetPriority'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------End Network -->  Simulation --> ScenarioBudgetPriority-------------------------------------------------------------------

			------Start  ScenarioCalculatedAttribute-------------------------------------------------------------------
			--Network --> Simulation --> ScenarioCalculatedAttribute --> ScenarioCalculatedAttributePair --> ScenarioCalculatedAttributePair_Criteria
			--Network --> Simulation --> ScenarioCalculatedAttribute --> ScenarioCalculatedAttributePair --> ScenarioCalculatedAttributePair_Equation


            BEGIN TRY

			ALTER TABLE ScenarioCalculatedAttributePair_Criteria NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioCalculatedAttribute AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioCalculatedAttributePair AS l4 ON l4.ScenarioCalculatedAttributeId = l3.Id
			JOIN ScenarioCalculatedAttributePair_Criteria AS l5 ON l5.ScenarioCalculatedAttributePairId = l4.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioCalculatedAttributePair_Criteria WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioCalculatedAttributePair_Criteria'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------------------------------------------------------------

			--Network --> Simulation --> ScenarioCalculatedAttribute --> ScenarioCalculatedAttributePair --> ScenarioCalculatedAttributePair_Equation

            BEGIN TRY

			ALTER TABLE ScenarioCalculatedAttributePair_Equation NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioCalculatedAttribute AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioCalculatedAttributePair AS l4 ON l4.ScenarioCalculatedAttributeId = l3.Id
			JOIN ScenarioCalculatedAttributePair_Equation AS l5 ON l5.ScenarioCalculatedAttributePairId = l4.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioCalculatedAttributePair_Equation WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioCalculatedAttributePair_Equation'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------------------------------------------------------------
			--Network --> Simulation --> ScenarioCalculatedAttribute --> ScenarioCalculatedAttributePair 

			BEGIN TRY

			ALTER TABLE ScenarioCalculatedAttributePair NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioCalculatedAttribute AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioCalculatedAttributePair AS l4 ON l4.ScenarioCalculatedAttributeId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioCalculatedAttributePair WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioCalculatedAttributePair'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------
			--Network --> Simulation --> ScenarioCalculatedAttribute 

            BEGIN TRY

			Print 'ScenarioCalculatedAttribute';

			ALTER TABLE ScenarioCalculatedAttribute NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioCalculatedAttribute AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioCalculatedAttribute WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioCalculatedAttribute'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			----End --Network --> Simulation --> ScenarioCalculatedAttribute-------------------------------------------------------------------

			------Start  Network --> Simulation --> ScenarioCashFlowRule-------------------------------------------------------------------
			----Network --> Simulation --> ScenarioCashFlowRule --> CriterionLibrary_ScenarioCashFlowRule --> 

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioCashFlowRule NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioCashFlowRule AS l3 ON l3.SimulationId = l2.Id
			JOIN CriterionLibrary_ScenarioCashFlowRule AS l4 ON l4.ScenarioCashFlowRuleId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_ScenarioCashFlowRule WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioCashFlowRule'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------------------------------------------------------------

			--Network --> Simulation --> ScenarioCashFlowRule --> ScenarioCashFlowDistributionRule --> 

			BEGIN TRY

			ALTER TABLE ScenarioCashFlowDistributionRule NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioCashFlowRule AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioCashFlowDistributionRule AS l4 ON l4.ScenarioCashFlowRuleId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioCashFlowDistributionRule WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioCashFlowDistributionRule'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------
			--Network --> Simulation --> ScenarioCashFlowRule 

            BEGIN TRY

			ALTER TABLE ScenarioCashFlowRule NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioCashFlowRule AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioCashFlowRule WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioCashFlowRule'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------End ScenarioCashFlowRule-------------------------------------------------------------------
			------Start  ScenarioDeficientConditionGoal-------------------------------------------------------------------
			--Network --> Simulation --> ScenarioDeficientConditionGoal --> CriterionLibrary_ScenarioDeficientConditionGoal --> 

            BEGIN TRY

			Print 'CriterionLibrary_ScenarioDeficientConditionGoal';

			ALTER TABLE CriterionLibrary_ScenarioDeficientConditionGoal NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioDeficientConditionGoal AS l3 ON l3.SimulationId = l2.Id
			JOIN CriterionLibrary_ScenarioDeficientConditionGoal AS l4 ON l4.ScenarioDeficientConditionGoalId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_ScenarioDeficientConditionGoal WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioDeficientConditionGoal'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------------------------------------------------------------

			--Network --> Simulation --> ScenarioDeficientConditionGoal 

            BEGIN TRY

			ALTER TABLE ScenarioDeficientConditionGoal NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioDeficientConditionGoal AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioDeficientConditionGoal WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioDeficientConditionGoal'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------End ScenarioDeficientConditionGoal-------------------------------------------------------------------
			------Start  ScenarioPerformanceCurve-------------------------------------------------------------------
			--Network --> Simulation --> ScenarioPerformanceCurve --> CriterionLibrary_ScenarioPerformanceCurve --> 
			--Network --> Simulation --> ScenarioPerformanceCurve --> ScenarioPerformanceCurve_Equation --> 

            BEGIN TRY

			Print 'CriterionLibrary_ScenarioPerformanceCurve';

			ALTER TABLE CriterionLibrary_ScenarioPerformanceCurve NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioPerformanceCurve AS l3 ON l3.SimulationId = l2.Id
			JOIN CriterionLibrary_ScenarioPerformanceCurve AS l4 ON l4.ScenarioPerformanceCurveId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_ScenarioPerformanceCurve WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioPerformanceCurve'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------------------------------------------------------------
			--Network --> Simulation --> ScenarioPerformanceCurve --> ScenarioPerformanceCurve_Equation --> 

            BEGIN TRY

			ALTER TABLE ScenarioPerformanceCurve_Equation NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioPerformanceCurve AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioPerformanceCurve_Equation AS l4 ON l4.ScenarioPerformanceCurveId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioPerformanceCurve_Equation WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioPerformanceCurve_Equation'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------

			--Network --> Simulation --> ScenarioPerformanceCurve

            BEGIN TRY

			Print 'ScenarioPerformanceCurve';

			ALTER TABLE ScenarioPerformanceCurve NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioPerformanceCurve AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioPerformanceCurve WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioPerformanceCurve'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------End ScenarioPerformanceCurve-------------------------------------------------------------------
			------Start ScenarioRemainingLifeLimit-------------------------------------------------------------------

			--Network --> Simulation --> ScenarioRemainingLifeLimit --> CriterionLibrary_ScenarioRemainingLifeLimit --> 

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioRemainingLifeLimit NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioRemainingLifeLimit AS l3 ON l3.SimulationId = l2.Id
			JOIN CriterionLibrary_ScenarioRemainingLifeLimit AS l4 ON l4.ScenarioRemainingLifeLimitId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_ScenarioRemainingLifeLimit WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioRemainingLifeLimit'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------

			--Network --> Simulation --> ScenarioRemainingLifeLimit 

            BEGIN TRY

			ALTER TABLE ScenarioRemainingLifeLimit NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioRemainingLifeLimit AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioRemainingLifeLimit WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioRemainingLifeLimit'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------End ScenarioRemainingLifeLimit----------------------------------------------------------
			------Start ScenarioSelectableTreatment--------------------------------------------------------

            BEGIN TRY

			ALTER TABLE CriterionLibrary_TreatmentSupersession NOCHECK CONSTRAINT all

			--Network --> Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentSupersession --> CriterionLibrary_ScenarioTreatmentSupersession --> TreatmentSupersession 
--> CriterionLibrary_ScenarioTreatmentSupersession --> CriterionLibrary_TreatmentSupersession --> TreatmentSupersession -->  --> 


			Delete l7  
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentSupersession AS l4 ON l4.TreatmentId = l3.Id
			JOIN CriterionLibrary_ScenarioTreatmentSupersession  AS l5  ON l5.TreatmentSupersessionId = l4.Id
			JOIN TreatmentSupersession  AS l6  ON l6.CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId = l5.CriterionLibraryId
			JOIN CriterionLibrary_TreatmentSupersession  AS l7 ON l7.TreatmentSupersessionId = l6.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_TreatmentSupersession WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_TreatmentSupersession'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------

		--Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentSupersession --> CriterionLibrary_ScenarioTreatmentSupersession --> TreatmentSupersession

            BEGIN TRY

			ALTER TABLE TreatmentSupersession NOCHECK CONSTRAINT all

			Delete l6 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentSupersession AS l4 ON l4.TreatmentId = l3.Id
			JOIN  CriterionLibrary_ScenarioTreatmentSupersession  AS l5  ON l5.TreatmentSupersessionId = l4.Id
			JOIN  TreatmentSupersession  AS l6  ON l6.CriterionLibraryScenarioTreatmentSupersessionJoinTreatmentSupersessionId = l5.TreatmentSupersessionId
			    OR l6.CriterionLibraryScenarioTreatmentSupersessionJoinCriterionLibraryId = l5.CriterionLibraryId
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE TreatmentSupersession WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in TreatmentSupersession'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------
			
		--Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentSupersession --> CriterionLibrary_ScenarioTreatmentSupersession

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioTreatmentSupersession NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentSupersession AS l4 ON l4.TreatmentId = l3.Id
			JOIN CriterionLibrary_ScenarioTreatmentSupersession AS l5 ON l5.TreatmentSupersessionId = l4.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_ScenarioTreatmentSupersession WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatmentSupersession'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-----------------------------------------------------------------------

		--Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentSupersession 

            BEGIN TRY

			ALTER TABLE ScenarioTreatmentSupersession NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentSupersession AS l4 ON l4.TreatmentId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioTreatmentSupersession WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentSupersession'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------
			
		--Simulation --> ScenarioSelectableTreatment --> ScenarioConditionalTreatmentConsequences   --> CriterionLibrary_ScenarioTreatmentConsequence

            BEGIN TRY

			Print 'CriterionLibrary_ScenarioTreatmentConsequence';

			ALTER TABLE CriterionLibrary_ScenarioTreatmentConsequence NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioConditionalTreatmentConsequences AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			JOIN  CriterionLibrary_ScenarioTreatmentConsequence AS l5 ON l5.ScenarioConditionalTreatmentConsequenceId = l4.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_ScenarioTreatmentConsequence WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatmentConsequence'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
		
		---------------------------------------------------------------------------

		--Simulation --> ScenarioSelectableTreatment --> ScenarioConditionalTreatmentConsequences   --> ScenarioTreatmentConsequence_Equation

            BEGIN TRY

			--ALTER TABLE ScenarioTreatmentConsequence_Equation NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioConditionalTreatmentConsequences AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			JOIN  ScenarioTreatmentConsequence_Equation AS l5 ON l5.ScenarioConditionalTreatmentConsequenceId = l4.Id
			WHERE l1.Id IN (@NetworkId);

			--ALTER TABLE ScenarioTreatmentConsequence_Equation WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentConsequence_Equation'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------
			
			--Simulation --> ScenarioSelectableTreatment --> ScenarioConditionalTreatmentConsequences

            BEGIN TRY

			ALTER TABLE ScenarioConditionalTreatmentConsequences NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioConditionalTreatmentConsequences AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioConditionalTreatmentConsequences WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioConditionalTreatmentConsequences'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

	  	--------------------------------------------------------------------------

			--Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentCost --> ScenarioTreatmentCost_Equation

            BEGIN TRY

			--ALTER TABLE ScenarioTreatmentCost_Equation NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentCost AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			JOIN  ScenarioTreatmentCost_Equation AS l5 ON l5.ScenarioTreatmentCostId = l4.Id
			WHERE l1.Id IN (@NetworkId);

			--ALTER TABLE ScenarioTreatmentCost_Equation WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentCost_Equation'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH


		---------------------------------------------------------------------------

		--Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentCost --> CriterionLibrary_ScenarioTreatmentCost

            BEGIN TRY

			Print 'CriterionLibrary_ScenarioTreatmentCost';

			ALTER TABLE CriterionLibrary_ScenarioTreatmentCost NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentCost AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			JOIN  CriterionLibrary_ScenarioTreatmentCost AS l5 ON l5.ScenarioTreatmentCostId = l4.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_ScenarioTreatmentCost WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatmentCost'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------


		--simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentCost


            BEGIN TRY

			ALTER TABLE ScenarioTreatmentCost NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentCost AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioTreatmentCost WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentCost'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH


		---------------------------------------------------------------------------

		--Simulation --> ScenarioSelectableTreatment --> ScenarioSelectableTreatment_ScenarioBudget --> 

            BEGIN TRY

			ALTER TABLE ScenarioSelectableTreatment_ScenarioBudget NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioSelectableTreatment_ScenarioBudget AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioSelectableTreatment_ScenarioBudget WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioSelectableTreatment_ScenarioBudget'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------

			--Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentPerformanceFactor --> 

            BEGIN TRY

			Print 'ScenarioTreatmentPerformanceFactor';

			ALTER TABLE ScenarioTreatmentPerformanceFactor NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentPerformanceFactor AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioTreatmentPerformanceFactor WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentPerformanceFactor'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------
			--Simulation --> ScenarioSelectableTreatment --> CriterionLibrary_ScenarioTreatment --> 

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioTreatment NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN CriterionLibrary_ScenarioTreatment AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_ScenarioTreatment WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatment'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------

			--Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentScheduling -->

            BEGIN TRY

			ALTER TABLE ScenarioTreatmentScheduling NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentScheduling AS l4 ON l4.TreatmentId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioTreatmentScheduling WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentScheduling'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------

			--Simulation --> ScenarioSelectableTreatment 15

            BEGIN TRY

			ALTER TABLE ScenarioSelectableTreatment NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioSelectableTreatment WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioSelectableTreatment'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---End ScenarioSelectableTreatment --------------------------------------------
			---------------------------------------------------------------------------
			---Start ScenarioTargetConditionGoals --------------------------------------------

			--Simulation --> ScenarioTargetConditionGoals --> CriterionLibrary_ScenarioTargetConditionGoal --> 

            BEGIN TRY

			Print 'CriterionLibrary_ScenarioTargetConditionGoal';

			ALTER TABLE CriterionLibrary_ScenarioTargetConditionGoal NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioTargetConditionGoals AS l3 ON l3.SimulationId = l2.Id
			JOIN CriterionLibrary_ScenarioTargetConditionGoal AS l4 ON l4.ScenarioTargetConditionGoalId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_ScenarioTargetConditionGoal WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTargetConditionGoal'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------

		--Simulation --> ScenarioTargetConditionGoals 

            BEGIN TRY

			ALTER TABLE ScenarioTargetConditionGoals NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioTargetConditionGoals AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioTargetConditionGoals WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioTargetConditionGoals'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---End ScenarioTargetConditionGoals --------------------------------------------
			---------------------------------------------------------------------------

		--Simulation --> Simulation_User 

            BEGIN TRY

			ALTER TABLE Simulation_User NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN Simulation_User AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE Simulation_User WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Simulation_User'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------

		--Simulation --> SimulationAnalysisDetail 

            BEGIN TRY

			Print 'Simulation --> SimulationAnalysisDetail';

			ALTER TABLE SimulationAnalysisDetail NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationAnalysisDetail AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE SimulationAnalysisDetail WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in SimulationAnalysisDetail'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------

            BEGIN TRY

			Print 'SimulationLog';

			ALTER TABLE SimulationLog NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationLog AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE SimulationLog WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in SimulationLog'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-------End SimulationLog-----------------------------------------
			
			----Start SimulationOutput Path---------------------------------------


   COMMIT TRANSACTION
   
    Print 'Partial Network Delete Before SimulationOutput Transaction Committed';
	END TRY
	BEGIN CATCH
  			Set @RetMessage = 'Failed';
			Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			Print 'Rolled Back Network Delete Transaction in Stored Procedure:  ' + @ErrorMessage;
			ROLLBACK TRANSACTION;
			RAISERROR  (@RetMessage, 16, 1); 
			Return -1;
	END CATCH;



			-----Start AssetSummaryDetail Path-----------------------------------------

			-------SimulationOutput --> AssetSummaryDetail --> AssetSummaryDetailValueIntI----
		--SET @CurrentDateTime = GETDATE();
		--PRINT 'Start SimulationOutput Delete: ' + CONVERT(NVARCHAR, @CurrentDateTime, 120);

        BEGIN TRY

          ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all
		  SET @RowsDeleted = 1;
		  Print 'AssetSummaryDetailValueIntId ';

		   WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
						Begin Transaction

				--Delete TOP (@BatchSize) l5 
				SELECT TOP  (@BatchSize) l5.Id  INTO #tempAssetSummaryDetailValueIntId	
				FROM Network AS l1
				JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
				JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
				JOIN AssetSummaryDetail AS l4 ON l4.SimulationOutputId = l3.Id
				Join AssetSummaryDetailValueIntId As l5 ON l5.AssetSummaryDetailId = l4.Id
				WHERE l1.Id IN (@NetworkId);

				--DELETE ar
				--FROM AssetSummaryDetailValueIntId As ar
				--JOIN #tempAssetSummaryDetailValueIntId T ON T.Id = ar.Id;

				DELETE FROM AssetSummaryDetailValueIntId WHERE Id in (SELECT Id FROM #tempAssetSummaryDetailValueIntId);

				SET @RowsDeleted = @@ROWCOUNT;

				DROP TABLE #tempAssetSummaryDetailValueIntId;
				--WAITFOR DELAY '00:00:01';

				COMMIT TRANSACTION
				
				Print 'Rows Affected Network  --> Simulation --> SimulationOutput --> AssetSummaryDetail -->  AssetSummaryDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
							ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all
  							Set @RetMessage = 'Failed';
							Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							Print 'Rolled Back Simulation --> SimulationOutput --> AssetSummaryDetail --> AssetSummaryDetailValueIntId Delete Transaction in NetworkDelete SP:  ' + @ErrorMessage;
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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			-----------------------------------------------------------------
			--Restart Transaction at AssetSummaryDetail
			Begin Transaction
				BEGIN TRY

     		-------SimulationOutput --> AssetSummaryDetail -----

            BEGIN TRY

			Print 'AssetSummaryDetail';

          ALTER TABLE AssetSummaryDetail NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN AssetSummaryDetail AS l4 ON l4.SimulationOutputId = l3.Id
			WHERE l1.Id IN (@NetworkId);

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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH


			----------------------------------------------------------------------
			-----End AssetSummaryDetail Path--------------------------------------

			-----Start SimulationYearDetail Path-----------------------------------------

			--SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail 

			BEGIN TRY

			Print 'BudgetUsageDetail';

            ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all

		    Delete l7  
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			JOIN TreatmentConsiderationDetail AS l6 ON l6.AssetDetailId = l5.Id
			JOIN BudgetUsageDetail AS l7 ON l7.TreatmentConsiderationDetailId = l6.Id
			WHERE l1.Id IN (@NetworkId);

		    SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

		--Print '*******BudgetUsageDetail**************'
	
	------------------------------------------------------------------

				--SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail

			BEGIN TRY

			Print 'CashFlowConsiderationDetail';

            ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all

		    Delete l7  
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			JOIN TreatmentConsiderationDetail AS l6 ON l6.AssetDetailId = l5.Id
			JOIN CashFlowConsiderationDetail AS l7 ON l7.TreatmentConsiderationDetailId = l6.Id
			WHERE l1.Id IN (@NetworkId);

		    SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected --SSimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------
	
		--SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail 

			BEGIN TRY

			Print 'TreatmentConsiderationDetail';

            ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all

		    Delete l6  
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			JOIN TreatmentConsiderationDetail AS l6 ON l6.AssetDetailId = l5.Id
			WHERE l1.Id IN (@NetworkId);

		    SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------


--SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentOptionDetail -->  -->  -->  -->  -->  --> 


			BEGIN TRY

			Print 'TreatmentOptionDetail';

            ALTER TABLE TreatmentOptionDetail NOCHECK CONSTRAINT all

			Delete l6 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			JOIN TreatmentOptionDetail AS l6 ON l6.AssetDetailId = l5.Id
			WHERE l1.Id IN (@NetworkId);

		    SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentOptionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
			
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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
		-----------------------------------------------------------------

   COMMIT TRANSACTION
   
    Print 'Partial Network Delete at AssetDetailValueIntId Transaction Committed';
	END TRY
	BEGIN CATCH
  			Set @RetMessage = 'Failed';
			Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			Print 'Rolled Back Network Delete Transaction at AssetDetailValueIntId in Stored Procedure:  ' + @ErrorMessage;
			ROLLBACK TRANSACTION;
			RAISERROR  (@RetMessage, 16, 1); 
			Return -1;
	END CATCH;


		---------------------------------------------------------------
			--SimulationOutput --> SimulationYearDetail --> AssetDetail --> AssetDetailValueIntId -->  -->  -->  -->  -->  --> 


			BEGIN TRY

			Print 'AssetDetailValueIntId';

            ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all
			SET @RowsDeleted = 1;

			WHILE @RowsDeleted > 0
			BEGIN
				BEGIN TRY
					Begin Transaction

						--Delete TOP (@BatchSize) l6 
						SELECT TOP  (@BatchSize) l6.Id  INTO #tempAssetDetailValueIntId
						FROM Network AS l1
						JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
						JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
						JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
						JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
						JOIN AssetDetailValueIntId AS l6 ON l6.AssetDetailId = l5.Id
						WHERE l1.Id IN (@NetworkId);

						--DELETE ar
						--FROM AssetDetailValueIntId As ar
						--JOIN #tempAssetDetailValueIntId T ON T.Id = ar.Id;

						DELETE FROM AssetDetailValueIntId WHERE Id in (SELECT Id FROM #tempAssetDetailValueIntId);

						SET @RowsDeleted = @@ROWCOUNT;

						DROP TABLE #tempAssetDetailValueIntId;
						--WAITFOR DELAY '00:00:01';
;
						COMMIT TRANSACTION
						
						Print 'Rows Affected Network --> --SimulationOutput --> SimulationYearDetail --> AssetDetail --> AssetDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
							ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all
  							Set @RetMessage = 'Failed';
							Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							Print 'Rolled Back AssetDetailValueIntId Delete Transaction in NetworkDelete SP:  ' + @ErrorMessage;
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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
		-----------------------------------------------------------------

		--SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentSchedulingCollisionDetail -->  -->  -->  -->  -->  --> 

			BEGIN TRY

			Print 'TreatmentRejectionDetail';

            ALTER TABLE TreatmentRejectionDetail NOCHECK CONSTRAINT all
			SET @RowsDeleted = 1;

			WHILE @RowsDeleted > 0
			BEGIN
				BEGIN TRY
					Begin Transaction

						--Delete TOP (@BatchSize) l6 
						SELECT TOP  (@BatchSize) l6.Id  INTO #tempTreatmentRejectionDetail
						FROM Network AS l1
						JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
						JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
						JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
						JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
						JOIN TreatmentRejectionDetail AS l6 ON l6.AssetDetailId = l5.Id
						WHERE l1.Id IN (@NetworkId);

						--DELETE ar
						--FROM TreatmentRejectionDetail As ar
						--JOIN #tempTreatmentRejectionDetail T ON T.Id = ar.Id;

						DELETE FROM TreatmentRejectionDetail WHERE Id in (SELECT Id FROM #tempTreatmentRejectionDetail);

						SET @RowsDeleted = @@ROWCOUNT;

						DROP TABLE #tempTreatmentRejectionDetail;
						--WAITFOR DELAY '00:00:01';

						COMMIT TRANSACTION
						
						Print 'Rows Affected Network --> --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentRejectionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
							ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all
  							Set @RetMessage = 'Failed';
							Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							Print 'Rolled Back TreatmentRejectionDetail Delete Transaction in NetworkDelete SP:  ' + @ErrorMessage;
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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	--Print '***TreatmentRejectionDetail***'
	-----------------------------------------------------------------
	--Restart Transaction at TreatmentSchedulingCollisionDetail
	Begin Transaction
	BEGIN TRY

	-----------------------------------------------------------------
	--SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentSchedulingCollisionDetail 


			BEGIN TRY

			Print 'TreatmentSchedulingCollisionDetail';

            ALTER TABLE TreatmentSchedulingCollisionDetail NOCHECK CONSTRAINT all

			Delete l6 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			JOIN TreatmentSchedulingCollisionDetail AS l6 ON l6.AssetDetailId = l5.Id
			WHERE l1.Id IN (@NetworkId);

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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	-----------------------------------------------------------------
	
			--SimulationOutput\SimulationYearDetail\AssetDetail
			
			BEGIN TRY

			Print 'Simulation\SimulationOutput\SimulationYearDetail\AssetDetail';

            ALTER TABLE AssetDetail NOCHECK CONSTRAINT all

			Delete l5
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected  Network ->  Simulation ->  SimulationOutput --> SimulationYearDetail-->AssetDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	-----------------------------------------------------------------


			--SimulationOutput\SimulationYearDetail\BudgetDetail
			
			BEGIN TRY

			Print 'SimulationOutput\SimulationYearDetail\BudgetDetailBudgetDetail';

            ALTER TABLE BudgetDetail NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			JOIN BudgetDetail AS l5 ON l5.SimulationYearDetailId = l3.Id
			WHERE l1.Id IN (@NetworkId);

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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------


				--SimulationOutput\SimulationYearDetail\DeficientConditionGoalDetail
			
			BEGIN TRY

            ALTER TABLE DeficientConditionGoalDetail NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			JOIN DeficientConditionGoalDetail AS l5 ON l5.SimulationYearDetailId = l3.Id
			WHERE l1.Id IN (@NetworkId);

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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------

	
				--SimulationOutput\SimulationYearDetail\TargetConditionGoalDetail
			
			BEGIN TRY

			Print 'TargetConditionGoalDetail';

            ALTER TABLE TargetConditionGoalDetail NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			JOIN TargetConditionGoalDetail AS l5 ON l5.SimulationYearDetailId = l3.Id
			WHERE l1.Id IN (@NetworkId);

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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
	
	------------------------------------------------------------------

			--SimulationOutput\SimulationYearDetail
			
			BEGIN TRY

			Print 'SimulationYearDetail';

            ALTER TABLE SimulationYearDetail NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
			WHERE l1.Id IN (@NetworkId);

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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			------------------------------------------------------------------

			
			--SimulationOutputJson Delete records where SimulationOutput is the parent

            BEGIN TRY

			ALTER TABLE SimulationOutputJson NOCHECK CONSTRAINT all

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			Join SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN SimulationOutputJson AS l4 ON l4.SimulationOutputId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE SimulationOutputJson WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in SimulationOutputJson'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------

			--SimulationOutput

            BEGIN TRY

            ALTER TABLE SimulationOutput NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

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
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

		   ---------------------------------------------------------------------------
		----End SimulationOutput Delete----------------------------------------
		    --SET @CurrentDateTime = GETDATE();
			--PRINT 'End SimulationOutput Delete: ' + CONVERT(NVARCHAR, @CurrentDateTime, 120);
 
			--SimulationOutputJson Delete records where Simulation is the parent

            BEGIN TRY

			Print 'SimulationOutputJson';

			ALTER TABLE SimulationOutputJson NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationOutputJson AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected  Network ->  Simulation ->  SimulationOutputJson: ' +  convert(NVARCHAR(50), @RowsDeleted);

			ALTER TABLE SimulationOutputJson WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in SimulationOutputJson'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------

			--SimulationReportDetail

            BEGIN TRY

			Print 'SimulationReportDetail';

			ALTER TABLE SimulationReportDetail NOCHECK CONSTRAINT all

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN SimulationReportDetail AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE SimulationReportDetail WITH CHECK CHECK CONSTRAINT all

 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in SimulationReportDetail'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------
			--Simulation

            BEGIN TRY

			Print 'Simulation';

			ALTER TABLE Simulation NOCHECK CONSTRAINT all

			Delete l2 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected  Network --> Simulation: ' +  convert(NVARCHAR(50), @RowsDeleted);

			ALTER TABLE Simulation WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Simulation'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH
		
		    SET @CurrentDateTime = GETDATE();
			PRINT 'End Simulation Delete: ' + CONVERT(NVARCHAR, @CurrentDateTime, 120);
			------End Simulation Delete-------------------------------------------------------------------
			-----End  ----Network --> Simulation Path---------

			-- Start Network

            BEGIN TRY

			Print 'Network';

            ALTER TABLE Network NOCHECK CONSTRAINT all

			Delete l1 
			FROM  Network AS l1
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			Print 'Rows Affected  Network: ' +  convert(NVARCHAR(50), @RowsDeleted);

			ALTER TABLE Network WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in Network'
		         RAISERROR (@CustomErrorMessage, 16, 1);
				 Set @RetMessage = @CustomErrorMessage;

            END CATCH

		   ---------------------------------------------------------------------------
			------End Network Delete-------------------------------------------------------------------

    COMMIT TRANSACTION
	
    Print 'Delete Network Committed End';
   	RAISERROR (@RetMessage, 0, 1);
	END TRY
	BEGIN CATCH
  			Set @RetMessage = 'Failed ' + @RetMessage;
			Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			Print 'Rolled Back Last Partial Delete Transaction in Network SP:  ' + @ErrorMessage;
			ROLLBACK TRANSACTION;
			RAISERROR  (@RetMessage, 16, 1);  
	END CATCH;

END 

--  DECLARE @RetMessage varchar(100); EXEC usp_delete_network '119AD446-3330-426B-864D-E9D471949D6B' , @RetMessage OUTPUT 

