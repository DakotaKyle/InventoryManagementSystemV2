CREATE DATABASE duco_db;

USE duco_db;

CREATE TABLE IF NOT EXISTS users (
	user_id INT AUTO_INCREMENT,
    user_name VARCHAR(16),
    users_password VARCHAR(64),
	PRIMARY KEY (user_id)
);
