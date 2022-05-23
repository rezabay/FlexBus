using System;

namespace FlexBus.Transport;

public enum MqLogType
{
    //RabbitMQ
    ConsumerCancelled,
    ConsumerRegistered,
    ConsumerUnregistered,
    ConsumerShutdown,

    //Kafka
    ConsumeError,
    ServerConnError,

    //AzureServiceBus
    ExceptionReceived,

    //Amazon SQS
    InvalidIdFormat,
    MessageNotInflight
}

public class LogMessageEventArgs : EventArgs
{
    public string Reason { get; set; }

    public MqLogType LogType { get; set; }
}