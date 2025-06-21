using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitHealthUI : MonoBehaviour
{
    public Slider hpSlider;
    public TMP_Text unitNameText;
    public TMP_Text statText;
    public Transform followTarget;

    private UnitStats unit;

    void Start()
    {
        unit = GetComponentInParent<UnitStats>();
        if (unit != null)
        {
            unitNameText.text = unit.unitName;
            UpdateHP();
        }
    }

    void Update()
    {
        if (unit != null)
        {
            UpdateHP();

            statText.text = $"LV {unit.level}  \nATK: {unit.attack}  DEF: {unit.defense}";

            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }

        if (followTarget != null)
        {
            transform.position = followTarget.position + Vector3.up * 2f;
        }
    }

    void UpdateHP()
    {
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
    }
}