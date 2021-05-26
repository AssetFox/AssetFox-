CREATE TABLE SimulationLog (
	Id uniqueidentifier not null PRIMARY KEY,
	SimulationId uniqueidentifier FOREIGN KEY REFERENCES Simulation(Id),
	Status int,
	Subject int,
	Message nvarchar(Max),
	CreatedDate datetime2 NOT NULL,
	LastModifiedDate datetime2 NOT NULL,
	CreatedBy uniqueidentifier NOT NULL,
	LastModifiedBy uniqueidentifier NOT NULL,
) 
GO