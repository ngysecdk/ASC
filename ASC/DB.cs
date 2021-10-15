using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace ASC
{
    class DB
    {
        public DB()
        {
        retry:
            conn = new MySqlConnection((new Login()).GetLoginString());
            try { conn.Open(); }
            catch (Exception e)
            {
                if (MessageBox.Show(e.Message +
                    "\nПовторить попытку? (Не думаю. что это поможет)",
                    "Ошибка при подключении к базе данных",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.Yes,
                    MessageBoxOptions.DefaultDesktopOnly) == MessageBoxResult.No)
                    Application.Current.Shutdown(-1);
                else goto retry;
            }
            

           
        
        }  
        
        public MySqlConnection conn;
    }
}
