using System;
using Core;
using EventManager;
using UnityEngine;
using UnityEngine.UI;

public class UIActor : MonoBehaviour
{
    public Slider healthBar;
    public Slider energyBar;

    public Actor actor;

    public void Init(Actor actor, bool show = false)
    {
        Events.AddListener(Flag.DamageRecieved, actor, OnDamageRecieved);
        DamageReceiver damageReceiver = actor.GetComponent<DamageReceiver>();
        UpdateHealth(damageReceiver);
        gameObject.SetActive(show);
        this.actor = actor;

        if (energyBar && actor is EnergyPylon)
        {
            gameObject.SetActive(true);
            healthBar.gameObject.SetActive(false); 
            energyBar.gameObject.SetActive(true); 
        }
    }

    public void OnEnable()
    {
        if (energyBar && actor is EnergyPylon)
        {
            gameObject.SetActive(true);
            healthBar.gameObject.SetActive(false); 
            energyBar.gameObject.SetActive(true); 
        }
    }

    private void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;

        if (energyBar && actor is EnergyPylon energyPylon)
        {
            energyBar.value = (float)energyPylon.currentCharge / energyPylon.maxCharge;
        }
    }

    private void OnDamageRecieved(object origin, EventArgs eventargs)
    {
        var damageArgs = eventargs as DamageRecievedArgs;
        UpdateHealth(damageArgs.reciever.GetComponent<DamageReceiver>());
    }

    public void UpdateHealth(DamageReceiver damageReceiver)
    {
        if(damageReceiver == null) return;
        
        var maxHealth = damageReceiver.maxHealth;
        var currentHealth = damageReceiver.currentHealth;
        gameObject.SetActive(true);
        healthBar.gameObject.SetActive(true);
        healthBar.value = (float)currentHealth/ maxHealth;
    }
}
