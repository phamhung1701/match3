using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Relic : MonoBehaviour
{
    public string relic_name;
    public string description;
    public int price;

    public virtual void Action()
    {

    }
}
