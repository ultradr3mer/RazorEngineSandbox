namespace RazorEngineSandbox.Views
{
  using System.ComponentModel;
  using System.Windows;

  using RazorEngineSandbox.ViewModels;

  /// <summary>Interaction logic for MainWindow.xaml</summary>
  public partial class MainWindow : Window
  {
    #region Constructors

    public MainWindow()
    {
      this.InitializeComponent();

      this.ViewModel.PropertyChanged += this.ViewModelPropertyChanged;
    }

    #endregion

    #region Properties

    /// <summary>Gets the view model which is the data context of this view.</summary>
    private MainWindowViewModel ViewModel
    {
      get { return this.DataContext as MainWindowViewModel; }
    }

    #endregion

    #region Methods

    /// <summary>Handles the PropertyChanged event of the ViewModel.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
    private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(MainWindowViewModel.Result))
      {
        string content = this.ViewModel.Result;
        if (string.IsNullOrWhiteSpace(content))
        {
          content = "<body/>";
        }

        this.WebBrowser.NavigateToString(content);
      }
    }

    #endregion
  }
}