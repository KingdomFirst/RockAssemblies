using System.Collections.Generic;

namespace EventBriteDotNetFramework.Entities
{
  public class User
  {
    /// <summary>
    ///   The user's id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///   The user’s name. Use this in preference to first_name/last_name if possible for forward compatibility with
    ///   non-Western names.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///   The user’s first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    ///   The user’s last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    ///   A list of user emails.
    /// </summary>
    public List<UserEmail> Emails { get; set; }
  }
}