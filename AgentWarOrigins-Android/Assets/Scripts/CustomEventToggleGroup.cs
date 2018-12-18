using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CustomEventToggleGroup : ToggleGroup {

    public delegate void ChangedEventHandler(Toggle newActive);

    public event ChangedEventHandler OnChange;
    protected override void Start()
    {
        base.Start();
        foreach (Toggle toggle in gameObject.GetComponentsInChildren<Toggle>())
        {
            toggle.onValueChanged.AddListener((isSelected) => {
                if (isSelected)
                {

                    var activeToggle = Active();
                    DoOnChange(toggle);
                }
             
            });
        }
    }
    public Toggle Active()
    {
        return ActiveToggles().FirstOrDefault();
    }

    protected virtual void DoOnChange(Toggle newactive)
    {
        var handler = OnChange;
        if (handler != null) handler(newactive);
    }
}
