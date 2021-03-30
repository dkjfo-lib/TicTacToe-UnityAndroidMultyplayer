using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMenu
{
    void Open();
    void Close();
}

public class MenuToggler : MonoBehaviour, IMenu
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        animator.SetTrigger("Open");
    }

    public void Close()
    {
        animator.SetTrigger("Close");
    }
}
