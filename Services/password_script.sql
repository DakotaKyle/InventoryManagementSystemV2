use duco_db;

DELIMITER //
CREATE PROCEDURE add_user(
	username NVARCHAR(16), 
    user_password NVARCHAR(50)
)
BEGIN
        INSERT INTO users (user_name, users_password)
        VALUES(username, user_password);
END//
DELIMITER ;

call add_user("test", "password");

select * from users
