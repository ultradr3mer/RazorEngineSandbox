namespace RazorEngineSandbox.Data
{
  /// <summary>Represents a parse request.</summary>
  internal class ParseRequest
  {
    #region Constructors

    /// <summary>Initializes a new instance of the <see cref="ParseRequest" /> class.</summary>
    /// <param name="code">The code.</param>
    /// <param name="template">The template.</param>
    public ParseRequest(string code, string template)
    {
      this.Code = code != null ? string.Copy(code) : string.Empty;
      this.Template = template != null ? string.Copy(template) : string.Empty;
    }

    #endregion

    #region Properties

    /// <summary>Gets the code.</summary>
    public string Code { get; }

    /// <summary>Gets the template.</summary>
    public string Template { get; }

    #endregion
  }
}