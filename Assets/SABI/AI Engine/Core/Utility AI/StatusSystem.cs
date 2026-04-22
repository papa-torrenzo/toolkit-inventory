namespace SABI
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public enum EnumLowerVsHigherIsButter
    {
        LowerIsBetter,
        HigherIsBetter,
    }

    public enum StatusElementType
    {
        None,
        Health,
        Stamina,
        Hunger,
        Bladder,
        Social,
        Fun,
        Happiness,
        Fear,
        Anger,
        Curiosity,
        Love,
        Embarrassment,
        Boredom,
        Stress,
    }

    public class StatusSystem : MonoBehaviour
    {
        [SerializeField]
        private bool randomize = true;

        // [Header("Physical Needs")]
        // [Range(0, 1)]
        public StatusData health =
                new(StatusElementType.Health, EnumLowerVsHigherIsButter.HigherIsBetter, 1),
            stamina = new(StatusElementType.Stamina, EnumLowerVsHigherIsButter.HigherIsBetter, 1),
            hunger = new(StatusElementType.Hunger, EnumLowerVsHigherIsButter.LowerIsBetter, 0),
            bladder = new(StatusElementType.Bladder, EnumLowerVsHigherIsButter.LowerIsBetter, 0.3f),
            social = new(StatusElementType.Social, EnumLowerVsHigherIsButter.HigherIsBetter, 0.5f),
            fun = new(StatusElementType.Fun, EnumLowerVsHigherIsButter.HigherIsBetter, 0.5f);

        // [Header("Emotions")]
        // [Range(0, EnumLowerVsHigherIsButter.LowerIsBetter, 1)]
        public StatusData happiness =
                new(StatusElementType.Happiness, EnumLowerVsHigherIsButter.HigherIsBetter, 0.5f),
            fear = new(StatusElementType.Fear, EnumLowerVsHigherIsButter.LowerIsBetter, 0),
            anger = new(StatusElementType.Anger, EnumLowerVsHigherIsButter.LowerIsBetter, 0),
            curiosity =
                new(StatusElementType.Curiosity, EnumLowerVsHigherIsButter.LowerIsBetter, 0),
            love = new(StatusElementType.Love, EnumLowerVsHigherIsButter.HigherIsBetter, 0.5f),
            embarrassment =
                new(StatusElementType.Embarrassment, EnumLowerVsHigherIsButter.LowerIsBetter, 0),
            boredom = new(StatusElementType.Boredom, EnumLowerVsHigherIsButter.LowerIsBetter, 0),
            stress = new(StatusElementType.Stress, EnumLowerVsHigherIsButter.LowerIsBetter, 0);

        public Dictionary<StatusElementType, StatusData> StatusDictionary = new();

        private void Start()
        {
            StatusDictionary.Add(StatusElementType.Health, health);
            StatusDictionary.Add(StatusElementType.Stamina, stamina);
            StatusDictionary.Add(StatusElementType.Hunger, hunger);
            StatusDictionary.Add(StatusElementType.Bladder, bladder);
            StatusDictionary.Add(StatusElementType.Social, social);
            StatusDictionary.Add(StatusElementType.Fun, fun);
            StatusDictionary.Add(StatusElementType.Happiness, happiness);
            StatusDictionary.Add(StatusElementType.Fear, fear);
            StatusDictionary.Add(StatusElementType.Anger, anger);
            StatusDictionary.Add(StatusElementType.Curiosity, curiosity);
            StatusDictionary.Add(StatusElementType.Love, love);
            StatusDictionary.Add(StatusElementType.Embarrassment, embarrassment);
            StatusDictionary.Add(StatusElementType.Boredom, boredom);
            StatusDictionary.Add(StatusElementType.Stress, stress);

            // Randomizing
            if (randomize)
                StatusDictionary.Values.ToList().ForEach(item =>
                {
                    item.CurrentValue = Random.Range(0.1f, 0.9f);
                });
        }

        void Update()
        {
            // mp = "minute percent" (standard rate of change)
            float mp = Time.deltaTime * 0.005f;

            // --- 1. NATURAL DECAY ---
            hunger.Add(mp * 0.5f); // Getting hungry over time
            stamina.Remove(mp * 0.2f); // Getting tired slowly
            boredom.Add(mp * 0.3f); // Getting bored if nothing happens
            bladder.Add(hunger.Get() * mp); // Bladder fills faster if they've been eating/drinking

            // --- 2. PHYSICAL INFLUENCES ---
            if (health.Get() < 0.3f)
            {
                fear.Add(mp * 2);
                stress.Add(mp);
                happiness.Remove(mp);
            }

            if (stamina.Get() < 0.2f)
            {
                stress.Add(mp);
                fear.Add(mp);
                happiness.Remove(mp * 0.5f); // Sleep deprivation makes you sad
            }

            // The "Hangry" Logic
            if (hunger.Get() > 0.75f)
            {
                stress.Add(mp);
                anger.Add(mp * 1.5f);
                stamina.Remove(mp);
                fun.Remove(mp);
                curiosity.Remove(mp);
            }

            // Bladder Panic (Comedy Gold)
            if (bladder.Get() > 0.8f)
            {
                stress.Add(mp * 2);
                fear.Add(mp); // Fear of an "accident"
                fun.Remove(mp * 2); // Hard to have fun when you need to go
                curiosity.ResetToMin(); // Zero focus on anything else
            }

            // --- 3. EMOTIONAL CROSS-POLLINATION ---
            // Stress is the "Multiplier": High stress makes every negative emotion worse
            float stressFactor = 1 + stress.Get();

            if (stress.Get() > 0.5f)
            {
                anger.Add(mp * stressFactor * 0.2f);
                happiness.Remove(mp * 0.1f);
            }

            // Boredom leads to curiosity or sadness
            if (boredom.Get() > 0.7f)
            {
                curiosity.Add(mp); // Looking for something to do
                happiness.Remove(mp * 0.5f); // Getting "The Blues"
            }

            // Loneliness (Low Social)
            if (social.Get() < 0.2f)
            {
                happiness.Remove(mp);
                boredom.Add(mp);
            }

            // --- 4. CLAMPING VALUES ---
            // Ensures your [Range(0,1)] stays valid
        }

        public void OnInteract(UtilityProvidingObject obj)
        {
            foreach (var item in obj.intractableObjectDatas)
            {
                StatusData data = StatusDictionary[item.statusElementType];
                if (item.addOrRemove == EnumAddRemove.Add)
                    data.Add(item.value);
                else
                    data.Remove(item.value);
            }
        }

        public void OnInteract(StatusData data)
        {
            if (data.enumLowerVsHigherIsButter == EnumLowerVsHigherIsButter.LowerIsBetter)
            {
                data.ResetToMin();
            }
            else
            {
                data.ResetToMax();
            }
        }
    }

    [System.Serializable]
    public class StatusData
    {
        public StatusElementType StatusType;

        [Range(0, 1)]
        public float CurrentValue = 0.5f;

        [HideInInspector]
        public float MinValue = 0,
            MaxValue = 1;
        public AnimationCurve AnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float weight = 1.0f; // Personality
        public EnumLowerVsHigherIsButter enumLowerVsHigherIsButter;
        public float UtilityValue = 0;

        public StatusData(
            StatusElementType StatusType,
            EnumLowerVsHigherIsButter enumLowerVsHigherIsButter,
            float startValue = 1f,
            float min = 0f,
            float max = 1f
        // ,
        // bool randomize = true
        )
        {
            this.StatusType = StatusType;
            this.CurrentValue = startValue;
            this.MinValue = min;
            this.MaxValue = max;
            // Initializes as a linear curve from (0,0) to (1,1)
            this.AnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            this.enumLowerVsHigherIsButter = enumLowerVsHigherIsButter;
        }

        public float Get() => CurrentValue;

        public void Set(float value) => CurrentValue = Mathf.Clamp(value, MinValue, MaxValue);

        public void ResetToMin() => Set(MinValue);

        public void ResetToMax() => Set(MaxValue);

        public void ResetToBetterExtremeValue()
        {
            Debug.Log(
                $"[SAB] ResetToBetterExtremeValue() 1 {StatusType} {enumLowerVsHigherIsButter}"
            );
            switch (enumLowerVsHigherIsButter)
            {
                case EnumLowerVsHigherIsButter.LowerIsBetter:
                    ResetToMin();
                    break;
                case EnumLowerVsHigherIsButter.HigherIsBetter:
                    ResetToMax();
                    break;
            }
            Debug.Log($"[SAB] ResetToBetterExtremeValue() 2 value: {CurrentValue}");
        }

        public void Add(float value) => Set(CurrentValue + value);

        public void Remove(float value) => Set(CurrentValue - value);

        public float GetUtilityValue()
        {
            float valueToEvaluate =
                enumLowerVsHigherIsButter == EnumLowerVsHigherIsButter.LowerIsBetter
                    ? CurrentValue
                    : 1 - CurrentValue;
            UtilityValue = AnimationCurve.Evaluate(valueToEvaluate) * weight;
            return UtilityValue;
        }
    }
}
