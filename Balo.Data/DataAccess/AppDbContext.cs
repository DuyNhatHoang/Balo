using Balo.Data.MongoCollections;
using Balo.Data.ViewModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Balo.Data.DataAccess
{
    public class AppDbContext
    {
        private readonly IMongoDatabase _db;
        private IMongoClient _mongoClient;

        public AppDbContext(IMongoClient client, string databaseName)
        {
            _db = client.GetDatabase(databaseName);
            _mongoClient = client;
        }

        public IMongoCollection<Board> Boards => _db.GetCollection<Board>("boards");
        public IMongoCollection<User> Users => _db.GetCollection<User>("users");
        public IMongoCollection<BoardInviation> BoardInviations => _db.GetCollection<BoardInviation>("boardInviations");
        public IMongoCollection<Group> Groups => _db.GetCollection<Group>("groups");
        public IMongoCollection<PlannedTask> Tasks => _db.GetCollection<PlannedTask>("tasks");
     

        public IClientSessionHandle StartSession()
        {
            return _mongoClient.StartSession();
        }
        public void CreateCollectionsIfNotExists()
        {
            var collectionNames = _db.ListCollectionNames().ToList();

            if (!collectionNames.Any(name => name == "boards"))
            {
                _db.CreateCollection("boards");
            }
            if (!collectionNames.Any(name => name == "users"))
            {
                _db.CreateCollection("users");
            }


        }
    }
}
