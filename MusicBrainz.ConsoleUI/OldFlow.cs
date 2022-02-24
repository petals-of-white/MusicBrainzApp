using Microsoft.Data.SqlClient;
using MusicBrainz.BLL.DbEntitySerialization;
using MusicBrainz.BLL.DbEntitySerialization.Serialization;
using MusicBrainz.BLL.Exceptions;
using MusicBrainz.Common.Enums;
using MusicBrainz.Common.TableModels;
using MusicBrainz.Tools.Logging;

namespace MusicBrainz.ConsoleUI
{
    internal static class OldFlow
    {
        [Flags]
        internal enum Mode
        {
            None,
            Export,
            Import
        }

        #region Fields

        //private const short _defaultPadding = 16;
        //private static DbExportImportConfig _config = new();
        private static DbExportConfig _exportConfig = new();

        private static DbImportConfig _importConfig = new();
        private static LoggerBase _logger = new FileLoggerFactory("musicbrainz.log").CreateLogger();
        private static DbEntitiesSerializer _mainSerializer = new();
        private static Mode _mode = Mode.None;
        private static ISerializationManager _serializationManager = new JsonSerializationManager();
        private static IList<ITableInfo> _tables = _mainSerializer.GetTablesInfo();

        #endregion Fields

        #region Methods

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="SqlException"></exception>
        internal static void ConfirmResult()
        {
            switch (_mode)
            {
                case Mode.Export:

                    StartExporting();

                    break;

                case Mode.Import:

                    StartImporting();

                    break;
            }
        }

        internal static void GreetUser()
        {
            Console.WriteLine("Greetings! This app will allow you export MusicBrainz DB tables into JSON files. Here are the tables:");
        }

        internal static void SayGoodbye()
        {
            Console.WriteLine("Thank you for using our app. Have a good day.");
        }

        internal static void SelectAction()
        {
            Console.Write("Would you like to export or import the tables ('i' or 'e')? ");
            string mode = Console.ReadLine()!;

            switch (mode.ToLower())
            {
                case "i" or "import":
                    _mode = Mode.Import;
                    Console.WriteLine("Import mode!");

                    ShowAvailableImportFiles();
                    SelectTablesToImport();

                    break;

                case "e" or "export":
                    _mode = Mode.Export;
                    Console.WriteLine("Export mode!");

                    SelectTablesToExport();
                    AskForPagination();

                    break;

                default:
                    _mode = Mode.Export;
                    Console.WriteLine("Export mode!");

                    SelectTablesToExport();
                    AskForPagination();

                    break;
            }
        }

        internal static void ShowAllTablesAndNumberOfRecords()
        {
            const short defaultPadding = 16;

            Console.WriteLine("#".PadRight(defaultPadding) + "Table name".PadRight(defaultPadding) + "Number of records".PadRight(defaultPadding));

            Console.WriteLine("".PadRight(defaultPadding * 3, '='));

            //for (int i = 0; i < _tables.Count; i++)
            //{
            //    Console.WriteLine($"{i + 1,-defaultPadding}{_tables [i].Name,-defaultPadding}{_tables [i].NumberOfRecords,-defaultPadding}");
            //}

            foreach (ITableInfo tableInfo in _tables)
            {
                Console.WriteLine($"{(int) tableInfo.Name,-defaultPadding}{tableInfo.Name,-defaultPadding}{tableInfo.NumberOfRecords,-defaultPadding}");
            }

            Console.WriteLine("".PadRight(defaultPadding * 3, '='));
        }

        private static void AskForPagination()
        {
            Console.Write("Would you like to enable pagination ( yes / no or y / n)? ");
            string answer = Console.ReadLine()!;

            switch (answer)
            {
                case "yes" or "y":

                    int pageNumber, recordsPerPage;

                    // Set records per page

                    Console.Write("How many records must a page contain? ");

                    while (int.TryParse(Console.ReadLine(), out recordsPerPage) == false || recordsPerPage < 1)
                    {
                        Console.WriteLine("The input data is wrong.");
                        Console.Write("How many records must a page contain? ");
                    }

                    // Set a page number
                    Console.Write("Please provide a page number you would like to export: ");
                    while (int.TryParse(Console.ReadLine(), out pageNumber) == false || pageNumber < 1)
                    {
                        Console.WriteLine("The input data is wrong.");
                        Console.Write("Please provide a page number you would like to export: ");
                    }

                    // enables pagination and sets page number
                    _exportConfig.EnablePaging(recordsPerPage, pageNumber);

                    Console.WriteLine($"The pagination will be enabled. Records per page: {recordsPerPage}. Page number is {pageNumber}.");
                    break;

                case "no" or "n":
                    Console.WriteLine("The pagination will be disabled.");
                    break;

                default:
                    Console.WriteLine("The pagination will be disabled.");
                    break;
            }
        }

        private static HashSet<T> MultipleOptionsSelector<T>(IList<T> choices)
        {
            // output set
            HashSet<T> choicesResult = new();

            // all available table entity enums
            int [] tablesEnums = ((int []) Enum.GetValues(typeof(Tables)));

            string? input;
            bool proceedFurther = false;

            Console.WriteLine("To select a table, please enter a corresponding number of a table. (e.g. '1' or '3'). " +
                "You can also select multiple tables by entering their numbers separated by space (e.g. '1 5 6 8'). To select all the tables, enter '*'");

            Console.Write("Your choice: ");

            while (proceedFurther == false)
            {
                input = Console.ReadLine();

                if (input == "*")
                {
                    // adding all elements to our set
                    choicesResult.UnionWith(choices);

                    Console.WriteLine("All the tables are going be exported.");

                    proceedFurther = true;

                    break;
                }
                else
                {
                    string [] splitInput = input.Split(' ',
                        StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries);

                    while (true)
                    {
                        try
                        {
                            // only distinct values
                            var tablesNumbers = Array.ConvertAll(splitInput, int.Parse).Distinct();

                            // check whether all values are in range
                            bool tableNumbersInValidRange = tablesNumbers.All(
                                x => x > 0 && x < choices.Count);

                            // then add those values to our hashset if true
                            if (tableNumbersInValidRange)
                            {
                                foreach (var tableNumber in tablesNumbers)
                                {
                                    choicesResult.Add(choices [tableNumber]);
                                }
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(tablesNumbers), $"Table numbers value were not in the range of {1}..{choices.Count}");
                            }

                            proceedFurther = true;

                            break;
                        }
                        catch (FormatException ex)
                        {
                            _logger.Log(ex.ToString());
                            Console.Write("Incorrent data. Please enter a valid table name(s) and try once again: ");
                            break;
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            _logger.Log(ex.ToString());
                            Console.WriteLine("Entered values were not in the defined range. Please enter valid value(s) and try again.");

                            break;
                        }
                        catch (ArgumentException ex)
                        {
                            _logger.Log(ex.ToString());
                            Console.WriteLine(ex.Message);

                            break;
                        }
                        catch (OverflowException ex)
                        {
                            _logger.Log(ex.ToString());
                            Console.WriteLine("Entered values were not in the defined range. Please enter valid value(s) and try again.");
                            break;
                        }
                    }
                }
            }
            return choicesResult;
        }

        private static void SelectTablesToExport()
        {
            string? input;
            bool proceedFurther = false;

            Console.WriteLine("To select a table you want to export, please enter a corresponding number of a table. (e.g. '1' or '3'). " +
                "You can also select multiple tables by entering their numbers separated by space (e.g. '1 5 6 8'). To export all the tables, enter '*'");

            Console.Write("Your choice: ");

            while (proceedFurther == false)
            {
                input = Console.ReadLine();

                if (input == "*")
                {
                    _exportConfig.AddTableToExport((Tables []) Enum.GetValues(typeof(Tables)));

                    Console.WriteLine("All the tables are going be exported.");

                    proceedFurther = true;

                    break;
                }
                else
                {
                    string [] splitInput = input.Split(' ',
                        StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries);

                    while (true)
                    {
                        try
                        {
                            // only distinct values
                            int [] tablesNumbers = Array.ConvertAll(splitInput, int.Parse).Distinct().ToArray();

                            // all available Tables enum values
                            int [] tablesEnums = ((int []) Enum.GetValues(typeof(Tables)));

                            // check whether all values are in range
                            bool tableNumbersInValidRange = tablesNumbers.All(
                                x => tablesEnums.Contains(x));

                            if (tableNumbersInValidRange)
                            {
                                _exportConfig.AddTableToExport((Tables []) (object) tablesNumbers);
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(tablesNumbers), $"Table numbers value were not in the range of {1}..{_tables.Count}");
                            }

                            proceedFurther = true;

                            break;
                        }
                        catch (FormatException ex)
                        {
                            _logger.Log(ex.ToString());
                            Console.Write("Incorrent data. Please enter a valid table name(s) and try once again: ");
                            break;
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            _logger.Log(ex.ToString());
                            Console.WriteLine("Entered values were not in the defined range. Please enter valid value(s) and try again.");

                            break;
                        }
                        catch (ArgumentException ex)
                        {
                            _logger.Log(ex.ToString());
                            Console.WriteLine(ex.Message);

                            break;
                        }
                        catch (OverflowException ex)
                        {
                            _logger.Log(ex.ToString());
                            Console.WriteLine("Entered values were not in the defined range. Please enter valid value(s) and try again.");
                            break;
                        }
                    }
                }
            }
        }

        private static void SelectTablesToImport()
        {
            //var something = MultipleOptionsSelector();
            /*
            bool proceedFurther = false;
            string input;
            Console.WriteLine("To select a table you want to import, please enter a corresponding number of a table. (e.g. '1' or '3'). " +
                "You can also select multiple tables by entering their numbers separated by space (e.g. '1 5 6 8'). To import all the tables, enter '*'");

            Console.Write("Your choice: ");

            while (proceedFurther == false)
            {
                input = Console.ReadLine()!;

                if (input == "*")
                {
                    _exportConfig.AddTableToExport((Tables []) Enum.GetValues(typeof(Tables)));

                    Console.WriteLine("All the tables are going be imported.");

                    proceedFurther = true;

                    break;
                }
                else
                {
                    string [] splitInput = input.Split(' ',
                        StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries);

                    while (true)
                    {
                        try
                        {
                            // only distinct values
                            int [] tablesNumbers = Array.ConvertAll(splitInput, int.Parse).Distinct().ToArray();

                            // all available Tables enum values
                            int [] tablesEnums = ((int []) Enum.GetValues(typeof(Tables)));

                            // check whether all values are in range
                            bool tableNumbersInValidRange = tablesNumbers.All(
                                x => tablesEnums.Contains(x));

                            if (tableNumbersInValidRange)
                            {
                                _importConfig.AddEntitiesToImport((Tables []) (object) tablesNumbers);
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException(nameof(tablesNumbers), $"Table numbers value were not in the range of {1}..{_tables.Count}");
                            }

                            proceedFurther = true;

                            break;
                        }
                        catch (FormatException ex)
                        {
                            _logger.Log(ex.ToString());
                            Console.Write("Incorrent data. Please enter a valid table numbers(s) and try once again: ");
                            break;
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            _logger.Log(ex.ToString());
                            Console.WriteLine("Entered values were not in the defined range. Please enter valid value(s) and try again.");

                            break;
                        }
                        catch (ArgumentException ex)
                        {
                            _logger.Log(ex.ToString());
                            Console.WriteLine(ex.Message);

                            break;
                        }
                        catch (OverflowException ex)
                        {
                            _logger.Log(ex.ToString());
                            Console.WriteLine("Entered values were not in the defined range. Please enter valid value(s) and try again.");
                            break;
                        }
                    }
                }
            }
            */
        }

        private static void ShowAvailableImportFiles()
        {
            const short defaultPadding = 16;
            EntitiesFileManager fileManager = new();

            Console.WriteLine($"Here are the JSON files we've found in your local '{fileManager.ImportFolder}' folder:");

            Console.WriteLine("#".PadRight(defaultPadding) + "File name".PadRight(defaultPadding) + "Size".PadRight(defaultPadding));
            Console.WriteLine("".PadRight(defaultPadding * 3, '='));

            IList<FileInfo> files = fileManager.GetImportFiles();

            // printing files info
            for (int i = 0; i < files.Count; i++)
            {
                string length = string.Empty;
                if (files [i].Length >= (1 << 10))
                    length = string.Format("{0}Kb", files [i].Length >> 10);
                else
                    length = $"{files [i].Length}b";

                Console.WriteLine($"{i + 1}".PadRight(defaultPadding) + files [i].Name.PadRight(defaultPadding) + length.PadRight(defaultPadding));
            }

            Console.WriteLine("".PadRight(defaultPadding * 3, '='));

            //foreach (FileInfo file in fileManager.GetImportFiles())
            //{
            //    string length = string.Empty;
            //    if (file.Length >= (1 << 10))
            //        length = string.Format("{0}Kb", file.Length >> 10);
            //    else
            //        length = $"{file.Length}b";

            //    Console.WriteLine(.PadRight(defaultPadding) + "File name".PadRight(defaultPadding) + "Size".PadRight(defaultPadding));
            //}

            //foreach (string filePath in files)
            //{
            //    Action kek = File.Exists(filePath)

            //        ? delegate ()
            //        {
            //            File.WriteAllText(filePath, "[ ]");
            //        }
            //    : delegate ()
            //    {
            //    };
            //}
        }

        private static void StartExporting()
        {
            Console.WriteLine("Exporting...");

            // Initialize serialization and confirm the result
            try
            {
                _mainSerializer.ConfigureExport(_exportConfig);
                Console.WriteLine("Exportation has been successfully completed.");
            }
            catch (UserFriendlyException ex)
            {
                _logger.Log(ex.ToString());
                Console.WriteLine(ex.Message);
            }
        }

        private static void StartImporting()
        {
            Console.WriteLine("Importing...");

            _mainSerializer.ConfigureImport(_importConfig);

            try
            {
                _mainSerializer.ImportSerializedTableEntitiesNew();
                Console.WriteLine("Exportation has been successfully completed.");
            }
            catch (UserFriendlyException ex)
            {
                _logger.Log(ex.ToString());
                Console.WriteLine(ex.Message);
            }
        }

        #endregion Methods
    }
}