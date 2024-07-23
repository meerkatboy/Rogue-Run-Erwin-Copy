using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterEvents
{
    //damaged and value
    public static UnityAction<GameObject, int> CharacterDamaged;
    //heal and value
    public static UnityAction<GameObject, int> CharacterHealed;
    //darkness and value
    public static UnityAction<GameObject, int> CharacterDropped;

}