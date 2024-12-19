using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace KMvXiXT_Lab3.MVVM.Model
{
    public class Component_kmx : INotifyPropertyChanged, IDataErrorInfo
    {
	    private string _name;
	    private double _initialConcentration;

	    public string Name
	    {
		    get => _name;
		    set
		    {
			    if (_name != value)
			    {
				    _name = value;
				    OnPropertyChanged(nameof(Name));
			    }
		    }
	    }

	    public double InitialConcentration
	    {
		    get => _initialConcentration;
		    set
		    {
			    if (_initialConcentration != value)
			    {
				    _initialConcentration = value;
				    OnPropertyChanged(nameof(InitialConcentration));
			    }
		    }
	    }

	    public string Error => null;

	    public string this[string columnName]
	    {
		    get
		    {
			    if (columnName == nameof(InitialConcentration) && InitialConcentration < 0)
			    {
				    return "Концентрация не может быть отрицательной.";
			    }
			    return null;
		    }
	    }
	    
	    public event PropertyChangedEventHandler PropertyChanged;

	    protected void OnPropertyChanged(string propertyName)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }
	}
}
