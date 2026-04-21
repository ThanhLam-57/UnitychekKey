using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;

public class TikTokUdpReceiver : MonoBehaviour
{
    [Header("UDP")]
    public int port = 7777;
    public bool logRaw = false;

    public event Action<TikEvent> OnEvent;

    UdpClient _udp;
    Thread _thread;
    volatile bool _running;

    void OnEnable() => StartReceiver();
    void OnDisable() => StopReceiver();

    public void StartReceiver()
    {
        if (_running) return;

        try
        {
            _udp = new UdpClient(port);
            _udp.Client.ReceiveTimeout = 1000;

            _running = true;
            _thread = new Thread(Loop) { IsBackground = true };
            _thread.Start();

            Debug.Log($"[UDP] Listening on 0.0.0.0:{port}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[UDP] StartReceiver error: {e}");
            StopReceiver();
        }
    }

    public void StopReceiver()
    {
        _running = false;

        try { _udp?.Close(); } catch { }
        _udp = null;

        try { _thread?.Join(200); } catch { }
        _thread = null;
    }

    void Loop()
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);

        while (_running)
        {
            try
            {
                byte[] data = _udp.Receive(ref ep);
                string json = Encoding.UTF8.GetString(data);

                if (logRaw) Debug.Log($"[UDP RAW] {json}");

                // ✅ parse chuẩn
                TikEvent ev = JsonConvert.DeserializeObject<TikEvent>(json);
                if (ev == null) continue;

                ev.rawJson = json; // debug

                MainThreadDispatcher.Run(() =>
                {
                    OnEvent?.Invoke(ev);
                });
            }
            catch (SocketException)
            {
                // timeout
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UDP] Loop error: {e.Message}");
            }
        }
    }
}