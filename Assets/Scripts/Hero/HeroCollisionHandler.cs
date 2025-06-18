using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCollisionHandler : MonoBehaviour
{
    [HideInInspector] public HeroController ownerController;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hero collided with: " + other.gameObject.name);

        if (other.CompareTag("HeroBody"))
        {
            Debug.Log("Game Over: Hit own body");
            // GameOver();
        }

        if (other.CompareTag("Collectable"))
        {
            Debug.Log("Collected Hero!");
            UnitStats unit = other.GetComponent<UnitStats>();
            if (unit != null)
            {
                ownerController.AddHeroToChain(unit);
                Destroy(other.gameObject);
            }
        }

        if (other.CompareTag("Enemy") && gameObject.tag == "Hero")
        {
            Debug.Log("Start Battle!");
            var myStats = GetComponent<UnitStats>();
            var enemyStats = other.GetComponent<UnitStats>();
            var controller = GetComponentInParent<HeroController>();

            if (myStats != null && enemyStats != null)
            {
                BattleResolver.Resolve(myStats, enemyStats, controller);

                if (enemyStats.IsAlive)
                {
                    BattleResolver.Resolve(enemyStats, myStats, controller);
                }
            }
        }

        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Hit obstacle. Front hero dies.");
            // RemoveFrontHero();
        }
    }
}