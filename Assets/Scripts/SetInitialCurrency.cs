using UnityEngine;

public class SetInitialValues : MonoBehaviour
{
    public bool setInitialValues = true;
    public int initialGems = 100;
    public int initialPeso = 500;

    private void Awake()
    {
        if (setInitialValues)
        {
            PlayerPrefs.DeleteKey("Gems");
            PlayerPrefs.DeleteKey("Peso");
            PlayerPrefs.SetInt("Gems", initialGems);
            PlayerPrefs.SetInt("Peso", initialPeso);
        }
    }
}