// See https://aka.ms/new-console-template for more information
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using Newtonsoft.Json;

BinanceClient? _client;
string ApiKey = "a4c675ddfa8005fdabf5580700bd87b2d0dff9108b1caa8295f5540e6cf118e5";
string SecretKey = "211c4565fb98ad121a10ce2cce9c31456890786cbce501ad426b0bbace6e1102";

Console.WriteLine("Start!");

BinanceClientOptions clientOption = new();
clientOption.UsdFuturesApiOptions.BaseAddress = "https://testnet.binancefuture.com";
_client = new(clientOption);
_client.SetApiCredentials(new ApiCredentials(ApiKey, SecretKey));



CancelAllOrdersAsync();
GetPositionInformationAsync();

Console.Read();

//while (true)
//{
//    Console.WriteLine("Enter");
//    Console.Read();
//    //CancelAllOrdersAsync();
//    //GetOpenOrdersAsync();
//    GetPositionInformationAsync();
//}

async void CancelAllOrdersAsync()
{
    var result = await _client.UsdFuturesApi.Trading.CancelAllOrdersAsync(symbol: "BTCUSDT");
    if (!result.Success)
    {
        Console.WriteLine($"Failed CancelAllOrdersAsync: {result.Error?.Message}");
    }
    else
    {
        Console.WriteLine("CancelAllOrdersAsync");
    }
}

async void GetOpenOrdersAsync()
{
    var result = await _client.UsdFuturesApi.Trading.GetOpenOrdersAsync("BTCUSDT");
    if (!result.Success)
    {
        Console.WriteLine($"Failed GetOpenOrdersAsync: {result.Error?.Message}");
    }
    else
    {
        Console.WriteLine("GetOpenOrdersAsync:");
        Console.WriteLine(JsonConvert.SerializeObject(result.Data.ToList()));

        //foreach (var item in result.Data.ToList())
        //{
        //    Console.WriteLine(JsonConvert.SerializeObject(item));
        //}
    }
}

async void GetPositionInformationAsync()
{
    var result = await _client.UsdFuturesApi.Account.GetPositionInformationAsync("BTCUSDT");
    if (!result.Success)
    {
        Console.WriteLine($"Failed GetPositionInformationAsync: {result.Error?.Message}");
    }
    else
    {
        Console.WriteLine("GetPositionInformationAsync:");
        decimal quantity = result.Data.ToList()[0].Quantity;
        if(quantity != 0m)
        {
            if(quantity > 0m)
            {
                OpenOrderMarketAsync(OrderSide.Sell, quantity);
            }
            else
            {
                OpenOrderMarketAsync(OrderSide.Buy, -quantity);
            }
        }
    }
}

async void OpenOrderMarketAsync(OrderSide side, decimal quantity)
{
    try
    {
        await Task.Run(async () =>
        {
            var result = await _client.UsdFuturesApi.Trading.PlaceOrderAsync(
                symbol: "BTCUSDT",
                side: side,
                type: FuturesOrderType.Market,
                quantity: quantity,
                positionSide: PositionSide.Both);
            if (!result.Success)
            {
                Console.WriteLine($"Failed OpenOrderMarketAsync: {result.Error?.Message}");
            }
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception OpenOrderMarketAsync: {ex.Message}");
    }
}