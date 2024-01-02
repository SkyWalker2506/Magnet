using LevelSelection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollStepLogic
{
    private Scrollbar scrollBar;
    private Transform content;
    private float scrollPos = 0;
    private float[] pos;
    private LevelView[] levelViews;

    public ScrollStepLogic(ScrollRect scrollRect, bool useHorizontal)
    {
        scrollBar = useHorizontal ? scrollRect.horizontalScrollbar : scrollRect.verticalScrollbar;
        content = scrollRect.content;
        levelViews = content.GetComponentsInChildren<LevelView>();
        scrollBar.value = 0;
    }

    public void Update(ref int selectedLevel)
    {
        pos = new float[content.childCount];
        float distance = 1f / (pos.Length - 1f);
        float halfDistance = distance * 0.5f;

        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            scrollPos = scrollBar.value;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (IsScrollPosInRange(scrollPos, pos[i], halfDistance))
                {
                    scrollBar.value = Mathf.Lerp(scrollBar.value, pos[i], 0.1f);
                }
            }
        }

        for (int i = 0; i < pos.Length; i++)
        {
            int level = levelViews[i].levelData.Level;
            float scale = 0.8f;
            if (IsScrollPosInRange(scrollPos, pos[i], halfDistance))
            {
                
                
                scale = 1f;
                if (selectedLevel != level)
                {
                    selectedLevel = level;
                    levelViews[i].OnFocus();
                }

            }
            else
            {
                if (selectedLevel+1 == level || selectedLevel -1 == level)
                {
                    levelViews[i].OnUnfocus();
                }

            }
            levelViews[i].transform.localScale = Vector2.Lerp(levelViews[i].transform.localScale, Vector3.one * scale, 0.1f);

        }
    }
    public bool IsScrollPosInRange(float scrollPos, float pos, float halfDistance)
    {
        return (scrollPos < pos + halfDistance && scrollPos > pos - halfDistance);
            
    }
}
