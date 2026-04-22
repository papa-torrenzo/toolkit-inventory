#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// Drop anywhere inside an Editor folder.   Menu: Tools ▶ PROTO ▶ Kanban Board

namespace SABI
{
    // ─────────────────────────────────────────────────────────────────────────────
    //  STYLE HELPERS
    // ─────────────────────────────────────────────────────────────────────────────
    internal static class SE
    {
        public static void Radius(this IStyle s, float r) =>
            s.borderTopLeftRadius =
                s.borderTopRightRadius =
                s.borderBottomLeftRadius =
                s.borderBottomRightRadius =
                    r;

        public static void BorderW(this IStyle s, float w) =>
            s.borderTopWidth = s.borderRightWidth = s.borderBottomWidth = s.borderLeftWidth = w;

        public static void BorderC(this IStyle s, Color c) =>
            s.borderTopColor = s.borderRightColor = s.borderBottomColor = s.borderLeftColor = c;

        public static void Pad(this IStyle s, float tb, float lr) =>
            s.paddingTop = s.paddingBottom = tb; // second assignment below:

        public static void PadAll(this IStyle s, float v) =>
            s.paddingTop = s.paddingBottom = s.paddingLeft = s.paddingRight = v;

        public static void PadTBLR(this IStyle s, float tb, float lr)
        {
            s.paddingTop = s.paddingBottom = tb;
            s.paddingLeft = s.paddingRight = lr;
        }

        public static void Margin(this IStyle s, float v) =>
            s.marginTop = s.marginBottom = s.marginLeft = s.marginRight = v;
    }

    // ─────────────────────────────────────────────────────────────────────────────
    //  ASSET REFERENCE
    // ─────────────────────────────────────────────────────────────────────────────
    [Serializable]
    public class AssetRef
    {
        public string globalId = "";
        public string label = "";
        public string typeName = "";

        public UnityEngine.Object Resolve() =>
            !string.IsNullOrEmpty(globalId) && GlobalObjectId.TryParse(globalId, out var g)
                ? GlobalObjectId.GlobalObjectIdentifierToObjectSlow(g)
                : null;

        public static AssetRef From(UnityEngine.Object obj)
        {
            if (obj == null)
                return null;
            var gid = GlobalObjectId.GetGlobalObjectIdSlow(obj);
            bool pfb = obj is GameObject && PrefabUtility.IsPartOfPrefabAsset(obj);
            string tp =
                pfb ? "Prefab"
                : obj is GameObject ? "GameObject"
                : obj is SceneAsset ? "Scene"
                : obj.GetType().Name;
            return new AssetRef
            {
                globalId = gid.ToString(),
                label = obj.name,
                typeName = tp,
            };
        }

        public AssetRef Clone() =>
            new AssetRef
            {
                globalId = globalId,
                label = label,
                typeName = typeName,
            };
    }

    // ─────────────────────────────────────────────────────────────────────────────
    //  CUSTOM PROPERTY DEFINITION
    //  values is List<string> — JsonUtility serialises List<string> fine in Unity 2020+
    // ─────────────────────────────────────────────────────────────────────────────
    [Serializable]
    public class PropDef
    {
        public string id = "";
        public string key = "";
        public List<string> values = new List<string>();
        public bool multiSelect = false;

        // ── Transient edit state ─────────────────────────────────────────────────
        [NonSerialized]
        public bool editing = false;

        [NonSerialized]
        public string editKey = "";

        [NonSerialized]
        public List<string> editVals = new List<string>();

        [NonSerialized]
        public bool editMulti = false;

        [NonSerialized]
        public string newValDraft = "";
    }

    // ─────────────────────────────────────────────────────────────────────────────
    //  PROPERTY ASSIGNMENT  (per-task value for one PropDef)
    // ─────────────────────────────────────────────────────────────────────────────
    [Serializable]
    public class PropAssignment
    {
        public string propId = "";
        public List<string> selected = new List<string>();

        public bool Has(string val) => selected != null && selected.Contains(val);

        public void Toggle(string val, bool multi)
        {
            selected ??= new List<string>();
            if (Has(val))
                selected.Remove(val);
            else
            {
                if (!multi)
                    selected.Clear();
                selected.Add(val);
            }
        }

        public PropAssignment Clone() =>
            new PropAssignment
            {
                propId = propId,
                selected = selected?.ToList() ?? new List<string>(),
            };
    }

    // ─────────────────────────────────────────────────────────────────────────────
    //  KANBAN TASK
    // ─────────────────────────────────────────────────────────────────────────────
    [Serializable]
    public class KanbanTask
    {
        public string id = "";
        public string title = "";
        public string description = "";
        public string tagsRaw = ""; // comma-joined (freeform)
        public int priority = 0; // 0 Low · 1 Med · 2 High
        public long createdTicks = 0;
        public bool isExpanded = true;
        public List<AssetRef> assetRefs = new List<AssetRef>();
        public List<PropAssignment> props = new List<PropAssignment>();

        // ── Transient edit state ─────────────────────────────────────────────────
        [NonSerialized]
        public bool editing = false;

        [NonSerialized]
        public string editTitle = "";

        [NonSerialized]
        public string editDesc = "";

        [NonSerialized]
        public string editTags = "";

        [NonSerialized]
        public int editPriority = 0;

        [NonSerialized]
        public List<AssetRef> editRefs = new List<AssetRef>();

        [NonSerialized]
        public List<PropAssignment> editProps = new List<PropAssignment>();

        public IEnumerable<string> Tags =>
            string.IsNullOrWhiteSpace(tagsRaw)
                ? Enumerable.Empty<string>()
                : tagsRaw.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0);

        public static string JoinTags(string csv) =>
            string.Join(", ", csv.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0));

        // Get or create an editProp entry for a given propId (for edit mode).
        public PropAssignment EditPropFor(string propId)
        {
            var ep = editProps?.FirstOrDefault(p => p.propId == propId);
            if (ep != null)
                return ep;
            ep = new PropAssignment { propId = propId };
            editProps ??= new List<PropAssignment>();
            editProps.Add(ep);
            return ep;
        }

        // Get committed assignment.
        public PropAssignment GetProp(string propId) =>
            props?.FirstOrDefault(p => p.propId == propId);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    //  KANBAN COLUMN  (replaces hard-coded todo/progress/done)
    // ─────────────────────────────────────────────────────────────────────────────
    [Serializable]
    public class KanbanColumn
    {
        public string id = "";
        public string name = "";
        public List<KanbanTask> tasks = new List<KanbanTask>();

        // ── Transient ────────────────────────────────────────────────────────────
        [NonSerialized]
        public bool editing = false;

        [NonSerialized]
        public string editName = "";
    }

    // ─────────────────────────────────────────────────────────────────────────────
    //  BOARD DATA
    // ─────────────────────────────────────────────────────────────────────────────
    [Serializable]
    public class KanbanBoardData
    {
        public List<KanbanColumn> columns = new List<KanbanColumn>();
        public List<PropDef> propDefs = new List<PropDef>();
    }

    // Legacy format wrapper — used only for migration of old save files.
    [Serializable]
    class LegacyBoard
    {
        public List<KanbanTask> todo = new List<KanbanTask>();
        public List<KanbanTask> progress = new List<KanbanTask>();
        public List<KanbanTask> done = new List<KanbanTask>();
    }

    // ─────────────────────────────────────────────────────────────────────────────
    //  EDITOR WINDOW
    // ─────────────────────────────────────────────────────────────────────────────
    public class KanbanBoard : EditorWindow
    {
        // ── Constants ────────────────────────────────────────────────────────────
        const string DRAG_ID = "PROTO_KANBAN_DRAG";
        static readonly string[] PriorityNames = { "Low", "Med", "High" };
        static readonly string[] SortLabels =
        {
            "Default",
            "Priority ↑",
            "Priority ↓",
            "Newest",
            "Oldest",
            "A–Z",
        };
        static readonly Color AccLow = new Color(0.28f, 0.68f, 1.00f);
        static readonly Color AccMed = new Color(1.00f, 0.76f, 0.12f);
        static readonly Color AccHigh = new Color(1.00f, 0.28f, 0.28f);

        // Column header accent colours (cycled by column index)
        static readonly Color[] ColAccents =
        {
            new Color(0.30f, 0.55f, 0.90f),
            new Color(0.60f, 0.35f, 0.90f),
            new Color(0.25f, 0.72f, 0.48f),
            new Color(0.90f, 0.58f, 0.20f),
            new Color(0.90f, 0.32f, 0.42f),
            new Color(0.22f, 0.72f, 0.80f),
            new Color(0.82f, 0.78f, 0.22f),
            new Color(0.72f, 0.42f, 0.32f),
        };

        // ── Drag payload ─────────────────────────────────────────────────────────
        sealed class Drag
        {
            public KanbanTask task;
            public KanbanColumn srcCol;
        }

        // ── Persistence ──────────────────────────────────────────────────────────
        KanbanBoardData data;
        string savePath;

        // ── UI state ─────────────────────────────────────────────────────────────
        string search = "";
        int sortMode = 0;
        string addTitle = "",
            addDesc = "",
            addTags = "";
        int addPriority = 0,
            addColIdx = 0;
        bool settingsOpen = false;

        // Pending new items (not committed to data until user confirms)
        PropDef pendingNewProp = null;
        bool showNewColForm = false;
        string newColDraft = "";

        // ── Live element refs ─────────────────────────────────────────────────────
        List<(VisualElement el, KanbanColumn col)> colElements = new();
        VisualElement boardScroll,
            settingsPanel;
        ScrollView settingsSV;
        ProgressBar pbar;
        Label pbarLabel;
        DropdownField addColDd; // kept to refresh choices when columns change

        // ─────────────────────────────────────────────────────────────────────────
        [MenuItem("Tools/Sabi/Kanban Board")]
        static void Open()
        {
            var w = GetWindow<KanbanBoard>("Kanban");
            w.minSize = new Vector2(920, 560);
        }

        void OnEnable()
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            savePath = Path.Combine(root, "UserSettings", "PROTO_Kanban.json");
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Load();
        }

        void OnDisable() => Save();

        void CreateGUI()
        {
            if (data == null)
                Load();

            var root = rootVisualElement;
            root.Clear();
            root.style.flexDirection = FlexDirection.Column;
            root.style.flexGrow = 1;
            root.style.backgroundColor = Th.rootBg;

            root.Add(BuildToolbar());
            root.Add(BuildAddBar());

            // Main area: board (flex) + settings side-panel (fixed 360px)
            var main = new VisualElement();
            main.style.flexDirection = FlexDirection.Row;
            main.style.flexGrow = 1;

            boardScroll = new VisualElement();
            boardScroll.name = "board-area";
            boardScroll.style.flexGrow = 1;
            boardScroll.style.flexDirection = FlexDirection.Row;
            main.Add(boardScroll);

            settingsPanel = BuildSettingsPanel();
            settingsPanel.style.display = settingsOpen ? DisplayStyle.Flex : DisplayStyle.None;
            main.Add(settingsPanel);

            root.Add(main);

            // Root-level drag events (fixes DragLeaveEvent-on-child-enter bug)
            root.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            root.RegisterCallback<DragPerformEvent>(OnDragPerform);
            root.RegisterCallback<DragExitedEvent>(_ => ClearDropHighlights());

            RebuildColumns();
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  THEME
        // ─────────────────────────────────────────────────────────────────────────
        struct Palette
        {
            public Color rootBg,
                colBg,
                colDrop,
                colBorder;
            public Color cardBg,
                cardHov,
                cardBorder;
            public Color toolBg,
                addBg,
                settBg,
                settSecBg;
            public Color border,
                border2;
            public Color text,
                dim,
                faint,
                accent;
            public Color tagBg,
                tagFg;
            public Color refBg,
                refFg;
            public Color propBg,
                propFg;
            public Color btnBg,
                btnFg,
                btnDanBg;
            public Color inputBg;
            public Color hdrOverlay;
        }

        Palette Th => EditorGUIUtility.isProSkin ? _dk : _lt;

        static readonly Palette _dk = new Palette
        {
            rootBg = new Color(0.16f, 0.16f, 0.16f),
            colBg = new Color(0.22f, 0.22f, 0.22f),
            colDrop = new Color(0.18f, 0.32f, 0.52f),
            colBorder = new Color(0.08f, 0.08f, 0.08f),
            cardBg = new Color(0.19f, 0.19f, 0.19f),
            cardHov = new Color(0.26f, 0.26f, 0.26f),
            cardBorder = new Color(0.10f, 0.10f, 0.10f),
            toolBg = new Color(0.12f, 0.12f, 0.12f),
            addBg = new Color(0.15f, 0.15f, 0.15f),
            settBg = new Color(0.13f, 0.13f, 0.13f),
            settSecBg = new Color(0.18f, 0.18f, 0.18f),
            border = new Color(0.08f, 0.08f, 0.08f),
            border2 = new Color(0.28f, 0.28f, 0.28f),
            text = new Color(0.90f, 0.90f, 0.90f),
            dim = new Color(0.60f, 0.60f, 0.60f),
            faint = new Color(0.38f, 0.38f, 0.38f),
            accent = new Color(0.30f, 0.60f, 1.00f),
            tagBg = new Color(0.13f, 0.28f, 0.46f),
            tagFg = new Color(0.72f, 0.87f, 1.00f),
            refBg = new Color(0.12f, 0.28f, 0.14f),
            refFg = new Color(0.65f, 0.92f, 0.68f),
            propBg = new Color(0.28f, 0.18f, 0.40f),
            propFg = new Color(0.88f, 0.72f, 1.00f),
            btnBg = new Color(0.26f, 0.26f, 0.26f),
            btnFg = new Color(0.85f, 0.85f, 0.85f),
            btnDanBg = new Color(0.60f, 0.14f, 0.14f),
            inputBg = new Color(0.14f, 0.14f, 0.14f),
            hdrOverlay = new Color(0f, 0f, 0f, 0.25f),
        };

        static readonly Palette _lt = new Palette
        {
            rootBg = new Color(0.76f, 0.76f, 0.76f),
            colBg = new Color(0.70f, 0.70f, 0.70f),
            colDrop = new Color(0.58f, 0.73f, 0.92f),
            colBorder = new Color(0.50f, 0.50f, 0.50f),
            cardBg = new Color(0.85f, 0.85f, 0.85f),
            cardHov = new Color(0.92f, 0.92f, 0.92f),
            cardBorder = new Color(0.60f, 0.60f, 0.60f),
            toolBg = new Color(0.64f, 0.64f, 0.64f),
            addBg = new Color(0.68f, 0.68f, 0.68f),
            settBg = new Color(0.61f, 0.61f, 0.61f),
            settSecBg = new Color(0.72f, 0.72f, 0.72f),
            border = new Color(0.50f, 0.50f, 0.50f),
            border2 = new Color(0.60f, 0.60f, 0.60f),
            text = new Color(0.08f, 0.08f, 0.08f),
            dim = new Color(0.26f, 0.26f, 0.26f),
            faint = new Color(0.44f, 0.44f, 0.44f),
            accent = new Color(0.15f, 0.42f, 0.85f),
            tagBg = new Color(0.55f, 0.73f, 0.94f),
            tagFg = new Color(0.04f, 0.16f, 0.38f),
            refBg = new Color(0.58f, 0.84f, 0.60f),
            refFg = new Color(0.04f, 0.26f, 0.07f),
            propBg = new Color(0.78f, 0.62f, 0.92f),
            propFg = new Color(0.20f, 0.04f, 0.38f),
            btnBg = new Color(0.62f, 0.62f, 0.62f),
            btnFg = new Color(0.10f, 0.10f, 0.10f),
            btnDanBg = new Color(0.88f, 0.25f, 0.25f),
            inputBg = new Color(0.80f, 0.80f, 0.80f),
            hdrOverlay = new Color(0f, 0f, 0f, 0.08f),
        };

        // Deterministic prop colour from id string
        static (Color bg, Color fg) PropColor(string id, bool dark)
        {
            uint h = 2166136261u;
            foreach (char c in id)
            {
                h ^= c;
                h *= 16777619u;
            }
            float hue = (h % 1000u) / 1000f;
            Color bg = Color.HSVToRGB(hue, dark ? 0.52f : 0.42f, dark ? 0.38f : 0.82f);
            Color fg = Color.HSVToRGB(hue, dark ? 0.28f : 0.78f, dark ? 0.94f : 0.22f);
            return (bg, fg);
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  TOOLBAR
        // ─────────────────────────────────────────────────────────────────────────
        VisualElement BuildToolbar()
        {
            var t = Th;
            var bar = Row(t.toolBg, 30);
            bar.style.borderBottomWidth = 1;
            bar.style.borderBottomColor = t.border;
            bar.style.PadTBLR(0, 8);

            // Search
            var sf = new ToolbarSearchField();
            sf.SetValueWithoutNotify(search);
            sf.style.width = 200;
            sf.style.marginRight = 4;
            sf.RegisterValueChangedCallback(e =>
            {
                search = e.newValue;
                RebuildColumns();
            });
            bar.Add(sf);

            if (!string.IsNullOrEmpty(search))
            {
                var clr = SmallBtn(
                    "✕",
                    () =>
                    {
                        search = "";
                        sf.SetValueWithoutNotify("");
                        RebuildColumns();
                    }
                );
                clr.style.marginRight = 6;
                bar.Add(clr);
            }

            // Sort
            var sd = new DropdownField();
            sd.choices = SortLabels.ToList();
            sd.index = sortMode;
            sd.style.width = 118;
            sd.style.marginRight = 10;
            sd.RegisterValueChangedCallback(_ =>
            {
                sortMode = sd.index;
                RebuildColumns();
            });
            bar.Add(sd);

            bar.Add(Flex());

            // Progress
            pbar = new ProgressBar();
            pbar.lowValue = 0;
            pbar.highValue = 100;
            pbar.style.width = 110;
            pbar.style.marginRight = 4;
            bar.Add(pbar);
            pbarLabel = MkLbl("", 11, t.dim);
            pbarLabel.style.marginRight = 10;
            bar.Add(pbarLabel);

            bar.Add(TBtn("Export MD", ExportMarkdown, marginR: 4));
            bar.Add(TBtn("Clear Done", ClearDone, marginR: 4));

            // Settings toggle
            var settBtn = TBtn(
                settingsOpen ? "✕ Settings" : "⚙ Settings",
                () =>
                {
                    settingsOpen = !settingsOpen;
                    if (settingsPanel != null)
                        settingsPanel.style.display = settingsOpen
                            ? DisplayStyle.Flex
                            : DisplayStyle.None;
                    // Rebuild toolbar to flip button label
                    rootVisualElement.schedule.Execute(() => CreateGUI());
                }
            );
            settBtn.style.backgroundColor = settingsOpen ? t.accent : t.btnBg;
            settBtn.style.color = settingsOpen ? Color.white : t.btnFg;
            bar.Add(settBtn);

            return bar;
        }

        void ClearDone()
        {
            // "Done" might not be column[2] anymore — find by name or just let user pick
            // For now clear the last column (convention) with a confirm.
            if (data.columns.Count == 0)
                return;
            if (
                EditorUtility.DisplayDialog(
                    "Clear Done",
                    "Permanently delete all tasks in the last column?",
                    "Delete",
                    "Cancel"
                )
            )
            {
                data.columns[data.columns.Count - 1].tasks.Clear();
                Save();
                RebuildColumns();
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  ADD-TASK BAR
        // ─────────────────────────────────────────────────────────────────────────
        VisualElement BuildAddBar()
        {
            var t = Th;
            var bar = Row(t.addBg, 30);
            bar.style.borderBottomWidth = 1;
            bar.style.borderBottomColor = t.border;
            bar.style.PadTBLR(4, 8);
            bar.style.flexWrap = Wrap.Wrap;

            bar.Add(BLbl("Title"));
            var titleTf = AddField(155, addTitle);
            titleTf.name = "add-title";
            titleTf.RegisterValueChangedCallback(e => addTitle = e.newValue);
            bar.Add(titleTf);

            bar.Add(BLbl("Desc"));
            var descTf = AddField(0, addDesc, grow: true);
            descTf.RegisterValueChangedCallback(e => addDesc = e.newValue);
            bar.Add(descTf);

            bar.Add(BLbl("Tags"));
            var tagsTf = AddField(90, addTags);
            tagsTf.RegisterValueChangedCallback(e => addTags = e.newValue);
            bar.Add(tagsTf);

            bar.Add(BLbl("Pri"));
            var priDd = MkDropdown(PriorityNames.ToList(), addPriority, 56);
            priDd.RegisterValueChangedCallback(_ => addPriority = priDd.index);
            bar.Add(priDd);

            bar.Add(BLbl("Col"));
            addColDd = MkDropdown(data.columns.Select(c => c.name).ToList(), 0, 90);
            addColDd.RegisterValueChangedCallback(_ => addColIdx = addColDd.index);
            bar.Add(addColDd);

            void DoAdd()
            {
                if (string.IsNullOrWhiteSpace(addTitle))
                    return;
                var colIdx = Mathf.Clamp(addColIdx, 0, data.columns.Count - 1);
                if (data.columns.Count == 0)
                {
                    Debug.LogWarning("[PROTO Kanban] No columns exist.");
                    return;
                }
                data.columns[colIdx]
                    .tasks.Add(
                        new KanbanTask
                        {
                            id = Guid.NewGuid().ToString(),
                            title = addTitle.Trim(),
                            description = addDesc.Trim(),
                            tagsRaw = KanbanTask.JoinTags(addTags),
                            priority = addPriority,
                            createdTicks = DateTime.UtcNow.Ticks,
                            isExpanded = true,
                            assetRefs = new List<AssetRef>(),
                            props = new List<PropAssignment>(),
                        }
                    );
                addTitle = "";
                addDesc = "";
                addTags = "";
                titleTf.SetValueWithoutNotify("");
                descTf.SetValueWithoutNotify("");
                tagsTf.SetValueWithoutNotify("");
                Save();
                RebuildColumns();
            }

            titleTf.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return && !string.IsNullOrWhiteSpace(addTitle))
                {
                    DoAdd();
                    evt.StopPropagation();
                }
            });

            using (new EditorGUI.DisabledScope(false)) { } // placeholder for disabled-scope pattern
            var addBtn = MkBtn("＋ Add", DoAdd, width: 65);
            addBtn.style.marginLeft = 4;
            bar.Add(addBtn);

            return bar;
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  BOARD  (dynamic columns)
        // ─────────────────────────────────────────────────────────────────────────
        void RebuildColumns()
        {
            if (boardScroll == null)
                return;
            boardScroll.Clear();
            colElements.Clear();

            // Re-sync addColDd choices
            if (addColDd != null)
            {
                var names = data.columns.Select(c => c.name).ToList();
                addColDd.choices = names.Count > 0 ? names : new List<string> { "(none)" };
                if (addColDd.index >= names.Count)
                    addColDd.index = 0;
            }

            for (int i = 0; i < data.columns.Count; i++)
            {
                var col = data.columns[i];
                var accent = ColAccents[i % ColAccents.Length];
                var el = BuildColumnEl(col, i, accent);
                colElements.Add((el, col));
                boardScroll.Add(el);
            }

            RefreshProgress();
        }

        VisualElement BuildColumnEl(KanbanColumn col, int colIdx, Color accent)
        {
            var t = Th;

            var el = new VisualElement();
            el.style.flexGrow = 1;
            el.style.flexBasis = 0;
            el.style.marginLeft = el.style.marginRight = 4;
            el.style.marginTop = el.style.marginBottom = 6;
            el.style.backgroundColor = t.colBg;
            el.style.Radius(7);
            el.style.BorderW(1);
            el.style.BorderC(t.colBorder);
            el.style.overflow = Overflow.Hidden;

            // ── Column header ────────────────────────────────────────────────────
            var hdr = new VisualElement();
            hdr.style.backgroundColor = new Color(accent.r, accent.g, accent.b, 0.22f);
            hdr.style.borderBottomWidth = 1;
            hdr.style.borderBottomColor = new Color(accent.r, accent.g, accent.b, 0.55f);
            hdr.style.PadTBLR(6, 10);
            hdr.style.flexDirection = FlexDirection.Row;
            hdr.style.alignItems = Align.Center;

            // Left accent bar
            var bar = new VisualElement();
            bar.style.width = 3;
            bar.style.Radius(2);
            bar.style.backgroundColor = accent;
            bar.style.marginRight = 7;
            bar.style.height = 16;
            hdr.Add(bar);

            var hdrLbl = MkLbl($"{col.name}  ({col.tasks.Count})", 13, t.text, bold: true);
            hdrLbl.style.flexGrow = 1;
            hdrLbl.style.unityTextAlign = TextAnchor.MiddleLeft;
            hdr.Add(hdrLbl);
            el.Add(hdr);

            // ── Scrollable card area ─────────────────────────────────────────────
            var sv = new ScrollView(ScrollViewMode.Vertical);
            sv.style.flexGrow = 1;
            sv.contentContainer.style.paddingLeft = sv.contentContainer.style.paddingRight = 5;
            sv.contentContainer.style.paddingTop = sv.contentContainer.style.paddingBottom = 4;

            // ── Populate cards ───────────────────────────────────────────────────
            foreach (var task in View(col.tasks))
                sv.contentContainer.Add(BuildCard(task, col));

            el.Add(sv);
            return el;
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  SETTINGS PANEL
        // ─────────────────────────────────────────────────────────────────────────
        VisualElement BuildSettingsPanel()
        {
            var t = Th;
            var panel = new VisualElement();
            panel.style.width = 365;
            panel.style.backgroundColor = t.settBg;
            panel.style.borderLeftWidth = 1;
            panel.style.borderLeftColor = t.border;

            // Header
            var hdr = Row(
                new Color(t.settBg.r - 0.03f, t.settBg.g - 0.03f, t.settBg.b - 0.03f),
                36
            );
            hdr.style.PadTBLR(0, 12);
            hdr.style.borderBottomWidth = 1;
            hdr.style.borderBottomColor = t.border;

            var title = MkLbl("⚙  Settings", 13, t.text, bold: true);
            title.style.flexGrow = 1;
            hdr.Add(title);

            var closBtn = SmallBtn(
                "✕",
                () =>
                {
                    settingsOpen = false;
                    settingsPanel.style.display = DisplayStyle.None;
                    rootVisualElement.schedule.Execute(() => CreateGUI());
                }
            );
            hdr.Add(closBtn);
            panel.Add(hdr);

            // Scrollable content
            settingsSV = new ScrollView(ScrollViewMode.Vertical);
            settingsSV.style.flexGrow = 1;
            panel.Add(settingsSV);

            RebuildSettingsContent();
            return panel;
        }

        void RebuildSettingsContent()
        {
            if (settingsSV == null)
                return;
            settingsSV.contentContainer.Clear();
            var c = settingsSV.contentContainer;
            var t = Th;

            // ── Section: Columns ─────────────────────────────────────────────────
            c.Add(SectionHeader("COLUMNS", t));

            for (int i = 0; i < data.columns.Count; i++)
            {
                var col = data.columns[i];
                int ci = i; // capture for closures
                c.Add(BuildColumnSettingsRow(col, ci));
            }

            // Add-column form
            if (showNewColForm)
            {
                var addRow = Row(t.settSecBg, 30);
                addRow.style.marginTop = 4;
                addRow.style.PadTBLR(4, 8);
                addRow.style.Radius(5);

                var tf = new TextField();
                tf.SetValueWithoutNotify(newColDraft);
                tf.style.flexGrow = 1;
                tf.style.marginRight = 4;
                tf.RegisterValueChangedCallback(e => newColDraft = e.newValue);
                tf.RegisterCallback<KeyDownEvent>(evt =>
                {
                    if (evt.keyCode == KeyCode.Return)
                        CommitNewColumn();
                });
                addRow.Add(tf);

                addRow.Add(MkBtn("Add", CommitNewColumn, width: 44, height: 22));
                addRow.Add(
                    MkBtn(
                        "✕",
                        () =>
                        {
                            showNewColForm = false;
                            newColDraft = "";
                            rootVisualElement.schedule.Execute(RebuildSettingsContent);
                        },
                        width: 24,
                        height: 22
                    )
                );

                c.Add(addRow);

                // Schedule focus so the text field is ready
                rootVisualElement.schedule.Execute(() =>
                {
                    if (tf != null && tf.panel != null)
                        tf.Focus();
                });
            }

            var addColBtn = MkBtn(
                "＋ Add Column",
                () =>
                {
                    showNewColForm = true;
                    newColDraft = "";
                    rootVisualElement.schedule.Execute(RebuildSettingsContent);
                },
                marginT: 8
            );
            addColBtn.style.alignSelf = Align.FlexStart;
            addColBtn.style.marginLeft = 8;
            c.Add(addColBtn);

            // Divider
            c.Add(Divider(t));

            // ── Section: Custom Properties ────────────────────────────────────────
            c.Add(SectionHeader("CUSTOM PROPERTIES", t));

            if (data.propDefs.Count == 0 && pendingNewProp == null)
            {
                var emptyLbl = MkLbl("No custom properties yet.", 11, t.faint);
                emptyLbl.style.marginLeft = 12;
                emptyLbl.style.marginBottom = 6;
                c.Add(emptyLbl);
            }

            foreach (var pd in data.propDefs)
                c.Add(BuildPropSettingsRow(pd, isPending: false));

            if (pendingNewProp != null)
                c.Add(BuildPropSettingsRow(pendingNewProp, isPending: true));

            var addPropBtn = MkBtn(
                "＋ Add Property",
                () =>
                {
                    pendingNewProp = new PropDef
                    {
                        id = Guid.NewGuid().ToString(),
                        editing = true,
                        editKey = "",
                        editVals = new List<string>(),
                        editMulti = false,
                    };
                    rootVisualElement.schedule.Execute(RebuildSettingsContent);
                },
                marginT: 8
            );
            addPropBtn.style.alignSelf = Align.FlexStart;
            addPropBtn.style.marginLeft = 8;
            c.Add(addPropBtn);

            c.Add(new VisualElement { style = { height = 20 } }); // bottom padding
        }

        // ── Column settings row ───────────────────────────────────────────────────
        VisualElement BuildColumnSettingsRow(KanbanColumn col, int ci)
        {
            var t = Th;
            var accent = ColAccents[ci % ColAccents.Length];

            var outer = new VisualElement();
            outer.style.marginTop = 3;
            outer.style.marginLeft = outer.style.marginRight = 6;

            if (col.editing)
            {
                // ── Rename form ──────────────────────────────────────────────────
                var form = Row(t.settSecBg, 32);
                form.style.PadTBLR(4, 8);
                form.style.Radius(5);

                var tf = new TextField();
                tf.SetValueWithoutNotify(col.editName);
                tf.style.flexGrow = 1;
                tf.style.marginRight = 6;
                tf.RegisterValueChangedCallback(e => col.editName = e.newValue);
                tf.RegisterCallback<KeyDownEvent>(evt =>
                {
                    if (evt.keyCode == KeyCode.Return)
                        CommitColumnRename(col);
                });
                form.Add(tf);

                form.Add(MkBtn("✓", () => CommitColumnRename(col), width: 24, height: 22));
                form.Add(
                    MkBtn(
                        "✕",
                        () =>
                        {
                            col.editing = false;
                            rootVisualElement.schedule.Execute(RebuildSettingsContent);
                        },
                        width: 24,
                        height: 22
                    )
                );

                outer.Add(form);

                rootVisualElement.schedule.Execute(() =>
                {
                    if (tf != null && tf.panel != null)
                        tf.Focus();
                });
            }
            else
            {
                // ── Display row ──────────────────────────────────────────────────
                var row = Row(t.settSecBg, 32);
                row.style.PadTBLR(0, 8);
                row.style.Radius(5);

                var dot = new VisualElement();
                dot.style.width = 3;
                dot.style.height = 18;
                dot.style.Radius(2);
                dot.style.backgroundColor = accent;
                dot.style.marginRight = 8;
                row.Add(dot);

                var lbl = MkLbl(col.name, 12, t.text, bold: true);
                lbl.style.flexGrow = 1;
                row.Add(lbl);

                var cnt = MkLbl($"{col.tasks.Count}", 11, t.faint);
                cnt.style.marginRight = 8;
                row.Add(cnt);

                // Reorder
                if (ci > 0)
                    row.Add(
                        SmallBtn(
                            "↑",
                            () =>
                            {
                                SwapColumns(ci, ci - 1);
                                rootVisualElement.schedule.Execute(RebuildSettingsContent);
                            }
                        )
                    );
                if (ci < data.columns.Count - 1)
                    row.Add(
                        SmallBtn(
                            "↓",
                            () =>
                            {
                                SwapColumns(ci, ci + 1);
                                rootVisualElement.schedule.Execute(RebuildSettingsContent);
                            }
                        )
                    );

                row.Add(
                    SmallBtn(
                        "✎",
                        () =>
                        {
                            col.editName = col.name;
                            col.editing = true;
                            rootVisualElement.schedule.Execute(RebuildSettingsContent);
                        }
                    )
                );

                var del = SmallBtn("🗑", () => DeleteColumn(col, ci));
                del.style.backgroundColor = t.btnDanBg;
                del.style.color = Color.white;
                row.Add(del);

                outer.Add(row);
            }

            return outer;
        }

        // ── Property settings row ─────────────────────────────────────────────────
        VisualElement BuildPropSettingsRow(PropDef pd, bool isPending)
        {
            var t = Th;
            var (pbg, pfg) = PropColor(pd.id, EditorGUIUtility.isProSkin);

            var outer = new VisualElement();
            outer.style.marginTop = 4;
            outer.style.marginLeft = outer.style.marginRight = 6;

            if (pd.editing)
            {
                // ── Full edit form ───────────────────────────────────────────────
                var form = new VisualElement();
                form.style.backgroundColor = t.settSecBg;
                form.style.Radius(6);
                form.style.PadAll(10);

                // Key row
                var keyRow = Row(Color.clear, 26);
                keyRow.style.marginBottom = 6;
                keyRow.Add(MkLbl("Key  ", 11, t.dim));

                var keyTf = new TextField();
                keyTf.SetValueWithoutNotify(pd.editKey);
                keyTf.style.flexGrow = 1;
                keyTf.style.marginRight = 8;
                keyTf.RegisterValueChangedCallback(e => pd.editKey = e.newValue);
                keyRow.Add(keyTf);

                var multiTog = new Toggle("Multi-select") { value = pd.editMulti };
                multiTog.style.marginLeft = 0;
                multiTog.RegisterValueChangedCallback(e => pd.editMulti = e.newValue);
                keyRow.Add(multiTog);
                form.Add(keyRow);

                // Values list
                form.Add(MkLbl("Values:", 11, t.dim));

                var valsBox = new VisualElement();
                valsBox.style.marginTop = 4;
                valsBox.style.marginBottom = 6;
                form.Add(valsBox);

                void RefreshValsUI()
                {
                    valsBox.Clear();
                    for (int vi = 0; vi < pd.editVals.Count; vi++)
                    {
                        int idx = vi; // capture
                        var vr = Row(Color.clear, 26);
                        vr.style.marginBottom = 3;

                        var vtf = new TextField();
                        vtf.SetValueWithoutNotify(pd.editVals[idx]);
                        vtf.style.flexGrow = 1;
                        vtf.style.marginRight = 4;
                        vtf.RegisterValueChangedCallback(e => pd.editVals[idx] = e.newValue);
                        vr.Add(vtf);

                        var xb = SmallBtn(
                            "×",
                            () =>
                            {
                                pd.editVals.RemoveAt(idx);
                                rootVisualElement.schedule.Execute(() => RefreshValsUI());
                            }
                        );
                        xb.style.backgroundColor = t.btnDanBg;
                        xb.style.color = Color.white;
                        vr.Add(xb);
                        valsBox.Add(vr);
                    }

                    // Add-value row
                    var addVRow = Row(Color.clear, 26);
                    addVRow.style.marginTop = 2;

                    var newVTf = new TextField { };
                    newVTf.SetValueWithoutNotify(pd.newValDraft);
                    newVTf.style.flexGrow = 1;
                    newVTf.style.marginRight = 4;
                    newVTf.RegisterValueChangedCallback(e => pd.newValDraft = e.newValue);

                    void CommitVal()
                    {
                        var v = pd.newValDraft?.Trim();
                        if (string.IsNullOrEmpty(v))
                            return;
                        pd.editVals.Add(v);
                        pd.newValDraft = "";
                        newVTf.SetValueWithoutNotify("");
                        rootVisualElement.schedule.Execute(() => RefreshValsUI());
                    }

                    newVTf.RegisterCallback<KeyDownEvent>(evt =>
                    {
                        if (evt.keyCode == KeyCode.Return)
                            CommitVal();
                    });

                    addVRow.Add(newVTf);
                    addVRow.Add(MkBtn("＋", CommitVal, width: 26, height: 22));
                    valsBox.Add(addVRow);
                }
                RefreshValsUI();

                // Save / Cancel buttons
                var btnRow = Row(Color.clear, 26);
                btnRow.style.marginTop = 6;

                btnRow.Add(
                    MkBtn(
                        "💾 Save",
                        () =>
                        {
                            if (isPending)
                                CommitNewProp(pd);
                            else
                                CommitEditProp(pd);
                        },
                        grow: true,
                        height: 24,
                        marginR: 4
                    )
                );

                btnRow.Add(
                    MkBtn(
                        "✖ Cancel",
                        () =>
                        {
                            if (isPending)
                            {
                                pendingNewProp = null;
                            }
                            else
                            {
                                pd.editing = false;
                            }
                            rootVisualElement.schedule.Execute(RebuildSettingsContent);
                        },
                        grow: true,
                        height: 24
                    )
                );

                form.Add(btnRow);
                outer.Add(form);
            }
            else
            {
                // ── Compact display row ──────────────────────────────────────────
                var row = new VisualElement();
                row.style.backgroundColor = t.settSecBg;
                row.style.Radius(5);
                row.style.PadTBLR(6, 8);

                var hdrRow = Row(Color.clear, 22);

                // Coloured dot
                var dot = new VisualElement();
                dot.style.width = 10;
                dot.style.height = 10;
                dot.style.Radius(5);
                dot.style.backgroundColor = pfg;
                dot.style.marginRight = 7;
                dot.style.alignSelf = Align.Center;
                hdrRow.Add(dot);

                var keyLbl = MkLbl(
                    string.IsNullOrEmpty(pd.key) ? "(unnamed)" : pd.key,
                    12,
                    t.text,
                    bold: true
                );
                keyLbl.style.flexGrow = 1;
                hdrRow.Add(keyLbl);

                var mLbl = MkLbl(pd.multiSelect ? "  ☑ Multi" : "  ☐ Single", 10, t.faint);
                mLbl.style.marginRight = 8;
                mLbl.style.alignSelf = Align.Center;
                hdrRow.Add(mLbl);

                hdrRow.Add(
                    SmallBtn(
                        "✎",
                        () =>
                        {
                            pd.editKey = pd.key;
                            pd.editVals = pd.values.ToList();
                            pd.editMulti = pd.multiSelect;
                            pd.editing = true;
                            rootVisualElement.schedule.Execute(RebuildSettingsContent);
                        }
                    )
                );

                var delBtn = SmallBtn("🗑", () => DeletePropDef(pd));
                delBtn.style.backgroundColor = t.btnDanBg;
                delBtn.style.color = Color.white;
                hdrRow.Add(delBtn);

                row.Add(hdrRow);

                // Values preview
                if (pd.values.Count > 0)
                {
                    var preview = Row(Color.clear, 0);
                    preview.style.flexWrap = Wrap.Wrap;
                    preview.style.marginTop = 4;
                    foreach (var v in pd.values)
                    {
                        var chip = new Label(v);
                        chip.style.backgroundColor = pbg;
                        chip.style.color = pfg;
                        chip.style.fontSize = 10;
                        chip.style.Radius(10);
                        chip.style.PadTBLR(2, 6);
                        chip.style.marginRight = 4;
                        chip.style.marginBottom = 3;
                        preview.Add(chip);
                    }
                    row.Add(preview);
                }
                else
                {
                    row.Add(MkLbl("  No values defined.", 10, t.faint));
                }

                outer.Add(row);
            }

            return outer;
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  CARD
        // ─────────────────────────────────────────────────────────────────────────
        VisualElement BuildCard(KanbanTask task, KanbanColumn col)
        {
            var t = Th;
            Color accent =
                task.priority == 2 ? AccHigh
                : task.priority == 1 ? AccMed
                : AccLow;

            var card = new VisualElement();
            card.style.backgroundColor = t.cardBg;
            card.style.Radius(5);
            card.style.borderLeftWidth = 4;
            card.style.borderLeftColor = accent;
            card.style.BorderW(1);
            card.style.BorderC(t.cardBorder);
            card.style.borderLeftWidth = 4; // re-assert after BorderW
            card.style.marginBottom = 5;
            card.style.PadTBLR(7, 10);

            card.RegisterCallback<MouseEnterEvent>(_ => card.style.backgroundColor = t.cardHov);
            card.RegisterCallback<MouseLeaveEvent>(_ => card.style.backgroundColor = t.cardBg);

            if (task.editing)
                BuildEditPanel(card, task, col);
            else
                BuildDisplayPanel(card, task, col);

            return card;
        }

        // ── Display panel ─────────────────────────────────────────────────────────
        void BuildDisplayPanel(VisualElement card, KanbanTask task, KanbanColumn col)
        {
            var t = Th;
            int colIdx = data.columns.IndexOf(col);

            // ── Header row ───────────────────────────────────────────────────────
            var hdr = Row(Color.clear, 0);
            hdr.style.alignItems = Align.Center;

            // Collapse toggle
            var tog = MkLbl(task.isExpanded ? "▾" : "▸", 13, t.dim);
            tog.style.width = 16;
            tog.style.marginRight = 3;
            tog.RegisterCallback<MouseDownEvent>(evt =>
            {
                task.isExpanded = !task.isExpanded;
                rootVisualElement.schedule.Execute(RebuildColumns);
                evt.StopPropagation();
            });
            hdr.Add(tog);

            // Priority dot
            Color acc =
                task.priority == 2 ? AccHigh
                : task.priority == 1 ? AccMed
                : AccLow;
            var dot = MkLbl("●", 9, acc);
            dot.style.marginRight = 5;
            hdr.Add(dot);

            // Title
            var titleLbl = MkLbl(task.title, 12, t.text, bold: true);
            titleLbl.style.flexGrow = 1;
            titleLbl.style.overflow = Overflow.Hidden;
            hdr.Add(titleLbl);

            // Quick-move ◀ ▶
            if (colIdx > 0)
                hdr.Add(MiniBtn("◀", () => DoMove(task, col, data.columns[colIdx - 1])));
            if (colIdx >= 0 && colIdx < data.columns.Count - 1)
                hdr.Add(MiniBtn("▶", () => DoMove(task, col, data.columns[colIdx + 1])));

            hdr.Add(
                MiniBtn(
                    "✏",
                    () =>
                    {
                        BeginEdit(task);
                        rootVisualElement.schedule.Execute(RebuildColumns);
                    }
                )
            );

            card.Add(hdr);
            if (!task.isExpanded)
                goto RegisterCardEvents;

            // ── Body ─────────────────────────────────────────────────────────────
            var body = new VisualElement();
            body.style.marginTop = 5;
            body.style.marginLeft = 22;

            if (!string.IsNullOrEmpty(task.description))
            {
                var desc = MkLbl(task.description, 11, t.dim);
                desc.style.whiteSpace = WhiteSpace.Normal;
                desc.style.marginBottom = 4;
                body.Add(desc);
            }

            // Tags
            var tags = task.Tags.ToList();
            if (tags.Count > 0)
            {
                var tagRow = ChipRow();
                foreach (var tag in tags)
                    tagRow.Add(Chip(tag, t.tagBg, t.tagFg));
                body.Add(tagRow);
            }

            // Custom properties (assigned only)
            foreach (var pd in data.propDefs)
            {
                var pa = task.GetProp(pd.id);
                if (pa == null || pa.selected == null || pa.selected.Count == 0)
                    continue;

                var propRow = Row(Color.clear, 0);
                propRow.style.flexWrap = Wrap.Wrap;
                propRow.style.marginBottom = 3;
                propRow.style.alignItems = Align.Center;

                var (pbg, pfg) = PropColor(pd.id, EditorGUIUtility.isProSkin);

                var keyChip = new Label($"{pd.key}:");
                keyChip.style.fontSize = 10;
                keyChip.style.color = pfg;
                keyChip.style.marginRight = 3;
                propRow.Add(keyChip);

                foreach (var sel in pa.selected)
                {
                    var vc = new Label(sel);
                    vc.style.backgroundColor = pbg;
                    vc.style.color = pfg;
                    vc.style.fontSize = 10;
                    vc.style.Radius(10);
                    vc.style.PadTBLR(2, 6);
                    vc.style.marginRight = 3;
                    propRow.Add(vc);
                }
                body.Add(propRow);
            }

            // Asset refs
            if (task.assetRefs != null && task.assetRefs.Count > 0)
            {
                var refRow = ChipRow();
                foreach (var ar in task.assetRefs)
                {
                    var chip = RefChip(ar, t, false, null);
                    chip.RegisterCallback<MouseDownEvent>(evt =>
                    {
                        if (evt.button != 0)
                            return;
                        var obj = ar.Resolve();
                        if (obj != null)
                        {
                            Selection.activeObject = obj;
                            EditorGUIUtility.PingObject(obj);
                        }
                        else
                            Debug.LogWarning($"[PROTO Kanban] Can't resolve '{ar.label}'");
                        evt.StopPropagation();
                    });
                    refRow.Add(chip);
                }
                body.Add(refRow);
            }

            // Date
            if (task.createdTicks > 0)
            {
                var dl = MkLbl(
                    new DateTime(task.createdTicks, DateTimeKind.Utc)
                        .ToLocalTime()
                        .ToString("MMM d, yyyy"),
                    10,
                    t.faint
                );
                dl.style.unityTextAlign = TextAnchor.MiddleRight;
                body.Add(dl);
            }

            card.Add(body);

            // ── Events ───────────────────────────────────────────────────────────
            RegisterCardEvents:
            RegisterCardInteraction(card, task, col);
        }

        // ── Edit panel ────────────────────────────────────────────────────────────
        void BuildEditPanel(VisualElement card, KanbanTask task, KanbanColumn col)
        {
            var t = Th;

            // Title
            card.Add(FieldLbl("Title"));
            var titleTf = new TextField();
            titleTf.SetValueWithoutNotify(task.editTitle);
            titleTf.style.marginBottom = 4;
            titleTf.RegisterValueChangedCallback(e => task.editTitle = e.newValue);
            card.Add(titleTf);

            // Description
            card.Add(FieldLbl("Description"));
            var descTf = new TextField { multiline = true };
            descTf.SetValueWithoutNotify(task.editDesc);
            descTf.style.minHeight = 42;
            descTf.style.whiteSpace = WhiteSpace.Normal;
            descTf.style.marginBottom = 4;
            descTf.RegisterValueChangedCallback(e => task.editDesc = e.newValue);
            card.Add(descTf);

            // Tags
            card.Add(FieldLbl("Tags (comma-separated)"));
            var tagsTf = new TextField();
            tagsTf.SetValueWithoutNotify(task.editTags);
            tagsTf.style.marginBottom = 4;
            tagsTf.RegisterValueChangedCallback(e => task.editTags = e.newValue);
            card.Add(tagsTf);

            // Priority
            card.Add(FieldLbl("Priority"));
            var priDd = MkDropdown(PriorityNames.ToList(), task.editPriority, 80);
            priDd.style.marginBottom = 8;
            priDd.RegisterValueChangedCallback(_ => task.editPriority = priDd.index);
            card.Add(priDd);

            // ── Custom Properties ────────────────────────────────────────────────
            if (data.propDefs.Count > 0)
            {
                card.Add(FieldLbl("Custom Properties"));
                foreach (var pd in data.propDefs)
                    card.Add(BuildEditPropPicker(task, pd));
                card.Add(new VisualElement { style = { height = 6 } });
            }
            else
            {
                var hint = MkLbl("No custom properties — create some in ⚙ Settings.", 10, t.faint);
                hint.style.marginBottom = 6;
                card.Add(hint);
            }

            // ── Asset References ─────────────────────────────────────────────────
            card.Add(FieldLbl("References  (drag asset / prefab / scene object)"));
            var refsBox = new VisualElement();
            refsBox.style.marginBottom = 4;
            card.Add(refsBox);

            void RefreshRefs()
            {
                refsBox.Clear();
                if (task.editRefs == null || task.editRefs.Count == 0)
                {
                    refsBox.Add(MkLbl("No references yet.", 10, t.faint));
                    return;
                }
                foreach (var ar in task.editRefs.ToList())
                {
                    var row = Row(Color.clear, 22);
                    row.style.marginBottom = 3;
                    row.Add(
                        RefChip(
                            ar,
                            t,
                            true,
                            () =>
                            {
                                task.editRefs.Remove(ar);
                                rootVisualElement.schedule.Execute(() => RefreshRefs());
                            }
                        )
                    );
                    var pingB = SmallBtn(
                        "⌖",
                        () =>
                        {
                            var o = ar.Resolve();
                            if (o != null)
                                EditorGUIUtility.PingObject(o);
                        }
                    );
                    pingB.style.marginLeft = 4;
                    row.Add(pingB);
                    refsBox.Add(row);
                }
            }
            RefreshRefs();

            var objF = new ObjectField();
            objF.objectType = typeof(UnityEngine.Object);
            objF.allowSceneObjects = true;
            objF.style.marginBottom = 6;
            objF.RegisterValueChangedCallback(evt =>
            {
                var o = evt.newValue;
                if (o == null)
                    return;
                var ar = AssetRef.From(o);
                if (ar == null)
                    return;
                task.editRefs ??= new List<AssetRef>();
                if (!task.editRefs.Any(r => r.globalId == ar.globalId))
                {
                    task.editRefs.Add(ar);
                    rootVisualElement.schedule.Execute(() => RefreshRefs());
                }
                // BUG FIX: SetValueWithoutNotify prevents re-firing callback on reset
                objF.SetValueWithoutNotify(null);
            });
            card.Add(objF);

            // ── Action buttons ───────────────────────────────────────────────────
            var btnRow = Row(Color.clear, 0);
            btnRow.style.marginTop = 4;

            btnRow.Add(
                MkBtn(
                    "💾 Save",
                    () =>
                    {
                        if (string.IsNullOrWhiteSpace(task.editTitle))
                            return;
                        task.title = task.editTitle.Trim();
                        task.description = task.editDesc?.Trim() ?? "";
                        task.tagsRaw = KanbanTask.JoinTags(task.editTags ?? "");
                        task.priority = task.editPriority;
                        task.assetRefs = task.editRefs ?? new List<AssetRef>();
                        // Merge editProps back — only keep non-empty assignments
                        task.props =
                            task.editProps?.Where(p => p.selected != null && p.selected.Count > 0)
                                .ToList()
                            ?? new List<PropAssignment>();
                        task.editing = false;
                        Save();
                        RebuildColumns();
                    },
                    grow: true,
                    height: 24,
                    marginR: 4
                )
            );

            btnRow.Add(
                MkBtn(
                    "✖ Cancel",
                    () =>
                    {
                        task.editing = false;
                        RebuildColumns();
                    },
                    grow: true,
                    height: 24,
                    marginR: 4
                )
            );

            var delBtn = MkBtn(
                "🗑",
                () =>
                {
                    if (
                        EditorUtility.DisplayDialog(
                            "Delete",
                            $"Delete \"{task.title}\"?",
                            "Delete",
                            "Cancel"
                        )
                    )
                    {
                        task.editing = false;
                        col.tasks.Remove(task);
                        Save();
                        RebuildColumns();
                    }
                },
                width: 28,
                height: 24
            );
            delBtn.style.backgroundColor = t.btnDanBg;
            delBtn.style.color = Color.white;
            btnRow.Add(delBtn);

            card.Add(btnRow);
        }

        // ── Property picker (in card edit mode) ───────────────────────────────────
        // Builds inline with pill toggles. When a pill is clicked, its style is updated
        // in-place — no card rebuild, so TextFields above stay focused and intact.
        VisualElement BuildEditPropPicker(KanbanTask task, PropDef pd)
        {
            var t = Th;
            var (pbg, pfg) = PropColor(pd.id, EditorGUIUtility.isProSkin);

            var outer = new VisualElement();
            outer.style.marginBottom = 6;
            outer.style.backgroundColor = new Color(pbg.r, pbg.g, pbg.b, 0.18f);
            outer.style.Radius(5);
            outer.style.PadTBLR(5, 8);

            // Header: prop key + clear button
            var hdr = Row(Color.clear, 20);
            hdr.style.marginBottom = 4;

            var dot = new VisualElement();
            dot.style.width = 8;
            dot.style.height = 8;
            dot.style.Radius(4);
            dot.style.backgroundColor = pfg;
            dot.style.marginRight = 5;
            dot.style.alignSelf = Align.Center;
            hdr.Add(dot);

            hdr.Add(MkLbl(pd.key, 11, pfg, bold: true));
            if (pd.multiSelect)
            {
                var ml = MkLbl("  (multi)", 10, new Color(pfg.r, pfg.g, pfg.b, 0.6f));
                hdr.Add(ml);
            }
            hdr.Add(Flex());

            var pa = task.EditPropFor(pd.id);

            var clearBtn = SmallBtn(
                "× clear",
                () =>
                {
                    pa.selected.Clear();
                    // Rebuild just the columns to update display, TextFields stay fine
                    rootVisualElement.schedule.Execute(RebuildColumns);
                }
            );
            clearBtn.style.fontSize = 9;
            hdr.Add(clearBtn);
            outer.Add(hdr);

            // Pills row — updated in-place on click
            if (pd.values.Count == 0)
            {
                outer.Add(MkLbl("No values defined. Edit in ⚙ Settings.", 10, t.faint));
                return outer;
            }

            var pillRow = Row(Color.clear, 0);
            pillRow.style.flexWrap = Wrap.Wrap;
            outer.Add(pillRow);

            // Build all pills first, then register click handlers that update styles in-place
            var pillsAndVals = new List<(VisualElement pill, string val)>();

            Color activeBg = pfg;
            Color activeFg = Color.white;
            Color inactBg = new Color(pbg.r, pbg.g, pbg.b, 0.28f);
            Color inactFg = new Color(pfg.r, pfg.g, pfg.b, 0.55f);

            void StylePill(VisualElement pill, bool active)
            {
                pill.style.backgroundColor = active ? activeBg : inactBg;
                // Find the label child
                if (pill.childCount > 0 && pill[0] is Label lbl)
                    lbl.style.color = active ? activeFg : inactFg;
                pill.style.borderTopColor =
                    pill.style.borderRightColor =
                    pill.style.borderBottomColor =
                    pill.style.borderLeftColor =
                        active ? activeBg : new Color(pfg.r, pfg.g, pfg.b, 0.3f);
            }

            foreach (var val in pd.values)
            {
                bool active = pa.Has(val);
                var pill = new VisualElement();
                pill.style.flexDirection = FlexDirection.Row;
                pill.style.alignItems = Align.Center;
                pill.style.Radius(12);
                pill.style.PadTBLR(3, 8);
                pill.style.marginRight = 5;
                pill.style.marginBottom = 3;
                pill.style.BorderW(1);

                var lbl = new Label(val);
                lbl.style.fontSize = 11;
                pill.Add(lbl);

                StylePill(pill, active);
                pillsAndVals.Add((pill, val));
                pillRow.Add(pill);
            }

            // Wire click handlers after all pills exist (so pillsAndVals is complete)
            foreach (var (pill, val) in pillsAndVals)
            {
                string capturedVal = val;
                VisualElement capturedPill = pill;
                pill.RegisterCallback<MouseDownEvent>(evt =>
                {
                    pa.Toggle(capturedVal, pd.multiSelect);

                    // In-place style update — no card rebuild, TextFields unaffected
                    foreach (var (p, v) in pillsAndVals)
                        StylePill(p, pa.Has(v));

                    evt.StopPropagation();
                });
            }

            return outer;
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  CARD INTERACTION  (drag, double-click, context menu)
        // ─────────────────────────────────────────────────────────────────────────
        void RegisterCardInteraction(VisualElement card, KanbanTask task, KanbanColumn col)
        {
            if (task.editing)
                return;

            Vector2 downPos = default;
            bool armed = false;
            EventCallback<MouseMoveEvent> rootMove = null;
            EventCallback<MouseUpEvent> rootUp = null;

            card.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button != 0)
                    return;

                if (evt.clickCount == 2)
                {
                    armed = false;
                    BeginEdit(task);
                    rootVisualElement.schedule.Execute(RebuildColumns);
                    evt.StopPropagation();
                    return;
                }

                downPos = evt.mousePosition;
                armed = true;

                // BUG FIX: register on root so fast mouse-exit doesn't lose the drag
                rootMove = mv =>
                {
                    if (!armed)
                        return;
                    if (Vector2.Distance(mv.mousePosition, downPos) < 5f)
                        return;
                    armed = false;
                    rootVisualElement.UnregisterCallback(rootMove);
                    rootVisualElement.UnregisterCallback(rootUp);

                    DragAndDrop.PrepareStartDrag();
                    // BUG FIX: zero objectReferences so Unity doesn't ghost-drag an asset
                    DragAndDrop.objectReferences = Array.Empty<UnityEngine.Object>();
                    DragAndDrop.SetGenericData(DRAG_ID, new Drag { task = task, srcCol = col });
                    DragAndDrop.StartDrag(task.title);
                    mv.StopPropagation();
                };
                rootUp = _ =>
                {
                    armed = false;
                    rootVisualElement.UnregisterCallback(rootMove);
                    rootVisualElement.UnregisterCallback(rootUp);
                };
                rootVisualElement.RegisterCallback(rootMove);
                rootVisualElement.RegisterCallback(rootUp);
            });

            // BUG FIX: StopPropagation so context click doesn't bubble to column/root
            card.RegisterCallback<ContextClickEvent>(evt =>
            {
                armed = false;
                var menu = new GenericMenu();
                int ci = data.columns.IndexOf(col);

                menu.AddItem(
                    new GUIContent("✏  Edit"),
                    false,
                    () =>
                    {
                        BeginEdit(task);
                        rootVisualElement.schedule.Execute(RebuildColumns);
                    }
                );
                menu.AddSeparator("");

                foreach (var c in data.columns)
                {
                    var destCol = c; // capture
                    menu.AddItem(
                        new GUIContent($"Move → {c.name}"),
                        c == col,
                        () => DoMove(task, col, destCol)
                    );
                }
                menu.AddSeparator("");
                menu.AddItem(
                    new GUIContent("🗑  Delete"),
                    false,
                    () =>
                    {
                        if (
                            EditorUtility.DisplayDialog(
                                "Delete",
                                $"Delete \"{task.title}\"?",
                                "Delete",
                                "Cancel"
                            )
                        )
                        {
                            col.tasks.Remove(task);
                            Save();
                            RebuildColumns();
                        }
                    }
                );
                menu.ShowAsContext();
                evt.StopPropagation();
            });
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  DRAG & DROP  (root-level — no false DragLeaveEvent fires)
        // ─────────────────────────────────────────────────────────────────────────
        void OnDragUpdated(DragUpdatedEvent evt)
        {
            if (DragAndDrop.GetGenericData(DRAG_ID) is not Drag)
                return;
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
            var mp = evt.mousePosition;
            for (int i = 0; i < colElements.Count; i++)
                colElements[i].el.style.backgroundColor = colElements[i].el.worldBound.Contains(mp)
                    ? Th.colDrop
                    : Th.colBg;
        }

        void OnDragPerform(DragPerformEvent evt)
        {
            ClearDropHighlights();
            if (DragAndDrop.GetGenericData(DRAG_ID) is not Drag dd)
                return;
            if (dd.srcCol == null || dd.task == null)
                return;

            var mp = evt.mousePosition;
            KanbanColumn target = null;
            foreach (var (el, col) in colElements)
                if (el.worldBound.Contains(mp))
                {
                    target = col;
                    break;
                }

            if (target == null || target == dd.srcCol)
                return;

            DragAndDrop.AcceptDrag();
            dd.srcCol.tasks.Remove(dd.task);
            target.tasks.Add(dd.task);
            Save();
            RebuildColumns();
        }

        void ClearDropHighlights()
        {
            foreach (var (el, _) in colElements)
                el.style.backgroundColor = Th.colBg;
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  DATA OPERATIONS
        // ─────────────────────────────────────────────────────────────────────────
        void BeginEdit(KanbanTask task)
        {
            task.editTitle = task.title;
            task.editDesc = task.description;
            task.editTags = task.tagsRaw;
            task.editPriority = task.priority;
            task.editRefs = task.assetRefs?.Select(r => r.Clone()).ToList() ?? new List<AssetRef>();
            task.editProps =
                task.props?.Select(p => p.Clone()).ToList() ?? new List<PropAssignment>();
            task.editing = true;
        }

        void DoMove(KanbanTask task, KanbanColumn from, KanbanColumn to)
        {
            if (from == to)
                return;
            from.tasks.Remove(task);
            to.tasks.Add(task);
            Save();
            RebuildColumns();
        }

        void SwapColumns(int a, int b)
        {
            var tmp = data.columns[a];
            data.columns[a] = data.columns[b];
            data.columns[b] = tmp;
            Save();
            RebuildColumns();
        }

        void CommitColumnRename(KanbanColumn col)
        {
            var name = col.editName?.Trim();
            if (string.IsNullOrEmpty(name))
                return;
            col.name = name;
            col.editing = false;
            Save();
            RebuildColumns();
            rootVisualElement.schedule.Execute(RebuildSettingsContent);
        }

        void CommitNewColumn()
        {
            var name = newColDraft?.Trim();
            if (string.IsNullOrEmpty(name))
                return;
            data.columns.Add(new KanbanColumn { id = Guid.NewGuid().ToString(), name = name });
            showNewColForm = false;
            newColDraft = "";
            Save();
            RebuildColumns();
            rootVisualElement.schedule.Execute(RebuildSettingsContent);
        }

        void DeleteColumn(KanbanColumn col, int ci)
        {
            if (data.columns.Count <= 1)
            {
                EditorUtility.DisplayDialog(
                    "Cannot Delete",
                    "The board must have at least one column.",
                    "OK"
                );
                return;
            }

            int tasks = col.tasks.Count;
            string msg =
                tasks > 0
                    ? $"Column '{col.name}' has {tasks} task(s). All tasks will be permanently deleted."
                    : $"Delete column '{col.name}'?";

            if (!EditorUtility.DisplayDialog("Delete Column", msg, "Delete", "Cancel"))
                return;
            data.columns.RemoveAt(ci);
            Save();
            RebuildColumns();
            rootVisualElement.schedule.Execute(RebuildSettingsContent);
        }

        void CommitNewProp(PropDef pd)
        {
            var key = pd.editKey?.Trim();
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("[PROTO Kanban] Property key is empty.");
                return;
            }
            pd.key = key;
            pd.values = pd
                .editVals.Select(v => v.Trim())
                .Where(v => v.Length > 0)
                .Distinct()
                .ToList();
            pd.multiSelect = pd.editMulti;
            pd.editing = false;
            data.propDefs.Add(pd);
            pendingNewProp = null;
            Save();
            RebuildColumns();
            rootVisualElement.schedule.Execute(RebuildSettingsContent);
        }

        void CommitEditProp(PropDef pd)
        {
            var key = pd.editKey?.Trim();
            if (string.IsNullOrEmpty(key))
                return;

            var newVals = pd
                .editVals.Select(v => v.Trim())
                .Where(v => v.Length > 0)
                .Distinct()
                .ToList();
            bool multiToSingle = !pd.editMulti && pd.multiSelect;

            // Validate task assignments: remove values no longer present,
            // truncate to 1 if switching from multi → single
            foreach (var col in data.columns)
            foreach (var task in col.tasks)
            {
                var pa = task.props?.FirstOrDefault(p => p.propId == pd.id);
                if (pa == null)
                    continue;
                pa.selected = pa.selected.Where(s => newVals.Contains(s)).ToList();
                if (multiToSingle && pa.selected.Count > 1)
                    pa.selected = pa.selected.Take(1).ToList();
            }

            pd.key = key;
            pd.values = newVals;
            pd.multiSelect = pd.editMulti;
            pd.editing = false;
            Save();
            RebuildColumns();
            rootVisualElement.schedule.Execute(RebuildSettingsContent);
        }

        void DeletePropDef(PropDef pd)
        {
            if (
                !EditorUtility.DisplayDialog(
                    "Delete Property",
                    $"Delete property '{pd.key}'? This will remove all assignments from tasks.",
                    "Delete",
                    "Cancel"
                )
            )
                return;

            // Cleanup task assignments
            foreach (var col in data.columns)
            foreach (var task in col.tasks)
                task.props?.RemoveAll(p => p.propId == pd.id);

            data.propDefs.Remove(pd);
            Save();
            RebuildColumns();
            rootVisualElement.schedule.Execute(RebuildSettingsContent);
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  VIEW  (filter + sort snapshot — never modifies source list)
        // ─────────────────────────────────────────────────────────────────────────
        List<KanbanTask> View(List<KanbanTask> src)
        {
            IEnumerable<KanbanTask> q = src;

            if (!string.IsNullOrEmpty(search))
                q = q.Where(x =>
                    CI(x.title, search)
                    || CI(x.description, search)
                    || x.Tags.Any(tag => CI(tag, search))
                    || (x.props?.Any(p => p.selected?.Any(s => CI(s, search)) ?? false) ?? false)
                    || (x.assetRefs?.Any(r => CI(r.label, search)) ?? false)
                );

            q = sortMode switch
            {
                1 => q.OrderBy(x => x.priority),
                2 => q.OrderByDescending(x => x.priority),
                3 => q.OrderByDescending(x => x.createdTicks),
                4 => q.OrderBy(x => x.createdTicks),
                5 => q.OrderBy(x => x.title, StringComparer.OrdinalIgnoreCase),
                _ => q,
            };

            return q.ToList();
        }

        static bool CI(string hay, string needle) =>
            !string.IsNullOrEmpty(hay)
            && hay.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;

        void RefreshProgress()
        {
            if (pbar == null)
                return;
            int total = data.columns.Sum(c => c.tasks.Count);
            int done =
                data.columns.Count > 0 ? data.columns[data.columns.Count - 1].tasks.Count : 0;
            float pct = total == 0 ? 0f : (float)done / total;
            pbar.value = pct * 100f;
            if (pbarLabel != null)
                pbarLabel.text = $"{done}/{total} done";
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  PERSISTENCE
        // ─────────────────────────────────────────────────────────────────────────
        void Load()
        {
            data = null;
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                try
                {
                    data = JsonUtility.FromJson<KanbanBoardData>(json);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[PROTO Kanban] Load failed: {ex.Message}");
                }

                // Migration: if columns is empty/null but we have legacy todo/progress/done
                if ((data == null || data.columns == null || data.columns.Count == 0))
                {
                    try
                    {
                        var leg = JsonUtility.FromJson<LegacyBoard>(json);
                        if (
                            leg != null
                            && (leg.todo.Count + leg.progress.Count + leg.done.Count) > 0
                        )
                        {
                            data ??= new KanbanBoardData();
                            data.columns = new List<KanbanColumn>
                            {
                                new KanbanColumn
                                {
                                    id = Guid.NewGuid().ToString(),
                                    name = "To Do",
                                    tasks = leg.todo ?? new List<KanbanTask>(),
                                },
                                new KanbanColumn
                                {
                                    id = Guid.NewGuid().ToString(),
                                    name = "In Progress",
                                    tasks = leg.progress ?? new List<KanbanTask>(),
                                },
                                new KanbanColumn
                                {
                                    id = Guid.NewGuid().ToString(),
                                    name = "Done",
                                    tasks = leg.done ?? new List<KanbanTask>(),
                                },
                            };
                            Debug.Log(
                                "[PROTO Kanban] Migrated legacy save file to new column format."
                            );
                        }
                    }
                    catch
                    { /* no legacy data */
                    }
                }
            }

            data ??= new KanbanBoardData();

            // Default columns if brand new
            if (data.columns == null || data.columns.Count == 0)
                data.columns = new List<KanbanColumn>
                {
                    new KanbanColumn { id = Guid.NewGuid().ToString(), name = "To Do" },
                    new KanbanColumn { id = Guid.NewGuid().ToString(), name = "In Progress" },
                    new KanbanColumn { id = Guid.NewGuid().ToString(), name = "Done" },
                };

            data.propDefs ??= new List<PropDef>();

            // Null-guard all task lists after load (BUG FIX from v3)
            foreach (var col in data.columns)
            {
                col.tasks ??= new List<KanbanTask>();
                foreach (var task in col.tasks)
                {
                    task.assetRefs ??= new List<AssetRef>();
                    task.props ??= new List<PropAssignment>();
                    foreach (var pa in task.props)
                        pa.selected ??= new List<string>();
                }
            }
            foreach (var pd in data.propDefs)
                pd.values ??= new List<string>();
        }

        void Save()
        {
            if (data == null)
                return;
            try
            {
                string json = JsonUtility.ToJson(data, prettyPrint: true);
                string tmp = savePath + ".tmp";
                File.WriteAllText(tmp, json);
                // Atomic replace — never leaves a corrupt file on crash
                if (File.Exists(savePath))
                    File.Replace(tmp, savePath, null);
                else
                    File.Move(tmp, savePath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[PROTO Kanban] Save failed: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  MARKDOWN EXPORT
        // ─────────────────────────────────────────────────────────────────────────
        void ExportMarkdown()
        {
            string path = EditorUtility.SaveFilePanel("Export Kanban", "", "kanban.md", "md");
            if (string.IsNullOrEmpty(path))
                return;

            var sb = new StringBuilder();
            sb.AppendLine("# PROTO Kanban Board");
            sb.AppendLine($"*Exported {DateTime.Now:yyyy-MM-dd HH:mm}*\n");

            string[] priIcons = { "🟢 Low", "🟡 Med", "🔴 High" };

            foreach (var col in data.columns)
            {
                sb.AppendLine($"## {col.name} ({col.tasks.Count})");
                if (col.tasks.Count == 0)
                {
                    sb.AppendLine("*No tasks.*\n");
                    continue;
                }
                foreach (var x in col.tasks)
                {
                    string tags = x.Tags.Any() ? $" · `{string.Join("` `", x.Tags)}`" : "";
                    sb.AppendLine($"- **{x.title}** [{priIcons[x.priority]}]{tags}");
                    if (!string.IsNullOrEmpty(x.description))
                        sb.AppendLine($"  > {x.description}");
                    foreach (var pa in x.props ?? Enumerable.Empty<PropAssignment>())
                    {
                        var pd = data.propDefs.FirstOrDefault(d => d.id == pa.propId);
                        if (pd != null && pa.selected?.Count > 0)
                            sb.AppendLine($"  - {pd.key}: {string.Join(", ", pa.selected)}");
                    }
                    if (x.assetRefs?.Count > 0)
                        sb.AppendLine(
                            $"  - Refs: {string.Join(", ", x.assetRefs.Select(r => $"{r.label} ({r.typeName})"))}"
                        );
                }
                sb.AppendLine();
            }

            File.WriteAllText(path, sb.ToString());
            EditorUtility.RevealInFinder(path);
            Debug.Log($"[PROTO Kanban] Exported → {path}");
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  UI FACTORY HELPERS
        // ─────────────────────────────────────────────────────────────────────────
        static VisualElement Row(Color bg, float h)
        {
            var v = new VisualElement();
            v.style.flexDirection = FlexDirection.Row;
            v.style.alignItems = Align.Center;
            if (h > 0)
                v.style.height = h;
            if (bg != Color.clear)
                v.style.backgroundColor = bg;
            return v;
        }

        static Label MkLbl(string text, int size, Color col, bool bold = false)
        {
            var l = new Label(text);
            l.style.fontSize = size;
            l.style.color = col;
            if (bold)
                l.style.unityFontStyleAndWeight = FontStyle.Bold;
            return l;
        }

        Label FieldLbl(string text)
        {
            var l = new Label(text);
            l.style.fontSize = 10;
            l.style.color = Th.faint;
            l.style.marginBottom = 2;
            return l;
        }

        Label BLbl(string text)
        {
            var l = new Label(text);
            l.style.fontSize = 11;
            l.style.color = Th.dim;
            l.style.marginRight = 4;
            return l;
        }

        static TextField AddField(int width, string val, bool grow = false)
        {
            var tf = new TextField();
            tf.SetValueWithoutNotify(val);
            if (grow)
                tf.style.flexGrow = 1;
            else
                tf.style.width = width;
            tf.style.marginRight = 6;
            return tf;
        }

        static DropdownField MkDropdown(List<string> choices, int idx, int width)
        {
            var d = new DropdownField();
            d.choices = choices.Count > 0 ? choices : new List<string> { "(none)" };
            // BUG FIX: set choices before index, use SetValueWithoutNotify to avoid spurious callbacks
            d.SetValueWithoutNotify(d.choices[Mathf.Clamp(idx, 0, d.choices.Count - 1)]);
            d.style.width = width;
            d.style.marginRight = 6;
            return d;
        }

        static Button MkBtn(
            string text,
            Action clicked,
            float width = -1,
            float height = -1,
            float marginR = 0,
            float marginT = 0,
            bool grow = false
        )
        {
            var b = new Button(clicked) { text = text };
            if (width > 0)
                b.style.width = width;
            if (height > 0)
                b.style.height = height;
            if (marginR > 0)
                b.style.marginRight = marginR;
            if (marginT > 0)
                b.style.marginTop = marginT;
            if (grow)
                b.style.flexGrow = 1;
            return b;
        }

        static Button TBtn(string text, Action clicked, float marginR = 0)
        {
            var b = new Button(clicked) { text = text };
            b.style.height = 22;
            b.style.marginRight = marginR;
            return b;
        }

        static Button SmallBtn(string text, Action clicked)
        {
            var b = new Button(clicked) { text = text };
            b.style.width = 24;
            b.style.height = 22;
            b.style.fontSize = 11;
            b.style.PadTBLR(0, 2);
            return b;
        }

        static Button MiniBtn(string text, Action clicked)
        {
            var b = new Button(clicked) { text = text };
            b.style.width = 22;
            b.style.height = 20;
            b.style.PadTBLR(0, 2);
            b.style.marginLeft = 2;
            b.style.fontSize = 11;
            return b;
        }

        static VisualElement Chip(string text, Color bg, Color fg)
        {
            var c = new VisualElement();
            c.style.flexDirection = FlexDirection.Row;
            c.style.alignItems = Align.Center;
            c.style.backgroundColor = bg;
            c.style.Radius(10);
            c.style.PadTBLR(2, 6);
            c.style.marginRight = 3;
            c.style.marginBottom = 2;
            c.Add(new Label(text) { style = { fontSize = 10, color = fg } });
            return c;
        }

        static VisualElement ChipRow()
        {
            var r = new VisualElement();
            r.style.flexDirection = FlexDirection.Row;
            r.style.flexWrap = Wrap.Wrap;
            r.style.marginBottom = 3;
            return r;
        }

        static VisualElement RefChip(AssetRef ar, Palette t, bool editMode, Action onRemove)
        {
            string icon = ar.typeName switch
            {
                "Prefab" => "⬡",
                "GameObject" => "◈",
                "Scene" => "⊞",
                "Material" => "◎",
                "AudioClip" => "♪",
                "Sprite" => "◉",
                _ => "◆",
            };
            var chip = new VisualElement();
            chip.style.flexDirection = FlexDirection.Row;
            chip.style.alignItems = Align.Center;
            chip.style.backgroundColor = t.refBg;
            chip.style.Radius(4);
            chip.style.PadTBLR(2, 5);
            chip.style.marginRight = 4;
            chip.style.marginBottom = 3;
            chip.Add(
                new Label($"{icon} {ar.label}") { style = { fontSize = 10, color = t.refFg } }
            );
            chip.Add(
                new Label($" ({ar.typeName})")
                {
                    style =
                    {
                        fontSize = 9,
                        color = new Color(t.refFg.r, t.refFg.g, t.refFg.b, 0.55f),
                    },
                }
            );
            if (editMode && onRemove != null)
            {
                var x = new Button(onRemove) { text = "×" };
                x.style.width = 14;
                x.style.height = 14;
                x.style.marginLeft = 3;
                x.style.backgroundColor = t.btnDanBg;
                x.style.color = Color.white;
                x.style.Radius(3);
                x.style.BorderW(0);
                x.style.PadAll(0);
                chip.Add(x);
            }
            return chip;
        }

        static VisualElement Flex()
        {
            var v = new VisualElement();
            v.style.flexGrow = 1;
            return v;
        }

        VisualElement SectionHeader(string title, Palette t)
        {
            var row = new VisualElement();
            row.style.marginTop = 12;
            row.style.marginBottom = 4;
            row.style.marginLeft = row.style.marginRight = 6;
            row.style.borderBottomWidth = 1;
            row.style.borderBottomColor = t.border2;
            row.style.paddingBottom = 4;

            var lbl = MkLbl(title, 10, t.accent, bold: true);
            lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
            row.Add(lbl);
            return row;
        }

        static VisualElement Divider(Palette t)
        {
            var v = new VisualElement();
            v.style.height = 1;
            v.style.backgroundColor = t.border;
            v.style.marginTop = 12;
            v.style.marginBottom = 2;
            return v;
        }
    }
}
#endif
