use duco_db;

DELIMITER //
CREATE PROCEDURE add_user_secure(
    username NVARCHAR(16), 
    user_password NVARCHAR(50)
)
BEGIN
    -- Generate a random salt
    DECLARE salt NVARCHAR(36) DEFAULT UUID();
    
    -- Create a salted hash of the password
    DECLARE hashed_password NVARCHAR(64) DEFAULT SHA2(CONCAT(user_password, salt), 256);
    
    -- Insert the username, hashed password, and salt into the users table
    INSERT INTO users (user_name, users_password, salt)
    VALUES(username, hashed_password, salt);
END//
DELIMITER ;

call add_user("test", "password");
