using UnityEngine;

public abstract class EffectData : ScriptableObject
{
    public abstract EffectInstance CreateInstance();
}

public abstract class EffectInstance
{
    public EffectData effectData;
    public abstract void Apply(GameObject target);
}