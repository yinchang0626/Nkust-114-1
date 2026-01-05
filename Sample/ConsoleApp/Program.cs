using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // 設定控制台輸出編碼為 UTF-8

            Console.WriteLine("食品營養成分資訊 - JSON 反序列化並寫入資料庫");



            Console.WriteLine("\n按任意鍵結束...");
            Console.ReadKey();
        }
    }
}
