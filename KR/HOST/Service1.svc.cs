using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KR
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    public class Service1 : IService1
    {
        SqlConnection db_Con = new SqlConnection("workstation id=PISKAREK.mssql.somee.com;packet size=4096;user id=secondAcy_SQLLogin_1;pwd=2o8gynzbz3;data source=PISKAREK.mssql.somee.com;persist security info=False;initial catalog=PISKAREK");
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }
        ///<param name="MinID">Минимальный ID записи</param>
        ///<param name="Table">Название таблицы</param>
        ///<param name="MaxID">Максимальный ID записи</param>
        ///<param name="Extend">Строки -> вхождение; Даты -> между</param>
        public List<List<string>> GetRecords(string Table,int MinID=0, int MaxID=int.MinValue, object[] Extend = null)
        {
            var query = new SqlCommand();
            var comandtext = "select ";
            var extended_params =new string[4];
            switch (Table)
            {
                case "Res":
                    comandtext += "Res.ID as [ID],Users.Name as [Клиент],TODO.Name as [Услуга], "+
                        $"TODO.Price as [Цена], CONVERT(VARCHAR(10), Res.Date, 111) as [Дата] from {Table} join "+
                        "Users on Users.ID=ID_user join TODO on TODO.ID=ID_todo ";
                    int ext_id = 0;
                    var was_string = false;
                    var was_date = false;
                    if(Extend!=null)
                    foreach (var rec in Extend)
                    {
                            if (rec is string)
                            {

                                extended_params[ext_id] += was_string ? $"and TODO.Name " : "and Users.Name";
                                extended_params[ext_id] += $" LIKE '%{(string)rec}%' ";
                                was_string = true;
                                ext_id++;
                            }
                            else if (rec is DateTime)
                            {
                                extended_params[ext_id] += was_date ? "and Res.Date <=" : "and Res.Date>=";
                                var date = ((DateTime)rec).ToShortDateString();
                                var a = date.Split(new[] { '.' });

                            extended_params[ext_id] += $"Cast('{a[1]}/{a[0]}/{a[2]}' as datetime) ";
                            was_date = true;
                            ext_id++;
                        }
                    }
                    break;
                default:
                    var d = false;
                    comandtext += $"* from {Table} ";
                    if(Extend!=null)
                    foreach (var rec in Extend)
                    {
                        if (rec is string)
                            extended_params[0] = $"and Name LIKE '%{(string)rec}%' ";
                            if ((rec is double || rec is float) && Table == "TODO")
                            {
                                if (!d)
                                {
                                    extended_params[1] = $"and Price>={((double)rec).ToString():0.00} ";
                                    d = true;
                                }
                                else
                                {
                                    extended_params[2] = $"and Price<={((double)rec).ToString():0.00} ";
                                }
                            }
                    }
                    break;
            }
            comandtext += $"where {Table}.ID Between {MinID} and {MaxID} ";
            foreach (var ex in extended_params)
                if (!string.IsNullOrEmpty(ex))
                    comandtext += ex;
            if (db_Con.State == System.Data.ConnectionState.Closed ||
                db_Con.State == System.Data.ConnectionState.Broken)
                db_Con.Open();
            query.CommandText = comandtext;
            query.Connection = db_Con;
            var reader = query.ExecuteReader();
            var result = new List<List<string>>();
            while (reader.Read())
            {
                var support_memory = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                    support_memory.Add(reader[i].ToString());
                result.Add(support_memory);
            }
            var cl = new List<string>();
            foreach (DataRow d in reader.GetSchemaTable().Rows)
                cl.Add(d.ItemArray[0].ToString());
            result.Insert(0,cl);
            reader.Close();
            return result;
        }
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public List<string> GetTables()
        {
            if (db_Con.State == System.Data.ConnectionState.Closed ||
                db_Con.State == System.Data.ConnectionState.Broken)
                db_Con.Open();
            var reader = new SqlCommand("SELECT * FROM information_schema.tables",db_Con).ExecuteReader();
            var res = new List<string>();
            while (reader.Read()) 
            {
                for(int i=2;i<reader.FieldCount;i+=4)
                    res.Add(reader[i].ToString());
            }
            reader.Close();
            return res;
        }
    }
}
