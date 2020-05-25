namespace Magitek.Utilities.CombatMessages
{
    //See CombatMessageManager.RegisterMessageStrategiesFoClass() for details on how to show combat
    //messages in the overlay
    internal interface ICombatMessageStrategy
    {
        //Strategy priority. Priorities with higher strategies (lower values) are checked first
        int Priority { get; }

        //Message to display if ShowMessage() is true
        string Message { get; }

        //Image to display if ShowMessage() is true. To show no image, use an empty string.
        //Image sources should be strings of the form:
        //   "pack://application:,,,/Magitek;component/Resources/Images/Monk/ArrowDownHighlighted.png"
        //Everything up "Magitek;component/" needs to stay the same. After that, it's just the path from the project root and the filename.
        //Files must be added to the project as resources, with 'Build Action' set to "Resource" and 'Copy to Output Directory' set to "Do not copy"
        string ImageSource { get; }

        //Test to see if message should be displayed. The combat manager checks strategies in priority
        //order, and the first one for which ShowMessage() returns true is displayed
        bool ShowMessage();
    }
}
