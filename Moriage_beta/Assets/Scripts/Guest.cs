using UnityEngine;

public class Guest : MonoBehaviour
{
    public enum GuestState
    {
        Entering,
        WaitingSeat,
        WaitingOrdering,
        WaitingFood,
        Eating,
        FinishedEating,
        Exiting,
    }

    private float startWaitingTime = 0;
    private float endWaitingTime = 0;

    private GuestState guestState = GuestState.Entering;

    public GuestState CurrentGuestState => guestState;
    public float waitTime => endWaitingTime - startWaitingTime;

    public void SetGuestState(GuestState newState)
    {
        guestState = newState;
    }

    public void StartWaiting()
    {
        startWaitingTime = Time.time;
        endWaitingTime = startWaitingTime;
    }

    public void EndWaiting()
    {
        endWaitingTime = Time.time;
    }
}
