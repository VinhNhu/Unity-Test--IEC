using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Object : MonoBehaviour
{
    public _ObjectsType e_TypeObj;

    public _ObjectsType E_TypeObj { get => e_TypeObj; }

    public bool IsMoved;
    public bool IsHighLight;
    [SerializeField] private bool isSorted;
    public bool IsAddCompleted = false;

    public bool IsFirstElement;

    public Vector3 startPos2;
    public Quaternion RotationBase;

    public Tween Move, Rotate, Rotation, Scale;

    public LayerMask raycastLayer;


    private void Start()
    {
        InitPos();
    }
    public void InitPos()
    {
        startPos2 = transform.position;
        RotationBase = transform.rotation;
        GameManager.Instance.isSort = true;
    }

    public void CheckHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayer))
        {
            return;
        }

        if (hit.collider.gameObject != this.gameObject)
        {
            return;
        }
    }

    private void OnMouseUp()
    {
        if (!GameManager.Instance.isSort) return;
        isSorted = false;
        if (GameManager.Instance.ListItemPicked.CheckIsFullItem()) return;
        if (IsMoved) return;
        if(GameManager.Instance.M_BoardController.IsBusy) return;
        if (GameManager.Instance.M_BoardController.m_gameOver) return;

        CheckHit();

        if (GameManager.Instance.ListItemPicked.L_Save1.Count > 0)
        {
            for (int i = 0; i < GameManager.Instance.ListItemPicked.L_Save1.Count; i++)
            {
                if (e_TypeObj == GameManager.Instance.ListItemPicked.L_Save1[i].e_TypeObj)
                {
                    IsAddCompleted = false;
                    GameManager.Instance.ListItemPicked.SortAndMoveElement(this);
                    GameManager.Instance.ListItemPicked.AddDic(E_TypeObj, this);
                    isSorted = true;
                    break;
                }
            }
        }
        else
        {
            IsAddCompleted = false;
            GameManager.Instance.ListItemPicked.AddDic(E_TypeObj, this);
            MoveToTarget(GameManager.Instance.ListItemPicked.L_Items[0].transform);
            Rotate = transform.DORotate(Vector3.zero, 0.3f).SetEase(Ease.Linear);
            IsFirstElement = true;
        }
        if (!isSorted && !IsFirstElement)
        {
            IsAddCompleted = false;
            GameManager.Instance.ListItemPicked.AddDic(E_TypeObj, this);
            int index = GameManager.Instance.ListItemPicked.L_Save1.Count - 1;
            MoveToTarget(GameManager.Instance.ListItemPicked.L_Items[index].transform);
        }

    }

    public void MoveToTargetWithUI(Transform uiTarget, Transform finalTarget, System.Action complete = null)
    {
        Move = transform.DOMove(uiTarget.position, 0.3f).SetEase(Ease.Linear);

        if (GameManager.Instance.ListItemPicked.L_Save1.Count > 0)
        {
            Object firstElement = GameManager.Instance.ListItemPicked.L_Save1[0];

            Vector3 firstElementRotation = firstElement.transform.rotation.eulerAngles;

            Rotate = transform.DORotate(firstElementRotation, 0.3f).SetEase(Ease.Linear);
        }




        Move.OnComplete(() =>
        {
            Scale = transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.2f).SetEase(Ease.Linear);
            transform.position = finalTarget.position + new Vector3(0f, 1f, 0f);
            complete?.Invoke();
            GameManager.Instance.L_BrickInLevel.Remove(this);
            GameManager.Instance.ListItemPicked.SortElement();
            IsAddCompleted = true;
            GameManager.Instance.ListItemPicked.CheckDic(this);
            GameManager.Instance.SetLayerRecursively(transform.gameObject);

        });

        IsMoved = true;
        transform.parent = null;
    }

    public void MoveToTarget(Transform target, System.Action complete = null)
    {
        int index = GameManager.Instance.ListItemPicked.L_Items.IndexOf(target.gameObject);
        Transform uiTarget = GameManager.Instance.ListItemPicked.L_Items[index].transform;
        MoveToTargetWithUI(uiTarget, target, complete);
    }

    public bool IsComplete()
    {
        return Move.IsComplete();
    }

    public void KillTween()
    {
        Move?.Kill();
        Rotate?.Kill();
        Rotation?.Kill();
        Scale?.Kill();
    }


    private void OnDisable()
    {
        KillTween();
    }

}
