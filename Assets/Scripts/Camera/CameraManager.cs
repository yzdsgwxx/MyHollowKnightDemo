using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/**
 * Title:调整Y轴跟随速度，修改相机偏移,X轴跟随点偏移,切换相机
 * Description:
 */
public class CameraManager : MonoBehaviour
{
    public static CameraManager _instance;
    /// <summary>
    /// 引用
    /// </summary>
    private GameObject _player;
    private PlayerMovement _playerMovement;
    private bool _isFalling;
    private List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
    public CinemachineVirtualCamera _curCamera;
    private CinemachineFramingTransposer _curFramingTransposer;
    /// <summary>
    /// Y轴跟随
    /// </summary>
    [SerializeField] private float _fallingYDamping = 0.3F;
    [SerializeField] private float _lerpYDuration = 1f;
    private float _normYDamping;
    private Coroutine _lerpYCoroutine;
    private bool _isLerping = false;
    private bool _isLerpingFalling = false;
    
    /// <summary>
    /// X轴跟随
    /// </summary>
    private bool _isFacingRight = false;
    private float _xLerpOffset;
    [SerializeField] private float _xLerpTime = 0.3f;
    /// <summary>
    /// 特写
    /// </summary>
    private bool bCloseUp = false;
    #region 生命周期
    private void Awake()
    {
        cameras = FindObjectsOfType<CinemachineVirtualCamera>().ToList<CinemachineVirtualCamera>();
        if (_instance == null)
        {
            _instance = this;
        }

        for (int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i].enabled)
            {
                _curCamera = cameras[i];
                _curFramingTransposer = _curCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                _normYDamping = _curFramingTransposer.m_YDamping;
                _xLerpOffset = _curFramingTransposer.m_TrackedObjectOffset.x;
            }
        }
        _playerMovement = GameObject.FindObjectOfType<PlayerMovement>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    [ExecuteAlways]
    private void Update()
    {
        CheckLerpY();
        CheckLerpX();
    }

    private void OnDisable()
    {
        DOTween.Kill(gameObject);
    }
    #endregion
    #region Y轴跟随速度
    IEnumerator LerpYAction(bool bfalling)
    {
        _isLerping = true;
        _isLerpingFalling = bfalling;
        float startY = _curFramingTransposer.m_YDamping;
        float endY = bfalling ? _fallingYDamping : _normYDamping;
        float elapsedTime = 0f;
        while (elapsedTime < _lerpYDuration)
        {
            elapsedTime += Time.deltaTime;
            _curFramingTransposer.m_YDamping = Mathf.Lerp(startY, endY, elapsedTime / _lerpYDuration);
            yield return null;
        }
        _isLerping = false;
    }

    private void CheckLerpY()
    {
        bool isfalling = _playerMovement._isFalling;
        if (_isFalling != isfalling)
        {
            if (isfalling)
            {
                if (_isLerping && _isLerpingFalling) return;
                if (_lerpYCoroutine != null) StopCoroutine(_lerpYCoroutine);
                _lerpYCoroutine = StartCoroutine(LerpYAction(isfalling));
            }
            else
            {
                if (_isLerping && !_isLerpingFalling) return;
                if (_lerpYCoroutine != null) StopCoroutine(_lerpYCoroutine);
                _lerpYCoroutine = StartCoroutine(LerpYAction(false));
            }
        }
        _isFalling = isfalling;
    }
    #endregion

    #region X轴跟随点
    private void CheckLerpX()
    {
        bool newIsFacingRight = _playerMovement.isFacingRight;

        if (_isFacingRight != newIsFacingRight && !bCloseUp)
        {
            LerpX(newIsFacingRight,_xLerpTime);
        }
        _isFacingRight = newIsFacingRight;
    }

    private void LerpX(bool isFacingRight,float lerpTime)
    {
        DOTween.To(
            () => _curFramingTransposer.m_TrackedObjectOffset,
            (x) => _curFramingTransposer.m_TrackedObjectOffset = x,
            new Vector3(_xLerpOffset * (isFacingRight ? 1 : -1),0,0),
            lerpTime);
    }

    private void LerpX(Vector3 targetOffset,float duration)
    {
        DOTween.To(
            () => _curFramingTransposer.m_TrackedObjectOffset,
            (x) => _curFramingTransposer.m_TrackedObjectOffset = x, 
            targetOffset,
            duration);
    }
    #endregion
    #region 特写相机
    public void CloseUp(Vector3 targetOffset,float duration,bool back)
    {
        bCloseUp = !back;
        if (back)
        {
            LerpX(_isFacingRight,duration);
        }
        else
        {
            LerpX(targetOffset, duration);
        }

    }
    #endregion
    #region 切换相机
/// <summary>
/// 切换相机并返回旧的跟随目标
/// </summary>
/// <param name="targetCamera"></param>
/// <param name="follow"></param>
/// <returns>旧的跟随目标</returns>
    public (CinemachineVirtualCamera,Transform) SwitchCamera(CinemachineVirtualCamera targetCamera,Transform follow)
    {
        CinemachineVirtualCamera oldCamera = _curCamera;
        Transform oldFollow = _curCamera.Follow;
        if (targetCamera)
        {
            foreach(var cam in cameras)
            {
                if(targetCamera == cam)
                {
                    cam.enabled = true;
                    _curCamera = cam;
                    _curFramingTransposer = _curCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    _normYDamping = _curFramingTransposer.m_YDamping;
                    _xLerpOffset = _curFramingTransposer.m_TrackedObjectOffset.x;
                }
                else
                {
                    cam.enabled = false;
                }
            }
        }
        if (follow)
        {
            _curCamera.Follow = follow;
        }
        return (oldCamera,oldFollow);
    }
    public void FollowPlayer()
    {
        _curCamera.Follow = _player.transform;
    }
    #endregion
}
