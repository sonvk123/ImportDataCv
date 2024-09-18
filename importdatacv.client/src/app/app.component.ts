import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { NgxPaginationModule } from 'ngx-pagination'; // Import module phân trang
import * as XLSX from 'xlsx';
interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

interface Contact {
  ContactName: string | null;
  MobilePhone: string | null;
  Email: string | null;
}
interface ColumnMapping {
  fieldName: string;
  columnIndex: number;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  public forecasts: WeatherForecast[] = [];
  public dataImportCV: Contact[] = [];
  public fileName: string = '';
  public p: number = 1; // Trang hiện tại
  public totalItems: number = 0; // Tổng số bản ghi
  public totalPages: number = 0; // Tổng số trang
  public itemsPerPage: number = 1000; // Số bản ghi mỗi trang
  public columnMappings: ColumnMapping[] = []; // Khai báo thuộc tính columnMapping
  constructor(private http: HttpClient) {}

  ngOnInit() {
  }

  onFileChange(event: any) {
    const file = event.target.files[0];
    this.fileName = file.name;
    const reader = new FileReader();

    reader.onload = (e: any) => {
      const data = e.target.result;
      const workbook = XLSX.read(data, { type: 'binary' });
      const sheetName = workbook.SheetNames[0];
      const worksheet = workbook.Sheets[sheetName];
      const json = XLSX.utils.sheet_to_json<any>(worksheet, { header: 1 });

      const headerRow = json[0];
      const columnIndices: { [key: string]: number } = {};
      headerRow.forEach((columnName: string, index: number) => {
        switch (columnName) {
          case 'Họ Tên': columnIndices['CONTACT_NAME'] = index; break;
          case 'Số điện thoại': columnIndices['MOBILE_PHONE'] = index; break;
          case 'Email': columnIndices['EMAIL'] = index; break;
        }
      });

      this.dataImportCV = json.slice(1).map((row: any[]) => ({
        ContactName: row[columnIndices['CONTACT_NAME']] || null,
        MobilePhone: row[columnIndices['MOBILE_PHONE']] ? row[columnIndices['MOBILE_PHONE']].toString() : null, // Chuyển đổi số thành chuỗi
        Email: row[columnIndices['EMAIL']] ? String(row[columnIndices['EMAIL']]) : null, // Đảm bảo kiểu string
   

      }));

      this.totalItems = this.dataImportCV.length; // Cập nhật tổng số bản ghi
      this.totalPages = Math.ceil(this.totalItems / this.itemsPerPage); // Cập nhật số trang
    };

    reader.readAsBinaryString(file);

  }

  saveToDatabase() {
    this.http.post('https://localhost:7131/api/SmartWord/save', this.dataImportCV).subscribe(
      (response) => {
        console.log('Dữ liệu đã được lưu thành công:', response);
      },
      (error) => {
        console.error('Có lỗi xảy ra khi lưu dữ liệu', error);
      }
    );
  }

  email = {
    toEmail: '',
    subject: '',
    message: ''
  };


  sendEmail() {
    if (!this.email.toEmail || !this.email.subject || !this.email.message) {
      console.error('All fields are required');
      return;
    }

    this.http.post('https://localhost:7131/api/SendEmail/SendEmail', this.email)
      .subscribe(
        (response) => {
          console.log('Email sent successfully:', response);
        },
        (error) => {
          console.error('Error sending email:', error);
        }
      );
  }
}
