import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  constructor(private http: HttpClient) { }

  getAccessToken(url: string): Observable<any> {
    return this.http.get(url);
  }

  getUserLists(url: string): Observable<any> {
    return this.http.get(url);
  }

  getMessages(url: string): Observable<any> {
    return this.http.get(url);
  }

  getMessageUserId(url: string): Observable<any> {
    return this.http.get(url);
  }
}
