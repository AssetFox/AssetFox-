
Create PROCEDURE dbo.usp_delete_simulation(
--@SimulationId AS uniqueidentifier=NULL,
@SimGuidList NVARCHAR(MAX)=NULL,
@RetMessage VARCHAR(250)=NULL OUTPUT
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
	DECLARE @BatchSize INT = 100000;  -- Adjust batch size as needed
	DECLARE @RowsDeleted INT = 1;

 ---------------------------------------------
 CREATE TABLE #SimTempGuids
    (
       -- Guid UNIQUEIDENTIFIER
		Guid NVARCHAR(36)
    );
	
	IF @SimGuidList IS NULL OR LEN(@SimGuidList) = 0
	BEGIN
		  PRINT 'String is NULL or empty';
		  Set  @SimGuidList = '00000000-0000-0000-0000-000000000000';
	END

    INSERT INTO #SimTempGuids (Guid)
	SELECT LEFT(LTRIM(RTRIM(value)), 36)
    FROM STRING_SPLIT(@SimGuidList, ',');

	--Select *, '' as 'aaa' from #SimTempGuids;

	UPDATE #SimTempGuids
	SET Guid = '00000000-0000-0000-0000-000000000000'
	WHERE TRY_CAST(Guid AS UNIQUEIDENTIFIER) IS NULL OR Guid = '';
	
	--Select *, '' as 'bbb' from #SimTempGuids;

		Begin Transaction
	BEGIN TRY

-----------------------------------------------------------------------------


            BEGIN TRY
 
            ALTER TABLE Benefit NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN AnalysisMethod AS l2 ON l2.SimulationId = l1.Id
			JOIN Benefit AS l3 ON l3.AnalysisMethodId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			Delete l3 
			FROM  Simulation  AS l1
			JOIN AnalysisMethod AS l2 ON l2.SimulationId = l1.Id
			JOIN CriterionLibrary_AnalysisMethod AS l3 ON l3.AnalysisMethodId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			Delete l2 
			FROM  Simulation  AS l1
			JOIN AnalysisMethod AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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
 
 			-----------------------------------------------------------------------

         BEGIN TRY

            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN CommittedProject AS l2 ON l2.SimulationId = l1.Id
			JOIN CommittedProjectConsequence AS l3 ON l3.CommittedProjectId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			 BEGIN TRY

            ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN CommittedProject AS l2 ON l2.SimulationId = l1.Id
			JOIN CommittedProjectLocation AS l3 ON l3.CommittedProjectId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE CommittedProject NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN CommittedProject AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

            ALTER TABLE CommittedProjectConsequence NOCHECK CONSTRAINT all

			Delete l4 
			FROM  Simulation  AS l1
			JOIN ScenarioBudget AS l2 ON l2.SimulationId = l1.Id
			JOIN CommittedProject AS l3 ON l3.ScenarioBudgetId = l2.Id
			JOIN CommittedProjectConsequence AS l4 ON l4.CommittedProjectId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			 BEGIN TRY

            ALTER TABLE CommittedProjectLocation NOCHECK CONSTRAINT all

			Delete l4 
			FROM  Simulation  AS l1
			JOIN ScenarioBudget AS l2 ON l2.SimulationId = l1.Id
			JOIN CommittedProject AS l3 ON l3.ScenarioBudgetId = l2.Id
			JOIN CommittedProjectLocation AS l4 ON l4.CommittedProjectId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			-----------------------------------------

            BEGIN TRY

			ALTER TABLE ScenarioBudgetAmount NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioBudget AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioBudgetAmount AS l3 ON l3.ScenarioBudgetId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE BudgetPercentagePair NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioBudget AS l2 ON l2.SimulationId = l1.Id
			JOIN BudgetPercentagePair AS l3 ON l3.ScenarioBudgetId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			-------------------------------------------------------------------------

            BEGIN TRY

			ALTER TABLE ScenarioBudget NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN ScenarioBudget AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			---------------------------------------------

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioBudgetPriority NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioBudgetPriority AS l2 ON l2.SimulationId = l1.Id
			JOIN CriterionLibrary_ScenarioBudgetPriority AS l3 ON l3.ScenarioBudgetPriorityId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE BudgetPercentagePair NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioBudgetPriority AS l2 ON l2.SimulationId = l1.Id
			JOIN BudgetPercentagePair AS l3 ON l3.ScenarioBudgetPriorityId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioBudgetPriority NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN ScenarioBudgetPriority AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			----------------------------------------------------------------------

            BEGIN TRY

			ALTER TABLE ScenarioCalculatedAttributePair_Criteria NOCHECK CONSTRAINT all

			Delete l4 
			FROM  Simulation  AS l1
			JOIN ScenarioCalculatedAttribute AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioCalculatedAttributePair AS l3 ON l3.ScenarioCalculatedAttributeId = l2.Id
			JOIN ScenarioCalculatedAttributePair_Criteria AS l4 ON l4.ScenarioCalculatedAttributePairId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioCalculatedAttributePair_Equation NOCHECK CONSTRAINT all

			Delete l4 
			FROM  Simulation  AS l1
			JOIN ScenarioCalculatedAttribute AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioCalculatedAttributePair AS l3 ON l3.ScenarioCalculatedAttributeId = l2.Id
			JOIN ScenarioCalculatedAttributePair_Equation AS l4 ON l4.ScenarioCalculatedAttributePairId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			BEGIN TRY

			ALTER TABLE ScenarioCalculatedAttributePair NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioCalculatedAttribute AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioCalculatedAttributePair AS l3 ON l3.ScenarioCalculatedAttributeId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioCalculatedAttribute NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN ScenarioCalculatedAttribute AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			--------------------------------------------------------------------

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioCashFlowRule NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioCashFlowRule AS l2 ON l2.SimulationId = l1.Id
			JOIN CriterionLibrary_ScenarioCashFlowRule AS l3 ON l3.ScenarioCashFlowRuleId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			BEGIN TRY

			ALTER TABLE ScenarioCashFlowDistributionRule NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioCashFlowRule AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioCashFlowDistributionRule AS l3 ON l3.ScenarioCashFlowRuleId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioCashFlowRule NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN ScenarioCashFlowRule AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			--------------------------------------------------------------------

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioDeficientConditionGoal NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioDeficientConditionGoal AS l2 ON l2.SimulationId = l1.Id
			JOIN CriterionLibrary_ScenarioDeficientConditionGoal AS l3 ON l3.ScenarioDeficientConditionGoalId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioDeficientConditionGoal NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN ScenarioDeficientConditionGoal AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			---------------------------------------------------------------------

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioPerformanceCurve NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioPerformanceCurve AS l2 ON l2.SimulationId = l1.Id
			JOIN CriterionLibrary_ScenarioPerformanceCurve AS l3 ON l3.ScenarioPerformanceCurveId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioPerformanceCurve_Equation NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioPerformanceCurve AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioPerformanceCurve_Equation AS l3 ON l3.ScenarioPerformanceCurveId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioPerformanceCurve NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN ScenarioPerformanceCurve AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			---------------------------------------------------------------------

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioRemainingLifeLimit NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioRemainingLifeLimit AS l2 ON l2.SimulationId = l1.Id
			JOIN CriterionLibrary_ScenarioRemainingLifeLimit AS l3 ON l3.ScenarioRemainingLifeLimitId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioRemainingLifeLimit NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN ScenarioRemainingLifeLimit AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			--------------------------------------------------------------------

			BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioTreatmentSupersedeRule NOCHECK CONSTRAINT all

			Delete l4
			FROM Simulation AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioTreatmentSupersedeRule AS l3 ON l3.TreatmentId = l2.Id
			JOIN CriterionLibrary_ScenarioTreatmentSupersedeRule AS l4  ON l4.ScenarioTreatmentSupersedeRuleId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			Delete l3
			FROM Simulation AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioTreatmentSupersedeRule AS l3 ON l3.TreatmentId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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
			
			   --Network --> Simulation --> ScenarioSelectableTreatment

            BEGIN TRY

			ALTER TABLE ScenarioSelectableTreatment NOCHECK CONSTRAINT all

			Delete l2
			FROM Simulation AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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
	 

			---------------------------------------------------------------------------


            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioTreatmentConsequence NOCHECK CONSTRAINT all

			Delete l4 
			FROM  Simulation  AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioConditionalTreatmentConsequences AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			JOIN  CriterionLibrary_ScenarioTreatmentConsequence AS l4 ON l4.ScenarioConditionalTreatmentConsequenceId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			--ALTER TABLE ScenarioTreatmentConsequence_Equation NOCHECK CONSTRAINT all

			Delete l4 
			FROM  Simulation  AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioConditionalTreatmentConsequences AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			JOIN  ScenarioTreatmentConsequence_Equation AS l4 ON l4.ScenarioConditionalTreatmentConsequenceId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioConditionalTreatmentConsequences NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioConditionalTreatmentConsequences AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			--ALTER TABLE ScenarioTreatmentCost_Equation NOCHECK CONSTRAINT all

			Delete l4 
			FROM  Simulation  AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioTreatmentCost AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			JOIN  ScenarioTreatmentCost_Equation AS l4 ON l4.ScenarioTreatmentCostId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioTreatmentCost NOCHECK CONSTRAINT all

			Delete l4 
			FROM  Simulation  AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioTreatmentCost AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			JOIN  CriterionLibrary_ScenarioTreatmentCost AS l4 ON l4.ScenarioTreatmentCostId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioTreatmentCost NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioTreatmentCost AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioSelectableTreatment_ScenarioBudget NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioSelectableTreatment_ScenarioBudget AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioTreatment NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			JOIN CriterionLibrary_ScenarioTreatment AS l3 ON l3.ScenarioSelectableTreatmentId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioTreatmentScheduling NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			JOIN ScenarioTreatmentScheduling AS l3 ON l3.TreatmentId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioSelectableTreatment NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN ScenarioSelectableTreatment AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			---------------------------------------------------------------------------

            BEGIN TRY

			ALTER TABLE CriterionLibrary_ScenarioTargetConditionGoal NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN ScenarioTargetConditionGoals AS l2 ON l2.SimulationId = l1.Id
			JOIN CriterionLibrary_ScenarioTargetConditionGoal AS l3 ON l3.ScenarioTargetConditionGoalId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE ScenarioTargetConditionGoals NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN ScenarioTargetConditionGoals AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			---------------------------------------------------------------------------

            BEGIN TRY

			ALTER TABLE InvestmentPlan NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN InvestmentPlan AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			---------------------------------------------------------------------------

            BEGIN TRY

			ALTER TABLE ReportIndex NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN ReportIndex AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			---------------------------------------------------------------------------

            BEGIN TRY

			ALTER TABLE Simulation_User NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN Simulation_User AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

            BEGIN TRY

			ALTER TABLE SimulationAnalysisDetail NOCHECK CONSTRAINT all

			Delete l2 
			FROM  Simulation  AS l1
			JOIN SimulationAnalysisDetail AS l2 ON l2.SimulationId = l1.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

          ALTER TABLE AssetSummaryDetailValueIntId NOCHECK CONSTRAINT all
		  SET @RowsDeleted = 1;
		  --Print 'AssetSummaryDetailValueIntId ';

		   WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
						Begin Transaction

						Delete TOP (@BatchSize) l4
						FROM  Simulation  AS l1
						JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
						JOIN AssetSummaryDetail AS l3 ON l3.SimulationOutputId = l2.Id
						Join AssetSummaryDetailValueIntId As l4 ON l4.AssetSummaryDetailId = l3.Id
						WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

						SET @RowsDeleted = @@ROWCOUNT;
						COMMIT TRANSACTION
						Print 'Rows Affected AssetSummaryDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
							ALTER TABLE AssetSummaryDetailValueIntId WITH CHECK CHECK CONSTRAINT all
  							Set @RetMessage = 'Failed';
							Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							Print 'Rolled Back AssetSummaryDetailValueIntId Delete Transaction in Simulation SP:  ' + @ErrorMessage;
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

			-----------------------------------------------------------------------

            BEGIN TRY

          ALTER TABLE AssetSummaryDetail NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			JOIN AssetSummaryDetail AS l3 ON l3.SimulationOutputId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			BEGIN TRY

            ALTER TABLE BudgetUsageDetail NOCHECK CONSTRAINT all

			Delete l6 
			FROM  Simulation  AS l1
			JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			JOIN TreatmentConsiderationDetail AS l5 ON l5.AssetDetailId = l4.Id
			JOIN BudgetUsageDetail AS l6 ON l6.TreatmentConsiderationDetailId = l5.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

	------------------------------------------------------------------

			BEGIN TRY

            ALTER TABLE CashFlowConsiderationDetail NOCHECK CONSTRAINT all
			
			Delete l6 
			FROM  Simulation  AS l1
			JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			JOIN TreatmentConsiderationDetail AS l5 ON l5.AssetDetailId = l4.Id
			JOIN CashFlowConsiderationDetail AS l6 ON l6.TreatmentConsiderationDetailId = l5.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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
	
			BEGIN TRY

            ALTER TABLE TreatmentConsiderationDetail NOCHECK CONSTRAINT all

			Delete l5 
			FROM  Simulation  AS l1
			JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			JOIN TreatmentConsiderationDetail AS l5 ON l5.AssetDetailId = l4.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			BEGIN TRY

            ALTER TABLE TreatmentOptionDetail NOCHECK CONSTRAINT all

			Delete l5 
			FROM  Simulation  AS l1
			JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			JOIN TreatmentOptionDetail AS l5 ON l5.AssetDetailId = l4.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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

			BEGIN TRY

            ALTER TABLE AssetDetailValueIntId NOCHECK CONSTRAINT all

			SET @RowsDeleted = 1;
		  	--Print 'AssetDetailValueIntId ';

		   WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
						Begin Transaction

						Delete TOP (@BatchSize) l5 
						FROM  Simulation  AS l1
						JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
						JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
						JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
						JOIN AssetDetailValueIntId AS l5 ON l5.AssetDetailId = l4.Id
						WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

						SET @RowsDeleted = @@ROWCOUNT;
						COMMIT TRANSACTION
						Print 'Rows Affected AssetDetailValueIntId: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
							ALTER TABLE AssetDetailValueIntId WITH CHECK CHECK CONSTRAINT all
  							Set @RetMessage = 'Failed';
							Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							Print 'Rolled Back AssetDetailValueIntId Delete Transaction in Simulation SP  ' + @ErrorMessage;
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

			BEGIN TRY

            ALTER TABLE TreatmentRejectionDetail NOCHECK CONSTRAINT all

		  	--Print 'TreatmentRejectionDetail ';

		   WHILE @RowsDeleted > 0
				BEGIN
					BEGIN TRY
						Begin Transaction

						Delete TOP (@BatchSize) l5 
						FROM  Simulation  AS l1
						JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
						JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
						JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
						JOIN TreatmentRejectionDetail AS l5 ON l5.AssetDetailId = l4.Id
						WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

 						SET @RowsDeleted = @@ROWCOUNT;
						COMMIT TRANSACTION
						Print 'Rows Affected TreatmentRejectionDetail: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
							ALTER TABLE TreatmentRejectionDetail WITH CHECK CHECK CONSTRAINT all
  							Set @RetMessage = 'Failed';
							Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							Print 'Rolled Back TreatmentRejectionDetail Delete Transaction in Simulation SP  ' + @ErrorMessage;
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
	
	-----------------------------------------------------------------

			BEGIN TRY

            ALTER TABLE TreatmentSchedulingCollisionDetail NOCHECK CONSTRAINT all

			Delete l5 
			FROM  Simulation  AS l1
			JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			JOIN TreatmentSchedulingCollisionDetail AS l5 ON l5.AssetDetailId = l4.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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
	
			BEGIN TRY

            ALTER TABLE AssetDetail NOCHECK CONSTRAINT all

			Delete l4 
			FROM  Simulation  AS l1
			JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			JOIN AssetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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
			
			BEGIN TRY

            ALTER TABLE BudgetDetail NOCHECK CONSTRAINT all

			Delete l4 
			FROM  Simulation  AS l1
			JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			JOIN BudgetDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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
			
			BEGIN TRY

            ALTER TABLE DeficientConditionGoalDetail NOCHECK CONSTRAINT all

			Delete l4 
			FROM  Simulation  AS l1
			JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			JOIN DeficientConditionGoalDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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
			
			BEGIN TRY

            ALTER TABLE TargetConditionGoalDetail NOCHECK CONSTRAINT all

			Delete l4 
			FROM  Simulation  AS l1
			JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			JOIN TargetConditionGoalDetail AS l4 ON l4.SimulationYearDetailId = l3.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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
			
			BEGIN TRY

            ALTER TABLE SimulationYearDetail NOCHECK CONSTRAINT all

			Delete l3 
			FROM  Simulation  AS l1
			JOIN SimulationOutput AS l2 ON l2.SimulationId = l1.Id
			JOIN SimulationYearDetail AS l3 ON l3.SimulationOutputId = l2.Id
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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
			
			--SimulationOutputJson Delete records where Simulation is the parent

            BEGIN TRY

			ALTER TABLE SimulationOutputJson NOCHECK CONSTRAINT all

			BEGIN TRY
				Begin Transaction

					Delete l2 
					FROM  Simulation AS l1
					JOIN SimulationOutputJson AS l2 ON l2.SimulationId = l1.Id
					WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

 					SET @RowsDeleted = @@ROWCOUNT;
					COMMIT TRANSACTION
					Print 'Rows Affected SimulationOutputJson: ' +  convert(NVARCHAR(50), @RowsDeleted);
					END TRY
					BEGIN CATCH
							ALTER TABLE SimulationOutputJson WITH CHECK CHECK CONSTRAINT all
  							Set @RetMessage = 'Failed';
							Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
							Print 'Rolled Back SimulationOutputJson Delete Transaction in Simulation SP  ' + @ErrorMessage;
							ROLLBACK TRANSACTION;
							RAISERROR  (@RetMessage, 16, 1); 
							Return -1;
					END CATCH;

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

            BEGIN TRY

			ALTER TABLE Simulation NOCHECK CONSTRAINT all

			Delete l1 
			FROM  Simulation  AS l1
			WHERE l1.Id IN (SELECT Guid FROM #SimTempGuids);

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
		
		-------------------------------------------------------------
	
	DROP TABLE #SimTempGuids;
    COMMIT TRANSACTION
    Print 'Simulation  Delete Transaction Committed';
   	--RAISERROR (@RetMessage, 0, 1);
	END TRY
	BEGIN CATCH
  			Set @RetMessage = 'Failed';
			Set @ErrorMessage =  ERROR_PROCEDURE() + ' (Error At Line: ' + cast( ERROR_LINE() as Varchar(5)) + ' ): ' + char(13) + char(10)  + ERROR_MESSAGE()  -- AS ErrorMessage;
			Print 'Rolled Back Simulation Delete Transaction in Simulation SP:  ' + @ErrorMessage;
			ROLLBACK TRANSACTION;
			RAISERROR  (@RetMessage, 16, 1);  
	END CATCH;

END 



