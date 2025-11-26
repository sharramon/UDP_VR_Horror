using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UDPMessagePrinter : MonoBehaviour
{
    public UdpListener listener;     // drag your UdpListener here
    public List<TMP_Text> outputs;   // list of TMP UI text fields

    private int index = 0;

    void Start()
    {
        if (listener == null)
        {
            Debug.LogError("UDPMessagePrinter has no UdpListener assigned.");
            return;
        }

        if (outputs == null || outputs.Count == 0)
        {
            Debug.LogError("UDPMessagePrinter has no TMP_Text fields assigned.");
            return;
        }

        listener.UDPEvent += HandleMessage;
    }

    private void HandleMessage(string msg)
    {
        // set text at current index
        outputs[index].text = msg;

        // move to next index, wrap around
        index = (index + 1) % outputs.Count;
    }

    void OnDestroy()
    {
        if (listener != null)
            listener.UDPEvent -= HandleMessage;
    }
}
