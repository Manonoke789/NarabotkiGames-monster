using UnityEngine;
using HutongGames.PlayMaker;

public class PauseAllFSMExceptParent : MonoBehaviour
{
    public void PauseAllExceptParent()
    {
        PlayMakerFSM parentFSM = GetComponent<PlayMakerFSM>();

        PlayMakerFSM[] allFSMs = FindObjectsOfType<PlayMakerFSM>();
        foreach (PlayMakerFSM fsm in allFSMs)
        {
            if (fsm != parentFSM)
            {
                fsm.enabled = false;
            }
        }
    }

    public void ResumeAll()
    {
        PlayMakerFSM[] allFSMs = FindObjectsOfType<PlayMakerFSM>();
        foreach (PlayMakerFSM fsm in allFSMs)
        {
            fsm.enabled = true;
        }
    }
}