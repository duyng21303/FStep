CREATE DATABASE FSTEP
USE FSTEP
CREATE TABLE Comment (
id_comment int IDENTITY NOT NULL, 
content nvarchar(255) NULL, 
[date] datetime NULL, 
id_user nvarchar(7) NOT NULL, 
id_post int NOT NULL, 
PRIMARY KEY (id_comment)
);

CREATE TABLE Feedback (
id_feedback int IDENTITY NOT NULL, 
content int NULL, 
rating int NULL, 
id_user nvarchar(7) NOT NULL, 
id_post int NOT NULL, 
PRIMARY KEY (id_feedback)
);

CREATE TABLE Notification (
id_notification int IDENTITY NOT NULL,
content nvarchar(255) NULL, 
[date] datetime NULL, 
PRIMARY KEY (id_notification)
);

CREATE TABLE Notification_Type (
id_type_Notif int IDENTITY NOT NULL, 
name nvarchar(50) NULL, 
PRIMARY KEY (id_type_Notif)
);

CREATE TABLE Payment (
id_payment int IDENTITY NOT NULL, 
pay_time datetime NULL, 
amount money NULL, 
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
feedback int NULL, 
id_user nvarchar(7) NOT NULL, 
id_type int NOT NULL, 
id_product int NULL, 
PRIMARY KEY (id_post)
);

CREATE TABLE Post_Type (
id_type int IDENTITY NOT NULL, 
Role nvarchar(20) NULL, 
PRIMARY KEY (id_type)
);

CREATE TABLE Product (
id_product int IDENTITY NOT NULL, 
name nvarchar(50) NULL, 
quantity int NULL, 
price float(10) NULL, 
received_seller_date datetime NULL, 
sent_buyer_date datetime NULL, 
status int NULL, 
recieve_img nvarchar(255) NULL, 
sent_img nvarchar(255) NULL, 
PRIMARY KEY (id_product)
);

CREATE TABLE Report (
id_report int IDENTITY NOT NULL, 
content nvarchar(255) NULL, 
[date] datetime NULL, 
id_post int NOT NULL, 
id_comment int NOT NULL, 
PRIMARY KEY (id_report)
);

CREATE TABLE Role (
id_role int IDENTITY NOT NULL, 
role_name int NULL, 
PRIMARY KEY (id_role)
);

CREATE TABLE [Transaction] (
id_transaction int IDENTITY NOT NULL, 
[date] datetime NULL, 
status bit NULL, 
quantity int NULL, 
amount money NULL, 
note nvarchar(255) NULL, 
id_user_seller nvarchar(7) NULL, 
id_post int NOT NULL, 
[id_user _buyer] nvarchar(7) NOT NULL, 
PRIMARY KEY (id_transaction)
);

CREATE TABLE [User] (
id_user nvarchar(7) NOT NULL, 
status bit NULL, 
name nchar(40) NULL, 
avatar_img nvarchar(255) NULL, 
address nvarchar(50) NULL, 
email nvarchar(50) NOT NULL, 
password nvarchar(50) NOT NULL, 
student_id nvarchar(10) NOT NULL UNIQUE, 
create_date datetime NULL, 
rating int NULL, 
id_role int NOT NULL, 
PRIMARY KEY (id_user)
);

CREATE TABLE User_Notification (
id_user nvarchar(7) NOT NULL, 
id_notification int NOT NULL, 
id_type_Notif int NOT NULL, 
id_comment int NULL, 
id_transaction int NULL, 
id_payment int NULL, 
id_report int NULL, 
PRIMARY KEY (id_user, id_notification)
);

ALTER TABLE [User] ADD CONSTRAINT FKUser245539 FOREIGN KEY (id_role) REFERENCES Role (id_role);
ALTER TABLE Post ADD CONSTRAINT FKPost118863 FOREIGN KEY (id_user) REFERENCES [User] (id_user);
ALTER TABLE Post ADD CONSTRAINT FKPost405877 FOREIGN KEY (id_product) REFERENCES Product (id_product);
ALTER TABLE Comment ADD CONSTRAINT FKComment441956 FOREIGN KEY (id_user) REFERENCES [User] (id_user);
ALTER TABLE Comment ADD CONSTRAINT FKComment984866 FOREIGN KEY (id_post) REFERENCES Post (id_post);
ALTER TABLE Report ADD CONSTRAINT FKReport245773 FOREIGN KEY (id_post) REFERENCES Post (id_post);
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif974340 FOREIGN KEY (id_notification) REFERENCES Notification (id_notification);
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif453825 FOREIGN KEY (id_user) REFERENCES [User] (id_user);
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif621415 FOREIGN KEY (id_type_Notif) REFERENCES Notification_Type (id_type_Notif);
ALTER TABLE Post ADD CONSTRAINT FKPost783430 FOREIGN KEY (id_type) REFERENCES Post_Type (id_type);
ALTER TABLE [Transaction] ADD CONSTRAINT FKTransactio922554 FOREIGN KEY (id_post) REFERENCES Post (id_post);
ALTER TABLE Payment ADD CONSTRAINT FKPayment371031 FOREIGN KEY (id_transaction) REFERENCES [Transaction] (id_transaction);
ALTER TABLE Report ADD CONSTRAINT FKReport93927 FOREIGN KEY (id_comment) REFERENCES Comment (id_comment);
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif720975 FOREIGN KEY (id_comment) REFERENCES Comment (id_comment);
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif556883 FOREIGN KEY (id_transaction) REFERENCES [Transaction] (id_transaction);
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif552335 FOREIGN KEY (id_payment) REFERENCES Payment (id_payment);
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif804938 FOREIGN KEY (id_report) REFERENCES Report (id_report);
ALTER TABLE [Transaction] ADD CONSTRAINT FKTransactio267620 FOREIGN KEY ([id_user _buyer]) REFERENCES [User] (id_user);
ALTER TABLE Feedback ADD CONSTRAINT FKFeedback469931 FOREIGN KEY (id_user) REFERENCES [User] (id_user);
ALTER TABLE Feedback ADD CONSTRAINT FKFeedback927020 FOREIGN KEY (id_post) REFERENCES Post (id_post);
ALTER TABLE Payment ADD CONSTRAINT FKPayment371032 FOREIGN KEY (id_transaction) REFERENCES [Transaction] (id_transaction);
ALTER TABLE [Transaction] ADD CONSTRAINT FKTransactio922555 FOREIGN KEY (id_post) REFERENCES Post (id_post);
