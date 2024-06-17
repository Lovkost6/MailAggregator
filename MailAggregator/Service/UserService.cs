using MailAggregator.Config;
using MailAggregator.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MailAggregator.Service;

public class UserService
{
    private readonly IMongoCollection<User> _usersCollection;

    public UserService(IOptions<MailDatabaseSettings> userDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            userDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            userDatabaseSettings.Value.DatabaseName);
        
        _usersCollection = mongoDatabase.GetCollection<User>("Users");
    }
    
    public async Task CreateAsync(User newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

    public async Task<User> GetByEmailAsync(string email) =>
        await _usersCollection.Find(x=> x.Email == email).FirstOrDefaultAsync();
}