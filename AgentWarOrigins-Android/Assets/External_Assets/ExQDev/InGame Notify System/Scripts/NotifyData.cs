using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class NotifyData{

    static List<Notify> queue = new List<Notify>();

    public class Notify
    {
        public string text;
        public Sprite ico;
        public delegate void Action();
        public static event Action DoAction;
        public static event Action OutAction;
        public Action act;
        public Action act2;

        public void InvokeAction()
        {
            if (act != null)
            {
                DoAction += act;
            }
            if (DoAction != null)
            {
                DoAction();
                DoAction -= DoAction;
            }
            
        }
        public void InvokeAction2()
        {
            if (act2 != null)
            {
                OutAction += act;
            }
            if (DoAction != null)
            {
                OutAction();
                OutAction -= OutAction;
            }
            
        }

        public Notify()
        {
            ico = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), Vector2.zero);
            text = "";
        }
        public Notify(string txt, Sprite icon)
        {
            ico = icon;
            text = txt;
        }
        public Notify(string txt)
        {
            ico = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height), Vector2.zero);
            text = txt;
        }
        public Notify(string txt, Action action)
        {
            ico = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 100, 100), Vector2.zero);
            text = txt;
            act = action;
        }
        public Notify(string txt, Action action, Action outAction)
        {
            ico = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 100, 100), Vector2.zero);
            text = txt;
            act = action;
            act2 = outAction;
        }
        public Notify(string txt, Sprite icon, Action action)
        {
            text = txt;
            ico = icon;
            act = action;
        }
        public Notify(string txt, Sprite icon, Action action, Action outAction)
        {
            text = txt;
            ico = icon;
            act = action;
            act2 = outAction;
        }
        
    }

    public static Notify current
    {
        get { return queue[0]; }
    }

    public static int count
    {
        get { return queue.Count; }
    }

    public static Sprite currentIcon
    {
        get { return queue[0].ico; }
    }

    public static void AddNew(string text)
    {
        if (count == 0)
        {
            queue.Add(new Notify());
        }
        queue.Add(new Notify(text));
    }
    public static void AddNew(string text, Notify.Action action)
    {
        if (count == 0)
        {
            queue.Add(new Notify());
        }
        queue.Add(new Notify(text, action));
    }

    public static void AddNew(string text, Notify.Action action, Notify.Action outAction)
    {
        if (count == 0)
        {
            queue.Add(new Notify());
        }
        queue.Add(new Notify(text, action, outAction));
    }
    public static void AddNew(string text, Sprite ico)
    {
        if (count == 0)
        {
            queue.Add(new Notify());
        }
        queue.Add(new Notify(text, ico));
    }

    public static void AddNew(string text, Sprite ico, Notify.Action action)
    {
        if (count == 0)
        {
            queue.Add(new Notify());
        }
        queue.Add(new Notify(text, ico, action));
    }

    public static void AddNew(string text, Sprite ico, Notify.Action action, Notify.Action outAction)
    {
        if (count == 0)
        {
            queue.Add(new Notify());
        }
        queue.Add(new Notify(text, ico, action, outAction));
    }

    public static bool IsNotificationAddedToQueue(Mission mission)
    {
        foreach (var name in queue)
        {
            if (name.text.Contains(mission.MissionTitle))
            {
                return true;
            }
        }
        return false;
    }
    public static void RemoveFirst()
    {
        queue.Remove(queue[0]);
    }
}
