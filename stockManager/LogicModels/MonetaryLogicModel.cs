using stockManager.Data;
using stockManager.Misc;
using stockManager.Models;
using System;

namespace stockManager.LogicModels
{
    public class MonetaryLogicModel
    {

        public MonetaryLogicModel(decimal value, Currency currency)
        {
            this.Value = value;
            this.Currency = currency;
        }

        public decimal Value { get; set; }
        public Currency Currency { get; set; }

        private CurrencyDao currencyDao = new CurrencyDao();

        public void Add(MonetaryLogicModel monetary)
        {
            Value += CurrencyConverter.Convert(monetary.Currency.Name, Currency.Name, monetary.Value);
        }

        public void Add(MonetaryLogicModel monetary, DateTime dateTime)
        {
            Value += CurrencyConverter.Convert(monetary.Currency.Name, Currency.Name, monetary.Value, dateTime);
        }

        public void Substract(MonetaryLogicModel monetary)
        {
            Value -= CurrencyConverter.Convert(monetary.Currency.Name, Currency.Name, monetary.Value);
        }

        public void Substract(MonetaryLogicModel monetary, DateTime dateTime)
        {
            Value -= CurrencyConverter.Convert(monetary.Currency.Name, Currency.Name, monetary.Value, dateTime);
        }

        public static MonetaryLogicModel operator +(MonetaryLogicModel left, MonetaryLogicModel right)
        {
            MonetaryLogicModel res = new MonetaryLogicModel(left.Value, left.Currency);
            res.Add(right);
            return res;
        }


        public static MonetaryLogicModel operator +(MonetaryLogicModel left, decimal right)
        {
            MonetaryLogicModel res = new MonetaryLogicModel(left.Value + right, left.Currency);
            return res;
        }

        public static MonetaryLogicModel operator -(MonetaryLogicModel left, MonetaryLogicModel right)
        {
            MonetaryLogicModel res = new MonetaryLogicModel(left.Value, left.Currency);
            res.Substract(right);
            return res;
        }

        public static MonetaryLogicModel operator *(MonetaryLogicModel left, decimal right)
        {
            return new MonetaryLogicModel(left.Value * right, left.Currency);
        }

        public static MonetaryLogicModel operator *(decimal left, MonetaryLogicModel right)
        {
            return right * left;
        }

        public static MonetaryLogicModel operator /(MonetaryLogicModel left, decimal right)
        {
            return new MonetaryLogicModel(left.Value / right, left.Currency);
        }

        public static MonetaryLogicModel operator /(decimal left, MonetaryLogicModel right)
        {
            return right / left;
        }

        public static MonetaryLogicModel operator /(MonetaryLogicModel left, MonetaryLogicModel right)
        {
            return new MonetaryLogicModel(left.Value / right.Value, left.Currency);
        }

        public MonetaryLogicModel ConvertTo(Currency toCurrency)
        {
            return ConvertTo(toCurrency.Name);
        }

        public MonetaryLogicModel ConvertTo(string toCurrency)
        {
            return new MonetaryLogicModel(
                    CurrencyConverter.Convert(Currency.Name, toCurrency, Value),
                    currencyDao.Get(toCurrency)
                );
        }

    }
}
