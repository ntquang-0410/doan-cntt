using System;

namespace ConvenienceStoreApp
{
    public static class SessionManager
    {
        private static int _userId = -1;
        private static string _username;
        private static string _fullName;
        private static string _role;
        private static int _currentShiftId = -1;
        private static bool _isDemoMode = false;

        public static int UserId 
        { 
            get { return _userId; } 
            set { _userId = value; } 
        }

        public static string Username 
        { 
            get { return _username; } 
            set { _username = value; } 
        }

        public static string FullName 
        { 
            get { return _fullName; } 
            set { _fullName = value; } 
        }

        public static string Role 
        { 
            get { return _role; } 
            set { _role = value; } 
        }

        public static int CurrentShiftId 
        { 
            get { return _currentShiftId; } 
            set { _currentShiftId = value; } 
        }

        public static bool IsDemoMode
        {
            get { return _isDemoMode; }
            set { _isDemoMode = value; }
        }

        public static bool IsLoggedIn 
        { 
            get { return UserId != -1; } 
        }

        public static bool IsAdmin 
        { 
            get { return Role == "Admin"; } 
        }

        public static bool IsManager 
        { 
            get { return Role == "Admin" || Role == "Manager"; } 
        }

        public static bool IsCashier 
        { 
            get { return Role == "Admin" || Role == "Manager" || Role == "Cashier"; } 
        }

        public static void Login(int userId, string username, string fullName, string role)
        {
            UserId = userId;
            Username = username;
            FullName = fullName;
            Role = role;
            CurrentShiftId = -1;
            IsDemoMode = false;
        }

        public static void LoginDemo()
        {
            UserId = 0;
            Username = "demo.admin";
            FullName = "Tài khoản Demo";
            Role = "Admin";
            CurrentShiftId = -1;
            IsDemoMode = true;
        }

        public static void Logout()
        {
            UserId = -1;
            Username = null;
            FullName = null;
            Role = null;
            CurrentShiftId = -1;
            IsDemoMode = false;
        }
    }
}
