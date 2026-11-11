namespace CinemaBooking.Events;

/// <summary>
/// In-process event bus that connects the write side to the read side.
/// <para>
/// By default an event is delivered to its handlers immediately, so the read models are
/// up to date when the command returns. With <c>deferDelivery</c> the events are queued
/// until <see cref="ProcessPending"/> is called. That simulates a real message broker and
/// shows eventual consistency: for a while the read side is stale.
/// </para>
/// </summary>
public sealed class InMemoryEventBus : IEventPublisher
{
    private readonly Dictionary<Type, List<Action<IDomainEvent>>> _handlers = new();
    private readonly Queue<IDomainEvent> _pending = new();
    private readonly bool _deferDelivery;
    private readonly object _gate = new();
    private readonly object _processGate = new(); // one ProcessPending at a time keeps events in order

    public InMemoryEventBus(bool deferDelivery = false) => _deferDelivery = deferDelivery;

    /// <summary>Number of published events that have not been delivered yet.</summary>
    public int PendingCount
    {
        get { lock (_gate) return _pending.Count; }
    }

    /// <summary>Registers a handler that is called for every published event of type <typeparamref name="TEvent"/>.</summary>
    public void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : IDomainEvent
    {
        ArgumentNullException.ThrowIfNull(handler);
        lock (_gate)
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
                _handlers[typeof(TEvent)] = handlers = [];

            handlers.Add(domainEvent => handler.Handle((TEvent)domainEvent));
        }
    }

    public void Publish<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        if (_deferDelivery)
        {
            lock (_gate) _pending.Enqueue(domainEvent);
            return;
        }

        Deliver(domainEvent);
    }

    /// <summary>Delivers all queued events in the order they were published.</summary>
    /// <returns>The number of events delivered.</returns>
    public int ProcessPending()
    {
        lock (_processGate)
        {
            var delivered = 0;
            while (TryDequeue(out var domainEvent))
            {
                Deliver(domainEvent);
                delivered++;
            }
            return delivered;
        }
    }

    private bool TryDequeue(out IDomainEvent domainEvent)
    {
        lock (_gate) return _pending.TryDequeue(out domainEvent!);
    }

    private void Deliver(IDomainEvent domainEvent)
    {
        Action<IDomainEvent>[] handlers;
        lock (_gate)
            handlers = _handlers.TryGetValue(domainEvent.GetType(), out var list) ? [.. list] : [];

        foreach (var handle in handlers)
            handle(domainEvent);
    }
}
