CREATE Table Movie (
	Movie_ID int PRIMARY KEY,
	Category varchar(255),
	Title varchar(255),
	Description varchar(255),
	DateLaunched Date NOT NULL,
	Origin varchar(255),
)