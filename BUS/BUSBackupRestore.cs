using System;
using System.Data.SqlClient;
using System.IO;

namespace BUS
{
    public class BUSBackupRestore
    {
        private static BUSBackupRestore instance;
        public static BUSBackupRestore Instance
        {
            get { if (instance == null) instance = new BUSBackupRestore(); return instance; }
            set => instance = value;
        }

        private string ConnectionString => DTO.QLTVDb.Instance.Database.Connection.ConnectionString;

        private string GetDatabaseName()
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString);
            return builder.InitialCatalog;
        }

        private string GetServerName()
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString);
            return builder.DataSource;
        }

        private string MasterConnection()
        {
            var builder = new SqlConnectionStringBuilder(ConnectionString);
            builder.InitialCatalog = "master";
            return builder.ConnectionString;
        }

        public string BackupDatabase(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return "Vui lòng chọn thư mục lưu file.";

            if (!Directory.Exists(folderPath))
                return "Thư mục không tồn tại.";

            string dbName = GetDatabaseName();
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filePath = Path.Combine(folderPath, $"{dbName}_{timestamp}.bak");

            string sql = $@"
                BACKUP DATABASE [{dbName}]
                TO DISK = N'{filePath}'
                WITH FORMAT, INIT, NAME = N'{dbName} Full Backup',
                SKIP, NOREWIND, NOUNLOAD, STATS = 10";

            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    cn.Open();
                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.CommandTimeout = 300;
                        cmd.ExecuteNonQuery();
                    }
                }

                if (File.Exists(filePath))
                    return $"Sao lưu thành công!\n\nFile: {filePath}";
                return "Sao lưu thất bại - file không được tạo.";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 3201)
                    return "Không thể ghi file sao lưu. Kiểm tra quyền ghi vào thư mục đã chọn.";
                return $"Lỗi SQL: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Lỗi: {ex.Message}";
            }
        }

        public string RestoreDatabase(string backupFilePath)
        {
            if (string.IsNullOrWhiteSpace(backupFilePath))
                return "Vui lòng chọn file sao lưu.";

            if (!File.Exists(backupFilePath))
                return "File sao lưu không tồn tại.";

            string dbName = GetDatabaseName();
            string serverName = GetServerName();

            try
            {
                using (var cn = new SqlConnection(MasterConnection()))
                {
                    cn.Open();

                    string killSessions = $@"
                        DECLARE @kill VARCHAR(8000) = '';
                        SELECT @kill = @kill + 'KILL ' + CONVERT(VARCHAR(5), session_id) + ';'
                        FROM sys.dm_exec_sessions WHERE database_id = DB_ID('{dbName}');
                        EXEC(@kill);";

                    using (var cmd = new SqlCommand(killSessions, cn))
                    {
                        cmd.CommandTimeout = 60;
                        cmd.ExecuteNonQuery();
                    }

                    string setSingleUser = $@"
                        ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                    using (var cmd = new SqlCommand(setSingleUser, cn))
                    {
                        cmd.CommandTimeout = 30;
                        cmd.ExecuteNonQuery();
                    }

                    string dataPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                        @"Microsoft SQL Server\MSSQL15.SQL2019\MSSQL\DATA");
                    string mdf = Path.Combine(dataPath, $"{dbName}.mdf");
                    string ldf = Path.Combine(dataPath, $"{dbName}_log.ldf");

                    string restoreSql = $@"
                        RESTORE DATABASE [{dbName}]
                        FROM DISK = N'{backupFilePath}'
                        WITH REPLACE, STATS = 10";

                    using (var cmd = new SqlCommand(restoreSql, cn))
                    {
                        cmd.CommandTimeout = 300;
                        cmd.ExecuteNonQuery();
                    }

                    string setMultiUser = $@"
                        ALTER DATABASE [{dbName}] SET MULTI_USER;";
                    using (var cmd = new SqlCommand(setMultiUser, cn))
                    {
                        cmd.CommandTimeout = 30;
                        cmd.ExecuteNonQuery();
                    }
                }

                return $"Khôi phục thành công!\n\nDatabase: {dbName}";
            }
            catch (SqlException ex)
            {
                TrySetMultiUser();
                return $"Lỗi SQL ({ex.Number}): {ex.Message}";
            }
            catch (Exception ex)
            {
                TrySetMultiUser();
                return $"Lỗi: {ex.Message}";
            }
        }

        private void TrySetMultiUser()
        {
            try
            {
                using (var cn = new SqlConnection(MasterConnection()))
                {
                    cn.Open();
                    using (var cmd = new SqlCommand(
                        $"ALTER DATABASE [{GetDatabaseName()}] SET MULTI_USER;", cn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }
    }
}
