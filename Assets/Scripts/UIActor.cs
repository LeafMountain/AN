using System;
using EventManager;
using UnityEngine;
using UnityEngine.UI;

public class UIActor : MonoBehaviour
{
    public Slider healthBar;

    public void Init(Actor actor, bool show = false)
    {
        Events.AddListener(Flag.DamageRecieved, actor, OnDamageRecieved);
        DamageReciever damageReciever = actor.GetComponent<DamageReciever>();
        UpdateHealth(damageReciever);
        gameObject.SetActive(show);
    }

    private void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    private void OnDamageRecieved(object origin, EventArgs eventargs)
    {
        var damageArgs = eventargs as DamageRecievedArgs;
        UpdateHealth(damageArgs.reciever.GetComponent<DamageReciever>());
    }

    public void UpdateHealth(DamageReciever damageReciever)
    {
        if(damageReciever == null) return;
        
        var maxHealth = damageReciever.maxHealth;
        var currentHealth = damageReciever.currentHealth;
        gameObject.SetActive(true);
        healthBar.gameObject.SetActive(true);
        healthBar.value = (float)currentHealth / maxHealth;
    }
}
