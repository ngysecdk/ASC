using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
namespace ASC
{
    class DB
    {
        public DB() => conn = new Login().GetLoginString();
        public class Table
        {
            public Table(MySqlDataAdapter da, string table, bool IsUpdate = true)
            {
                dataTable = new DataTable(table);
                (dA = da).FillAsync(dataTable);
                if (IsUpdate) {
                    dataTable.RowChanged += DataTable_RowChanged;
                    dataTable.RowDeleted += DataTable_RowChanged;
                }
            }
            private void DataTable_RowChanged(object sender, DataRowChangeEventArgs e) => dA.Update((DataTable)sender);
            MySqlDataAdapter dA;
            public DataTable dataTable;
        }
        public DataView GetTable(string Table, string Info = "", string Firm= "Фирма", string Model = "Модель", string Cost = "Примерная_цена")
        {
            MySqlCommand cmd = new MySqlCommand($"SELECT * FROM {Table}", conn);
            cmd.ExecuteNonQuery();
            MySqlDataAdapter dA = new MySqlDataAdapter(cmd);
            if (Info != "") {
                dA.InsertCommand = new MySqlCommand($"INSERT INTO {Table} ({Info}, {Cost}) VALUES (@{Info}, @{Cost})", conn);
                dA.InsertCommand.Parameters.Add($"@{Info}", MySqlDbType.Text, 65535, Info);
                dA.UpdateCommand = new MySqlCommand($"UPDATE {Table} SET {Info}=@{Info}, {Cost}=@{Cost} WHERE Код=@Код", conn);
                dA.UpdateCommand.Parameters.Add($"@{Info}", MySqlDbType.Text, 65535, Info);
            } else
            {
                dA.InsertCommand = new MySqlCommand($"INSERT INTO {Table} ({Firm}, {Model}, {Cost}) VALUES (@{Firm}, @{Model}, @{Cost})", conn);
                dA.InsertCommand.Parameters.Add($"@{Firm}", MySqlDbType.Text, 65535, Firm);
                dA.InsertCommand.Parameters.Add($"@{Model}", MySqlDbType.Text, 65535, Model);
                dA.UpdateCommand = new MySqlCommand($"UPDATE {Table} SET {Firm}=@{Firm}, {Model}=@{Model}, {Cost}=@{Cost} WHERE Код=@Код", conn);
                dA.UpdateCommand.Parameters.Add($"@{Firm}", MySqlDbType.Text, 65535, Firm);
                dA.UpdateCommand.Parameters.Add($"@{Model}", MySqlDbType.Text, 65535, Model);
            }
            dA.DeleteCommand = new MySqlCommand($"DELETE FROM {Table} WHERE Код = @Код", conn);
            dA.UpdateCommand.Parameters.Add($"@{Cost}", (Cost == "Должность") ? MySqlDbType.Text : MySqlDbType.Int32, 15, Cost);
            dA.InsertCommand.Parameters.Add($"@{Cost}", (Cost == "Должность") ? MySqlDbType.Text : MySqlDbType.Int32, 15, Cost);
            dA.UpdateCommand.Parameters.Add("@Код", MySqlDbType.Int32, 15, "Код");
            dA.DeleteCommand.Parameters.Add("@Код", MySqlDbType.Int32, 15, "Код");
            Table table = new Table(dA, Table);
            Tables.Add(table);
            return table.dataTable.DefaultView;
        }
        public List<Table> Tables = new List<Table>();
        public void Req(string req) => new MySqlCommand(req, conn).ExecuteNonQuery();
        public MySqlConnection conn;
    }
}
