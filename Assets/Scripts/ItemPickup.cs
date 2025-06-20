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

        amount = Random.Range(1, amount);

        if (itemType == ItemType.Heal)
        {
            stats.Heal(amount);
            Debug.Log($"Healed {amount} HP");
        }

        if (itemType == ItemType.AttackBoost)
        {
            stats.attack += amount;
            Debug.Log($"Increased ATK by {amount}");
        }

        if (itemType == ItemType.DefenseBoost)
        {
            stats.defense += amount;
            Debug.Log($"Increased DEF by {amount}");
        }

        Destroy(gameObject);
    }
}