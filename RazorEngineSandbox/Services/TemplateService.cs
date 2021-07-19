namespace RazorEngineSandbox.Services
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Security;
  using System.Security.Policy;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Web.Razor;

  using Microsoft.CodeAnalysis.CSharp.Scripting;
  using Microsoft.CodeAnalysis.Scripting;

  using Prism.Events;

  using RazorEngine.Templating;

  using RazorEngineSandbox.Data;
  using RazorEngineSandbox.Events;
  using Unity;

  /// <summary>The Template Service for parsing razor templates.</summary>
  internal class TemplateService
  {
    #region Fields

    private readonly IEventAggregator eventAggregator;

    private readonly ConcurrentQueue<ParseRequest> requests = new ConcurrentQueue<ParseRequest>();

    private readonly IRazorEngineService service;

    private bool running;

    #endregion

    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="TemplateService" /> class.</summary>
    /// <param name="container">The container.</param>
    public TemplateService(IUnityContainer container)
    {
      this.eventAggregator = container.Resolve<IEventAggregator>();

      this.service = IsolatedRazorEngineService.Create(TemplateService.SandboxCreator);
    }

    #endregion

    #region Methods

    /// <summary>Parses the specified code and template.</summary>
    /// <param name="code">The code.</param>
    /// <param name="template">The template.</param>
    public void Parse(string code, string template)
    {
      this.requests.Enqueue(new ParseRequest(code, template));

      if (!this.running)
      {
        this.running = true;
        Task.Factory.StartNew(this.ProcessRequests);
      }
    }

    /// <summary>Runs the code.</summary>
    /// <param name="result">The parse result.</param>
    /// <param name="request">The parse request.</param>
    /// <returns>The await handle</returns>
    private static async Task RunCode(ParseResult result, ParseRequest request)
    {
      try
      {
        var scriptOptions = ScriptOptions.Default;
        scriptOptions = scriptOptions.AddReferences(typeof(TemplateService).Assembly);
        scriptOptions = scriptOptions.AddReferences(typeof(Dictionary<string,string>).Assembly);
        scriptOptions = scriptOptions.AddImports("System", "RazorEngineSandbox.Data", "System.Collections.Generic");
        //scriptOptions.all
        result.Model = await CSharpScript.EvaluateAsync(request.Code, scriptOptions);
      }
      catch (CompilationErrorException e)
      {
        result.CodeDiagnostics = e.Message;
      }
    }

    /// <summary>We have to create seperate domain shizzle to prevent hackers from beeing able abuse our templating power. I would like to understand better how this works, just doing as told by this page now: https://antaris.github.io/RazorEngine/Isolation.html </summary>
    /// <returns>An AppDomain that you should use to execute the sandbox stuff.</returns>
    private static AppDomain SandboxCreator()
    {
      var evidence = new Evidence();
      evidence.AddHostEvidence(new Zone(SecurityZone.Internet));
      var permissionSet = SecurityManager.GetStandardSandbox(evidence);

      // You have to sign your assembly in order to get strong names (http://stackoverflow.com/questions/8349573/getting-null-from-gethostevidence)
      // But doing this results in: 
      // Error	1	Assembly generation failed -- Referenced assembly 'RazorEngine' does not have a strong name	D:\git\presence-engine\RazorExperiment\CSC	RazorExperiment
      // Error	2	Assembly generation failed -- Referenced assembly 'Microsoft.AspNet.Razor' does not have a strong name	D:\git\presence-engine\RazorExperiment\CSC	RazorExperiment
      var razorEngineAssembly = typeof(RazorEngineService).Assembly.Evidence.GetHostEvidence<StrongName>();
      var razorAssembly = typeof(RazorTemplateEngine).Assembly.Evidence.GetHostEvidence<StrongName>();

      var appDomainSetup = new AppDomainSetup();
      appDomainSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

      var result = AppDomain.CreateDomain("Sandbox", null, appDomainSetup, permissionSet, razorEngineAssembly, razorAssembly);
      return result;
    }

    /// <summary>Dequeues all but the latest.</summary>
    private void DequeueAllButTheLatest()
    {
      while (this.requests.Count > 1)
      {
        this.requests.TryDequeue(out _);
      }
    }

    /// <summary>Processes the requests.</summary>
    private async void ProcessRequests()
    {
      while (this.running)
      {
        this.DequeueAllButTheLatest();

        if (!this.requests.TryDequeue(out var request))
        {
          continue;
        }

        var result = new ParseResult();

        await TemplateService.RunCode(result, request);
        this.RunTemplate(result, request);

        this.eventAggregator.GetEvent<ParseCompleted>().Publish(result);

        Thread.Sleep(500);
      }
    }

    /// <summary>Runs the template.</summary>
    /// <param name="result">The parse result.</param>
    /// <param name="request">The parse request.</param>
    private void RunTemplate(ParseResult result, ParseRequest request)
    {
      if (result.Model != null)
      {
        try
        {
          Type modelType = result.Model.GetType() == typeof(Bestellung) ? typeof(Bestellung) : null;
          var name = request.Template.GetHashCode().ToString("X");
          this.service.Compile(request.Template, name, modelType);
          result.Value = this.service.Run(name, modelType, result.Model);
        }
        catch (Exception e)
        {
            result.TemplateDiagnostics = e.Message;
        }
      }
    }

    #endregion
  }
}