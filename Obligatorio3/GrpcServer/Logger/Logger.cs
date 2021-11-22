using Common;
using Common.Interfaces;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server
{
    public static class Logger
    {
        private static string host;
        private static IModel channel;

        public static async Task Log(LogRecord log)
        {

            ISettingsManager SettingsMgr = new SettingsManager();
            host = SettingsMgr.ReadSetting(ServerConfig.MQUri);
            var date = DateTime.UtcNow;
            log.DateAndTime = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
            SendMessage(log);
        }

        private static async void SendMessage(LogRecord log)
        {
            if (channel == null)
                CreateChannel();

            string logInString = JsonSerializer.Serialize(log);
            var body = Encoding.UTF8.GetBytes(logInString);

            channel.BasicPublish(exchange: "logs",
                routingKey: log.Severity,
                basicProperties: null,
                body: body);

        }

        public async static Task CreateChannel()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri(host);
            var connection = factory.CreateConnection();
            var newChannel = connection.CreateModel();
            newChannel.ExchangeDeclare(exchange: "logs", ExchangeType.Topic);
            channel = newChannel;
        }
    }
}
