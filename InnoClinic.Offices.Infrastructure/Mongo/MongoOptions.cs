namespace InnoClinic.Offices.Infrastructure.Mongo
{
    public class MongoOptions
    {
        public string ConnectionUri { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public IEnumerable<string> CollectionsNames { get; set; } = new List<string>();
    }
}
