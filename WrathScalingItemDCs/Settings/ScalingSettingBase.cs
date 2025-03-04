using Kingmaker.Settings;
using System;
using UnityEngine;

namespace WrathScalingItemDCs.Settings
{
    public interface IScaleSetting
    {
        public int GetModifier(int orginalDc);
    }

    public enum ScaleSettingDifficulty
    {
        Normal,
        Hard,
        Unfair,
    }

    public abstract class ScalingSettingBase<T> : IScaleSetting
    {
        public T Normal;
        public T Hard;
        public T Unfair;

        public ScaleSettingDifficulty Difficulty = ScaleSettingDifficulty.Normal;

        protected ScalingSettingBase()
        {

        }

        protected ScalingSettingBase(T normal, T hard, T unfair)
        {
            Normal = normal;
            Hard = hard;
            Unfair = unfair;
        }

        public abstract int GetModifier(int orginalDC);
    }

    public class ScaleSettingFlat : ScalingSettingBase<int>, IScaleSetting
    {
        public ScaleSettingFlat() : base() { }

        public ScaleSettingFlat(int normal, int hard, int unfair) : base(normal, hard, unfair) { }

        public override int GetModifier(int orginalDC)
        {
            int result = Difficulty switch
            {
                ScaleSettingDifficulty.Unfair => orginalDC + Unfair,
                ScaleSettingDifficulty.Hard => orginalDC + Hard,
                _ => orginalDC + Normal
            };

            return result;
        }
    }

    public class ScaleSettingPercent : ScalingSettingBase<float>, IScaleSetting
    {
        public ScaleSettingPercent() : base() { }

        public ScaleSettingPercent(float normal, float hard, float unfair) : base(normal, hard, unfair) { }

        public override int GetModifier(int orginalDC)
        {
            int result = Difficulty switch
            {
                ScaleSettingDifficulty.Unfair => (int)(orginalDC + (orginalDC * Unfair)),
                ScaleSettingDifficulty.Hard => (int)(orginalDC + (orginalDC * Hard)),
                _ => (int)(orginalDC + (orginalDC * Normal))
            };

            return result;
        }
    }

    public class ScaleSettingDiminishingReturns : ScalingSettingBase<(float, float, float)>, IScaleSetting
    {
        public ScaleSettingDiminishingReturns() : base() { }

        public ScaleSettingDiminishingReturns((float, float, float) normal, (float, float, float) hard, (float, float, float) unfair) : base(normal, hard, unfair) { }

        public override int GetModifier(int orginalDC)
        {
            int result = Difficulty switch
            {
                ScaleSettingDifficulty.Unfair => (int)DiminishingReturns(orginalDC, Unfair),
                ScaleSettingDifficulty.Hard => (int)DiminishingReturns(orginalDC, Hard),
                _ => (int)DiminishingReturns(orginalDC, Normal),
            };

            return result;
        }

        public static int DiminishingReturns(double inputValue, double a, double b, double c)
        {
            return (int)(inputValue + ((inputValue + b) / (inputValue * a) + c));
        }
        
        public static int DiminishingReturns(double inputValue, (double a, double b, double c) values) =>
            DiminishingReturns(inputValue, values.a, values.b, values.c);
        


    }
}
