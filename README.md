# BaseCore Microservices

Hệ thống microservices cho ứng dụng game account marketplace với admin và user frontend.

## 🏗️ Kiến trúc

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Admin App     │    │   User App      │    │   API Gateway   │
│   (React)       │    │   (Vue.js)      │    │   (Ocelot)      │
│   Port: 5000    │    │   Port: 5000    │    │   Port: 5000    │
│   /admin/       │    │   /user/        │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         └───────────────────────┼───────────────────────┘
                                 │
                    ┌─────────────────┐    ┌─────────────────┐
                    │  Auth Service  │    │   API Service   │
                    │   Port: 5002   │    │   Port: 5001    │
                    │   JWT Auth     │    │ Game/Wallet APIs│
                    └─────────────────┘    └─────────────────┘
                                             │
                                             ▼
                                   ┌─────────────────┐
                                   │   SQL Server    │
                                   │   BaseCoreDB    │
                                   └─────────────────┘
```

## 🚀 Chạy hệ thống

### Cách 1: Chạy từng service (Manual)

```bash
# Terminal 1: Auth Service
cd BaseCore.AuthService
dotnet run --urls "http://localhost:5002"

# Terminal 2: API Service
cd BaseCore.APIService
dotnet run --urls "http://localhost:5001"

# Terminal 3: API Gateway
cd BaseCore.ApiGateway
dotnet run --urls "http://localhost:5000"
```

### Cách 2: Chạy tất cả cùng lúc (Windows)

```bash
# Sử dụng Batch script
start-services.bat

# Hoặc PowerShell script
.\start-services.ps1
```

## 🌐 Truy cập ứng dụng

Sau khi khởi động thành công:

- **API Gateway**: http://localhost:5000
- **Admin App**: http://localhost:5000/admin/
- **User App**: http://localhost:5000/user/
- **API Documentation**: http://localhost:5000/swagger

## 📋 APIs có sẵn

### Authentication
- `POST /api/auth/login` - Đăng nhập
- `POST /api/auth/register` - Đăng ký
- `GET /api/users` - Lấy danh sách users (Admin)

### Game Accounts
- `GET /api/GameAccount` - Danh sách tài khoản game
- `GET /api/GameAccount/my` - Tài khoản của tôi
- `POST /api/GameAccount` - Tạo tài khoản mới
- `POST /api/GameAccount/{id}/purchase` - Mua tài khoản

### Wallet
- `GET /api/Wallet` - Xem số dư ví
- `POST /api/Wallet/deposit` - Nạp tiền
- `POST /api/Wallet/withdraw` - Rút tiền

### Transactions
- `GET /api/Transaction` - Lịch sử giao dịch
- `GET /api/Transaction/pending` - Giao dịch chờ xử lý (Admin)

### E-commerce (Legacy)
- `GET /api/products` - Danh sách sản phẩm
- `GET /api/categories` - Danh sách danh mục
- `GET /api/orders` - Danh sách đơn hàng

## 🗄️ Cơ sở dữ liệu

- **Server**: SQL Server LocalDB
- **Database**: BaseCoreDB
- **Connection**: `Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BaseCoreDB;Integrated Security=True`

### Tables chính:
- `Users` - Thông tin người dùng
- `GameAccounts` - Tài khoản game
- `UserWallets` - Ví tiền người dùng
- `TransactionHistories` - Lịch sử giao dịch
- `Products`, `Categories`, `Orders` - E-commerce

## 🔧 Công nghệ sử dụng

- **Backend**: .NET 8, ASP.NET Core, Entity Framework Core
- **Frontend Admin**: React 18, React Router, Bootstrap
- **Frontend User**: Vue 3, Vue Router, Vuetify
- **API Gateway**: Ocelot
- **Database**: SQL Server
- **Authentication**: JWT Bearer Tokens

## 📁 Cấu trúc thư mục

```
BaseCore/
├── BaseCore.ApiGateway/     # API Gateway (Ocelot)
├── BaseCore.AuthService/    # Authentication Service
├── BaseCore.APIService/     # Main API Service
├── BaseCore.WebClient/      # Admin React App
├── frontend/               # User Vue App
├── BaseCore.Entities/      # Data Models
├── BaseCore.Repository/    # Data Access Layer
├── BaseCore.Services/      # Business Logic Layer
└── start-services.bat      # Startup script
```

## 🐛 Troubleshooting

### Lỗi "Unable to resolve service for type DbContextOptions"
- Đảm bảo chạy migration từ thư mục APIService
- Kiểm tra connection string trong appsettings.json

### Lỗi build
- Chạy `dotnet clean` và `dotnet build`
- Kiểm tra missing using directives

### Frontend không load
- Đảm bảo build files đã được copy vào `ApiGateway/wwwroot/`
- Kiểm tra base path configuration

### API calls fail
- Kiểm tra tất cả services đang chạy
- Verify Ocelot configuration trong `ocelot.json`