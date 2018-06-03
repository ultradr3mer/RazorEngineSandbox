namespace RazorEngineSandbox.Data
{
  using System;

  [Serializable]
  public class Position
  {
    #region Properties

    public string Bezeichnung { get; set; }

    public decimal Preis { get; set; }

    #endregion
  }
}