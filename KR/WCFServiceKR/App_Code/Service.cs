using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

// ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Реструктуризация" можно использовать для одновременного изменения имени класса "Service" в коде, SVC-файле и файле конфигурации.
public class Service : IService
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
    public List<List<string>> GetRecords(string Table, int MinID = 0, int MaxID = int.MinValue, params object[] Extend)
    {
        var query = new SqlCommand();
        var comandtext = "select ";
        var extended_params = new string[4];
        switch (Table)
        {
            case "Res":
                comandtext += "Res.ID as [ID],Users.Name as [Клиент],TODO.Name as [Услуга], " +
                    $"TODO.Price as [Цена], Res.Date as [Дата] from {Table} join " +
                    "Users on Users.ID=ID_user join TODO on TODO.ID=ID_todo ";
                int ext_id = 0;
                var was_string = false;
                var was_date = false;
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
                        extended_params[ext_id] += was_date ? "and Rec.Date >=" : "and Rec.Date<=";
                        extended_params[ext_id] += $"'{ ((DateTime)rec).ToShortDateString()}' ";
                        was_date = true;
                        ext_id++;
                    }
                }
                break;
            default:
                comandtext += $"* from {Table} ";
                foreach (var rec in Extend)
                {
                    if (rec is string)
                        extended_params[0] = $"and Name LIKE '%{(string)rec}%' ";
                    if ((rec is double || rec is float) && Table == "TODO")
                        extended_params[1] = $"and Price<={((double)rec).ToString():0.00} ";
                }
                break;
        }
        comandtext += $"where Res.ID is Betwenn {MinID} and {MaxID} ";
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
        return result;
    }
    public string GetData(int value)
	{
		return string.Format("You entered: {0}", value);
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
}
