using UnityEngine;

public class WeaponData : MonoBehaviour
{
    [Header("Weapon Prefab")]
    [SerializeField] private GameObject pickupPrefab;

    [Header("UI")]
    [SerializeField] private Sprite weaponIcon;

    [Header("Animation")]
    [SerializeField] private string shootTrigger;
    [SerializeField] private string idleTrigger;
    [SerializeField] private string sprintTrigger;

    public GameObject PickupPrefab => pickupPrefab;
    public Sprite WeaponIcon => weaponIcon;

    public string ShootTrigger => shootTrigger;
    public string IdleTrigger => idleTrigger;
    public string SprintTrigger => sprintTrigger;
}