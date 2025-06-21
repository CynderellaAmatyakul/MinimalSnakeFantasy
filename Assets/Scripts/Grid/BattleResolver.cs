using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BattleResolver
{
    public static void Resolve(UnitStats attacker, UnitStats defender, HeroController heroController = null)
    {
        // Trigger attack animation
        if (attacker != null)
        {
            Animator attackerAnim = attacker.GetComponentInChildren<Animator>();
            if (attackerAnim != null)
            {
                attackerAnim.SetTrigger("Attack");
            }
        }

        int damage = CalculateDamage(attacker, defender);
        defender.TakeDamage(damage);

        Debug.Log($"{attacker.unitName} attacks {defender.unitName} for {damage} damage");

        if (!defender.IsAlive)
        {
            Debug.Log($"{defender.unitName} is defeated!");

            Animator defenderAnim = defender.GetComponentInChildren<Animator>();
            if (defenderAnim != null)
            {
                defenderAnim.SetBool("IsDead", true);
            }

            if (attacker != null)
            {
                attacker.GainXP(7);
            }

            Object.Destroy(defender.gameObject, 1.5f); // Wait for death animation
        }
        else if (!defender.IsAlive && defender.tag == "Hero" && heroController != null)
        {
            heroController.RemoveFrontHero();
        }
    }

    private static int CalculateDamage(UnitStats attacker, UnitStats defender)
    {
        int baseDamage = Mathf.Max(0, attacker.attack - defender.defense);

        // Bonus if class advantage (Rock-Paper-Scissors)
        if (HasAdvantage(attacker.unitClass, defender.unitClass))
        {
            baseDamage *= 2;
            Debug.Log($"Class advantage! {attacker.unitClass} > {defender.unitClass}");
        }

        return baseDamage;
    }

    private static bool HasAdvantage(UnitClass a, UnitClass b)
    {
        return (a == UnitClass.Warrior && b == UnitClass.Rogue) ||
               (a == UnitClass.Rogue && b == UnitClass.Wizard) ||
               (a == UnitClass.Wizard && b == UnitClass.Warrior);
    }
}