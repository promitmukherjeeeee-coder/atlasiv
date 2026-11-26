namespace Atlasiv.Core.Orders;

/// <summary>What an order asks for. Each system that takes orders defines its own payload records by deriving from this one.</summary>
public abstract record OrderPayload;
