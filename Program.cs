using BitsoDotNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitso2Cointracking
{
    public class Program
    {
        public const string BitsoName = "Bitso";
        public const string Mxn = "MXN";
        public const int BitsoQueryPageSize = 100;

        static void Main(string[] args)
        {
            // Read settings.
            var apikey = ConfigurationManager.AppSettings["BitsoApikey"];
            var secret = ConfigurationManager.AppSettings["BitsoSecret"];
            var cointrackingCurrency = ConfigurationManager.AppSettings["CointrackingAccountCurrency"].ToUpperInvariant();

            // Connect to Bitso and download books (trading pairs).
            Console.Error.Write("Connecting to Bitso... ");
            var bitsoClient = new Bitso(apikey, secret, true);
            var books = bitsoClient.PublicAPI.GetAvailableBooks();
            Console.Error.WriteLine("Done");

            // All downloaded transactions will be saved in this list.
            var transactions = new List<Transaction>();

            // Get withdrawals.
            Console.Error.Write("Downloading withdrawals... ");
            var withdrawals = bitsoClient.PrivateAPI.GetWithdrawals(BitsoQueryPageSize);
            transactions.AddRange(
                withdrawals.Select(w => new Transaction
                {
                    Type = TransactionType.Withdrawal,
                    SellAmount = w.AmountAsDecimal,
                    SellCurrency = w.Currency.ToUpperInvariant(),
                    TxId = w.Wid,
                    Exchange = BitsoName,
                    Date = DateTime.Parse(w.CreatedAt)
                }));
            Console.Error.WriteLine("Done");

            // Get fundings.
            Console.Error.Write("Downloading fundings... ");
            var currencyConverter = new CurrencyConverter();
            var fundings = bitsoClient.PrivateAPI.GetFundings(BitsoQueryPageSize);
            transactions.AddRange(
                fundings.Select(w => new Transaction
                {
                    Type = w.Currency.ToUpperInvariant().Equals(Mxn) && !cointrackingCurrency.Equals(Mxn) ? TransactionType.Income : TransactionType.Deposit,
                    BuyAmount = w.AmountAsDecimal,
                    BuyCurrency = w.Currency.ToUpperInvariant(),
                    BuyValueInCurrency = w.Currency.ToUpperInvariant().Equals(Mxn) && !cointrackingCurrency.Equals(Mxn) ? (decimal?)w.AmountAsDecimal * currencyConverter.GetExchangeRate(Mxn, cointrackingCurrency, DateTime.Parse(w.CreatedAt)).Result : null,
                    TxId = w.Fid,
                    Exchange = BitsoName,
                    Date = DateTime.Parse(w.CreatedAt),
                    Comment = w.Currency.ToUpperInvariant().Equals(Mxn) && !cointrackingCurrency.Equals(Mxn) ? "Foreign currency deposit (not income)" : null
                }));
            Console.Error.WriteLine("Done");

            // BUGBUG: BitsoDotNet doesn't know how to get the next page of data of withdrawals and fundings.
            if (withdrawals.Length == BitsoQueryPageSize)
            {
                throw new NotImplementedException(
                    $"Can't download more than {BitsoQueryPageSize} withdrawals. " + 
                    "Consider contributing to BitsoDotNet library and this project.");
            }
            else if (fundings.Length == BitsoQueryPageSize)
            {
                throw new NotImplementedException(
                    $"Can't download more than {BitsoQueryPageSize} fundings. " +
                    "Consider contributing to BitsoDotNet library and this project.");
            }

            // Get trades.
            foreach (var book in books)
            {
                Console.Error.Write($"Downloading trades for {book.Book}...");

                long marker = 0;
                do
                {
                    var userTrades = bitsoClient.PrivateAPI.GetUserTrades(
                        book: book.Book,
                        marker: marker.ToString(CultureInfo.InvariantCulture),
                        limit: BitsoQueryPageSize);

                    marker = 0;
                    foreach (var userTrade in userTrades)
                    {
                        var bookSymbols = userTrade.Book.ToUpperInvariant().Split('_');
                        var transaction = new Transaction
                        {
                            Type = TransactionType.Trade,
                            Exchange = BitsoName,
                            Date = DateTime.Parse(userTrade.CreatedAt),
                            TxId = userTrade.Tid.ToString(CultureInfo.InvariantCulture),
                            FeeCurrency = userTrade.FeesCurrency.ToUpperInvariant(),
                            Fee = userTrade.FeesAmountAsDecimal
                        };

                        if (userTrade.Side.Equals("buy", StringComparison.OrdinalIgnoreCase))
                        {
                            transaction.BuyAmount = userTrade.MajorAsDecimal - userTrade.FeesAmountAsDecimal;
                            transaction.BuyCurrency = bookSymbols[0];
                            transaction.SellAmount = -userTrade.MinorAsDecimal;
                            transaction.SellCurrency = bookSymbols[1];
                        }
                        else
                        {
                            transaction.BuyAmount = userTrade.MinorAsDecimal - userTrade.FeesAmountAsDecimal;
                            transaction.BuyCurrency = bookSymbols[1];
                            transaction.SellAmount = -userTrade.MajorAsDecimal;
                            transaction.SellCurrency = bookSymbols[0];
                        }

                        transactions.Add(transaction);
                        marker = userTrade.Tid;

                        Console.Error.Write(".");
                    }
                } while (marker != 0);

                Console.Error.WriteLine(" Done");
            }

            // Print
            Console.Error.WriteLine("Sorting and outputting data to stdout:");
            Transaction.WriteCsvHeader(Console.Out);
            foreach (var transaction in transactions.OrderByDescending(t => t.Date).ThenByDescending(t => t.TxId))
            {
                transaction.WriteCsvLine(Console.Out);
            }
            Console.Error.WriteLine("Done Ssrting and outputting data to stdout.");
        }
    }
}
