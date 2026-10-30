using System.Data; using System.Data.SqlClient;
namespace Lab08_SQLCommand {
public static class SqlHelper {
 public static object ExecuteScalar(string sql, params SqlParameter[] ps){
  using(var c=DbConnectionFactory.Create()) using(var cmd=new SqlCommand(sql,c)){ if(ps!=null)cmd.Parameters.AddRange(ps); c.Open(); return cmd.ExecuteScalar(); }
 }
 public static int ExecuteNonQuery(string sql, params SqlParameter[] ps){
  using(var c=DbConnectionFactory.Create()) using(var cmd=new SqlCommand(sql,c)){ if(ps!=null)cmd.Parameters.AddRange(ps); c.Open(); return cmd.ExecuteNonQuery(); }
 }
 public static DataTable ExecuteTable(string sql, params SqlParameter[] ps){
  using(var c=DbConnectionFactory.Create()) using(var cmd=new SqlCommand(sql,c)) using(var da=new SqlDataAdapter(cmd)){
   if(ps!=null)cmd.Parameters.AddRange(ps); var dt=new DataTable(); da.Fill(dt); return dt;
  }
 }
}}