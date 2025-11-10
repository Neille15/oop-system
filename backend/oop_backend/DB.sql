drop TABLE attendance;
CREATE TABLE users (
	userID INT IDENTITY(1,1) PRIMARY KEY,
	firstName VARCHAR(50) NOT NULL,
	lastName VARCHAR(50) NOT NULL,
	email VARCHAR(100) NOT NULL UNIQUE,
	birthDate DATE
);

CREATE TABLE attendance (
	attendanceID INT IDENTITY(1,1) PRIMARY KEY,
	userID INT,
	eventDate DATE NOT NULL,
	eventTime TIME NOT NULL,
	status VARCHAR(20) NOT NULL,
	FOREIGN KEY (userID) REFERENCES users(userID)
);

