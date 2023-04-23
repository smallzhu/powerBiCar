using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace CarRegistrations
{
    class Program
    {
        //
        // CHANGE these values to increase the number of owners and registrations to the tables, respectively.
        // NOTE: Since Registrations depend on Owners, Owners are added to the DB first
        // - this is how it is set up now in Main method.
        //
        static int ADDNUMBEROFOWNERS = 10000;
        static int ADDNUMBEROFREGS = 10000;

        //
        // CHANGE strings for new connection if values are different for server and database name.
        // 
        static string _serverName = "SURFACE3-NM-JEN\\SQLEXPRESS";
        static string _databaseName = "CarRegistrations";

        // 
        // UPDATE locations of files for importing data into these tables:

        static string filePathLocation = @"C:\Users\Public\Documents\";
        
        // States:
        static string statesFileLocation = filePathLocation + "States.txt";
        // Counties:
        static string countiesFileLocation = filePathLocation + "Counties.txt";
        // CarMake:
        static string carMakeFileLocation = filePathLocation + "CarMake.txt";
        // CarModel:
        static string carModelFileLocation = filePathLocation + "CarModels.txt";
        // CarType:
        static string carTypeFileLocation = filePathLocation + "CarType.txt";
        // CarColor:
        static string carColorFileLocation = filePathLocation + "CarColors.txt";

        //
        // Creates connection to SQL DB, imports data through files and also imports data through this script,
        // then closes connection.
        //
        static void Main(string[] args)
        {
            Console.WriteLine("Opening connection to: " + _serverName);

            using (SqlConnection myConn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    myConn.Open();
                    Console.WriteLine("Connection to " + _serverName + " is successful!");

                    CheckTableStatusAndAddValues(myConn);

                    AddDataOwnersTable(myConn);

                    AddDataMainTable(myConn);

                    myConn.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            Console.WriteLine("Press enter to exit:");
            Console.ReadLine();
        }

        //
        // Adds data to the Owners table.
        //
        static void AddDataOwnersTable(SqlConnection myConnection)
        {
            Random rand = new Random();
            int count = 0;
            using (SqlCommand getCountofOwners = new SqlCommand("SELECT COUNT(*) FROM Owners;", myConnection))
            {
                count = int.Parse(getCountofOwners.ExecuteScalar().ToString());
            }

            for (int next = count; next < count + ADDNUMBEROFOWNERS; next++)
            {
                int ownerAge = rand.Next(16, 80);
                int ownerGender = rand.Next(2);
                char ownerGenderChar;
                string ownerFirstName = "first" + (next + 1);
                string ownerLastName = "last" + (next + 1);

                int ownerState = rand.Next(1, 51);
                int ownerCounty = GetRangeOfValues(myConnection, ownerState, "Counties", "County_ID", "State_ID");

                ownerGenderChar = (ownerGender == 0) ? 'F' : 'M';

                string valueString = string.Format("INSERT INTO Owners " +
                                                   "(OwnerLastName, OwnerFirstName, OwnerAge, OwnerGender, OwnerState_ID, OwnerCounty_ID)" +
                                                   "VALUES ('{0}', '{1}', {2}, '{3}', {4}, {5});", 
                                                   ownerLastName, ownerFirstName, ownerAge, ownerGenderChar, ownerState, ownerCounty);

                using (SqlCommand addValue = new SqlCommand(valueString, myConnection))
                {
                    addValue.ExecuteNonQuery();
                }
            }
            Console.WriteLine(ADDNUMBEROFOWNERS + " values have been added to the Owner table.");
        }

        //
        // Adds data to the Main table
        //
        static void AddDataMainTable(SqlConnection myConnection)
        {
            Random rand = new Random();
            int count = 0;

            using (SqlCommand getCountOfRegs = new SqlCommand("SELECT COUNT(*) FROM Main;", myConnection))
            {
                count = int.Parse(getCountOfRegs.ExecuteScalar().ToString());
            }

            int carColorCount = 0;
            int carMakeCount = 0;
            int carTypeCount = 0;
            int ownerIDCount = 0;

            using (SqlCommand getCountOfCarColors = new SqlCommand("SELECT COUNT(*) FROM CarColor;", myConnection))
            {
                carColorCount = int.Parse(getCountOfCarColors.ExecuteScalar().ToString());
            }
            using (SqlCommand getCountOfCarMakes = new SqlCommand("SELECT COUNT(*) FROM CarMake;", myConnection))
            {
                carMakeCount = int.Parse(getCountOfCarMakes.ExecuteScalar().ToString());
            }
            using (SqlCommand getCountOfCarType = new SqlCommand("SELECT COUNT(*) FROM CarType;", myConnection))
            {
                carTypeCount = int.Parse(getCountOfCarType.ExecuteScalar().ToString());
            }
            using (SqlCommand getCountOfOwners = new SqlCommand("SELECT COUNT(*) FROM Owners;", myConnection))
            {
                ownerIDCount = int.Parse(getCountOfOwners.ExecuteScalar().ToString());
            }

                for (int next = count; next < count + ADDNUMBEROFREGS; next++)
                {
                    int carYear = rand.Next(1972, 2017);
                    int carColor = rand.Next(1, carColorCount + 1);
                    int carType = rand.Next(1, carTypeCount + 1);
                    int carMake = rand.Next(1, carMakeCount + 1);
                    int carModel = GetRangeOfValues(myConnection, carMake, "CarModel", "Model_ID", "Make_ID");
                    int carValue = rand.Next(1000, 180000);

                    int regState = rand.Next(1, 51);
                    int regCounty = GetRangeOfValues(myConnection, regState, "Counties", "County_ID", "State_ID");

                    int ownerID = rand.Next(1, ownerIDCount + 1);

                    DateTime dateRegistered = new DateTime(2015, 1, 1);
                    int range = (DateTime.Today - dateRegistered).Days;
                    dateRegistered = dateRegistered.AddDays(rand.Next(range));

                    string createQuery = string.Format("INSERT INTO Main " +
                                                       "(Owner_ID, DateRegistered, CarMake_ID, CarModel_ID, CarType_ID," +
                                                       "CarYear, CarColor_ID, RegisteredState_ID, RegisteredCounty_ID, CarValue)" +
                                                       "VALUES ({0}, @Value, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8});", ownerID, carMake, carModel,
                                                       carType, carYear, carColor, regState, regCounty, carValue);

                    using (SqlCommand addValue = new SqlCommand(createQuery, myConnection))
                    {
                        addValue.Parameters.AddWithValue("@Value", dateRegistered);
                        addValue.ExecuteNonQuery();
                    }
                }
            Console.WriteLine(ADDNUMBEROFREGS + " values have been added to the Main table.");
        }

        //
        // Gets range of values - helper method
        //
        static int GetRangeOfValues(SqlConnection myConnection, int value, string table, string sortColumn, string column)
        {
            Random rand = new Random();
            string firstValue = string.Format("SELECT TOP 1 {0} FROM {1} WHERE {2} = {3}", sortColumn, table, column, value);
            string lastValue = string.Format(firstValue + " ORDER BY {0} DESC;", sortColumn);

            int low = 0;
            int high = 0;

            using (SqlCommand getFirstValue = new SqlCommand(firstValue, myConnection))
            {
                low = int.Parse(getFirstValue.ExecuteScalar().ToString());
            }
            using (SqlCommand getLastValue = new SqlCommand(lastValue, myConnection))
            {
                high = int.Parse(getLastValue.ExecuteScalar().ToString());
            }
            return rand.Next(low, high + 1);
        }

        //
        // Checks tables whose data is being imported. If the tables are empty, the data is imported from the files.
        //
        static void CheckTableStatusAndAddValues(SqlConnection myConnection)
        {

            //
            // Check and add data for States table
            //
            if (TableIsNull("States", myConnection))
            {
                string[] columns = { "StateName" };
                AddDataToSQL(myConnection, statesFileLocation, "States", columns);
                Console.WriteLine("Query for adding States data is successful.");
            } else
                Console.WriteLine("States data has already been added.");

            //
            // Check and add data for Counties table
            //
            if (TableIsNull("Counties", myConnection))
            {
                string[] columns = { "State_ID", "CountyName", "CountyPopulation" };
                AddDataToSQL(myConnection, countiesFileLocation, "Counties", columns);
                Console.WriteLine("Query for adding Counties data is successful.");
            } else
                Console.WriteLine("Counties data has already been added.");

            //
            // Check and add data for CarMake table
            //
            if (TableIsNull("CarMake", myConnection))
            {
                string[] columns = { "MakeName" };
                AddDataToSQL(myConnection, carMakeFileLocation, "CarMake", columns);
                Console.WriteLine("Query for adding CarMake data is successful.");
            } else
                Console.WriteLine("CarMake data has already been added.");

            //
            // Check and add data for CarModel table
            //
            if (TableIsNull("CarModel", myConnection))
            {
                string[] columns = { "Make_ID", "ModelName" };
                AddDataToSQL(myConnection, carModelFileLocation, "CarModel", columns);
                Console.WriteLine("Query for adding CarModel data is successful.");
            } else
                Console.WriteLine("CarModel data has already been added.");

            //
            // Check and add data for CarType table
            //
            if (TableIsNull("CarType", myConnection))
            {
                string[] columns = { "CarTypeName" };
                AddDataToSQL(myConnection, carTypeFileLocation, "CarType", columns);
                Console.WriteLine("Query for adding CarType data is successful.");
            } else
                Console.WriteLine("CarType data has already been added.");

            //
            // Check and add data for CarColor table
            //
            if (TableIsNull("CarColor", myConnection))
            {
                string[] columns = { "ColorName" };
                AddDataToSQL(myConnection, carColorFileLocation, "CarColor", columns);
                Console.WriteLine("Query for adding CarType data is successful.");
            } else
                Console.WriteLine("CarColor data has already been added.");
        }

        //
        // Check to see if the table is null (empty)
        //
        static Boolean TableIsNull(string tableName, SqlConnection myConnection)
        {
            string rowCount = string.Format("SELECT COUNT(*) FROM {0};", tableName);
            int rows = 0;

            using (SqlCommand getRowCount = new SqlCommand(rowCount, myConnection))
            {
                rows = int.Parse(getRowCount.ExecuteScalar().ToString());
            }
            return (rows == 0);
        }

        // 
        // Add data to a table that has any columns of values
        // 
        static void AddDataToSQL(SqlConnection myConnection, string fileLocation, string tableName, string[] columns)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(fileLocation);

            // Get column data types 
            string[] columnDataTypes = new string[columns.Length];

            for (int counter = 0; counter < columns.Length; counter++) 
            {
                string getType = string.Format("SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'"
                                               + "AND COLUMN_NAME = '{1}'", tableName, columns[counter]);

                using (SqlCommand getColumnType = new SqlCommand(getType, myConnection))
                {
                    columnDataTypes[counter] = getColumnType.ExecuteScalar().ToString();
                }
            }

            string line;
            while ((line = file.ReadLine()) != null)
            {
                string sqlCommand = string.Format("INSERT INTO {0} (", tableName);
                for (int i = 0; i < columns.Length - 1; i++)
                {
                    sqlCommand += string.Format("{0}, ", columns[i]);
                }
                sqlCommand += string.Format("{0}) VALUES (", columns[columns.Length - 1]);

                string[] values = line.Split('\t');

                for (int i = 0; i < values.Length - 1; i++)
                {
                    sqlCommand += (columnDataTypes[i] == "varchar") ?
                        string.Format("'{0}', ", values[i]) : string.Format("{0}, ", values[i]);
                }

                sqlCommand += (columnDataTypes[values.Length - 1] == "varchar") ?
                    string.Format("'{0}' );", values[values.Length - 1]) : string.Format("{0} );", values[values.Length - 1]);

                using (SqlCommand addValues = new SqlCommand(sqlCommand, myConnection))
                {
                    addValues.ExecuteNonQuery();
                }
            }
            file.Close();
        }

        //
        // Get SQL database connection and test the connection. Returns connection if successful and writes to console.
        //
        static string GetConnectionString()
        {
            return string.Format("server={0};" + "Trusted_Connection=yes; database={1}; connection timeout=30", _serverName, _databaseName);
        }
    }
}
