using UnityEngine;
using UnityEngine.EventSystems;

public class PointerDown : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private CombatSystem _combatSystem;
    public void OnPointerDown(PointerEventData eventData)

    {
        if (_combatSystem.noteMovement != null)
        {
            _combatSystem.noteMovement.PressingtappingTheScreen();
        }
        else if (_combatSystem.Miss == false && Time.timeScale != 0)
        {
            _combatSystem.Accuracy(0, _combatSystem.ArmorOrDamage);
        }
        if (_combatSystem.noteMovement == null && _combatSystem.noteMovement2 != null)
        {
            _combatSystem.noteMovement2.HidingObject();
        }

    }
}
