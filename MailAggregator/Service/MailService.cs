using MailAggregator.Config;
using MailAggregator.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MailAggregator.Service;

public class MailService
{
        private readonly IMongoCollection<Mail> _emailsCollection;

        public MailService(
            IOptions<MailDatabaseSettings> emailDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                emailDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                emailDatabaseSettings.Value.DatabaseName);

            _emailsCollection = mongoDatabase.GetCollection<Mail>("Mails");
        }

        public async Task<List<Mail>> GetAsync() =>
            await _emailsCollection.Find(_ => true).ToListAsync();

        public async Task<Mail?> GetAsync(string id) =>
            await _emailsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Mail newMail) =>
            await _emailsCollection.InsertOneAsync(newMail);

        public async Task UpdateAsync(string id, Mail updateMail) =>
            await _emailsCollection.ReplaceOneAsync(x => x.Id == id, updateMail);


}