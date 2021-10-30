using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System;
namespace ASC
{
    public partial class MainWindow : Window
    {
        private DB db;
        public MainWindow()
        {
            db = new DB();
            InitializeComponent();
            Ankers.ItemsSource = db.GetTable("Анкер", "Тип_анкера");
            BuildType.ItemsSource = db.GetTable("Вид_сборки", "Сборка", Cost: "Цена");
            DekaMaterial.ItemsSource = db.GetTable("Материал_корпуса", "Материал");
            GrifMaterial.ItemsSource = db.GetTable("Материал_грифа", "Материал");
            Coloring.ItemsSource = db.GetTable("Покраска", "Тип", Cost: "Цена");
            Electronic.ItemsSource = db.GetTable("Электронная_начинка", "Конфигурация");
            Bridges.ItemsSource = db.GetTable("Бридж");
            Strings.ItemsSource = db.GetTable("Струны");
            Kolk.ItemsSource = db.GetTable("Колки");
            SoundGetter.ItemsSource = db.GetTable("Звукосниматель");
            Staff.ItemsSource = db.GetTable("Сотрудники", "ФИО", Cost: "Должность");
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Заказ", db.conn);
            cmd.ExecuteNonQuery();
            DB.Table Table = new DB.Table(new MySqlDataAdapter(cmd), "Заказы", false);
            Orders.ItemsSource = Table.dataTable.DefaultView;
        }
        private void InitializingNewItem(object sender, InitializingNewItemEventArgs e)=> ((DataRowView)e.NewItem)["Код"] = 0;
        private void AccessOrder_Click(object sender, RoutedEventArgs e)
        {
            try { db.Req($"UPDATE Заказ SET Подтверждение_оплаты=TRUE WHERE Код={OrderId.Text}"); }
            catch(Exception ex) {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            (Orders.SelectedItem as DataRowView).Row["Подтверждение_оплаты"] = true;
            MessageBox.Show("Успех", "Выполнено!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void Orders_SelectionChanged_1(object sender, SelectionChangedEventArgs e)=>
            OrderId.Text = (Orders.SelectedItem as DataRowView).Row["Код"].ToString();
    }
}
