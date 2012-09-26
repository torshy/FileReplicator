using System.Windows;
using System.Windows.Controls;

namespace TRock.FileReplicator.Controls
{
    public class CommandBarContextMenu : ContextMenu
    {
        static CommandBarContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CommandBarContextMenu), new FrameworkPropertyMetadata(typeof(CommandBarContextMenu)));
        }
    }
}