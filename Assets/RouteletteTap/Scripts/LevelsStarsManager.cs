using System;
using UnityEngine;

public class LevelsStarsManager : MonoBehaviour
{
    [SerializeField] private Transform[] _levels;

    private int _level;
    private int _stars;
    
    private void Start()
    {
        if (PlayerPrefs.HasKey("level"))
        {
            _level = PlayerPrefs.GetInt("level");
            _stars = PlayerPrefs.GetInt("stars");
            PlayerPrefs.DeleteKey("level");
            PlayerPrefs.DeleteKey("stars");
        }
    }
}
