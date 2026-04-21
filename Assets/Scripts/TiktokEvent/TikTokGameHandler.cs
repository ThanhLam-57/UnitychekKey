using UnityEngine;

public class TikTokGameHandler : MonoBehaviour
{
    public TikTokUdpReceiver receiver;

    void Start()
    {
        receiver.OnEvent += HandleEvent;
    }

    void OnDestroy()
    {
        receiver.OnEvent -= HandleEvent;
    }

    void HandleEvent(TikEvent ev)
    {
        switch (ev.type)
        {
            case "connect":
                Debug.Log("LIVE CONNECTED");
                break;

            case "disconnect":
                Debug.Log("LIVE DISCONNECTED");
                break;

            case "comment":
                Debug.Log($"{ev.user.nickname}: {ev.comment}");
                break;

            case "like":
                Debug.Log($"❤️ {ev.user.nickname} +{ev.like_count} (total {ev.total_likes})");
                break;

            case "follow":
                Debug.Log($"👤 {ev.user.nickname} followed");
                break;

            case "share":
                Debug.Log($"🔁 {ev.user.nickname} shared");
                break;

            case "gift_final":
                Debug.Log($"🎁 {ev.user.nickname} gửi {ev.count} {ev.gift.name} ({ev.total} xu)");

                HandleGift(ev);
                break;
        }
    }

    void HandleGift(TikEvent ev)
    {
        int count = ev.count;
        int totalCoin = ev.total;
        int price = ev.gift.price;

        // ví dụ logic game
        if (totalCoin >= 100)
        {
            Debug.Log("🔥 SPAWN BOSS");
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                Debug.Log("Spawn unit");
            }
        }
    }
}