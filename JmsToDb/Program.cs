using System;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.Util;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JmsToDb
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Consume JMS Posts ...");

            var processor = new PostProcessor();
            processor.Start();
        }
    }

    // Represents a post resource from https://jsonplaceholder.typicode.com/posts
    //
    // Assignment: Only asks to write userId and title to the database, so not 
    //      including the other fields
    public class Post
    {
        public int userId { get; set; }
        public string title { get; set; }
    }

    public class PostDbContext : DbContext
    {
        private const string DB_FILE_PATH = "Filename=./posts.sqlite";
        public DbSet<Post> posts { get; set; }

        // Composite primary key constraint to avoid duplicate entries
        //
        // Note: This is not specified in the assignment, but it didn't
        // seem useful to have duplicate records unless we provided
        // a timestamp or message id to distinguish them
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasKey(post => new { post.userId, post.title });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            // Specify the path of the database here
            optionsBuilder.UseSqlite(DB_FILE_PATH);
        }
    }

    // Process Posts from JMS messages and write to a database per assigment
    // 
    // Reference: https://activemq.apache.org/components/nms/examples/nms-simple-synchronous-consumer-example
    public class PostProcessor
    {
        private const string JMS_URI = "tcp://localhost:61616";
        private const string JMS_QUEUE = "queue://posts";

        public void Start()
        {
            consumeJmsQueue();
        }

        private void consumeJmsQueue() {
            Uri connecturi = new Uri(JMS_URI);

            Console.WriteLine("About to connect to " + connecturi);

            IConnectionFactory factory = new Apache.NMS.ActiveMQ.ConnectionFactory(connecturi);

            using(IConnection connection = factory.CreateConnection())
            using(ISession session = connection.CreateSession())
            {
                IDestination destination = SessionUtil.GetDestination(session, JMS_QUEUE);
                Console.WriteLine("Using destination: " + destination);

                // Create a consumer, Part 2a.
                using(IMessageConsumer consumer = session.CreateConsumer(destination))
                {
                    // Start the connection so that messages will be processed.
                    connection.Start();
                        
                    // Consume a messages
                    IMessage message;
                    
                    using (var dbContext = new PostDbContext()) {
                        // For the purposes of this assignment, clear and reset the database
                        dbContext.Database.EnsureDeleted();
                        
                        // Create the database if it does not exist
                        dbContext.Database.EnsureCreated ();

                        // Grab all messages from the queue
                        while ((message = consumer.Receive(TimeSpan.FromMilliseconds(2000))) != null)
                        {
                            processMessage(message, dbContext);
                        }

                        // Fetch all the posts to verify they were written per assignment
                        Console.WriteLine ("Posts database content: ");
                        foreach (var aPost in dbContext.posts.ToList ()) {
                            Console.WriteLine ($"{aPost.userId} - {aPost.title}");
                        }
                    }

                    Console.WriteLine("Queue is empty!");
                }
            }
        }

        private void processMessage(IMessage message, PostDbContext dbContext) {
            ITextMessage textMessage = message as ITextMessage;

            Console.WriteLine("Received message with ID:   " + textMessage.NMSMessageId);
            Console.WriteLine("Received message with text: " + textMessage.Text);

            Post post = JsonSerializer.Deserialize<Post>(textMessage.Text);
            
            Console.WriteLine("Writing Post to Database: " + post.title);
            
            // Part 2b.
            dbContext.posts.Add(post);
            dbContext.SaveChanges();
        }
    }

    
}
