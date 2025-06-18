using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitClass
{
    Warrior,
    Rogue,
    Wizard
}

public class UnitStats : MonoBehaviour
{
    public string unitName = "Unit";
    public UnitClass unitClass;

    [Header("Base Stats")]
    public int maxHP = 10;
    public int currentHP = 10;
    public int attack = 5;
    public int defense = 2;

    public bool IsAlive => currentHP > 0;

    public void Setup(UnitClass unitClass, int hp, int atk, int def)
    {
        this.unitClass = unitClass;
        this.maxHP = this.currentHP = hp;
        this.attack = atk;
        this.defense = def;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(0, currentHP);
        Debug.Log($"{unitName} took {amount} damage. HP now {currentHP}");
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(maxHP, currentHP);
    }

    public void CopyFrom(UnitStats source)
    {
        this.unitName = source.unitName;
        this.unitClass = source.unitClass;
        this.maxHP = source.maxHP;
        this.attack = source.attack;
        this.defense = source.defense;
    }
}