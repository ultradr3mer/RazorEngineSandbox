namespace RazorEngineSandbox
{
  using Microsoft.Practices.Unity;

  using Prism.Events;

  /// <summary>Factory for creating the base unity container for this application.</summary>
  internal static class ContainerFactory
  {
    #region Methods

    /// <summary>Creates the base unity container for this application.</summary>
    /// <returns>The <see cref="IUnityContainer" />.</returns>
    public static IUnityContainer Create()
    {
      LifetimeManager Lm()
      {
        return new ContainerControlledLifetimeManager();
      }

      var container = new UnityContainer();
      container.RegisterType<IEventAggregator, EventAggregator>(Lm());

      return container;
    }

    #endregion
  }
}