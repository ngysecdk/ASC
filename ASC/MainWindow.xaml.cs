using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Collections.Specialized;
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
        object4Collection bridge, strings, kolk, soundGetter;
        public MainWindow()
        {
            db = new DB();
            anker = new objectCollection(db, "Тип_анкера", "Анкер");
            buildType = new objectCollection(db, "Сборка", "Вид_сборки", "Цена");
            dekaMaterial = new objectCollection(db, "Материал", "Материал_корпуса");
            grifMaterial = new objectCollection(db, "Материал", "Материал_грифа");
            coloring = new objectCollection(db, "Тип", "Покраска", "Цена");
            electric = new objectCollection(db, "Конфигурация", "Электронная_начинка");
            bridge = new object4Collection(db, "Бридж");
            strings = new object4Collection(db, "Струны");
            kolk = new object4Collection(db, "Колки");
            soundGetter = new object4Collection(db, "Звукосниматель");
            InitializeComponent();
            Ankers.ItemsSource = anker.collector;
            BuildType.ItemsSource = buildType.collector;
            DekaMaterial.ItemsSource = dekaMaterial.collector;
            GrifMaterial.ItemsSource = grifMaterial.collector;
            Coloring.ItemsSource = coloring.collector;
            Electronic.ItemsSource = electric.collector;
            Bridges.ItemsSource = bridge.collector;
            Strings.ItemsSource = strings.collector;
            Kolk.ItemsSource = kolk.collector;
            SoundGetter.ItemsSource = soundGetter.collector;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => Update();
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Update();
            MessageBox.Show("Выполнено!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information,
                    MessageBoxResult.Yes,
                    MessageBoxOptions.DefaultDesktopOnly);
        }
        void Update()
        {
            anker.Update();
            buildType.Update();
            dekaMaterial.Update();
            grifMaterial.Update();
            coloring.Update();
            electric.Update();
            bridge.Update();
            strings.Update();
            kolk.Update();
            soundGetter.Update();
        }
        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            db.Req(string.Format("INSERT INTO Сотрудники (ФИО, Должность) VALUES ('{0}', '{1}')", AddFio, AddPost));
            MessageBox.Show("Выполнено!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information,
                    MessageBoxResult.Yes,
                    MessageBoxOptions.DefaultDesktopOnly);
        }
        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            db.Req(string.Format("DELETE FROM Сотрудники WHERE Код={0}", uint.Parse(DeleteID.Text)));
            MessageBox.Show("Выполнено!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information,
                    MessageBoxResult.Yes,
                    MessageBoxOptions.DefaultDesktopOnly);
        }
        private void AccessOrder_Click(object sender, RoutedEventArgs e)
        {
            db.Req(string.Format("UPDATE Заказ SET Подтвержден=TRUE WHERE Код={0}", OrderId));
            MessageBox.Show("Выполнено!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information,
                    MessageBoxResult.Yes,
                    MessageBoxOptions.DefaultDesktopOnly);
        }
        #region keyEnter
        private void Ankers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab) db.Req(anker.Update((StandartObj)Ankers.SelectedItem));
        }
        private void BuildType_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab) db.Req(buildType.Update((StandartObj)BuildType.SelectedItem));
        }
        private void DekaMaterial_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab) db.Req(dekaMaterial.Update((StandartObj)DekaMaterial.SelectedItem));
        }
        private void Strings_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab) db.Req(strings.Update((StandartObj4)Strings.SelectedItem));
        }
        private void GrifMaterial_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab) db.Req(grifMaterial.Update((StandartObj)GrifMaterial.SelectedItem));
        }
        private void Coloring_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab) db.Req(coloring.Update((StandartObj)Coloring.SelectedItem));
        }
        private void Electronic_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab) db.Req(electric.Update((StandartObj)Electronic.SelectedItem));
        }
        private void Kolk_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab) db.Req(kolk.Update((StandartObj4)Kolk.SelectedItem));
        }
        private void SoundGetter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab) db.Req(soundGetter.Update((StandartObj4)SoundGetter.SelectedItem));
        }
        private void Bridges_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab) db.Req(bridge.Update((StandartObj4)Bridges.SelectedItem));
        }
        #endregion
    }
    public class StandartObj4
    {
        public uint ID { get; set; }
        public string Firm { get; set; }
        public string Model { get; set; }
        public int Cost { get; set; }
    }

    class object4Collection
    {
        string Cost = "Примерная_цена", Firm="Фирма", Model="Модель", Table;
        DB dB;
        public object4Collection(DB db, string table)
        {
            Table = table;
            dB = db;
            collector = new ObservableCollection<StandartObj4>();
            Load();
            collector.CollectionChanged += CollectionChanged;
        }

        public void Load()
        {
            MySqlDataReader reader = new MySqlCommand(GetAll(), dB.conn).ExecuteReader();
            while (reader.Read()) collector.Add(new StandartObj4 { ID = (uint)reader[0], Firm = (string)reader[1], Model = (string)reader[2], Cost = (int)reader[3] });
            reader.Close();
        }
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (StandartObj4 i in e.NewItems)
                    {
                        dB.Req(Add(i));
                        MySqlDataReader reader = new MySqlCommand(IdFromData(i), dB.conn).ExecuteReader();
                        reader.Read();
                        i.ID = (uint)reader[0];
                        reader.Close();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove: foreach (StandartObj4 i in e.OldItems) dB.Req(Remove(i)); break;
            }
        }
        public void Update() { foreach (StandartObj4 i in collector) dB.Req(Update(i)); }
        string IdFromData(StandartObj4 obj) => string.Format("SELECT Код FROM {0} WHERE {1}='{2}' AND {3}='{4}' AND {5}={6}", Table, Firm, obj.Firm, Model, obj.Model, Cost, obj.Cost);
        string GetAll() => string.Format("SELECT Код, {0}, {1}, {2} FROM {3}", Firm, Model, Cost, Table);
        string Add(StandartObj4 obj) => string.Format("INSERT INTO {0} ({1}, {2}, {3}) VALUES ('{4}', '{5}', {6})", Table, Firm, Model, Cost, obj.Firm, obj.Model, obj.Cost);
        public string Update(StandartObj4 obj) => string.Format("UPDATE {0} SET {1}='{2}', {3}='{4}', {5}={6} WHERE Код={7}", Table, Firm, obj.Firm, Model, obj.Model, Cost, obj.Cost, obj.ID);
        string Remove(StandartObj4 obj) => string.Format("DELETE FROM {0} WHERE Код={1}", Table, obj.ID);
        public ObservableCollection<StandartObj4> collector;
    }
    /// <summary>
    /// Коллекция для обработки таблиц с 3 столбцами
    /// </summary>
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
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (StandartObj i in e.NewItems)
                    {
                        dB.Req(Add(i));
                        MySqlDataReader reader = new MySqlCommand(IdFromData(i), dB.conn).ExecuteReader();
                        reader.Read();
                        i.ID = (uint)reader[0];
                        reader.Close();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove: foreach (StandartObj i in e.OldItems) dB.Req(Remove(i)); break;
            }
        }
        public void Update() { foreach (StandartObj i in collector) dB.Req(Update(i)); }
        string IdFromData(StandartObj obj) => string.Format("SELECT Код FROM {0} WHERE {1}='{2}' AND {3}={4}", Table, Info, obj.Info, Cost, obj.Cost);
        string GetAll() => string.Format("SELECT Код, {0}, {1} FROM {2}", Info, Cost, Table);
        string Add(StandartObj obj) => string.Format("INSERT INTO {0} ({1}, {2}) VALUES ('{3}', {4})", Table, Info, Cost, obj.Info, obj.Cost);
        public string Update(StandartObj obj) => string.Format("UPDATE {3} SET {4}='{0}', {5}={1} WHERE Код={2}", obj.Info, obj.Cost, obj.ID, Table, Info, Cost);
        string Remove(StandartObj obj) => string.Format("DELETE FROM {0} WHERE Код={1}", Table, obj.ID);
        public ObservableCollection<StandartObj> collector;
    }

    /// <summary>
    /// Общее описание элементов таблицы с 3мя основными полями
    /// </summary>
    public class StandartObj
    {
        public uint ID { get; set; }
        public string Info { get; set; }
        public int Cost { get; set; }
    }
}
