using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BingLibrary.hjb.permission
{

    public class UserData
    {
        public string UserName { set; get; }
        public string Password { set; get; }
        public bool IsLogin { set; get; } = false;
        public PermissionLevel Level { set; get; } = PermissionLevel.Operator;
    }
    

    public enum PermissionLevel
    {
        Operator = 0,
        Engeneer = 1,
        Administrator = 2,
        SuperAdministrator = 9,
    }

    

    public class Permission
    {
        public static int Level = 0;
        public static UserData CurrentUser = new UserData();

        public static List<string> GetUserNames()
        {
            List<string> usernames = new List<string>();
            foreach (var r in AllUsers)
            {
                usernames.Add(r.UserName);
            }
            return usernames;
        }
        

        public static void AddUser(UserData user)
        {
            AllUsers.Add(user);
        }


        public static UserData GetUserData(string name)
        {
            UserData ud = new UserData();
            foreach (var r in AllUsers)
            {
                if (r.UserName == name)
                {
                    ud = r;
                    break;
                }
            }

            return ud;
        }


        public static void Login(string name,string password)
        {
            foreach (var r in AllUsers)
            {
                if (r.UserName == name)
                {
                    if (r.Password == password)
                    {
                        CurrentUser = r;
                        CurrentUser.IsLogin = true;
                        break;
                    }
                }
            }
        }

        public static void Logout()
        {
            CurrentUser.IsLogin = false;
        }


        public static int GetCurrentLevel()
        {
            if (CurrentUser.IsLogin == false)
                return 0;
            else
                return (int)CurrentUser.Level;
        }

        public static List<UserData> AllUsers = new List<UserData>()
        {
            new UserData() { UserName="Bing",Password="Bing",Level=PermissionLevel.SuperAdministrator },
        };


       
    }

    public class PermitLevelConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int v = int.Parse(parameter.ToString());
            bool pl = false;
            if ((int)value >= v)
                pl = true;
            return pl;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "Administrator";
        }
    }

}
