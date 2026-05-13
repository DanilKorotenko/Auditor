using Microsoft.Data.Sqlite;

namespace common;

public class OfflineDatabaseController
{
    private static readonly string DB_FILE_NAME = "activities.db";
    private static OfflineDatabaseController? _instance;
    public static OfflineDatabaseController? SharedController()
    {
        if (_instance == null)
        {
            _instance = new OfflineDatabaseController();
        }
        return _instance;
    }
    private OfflineDatabaseController() 
    {

    }

    //public Protector? searchSerial(string aSerial)
    //{
    //    string? currentProcessPath = Environment.ProcessPath;

    //    if (null == currentProcessPath)
    //    {
    //        throw new Exception("Unable to get process path");
    //    }

    //    string? directory = Path.GetDirectoryName(currentProcessPath);

    //    string filename = Path.Combine(directory ?? string.Empty, OFFLINE_FILE_NAME);

    //    return searchSerial(aSerial, filename);
    //}

    ///// <summary>
    ///// Look up a serial in an offline flash database at an explicit file path (e.g. user-selected copy).
    ///// </summary>
    //public OfflineDBProtector? searchSerial(string aSerial, string databaseFilePath)
    //{
    //    if (string.IsNullOrWhiteSpace(databaseFilePath))
    //    {
    //        throw new ArgumentException("Database file path is required.", nameof(databaseFilePath));
    //    }

    //    var connectionString = new SqliteConnectionStringBuilder($"Data Source={databaseFilePath}")
    //    {
    //        Password = cryptoApiClient.SharedSecret.SS,
    //        Mode = SqliteOpenMode.ReadOnly
    //    }.ToString();

    //    OfflineDBProtector? result = null;

    //    using (var connection = new SqliteConnection(connectionString))
    //    {
    //        connection.Open();

    //        var command = connection.CreateCommand();
    //        command.CommandText =
    //            @"
    //                SELECT 
    //                    protector_type, 
    //                    protector_value,
    //                    can_decrypt,
    //                    status 
    //                FROM offline_flash
    //                WHERE serial_number = $serial
    //            ";
    //        command.Parameters.AddWithValue("$serial", aSerial);

    //        using (var reader = command.ExecuteReader())
    //        {
    //            while (reader.Read())
    //            {
    //                result = new OfflineDBProtector();
    //                result.type = reader.GetInt16(0);
    //                result.key = reader.GetString(1);
    //                result.can_decrypt = reader.GetInt16(2);
    //                result.status = reader.GetString(3);
    //                break;
    //            }
    //        }
    //        connection.Close();
    //    }

    //    return result;
    //}
}
