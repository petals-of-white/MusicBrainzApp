using Microsoft.Data.SqlClient;
using MusicBrainz.BLL.Exceptions;
using MusicBrainz.BLL.Exporting;
using MusicBrainz.Common.TableModels;
using MusicBrainz.Tools.Logging;

namespace MusicBrainz.ConsoleUI
{
    internal static class UserMessages
    {
        private static TableToJsonExporterBuilder _exporterBuilder = new();

        private static IList<DbTableInfo> _tables = _exporterBuilder.GetTableInfo();

        private static LoggerBase _logger = new FileLoggerFactory("musicbrainz.log").CreateLogger();

        internal static void GreetUser()
        {
            Console.WriteLine("Greetings! This app will allow you export MusicBrainz DB tables into JSON. Here are the tables:");
        }

        internal static void ShowAllTablesAndNumberOfRecords()
        {

            const short defaultPadding = 15;

            Console.WriteLine("#".PadRight(defaultPadding) + "Table name".PadRight(defaultPadding) + "Number of records".PadRight(defaultPadding));

            Console.WriteLine("".PadRight(defaultPadding * 3, '='));

            //var tables = _exporterBuilder.GetTableInfo();

            for (int i = 0; i < _tables.Count; i++)
            {
                Console.WriteLine($"{i + 1,-defaultPadding}{_tables [i].Name,-defaultPadding}{_tables [i].NumberOfRecords,-defaultPadding}");

            }

            Console.WriteLine("".PadRight(defaultPadding * 3, '='));

        }

        internal static void PromptSelect()
        {
            string? input;

            Console.WriteLine("To select a table you want to export, please enter a corresponding number of a table. (e.g. '1' or '3'). " +
                "You can also select multiple tables, separated by space (e.g. '1 5 6 8'). To export all the tables, enter '*'");

            Console.Write("Your choice: ");

            bool proceedFurther = false;
            //while (true)
            while (proceedFurther == false)
            {
                input = Console.ReadLine();


                if (input == "*")
                {
                    _exporterBuilder.UseAllTables();

                    Console.WriteLine("All the tables are going be exported.");

                    proceedFurther = true;

                    break;
                }

                else
                {
                    string [] splitInput = input.Split(' ', StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries);

                    while (true)
                    {
                        try
                        {
                            // only distinct values
                            int [] tablesNumbers = Array.ConvertAll(splitInput, int.Parse).Distinct().ToArray();

                            // check whether all values are in range
                            bool tableNumbersInValidRange = tablesNumbers.All(x => x > 0 && x <= _tables.Count);

                            if (tableNumbersInValidRange)
                            {
                                foreach (int tableNumber in tablesNumbers)
                                {
                                    _exporterBuilder.UseTable(_tables [tableNumber - 1].Name);
                                }
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

        internal static void AskForPagination()
        {

            Console.Write("Would you like to enable pagination ( yes / no or y / n)? ");
            string answer = Console.ReadLine();

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
                    try
                    {
                        _exporterBuilder.EnablePagination(recordsPerPage, pageNumber);
                    }

                    catch (UserFriendlyException ex)
                    {
                        _logger.Log(ex.ToString());
                        Console.WriteLine(ex.Message);
                    }
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

            Console.WriteLine("Exporting...");

            // Initialize serialization and confirm the result
            try
            {
                ((TableToJsonExporter) _exporterBuilder.Build()).Export();
                Console.WriteLine("Exportation has been successfully completed.");
            }

            catch (UserFriendlyException ex)
            {
                _logger.Log(ex.ToString());
                Console.WriteLine(ex.Message);
            }
            //catch (IOException ex)
            //{
            //    _logger.Log(ex.ToString());

            //    Console.WriteLine(ex.Message);
            //}

            //catch (UnauthorizedAccessException ex)
            //{
            //    _logger.Log(ex.ToString());
            //    Console.WriteLine("An error has occured while trying to open json file. Please try again later.");


            //    Environment.Exit(0);
            //}

            //catch (ArgumentOutOfRangeException ex)
            //{

            //    _logger.Log(ex.ToString());
            //    Console.WriteLine(ex.Message);


            //    Environment.Exit(0);

            //}
            //catch (ArgumentException ex)
            //{
            //    _logger.Log(ex.ToString());
            //    Console.WriteLine(ex.Message);


            //    Environment.Exit(0);
            //}

            //catch (SqlException ex)
            //{
            //    _logger.Log(ex.ToString());
            //    Console.WriteLine("An error has occured while connecting to database. Please try again later.");
            //    Environment.Exit(0);
            //}
        }
        internal static void SayGoodbye()
        {
            Console.WriteLine("Thank you for using our app. Have a good day.");
        }
    }
}
