using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    [Header("Background Sounds")]
    public AudioClip basicAmbience;
    public AudioClip scaryAmbience;

    [Header("Object Sounds")]
    public AudioClip lightBuzz;
    public AudioClip doorCreak;
    public AudioClip doorShut;
    public AudioClip doorRattle;
    public AudioClip doorSlamOpen;

    [Header("Radio Sounds")]
    public AudioClip radioStatic;
    public AudioClip radioTalk;

    [Header("NPC Sounds")]
    public AudioClip dadGrowling;
    public AudioClip dadSeeYou;
    public AudioClip daughterDadWhy;
    public AudioClip daughterNotADemon;
    public AudioClip daughterCry;
    public AudioClip headSomeone;
}
