# microservice_project_ecom_ASP.NET_and_Angular

Microservice project - the ecommerce application with backend in ASP.NET and frontend in Angular

I have spent most of time on learning to develop the two backend applications. This leaves me with only little time to learn Angular and develop frontend feature before the start of new semester at ENSIMAG. The deployment on Docker-Compose alto takes a lot of time as inter-container communication leads to complication that requires code adjustement. 

Thus i have to accept to present this frontend version with just basic functionalities. More advanced features could be added during the semester. 


## 1. Application Launch Steps

(1) Create docker network to be shared by both applications

```bash

docker network create aspspring-project-net
```

(1) Start rabbit Mq container

```bash

docker run -d --name rabbitmq --network aspspring-project-net -p 5672:5672 -p 15672:15672 rabbitmq:management
```


(2) Build the images for both front and backend services:

```bash
#inside/backend-asp.net
docker-compose build

#inside /frontend-angular
docker-compose build
# this build the image for 

```

The image building process involves compiling the source code for all microservices, thus this can take several minutes to finish.


(3) Launch the containers

```bash
#inside/backend-asp.net
docker-compose up -d

#inside /frontend-angular
docker-compose up -d

```

The frontend is accessible at localhost:81



# 2. Backend application overview

The application is composed of 5 microservices:
+ Gateway : all other services are exposed via the gateway url 
+ AuthAPI : handles user registration and login
+ ProductAPI : handles Product CRUD operations, as well as stock management
+ OrderAPI: handles Order CRUD operations. At order creation request, it communicates with `ProductAPI` (via GRPC channel) to reserve stock, and publish new payment event (via RabbitMQ) to `PaymentAPI`. 
+ PaymentAPI : handle payment operation and  communication with Bank application 


To avoid creating too many containers, I have replaced most of MySQL databases, chosen initially, by Sqlite, which allows storing database file inside the API container. 

![Asp.net Endpoints](./bankApp_ASP.NET-.NET%20Backend%20General%20Schema.drawio.png)


## 2.2 Services communication

GRPC channel is used for synchronous information exchange between `OrderAPI` (client) and `ProductAPI` (server) for product stock management (reservation, consummation, return): the message sender needs the response info to continue its execution flow.

RabbitMq is used for asynchronous message exchange between `OrderAPI` and `PaymentAPI` (for publishing new payment request or payment result) : the sender just needs to inform the destination service of new event. 


![Asp.net communicaction flow](./bankApp_ASP.NET-ASP.NET%20communication%20flow.drawio.png)



### 2.2.3 Http communication to Bank application


# 3. Frontend Application overview


## 3.1 Site interface

The frontend allows user to perform the following actions:
- Register and login as normal user 
- Add product to cart
- Place order by entering payment information
- View order history


The interface for (1) creating new product, (2) deleting product, (3) product stock are only visible to admin, whose credential is: email = admin@gmail.com, password = admin.


It is in payment and transaction validation steps that **the linkage between the `ecommerce application` and the `bank application` ** is applied: 
+ The `bank account id` obtained in `bank application` serves as payment info for check-out in `ecommerce application`. 
+ Once checkout completed, the order is in `PENDING PAYMENT` status. ***For each product, the ordered quantity is deducted from stock and added to reserved stock***. In the bank account, a new `incoming transaction` is created, to be validated and refused by account owner, similar to real life payment flow.
+ After validation of pending transaction, new transaction is created in transaction history. In the ecommerce application, the order status is changed to `CONFIRMED`, and the reserved stock is reduced. 




