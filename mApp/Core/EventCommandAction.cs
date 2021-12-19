using Microsoft.Xaml.Behaviors;

namespace mApp.Core;
public class EventCommandAction : TriggerAction<DependencyObject>
{
    public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
      nameof(Command),
      typeof(ICommand),
      typeof(EventCommandAction),
      null);
    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    protected override void Invoke(object parameter)
    {
        if (Command != null && Command.CanExecute(parameter))
            Command.Execute(parameter);
    }
}
