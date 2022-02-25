using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Need : MonoBehaviour
{
    public enum Type
	{
        Energy,
        HUNGER,
        BLADDER,
        HYGIENE,
        FUN
	}

    public Type type;
    public AnimationCurve curve;
    public float input = 1; //time
    public float decay = 0;
    public MeterUi meter;

    void Start()
    {
        meter.name = type.ToString();
        //meter.Text.text = type.ToString();
    }

    public float motive { get { return getMotive(input); } }

    void Update()
    {
        input = input - (decay * Time.deltaTime);
        input = Mathf.Clamp(input, -1, 1);
        //meter.slider.value = 1 - motive;
    }

    public float getMotive(float value)
    {
        return Mathf.Clamp(curve.Evaluate(value),0,1);
    }
}
