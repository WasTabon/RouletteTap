using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject _tutorialWindow;
    [SerializeField] private Button _button;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("tutorial"))
        {
            _tutorialWindow.SetActive(true);
            _button.onClick.AddListener(SetTutorialOff);
        }
    }

    public void SetTutorialOff()
    {
        PlayerPrefs.SetInt("tutorial", 1);
        PlayerPrefs.Save();
    }
}
