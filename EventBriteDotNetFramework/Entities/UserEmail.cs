namespace EventBriteDotNetFramework.Entities
{
  public class UserEmail
  {
    /// <summary>
    ///   The email address.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///   If this email address has been verified to belong to the user.
    /// </summary>
    public bool Verified { get; set; }

    /// <summary>
    ///   If this email address is the primary one for the account.
    /// </summary>
    public bool Primary { get; set; }
  }
}