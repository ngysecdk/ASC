using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ASC
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DB db;
        ObservableCollection<StandartObj> ankers = new ObservableCollection<StandartObj>();
        objectCollection anker, buildType, dekaMaterial, grifMaterial, coloring, electric;

        public MainWindow()
        {
            db = new DB();
            anker = new objectCollection(db, "Тип_анкера", "Анкер");
            buildType = new objectCollection(db, "Сборка", "Вид_сборки", "Цена");
            dekaMaterial = new objectCollection(db, "Материал", "Материал_корпуса");
            grifMaterial = new objectCollection(db, "Материал", "Материал_грифа");
            coloring = new objectCollection(db, "Тип", "Покраска", "Цена");
            electric = new objectCollection(db, "Конфигурация", "Электронная_начинка");
            InitializeComponent();
            Ankers.ItemsSource = anker.collector;
            BuildType.ItemsSource = buildType.collector;
            DekaMaterial.ItemsSource = dekaMaterial.collector;
            GrifMaterial.ItemsSource = grifMaterial.collector;
            Coloring.ItemsSource = coloring.collector;
            Electronic.ItemsSource = electric.collector;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            anker.Update();
            buildType.Update();
            dekaMaterial.Update();
            grifMaterial.Update();
            coloring.Update();
            electric.Update();
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            anker.Update();
            buildType.Update();
            dekaMaterial.Update();
            grifMaterial.Update();
            coloring.Update();
            electric.Update();
        }
        private void Ankers_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
                new MySqlCommand(anker.Update((StandartObj)Ankers.SelectedItem), db.conn).ExecuteNonQuery();
        }
        private void BuildType_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
                new MySqlCommand(buildType.Update((StandartObj)BuildType.SelectedItem), db.conn).ExecuteNonQuery();
        }
        private void DekaMaterial_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
                new MySqlCommand(dekaMaterial.Update((StandartObj)DekaMaterial.SelectedItem), db.conn).ExecuteNonQuery();
        }

        private void Strings_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void GrifMaterial_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
                new MySqlCommand(grifMaterial.Update((StandartObj)GrifMaterial.SelectedItem), db.conn).ExecuteNonQuery();
        }

        private void Coloring_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
                new MySqlCommand(coloring.Update((StandartObj)Coloring.SelectedItem), db.conn).ExecuteNonQuery();
        }

        private void Electronic_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
                new MySqlCommand(electric.Update((StandartObj)Electronic.SelectedItem), db.conn).ExecuteNonQuery();
        }

        private void Kolk_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void SoundGetter_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Bridges_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

    }

    class objectCollection
    {
        string Info, Cost, Table;
        DB dB;
        public objectCollection(DB db, string info, string table, string cost = "Примерная_цена")
        {
            dB = db;
            Info = info;
            Table = table;
            Cost = cost;
            collector = new ObservableCollection<StandartObj>();
            Load();
            collector.CollectionChanged += CollectionChanged;
        }

        public void Load()
        {
            MySqlDataReader reader = new MySqlCommand(GetAll(), dB.conn).ExecuteReader();
            while (reader.Read()) collector.Add(new StandartObj { ID = (uint)reader[0], Info = (string)reader[1], Cost = (int)reader[2] });
            reader.Close();
        }
        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (StandartObj i in e.NewItems)
                    {
                        new MySqlCommand(Add(i), dB.conn).ExecuteNonQuery();
                        MySqlDataReader reader = new MySqlCommand(IdFromData(i), dB.conn).ExecuteReader();
                        reader.Read();
                        i.ID = (uint)reader[0];
                        reader.Close();
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (StandartObj i in e.OldItems) new MySqlCommand(Remove(i), dB.conn).ExecuteNonQuery();
                    break;
            }
        }
        public void Update()
        {
            foreach (StandartObj i in collector) new MySqlCommand(Update(i), dB.conn).ExecuteNonQuery();
        }
        string IdFromData(StandartObj obj) => 
            string.Format("SELECT Код FROM {0} WHERE {1}='{2}' AND {3}={4}", Table, Info, obj.Info, Cost, obj.Cost);
        string GetAll() => string.Format("SELECT Код, {0}, {1} FROM {2}", Info, Cost, Table);
        string Add(StandartObj obj) => string.Format("INSERT INTO {0} ({1}, {2}) VALUES ('{3}', {4})", Table, Info, Cost, obj.Info, obj.Cost);
        public string Update(StandartObj obj) => string.Format("UPDATE {3} SET {4}='{0}', {5}={1} WHERE Код={2}", obj.Info, obj.Cost, obj.ID, Table, Info, Cost);
        string Remove(StandartObj obj) => string.Format("DELETE FROM {0} WHERE Код={1}", obj.ID, Table);
        public ObservableCollection<StandartObj> collector;
    }

    /// <summary>
    /// Общее описание элементов таблицы
    /// </summary>
    public class StandartObj
    {
        public uint ID { get; set; }
        public string Info { get; set; }
        public int Cost { get; set; }
    }
}
