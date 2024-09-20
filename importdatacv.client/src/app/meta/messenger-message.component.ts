import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http'; // Ensure this import is present
import { Observable } from 'rxjs';
import { MessengerMessage } from './messenger-message.model';

@Component({
  selector: 'messenger-message-component',
  templateUrl: './messenger-message.component.html',
  styleUrls: ['./messenger-message.component.css']
})
export class MessengerMessageComponent implements OnInit {
  public oauthCode: string | null = null;
  public accessToken: string | null = null;
  public pageToken: string | null = null;
  private pageId: string = '157466200794611';
  private clientIdApp: string = '2957941441176549'; // Facebook App ID
  private clientIdPage: string = '130700533182452';
  private redirectUri: string = 'https://127.0.0.1:4200/'; // Đường dẫn hứng OAuth code
  private scope: string = 'user_likes,user_posts,read_insights,pages_show_list,read_page_mailboxes,ads_management,business_management,pages_messaging,pages_read_engagement,pages_manage_metadata,pages_read_user_content,pages_manage_posts,public_profile';
  private oauthUrl: string = `https://www.facebook.com/v10.0/dialog/oauth?client_id=${this.clientIdApp}&redirect_uri=${this.redirectUri}&scope=${this.scope}&response_type=code`;
  private appSecret: string = 'd41338cca30750d570300f458d8fa22e'

  private apiSaveToken: string = 'https://localhost:7131/api/meta/saveTokens';
  private apiSaveMessages: string = 'https://localhost:7131/api/meta/saveMessages';
  messengerMessages: MessengerMessage[] = [];

  public conversations: any[] = [];
  public messages: any[] = [];
  public inputConversationId: string = "";

  constructor(private http: HttpClient) { }

  ngOnInit() {
    const urlParams = new URLSearchParams(window.location.search);
    this.oauthCode = urlParams.get('code');

    if (this.oauthCode) {
      this.genTokenUser();
    }
  }

  redirectToFacebookOAuth() {
    window.location.href = this.oauthUrl; // Chuyển hướng người dùng đến Facebook OAuth URL
  }

  genTokenUser() {
    if (this.oauthCode) {
      const tokenUrl = `https://graph.facebook.com/v20.0/oauth/access_token?client_id=${this.clientIdApp}&redirect_uri=${this.redirectUri}&client_secret=${this.appSecret}&code=${this.oauthCode}`;

      this.http.get<any>(tokenUrl).subscribe(response => {
        this.accessToken = response.access_token;
        console.log('Short-term Access Token:', this.accessToken);

        // Call to get the long-term token after getting the short-term token
        this.genLongTermToken(this.accessToken!);

        // Call genTokenPage after fetching access token
        this.genTokenPage();
      }, error => {
        console.error('Error fetching access token:', error);
      });
    }
  }

  genLongTermToken(shortTermToken: string) {
    const longTermTokenUrl = `https://graph.facebook.com/v20.0/oauth/access_token?grant_type=fb_exchange_token&client_id=${this.clientIdApp}&client_secret=${this.appSecret}&fb_exchange_token=${shortTermToken}`;

    this.http.get<any>(longTermTokenUrl).subscribe(response => {
      const longTermAccessToken = response.access_token;
      console.log('Long-term Access Token:', longTermAccessToken);

      // You can now store the long-term access token or use it
      this.accessToken = longTermAccessToken;

    }, error => {
      console.error('Error fetching long-term token:', error);
    });
  }

  genTokenPage() {
    if (this.accessToken) {
      const tokenUrl = `https://graph.facebook.com/v20.0/${this.clientIdPage}/accounts?access_token=${this.accessToken}`;

      this.http.get<any>(tokenUrl).subscribe(response => {
        if (response && response.data && response.data.length > 0) {

          const firstPage = response.data[0];
          this.pageToken = firstPage.access_token;
          console.log('Page Access Token:', this.pageToken);

        } else {
          console.error('No pages found or no page access tokens available');
        }
      }, error => {
        console.error('Error fetching page access token:', error);
      });
    }
  }

  saveTokensToDb() {
    if (!this.accessToken || !this.pageToken) {
      console.error('Access Token or Page Token is null or undefined. Cannot save tokens.');
      return;
    }

    const payload = { accessToken: this.accessToken, pageToken: this.pageToken };
    this.http.post(this.apiSaveToken, payload).subscribe(
      response => {
        console.log('Tokens saved:', response);
      },
      error => {
        console.error('Error saving tokens:', error);
      }
    );
  }

  //Messenger Component
  getMessengerList(): Observable<any> {
    const url = `${this.redirectUri}${this.pageId}/conversations?platform=MESSENGER&access_token=${this.accessToken}`;
    return this.http.get<any>(url);
  }

  getMessagesByConversation(conversationId: string): Observable<any> {
    const url = `${this.redirectUri}${conversationId}?fields=messages,id&access_token=${this.accessToken}`;
    return this.http.get<any>(url);
  }

  getMessageDetails(messageId: string): Observable<any> {
    const url = `${this.redirectUri}{messageId}?fields=id,created_time,from,to,message&access_token=${this.accessToken}`;
    return this.http.get<any>(url);
  }

  onSaveMessages(): void {
    // Tạo đối tượng yêu cầu với danh sách tin nhắn
    const request = { Messages: this.messengerMessages };

    // Gọi API để lưu tin nhắn
    this.saveMessagesToApi(request).subscribe({
      next: (response) => {
        console.log('Messages saved successfully:', response);
        // Xử lý phản hồi thành công nếu cần
      },
      error: (error) => {
        console.error('Error saving messages:', error);
        // Xử lý lỗi nếu cần
      }
    });
  }

  saveMessagesToApi(request: { Messages: MessengerMessage[] }): Observable<any> {
    return this.http.post<any>(this.apiSaveMessages, request);
  }

  async processAndSaveMessages() {
    try {
      // Lấy danh sách cuộc trò chuyện
      const conversationList = await this.getMessengerList().toPromise();

      // Tạo danh sách tin nhắn để gửi
      const messagesToSave: MessengerMessage[] = [];

      for (const conversation of conversationList.data) {
        // Lấy tin nhắn của từng cuộc trò chuyện
        const messages = await this.getMessagesByConversation(conversation.id).toPromise();

        for (const message of messages.messages.data) {
          // Lấy chi tiết tin nhắn
          const messageDetails = await this.getMessageDetails(message.id).toPromise();

          // Tạo đối tượng MessengerMessage
          const messengerMessage: MessengerMessage = {
            ConversationId: conversation.id,
            UpdatedTime: new Date(conversation.updated_time),
            MessageId: messageDetails.id,
            MessageCreatedTime: new Date(messageDetails.created_time),
            FromId: messageDetails.from.id,
            FromName: messageDetails.from.name,
            FromEmail: messageDetails.from.email,
            ToId: messageDetails.to.data[0].id,
            ToName: messageDetails.to.data[0].name,
            ToEmail: messageDetails.to.data[0].email,
            MessageText: messageDetails.message
          };

          // Thêm tin nhắn vào danh sách để lưu
          messagesToSave.push(messengerMessage);
        }
      }

      // Gửi danh sách tin nhắn để lưu vào API
      await this.saveMessagesToApi({ Messages: messagesToSave }).toPromise();
    } catch (error) {
      console.error('Error processing and saving messages', error);
    }
  }


  getConversations() {
    const url = "https://localhost:7131/api/meta/conversations";
    this.http.get<any>(url).subscribe(
      response => {
        this.conversations = response.data; 
        console.log("Danh sách tin nhắn:", response.data);
        // Xử lý dữ liệu tin nhắn ở đây, có thể hiển thị trên giao diện
      },
      error => {
        console.error("Lỗi khi lấy danh sách tin nhắn:", error);
      }
    );
  }

  getConversationId() {
    // Kiểm tra inputConversationId có phải là chuỗi số không
    console.log(this.inputConversationId)
    if (!this.inputConversationId) {
      console.error("Conversation ID phải là một chuỗi số hợp lệ.");
      return;
    }
    const url = `https://localhost:7131/api/meta/conversationId?conversationId=${this.inputConversationId}`;
    this.http.get<any>(url).subscribe(
      response => {
        this.messages = response.Messages.Data;
      },
      error => {
        console.error("Lỗi khi lấy danh sách tin nhắn:", error);
      }
    );
  }

  saveMessages() {
    // Kiểm tra inputConversationId có phải là chuỗi số không
    if (!this.inputConversationId) {
      console.error("Conversation ID phải là một chuỗi số hợp lệ.");
      return;
    }
    const url = `https://localhost:7131/api/meta/conversationId?conversationId=${this.inputConversationId}`;
    this.http.get<any>(url).subscribe(
      response => {
        this.messages = response.Messages.Data;
      },
      error => {
        console.error("Lỗi khi lấy danh sách tin nhắn:", error);
      }
    );
  }
}


