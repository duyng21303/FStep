CREATE DATABASE FStepDB

USE FStepDB

CREATE TABLE Chat (id_chat int IDENTITY NOT NULL, chat_msg nvarchar(255) NULL, chat_date datetime NULL, reciever_user_id nvarchar(50) NULL, sender_user_id nvarchar(50) NOT NULL, id_post int NULL, PRIMARY KEY (id_chat));
CREATE TABLE Comment (id_comment int IDENTITY NOT NULL, content ntext NULL, [date] datetime NULL, img nvarchar(255) NULL, type nvarchar(255) NULL, id_post int NOT NULL, id_user nvarchar(50) NOT NULL, PRIMARY KEY (id_comment));
CREATE TABLE Confirm (id_confirm int IDENTITY NOT NULL, id_user_confirm nvarchar(50) NULL, id_user_connect nvarchar(50) NULL, confirm bit NULL, id_post int NULL, id_comment int NULL, PRIMARY KEY (id_confirm));
CREATE TABLE Feedback (id_feedback int IDENTITY NOT NULL, content nvarchar(255) NULL, rating int NULL, id_user nvarchar(50) NOT NULL, id_post int NOT NULL, PRIMARY KEY (id_feedback));
CREATE TABLE Notification (id_notification int IDENTITY NOT NULL, name nvarchar(255) NULL, content nvarchar(255) NULL, [date] datetime NULL, PRIMARY KEY (id_notification));
CREATE TABLE Payment (id_payment int IDENTITY NOT NULL, pay_time datetime NULL, amount float(10) NULL, external_momo_transaction_code nvarchar(255) NULL, type nvarchar(255) NULL, id_transaction int NOT NULL, PRIMARY KEY (id_payment));
CREATE TABLE Post (id_post int IDENTITY NOT NULL, content nvarchar(255) NULL, [date] datetime NULL, img nvarchar(255) NULL, status nvarchar(255) NULL, type nvarchar(20) NOT NULL, detail ntext NULL, location nvarchar(255) NULL, id_product int NULL, id_user nvarchar(50) NOT NULL, PRIMARY KEY (id_post));
CREATE TABLE Product (id_product int IDENTITY NOT NULL, quantity int NULL, price float(10) NULL, received_seller_date datetime NULL, sent_buyer_date datetime NULL, status nvarchar(255) NULL, recieve_img nvarchar(255) NULL, sent_img nvarchar(255) NULL, item_location nvarchar(255) NULL, sold_quantity int NULL, PRIMARY KEY (id_product));
CREATE TABLE Report (id_report int IDENTITY NOT NULL, content nvarchar(255) NULL, [date] datetime NULL, id_comment int NULL, id_post int NULL, PRIMARY KEY (id_report));
CREATE TABLE [Transaction] (id_transaction int IDENTITY NOT NULL, [date] datetime NULL, status nvarchar(255) NULL, quantity int NULL, amount float(10) NULL, note nvarchar(255) NULL, unit_price float(10) NULL, id_post int NOT NULL, id_user_buyer nvarchar(50) NOT NULL, id_user_seller nvarchar(50) NOT NULL, type nvarchar(255) NULL, code_transaction nvarchar(255) NULL, PRIMARY KEY (id_transaction));
CREATE TABLE [User] (id_user nvarchar(50) NOT NULL, status bit NULL, name nvarchar(40) NULL, avatar_img nvarchar(255) NULL, address nvarchar(100) NULL, email nvarchar(50) NULL, password nvarchar(50) NOT NULL, student_id nvarchar(10) NULL, create_date datetime NULL, rating int NULL, role nvarchar(30) NOT NULL, token_google nvarchar(255) NULL, hash_key nvarchar(255) NULL, gender nvarchar(255) NULL, reset_token nvarchar(255) NULL, bank_name int NULL, bank_account_number bigint NULL, PRIMARY KEY (id_user));
CREATE TABLE User_Notification (id_user nvarchar(50) NOT NULL, id_notification int NOT NULL, type nvarchar(50) NULL, id_report int NULL, id_comment int NULL, id_payment int NULL, id_transaction int NULL, PRIMARY KEY (id_user, id_notification));
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif453825 FOREIGN KEY (id_user) REFERENCES [User] (id_user);
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif974340 FOREIGN KEY (id_notification) REFERENCES Notification (id_notification);
ALTER TABLE Feedback ADD CONSTRAINT FKFeedback469931 FOREIGN KEY (id_user) REFERENCES [User] (id_user);
ALTER TABLE Feedback ADD CONSTRAINT FKFeedback927020 FOREIGN KEY (id_post) REFERENCES Post (id_post);
ALTER TABLE [Transaction] ADD CONSTRAINT FKTransactio922554 FOREIGN KEY (id_post) REFERENCES Post (id_post);
ALTER TABLE Comment ADD CONSTRAINT FKComment984866 FOREIGN KEY (id_post) REFERENCES Post (id_post);
ALTER TABLE Comment ADD CONSTRAINT FKComment441956 FOREIGN KEY (id_user) REFERENCES [User] (id_user);
ALTER TABLE Report ADD CONSTRAINT FKReport93927 FOREIGN KEY (id_comment) REFERENCES Comment (id_comment);
ALTER TABLE Report ADD CONSTRAINT FKReport245773 FOREIGN KEY (id_post) REFERENCES Post (id_post);
ALTER TABLE Chat ADD CONSTRAINT FKChat407738 FOREIGN KEY (sender_user_id) REFERENCES [User] (id_user);
ALTER TABLE Chat ADD CONSTRAINT FKChat970520 FOREIGN KEY (id_post) REFERENCES Post (id_post);
ALTER TABLE Payment ADD CONSTRAINT FKPayment371031 FOREIGN KEY (id_transaction) REFERENCES [Transaction] (id_transaction);
ALTER TABLE Post ADD CONSTRAINT FKPost405877 FOREIGN KEY (id_product) REFERENCES Product (id_product);
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif804938 FOREIGN KEY (id_report) REFERENCES Report (id_report);
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif720975 FOREIGN KEY (id_comment) REFERENCES Comment (id_comment);
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif552335 FOREIGN KEY (id_payment) REFERENCES Payment (id_payment);
ALTER TABLE User_Notification ADD CONSTRAINT FKUser_Notif556883 FOREIGN KEY (id_transaction) REFERENCES [Transaction] (id_transaction);
ALTER TABLE Post ADD CONSTRAINT FKPost118863 FOREIGN KEY (id_user) REFERENCES [User] (id_user);
ALTER TABLE [Transaction] ADD CONSTRAINT FKTransactio22282 FOREIGN KEY (id_user_buyer) REFERENCES [User] (id_user);
ALTER TABLE Confirm ADD CONSTRAINT FKConfirm703812 FOREIGN KEY (id_post) REFERENCES Post (id_post);
ALTER TABLE Confirm ADD CONSTRAINT FKConfirm635887 FOREIGN KEY (id_comment) REFERENCES Comment (id_comment);