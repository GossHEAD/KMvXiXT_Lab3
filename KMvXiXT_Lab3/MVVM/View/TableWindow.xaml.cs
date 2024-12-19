using System.Collections.ObjectModel;
using System.Windows;
using KMvXiXT_Lab3.MVVM.Model;
using KMvXiXT_Lab3.MVVM.ViewModel;

namespace KMvXiXT_Lab3.MVVM.View
{
    public partial class TableWindow : Window
    {
        public TableWindow()
        {
            InitializeComponent();
        }
        public TableWindow(List<double[]> results, ObservableCollection<Component_kmx> components, ReactorParameters reactorParameters)
        {
            InitializeComponent();
            DataContext = new TableViewModel(results, components, reactorParameters);
        }
        
    }
}

