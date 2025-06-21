using UnityEngine;

public class HeroModelManager : MonoBehaviour
{
    public GameObject warriorModel;
    public GameObject wizardModel;
    public GameObject rogueModel;

    private UnitStats stats;

    void Awake()
    {
        stats = GetComponent<UnitStats>();

    }

    private void Start()
    {
        UpdateModelBasedOnClass();
    }

    public void UpdateModelBasedOnClass()
    {
        if (stats == null)
        {
            Debug.LogWarning("UnitStats not found on HeroModelManager");
            return;
        }

        if (warriorModel != null) warriorModel.SetActive(false);
        if (wizardModel != null) wizardModel.SetActive(false);
        if (rogueModel != null) rogueModel.SetActive(false);

        switch (stats.unitClass)
        {
            case UnitClass.Warrior:
                if (warriorModel != null) warriorModel.SetActive(true);
                break;
            case UnitClass.Wizard:
                if (wizardModel != null) wizardModel.SetActive(true);
                break;
            case UnitClass.Rogue:
                if (rogueModel != null) rogueModel.SetActive(true);
                break;
        }
    }
}