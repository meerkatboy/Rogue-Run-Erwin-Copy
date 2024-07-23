using System.Collections;
using UnityEngine;

public interface IEnemy
{
    //applies vigilant debuff onto enemy
    public IEnumerator ApplyGraviton(int seconds);
    
    //when the player is hit.
    public void OnHit(int damage, Vector2 knockback);
}