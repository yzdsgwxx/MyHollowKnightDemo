using Cinemachine;
using DG.Tweening;
using UnityEngine;

/**
 * Title:
 * Description:
 */
public class SwitchCameraTrigger : MonoBehaviour
{
    [Tooltip("不填就不变")]
    [SerializeField] private CinemachineVirtualCamera _newCamera;
    [Tooltip("不填就不变")]
    [SerializeField] private Transform _newXFollow;

    private Collider2D _col;
    private Collider2D _playerCol;
    private CinemachineVirtualCamera _oldCamera;
    private Transform _oldFollow;
    private bool _isFollowing;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && IsEnterFromLeft(collision))
        {
            _isFollowing = _newXFollow;
            _playerCol = collision;
            (_oldCamera, _oldFollow) = CameraManager._instance.SwitchCamera(_newCamera, _newXFollow);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && IsEnterFromLeft(collision))
        {
            _isFollowing = false;
            _playerCol = null;
            CameraManager._instance.SwitchCamera(_oldCamera, _oldFollow);
        }
    }
    private void Update()
    {
        if (_isFollowing && _newXFollow && _playerCol)
        {
            _newXFollow.transform.position = new Vector2(_playerCol.transform.position.x, _newXFollow.transform.position.y);
        }
    }

    private bool IsEnterFromLeft(Collider2D playerCol)
    {
        return (playerCol.transform.position - _col.bounds.center).x < 0;
    }
}
