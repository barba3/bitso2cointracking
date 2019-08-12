namespace Bitso2Cointracking
{
    using System;
    using System.Globalization;
    using System.IO;

    public enum TransactionType
    {
        Trade,
        Income,
        Deposit,
        Withdrawal,
        Spend,
    }

    public class Transaction
    {
        public TransactionType Type { get; set; }

        public decimal? BuyAmount { get; set; }

        public string BuyCurrency { get; set; }

        public decimal? SellAmount { get; set; }

        public string SellCurrency { get; set; }

        public decimal? Fee { get; set; }

        public string FeeCurrency { get; set; }

        public string Exchange { get; set; }

        public string TradeGroup { get; set; }

        public string Comment { get; set; }

        public DateTime Date { get; set; }

        public string TxId { get; set; }

        public decimal? BuyValueInCurrency { get; set; }

        public decimal? SellValueInCurrency { get; set; }

        public static void WriteCsvHeader(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.WriteLine(@"""Type"",""Buy Amount"",""Buy Currency"",""Sell Amount""," +
                @"""Sell Currency"",""Fee"",""Fee Currency"",""Exchange"",""Trade-Group""," +
                @"""Comment"",""Date"",""Tx-ID"",""Buy Value in your Account Currency""," +
                @"""Sell Value in your Account Currency""");
        }

        public void WriteCsvLine(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            writer.Write(this.Type.ToString());
            writer.Write(",");
            writer.Write(this.BuyAmount);
            writer.Write(",");
            writer.Write(this.BuyCurrency);
            writer.Write(",");
            writer.Write(this.SellAmount);
            writer.Write(",");
            writer.Write(this.SellCurrency);
            writer.Write(",");
            writer.Write(this.Fee);
            writer.Write(",");
            writer.Write(this.FeeCurrency);
            writer.Write(",");
            writer.Write(this.Exchange);
            writer.Write(",");
            writer.Write(this.TradeGroup);
            writer.Write(",");
            writer.Write(this.Comment);
            writer.Write(",");
            writer.Write(this.Date.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK", CultureInfo.InvariantCulture));
            writer.Write(",");
            writer.Write(this.TxId);
            writer.Write(",");
            writer.Write(this.BuyValueInCurrency);
            writer.Write(",");
            writer.Write(this.SellValueInCurrency);
            writer.WriteLine();
        }
    }
}
