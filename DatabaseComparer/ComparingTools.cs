using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseComparer
{
    public class ComparingTools
    {        
        public static void GetDatabaseInformationFromDatabase(ILogger logger)
        {
            
            logger.Log("Please write connection string: ");
            var connectionString = Console.ReadLine();
            logger.Log("Database Informations fetching from database");
            var databaseInfo = new DatabaseInfo(connectionString);
            logger.Log("Database Informations fetched.");
            logger.Log("Write file name for saving it to disk: ");
            var filePath = Console.ReadLine();
            logger.Log($"Database Informations writing to {filePath}");
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(fs, databaseInfo);
            }
            logger.Log($"Database Informations has been written to disk!");
        }

        public static DatabaseInfo GetDatabaseInformationFromDisk(ILogger logger)
        {
            logger.Log("Please write file path: ");
            var filePath = Console.ReadLine();
            if (File.Exists(filePath))
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var formatter = new BinaryFormatter();
                    var result = (DatabaseInfo)formatter.Deserialize(fs);
                    logger.Log("Database Info successfully loaded");
                    return result;
                }
            logger.Log("The file doesn't exist");
            return null;
        }      

        public static void CompareDatabases(ILogger logger)
        {
            var result = true;
            DatabaseInfo diOne;
            do
            {
                logger.Log("Please write first database's path");
                diOne = GetDatabaseInformationFromDisk(logger);
            } while (diOne == null);         

            DatabaseInfo diTwo;
            do
            {
                logger.Log("Please write second database's path");
                diTwo = GetDatabaseInformationFromDisk(logger);
            } while (diTwo == null);


            result &= CheckServerCollation(diOne.ServerCollation, diTwo.ServerCollation, logger);
            result &= CheckDatabaseCollation(diOne.DatabaseCollation, diTwo.DatabaseCollation, logger);
            result &= CheckForTable(diOne.Tables, diTwo.Tables, logger);
            result &= CheckForIndexes(diOne.Indexes, diTwo.Indexes, logger);
            result &= CheckForObjects(diOne.Procedures, diTwo.Procedures, logger);
            result &= CheckForObjects(diOne.Triggers, diTwo.Triggers, logger);
            result &= CheckForObjects(diOne.Functions, diTwo.Functions, logger);
            result &= CheckForObjects(diOne.ForeignKeys, diTwo.ForeignKeys, logger);
        }

        static bool CheckServerCollation(string serverCollationOne, string serverCollationTwo, ILogger logger)
        {
            var isEqual = serverCollationOne == serverCollationTwo;
            if (isEqual)
                logger.Log("Both server uses same Server Collation Name");
            else
            {
                logger.Log("Server Collation Name is different for these databases");
                logger.Log($"First One's Server Collation Name is '{serverCollationOne}'");
                logger.Log($"Second One's Server Collation Name is '{serverCollationTwo}'");
            }
            return isEqual;
        }

        static bool CheckDatabaseCollation(string databaseCollationOne, string databaseCollationTwo, ILogger logger)
        {
            var isEqual = databaseCollationOne == databaseCollationTwo;
            if (isEqual)
                logger.Log("Both databases uses same Database Collation Name");
            else
            {
                logger.Log("Database Collation Name is different for these databases");
                logger.Log($"First One's Database Collation Name is '{databaseCollationOne}'");
                logger.Log($"Second One's Database Collation Name is '{databaseCollationTwo}'");
            }
            return isEqual;
        }

        static bool CheckForTable(List<TableInfo> tablesOne, List<TableInfo> tablesTwo, ILogger logger)
        {
            logger.Log(Environment.NewLine);
            logger.Log("Tables are checking!");
            var result = true;
            foreach (var tiOne in tablesOne)
            {
                if (tablesTwo.Where(q => q == tiOne).Count() == 0)
                {
                    logger.Log($"(*) TABLE IS NOT EXIST IN SECOND DB: '{tiOne}'");
                    result &= false;
                }
                else result &= CheckForColumns(tiOne.Columns
                    , tablesTwo.SingleOrDefault(q => q.Name == tiOne.Name).Columns
                    , logger);                

            }
            logger.Log(Environment.NewLine);
            foreach (var tiTwo in tablesTwo)
            {
                if (tablesOne.Where(q => q == tiTwo).Count() == 0)
                {
                    logger.Log($"(*) TABLE IS NOT EXIST IN FIRST DB: '{tiTwo}'");
                    result &= false;
                }
                else result &= CheckForColumns(tiTwo.Columns
                    , tablesOne.SingleOrDefault(q => q.Name == tiTwo.Name).Columns
                    , logger);
            }
            return result;
        }
        static bool CheckForColumns(List<ColumnInfo> columnsOne, List<ColumnInfo> columnsTwo, ILogger logger)
        {
            var result = true;
            foreach (var ciOne in columnsOne)
            {

                if (columnsTwo.Where(q => q.TableName == ciOne.TableName && q.ColumnName == ciOne.ColumnName).Count() == 0)
                {
                    logger.Log($"(*) COLUMN IS NOT EXIST IN SECOND DB: '{ciOne}'");
                    result &= false;
                }
                else if (columnsTwo.Where(q => q == ciOne).Count() == 0)
                {
                    logger.Log($"(!) COLUMN IS NOT SAME IN SECOND DB: '{ciOne}'");
                    result &= false;
                }
            }
            foreach (var ciTwo in columnsTwo)
            {
                if (columnsOne.Where(q => q.TableName == ciTwo.TableName && q.ColumnName == ciTwo.ColumnName).Count() == 0)
                {
                    logger.Log($"(*) COLUMN IS NOT EXIST IN FIRST DB:  '{ciTwo}'");
                    result &= false;
                }
                else if (columnsOne.Where(q => q == ciTwo).Count() == 0)
                {
                    logger.Log($"(!) COLUMN IS NOT SAME IN FIRST DB: '{ciTwo}'");
                    result &= false;
                }
            }
            return result;
        }
        static bool CheckForIndexes(List<string> indexesOne, List<string> indexesTwo, ILogger logger)
        {
            var result = true;
            logger.Log(Environment.NewLine);
            logger.Log("Indexes are checking!");            
            foreach (var iOne in indexesOne)
            {
                if (indexesTwo.Where(q => q == iOne).Count() == 0)
                {
                    logger.Log($"(*) INDEX IS NOT EXIST IN SECOND DB: '{iOne}'");
                    result &= false;
                }
            }
            foreach (var iTwo in indexesTwo)
            {
                if (indexesOne.Where(q => q == iTwo).Count() == 0)
                {
                    logger.Log($"(*) INDEX IS NOT EXIST IN FIRST DB: '{iTwo}'");
                    result &= false;
                }
            }
            return result;
        }

        static bool CheckForObjects<T>(List<T> objectsOne, List<T> objectsTwo, ILogger logger) where T : SysObject
        {
            var result = true;
            var type = typeof(T).Name;
            logger.Log(Environment.NewLine);            
            logger.Log($"{type}s are checking!");
            foreach (var ciOne in objectsOne)
            {
                if (objectsTwo.Where(q => q.Name == ciOne.Name && q.Type == ciOne.Type).Count() == 0)
                {
                    logger.Log($"(*) {type} IS NOT EXIST IN SECOND DB: {ciOne}");
                    result &= false;
                }
                else if (objectsTwo.Where(q => q.Name == ciOne.Name 
                                            && q.Type == ciOne.Type 
                                            && q.Definition == ciOne.Definition
                                            ).Count() == 0)
                {
                    logger.Log($"(!) {type} IS NOT SAME IN SECOND DB: {ciOne}");
                    result &= false;
                }
            }
            foreach (var ciTwo in objectsTwo)
            {
                if (objectsOne.Where(q => q.Name == ciTwo.Name && q.Type == ciTwo.Type).Count() == 0)
                {
                    logger.Log($"(*) {type} IS NOT EXIST IN FIRST DB: '{ciTwo}'");
                    result &= false;
                }
                else if (objectsOne.Where(q => q.Name == ciTwo.Name
                                            && q.Type == ciTwo.Type
                                            && q.Definition == ciTwo.Definition
                                            ).Count() == 0)
                {
                    logger.Log($"(!) {type} IS NOT SAME IN FIRST DB: '{ciTwo}'");
                    result &= false;
                }
            }
            return result;
        }
    }
}
