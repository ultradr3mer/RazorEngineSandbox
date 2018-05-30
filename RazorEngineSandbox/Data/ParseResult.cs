namespace RazorEngineSandbox.Data
{
  /// <summary>Represents a parse result.</summary>
  internal class ParseResult
  {
    #region Properties

    /// <summary>Gets or sets the code diagnostics.</summary>
    public string CodeDiagnostics { get; set; }

    /// <summary>Gets or sets the model.</summary>
    public object Model { get; set; }

    /// <summary>Gets or sets the template diagnostics.</summary>
    public string TemplateDiagnostics { get; set; }

    /// <summary>Gets or sets the value.</summary>
    public string Value { get; set; }

    #endregion
  }
}