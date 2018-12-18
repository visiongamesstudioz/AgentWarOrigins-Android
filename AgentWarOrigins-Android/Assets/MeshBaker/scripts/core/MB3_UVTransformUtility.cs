using UnityEngine;

namespace DigitalOpus.MB.Core
{
  public class MB3_UVTransformUtility
  {
    public static void Test()
    {
      DRect drect = new DRect(0.5, 0.5, 2.0, 2.0);
      DRect t = new DRect(0.25, 0.25, 3.0, 3.0);
      DRect r1 = InverseTransform(ref drect);
      DRect r2 = InverseTransform(ref t);
      DRect r3 = CombineTransforms(ref drect, ref r2);
      Debug.Log((object) r1);
      Debug.Log((object) r3);
      Debug.Log((object) ("one mat trans " + (object) TransformPoint(ref drect, new Vector2(1f, 1f))));
      string str1 = "one inv mat trans ";
      Vector2 vector2_1 = TransformPoint(ref r1, new Vector2(1f, 1f));
      // ISSUE: explicit reference operation
      string str2 = ((Vector2) @vector2_1).ToString("f4");
      Debug.Log((object) (str1 + str2));
      string str3 = "zero ";
      Vector2 vector2_2 = TransformPoint(ref r3, new Vector2(0.0f, 0.0f));
      // ISSUE: explicit reference operation
      string str4 = ((Vector2) @vector2_2).ToString("f4");
      Debug.Log((object) (str3 + str4));
      string str5 = "one ";
      Vector2 vector2_3 = TransformPoint(ref r3, new Vector2(1f, 1f));
      // ISSUE: explicit reference operation
      string str6 = ((Vector2) @vector2_3).ToString("f4");
      Debug.Log((object) (str5 + str6));
    }

    public static float TransformX(DRect r, double x)
    {
      return (float) (r.width * x + r.x);
    }

    public static DRect CombineTransforms(ref DRect r1, ref DRect r2)
    {
      return new DRect(r1.x * r2.width + r2.x, r1.y * r2.height + r2.y, r1.width * r2.width, r1.height * r2.height);
    }

    public static Rect CombineTransforms(ref Rect r1, ref Rect r2)
    {
            Rect rCombined = new Rect(r1.x * r2.width + r2.x,
                              r1.y * r2.height + r2.y,
                              r1.width * r2.width,
                              r1.height * r2.height);
            //rCombined.x = rCombined.x - Mathf.FloorToInt(rCombined.x);
            //rCombined.y = rCombined.y - Mathf.FloorToInt(rCombined.y);
            return rCombined;
        }

    public static void Canonicalize(ref DRect r, double minX, double minY)
    {
      r.x -= (double) Mathf.FloorToInt((float) r.x);
      if (r.x < minX)
        r.x += (double) Mathf.CeilToInt((float) minX);
      r.y -= (double) Mathf.FloorToInt((float) r.y);
      if (r.y >= minY)
        return;
      r.y += (double) Mathf.CeilToInt((float) minY);
    }

    public static void Canonicalize(ref Rect r, float minX, float minY)
    {
            r.x = r.x - Mathf.FloorToInt(r.x);
            if (r.x < minX) { r.x += Mathf.CeilToInt(minX); }
            r.y = r.y - Mathf.FloorToInt(r.y);
            if (r.y < minY) { r.y += Mathf.CeilToInt(minY); }
        }

    public static DRect InverseTransform(ref DRect t)
    {
      return new DRect()
      {
        x = -t.x / t.width,
        y = -t.y / t.height,
        width = 1.0 / t.width,
        height = 1.0 / t.height
      };
    }

    public static DRect GetEncapsulatingRect(ref DRect uvRect1, ref DRect uvRect2)
    {
      double x1 = uvRect1.x;
      double y1 = uvRect1.y;
      double num1 = uvRect1.x + uvRect1.width;
      double num2 = uvRect1.y + uvRect1.height;
      double x2 = uvRect2.x;
      double y2 = uvRect2.y;
      double num3 = uvRect2.x + uvRect2.width;
      double num4 = uvRect2.y + uvRect2.height;
      double num5;
      double xx = num5 = x1;
      double num6;
      double yy = num6 = y1;
      if (x2 < xx)
        xx = x2;
      if (x1 < xx)
        xx = x1;
      if (y2 < yy)
        yy = y2;
      if (y1 < yy)
        yy = y1;
      if (num3 > num5)
        num5 = num3;
      if (num1 > num5)
        num5 = num1;
      if (num4 > num6)
        num6 = num4;
      if (num2 > num6)
        num6 = num2;
      return new DRect(xx, yy, num5 - xx, num6 - yy);
    }

    public static bool RectContains(ref DRect bigRect, ref DRect smallToTestIfFits)
    {
      double x = smallToTestIfFits.x;
      double y = smallToTestIfFits.y;
      double num1 = smallToTestIfFits.x + smallToTestIfFits.width;
      double num2 = smallToTestIfFits.y + smallToTestIfFits.height;
      double num3 = bigRect.x - 0.00999999977648258;
      double num4 = bigRect.y - 0.00999999977648258;
      double num5 = bigRect.x + bigRect.width + 0.0199999995529652;
      double num6 = bigRect.y + bigRect.height + 0.0199999995529652;
      return num3 <= x && x <= num5 && (num3 <= num1 && num1 <= num5) && (num4 <= y && y <= num6 && num4 <= num2) && num2 <= num6;
    }

    public static bool RectContains(ref Rect bigRect, ref Rect smallToTestIfFits)
    {
            float smnx = smallToTestIfFits.x;
            float smny = smallToTestIfFits.y;
            float smxx = smallToTestIfFits.x + smallToTestIfFits.width;
            float smxy = smallToTestIfFits.y + smallToTestIfFits.height;
            //expand slightly to deal with rounding errors
            float bmnx = bigRect.x - 10e-3f;
            float bmny = bigRect.y - 10e-3f;
            float bmxx = bigRect.x + bigRect.width + 2f * 10e-3f;
            float bmxy = bigRect.y + bigRect.height + 2f * 10e-3f;
            //is smn in box
            /*
            Debug.Log("==== " + bigRect + " " + smallToTestIfFits);
            Debug.Log(bmnx <= smnx);
            Debug.Log(smnx <= bmxx);
            Debug.Log(bmnx <= smxx);
            Debug.LogFormat("{0} {1} {2}", (smxx <= bmxx).ToString(), smxx.ToString("F5"), bmxx.ToString("F5"));
            Debug.LogFormat("{0} {1} {2}", (bmny <= smny), bmny.ToString("F5"), smny.ToString("F5"));
            Debug.Log(smny <= bmxy);
            Debug.Log(bmny <= smxy);
            Debug.LogFormat("{0} {1} {2}", (smxy <= bmxy).ToString(), smxy.ToString("F5"), bmxy.ToString("F5"));
            Debug.Log("----------------");
            */
            return bmnx <= smnx && smnx <= bmxx &&
                    bmnx <= smxx && smxx <= bmxx &&
                    bmny <= smny && smny <= bmxy &&
                    bmny <= smxy && smxy <= bmxy;
        }

    internal static Vector2 TransformPoint(ref DRect r, Vector2 p)
    {
      return new Vector2((float) (r.width * (double) p.x + r.x), (float) (r.height * (double) p.y + r.y));
    }
  }
}
