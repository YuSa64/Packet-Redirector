using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PacketRedirector.Models;

namespace PacketRedirector.Services;

public class RedirectorService
{
    private CancellationTokenSource? _cts;
    private Task? _worker;
    public event Action<string>? OnPacketReceived;
    public event Action<int>? OnPacketRateUpdate;

    private int _packetCount;

    public void Start(RedirectSettings settings)
    {
        _cts = new CancellationTokenSource();
        _packetCount = 0;

        _worker = Task.Run(async () =>
        {
            if (settings.ReceiveProtocol == "UDP")
                await RunUdpRedirectAsync(settings, _cts.Token);
            else if (settings.ReceiveProtocol == "TCP")
                await RunTcpRedirectAsync(settings, _cts.Token);
        });

        // Packet rate timer
        Task.Run(async () =>
        {
            while (!_cts.IsCancellationRequested)
            {
                await Task.Delay(1000);
                OnPacketRateUpdate?.Invoke(Interlocked.Exchange(ref _packetCount, 0));
            }
        });
    }

    public void Stop()
    {
        _cts?.Cancel();
    }

    private async Task RunUdpRedirectAsync(RedirectSettings settings, CancellationToken token)
    {
        using var receiver = new UdpClient(new IPEndPoint(IPAddress.Parse(settings.ReceiveIP), settings.ReceivePort));
        using var sender = new UdpClient();

        try
        {
            while (!token.IsCancellationRequested)
            {
                var result = await receiver.ReceiveAsync(token);
                Interlocked.Increment(ref _packetCount);

                await sender.SendAsync(result.Buffer, result.Buffer.Length, settings.TargetIP, settings.TargetPort);

                OnPacketReceived?.Invoke(Encoding.UTF8.GetString(result.Buffer));
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
    }

    private async Task RunTcpRedirectAsync(RedirectSettings settings, CancellationToken token)
    {
        var listener = new TcpListener(IPAddress.Parse(settings.ReceiveIP), settings.ReceivePort);
        listener.Start();

        try
        {
            while (!token.IsCancellationRequested)
            {
                var sourceClient = await listener.AcceptTcpClientAsync(token);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var sourceStream = sourceClient.GetStream();
                        using var destinationClient = new TcpClient();
                        await destinationClient.ConnectAsync(settings.TargetIP, settings.TargetPort, token);
                        using var destinationStream = destinationClient.GetStream();

                        byte[] buffer = new byte[4096];
                        while (!token.IsCancellationRequested)
                        {
                            int bytesRead = await sourceStream.ReadAsync(buffer.AsMemory(0, buffer.Length), token);
                            if (bytesRead <= 0) break;

                            await destinationStream.WriteAsync(buffer.AsMemory(0, bytesRead), token);

                            Interlocked.Increment(ref _packetCount);
                            OnPacketReceived?.Invoke(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);
                    }
                }, token);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
        }
        finally
        {
            listener.Stop();
        }
    }
}
