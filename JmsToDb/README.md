# Assignment: Part 2
Language: C# .NET Core v5.0.400

OS: Mac OS 11.5.2

IDE: Visual Studio Code

Database: SQLite

## Setup
* Install C# Extension into Visual Studio Code
* Install .NET SDK for macOS
* run dotnet new console --framework net5.0

## File Summary
* Program.cs - For this assignment, all program code contained in this file.
* posts.sqlite - SQLite database file

## Run the app
* dotnet run

## Class Summary
* Program - Entrypoint to the application
* Post - Simple representation of a single 'post' from the web service used for both JSON deserialization and as an entity for writing to the database
* PostDbContext - DbContext for the Microsoft Entity Framework to connect and write to SQLite database
* PostProcessor - The core logic of the application which includes reading from the JMS queue and writing to the database

## References
JSON Deserialization

https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-5-0