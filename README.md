# fyp_code_v2
2nd attempt
# OrderKaro — Cafe Ordering & Forecasting System

OrderKaro is a **university cafe ordering system** with integrated **sales forecasting**.  
It allows students to place orders online, staff to manage orders in real time, and admins to forecast demand for better inventory and resource planning.

---

## 🔑 Features
- 📱 Place orders through a mobile app and track orders through a web interface.  
- 👨‍🍳 Staff dashboard to update and complete orders live.  
- 🛠️ Admin panel for menu, users, and reports.  
- 📊 Forecasting module (Python Prophet) to predict future sales.
- RFID cards connected with student accounts for easy balance top up and order payment.  

---

## 🧰 Tech Stack
- **Frontend:** Angular, TypeScript, TailwindCSS  
- **Backend:** ASP.NET Core (C#), Entity Framework Core  
- **Database:** SQL Server  
- **Forecasting:** Python, Pandas, Prophet  

---

## 📂 Project Structure
OrderKaro_Final_Year_Project/
├── backend/ # ASP.NET Core backend
├── frontend/ # Angular frontend
├── forecaster/ # Python forecasting scripts
└── README.md # Project documentation

---

## 🚀 Getting Started

### 1. Clone the project
```bash
git clone https://github.com/Muhammad-Mak/OrderKaro_Final_Year_Project.git
cd OrderKaro_Final_Year_Project

2. Run the backend

- open the sln file in visual studio 

- or

cd fyp_backend
dotnet restore
dotnet ef database update
dotnet run


3. Run the frontend

cd fyp_frontend
npm install
ng serve 

4. Run the forecaster

cd fyp_forecaster
pip install -r requirements.txt
python forecast.py

