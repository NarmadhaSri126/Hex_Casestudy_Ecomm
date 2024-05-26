create table customers(
customer_id int primary key identity(1,1),
name varchar(255) not null,
email varchar(255) unique not null,
password varchar(255) not null check(len(password)>=6))

create table products(
product_id int primary key identity(1,1),
name varchar(255) not null,
price decimal(8,2) not null check(price>0),
description varchar(255) not null,
stockquantity int not null check(stockquantity>=0))

create table cart(
cart_refid int primary key identity(1,1),
cart_id int references customers(customer_id) not null,
customer_id int references customers(customer_id) not null,
product_id int references products(product_id) not null,
quantity int not null check(quantity>0) )

create table orders(
order_id int primary key IDENTITY(1,1),
customer_id int references customers(customer_id) not null,
product_id int references products(product_id) not null,
quantity int not null check(quantity>0),
total_price decimal(10,2) not null check(total_price>0),
order_date DATETIME DEFAULT GETDATE(),
shipping_address varchar(255) not null);

select * from customers
select * from products
select * from cart
select * from orders
