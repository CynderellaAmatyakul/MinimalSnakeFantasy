using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCollisionHandler : MonoBehaviour
{
    [HideInInspector] public HeroController ownerController;

    public GameController gameController;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;
        Debug.Log("Hero collided with: " + other.gameObject.name);

        switch (tag)
        {
            case "HeroBody":
                Debug.Log("Game Over: Hit own body");
                gameController.TriggerGameOver();
                audioSource.PlayOneShot(audioClips[0]);
                break;

            case "Collectable":
                Debug.Log("Collected Hero!");
                UnitStats unit = other.GetComponent<UnitStats>();
                if (unit != null)
                {
                    ownerController.AddHeroToChain(unit);
                    Destroy(other.gameObject);
                    audioSource.PlayOneShot(audioClips[1]);
                }
                break;

            case "Enemy":
                if (gameObject.CompareTag("Hero"))
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

                    audioSource.PlayOneShot(audioClips[2]);
                }
                break;

            case "Obstacle":
                if (gameObject.CompareTag("Hero"))
                {
                    Debug.Log("Hit obstacle. Front hero dies.");
                    ownerController?.RemoveFrontHero();

                    audioSource.PlayOneShot(audioClips[0]);
                }
                break;

            case "Item":
                if (gameObject.CompareTag("Hero"))
                {
                    Debug.Log("Picked up item.");
                    // ItemPickup logic will auto-run in its own script
                    audioSource.PlayOneShot(audioClips[1]);
                }
                break;

            default:
                break;
        }
    }
}