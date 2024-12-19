using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;


namespace KMvXiXT_Lab3.MVVM.Model 
{
    public class Reaction : INotifyPropertyChanged, IDataErrorInfo
	{
		private int _number;
		private string _element;
		private string _formula;
		private double _activationEnergy;
		private double _preExponentialFactor;

		public int Number
		{
			get => _number;
			set
			{
				if (_number != value)
				{
					_number = value;
					OnPropertyChanged(nameof(Number));
				}
			}
		}

		public string Formula
		{
			get => _formula;
			set
			{
				if (_formula != value)
				{
					_formula = value;
					OnPropertyChanged(nameof(Formula));
				}
			}
		}

		public double ActivationEnergy
		{
			get => _activationEnergy;
			set
			{
				if (_activationEnergy != value)
				{
					_activationEnergy = value;
					OnPropertyChanged(nameof(ActivationEnergy));
				}
			}
		}

		public double PreExponentialFactor
		{
			get => _preExponentialFactor;
			set
			{
				if (_preExponentialFactor != value)
				{
					_preExponentialFactor = value;
					OnPropertyChanged(nameof(PreExponentialFactor));
				}
			}
		}
		public string Error => null;

		public string this[string columnName]
		{
			get
			{
				if (columnName == nameof(ActivationEnergy) && ActivationEnergy < 0)
				{
					return "Энергия активации не может быть отрицательной.";
				}
				if (columnName == nameof(PreExponentialFactor) && PreExponentialFactor <= 0 || PreExponentialFactor > 1e20)
				{
					return "Предэкспоненциальный множитель не может быть отрицательной.";
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
