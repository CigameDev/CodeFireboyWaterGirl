using UnityEngine;


using Intelligences;
using Health;
public class TrongPham : DadAndMom
{
    private void Start()
    {
        this.CryHelloWorld();
    }
    void Update()
    {
        if (this.Alive)
        {
            this.CodeAndFixbug();
        }
        else
        {
            this.MissionComplete();
        }
    }
}



public  class DadAndMom : MonoBehaviour
{
    public void OnSpawn() { }
    public void CryHelloWorld() { }
    public bool Alive;
    public void CodeAndFixbug() { }
    public void MissionComplete() { }

    public State state;
    public Work work;
}


namespace Intelligences
{
    public enum State { }
}
namespace Health
{
    public enum Work { }
}
namespace Energy.Sleep
{
    public enum Fruit { }
}