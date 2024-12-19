using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KMvXiXT_Lab3.MVVM.ViewModel;

namespace KMvXiXT_Lab3.MVVM.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		readonly private Stopwatch _stopwatch;
		private MainViewModel viewModel = new MainViewModel();
		public MainWindow()
		{
			InitializeComponent();
			DataContext = viewModel;
			_stopwatch = new Stopwatch();
			UpdateMemoryUsage();
			UpdateTimeElapsed();
		}
		private void UpdateMemoryUsage()
		{
			var memoryUsage = Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024); // MB
			MemoryUsageText.Header = $"Использовано памяти: {memoryUsage} MB";
		}
		
		private void UpdateTimeElapsed()
		{
			var elapsedTime = _stopwatch.Elapsed.TotalMilliseconds;
			CalculationTimeText.Header = $"Время расчета: {elapsedTime:F2}мс";
		}

		private void OnCalculateBtnClick(object sender, RoutedEventArgs e)
		{
			_stopwatch.Start();
			Timer_Tick();
			_stopwatch.Stop();
			_stopwatch.Reset();
		}
		
		private void Timer_Tick()
		{
			UpdateMemoryUsage();
			UpdateTimeElapsed();
		}
		
		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			string allowedChars = "0123456789Ee.+-";
			if (!allowedChars.Contains(e.Text))
			{
				e.Handled = true; 
				return;
			}
			var textBox = sender as TextBox;
			if (textBox == null) return;
			string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
			if (!IsValidNumber(fullText))
			{
				e.Handled = true;
			}
		}

		private bool IsValidNumber(string text)
		{
			if (string.IsNullOrEmpty(text))
				return false;
			return double.TryParse(
				text,
				System.Globalization.NumberStyles.Float,
				System.Globalization.CultureInfo.InvariantCulture,
				out _);
		}

	}
}