Create PROCEDURE dbo.usp_delete_network(
@NetworkId AS uniqueidentifier=NULL,
@RetMessage VARCHAR(250) OUTPUT
)
AS 

    BEGIN 
	          BEGIN TRY

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
	DECLARE @RowsDeleted INT = 0;


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

		  	Print 'BenefitQuantifier ';

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

			Print 'NetworkRollupDetail ';

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

		   Print 'NetworkAttribute ';

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

		  Print 'AnalysisMaintainableAsset ';

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

   ------End AnalysisMaintainableAsset------------------------------------------
	----- Start MaintainableAsset Path
	-----Start AggregatedResult Path-----------------------------------------

			--MaintainableAsset --> AggregatedResult

	BEGIN TRY

		Set @RowsDeleted = 0;

		ALTER TABLE AggregatedResult NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AggregatedResult DISABLE;
		
		Print 'AggregatedResult ';

		Select l3.* INTO #tempAggregatedResult
		FROM Network AS l1
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		JOIN AggregatedResult AS l3 ON l3.MaintainableAssetId = l2.Id
		WHERE Not l1.Id IN (@NetworkId);

		Select @RowsDeleted = Count(l3.Id) 
		FROM Network AS l1
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		JOIN AggregatedResult AS l3 ON l3.MaintainableAssetId = l2.Id
		WHERE l1.Id IN (@NetworkId);

		Drop  Table AggregatedResult;

		Select * into AggregatedResult from #tempAggregatedResult;

		Drop table #tempAggregatedResult;
				
		----Print 'Rows Affected Network --> MaintainableAsset-->AggregatedResult: ' +  convert(NVARCHAR(50), @RowsDeleted);
					
		ALTER TABLE AggregatedResult WITH CHECK CHECK CONSTRAINT all
		--ALTER INDEX ALL ON AggregatedResult REBUILD;
					
	END TRY
	BEGIN CATCH
		ALTER TABLE AggregatedResult WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AggregatedResult REBUILD;
		Print 'Query Error in  Network --> MaintainableAsset-->AggregatedResult ***Failed***';
		SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

		SELECT @CustomErrorMessage = 'Query Error in  Network --> MaintainableAsset-->AggregatedResult'
		RAISERROR (@CustomErrorMessage, 16, 1);
		Set @RetMessage = @CustomErrorMessage;
	END CATCH;

-----End AggregatedResult -----------------------------------------------------------------


-------Start AttributeDatum Path-

--MaintainableAsset --> AttributeDatum --> AttributeDatumLocation -->  -->  --> 

	BEGIN TRY

		Set @RowsDeleted = 0;

		ALTER TABLE AttributeDatumLocation NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AttributeDatumLocation DISABLE;

		Print 'AttributeDatumLocation ';

		SELECT l4.*  INTO #tempAttributeDatumLocation
		FROM Network AS l1
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
		Join AttributeDatumLocation As l4 ON l4.AttributeDatumId = l3.Id
		WHERE Not l1.Id IN (@NetworkId);

		Select @RowsDeleted = Count(l3.Id) 
		FROM Network AS l1
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
		Join AttributeDatumLocation As l4 ON l4.AttributeDatumId = l3.Id
		WHERE l1.Id IN (@NetworkId);

		Drop  Table AttributeDatumLocation;

		Select * into AttributeDatumLocation from #tempAttributeDatumLocation;

		Drop table #tempAttributeDatumLocation;
				
		--Print 'Rows Affected Network --> MaintainableAsset-->AttributeDatum-->AttributeDatumLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);

		ALTER TABLE AttributeDatumLocation WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AttributeDatumLocation REBUILD;
					
	END TRY
	BEGIN CATCH
			ALTER TABLE AttributeDatumLocation WITH CHECK CHECK CONSTRAINT all;
			--ALTER INDEX ALL ON AttributeDatumLocation REBUILD;
			Print 'Query Error in Network --> MaintainableAsset-->AttributeDatum-->AttributeDatumLocationt ***Failed***';
			SELECT ERROR_NUMBER() AS ErrorNumber
			,ERROR_SEVERITY() AS ErrorSeverity
			,ERROR_STATE() AS ErrorState
			,ERROR_PROCEDURE() AS ErrorProcedure
			,ERROR_LINE() AS ErrorLine
			,ERROR_MESSAGE() AS ErrorMessage;

			SELECT @CustomErrorMessage = 'Query Error in  Network --> MaintainableAsset-->AttributeDatum-->AttributeDatumLocation'
			RAISERROR (@CustomErrorMessage, 16, 1);
			Set @RetMessage = @CustomErrorMessage;
	END CATCH;



--------End AttributeDatumLocation---------------------------------------------------------------

--------MaintainableAsset --> AttributeDatum--
          
	BEGIN TRY

		Set @RowsDeleted = 0;

		ALTER TABLE AttributeDatum NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AttributeDatum DISABLE;

		Print 'AttributeDatum ';

		SELECT l3.*  INTO #tempAttributeDatumA
		FROM Network AS l1
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
		WHERE Not l1.Id IN (@NetworkId);

		Select @RowsDeleted = Count(l3.Id) 
		FROM Network AS l1
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		JOIN AttributeDatum AS l3 ON l3.MaintainableAssetId = l2.Id
		WHERE l1.Id IN (@NetworkId);

		DROP TABLE AttributeDatum;

		Select * into AttributeDatum from #tempAttributeDatumA;

		Drop table #tempAttributeDatumA;
				
		--Print 'Rows Affected Network --> MaintainableAsset-->AttributeDatum: ' +  convert(NVARCHAR(50), @RowsDeleted);

		ALTER TABLE AttributeDatum WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AttributeDatum REBUILD;

	END TRY
	BEGIN CATCH
		ALTER TABLE AttributeDatum WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AttributeDatum REBUILD
		Print 'Query Error in Network --> MaintainableAsset-->AttributeDatum-->AttributeDatum ***Failed***';
		SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

		SELECT @CustomErrorMessage = 'Query Error in  Network --> MaintainableAsset-->AttributeDatum-->AttributeDatum'
		RAISERROR (@CustomErrorMessage, 16, 1);
		Set @RetMessage = @CustomErrorMessage;
	END CATCH

	----------------------------------------------------------------------
	-----End AttributeDatum Path--------------------------------------

	-----Start AssetSummaryDetail Path-----------------------------------------
	--MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId -->  -->  --> 

	BEGIN TRY

		Set @RowsDeleted = 0;

		ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AssetSummaryDetailValueIntId DISABLE;

		Print 'AssetSummaryDetailValueIntId ';

		Select l4.* INTO #tempAssetSummaryDetailValueIntId
						FROM Network AS l1
						Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
						JOIN AssetSummaryDetail AS l3 ON l3.MaintainableAssetId = l2.Id
						Join AssetSummaryDetailValueIntId As l4 ON l4.AssetSummaryDetailId = l3.Id
						WHERE Not l1.Id IN (@NetworkId);

		Select @RowsDeleted = Count(l4.Id) 
						FROM Network AS l1
						Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
						JOIN AssetSummaryDetail AS l3 ON l3.MaintainableAssetId = l2.Id
						Join AssetSummaryDetailValueIntId As l4 ON l4.AssetSummaryDetailId = l3.Id
						WHERE l1.Id IN (@NetworkId);

		Drop  Table AssetSummaryDetailValueIntId;

		Select * into AssetSummaryDetailValueIntId from #tempAssetSummaryDetailValueIntId;

		Drop table #tempAssetSummaryDetailValueIntId;
					
		--Print 'Rows Affected MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					
		ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AssetSummaryDetailValueIntId REBUILD;

	END TRY 
	BEGIN CATCH
		ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AssetSummaryDetailValueIntId REBUILD;
		Print 'Query Error in Network --> MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId ***Failed***';
		SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

		SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetSummaryDetail --> AssetSummaryDetailValueIntId'
		RAISERROR (@CustomErrorMessage, 16, 1);
		Set @RetMessage = @CustomErrorMessage;

	END CATCH

			-----------------------------------------------------------------------
    		-------MaintainableAsset --> AssetSummaryDetail -----

	BEGIN TRY

		ALTER TABLE AssetSummaryDetail NOCHECK CONSTRAINT all
		SET @RowsDeleted = 1;
		    
		Print 'AssetSummaryDetail ';

		WHILE @RowsDeleted > 0
		BEGIN
		SELECT TOP (@BatchSize) l3.Id INTO #tempAssetSummaryDetail
		FROM Network AS l1
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		JOIN AssetSummaryDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		WHERE l1.Id IN (@NetworkId);

		DELETE FROM AssetSummaryDetail WHERE Id in (SELECT Id FROM #tempAssetSummaryDetail);

		SET @RowsDeleted = @@ROWCOUNT;

		DROP TABLE #tempAssetSummaryDetail;
					
		--Print 'Rows Affected --MaintainableAsset --> AssetSummaryDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
 
		END
		
		ALTER TABLE AssetSummaryDetail WITH CHECK CHECK CONSTRAINT all

	END TRY 
	BEGIN CATCH
		ALTER TABLE AssetSummaryDetail WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AssetSummaryDetail REBUILD;
		Print 'Query Error in Network --> MaintainableAsset --> AssetSummaryDetail ***Failed***';
		SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

		SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetSummaryDetail'
		RAISERROR (@CustomErrorMessage, 16, 1);
		Set @RetMessage = @CustomErrorMessage;
	END CATCH

	-----End AssetSummaryDetail Path--------------------------------------

--MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail -->  --> 
---Start BudgetUsageDetail --------------------------------------------------------------------

	BEGIN TRY

		Set @RowsDeleted = 0;

		ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON BudgetUsageDetail DISABLE;

		Print 'BudgetUsageDetail1 ';

		Select l5.* INTO  #tempBudgetUsageDetail
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			JOIN BudgetUsageDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
			WHERE Not l1.Id IN (@NetworkId);

		Select @RowsDeleted = Count(l5.Id) 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			JOIN BudgetUsageDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
			WHERE l1.Id IN (@NetworkId);

		Drop  Table BudgetUsageDetail;

		Select * into BudgetUsageDetail from #tempBudgetUsageDetail;

		Drop table #tempBudgetUsageDetail;
					
		--Print 'Rows Affected --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
					
		ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON BudgetUsageDetail DISABLE;

	END TRY
	BEGIN CATCH
		ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON BudgetUsageDetail DISABLE;
		Print 'Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail ***Failed***';
		SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

		SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail'
		RAISERROR (@CustomErrorMessage, 16, 1);
		Set @RetMessage = @CustomErrorMessage;
	END CATCH
	
	---End BudgetUsageDetail----------------------------------------------------------------------

	-- MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail -->  --> 
	---Start CashFlowConsiderationDetail --------------------------------------------------------------------

	BEGIN TRY

		Set @RowsDeleted = 0;

		ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON CashFlowConsiderationDetail DISABLE;

		Print 'CashFlowConsiderationDetail ';

		Select l5.* INTO  #tempCashFlowConsiderationDetail
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			JOIN CashFlowConsiderationDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
			WHERE Not l1.Id IN (@NetworkId);

		Select @RowsDeleted = Count(l5.Id) 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
			JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
			JOIN CashFlowConsiderationDetail AS l5 ON l5.TreatmentConsiderationDetailId = l4.Id
			WHERE l1.Id IN (@NetworkId);

		Drop  Table CashFlowConsiderationDetail;

		Select * into CashFlowConsiderationDetail from #tempCashFlowConsiderationDetail;

		Drop table #tempCashFlowConsiderationDetail;
					
		--Print 'Rows Affected --MaintainableAsset --> AssetDetail --> CashFlowConsiderationDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
		
		ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON CashFlowConsiderationDetail DISABLE;

	END TRY
	BEGIN CATCH
		ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON CashFlowConsiderationDetail DISABLE;
		Print 'Query Error in Network --> MaintainableAsset --> AssetDetail --> CashFlowConsiderationDetail ***Failed***';
		SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

		SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetDetail --> CashFlowConsiderationDetail'
		RAISERROR (@CustomErrorMessage, 16, 1);
		Set @RetMessage = @CustomErrorMessage;
	END CATCH
	
	---End CashFlowConsiderationDetail --------------------------------------------------------------------
	---Start TreatmentConsiderationDetail --------------------------------------------------------------------
	
		--MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail 

	BEGIN TRY

		Set @RowsDeleted = 0;

		ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON TreatmentConsiderationDetail DISABLE;

		Print 'TreatmentConsiderationDetail1 ';

		SELECT l4.* INTO #tempTreatmentConsiderationDetail
		FROM Network AS l1
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
		WHERE Not l1.Id IN (@NetworkId);

		Select @RowsDeleted = Count(l4.Id) 
		FROM Network AS l1
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		JOIN TreatmentConsiderationDetail AS l4 ON l4.AssetDetailId = l3.Id
		WHERE l1.Id IN (@NetworkId);

		Drop  Table TreatmentConsiderationDetail;

		Select * into TreatmentConsiderationDetail from #tempTreatmentConsiderationDetail;

		Drop table #tempTreatmentConsiderationDetail;
					
		--Print 'Rows Affected --MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
		
		ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON TreatmentConsiderationDetail DISABLE;

	END TRY
	BEGIN CATCH
		ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON TreatmentConsiderationDetail DISABLE;
		Print 'Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail ***Failed***';
		SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

		SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetDetail --> TreatmentConsiderationDetail'
		RAISERROR (@CustomErrorMessage, 16, 1);
		Set @RetMessage = @CustomErrorMessage;
	END CATCH

	---End TreatmentConsiderationDetail --------------------------------------------------------------------
	---Start AssetDetailValueIntId --------------------------------------------------------------------

	-- Network --> MaintainableAsset --> AssetDetail --> AssetDetailValueIntId -

	BEGIN TRY

		Set @RowsDeleted = 0;

		ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AssetDetailValueIntId DISABLE;

		Print 'AssetDetailValueIntId ';

		Select l4.* INTO #tempAssetDetailValueIntId
		FROM Network AS l1 
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		JOIN AssetDetailValueIntId AS l4 ON l4.AssetDetailId = l3.Id
		WHERE Not l1.Id IN (@NetworkId);

		Select @RowsDeleted = Count(l4.Id) 
		FROM Network AS l1 
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		JOIN AssetDetailValueIntId AS l4 ON l4.AssetDetailId = l3.Id
		WHERE l1.Id IN (@NetworkId);

		Drop  Table AssetDetailValueIntId;

		Select * into AssetDetailValueIntId from #tempAssetDetailValueIntId;

		Drop table #tempAssetDetailValueIntId;
					
		--Print 'Rows Affected Network --> MaintainableAsset --> AssetDetail --> AssetDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					
		ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AssetDetailValueIntId REBUILD;

	END TRY 
	BEGIN CATCH
		ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AssetDetailValueIntId REBUILD;
		Print 'Query Error in Network --> MaintainableAsset --> AssetSummaryDetail --> AssetDetailValueIntId ***Failed***';
		SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

		SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetSummaryDetail --> AssetDetailValueIntId'
		RAISERROR (@CustomErrorMessage, 16, 1);
		Set @RetMessage = @CustomErrorMessage;

	END CATCH

	---End AssetDetailValueIntId --------------------------------------------------------------------
	---Start TreatmentOptionDetail --------------------------------------------------------------------

	-- MaintainableAsset --> AssetDetail --> TreatmentOptionDetail -->  -->  --> 

	BEGIN TRY

		ALTER TABLE TreatmentOptionDetail NOCHECK CONSTRAINT all

		SET @RowsDeleted = 1;
		Print 'TreatmentOptionDetail ';

		WHILE @RowsDeleted > 0
		BEGIN

		SELECT TOP (@BatchSize) l4.Id INTO #tempTreatmentOptionDetail
		FROM Network AS l1
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		JOIN TreatmentOptionDetail AS l4 ON l4.AssetDetailId = l3.Id
		WHERE l1.Id IN (@NetworkId);

		DELETE FROM TreatmentOptionDetail WHERE Id in (SELECT Id FROM #tempTreatmentOptionDetail);

		SET @RowsDeleted = @@ROWCOUNT;

		DROP TABLE #tempTreatmentOptionDetail;
		--WAITFOR DELAY '00:00:01';

		--Print 'Rows Affected MaintainableAsset --> AssetDetail --> TreatmentOptionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
		END
		ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all

	END TRY 
	BEGIN CATCH
		ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON TreatmentOptionDetail REBUILD;
		Print 'Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentOptionDetaild ***Failed***';
		SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

		SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetDetail --> TreatmentOptionDetail'
		RAISERROR (@CustomErrorMessage, 16, 1);
		Set @RetMessage = @CustomErrorMessage;
	END CATCH


---End TreatmentOptionDetail --------------------------------------------------------------------
---Start TreatmentOptionDetail --------------------------------------------------------------------
	-----------------------------------------------------------------

		-- AssetDetail --> TreatmentRejectionDetail 
			
		BEGIN TRY

		Set @RowsDeleted = 0;

		ALTER TABLE TreatmentRejectionDetail NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON TreatmentRejectionDetail DISABLE;

		Print 'TreatmentRejectionDetail ';

		Select l4.* INTO #tempTreatmentRejectionDetail		
				FROM Network AS l1
				Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
				Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
				JOIN TreatmentRejectionDetail AS l4 ON l4.AssetDetailId = l3.Id
				WHERE Not l1.Id IN (@NetworkId);

		Select @RowsDeleted = Count(l4.Id) 	
				FROM Network AS l1
				Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
				Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
				JOIN TreatmentRejectionDetail AS l4 ON l4.AssetDetailId = l3.Id
				WHERE l1.Id IN (@NetworkId);

		Drop  Table TreatmentRejectionDetail;

		Select * into TreatmentRejectionDetail from #tempTreatmentRejectionDetail;

		Drop table #tempTreatmentRejectionDetail;
					
		--Print 'Rows Affected Network --> MaintainableAsset --> AssetDetail --> TreatmentRejectionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
					
		ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON TreatmentRejectionDetail REBUILD;

	END TRY 
	BEGIN CATCH
		ALTER TABLE TreatmentOptionDetail WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON TreatmentOptionDetail REBUILD;
		Print 'Query Error in Network --> MaintainableAsset --> AssetDetail --> TreatmentRejectionDetail ***Failed***';
		SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

		SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetDetail --> TreatmentRejectionDetail'
		RAISERROR (@CustomErrorMessage, 16, 1);
		Set @RetMessage = @CustomErrorMessage;

	END CATCH

	-----------------------------------------------------------------

	-- AssetDetail --> TreatmentSchedulingCollisionDetail 

	BEGIN TRY

		ALTER TABLE TreatmentSchedulingCollisionDetail NOCHECK CONSTRAINT all;
		--ALTER INDEX ALL ON TreatmentSchedulingCollisionDetail DISABLE;

		SET @RowsDeleted = 1;
		Print 'TreatmentSchedulingCollisionDetail ';

		WHILE @RowsDeleted > 0
		BEGIN

		SELECT TOP (@BatchSize) l4.Id INTO #tempTreatmentSchedulingCollisionDetail
		FROM Network AS l1
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		JOIN TreatmentSchedulingCollisionDetail AS l4 ON l4.AssetDetailId = l3.Id
		WHERE l1.Id IN (@NetworkId);

		DELETE FROM TreatmentSchedulingCollisionDetail WHERE Id in (SELECT Id FROM #tempTreatmentSchedulingCollisionDetail);

		SET @RowsDeleted = @@ROWCOUNT;

		DROP TABLE #tempTreatmentSchedulingCollisionDetail;
		--WAITFOR DELAY '00:00:01';
						
		--Print 'Rows Affected Network --> MaintainableAsset-->AssetDetail --> TreatmentSchedulingCollisionDetail : ' +  convert(NVARCHAR(50), @RowsDeleted);
		ALTER TABLE TreatmentSchedulingCollisionDetail WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON TreatmentSchedulingCollisionDetail REBUILD;
		END

	END TRY 
	BEGIN CATCH
		ALTER TABLE TreatmentSchedulingCollisionDetail WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON TreatmentSchedulingCollisionDetail REBUILD;
		SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

		SELECT @CustomErrorMessage = 'Query Error in Network --> MaintainableAsset-->AssetDetail -->  TreatmentSchedulingCollisionDetail'
		RAISERROR (@CustomErrorMessage, 16, 1);
		Set @RetMessage = @CustomErrorMessage;

	END CATCH

	-----------------------------------------------------------------
	
			--MaintainableAsset\AssetDetail

	BEGIN TRY

		ALTER TABLE AssetDetail NOCHECK CONSTRAINT all

		SET @RowsDeleted = 1;
		Print 'AssetDetail ';

		WHILE @RowsDeleted > 0
		BEGIN

		SELECT TOP (@BatchSize) l3.Id INTO #tempAssetDetailId
		FROM Network AS l1
		Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
		Join AssetDetail AS l3 ON l3.MaintainableAssetId = l2.Id
		WHERE l1.Id IN (@NetworkId);

		DELETE FROM AssetDetail WHERE Id in (SELECT Id FROM #tempAssetDetailId);

		SET @RowsDeleted = @@ROWCOUNT;

		DROP TABLE #tempAssetDetailId;
		----WAITFOR DELAY '00:00:01';
				
		--Print 'Rows Affected Network --> MaintainableAsset-->AssetDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
		END

		ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all

	END TRY
	BEGIN CATCH
		ALTER TABLE AssetDetail WITH CHECK CHECK CONSTRAINT all;
		--ALTER INDEX ALL ON AssetDetail REBUILD;
		Print 'Query Error in Network --> MaintainableAsset --> AssetDetail ***Failed***';
		SELECT ERROR_NUMBER() AS ErrorNumber
		,ERROR_SEVERITY() AS ErrorSeverity
		,ERROR_STATE() AS ErrorState
		,ERROR_PROCEDURE() AS ErrorProcedure
		,ERROR_LINE() AS ErrorLine
		,ERROR_MESSAGE() AS ErrorMessage;

		SELECT @CustomErrorMessage = 'Query Error in  Network -->MaintainableAsset --> AssetDetail '
		RAISERROR (@CustomErrorMessage, 16, 1);
		Set @RetMessage = @CustomErrorMessage;

	END CATCH


		---End  --Network -->MaintainableAsset --> AssetDetail Path---------
	  -----Start CommittedProject Path-----------------------------------------
			
		--Network --> MaintainableAsset --> CommittedProject --> CommittedProjectConsequence -->  -->  -->  -->  -->  --> 

         BEGIN TRY

            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

			Print 'CommittedProjectConsequence ';

			Delete l4 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join CommittedProject  AS l3 ON l3.MaintainableAssetEntityId = l2.Id
			JOIN CommittedProjectConsequence AS l4 ON l4.CommittedProjectId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			--Print 'Rows Affected Network Delete CommittedProjectConsequence: ' +  convert(NVARCHAR(50), @RowsDeleted);

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

			Print 'CommittedProjectLocation ';

			Delete l4 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join CommittedProject  AS l3 ON l3.MaintainableAssetEntityId = l2.Id
			JOIN CommittedProjectLocation AS l4 ON l4.CommittedProjectId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			--Print 'Rows Affected Network Delete CommittedProjectLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);

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

			Print 'CommittedProject ';

			Delete l3 
			FROM Network AS l1
			Join MaintainableAsset AS l2 ON l2.NetworkId = l1.Id
			Join CommittedProject AS l3 ON l3.MaintainableAssetEntityId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			--Print 'Rows Affected Network Delete CommittedProject: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
			--Print 'Rows Affected Network --> MaintainableAsset-->MaintainableAssetLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
			--Print 'Rows Affected Network --> MaintainableAsset: ' +  convert(NVARCHAR(50), @RowsDeleted);

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

			Print 'Benefit ';
			--AnalysisMethod	Benefit							FK_Benefit_AnalysisMethod_AnalysisMethodId
			--AnalysisMethod	CriterionLibrary_AnalysisMethod	FK_CriterionLibrary_AnalysisMethod_AnalysisMethod_AnalysisMethodId

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN AnalysisMethod AS l3 ON l3.SimulationId = l2.Id
			JOIN Benefit AS l4 ON l4.AnalysisMethodId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			--Print 'Rows Affected Network --> Benefit: ' +  convert(NVARCHAR(50), @RowsDeleted);

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

			Print 'CriterionLibrary_AnalysisMethod ';

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN AnalysisMethod AS l3 ON l3.SimulationId = l2.Id
			JOIN CriterionLibrary_AnalysisMethod AS l4 ON l4.AnalysisMethodId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			--Print 'Rows Affected Network --> CriterionLibrary_AnalysisMethod: ' +  convert(NVARCHAR(50), @RowsDeleted);

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

			Print 'AnalysisMethod ';

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN AnalysisMethod AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			--Print 'Rows Affected Network --> AnalysisMethod: ' +  convert(NVARCHAR(50), @RowsDeleted);

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

			Print 'CommittedProjectConsequence ';

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN CommittedProject AS l3 ON l3.SimulationId = l2.Id
			JOIN CommittedProjectConsequence AS l4 ON l4.CommittedProjectId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			--Print 'Rows Affected Network --> CommittedProjectConsequence: ' +  convert(NVARCHAR(50), @RowsDeleted);

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

			Print 'CommittedProjectLocation ';

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN CommittedProject AS l3 ON l3.SimulationId = l2.Id
			JOIN CommittedProjectLocation AS l4 ON l4.CommittedProjectId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			SET @RowsDeleted = @@ROWCOUNT;
			--Print 'Rows Affected Network --> CommittedProjectLocation: ' +  convert(NVARCHAR(50), @RowsDeleted);

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

			Print 'CommittedProject ';

			Delete l3 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN CommittedProject AS l3 ON l3.SimulationId = l2.Id
			WHERE l1.Id IN (@NetworkId)

			SET @RowsDeleted = @@ROWCOUNT;
			--Print 'Rows Affected Network --> CommittedProject: ' +  convert(NVARCHAR(50), @RowsDeleted);

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

			ALTER TABLE InvestmentPlan NOCHECK CONSTRAINT all

			Print 'InvestmentPlan';

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

			ALTER TABLE BudgetPercentagePair NOCHECK CONSTRAINT all

			Print 'BudgetPercentagePair';

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

            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

			Print 'CommittedProjectConsequence';

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

			Print 'CommittedProjectLocation ';

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

			Print 'CommittedProject ';

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

			Print 'CriterionLibrary_ScenarioBudget ';

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

			ALTER TABLE ScenarioBudgetAmount NOCHECK CONSTRAINT all

			Print 'ScenarioBudgetAmount';

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

			Print 'CriterionLibrary_ScenarioBudgetPriority ';

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

			Print 'ScenarioBudgetPriority ';

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

			Print 'ScenarioCalculatedAttributePair_Criteria';

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

			Print 'ScenarioCalculatedAttributePair_Equation';

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

			Print 'ScenarioCalculatedAttributePair';

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

			Print 'ScenarioCalculatedAttribute ';

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

			Print 'CriterionLibrary_ScenarioCashFlowRule ';

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

			Print 'ScenarioCashFlowDistributionRule ';

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

			Print 'ScenarioCashFlowRule ';

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

			Print 'ScenarioDeficientConditionGoal ';

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

			Print 'ScenarioPerformanceCurve_Equation ';

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

			Print 'CriterionLibrary_ScenarioRemainingLifeLimit';

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

			Print 'ScenarioRemainingLifeLimit';

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

 
  --Network --> Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentSupersedeRule --> CriterionLibrary_ScenarioTreatmentSupersedeRule --> 

			BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioTreatmentSupersedeRule NOCHECK CONSTRAINT all

			Print 'CriterionLibrary_ScenarioTreatmentSupersedeRule';

			Delete l5
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentSupersedeRule AS l4 ON l4.TreatmentId = l3.Id
			JOIN CriterionLibrary_ScenarioTreatmentSupersedeRule AS l5  ON l5.ScenarioTreatmentSupersedeRuleId = l4.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE CriterionLibrary_ScenarioTreatmentSupersedeRule WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in CriterionLibrary_ScenarioTreatmentSupersedeRule'
                 RAISERROR  (@CustomErrorMessage, 16, 1);  
                 Set @RetMessage = @CustomErrorMessage;

            END CATCH

			---------------------------------------------------------------------------

   --Network --> Simulation --> ScenarioSelectableTreatment --> ScenarioTreatmentSupersedeRule  

         BEGIN TRY

			ALTER TABLE ScenarioTreatmentSupersedeRule NOCHECK CONSTRAINT all

			Print 'ScenarioTreatmentSupersedeRule ';

			Delete l4
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentSupersedeRule AS l4 ON l4.TreatmentId = l3.Id
			WHERE l1.Id IN (@NetworkId);

			ALTER TABLE ScenarioTreatmentSupersedeRule WITH CHECK CHECK CONSTRAINT all
 
            END TRY 
			BEGIN CATCH
                 SELECT ERROR_NUMBER() AS ErrorNumber
                       ,ERROR_SEVERITY() AS ErrorSeverity
                       ,ERROR_STATE() AS ErrorState
                       ,ERROR_PROCEDURE() AS ErrorProcedure
                       ,ERROR_LINE() AS ErrorLine
                       ,ERROR_MESSAGE() AS ErrorMessage;

 		         SELECT @CustomErrorMessage = 'Query Error in ScenarioTreatmentSupersedeRule'
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

			Print 'ScenarioTreatmentConsequence_Equation';

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

			Print 'ScenarioConditionalTreatmentConsequences';

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

			Print 'ScenarioTreatmentCost_Equation';

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentCost AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			JOIN ScenarioTreatmentCost_Equation AS l5 ON l5.ScenarioTreatmentCostId = l4.Id
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

			Print 'CriterionLibrary_ScenarioTreatmentCost ';

			ALTER TABLE CriterionLibrary_ScenarioTreatmentCost NOCHECK CONSTRAINT all

			Delete l5 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			JOIN ScenarioSelectableTreatment AS l3 ON l3.SimulationId = l2.Id
			JOIN ScenarioTreatmentCost AS l4 ON l4.ScenarioSelectableTreatmentId = l3.Id
			JOIN CriterionLibrary_ScenarioTreatmentCost AS l5 ON l5.ScenarioTreatmentCostId = l4.Id
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

			Print 'ScenarioTreatmentCost ';

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

			Print 'ScenarioSelectableTreatment_ScenarioBudget ';

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

			Print 'CriterionLibrary_ScenarioTreatment ';

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

			Print 'ScenarioTreatmentScheduling ';

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

			Print 'ScenarioSelectableTreatment ';

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

			Print 'ScenarioTargetConditionGoals ';

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

			Print 'Simulation_User ';

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

			Print 'SimulationAnalysisDetail';

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

				--Delete TOP (@BatchSize) l5 
				SELECT TOP  (@BatchSize) l5.Id  INTO #tempAssetSummaryDetailValueIntIdSim	
				FROM Network AS l1
				JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
				JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
				JOIN AssetSummaryDetail AS l4 ON l4.SimulationOutputId = l3.Id
				Join AssetSummaryDetailValueIntId As l5 ON l5.AssetSummaryDetailId = l4.Id
				WHERE l1.Id IN (@NetworkId);

				DELETE FROM AssetSummaryDetailValueIntId WHERE Id in (SELECT Id FROM #tempAssetSummaryDetailValueIntIdSim);

				SET @RowsDeleted = @@ROWCOUNT;

				DROP TABLE #tempAssetSummaryDetailValueIntIdSim;
				--WAITFOR DELAY '00:00:01';
			END	

            ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all;

            END TRY 
			BEGIN CATCH
			     ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all;
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

			Print 'BudgetUsageDetail2';

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
			--Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
			--Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> BudgetUsageDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
			--Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail --> CashFlowConsiderationDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
			--Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentConsiderationDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
			--Print 'Rows Affected --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentOptionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
			
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
		---------------------------------------------------------------
			--SimulationOutput --> SimulationYearDetail --> AssetDetail --> AssetDetailValueIntId -->  -->  -->  -->  -->  --> 


			BEGIN TRY

			Print 'AssetDetailValueIntId';

            ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all
			SET @RowsDeleted = 1;

			WHILE @RowsDeleted > 0
			BEGIN

						--Delete TOP (@BatchSize) l6 
						SELECT TOP  (@BatchSize) l6.Id  INTO #tempAssetDetailValueIntId2
						FROM Network AS l1
						JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
						JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
						JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
						JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
						JOIN AssetDetailValueIntId AS l6 ON l6.AssetDetailId = l5.Id
						WHERE l1.Id IN (@NetworkId);

						DELETE FROM AssetDetailValueIntId WHERE Id in (SELECT Id FROM #tempAssetDetailValueIntId2);

						SET @RowsDeleted = @@ROWCOUNT;

						DROP TABLE #tempAssetDetailValueIntId2;
					
						--Print 'Rows Affected Network --> --SimulationOutput --> SimulationYearDetail --> AssetDetail --> AssetDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
			END			
						
						ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all
            END TRY 
			BEGIN CATCH
			ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all
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

						--Delete TOP (@BatchSize) l6 
						SELECT TOP  (@BatchSize) l6.Id  INTO #tempTreatmentRejectionDetailSim
						FROM Network AS l1
						JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
						JOIN SimulationOutput AS l3 ON l3.SimulationId = l2.Id
						JOIN SimulationYearDetail AS l4 ON l4.SimulationOutputId = l3.Id
						JOIN AssetDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
						JOIN TreatmentRejectionDetail AS l6 ON l6.AssetDetailId = l5.Id
						WHERE l1.Id IN (@NetworkId);

						DELETE FROM TreatmentRejectionDetail WHERE Id in (SELECT Id FROM #tempTreatmentRejectionDetailSim);

						SET @RowsDeleted = @@ROWCOUNT;

						DROP TABLE #tempTreatmentRejectionDetailSim;
						--WAITFOR DELAY '00:00:01';
						
						--Print 'Rows Affected Network --> --SimulationOutput --> SimulationYearDetail --> AssetDetail --> TreatmentRejectionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
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
	
	----End TreatmentRejectionDetail-------------------------------------------------------------
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
			--Print 'Rows Affected  Network ->  Simulation ->  SimulationOutput --> SimulationYearDetail-->AssetDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
			JOIN DeficientConditionGoalDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
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
			JOIN TargetConditionGoalDetail AS l5 ON l5.SimulationYearDetailId = l4.Id
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

			Print 'SimulationYearDetail ';

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

			Print 'SimulationOutputJson ';

			Delete l4 
			FROM Network AS l1
			JOIN Simulation  AS l2 ON l2.NetworkId = l1.Id
			Join SimulationOutput AS l3 ON l3.SimulationId = l2.Id
			JOIN SimulationOutputJson AS l4 ON l4.SimulationOutputId = l3.Id
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

			Print 'SimulationOutput ';

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
			--Print 'Rows Affected  Network ->  Simulation ->  SimulationOutputJson: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
			--Print 'Rows Affected  Network --> Simulation: ' +  convert(NVARCHAR(50), @RowsDeleted);

			ALTER TABLE Simulation WITH CHECK CHECK CONSTRAINT all;
 
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
		
		 --   SET @CurrentDateTime = GETDATE();
			--PRINT 'End Simulation Delete: ' + CONVERT(NVARCHAR, @CurrentDateTime, 120);
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
			--Print 'Rows Affected  Network: ' +  convert(NVARCHAR(50), @RowsDeleted);

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
    Print 'Delete Network Committed End';
 	RAISERROR (@RetMessage, 0, 1);
	END TRY
	BEGIN CATCH
  			Set @RetMessage = 'Failed ' + @RetMessage;
			Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			Print 'Overall Catch in Network SP:  ' + @ErrorMessage;

			RAISERROR  (@RetMessage, 16, 1);  
	END CATCH;

END 