using System.Collections;
using System.Collections.Generic;
using Automate.Controller.Modules;
using Boo.Lang;
using src.View;
using UnityEngine;
using UnityEngine.UI;

public class ScrollSelectionBehaviour : MonoBehaviour
{
    public RectTransform ButtonRoot;
    public Dropdown Dropdown;
    public Sprite DottySprite;
    Logger _logger = new Logger(new AutomateLogHandler());

    // Use this for initialization
    void Start()
    {
        //an example of how to use this
        CreateButton(ButtonRoot, Vector3.zero, Vector2.one * 150, () => { _logger.Log(LogType.Log, "HELLO"); }, DottySprite);
        AddDropdownOption(Dropdown, "Default1", () => { _logger.Log(LogType.Log, "Default1 Type selected"); });
        AddDropdownOption(Dropdown, "Default2", () => { _logger.Log(LogType.Log, "Default2 Type selected"); });
        AddDropdownOption(Dropdown, "Default3", () => { _logger.Log(LogType.Log, "Default3 Type selected"); });
        AddDropdownOption(Dropdown, "Default4", () => { _logger.Log(LogType.Log, "Default4 Type selected"); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Random.Range(0,60) == 0)
            CreateButton(ButtonRoot, Vector3.zero, Vector2.one * 150, () => { _logger.Log(LogType.Log, "HELLO"); }, DottySprite);
    }

    public static void CreateButton(RectTransform buttonContentRoot, Vector3 position, Vector2 size,
        UnityEngine.Events.UnityAction method, Sprite sprite)
    {
        int amountOfExistingButtons = buttonContentRoot.transform.childCount;
        GameObject button = new GameObject();
        button.transform.parent = buttonContentRoot.transform;
        button.AddComponent<RectTransform>();
        button.AddComponent<Button>();
        Image image = button.AddComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        button.transform.position = position;
        button.GetComponent<RectTransform>().sizeDelta = size;
        //button.GetComponent<RectTransform>().position = ButtonRoot.anchoredPosition;
        button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
        button.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        button.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
        button.GetComponent<RectTransform>().anchoredPosition = Vector2.up * (-70 + -150 * amountOfExistingButtons);
        buttonContentRoot.GetComponent<RectTransform>().sizeDelta = Vector2.up * (amountOfExistingButtons * 150 - 300);
        //button.GetComponent<Button>().transform. = new Vector3(100,100,1);
        method();
        button.GetComponent<Button>().onClick.AddListener(method);
        button.GetComponent<Button>().onClick.Invoke();
    }

    public static void AddDropdownOption(Dropdown dropdownRoot, string text, UnityEngine.Events.UnityAction method)
    {
        dropdownRoot.AddOptions(new System.Collections.Generic.List<string>() {text});
        var dropdownCount = dropdownRoot.options.Count;
        dropdownRoot.onValueChanged.AddListener((x) =>
        {
            if (x == dropdownCount - 1)
            {
                method.Invoke();
            }
        });
    }
}
