--Assuming DB is called "CarRegistrations"--
use [CarRegistrations];

IF OBJECT_ID('Main', 'U') IS NOT NULL DROP TABLE Main;
IF OBJECT_ID('Owners', 'U') IS NOT NULL DROP TABLE Owners;
IF OBJECT_ID('States', 'U') IS NOT NULL DROP TABLE States;
IF OBJECT_ID('Counties', 'U') IS NOT NULL DROP TABLE Counties;
IF OBJECT_ID('CarMake', 'U') IS NOT NULL DROP TABLE CarMake;
IF OBJECT_ID('CarModel', 'U') IS NOT NULL DROP TABLE CarModel;
IF OBJECT_ID('CarType', 'U') IS NOT NULL DROP TABLE CarType;
IF OBJECT_ID('CarColor', 'U') IS NOT NULL DROP TABLE CarColor;


CREATE TABLE Main
(
	Registration_ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Owner_ID INT NOT NULL,
	DateRegistered DATETIME,
	CarMake_ID INT,
	CarModel_ID INT,
	CarType_ID INT,
	CarYear INT,
	CarColor_ID INT,
	RegisteredState_ID INT,
	RegisteredCounty_ID INT,
	CarValue FLOAT
);

CREATE TABLE Owners
(
	Owner_ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	OwnerLastName VARCHAR(50),
	OwnerFirstName VARCHAR(50),
	OwnerAge INT,
	OwnerGender VARCHAR(1),
	OwnerState_ID INT,
	OwnerCounty_ID INT,
);

CREATE TABLE States
(
	State_ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	StateName VARCHAR(50)
);

CREATE TABLE Counties
(
	County_ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	State_ID INT,
	CountyName VARCHAR(50),
	CountyPopulation INT
);

CREATE TABLE CarMake
(
	Make_ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	MakeName VARCHAR(50)
);

CREATE TABLE CarModel
(
	Model_ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	Make_ID INT,
	ModelName VARCHAR(50)
);

CREATE TABLE CarType
(
	CarType_ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	CarTypeName VARCHAR(50)
);

CREATE TABLE CarColor
(
	Color_ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	ColorName VARCHAR(50)
);

--Main Constraints
ALTER TABLE Main ADD CONSTRAINT OwnerConstraint FOREIGN KEY (Owner_ID) REFERENCES Owners(Owner_ID);
ALTER TABLE Main ADD CONSTRAINT CarMakeConstraint FOREIGN KEY (CarMake_ID) REFERENCES CarMake(Make_ID);
ALTER TABLE Main ADD CONSTRAINT CarModelConstraint FOREIGN KEY (CarModel_ID) REFERENCES CarModel(Model_ID);
ALTER TABLE Main ADD CONSTRAINT CarTypeConstraint FOREIGN KEY (CarType_ID) REFERENCES CarType(CarType_ID);
ALTER TABLE Main ADD CONSTRAINT CarColorConstraint FOREIGN KEY (CarColor_ID) REFERENCES CarColor(Color_ID);
ALTER TABLE Main ADD CONSTRAINT StateConstraint FOREIGN KEY (RegisteredState_ID) REFERENCES States(State_ID);
ALTER TABLE Main ADD CONSTRAINT CountyConstraint FOREIGN KEY (RegisteredCounty_ID) REFERENCES Counties(County_ID);

--Owner Constraints
ALTER TABLE Owners ADD CONSTRAINT OwnersStateConstraint FOREIGN KEY (OwnerState_ID) REFERENCES States(State_ID);
ALTER TABLE Owners ADD CONSTRAINT OwnersCountyConstraint FOREIGN KEY (OwnerCounty_ID) REFERENCES Counties(County_ID);

--State and County Constraints
ALTER TABLE Counties ADD CONSTRAINT CountiesStateConstraint FOREIGN KEY (State_ID) REFERENCES States(State_ID);

--Car Model Constraint
ALTER TABLE CarModel ADD CONSTRAINT CarModelMakeConstraint FOREIGN KEY (Make_ID) REFERENCES CarMake(Make_ID);
