using MusicBrainz.Common.Enums;
using MusicBrainz.Common.TableModels;

namespace MusicBrainz.ConsoleUI.Interactive
{
    internal static class DataPresenter
    {
        private const int _defaultPadding = 16;

        /// <summary>
        /// Shows files info in a user friendly format
        /// </summary>
        /// <param name="tablesInfo"></param>
        public static void ShowFilesInfo(IList<FileInfo> files)
        {
            Console.WriteLine("#".PadRight(_defaultPadding) + "File name".PadRight(_defaultPadding) + "Size".PadRight(_defaultPadding));

            Console.WriteLine("".PadRight(_defaultPadding * 3, '='));

            // printing files info
            for (int i = 0; i < files.Count; i++)
            {
                string length = string.Empty;
                if (files [i].Length >= 1 << 10)
                    length = string.Format("{0}Kb", files [i].Length >> 10);
                else
                    length = $"{files [i].Length}b";

                Console.WriteLine($"{i + 1}".PadRight(_defaultPadding) + files [i].Name.PadRight(_defaultPadding) + length.PadRight(_defaultPadding));
            }

            Console.WriteLine("".PadRight(_defaultPadding * 3, '='));
        }

        /// <summary>
        /// Shows reports description in a user-friendly (kind of) format
        /// </summary>
        public static void ShowReportsInfo()
        {
            Console.WriteLine("#".PadRight(_defaultPadding) + "Report description".PadRight(_defaultPadding));

            Console.WriteLine("".PadRight(_defaultPadding * 3, '='));

            var reports =
            $"{(int) Report.PlacesInArea}".PadRight(_defaultPadding) + "Number of Places in each Area.\n".PadRight(_defaultPadding) +
            $"{(int) Report.NumberOfArtistsWithAreaEnded}".PadRight(_defaultPadding) + "Number of Artists, whose Area has already 'ended'.\n".PadRight(_defaultPadding) +
            $"{(int) Report.ReleaseGroups_ReleasesAvgEditsPending}".PadRight(_defaultPadding) + "Average number of Releases's edits pending for each Release Group.\n".PadRight(_defaultPadding);

            Console.WriteLine(reports);

            Console.WriteLine("".PadRight(_defaultPadding * 3, '='));
        }

        /// <summary>
        /// Shows tables info in a user friendly format
        /// </summary>
        /// <param name="tablesInfo"></param>
        public static void ShowTablesInfo(IList<ITableInfo> tablesInfo)
        {
            Console.WriteLine("#".PadRight(_defaultPadding) + "Table name".PadRight(_defaultPadding) + "Number of records".PadRight(_defaultPadding));

            Console.WriteLine("".PadRight(_defaultPadding * 3, '='));

            foreach (ITableInfo tableInfo in tablesInfo)
            {
                Console.WriteLine($"{(int) tableInfo.Name,-_defaultPadding}{tableInfo.Name,-_defaultPadding}{tableInfo.NumberOfRecords,-_defaultPadding}");
            }

            Console.WriteLine("".PadRight(_defaultPadding * 3, '='));
        }
    }
}
