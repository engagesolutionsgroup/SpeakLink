namespace SpeakLink.Sample;

public class MentionService
{
    private readonly List<User> allUsers;

    public MentionService()
    {
        allUsers = GenerateSampleUsers(50);
    }
    
    public List<User> SearchForUsers(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
            return allUsers;
        
        return allUsers.Where(
                u => u.FullName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
            .ToList();
    }
    
    public static List<User> GenerateSampleUsers(int numberOfUsers)
    {
        var users = new List<User>();
        var random = new Random();

        string[] firstNames = { "Artem", "Alice", "Bob", "Charlie", "David", "Emily", "Frank", "Grace", "Henry", "Ivy", "Jack" };
        string[] lastNames = { "Valieiev", "Johnson", "Smith", "Brown", "Williams", "Jones", "Miller", "Davis", "Garcia", "Rodriguez", "Wilson" };

        for (int i = 1; i <= numberOfUsers; i++)
        {
            users.Add(new User
            {
                Id = i,
                FirstName = firstNames[random.Next(firstNames.Length)],
                LastName = lastNames[random.Next(lastNames.Length)]
            });
        }

        return users;
    }
}