using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// телекинез для игры по сети
/// удерж пкм - притягивать, щелкнуть лкм- толкнуть
/// </summary>
public class Telecines : MonoBehaviour, IOnEventCallback
{
	[SerializeField] int _telecinesDistance, _forcePush, _forceMagnetic;
	[SerializeField] Transform _magneticPoint;
	[SerializeField] PhotonView _view;
	[SerializeField] Transform _camera;
	[SerializeField] LayerMask _notBodyLayers;

	int _targetViewID = -1;

	void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}

    private void Start()
    {
		if (!_view.IsMine) _camera.gameObject.SetActive(false);
	}

    void Update()
	{
		if (!_view.IsMine) return;

		if (Input.GetMouseButtonDown(0))
        {
			PhotonNetwork.RaiseEvent(1,
				JsonUtility.ToJson(new pushEventData { cameraPosition = _camera.position, cameraForward = _camera.forward, TargetViewID = _targetViewID }),
				new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
				new SendOptions());
			_targetViewID = -1;
		}

		// вот тут нужно отслеивать состояние target с помощью PhotonView
		// если target уже притягивается другим персонажем, то ты не можешь претендовать на target
		if (Input.GetMouseButtonDown(1)) _targetViewID = getHitRigidbodyViewID();
		if (Input.GetMouseButtonUp(1)) _targetViewID = -1;
	}

	void FixedUpdate()
    {
		if (_targetViewID != -1)
			PhotonNetwork.RaiseEvent(2,
				JsonUtility.ToJson(new magneticEventData { TargetViewID = _targetViewID, TargetPoint = _magneticPoint.position }),
				new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
				new SendOptions());
	}

	void push(pushEventData data)
	{
		if (data.TargetViewID == -1)
        {
			RaycastHit hit;
			Rigidbody rb = null;
			if (Physics.Raycast(data.cameraPosition, data.cameraForward, out hit, _telecinesDistance, _notBodyLayers))
				rb = hit.rigidbody;
			if (rb != null) rb.AddForceAtPosition(data.cameraForward * _forcePush, hit.point, ForceMode.Impulse);
		}
		else PhotonView.Find(data.TargetViewID).GetComponent<Rigidbody>().AddForce(data.cameraForward * _forcePush, ForceMode.Impulse);
	}

	void magnite(magneticEventData data)
    {
		Rigidbody target = PhotonView.Find(data.TargetViewID).GetComponent<Rigidbody>(); // Серьезно?! никак дешевле не получится?

		Vector3 vector = data.TargetPoint - target.position;
		target.AddForce(vector * vector.magnitude * _forceMagnetic);
	}

	int getHitRigidbodyViewID()
	{
		RaycastHit hit;
		if (Physics.Raycast(_camera.position, _camera.forward, out hit, _telecinesDistance))
		{
			var view = hit.transform.GetComponent<PhotonView>();
			if (view != null) return view.ViewID;
			return -1;
		}
		return -1;
	}

    public void OnEvent(EventData photonEvent)
    {
		if (!PhotonNetwork.IsMasterClient) return;

        switch (photonEvent.Code)
        {
			case 1:
				var pushData = JsonUtility.FromJson<pushEventData>((string)photonEvent.CustomData);
				push(pushData);
				break;
			case 2:
				var magniteData = JsonUtility.FromJson<magneticEventData>((string)photonEvent.CustomData);
				magnite(magniteData);
				break;
            default:
                break;
        }
    }
	void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
	void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);
}

struct magneticEventData
{
	public int TargetViewID;
	public Vector3 TargetPoint;
}
struct pushEventData
{
	public Vector3 cameraPosition, cameraForward;
	public int TargetViewID;
}
