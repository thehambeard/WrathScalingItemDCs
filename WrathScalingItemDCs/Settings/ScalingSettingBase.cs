using Kingmaker.Settings;
using System;

namespace WrathScalingItemDCs.Settings
{
    public interface IScaleSetting
    {
        public int GetModifier(int orginalDc);
    }

    public abstract class ScalingSettingBase<T> : IScaleSetting
    {
        public T Normal;
        public T Hard;
        public T Unfair;

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
            int result = SettingsRoot.Difficulty.GameDifficulty.GetValue() switch
            {
                GameDifficultyOption.Unfair => orginalDC + Unfair,
                GameDifficultyOption.Hard => orginalDC + Hard,
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
            int result = SettingsRoot.Difficulty.GameDifficulty.GetValue() switch
            {
                GameDifficultyOption.Unfair => (int)Math.Round(orginalDC + (orginalDC * Unfair)),
                GameDifficultyOption.Hard => (int)Math.Round(orginalDC + (orginalDC * Hard)),
                _ => (int)Math.Round(orginalDC + (orginalDC * Normal))
            };

            return result;
        }
    }
}
