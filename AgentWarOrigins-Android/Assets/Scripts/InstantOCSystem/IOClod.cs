
using System;
using UnityEngine;
using System.Collections;
using System.Linq;

public class IOClod : IOCcomp {

	public bool Occludee;
	public float Lod1;
	public float Lod2;
	public float LodMargin;
	public bool LodOnly;

	private int currentLayer;
	private Vector3 hitPoint;
	private float lod_1;
	private float lod_2;
	private float lodMargin;
	private bool realtimeShadows;
	private IOCcam iocCam;
	private int counter;
	private Renderer[] rs0;
	private Renderer[] rs1;
	private Renderer[] rs2;
	private Renderer[] rs;
	private bool hidden;
	private int currentLod;
	private float prevDist;
	private float distOffset;
	private int lods;
	private float dt;
	private float hitTimeOffset;
	private float prevHitTime;
	private bool sleeping;
	private UnityEngine.Rendering.ShadowCastingMode[] originalShadowCastingMode;
	private float distanceFromCam;
	private float shadowDistance;
	private int frameInterval;
	private RaycastHit h;
	private Ray r;
	private bool visible;
	private Vector3 p;
	
	void Awake () {
		Init();
	}
	
	public override void Init () {
		try
		{
			iocCam =  Camera.main.GetComponent<IOCcam>();
			shadowDistance = QualitySettings.shadowDistance * 2f;
			currentLayer = gameObject.layer;
			prevDist = 0f;
			prevHitTime = Time.time;
			sleeping = true;
			h = new RaycastHit();
			this.enabled = true;
		}
		catch(Exception e)
		{
			this.enabled = false;
			Debug.Log(e.Message);
		}
	}
	
	void Start () {
	//	UpdateValues();


	    rs = GetComponentsInChildren<Renderer>(false).Where(
	        x => x.gameObject.GetComponent<Light>() == null
	    ).ToArray();
	    originalShadowCastingMode = new UnityEngine.Rendering.ShadowCastingMode[rs.Length];
	    for (int i = 0; i < rs.Length; i++)
	    {
	        originalShadowCastingMode[i] = rs[i].shadowCastingMode;
	    }

        //      if (LOD0)
        //{
        //	lods = 1;
        //    rs0 = LOD0.GetComponentsInChildren<Renderer>(false);

        //	originalShadowCastingMode = new UnityEngine.Rendering.ShadowCastingMode[rs0.Length];
        //	for(int i=0;i<rs0.Length;i++)
        //	{
        //		originalShadowCastingMode[i] = rs0[i].shadowCastingMode;
        //	}

        //	if(LOD1)
        //	{

        //              lods++;
        //	    rs1 = LOD1.GetComponentsInChildren<Renderer>(false);

        //		if(LOD2)
        //		{

        //                  lods++;
        //		    rs2 = LOD2.GetComponentsInChildren<Renderer>(false);
        //		}
        //	}
        //}
        //else
        //{
        //	lods = 0;

        //}

        Initialize();
	}
	public void Initialize() {
		if(iocCam.enabled == true)
		{
			HideAll();
		}
		else
		{
			this.enabled = false;
			ShowRenderers();
		}
		gameObject.layer = currentLayer;
	}
	//void Update () {
	//	frameInterval = Time.frameCount % 4;
	//	if(frameInterval == 0)
	//	{
	//		switch(LodOnly)
	//		{
	//		case false:
	//			if(!hidden && Time.frameCount - counter > iocCam.hideDelay)
	//			{
	//				switch(currentLod)
	//				{
	//				case 0:
	//					visible = rs0[0].isVisible;
	//					break;
	//				case 1:
	//					visible = rs1[0].isVisible;
	//					break;
	//				case 2:
	//					visible = rs2[0].isVisible;
	//					break;
	//				default:
	//					visible = rs[0].isVisible;
	//					break;
	//				}
	//				if((iocCam.preCullCheck && visible) | Occludee && visible)
	//				{
	//					p = transform.localToWorldMatrix.MultiplyPoint(hitPoint);
	//					r = new Ray(p, iocCam.transform.position - p);
	//					if(Physics.Raycast(r, out h, iocCam.viewDistance))
	//					{
	//						if(!h.collider.CompareTag(iocCam.tag))
	//						{
	//							Hide();
	//						}
	//						else
	//						{
	//							counter = Time.frameCount;
	//							if(Occludee & lods > 0)
	//							{
	//								ShowLod(h.distance);
	//							}
	//						}
	//					}
	//				}
	//				else
	//				{
	//					Hide();
	//				}
	//			}
	//			break;
	//		case true:
	//			if(!sleeping && Time.frameCount - counter > iocCam.hideDelay)
	//			{
	//				if(Occludee)
	//				{
	//					gameObject.layer = currentLayer;
	//					var p = transform.localToWorldMatrix.MultiplyPoint(hitPoint);
	//					r = new Ray(p, iocCam.transform.position - p);
	//					if(Physics.Raycast(r, out h, iocCam.viewDistance))
	//					{
	//						if(!h.collider.CompareTag(iocCam.tag))
	//						{
	//							ShowLod(3000f);
	//							sleeping = true;
	//						}
	//					}
	//				}
	//				else
	//				{
	//					ShowLod(3000f);
	//					sleeping = true;
	//				}
	//			}
	//			break;
	//		}
	//	}
	//	else if(realtimeShadows && frameInterval == 2)
	//	{
	//		distanceFromCam = Vector3.Distance(transform.position, iocCam.transform.position);
	//		if(hidden)
	//		{
	//			switch(lods)
	//			{
	//			case 0:
	//				if(distanceFromCam > shadowDistance)
	//				{
	//					if(rs[0].enabled)
	//					{
	//						for(int i=0;i<rs.Length;i++)
	//						{
	//							rs[i].enabled = false;
	//							rs[i].shadowCastingMode = originalShadowCastingMode[i];
	//						}
	//					}
	//				}
	//				else
	//				{
	//					if(!rs[0].enabled)
	//					{
	//						for(int i=0;i<rs.Length;i++)
	//						{
	//							rs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
	//							rs[i].enabled = true;
	//						}
	//					}
	//				}
	//				break;
	//			default:
	//				if(distanceFromCam > shadowDistance)
	//				{
	//					if(rs0[0].enabled)
	//					{
	//						for(int i=0;i<rs0.Length;i++)
	//						{
	//							rs0[i].enabled = false;
	//							rs0[i].shadowCastingMode = originalShadowCastingMode[i];
	//						}
	//					}
	//				}
	//				else
	//				{
	//					if(!rs0[0].enabled)
	//					{
	//						for(int i=0;i<rs0.Length;i++)
	//						{
	//							rs0[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
	//							rs0[i].enabled = true;
	//						}
	//					}
	//				}
	//				break;
	//			}
	//		}
	//	}
	//}
	
	public void UpdateValues () {
		if(Lod1 != 0)
		{
			lod_1 = Lod1;
		}
		else lod_1 = iocCam.lod1Distance;
		if(Lod2 != 0)
		{
			lod_2 = Lod2;
		}
		else lod_2 = iocCam.lod2Distance;
		if(LodMargin != 0)
		{
			lodMargin = LodMargin;
		}
		else lodMargin = iocCam.lodMargin;
		realtimeShadows = iocCam.realtimeShadows;
	}
	
	public override void UnHide(RaycastHit h)
	{
		counter = Time.frameCount;
		hitPoint = transform.worldToLocalMatrix.MultiplyPoint(h.point);
		if(hidden)
		{
			hidden = false;
            ShowRenderers();
			//ShowLod(h.distance);
			if(Occludee)
			{
				gameObject.layer = iocCam.occludeeLayer;
			}
		}
		else
		{
			if(lods > 0)
			{
				distOffset = prevDist - h.distance;
				hitTimeOffset = Time.time - prevHitTime;
				prevHitTime = Time.time;
				if(Mathf.Abs(distOffset) > lodMargin | hitTimeOffset > 1f)
				{
                    ShowRenderers();
					//ShowLod(h.distance);
					prevDist = h.distance;
					sleeping = false;
					if(Occludee)
					{
						gameObject.layer = iocCam.occludeeLayer;
					}
				}
			}
		}
	}

    public void ShowRenderers()
    {
        int i;

        if (rs != null)
        {
            for (i = 0; i < rs.Length; i++)
            {
                rs[i].enabled = true;
            }
        }
     
    }

    //public void ShowLOD()
    //{
    //    if (frameInterval == 0)
    //    {
    //        switch (LodOnly)
    //        {
    //            case false:
    //                if (!hidden && Time.frameCount - counter > iocCam.hideDelay)
    //                {
    //                    switch (currentLod)
    //                    {
    //                        case 0:
    //                            Debug.Log(transform.name);
    //                            visible = rs0[0].isVisible;
    //                            break;
    //                        case 1:
    //                            visible = rs1[0].isVisible;
    //                            break;
    //                        case 2:
    //                            visible = rs2[0].isVisible;
    //                            break;
    //                        default:
    //                            visible = rs[0].isVisible;
    //                            break;
    //                    }
    //                    if ((iocCam.preCullCheck && visible) | Occludee && visible)
    //                    {
    //                        p = transform.localToWorldMatrix.MultiplyPoint(hitPoint);
    //                        r = new Ray(p, iocCam.transform.position - p);
    //                        if (Physics.Raycast(r, out h, iocCam.viewDistance))
    //                        {
    //                            if (!h.collider.CompareTag(iocCam.tag))
    //                            {
    //                                Hide();
    //                            }
    //                            else
    //                            {
    //                                counter = Time.frameCount;
    //                                if (Occludee & lods > 0)
    //                                {
    //                                    ShowLod(h.distance);
    //                                }
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {
    //                        Hide();
    //                    }
    //                }
    //                break;
    //            case true:
    //                if (!sleeping && Time.frameCount - counter > iocCam.hideDelay)
    //                {
    //                    if (Occludee)
    //                    {
    //                        gameObject.layer = currentLayer;
    //                        var p = transform.localToWorldMatrix.MultiplyPoint(hitPoint);
    //                        r = new Ray(p, iocCam.transform.position - p);
    //                        if (Physics.Raycast(r, out h, iocCam.viewDistance))
    //                        {
    //                            if (!h.collider.CompareTag(iocCam.tag))
    //                            {
    //                                ShowLod(3000f);
    //                                sleeping = true;
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {
    //                        ShowLod(3000f);
    //                        sleeping = true;
    //                    }
    //                }
    //                break;
    //        }
    //    }
    //    else if (realtimeShadows && frameInterval == 2)
    //    {
    //        distanceFromCam = Vector3.Distance(transform.position, iocCam.transform.position);
    //        if (hidden)
    //        {
    //            switch (lods)
    //            {
    //                case 0:
    //                    if (distanceFromCam > shadowDistance)
    //                    {
    //                        if (rs[0].enabled)
    //                        {
    //                            for (int i = 0; i < rs.Length; i++)
    //                            {
    //                                rs[i].enabled = false;
    //                                rs[i].shadowCastingMode = originalShadowCastingMode[i];
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (!rs[0].enabled)
    //                        {
    //                            for (int i = 0; i < rs.Length; i++)
    //                            {
    //                                rs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    //                                rs[i].enabled = true;
    //                            }
    //                        }
    //                    }
    //                    break;
    //                default:
    //                    if (distanceFromCam > shadowDistance)
    //                    {
    //                        if (rs0[0].enabled)
    //                        {
    //                            for (int i = 0; i < rs0.Length; i++)
    //                            {
    //                                rs0[i].enabled = false;
    //                                rs0[i].shadowCastingMode = originalShadowCastingMode[i];
    //                            }
    //                        }
    //                    }
    //                    else
    //                    {
    //                        if (!rs0[0].enabled)
    //                        {
    //                            for (int i = 0; i < rs0.Length; i++)
    //                            {
    //                                rs0[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    //                                rs0[i].enabled = true;
    //                            }
    //                        }
    //                    }
    //                    break;
    //            }
    //        }
    //    }
    //}

 //   public void ShowLod(float d)
	//{
	//	int i = 0;
	//	switch(lods)
	//	{
	//	case 0:
	//		currentLod = -1;
	//		break;
	//	case 2:
	//		if(d < lod_1)
	//		{
	//			currentLod = 0;
	//		}
	//		else
	//		{
	//			currentLod = 1;
	//		}
	//		break;
	//	case 3:
	//		if(d < lod_1)
	//		{
	//			currentLod = 0;
	//		}
	//		else if(d > lod_1 & d < lod_2)
	//		{
	//			currentLod = 1;
	//		}
	//		else
	//		{
	//			currentLod = 2;
	//		}
	//		break;
	//	}

	//	switch(currentLod)
	//	{
	//	case 0:
	//	    if (rs0 != null)
	//	    {

	//	        if (!LodOnly && rs0[0].enabled)
	//	        {
	//	            for (i = 0; i < rs0.Length; i++)
	//	            {
	//	                rs0[i].shadowCastingMode = originalShadowCastingMode[i];
	//	            }
	//	        }
 //               }
		
	//		else
	//		{
	//		    if (rs0 != null)
	//		    {
	//		        for (i = 0; i < rs0.Length; i++)
	//		        {
	//		            rs0[i].enabled = true;
	//		        }
 //                   }
				
	//		}

	//	    if (rs1 != null)
	//	    {
	//	        for (i = 0; i < rs1.Length; i++)
	//	        {
	//	            rs1[i].enabled = false;
	//	        }
 //               }
	
	//		if(lods == 3)
	//		{
	//			for(i=0;i<rs2.Length;i++)
	//			{
	//				rs2[i].enabled = false;
	//			}
	//		}
	//		break;
	//	case 1:
	//	    if (rs1 != null)
	//	    {
	//	        for (i = 0; i < rs1.Length; i++)
	//	        {
	//	            rs1[i].enabled = true;
	//	        }
	//	        for (i = 0; i < rs0.Length; i++)
	//	        {
	//	            rs0[i].enabled = false;
	//	            if (!LodOnly && realtimeShadows)
	//	            {
	//	                rs0[i].shadowCastingMode = originalShadowCastingMode[i];
	//	            }
	//	        }
 //               }

	//		if(lods == 3)
	//		{
	//		    if (rs2 != null)
	//		    {
	//		        for (i = 0; i < rs2.Length; i++)
	//		        {
	//		            rs2[i].enabled = false;
	//		        }
 //                   }
			
	//		}
	//		break;
	//	case 2:
	//	    if (rs2 != null)
	//	    {
	//	        for (i = 0; i < rs2.Length; i++)
	//	        {
	//	            rs2[i].enabled = true;
	//	        }
	//	        for (i = 0; i < rs0.Length; i++)
	//	        {
	//	            rs0[i].enabled = false;
	//	            if (!LodOnly && realtimeShadows)
	//	            {
	//	                rs0[i].shadowCastingMode = originalShadowCastingMode[i];
	//	            }
	//	        }
	//	        for (i = 0; i < rs1.Length; i++)
	//	        {
	//	            rs1[i].enabled = false;
	//	        }
 //               }
			
	//		break;
	//	default:

	//		if(!LodOnly && rs[0].enabled)
	//		{
	//			for(i=0;i<rs.Length;i++)
	//			{
	//				rs[i].shadowCastingMode = originalShadowCastingMode[i];
	//			}
	//		}
	//		else
	//		{
	//			for(i=0;i<rs.Length;i++)
	//			{
	//				rs[i].enabled = true;
	//			}
	//		}
	//		break;
	//	}
	//}
	public void Hide()
	{
		int i = 0;
		hidden = true;

	    if (realtimeShadows && distanceFromCam <= shadowDistance)
	    {
	        for (i = 0; i < rs.Length; i++)
	        {
	            rs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
	        }
	    }
	    else
	    {
	        for (i = 0; i < rs.Length; i++)
	        {
	            rs[i].enabled = false;
	        }
	    }

  //      switch (currentLod)
		//{
		//case 0:
		//    if (rs0 != null)
		//    {
		//        if (realtimeShadows && distanceFromCam <= shadowDistance)
		//        {
		//            for (i = 0; i < rs0.Length; i++)
		//            {
		//                rs0[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		//            }
		//        }
		//        else
		//        {
		//            for (i = 0; i < rs0.Length; i++)
		//            {
		//                rs0[i].enabled = false;
		//            }
		//        }
  //              }
	
		//	break;
		//case 1:
		//    if (rs1 != null)
		//    {
		//        for (i = 0; i < rs1.Length; i++)
		//        {
		//            rs1[i].enabled = false;
		//        }
  //              }
		
		//	break;
		//case 2:
		//    if (rs2 != null)
		//    {
		//        for (i = 0; i < rs2.Length; i++)
		//        {
		//            rs2[i].enabled = false;
		//        }
  //              }
		
		//	break;
		//default:
		//	if(realtimeShadows && distanceFromCam <= shadowDistance)
		//	{
		//		for(i=0;i<rs.Length;i++)
		//		{
		//			rs[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		//		}
		//	}
		//	else
		//	{
		//		for(i=0;i<rs.Length;i++)
		//		{
		//			rs[i].enabled = false;
		//		}
		//	}
		//	break;
		//}
		if(Occludee)
		{
			gameObject.layer = currentLayer;
		}
	}

	public void HideAll()
	{
		int i = 0;
		switch(LodOnly)
		{
		case false:
			hidden = true;
			if(/*lods == 0 && */rs != null)
			{
				for(i=0;i<rs.Length;i++)
				{
					rs[i].enabled = false;
					if(realtimeShadows)
					{
						rs[i].shadowCastingMode = originalShadowCastingMode[i];
					}
				}
			}
			//else
			//{

			//    if (rs0 != null)
			//    {


			//        for (i = 0; i < rs0.Length; i++)
			//        {
			//            rs0[i].enabled = false;
			//            if (realtimeShadows)
			//            {
			//                rs0[i].shadowCastingMode = originalShadowCastingMode[i];
			//            }
			//        }
   //                 }
	

			//    if (rs1 != null)
			//    {

			//        for (i = 0; i < rs1.Length; i++)
			//        {
			//            rs1[i].enabled = false;
			//        }
   //                 }
		
			//	if(rs2 != null)
			//	{
			//		for(i=0;i<rs2.Length;i++)
			//		{
			//			rs2[i].enabled = false;
			//		}
			//	}
			//}
			break;
		case true:
			prevHitTime = prevHitTime - 3f;
            ShowRenderers();
		//	ShowLod(3000f);
			break;
		}
	}
}
