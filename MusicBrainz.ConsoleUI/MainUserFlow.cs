using MusicBrainz.BLL.DbEntitySerialization;
using MusicBrainz.BLL.DbEntitySerialization.Serialization;
using MusicBrainz.BLL.Exceptions;
using MusicBrainz.Common.Enums;
using MusicBrainz.Common.TableModels;
using MusicBrainz.ConsoleUI.Interactive;
using MusicBrainz.Tools.Logging;

namespace MusicBrainz.ConsoleUI
{
    internal class MainUserFlow
    {
        private DbExportConfig _exportConfig = new();

        private EntitiesFileManager _fileManager = new();
        private DbImportConfig _importConfig = new();

        private LoggerBase _logger = new FileLoggerFactory("musicbrainz.log").CreateLogger();

        private DbEntitiesSerializer _mainSerializer = new();

        private Mode _mode = Mode.None;

        private Report _report;

        private ISerializationManager _serializationManager = new JsonSerializationManager();

        [Flags]
        internal enum Mode
        {
            None,
            Export,
            Import,
            Report
        }

        #region Main methods

        public void ConfigureAction()
        {
            GeneralMessages.AskForMode();

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
                    ConfigurePaging();

                    break;

                case "r" or "report":
                    _mode = Mode.Report;
                    Console.WriteLine("Reports mode!");

                    SelectReport();

                    break;

                default:
                    _mode = Mode.Export;
                    Console.WriteLine("Export mode!");

                    SelectTablesToExport();
                    ConfigurePaging();

                    break;
            }
        }

        public void DoAction()
        {
            // export
            _mainSerializer.ConfigureExport(_exportConfig);

            // import
            _mainSerializer.ConfigureImport(_importConfig);

            switch (_mode)
            {
                case Mode.Export:
                    Console.WriteLine("Exporting...");

                    (Dictionary<Tables, string> serializedTables, _) = _mainSerializer.SerializeTableEntitiesTypeMapped();

                    foreach (KeyValuePair<Tables, string> serializedTable in serializedTables)
                    {
                        _fileManager.WriteToFile(serializedTable.Key, serializedTable.Value);
                    }

                    break;

                case Mode.Import:
                    Console.WriteLine("Importing...");
                    _mainSerializer.ImportSerializedTableEntitiesNew();
                    break;

                case Mode.Report:
                    Console.WriteLine("Generating a report...");

                    var reportTable = _mainSerializer.GenerateReport(_report);
                    _fileManager.WriteToFile(_report, reportTable);
                    break;
            }

            ExitApplication();
        }

        public void Start()
        {
            GeneralMessages.GreetUser();
            try
            {
                DataPresenter.ShowTablesInfo(_mainSerializer.GetTablesInfo());
            }
            catch (UserFriendlyException ex)
            {
                GeneralMessages.PrintException(ex);
                ExitApplication();
            }
        }

        private void ExitApplication()
        {
            GeneralMessages.SayGoodbye();
            Console.ReadKey();
            Environment.Exit(0);
        }

        #endregion Main methods

        #region Inner methods

        private void ConfigurePaging()
        {
            GeneralMessages.AskForPaging();

            string answer = Console.ReadLine()!;

            switch (answer)
            {
                case "yes" or "y":

                    int pageNumber, recordsPerPage;

                    // Set records per page

                    GeneralMessages.AskForRecordsPerPage();

                    while (int.TryParse(Console.ReadLine(), out recordsPerPage) == false || recordsPerPage < 1)
                    {
                        GeneralMessages.WrongInputData();
                        GeneralMessages.AskForRecordsPerPage();
                    }

                    // Set a page number
                    GeneralMessages.AskForPageNumber();

                    while (int.TryParse(Console.ReadLine(), out pageNumber) == false || pageNumber < 1)
                    {
                        GeneralMessages.WrongInputData();
                        GeneralMessages.AskForPageNumber();
                    }

                    // enables pagination and sets page number
                    _exportConfig.EnablePaging(recordsPerPage, pageNumber);

                    GeneralMessages.PagingEnabled(recordsPerPage, pageNumber);
                    break;

                case "no" or "n":
                    GeneralMessages.PagingDisabled();
                    break;

                default:
                    GeneralMessages.PagingDisabled();
                    break;
            }
        }

        private HashSet<T> SelectMultipleTables<T>(IList<T> choices)
        {
            // output set
            HashSet<T> choicesResult = new();

            // all available table entity enums
            int [] tablesEnums = ((int []) Enum.GetValues(typeof(Tables)));

            string? input;
            bool proceedFurther = false;

            Console.Write("Your choice: ");

            while (proceedFurther == false)
            {
                input = Console.ReadLine();

                if (input == "*")
                {
                    // adding all elements to our set
                    choicesResult.UnionWith(choices);

                    GeneralMessages.AllTablesExportation();

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
                                x => x > 0 && x <= choices.Count);

                            // then add those values to our hashset if true
                            if (tableNumbersInValidRange)
                            {
                                foreach (var tableNumber in tablesNumbers)
                                {
                                    choicesResult.Add(choices [tableNumber - 1]);
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
                            GeneralMessages.WrongFormat();
                            break;
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            _logger.Log(ex.ToString());

                            GeneralMessages.ValuesNotInRange();

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
                            GeneralMessages.ValuesNotInRange();

                            break;
                        }
                    }
                }
            }
            return choicesResult;
        }

        private void SelectReport()
        {
            DataPresenter.ShowReportsInfo();

            GeneralMessages.ExplainReports();

            int intValue;

            while (int.TryParse(Console.ReadLine()!.Trim(), out intValue) && Enum.IsDefined(typeof(Report), intValue) == false)
            {
                GeneralMessages.WrongInputData();
                Console.Write("Please try again: ");
            }
            //selectedReport =
            _report = (Report) intValue;
        }

        private void SelectTablesToExport()
        {
            GeneralMessages.ExplainTableSelection();
            HashSet<ITableInfo> selectedTablesToExport;
            try
            {
                selectedTablesToExport = SelectMultipleTables(_mainSerializer.GetTablesInfo());

                // adding tables to export
                _exportConfig.AddTableToExport(selectedTablesToExport.Select(t => t.Name).ToArray());
            }
            catch (UserFriendlyException ex)
            {
                GeneralMessages.PrintException(ex);
                ExitApplication();
            }
        }

        private void SelectTablesToImport()
        {
            GeneralMessages.ExplainTableSelection();

            HashSet<FileInfo> selectedImportFiles;
            try
            {
                selectedImportFiles = SelectMultipleTables(_fileManager.GetImportFiles());
                foreach (FileInfo importFile in selectedImportFiles)
                {
                    Tables tableName = (Tables) Enum.Parse(typeof(Tables), Path.GetFileNameWithoutExtension(importFile.FullName));

                    string jsonContent = _fileManager.ReadFromFile(importFile);

                    _importConfig.AddEntitiesToImport(tableName, jsonContent);
                }
            }
            catch (UserFriendlyException ex)
            {
                GeneralMessages.PrintException(ex);
                ExitApplication();
            }
        }

        private void ShowAvailableImportFiles()
        {
            GeneralMessages.FilesFound();
            IList<FileInfo> importFiles;
            try
            {
                importFiles = _fileManager.GetImportFiles();

                if (importFiles.Count == 0)
                {
                    GeneralMessages.NoImportFilesFound();
                    ExitApplication();
                }

                DataPresenter.ShowFilesInfo(importFiles);
            }
            catch (UserFriendlyException ex)
            {
                GeneralMessages.PrintException(ex);
                ExitApplication();
            }
        }

        #endregion Inner methods
    }
}
