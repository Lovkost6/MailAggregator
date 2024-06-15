using MailAggregator.Config;
using MailAggregator.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MailAggregator.Service;

public class ServerService
{
    private readonly IMongoCollection<EmailServer> _serverCollection;

    public ServerService(IOptions<MailDatabaseSettings> serverDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            serverDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            serverDatabaseSettings.Value.DatabaseName);
        
        _serverCollection = mongoDatabase.GetCollection<EmailServer>("Servers");
    }
    
    public async Task<List<EmailServer>> GetAsync() =>
        await _serverCollection.Find(_ => true).ToListAsync();

    public async Task<EmailServer> GetServerAsync(string name) =>
        await _serverCollection.Find(x => x.Name == name).FirstOrDefaultAsync();
    
}