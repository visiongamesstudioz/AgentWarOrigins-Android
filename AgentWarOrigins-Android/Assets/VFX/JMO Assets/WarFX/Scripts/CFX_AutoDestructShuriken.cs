using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
	public bool OnlyDeactivate;
	
	void OnEnable()
	{
      //  Invoke("CheckIfAlive",10);
	}

    void CheckIfAlive()
    {     
            if (OnlyDeactivate)
            {
#if UNITY_3_5
						this.gameObject.SetActiveRecursively(false);
					#else
                this.gameObject.SetActive(false);
#endif
            }
            else
                GameObject.Destroy(this.gameObject);        
    }
}
