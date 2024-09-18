import { Component, OnInit } from '@angular/core';
import { ApiService } from '../api.service';

@Component({
  selector: 'app-data-display',
  templateUrl: './data-display.component.html',
  styleUrls: ['./data-display.component.css']
})
export class DataDisplayComponent implements OnInit {
  inputCode: string = '';
  inputUserId: string = '';
  data: any;
  dataMessages: any[] = []; // Đảm bảo đây là mảng
  dataMessage: any;
  code: string | null = null;
  errorMessage: string = ''; // Thêm biến để lưu thông báo lỗi

  constructor(private apiService: ApiService) { }

  ngOnInit(): void {
    const urlParams = new URLSearchParams(window.location.search);
    this.code = urlParams.get('code');  // Lấy giá trị của 'code' từ URL

    if (this.code) {
      this.inputCode = this.code
    }
  }

  getCode(): void {
    const url = `https://oauth.zaloapp.com/v4/oa/permission?app_id=3149800129752642817&redirect_uri=https%3A%2F%2Fsonvk123.github.io%2FNews_App%2F`;
    window.open(url, '_blank');
  }

  fetchData(): void {
    const url = `https://localhost:7131/api/SendMessage/AccessToken?code=${this.inputCode}`;
    this.apiService.getAccessToken(url).subscribe(

      response => {
        this.data = response.AccessToken;
      },
      error => {
        console.error('Error fetching data', error);
      }
    );
  }

  fetchDataMessages(): void {
    const url = `https://localhost:7131/api/SendMessage/getMessages`;
    this.apiService.getMessages(url).subscribe(
      response => {
        if (response) {
          console.log(response)
          this.dataMessages = response.data;
          console.log(response.data)

        } else {
          this.errorMessage = 'No found in the response'; // Xử lý nếu không có 
        }
      },
      error => {
        console.error('Error fetching data', error);
        this.errorMessage = 'Error fetching data'; // Thông báo lỗi cho người dùng
      }
    );
  }

  fetchDataMessageUserId(): void {
    const url = `https://localhost:7131/api/SendMessage/messageUserId?userId=${this.inputUserId}`;
    this.apiService.getMessageUserId(url).subscribe(
      response => {
        if (response) {
          console.log(response)
          this.dataMessage = response.res;
          console.log(this.dataMessage)
        } else {
          this.errorMessage = 'No found in the response'; // Xử lý nếu không có 
        }
      },
      error => {
        console.error('Error fetching data', error);
        this.errorMessage = 'Error fetching data'; // Thông báo lỗi cho người dùng
      }
    );
  }
}
