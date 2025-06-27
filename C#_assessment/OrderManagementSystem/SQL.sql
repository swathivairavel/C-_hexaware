create database OrderManagementSystem

use OrderManagementSystem

create table product (
    productid int primary key identity,
    productname varchar(100) not null,
    [description] text,
    price decimal(10, 2) not null,
    quantityinstock int not null,
    type varchar(50)
)
alter table product add brand varchar(100) 
alter table product add warrantyperiod int 
alter table product add size varchar(50) 
alter table product add color varchar(50) 

create table [user] (
    userid int primary key identity,
    username varchar(100) not null,
    [password] varchar(100) not null,
    [role] varchar(50) not null
)

create table orders (
    orderid int primary key identity(1,1),
    userid int not null,
    orderdate datetime default getdate(),
    foreign key (userid) references [user](userid)
)


create table order_item (
    orderitemid int primary key identity(1,1),
    orderid int not null,
    productid int not null,
    quantity int not null,

    foreign key (orderid) references orders(orderid),
    foreign key (productid) references product(productid)
)


