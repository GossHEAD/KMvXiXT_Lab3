using System.Collections.ObjectModel;
using System.Data;
using KMvXiXT_Lab3.MVVM.Core;
using KMvXiXT_Lab3.MVVM.Model;

namespace KMvXiXT_Lab3.MVVM.ViewModel;

public class TableViewModel : ViewModelBase
{
    public DataTable TableData { get; set; }
    public TableViewModel(List<double[]> results, ObservableCollection<Component_kmx> components, ReactorParameters parameters )
    {
        TableData = new DataTable();

        TableData.Columns.Add("Время (мин)", typeof(string)); 
        foreach (var component in components)
        {
            TableData.Columns.Add(component.Name, typeof(string)); 
        }
        
        double stepMinutes = parameters.StepMinutes; // Шаг моделирования
        for (int i = 0; i < results.Count; i++)
        {
            var row = TableData.NewRow();
            row[0] = (i * stepMinutes).ToString("F2"); // Время
            for (int j = 0; j < components.Count; j++)
            {
                row[j + 1] = results[i][j].ToString("F3"); // Концентрации компонентов
            }
            TableData.Rows.Add(row);
        }
    }
}

