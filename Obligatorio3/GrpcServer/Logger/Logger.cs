using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;

namespace Server
{
    public static class Logger
    {
        // TODO pasar a app config
        private const string host = "amqps://yqkizmwe:PLX7QHzf3_iKjXhKCgBcn_Lb3s6gbRKO@clam.rmq.cloudamqp.com/yqkizmwe"
        private static IModel channel;
        
        public static void Log(Log log) {
            log.DateAndTime = DateTime.Now;
            SendMessage(log);
        }

        private static  void SendMessage(Log log) {
            if (channel == null)
                CreateChannel();

            string logInString = JsonSerializer.Serialize(log);
            var body = Encoding.UTF8.GetBytes(logInString);
            
            channel.BasicPublish(exchange: "logs",
                routingKey: log.Severity,
                basicProperties: null,
                body: body);

        }

        private static void CreateChannel()
        {
            var newChannel = new ConnectionFactory() { HostName = host }.CreateConnection().CreateModel();

            newChannel.ExchangeDeclare(exchange: "logs", ExchangeType.Topic);
            this.channel = newChannel;
        }
    }
}
