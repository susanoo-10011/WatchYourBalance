using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WatchYourBalance.Entity;
using WatchYourBalance.Models.Market.Servers.Binance.Futures.Entity;
using WatchYourBalance.Models.Market.Servers.Binance.Spot.BinanceSpotEntity;
using WatchYourBalance.Models.Market.Servers.Entity;
using WebSocket4Net;
using static WatchYourBalance.Entity.Position;

namespace WatchYourBalance.Models.Market.Servers.Binance.Futures
{
    public class BinanceClientFutures
    {
        private string ApiKey;
        private string ApiSecret;
        public BinanceClientFutures(string apiKey, string apiSecret)
        {
            ApiKey = apiKey;
            ApiSecret = apiSecret;
        }

        public string _baseUrl = "https://fapi.binance.com";
        public string wss_point = "wss://fstream.binance.com";
        public string type_str_selector = "fapi";

        public bool IsConnected;
        public void Connect()
        {
            if (string.IsNullOrEmpty(ApiKey) ||
                string.IsNullOrEmpty(ApiSecret))
            {
                return;
            }

            // проверяем доступность сервера для HTTP общения с ним
            Uri uri = new Uri(_baseUrl + "/" + type_str_selector + "/v1/time");
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Сервер не доступен. Отсутствует интернет.");
                return;
            }

            IsConnected = true;

            CreateDataStreams();
        }

        private string _listenKey = "";

        private WebSocket _socketClient;
        private void CreateDataStreams()
        {
            try
            {
                _listenKey = CreateListenKey();
                string urlStr = wss_point + "/ws/" + _listenKey;

                _socketClient = new WebSocket(urlStr);
                _socketClient.Opened += Connect;
                _socketClient.Closed += Disconnect;
                _socketClient.Error += WsError;
                _socketClient.MessageReceived += UserDataMessageHandler;
                _socketClient.Open();

                _wsStreams.Add("userDataStream", _socketClient);

                Thread keepalive = new Thread(KeepaliveUserDataStream);
                keepalive.CurrentCulture = new CultureInfo("ru-RU");
                keepalive.IsBackground = true;
                keepalive.Start();

                Thread converter = new Thread(Converter);
                converter.CurrentCulture = new CultureInfo("ru-RU");
                converter.IsBackground = true;
                converter.Start();

                Thread converterUserData = new Thread(ConverterUserData);
                converterUserData.CurrentCulture = new CultureInfo("ru-RU");
                converterUserData.IsBackground = true;
                converterUserData.Start();
            }
            catch (Exception exception)
            {
                //SendLogMessage(exception.Message, LogMessageType.Connect);
            }
        }

        private object _lock = new object();

        /// <summary>
        /// показывает статистику по аккаунту пользователя
        /// </summary>
        public AccountResponseFutures GetAccountInfo()
        {
            lock (_lock)
            {
                try
                {
                    string res = null;

                    if (type_str_selector == "dapi")
                    {
                        res = CreateQuery(Method.Get, "/" + type_str_selector + "/v1/account", null, true);
                    }
                    else if (type_str_selector == "fapi")
                    {
                        res = CreateQuery(Method.Get, "/" + type_str_selector + "/v2/account", null, true);
                    }

                    if (res == null)
                    {
                        return null;
                    }

                    AccountResponseFutures resp = JsonConvert.DeserializeAnonymousType(res, new AccountResponseFutures());
                    return resp;
                }
                catch (Exception ex)
                {
                    //SendLogMessage(ex.ToString(), LogMessageType.Error);
                    return null;
                }
            }
        }

        /// <summary>
        /// баланс портфеля
        /// </summary>
        public void GetBalance()
        {
            try
            {
                
                AccountResponseFutures resp = GetAccountInfo();
                if (NewPortfolio != null && resp != null)
                {
                    NewPortfolio(resp);
                }
            }
            catch (Exception ex)
            {
                //SendLogMessage(ex.ToString(), LogMessageType.Error);
            }
        }

        public event Action<AccountResponseFutures> NewPortfolio;

        /// <summary>
        /// берет сообщения из общей очереди, конвертирует их в классы C# и отправляет на верх
        /// </summary>
        public void Converter()
        {
            while (true)
            {
                try
                {

                    if (!_newMessage.IsEmpty)
                    {
                        string mes;

                        if (_newMessage.TryDequeue(out mes))
                        {
                            if (mes.Contains("error"))
                            {
                                //SendLogMessage(mes, LogMessageType.Error);
                            }

                            else if (mes.Contains("\"e\":\"trade\""))
                            {
                                var quotes = JsonConvert.DeserializeAnonymousType(mes, new TradeResponse());


                                if (quotes.data.X.ToString() != "MARKET")
                                {//INSURANCE_FUND
                                    continue;
                                }

                                if (NewTradesEvent != null)
                                {
                                    NewTradesEvent(quotes);
                                }
                                continue;
                            }

                            else if (mes.Contains("\"depthUpdate\""))
                            {
                                var quotes = JsonConvert.DeserializeAnonymousType(mes, new DepthResponseFutures());

                                if (UpdateMarketDepth != null)
                                {
                                    UpdateMarketDepth(quotes);
                                }
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (_isDisposed)
                        {
                            return;
                        }
                        Thread.Sleep(1);
                    }
                }

                catch (Exception exception)
                {
                    //SendLogMessage(exception.ToString(), LogMessageType.Error);
                }
            }
        }


        public bool HedgeMode
        {
            get { return _hedgeMode; }
            set
            {
                if (value == _hedgeMode)
                {
                    return;
                }
                _hedgeMode = value;
                SetPositionMode();
            }
        }
        private bool _hedgeMode;
        public void SetPositionMode()
        {
            try
            {
                if (IsConnected == false)
                {
                    return;
                }
                var rs = CreateQuery(Method.Get, "/" + type_str_selector + "/v1/positionSide/dual", new Dictionary<string, string>(), true);
                if (rs != null)
                {
                    var modeNow = JsonConvert.DeserializeAnonymousType(rs, new HedgeModeResponse());
                    if (modeNow.dualSidePosition != HedgeMode)
                    {
                        var param = new Dictionary<string, string>();
                        param.Add("dualSidePosition=", HedgeMode.ToString().ToLower());
                        CreateQuery(Method.Post, "/" + type_str_selector + "/v1/positionSide/dual", param, true);
                    }
                }
            }
            catch (Exception ex)
            {
                //SendLogMessage(ex.ToString(), LogMessageType.Error);

            }

        }

        /// <summary>
        /// берет сообщения из общей очереди, конвертирует их в классы C# и отправляет на верх
        /// </summary>
        public void ConverterUserData()
        {
            while (true)
            {
                try
                {
                    if (!_newUserDataMessage.IsEmpty)
                    {
                        BinanceUserMessage messsage;

                        if (_newUserDataMessage.TryDequeue(out messsage))
                        {
                            string mes = messsage.MessageStr;

                            if (mes.Contains("code"))
                            {
                                // если есть code ошибки, то пытаемся распарсить
                                ErrorMessage _err = new ErrorMessage();

                                try
                                {
                                    _err = JsonConvert.DeserializeAnonymousType(mes, new ErrorMessage());
                                }
                                catch (Exception e)
                                {
                                    // если не смогли распарсить, то просто покажем что пришло
                                    _err.code = 9999;
                                    _err.msg = mes;
                                }
                                //SendLogMessage("code:" + _err.code.ToString() + ",msg:" + _err.msg, LogMessageType.Error);
                            }

                            else if (mes.Contains("\"e\"" + ":" + "\"ORDER_TRADE_UPDATE\""))
                            {
                                // если ошибки в ответе ордера
                                OrderUpdResponse ord = new OrderUpdResponse();
                                try
                                {
                                    ord = JsonConvert.DeserializeAnonymousType(mes, new OrderUpdResponse());
                                }
                                catch (Exception)
                                {
                                    //SendLogMessage("error in order update:" + mes, LogMessageType.Error);
                                    continue;
                                }

                                var order = ord.o;

                                int orderNumUser;

                                try
                                {
                                    orderNumUser = Convert.ToInt32(order.c.ToString().Replace("x-gnrPHWyE", ""));
                                }
                                catch (Exception)
                                {
                                    orderNumUser = Convert.ToInt32(order.c.GetHashCode());
                                }

                                if (order.x == "NEW")
                                {
                                    Order newOrder = new Order();
                                    newOrder.SecurityNameCode = order.s;
                                    newOrder.TimeCallBack = new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToDouble(ord.T));
                                    newOrder.NumberUser = orderNumUser;

                                    newOrder.NumberMarket = order.i.ToString();
                                    //newOrder.PortfolioNumber = order.PortfolioNumber; добавить в сервере
                                    newOrder.Side = order.S == "BUY" ? Side.Buy : Side.Sell;
                                    newOrder.State = OrderStateType.Activ;
                                    newOrder.Volume = order.q.ToDecimal();
                                    newOrder.Price = order.p.ToDecimal();
                                    newOrder.ServerType = ServerType.BinanceFutures;
                                    newOrder.PortfolioNumber = "BinanceFutures";

                                    if (MyOrderEvent != null)
                                    {
                                        MyOrderEvent(newOrder);
                                    }
                                }
                                else if (order.x == "CANCELED")
                                {
                                    Order newOrder = new Order();
                                    newOrder.SecurityNameCode = order.s;
                                    newOrder.TimeCallBack = new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToDouble(ord.T));
                                    newOrder.TimeCancel = newOrder.TimeCallBack;
                                    newOrder.NumberUser = orderNumUser;
                                    newOrder.NumberMarket = order.i.ToString();
                                    newOrder.Side = order.S == "BUY" ? Side.Buy : Side.Sell;
                                    newOrder.State = OrderStateType.Cancel;
                                    newOrder.Volume = order.q.ToDecimal();
                                    newOrder.Price = order.p.ToDecimal();
                                    newOrder.ServerType = ServerType.BinanceFutures;
                                    newOrder.PortfolioNumber = "BinanceFutures";

                                    if (MyOrderEvent != null)
                                    {
                                        MyOrderEvent(newOrder);
                                    }
                                }
                                else if (order.x == "REJECTED")
                                {
                                    Order newOrder = new Order();
                                    newOrder.SecurityNameCode = order.s;
                                    newOrder.TimeCallBack = new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToDouble(ord.T));
                                    newOrder.NumberUser = orderNumUser;
                                    newOrder.NumberMarket = order.i.ToString();
                                    newOrder.Side = order.S == "BUY" ? Side.Buy : Side.Sell;
                                    newOrder.State = OrderStateType.Fail;
                                    newOrder.Volume = order.q.ToDecimal();
                                    newOrder.Price = order.p.ToDecimal();
                                    newOrder.ServerType = ServerType.BinanceFutures;
                                    newOrder.PortfolioNumber = "BinanceFutures";

                                    if (MyOrderEvent != null)
                                    {
                                        MyOrderEvent(newOrder);
                                    }
                                }
                                else if (order.x == "TRADE")
                                {

                                    MyTrade trade = new MyTrade();
                                    trade.Time = new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToDouble(order.T));
                                    trade.NumberOrderParent = order.i;
                                    trade.NumberTrade = order.t;
                                    trade.Volume = order.l.ToDecimal();
                                    trade.Price = order.L.ToDecimal();
                                    trade.SecurityNameCode = order.s;
                                    trade.Side = order.S == "BUY" ? Side.Buy : Side.Sell;

                                    if (MyTradeEvent != null)
                                    {
                                        MyTradeEvent(trade);
                                    }
                                }
                                else if (order.x == "EXPIRED")
                                {
                                    Order newOrder = new Order();
                                    newOrder.SecurityNameCode = order.s;
                                    newOrder.TimeCallBack = new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToDouble(ord.T));
                                    newOrder.TimeCancel = newOrder.TimeCallBack;
                                    newOrder.NumberUser = orderNumUser;
                                    newOrder.NumberMarket = order.i.ToString();
                                    newOrder.Side = order.S == "BUY" ? Side.Buy : Side.Sell;
                                    newOrder.State = OrderStateType.Cancel;
                                    newOrder.Volume = order.q.ToDecimal();
                                    newOrder.Price = order.p.ToDecimal();
                                    newOrder.ServerType = ServerType.BinanceFutures;
                                    newOrder.PortfolioNumber = "BinanceFutures";

                                    if (MyOrderEvent != null)
                                    {
                                        MyOrderEvent(newOrder);
                                    }
                                }

                                continue;
                            }

                            else if (mes.Contains("\"e\"" + ":" + "\"ACCOUNT_UPDATE\""))
                            {
                                var portfolios = JsonConvert.DeserializeAnonymousType(mes, new AccountResponseFuturesFromWebSocket());

                                if (UpdatePortfolio != null)
                                {
                                    UpdatePortfolio(portfolios);
                                }
                                continue;
                            }

                            else if (IsListenKeyExpiredEvent(mes))
                            {
                                if (ListenKeyExpiredEvent != null)
                                {
                                    ListenKeyExpiredEvent(this);
                                }
                                continue;
                            }
                            else
                            {

                            }

                            //ORDER_TRADE_UPDATE
                            // "{\"e\":\"ORDER_TRADE_UPDATE\",\"T\":1579688850841,\"E\":1579688850846,\"o\":{\"s\":\"BTCUSDT\",\"c\":\"1998\",\"S\":\"BUY\",\"o\":\"LIMIT\",\"f\":\"GTC\",\"q\":\"0.001\",\"p\":\"8671.86\",\"ap\":\"0.00000\",\"sp\":\"0.00\",\"x\":\"NEW\",\"X\":\"NEW\",\"i\":760799835,\"l\":\"0.000\",\"z\":\"0.000\",\"L\":\"0.00\",\"T\":1579688850841,\"t\":0,\"b\":\"0.00000\",\"a\":\"0.00000\",\"m\":false,\"R\":false,\"wt\":\"CONTRACT_PRICE\",\"ot\":\"LIMIT\"}}"

                            //ACCOUNT_UPDATE
                            //"{\"e\":\"ACCOUNT_UPDATE\",\"T\":1579688850841,\"E\":1579688850846,\"a\":{\"B\":[{\"a\":\"USDT\",\"wb\":\"29.88018817\",\"cw\":\"29.88018817\"},{\"a\":\"BNB\",\"wb\":\"0.00000000\",\"cw\":\"0.00000000\"}],\"P\":[{\"s\":\"BTCUSDT\",\"pa\":\"0.000\",\"ep\":\"0.00000\",\"cr\":\"-0.05040000\",\"up\":\"0.00000000\",\"mt\":\"cross\",\"iw\":\"0.00000000\"},{\"s\":\"ETHUSDT\",\"pa\":\"0.000\",\"ep\":\"0.00000\",\"cr\":\"0.00000000\",\"up\":\"0.00000000\",\"mt\":\"cross\",\"iw\":\"0.00000000\"},{\"s\":\"BCHUSDT\",\"pa\":\"0.000\",\"ep\":\"0.00000\",\"cr\":\"0.00000000\",\"up\":\"0.00000000\",\"mt\":\"cross\",\"iw\":\"0.00000000\"},{\"s\":\"XRPUSDT\",\"pa\":\"0.0\",\"ep\":\"0.00000\",\"cr\":\"0.00000000\",\"up\":\"0.000000\",\"mt\":\"cross\",\"iw\":\"0.00000000\"},{\"s\":\"EOSUSDT\",\"pa\":\"0.0\",\"ep\":\"0.0000\",\"cr\":\"0.00000000\",\"up\":\"0.00000\",\"mt\":\"cross\",\"iw\":\"0.00000000\"},{\"s\":\"LTCUSDT\",\"pa\":\"0.000\",\"ep\":\"0.00000\",\"cr\":\"0.00000000\",\"up\":\"0.00000000\",\"mt\":\"cross\",\"iw\":\"0.00000000\"},{\"s\":\"TRXUSDT\",\"pa\":\"0\",\"ep\":\"0.00000\",\"cr\":\"0.00000000\",\"up\":\"0.00000\",\"mt\":\"cross\",\"iw\":\"0.00000000\"},{\"s\":\"ETCUSDT\",\"pa\":\"0.00\",\"ep\":\"0.00000\",\"cr\":\"0.00000000\",\"up\":\"0.0000000\",\"mt\":\"cross\",\"iw\":\"0.00000000\"},{\"s\":\"LINKUSDT\",\"pa\":\"0.00\",\"ep\":\"0.00000\",\"cr\":\"0.00000000\",\"up\":\"0.0000000\",\"mt\":\"cross\",\"iw\":\"0.00000000\"},{\"s\":\"XLMUSDT\",\"pa\":\"0\",\"ep\":\"0.00000\",\"cr\":\"0.00000000\",\"up\":\"0.00000\",\"mt\":\"cross\",\"iw\":\"0.00000000\"}]}}"

                            //LISTEN_KEY_EXPIRED
                            //"{\"e\": \"listenKeyExpired\", \"E\": 1653994245400}
                        }
                    }
                    else
                    {
                        if (_isDisposed)
                        {
                            return;
                        }
                        Thread.Sleep(1);
                    }
                }

                catch (Exception exception)
                {
                    //SendLogMessage(exception.ToString(), LogMessageType.Error);
                }

            }
        }

        private static bool IsListenKeyExpiredEvent(string userDataMsg)
        {
            const string EVENT_NAME_KEY = "e";
            const string LISTEN_KEY_EXPIRED_EVENT_NAME = "listenKeyExpired";
            JObject userDataMsgJSON = ParseToJson(userDataMsg);
            if (userDataMsgJSON != null && userDataMsgJSON.Property(EVENT_NAME_KEY) != null)
            {
                string eventName = userDataMsgJSON.Value<string>(EVENT_NAME_KEY);
                return String.Equals(eventName, LISTEN_KEY_EXPIRED_EVENT_NAME, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        private static JObject ParseToJson(string jsonMessage)
        {
            try
            {
                return JObject.Parse(jsonMessage);
            }
            catch
            {
                return null;
            }
        }

        private DateTime _timeStart;

        /// <summary>
        /// каждые полчаса отправляем сообщение, чтобы поток не закрылся
        /// </summary>
        private void KeepaliveUserDataStream()
        {
            while (true)
            {
                Thread.Sleep(30000);

                if (_listenKey == "")
                {
                    return;
                }

                if (_isDisposed == true)
                {
                    return;
                }

                if (_timeStart.AddMinutes(25) < DateTime.Now)
                {
                    _timeStart = DateTime.Now;

                    CreateQueryNoLock(Method.Put,
                        "/" + type_str_selector + "/v1/listenKey", new Dictionary<string, string>()
                            { { "listenKey=", _listenKey } }, false);

                }
            }
        }

        /// <summary>
        /// очередь новых сообщений, пришедших с сервера биржи
        /// </summary>
        private ConcurrentQueue<BinanceUserMessage> _newUserDataMessage = new ConcurrentQueue<BinanceUserMessage>();

        /// <summary>
        /// обработчик пользовательских данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserDataMessageHandler(object sender, MessageReceivedEventArgs e)
        {
            if (_isDisposed)
            {
                return;
            }
            BinanceUserMessage message = new BinanceUserMessage();
            message.MessageStr = e.Message;
            _newUserDataMessage.Enqueue(message);
        }

        /// <summary>
        /// соединение по ws открыто
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect(object sender, EventArgs e)
        {
            IsConnected = true;
        }



        /// <summary>
        /// ws-connection is closed
        /// соединение по ws закрыто
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Disconnect(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                IsConnected = false;

                foreach (var ws in _wsStreams)
                {
                    ws.Value.Opened -= new EventHandler(Connect);
                    ws.Value.Closed -= new EventHandler(Disconnect);
                    ws.Value.Error -= new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(WsError);
                    ws.Value.MessageReceived -= new EventHandler<MessageReceivedEventArgs>(GetRes);

                    ws.Value.Close();
                    ws.Value.Dispose();
                }

                _wsStreams.Clear();

                if (Disconnected != null)
                {
                    Disconnected();
                }
            }
        }

        /// <summary>
        /// очередь новых сообщений, пришедших с сервера биржи
        /// </summary>
        private ConcurrentQueue<string> _newMessage = new ConcurrentQueue<string>();

        /// <summary>
        /// data stream collection
        /// коллекция потоков данных
        /// </summary>
        private Dictionary<string, WebSocket> _wsStreams = new Dictionary<string, WebSocket>();

        /// <summary>
        /// берет пришедшие через ws сообщения и кладет их в общую очередь
        /// </summary>        
        private void GetRes(object sender, MessageReceivedEventArgs e)
        {
            if (_isDisposed == true)
            {
                return;
            }
            _newMessage.Enqueue(e.Message);
        }

        /// <summary>
        /// произошёл запрос на очистку объекта
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// ошибка из ws4net
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WsError(object sender, EventArgs e)
        {
            //SendLogMessage("Ошибка из ws4net :" + e.ToString(), LogMessageType.Error);
        }

        /// <summary>
        /// запрвшиваем новый listenKey для веб сокета от Binance через отправку делаем HTTP запроса
        /// </summary>
        public void RenewListenKey()
        {
            try
            {
                _listenKey = CreateListenKey();
            }
            catch (Exception exception)
            {
                //SendLogMessage(exception.Message, LogMessageType.Connect);
            }
        }

        /// <summary>
        /// делаем HTTP запрос на Binance чтобы создать и получить listenKey для веб сокета
        /// </summary>
        private string CreateListenKey()
        {
            string createListenKeyUrl = string.Format("/{0}/v1/listenKey", type_str_selector);
            var createListenKeyResult = CreateQueryNoLock(Method.Post, createListenKeyUrl, null, false);
            return JsonConvert.DeserializeAnonymousType(createListenKeyResult, new ListenKey()).listenKey;
        }


        #region Аутунтификация запроса

        /// <summary>
        /// метод отправляет запрос и возвращает ответ от сервера
        /// в этом методе отсутствует локирование чтобы дать возможность 
        /// в некоторых важных случаях возможность сделать незамедлительный запрос на сервер
        /// </summary>
        public string CreateQueryNoLock(Method method, string endpoint, Dictionary<string, string> param = null, bool auth = false)
        {
            try
            {
                return PerformHttpRequest(method, endpoint, param, auth);
            }
            catch (Exception ex)
            {
                return HandleHttpRequestException(ex);
            }
        }

        /// <summary>
        /// блокиратор многопоточного доступа к http запросам
        /// </summary>
        private object _queryHttpLocker = new object();

        RateGate _rateGate = new RateGate(1, TimeSpan.FromMilliseconds(100));

        /// <summary>
        /// метод отправляет запрос и возвращает ответ от сервера
        /// отправка запросов контроллируется локером чтобы избежать
        /// отправки чрезмерного количества запросов на сервер в случае многих потоков
        /// </summary>
        public string CreateQuery(Method method, string endpoint, Dictionary<string, string> param = null, bool auth = false)
        {
            try
            {
                lock (_queryHttpLocker)
                {
                    _rateGate.WaitToProceed();
                    return PerformHttpRequest(method, endpoint, param, auth);
                }
            }
            catch (Exception ex)
            {
                return HandleHttpRequestException(ex);
            }
        }

        private string PerformHttpRequest(Method method, string endpoint, Dictionary<string, string> param = null, bool auth = false)
        {
            string fullUrl = "";

            if (param != null)
            {
                fullUrl += "?";

                foreach (var onePar in param)
                {
                    fullUrl += onePar.Key + onePar.Value;
                }
            }

            if (auth)
            {
                string message = "";

                string timeStamp = GetNonce();

                message += "timeStamp=" + timeStamp;

                if (fullUrl == "")
                {
                    fullUrl = "?timeStamp=" + timeStamp + "&signature=" + CreateSignature(message);
                }
                else
                {
                    message = fullUrl + "&timeStamp=" + timeStamp;
                    fullUrl += "&timeStamp=" + timeStamp + "&signature=" + CreateSignature(message.Trim('?'));
                }
            }

            var request = new RestRequest(endpoint + fullUrl, method);
            request.AddHeader("X-MBX-APIKEY", ApiKey);

            string baseUrl = _baseUrl;

            var response = new RestClient(baseUrl).Execute(request).Content;

            if (response.StartsWith("<!DOCTYPE"))
            {
                throw new Exception(response);
            }
            else if (response.Contains("code") && !response.Contains("code\": 200"))
            {
                var error = JsonConvert.DeserializeAnonymousType(response, new ErrorMessage());
                throw new Exception(error.msg);
            }

            return response;
        }

        private string GetNonce()
        {
            var restime = CreateQuery(Method.Get, "/" + type_str_selector + "/v1/time", null, false);

            if (!string.IsNullOrEmpty(restime))
            {
                var result = JsonConvert.DeserializeAnonymousType(restime, new BinanceTime());
                return (result.serverTime + 500).ToString();
            }
            else
            {
                DateTime yearBegin = new DateTime(1970, 1, 1);
                var timeStamp = DateTime.UtcNow - yearBegin;
                var r = timeStamp.TotalMilliseconds;
                var re = Convert.ToInt64(r);

                return re.ToString();
            }
        }

        private string CreateSignature(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var keyBytes = Encoding.UTF8.GetBytes(ApiSecret);
            var hash = new HMACSHA256(keyBytes);
            var computedHash = hash.ComputeHash(messageBytes);
            return BitConverter.ToString(computedHash).Replace("-", "").ToLower();
        }

        private byte[] Hmacsha256(byte[] keyByte, byte[] messageBytes)
        {
            using (var hash = new HMACSHA256(keyByte))
            {
                return hash.ComputeHash(messageBytes);
            }
        }

        private string HandleHttpRequestException(Exception ex)
        {
            if (ex.ToString().Contains("This listenKey does not exist"))
            {
                RenewListenKey();
                return null;
            }
            if (ex.ToString().Contains("Unknown order sent"))
            {
                //SendLogMessage(ex.ToString(), LogMessageType.System);
                return null;
            }

            //SendLogMessage(ex.ToString(), LogMessageType.Error);
            return null;
        }
        #endregion

        #region outgoing events / исходящие события

        /// <summary>
        /// новые мои ордера
        /// </summary>
        public event Action<Order> MyOrderEvent;

        /// <summary>
        /// новые мои сделки
        /// </summary>
        public event Action<MyTrade> MyTradeEvent;

        /// <summary>
        /// срок действия listen key, необходимого для жизни сокет коннекшена, истек
        /// </summary>
        public event Action<BinanceClientFutures> ListenKeyExpiredEvent;

        /// <summary>
        /// обновились портфели
        /// </summary>
        public event Action<AccountResponseFuturesFromWebSocket> UpdatePortfolio;

        /// <summary>
        /// обновился стакан
        /// </summary>
        public event Action<DepthResponseFutures> UpdateMarketDepth;

        /// <summary>
        /// обновились тики
        /// </summary>
        public event Action<TradeResponse> NewTradesEvent;

        /// <summary>
        /// соединение с API установлено
        /// </summary>
        public event Action Connected;

        /// <summary>
        /// соединение с API разорвано
        /// </summary>
        public event Action Disconnected;

        #endregion

        public class BinanceUserMessage
        {
            public string MessageStr;
        }
    }
}
