using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class BuyController : MonoBehaviour
{
    public GameObject addMoneyPanel;
    
    [SerializeField] private GameObject _slowButton;
    [SerializeField] private GameObject _showButton;
    [SerializeField] private GameObject _changeButton;
    [SerializeField] private TextMeshProUGUI _gemsCountText;
    [SerializeField] private AudioClip _buyBoosterSound;
    [SerializeField] private AudioClip _buyGemsSound;
    [SerializeField] private AudioClip _generalClick;
    [SerializeField] private GameObject _dontHaveMoney;
    [SerializeField] private AudioController _audioController;
    
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private GameObject _loadingButton;

    private int _gemsCount;

    private void Awake()
    {
        _loadingButton.SetActive(false);
    }
    
    private void Start()
    {
        _gemsCount = PlayerPrefs.GetInt("gems", 50);
        
        if (PlayerPrefs.HasKey("slow"))
        {
            _slowButton.SetActive(false);
        }
        if (PlayerPrefs.HasKey("show"))
        {
            _showButton.SetActive(false);
        }
        if (PlayerPrefs.HasKey("change"))
        {
            _changeButton.SetActive(false);
        }
    }

    private void Update()
    {
        _gemsCountText.text = _gemsCount.ToString();
    }

    public void ShowLoadingButton()
    {
        _loadingButton.SetActive(true);
    }
    
    public void OnPurchaseComlete(Product product)
    {
        if (product.definition.id == "com.consumable.purchase.first")
        {
            Debug.Log("Complete");
            addMoneyPanel.SetActive(true);
            _gemsCount += 50;
            PlayerPrefs.SetInt("gems", _gemsCount);
            PlayerPrefs.Save();
            _audioController.PlaySound(_buyGemsSound);
            _loadingButton.SetActive(false);
        }
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription description)
    {
        if (product.definition.id == "com.consumable.purchase.first")
        {
            _loadingButton.SetActive(false);
            Debug.Log($"Failed: {description.message}");
            _audioController.PlaySound(_generalClick);
        }
    }
    
    public void OnProductFetched(Product product)
    {
        Debug.Log("Fetched");
        _buttonText.text = product.metadata.localizedPriceString;
    }
    
    public void BuySlow()
    {
        if (_gemsCount >= 50)
        {
            _gemsCount -= 50;
            PlayerPrefs.SetInt("gems", _gemsCount);
            _slowButton.SetActive(false);
            _audioController.PlaySound(_buyBoosterSound);
            PlayerPrefs.SetInt("slow", 1);
            PlayerPrefs.Save();
        }
        else
        {
            _audioController.PlaySound(_generalClick);
            _dontHaveMoney.SetActive(true);
        }
    }
    public void BuyShow()
    {
        if (_gemsCount >= 50)
        {
            _gemsCount -= 50;
            PlayerPrefs.SetInt("gems", _gemsCount);
            _showButton.SetActive(false);
            _audioController.PlaySound(_buyBoosterSound);
            PlayerPrefs.SetInt("show", 1);
            PlayerPrefs.Save();
        }
        else
        {
            _audioController.PlaySound(_generalClick);
            _dontHaveMoney.SetActive(true);
        }
    }
    public void BuyChange()
    {
        if (_gemsCount >= 50)
        {
            _gemsCount -= 50;
            PlayerPrefs.SetInt("gems", _gemsCount);
            _changeButton.SetActive(false);
            _audioController.PlaySound(_buyBoosterSound);
            PlayerPrefs.SetInt("change", 1);
            PlayerPrefs.Save();
        }
        else
        {
            _audioController.PlaySound(_generalClick);
            _dontHaveMoney.SetActive(true);
        }
    }
}
