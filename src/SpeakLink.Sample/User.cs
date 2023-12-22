namespace SpeakLink.Sample;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string FullName => FirstName + " " + LastName;

    public string ImageUrl => $"https://picsum.photos/id/{Id}/64/64";
}