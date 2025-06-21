using UnityEngine;

public enum ItemType
{
    Heal,
    AttackBoost,
    DefenseBoost
}

public class ItemPickup : MonoBehaviour
{
    public ItemType itemType;
    public int amount;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Hero")) return;

        UnitStats stats = other.GetComponent<UnitStats>();
        if (stats == null) return;

        int actualAmount = Random.Range(1, amount);

        switch (itemType)
        {
            case ItemType.Heal:
                stats.Heal(actualAmount);
                Debug.Log($"Healed {actualAmount} HP");
                break;

            case ItemType.AttackBoost:
                stats.attack += actualAmount;
                Debug.Log($"Increased ATK by {actualAmount}");
                break;

            case ItemType.DefenseBoost:
                stats.defense += actualAmount;
                Debug.Log($"Increased DEF by {actualAmount}");
                break;
        }

        Destroy(gameObject);
    }
}