![Build status](http://teamcity.lykkex.net/app/rest/builds/aggregated/strob:(buildType:(project:(id:CommonLibraries_LykkeRabbitMqBroker)))/statusIcon.svg)

Nuget: https://www.nuget.org/packages/Lykke.RabbitMqBroker/


# Lykke.RabbitMqDotNetBroker

Use this library to communicate using abstraction Publisher/Subscriber via RabbitMQ.

The basic pattern to use this implementation is: create, configure, start - everything works. 
If connection is lost - system reconnects automatically
If there is an exception during any process of message handling - Lykke log system will inform IT support services to investigate the case.
We use one exchange/funout - one type of contract model

# How to Publish 

If you want to use a publisher partten answer next questions:
 - Which RabbitMQ server do you want to be connected using notation: https://www.rabbitmq.com/uri-spec.html 
 - How do you want serealize your model to array of bytes: implement your IMessageSerializer<TModel> interface and confugre it
 - How do you want to store unpublished messages queue, to recover when app is restarted: reference [Lykke.RabbitMq.Azure](https://www.nuget.org/packages/Lykke.RabbitMq.Azure) and use BlobPublishingQueueRepository implementation of IPublishingQueueRepository, or implement your own
 - Are you able to lose published message? Then use DisableInMemoryQueuePersistence() to disable in-memory queue persistence
 - Is in-memory queue's monitoring default configuration suitable for you: queue size threshold = 1000, monitor period = 10 sec? If no, call MonitorInMemoryQueue with parameters of you choise
 - Which RabbitMQ publish strategy do you want to use. Use whichever we already have or feel free to write your own IMessagePublishStrategy and do pull request; https://www.rabbitmq.com/tutorials/tutorial-three-dotnet.html
 - Specify Lykke Logging system: ILog;
 - Start the publisher;
 - Publish, publish, publish
 
 
 Minimal working example:
```csharp
       using Lykke.RabbitMq.Azure;
       
       ...

       public static void Example(RabbitMqSubscriptionSettings settings)
        {

            var connection
                = new RabbitMqPublisher<string>(settings)
                .SetLogger(log)
                .SetSerializer(new JsonMessageSerializer())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .SetQueueRepository(new BlobPublishingQueueRepository(new AzureBlobStorage(ConnectionString)))
                // OR
                //.DisableInMemoryQueuePersistence()
                .Start();


            for (var i = 0; i <= 10; i++)
                connection.ProduceAsync("message#" + i);
        }
```

# How to subscribe and consume

To consume messages produced by publisher - answer these questions:

 - Which RabbitMQ server do you want to be connected using notation: https://www.rabbitmq.com/uri-spec.html 
 - How do you want deserealize array of bytes to your model: implement your IMessageDeserializer<TModel> interface and confugre it
 - Which RabbitMQ strategy do you want to use. Use whichever we already have or feel free to write your own IMessageReadStrategy and do pull request; https://www.rabbitmq.com/tutorials/tutorial-three-dotnet.html
 - Specify Lykke Logging system: ILog;
 - Specify callback method for messages to be delivered;
 - Run the Broker;
 - How are you going to handle exceptions on the client side. There are three strategies:
	 - *DefaultErrorHandlingStrategy* - it just logs an exception and receives a new message from RabbitMQ
	 - *ResilientErrorHandlingStrategy* - it accepts two arguments: *retryTimeout* and *retryNum*. In case of an exception it tries execute the handle *retryNum* times with *retryTimeout* between attempts. 
	 - *DeadQueueErrorHandlingStrategy* - in case of an exception it rejects the message and if it is provided a name of the dead letter exchange in the *RabbitMqSubscriptionSettings*  and that exchange exists, then the message will be routed to the dead-letter queue. For more details about dead-letters see [Dead Letter Exchanges](https://www.rabbitmq.com/dlx.html)
Strategies can be chained. If the first failed to handle the exception it call next (if any). Please note that with the first two strategies you will lose your message forever in case of unrecoverable exception. For important messages it is recommended to use the second and third strategies chained.
   
 Minimal working example:
 ```csharp
    public class HowToSubscribe
    {
        private static RabbitMqSubscriber<string> _connector;
        public static void Example(RabbitMqSubscriptionSettings settings)
        {
            var looger = new LogToConsole();

            _connector =
                new RabbitMqSubscriber<string>(settings, new DefaultErrorHandlingStrategy(looger, settings))
                  .SetMessageDeserializer(new TestMessageDeserializer())
                  .CreateDefaultBinding()
                  .Subscribe(HandleMessage)
                  .Start();
        }

        public static void Stop()
        {
            _connector.Stop();
        }

        private static Task HandleMessage(string msg)
        {
            Console.WriteLine(msg);
            return Task.FromResult(0);
        }
    }
```
