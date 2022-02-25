using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UtillityAgent : Agent
{
    [SerializeField] Perception perception;
    [SerializeField] MeterUi meter;

    const float MIN_SOCRE = 0.2f;

    Need[] needs;
    //MeterUi meter;
    UtilityObject activeUtilityObject = null;
    public bool isUsingUtilityObject { get;{ return activeUtilityObject != null; } }

    public float happiness
    {
        get
        {
            float totalMotive = 0;
            foreach(var need in needs)
            {
                totalMotive += need.motive;

            }
            return totalMotive / needs.Length;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        needs = GetComponentsInChildren<Need>();

        //meter.Text.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("speed", movement.velocity.magnitude);
        if(activeUtilityObject == null)
        {
            var gameObjects = perception.GetGameObjects();
            List<UtilityObject> utilityObjects = new List<UtilityObject>();
            foreach(var go in gameObjects)
            {
                if (go.TryGetComponent<UtilityObject>(out UtilityObject utilityObject));
                {
                    utilityObject.visible = true;
                    utilityObject.score = GetUtilityObjectScore(utilityObject);
                    if(utilityObject.score >MIN_SOCRE) utilityObjects.Add(utilityObject);
                }
            }
        }

        //set first utility object to the first utility object
        activeUtilityObject = (utilityObjects.count == 0) ? null : utitilyObjects[0];
        if(activeUtilityObject != null)
        {
            StartCoroutine(ExecuteUtilityObject(activeUtilityObject));
        }
    }

    IEnumerator ExecuteUtilityObject(UtilityObject utilityObject)
    {
        movement.MoveTowards(utilityObject.location.position);
        while(Vector3.Distance(transform.position, utilityObject.location.position)>0.25f)
        {
            Debug.DrawLine(transform.position, utilityObject.location.position);

            yield return null;
        }

        print("start effect");
        if (utilityObject.effect != null) utilityObject.effect.SetActive(true);

        yield return new WaitForSeconds(utilityObject.duration);

        if (utilityObject.effect != null) utilityObject.effect.SetActive(false);
        print("stop effect");
        yield return null;

    }

    void ApplyUtillityObject(UtilityObject utilityObject)
    {
        foreach (var effector in utilityObject.effectors)
        {
            Need need = getNeedByType(effector.type);
            if(need != null)
            {
                need.input += effector.change;
                need.input = Mathf.Clamp(need.input, -1, 1);
            }
        }
    }


    private void OnGUI()
    {
        Vector2 screen = Camera.main.WorldToScreenPoint(transform.position);



        GUI.color = Color.black;
        int offset = 0;
        foreach (var need in needs)
        {
            GUI.Label(new Rect(screen.x + 20, Screen.height - screen.y - offset, 300, 20), need.type.ToString() + ": " + need.motive);
            offset += 20;
        }
       // GUI.Label(new Rect(screen.x + 20, Screen.height - screen.y - offset, 300, 20), mood.ToString());
    }

    public void LateUpdate()
    {
        //meter.slider.value = happiness;
        //meter.worldPosition = transform.position + Vector3.up * 4;

    }

    float GetUtilityObjectScore(UtilityObject utility)
    {
        float score = 0;

        foreach (var effector in utility.effectors)
        {
            Need need = getNeedByType(effector.type);
            if(need != null)
            {
                float futureNeed = need.getMotive(need.input + effector.change);
                score += need.motive - futureNeed;
            }
        }

        return score;
    }

    Need getNeedByType(Need.Type type)
    {
        return needs.First(need => need.type == type);
    }
}
