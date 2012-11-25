namespace Watchtower
{
    internal struct Constants
    {
        internal struct Configuration
        {
            internal const string DbFileName = @"Configuration.db";
            internal const string ConnectionString = "Version=3,uri=file:Configuration.db";


            internal const string PeriodKey = "Period";
            internal const int PeriodValue = 15;

            internal const string SequentialUpdateKey = "SequentialUpdate";
            internal const bool SequentialUpdateValue = true;
        }
        internal struct Application
        {
            internal const string Title = "Watchtower";
            internal const string Description = "Repository Monitor";
            internal const string BaloonTip = @"Watchtower has been minimised.
Click the tray icon to show.";
        }
    }
}
