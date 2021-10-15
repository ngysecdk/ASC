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
        public void UpdateAnker(StandartObj anker) => new MySqlCommand(string.Format("UPDATE Анкер SET Тип_анкера='{0}', Примерная_цена={1} WHERE Код={2}", anker.Info, anker.Cost, anker.ID), conn).ExecuteNonQuery();
        public void GetAnkers(ref ObservableCollection<StandartObj> ankers)
        {
            MySqlDataReader reader = new MySqlCommand("SELECT Код, Тип_анкера, Примерная_цена FROM Анкер", conn).ExecuteReader();
            while (reader.Read()) ankers.Add(new StandartObj { ID = (uint)reader[0], Info = (string)reader[1], Cost = (int)reader[2] });
            reader.Close();
        }
        public uint AddAnker(StandartObj anker)
        {
            new MySqlCommand(string.Format("INSERT INTO Анкер (Тип_анкера, Примерная_цена) VALUES ('{0}', {1})",
                anker.Info,
                anker.Cost), conn).ExecuteNonQuery();
            MySqlDataReader reader = 
                new MySqlCommand(string.Format("SELECT Код FROM Анкер WHERE Тип_анкера='{0}' AND Примерная_цена={1}",
                anker.Info,
                anker.Cost), conn).ExecuteReader();
            reader.Read();
            anker.ID = (uint)reader[0];
            reader.Close();
            return anker.ID;
        }  
        public void Remove(StandartObj anker,string table) => new MySqlCommand(string.Format("DELETE FROM {0} WHERE Код={1}", anker.ID, table), conn).ExecuteNonQuery();
        
        public MySqlConnection conn;
    }
}
