using Common;
using Common.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;

namespace LogServer
{
    public class LogsServerLogic
    {

        private readonly DataAccess da;
        public LogsServerLogic()
        {

            ISettingsManager SettingsMgr = new SettingsManager();
            string host = SettingsMgr.ReadSetting(LogConfig.MQUri);

            da = DataAccess.GetInstance();

            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri(host);
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "logs", ExchangeType.Topic);

            var severities = SettingsMgr.ReadSetting(LogConfig.Severities).Split(",");


            var queueName = channel.QueueDeclare().QueueName;
            foreach (var severity in severities)
            {
                channel.QueueBind(queue: queueName,
                    exchange: "logs",
                    routingKey: severity);
            }

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                LogRecord log = JsonSerializer.Deserialize<LogRecord>(message);
                da.SaveLog(log);
            };
            channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);


        }

    }
}
