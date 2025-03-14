using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal.VersionControl;
using UnityEngine;

public class ListItemPicked : MonoBehaviour
{
    public List<GameObject> L_Items;
    public Dictionary<_ObjectsType, List<Object>> Dic_TypeCheck = new Dictionary<_ObjectsType, List<Object>>();


    [Header("Sort")]
    [SerializeField] public List<Object> L_Save1;

    private Sequence sequence;

    int TweenSequece = 0;

    public void AddDic(_ObjectsType Type, Object brick)
    {
        if (Dic_TypeCheck.ContainsKey(Type))
        {
            IncreaseDic(Type, brick);
        }
        else
        {
            List<Object> L_Bricks = new List<Object>();
            L_Bricks.Add(brick);
            Dic_TypeCheck.Add(Type, L_Bricks);
            L_Save1.Add(brick);
        }
    }

    void IncreaseDic(_ObjectsType Type, Object brick)
    {
        List<Object> L_Bricks = Dic_TypeCheck[Type];

        L_Bricks.Add(brick);
        Dic_TypeCheck[Type] = L_Bricks;
    }

    public void CheckDic(Object brick)
    {

        List<Object> L_Bricks = Dic_TypeCheck[brick.E_TypeObj];


        if (L_Bricks.Count >= 3)
        {

            TweenSequece = 0;
            GameManager.Instance.isSort = false;
            if (!L_Bricks[2].IsComplete()) return;

            foreach (Object obj in L_Bricks)
            {
                obj.transform.parent = null;

                L_Save1.Remove(obj);
            }

            TweenSequece++;

            if (TweenSequece == 1)
            {
                sequence = DOTween.Sequence();


                Tweener moveUp1 = L_Bricks[0].transform.DOMoveY(L_Bricks[0].transform.position.y + 0.8f, 0.4f);
                Tweener moveUp2 = L_Bricks[1].transform.DOMoveY(L_Bricks[1].transform.position.y + 0.8f, 0.4f);
                Tweener moveUp3 = L_Bricks[2].transform.DOMoveY(L_Bricks[2].transform.position.y + 0.8f, 0.4f);
                Tweener Move1 = L_Bricks[0].transform.DOMoveX(L_Bricks[1].transform.position.x, 0.4f).SetEase(Ease.InBack);
                Tweener Move2 = L_Bricks[2].transform.DOMoveX(L_Bricks[1].transform.position.x, 0.4f).SetEase(Ease.InBack);

                sequence.Append(moveUp1);
                sequence.Join(moveUp2);
                sequence.Join(moveUp3);
                sequence.Append(Move1);
                sequence.Join(Move2);

                sequence.OnComplete(() =>
                {

                    for (int i = 2; i >= 0; i--)
                    {
                        L_Bricks[i].KillTween();
                        Destroy(L_Bricks[i].gameObject);
                        L_Save1.Remove(L_Bricks[i]);
                        Dic_TypeCheck[brick.E_TypeObj].Remove(L_Bricks[i]);
                        
                    }

                    GameManager.Instance.isSort = true;

                    SortElementTween();

                    if (Dic_TypeCheck[brick.E_TypeObj].Count <= 0)
                    {
                        Dic_TypeCheck.Remove(brick.E_TypeObj);
                    }


                    if (GameManager.Instance.L_BrickInLevel.Count == 0)
                    {
                        Debug.Log("Win");
                        GameManager.Instance.M_UImanager.showMenuWin();
                    }

                    moveUp1.Kill();
                    moveUp2.Kill();
                    moveUp3.Kill();
                    Move1.Kill();
                    Move2.Kill();

                    
                });
            }
        }
        else
        {
            if (CheckIsFullItem() && brick.IsAddCompleted)
            {
                List<_ObjectsType> uniqueTypeIdPairs = new List<_ObjectsType>();

                for (int i = 0; i < L_Save1.Count; i++)
                {
                    var type = L_Save1[i].E_TypeObj;


                    if (!uniqueTypeIdPairs.Contains(type))
                    {
                        uniqueTypeIdPairs.Add(type);
                    }
                }

                for (int i = 0; i < uniqueTypeIdPairs.Count; i++)
                {
                    int count = GetAmoutBrickInListItem(uniqueTypeIdPairs[i]);

                    if (count >= 3)
                    {
                        return;
                    }
                }
                Debug.Log("YouLose");
                GameManager.Instance.M_UImanager.showMenuLose();
            }
        }

       
    }

    public int GetPosMoveTo(_ObjectsType e_TypeBrick)
    {
        if (L_Save1 == null) return -1;

        if (L_Save1.Count == 5) return -1;


        for (int i = L_Save1.Count - 1; i >= 0; i--)
        {
            if (L_Save1[i].E_TypeObj == e_TypeBrick)
            {
                return i + 1;
            }
        }
        return -1;
    }

    public void SortAndMoveElement(Object brick)
    {
        int k = GetPosMoveTo(brick.E_TypeObj);
        if (k > L_Items.Count) return;

        brick.MoveToTarget(L_Items[k].transform);

        for (int i = k; i < L_Save1.Count; i++)
        {
            L_Save1[i].transform.DOMove(L_Items[i + 1].transform.position + new Vector3(0f, 0.1f, 0f), 0.2f).SetEase(Ease.Linear);
        }

        Object[] Newlist = new Object[L_Save1.Count + 1];
        int vitrichen = k;
        for (int i = 0; i < vitrichen; i++)
        {
            Newlist[i] = L_Save1[i];
        }
        Newlist[k] = brick;
        for (int j = vitrichen + 1; j < Newlist.Length; j++)
        {
            Newlist[j] = L_Save1[j - 1];
        }
        L_Save1 = new List<Object>(Newlist);
    }

    public void SortElement()
    {
        if (L_Save1.Count > L_Items.Count) return;

        for (int i = 0; i < L_Save1.Count; i++)
        {
            L_Save1[i].transform.position = L_Items[i].transform.position + new Vector3(0f, 0.1f, 0f);
        }
    }

    public void SortElementTween()
    {
        if (L_Save1.Count > L_Items.Count) return;

        for (int i = 0; i < L_Save1.Count; i++)
        {
            int index = i;
            L_Save1[i].transform.DOMove(L_Items[i].transform.position + new Vector3(0f, 0.1f, 0f), 0.3f)
                .SetEase(Ease.Linear);
        }
        return;
    }

    public void Move2BrickToStartPos()
    {
        if (L_Save1.Count > 0)
        {
            var brick = L_Save1[L_Save1.Count - 1];
            var typeKey = (brick.E_TypeObj);

            if (Dic_TypeCheck.ContainsKey(typeKey))
            {
                Dic_TypeCheck[typeKey].Remove(brick);
                if (Dic_TypeCheck[typeKey].Count == 0)
                {
                    Dic_TypeCheck.Remove(typeKey);
                }
            }

            Tweener moveTween = brick.transform.DOMove(brick.startPos2, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                brick.Rotation?.Kill();
                brick.IsMoved = false;
                brick.IsAddCompleted = false;
                brick.IsFirstElement = false;
                brick.transform.rotation = brick.RotationBase;
                brick.transform.localScale = Vector3.one;
                GameManager.Instance.isSort = true;
                GameManager.Instance.L_BrickInLevel.Add(brick);
                L_Save1.Remove(brick);

            });
        }
    }

    public void MoveAllBrick()
    {

        if (L_Save1.Count == 0) return;


        for (int i = L_Save1.Count - 1; i >= 0; i--)
        {
            var brick = L_Save1[i];
            var typeKey = brick.E_TypeObj;
            if (Dic_TypeCheck.ContainsKey(typeKey))
            {
                Dic_TypeCheck[typeKey].Remove(brick);
                if (Dic_TypeCheck[typeKey].Count == 0)
                {
                    Dic_TypeCheck.Remove(typeKey);
                }
            }

            brick.transform.DOMove(brick.startPos2, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
            {
                brick.Rotation?.Kill();
                brick.IsMoved = false;
                brick.IsAddCompleted = false;
                brick.IsFirstElement = false;
                brick.transform.rotation = brick.RotationBase;
                brick.transform.localScale = Vector3.one;
                GameManager.Instance.isSort = true;
                GameManager.Instance.L_BrickInLevel.Add(brick);
            });


            L_Save1.RemoveAt(i);
        }
    }


    public int GetAmoutBrickInListItem(_ObjectsType Type)
    {
        int Count = 0;
        for (int i = 0; i < L_Save1.Count; i++)
        {
            if (Type == L_Save1[i].E_TypeObj)
            {
                Count++;
            }
        }
        return Count;
    }

    public bool CheckIsFullItem()
    {
        if (L_Save1.Count >= 5)
        {
            return true;
        }
        return false;
    }

    public bool IsAlmostFullItem()
    {
        return L_Save1.Count >= L_Items.Count - 1;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
