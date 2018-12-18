using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace DigitalOpus.MB.Core
{
    [Serializable]
    public class MB3_AgglomerativeClustering
    {
        public List<item_s> items = new List<item_s>();
        public ClusterNode[] clusters;
        public bool wasCanceled;
        private const int MAX_PRIORITY_Q_SIZE = 2048;

        private float euclidean_distance(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        public bool agglomerate(ProgressUpdateCancelableDelegate progFunc)
        {
            wasCanceled = true;
            if (progFunc != null)
                wasCanceled = progFunc("Filling Priority Queue:", 0.0f);
            if (items.Count <= 1)
            {
                clusters = new ClusterNode[0];
                return false;
            }
            clusters = new ClusterNode[items.Count * 2 - 1];
            for (int index = 0; index < items.Count; ++index)
                clusters[index] = new ClusterNode(items[index], index);
            int count = items.Count;
            List<ClusterNode> unclustered = new List<ClusterNode>();
            for (int index = 0; index < count; ++index)
            {
                clusters[index].isUnclustered = true;
                unclustered.Add(clusters[index]);
            }
            int h = 0;
            new Stopwatch().Start();
            float num1 = 0.0f;
            long num2 = GC.GetTotalMemory(false) / 1000000L;
            PriorityQueue<float, ClusterDistance> pq = new PriorityQueue<float, ClusterDistance>();
            int num3 = 0;
            while (unclustered.Count > 1)
            {
                int num4 = 0;
                ++h;
                if (pq.Count == 0)
                {
                    ++num3;
                    long num5 = GC.GetTotalMemory(false) / 1000000L;
                    if (progFunc != null)
                        wasCanceled = progFunc("Refilling Q:" + (float)((items.Count - unclustered.Count) * 100.0 / (double)items.Count) + " unclustered:" + unclustered.Count + " inQ:" + pq.Count + " usedMem:" + num5, (float)((double)(items.Count - unclustered.Count) / (double)items.Count));
                    num1 = _RefillPriorityQWithSome(pq, unclustered, clusters, progFunc);
                    if (pq.Count == 0)
                        break;
                }
                KeyValuePair<float, ClusterDistance> keyValuePair = pq.Dequeue();
                while (!keyValuePair.Value.a.isUnclustered || !keyValuePair.Value.b.isUnclustered)
                {
                    if (pq.Count == 0)
                    {
                        ++num3;
                        long num5 = GC.GetTotalMemory(false) / 1000000L;
                        if (progFunc != null)
                            wasCanceled = progFunc("Creating clusters:" + (float)((items.Count - unclustered.Count) * 100.0 / (double)items.Count) + " unclustered:" + unclustered.Count + " inQ:" + pq.Count + " usedMem:" + num5, (float)((double)(items.Count - unclustered.Count) / (double)items.Count));
                        num1 = _RefillPriorityQWithSome(pq, unclustered, clusters, progFunc);
                        if (pq.Count == 0)
                            break;
                    }
                    keyValuePair = pq.Dequeue();
                    ++num4;
                }
                ++count;
                ClusterNode aa = new ClusterNode(keyValuePair.Value.a, keyValuePair.Value.b, count - 1, h, keyValuePair.Key, clusters);
                unclustered.Remove(keyValuePair.Value.a);
                unclustered.Remove(keyValuePair.Value.b);
                keyValuePair.Value.a.isUnclustered = false;
                keyValuePair.Value.b.isUnclustered = false;
                int index1 = count - 1;
                if (index1 == clusters.Length)
                    UnityEngine.Debug.LogError("how did this happen");
                clusters[index1] = aa;
                unclustered.Add(aa);
                aa.isUnclustered = true;
                for (int index2 = 0; index2 < unclustered.Count - 1; ++index2)
                {
                    float key = euclidean_distance(aa.centroid, unclustered[index2].centroid);
                    if ((double)key < (double)num1)
                        pq.Add(new KeyValuePair<float, ClusterDistance>(key, new ClusterDistance(aa, unclustered[index2])));
                }
                if (!wasCanceled)
                {
                    long num5 = GC.GetTotalMemory(false) / 1000000L;
                    if (progFunc != null)
                        wasCanceled = progFunc("Creating clusters:" + (float)((double)(items.Count - unclustered.Count) * 100.0 / (double)items.Count) + " unclustered:" + unclustered.Count + " inQ:" + pq.Count + " usedMem:" + num5, (float)((double)(items.Count - unclustered.Count) / (double)items.Count));
                }
                else
                    break;
            }
            if (progFunc != null)
                wasCanceled = progFunc("Finished clustering:", 100f);
            return !wasCanceled;
        }

        private float _RefillPriorityQWithSome(PriorityQueue<float, ClusterDistance> pq, List<ClusterNode> unclustered, ClusterNode[] clusters, ProgressUpdateCancelableDelegate progFunc)
        {
            List<float> array = new List<float>(2048);
            for (int index1 = 0; index1 < unclustered.Count; ++index1)
            {
                for (int index2 = index1 + 1; index2 < unclustered.Count; ++index2)
                    array.Add(euclidean_distance(unclustered[index1].centroid, unclustered[index2].centroid));
                wasCanceled = progFunc("Refilling Queue Part A:", (float)index1 / ((float)unclustered.Count * 2f));
                if (wasCanceled)
                    return 10f;
            }
            if (array.Count == 0)
                return 1E+11f;
            float num = NthSmallestElement<float>(array, 2048);
            for (int index1 = 0; index1 < unclustered.Count; ++index1)
            {
                for (int index2 = index1 + 1; index2 < unclustered.Count; ++index2)
                {
                    int idx1 = unclustered[index1].idx;
                    int idx2 = unclustered[index2].idx;
                    float key = euclidean_distance(unclustered[index1].centroid, unclustered[index2].centroid);
                    if ((double)key <= (double)num)
                        pq.Add(new KeyValuePair<float, ClusterDistance>(key, new ClusterDistance(clusters[idx1], clusters[idx2])));
                }
                wasCanceled = progFunc("Refilling Queue Part B:", (float)(unclustered.Count + index1) / ((float)unclustered.Count * 2f));
                if (wasCanceled)
                    return 10f;
            }
            return num;
        }

        public int TestRun(List<GameObject> gos)
        {
            List<item_s> itemSList = new List<item_s>();
            for (int index = 0; index < gos.Count; ++index)
                itemSList.Add(new item_s()
                {
                    go = gos[index],
                    coord = gos[index].transform.position
                });
            items = itemSList;
            if (items.Count > 0)
                agglomerate((ProgressUpdateCancelableDelegate)null);
            return 0;
        }

        public static void Main()
        {
            List<float> array = new List<float>();
            array.AddRange((IEnumerable<float>)new float[10]
            {
        19f,
        18f,
        17f,
        16f,
        15f,
        10f,
        11f,
        12f,
        13f,
        14f
            });
            Debug.Log("Loop quick select 10 times.");
            Debug.Log(NthSmallestElement<float>(array, 0));
        }

        public static T NthSmallestElement<T>(List<T> array, int n) where T : IComparable<T>
        {
            if (n < 0)
                n = 0;
            if (n > array.Count - 1)
                n = array.Count - 1;
            if (array.Count == 0)
                throw new ArgumentException("Array is empty.", array.ToString());
            if (array.Count == 1)
                return array[0];
            return QuickSelectSmallest<T>(array, n)[n];
        }

        private static List<T> QuickSelectSmallest<T>(List<T> input, int n) where T : IComparable<T>
        {
            List<T> array = input;
            int num1 = 0;
            int num2 = input.Count - 1;
            int pivotIndex = n;
            Random random = new Random();
            while (num2 > num1)
            {
                int num3 = QuickSelectPartition<T>(array, num1, num2, pivotIndex);
                if (num3 != n)
                {
                    if (num3 > n)
                        num2 = num3 - 1;
                    else
                        num1 = num3 + 1;
                    pivotIndex = random.Next(num1, num2);
                }
                else
                    break;
            }
            return array;
        }

        private static int QuickSelectPartition<T>(List<T> array, int startIndex, int endIndex, int pivotIndex) where T : IComparable<T>
        {
            T other = array[pivotIndex];
            Swap<T>(array, pivotIndex, endIndex);
            for (int index1 = startIndex; index1 < endIndex; ++index1)
            {
                if (array[index1].CompareTo(other) <= 0)
                {
                    Swap<T>(array, index1, startIndex);
                    ++startIndex;
                }
            }
            Swap<T>(array, endIndex, startIndex);
            return startIndex;
        }

        private static void Swap<T>(List<T> array, int index1, int index2)
        {
            if (index1 == index2)
                return;
            T obj = array[index1];
            array[index1] = array[index2];
            array[index2] = obj;
        }

        [Serializable]
        public class ClusterNode
        {
            public bool isUnclustered = true;
            public item_s leaf;
            public ClusterNode cha;
            public ClusterNode chb;
            public int height;
            public float distToMergedCentroid;
            public Vector3 centroid;
            public int[] leafs;
            public int idx;

            public ClusterNode(item_s ii, int index)
            {
                leaf = ii;
                idx = index;
                leafs = new int[1];
                leafs[0] = index;
                centroid = ii.coord;
                height = 0;
            }

            public ClusterNode(ClusterNode a, ClusterNode b, int index, int h, float dist, ClusterNode[] clusters)
            {
                cha = a;
                chb = b;
                idx = index;
                leafs = new int[a.leafs.Length + b.leafs.Length];
                Array.Copy(a.leafs, leafs, a.leafs.Length);
                Array.Copy(b.leafs, 0, leafs, a.leafs.Length, b.leafs.Length);
                Vector3 vector3 = Vector3.zero;
                for (int index1 = 0; index1 < leafs.Length; ++index1)
                    vector3 =vector3 + clusters[leafs[index1]].centroid;
                centroid = vector3/(float)leafs.Length;
                height = h;
                distToMergedCentroid = dist;
            }
        }

        [Serializable]
        public class item_s
        {
            public GameObject go;
            public Vector3 coord;
        }

        public class ClusterDistance
        {
            public ClusterNode a;
            public ClusterNode b;

            public ClusterDistance(ClusterNode aa, ClusterNode bb)
            {
                a = aa;
                b = bb;
            }
        }
    }
}
