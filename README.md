# MomentOfUs-API

This repository contains the backend API for the Moment of Us mobile app, a journaling app designed to help individuals and couples capture and share their life experiences, thoughts, and feelings.

## About Moment of Us

Moment of Us provides a platform for users to:

* Create and manage private or shared journals.
* Express themselves through text, photos, and mood tracking.
* Set and track personal or shared goals.
* Connect with loved ones through shared journaling experiences.

## Features

* **User Authentication:** Secure user registration and login with JWT (JSON Web Tokens).
* **Journal Management:** Create, read, update, and delete journal entries.
* **Sharing and Collaboration:** Share journals with other users and control access permissions.
* **Mood Tracking:**  Associate moods with journal entries to track emotional trends.
* **Photo Uploads:**  Upload and store photos within journal entries.
* **Real-time Collaboration:**  Real-time updates for shared journals using SignalR.

## Technology Stack

* ASP.NET Core Web API: Framework for building RESTful APIs.
* C#: Programming language for backend logic.
* Entity Framework Core: ORM for data access.
* SQLite: Database for storing data (with potential for future migration to PostgreSQL).
* SignalR:  Library for real-time communication.
* Swagger/OpenAPI: For API documentation.

## Getting Started

1. **Clone the repository:** `git clone https://github.com/your-username/MomentOfUs-API.git`
2. **Open the project:** Open the `MomentOfUs-API.sln` solution file in Visual Studio 2022.
3. **Restore dependencies:** Make sure all the required NuGet packages are installed.
4. **Configure the database:** Update the connection string in `appsettings.json` to point to your SQLite database.
5. **Build and run:** Build the project and run it. The API will be available at the configured URL (e.g., `https://localhost:5001`).

## API Documentation

* **[Link to API documentation (Swagger/OpenAPI)]** 
    * Example: `https://your-api-url/swagger`

## Wiki

For more detailed documentation, including the development plan, database schema, and deployment instructions, please refer to the project wiki:

* **[Link to Wiki](https://github.com/Abdullahtariq11/MomentOfUs-API/wiki)`

## Contributing

If you'd like to contribute to this project, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Make your changes and commit them with clear messages.
4. Push your branch to your forked repository.
5. Create a pull request to the main repository.

## License

This project is licensed under the [MIT License](LICENSE).

## Contact

Abdullah Tariq

## Acknowledgements

* [List any third-party libraries or resources used]
