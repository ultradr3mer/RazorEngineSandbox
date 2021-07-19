namespace RazorEngineSandbox.ViewModels
{
  using Microsoft.Practices.Unity;
  using Prism.Events;
  using Prism.Mvvm;
  using RazorEngineSandbox.Data;
  using RazorEngineSandbox.Events;
  using RazorEngineSandbox.Properties;
  using RazorEngineSandbox.Services;
  using System.ComponentModel;

  internal class MainWindowViewModel : BindableBase
  {
    #region Fields

    private readonly TemplateService templateService;

    #endregion Fields

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="MainWindowViewModel" /> class.</summary>
    public MainWindowViewModel()
    {
      this.PropertyChanged += this.MainWindowViewModelPropertyChanged;

      var container = ContainerFactory.Create();
      this.templateService = container.Resolve<TemplateService>();

      var eventAggregator = container.Resolve<IEventAggregator>();
      eventAggregator.GetEvent<ParseCompleted>().Subscribe(this.HandleParseCompletedEvent, ThreadOption.UIThread);

      this.Input = "new { Name = \"World\" }";
      this.Template = Resources.Template;
    }

    #endregion Constructors

    #region Properties

    /// <summary>Gets or sets the code diagnostics info.</summary>
    public string CodeDiagnostics { get; set; }

    /// <summary>Gets or sets the input.</summary>
    public string Input { get; set; }

    public string Output { get; set; }

    /// <summary>Gets or sets the result.</summary>
    public string Result { get; set; }

    /// <summary>Gets or sets the template.</summary>
    public string Template { get; set; }

    /// <summary>Gets or sets the templateDiagnostics.</summary>
    public string TemplateDiagnostics { get; set; }

    #endregion Properties

    #region Methods

    /// <summary>Handles the parse completed event.</summary>
    /// <param name="result">The result data.</param>
    private void HandleParseCompletedEvent(ParseResult result)
    {
      this.CodeDiagnostics = result.CodeDiagnostics;
      this.TemplateDiagnostics = result.TemplateDiagnostics;
      this.Result = result.Value;
    }

    /// <summary>Handles the PropertyChanged event of the MainWindowViewModel.</summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
    private void MainWindowViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(MainWindowViewModel.Input) || e.PropertyName == nameof(MainWindowViewModel.Template))
      {
        this.templateService.Parse(this.Input, this.Template);
      }
    }

    #endregion Methods
  }
}