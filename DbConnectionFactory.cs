using System.Data.SqlClient;
namespace Lab08_SQLCommand {
public static class DbConnectionFactory {
 // Sua Data Source theo ten SQL Server tren may.
 private const string ConnectionString = @"Data Source=.\\SQLEXPRESS;Initial Catalog=QLSinhVien;Integrated Security=True";
 public static SqlConnection Create(){ return new SqlConnection(ConnectionString); }
}}