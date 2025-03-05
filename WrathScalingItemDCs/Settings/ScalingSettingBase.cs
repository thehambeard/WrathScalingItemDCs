using Kingmaker.Settings;
using System;
using UnityEngine;

namespace WrathScalingItemDCs.Settings
{
    public interface IScaleSetting
    {
        public int GetModifier(int orginalDc);
        public ScaleSettingPreset GetPreset();
        public void SetPreset(ScaleSettingPreset preset);
    }

    public enum ScaleSettingPreset
    {
        Preset1,
        Preset2,
        Preset3,
    }

    public abstract class ScalingSettingBase<T> : IScaleSetting
    {
        public T Preset1;
        public T Preset2;
        public T Preset3;

        public ScaleSettingPreset Preset = ScaleSettingPreset.Preset1;

        protected ScalingSettingBase()
        {

        }

        protected ScalingSettingBase(T preset1, T preset2, T preset3)
        {
            Preset1 = preset1;
            Preset2 = preset2;
            Preset3 = preset3;
        }

        public abstract int GetModifier(int orginalDC);

        public ScaleSettingPreset GetPreset() => Preset;

        public void SetPreset(ScaleSettingPreset preset) => Preset = preset;
    }

    public class ScaleSettingFlat : ScalingSettingBase<int>, IScaleSetting
    {
        public ScaleSettingFlat() : base() { }

        public ScaleSettingFlat(int preset1, int preset2, int preset3) : base(preset1, preset2, preset3) { }

        public override int GetModifier(int orginalDC)
        {
            int result = Preset switch
            {
                ScaleSettingPreset.Preset3 => Flat(orginalDC, Preset3),
                ScaleSettingPreset.Preset2 => Flat(orginalDC, Preset2),
                _ => Flat(orginalDC, Preset1)
            };

            return result;
        }

        public static int Flat(int inputValue, int flatRate) =>
            inputValue + flatRate;
    }

    public class ScaleSettingPercent : ScalingSettingBase<float>, IScaleSetting
    {
        public ScaleSettingPercent() : base() { }

        public ScaleSettingPercent(float normal, float hard, float unfair) : base(normal, hard, unfair) { }

        public override int GetModifier(int orginalDC)
        {
            int result = Preset switch
            {
                ScaleSettingPreset.Preset3 => Percentage(orginalDC, Preset3),
                ScaleSettingPreset.Preset2 => Percentage(orginalDC, Preset2),
                _ => Percentage(orginalDC, Preset1)
            };

            return result;
        }

        public static int Percentage (int inputValue, float percentage) =>
            (int)(inputValue + (inputValue * percentage));
    }

    public class ScaleSettingDiminishingReturns : ScalingSettingBase<(float, float, float)>, IScaleSetting
    {
        public ScaleSettingDiminishingReturns() : base() { }

        public ScaleSettingDiminishingReturns((float, float, float) normal, (float, float, float) hard, (float, float, float) unfair) : base(normal, hard, unfair) { }

        public override int GetModifier(int orginalDC)
        {
            int result = Preset switch
            {
                ScaleSettingPreset.Preset3 => DiminishingReturns(orginalDC, Preset3),
                ScaleSettingPreset.Preset2 => DiminishingReturns(orginalDC, Preset2),
                _ => DiminishingReturns(orginalDC, Preset1),
            };

            return result;
        }

        public static int DiminishingReturns(int inputValue, double a, double b, double c)
        {
            return (int)(inputValue + ((inputValue + b) / (inputValue * a) + c));
        }
        
        public static int DiminishingReturns(int inputValue, (double a, double b, double c) values) =>
            DiminishingReturns(inputValue, values.a, values.b, values.c);
        


    }
}
