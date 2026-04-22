using System.Collections.Generic;
using JetBrains.Annotations;
using SABI.Flow;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor;
#endif

namespace SABI
{
    [AddComponentMenu("SABI/ModularWeapon")]
    public class ModularWeapon : MonoBehaviour, IDamagableSource
    {
        #region Variables ---------------------------------------------------------------------- <Reg: Variables>

        // [field: SerializeField, MinimalNullValidation]
        // public GunBaseModule_Input gunModule_Input { get; private set; }

        [field: SerializeField, MinimalNullValidation]
        public MWM_SecondaryAimer MWM_SecondaryAimer { get; private set; }

        [field: SerializeField, MinimalNullValidation]
        public MWM_Magazine MWM_Magazine { get; private set; }

        [field: SerializeField, MinimalNullValidation]
        public MWM_BulletType MWM_BulletType { get; private set; }

        [field: SerializeField, MinimalNullValidation]
        public MWM_MuzzleFlash MWM_MuzzleFlash { get; private set; }

        [field: SerializeField, MinimalNullValidation]
        public MWM_Trigger MWM_Trigger { get; private set; }

        [field: SerializeField, MinimalNullValidation]
        public MWM_Shooter MWM_Shooter { get; private set; }

        [field: SerializeField, MinimalNullValidation]
        public MWM_Reload MWM_Reload { get; private set; }

        public Camera fpsCamera;
        public AnimationManager animationManager;
        public Transform firePoint;

        List<MWM> allUsedBaseGunModules = new();

        #endregion Variables ------------------------------------------------------------------- </Reg: Variables>
        private void Awake()
        {
            SetGunReferenceToModules();
            allUsedBaseGunModules.Clear();
            allUsedBaseGunModules.AddMultiple(
                // gunModule_Input,
                MWM_SecondaryAimer,
                MWM_Magazine,
                MWM_BulletType,
                MWM_MuzzleFlash,
                MWM_Trigger,
                MWM_Shooter,
                MWM_Reload
            );
        }

        private void SetGunReferenceToModules()
        {
            MWM_SecondaryAimer.SetGun(this);
            MWM_Magazine.SetGun(this);
            MWM_BulletType.SetGun(this);
            MWM_MuzzleFlash.SetGun(this);
            MWM_Trigger.SetGun(this);
            MWM_Shooter.SetGun(this);
            // gunModule_Input.SetGun(this);
            MWM_Reload.SetGun(this);
        }

        // public void SetModule_Input(GunBaseModule_Input module) => gunModule_Input = module;

        public void SetModule_Aimer(MWM_SecondaryAimer module) => MWM_SecondaryAimer = module;

        public void SetModule_Ammo(MWM_Magazine module) => MWM_Magazine = module;

        public void SetModule_BulletType(MWM_BulletType module) => MWM_BulletType = module;

        public void SetModule_MuzzleFlash(MWM_MuzzleFlash module) => MWM_MuzzleFlash = module;

        public void SetModule_Trigger(MWM_Trigger module) => MWM_Trigger = module;

        public void SetModule_Shooter(MWM_Shooter module) => MWM_Shooter = module;

        public void SetModule_Reload(MWM_Reload module) => MWM_Reload = module;

        #region Context Menu Items -------------------------------------------------------- <Reg: UnityLifeCycle>

        [ContextMenu("GetGunModuleReferences")]
        public void GetGunModuleReferences()
        {
            if (MWM_SecondaryAimer == null)
                MWM_SecondaryAimer = GetComponentInChildren<MWM_SecondaryAimer>();
            if (MWM_Magazine == null)
                MWM_Magazine = GetComponentInChildren<MWM_Magazine>();
            if (MWM_BulletType == null)
                MWM_BulletType = GetComponentInChildren<MWM_BulletType>();
            if (MWM_MuzzleFlash == null)
                MWM_MuzzleFlash = GetComponentInChildren<MWM_MuzzleFlash>();
            if (MWM_Trigger == null)
                MWM_Trigger = GetComponentInChildren<MWM_Trigger>();
            if (MWM_Shooter == null)
                MWM_Shooter = GetComponentInChildren<MWM_Shooter>();
            if (MWM_Reload == null)
                MWM_Reload = GetComponentInChildren<MWM_Reload>();
            // if (gunModule_Input == null)
            //     gunModule_Input = GetComponentInChildren<GunBaseModule_Input>();
            // ---------------------------------------------------------------------------------------------
            if (animationManager == null)
                animationManager = GetComponentInChildren<AnimationManager>();
        }

        [ContextMenu("FillMissingModulesWithNoneModules")]
        public void FillMissingModulesWithNoneModules()
        {
            if (!MWM_SecondaryAimer)
                MWM_SecondaryAimer = gameObject.AddComponent<MWM_SecondaryAimer_None>();
            if (!MWM_Magazine)
                MWM_Magazine = gameObject.AddComponent<MWM_Magazine_None>();
            if (!MWM_BulletType)
                MWM_BulletType = gameObject.AddComponent<MWM_BulletType_None>();
            if (!MWM_MuzzleFlash)
                MWM_MuzzleFlash = gameObject.AddComponent<MWM_MuzzleFlash_None>();
            if (!MWM_Trigger)
                MWM_Trigger = gameObject.AddComponent<MWM_Trigger_None>();
            if (!MWM_Shooter)
                MWM_Shooter = gameObject.AddComponent<MWM_Shooter_None>();
            if (!MWM_Reload)
                MWM_Reload = gameObject.AddComponent<MWM_Reload_None>();
        }

        [ContextMenu("DestroyAllGunModules")]
        public void DestroyAllGunModules()
        {
            if (MWM_SecondaryAimer)
                DestroyImmediate(MWM_SecondaryAimer);
            if (MWM_Magazine)
                DestroyImmediate(MWM_Magazine);
            if (MWM_BulletType)
                DestroyImmediate(MWM_BulletType);
            if (MWM_MuzzleFlash)
                DestroyImmediate(MWM_MuzzleFlash);
            if (MWM_Trigger)
                DestroyImmediate(MWM_Trigger);
            if (MWM_Shooter)
                DestroyImmediate(MWM_Shooter);
            if (MWM_Shooter)
                DestroyImmediate(MWM_Shooter);
            if (MWM_Reload)
                DestroyImmediate(MWM_Reload);
            // if (gunModule_Input)
            //     DestroyImmediate(gunModule_Input);
        }

        [ContextMenu("DestroyUnusedGunModules")]
        public void DestroyUnusedGunModules()
        {
            GetGunModuleReferences();
            foreach (var item in GetComponentsInChildren<MWM>())
            {
                if (!IsModuleUsed(item))
                    DestroyImmediate(item);
            }
        }

        bool IsModuleUsed(MWM module)
        {
            foreach (var item in allUsedBaseGunModules)
            {
                if (item == module)
                    return true;
            }
            return false;
        }
        #endregion Context Menu Items ---------------------------------------------------- </Reg: UnityLifeCycle>
    }

    #region Editor ------------------------------------------------------------------------- <Reg: Editor>

#if UNITY_EDITOR

    [CustomEditor(typeof(ModularWeapon))]
    public class Editor_Gun : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            ModularWeapon gun = target as ModularWeapon;
            var root = new VisualElement();

            FContainer addButtonContainer = new FContainer();
            AddModule(addButtonContainer);
            AddAndSetModule_Input(gun, addButtonContainer);
            AddAndSetModule_Aimer(gun, addButtonContainer);
            AddAndSetModule_Ammo(gun, addButtonContainer);
            AddAndSetModule_BulletType(gun, addButtonContainer);
            AddAndSetModule_MuzzleFlash(gun, addButtonContainer);
            AddAndSetModule_Trigger(gun, addButtonContainer);
            AddAndSetModule_Shooter(gun, addButtonContainer);
            AddAndSetModule_Reload(gun, addButtonContainer);

            FContainer mainContainer = new FContainer();
            InspectorElement.FillDefaultInspector(mainContainer, serializedObject, this);

            FContainer moreOptionsContainer = new FContainer();

            moreOptionsContainer.Add(
                new Button() { text = "Remove All Modules" }.OnClick(
                    () => gun.DestroyAllGunModules()
                )
            );

            moreOptionsContainer.Add(
                new Button() { text = "Remove Unused Modules" }.OnClick(
                    () => gun.DestroyUnusedGunModules()
                )
            );

            moreOptionsContainer.Add(
                new Button() { text = "Fill Missing Modules With None Modules" }.OnClick(
                    () => gun.FillMissingModulesWithNoneModules()
                )
            );

            moreOptionsContainer.Add(
                new Button() { text = "Auto Get References" }.OnClick(
                    () => gun.GetGunModuleReferences()
                )
            );

            root.Insert(
                new FText("Modular Weapon"),
                addButtonContainer,
                mainContainer,
                moreOptionsContainer
            );
            return root;
        }

        private void AddModule(VisualElement root)
        {
            var button_AddAllModule = new Button(() => { }) { text = "Add Modules" };
            button_AddAllModule.clicked += () =>
            {
                var menu = new GenericMenu();

                foreach (var item in TypeCache.GetTypesDerivedFrom<MWM>())
                {
                    menu.AddItem(
                        new GUIContent(item.Name),
                        false,
                        () => (target as GameObject).AddComponent(item)
                    );
                }

                menu.ShowAsContext();
            };
            root.Add(button_AddAllModule);
        }

        private static void AddAndSetModule_Input(ModularWeapon gun, VisualElement root)
        {
            var button_AddInput = new Button(() => { }) { text = "Add & Assign Input" };
            button_AddInput.clicked += () =>
            {
                var menu = new GenericMenu();

                foreach (var item in TypeCache.GetTypesDerivedFrom<MWM_Input>())
                {
                    menu.AddItem(
                        new GUIContent(item.Name),
                        false,
                        () =>
                        {
                            var module = gun.gameObject.AddComponent(item);
                            (module as MWM_Input).weapon = gun;
                            // gun.SetModule_Input(module as GunBaseModule_Input);
                        }
                    );
                }

                menu.ShowAsContext();
            };
            root.Add(button_AddInput);
        }

        private static void AddAndSetModule_Aimer(ModularWeapon gun, VisualElement root)
        {
            var button = new Button(() => { }) { text = "Add & Assign Aimer" };
            button.clicked += () =>
            {
                var menu = new GenericMenu();

                foreach (var item in TypeCache.GetTypesDerivedFrom<MWM_SecondaryAimer>())
                {
                    menu.AddItem(
                        new GUIContent(item.Name),
                        false,
                        () =>
                        {
                            var module = gun.gameObject.AddComponent(item);
                            gun.SetModule_Aimer(module as MWM_SecondaryAimer);
                        }
                    );
                }
                menu.ShowAsContext();
            };
            root.Add(button);
        }

        private static void AddAndSetModule_Ammo(ModularWeapon gun, VisualElement root)
        {
            var button = new Button(() => { }) { text = "Add & Assign Magazine" };
            button.clicked += () =>
            {
                var menu = new GenericMenu();

                foreach (var item in TypeCache.GetTypesDerivedFrom<MWM_Magazine>())
                {
                    menu.AddItem(
                        new GUIContent(item.Name),
                        false,
                        () =>
                        {
                            var module = gun.gameObject.AddComponent(item);
                            gun.SetModule_Ammo(module as MWM_Magazine);
                        }
                    );
                }
                menu.ShowAsContext();
            };
            root.Add(button);
        }

        private static void AddAndSetModule_BulletType(ModularWeapon gun, VisualElement root)
        {
            var button = new Button(() => { }) { text = "Add & Assign BulletType" };
            button.clicked += () =>
            {
                var menu = new GenericMenu();

                foreach (var item in TypeCache.GetTypesDerivedFrom<MWM_BulletType>())
                {
                    menu.AddItem(
                        new GUIContent(item.Name),
                        false,
                        () =>
                        {
                            var module = gun.gameObject.AddComponent(item);
                            gun.SetModule_BulletType(module as MWM_BulletType);
                        }
                    );
                }
                menu.ShowAsContext();
            };
            root.Add(button);
        }

        private static void AddAndSetModule_MuzzleFlash(ModularWeapon gun, VisualElement root)
        {
            var button = new Button(() => { }) { text = "Add & Assign MuzzleFlash" };
            button.clicked += () =>
            {
                var menu = new GenericMenu();

                foreach (var item in TypeCache.GetTypesDerivedFrom<MWM_MuzzleFlash>())
                {
                    menu.AddItem(
                        new GUIContent(item.Name),
                        false,
                        () =>
                        {
                            var module = gun.gameObject.AddComponent(item);
                            gun.SetModule_MuzzleFlash(module as MWM_MuzzleFlash);
                        }
                    );
                }
                menu.ShowAsContext();
            };
            root.Add(button);
        }

        private static void AddAndSetModule_Trigger(ModularWeapon gun, VisualElement root)
        {
            var button = new Button(() => { }) { text = "Add & Assign Trigger" };
            button.clicked += () =>
            {
                var menu = new GenericMenu();

                foreach (var item in TypeCache.GetTypesDerivedFrom<MWM_Trigger>())
                {
                    menu.AddItem(
                        new GUIContent(item.Name),
                        false,
                        () =>
                        {
                            var module = gun.gameObject.AddComponent(item);
                            gun.SetModule_Trigger(module as MWM_Trigger);
                        }
                    );
                }
                menu.ShowAsContext();
            };
            root.Add(button);
        }

        private static void AddAndSetModule_Shooter(ModularWeapon gun, VisualElement root)
        {
            var button = new Button(() => { }) { text = "Add & Assign Shooter" };
            button.clicked += () =>
            {
                var menu = new GenericMenu();

                foreach (var item in TypeCache.GetTypesDerivedFrom<MWM_Shooter>())
                {
                    menu.AddItem(
                        new GUIContent(item.Name),
                        false,
                        () =>
                        {
                            var module = gun.gameObject.AddComponent(item);
                            gun.SetModule_Shooter(module as MWM_Shooter);
                        }
                    );
                }
                menu.ShowAsContext();
            };
            root.Add(button);
        }

        private static void AddAndSetModule_Reload(ModularWeapon gun, VisualElement root)
        {
            var button = new Button(() => { }) { text = "Add & Assign Reload" };
            button.clicked += () =>
            {
                var menu = new GenericMenu();

                foreach (var item in TypeCache.GetTypesDerivedFrom<MWM_Reload>())
                {
                    menu.AddItem(
                        new GUIContent(item.Name),
                        false,
                        () =>
                        {
                            var module = gun.gameObject.AddComponent(item);
                            gun.SetModule_Reload(module as MWM_Reload);
                        }
                    );
                }
                menu.ShowAsContext();
            };
            root.Add(button);
        }

        //public override VisualElement CreateInspectorGUI()
        //{
        // var target = (Gun)base.target;

        // DrawDefaultInspector();

        // EditorGUILayout.Space(10);

        // if (target.gunModule_Input == null) EditorGUILayout.HelpBox("Input Module reference is missing!", MessageType.Error);
        // if (target.gunModule_Aimer == null) EditorGUILayout.HelpBox("Aimer Module reference is missing!", MessageType.Error);
        // if (target.gunModule_Ammo == null) EditorGUILayout.HelpBox("Ammon Module reference is missing!", MessageType.Error);
        // if (target.gunModule_BulletType == null) EditorGUILayout.HelpBox("BulletType Module reference is missing!", MessageType.Error);
        // if (target.gunModule_MuzzleFlash == null) EditorGUILayout.HelpBox("MuzzleFlash Module reference is missing!", MessageType.Error);
        // if (target.gunModule_Trigger == null) EditorGUILayout.HelpBox("Trigger Module reference is missing!", MessageType.Error);
        // if (target.gunModule_Shooter == null) EditorGUILayout.HelpBox("Shooter Module reference is missing!", MessageType.Error);
        // if (target.gunModule_Reload == null) EditorGUILayout.HelpBox("Reload Module reference is missing!", MessageType.Error);
        // if (target.fpsCamera == null) EditorGUILayout.HelpBox("Reload Module reference is missing!", MessageType.Error);
        // if (target.animationManager == null) EditorGUILayout.HelpBox("Reload Module reference is missing!", MessageType.Error);


        // serializedObject.ApplyModifiedProperties();
        //}
    }

#endif

    #endregion Editor ---------------------------------------------------------------------- </Reg: Editor>
}
