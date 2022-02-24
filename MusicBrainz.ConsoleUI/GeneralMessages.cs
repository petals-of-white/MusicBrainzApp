using MusicBrainz.BLL.Exceptions;

namespace MusicBrainz.ConsoleUI
{
    internal static class GeneralMessages
    {
        internal static void AllTablesExportation()
        {
            Console.WriteLine("All the tables are going be exported.");
        }

        internal static void AskForMode()
        {
            Console.Write("Would you like to export or import the tables ('i' or 'e')? ");
        }

        internal static void AskForPageNumber()
        {
            Console.Write("Please provide a page number you would like to export: ");
        }

        internal static void AskForPaging()
        {
            Console.Write("Would you like to enable paging ( yes / no or y / n)? ");
        }

        internal static void AskForRecordsPerPage()
        {
            Console.Write("How many records must a page contain? ");
        }

        internal static void ExplainTableSelection()
        {
            Console.WriteLine("To select a table, please enter a corresponding number of a table. (e.g. '1' or '3'). " +
                "You can also select multiple tables by entering their numbers separated by space (e.g. '1 5 6 8'). To select all the tables, enter '*'");
        }

        internal static void GreetUser()
        {
            Console.WriteLine("Greetings! This app will allow you export MusicBrainz DB tables into JSON files. Here are the tables:");
        }

        internal static void FilesFound()
        {
            Console.WriteLine($"Here are the JSON files we've found in your local Import folder:");
        }

        internal static void PagingDisabled()
        {
            Console.WriteLine("Paging will be disabled.");
        }

        internal static void PagingEnabled(int recordsPerPage, int pageNumber)
        {
            Console.WriteLine($"The pagination will be enabled. Records per page: {recordsPerPage}. Page number is {pageNumber}.");
        }

        internal static void PrintException(UserFriendlyException exception)
        {
            Console.WriteLine(exception.Message);
        }

        internal static void SayGoodbye()
        {
            Console.WriteLine("Thank you for using our app. Have a good day.");
            //Console.ReadKey();
        }

        internal static void ValuesNotInRange()
        {
            Console.WriteLine("Entered values were not in the defined range. Please enter valid value(s) and try again.");
        }

        internal static void WrongFormat()
        {
            Console.Write("Incorrent data. Please enter a valid table name(s) and try once again: ");
        }

        internal static void WrongInputData()
        {
            Console.WriteLine("The input data is wrong.");
        }

        internal static void NoImportFilesFound()
        {
            Console.WriteLine(
                "Unfortunately, no JSON import files were found " +
                "(Area.json, Artist.json, Label.Json, Place.json, Recording.json, Release.json, ReleaseGroup.json, Url.json, Work.json). " +
                "Please create them first, then try to use our app once again.");
        }
    }
}