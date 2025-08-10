
using System.Reflection.Metadata.Ecma335;
using System.Windows.Input;

//lives in global so it can be used throughout project
public class SimpleCommand : ICommand
{
	readonly Action<object?> _execute;
	//nullable predicate, canExecute isn't always going to be defined.
	readonly Predicate<object?>? _canExecute;

	#region Constructors 
	public SimpleCommand(Action<object?> execute) : this(execute, null) { }
	public SimpleCommand(Action<object?> execute, Predicate<object?>? canExecute)
	{
		if (execute == null)
			throw new ArgumentNullException("execute");
		_execute = execute;
		_canExecute = canExecute ?? null;

	}
	#endregion // Constructors 

	#region ICommand Members 
	public bool CanExecute(object? parameter)
	{
		return _canExecute == null || _canExecute(parameter ?? null);
	}
	public event EventHandler? CanExecuteChanged
	{
		add { CommandManager.RequerySuggested += value; }
		remove { CommandManager.RequerySuggested -= value; }
	}
	public void Execute(object? parameter) { _execute(parameter ?? null); }

	#endregion // ICommand Members 
}

