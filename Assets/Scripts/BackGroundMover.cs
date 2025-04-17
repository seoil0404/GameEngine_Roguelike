using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BackGroundMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float interval;
    [SerializeField] private float endX;

    [SerializeField] BackGroundMover afterBackGround;

    public BackGroundMover AfterBackGround
    {
        set => afterBackGround = value;
    }

    public BackGroundMover GetLastBackGround()
    {
        if (afterBackGround == null) return this;

        return afterBackGround.GetLastBackGround();
    }

    private void Update()
    {
        transform.position -= new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        if (transform.position.x < endX)
        {
            transform.position = new Vector3(GetLastBackGround().transform.position.x + interval, transform.position.y, transform.position.z);
            GetLastBackGround().AfterBackGround = this;
            afterBackGround = null;
        }
    }
}
