namespace CraftBot.ServerSpecial.MessengerGeek
{
    public class User
    {
        public int Id;
        public string Username;
        public string Name;

        public string Avatar_Template;

        public string GetAvatarUrl(int size) => "https://wink.messengergeek.com" + Avatar_Template.Replace("{size}", size.ToString());

        public string GetName() => string.IsNullOrWhiteSpace(Name) ? Username : $"{Name} ({Username})";
    }
}
