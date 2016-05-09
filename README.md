# Prism.CQRS
A simple PCL CQRS implementation

public class MyEventPayload
{
	public string MyPayloadData { get; set; }
}

public class MyEventEvent : PubSubEvent<MyEventPayload>{}

var messageBus = new MessageBus();


eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Subscribe(ShowNews);

   eventAggregator.GetEvent<TickerSymbolSelectedEvent>().Subscribe(ShowNews,
                                                      ThreadOption.UIThread);


													  fundAddedEvent.Subscribe(FundAddedEventHandler, 
                         ThreadOption.UIThread, false,
                         fundOrder => fundOrder.CustomerId == _customerId);



						 FundAddedEvent fundAddedEvent = eventAggregator.GetEvent<FundAddedEvent>();

bool keepSubscriberReferenceAlive = true;

fundAddedEvent.Subscribe(FundAddedEventHandler, 
                         ThreadOption.UIThread, keepSubscriberReferenceAlive,
                         fundOrder => fundOrder.CustomerId == _customerId);



						 EventAggregator.GetEvent<TickerSymbolSelectedEvent>().Publish(e.Value);



						     compositeWpfEvent.Subscribe(
        FundAddedEventHandler,
        ThreadOption.PublisherThread);

    compositeWpfEvent.Unsubscribe(FundAddedEventHandler);


	FundAddedEvent fundAddedEvent = eventAggregator.GetEvent<FundAddedEvent>();

subscriptionToken = fundAddedEvent.Subscribe(FundAddedEventHandler,   
                                  ThreadOption.UIThread, false,
                                  fundOrder => fundOrder.CustomerId == _customerId);

fundAddedEvent.Unsubscribe(subscriptionToken);