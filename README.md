# FSTep - Real-Time Trading and Exchange Platform for FPT University Students


## About The Project
![1](https://github.com/user-attachments/assets/69e6567a-f938-4e2e-a67c-e9876134efdf)

**FSTep** is a dynamic online trading and exchange platform designed exclusively for students of FPT University. This platform enables students to buy, sell, and exchange items in real-time while facilitating real-time chat to enhance user interaction. With integrated payment solutions via VNPAY and cutting-edge technology like SignalR, FSTep ensures a seamless and interactive user experience tailored specifically for the FPT student community.

### Key Features

- **Real-Time Trading and Exchange:** Students can buy, sell, and exchange items in a secure, real-time environment.
- **Real-Time Chat:** Engage in live conversations with other users, making transactions smoother and more interactive.
- **Secure Payment via VNPAY:** A trusted and secure payment gateway, ensuring safe transactions for all users.
- **SignalR Integration:** Leveraging SignalR technology for real-time updates and seamless communication between users.
- **User-Friendly Interface:** Designed with students in mind, the platform is intuitive and easy to navigate.


## Built With

FSTep is built using a robust technology stack to ensure reliability and performance.

- **ASP.NET Core:** The backbone of the platform, providing a strong and scalable framework.
- **SignalR:** For real-time communication, ensuring instant updates and interactions.
- **VNPAY:** A secure payment gateway integrated into the platform.
- **Entity Framework Core:** For efficient database management and operations.
- **Bootstrap:** For a responsive and user-friendly interface.
- **jQuery:** Simplifying client-side scripting and enhancing user interactions.


## Getting Started

To get started with FSTep, follow the instructions below to set up the project locally on your machine.

### Installation

1. **Clone the repository:**
   ```sh
   git clone https://github.com/duyng21303/FStep
   ```
2. **Navigate to the project directory:**
   ```sh
   cd fstep
   ```
3. **Restore the .NET packages:**
   ```sh
   dotnet restore
   ```
4. **Set up the database:**
   - Update the connection string in `appsettings.json`.
   - Apply migrations to set up the database schema:
     ```sh
     dotnet ef database update
     ```
5. **Run the application:**
   ```sh
   dotnet run
   ```


## Usage

FSTep can be used for a variety of student-focused activities, including:

- **Buying and Selling:** Easily list items for sale or browse and purchase items listed by other students.
- **Real-Time Exchanges:** Initiate and negotiate exchanges in real-time.
- **Live Chat:** Communicate instantly with potential buyers or sellers.
- **Secure Payments:** Complete transactions securely using VNPAY.

### Screenshots

![image](https://github.com/user-attachments/assets/699bbca0-16e5-40e0-bd66-589cccc8afef)
![image](https://github.com/user-attachments/assets/a420ff8c-b57a-42c6-8e9d-59bbdb4ca925)


## Contributing

Contributions to FSTep are welcome and encouraged! To contribute:

1. Fork the project.
2. Create your feature branch (`git checkout -b feature/AmazingFeature`).
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`).
4. Push to the branch (`git push origin feature/AmazingFeature`).
5. Open a Pull Request.

## Acknowledgments

Special thanks to the following resources and individuals who contributed to the development of FSTep:

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [SignalR Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr/)
- [VNPAY Integration Guide](https://sandbox.vnpayment.vn/)

