CREATE DATABASE FStepDB
USE FStepDB

CREATE TABLE Chat (
    id_chat int IDENTITY NOT NULL,
    chat_msg nvarchar(255) NULL,
    chat_date datetime NULL,
    reciever_user_id nvarchar(7) NULL,
    sender_user_id nvarchar(7) NOT NULL,
    id_post int NOT NULL,
    PRIMARY KEY (id_chat)
);

CREATE TABLE Comment (
    id_comment int IDENTITY NOT NULL,
    content nvarchar(255) NULL,
    [date] datetime NULL,
    id_post int NOT NULL,
    id_user nvarchar(7) NOT NULL,
    PRIMARY KEY (id_comment)
);

CREATE TABLE Feedback (
    id_feedback int IDENTITY NOT NULL,
    content nvarchar(255) NULL,
    rating int NULL,
    id_user nvarchar(7) NOT NULL,
    id_post int NOT NULL,
    PRIMARY KEY (id_feedback)
);

CREATE TABLE Notification (
    id_notification int IDENTITY NOT NULL,
    name nvarchar(255) NULL,
    content nvarchar(255) NULL,
    [date] datetime NULL,
    PRIMARY KEY (id_notification)
);

CREATE TABLE Payment (
    id_payment int IDENTITY NOT NULL,
    pay_time datetime NULL,
    amount float(10) NULL,
    external_momo_transaction_code nvarchar(255) NULL,
    id_transaction int NOT NULL,
    PRIMARY KEY (id_payment)
);

CREATE TABLE Post (
    id_post int IDENTITY NOT NULL,
    content nvarchar(255) NULL,
    [date] datetime NULL,
    img nvarchar(255) NULL,
    status bit NULL,
    type nvarchar(20) NOT NULL,
    detail nvarchar(255) NULL,
    id_product int NULL,
    id_user nvarchar(7) NOT NULL,
    PRIMARY KEY (id_post)
);

CREATE TABLE Product (
    id_product int IDENTITY NOT NULL,
    name nvarchar(255) NULL,
    quantity int NULL,
    price float(10) NULL,
    received_seller_date datetime NULL,
    sent_buyer_date datetime NULL,
    status bit NULL,
    detail nvarchar(255) NULL,
    recieve_img nvarchar(255) NULL,
    sent_img nvarchar(255) NULL,
    PRIMARY KEY (id_product)
);

CREATE TABLE Report (
    id_report int IDENTITY NOT NULL,
    content nvarchar(255) NULL,
    [date] datetime NULL,
    id_comment int NULL,
    id_post int NULL,
    PRIMARY KEY (id_report)
);

CREATE TABLE [Transaction] (
    id_transaction int IDENTITY NOT NULL,
    [date] datetime NULL,
    status bit NULL,
    quantity int NULL,
    amount float(10) NULL,
    note nvarchar(255) NULL,
    id_post int NOT NULL,
    id_user_buyer nvarchar(7) NOT NULL,
    id_user_seller nvarchar(7) NOT NULL,
    type int NULL,
    PRIMARY KEY (id_transaction)
);

CREATE TABLE [User] (
    id_user nvarchar(7) NOT NULL,
    status bit NULL,
    name nvarchar(40) NULL,
    avatar_img nvarchar(255) NULL,
    address nvarchar(50) NULL,
    email nvarchar(50) NULL,
    password nvarchar(50) NOT NULL,
    student_id nvarchar(10) NULL UNIQUE,
    create_date datetime NULL,
    rating int NULL,
    role nvarchar(30) NOT NULL,
    token_google nvarchar(255) NULL,
    PRIMARY KEY (id_user)
);

CREATE TABLE User_Notification (
    id_user nvarchar(7) NOT NULL,
    id_notification int NOT NULL,
    type nvarchar(50) NULL,
    id_report int NULL,
    id_comment int NULL,
    id_payment int NULL,
    id_transaction int NULL,
    PRIMARY KEY (id_user, id_notification)
);

ALTER TABLE User_Notification
ADD CONSTRAINT FKUser_Notif453825 FOREIGN KEY (id_user) REFERENCES [User] (id_user);

ALTER TABLE User_Notification
ADD CONSTRAINT FKUser_Notif974340 FOREIGN KEY (id_notification) REFERENCES Notification (id_notification);

ALTER TABLE Feedback
ADD CONSTRAINT FKFeedback469931 FOREIGN KEY (id_user) REFERENCES [User] (id_user);

ALTER TABLE Feedback
ADD CONSTRAINT FKFeedback927020 FOREIGN KEY (id_post) REFERENCES Post (id_post);

ALTER TABLE [Transaction]
ADD CONSTRAINT FKTransactio922554 FOREIGN KEY (id_post) REFERENCES Post (id_post);

ALTER TABLE Comment
ADD CONSTRAINT FKComment984866 FOREIGN KEY (id_post) REFERENCES Post (id_post);

ALTER TABLE Comment
ADD CONSTRAINT FKComment441956 FOREIGN KEY (id_user) REFERENCES [User] (id_user);

ALTER TABLE Report
ADD CONSTRAINT FKReport93927 FOREIGN KEY (id_comment) REFERENCES Comment (id_comment);

ALTER TABLE Report
ADD CONSTRAINT FKReport245773 FOREIGN KEY (id_post) REFERENCES Post (id_post);

ALTER TABLE Chat
ADD CONSTRAINT FKChat407738 FOREIGN KEY (sender_user_id) REFERENCES [User] (id_user);

ALTER TABLE Chat
ADD CONSTRAINT FKChat970520 FOREIGN KEY (id_post) REFERENCES Post (id_post);

ALTER TABLE Payment
ADD CONSTRAINT FKPayment371031 FOREIGN KEY (id_transaction) REFERENCES [Transaction] (id_transaction);

ALTER TABLE Post
ADD CONSTRAINT FKPost405877 FOREIGN KEY (id_product) REFERENCES Product (id_product);

ALTER TABLE User_Notification
ADD CONSTRAINT FKUser_Notif804938 FOREIGN KEY (id_report) REFERENCES Report (id_report);

ALTER TABLE User_Notification
ADD CONSTRAINT FKUser_Notif720975 FOREIGN KEY (id_comment) REFERENCES Comment (id_comment);

ALTER TABLE User_Notification
ADD CONSTRAINT FKUser_Notif552335 FOREIGN KEY (id_payment) REFERENCES Payment (id_payment);

ALTER TABLE User_Notification
ADD CONSTRAINT FKUser_Notif556883 FOREIGN KEY (id_transaction) REFERENCES [Transaction] (id_transaction);

ALTER TABLE Post
ADD CONSTRAINT FKPost118863 FOREIGN KEY (id_user) REFERENCES [User] (id_user);

ALTER TABLE [Transaction]
ADD CONSTRAINT FKTransactio22282 FOREIGN KEY (id_user_buyer) REFERENCES [User] (id_user);


INSERT INTO [User] (id_user, status, name, avatar_img, address, email, password, student_id, create_date, rating, role, token_google)
VALUES 
    ('user001', 1, 'John Doe', 'avatar1.jpg', '123 Main St, City, Country', 'john.doe@example.com', 'password123', 'STUDENT001', '2023-01-15', 4, 'customer', NULL),
    ('user002', 1, 'Jane Smith', 'avatar2.jpg', '456 Oak Ave, Town, Country', 'jane.smith@example.com', 'password456', 'STUDENT002', '2023-02-20', 5, 'customer', NULL),
    ('user003', 1, 'Michael Johnson', 'avatar3.jpg', '789 Pine Rd, Village, Country', 'michael.johnson@example.com', 'password789', 'STUDENT003', '2023-03-10', 3, 'admin', NULL),
    ('user004', 1, 'Emily Davis', 'avatar4.jpg', '321 Elm Blvd, Suburb, Country', 'emily.davis@example.com', 'passwordabc', 'STUDENT004', '2023-04-05', 4, 'customer', NULL),
    ('user005', 1, 'Chris Wilson', 'avatar5.jpg', '654 Cedar Ln, District, Country', 'chris.wilson@example.com', 'passwordxyz', 'STUDENT005', '2023-05-01', 5, 'customer', 'google_token_xyz');


INSERT INTO Post (content, [date], img, status, type, detail, id_product, id_user)
VALUES 
    ('Hello everyone!', '2023-05-01 10:15:00', 'img1.jpg', 1, 'Sale', 'First post content.', 1, 'user001'),
    ('Check out this photo!', '2023-05-02 14:30:00', 'img2.jpg', 1, 'Exchange', 'A nice landscape.', 2, 'user002'),
    ('Product review', '2023-05-03 09:00:00', NULL, 0, 'Sale', 'Detailed review here.', 5, 'user003'),
    ('New blog post', '2023-05-04 16:45:00', NULL, 1, 'Exchange', 'Exciting content.', 3, 'user001'),
    ('Event announcement', '2023-05-05 11:20:00', 'event.jpg', 1, 'Sale', 'Upcoming event details.', 4, 'user004');


INSERT INTO Product (name, quantity, price, received_seller_date, status, detail, recieve_img)
VALUES 
    ('Apple', 100, 1.5, '2023-04-25', 1, 'Fresh apples.', 'apple.jpg'),
    ('Banana', 150, 0.8, '2023-04-28', 1, 'Yellow bananas.', 'banana.jpg'),
    ('Orange', 120, 1.2, '2023-04-30', 1, 'Juicy oranges.', 'orange.jpg'),
    ('Grapes', 80, 2.0, '2023-05-02', 1, 'Sweet grapes.', 'grapes.jpg'),
    ('Pineapple', 50, 3.0, '2023-05-05', 1, 'Fresh pineapple.', 'pineapple.jpg');