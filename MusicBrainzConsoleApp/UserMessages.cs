using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicBrainzExportLibrary.Exporting;

namespace MusicBrainzConsoleApp
{
    internal static class UserMessages
    {
        private static ITableExporterBuilder _exporter = new TableToJsonExporterBuilder();

        internal static void GreetUser()
        {
            Console.WriteLine("Greetings! This app will allow you export MusicBrainz DB tables into JSON. Here are the tables");
        }

        internal static void ShowAllTablesAndNumberOfRecords()
        {

            const short defaultPadding = 15;

            Console.WriteLine("".PadRight(defaultPadding * 3, '='));

            Console.WriteLine("#".PadRight(defaultPadding) + "Table name".PadRight(defaultPadding) + "Amount of records".PadRight(defaultPadding));

            foreach (var table in _exporter.TablesInfo)
            {
                Console.WriteLine($"{table.Key,-defaultPadding}{table.Value.TableName,-defaultPadding}{table.Value.NumberOfRows,-defaultPadding}");
            }

            Console.WriteLine("".PadRight(defaultPadding * 3, '='));

        }

        internal static void PromptSelect()
        {
            bool exportAllTables;
            int tableNumber;
            string input;

            Console.WriteLine("To select a table you want to serialize, please enter a corresponding number of a table. (e.g. '1' or '3'). To serialize all the tables, enter '*'");
            Console.Write("Your choice: ");

            while (true)
            {
                input = Console.ReadLine();

                if (input == "*")
                {
                    exportAllTables = true;
                    Console.WriteLine("All tables are going be serialized be serialized.");
                    break;
                }

                else
                {
                    if ((int.TryParse(input, out tableNumber) == true) &&
                        (_exporter.TablesInfo.ContainsKey(tableNumber) == true))
                    {
                        _dbToJSONSerializer.TableToExport
                        Console.WriteLine($"Good! The table {_exporter.TablesInfo [tableNumber].TableName} is going to be serialized;");
                        break;
                    }

                    else
                    {
                        Console.Write("Incorrent data. Please enter a valid value and try once again: ");
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
                    while ((int.TryParse(Console.ReadLine(), out recordsPerPage) == false) || recordsPerPage < 1)
                    {
                        Console.WriteLine("The input data is wrong.");
                        Console.Write("How many records must a page contain? ");
                    }

                    // Set a page number
                    Console.Write("Please provide a page number you would like to serialize: ");
                    while ((int.TryParse(Console.ReadLine(), out pageNumber) == false) || pageNumber < 1)
                    {
                        Console.WriteLine("The input data is wrong.");
                        Console.Write("Please provide a page number you would like to serialize: ");
                    }

                    // enables pagination and sets page number
                    _exporter.EnablePagination(recordsPerPage, pageNumber);

                    Console.WriteLine($"The pagination will be enabled. Records per page: {recordsPerPage}. Page number is {pageNumber}.");

                    break;


                case "no" or "n":

                    // in this case the DBToJSONSerializer will do nothing about pagination,
                    // because it's off by default

                    //_dbToJSONSerializer.EnablePagination = false;
                    Console.WriteLine("The pagination will be disabled.");
                    break;

                default:

                    // same here
                    //_dbToJSONSerializer.EnablePagination = false;
                    Console.WriteLine("The pagination will be disabled.");
                    break;

            }
        }

        internal static void ConfirmResult()
        {
            // Initialize serialization and confirm the result
            try
            {
                _exporter.Build().Export();
                Console.WriteLine("Serialization successfully completed.");
            }
            catch (Exception ex)
            {
                // Handle this exception
                Console.WriteLine(ex.Message);
            }


        }

        internal static void SayGoodbye()
        {
            Console.WriteLine("Thank you for using our app. Have a good day.");
        }
    }
}
