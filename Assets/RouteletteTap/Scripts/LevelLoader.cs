using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private string _levelName;
    [SerializeField] private GameObject _lock;

    public void StartLevel()
    {
        if (!_lock.activeSelf)
        {
            SceneManager.LoadScene(_levelName);
        }
    }
}
