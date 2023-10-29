CREATE DATABASE duco_db;

USE duco_db;

CREATE TABLE IF NOT EXISTS users (
	user_id INT AUTO_INCREMENT,
    user_name VARCHAR(16),
    users_password VARCHAR(64),
	PRIMARY KEY (user_id)
);

CREATE TABLE IF NOT EXISTS parts(
	part_id INT UNIQUE,
    part_name VARCHAR(16),
    quantity decimal(18,2),
    unit_cost decimal(18,2),
    created_on datetime,
    machine_id int,
    company_name varchar(32),
    primary key (part_id)
);

drop table parts