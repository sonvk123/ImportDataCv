using TL;
using WTelegram;
using System.Collections.Generic;

namespace  ImportDataCv.Server
{
public class ApiTeleService
{

    private readonly WTelegram.Client _client;

    public ApiTeleService()
    {
            _client = new WTelegram.Client(22086187, "98fa69f08c20f6a3f12eebe132f7dfe7"); // API ID and Hash
    }


        public async Task<string> DoLogin(string loginInfo)
        {
            while (_client.User == null)
            {
                var config = await _client.Login(loginInfo);

                switch (config)
                {
                    case "verification_code":
                        Console.Write("Code: ");
                        loginInfo = Console.ReadLine();
                        break;
                        default:
                        loginInfo = null;
                        break;
                }
            }

            return $"We are logged-in as {_client.User} (id {_client.User.id})";
        }

        public WTelegram.Client GetClient()
        {
            return _client;
        }
        public async Task<List<object>> MessagesGetAllDialogs()
        {
            WTelegram.Client client = GetClient();
            var dialogs = await client.Messages_GetAllDialogs();
            var result = new List<object>(); // Sử dụng object để có thể chứa cả User và Chat

            foreach (var dialog in dialogs.dialogs)
            {
                var peer = dialogs.UserOrChat(dialog); // Lấy thông tin người dùng hoặc nhóm
                result.Add(peer); // Thêm vào danh sách
            }

            return result;
        }

    }


}