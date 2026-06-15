using System.Threading.Channels;

namespace CryBits.Transport.Transports;

public sealed class LoopbackPair
{
    public LoopbackServerTransport Server { get; }
    public LoopbackClientTransport Client { get; }

    public LoopbackPair()
    {
        var serverToClient = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
        var clientToServer = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

        Server = new LoopbackServerTransport(clientToServer.Reader, serverToClient.Writer);
        Client = new LoopbackClientTransport(serverToClient.Reader, clientToServer.Writer);
    }
}
