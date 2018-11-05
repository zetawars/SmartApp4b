using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
namespace System
{
    public class DefaultQuery : Attribute
    {
        public string Query { get; set; }
        public DefaultQuery(string Query)
        {
            this.Query = Query;
        }
    }

    public class PageListObject<T>
    {
        public Pager pager { get; set; }
        public List<T> Results { get; set; }
    }

    public class PagedQuery
    {
        public string Query { get; set; }
        public string CountQuery { get; set; }
    }

    public class DBHelper
    {
        public string Query { get; set; }
        public Dictionary<string, string> values { get; set; }
        public static string ConnectionString = Configuration.ConfigurationManager.ConnectionStrings["4BDATAConnectionString"].ToString();
        public DBHelper(bool debug = true)
        {
            this.Query = string.Empty;
            this.values = new Dictionary<string, string>();
        }


        private static string GetColumnName(PropertyInfo pi)
        {
            if (Attribute.IsDefined(pi, typeof(Column)))
            {
                Column k = (Column)Attribute.GetCustomAttribute(pi, typeof(Column));
                return $"{k.Name}";
            }
            else
            {
                return $"{pi.Name}";
            }
        }



        #region InsertFunctions
        public static bool Insert<T>(T _Object, string schemaName = null, string tableName = null)
        {
            return ExecuteQuery(QueryMaker.InsertQuery(_Object, schemaName, tableName));
        }
        public static bool Insert(string tableName, Dictionary<string, string> dataset)
        {
            return ExecuteQuery(QueryMaker.InsertQuery(tableName, dataset));
        }
        public string InsertAndGetId(string tableName, Dictionary<string, string> dataset)
        {
            string query = QueryMaker.InsertQueryWithID(tableName, dataset);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                int id = 0;
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                id = Convert.ToInt32(cmd.ExecuteScalar());
                connection.Close();
                return id.ToString();
            }
        }
        public static int GetScaler(string query, Dictionary<string, string> Params = null)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                SqlCommand cmd = GetSqlCommandWithParams(query, Connection, Params);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                Connection.Close();
                return count;
            }
        }
        #endregion

        #region UpdateFunctions
        public static bool Update<T>(T _Object, string whereClause, Dictionary<string, string> values, string schemaName = null, string tableName = null)
        {
            return ExecuteQuery(QueryMaker.UpdateQuery(_Object, whereClause, schemaName, tableName), values);
        }
        public static bool Update(string tableName, Dictionary<string, string> dataset, string whereClause)
        {
            return ExecuteQuery(QueryMaker.UpdateQuery(tableName, dataset, whereClause));
        }


        #endregion

        #region DeleteFunctions
        public static bool Delete<T>(string whereClause, Dictionary<string, string> values, string schemaName = null, string tableName = null)
        {
            return ExecuteQuery(QueryMaker.DeleteQuery<T>(whereClause), values);
        }

        #endregion

        #region OtherFunctions
        public void BeginTrans()
        {
            Query = string.Empty;
            Query += QueryMaker.BeginTransQuery();
        }


        public bool CommitTrans()
        {
            Query += QueryMaker.CommitTransQuery();
            bool result = ExecuteQuery(Query);
            Query = string.Empty;
            return result;
        }
        public bool CommitTrans(Dictionary<string, string> Params)
        {
            Query += QueryMaker.CommitTransQuery();
            bool result = ExecuteQuery(Query, Params);
            Query = string.Empty;
            return result;
        }

        public static bool ExecuteQuery(string query, Dictionary<string, string> Params = null)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                SqlCommand cmd = GetSqlCommandWithParams(query, Connection, Params);
                int rowseffected = cmd.ExecuteNonQuery();
                Connection.Close();
                return true;
            }
        }


        public static bool ExecuteTransactionQuery(string query, Dictionary<string, string> Params = null)
        {
            query = $"{QueryMaker.BeginTransQuery()} {query} {QueryMaker.CommitTransQuery()}";
            using (SqlConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                SqlCommand cmd = GetSqlCommandWithParams(query, Connection, Params);
                int rowseffected = cmd.ExecuteNonQuery();
                Connection.Close();
                return true;
            }
        }

        #endregion

        #region ReadFunctions
        public static List<Dictionary<string, string>> QueryList(string query, Dictionary<string, string> Params = null)
        {
            List<Dictionary<string, string>> appList = new List<Dictionary<string, string>>();
            using (SqlConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                SqlCommand cmd = GetSqlCommandWithParams(query, Connection, Params);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Dictionary<string, string> app = new Dictionary<string, string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            app.Add(reader.GetName(i), reader[reader.GetName(i)].ToString());
                        }
                        appList.Add(app);
                    }
                }
                Connection.Close();
            }
            return appList;
        }

        public static PageListObject<T> GetPagedList<T>(string query, string sortBy, string order, int numberOfRecords, int pageNumber, Dictionary<string, string> Params)
        {
            int offset = numberOfRecords * pageNumber;
            var Querries = QueryMaker.GetPagerQueries<T>(query, sortBy, order, offset, numberOfRecords);
            string mainquery = Querries.Query;
            string countquery = Querries.CountQuery;
            return PagedResults<T>(numberOfRecords, pageNumber, Params, offset, mainquery, countquery);
        }
        public static PageListObject<T> GetPagedList<T>(string query, Dictionary<string, string> sortAndOrder, int numberOfRecords, int pageNumber, Dictionary<string, string> Params)
        {
            int offset = numberOfRecords * pageNumber;
            var Querries = QueryMaker.GetPagerQueries<T>(query, sortAndOrder, offset, numberOfRecords);
            string mainquery = Querries.Query;
            string countquery = Querries.CountQuery;
            return PagedResults<T>(numberOfRecords, pageNumber, Params, offset, mainquery, countquery);
        }
        private static PageListObject<T> PagedResults<T>(int numberOfRecords, int pageNumber, Dictionary<string, string> Params, int offset, string mainquery, string countquery)
        {

            int count = 0;
            List<T> results = new List<T>();
            using (SqlConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                SqlCommand cmd = GetSqlCommandWithParams(mainquery, Connection, Params);
                SqlDataReader reader = cmd.ExecuteReader();
                var properties = GetReadableProperties<T>();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var item = Activator.CreateInstance<T>();
                        foreach (var property in properties)
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                            {
                                DBReader(item, property, reader);
                            }
                        }
                        results.Add(item);
                    }
                }
                reader.Close();
                cmd = GetSqlCommandWithParams(countquery, Connection, Params);
                count = (int)cmd.ExecuteScalar();
                Connection.Close();
                Pager pager = GetPagerSettings(numberOfRecords, pageNumber, count, offset);
                return new PageListObject<T> { pager = pager, Results = results };
            }
        }



        //public (Pager pager, List<Dictionary<string,string>> Results) PagedQuery(string query, string sortBy, string order, int numberOfRecords, int pageNumber)
        //{
        //    int offset = numberOfRecords * pageNumber;
        //    var Querries = QueryMaker.GetPagerQueries(query, sortBy, order, offset, numberOfRecords);
        //    string mainquery = Querries.MainQuery;
        //    string countquery = Querries.CountQuery;
        //    int count = 0;
        //    List<Dictionary<string, string>> appList = new List<Dictionary<string, string>>();
        //    using (SqlConnection Connection = new SqlConnection(ConnectionString))
        //    {
        //        Connection.Open();
        //        SqlCommand cmd = new SqlCommand(mainquery, Connection);
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        if (reader.HasRows)
        //        {
        //            while (reader.Read())
        //            {
        //                Dictionary<string, string> app = new Dictionary<string, string>();
        //                for (int i = 0; i < reader.FieldCount; i++)
        //                {
        //                    app.Add(reader.GetName(i), reader[reader.GetName(i)].ToString());
        //                }
        //                appList.Add(app);
        //            }
        //        }
        //        reader.Close();
        //        cmd = new SqlCommand(countquery, Connection);
        //        count = (int)cmd.ExecuteScalar();
        //        Connection.Close();
        //        Pager pager = GetPagerSettings(numberOfRecords, pageNumber, count, offset);
        //        return (pager, appList);
        //    }
        //}
        private static Pager GetPagerSettings(int numberOfRecords, int pageNumber, int count, int offset)
        {
            Pager pager = new Pager();
            pager.RecordsPerPage = numberOfRecords;
            pager.Offset = offset;
            pager.Fetch = numberOfRecords;
            pager.TotalRecords = count;
            pager.PageNumber = pageNumber;
            double d = pager.TotalRecords / pager.RecordsPerPage;
            pager.TotalPages = Convert.ToInt32(Math.Ceiling(d));
            return pager;
        }
        public static Dictionary<string, string> QueryRow(string query, Dictionary<string, string> Params = null)
        {
            Dictionary<string, string> appList = new Dictionary<string, string>();
            using (SqlConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                SqlCommand cmd = GetSqlCommandWithParams(query, Connection, Params);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            appList.Add(reader.GetName(i), reader[reader.GetName(i)].ToString());
                        }
                        break;
                    }
                }
                Connection.Close();
            }
            return appList;
        }
        public static List<string> QueryColumn(string query, Dictionary<string, string> Params)
        {
            List<string> list = new List<string>();
            using (SqlConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                SqlCommand cmd = GetSqlCommandWithParams(query, Connection, Params);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(reader[reader.GetName(0)].ToString());
                    }
                }
                Connection.Close();
            }
            return list;
        }
        public static T Get<T>(string query = null, string whereClause = null, Dictionary<string, string> Params = null)
        {
            T t = Activator.CreateInstance<T>();
            List<PropertyInfo> properties = GetReadableProperties<T>();
            query = QueryMaker.SelectQuery<T>(query, whereClause);
            using (SqlConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                SqlCommand cmd = GetSqlCommandWithParams(query, Connection, Params);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        foreach (var property in properties)
                        {
                            DBReader(t, property, reader);
                        }
                    }
                }
                Connection.Close();
            }
            return t;
        }
        public static bool Get<T>(T _Object, string query = "", string where_clause = "", Dictionary<string, string> Params = null)
        {
            List<PropertyInfo> _properties = GetReadableProperties<T>();
            query = QueryMaker.SelectQuery<T>(query, where_clause);

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand cmd = GetSqlCommandWithParams(query, connection, Params);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        foreach (PropertyInfo pi in _properties)
                        {
                            DBReader(_Object, pi, reader);
                        }
                    }
                }
                connection.Close();
                return true;
            }
        }

        public static List<T> GetList<T>(string query = null, string whereClause = null, Dictionary<string, string> Params = null)
        {
            var results = new List<T>();
            var properties = GetReadableProperties<T>();
            query = QueryMaker.SelectQuery<T>(query, whereClause);
            using (SqlConnection Connection = new SqlConnection(ConnectionString))
            {
                Connection.Open();
                SqlCommand cmd = GetSqlCommandWithParams(query, Connection, Params);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var item = Activator.CreateInstance<T>();
                        foreach (var property in properties)
                        {
                            DBReader(item, property, reader);
                        }
                        results.Add(item);
                    }
                }
                Connection.Close();
                return results;
            }
        }
        public static DataTable ReadDataTable(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection Connection = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter sda = new SqlDataAdapter();
                Connection.Open();
                SqlCommand cmd = new SqlCommand(query, Connection);
                sda.SelectCommand = cmd;
                sda.Fill(dt);
                return dt;
            }

        }
        #endregion

        #region Private methods   
        private static SqlCommand GetSqlCommandWithParams(string query, SqlConnection Connection, Dictionary<string, string> Params)
        {
            SqlCommand cmd = new SqlCommand(query, Connection);
            foreach (var element in Params ?? new Dictionary<string, string>())
            {
                cmd.Parameters.AddWithValue(element.Key, element.Value);
            }
            return cmd;
        }
        private static void DBReader(Object _Object, PropertyInfo property, SqlDataReader reader)

        {
            if (!reader.IsDBNull(reader.GetOrdinal(GetColumnName(property))))
            {
                Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                property.SetValue(_Object, Convert.ChangeType(reader[GetColumnName(property)], convertTo), null);
            }
            else
            {
                property.SetValue(_Object, null, null);
            }
        }
        public static List<PropertyInfo> GetReadableProperties<T>()
        {
            return typeof(T).GetProperties().Where(x => !(Attribute.IsDefined(x, typeof(DontLoad)))).ToList();
        }
        #endregion Private methods
    }
    public class QueryMaker
    {
        #region Public methods
        public static string InsertQueryWithID(string tableName, Dictionary<string, string> dataset)
        {
            return $"{InsertQuery(tableName, dataset)};Select Scope_Identity()";
        }
        public static string SelectQuery<T>(string query = null, string where_clause = null)
        {
            StringBuilder queryBuilder = new StringBuilder();
            if (string.IsNullOrWhiteSpace(query))
            {
                List<PropertyInfo> _properties = GetReadableProperties<T>();
                queryBuilder.Append("SELECT ");
                foreach (var element in _properties)
                {
                    queryBuilder.Append($"{GetColumnName(element)}, ");
                }
                queryBuilder.Length -= 2;
                queryBuilder.Append($"FROM {GetSchemaName<T>()}.{GetTableName<T>()} { where_clause }");
            }
            else
            {
                queryBuilder.Append(query);
                queryBuilder.Append($" { where_clause} ");
            }
            return queryBuilder.ToString();
        }
        public static string InsertQuery<T>(T _Object, string schemaName = null, string tableName = null)
        {
            List<PropertyInfo> properties = GetInsertProperties<T>();
            StringBuilder ColumnQueryBuilder = new StringBuilder();
            StringBuilder ValuesQueryBuilder = new StringBuilder();
            ColumnQueryBuilder.Append($"INSERT INTO {schemaName ?? GetSchemaName<T>()}.{tableName ?? GetTableName<T>()}");
            ColumnQueryBuilder.Append("(");
            ValuesQueryBuilder.Append(" VALUES (");
            foreach (var pi in properties)
            {
                ColumnQueryBuilder.Append($"{GetColumnName(pi)} , ");
                ValuesQueryBuilder.Append($"{ValueReader(pi.GetValue(_Object, null))}, ");
            }
            ColumnQueryBuilder.Length -= 2;
            ColumnQueryBuilder.Append(")");
            ValuesQueryBuilder.Length -= 2;
            ValuesQueryBuilder.Append(")");
            ColumnQueryBuilder.Append(ValuesQueryBuilder.ToString());
            return ColumnQueryBuilder.ToString();
        }
        public static string InsertQuery(string table, Dictionary<string, string> dataset)
        {
            StringBuilder InsertQueryBuilder = new StringBuilder();
            StringBuilder ColumnQueryBuilder = new StringBuilder();
            StringBuilder ValuesQueryBuilder = new StringBuilder();
            InsertQueryBuilder.Append($"Insert into [dbo].[{table}] ");
            ColumnQueryBuilder.Append("(");
            ValuesQueryBuilder.Append("VALUES (");
            foreach (var col in dataset)
            {
                ColumnQueryBuilder.Append($"{col.Key.ToString()}, ");
                ValuesQueryBuilder.Append($"{ValueReader(col.Value)}, ");
            }
            ColumnQueryBuilder.Length -= 2;
            ColumnQueryBuilder.Append(")");
            ValuesQueryBuilder.Length -= 2;
            ValuesQueryBuilder.Append(")");
            InsertQueryBuilder.Append(ColumnQueryBuilder.ToString() + ValuesQueryBuilder.ToString() + ";").ToString();
            return InsertQueryBuilder.ToString();
        }
        public static string UpdateQuery<T>(T _Object, string whereClause, string schemaName = null, string tableName = null)
        {
            List<PropertyInfo> properties = GetUpdateProperties<T>();
            StringBuilder UpdateQueryBuilder = new StringBuilder();
            UpdateQueryBuilder.Append($"UPDATE {schemaName ?? GetSchemaName<T>()}.{tableName ?? GetTableName<T>()} SET ");
            foreach (var pi in properties)
            {
                UpdateQueryBuilder.Append($"{GetColumnName(pi)} = {ValueReader(pi.GetValue(_Object, null))}, ");
            }
            UpdateQueryBuilder.Length -= 2;
            UpdateQueryBuilder.Append($" {whereClause}");
            return UpdateQueryBuilder.ToString();
        }
        public static string UpdateQuery(string table, Dictionary<string, string> dataset, string whereClause)
        {
            StringBuilder UpdateQueryBuilder = new StringBuilder();
            UpdateQueryBuilder.Append($"Update [{table}] SET ");
            foreach (var col in dataset)
            {
                UpdateQueryBuilder.Append($"{col.Key} = {ValueReader(col.Value)}, ");
            }
            UpdateQueryBuilder.Length -= 2;
            UpdateQueryBuilder.Append($" {whereClause};");
            return UpdateQueryBuilder.ToString();
        }
        public static string DeleteQuery<T>(string whereClause, string schemaName = null, string tableName = null)
        {
            return $"Delete from {schemaName ?? GetSchemaName<T>()}.{tableName ?? GetTableName<T>()} {whereClause};";
        }
        public static string BeginTransQuery()
        {
            string query = string.Empty;
            query +=
                "BEGIN TRY " +
                "BEGIN TRANSACTION ";
            return query;
        }
        public static string CommitTransQuery()
        {
            string query = string.Empty;
            query +=
                "COMMIT " +
                "END TRY " +
                "BEGIN CATCH " +
                "declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;" +
                "select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();" +
                "rollback transaction;" +
                "raiserror(@ErrorMessage, @ErrorSeverity, @ErrorState);" +
                "END CATCH ";
            return query;
        }
        public static PagedQuery GetPagerQueries<T>(string query, string sortBy, string order, int offset, int numberOfRecords)
        {
            string mainquery = $"SELECT * FROM ({  query ?? SelectQuery<T>(query, "")}) MyList order by {sortBy} {order} offset {offset} rows fetch next {numberOfRecords} rows only";
            string countquery = $"SELECT COUNT(*) FROM ({  query ?? SelectQuery<T>(query, "") }) MyList";
            return new PagedQuery { Query = mainquery, CountQuery = countquery };
        }
        public static PagedQuery GetPagerQueries<T>(string query, Dictionary<string, string> sortAndOrder, int offset, int numberOfRecords)
        {
            string mainquery = $"SELECT * FROM ({ query ?? SelectQuery<T>(query, "")}) MyList {MultipleOrderByQuery(sortAndOrder, numberOfRecords, offset)}";
            string countquery = $"SELECT COUNT(*) FROM ({ query ?? SelectQuery<T>(query, "")}) MyList";
            return new PagedQuery { Query = mainquery, CountQuery = countquery };
        }
        #endregion Public methods

        #region Private methods
        private static List<PropertyInfo> GetReadableProperties<T>()
        {
            return typeof(T).GetProperties().Where(x => !(Attribute.IsDefined(x, typeof(DontLoad)))).ToList();
        }
        private static List<PropertyInfo> GetInsertProperties<T>()
        {
            return typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly).ToList().Where(x => !(Attribute.IsDefined(x, typeof(DontInsert)))).ToList();
        }
        private static List<PropertyInfo> GetUpdateProperties<T>()
        {
            return typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly).ToList().Where(x => !(Attribute.IsDefined(x, typeof(DontUpdate)))).ToList();
        }
        private static string GetColumnName(PropertyInfo pi)
        {
            if (Attribute.IsDefined(pi, typeof(Column)))
            {
                Column k = (Column)Attribute.GetCustomAttribute(pi, typeof(Column));
                return $"[{k.Name}]";
            }
            else
            {
                return $"[{pi.Name}]";
            }
        }
        private static string GetTableName<T>()
        {
            if (Attribute.IsDefined(typeof(T), typeof(Table)))
            {
                Table t = (Table)Attribute.GetCustomAttribute(typeof(T), typeof(Table));
                return $"[{t.TableName }]";
            }
            else
            {
                return $"[{typeof(T).Name}]";
            }
        }



        private static string GetSchemaName<T>()
        {
            if (Attribute.IsDefined(typeof(T), typeof(Schema)))
            {
                Schema t = (Schema)Attribute.GetCustomAttribute(typeof(T), typeof(Schema));
                return $"[{t.SchemaName}]";
            }
            else
            {
                return "[dbo]";
            }
        }
        private static string ValueReader(Object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else if (value.GetType() == typeof(DateTime))
            {
                DateTime date = (DateTime)value;
                return $"'{date.ToString("yyyy-MM-dd hh:mm:ss")}'";
            }
            else
            {
                return $"'{value.ToString().Replace("\'", "'\'\'")}'";
            }
        }
        private static string MultipleOrderByQuery(Dictionary<string, string> sortByAndOrder, int numberOfRecords, int offset)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" order by ");
            foreach (var element in sortByAndOrder)
            {
                sb.Append(element.Key);
                sb.Append($" {element.Value}, ");
            }
            sb.Length -= 2;
            sb.Append($" offset {offset} rows fetch next {numberOfRecords} rows only");
            return sb.ToString();
        }
        #endregion PrivateMethods

    }
    public class Pager
    {
        public int TotalRecords { get; set; }
        public int RecordsPerPage { get; set; }
        public int PageNumber { get; set; }
        public int Offset { get; set; }
        public int Fetch { get; set; }
        public int TotalPages { get; set; }
    }
    public class Schema : Attribute
    {
        public string SchemaName { get; set; }
        public Schema(string schemaName)
        {
            this.SchemaName = schemaName;
        }
    }
    public class Table : Attribute
    {
        public string TableName { get; set; }
        public Table(string tableName)
        {
            this.TableName = tableName;
        }
    }
    public class Column : Attribute
    {
        public string Name { get; set; }
        public Column(string name)
        {
            this.Name = name;
        }
    }
    public class DontUpdate : Attribute
    {
    }
    public class DontLoad : Attribute
    {
    }
    public class DontInsert : Attribute
    {
    }

}

