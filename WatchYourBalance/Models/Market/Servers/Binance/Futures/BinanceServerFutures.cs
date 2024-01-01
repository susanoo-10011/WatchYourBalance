using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WatchYourBalance.Entity;
using WatchYourBalance.Models.Market.Servers.Binance.Futures.Entity;

namespace WatchYourBalance.Models.Market.Servers.Binance.Futures
{
    public class BinanceServerFutures
    {
        public BinanceServerFutures()
        {

        }
    }

    public sealed class BinanceServerFuturesRealization
    {
        private static BinanceServerFuturesRealization _instance;
        private static readonly object lockObject = new object();

        private BinanceServerFuturesRealization()
        {
            ServerStatus = ServerConnectStatus.Disconnect;

            Thread worker = new Thread(PortfolioUpdater);
                worker.Start();
        }

        public static BinanceServerFuturesRealization Instance()
        {
            lock (lockObject)
            {
                if(_instance == null)
                {
                    _instance = new BinanceServerFuturesRealization();
                }
            }
            return _instance;
        }

        public void Connect()
        {
            if (_client == null)
            {
                _client = new BinanceClientFutures(ApiJson.ApiKeys().ApiKey, ApiJson.ApiKeys().ApiSecret);
                _client.Connected += _client_Connected;
                //_client.UpdatePairs += _client_UpdatePairs;
                _client.Disconnected += _client_Disconnected;
                    _client.NewPortfolio += _client_NewPortfolio;
                //    _client.UpdatePortfolio += _client_UpdatePortfolio;
                //    _client.UpdateMarketDepth += _client_UpdateMarketDepth;
                //    _client.NewTradesEvent += _client_NewTradesEvent;
                //    _client.MyTradeEvent += _client_MyTradeEvent;
                //    _client.MyOrderEvent += _client_MyOrderEvent;
                //    _client.ListenKeyExpiredEvent += _client_ListenKeyExpiredEvent;
                //    //_client.LogMessageEvent += SendLogMessage;
                //}

                //if (((ServerParameterEnum)ServerParameters[2]).Value == "USDT-M")
                //{
                //    this.futures_type = FuturesType.USDT;
                //    _client._baseUrl = "https://fapi.binance.com";
                //    _client.wss_point = "wss://fstream.binance.com";
                //    _client.type_str_selector = "fapi";
                //}

                //_client.futures_type = this.futures_type;
                //_client.HedgeMode = ((ServerParameterBool)ServerParameters[3]).Value;
                _client.Connect();
                ServerStatus = ServerConnectStatus.Connect;

            }
        }

        #region Портфели

        void _client_NewPortfolio(AccountResponseFutures portfs)
        {
            try
            {
                if (AccountEvent != null)
                {
                    AccountEvent(portfs);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public event Action<AccountResponseFutures> AccountEvent;

        /*private List<Portfolio> _portfolios = new List<Portfolio>();

        void _client_NewPortfolio(AccountResponseFutures portfs)
        {
            try
            {
                Portfolio myPortfolio = _portfolios.Find(p => p.Number == "BinanceFutures");

                if (myPortfolio == null)
                {
                    Portfolio newPortf = new Portfolio();
                    newPortf.Number = "BinanceFutures";
                    newPortf.ValueBegin = 1;
                    newPortf.ValueCurrent = 1;
                    _portfolios.Add(newPortf);
                    myPortfolio = newPortf;
                }



                if (portfs.assets == null)
                {
                    return;
                }

                foreach (var onePortf in portfs.assets)
                {
                    PositionOnBoard newPortf = new PositionOnBoard();
                    newPortf.SecurityNameCode = onePortf.asset;
                    newPortf.ValueBegin =
                        onePortf.marginBalance.ToDecimal();
                    newPortf.ValueCurrent =
                        onePortf.marginBalance.ToDecimal();


                    decimal lockedBalanceUSDT = 0m;
                    if (onePortf.asset.Equals("USDT"))
                    {

                        foreach (var position in portfs.positions)
                        {
                            if (position.symbol == "USDTUSDT") continue;

                            lockedBalanceUSDT += (position.initialMargin.ToDecimal() + position.maintMargin.ToDecimal());
                        }
                    }

                    newPortf.ValueBlocked = lockedBalanceUSDT;

                    myPortfolio.SetNewPosition(newPortf);
                }

                foreach (var onePortf in portfs.positions)
                {
                    if (string.IsNullOrEmpty(onePortf.positionAmt))
                    {
                        continue;
                    }

                    PositionOnBoard newPortf = new PositionOnBoard();

                    string name = onePortf.symbol + "_" + onePortf.positionSide;

                    newPortf.SecurityNameCode = name;
                    newPortf.ValueBegin =
                        onePortf.positionAmt.ToDecimal();

                    newPortf.ValueCurrent =
                        onePortf.positionAmt.ToDecimal();

                    myPortfolio.SetNewPosition(newPortf);
                }

                if (PortfolioEvent != null)
                {
                    PortfolioEvent(_portfolios);
                }
            }
            catch (Exception error)
            {
                //SendLogMessage(error.ToString(), LogMessageType.Error);
            }
        }*/

        private void PortfolioUpdater()
        {
            while (true)
            {
                Thread.Sleep(3000);

                if (this.ServerStatus == ServerConnectStatus.Disconnect)
                {
                    continue;
                }

                _client.GetBalance();
            }
        }
        #endregion



        void _client_Disconnected()
        {
            ServerStatus = ServerConnectStatus.Disconnect;

            if (DisconnectEvent != null)
            {
                DisconnectEvent();
            }

        }

        void _client_Connected()
        {
            ServerStatus = ServerConnectStatus.Connect;
            //Выставить HedgeMode
            _client.SetPositionMode();

            if (ConnectEvent != null)
            {
                ConnectEvent();
            }

        }

        /*/// <summary>
        /// появились новые портфели
        /// </summary>
        public event Action<List<Portfolio>> PortfolioEvent;*/

        /// <summary>
        /// binance client
        /// </summary>
        private BinanceClientFutures _client;


        /// <summary>
        /// request account info
        /// запросить статистику по аккаунту пользователя
        /// </summary>
        public AccountResponseFutures GetAccountInfo()
        {
            return _client.GetAccountInfo();
        }

        /// <summary>
        /// server status
        /// статус серверов
        /// </summary>
        public ServerConnectStatus ServerStatus { get; set; }

        /// <summary>
        /// API connection established
        /// соединение с API установлено
        /// </summary>
        public event Action ConnectEvent;

        /// <summary>
        /// API connection lost
        /// соединение с API разорвано
        /// </summary>
        public event Action DisconnectEvent;
    }
}
