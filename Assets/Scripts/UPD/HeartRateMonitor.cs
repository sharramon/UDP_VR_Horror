using UnityEngine;
using System;

public class HeartRateMonitor : MonoBehaviour
{
    [SerializeField] private UdpListener udpListener;

    public event Action<int> HeartRateUpdated;

    void Awake()
    {
        if (udpListener == null)
            udpListener = FindFirstObjectByType<UdpListener>();

        udpListener.UDPEvent += OnUdpMessage;
    }

    private void OnUdpMessage(string msg)
    {
        msg = msg.Trim();

        // Attempt to parse the entire message as an int
        if (int.TryParse(msg, out int hrValue))
        {
            HeartRateUpdated?.Invoke(hrValue);
            Debug.Log($"[HR] Heart rate = {hrValue}");
        }
    }

    void OnDestroy()
    {
        if (udpListener != null)
            udpListener.UDPEvent -= OnUdpMessage;
    }
}
