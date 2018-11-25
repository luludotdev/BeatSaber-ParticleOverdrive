#if !NewUI

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRUI;
using TMPro;
using HMUI;
using ParticleOverdrive.Misc;

namespace ParticleOverdrive.UI.Settings
{
    public class ListViewController : ListSettingsController
    {
        public delegate float GetFloat();
        public event GetFloat GetValue;

        public delegate void SetFloat(float value);
        public event SetFloat SetValue;

        public delegate string StringForValue(float value);
        public event StringForValue FormatValue;

        protected float[] values;

        public void SetValues(float[] values)
        {
            this.values = values;
        }

        protected override void GetInitValues(out int idx, out int numberOfElements)
        {
            numberOfElements = values.Length;
            float value = 0;
            if (GetValue != null)
            {
                value = GetValue();
            }

            idx = numberOfElements - 1;
            for (int j = 0; j < values.Length; j++)
            {
                if (value == values[j])
                {
                    idx = j;
                    return;
                }
            }
        }

        protected override void ApplyValue(int idx)
        {
            if (SetValue != null)
                SetValue(values[idx]);
        }

        protected override string TextForValue(int idx)
        {
            string text = "?";
            if (FormatValue != null)
                text = FormatValue(values[idx]);

            return text;
        }
    }

    public class BoolViewController : SwitchSettingsController
    {
        public delegate bool GetBool();
        public event GetBool GetValue;

        public delegate void SetBool(bool value);
        public event SetBool SetValue;

        protected override bool GetInitValue()
        {
            bool value = false;
            if (GetValue != null)
                value = GetValue();

            return value;
        }

        protected override void ApplyValue(bool value)
        {
            if (SetValue != null)
                SetValue(value);
        }

        protected override string TextForValue(bool value) => (value) ? "ON" : "OFF";
    }

    public abstract class IntSettingsController : IncDecSettingsController
    {
        private int _value;
        protected int _min;
        protected int _max;
        protected int _increment;

        protected abstract int GetInitValue();
        protected abstract void ApplyValue(int value);
        protected abstract string TextForValue(int value);


        public override void Init()
        {
            _value = this.GetInitValue();
            this.RefreshUI();
        }

        public override void ApplySettings()
        {
            this.ApplyValue(this._value);
        }

        private void RefreshUI()
        {
            this.text = this.TextForValue(this._value);

            this.enableDec = _value > _min;
            this.enableInc = _value < _max;
        }

        public override void IncButtonPressed()
        {
            this._value += _increment;
            if (this._value > _max) this._value = _max;

            this.RefreshUI();
        }

        public override void DecButtonPressed()
        {
            this._value -= _increment;
            if (this._value < _min) this._value = _min;

            this.RefreshUI();
        }
    }

    public class IntViewController : IntSettingsController
    {
        public delegate int GetInt();
        public event GetInt GetValue;

        public delegate void SetInt(int value);
        public event SetInt SetValue;

        public void SetValues(int min, int max, int increment)
        {
            _min = min;
            _max = max;
            _increment = increment;
        }

        public void UpdateIncrement(int increment)
        {
            _increment = increment;
        }

        private int FixValue(int value)
        {
            if (value % _increment != 0) value -= (value % _increment);
            if (value > _max) value = _max;
            if (value < _min) value = _min;

            return value;
        }

        protected override int GetInitValue()
        {
            int value = 0;
            if (GetValue != null)
                value = FixValue(GetValue());

            return value;
        }

        protected override void ApplyValue(int value)
        {
            if (SetValue != null) SetValue(FixValue(value));
        }

        protected override string TextForValue(int value) => value.ToString();
    }

    public class SubMenu
    {
        public Transform transform;

        public SubMenu(Transform transform)
        {
            this.transform = transform;
        }

        public BoolViewController AddBool(string name) => AddToggleSetting<BoolViewController>(name);

        public IntViewController AddInt(string name, int min, int max, int increment)
        {
            var view = AddIntSetting<IntViewController>(name);
            view.SetValues(min, max, increment);

            return view;
        }

        public ListViewController AddList(string name, float[] values)
        {
            var view = AddListSetting<ListViewController>(name);
            view.SetValues(values);

            return view;
        }

        public T AddListSetting<T>(string name) where T : ListSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<VolumeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            VolumeSettingsController volume = newSettingsObject.GetComponent<VolumeSettingsController>();
            T newListSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(ListSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            newSettingsObject.GetComponentInChildren<TMP_Text>().text = name;

            return newListSettingsController;
        }

        public T AddToggleSetting<T>(string name) where T : SwitchSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<WindowModeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newToggleSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(SwitchSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            newSettingsObject.GetComponentInChildren<TMP_Text>().text = name;

            return newToggleSettingsController;
        }

        public T AddIntSetting<T>(string name) where T : IntSettingsController
        {
            var volumeSettings = Resources.FindObjectsOfTypeAll<WindowModeSettingsController>().FirstOrDefault();
            GameObject newSettingsObject = MonoBehaviour.Instantiate(volumeSettings.gameObject, transform);
            newSettingsObject.name = name;

            WindowModeSettingsController volume = newSettingsObject.GetComponent<WindowModeSettingsController>();
            T newToggleSettingsController = (T)ReflectionUtil.CopyComponent(volume, typeof(IncDecSettingsController), typeof(T), newSettingsObject);
            MonoBehaviour.DestroyImmediate(volume);

            newSettingsObject.GetComponentInChildren<TMP_Text>().text = name;

            return newToggleSettingsController;
        }
    }

    public class SettingsUI : MonoBehaviour
    {
        public static SettingsUI Instance = null;

        static bool ready = false;
        public static bool Ready
        {
            get => ready;
        }

        static MainMenuViewController _mainMenuViewController = null;
        static SettingsNavigationController settingsMenu = null;
        static MainSettingsMenuViewController mainSettingsMenu = null;
        static MainSettingsTableView _mainSettingsTableView = null;
        static TableView subMenuTableView = null;
        static MainSettingsTableCell tableCell = null;
        static TableView subMenuTableViewHelper;

        static Transform othersSubmenu = null;

        static SimpleDialogPromptViewController prompt = null;

        static Button _pageUpButton = null;
        static Button _pageDownButton = null;
        static Vector2 buttonOffset = new Vector2(24, 0);

        static bool initialized = false;

        public static void OnLoad()
        {
            if (Instance != null) return;

            new GameObject("SettingsUI").AddComponent<SettingsUI>();
        }

        public static bool isMenuScene(Scene scene) => scene.name == "Menu";
        public static bool isGameScene(Scene scene) => scene.name == "StandardLevelLoader";

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        public void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            try
            {
                if (isMenuScene(scene) && !initialized)
                {
                    SetupUI();
                    initialized = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SettingsUI done fucked up: " + e);
            }
        }

        private static void SetupUI()
        {
            if (mainSettingsMenu == null)
            {
                ready = false;
            }

            if (!Ready)
            {
                try
                {
                    var _menuMasterViewController = Resources.FindObjectsOfTypeAll<MainFlowCoordinator>().First();
                    prompt = ReflectionUtil.GetField<SimpleDialogPromptViewController>(_menuMasterViewController, "_simpleDialogPromptViewController");

                    _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                    settingsMenu = Resources.FindObjectsOfTypeAll<SettingsNavigationController>().FirstOrDefault();
                    mainSettingsMenu = Resources.FindObjectsOfTypeAll<MainSettingsMenuViewController>().FirstOrDefault();
                    _mainSettingsTableView = mainSettingsMenu.GetField<MainSettingsTableView>("_mainSettingsTableView");
                    subMenuTableView = _mainSettingsTableView.GetComponentInChildren<TableView>();
                    subMenuTableViewHelper = subMenuTableView.gameObject.AddComponent<TableView>();
                    othersSubmenu = settingsMenu.transform.Find("OtherSettings");

                    if (_mainSettingsTableView != null)
                        AddPageButtons();

                    if (tableCell == null)
                    {
                        tableCell = Resources.FindObjectsOfTypeAll<MainSettingsTableCell>().FirstOrDefault();
                        // Get a refence to the Settings Table cell text in case we want to change font size, etc
                        var text = tableCell.GetField<TextMeshProUGUI>("_settingsSubMenuText");
                    }

                    ready = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Beat Saver UI: Oops - " + e.Message);
                }
            }
        }

        static void AddPageButtons()
        {
            RectTransform viewport = _mainSettingsTableView.GetComponentsInChildren<RectTransform>().First(x => x.name == "Viewport");
            viewport.anchorMin = new Vector2(0f, 0.5f);
            viewport.anchorMax = new Vector2(1f, 0.5f);
            viewport.sizeDelta = new Vector2(0f, 48f);
            viewport.anchoredPosition = new Vector2(0f, 0f);

            if (_pageUpButton == null)
            {
                _pageUpButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageUpButton")), _mainSettingsTableView.transform, false);
                (_pageUpButton.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                (_pageUpButton.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                (_pageUpButton.transform as RectTransform).anchoredPosition = new Vector2(0f, 24f);
                _pageUpButton.interactable = true;
                _pageUpButton.onClick.AddListener(delegate ()
                {
                    subMenuTableViewHelper.PageScrollUp();
                });
            }

            if (_pageDownButton == null)
            {
                _pageDownButton = Instantiate(Resources.FindObjectsOfTypeAll<Button>().First(x => (x.name == "PageDownButton")), _mainSettingsTableView.transform, false);
                (_pageDownButton.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                (_pageDownButton.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                (_pageDownButton.transform as RectTransform).anchoredPosition = new Vector2(0f, -24f);
                _pageDownButton.interactable = true;
                _pageDownButton.onClick.AddListener(delegate ()
                {
                    subMenuTableViewHelper.PageScrollDown();
                });
            }

            subMenuTableViewHelper.SetField("_pageUpButton", _pageUpButton);
            subMenuTableViewHelper.SetField("_pageDownButton", _pageDownButton);
        }

        public static void LogComponents(Transform t, string prefix = "=", bool includeScipts = false)
        {
            Console.WriteLine(prefix + ">" + t.name);

            if (includeScipts)
            {
                foreach (var comp in t.GetComponents<MonoBehaviour>())
                {
                    Console.WriteLine(prefix + "-->" + comp.GetType());
                }
            }

            foreach (Transform child in t)
            {
                LogComponents(child, prefix + "=", includeScipts);
            }
        }

        public static SubMenu CreateSubMenu(string name)
        {
            if (!isMenuScene(SceneManager.GetActiveScene()))
            {
                Console.WriteLine("Cannot create settings menu when no in the main scene.");
                return null;
            }

            SetupUI();

            var subMenuGameObject = Instantiate(othersSubmenu.gameObject, othersSubmenu.transform.parent);
            Transform mainContainer = CleanScreen(subMenuGameObject.transform);

            var newSubMenuInfo = new SettingsSubMenuInfo();
            newSubMenuInfo.SetField("_menuName", name);
            newSubMenuInfo.SetField("_viewController", subMenuGameObject.GetComponent<VRUIViewController>());

            var subMenuInfos = mainSettingsMenu.GetField<SettingsSubMenuInfo[]>("_settingsSubMenuInfos").ToList();
            subMenuInfos.Add(newSubMenuInfo);
            mainSettingsMenu.SetField("_settingsSubMenuInfos", subMenuInfos.ToArray());

            SubMenu menu = new SubMenu(mainContainer);
            return menu;
        }

        static Transform CleanScreen(Transform screen)
        {
            var container = screen.Find("Content").Find("SettingsContainer");
            var tempList = container.Cast<Transform>().ToList();

            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }

            return container;
        }
    }
}

#endif
