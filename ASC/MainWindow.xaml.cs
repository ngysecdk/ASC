using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System;

namespace ASC
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DB db;
        public MainWindow()
        {
            db = new DB();
            InitializeComponent();
            Ankers.ItemsSource = db.GetTable("Анкер","Тип_анкера");
            BuildType.ItemsSource = db.GetTable("Вид_сборки", "Сборка","","", "Цена");
            DekaMaterial.ItemsSource = db.GetTable("Материал_корпуса", "Материал");
            GrifMaterial.ItemsSource = db.GetTable("Материал_грифа", "Материал");
            Coloring.ItemsSource = db.GetTable("Покраска", "Тип", "", "", "Цена");
            Electronic.ItemsSource = db.GetTable("Электронная_начинка", "Конфигурация");
            Bridges.ItemsSource = db.GetTable("Бридж", "");
            Strings.ItemsSource = db.GetTable("Струны", "");
            Kolk.ItemsSource = db.GetTable("Колки", "");
            SoundGetter.ItemsSource = db.GetTable("Звукосниматель", "");
            Staff.ItemsSource = db.GetTable("Сотрудники", "ФИО", "", "", "Должность");
        }
        private void InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            DataView dataView = ((DataGrid)sender).ItemsSource as DataView;
            MySqlDataReader reader = new MySqlCommand("SELECT MAX(КОД) FROM " + dataView.Table.TableName, db.conn).ExecuteReader();
            reader.Read();
            if (reader.IsDBNull(0)) ((DataRowView)e.NewItem)["Код"] = 1;
            else ((DataRowView)e.NewItem)["Код"] = (uint)reader[0] + 1;
            reader.Close();
        }
        private void AccessOrder_Click(object sender, RoutedEventArgs e)
        {
            try { db.Req(string.Format("UPDATE Заказ SET Подтвержден=TRUE WHERE Код={0}", OrderId)); }
            catch(Exception ex) {
                MessageBox.Show("Ошибка!", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("Выполнено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
