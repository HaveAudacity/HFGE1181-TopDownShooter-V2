using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Blockade : MonoBehaviour
{
    [SerializeField] private GameObject crate;
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;
    [SerializeField] private float damageTick;

    private bool playerInRange;
    private bool crateActive;


    private void Start()
    {
        health = maxHealth;
        crate.SetActive(false);
        crateActive = false;
    }

    private void Update()
    {
        if (health >= 0 && crateActive )
        {
            health -= damageTick * Time.deltaTime;
        }
        else
        {
            crate.SetActive(false);
            crateActive = false;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!playerInRange)
        {
            return;
        }

        health = maxHealth;
        crate.SetActive(true);
        crateActive = true;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        UIManager.Instance.UpdateInteractText("E");
        playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        UIManager.Instance.UpdateInteractText(" ");
        playerInRange = false;
    }


}
