using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class UdpListener : MonoBehaviour
{
    public int port = 2910;

    private UdpClient client;
    private Thread thread;
    private volatile bool running;

    private string latestMessage = "";
    private readonly object lockObj = new object();

    public Action<string> UDPEvent;

    void Start()
    {
        // Print all IPv4 interfaces Unity can see
        foreach (var ni in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ni.AddressFamily == AddressFamily.InterNetwork)
                Debug.Log("Unity sees local IP: " + ni);
        }

        // Explicit IPv4 bind (fixes Unity 6000 bugs + Windows dual-stack issues)
        try
        {
            var localEP = new IPEndPoint(IPAddress.Any, port);
            client = new UdpClient(localEP);
            client.Client.ReceiveTimeout = 1000;

            Debug.Log($"[UDP] Successfully bound to 0.0.0.0:{port}");
            UDPEvent?.Invoke($"[UDP] Successfully bound to 0.0.0.0:{port}");
        }
        catch (Exception ex)
        {
            Debug.LogError("[UDP] Bind FAILED: " + ex);
            UDPEvent?.Invoke("[UDP] Bind FAILED: " + ex);
            return;
        }

        // Start receive thread
        running = true;
        thread = new Thread(ReceiveLoop);
        thread.IsBackground = true;
        thread.Start();

        Debug.Log("[UDP] Listener thread started");
        UDPEvent?.Invoke("[UDP] Listener thread started");
    }

    private void ReceiveLoop()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

        while (running)
        {
            try
            {
                // Blocks until data arrives OR 1000 ms timeout happens
                byte[] data = client.Receive(ref remoteEP);
                string msg = Encoding.UTF8.GetString(data);

                lock (lockObj)
                    latestMessage = msg;
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.TimedOut)
                    Debug.LogWarning("[UDP] Socket error: " + ex.SocketErrorCode);
            }
            catch (Exception ex)
            {
                Debug.LogError("[UDP] Exception: " + ex);
            }
        }
    }

    void Update()
    {
        string msg = null;

        lock (lockObj)
        {
            if (!string.IsNullOrEmpty(latestMessage))
            {
                msg = latestMessage;
                latestMessage = "";
            }
        }

        if (msg != null)
        {
            Debug.Log("[UDP] Received: " + msg);
            UDPEvent?.Invoke(msg);
        }
    }

    void OnApplicationQuit()
    {
        running = false;
        client?.Close();
    }

    void OnDestroy()
    {
        running = false;
        client?.Close();
    }
}
