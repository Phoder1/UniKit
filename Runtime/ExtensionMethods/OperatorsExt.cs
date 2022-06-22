using UnityEngine;

namespace UniKit
{
    public enum OperatorEnum
    {
        Add,
        Multiply,
        Set,
        Reduce,
        Divide,
        Power,
        PrecentageAdd,
        PrecentageReduce,
        PrecentageSet,
        PrecentageDivide,
        PrecentagePower,
    }


    public static class OperatorsExt
    {
        public static bool IsEquals(this object obj1, object obj2) => obj1 == obj2;



        //public static int EffectOperator(this int val, int modifingValue, OperatorEnum effectOperator)
        //    => Mathf.FloorToInt(val.EffectOperator(modifingValue, effectOperator));
        public static float Apply(this OperatorEnum effectOperator, float val, float modifingValue)
        {
            switch (effectOperator)
            {
                case OperatorEnum.Add:
                    return val + modifingValue;
                case OperatorEnum.Multiply:
                    return val * modifingValue;
                case OperatorEnum.Set:
                    return modifingValue;
                case OperatorEnum.Reduce:
                    return val - modifingValue;
                case OperatorEnum.Divide:
                    return val / modifingValue;
                case OperatorEnum.Power:
                    return Mathf.Pow(val, modifingValue);
                case OperatorEnum.PrecentageAdd:
                    return val + val * modifingValue / 100;
                case OperatorEnum.PrecentageReduce:
                    return val - val * modifingValue / 100;
                case OperatorEnum.PrecentageSet:
                    return val * modifingValue / 100;
                case OperatorEnum.PrecentageDivide:
                    return val / (val * modifingValue / 100);
                case OperatorEnum.PrecentagePower:
                    modifingValue = val * modifingValue / 100;
                    return Mathf.Pow(val, modifingValue);

            }

            throw new System.Exception($"EffectOperator Is not valid!");
        }
    }
}

