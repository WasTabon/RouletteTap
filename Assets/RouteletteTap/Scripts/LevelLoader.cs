using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private string _levelName;
    [SerializeField] private GameObject _lock;
    [SerializeField] private GameObject _loadingPanel;

    public void StartLevel()
    {
        if (!_lock.activeSelf)
        {
            _loadingPanel.SetActive(true);
            StartCoroutine(LoadSceneAsync());
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_levelName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        operation.allowSceneActivation = true;
    }
}
