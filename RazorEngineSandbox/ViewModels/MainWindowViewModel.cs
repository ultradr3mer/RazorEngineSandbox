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
  using System.Windows;

  internal class MainWindowViewModel : BindableBase
  {
    #region Fields

    private readonly TemplateService templateService;

    /// <summary>The <see cref="Code" /> property's value.</summary>
    private string propCode;

    /// <summary>The <see cref="CodeDiagnostics" /> property's value.</summary>
    private string propCodeDiagnostics;

    private Visibility propHtmlResultVisibility;
    private bool propIsShowHtml;

    /// <summary>The <see cref="Result" /> property's value.</summary>
    private string propResult;

    /// <summary>The <see cref="Template" /> property's value.</summary>
    private string propTemplate;

    /// <summary>The <see cref="TemplateDiagnostics" /> property's value.</summary>
    private string propTemplateDiagnostics;

    private Visibility propTextResultVisibility;

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

      this.Code = "new { Name = \"World\" }";
      this.Template = Resources.Template;
    }

    #endregion Constructors

    #region Properties

    /// <summary>Gets or sets the code.</summary>
    public string Code
    {
      get { return this.propCode; }
      set { this.SetProperty(ref this.propCode, value); }
    }

    /// <summary>Gets or sets the code diagnostics info.</summary>
    public string CodeDiagnostics
    {
      get { return this.propCodeDiagnostics; }
      set { this.SetProperty(ref this.propCodeDiagnostics, value); }
    }

    public Visibility HtmlResultVisibility
    {
      get { return propHtmlResultVisibility; }
      set { SetProperty(ref propHtmlResultVisibility, value); }
    }

    public bool IsShowHtml
    {
      get { return propIsShowHtml; }
      set { SetProperty(ref propIsShowHtml, value); }
    }

    /// <summary>Gets or sets the result.</summary>
    public string Result
    {
      get { return this.propResult; }
      set { this.SetProperty(ref this.propResult, value); }
    }

    /// <summary>Gets or sets the template.</summary>
    public string Template
    {
      get { return this.propTemplate; }
      set { this.SetProperty(ref this.propTemplate, value); }
    }

    /// <summary>Gets or sets the templateDiagnostics.</summary>
    public string TemplateDiagnostics
    {
      get { return this.propTemplateDiagnostics; }
      set { this.SetProperty(ref this.propTemplateDiagnostics, value); }
    }

    public Visibility TextResultVisibility
    {
      get { return propTextResultVisibility; }
      set { SetProperty(ref propTextResultVisibility, value); }
    }

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
      if (e.PropertyName == nameof(MainWindowViewModel.Code) || e.PropertyName == nameof(MainWindowViewModel.Template))
      {
        this.templateService.Parse(this.Code, this.Template);
      }

      if (e.PropertyName == nameof(MainWindowViewModel.IsShowHtml))
      {
        this.HtmlResultVisibility = this.IsShowHtml ? Visibility.Visible : Visibility.Collapsed;
        this.TextResultVisibility = this.IsShowHtml ? Visibility.Collapsed : Visibility.Visible;
      }
    }

    #endregion Methods
  }
}