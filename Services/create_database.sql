CREATE DATABASE duco_db;
USE duco_db;

CREATE TABLE IF NOT EXISTS users (
	user_id INT AUTO_INCREMENT,
    user_name VARCHAR(16),
    users_password VARCHAR(64),
    salt CHAR(36),
	PRIMARY KEY (user_id)
);

CREATE TABLE IF NOT EXISTS parts(
	part_id INT auto_increment,
    part_name VARCHAR(16),
    quantity decimal(18,2),
    unit_cost decimal(18,2),
    created_on datetime,
    machine_id int,
    company_name varchar(32),
    primary key (part_id)
);

CREATE TABLE IF NOT EXISTS products(
	product_id int auto_increment,
    product_name varchar(16),
    quantity decimal(18,2),
    unit_cost decimal(18,2),
    created_on datetime,
    primary key (product_id)
);

CREATE TABLE IF NOT EXISTS associated_parts(
	product_id INT NOT NULL,
    part_id INT NOT NULL,
    FOREIGN KEY (product_id) REFERENCES products(product_id),
    FOREIGN KEY (part_id) REFERENCES parts(part_id),
    UNIQUE (product_id, part_id)
);

select * from parts;
drop table parts