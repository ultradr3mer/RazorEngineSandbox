namespace RazorEngineSandbox.Events
{
  using Prism.Events;

  using RazorEngineSandbox.Data;

  /// <summary>The event which occurs when the parsing is completed.</summary>
  /// <seealso cref="Prism.Events.PubSubEvent{RazorEngineSandbox.Data.ParseResult}" />
  internal class ParseCompleted : PubSubEvent<ParseResult>
  {
  }
}