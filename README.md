# Microservice project - Part A - ecommerce application with backend in ASP.NET and frontend in Angular

This is the part A of my project. The part B, which should be launched after part A can be accessed [Here](https://github.com/giangNguyen2007/microservice_project_part2_bank_SpringBoot_and_Angular)

My project consists of two applications:

- Part A : ecommerce applcation with  frontend in Angular and backend in ASP.NET  
- Part B : bank application with frontend in Angular and backend in Spring. 

The two applications are linked in the payment process : The ecommerce application's payment operation is linked to the bank application's transaction management.

I have developed this project as a way to learn ASP.NET, SPRING, Angular as well as microservice backend development. 


### Note on time constraint for frontend development

<span style="color: orange;"> Planning initially to create only the 2 backend microservice applications in Spring & ASP.NET, I have spent most of time on learning them, which are not part of ENSIMAG cursus. The microservices in themselves are similar in structure and thus do not take too much time, but learning to apply gRPC and rabbitMQ communication between them, then ensuring correct message exchange (with automatic and manuel testing) are quite time-consuming.  

This leaves me with only little time to learn Angular and develop 2 frontends (part A & B) before the start of new semester at ENSIMAG. The deployment on Docker-Compose has also taken a lot of time because inter-container communication leads to complication that requires further adjustement.</span>

<span style="color: orange;">Thus I have to accept to present a frontend version with just basic functionalities. More advanced features could be added during the semester.  </span>


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


# 2. Frontend overview


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


# 3. Backend overview

The backend is composed of 5 microservices:
+ Gateway : all other services are exposed via the gateway url 
+ AuthAPI : handles user registration and login
+ ProductAPI : handles Product CRUD operations, as well as stock management
+ OrderAPI: handles Order CRUD operations. At order creation request, it communicates with `ProductAPI` (via GRPC channel) to reserve stock, and publish new payment event (via RabbitMQ) to `PaymentAPI`. 
+ PaymentAPI : handle payment operation and  communication with Bank application 

To avoid creating too many containers, I have replaced most of MySQL databases, chosen initially, by Sqlite, which allows storing database file inside the API container. 

![Asp.net Endpoints](./bankApp_ASP.NET-.NET%20Backend%20General%20Schema.drawio.png)

<!-- ## 2.1 technical highlights

I would like to hightlight the folollwing technical points :
- Separation of concern: separate service layers for database interaction, grpc & rabbitMQ communication are handled in separate services in each API
- Authentication is performed at Gateway level, which decodes Jwt token then attachs user info to the Request Object. At Downstream API, request passes through authentication Interceptor before reaching controller.  
- Any database services' function involving more than one database operations is annotated with @Transaction -->


Further improvement idea (if time available):
- implement concurrency control on database operations
- deployment on Kubernetes to get service replication 
- apply SAGA pattern to ensure coherent data state between (rollback in case of failure )
- implement retry mechanism in case of message transfer failure
- implement idempotency


### Services communication

GRPC channel is used for synchronous information exchange between `OrderAPI` (client) and `ProductAPI` (server) for product stock management (reservation, consummation, return): the message sender needs the response info to continue its execution flow.

RabbitMq is used for asynchronous message exchange between `OrderAPI` and `PaymentAPI` (for publishing new payment request or payment result) : the sender just needs to inform the destination service of new event. 


![Asp.net communicaction flow](./bankApp_ASP.NET-ASP.NET%20communication%20flow.drawio.png)









