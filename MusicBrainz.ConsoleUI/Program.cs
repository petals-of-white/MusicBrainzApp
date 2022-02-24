using MusicBrainz.ConsoleUI;

//MainFlow.GreetUser();

//MainFlow.ShowAllTablesAndNumberOfRecords();

//MainFlow.SelectAction();

//MainFlow.ConfirmResult();

//MainFlow.SayGoodbye();

//Console.ReadKey();

MainUserFlow mainFlow = new();

mainFlow.Start();
mainFlow.ConfigureAction();
mainFlow.Finish();