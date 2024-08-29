using UnityEngine;

public class Currency : MonoBehaviour
{
    private const string GemsKey = "Gems";
    private const string PesoKey = "Peso";

    [SerializeField] private int _gems;
    [SerializeField] private int _peso;

    public int Gems
    {
        get { return _gems; }
        set
        {
            _gems = value;
            PlayerPrefs.SetInt(GemsKey, _gems);
        }
    }

    public int Peso
    {
        get { return _peso; }
        set
        {
            _peso = value;
            PlayerPrefs.SetInt(PesoKey, _peso);
        }
    }

    public void AddGems(int amount)
    {
        Gems += amount;
    }

    public void SubtractGems(int amount)
    {
        Gems -= amount;
    }

    public void AddPeso(int amount)
    {
        Peso += amount;
    }

    public void SubtractPeso(int amount)
    {
        Peso -= amount;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(GemsKey))
            Gems = PlayerPrefs.GetInt(GemsKey);
        if (PlayerPrefs.HasKey(PesoKey))
            Peso = PlayerPrefs.GetInt(PesoKey);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(GemsKey, _gems);
        PlayerPrefs.SetInt(PesoKey, _peso);
    }
}

