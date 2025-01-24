using System.Collections;
using UnityEngine;

public class NotificationManager: MonoBehaviour
{
    public static NotificationManager Instance;
    public GameObject NotificationPrefab;
    public Transform NotificationPoint;
    public float Timer;

    private void Start()
    {
        Instance = this;
    }

    public void PushNotificationDetailsOfTree(string strain, int flowers, int totalScore)
    {
        print("Push");
        GameObject notificationObject = Instantiate(NotificationPrefab, NotificationPoint);

        Animator anim = notificationObject.GetComponent<Animator>();
        Notification notification = notificationObject.GetComponent<Notification>();

        notification.strain.text = strain;
        notification.details.text = "Flowers : " + flowers;
        notification.totalScore.text = totalScore.ToString();

        StartCoroutine(DeleteNotification(anim, notificationObject));
    }

    IEnumerator DeleteNotification(Animator anim, GameObject notificationObject)
    {
        yield return new WaitForSeconds(Timer);
        anim.SetTrigger("isEnd");
        Destroy(notificationObject, 1);
    }
}