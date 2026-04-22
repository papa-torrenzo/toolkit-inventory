namespace SABI
{
    using UnityEngine;
    using UnityEngine.UIElements;
    #if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
    #endif

    [System.Serializable]
    public abstract class RandomNumber<T>
        where T : struct
    {
        public enum RandomType
        {
            None,
            Deviation,
            MinMaxRange,
        }

        public RandomType randomType = RandomType.None;
        public T value;
        public float deviation = 0.1f;
        public T minValue;
        public T maxValue;

        protected abstract T GetRangeValue(T min, T max);
        protected abstract T ApplyDeviation(T input, float dev);

        public T GetValue()
        {
            switch (randomType)
            {
                case RandomType.None:
                    return value;
                case RandomType.Deviation:
                    value = ApplyDeviation(value, deviation);
                    return value;
                case RandomType.MinMaxRange:
                    value = GetRangeValue(minValue, maxValue);
                    return value;
            }

            return value;
        }
    }

    [System.Serializable]
    public class RandomInt : RandomNumber<int>
    {
        protected override int GetRangeValue(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        protected override int ApplyDeviation(int input, float dev)
        {
            float baseValue = input;
            float biased = baseValue.RandomBias(dev);
            return Mathf.RoundToInt(biased);
        }
    }

    [System.Serializable]
    public class RandomFloat : RandomNumber<float>
    {
        protected override float GetRangeValue(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        protected override float ApplyDeviation(float input, float dev)
        {
            return input.RandomBias(dev);
        }
    }

    [System.Serializable]
    public class RandomDouble : RandomNumber<double>
    {
        protected override double GetRangeValue(double min, double max)
        {
            float rangeMin = (float)min;
            float rangeMax = (float)max;
            return (double)UnityEngine.Random.Range(rangeMin, rangeMax);
        }

        protected override double ApplyDeviation(double input, float dev)
        {
            float floatInput = (float)input;
            float biased = floatInput.RandomBias(dev);
            return (double)biased;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(RandomInt))]
    public class RandomIntPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return CreatePropertyGUIInternal(property);
        }

        private static VisualElement CreatePropertyGUIInternal(SerializedProperty property)
        {
            var root = new VisualElement
            {
                style =
                {
                    backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 0.8f)),
                    color = new StyleColor(Color.white),
                    paddingLeft = 8,
                    paddingRight = 8,
                    paddingTop = 6,
                    paddingBottom = 6,
                    borderBottomLeftRadius = 4,
                    borderBottomRightRadius = 4,
                    borderTopLeftRadius = 4,
                    borderTopRightRadius = 4,
                    marginBottom = 5,
                    marginTop = 5,
                },
            };

            // Field Name Header
            var fieldNameLabel = new Label(property.displayName)
            {
                style =
                {
                    marginBottom = 4,
                    color = new StyleColor(new Color(0.8f, 0.8f, 0.8f, 1f)),
                    fontSize = 12,
                    unityFontStyleAndWeight = FontStyle.Bold,
                },
            };
            root.Add(fieldNameLabel);

            var randomTypeProp = property.FindPropertyRelative("randomType");
            var valueProp = property.FindPropertyRelative("value");
            var deviationProp = property.FindPropertyRelative("deviation");
            var minValueProp = property.FindPropertyRelative("minValue");
            var maxValueProp = property.FindPropertyRelative("maxValue");

            // Random Type Header
            var randomTypeField = new EnumField(
                "Random Type",
                (RandomInt.RandomType)randomTypeProp.enumValueIndex
            )
            {
                style = { marginBottom = 6, color = new StyleColor(Color.white) },
            };
            randomTypeField.BindProperty(randomTypeProp);
            root.Add(randomTypeField);

            // Value Field
            var valueField = new PropertyField(valueProp, "Value")
            {
                style = { marginBottom = 4, color = new StyleColor(Color.white) },
            };
            root.Add(valueField);

            // Deviation Field
            var deviationField = new PropertyField(deviationProp, "Deviation")
            {
                style = { marginBottom = 4, color = new StyleColor(Color.white) },
            };
            root.Add(deviationField);

            // min/max row with explicit label width and spacing
            var minMaxContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween,
                    alignItems = Align.FlexStart,
                    marginTop = 2,
                },
            };

            var minField = new IntegerField("Min")
            {
                style =
                {
                    flexGrow = 1,
                    minWidth = new Length(50, LengthUnit.Percent),
                    color = new StyleColor(Color.white),
                    marginRight = 4,
                },
            };
            minField.BindProperty(minValueProp);

            var maxField = new IntegerField("Max")
            {
                style =
                {
                    flexGrow = 1,
                    minWidth = new Length(50, LengthUnit.Percent),
                    color = new StyleColor(Color.white),
                    marginLeft = 4,
                },
            };
            maxField.BindProperty(maxValueProp);

            minMaxContainer.Add(minField);
            minMaxContainer.Add(maxField);
            root.Add(minMaxContainer);

            void UpdateFields()
            {
                var mode = (RandomInt.RandomType)randomTypeProp.enumValueIndex;
                valueField.style.display =
                    mode == RandomInt.RandomType.None || mode == RandomInt.RandomType.Deviation
                        ? DisplayStyle.Flex
                        : DisplayStyle.None;
                deviationField.style.display =
                    mode == RandomInt.RandomType.Deviation ? DisplayStyle.Flex : DisplayStyle.None;
                minMaxContainer.style.display =
                    mode == RandomInt.RandomType.MinMaxRange
                        ? DisplayStyle.Flex
                        : DisplayStyle.None;
            }

            property.serializedObject.Update();
            UpdateFields();
            randomTypeField.RegisterValueChangedCallback(evt =>
            {
                property.serializedObject.ApplyModifiedProperties();
                UpdateFields();
            });

            return root;
        }
    }

    [CustomPropertyDrawer(typeof(RandomFloat))]
    public class RandomFloatPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return RandomNumberDrawerHelper.CreatePropertyGUIShared(property);
        }
    }

    [CustomPropertyDrawer(typeof(RandomDouble))]
    public class RandomDoublePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return RandomNumberDrawerHelper.CreatePropertyGUIShared(property);
        }
    }

    public static class RandomNumberDrawerHelper
    {
        public static VisualElement CreatePropertyGUIShared(SerializedProperty property)
        {
            var root = new VisualElement
            {
                style =
                {
                    backgroundColor = new StyleColor(new Color(0.2f, 0.2f, 0.2f, 0.8f)),
                    color = new StyleColor(Color.white),
                    paddingLeft = 8,
                    paddingRight = 8,
                    paddingTop = 6,
                    paddingBottom = 6,
                    borderBottomLeftRadius = 4,
                    borderBottomRightRadius = 4,
                    borderTopLeftRadius = 4,
                    borderTopRightRadius = 4,
                    marginBottom = 5,
                    marginTop = 5,
                },
            };

            // Field Name Header
            var fieldNameLabel = new Label(property.displayName)
            {
                style =
                {
                    marginBottom = 4,
                    color = new StyleColor(new Color(0.8f, 0.8f, 0.8f, 1f)),
                    fontSize = 12,
                    unityFontStyleAndWeight = FontStyle.Bold,
                },
            };
            root.Add(fieldNameLabel);

            var randomTypeProp = property.FindPropertyRelative("randomType");
            var valueProp = property.FindPropertyRelative("value");
            var deviationProp = property.FindPropertyRelative("deviation");
            var minValueProp = property.FindPropertyRelative("minValue");
            var maxValueProp = property.FindPropertyRelative("maxValue");

            // Random Type Header - use generic enum name
            var randomTypeField = new EnumField(
                "Random Type",
                randomTypeProp.enumValueIndex == 0
                    ? (System.Enum)(object)0
                    : (
                        randomTypeProp.enumValueIndex == 1
                            ? (System.Enum)(object)1
                            : (System.Enum)(object)2
                    )
            )
            {
                style = { marginBottom = 6, color = new StyleColor(Color.white) },
            };
            randomTypeField.BindProperty(randomTypeProp);
            root.Add(randomTypeField);

            // Value Field
            var valueField = new PropertyField(valueProp, "Value")
            {
                style = { marginBottom = 4, color = new StyleColor(Color.white) },
            };
            root.Add(valueField);

            // Deviation Field
            var deviationField = new PropertyField(deviationProp, "Deviation")
            {
                style = { marginBottom = 4, color = new StyleColor(Color.white) },
            };
            root.Add(deviationField);

            // min/max row with explicit label width and spacing
            var minMaxContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween,
                    alignItems = Align.FlexStart,
                    marginTop = 2,
                },
            };

            var minField = new PropertyField(minValueProp, "Min")
            {
                style =
                {
                    flexGrow = 1,
                    minWidth = new Length(50, LengthUnit.Percent),
                    color = new StyleColor(Color.white),
                    marginRight = 4,
                },
            };

            var maxField = new PropertyField(maxValueProp, "Max")
            {
                style =
                {
                    flexGrow = 1,
                    minWidth = new Length(50, LengthUnit.Percent),
                    color = new StyleColor(Color.white),
                    marginLeft = 4,
                },
            };

            minMaxContainer.Add(minField);
            minMaxContainer.Add(maxField);
            root.Add(minMaxContainer);

            void UpdateFields()
            {
                var mode = randomTypeProp.enumValueIndex;
                valueField.style.display =
                    mode == 0 || mode == 1 ? DisplayStyle.Flex : DisplayStyle.None;
                deviationField.style.display = mode == 1 ? DisplayStyle.Flex : DisplayStyle.None;
                minMaxContainer.style.display = mode == 2 ? DisplayStyle.Flex : DisplayStyle.None;
            }

            property.serializedObject.Update();
            UpdateFields();
            randomTypeField.RegisterValueChangedCallback(evt =>
            {
                property.serializedObject.ApplyModifiedProperties();
                UpdateFields();
            });

            return root;
        }
    }
#endif
}
