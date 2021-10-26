using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeapondDisplayDataHolder : MonoBehaviour
{
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TMP_Text weaponName;
    [SerializeField] private GameObject unselectedHighlight;

    public void SetValues(Sprite weaponIcon, string weaponName)
    {
        this.weaponIcon.sprite = weaponIcon;
        this.weaponName.text = weaponName;
    }

    public void DeselectWeapon()
    {
        unselectedHighlight.SetActive(true);
    }

    public void SelectWeapon()
    {
        unselectedHighlight.SetActive(false);
    }
}
