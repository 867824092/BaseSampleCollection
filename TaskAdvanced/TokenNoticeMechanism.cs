using Microsoft.Extensions.Primitives;

namespace TaskAdvanced; 

public class TokenNoticeMechanism {

    public static void Run() {
        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        cancellationToken.Token.Register(() => {
            Console.WriteLine("cancellationToken 回调");
        });
        int count = 1;
        while(count < 2) {
            count++;
            Thread.Sleep(100);
        }
        cancellationToken.Cancel();
//var tokenSource = new CancellationTokenSource();

//ChangeToken.OnChange(() => new CancellationChangeToken(tokenSource.Token), () => {
//    Console.WriteLine("解决CancellationChangeToken注册的回调,注册一次只能被触发一次的问题,但是当前用法会递归");
//});

//tokenSource.Cancel();

//声明类的实例
        TestCancellationChangeToken cancellationChangeToken = new TestCancellationChangeToken();
        ChangeToken.OnChange(() => cancellationChangeToken.CreatChanageToken(), () => {
            System.Console.WriteLine($"{DateTime.Now:HH:mm:ss}被触发可一次");
        });
//cancellationChangeToken.CancelToken();
//模拟多次调用CancelToken
        for (int i = 0; i < 3; i++) {
            Thread.Sleep(1000);
            cancellationChangeToken.CancelToken();
        }
    }
    
    public class TestCancellationChangeToken {
        private CancellationTokenSource tokenSource;// = new CancellationTokenSource();

        /// <summary>
        /// 获取CancellationChangeToken实例方法
        /// </summary>
        public CancellationChangeToken CreatChanageToken() {
       
            tokenSource = new CancellationTokenSource();
            Console.WriteLine($"创建了CancellationTokenSource:{tokenSource.GetHashCode()}");
            return new CancellationChangeToken(tokenSource.Token);
        }

        /// <summary>
        /// 取消CancellationTokenSource
        /// </summary>
        public void CancelToken() {
            tokenSource.Cancel();
        }
    }
}