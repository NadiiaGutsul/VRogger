using UnityEngine;


public class TimerCollectible : MonoBehaviour
{
    [SerializeField] private float cooldownReduction = 5f;
    [SerializeField] private AudioClip grabSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PCPlayer"))
        {
            Collect();
        }
    }


    private void Collect()
    {
        
        ShopItem[] shopItems = Object.FindObjectsByType<ShopItem>(FindObjectsSortMode.None);
        foreach (ShopItem item in shopItems)
        {
            item.ReduceCooldown(cooldownReduction);
        }
        
        
        Destroy(gameObject);
    }

 
}
