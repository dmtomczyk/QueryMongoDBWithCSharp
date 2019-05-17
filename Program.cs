using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace WorkingWithMongoDB
{
    class Program
    {
        static void Main(string[] args)
        {
            // Calling our MainAsync() method defined below.
            QueryDBForWinnerCollectionInformation().Wait();

            // Reads the output we would have seen from console. Unsure of purpose, runs without this line
            Console.ReadLine();
        }

        // Creating a class for our WINNERS Collection -- with getters and setters. Every object in DB must be defined
        public class WinnerDB
        {
            // [BsonElement("db item name")] needs to be the same as what is stored on the DB
            // the following variable declarations are used to rename those db items for use in this application
            [BsonId] public ObjectId Id { get; set; }

            [BsonElement("PlayerFirstName")] public string PlayerFirstName { get; set; }

            [BsonElement("PlayerLastName")] public string PlayerLastName { get; set; }

            [BsonElement("PlayerPhoneNumber")] public string PlayerPhoneNumber { get; set; }

            [BsonElement("PlayerEmail")] public string PlayerEmail { get; set; }

            [BsonElement("PlayerPendingPrizeRedemption")] public bool PlayerPendingPrizeRedemption { get; set; }

            [BsonElement("PlayerLastPlayed")] public string PlayerLastPlayed { get; set; }
        }

        // ASYNC Method since we need to "await" on the DB Collection query to ensure we have needed information before contiuing
        static async Task QueryDBForWinnerCollectionInformation()
        {
            // Location of our database
            var connectionString = "mongodb://localhost:27017";

            // Creating a MongoClient() for the DB
            var client = new MongoClient(connectionString);

            // Declaring a StartSession for use later
            var session = client.StartSession();

            // Longwinded way of getting a collection from the db and assigning to var
            var collection = session.Client.GetDatabase("local").GetCollection<WinnerDB>("winners");

            session.StartTransaction();

            try
            {
                // Creating an empty filter so that we pull all information from the WinnerDB Collection (Created as a class above)
                var filter = new FilterDefinitionBuilder<WinnerDB>().Empty;

                // Applying our filter to the collection and converting it to a list that is usable
                var results = await collection.Find<WinnerDB>(filter).ToListAsync<WinnerDB>();

                // Iterate through the results and output information as needed
                foreach (WinnerDB winner in results)
                {
                    Console.WriteLine($"First Name: {winner.PlayerFirstName}");
                    Console.WriteLine($"Last Name: {winner.PlayerLastName}");
                    Console.WriteLine($"Phone Number: {winner.PlayerPhoneNumber}");
                    Console.WriteLine($"Email: {winner.PlayerEmail}");
                    Console.WriteLine($"Pending Prize: {winner.PlayerPendingPrizeRedemption}");
                    Console.WriteLine($"Last Played: {winner.PlayerLastPlayed}");
                }
            }

            // Aborting the transaction if we encounter any exceptions
            catch (Exception ex)
            {
                Console.WriteLine("Error Querying MongoDB: " + ex.Message);
                session.AbortTransaction();
            }
        }
    }
}