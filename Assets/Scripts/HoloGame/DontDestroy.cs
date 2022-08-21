using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
        void Awake()
        {
             List<DontDestroy> objs = FindObjectsOfType<DontDestroy>().ToList();

            if (objs.FindAll(o=>o.name==gameObject.name).Count>1)
            {
               Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }
}
