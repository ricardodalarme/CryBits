namespace CryBits.Transport;

public enum DeliveryChannel : byte
{
    ReliableOrdered,
    ReliableUnordered,
    Sequenced,
    ReliableSequenced
}
