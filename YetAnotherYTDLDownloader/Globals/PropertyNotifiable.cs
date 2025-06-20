using System.ComponentModel;

public class PropertyNotifiable : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	private void NotifyChange(PropertyChangedEventArgs e)
	{
		if (PropertyChanged != null)
		{
			PropertyChanged(this, e);
		}
	}

	protected void Notify(string propertyName)
	{
		NotifyChange(new PropertyChangedEventArgs(propertyName));
	}
}

