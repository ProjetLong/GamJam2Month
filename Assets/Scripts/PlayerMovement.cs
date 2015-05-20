using UnityEngine;

public class PlayerMovement : Photon.MonoBehaviour
{
    [Header("Links")]
    [SerializeField]
    [Tooltip("Layer of the floor")]
    LayerMask _floorMask;

    public float speed = 6.0f;
    public float rotationSpeed = 1.0f;
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;
    private Quaternion syncStartRotation = Quaternion.identity;
    private Quaternion syncEndRotation = Quaternion.identity;

    private Vector3 movement;
    private Rigidbody playerRigidbody;
    public Vector3 offset = new Vector3(0.0f, 6.0f, -8.0f);
    public bool editorMode;

    void Start()
    {

        //this.floorMask = LayerMask.GetMask("Floor");
        //anim = GetComponent<Animator>();
        this.playerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //if (isMine)
        if (this.photonView.isMine)
            InputMovement();
        else
            SyncedTransform();
    }

    void InputMovement()
    {
        /*float h;
        float v;
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        
        if (h != 0 || v != 0)
        {
            //rotation
            //this.targetRotation = (Quaternion.AngleAxis(Mathf.Atan2(h, v) * Mathf.Rad2Deg, Vector3.up));
            //this.canRotate = true;
            //movement
            this.playerRigidbody.MovePosition(this.playerRigidbody.position + transform.forward * this.speed * Time.fixedDeltaTime);
            ////this.transform.Translate(transform.forward * this.speed * Time.fixedDeltaTime, Space.World);
            /*if (NetworkManager.Instance.isConnected)
                NetworkManager.Instance.sendPosition(this.playerRigidbody.position);*/
        //}

        /*if (this.canRotate)
        {
            this.transform.rotation = Quaternion.Lerp(transform.rotation, this.targetRotation, this.rotationSpeed * Time.fixedDeltaTime);
            if (Mathf.Abs(this.targetRotation.x - this.transform.rotation.x) <= Quaternion.kEpsilon)
            {
                this.canRotate = false;
                this.transform.rotation = this.targetRotation;
            }
        }*/

        #region Movement
        float h, v;
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);
        move.Normalize();
        Vector3 newPos = transform.position + move * speed * Time.fixedDeltaTime;
        transform.position = newPos;
        // Physic hack to avoid player auto move
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        #endregion

        #region Static rotation
        // Rotation with mouse
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, 100, _floorMask))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            GetComponent<Rigidbody>().MoveRotation(newRotation);
            //transform.rotation = newRotation;
        }
        #endregion

        #region Animation
        GetComponentInChildren<Animator>().SetBool("IsWalking", 0 != h || 0 != v);
        #endregion
    }

    private void SyncedTransform()
    {
        syncTime += Time.deltaTime;
        playerRigidbody.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
        playerRigidbody.rotation = Quaternion.Lerp(syncStartRotation, syncEndRotation, syncTime / syncDelay);
    }

    public void moveTo(float x, float y, float z)
    {
        Vector3 syncPosition = new Vector3(x, y, z);
        //Quaternion syncRotation = (Quaternion)stream.ReceiveNext();
        //Vector3 syncVelocity = (Vector3)stream.ReceiveNext();

        syncTime = 0f;
        syncDelay = Time.time - lastSynchronizationTime;
        lastSynchronizationTime = Time.time;

        syncEndPosition = syncPosition;
        syncStartPosition = playerRigidbody.position;

        /*syncEndRotation = syncRotation;
        syncStartRotation = playerRigidbody.rotation;*/
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(rigidbody.position);
            stream.SendNext(rigidbody.rotation);
            stream.SendNext(rigidbody.velocity);
        }
        else
        {
            Vector3 syncPosition = (Vector3)stream.ReceiveNext();
            Quaternion syncRotation = (Quaternion)stream.ReceiveNext();
            Vector3 syncVelocity = (Vector3)stream.ReceiveNext();

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncEndPosition = syncPosition + syncVelocity * syncDelay;
            syncStartPosition = playerRigidbody.position;

            syncEndRotation = syncRotation;
            syncStartRotation = playerRigidbody.rotation;
        }
    }
}
