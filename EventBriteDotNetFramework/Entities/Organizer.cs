
namespace EventBriteDotNetFramework.Entities
{
  public class Organizer
  {
    /// <summary>
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///   The organizer’s name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///   The URL to the organizer’s page on Eventbrite.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// </summary>
    public int NumberOfPastEvents { get; set; }

    /// <summary>
    /// </summary>
    public int NumberOfFutureEvents { get; set; }
  }
}