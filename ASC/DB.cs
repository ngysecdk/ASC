using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace ASC
{
    class DB
    {
        public DB()
        {
           conn = new Login().GetLoginString();
        }
        class Table
        {
            public Table(MySqlDataAdapter da, string table)
            {
                dA = da;
                dataTable = new DataTable(table);
                dA.FillAsync(dataTable);
                dataTable.RowChanged += DataTable_RowChanged;
                dataTable.RowDeleted += DataTable_RowChanged;
            }
            private void DataTable_RowChanged(object sender, DataRowChangeEventArgs e) => dA.Update((DataTable)sender);
            MySqlDataAdapter dA;
            public DataTable dataTable;
        }
        List<Table> Tables = new List<Table>();
        public DataView GetTable(string Table, string Info = "", string Firm= "Фирма", string Model = "Модель", string Cost = "Примерная_цена")
        {
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM " + Table, conn);
            cmd.ExecuteNonQuery();
            MySqlDataAdapter dA = new MySqlDataAdapter(cmd);
            if (Info != "") {
                dA.InsertCommand = new MySqlCommand(string.Format("INSERT INTO {0} ({1}, {2}) VALUES (@{1}, @{2})", Table, Info, Cost), conn);
                dA.InsertCommand.Parameters.Add("@" + Info, MySqlDbType.Text, 65535, Info);
                dA.UpdateCommand = new MySqlCommand(string.Format("UPDATE {0} SET {1}=@{1}, {2}=@{2} WHERE Код=@Код", Table, Info, Cost), conn);
                dA.UpdateCommand.Parameters.Add("@" + Info, MySqlDbType.Text, 65535, Info);
            }
            else {
                dA.InsertCommand = new MySqlCommand(string.Format("INSERT INTO {0} ({1}, {2}, {3}) VALUES (@{1}, @{2}, @{3})", Table, Firm, Model, Cost), conn);
                dA.InsertCommand.Parameters.Add("@" + Firm, MySqlDbType.Text, 65535, Firm);
                dA.InsertCommand.Parameters.Add("@" + Model, MySqlDbType.Text, 65535, Model);
                dA.UpdateCommand = new MySqlCommand(string.Format("UPDATE {0} SET {1}=@{1}, {2}=@{2} , {3}=@{3} WHERE Код=@Код", Table, Firm, Model, Cost), conn);
                dA.UpdateCommand.Parameters.Add("@" + Firm, MySqlDbType.Text, 65535, Firm);
                dA.UpdateCommand.Parameters.Add("@" + Model, MySqlDbType.Text, 65535, Model);
            }
            dA.DeleteCommand = new MySqlCommand(string.Format("DELETE FROM {0} WHERE Код = @Код", Table), conn);
            dA.UpdateCommand.Parameters.Add("@" + Cost, (Cost == "Должность") ? MySqlDbType.Text : MySqlDbType.Int32, 15, Cost);
            dA.InsertCommand.Parameters.Add("@" + Cost, (Cost == "Должность") ? MySqlDbType.Text : MySqlDbType.Int32, 15, Cost);
            dA.UpdateCommand.Parameters.Add("@Код", MySqlDbType.Int32, 15, "Код");
            dA.DeleteCommand.Parameters.Add("@Код", MySqlDbType.Int32, 15, "Код");
            Table table = new Table(dA, Table);
            Tables.Add(table);
            return table.dataTable.DefaultView;
        }
        public void Req(string req) => new MySqlCommand(req, conn).ExecuteNonQuery();
        public MySqlConnection conn;
    }
}
