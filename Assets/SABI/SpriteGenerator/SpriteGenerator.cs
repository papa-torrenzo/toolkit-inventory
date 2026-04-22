#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SABI
{
    public class SpriteGenerator : EditorWindow
    {
        // UI Constants
        private const int MIN_DIMENSION = 1;
        private const int MAX_DIMENSION = 2048;
        private const int DEFAULT_DIMENSION = 256;
        private const float DEFAULT_BORDER_RADIUS = 32f;
        private const string DEFAULT_IMAGE_NAME = "GeneratedSprite";

        // UI State
        private bool showPreview = true;

        private bool uniformSize = false;
        private int size = DEFAULT_DIMENSION;
        private int width = DEFAULT_DIMENSION;
        private int height = DEFAULT_DIMENSION;

        private bool enableBorderRadius = true;
        private bool uniformRadius = true;
        private float borderRadiusTopLeft = DEFAULT_BORDER_RADIUS;
        private float borderRadiusTopRight = DEFAULT_BORDER_RADIUS;
        private float borderRadiusBottomRight = DEFAULT_BORDER_RADIUS;
        private float borderRadiusBottomLeft = DEFAULT_BORDER_RADIUS;
        private float uniformBorderRadius = DEFAULT_BORDER_RADIUS;

        private bool enableFill = true;
        private Color fillColor = Color.white;

        private bool enableBorder = true;
        private Color borderColor = Color.black;
        private int borderWidth = 12;

        private bool antiAlias = true;
        private bool auto9Slice = true;

        // Visual Elements
        private ScrollView leftPanel;
        private VisualElement rightPanel;
        private Image previewImage;
        private Texture2D previewTexture;
        private Texture2D checkerboardTexture;

        [MenuItem("Tools/Sabi/SpriteGenerator")]
        private static void ShowWindow()
        {
            var window = GetWindow<SpriteGenerator>();
            window.titleContent = new GUIContent("Sprite Generator");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }

        private void OnEnable()
        {
            checkerboardTexture = CreateCheckerboardTexture();
        }

        private void OnDisable()
        {
            // Prevent editor memory leaks when the window is closed or code recompiles
            if (previewTexture != null)
                DestroyImmediate(previewTexture);
            if (checkerboardTexture != null)
                DestroyImmediate(checkerboardTexture);
            if (previewImage != null)
                previewImage.image = null;
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Row;
            root.style.flexGrow = 1;

            // --- LEFT PANEL (CONTROLS) ---
            leftPanel = new ScrollView();
            leftPanel.style.width = showPreview ? 350 : Length.Percent(100);
            leftPanel.style.minWidth = 350;
            leftPanel.style.flexShrink = 0;
            leftPanel.style.paddingLeft = 10;
            leftPanel.style.paddingRight = 10;
            leftPanel.style.paddingTop = 10;
            leftPanel.style.paddingBottom = 10;
            leftPanel.style.borderRightWidth = 1;
            leftPanel.style.borderRightColor = new Color(0.15f, 0.15f, 0.15f, 1f);
            leftPanel.style.backgroundColor = new Color(0.22f, 0.22f, 0.22f, 1f);

            leftPanel.Add(CreateHeaderSection());
            leftPanel.Add(CreateDimensionsSection());
            leftPanel.Add(CreateRadiusSection());
            leftPanel.Add(CreateFillSection());
            leftPanel.Add(CreateBorderSection());
            leftPanel.Add(CreateExportSection());

            root.Add(leftPanel);

            // --- RIGHT PANEL (PREVIEW) ---
            rightPanel = new VisualElement();
            rightPanel.style.flexGrow = 1;
            rightPanel.style.display = showPreview ? DisplayStyle.Flex : DisplayStyle.None;
            rightPanel.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
            rightPanel.style.alignItems = Align.Center;
            rightPanel.style.justifyContent = Justify.Center;
            rightPanel.style.backgroundImage = checkerboardTexture;
            rightPanel.style.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;

            previewImage = new Image();
            previewImage.style.maxWidth = Length.Percent(90);
            previewImage.style.maxHeight = Length.Percent(90);
            previewImage.scaleMode = ScaleMode.ScaleToFit;

            previewImage.style.borderTopWidth = 1;
            previewImage.style.borderBottomWidth = 1;
            previewImage.style.borderLeftWidth = 1;
            previewImage.style.borderRightWidth = 1;
            previewImage.style.borderTopColor = new Color(0, 0, 0, 0.5f);
            previewImage.style.borderBottomColor = new Color(0, 0, 0, 0.5f);
            previewImage.style.borderLeftColor = new Color(0, 0, 0, 0.5f);
            previewImage.style.borderRightColor = new Color(0, 0, 0, 0.5f);

            rightPanel.Add(previewImage);
            root.Add(rightPanel);

            UpdatePreview();
        }

        private VisualElement CreateHeaderSection()
        {
            var header = new VisualElement();
            header.style.marginBottom = 15;
            header.style.paddingBottom = 10;
            header.style.borderBottomWidth = 1;
            header.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

            var titleRow = new VisualElement();
            titleRow.style.flexDirection = FlexDirection.Row;
            titleRow.style.justifyContent = Justify.SpaceBetween;
            titleRow.style.alignItems = Align.Center;

            var title = new Label("🎨 Sprite Generator");
            title.style.fontSize = 18;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.color = new Color(0.9f, 0.9f, 0.9f);

            var previewToggle = new Button(() =>
            {
                showPreview = !showPreview;
                rightPanel.style.display = showPreview ? DisplayStyle.Flex : DisplayStyle.None;
                leftPanel.style.width = showPreview ? 350 : Length.Percent(100);
            })
            {
                text = "👁️ Toggle Preview",
            };
            previewToggle.style.height = 25;
            previewToggle.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f);
            previewToggle.style.color = Color.white;

            titleRow.Add(title);
            titleRow.Add(previewToggle);
            header.Add(titleRow);

            var subtitle = new Label("Perfect Signed Distance Field (SDF) generation");
            subtitle.style.fontSize = 11;
            subtitle.style.color = new Color(0.6f, 0.6f, 0.6f);
            subtitle.style.marginTop = 5;

            header.Add(subtitle);
            return header;
        }

        private VisualElement CreateDimensionsSection()
        {
            var section = CreateSectionContainer("Dimensions");

            section.Add(
                CreateToggleField(
                    "Uniform Size (Square)",
                    uniformSize,
                    v =>
                    {
                        uniformSize = v;
                        RefreshUI();
                    }
                )
            );

            if (uniformSize)
            {
                section.Add(
                    CreateIntegerField(
                        "Size",
                        size,
                        v =>
                        {
                            size = Mathf.Clamp(v, MIN_DIMENSION, MAX_DIMENSION);
                            width = size;
                            height = size;
                        }
                    )
                );
            }
            else
            {
                section.Add(
                    CreateIntegerField(
                        "Width",
                        width,
                        v =>
                        {
                            width = Mathf.Clamp(v, MIN_DIMENSION, MAX_DIMENSION);
                        }
                    )
                );
                section.Add(
                    CreateIntegerField(
                        "Height",
                        height,
                        v =>
                        {
                            height = Mathf.Clamp(v, MIN_DIMENSION, MAX_DIMENSION);
                        }
                    )
                );
            }

            return section;
        }

        private VisualElement CreateRadiusSection()
        {
            var section = CreateSectionContainer("Corner Radii");

            section.Add(
                CreateToggleField(
                    "Enable Corner Radii",
                    enableBorderRadius,
                    v =>
                    {
                        enableBorderRadius = v;
                        RefreshUI();
                    }
                )
            );

            if (enableBorderRadius)
            {
                var infoLabel = new Label(
                    "Radii are automatically clamped to prevent shape breaking."
                );
                infoLabel.style.fontSize = 10;
                infoLabel.style.color = new Color(0.7f, 0.7f, 0.3f);
                infoLabel.style.marginBottom = 10;
                infoLabel.style.marginTop = 5;
                section.Add(infoLabel);

                section.Add(
                    CreateToggleField(
                        "Uniform Radius",
                        uniformRadius,
                        v =>
                        {
                            uniformRadius = v;
                            RefreshUI();
                        }
                    )
                );

                if (uniformRadius)
                {
                    section.Add(
                        CreateFloatField(
                            "Border Radius",
                            uniformBorderRadius,
                            v => uniformBorderRadius = Mathf.Max(0, v)
                        )
                    );
                }
                else
                {
                    section.Add(
                        CreateFloatField(
                            "Top Left",
                            borderRadiusTopLeft,
                            v => borderRadiusTopLeft = Mathf.Max(0, v)
                        )
                    );
                    section.Add(
                        CreateFloatField(
                            "Top Right",
                            borderRadiusTopRight,
                            v => borderRadiusTopRight = Mathf.Max(0, v)
                        )
                    );
                    section.Add(
                        CreateFloatField(
                            "Bottom Left",
                            borderRadiusBottomLeft,
                            v => borderRadiusBottomLeft = Mathf.Max(0, v)
                        )
                    );
                    section.Add(
                        CreateFloatField(
                            "Bottom Right",
                            borderRadiusBottomRight,
                            v => borderRadiusBottomRight = Mathf.Max(0, v)
                        )
                    );
                }
            }

            return section;
        }

        private VisualElement CreateFillSection()
        {
            var section = CreateSectionContainer("Fill");
            section.Add(
                CreateToggleField(
                    "Enable Fill",
                    enableFill,
                    v =>
                    {
                        enableFill = v;
                        RefreshUI();
                    }
                )
            );
            if (enableFill)
            {
                section.Add(CreateColorField("Fill Color", fillColor, v => fillColor = v));
            }
            return section;
        }

        private VisualElement CreateBorderSection()
        {
            var section = CreateSectionContainer("Border");
            section.Add(
                CreateToggleField(
                    "Enable Border",
                    enableBorder,
                    v =>
                    {
                        enableBorder = v;
                        RefreshUI();
                    }
                )
            );
            if (enableBorder)
            {
                section.Add(
                    CreateIntegerField(
                        "Border Width",
                        borderWidth,
                        v => borderWidth = Mathf.Max(0, v)
                    )
                );
                section.Add(CreateColorField("Border Color", borderColor, v => borderColor = v));
            }
            return section;
        }

        private VisualElement CreateExportSection()
        {
            var section = CreateSectionContainer("Export");

            section.Add(
                CreateToggleField("Smooth Edges (Anti-Aliasing)", antiAlias, v => antiAlias = v)
            );
            section.Add(
                CreateToggleField("Auto-Configure 9-Slice", auto9Slice, v => auto9Slice = v)
            );

            var generateButton = new Button(ExportSprite) { text = "💾 Save to Project" };
            generateButton.style.height = 40;
            generateButton.style.fontSize = 16;
            generateButton.style.marginTop = 10;
            generateButton.style.backgroundColor = new Color(0.2f, 0.6f, 1.0f);
            generateButton.style.color = Color.white;
            generateButton.style.borderTopLeftRadius = 5;
            generateButton.style.borderTopRightRadius = 5;
            generateButton.style.borderBottomLeftRadius = 5;
            generateButton.style.borderBottomRightRadius = 5;

            section.Add(generateButton);
            return section;
        }

        private VisualElement CreateSectionContainer(string title)
        {
            var container = new VisualElement();
            container.style.marginBottom = 15;
            container.style.paddingTop = 10;
            container.style.paddingBottom = 10;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;
            container.style.backgroundColor = new Color(0.18f, 0.18f, 0.18f, 1f);
            container.style.borderTopLeftRadius = 6;
            container.style.borderTopRightRadius = 6;
            container.style.borderBottomLeftRadius = 6;
            container.style.borderBottomRightRadius = 6;

            var titleLabel = new Label(title);
            titleLabel.style.fontSize = 13;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.color = new Color(0.8f, 0.8f, 0.8f);
            titleLabel.style.marginBottom = 10;
            titleLabel.style.borderBottomWidth = 1;
            titleLabel.style.borderBottomColor = new Color(0.4f, 0.4f, 0.4f, 0.5f);
            titleLabel.style.paddingBottom = 5;

            container.Add(titleLabel);
            return container;
        }

        // --- HELPER CONTROLS ---

        private IntegerField CreateIntegerField(string label, int val, System.Action<int> onChanged)
        {
            var field = new IntegerField(label) { value = val };
            field.RegisterValueChangedCallback(evt =>
            {
                onChanged(evt.newValue);
                UpdatePreview();
            });
            field.style.marginBottom = 5;
            return field;
        }

        private FloatField CreateFloatField(string label, float val, System.Action<float> onChanged)
        {
            var field = new FloatField(label) { value = val };
            field.RegisterValueChangedCallback(evt =>
            {
                onChanged(evt.newValue);
                UpdatePreview();
            });
            field.style.marginBottom = 5;
            return field;
        }

        private ColorField CreateColorField(string label, Color val, System.Action<Color> onChanged)
        {
            var field = new ColorField(label) { value = val };
            field.RegisterValueChangedCallback(evt =>
            {
                onChanged(evt.newValue);
                UpdatePreview();
            });
            field.style.marginBottom = 5;
            return field;
        }

        private Toggle CreateToggleField(string label, bool val, System.Action<bool> onChanged)
        {
            var field = new Toggle(label) { value = val };
            field.RegisterValueChangedCallback(evt =>
            {
                onChanged(evt.newValue);
                UpdatePreview();
            });
            field.style.marginBottom = 5;
            return field;
        }

        private void RefreshUI()
        {
            rootVisualElement.Clear();
            CreateGUI();
        }

        private Texture2D CreateCheckerboardTexture()
        {
            var tex = new Texture2D(32, 32);
            tex.hideFlags = HideFlags.HideAndDontSave;
            Color c1 = new Color(0.18f, 0.18f, 0.18f, 1f);
            Color c2 = new Color(0.12f, 0.12f, 0.12f, 1f);
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    tex.SetPixel(x, y, ((x / 16) + (y / 16)) % 2 == 0 ? c1 : c2);
                }
            }
            tex.Apply();
            return tex;
        }

        // --- CORE LOGIC ---

        private void UpdatePreview()
        {
            int finalWidth = uniformSize ? size : width;
            int finalHeight = uniformSize ? size : height;

            if (
                previewTexture == null
                || previewTexture.width != finalWidth
                || previewTexture.height != finalHeight
            )
            {
                if (previewTexture != null)
                    DestroyImmediate(previewTexture);
                previewTexture = new Texture2D(
                    finalWidth,
                    finalHeight,
                    TextureFormat.RGBA32,
                    false
                );
            }

            if (previewImage != null)
            {
                previewImage.image = previewTexture;
            }

            GenerateSpriteData(previewTexture);
        }

        private void GenerateSpriteData(Texture2D targetTexture)
        {
            int w = targetTexture.width;
            int h = targetTexture.height;

            float maxSafeRadius = Mathf.Min(w, h) / 2f;

            float rTL = enableBorderRadius
                ? Mathf.Clamp(
                    uniformRadius ? uniformBorderRadius : borderRadiusTopLeft,
                    0,
                    maxSafeRadius
                )
                : 0f;
            float rTR = enableBorderRadius
                ? Mathf.Clamp(
                    uniformRadius ? uniformBorderRadius : borderRadiusTopRight,
                    0,
                    maxSafeRadius
                )
                : 0f;
            float rBL = enableBorderRadius
                ? Mathf.Clamp(
                    uniformRadius ? uniformBorderRadius : borderRadiusBottomLeft,
                    0,
                    maxSafeRadius
                )
                : 0f;
            float rBR = enableBorderRadius
                ? Mathf.Clamp(
                    uniformRadius ? uniformBorderRadius : borderRadiusBottomRight,
                    0,
                    maxSafeRadius
                )
                : 0f;

            Vector2 halfSize = new Vector2(w / 2f, h / 2f);
            Color[] pixels = new Color[w * h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Vector2 p = new Vector2(x - halfSize.x + 0.5f, y - halfSize.y + 0.5f);
                    float d = SDFBox(p, halfSize, rTL, rTR, rBL, rBR);

                    pixels[y * w + x] = CalculateFinalPixel(d);
                }
            }

            targetTexture.SetPixels(pixels);
            targetTexture.Apply();
        }

        private Color CalculateFinalPixel(float d)
        {
            if (!enableFill && !enableBorder)
                return Color.clear;

            if (antiAlias)
            {
                float outerAlpha = Mathf.Clamp01(0.5f - d);
                if (outerAlpha <= 0f)
                    return Color.clear;

                float innerAlpha = Mathf.Clamp01(0.5f - (d + borderWidth));
                Color finalColor = Color.clear;

                if (enableBorder && enableFill)
                {
                    finalColor = Color.Lerp(borderColor, fillColor, innerAlpha);
                    finalColor.a *= outerAlpha;
                }
                else if (enableBorder && !enableFill)
                {
                    finalColor = borderColor;
                    finalColor.a *= outerAlpha * (1f - innerAlpha);
                }
                else if (!enableBorder && enableFill)
                {
                    finalColor = fillColor;
                    finalColor.a *= outerAlpha;
                }

                return finalColor;
            }
            else
            {
                bool insideOuterBoundary = d <= 0f;
                bool insideInnerBoundary = d <= -borderWidth;

                if (!insideOuterBoundary)
                    return Color.clear;

                if (enableBorder && enableFill)
                    return insideInnerBoundary ? fillColor : borderColor;
                else if (enableBorder && !enableFill)
                    return insideInnerBoundary ? Color.clear : borderColor;
                else
                    return fillColor;
            }
        }

        private float SDFBox(Vector2 p, Vector2 b, float rTL, float rTR, float rBL, float rBR)
        {
            float r;
            if (p.x > 0f && p.y > 0f)
                r = rTR;
            else if (p.x > 0f && p.y <= 0f)
                r = rBR;
            else if (p.x <= 0f && p.y > 0f)
                r = rTL;
            else
                r = rBL;

            Vector2 q = new Vector2(Mathf.Abs(p.x), Mathf.Abs(p.y)) - b + new Vector2(r, r);
            return Mathf.Min(Mathf.Max(q.x, q.y), 0f) + Vector2.Max(q, Vector2.zero).magnitude - r;
        }

        private void ExportSprite()
        {
            // Opens Unity's native save panel inside the project
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Generated Sprite",
                DEFAULT_IMAGE_NAME,
                "png",
                "Please enter a file name to save the sprite to"
            );

            if (string.IsNullOrEmpty(path))
                return; // User canceled the save dialog

            try
            {
                byte[] imageData = previewTexture.EncodeToPNG();
                File.WriteAllBytes(path, imageData);

                // Force Unity to immediately recognize the new/updated file
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

                var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (textureImporter != null)
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                    textureImporter.spriteImportMode = SpriteImportMode.Single;
                    textureImporter.alphaIsTransparency = true;

                    if (auto9Slice)
                    {
                        float maxRadius = 0;
                        if (enableBorderRadius)
                        {
                            maxRadius = uniformRadius
                                ? uniformBorderRadius
                                : Mathf.Max(
                                    borderRadiusTopLeft,
                                    borderRadiusTopRight,
                                    borderRadiusBottomLeft,
                                    borderRadiusBottomRight
                                );
                        }

                        // Calculates the exact safe border needed for standard UI scaling
                        int sliceBorder =
                            Mathf.CeilToInt(Mathf.Max(maxRadius, enableBorder ? borderWidth : 0))
                            + 1;
                        textureImporter.spriteBorder = new Vector4(
                            sliceBorder,
                            sliceBorder,
                            sliceBorder,
                            sliceBorder
                        );
                    }

                    textureImporter.SaveAndReimport();
                }

                // EditorUtility.DisplayDialog(
                //     "Success",
                //     $"Sprite successfully saved to:\n{path}",
                //     "OK"
                // );
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog(
                    "Error",
                    $"Failed to export sprite:\n{ex.Message}",
                    "OK"
                );
            }
        }
    }
}
#endif
