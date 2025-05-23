CREATE TABLE "Users" (
    id SERIAL PRIMARY KEY,
    username VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "UserActivity" (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    activity_type VARCHAR(50) NOT NULL, -- e.g., 'update_username', 'update_email', 'update_password', 'login'
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    old_value TEXT, -- Store the old value before the update
    new_value TEXT, -- Store the new value after the update
    ip_address VARCHAR(45), -- Store the IP address of the user
    device_info TEXT, -- Store device information (e.g., browser, OS)
    FOREIGN KEY (user_id) REFERENCES "Users"(id) ON DELETE CASCADE
);