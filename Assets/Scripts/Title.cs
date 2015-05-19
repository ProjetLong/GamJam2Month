using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Title : MonoBehaviour {
    #region Properties
    [Header ("Configuration")]
    [SerializeField]
    [Tooltip ("Offset")]
    [Range (0, 10)]
    float _offset = 1f;
    #endregion

    #region Unity
    void FixedUpdate () {
        float x = Random.Range (_basePos.x - _offset, _basePos.x + _offset);
        float y = Random.Range (_basePos.y - _offset, _basePos.y + _offset);
        GetComponent<RectTransform> ().anchoredPosition = new Vector2 (x, y);
    }

    void Start () {
        _basePos = GetComponent<RectTransform> ().anchoredPosition;
    }
    #endregion

    #region Private properties
    Vector3 _basePos;
    #endregion
}
