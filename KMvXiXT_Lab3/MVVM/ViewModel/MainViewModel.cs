using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using ClosedXML.Excel;
using KMvXiXT_Lab3.MVVM.Core;
using KMvXiXT_Lab3.MVVM.Model;
using KMvXiXT_Lab3.MVVM.Utils;
using KMvXiXT_Lab3.MVVM.View;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Win32;
using SkiaSharp;
using Path = System.IO.Path;

namespace KMvXiXT_Lab3.MVVM.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
		private Stopwatch _stopwatch;
		private long _memoryBefore;
		private long _memoryAfter;
		TableWindow tableWindow;
		
		private ObservableCollection<Component_kmx> _components;
		public ObservableCollection<Component_kmx> Components
		{
			get => _components;
			set => SetProperty(ref _components, value, nameof(Components));
		}
		
		private ObservableCollection<Reaction> _reactions;
		public ObservableCollection<Reaction> Reactions
		{
			get => _reactions;
			set => SetProperty(ref _reactions, value, nameof(Reactions));
		}
		
		private ReactorParameters _reactorParameters;
		public ReactorParameters ReactorParameters
		{
			get => _reactorParameters;
			set => SetProperty(ref _reactorParameters, value, nameof(ReactorParameters));
		}
		
		private ObservableCollection<double[]> _results;
		public ObservableCollection<double[]> Results
		{
			get => _results;
			set => SetProperty(ref _results, value, nameof(Results));
		}
		public ObservableCollection<ISeries> Series { get; set; }

		public Axis[] XAxes { get; set; }
		public Axis[] YAxes { get; set; }
		
		private string _executionTime;
		public string ExecutionTime
		{
			/*
			get => _executionTime;
			set
			{
				_executionTime = value;
				OnPropertyChanged(); 
			}
			*/
			get => _executionTime;
			set => SetProperty(ref _executionTime, value, nameof(ExecutionTime));
		}

		private string _memoryUsage;
		public string MemoryUsage
		{
			get => _executionTime;
			set => SetProperty(ref _memoryUsage, value, nameof(MemoryUsage));
		}

		#region ICommands

		public RelayCommand? _calculateCommand { get; set; }
		public RelayCommand CalculateCommand
	    {
		    get
		    {
			    return _calculateCommand ??= new RelayCommand(obj =>
			    {
				    try
				    {
					    
					    _stopwatch = Stopwatch.StartNew();
					    var results = ReactionCalculator.CalculateConcentrations(Components.ToList(), Reactions.ToList(), ReactorParameters);
					    _stopwatch.Stop();
					    ExecutionTime = "Время вычисления: " + _stopwatch.ElapsedMilliseconds.ToString();
					    MemoryUsage = "Использовано памяти: " + (Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024)).ToString();
					    

					    Results.Clear();
					    foreach (var res in results)
						    Results.Add(res);
					    GenerateGraph();
				    }
				    catch (Exception ex)
				    {
					    MessageBox.Show($"Ошибка при расчете: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				    }
			    });
		    }

	    }

	    public RelayCommand? _addReactionCommand { get; set; }
	    public RelayCommand AddReactionCommand
	    {
		    get
		    {
			    return _addReactionCommand ?? (_addReactionCommand = new RelayCommand(obj =>
			           {
				           int newNumber = Reactions.Count + 1;
				           Reactions.Add(new Reaction
				           {
					           Number = newNumber,
					           Formula = $"",
					           ActivationEnergy = 1000000,
					           PreExponentialFactor = 1e12
				           });
			           }));
		    }
	    }
	    public RelayCommand? _removeReactionCommand { get; set; }
		public RelayCommand RemoveReactionCommand
	    {
		    get
		    {
			    return _removeReactionCommand ??
			           (_removeReactionCommand = new RelayCommand(obj =>
			           {
				           if (Reactions.Count > 0)
				           {
					           Reactions.RemoveAt(Reactions.Count - 1);
					           
					           for (int i = 0; i < Reactions.Count; i++)
					           {
						           Reactions[i].Number = i + 1;
					           }
				           }
			           }));
		    }
	    }
	    public RelayCommand? _exportToExcelCommand { get; set; }
	    public RelayCommand ExportToExcelCommand
	    {
		    get
		    {
			    return _exportToExcelCommand ??= new RelayCommand(obj =>
			    {
				    var saveFileDialog = new SaveFileDialog
				    {
					    Filter = "Excel Workbook (*.xlsx)|*.xlsx",
					    Title = "Сохранить данные в Excel",
					    FileName = "Data.xlsx"
				    };

				    if (saveFileDialog.ShowDialog() == true)
				    {
					    try
					    {
						    var workbook = new XLWorkbook();
						    var worksheet = workbook.AddWorksheet("Итоги");

						    int row = 1;
						    
						    worksheet.Cell(row, 1).Value = "Реакции";
						    worksheet.Cell(row, 1).Style.Font.Bold = true;
						    row++;
						    
						    foreach (var reaction in Reactions)
						    {
							    worksheet.Cell(row, 1).Value = reaction.Formula;
							    worksheet.Cell(row, 2).Value = "Энергия активации (Дж/моль)";
							    worksheet.Cell(row, 2).Style.Font.Bold = true;
							    worksheet.Cell(row, 3).Value = reaction.ActivationEnergy;
							    worksheet.Cell(row, 4).Value = "Предэкспоненциальный множитель (1/min)";
							    worksheet.Cell(row, 4).Style.Font.Bold = true;
							    worksheet.Cell(row, 5).Value = reaction.PreExponentialFactor;
							    row++;
						    }
						    row++;
						    
						    worksheet.Cell(row, 1).Value = "Параметры";
						    worksheet.Cell(row, 1).Style.Font.Bold = true;
						    row++;

						    worksheet.Cell(row, 1).Value = "Производительность (л/мин)";
						    worksheet.Cell(row, 2).Value = ReactorParameters.FlowRate;
						    row++;

						    worksheet.Cell(row, 1).Value = "Температура (°C)";
						    worksheet.Cell(row, 2).Value = ReactorParameters.Temperature;
						    row++;

						    worksheet.Cell(row, 1).Value = "Время выполнения (мин)";
						    worksheet.Cell(row, 2).Value = ReactorParameters.TotalTimeMinutes;
						    row++;

						    worksheet.Cell(row, 1).Value = "Шаг (мин)";
						    worksheet.Cell(row, 2).Value = ReactorParameters.StepMinutes;
						    row++;

						    row++; 
						    worksheet.Cell(row, 1).Value = "Концентрация";
						    worksheet.Cell(row, 1).Style.Font.Bold = true;
						    row++;
						    
						    worksheet.Cell(row, 1).Value = "Время (мин)";
						    int col = 2;
						    foreach (var component in Components)
						    {
							    worksheet.Cell(row, col).Value = component.Name;
							    col++;
						    }

						    row++;
						    
						    double stepMinutes = ReactorParameters.StepMinutes;
						    for (int i = 0; i < Results.Count; i++)
						    {
							    worksheet.Cell(row, 1).Value = (i * stepMinutes).ToString("F2");
							    for (int j = 0; j < Components.Count; j++)
							    {
								    worksheet.Cell(row, j + 2).Value = Results[i][j].ToString("F3");
							    }

							    row++;
						    }
						    
						    workbook.SaveAs(saveFileDialog.FileName);

						    MessageBox.Show($"Данные успешно экспортированы в файл: {saveFileDialog.FileName}",
							    "Экспорт завершён",
							    MessageBoxButton.OK,
							    MessageBoxImage.Information);
					    }
					    catch (Exception ex)
					    {
						    MessageBox.Show($"Ошибка при экспорте данных: {ex.Message}", "Ошибка", MessageBoxButton.OK,
							    MessageBoxImage.Error);
					    }
				    }
			    });
		    }
	    }

		public RelayCommand? _addComponentCommand { get; set; }

		public RelayCommand AddComponentCommand
		{
			get
			{
				return _addComponentCommand ??= new RelayCommand(obj =>
				{
					try
					{
						char newName = (char)('A' + Components.Count);
						Components.Add(new Component_kmx { Name = newName.ToString(), InitialConcentration = 0.0 });
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Ошибка при добавление компонента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				});
			}
		}
		
		public RelayCommand? _removeComponentCommand { get; set; }

		public RelayCommand RemoveComponentCommand
		{
			get
			{
				return _removeComponentCommand ??= new RelayCommand(obj =>
				{
					if (Components.Count > 0)
						Components.RemoveAt(Components.Count - 1);
				});
			}
		}
	    
		public RelayCommand? _showTableCommand{ get; set; }

		public RelayCommand ShowTableCommand
		{
			get
			{
				return _showTableCommand ??= new RelayCommand(obj =>
				{
					try
					{
						if (Results == null || Results.Count == 0)
						{
							MessageBox.Show("Результаты отсутствуют. Сначала выполните расчёт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
							return;
						}

						if (Components == null || Components.Count == 0)
						{
							MessageBox.Show("Компоненты отсутствуют. Добавьте компоненты перед выполнением расчёта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
							return;
						}
						
						tableWindow = new TableWindow(Results.ToList<double[]>(), Components, ReactorParameters);
						tableWindow.Show();
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Ошибка при открытии таблицы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				});
			}
		}
		
		#endregion
		
		public MainViewModel()
	    {
		    _stopwatch = new Stopwatch();
		    tableWindow = new TableWindow();
		    
		    Series = new ObservableCollection<ISeries>();
		    
		    Components = new ObservableCollection<Component_kmx>
		    {
			    new Component_kmx { Name = "A", InitialConcentration = 0.9 },
			    new Component_kmx { Name = "B", InitialConcentration = 0.0 },
			    new Component_kmx { Name = "C", InitialConcentration = 0.0 },
			    new Component_kmx { Name = "D", InitialConcentration = 0.0 },
			    new Component_kmx { Name = "E", InitialConcentration = 0.0 }
		    };
			Reactions = new ObservableCollection<Reaction>
			{
				new Reaction { Number = 1, Formula = "A > 2B + C", ActivationEnergy = 74000, PreExponentialFactor = 0.20e+014 },
				new Reaction { Number = 2, Formula = "B > D + E", ActivationEnergy = 89000, PreExponentialFactor = 9.0e+015 },
				new Reaction { Number = 3, Formula = "2B + C > A", ActivationEnergy = 85000, PreExponentialFactor = 0.5e+014 }
			};
			ReactorParameters = new ReactorParameters
		    {
			    FlowRate = 15,
			    Temperature = -7,
			    TotalTimeMinutes = 50,
			    StepMinutes = 1
		    };
		    Results = new ObservableCollection<double[]>();
		    
		    XAxes = new Axis[]
		    {
			    new Axis
			    {
				    Name = "Время (мин)",
				    Labeler = value => value.ToString("F2") 
			    }
		    };
		    YAxes = new Axis[]
		    {
			    new Axis
			    {
				    Name = "Концентрация (моль/л)",
				    Labeler = value => value.ToString("F4")
			    }
		    };
		}
		
		private void GenerateGraph()
		{
			Series.Clear();
			
			double totalTimeMinutes = ReactorParameters.TotalTimeMinutes;
			double stepMinutes = ReactorParameters.StepMinutes;
			int nPoints = (int)(totalTimeMinutes / stepMinutes) + 1;

			double[] times = Enumerable.Range(0, nPoints).Select(i => i * stepMinutes).ToArray(); // В минутах

			double[][] concentrations = Results.Select(r => r.ToArray()).ToArray();
			
			for (int i = 0; i < Components.Count; i++)
			{
				double[] componentConcentrations = concentrations.Select(row => row[i]).ToArray();
        
				var values = new ObservableCollection<ObservablePoint>();
				for (int k = 0; k < times.Length; k++)
				{
					values.Add(new ObservablePoint(times[k], componentConcentrations[k]));
				}
				
				var lineSeries = new LineSeries<ObservablePoint>
				{
					Values = values,
					Name = $"Компонент {Components[i].Name}",
					Fill = null, 
					Stroke = new SolidColorPaint(SKColors.Black, 3)
				};

				Series.Add(lineSeries);
			}
			
		}
		
		
	    public event PropertyChangedEventHandler? PropertyChanged;
	    public void OnPropertyChanged([CallerMemberName] string prop = "")
	    {
		    if (PropertyChanged != null)
			    PropertyChanged(this, new PropertyChangedEventArgs(prop));
	    }
	}
}
