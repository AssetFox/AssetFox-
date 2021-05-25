CREATE TABLE SimulationLog (
	Id uniqueidentifier not null PRIMARY KEY,
	SimulationId uniqueidentifier FOREIGN KEY REFERENCES Simulation(Id),
	Status int,
	TimeStamp datetime2,
	Subject int,
	Message nvarchar(Max)
);