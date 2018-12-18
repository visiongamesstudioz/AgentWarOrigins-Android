using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DigitalOpus.MB.Core
{
    public class MB2_TexturePacker
    {
        public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;
        public bool doPowerOfTwoTextures = true;
        private ProbeResult bestRoot;
        public int atlasY;

        private static void printTree(Node r, string spc)
        {
            Debug.Log((spc + "Nd img=" + (r.img != null).ToString() + " r=" + r.r));
            if (r.child[0] != null)
                printTree(r.child[0], spc + "      ");
            if (r.child[1] == null)
                return;
            printTree(r.child[1], spc + "      ");
        }

        private static void flattenTree(Node r, List<Image> putHere)
        {
            if (r.img != null)
            {
                r.img.x = r.r.x;
                r.img.y = r.r.y;
                putHere.Add(r.img);
            }
            if (r.child[0] != null)
                flattenTree(r.child[0], putHere);
            if (r.child[1] == null)
                return;
            flattenTree(r.child[1], putHere);
        }

        private static void drawGizmosNode(Node r)
        {
            Vector3 extends=new Vector3((float)r.r.w, (float)r.r.h, 0.0f);

            Vector3 pos=new Vector3((float)r.r.x + (float)(extends.x / 2.0), (float)-r.r.y - (float)(extends.y / 2.0), 0.0f);
        
            Gizmos.color= Color.yellow;
            Gizmos.DrawWireCube(pos, extends);
            if (r.img != null)
            {
                Gizmos.color=new Color(Random.value, Random.value, Random.value);
                Vector3 extends1=new Vector3((float)r.img.w, (float)r.img.h, 0.0f);
                Vector3 pos1=new Vector3((float)r.r.x + (float)(extends.x / 2.0), (float)-r.r.y - (float)(extends.y / 2.0), 0.0f);

                Gizmos.DrawCube(pos1, extends1);
            }
            if (r.child[0] != null)
            {
                Gizmos.color= Color.red;
                drawGizmosNode(r.child[0]);
            }
            if (r.child[1] == null)
                return;
            Gizmos.color=Color.green;
           drawGizmosNode(r.child[1]);
        }

        private static Texture2D createFilledTex(Color c, int w, int h)
        {
            Texture2D texture2D = new Texture2D(w, h);
            for (int index1 = 0; index1 < w; ++index1)
            {
                for (int index2 = 0; index2 < h; ++index2)
                    texture2D.SetPixel(index1, index2, c);
            }
            texture2D.Apply();
            return texture2D;
        }

        public void DrawGizmos()
        {
            if (this.bestRoot == null)
                return;
            drawGizmosNode(this.bestRoot.root);
            Gizmos.color = Color.yellow;
            Vector3 vector3_1=new Vector3((float)bestRoot.outW, (float)-this.bestRoot.outH, 0.0f);

            Vector3 vector3_2=new Vector3((float)(vector3_1.x / 2.0), (float)(vector3_1.y / 2.0), 0.0f);

            Gizmos.DrawWireCube(vector3_2, vector3_1);
        }

        private bool ProbeSingleAtlas(Image[] imgsToAdd, int idealAtlasW, int idealAtlasH, float imgArea, int maxAtlasDim, ProbeResult pr)
        {
            Node r = new Node(NodeType.maxDim);
            r.r = new PixRect(0, 0, idealAtlasW, idealAtlasH);
            for (int index = 0; index < imgsToAdd.Length; ++index)
            {
                if (r.Insert(imgsToAdd[index], false) == null)
                    return false;
                if (index == imgsToAdd.Length - 1)
                {
                    int x = 0;
                    int y = 0;
                    this.GetExtent(r, ref x, ref y);
                    int outw = x;
                    int outh = y;
                    bool fits;
                    float e;
                    float sq;
                    if (this.doPowerOfTwoTextures)
                    {
                        outw = Mathf.Min(CeilToNearestPowerOfTwo(x), maxAtlasDim);
                        outh = Mathf.Min(CeilToNearestPowerOfTwo(y), maxAtlasDim);
                        if (outh < outw / 2)
                            outh = outw / 2;
                        if (outw < outh / 2)
                            outw = outh / 2;
                        fits = x <= maxAtlasDim && y <= maxAtlasDim;
                        float num1 = Mathf.Max(1f, (float)x / (float)maxAtlasDim);
                        float num2 = Mathf.Max(1f, (float)y / (float)maxAtlasDim);
                        float num3 = (float)outw * num1 * (float)outh * num2;
                        e = (float)(1.0 - ((double)num3 - (double)imgArea) / (double)num3);
                        sq = 1f;
                    }
                    else
                    {
                        e = (float)(1.0 - ((double)(x * y) - (double)imgArea) / (double)(x * y));
                        sq = x >= y ? (float)y / (float)x : (float)x / (float)y;
                        fits = x <= maxAtlasDim && y <= maxAtlasDim;
                    }
                    pr.Set(x, y, outw, outh, r, fits, e, sq);
                    if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                        MB2_Log.LogDebug("Probe success efficiency w=" + x + " h=" + y + " e=" + e + " sq=" + sq + " fits=" + fits.ToString());
                    return true;
                }
            }
            Debug.LogError("Should never get here.");
            return false;
        }

        private bool ProbeMultiAtlas(Image[] imgsToAdd, int idealAtlasW, int idealAtlasH, float imgArea, int maxAtlasDim, ProbeResult pr)
        {
            int num = 0;
            Node node1 = new Node(NodeType.maxDim);
            node1.r = new PixRect(0, 0, idealAtlasW, idealAtlasH);
            for (int index = 0; index < imgsToAdd.Length; ++index)
            {
                if (node1.Insert(imgsToAdd[index], false) == null)
                {
                    if (imgsToAdd[index].x > idealAtlasW && imgsToAdd[index].y > idealAtlasH)
                        return false;
                    Node node2 = new Node(NodeType.Container);
                    node2.r = new PixRect(0, 0, node1.r.w + idealAtlasW, idealAtlasH);
                    node2.child[1] = new Node(NodeType.maxDim)
                    {
                        r = new PixRect(node1.r.w, 0, idealAtlasW, idealAtlasH)
                    };
                    node2.child[0] = node1;
                    node1 = node2;
                    node1.Insert(imgsToAdd[index], false);
                    ++num;
                }
            }
            pr.numAtlases = num;
            pr.root = node1;
            pr.totalAtlasArea = (float)(num * maxAtlasDim * maxAtlasDim);
            if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                MB2_Log.LogDebug("Probe success efficiency numAtlases=" + num + " totalArea=" + pr.totalAtlasArea);
            return true;
        }

        private void GetExtent(Node r, ref int x, ref int y)
        {
            if (r.img != null)
            {
                if (r.r.x + r.img.w > x)
                    x = r.r.x + r.img.w;
                if (r.r.y + r.img.h > y)
                    y = r.r.y + r.img.h;
            }
            if (r.child[0] != null)
                this.GetExtent(r.child[0], ref x, ref y);
            if (r.child[1] == null)
                return;
            this.GetExtent(r.child[1], ref x, ref y);
        }

        private int StepWidthHeight(int oldVal, int step, int maxDim)
        {
            if (this.doPowerOfTwoTextures && oldVal < maxDim)
                return oldVal * 2;
            int num = oldVal + step;
            if (num > maxDim && oldVal < maxDim)
                num = maxDim;
            return num;
        }

        public static int RoundToNearestPositivePowerOfTwo(int x)
        {
            int num = (int)Mathf.Pow(2f, (float)Mathf.RoundToInt(Mathf.Log((float)x) / Mathf.Log(2f)));
            if (num == 0 || num == 1)
                num = 2;
            return num;
        }

        public static int CeilToNearestPowerOfTwo(int x)
        {
            int num = (int)Mathf.Pow(2f, Mathf.Ceil(Mathf.Log((float)x) / Mathf.Log(2f)));
            if (num == 0 || num == 1)
                num = 2;
            return num;
        }

        public AtlasPackingResult[] GetRects(List<Vector2> imgWidthHeights, int maxDimension, int padding)
        {
            return this.GetRects(imgWidthHeights, maxDimension, padding, false);
        }

        public AtlasPackingResult[] GetRects(List<Vector2> imgWidthHeights, int maxDimension, int padding, bool doMultiAtlas)
        {
            if (doMultiAtlas)
                return _GetRectsMultiAtlas(imgWidthHeights, maxDimension, padding, 2 + padding * 2, 2 + padding * 2, 2 + padding * 2, 2 + padding * 2);
            AtlasPackingResult rectsSingleAtlas = this._GetRectsSingleAtlas(imgWidthHeights, maxDimension, padding, 2 + padding * 2, 2 + padding * 2, 2 + padding * 2, 2 + padding * 2, 0);
            if (rectsSingleAtlas == null)
                return (AtlasPackingResult[])null;
            return new AtlasPackingResult[1] { rectsSingleAtlas };
        }

        private AtlasPackingResult _GetRectsSingleAtlas(List<Vector2> imgWidthHeights, int maxDimension, int padding, int minImageSizeX, int minImageSizeY, int masterImageSizeX, int masterImageSizeY, int recursionDepth)
        {
            if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(string.Format("_GetRects numImages={0}, maxDimension={1}, padding={2}, minImageSizeX={3}, minImageSizeY={4}, masterImageSizeX={5}, masterImageSizeY={6}, recursionDepth={7}", imgWidthHeights.Count, maxDimension, padding, minImageSizeX, minImageSizeY, masterImageSizeX, masterImageSizeY, recursionDepth));
            if (recursionDepth > 10)
            {
                if (this.LOG_LEVEL >= MB2_LogLevel.error)
                    Debug.LogError("Maximum recursion depth reached. Couldn't find packing for these textures.");
                return (AtlasPackingResult)null;
            }
            float imgArea = 0.0f;
            int num1 = 0;
            int num2 = 0;
            Image[] imageArray = new Image[imgWidthHeights.Count];
            for (int id = 0; id < imageArray.Length; ++id)
            {
                int x = (int)imgWidthHeights[id].x;
                int y = (int)imgWidthHeights[id].y;
                Image image = imageArray[id] = new Image(id, x, y, padding, minImageSizeX, minImageSizeY);
                imgArea += (float)(image.w * image.h);
                num1 = Mathf.Max(num1, image.w);
                num2 = Mathf.Max(num2, image.h);
            }
            if ((double)num2 / (double)num1 > 2.0)
            {
                if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                    MB2_Log.LogDebug("Using height Comparer");
                Array.Sort<Image>(imageArray, (IComparer<Image>)new ImageHeightComparer());
            }
            else if ((double)num2 / (double)num1 < 0.5)
            {
                if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                    MB2_Log.LogDebug("Using width Comparer");
                Array.Sort<Image>(imageArray, (IComparer<Image>)new ImageWidthComparer());
            }
            else
            {
                if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                    MB2_Log.LogDebug("Using area Comparer");
                Array.Sort<Image>(imageArray, (IComparer<Image>)new ImageAreaComparer());
            }
            int x1 = (int)Mathf.Sqrt(imgArea);
            int x2;
            int x3;
            if (this.doPowerOfTwoTextures)
            {
                x3 = x2 = RoundToNearestPositivePowerOfTwo(x1);
                if (num1 > x3)
                    x3 = CeilToNearestPowerOfTwo(x3);
                if (num2 > x2)
                    x2 = CeilToNearestPowerOfTwo(x2);
            }
            else
            {
                x3 = x1;
                x2 = x1;
                if (num1 > x1)
                {
                    x3 = num1;
                    x2 = Mathf.Max(Mathf.CeilToInt(imgArea / (float)num1), num2);
                }
                if (num2 > x1)
                {
                    x3 = Mathf.Max(Mathf.CeilToInt(imgArea / (float)num2), num1);
                    x2 = num2;
                }
            }
            if (x3 == 0)
                x3 = 4;
            if (x2 == 0)
                x2 = 4;
            int step1 = (int)((double)x3 * 0.150000005960464);
            int step2 = (int)((double)x2 * 0.150000005960464);
            if (step1 == 0)
                step1 = 1;
            if (step2 == 0)
                step2 = 1;
            int num3 = 2;
            int num4 = x2;
            while (num3 >= 1 && num4 < x1 * 1000)
            {
                bool flag = false;
                num3 = 0;
                int num5 = x3;
                while (!flag && num5 < x1 * 1000)
                {
                    ProbeResult pr = new ProbeResult();
                    if (this.LOG_LEVEL >= MB2_LogLevel.trace)
                        Debug.Log(("Probing h=" + num4 + " w=" + num5));
                    if (this.ProbeSingleAtlas(imageArray, num5, num4, imgArea, maxDimension, pr))
                    {
                        flag = true;
                        if (this.bestRoot == null)
                            this.bestRoot = pr;
                        else if ((double)pr.GetScore(this.doPowerOfTwoTextures) > (double)this.bestRoot.GetScore(this.doPowerOfTwoTextures))
                            this.bestRoot = pr;
                    }
                    else
                    {
                        ++num3;
                        num5 = this.StepWidthHeight(num5, step1, maxDimension);
                        if (this.LOG_LEVEL >= MB2_LogLevel.trace)
                            MB2_Log.LogDebug("increasing Width h=" + num4 + " w=" + num5);
                    }
                }
                num4 = this.StepWidthHeight(num4, step2, maxDimension);
                if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                    MB2_Log.LogDebug("increasing Height h=" + num4 + " w=" + num5);
            }
            if (this.bestRoot == null)
                return (AtlasPackingResult)null;
            int outW;
            int outH;
            if (this.doPowerOfTwoTextures)
            {
                outW = Mathf.Min(CeilToNearestPowerOfTwo(this.bestRoot.w), maxDimension);
                outH = Mathf.Min(CeilToNearestPowerOfTwo(this.bestRoot.h), maxDimension);
                if (outH < outW / 2)
                    outH = outW / 2;
                if (outW < outH / 2)
                    outW = outH / 2;
            }
            else
            {
                outW = Mathf.Min(this.bestRoot.w, maxDimension);
                outH = Mathf.Min(this.bestRoot.h, maxDimension);
            }
            this.bestRoot.outW = outW;
            this.bestRoot.outH = outH;
            if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(("Best fit found: atlasW=" + outW + " atlasH" + outH + " w=" + this.bestRoot.w + " h=" + this.bestRoot.h + " efficiency=" + this.bestRoot.efficiency + " squareness=" + this.bestRoot.squareness + " fits in max dimension=" + this.bestRoot.largerOrEqualToMaxDim.ToString()));
            List<Image> imageList = new List<Image>();
            flattenTree(this.bestRoot.root, imageList);
            imageList.Sort((IComparer<Image>)new ImgIDComparer());
            AtlasPackingResult fitMaxDim = this.ScaleAtlasToFitMaxDim(this.bestRoot, imgWidthHeights, imageList, maxDimension, padding, minImageSizeX, minImageSizeY, masterImageSizeX, masterImageSizeY, outW, outH, recursionDepth);
            if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                MB2_Log.LogDebug(string.Format("Done GetRects atlasW={0} atlasH={1}", this.bestRoot.w, this.bestRoot.h));
            return fitMaxDim;
        }

        private AtlasPackingResult ScaleAtlasToFitMaxDim(ProbeResult root, List<Vector2> imgWidthHeights, List<Image> images, int maxDimension, int padding, int minImageSizeX, int minImageSizeY, int masterImageSizeX, int masterImageSizeY, int outW, int outH, int recursionDepth)
        {
            int minImageSizeX1 = minImageSizeX;
            int minImageSizeY1 = minImageSizeY;
            bool flag = false;
            float num1 = (float)padding / (float)outW;
            if (root.w > maxDimension)
            {
                num1 = (float)padding / (float)maxDimension;
                float num2 = (float)maxDimension / (float)root.w;
                if (this.LOG_LEVEL >= MB2_LogLevel.warn)
                    Debug.LogWarning(("Packing exceeded atlas width shrinking to " + num2));
                for (int index = 0; index < images.Count; ++index)
                {
                    Image image = images[index];
                    if ((double)image.w * (double)num2 < (double)masterImageSizeX)
                    {
                        if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                            Debug.Log("Small images are being scaled to zero. Will need to redo packing with larger minTexSizeX.");
                        flag = true;
                        minImageSizeX1 = Mathf.CeilToInt((float)minImageSizeX / num2);
                    }
                    int num3 = (int)((double)(image.x + image.w) * (double)num2);
                    image.x = (int)((double)num2 * (double)image.x);
                    image.w = num3 - image.x;
                }
                outW = maxDimension;
            }
            float num4 = (float)padding / (float)outH;
            if (root.h > maxDimension)
            {
                num4 = (float)padding / (float)maxDimension;
                float num2 = (float)maxDimension / (float)root.h;
                if (this.LOG_LEVEL >= MB2_LogLevel.warn)
                    Debug.LogWarning(("Packing exceeded atlas height shrinking to " + num2));
                for (int index = 0; index < images.Count; ++index)
                {
                    Image image = images[index];
                    if ((double)image.h * (double)num2 < (double)masterImageSizeY)
                    {
                        if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                            Debug.Log("Small images are being scaled to zero. Will need to redo packing with larger minTexSizeY.");
                        flag = true;
                        minImageSizeY1 = Mathf.CeilToInt((float)minImageSizeY / num2);
                    }
                    int num3 = (int)((double)(image.y + image.h) * (double)num2);
                    image.y = (int)((double)num2 * (double)image.y);
                    image.h = num3 - image.y;
                }
                outH = maxDimension;
            }
            if (!flag)
            {
                AtlasPackingResult atlasPackingResult = new AtlasPackingResult();
                atlasPackingResult.rects = new Rect[images.Count];
                atlasPackingResult.srcImgIdxs = new int[images.Count];
                atlasPackingResult.atlasX = outW;
                atlasPackingResult.atlasY = outH;
                atlasPackingResult.usedW = -1;
                atlasPackingResult.usedH = -1;
                for (int index = 0; index < images.Count; ++index)
                {
                    Image image = images[index];
                    Rect rect = atlasPackingResult.rects[index] = new Rect((float)image.x / (float)outW + num1, (float)image.y / (float)outH + num4, (float)((double)image.w / (double)outW - (double)num1 * 2.0), (float)((double)image.h / (double)outH - (double)num4 * 2.0));
                    atlasPackingResult.srcImgIdxs[index] = image.imgId;
                    if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                    {
                        // ISSUE: explicit reference operation
                        // ISSUE: explicit reference operation
                        // ISSUE: explicit reference operation
                        // ISSUE: explicit reference operation
                        MB2_Log.LogDebug("Image: " + index + " imgID=" + image.imgId + " x=" + rect.x * (double)outW + " y=" + rect.y * (double)outH + " w=" + rect.width * (double)outW + " h=" + rect.height * (double)outH + " padding=" + padding);
                    }
                }
                return atlasPackingResult;
            }
            if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log("==================== REDOING PACKING ================");
            root = (ProbeResult)null;
            return this._GetRectsSingleAtlas(imgWidthHeights, maxDimension, padding, minImageSizeX1, minImageSizeY1, masterImageSizeX, masterImageSizeY, recursionDepth + 1);
        }

        private AtlasPackingResult[] _GetRectsMultiAtlas(List<Vector2> imgWidthHeights, int maxDimensionPassed, int padding, int minImageSizeX, int minImageSizeY, int masterImageSizeX, int masterImageSizeY)
        {
            if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(string.Format("_GetRects numImages={0}, maxDimension={1}, padding={2}, minImageSizeX={3}, minImageSizeY={4}, masterImageSizeX={5}, masterImageSizeY={6}", imgWidthHeights.Count, maxDimensionPassed, padding, minImageSizeX, minImageSizeY, masterImageSizeX, masterImageSizeY));
            float imgArea = 0.0f;
            int num1 = 0;
            int num2 = 0;
            Image[] imageArray = new Image[imgWidthHeights.Count];
            int num3 = maxDimensionPassed;
            if (this.doPowerOfTwoTextures)
                num3 = RoundToNearestPositivePowerOfTwo(num3);
            for (int id = 0; id < imageArray.Length; ++id)
            {
                int x = (int)imgWidthHeights[id].x;
                int y = (int)imgWidthHeights[id].y;
                int tw = Mathf.Min(x, num3 - padding * 2);
                int th = Mathf.Min(y, num3 - padding * 2);
                Image image = imageArray[id] = new Image(id, tw, th, padding, minImageSizeX, minImageSizeY);
                imgArea += (float)(image.w * image.h);
                num1 = Mathf.Max(num1, image.w);
                num2 = Mathf.Max(num2, image.h);
            }
            int idealAtlasH;
            int idealAtlasW;
            if (this.doPowerOfTwoTextures)
            {
                idealAtlasH = RoundToNearestPositivePowerOfTwo(num3);
                idealAtlasW = RoundToNearestPositivePowerOfTwo(num3);
            }
            else
            {
                idealAtlasH = num3;
                idealAtlasW = num3;
            }
            if (idealAtlasW == 0)
                idealAtlasW = 4;
            if (idealAtlasH == 0)
                idealAtlasH = 4;
            ProbeResult pr = new ProbeResult();
            Array.Sort<Image>(imageArray, (IComparer<Image>)new ImageHeightComparer());
            if (this.ProbeMultiAtlas(imageArray, idealAtlasW, idealAtlasH, imgArea, num3, pr))
                this.bestRoot = pr;
            Array.Sort<Image>(imageArray, (IComparer<Image>)new ImageWidthComparer());
            if (this.ProbeMultiAtlas(imageArray, idealAtlasW, idealAtlasH, imgArea, num3, pr) && (double)pr.totalAtlasArea < (double)this.bestRoot.totalAtlasArea)
                this.bestRoot = pr;
            Array.Sort<Image>(imageArray, (IComparer<Image>)new ImageAreaComparer());
            if (this.ProbeMultiAtlas(imageArray, idealAtlasW, idealAtlasH, imgArea, num3, pr) && (double)pr.totalAtlasArea < (double)this.bestRoot.totalAtlasArea)
                this.bestRoot = pr;
            if (this.bestRoot == null)
                return (AtlasPackingResult[])null;
            if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(("Best fit found: w=" + this.bestRoot.w + " h=" + this.bestRoot.h + " efficiency=" + this.bestRoot.efficiency + " squareness=" + this.bestRoot.squareness + " fits in max dimension=" + this.bestRoot.largerOrEqualToMaxDim.ToString()));
            List<AtlasPackingResult> atlasPackingResultList = new List<AtlasPackingResult>();
            List<Node> nodeList = new List<Node>();
            Stack<Node> nodeStack = new Stack<Node>();
            for (Node root = this.bestRoot.root; root != null; root = root.child[0])
                nodeStack.Push(root);
            while (nodeStack.Count > 0)
            {
                Node node1 = nodeStack.Pop();
                if (node1.isFullAtlas == NodeType.maxDim)
                    nodeList.Add(node1);
                if (node1.child[1] != null)
                {
                    for (Node node2 = node1.child[1]; node2 != null; node2 = node2.child[0])
                        nodeStack.Push(node2);
                }
            }
            for (int index1 = 0; index1 < nodeList.Count; ++index1)
            {
                List<Image> putHere = new List<Image>();
                flattenTree(nodeList[index1], putHere);
                Rect[] rectArray = new Rect[putHere.Count];
                int[] numArray = new int[putHere.Count];
                for (int index2 = 0; index2 < putHere.Count; ++index2)
                {
                    rectArray[index2] = new Rect((float)(putHere[index2].x - nodeList[index1].r.x), (float)putHere[index2].y, (float)putHere[index2].w, (float)putHere[index2].h);
                    numArray[index2] = putHere[index2].imgId;
                }
                AtlasPackingResult rr = new AtlasPackingResult();
                this.GetExtent(nodeList[index1], ref rr.usedW, ref rr.usedH);
                rr.usedW -= nodeList[index1].r.x;
                int w = nodeList[index1].r.w;
                int h = nodeList[index1].r.h;
                int num4;
                int num5;
                if (this.doPowerOfTwoTextures)
                {
                    num4 = Mathf.Min(CeilToNearestPowerOfTwo(rr.usedW), nodeList[index1].r.w);
                    num5 = Mathf.Min(CeilToNearestPowerOfTwo(rr.usedH), nodeList[index1].r.h);
                    if (num5 < num4 / 2)
                        num5 = num4 / 2;
                    if (num4 < num5 / 2)
                        num4 = num5 / 2;
                }
                else
                {
                    num4 = rr.usedW;
                    num5 = rr.usedH;
                }
                rr.atlasY = num5;
                rr.atlasX = num4;
                rr.rects = rectArray;
                rr.srcImgIdxs = numArray;
                atlasPackingResultList.Add(rr);
                this.normalizeRects(rr, padding);
                if (this.LOG_LEVEL >= MB2_LogLevel.debug)
                    MB2_Log.LogDebug(string.Format("Done GetRects "));
            }
            return atlasPackingResultList.ToArray();
        }

        private void normalizeRects(AtlasPackingResult rr, int padding)
        {
            for (int index = 0; index < rr.rects.Length; ++index)
            {

                rr.rects[index].x = rr.rects[index].x + (float)padding / rr.atlasX;
                rr.rects[index].y = rr.rects[index].y + (float)padding / rr.atlasY;
                        
                rr.rects[index].width=rr.rects[index].width -(float)(padding * 2) /
                rr.atlasX;

                rr.rects[index].height=rr.rects[index].height- (float)(padding * 2) /
                 rr.atlasY;
            }
        }

       private enum NodeType
        {
            Container,
            maxDim,
            regular,
        }

        class PixRect
        {
            public int x;
            public int y;
            public int w;
            public int h;

            public PixRect()
            {
            }

            public PixRect(int xx, int yy, int ww, int hh)
            {
                this.x = xx;
                this.y = yy;
                this.w = ww;
                this.h = hh;
            }

            public override string ToString()
            {
                return string.Format("x={0},y={1},w={2},h={3}", this.x, this.y, this.w, this.h);
            }
        }

        class Image
        {
            public int imgId;
            public int w;
            public int h;
            public int x;
            public int y;

            public Image(int id, int tw, int th, int padding, int minImageSizeX, int minImageSizeY)
            {
                this.imgId = id;
                this.w = Mathf.Max(tw + padding * 2, minImageSizeX);
                this.h = Mathf.Max(th + padding * 2, minImageSizeY);
            }

            public Image(Image im)
            {
                this.imgId = im.imgId;
                this.w = im.w;
                this.h = im.h;
                this.x = im.x;
                this.y = im.y;
            }
        }

         class ImgIDComparer : IComparer<Image>
        {
            public int Compare(Image x, Image y)
            {
                if (x.imgId > y.imgId)
                    return 1;
                return x.imgId == y.imgId ? 0 : -1;
            }
        }

         class ImageHeightComparer : IComparer<Image>
        {
            public int Compare(Image x, Image y)
            {
                if (x.h > y.h)
                    return -1;
                return x.h == y.h ? 0 : 1;
            }
        }

         class ImageWidthComparer : IComparer<Image>
        {
            public int Compare(Image x, Image y)
            {
                if (x.w > y.w)
                    return -1;
                return x.w == y.w ? 0 : 1;
            }
        }

         class ImageAreaComparer : IComparer<Image>
        {
            public int Compare(Image x, Image y)
            {
                int num1 = x.w * x.h;
                int num2 = y.w * y.h;
                if (num1 > num2)
                    return -1;
                return num1 == num2 ? 0 : 1;
            }
        }

         class ProbeResult
        {
            public int w;
            public int h;
            public int outW;
            public int outH;
            public Node root;
            public bool largerOrEqualToMaxDim;
            public float efficiency;
            public float squareness;
            public float totalAtlasArea;
            public int numAtlases;

            public void Set(int ww, int hh, int outw, int outh, Node r, bool fits, float e, float sq)
            {
                this.w = ww;
                this.h = hh;
                this.outW = outw;
                this.outH = outh;
                this.root = r;
                this.largerOrEqualToMaxDim = fits;
                this.efficiency = e;
                this.squareness = sq;
            }

            public float GetScore(bool doPowerOfTwoScore)
            {
                float num = this.largerOrEqualToMaxDim ? 1f : 0.0f;
                if (doPowerOfTwoScore)
                    return num * 2f + this.efficiency;
                return this.squareness + 2f * this.efficiency + num;
            }

            public void PrintTree()
            {
                printTree(this.root, "  ");
            }
        }

         class Node
        {
            public Node[] child = new Node[2];
            public NodeType isFullAtlas;
            public PixRect r;
            public Image img;

            public Node(NodeType rootType)
            {
                this.isFullAtlas = rootType;
            }

            private bool isLeaf()
            {
                return this.child[0] == null || this.child[1] == null;
            }

            public Node Insert(Image im, bool handed)
            {
                int index1;
                int index2;
                if (handed)
                {
                    index1 = 0;
                    index2 = 1;
                }
                else
                {
                    index1 = 1;
                    index2 = 0;
                }
                if (!this.isLeaf())
                {
                    Node node = this.child[index1].Insert(im, handed);
                    if (node != null)
                        return node;
                    return this.child[index2].Insert(im, handed);
                }
                if (this.img != null || (this.r.w < im.w || this.r.h < im.h))
                    return (Node)null;
                if (this.r.w == im.w && this.r.h == im.h)
                {
                    this.img = im;
                    return this;
                }
                this.child[index1] = new Node(NodeType.regular);
                this.child[index2] = new Node(NodeType.regular);
                if (this.r.w - im.w > this.r.h - im.h)
                {
                    this.child[index1].r = new PixRect(this.r.x, this.r.y, im.w, this.r.h);
                    this.child[index2].r = new PixRect(this.r.x + im.w, this.r.y, this.r.w - im.w, this.r.h);
                }
                else
                {
                    this.child[index1].r = new PixRect(this.r.x, this.r.y, this.r.w, im.h);
                    this.child[index2].r = new PixRect(this.r.x, this.r.y + im.h, this.r.w, this.r.h - im.h);
                }
                return this.child[index1].Insert(im, handed);
            }
        }
    }
}
