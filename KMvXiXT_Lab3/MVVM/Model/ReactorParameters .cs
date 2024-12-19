using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace KMvXiXT_Lab3.MVVM.Model
{
    public class ReactorParameters : INotifyPropertyChanged, IDataErrorInfo
	{
		private double _flowRate;
		private double _temperature;
		private double _totalTimeMinutes;
		private double _stepMinutes;

		public double FlowRate
		{
			get => _flowRate;
			set
			{
				if (_flowRate != value)
				{
					_flowRate = value;
					OnPropertyChanged(nameof(FlowRate));
				}
			}
		}

		public double Temperature
		{
			get => _temperature;
			set
			{
				if (_temperature != value)
				{
					_temperature = value;
					OnPropertyChanged(nameof(Temperature));
				}
			}
		}
		
		public double TotalTimeMinutes
		{
			get => _totalTimeMinutes;
			set
			{
				if (_totalTimeMinutes != value)
				{
					_totalTimeMinutes = value;
					OnPropertyChanged(nameof(TotalTimeMinutes));
				}
			}
		}
		
		public double StepMinutes
		{
			get => _stepMinutes;
			set
			{
				if (_stepMinutes != value)
				{
					_stepMinutes = value;
					OnPropertyChanged(nameof(StepMinutes));
				}
			}
		}
		
		public string Error => null;

		public string this[string columnName]
		{
			get
			{
				if (columnName == nameof(FlowRate) && FlowRate < 0)
				{
					return "Производительность не может быть отрицательной.";
				}
				if (columnName == nameof(TotalTimeMinutes) && TotalTimeMinutes < 0)
				{
					return "Время выполнения не может быть отрицательным.";
				}
				if (columnName == nameof(StepMinutes) && StepMinutes < 0)
				{
					return "Шаг не может быть отрицательным.";
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
