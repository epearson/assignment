# Assignment: Part 1
Language: Node.js v14.17.6

OS: Mac OS 11.5.2

IDE: Visual Studio Code

## Setup
### Install ActiveMQ
* brew install apache-activemq

Start/Stop ActiveMQ


https://stackoverflow.com/questions/64323055/find-port-of-activemq-in-macos

* activemq start
* activemq stop

Test credentials: admin/admin

# Run the app
* npm start

## File Summary
* index.js - Entrypoint to the application
* service.js - Worker for multi-threading, JMS message producer