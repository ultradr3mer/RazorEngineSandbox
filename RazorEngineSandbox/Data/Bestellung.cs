namespace RazorEngineSandbox.Data
{
  using System;

  [Serializable]
  public class Bestellung
  {
    #region Properties

    public int KundenNummer { get; set; }

    public string Land { get; set; }

    public string Name { get; set; }

    public Position[] Positionen;

    #endregion
  }
}